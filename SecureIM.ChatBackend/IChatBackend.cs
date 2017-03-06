using System.ServiceModel;
using SecureIM.ChatBackend.model;

namespace SecureIM.ChatBackend
{
    public delegate void DisplayMessageDelegate(MessageComposite data);
    public delegate void SendMessageDelegate(User sender, User reciever, string messageText, MessageFlags flags = MessageFlags.None);
    public delegate void ProcessMessageDelegate(MessageComposite data, DisplayMessageDelegate dmd);


    /// <summary>
    /// IChatBackend
    /// </summary>
    [ServiceContract]
    public interface IChatBackend
    {
        #region Public Methods

        /// <summary>
        /// Displays the message.
        /// </summary>
        /// <param name="messageComposite">The messageComposite.</param>
        [OperationContract(IsOneWay = true)]
        void DisplayMessage(MessageComposite messageComposite);

        /// <summary>
        /// Sends the message.
        /// </summary>
        /// <param name="text">The text.</param>
        void SendMessage(string text);

        #endregion Public Methods
    }
}