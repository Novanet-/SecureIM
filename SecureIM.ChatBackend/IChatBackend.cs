using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using SecureIM.ChatBackend.model;

namespace SecureIM.ChatBackend
{
    public delegate void DisplayMessageDelegate(MessageComposite data);

    [ServiceContract]
    public interface IChatBackend
    {
        #region Public Methods

        /// <summary>
        ///     Displays the message.
        /// </summary>
        /// <param name="messageComposite">The messageComposite.</param>
        [OperationContract(IsOneWay = true)]
        void DisplayMessage(MessageComposite messageComposite);

        /// <summary>
        ///     Sends the message.
        /// </summary>
        /// <param name="text">The text.</param>
        void SendMessage(string text);

        #endregion Public Methods
    }

    [DataContract]
    public class MessageComposite
    {
        #region Public Properties

        [DataMember]
        public User Sender { get; set; }

        [DataMember]
        public Message Message { get; set; }

        #endregion Public Properties

        #region Public Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MessageComposite" /> class.
        /// </summary>
        /// <param name="u">The u.</param>
        /// <param name="m">The m.</param>
        public MessageComposite(User u, string m)
        {
            Sender = u;
            Message = new Message(m);
        }

        public MessageComposite(string messageText, User sender)
        {
            if (messageText == null) throw new ArgumentNullException(nameof(messageText));
            if (sender == null) throw new ArgumentNullException(nameof(sender));

            Sender = sender;
            Message = new Message(messageText);
        }

        #endregion Public Constructors
    }
}