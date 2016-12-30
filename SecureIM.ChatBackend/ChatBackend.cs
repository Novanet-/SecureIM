using System;
using System.ServiceModel;

namespace SecureIM.ChatBackend
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class ChatBackend : IChatBackend
    {
        #region Public Constructors

        /// <summary>
        ///     ChatBackend constructor should be called with a delegate that is capable of displaying messages.
        /// </summary>
        /// <param name="dmd">DisplayMessageDelegate</param>
        public ChatBackend(DisplayMessageDelegate dmd)
        {
            _displayMessageDelegate = dmd;
            StartService();
        }

        #endregion Public Constructors

        #region Private Constructors

        /// <summary>
        ///     The default constructor is only here for testing purposes.
        /// </summary>
        private ChatBackend()
        {
        }

        #endregion Private Constructors

        #region Private Fields

        private readonly DisplayMessageDelegate _displayMessageDelegate;

        private IChatBackend _channel;

        private string _myUserName = "Anonymous";

        private ChannelFactory<IChatBackend> _channelFactory;

        private ServiceHost _host;

        #endregion Private Fields

        #region Public Methods

        /// <summary>
        ///     This method gets called by our friends when they want to display a message on our screen.
        ///     We're really only returning a string for demonstration purposes … it might be cleaner
        ///     to return void and also make this a one-way communication channel.
        /// </summary>
        /// <param name="composite"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public void DisplayMessage(CompositeType composite)
        {
            if (composite == null)
                throw new ArgumentNullException(nameof(composite));

            _displayMessageDelegate?.Invoke(composite);
        }

        /// <summary>
        ///     The front-end calls the SendMessage method in order to broadcast a message to our friends
        /// </summary>
        /// <param name="text"></param>
        public void SendMessage(string text)
        {
            if (text.StartsWith("setname:", StringComparison.OrdinalIgnoreCase))
            {
                _myUserName = text.Substring("setname:".Length).Trim();
                _displayMessageDelegate(new CompositeType("Event", "Setting your name to " + _myUserName));
            }
            else
            {
                // In order to send a message, we call our friends' DisplayMessage method
                _channel.DisplayMessage(new CompositeType(_myUserName, text));
            }
        }

        #endregion Public Methods

        #region Private Methods

        private void StartService()
        {
            _host = new ServiceHost(this);
            _host.Open();
            _channelFactory = new ChannelFactory<IChatBackend>("ChatEndpoint");
            _channel = _channelFactory.CreateChannel();

            // Information to send to the channel
            _channel.DisplayMessage(new CompositeType("Event", _myUserName + " has entered the conversation."));

            // Information to display locally
            _displayMessageDelegate(new CompositeType("Info", "To change your name, type setname: NEW_NAME"));
        }

        private void StopService()
        {
            if (_host == null) return;

            _channel.DisplayMessage(new CompositeType("Event", _myUserName + " is leaving the conversation."));

            if (_host.State == CommunicationState.Closed) return;

            _channelFactory.Close();
            _host.Close();
        }

        #endregion Private Methods
    }
}