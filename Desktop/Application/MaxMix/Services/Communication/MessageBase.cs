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
            MessageId = new Random().Next(255);
        }
        #endregion

        #region Consts
        #endregion

        #region Fields
        #endregion

        #region Properties
        public int MessageId
        {
            get;
            protected set;
        }

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