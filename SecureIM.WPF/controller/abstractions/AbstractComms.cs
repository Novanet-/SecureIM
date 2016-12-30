namespace SecureIM.WPF.controller.abstractions
{
    internal interface IComms
    {
        bool RecieveMessage(string message);

        bool SendMessage(string message);

        bool ResolveAddress(string ip);
    }

    internal abstract class AbstractComms : IComms
    {
        public abstract bool RecieveMessage(string message);

        public abstract bool SendMessage(string message);

        public abstract bool ResolveAddress(string ip);
    }
}