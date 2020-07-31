using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxMix.Services.Communication
{
    internal class MessageHandShakeRequest : MessageBase
    {
        #region Constructor
        public MessageHandShakeRequest() : base(){ }
        #endregion

        #region Consts
        #endregion

        #region Fields
        #endregion

        #region Properties
        #endregion

        #region Public Methods
        public override byte[] GetBytes()
        {
            var result = new List<byte>();
            result.Add(Convert.ToByte(252));
            return result.ToArray();
        }
        #endregion
    }
}