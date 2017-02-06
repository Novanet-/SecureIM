using System;
using System.ServiceModel;
using System.Text;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using SecureIM.ChatBackend.model;
using SecureIM.Smartcard.controller.smartcard;
using SecureIM.Smartcard.helpers;
using SecureIM.Smartcard.model.abstractions;

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

            if (messageComposite.Flags.HasFlag(MessageFlags.Encoded) &&
                messageComposite.Flags.HasFlag(MessageFlags.Encrypted))
            {
                string decodedMessageText = Encoding.UTF8.DecodeBase64(messageComposite.Message.Text);

                byte[] targetPubKeyBytes = CryptoHandler.GetPublicKey();
                string plainText = CryptoHandler.Decrypt(decodedMessageText, targetPubKeyBytes);
                messageComposite = new MessageComposite(messageComposite.Sender, plainText);
            }

            DisplayMessageDelegate(messageComposite);
        }

        /// <summary>
        ///     The front-end calls the SendMessage method in order to broadcast a message to our friends
        /// </summary>
        /// <param name="text">The text.</param>
        public void SendMessage([NotNull] string text)
        {
            var commandRegEx = new Regex(@"^([\w]+:)\s*(.*)", RegexOptions.Multiline);
            Match commandMatch = commandRegEx.Match(text);
            GroupCollection messageGroups = commandMatch.Groups;
            string commandMatchString = commandMatch.Success ? messageGroups[1].Value.ToLower() : "";

            string plainText;
            string cipherText;
            string pubKeyB64;
            switch (commandMatchString)
            {
                case "setname:":
                    CurrentUser.Name = text.Substring(commandMatchString.Length).Trim();
                    string nameSetString = "Setting your name to " + CurrentUser.Name;
                    SendMessageToChannel(_eventUser, nameSetString);
                    break;

                case "genkey:":
                    CryptoHandler.GenerateAsymmetricKeyPair();

                    pubKeyB64 = EncodeByteArrayBase64(CryptoHandler.GetPublicKey());
                    if (!string.IsNullOrEmpty(pubKeyB64)) SendMessageToChannel(CurrentUser, pubKeyB64, MessageFlags.Encoded);

                    string priKeyB64 = EncodeByteArrayBase64(CryptoHandler.GetPrivateKey());
                    if (!string.IsNullOrEmpty(priKeyB64)) SendMessageToChannel(CurrentUser, priKeyB64, MessageFlags.Encoded);
                    break;

                case "getpub:":
                    pubKeyB64 = EncodeByteArrayBase64(CryptoHandler.GetPublicKey());
                    if (!string.IsNullOrEmpty(pubKeyB64)) SendMessageToChannel(CurrentUser, pubKeyB64, MessageFlags.Encoded);
                    break;

                case "regpub:":
                    pubKeyB64 = EncodeByteArrayBase64(CryptoHandler.GetPublicKey());
                    CurrentUser.PublicKey = pubKeyB64;
                    SendMessageToChannel(CurrentUser, $"Registered: {pubKeyB64}", MessageFlags.Encoded);
                    break;

                case "encrypt:":
                    plainText = messageGroups[2].Value;
                    cipherText = EncryptChatMessage(plainText);
                    cipherText = Encoding.UTF8.EncodeBase64(cipherText);
                    if (!string.IsNullOrEmpty(cipherText))
                        SendMessageToChannel(CurrentUser, cipherText, MessageFlags.Encoded | MessageFlags.Encrypted);
                    break;

                case "decrypt:":
                    cipherText = messageGroups[2].Value;
                    plainText = DecryptChatMessage(cipherText);
                    plainText = Encoding.UTF8.EncodeBase64(plainText);
                    if (!string.IsNullOrEmpty(plainText)) SendMessageToChannel(CurrentUser, plainText, MessageFlags.Encoded);
                    break;

                case "db64:":
                    cipherText = messageGroups[2].Value;
                    plainText = Encoding.UTF8.DecodeBase64(cipherText);
                    if (!string.IsNullOrEmpty(plainText)) SendMessageToChannel(CurrentUser, plainText);
                    break;

                case "eb64:":
                    plainText = messageGroups[2].Value;
                    cipherText = Encoding.UTF8.EncodeBase64(plainText);
                    if (!string.IsNullOrEmpty(plainText)) SendMessageToChannel(CurrentUser, cipherText, MessageFlags.Encoded);
                    break;

                default:
                    SendMessageToChannel(CurrentUser, text);
                    break;
            }
        }

        #endregion Public Methods

        #region Private Methods

        [CanBeNull]
        private static string EncodeByteArrayBase64([NotNull] byte[] dataBytes)
        {
            string pubKeyB64 = Encoding.UTF8.EncodeBase64(Encoding.UTF8.GetString(dataBytes));
            return pubKeyB64;
        }

        //        [NotNull]
        //        private static string GetMessageCommandData([NotNull] string text, [NotNull] string command)
        //            => text.Split(new[] { command }, StringSplitOptions.None)[1];

        private static void SendMessageToChannel([NotNull] User sender, [NotNull] string messageText,
            MessageFlags messageFlags = MessageFlags.None)
        {
            var messageComposite = new MessageComposite(sender, messageText, messageFlags);
            new ChannelFactory<IChatBackend>("ChatEndpoint").CreateChannel().DisplayMessage(messageComposite);
        }

        [NotNull]
        private string DecryptChatMessage([NotNull] string text)
        {
            //            text = GetMessageCommandData(text, "decrypt:");
            string plainText = CryptoHandler.Decrypt(text, CryptoHandler.GetPublicKey());
            return plainText;
        }

        [NotNull]
        private string EncryptChatMessage([NotNull] string text)
        {
            //            text = GetMessageCommandData(text, "encrypt:");
            string cipherText = CryptoHandler.Encrypt(text, CryptoHandler.GetPublicKey());
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