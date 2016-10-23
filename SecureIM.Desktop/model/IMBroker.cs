using SecureIM.Desktop.model.abstractions;
using System;

namespace SecureIM.Desktop.model
{
    internal class IMBroker : AbstractClient
    {
        public override bool RecieveMessage(string message)
        {
            throw new NotImplementedException();
        }

        public override bool SendMessage(string message)
        {
            throw new NotImplementedException();
        }
    }
}