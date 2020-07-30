using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxMix.Services.Communication
{
    internal class MessageRemoveSession : MessageBase
    {
        #region Constructor
        public MessageRemoveSession() : base() { }
        public MessageRemoveSession(int id)
            : base()
        {
            _id = id;
        }
        #endregion

        #region Consts
        #endregion

        #region Fields
        private int _id;
        #endregion

        #region Properties
        public int Id { get => _id; }
        #endregion

        #region Private Methods
        #endregion

        #region Public Methods
        /*
        * ---------------------------------------
        * CHUNK        TYPE        SIZE (BYTES)
        * ---------------------------------------
        * ID           INT32       4
        * ---------------------------------------
        */

        public override byte[] GetBytes()
        {
            var result = new List<byte>();
            result.Add(Convert.ToByte(MessageId));
            result.Add(Convert.ToByte(Id));
            return result.ToArray();
        }
        #endregion
    }
}