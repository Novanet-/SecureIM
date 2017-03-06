using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using JetBrains.Annotations;
using PostSharp.Patterns.Diagnostics;
using SecureIM.ChatBackend.helpers;
using SecureIM.ChatBackend.model;
using SecureIM.Smartcard.controller.smartcard;
using SecureIM.Smartcard.helpers;
using SecureIM.Smartcard.model.abstractions;

namespace SecureIM.ChatBackend
{
    /// <summary>
    /// ChatBackend
    /// </summary>
    /// <seealso cref="IChatBackend" />
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
        public User EventUser { get; } = new User("Event", "event");
        public List<User> FriendsList { get; }
        public User InfoUser { get; } = new User("Info", "info");
        [NotNull] public static ChatBackend Instance => Lazy.Value;
        public bool IsRegistered { get; set; }
        public ProcessMessageDelegate ProcessMessageDelegate { get; set; }
        public SendMessageDelegate SendMessageDelegate { get; }
        public bool ServiceStarted { get; set; }

        #endregion Public Properties


        #region Private Properties

        private bool IsCurrentPubKeyInFriendsList => FriendsList.Where(x =>
                                                                       {
                                                                           string currentPubKeyB64 =
                                                                                   BackendHelper.EncodeFromByteArrayBase64(
                                                                                                                           CryptoHandler.GetPublicKey());
                                                                           return x.PublicKey.Equals(currentPubKeyB64);
                                                                       }).FirstOrDefault() != null;

        #endregion Private Properties


        #region Private Constructors

        /// <summary>
        /// Prevents a default instance of the <see cref="ChatBackend"/> class from being created.
        /// </summary>
        [Log("MyProf")]
        private ChatBackend()
        {
            CurrentUser = new User();
            CryptoHandler = new SmartcardCryptoHandler();
            FriendsList = new List<User>();
            ChatCommandHandler = new ChatCommandHandler();
            SendMessageDelegate = SendMessageToChannel;
            IsRegistered = IsCurrentPubKeyInFriendsList;
        }

        #endregion Private Constructors


        #region Public Methods

        /// <summary>
        /// The default constructor is only here for testing purposes.
        /// </summary>
        /// <param name="messageComposite">The messageComposite.</param>
        /// <exception cref="System.ArgumentNullException">messageComposite</exception>
        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        [Log("MyProf")]
        public void DisplayMessage([NotNull] MessageComposite messageComposite)
        {
            if (messageComposite == null) throw new ArgumentNullException(nameof(messageComposite));

            string currentPubKeyB64 = BackendHelper.EncodeFromByteArrayBase64(CryptoHandler.GetPublicKey());
            bool isEventSender = messageComposite.Sender.PublicKey.Equals(EventUser.PublicKey) ||
                                 messageComposite.Sender.PublicKey.Equals(InfoUser.PublicKey);
            bool isReceiverCurrentUser = !string.IsNullOrEmpty(messageComposite.Receiver.PublicKey) &&
                                         messageComposite.Receiver.PublicKey.Equals(currentPubKeyB64);
            if (isEventSender || isReceiverCurrentUser)
            {
                bool isEncodedMessage = messageComposite.Flags.HasFlag(MessageFlags.Encoded);
                bool isEncryptedMessage = messageComposite.Flags.HasFlag(MessageFlags.Encrypted);

                if (isEncodedMessage && isEncryptedMessage) messageComposite = DecodeMessage(messageComposite);

                if (DisplayMessageDelegate != null) ProcessMessageDelegate.Invoke(messageComposite, DisplayMessageDelegate);
            }
        }

