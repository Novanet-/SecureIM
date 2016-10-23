using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecureIM.Desktop.model.abstractions
{
    internal interface IComms
    {
        bool RecieveMessage(string message);
        bool SendMessage(string message);
        bool ResolveAddress(string ip);
    }

    abstract class AbstractComms : IComms
    {
        public abstract bool RecieveMessage(string message);
        public abstract bool SendMessage(string message);
        public abstract bool ResolveAddress(string ip);
    }
}
