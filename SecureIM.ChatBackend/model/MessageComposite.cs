using System;
using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace SecureIM.ChatBackend.model
{
    [DataContract]
    public class MessageComposite
    {
        #region Public Properties

        [DataMember] public MessageFlags Flags { get; set; }

        [DataMember] public Message Message { get; set; }

        [DataMember] public User Receiver { get; set; }

        [DataMember] public User Sender { get; set; }

        #endregion Public Properties


        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageComposite"/> class.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="receiver">The receiver.</param>
        /// <param name="messageText">The message text.</param>
        /// <param name="flags">The flags.</param>
        /// <exception cref="System.ArgumentNullException">
        /// messageText
        /// or
        /// sender
        /// </exception>
        public MessageComposite([NotNull] User sender, [NotNull] User receiver, [NotNull] string messageText, MessageFlags flags = MessageFlags.None)
        {
            if (messageText == null) throw new ArgumentNullException(nameof(messageText));
            if (sender == null) throw new ArgumentNullException(nameof(sender));

            Sender = sender;
            Receiver = receiver;
            Message = new Message(messageText);
            Flags = flags;
        }

        #endregion Public Constructors
    }
}