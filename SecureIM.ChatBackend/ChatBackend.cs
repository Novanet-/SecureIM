using System;
using System.ServiceModel;
using SecureIM.ChatBackend.model;

namespace SecureIM.ChatBackend
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class ChatBackend : IChatBackend
    {
        #region Public Properties


        public Comms Comms { get; private set; }
        public DisplayMessageDelegate DisplayMessageDelegate { get; }
        public User User { get; }


        #endregion Public Properties




        #region Public Constructors


        /// <summary>
        /// ChatBackend constructor should be called with a delegate that is capable of displaying messages.
        /// </summary>
        /// <param name="dmd">DisplayMessageDelegate</param>
        public ChatBackend(DisplayMessageDelegate dmd)
        {
            User = new User();
            DisplayMessageDelegate = dmd;
            StartService();
        }


        #endregion Public Constructors




        #region Private Constructors


        /// <summary>
        /// The default constructor is only here for testing purposes.
        /// </summary>
        private ChatBackend() { }


        #endregion Private Constructors




        #region Public Methods


        /// <summary>
        /// This method gets called by our friends when they want to display a message on our screen.
        /// We're really only returning a string for demonstration purposes … it might be cleaner
        /// to return void and also make this a one-way communication channel.
        /// </summary>
        /// <param name="messageComposite">The messageComposite.</param>
        /// <exception cref="System.ArgumentNullException">messageComposite</exception>
        public void DisplayMessage(MessageComposite messageComposite)
        {
            if (messageComposite == null) throw new ArgumentNullException(nameof(messageComposite));

            DisplayMessageDelegate?.Invoke(messageComposite);
        }

        /// <summary>
        /// The front-end calls the SendMessage method in order to broadcast a message to our friends
        /// </summary>
        /// <param name="text">The text.</param>
        public void SendMessage(string text)
        {
            if (text.StartsWith("setname:", StringComparison.OrdinalIgnoreCase))
            {
                User.Name = text.Substring("setname:".Length).Trim();
                DisplayMessageDelegate(new MessageComposite("Event", "Setting your name to " + User.Name));
            }
            else
            {
                // In order to send a message, we call our friends' DisplayMessage method
                new ChannelFactory<IChatBackend>("ChatEndpoint").CreateChannel()
                                                                .DisplayMessage(new MessageComposite(User.Name, text));
            }
        }


        #endregion Public Methods




        #region Private Methods


        /// <summary>
        /// Starts the service.
        /// </summary>
        private void StartService()
        {
            var channelFactory = new ChannelFactory<IChatBackend>("ChatEndpoint");
            IChatBackend channel = channelFactory.CreateChannel();
            var serviceHost = new ServiceHost(this);

            Comms = new Comms(channelFactory, channel, serviceHost);
            Comms.Host.Open();

            // Information to send to the channel
            channel.DisplayMessage(new MessageComposite("Event", User.Name + " has entered the conversation."));

            // Information to display locally
            DisplayMessageDelegate(new MessageComposite("Info", "To change your name, type setname: NEW_NAME"));
        }

        /// <summary>
        /// Stops the service.
        /// </summary>
        private void StopService()
        {
            if (Comms.Host == null) return;

            var channelFactory = new ChannelFactory<IChatBackend>("ChatEndpoint");
            channelFactory.CreateChannel()
                          .DisplayMessage(new MessageComposite("Event", User.Name + " is leaving the conversation."));

            if (Comms.Host.State == CommunicationState.Closed) return;

            channelFactory.Close();
            Comms.Host.Close();
        }


        #endregion Private Methods
    }
}