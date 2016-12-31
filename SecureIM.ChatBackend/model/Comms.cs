using System.ServiceModel;

namespace SecureIM.ChatBackend.model
{
    public class Comms
    {
        #region Public Properties


        public ChannelFactory<IChatBackend> ChannelFactory { get; }
        public IChatBackend ChatChannel { get; }
        public ServiceHost Host { get; }


        #endregion Public Properties




        #region Public Constructors


        /// <summary>
        /// Initializes a new instance of the <see cref="Comms"/> class.
        /// </summary>
        /// <param name="channelFactory">The channel factory.</param>
        /// <param name="chatChannel">The chat channel.</param>
        /// <param name="host">The host.</param>
        public Comms(ChannelFactory<IChatBackend> channelFactory, IChatBackend chatChannel, ServiceHost host)
        {
            ChatChannel = chatChannel;
            ChannelFactory = channelFactory;
            Host = host;
        }


        #endregion Public Constructors
    }
}