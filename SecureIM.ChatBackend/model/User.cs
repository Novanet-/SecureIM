using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace SecureIM.ChatBackend.model
{
    /// <summary>
    ///
    /// </summary>
    [DataContract]
    public class User
    {
        #region Public Properties

        [DataMember] public string Name { get; set; } = "Anonymous";

        [DataMember] public string PublicKey { get; set; }

        #endregion Public Properties


        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="User"/> class.
        /// </summary>
        public User() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="User"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="pubKeyBytes">The pub key bytes.</param>
        public User([NotNull] string name, [NotNull] string pubKeyBytes = null)
        {
            Name = name;
            PublicKey = pubKeyBytes;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="User"/> class.
        /// </summary>
        /// <param name="pubKeyBytes">The pub key bytes.</param>
        public User([NotNull] string pubKeyBytes) { PublicKey = pubKeyBytes; }

        #endregion Public Constructors
    }
}