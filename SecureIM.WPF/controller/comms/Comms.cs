using System;
using SecureIM.WPF.controller.abstractions;

namespace SecureIM.WPF.controller.comms
{
    internal class Comms : AbstractComms
    {
        public override bool RecieveMessage(string message)
        {
            throw new NotImplementedException();
        }

        public override bool SendMessage(string message)
        {
            throw new NotImplementedException();
        }

        public override bool ResolveAddress(string ip)
        {
            throw new NotImplementedException();
        }
    }
}