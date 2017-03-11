using System.Text;
using System.Text.RegularExpressions;
using Castle.Core.Internal;
using JetBrains.Annotations;
using PostSharp.Patterns.Diagnostics;
using SecureIM.ChatBackend.helpers;
using SecureIM.ChatBackend.model;
using SecureIM.Smartcard.helpers;

namespace SecureIM.ChatBackend
{
    /// <summary>
    /// ChatCommandHandlerd
    /// </summary>
    internal static class ChatCommandHandler
    {
        #region Internal Methods

        /// <summary>
        /// Adds the friend.
        /// </summary>
        /// <param name="commandMatchGroups">The command match groups.</param>
        /// <param name="sendMessageDelegate">The send message delegate.</param>
        [Log("MyProf")]
        internal static void AddFriend([NotNull] GroupCollection commandMatchGroups, [NotNull] SendMessageDelegate sendMessageDelegate)
        {
            ChatBackend chatBackend = ChatBackend.Instance;

            string commandData = commandMatchGroups[2].Value;
            string[] commandDataSplit = commandData.Split(':');
            string alias = commandDataSplit[0];
            string pubKeyB64 = commandDataSplit[1];
            var newFriend = new User(alias, pubKeyB64);
            chatBackend.FriendsList.Add(newFriend);
            string confirmMessage = $"Friend ({alias}) added with public key: {pubKeyB64}";
            User messageSender = chatBackend.CurrentUser;

            sendMessageDelegate(chatBackend.EventUser, messageSender, confirmMessage, MessageFlags.Local);
        }

        /// <summary>
        /// Decodes the base64.
        /// </summary>
        /// <param name="commandMatchGroups">The command match groups.</param>
        /// <param name="sendMessageDelegate">The send message delegate.</param>
        [Log("MyProf")]
        internal static void DecodeBase64([NotNull] GroupCollection commandMatchGroups, [NotNull] SendMessageDelegate sendMessageDelegate)
        {
            ChatBackend chatBackend = ChatBackend.Instance;

            string cipherText = commandMatchGroups[2].Value;
            string commandData = Encoding.Default.DecodeBase64(cipherText);
            User messageSender = chatBackend.CurrentUser;

            if (!string.IsNullOrEmpty(commandData)) sendMessageDelegate(chatBackend.EventUser, messageSender, commandData);
        }

        /// <summary>
        /// Decrypts the specified command match groups.
        /// </summary>
        /// <param name="commandMatchGroups">The command match groups.</param>
        /// <param name="sendMessageDelegate">The send message delegate.</param>
        [Log("MyProf")]
        internal static void Decrypt([NotNull] GroupCollection commandMatchGroups, [NotNull] SendMessageDelegate sendMessageDelegate)
        {
            ChatBackend chatBackend = ChatBackend.Instance;

            string commandData = commandMatchGroups[2].Value;
            string[] commandDataSplit = commandData.Split(':');
            string alias = commandDataSplit[0];
            string cipherText = commandDataSplit[1];
            User targetUser = chatBackend.FriendsList.Find(x => x.Name.Equals(alias));
            string plainText = chatBackend.DecryptChatMessage(cipherText, BackendHelper.DecodeToByteArrayBase64(targetUser.PublicKey));
            plainText = Encoding.Default.EncodeBase64(plainText);
            User messageSender = chatBackend.CurrentUser;

            if (!string.IsNullOrEmpty(plainText)) sendMessageDelegate(messageSender, targetUser, plainText, MessageFlags.Encoded);
        }

        /// <summary>
        /// Encodes the base64.
        /// </summary>
        /// <param name="commandMatchGroups">The command match groups.</param>
        /// <param name="sendMessageDelegate">The send message delegate.</param>
        [Log("MyProf")]
        internal static void EncodeBase64([NotNull] GroupCollection commandMatchGroups, [NotNull] SendMessageDelegate sendMessageDelegate)
        {
            ChatBackend chatBackend = ChatBackend.Instance;

            string commandData = commandMatchGroups[2].Value;
            string cipherText = Encoding.Default.EncodeBase64(commandData);
            User messageSender = chatBackend.CurrentUser;

            if (!string.IsNullOrEmpty(cipherText)) sendMessageDelegate(chatBackend.EventUser, messageSender, cipherText, MessageFlags.Encoded);
        }

        /// <summary>
        /// Encrypts the specified command match groups.
        /// </summary>
        /// <param name="commandMatchGroups">The command match groups.</param>
        /// <param name="sendMessageDelegate">The send message delegate.</param>
        /// <returns></returns>
        [NotNull]
        [Log("MyProf")]
        internal static string Encrypt([NotNull] GroupCollection commandMatchGroups, [NotNull] SendMessageDelegate sendMessageDelegate)
        {
            ChatBackend chatBackend = ChatBackend.Instance;

            string commandData = commandMatchGroups[2].Value;
            string[] commandDataSplit = commandData.Split(':');
            string alias = commandDataSplit[0];
            string plainText = commandDataSplit[1];

            User messageSender = chatBackend.CurrentUser;
            User targetUser = chatBackend.FriendsList.Find(x => x.Name.Equals(alias));

            byte[] targetUserPubKeyBytes = BackendHelper.DecodeToByteArrayBase64(targetUser.PublicKey);
            string cipherText = chatBackend.EncryptChatMessage(plainText, targetUserPubKeyBytes);
            cipherText = Encoding.Default.EncodeBase64(cipherText);

            if (!string.IsNullOrEmpty(cipherText))
                sendMessageDelegate(messageSender, targetUser, cipherText, MessageFlags.Encoded | MessageFlags.Encrypted);
            return alias;
        }

