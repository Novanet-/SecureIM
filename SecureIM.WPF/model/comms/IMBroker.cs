using System;
using SecureIM.WPF.model.abstractions;

namespace SecureIM.WPF.model.comms
{
    // ReSharper disable once InconsistentNaming
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