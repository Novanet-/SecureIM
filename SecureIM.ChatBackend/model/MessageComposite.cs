using System;
using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace SecureIM.ChatBackend.model
{
    [DataContract]
    public class MessageComposite
    {
        #region Public Properties

        [DataMember] public User Sender { get; set; }
        [DataMember] public Message Message { get; set; }
        [DataMember] public MessageFlags Flags { get; set; }

        #endregion Public Properties

        #region Public Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MessageComposite" /> class.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="messageText">The messageText.</param>
        /// <param name="flags"></param>
        public MessageComposite([NotNull] User sender, [NotNull] string messageText, MessageFlags flags = MessageFlags.None)
        {
            if (messageText == null) throw new ArgumentNullException(nameof(messageText));
            if (sender == null) throw new ArgumentNullException(nameof(sender));

            Sender = sender;
            Message = new Message(messageText);
            Flags = flags;
        }

        #endregion Public Constructors
    }
}