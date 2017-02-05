using System;
using System.Runtime.Serialization;
using System.ServiceModel;

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
        public string Message { get; set; } = "";

        [DataMember]
        public User Sender { get; }

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
            Message = m;
        }

        public MessageComposite(string message, User sender)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            if (sender == null) throw new ArgumentNullException(nameof(sender));
            Message = message;
            Sender = sender;
        }

        #endregion Public Constructors
    }
}