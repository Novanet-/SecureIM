﻿using System;
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
using static System.String;

namespace SecureIM.ChatBackend
{
    /// <summary>
    /// THe main class that functions as the backend for the chat system, controls methods for
    /// sending and displaying messages, as well as dispatching command messages to th5e ChatCommandHandler
    /// </summary>
    /// <seealso cref="IChatBackend"/>
    ///
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public sealed class ChatBackend : IChatBackend
    {
        #region Public Properties

        [NotNull] public static ChatBackend Instance => Lazy.Value;
        [NotNull] public ICryptoHandler CryptoHandler { get; }
        [NotNull] public User CurrentUser { get; }

        [CanBeNull] public DisplayMessageDelegate DisplayMessageDelegate { get; set; }

        public User EventUser { get; } = new User("Event", "event");
        public List<User> FriendsList { get; }
        public bool IsRegistered { get; internal set; }
        public ProcessMessageDelegate ProcessMessageDelegate { private get; set; }
        public bool ServiceStarted { get; private set; }

        #endregion Public Properties

        #region Private Properties

        [ItemNotNull] private static Lazy<ChatBackend> Lazy { get; } = new Lazy<ChatBackend>(() => new ChatBackend());
        private User BroadcastUser { get; } = new User("Broadcast");
        private Comms Comms { get; set; }
        private User InfoUser { get; } = new User("Info", "info");

        private bool IsCurrentPubKeyInFriendsList => FriendsList.Where(x =>
        {
            string currentPubKeyB64 =
                BackendHelper.EncodeFromByteArrayBase64(
                    CryptoHandler.GetPublicKey());
            return x.PublicKey.Equals(currentPubKeyB64);
        }).FirstOrDefault() != null;

        private SendMessageDelegate SendMessageDelegate { get; }

        #endregion Private Properties

        #region Private Constructors

        /// <summary>
        /// Constructor for ChatBackend invoked via lazy initialization
        /// </summary>
        [Log("MyProf")]
        private ChatBackend()
        {
            CurrentUser = new User();
            CryptoHandler = new SmartcardCryptoHandler();
            FriendsList = new List<User>();
            SendMessageDelegate = SendMessageToChannel;
            IsRegistered = IsCurrentPubKeyInFriendsList;
        }

        #endregion Private Constructors

        #region Public Methods

        /// <summary>
        /// Displays a MessageComposite to a user, user is validated against intended message
        /// receiver, or the sender being the event user. <br/> The flags of the message are checked,
        /// and the message is decoded and decrypted. <br/> The display message delegate is then
        /// invoked, which contains logic for rendering the message in the user's view.
        /// </summary>
        /// <param name="messageComposite">
        /// The message to be displayed, contained withina MessageComposite
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// messageComposite
        /// </exception>
        /// <exception cref="Exception">
        /// A delegate callback throws an exception.
        /// </exception>
        [Log("MyProf")]
        public void DisplayMessage([NotNull] MessageComposite messageComposite)
        {
            if (messageComposite == null) throw new ArgumentNullException(nameof(messageComposite));

            string currentPubKeyB64 = BackendHelper.EncodeFromByteArrayBase64(CryptoHandler.GetPublicKey());

            bool isValidRecipient = currentPubKeyB64 != null && IsValidRecipient(messageComposite, currentPubKeyB64);

            if (messageComposite.Flags.HasFlag(MessageFlags.Local))
                DisplayMessageDelegate?.Invoke(messageComposite);
            else if (IsEncodedEncrypted(messageComposite))
                messageComposite = DecodeandDecryptMessage(messageComposite);

            if (DisplayMessageDelegate == null || !isValidRecipient) return;

            User targetUserForTabDisplay = null;

            if (IsSenderEventOrInfoUser(messageComposite))
                targetUserForTabDisplay = EventUser;
            else if (IsSenderCurrentUser(messageComposite, currentPubKeyB64))
                targetUserForTabDisplay = messageComposite.Receiver;
            else if (IsReceiverCurrentUser(messageComposite, currentPubKeyB64))
                targetUserForTabDisplay = messageComposite.Sender;

            if (targetUserForTabDisplay == null) return;

            if (DisplayMessageDelegate != null) ProcessMessageDelegate.Invoke(messageComposite, DisplayMessageDelegate, targetUserForTabDisplay);
        }

        /// <summary>
        /// The front-end calls the SendMessage method in order to broadcast a message to our
        /// friends. This will either be a command message, in which case it is dispatched to the
        /// ChatCommandHandler, or a normal message, where it will be handled by a SendMessageDelegate
        /// </summary>
        /// <param name="text">
        /// The text of the message to be sent
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// text
        /// </exception>
        /// <exception cref="RegexMatchTimeoutException">
        /// A time-out occurred. For more information about time-outs, see the Remarks section.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// A regular expression parsing error occurred.
        /// </exception>
        [Log("MyProf")]
        public void SendMessage([NotNull] string text)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));

            var commandRegEx = new Regex(@"^([\w]+:)\s*(.*)", RegexOptions.Multiline); //Matches against command messages ( "cmd:data" )
            Match commandMatch = commandRegEx.Match(text);
            GroupCollection commandMatchGroups = commandMatch.Groups;
            string commandMatchString = commandMatch.Success ? commandMatchGroups[1].Value.ToLower() : Empty;

            if (!IsRegistered) BaseCommandSwitch(text, commandMatchString); //Handles commands that any user can access
            else RegisteredCommandSwitch(text, commandMatchString, commandMatchGroups); //Handles commands that require a registered public key
        }

        /// <summary>
        /// Starts the WCF service. Displays messages to the channel indicating name change options and the fact that the user has entered the channel
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

            string userJoinedMessage = $"{CurrentUser.Name} has entered the conversation.";
            DisplayMessage(new MessageComposite(EventUser, CurrentUser, userJoinedMessage, MessageFlags.Broadcast));

            const string changeNamePrompt = "To change your name, type setname: NEW_NAME";
            DisplayMessage(new MessageComposite(InfoUser, CurrentUser, changeNamePrompt, MessageFlags.Broadcast));
        }

        #endregion Public Methods

        #region Internal Methods

        /// <summary>
        /// Decrypts a chat message using the CryptoHandler created in the ChatBackend constructor.
        /// </summary>
        /// <param name="text">
        /// The text to decrypt
        /// </param>
        /// <param name="targetPubKey">
        /// The public key to use in the decryption process
        /// </param>
        /// <returns>
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// text or targetPubKey
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="text"/> is <see langword="null"/>
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="text"/> is <see langword="null"/>
        /// </exception>
        [NotNull]
        [Log("MyProf")]
        internal string DecryptChatMessage([NotNull] string text, [NotNull] byte[] targetPubKey)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));
            if (targetPubKey == null) throw new ArgumentNullException(nameof(targetPubKey));

            // TODO: Proper pub key targetPubKey = CryptoHandler.GetPublicKey();
            string plainText = CryptoHandler.Decrypt(text, targetPubKey);
            return plainText;
        }

        /// <summary>
        /// Decrypts a chat message using the CryptoHandler created in the ChatBackend constructor.
        /// </summary>
        /// <param name="text">
        /// The text to encrypt
        /// </param>
        /// <param name="targetPubKey">
        /// Encrypts a chat message using the CryptoHandler created in the ChatBackend constructor.
        /// </param>
        /// <returns>
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// text or targetPubKey
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="text"/> is <see langword="null"/>
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="text"/> is <see langword="null"/>
        /// </exception>
        [NotNull]
        [Log("MyProf")]
        internal string EncryptChatMessage([NotNull] string text, [NotNull] byte[] targetPubKey)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));
            if (targetPubKey == null) throw new ArgumentNullException(nameof(targetPubKey));

            // TODO: Proper pub key targetPubKey = CryptoHandler.GetPublicKey();
            string cipherText = CryptoHandler.Encrypt(text, targetPubKey);
            return cipherText;
        }

        #endregion Internal Methods

        #region Private Methods

        private static bool IsEncodedEncrypted([NotNull] MessageComposite messageComposite)
        {
            bool isEncodedMessage = messageComposite.Flags.HasFlag(MessageFlags.Encoded);
            bool isEncryptedMessage = messageComposite.Flags.HasFlag(MessageFlags.Encrypted);

            bool isEncodedEncrypted = isEncodedMessage && isEncryptedMessage;
            return isEncodedEncrypted;
        }

        private static bool IsReceiverCurrentUser([NotNull] MessageComposite messageComposite, [NotNull] string currentPubKeyB64)
        {
            var isReceiverCurrentUser = false;
            if (!IsNullOrEmpty(messageComposite.Receiver.PublicKey))
                isReceiverCurrentUser = messageComposite.Receiver.PublicKey.Equals(currentPubKeyB64);
            return isReceiverCurrentUser;
        }

        private static bool IsSenderCurrentUser([NotNull] MessageComposite messageComposite, [NotNull] string currentPubKeyB64)
        {
            var isSenderCurrentUser = false;
            if (!IsNullOrEmpty(messageComposite.Sender.PublicKey))
                isSenderCurrentUser = messageComposite.Sender.PublicKey.Equals(currentPubKeyB64);
            return isSenderCurrentUser;
        }

        /// <summary>
        /// Checks which command has been given in the command message, only matches commands that unregistered users can access
        /// </summary>
        /// <param name="text">
        /// The full command message
        /// </param>
        /// <param name="commandMatchString">
        /// String containing command message data
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// text or commandMatchString
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
                    SendMessageToChannel(EventUser, CurrentUser, "You must register before you can use this command", MessageFlags.Local);
                    break;
            }
        }

        /// <summary>
        /// Decodes the message.
        /// </summary>
        /// <param name="messageComposite">
        /// The message composite.
        /// </param>
        /// <returns>
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// messageComposite
        /// </exception>
        [NotNull]
        [Log("MyProf")]
        private MessageComposite DecodeandDecryptMessage([NotNull] MessageComposite messageComposite)
        {
            if (messageComposite == null) throw new ArgumentNullException(nameof(messageComposite));

            string decodedMessageText = Encoding.Default.DecodeBase64(messageComposite.Message.Text);

            if (decodedMessageText != null) messageComposite = DecryptMessage(messageComposite, decodedMessageText);
            return messageComposite;
        }

        /// <summary>
        /// Decrypts the message.
        /// </summary>
        /// <param name="messageComposite">
        /// The message composite.
        /// </param>
        /// <param name="decodedMessageText">
        /// The decoded message text.
        /// </param>
        /// <returns>
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// messageComposite or decodedMessageText
        /// </exception>
        [NotNull]
        [Log("MyProf")]
        private MessageComposite DecryptMessage([NotNull] MessageComposite messageComposite, [NotNull] string decodedMessageText)
        {
            if (messageComposite == null) throw new ArgumentNullException(nameof(messageComposite));
            if (decodedMessageText == null) throw new ArgumentNullException(nameof(decodedMessageText));

            byte[] currentPubKey = CryptoHandler.GetPublicKey();
            byte[] targetPubKeyBytes = null;
            byte[] senderPubKeyBytes = BackendHelper.DecodeToByteArrayBase64(messageComposite.Sender.PublicKey);
            byte[] receiverPubKeyBytes = BackendHelper.DecodeToByteArrayBase64(messageComposite.Receiver.PublicKey);

            if (senderPubKeyBytes != null && currentPubKey.SequenceEqual(senderPubKeyBytes))
            {
                targetPubKeyBytes = receiverPubKeyBytes;
            }
            else if (receiverPubKeyBytes != null && currentPubKey.SequenceEqual(receiverPubKeyBytes))
            {
                targetPubKeyBytes = senderPubKeyBytes;
            }

            if (targetPubKeyBytes == null) return messageComposite;

            string plainText = CryptoHandler.Decrypt(decodedMessageText, targetPubKeyBytes);
            messageComposite = new MessageComposite(messageComposite.Sender, messageComposite.Receiver, plainText, messageComposite.Flags);
            return messageComposite;
        }

        private bool IsSenderEventOrInfoUser([NotNull] MessageComposite messageComposite)
            => messageComposite.Sender.PublicKey.Equals(EventUser.PublicKey) || messageComposite.Sender.PublicKey.Equals(InfoUser.PublicKey);

        private bool IsValidRecipient([NotNull] MessageComposite messageComposite, [NotNull] string currentPubKeyB64)
        {
            bool isValidRecipient;

            bool isSenderEventOrInfoUser = IsSenderEventOrInfoUser(messageComposite);

            bool isSenderCurrentUser = IsSenderCurrentUser(messageComposite, currentPubKeyB64);
            bool isReceiverCurrentUser = IsReceiverCurrentUser(messageComposite, currentPubKeyB64);

            if (messageComposite.Flags.HasFlag(MessageFlags.Local))
                isValidRecipient = isSenderEventOrInfoUser && isReceiverCurrentUser;
            else
                isValidRecipient = messageComposite.Flags.HasFlag(MessageFlags.Broadcast)
                    ? isSenderEventOrInfoUser
                    : isSenderCurrentUser || isReceiverCurrentUser;

            return isValidRecipient;
        }

        /// <summary>
        /// Checks which command has been given in the command message, allows matching to commands that require a registered public key
        ///
        /// </summary>
        /// <param name="text">
        /// The full command message text.
        /// </param>
        /// <param name="commandMatchString">
        /// Command message data
        /// </param>
        /// <param name="commandMatchGroups">
        /// The command match groups.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// text or commandMatchString or commandMatchGroups
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
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="receiver">
        /// The receiver.
        /// </param>
        /// <param name="messageText">
        /// The message text.
        /// </param>
        /// <param name="messageFlags">
        /// The message flags.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// sender or receiver or messageText
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

            // var messageComposite = new MessageComposite(sender, receiver, messageText,
            // messageFlags); new ChannelFactory<IChatBackend>("ChatEndpoint").CreateChannel().DisplayMessage(messageComposite);
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