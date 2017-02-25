using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using JetBrains.Annotations;
using SecureIM.ChatBackend.helpers;
using SecureIM.ChatBackend.model;
using SecureIM.Smartcard.controller.smartcard;
using SecureIM.Smartcard.helpers;
using SecureIM.Smartcard.model.abstractions;

namespace SecureIM.ChatBackend
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public sealed class ChatBackend : IChatBackend
    {
        #region Public Properties

        [ItemNotNull] public static Lazy<ChatBackend> Lazy { get; } = new Lazy<ChatBackend>(() => new ChatBackend());

        public User BroadcastUser { get; } = new User("Broadcast");
        public ChatCommandHandler ChatCommandHandler { get; }
        public Comms Comms { get; private set; }

        [NotNull] public ICryptoHandler CryptoHandler { get; }
        [NotNull] public User CurrentUser { get; }
        [CanBeNull] public DisplayMessageDelegate DisplayMessageDelegate { get; set; }

        public User EventUser { get; } = new User("Event");
        public List<User> FriendsList { get; }
        public User InfoUser { get; } = new User("Info");

        [NotNull] public static ChatBackend Instance => Lazy.Value;

        public SendMessageDelegate SendMessageDelegate { get; }

        public bool ServiceStarted { get; set; }

        #endregion Public Properties

        #region Private Constructors

        private ChatBackend()
        {
            CurrentUser = new User();
            CryptoHandler = new SmartcardCryptoHandler();
            FriendsList = new List<User>();
            ChatCommandHandler = new ChatCommandHandler();
            SendMessageDelegate = SendMessageToChannel;
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
            if (messageComposite == null) throw new ArgumentNullException(nameof(messageComposite));

            if (messageComposite.Flags.HasFlag(MessageFlags.Encoded)
                && messageComposite.Flags.HasFlag(MessageFlags.Encrypted))
            {
                string decodedMessageText = Encoding.Default.DecodeBase64(messageComposite.Message.Text);

                if (decodedMessageText != null)
                {
                    byte[] currentPubKey = CryptoHandler.GetPublicKey();
                    byte[] targetPubKeyBytes = null;
                    byte[] senderPubKeyBytes = BackendHelper.DecodeToByteArrayBase64(messageComposite.Sender.PublicKey);
                    byte[] receiverPubKeyBytes = BackendHelper.DecodeToByteArrayBase64(messageComposite.Receiver.PublicKey);

                    if (currentPubKey.SequenceEqual(senderPubKeyBytes))
                        targetPubKeyBytes = receiverPubKeyBytes;
                    else if (currentPubKey.SequenceEqual(receiverPubKeyBytes))
                        targetPubKeyBytes = senderPubKeyBytes;
                    if (targetPubKeyBytes != null)
                    {
                        string plainText = CryptoHandler.Decrypt(decodedMessageText, targetPubKeyBytes);
                        messageComposite = new MessageComposite(messageComposite.Sender, messageComposite.Receiver, plainText, messageComposite.Flags);
                    }
                }
            }

            if (DisplayMessageDelegate != null) DisplayMessageDelegate?.Invoke(messageComposite);
            else
                throw new IMException(IMException.DisplayMessageDelegateError);
        }

        /// <summary>
        ///     The front-end calls the SendMessage method in order to broadcast a message to our friends
        /// </summary>
        /// <param name="text">The text.</param>
        /// <exception cref="RegexMatchTimeoutException">A time-out occurred. For more information about time-outs, see the Remarks section.</exception>
        /// <exception cref="ArgumentException">A regular expression parsing error occurred. </exception>
        public void SendMessage(string text)
        {
            var commandRegEx = new Regex(@"^([\w]+:)\s*(.*)", RegexOptions.Multiline);
            Match commandMatch = commandRegEx.Match(text);
            GroupCollection commandMatchGroups = commandMatch.Groups;
            string commandMatchString = commandMatch.Success ? commandMatchGroups[1].Value.ToLower() : string.Empty;

            switch (commandMatchString)
            {
                case "setname:":
                    ChatCommandHandler.SetName(text, commandMatchString, SendMessageDelegate);
                    break;

                case "genkey:":
                    ChatCommandHandler.GenerateKeyPair(SendMessageDelegate);
                    break;

                case "getpub:":
                    ChatCommandHandler.GetPublicKey(SendMessageDelegate);
                    break;

                case "regpub:":
                    ChatCommandHandler.RegisterPublicKey(SendMessageDelegate);
                    break;

                case "encrypt:":
                    ChatCommandHandler.Encrypt(commandMatchGroups, SendMessageDelegate);
                    break;

                case "decrypt:":
                    ChatCommandHandler.Decrypt(commandMatchGroups, SendMessageDelegate);
                    break;

                case "db64:":
                    ChatCommandHandler.DecodeBase64(commandMatchGroups, SendMessageDelegate);
                    break;

                case "eb64:":
                    ChatCommandHandler.EncodeBase64(commandMatchGroups, SendMessageDelegate);
                    break;

                case "addfriend:":
                    ChatCommandHandler.AddFriend(commandMatchGroups, SendMessageDelegate);
                    break;

                default:
                    ChatCommandHandler.PlainMessageSend(text, SendMessageDelegate);
                    break;
            }
        }

        /// <summary>
        ///     Starts the service.
        /// </summary>
        public void StartService()
        {
            var channelFactory = new ChannelFactory<IChatBackend>("ChatEndpoint");
            IChatBackend channel = channelFactory.CreateChannel();
            var serviceHost = new ServiceHost(this);

            Comms = new Comms(channelFactory, channel, serviceHost);
            Comms.Host.Open();

            ServiceStarted = true;

            // Information to send to the channel
            string userJoinedMessage = $"{CurrentUser.Name} has entered the conversation.";
            channel.DisplayMessage(new MessageComposite(EventUser, BroadcastUser, userJoinedMessage, MessageFlags.Broadcast));

            // Information to display locally
            const string changeNamePrompt = "To change your name, type setname: NEW_NAME";
            DisplayMessageDelegate?.Invoke(new MessageComposite(InfoUser, CurrentUser, changeNamePrompt, MessageFlags.Broadcast));
        }

        #endregion Public Methods

        #region Internal Methods

        [NotNull]
        internal string DecryptChatMessage([NotNull] string text, byte[] targetPubKey)
        {
            // TODO: Proper pub key
            //            targetPubKey = CryptoHandler.GetPublicKey();
            string plainText = CryptoHandler.Decrypt(text, targetPubKey);
            return plainText;
        }

        [NotNull]
        internal string EncryptChatMessage([NotNull] string text, byte[] targetPubKey)
        {
            // TODO: Proper pub key
            //            targetPubKey = CryptoHandler.GetPublicKey();
            string cipherText = CryptoHandler.Encrypt(text, targetPubKey);
            return cipherText;
        }

        #endregion Internal Methods

        #region Private Methods

        private void SendMessageToChannel([NotNull] User sender, [NotNull] User receiver, [NotNull] string messageText,
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