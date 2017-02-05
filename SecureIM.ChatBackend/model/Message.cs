using System.Runtime.Serialization;

namespace SecureIM.ChatBackend.model
{
    [DataContract]
    public class Message
    {
        public Message(string messageText)
        {
            MessageText = messageText;
        }

        [DataMember]
        public string MessageText { get; set; }
    }
}