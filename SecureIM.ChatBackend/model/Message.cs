using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace SecureIM.ChatBackend.model
{
    /// <summary>
    /// Message
    /// </summary>
    [DataContract]
    public class Message
    {
        #region Public Properties

        [DataMember] public string Text { get; set; }

        #endregion Public Properties


        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Message"/> class.
        /// </summary>
        /// <param name="text">The text.</param>
        public Message([NotNull] string text) { Text = text; }

        #endregion Public Constructors
    }
}