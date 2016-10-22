using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SecureIM.Desktop.model.abstractions;

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
