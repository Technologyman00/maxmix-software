using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxMix.Services.Communication
{
    internal class MessageConfirm : MessageBase
    {
        #region Constructor
        public MessageConfirm() : base() { }
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
            MessageId = Convert.ToInt16(bytes[0]);
            return true;
        }
        #endregion
    }
}