using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxMix.Services.Communication
{
    internal class MessageHandShakeResponse : MessageBase
    {
        #region Constructor
        public MessageHandShakeResponse() :base() { }
        #endregion

        #region Consts
        #endregion

        #region Fields
        #endregion

        #region Properties
        #endregion

        #region Public Methods
        public override bool SetBytes(byte[] bytes)
        {
            return true;
        }
        #endregion
    }
}