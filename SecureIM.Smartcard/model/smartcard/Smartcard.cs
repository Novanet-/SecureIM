using System;
using SecureIM.Smartcard.model.abstractions;

namespace SecureIM.Smartcard.model.smartcard
{
    internal class Smartcard : AbstractSmartcard
    {
        public override string Decrypt()
        {
            throw new NotImplementedException();
        }

        public override string Encrypt()
        {
            throw new NotImplementedException();
        }

        public override bool EraseSmartcard()
        {
            throw new NotImplementedException();
        }

        public override bool RegisterSmartcard()
        {
            throw new NotImplementedException();
        }

        public override bool UnlockCertificate()
        {
            throw new NotImplementedException();
        }

        public override bool Ping()
        {
            throw new NotImplementedException();
        }
    }
}