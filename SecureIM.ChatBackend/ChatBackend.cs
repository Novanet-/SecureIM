using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using JetBrains.Annotations;
using SecureIM.ChatBackend.model;
using SecureIM.Smartcard.controller.smartcard;
using SecureIM.Smartcard.helpers;
using SecureIM.Smartcard.model.abstractions;

namespace SecureIM.ChatBackend
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public sealed class ChatBackend : IChatBackend
    {
        #region Private Fields

        private DisplayMessageDelegate _displayMessageDelegate;
        public User BroadcastUser { get; } = new User("Broadcast");
        public List<User> FriendsList { get; }

        #endregion Private Fields

        #region Public Properties

        [ItemNotNull] public static Lazy<ChatBackend> Lazy { get; } = new Lazy<ChatBackend>(() => new ChatBackend());
        public Comms Comms { get; private set; }
        [NotNull] public ICryptoHandler CryptoHandler { get; }
        [NotNull] public User CurrentUser { get; }
        [NotNull] public DisplayMessageDelegate DisplayMessageDelegate
        {
            get { return _displayMessageDelegate; }
            set
            {
                if (DisplayMessageDelegateIsSet) return;

                _displayMessageDelegate = value;
                DisplayMessageDelegateIsSet = true;
            }
        }

        public bool DisplayMessageDelegateIsSet { get; set; }
        public User EventUser { get; } = new User("Event");
        public User InfoUser { get; } = new User("Info");

        [NotNull] public static ChatBackend Instance => Lazy.Value;

        #endregion Public Properties

        #region Private Constructors

        private ChatBackend()
        {
            CurrentUser = new User();
            CryptoHandler = new SmartcardCryptoHandler();
            FriendsList = new List<User>();
        }

        #endregion Private Constructors

        /// <summary>
        ///     The default constructor is only here for testing purposes.
        /// </summary>
        // ReSharper disable once NotNullMemberIsNotInitialized

        #region Public Methods

        /// <summary>
        ///     This method gets called by our friends when they want to display a message on our screen.
        ///     We're really only returning a string for demonstration purposes … it might be cleaner
        ///     to return void and also make this a one-way communication channel.
        /// </summary>
        /// <param name="messageComposite">The messageComposite.</param>
        /// <exception cref="System.ArgumentNullException">messageComposite</exception>
        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        public void DisplayMessage([NotNull] MessageComposite messageComposite)
        {
            if (!DisplayMessageDelegateIsSet) return;

            if (messageComposite == null) throw new ArgumentNullException(nameof(messageComposite));

            if (messageComposite.Flags.HasFlag(MessageFlags.Encoded)
                && messageComposite.Flags.HasFlag(MessageFlags.Encrypted))
            {
                string decodedMessageText = Encoding.UTF8.DecodeBase64(messageComposite.Message.Text);

                if (decodedMessageText != null)
                {
                    byte[] targetPubKeyBytes = CryptoHandler.GetPublicKey();
                    string plainText = CryptoHandler.Decrypt(decodedMessageText, targetPubKeyBytes);
                    messageComposite = new MessageComposite(messageComposite.Sender, messageComposite.Receiver, plainText, messageComposite.Flags);
                }
            }

            DisplayMessageDelegate(messageComposite);
        }

        /// <summary>
        ///     The front-end calls the SendMessage method in order to broadcast a message to our friends
        /// </summary>
        /// <param name="text">The text.</param>
        /// <exception cref="RegexMatchTimeoutException">A time-out occurred. For more information about time-outs, see the Remarks section.</exception>
        /// <exception cref="ArgumentException">A regular expression parsing error occurred. </exception>
        public void SendMessage(string text)
        {
            if (!DisplayMessageDelegateIsSet) return;

            var commandRegEx = new Regex(@"^([\w]+:)\s*(.*)", RegexOptions.Multiline);
            Match commandMatch = commandRegEx.Match(text);
            GroupCollection messageGroups = commandMatch.Groups;
            string commandMatchString = commandMatch.Success ? messageGroups[1].Value.ToLower() : string.Empty;

            string commandData;
            string cipherText;
            string pubKeyB64;
            var messageReceiver = new User();
            string messageText;
            User messageSender = EventUser;
            switch (commandMatchString)
            {
                case "setname:":
                    CurrentUser.Name = text.Substring(commandMatchString.Length).Trim();
                    string nameSetString = "Setting your name to " + CurrentUser.Name;
                    messageText = nameSetString;
                    SendMessageToChannel(messageSender, messageReceiver, messageText);
                    break;

                case "genkey:":
                    CryptoHandler.GenerateAsymmetricKeyPair();

                    pubKeyB64 = EncodeByteArrayBase64(CryptoHandler.GetPublicKey());
                    if (!string.IsNullOrEmpty(pubKeyB64))
                    {
                        messageSender = CurrentUser;
                        messageText = pubKeyB64;
                        SendMessageToChannel(messageSender, messageReceiver, messageText, MessageFlags.Encoded);
                    }

                    string priKeyB64 = EncodeByteArrayBase64(CryptoHandler.GetPrivateKey());
                    if (!string.IsNullOrEmpty(priKeyB64)) SendMessageToChannel(messageSender, messageReceiver, priKeyB64, MessageFlags.Encoded);
                    break;

                case "getpub:":
                    pubKeyB64 = EncodeByteArrayBase64(CryptoHandler.GetPublicKey());
                    messageSender = CurrentUser;
                    messageText = pubKeyB64;
                    if (!string.IsNullOrEmpty(pubKeyB64)) SendMessageToChannel(messageSender, messageReceiver, messageText, MessageFlags.Encoded);
                    break;

                case "regpub:":
                    pubKeyB64 = EncodeByteArrayBase64(CryptoHandler.GetPublicKey());
                    CurrentUser.PublicKey = pubKeyB64;
                    messageSender = CurrentUser;
                    messageText = $"Registered: {pubKeyB64}";
                    SendMessageToChannel(messageSender, messageReceiver, messageText, MessageFlags.Encoded);
                    break;

                case "encrypt:":
                    commandData = messageGroups[2].Value;
                    cipherText = EncryptChatMessage(commandData);
                    cipherText = Encoding.UTF8.EncodeBase64(cipherText);
                    if (!string.IsNullOrEmpty(cipherText))
                        messageSender = CurrentUser;
                    SendMessageToChannel(messageSender, messageReceiver, cipherText, MessageFlags.Encoded | MessageFlags.Encrypted);
                    break;

                case "decrypt:":
                    cipherText = messageGroups[2].Value;
                    commandData = DecryptChatMessage(cipherText);
                    commandData = Encoding.UTF8.EncodeBase64(commandData);
                    messageSender = CurrentUser;
                    if (!string.IsNullOrEmpty(commandData)) SendMessageToChannel(messageSender, messageReceiver, commandData, MessageFlags.Encoded);
                    break;

                case "db64:":
                    cipherText = messageGroups[2].Value;
                    commandData = Encoding.UTF8.DecodeBase64(cipherText);
                    messageSender = CurrentUser;
                    if (!string.IsNullOrEmpty(commandData)) SendMessageToChannel(messageSender, messageReceiver, commandData);
                    break;

                case "eb64:":
                    commandData = messageGroups[2].Value;
                    cipherText = Encoding.UTF8.EncodeBase64(commandData);
                    messageSender = CurrentUser;
                    if (!string.IsNullOrEmpty(cipherText)) SendMessageToChannel(messageSender, messageReceiver, cipherText, MessageFlags.Encoded);
                    break;
                case "addfriend:":
                    commandData = messageGroups[2].Value;
                    string[] commandDataSplit = commandData.Split(':');
                    string alias = commandDataSplit[0];
                    pubKeyB64 = commandDataSplit[1];
                    var newFriend = new User(alias, pubKeyB64);
                    FriendsList.Add(newFriend);
                    string confirmMessage = $"Friend ({alias}) added with public key: {pubKeyB64}";
                    messageSender = CurrentUser;
                    SendMessageToChannel(messageSender, CurrentUser, confirmMessage);
                    break;

                default:
                    messageSender = CurrentUser;
                    SendMessageToChannel(messageSender, messageReceiver, text);
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

        private static void SendMessageToChannel([NotNull] User sender, [NotNull] User receiver, [NotNull] string messageText,
            MessageFlags messageFlags = MessageFlags.None)
        {
            Task.Run(() =>
            {
                var messageComposite = new MessageComposite(sender, receiver, messageText, messageFlags);
                new ChannelFactory<IChatBackend>("ChatEndpoint").CreateChannel().DisplayMessage(messageComposite);
            });


//            var messageComposite = new MessageComposite(sender, receiver, messageText, messageFlags);
//            new ChannelFactory<IChatBackend>("ChatEndpoint").CreateChannel().DisplayMessage(messageComposite);
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
        public void StartService()
        {
            if (!DisplayMessageDelegateIsSet) return;

            var channelFactory = new ChannelFactory<IChatBackend>("ChatEndpoint");
            IChatBackend channel = channelFactory.CreateChannel();
            var serviceHost = new ServiceHost(this);

            Comms = new Comms(channelFactory, channel, serviceHost);
            Comms.Host.Open();

            // Information to send to the channel
            string userJoinedMessage = $"{CurrentUser.Name} has entered the conversation.";
            channel.DisplayMessage(new MessageComposite(EventUser, BroadcastUser, userJoinedMessage, MessageFlags.Broadcast));

            // Information to display locally
            const string changeNamePrompt = "To change your name, type setname: NEW_NAME";
            DisplayMessageDelegate(new MessageComposite(InfoUser, CurrentUser, changeNamePrompt, MessageFlags.Broadcast));
        }

        /// <summary>
        ///     Stops the service.
        /// </summary>
        private void StopService()
        {
            var channelFactory = new ChannelFactory<IChatBackend>("ChatEndpoint");
            string userLeftMessage = $"{CurrentUser.Name} is leaving the conversation.";
            channelFactory.CreateChannel().DisplayMessage(new MessageComposite(EventUser, BroadcastUser, userLeftMessage, MessageFlags.Broadcast));

            if (Comms.Host.State == CommunicationState.Closed) return;

            channelFactory.Close();
            Comms.Host.Close();
        }

        #endregion Private Methods
    }
}