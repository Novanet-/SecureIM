using System;
using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace SecureIM.ChatBackend.model
{
    [DataContract]
    public class User
    {
        public User()
        {
        }

        public User([NotNull] string name, [NotNull] string pubKeyBytes = null)
        {
            Name = name;
            PublicKey = pubKeyBytes;
        }

        public User([NotNull] string pubKeyBytes)
        {
            PublicKey = pubKeyBytes;
        }

        [DataMember] public string Name { get; set; } = "Anonymous";
        [DataMember] public string PublicKey { get; set; }
    }
}