        /// <summary>
        /// Generates the key pair.
        /// </summary>
        /// <param name="sendMessageDelegate">The send message delegate.</param>
        [Log("MyProf")]
        internal static void GenerateKeyPair([NotNull] SendMessageDelegate sendMessageDelegate)
        {
            ChatBackend chatBackend = ChatBackend.Instance;

            chatBackend.CryptoHandler.GenerateAsymmetricKeyPair();

            string pubKeyB64 = BackendHelper.EncodeFromByteArrayBase64(chatBackend.CryptoHandler.GetPublicKey());
            if (!string.IsNullOrEmpty(pubKeyB64))
            {
                string messageText = pubKeyB64;

                sendMessageDelegate(chatBackend.EventUser, chatBackend.CurrentUser, messageText, MessageFlags.Encoded | MessageFlags.Local);
            }
            //             TODO: Remove this
            //            string priKeyB64 = BackendHelper.EncodeFromByteArrayBase64(chatBackend.CryptoHandler.GetPrivateKey());
            //
            //            if (string.IsNullOrEmpty(priKeyB64)) return;
            //
            //            if (messageSender != null) sendMessageDelegate(messageSender, chatBackend.EventUser, priKeyB64, MessageFlags.Encoded);
        }

        /// <summary>
        /// Gets the public key.
        /// </summary>
        /// <param name="sendMessageDelegate">The send message delegate.</param>
        [Log("MyProf")]
        internal static void GetPublicKey([NotNull] SendMessageDelegate sendMessageDelegate)
        {
            ChatBackend chatBackend = ChatBackend.Instance;

            string pubKeyB64 = BackendHelper.EncodeFromByteArrayBase64(chatBackend.CryptoHandler.GetPublicKey());
            string messageText = pubKeyB64;

            if (!string.IsNullOrEmpty(pubKeyB64))
                sendMessageDelegate(chatBackend.EventUser, chatBackend.CurrentUser, messageText, MessageFlags.Encoded | MessageFlags.Local);
        }

        /// <summary>
        /// Plains the message send.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="sendMessageDelegate">The send message delegate.</param>
        internal static void PlainMessageSend([NotNull] string text, [NotNull] SendMessageDelegate sendMessageDelegate)
        {
            ChatBackend chatBackend = ChatBackend.Instance;

            User messageSender = chatBackend.CurrentUser;
            sendMessageDelegate(messageSender, messageSender, text);
        }

        /// <summary>
        /// Registers the public key.
        /// </summary>
        /// <param name="sendMessageDelegate">The send message delegate.</param>
        [Log("MyProf")]
        internal static void RegisterPublicKey([NotNull] SendMessageDelegate sendMessageDelegate)
        {
            ChatBackend chatBackend = ChatBackend.Instance;

            byte[] publicKey = chatBackend.CryptoHandler.GetPublicKey();
            if (!publicKey.IsNullOrEmpty())
            {
                string pubKeyB64 = BackendHelper.EncodeFromByteArrayBase64(publicKey);
                chatBackend.CurrentUser.PublicKey = pubKeyB64;
                chatBackend.FriendsList.Add(chatBackend.CurrentUser);
                string messageText = $"Registered: {pubKeyB64}";
                chatBackend.IsRegistered = true;

                sendMessageDelegate(chatBackend.EventUser, chatBackend.CurrentUser, messageText, MessageFlags.Encoded | MessageFlags.Local);
            }
            else
            {
                chatBackend.DisplayMessageDelegate?.Invoke(new MessageComposite(chatBackend.EventUser, chatBackend.CurrentUser,
                    "You must generate a key before you can register one", MessageFlags.Local));
            }
        }

        /// <summary>
        /// Sets the name.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="commandMatchString">The command match string.</param>
        /// <param name="sendMessageDelegate">The send message delegate.</param>
        [Log("MyProf")]
        internal static void SetName([NotNull] string text, [NotNull] string commandMatchString, [NotNull] SendMessageDelegate sendMessageDelegate)
        {
            ChatBackend chatBackend = ChatBackend.Instance;

            chatBackend.CurrentUser.Name = text.Substring(commandMatchString.Length).Trim();
            string messageText = $"Setting your name to {chatBackend.CurrentUser.Name}";

            sendMessageDelegate(chatBackend.EventUser, chatBackend.CurrentUser, messageText, MessageFlags.Local);
        }

        #endregion Internal Methods
    }
}