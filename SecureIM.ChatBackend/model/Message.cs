using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace SecureIM.ChatBackend.model
{
    [DataContract]
    public class Message
    {
        public Message([NotNull] string text)
        {
            Text = text;
        }

        [DataMember]
        public string Text { get; set; }
    }
}