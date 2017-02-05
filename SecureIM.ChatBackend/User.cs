using System.Runtime.Serialization;

namespace SecureIM.ChatBackend
{
    public class User
    {
        public User()
        {
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