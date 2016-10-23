using SecureIM.Desktop.model.abstractions;
using System;

namespace SecureIM.Desktop.model
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