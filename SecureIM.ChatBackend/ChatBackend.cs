using System;
using System.Diagnostics;
using System.ServiceModel;
using System.Text;
using System.Text.RegularExpressions;
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
        #region Private Fields

        private readonly User _eventUser = new User("Event");
        private readonly User _infoUser = new User("Info");

        #endregion Private Fields

        #region Public Properties

        [NotNull] public Comms Comms { get; private set; }
        [NotNull] public ICryptoHandler CryptoHandler { get; }
        [NotNull] public User CurrentUser { get; }
        [NotNull] public DisplayMessageDelegate DisplayMessageDelegate { get; }

        #endregion Public Properties

        #region Public Constructors

        /// <summary>
        ///     ChatBackend constructor should be called with a delegate that is capable of displaying messages.
        /// </summary>
        /// <param name="dmd">DisplayMessageDelegate</param>
        // ReSharper disable once NotNullMemberIsNotInitialized
        public ChatBackend([NotNull] DisplayMessageDelegate dmd)
        {
            CurrentUser = new User();
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
                messageComposite.Message.MessageText = plaintext;
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
            var commandRegEx = new Regex(@"^([a-z]+):", RegexOptions.Multiline);
            string commandMatch = commandRegEx.Match(text).Value.ToLower();

            switch (commandMatch)
            {
                case "setname:":
                    string nameSetString = "Setting your name to " + CurrentUser.Name;
                    SendMessageToChannel(_eventUser, nameSetString);
                    break;

                case "genkey:":
                    CryptoHandler.GenerateAsymmetricKeyPair();
                    SendMessageToChannel(CurrentUser, EncodeByteArrayBase64(CryptoHandler.GetPublicKey()));
                    SendMessageToChannel(CurrentUser, EncodeByteArrayBase64(CryptoHandler.GetPrivateKey()));
                    break;

                case "getpub:":
                    SendMessageToChannel(CurrentUser, EncodeByteArrayBase64(CryptoHandler.GetPublicKey()));
                    break;

                case "encrypt:":
                    string cipherText = EncryptChatMessage(text);
                    SendMessageToChannel(CurrentUser, cipherText);
                    break;

                case "decrypt:":
                    string plainText = DecryptChatMessage(text);
                    SendMessageToChannel(CurrentUser, plainText);
                    break;

                default:
                    SendMessageToChannel(CurrentUser, text);
                    break;
            }

            //            if (text.StartsWith("setname:", StringComparison.OrdinalIgnoreCase))
            //            {
            //                string nameSetString = "Setting your name to " + CurrentUser.Name;
            //                SendMessageToChannel(_eventUser, nameSetString);
            //            }
            //            else if (text.StartsWith("genkey:", StringComparison.OrdinalIgnoreCase))
            //            {
            //                CryptoHandler.GenerateAsymmetricKeyPair();
            //
            //                SendMessageToChannel(CurrentUser, EncodeByteArrayBase64(CryptoHandler.GetPublicKey()));
            //                SendMessageToChannel(CurrentUser, EncodeByteArrayBase64(CryptoHandler.GetPrivateKey()));
            //            }
            //            else if (text.StartsWith("getpub:", StringComparison.OrdinalIgnoreCase))
            //            {
            //                SendMessageToChannel(CurrentUser, EncodeByteArrayBase64(CryptoHandler.GetPublicKey()));
            //            }
            //            else if (text.StartsWith("encrypt:", StringComparison.OrdinalIgnoreCase))
            //            {
            //                text = GetMessageCommandData(text, "encrypt:");
            //                string cipherText = CryptoHandler.Encrypt(text, CryptoHandler.GetPublicKey());
            //                cipherText = Encoding.UTF8.EncodeBase64(cipherText);
            //
            //                SendMessageToChannel(CurrentUser, cipherText);
            //            }
            //            else if (text.StartsWith("decrypt:", StringComparison.OrdinalIgnoreCase))
            //            {
            //                text = GetMessageCommandData(text, "decrypt:");
            //                string plainText = CryptoHandler.Decrypt(text, CryptoHandler.GetPublicKey());
            //                plainText = Encoding.UTF8.EncodeBase64(plainText);
            //
            //                SendMessageToChannel(CurrentUser, plainText);
            //            }
            //            else
            //            {
            //                SendMessageToChannel(CurrentUser, text);
            //            }
        }

        #endregion Public Methods

        #region Private Methods

        [NotNull]
        private static string EncodeByteArrayBase64([NotNull] byte[] dataBytes)
        {
            string pubKeyB64 = Encoding.UTF8.EncodeBase64(Encoding.UTF8.GetString(dataBytes));
            return pubKeyB64;
        }

        [NotNull]
        private static string GetMessageCommandData([NotNull] string text, [NotNull] string command)
            => text.Split(new[] {command}, StringSplitOptions.None)[1];

        private static void SendMessageToChannel([NotNull] User user, [NotNull] string plainText)
        {
            var messageComposite = new MessageComposite(user, plainText);
            new ChannelFactory<IChatBackend>("ChatEndpoint").CreateChannel().DisplayMessage(messageComposite);
        }

        [NotNull]
        private string DecryptChatMessage(string text)
        {
            text = GetMessageCommandData(text, "decrypt:");
            string plainText = CryptoHandler.Decrypt(text, CryptoHandler.GetPublicKey());
            plainText = Encoding.UTF8.EncodeBase64(plainText);
            return plainText;
        }

        [NotNull]
        private string EncryptChatMessage(string text)
        {
            text = GetMessageCommandData(text, "encrypt:");
            string cipherText = CryptoHandler.Encrypt(text, CryptoHandler.GetPublicKey());
            cipherText = Encoding.UTF8.EncodeBase64(cipherText);
            return cipherText;
        }

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
            string userJoinedMessage = $"{CurrentUser.Name} has entered the conversation.";
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
            string userLeftMessage = $"{CurrentUser.Name} is leaving the conversation.";
            channelFactory.CreateChannel().DisplayMessage(new MessageComposite(_eventUser, userLeftMessage));

            if (Comms.Host.State == CommunicationState.Closed) return;

            channelFactory.Close();
            Comms.Host.Close();
        }

        #endregion Private Methods
    }
}