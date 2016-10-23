using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SecureIM.Desktop.model.abstractions;

namespace SecureIM.Desktop.controller
{
    class Comms : AbstractComms
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
