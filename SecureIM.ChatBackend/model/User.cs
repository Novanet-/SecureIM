using System;
using System.Runtime.Serialization;

namespace SecureIM.ChatBackend.model
{
    [DataContract]
    public class User
    {
        public User()
        {
        }

        public User(string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            Name = name;
        }

        public User(string name, byte[] pubKeyBytes)
        {
            Name = name;
            PublicKey = pubKeyBytes;
        }

        [DataMember] public string Name { get; set; } = "Anonymous";
        [DataMember] public byte[] PublicKey { get; set; }
    }
}