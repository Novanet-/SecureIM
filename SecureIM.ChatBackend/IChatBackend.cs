using System.Collections.Generic;
using System.ServiceModel;
using SecureIM.ChatBackend.model;

namespace SecureIM.ChatBackend
{
    public delegate void DisplayMessageDelegate(MessageComposite data);

    public delegate void ProcessMessageDelegate(MessageComposite data, DisplayMessageDelegate dmd, User targetUserForDisplay);

    public delegate void SendMessageDelegate(User sender, User receiver, string messageText, MessageFlags flags = MessageFlags.None);

    /// <summary>
    /// IChatBackend
    /// </summary>
    [ServiceContract]
    public interface IChatBackend
    {
        #region Public Properties

        User CurrentUser { get; }
        DisplayMessageDelegate DisplayMessageDelegate { get; set; }
        User EventUser { get; }
        List<User> FriendsList { get; }
        bool IsRegistered { get; }
        bool ServiceStarted { get; }

        #endregion Public Properties

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

        void StartService();

        #endregion Public Methods
    }
}