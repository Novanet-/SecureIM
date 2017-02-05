using System;
using System.Diagnostics;
using System.ServiceModel;
using System.Text;
using JetBrains.Annotations;
using SecureIM.ChatBackend.model;
using SecureIM.Smartcard.helpers;
using SecureIM.Smartcard.model.abstractions;
using SecureIM.Smartcard.model.smartcard;

namespace SecureIM.ChatBackend
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class ChatBackend : IChatBackend
    {
        private readonly User _eventUser = new User("Event");
        private readonly User _infoUser = new User("Info");

        #region Public Constructors

        /// <summary>
        ///     ChatBackend constructor should be called with a delegate that is capable of displaying messages.
        /// </summary>
        /// <param name="dmd">DisplayMessageDelegate</param>
        // ReSharper disable once NotNullMemberIsNotInitialized
        public ChatBackend([NotNull] DisplayMessageDelegate dmd)
        {
            User = new User();
            DisplayMessageDelegate = dmd;
            CryptoHandler = new SmartcardCryptoHandler();
            StartService();
        }

        #endregion Public Constructors

        #region Private Constructors

        /// <summary>
        ///     The default constructor is only here for testing purposes.
        /// </summary>
        // ReSharper disable once NotNullMemberIsNotInitialized
        private ChatBackend()
        {
        }

        #endregion Private Constructors

        #region Public Properties

        [NotNull]
        public Comms Comms { get; private set; }

        [NotNull]
        public DisplayMessageDelegate DisplayMessageDelegate { get; }

        [NotNull]
        public User User { get; }

        [NotNull]
        public ICryptoHandler CryptoHandler { get; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        ///     This method gets called by our friends when they want to display a message on our screen.
        ///     We're really only returning a string for demonstration purposes … it might be cleaner
        ///     to return void and also make this a one-way communication channel.
        /// </summary>
        /// <param name="messageComposite">The messageComposite.</param>
        /// <exception cref="System.ArgumentNullException">messageComposite</exception>
        public void DisplayMessage([NotNull] MessageComposite messageComposite)
        {
            if (messageComposite == null) throw new ArgumentNullException(nameof(messageComposite));

            try
            {
                string cipherText = Encoding.UTF8.DecodeBase64(messageComposite.Message.MessageText);
                //TODO: This is temporary, fix it
                byte[] targetPubKeyBytes = CryptoHandler.GetPublicKey();

                string plaintext = CryptoHandler.Decrypt(cipherText, targetPubKeyBytes);
//                messageComposite.Message.MessageText = plaintext;
                messageComposite = new MessageComposite(messageComposite.Sender, messageComposite.Message.MessageText);
            }
            catch
            {
                // ignored
            }

            DisplayMessageDelegate(messageComposite);
        }

        /// <summary>
        ///     The front-end calls the SendMessage method in order to broadcast a message to our friends
        /// </summary>
        /// <param name="text">The text.</param>
        public void SendMessage([NotNull] string text)
        {
            if (text.StartsWith("setname:", StringComparison.OrdinalIgnoreCase))
            {
                User.Name = text.Substring("setname:".Length).Trim();
//                DisplayMessageDelegate(new MessageComposite("Event", "Setting your name to " + User.Name));
                DisplayMessageDelegate(new MessageComposite(new User("Event", null), "Setting your name to " + User.Name));
            }
            else if (text.StartsWith("test1:", StringComparison.OrdinalIgnoreCase))
            {
                CryptoHandler.GenerateAsymmetricKeyPair();
                byte[] pubKeyBytes = CryptoHandler.GetPublicKey();
                byte[] priKeyBytes = CryptoHandler.GetPrivateKey();

                string ct = CryptoHandler.Encrypt("hello", pubKeyBytes);
                string pt = CryptoHandler.Decrypt(ct, pubKeyBytes);
                Debug.Write($"{pt} should = \"hello\" ");
            }
            else if (text.StartsWith("test2:", StringComparison.OrdinalIgnoreCase))
            {
                //TODO: This is temporary, fix it
                byte[] targetPubKeyBytes = CryptoHandler.GetPublicKey();

                string cipherText = CryptoHandler.Encrypt(text, targetPubKeyBytes);
                cipherText = Encoding.UTF8.EncodeBase64(cipherText);
                // In order to send a message, we call our friends' DisplayMessage method
                var messageComposite = new MessageComposite(User, cipherText);
                new ChannelFactory<IChatBackend>("ChatEndpoint").CreateChannel()
                    .DisplayMessage(messageComposite);
            }
            else if (text.StartsWith("exit:", StringComparison.OrdinalIgnoreCase))
            {
                StopService();
            }
            else
            {
                var messageComposite = new MessageComposite(User, text);
                new ChannelFactory<IChatBackend>("ChatEndpoint").CreateChannel()
                    .DisplayMessage(messageComposite);
            }
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        ///     Starts the service.
        /// </summary>
        private void StartService()
        {
            var channelFactory = new ChannelFactory<IChatBackend>("ChatEndpoint");
            IChatBackend channel = channelFactory.CreateChannel();
            var serviceHost = new ServiceHost(this);

            Comms = new Comms(channelFactory, channel, serviceHost);
            Comms.Host.Open();

            // Information to send to the channel
            string userJoinedMessage = $"{User.Name} has entered the conversation.";
            channel.DisplayMessage(new MessageComposite(_eventUser, userJoinedMessage));

            // Information to display locally
            const string changeNamePrompt = "To change your name, type setname: NEW_NAME";
            DisplayMessageDelegate(new MessageComposite(_infoUser, changeNamePrompt));
        }

        /// <summary>
        ///     Stops the service.
        /// </summary>
        private void StopService()
        {
            var channelFactory = new ChannelFactory<IChatBackend>("ChatEndpoint");
            string userLeftMessage = $"{User.Name} is leaving the conversation.";
            channelFactory.CreateChannel().DisplayMessage(new MessageComposite(_eventUser, userLeftMessage));

            if (Comms.Host.State == CommunicationState.Closed) return;

            channelFactory.Close();
            Comms.Host.Close();
        }

        #endregion Private Methods
    }
}