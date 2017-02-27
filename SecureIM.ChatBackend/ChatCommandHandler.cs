using System.Text;
using System.Text.RegularExpressions;
using SecureIM.ChatBackend.helpers;
using SecureIM.ChatBackend.model;
using SecureIM.Smartcard.helpers;

namespace SecureIM.ChatBackend
{
    public sealed class ChatCommandHandler
    {
        #region Public Properties

        #endregion Public Properties

        #region Public Constructors

        #endregion Public Constructors

        #region Public Methods

        public void PlainMessageSend(string text, SendMessageDelegate sendMessageDelegate)
        {
            ChatBackend chatBackend = ChatBackend.Instance;

            User messageSender = chatBackend.CurrentUser;
            sendMessageDelegate(messageSender, messageSender, text);
        }

        #endregion Public Methods

        #region Internal Methods

        internal void AddFriend(GroupCollection commandMatchGroups, SendMessageDelegate sendMessageDelegate)
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
            sendMessageDelegate(messageSender, chatBackend.EventUser, confirmMessage);
        }

        internal void DecodeBase64(GroupCollection commandMatchGroups, SendMessageDelegate sendMessageDelegate)
        {
            ChatBackend chatBackend = ChatBackend.Instance;

            string cipherText = commandMatchGroups[2].Value;
            string commandData = Encoding.Default.DecodeBase64(cipherText);
            User messageSender = chatBackend.CurrentUser;
            if (!string.IsNullOrEmpty(commandData)) sendMessageDelegate(messageSender, chatBackend.EventUser, commandData);
        }

        internal void Decrypt(GroupCollection commandMatchGroups,  SendMessageDelegate sendMessageDelegate)
        {
            ChatBackend chatBackend = ChatBackend.Instance;

            string commandData = commandMatchGroups[2].Value;
            string[] commandDataSplit = commandData.Split(':');
            var alias = commandDataSplit[0];
            string cipherText = commandDataSplit[1];
            User targetUser = chatBackend.FriendsList.Find(x => x.Name.Equals(alias));
            string plainText = chatBackend.DecryptChatMessage(cipherText, BackendHelper.DecodeToByteArrayBase64(targetUser.PublicKey));
            plainText = Encoding.Default.EncodeBase64(plainText);
            User messageSender = chatBackend.CurrentUser;
            if (!string.IsNullOrEmpty(plainText)) sendMessageDelegate(messageSender, targetUser, plainText, MessageFlags.Encoded);
        }

        internal void EncodeBase64(GroupCollection commandMatchGroups, SendMessageDelegate sendMessageDelegate)
        {
            ChatBackend chatBackend = ChatBackend.Instance;

            string commandData = commandMatchGroups[2].Value;
            string cipherText = Encoding.Default.EncodeBase64(commandData);
            User messageSender = chatBackend.CurrentUser;
            if (!string.IsNullOrEmpty(cipherText)) sendMessageDelegate(messageSender, chatBackend.EventUser, cipherText, MessageFlags.Encoded);
        }

        internal string Encrypt(GroupCollection commandMatchGroups, SendMessageDelegate sendMessageDelegate)
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

        internal void GenerateKeyPair(SendMessageDelegate sendMessageDelegate)
        {
            ChatBackend chatBackend = ChatBackend.Instance;

            chatBackend.CryptoHandler.GenerateAsymmetricKeyPair();

            string pubKeyB64 = BackendHelper.EncodeFromByteArrayBase64(chatBackend.CryptoHandler.GetPublicKey());
            User messageSender = null;
            if (!string.IsNullOrEmpty(pubKeyB64))
            {
                messageSender = chatBackend.CurrentUser;
                string messageText = pubKeyB64;
                sendMessageDelegate(messageSender, messageSender, messageText, MessageFlags.Encoded);
            }

            string priKeyB64 = BackendHelper.EncodeFromByteArrayBase64(chatBackend.CryptoHandler.GetPrivateKey());

            if (string.IsNullOrEmpty(priKeyB64)) return;

            if (messageSender != null) sendMessageDelegate(messageSender, chatBackend.EventUser, priKeyB64, MessageFlags.Encoded);
        }

        internal void GetPublicKey( SendMessageDelegate sendMessageDelegate)
        {
            ChatBackend chatBackend = ChatBackend.Instance;

            string pubKeyB64 = BackendHelper.EncodeFromByteArrayBase64(chatBackend.CryptoHandler.GetPublicKey());
            User messageSender = chatBackend.CurrentUser;
            string messageText = pubKeyB64;
            if (!string.IsNullOrEmpty(pubKeyB64)) sendMessageDelegate(messageSender, chatBackend.EventUser, messageText, MessageFlags.Encoded);
        }

        internal void RegisterPublicKey(SendMessageDelegate sendMessageDelegate)
        {
            ChatBackend chatBackend = ChatBackend.Instance;

            string pubKeyB64 = BackendHelper.EncodeFromByteArrayBase64(chatBackend.CryptoHandler.GetPublicKey());
            chatBackend.CurrentUser.PublicKey = pubKeyB64;
            chatBackend.FriendsList.Add(chatBackend.CurrentUser);
            User messageSender = chatBackend.CurrentUser;
            string messageText = $"Registered: {pubKeyB64}";
            sendMessageDelegate(messageSender, chatBackend.EventUser, messageText, MessageFlags.Encoded);
        }

        internal void SetName(string text, string commandMatchString, SendMessageDelegate sendMessageDelegate)
        {
            ChatBackend chatBackend = ChatBackend.Instance;

            chatBackend.CurrentUser.Name = text.Substring(commandMatchString.Length).Trim();
            string nameSetString = "Setting your name to " + chatBackend.CurrentUser.Name;
            string messageText = nameSetString;
            sendMessageDelegate(chatBackend.EventUser, chatBackend.CurrentUser, messageText);
        }

        #endregion Internal Methods
    }
}