        /// <summary>
        /// The front-end calls the SendMessage method in order to broadcast a message to our friends
        /// </summary>
        /// <param name="text">The text.</param>
        /// <exception cref="System.ArgumentNullException">text</exception>
        /// <exception cref="RegexMatchTimeoutException">A time-out occurred. For more information about time-outs, see the Remarks section.</exception>
        /// <exception cref="ArgumentException">A regular expression parsing error occurred.</exception>
        [Log("MyProf")]
        public void SendMessage([NotNull] string text)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));

            var commandRegEx = new Regex(@"^([\w]+:)\s*(.*)", RegexOptions.Multiline);
            Match commandMatch = commandRegEx.Match(text);
            GroupCollection commandMatchGroups = commandMatch.Groups;
            string commandMatchString = commandMatch.Success ? commandMatchGroups[1].Value.ToLower() : string.Empty;

            if (!IsRegistered) BaseCommandSwitch(text, commandMatchString);
            else RegisteredCommandSwitch(text, commandMatchString, commandMatchGroups);
        }

        /// <summary>
        /// Starts the service.
        /// </summary>
        [Log("MyProf")]
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
            DisplayMessage(new MessageComposite(EventUser, CurrentUser, userJoinedMessage, MessageFlags.Broadcast));

            // Information to display locally
            const string changeNamePrompt = "To change your name, type setname: NEW_NAME";
            DisplayMessage(new MessageComposite(InfoUser, CurrentUser, changeNamePrompt, MessageFlags.Broadcast));
        }

        #endregion Public Methods


        #region Internal Methods

        /// <summary>
        /// Decrypts the chat message.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="targetPubKey">The target pub key.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// text
        /// or
        /// targetPubKey
        /// </exception>
        /// <exception cref="ArgumentNullException"><paramref name="text" /> is <see langword="null" /></exception>
        /// <exception cref="ArgumentNullException"><paramref name="text" /> is <see langword="null" /></exception>
        [NotNull]
        [Log("MyProf")]
        internal string DecryptChatMessage([NotNull] string text, [NotNull] byte[] targetPubKey)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));
            if (targetPubKey == null) throw new ArgumentNullException(nameof(targetPubKey));

            // TODO: Proper pub key
            //            targetPubKey = CryptoHandler.GetPublicKey();
            string plainText = CryptoHandler.Decrypt(text, targetPubKey);
            return plainText;
        }

        /// <summary>
        /// Encrypts the chat message.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="targetPubKey">The target pub key.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// text
        /// or
        /// targetPubKey
        /// </exception>
        /// <exception cref="ArgumentNullException"><paramref name="text" /> is <see langword="null" /></exception>
        /// <exception cref="ArgumentNullException"><paramref name="text" /> is <see langword="null" /></exception>
        [NotNull]
        [Log("MyProf")]
        internal string EncryptChatMessage([NotNull] string text, [NotNull] byte[] targetPubKey)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));
            if (targetPubKey == null) throw new ArgumentNullException(nameof(targetPubKey));

            // TODO: Proper pub key
            //            targetPubKey = CryptoHandler.GetPublicKey();
            string cipherText = CryptoHandler.Encrypt(text, targetPubKey);
            return cipherText;
        }

        #endregion Internal Methods


        #region Private Methods

        /// <summary>
        /// Bases the command switch.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="commandMatchString">The command match string.</param>
        /// <exception cref="System.ArgumentNullException">
        /// text
        /// or
        /// commandMatchString
        /// </exception>
        [Log("MyProf")]
        private void BaseCommandSwitch([NotNull] string text, [NotNull] string commandMatchString)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));
            if (commandMatchString == null) throw new ArgumentNullException(nameof(commandMatchString));

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

                default:
                    SendMessageToChannel(EventUser, CurrentUser, "You must register before you can use this command");
                    break;
            }
        }

        /// <summary>
        /// Decodes the message.
        /// </summary>
        /// <param name="messageComposite">The message composite.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">messageComposite</exception>
        [Log("MyProf")]
        private MessageComposite DecodeMessage([NotNull] MessageComposite messageComposite)
        {
            if (messageComposite == null) throw new ArgumentNullException(nameof(messageComposite));

            string decodedMessageText = Encoding.Default.DecodeBase64(messageComposite.Message.Text);

            if (decodedMessageText != null) messageComposite = DecryptMessage(messageComposite, decodedMessageText);
            return messageComposite;
        }

        /// <summary>
        /// Decrypts the message.
        /// </summary>
        /// <param name="messageComposite">The message composite.</param>
        /// <param name="decodedMessageText">The decoded message text.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// messageComposite
        /// or
        /// decodedMessageText
        /// </exception>
        [Log("MyProf")]
        private MessageComposite DecryptMessage([NotNull] MessageComposite messageComposite, [NotNull] string decodedMessageText)
        {
            if (messageComposite == null) throw new ArgumentNullException(nameof(messageComposite));
            if (decodedMessageText == null) throw new ArgumentNullException(nameof(decodedMessageText));

            byte[] currentPubKey = CryptoHandler.GetPublicKey();
            byte[] targetPubKeyBytes = null;
            byte[] senderPubKeyBytes = BackendHelper.DecodeToByteArrayBase64(messageComposite.Sender.PublicKey);
            byte[] receiverPubKeyBytes = BackendHelper.DecodeToByteArrayBase64(messageComposite.Receiver.PublicKey);

            if (currentPubKey.SequenceEqual(senderPubKeyBytes)) targetPubKeyBytes = receiverPubKeyBytes;
            else if (currentPubKey.SequenceEqual(receiverPubKeyBytes)) targetPubKeyBytes = senderPubKeyBytes;
            if (targetPubKeyBytes != null)
            {
                string plainText = CryptoHandler.Decrypt(decodedMessageText, targetPubKeyBytes);
                messageComposite = new MessageComposite(messageComposite.Sender, messageComposite.Receiver, plainText, messageComposite.Flags);
            }
            return messageComposite;
        }

        /// <summary>
        /// Registereds the command switch.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="commandMatchString">The command match string.</param>
        /// <param name="commandMatchGroups">The command match groups.</param>
        /// <exception cref="System.ArgumentNullException">
        /// text
        /// or
        /// commandMatchString
        /// or
        /// commandMatchGroups
        /// </exception>
        [Log("MyProf")]
        private void RegisteredCommandSwitch([NotNull] string text, [NotNull] string commandMatchString, [NotNull] GroupCollection commandMatchGroups)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));
            if (commandMatchString == null) throw new ArgumentNullException(nameof(commandMatchString));
            if (commandMatchGroups == null) throw new ArgumentNullException(nameof(commandMatchGroups));

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
        /// Sends the message to channel.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="receiver">The receiver.</param>
        /// <param name="messageText">The message text.</param>
        /// <param name="messageFlags">The message flags.</param>
        /// <exception cref="System.ArgumentNullException">
        /// sender
        /// or
        /// receiver
        /// or
        /// messageText
        /// </exception>
        [Log("MyProf")]
        private void SendMessageToChannel([NotNull] User sender, [NotNull] User receiver, [NotNull] string messageText,
                                          MessageFlags messageFlags = MessageFlags.None)
        {
            if (sender == null) throw new ArgumentNullException(nameof(sender));
            if (receiver == null) throw new ArgumentNullException(nameof(receiver));
            if (messageText == null) throw new ArgumentNullException(nameof(messageText));

            Task.Run(() =>
                     {
                         IChatBackend messageDestination = messageFlags.HasFlag(MessageFlags.Local)
                                                               ? this
                                                               : new ChannelFactory<IChatBackend>("ChatEndpoint").CreateChannel();
                         var messageComposite = new MessageComposite(sender, receiver, messageText, messageFlags);
                         messageDestination.DisplayMessage(messageComposite);
                     });

            //            var messageComposite = new MessageComposite(sender, receiver, messageText, messageFlags);
            //            new ChannelFactory<IChatBackend>("ChatEndpoint").CreateChannel().DisplayMessage(messageComposite);
        }

        /// <summary>
        /// Stops the service.
        /// </summary>
        [Log("MyProf")]
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