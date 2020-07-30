using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxMix.Services.Communication
{
    internal class MessageBase : IMessage
    {
        #region Constructor
        public MessageBase()
        {
            _msgId = new Random().Next();
        }
        #endregion

        #region Consts
        #endregion

        #region Fields
        private int _msgId;
        #endregion

        #region Properties
        public int MessageId => _msgId;

        public virtual byte[] GetBytes()
        {
            throw new NotImplementedException();
        }

        public virtual bool SetBytes(byte[] bytes)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}