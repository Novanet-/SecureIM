using SecureIM.Desktop.model.abstractions;
using System;

namespace SecureIM.Desktop.model
{
    internal class Client : AbstractClient
    {
        public override bool RecieveMessage()
        {
            throw new NotImplementedException();
        }

        public override bool SendMessage()
        {
            throw new NotImplementedException();
        }
    }
}