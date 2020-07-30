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

        public MessageConfirm(int msgId)
            :base()
        {
            _msgId = msgId;
        }
        #endregion

        #region Consts
        #endregion

        #region Fields
        private int _msgId;
        #endregion

        #region Properties
        public int MsgId => _msgId;
        #endregion

        #region Public Methods
        public override bool SetBytes(byte[] bytes)
        {
            _msgId = Convert.ToInt16(bytes[0]);
            return true;
        }
        #endregion
    }
}