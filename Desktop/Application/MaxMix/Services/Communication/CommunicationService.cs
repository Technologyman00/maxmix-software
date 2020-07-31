using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MaxMix.Services.Communication
{
    /// <summary>
    /// Manages sending and receiving messages between application and device.
    /// It is protocol agnostic, it uses the provided message ISerializationService.
    /// </summary>
    internal class CommunicationService : ICommunicationService
    {
        #region Constructor
        public CommunicationService(ISerializationService serializationService)
        {
            _serializationService = serializationService;
            _synchronizationContext = SynchronizationContext.Current;
            
            _sendQueue = new ConcurrentQueue<IMessage>();
            _receiveBuffer = new List<byte>();
        }
        #endregion

        #region Consts
        private const int _checkPortInterval = 500;
        private const int _sendConfirmInterval = 10;
        private const int _timeout = 1000;
        private const int _sendQueueMax = 100;
        private const int _sendRetryMax = 3;
        #endregion

        #region Fields
        private readonly ISerializationService _serializationService;
        private readonly ConcurrentQueue<IMessage> _sendQueue;
        private readonly IList<byte> _receiveBuffer;
        private readonly object _lastSentLock = new object();


        private string _portName;
        private int _baudRate;
        private SerialPort _serialPort;
        
        private IMessage _lastSent;
        private Thread _sendThread;
        private bool _isSend;

        private Thread _portStateThread;
        private bool _isCheckPortState;
        private SynchronizationContext _synchronizationContext;
        #endregion
         
        #region Properties
        #endregion

        #region Events
        /// <summary>
        /// Raised when a message has been received and deserialized.
        /// </summary>
        public event EventHandler<IMessage> MessageReceived;

        /// <summary>
        /// Raised when an error has happend.
        /// </summary>
        public event EventHandler<string> Error;
        #endregion

        #region Public Methods
        /// <summary>
        /// Establishes a connection and begins the communication process.
        /// </summary>
        /// <param name="portName">Name of the COM port to connect to.</param>
        /// <param name="baudRate">Baudrate of the connection.</param>
        public void Start(string portName, int baudRate = 115200)
        {
            _portName = portName;
            _baudRate = baudRate;

            Connect(_portName, _baudRate);

            _isCheckPortState = true;
            _portStateThread = new Thread(() => CheckPortState());
            _portStateThread.Start();

            _isSend = true;
            _sendThread = new Thread(() => Send());
            _sendThread.Start();
        }

        /// <summary>
        /// Properly ends the connection.
        /// </summary>
        public void Stop()
        {
            _isCheckPortState = false;
            _isSend = false;

            _portStateThread?.Join();
            _sendThread?.Join();

            Disconnect();
        }

        /// <summary>
        /// Sends the message using the ISerializationService provided.
        /// </summary>
        /// <param name="message">The message object to send.</param>
        public void Send(IMessage message)
        {
            if(_sendQueue.Count < _sendQueueMax)
                _sendQueue.Enqueue(message);
            else
                RaiseError("Send queue full.");
        }
        #endregion

        #region Private Methods
        private void CheckPortState()
        { 
            while (_isCheckPortState)
            {
                if (!_serialPort.IsOpen)
                {
                    RaiseError("Port closed.");
                    return;
                }

                Thread.Sleep(_checkPortInterval);
            }
        }

        private void Send()
        {
            while (_isSend)
            {
                if (_lastSent != null)
                {
                    Thread.Sleep(_sendConfirmInterval);
                    continue;
                }

                lock (_lastSentLock)
                {
                    _sendQueue.TryDequeue(out _lastSent);
                    if (_lastSent == null)
                        continue;
                }

                try
                {
                    byte[] bytes;
                    lock(_lastSentLock)
                        bytes = _serializationService.Serialize(_lastSent);
                    _serialPort.Write(bytes, 0, bytes.Length);
                }
                catch (Exception e)
                {
                    RaiseError(e.Message);
                }
            }
        }

        private void Receive()
        {
            while (_serialPort.BytesToRead > 0)
            {
                byte received = (byte)_serialPort.ReadByte();
                _receiveBuffer.Add(received);

                if (received == _serializationService.Delimiter)
                {
                    var message = _serializationService.Deserialize(_receiveBuffer.ToArray());
                    if (message != null)
                    {
                        if (message.MessageId == _lastSent.MessageId)
                            lock(_lastSentLock)
                                _lastSent = null;

                        RaiseMessageReceived(message);
                    }
                    else
                        RaiseError("Deserialization error.");

                    _receiveBuffer.Clear();
                }
            }
        }

        private void Connect(string portName, int baudRate)
        {
            try
            {
                _serialPort = new SerialPort(portName, baudRate);
                _serialPort.ReadTimeout = _timeout;
                _serialPort.WriteTimeout = _timeout;

                _serialPort.Open();
                _serialPort.DataReceived += OnDataReceived;
            }
            catch { RaiseError($"Can't connect to port {portName}"); }
        }

        private void Disconnect()
        {
            try
            {
                _serialPort.DataReceived -= OnDataReceived;
                _serialPort.Close();
                _serialPort.Dispose();
            }
            catch { }
        }
        #endregion

        #region EventHandlers
        private void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            Receive();
        }
        #endregion

        #region Event Dispatchers
        private void RaiseMessageReceived(IMessage message)
        {
            MessageReceived?.Invoke(this, message);
        }

        private void RaiseError(string error)
        {
            if(_synchronizationContext != SynchronizationContext.Current)
                _synchronizationContext.Post(o => Error?.Invoke(this, error), null);
            else
                Error?.Invoke(this, error);
        }
        #endregion
    }
}
