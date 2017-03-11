using System.ServiceModel;
using JetBrains.Annotations;

namespace SecureIM.ChatBackend.model
{
    /// <summary>
    /// Comms
    /// </summary>
    internal class Comms
    {
        #region Public Properties

        private ChannelFactory<IChatBackend> ChannelFactory { get; }
        private IChatBackend ChatChannel { get; }
        public ServiceHost Host { get; }

        #endregion Public Properties


        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Comms" /> class.
        /// </summary>
        /// <param name="channelFactory">The channel factory.</param>
        /// <param name="chatChannel">The chat channel.</param>
        /// <param name="host">The host.</param>
        public Comms([NotNull] ChannelFactory<IChatBackend> channelFactory, [NotNull] IChatBackend chatChannel,
                     [NotNull] ServiceHost host)
        {
            ChatChannel = chatChannel;
            ChannelFactory = channelFactory;
            Host = host;
        }

        #endregion Public Constructors
    }
}