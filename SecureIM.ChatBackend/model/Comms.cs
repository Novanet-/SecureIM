using System.ServiceModel;

namespace SecureIM.ChatBackend.model
{
    public class Comms
    {
        public IChatBackend ChatChannel { get; }
        public ChannelFactory<IChatBackend> ChannelFactory { get; }
        public ServiceHost Host { get; }

        public Comms(ChannelFactory<IChatBackend> channelFactory, IChatBackend chatChannel, ServiceHost host)
        {
            ChatChannel = chatChannel;
            ChannelFactory = channelFactory;
            Host = host;
        }
    }
}