using System;
using SecureIM.Smartcard.controller.abstractions;

namespace SecureIM.Smartcard.controller.comms
{
    internal class Comms : AbstractComms
    {
        #region Public Methods


        public override bool RecieveMessage(string message) { throw new NotImplementedException(); }

        public override bool ResolveAddress(string ip) { throw new NotImplementedException(); }

        public override bool SendMessage(string message) { throw new NotImplementedException(); }


        #endregion Public Methods
    }
}