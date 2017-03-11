using JetBrains.Annotations;
using SecureIM.ChatBackend.model;

namespace SecureIM.ChatGUI.ViewModel.TabClasses
{
    public class TabChatWindow : TabBase
    {
        #region Private Fields

        private string _chatHistory;

        #endregion Private Fields

        #region Public Properties

        /// <summary>
        /// Gets or sets the chat entry.
        /// </summary>
        /// <value>
        /// The chat entry.
        /// </value>
        public string ChatEntry { get; set; }

        /// <summary>
        /// Gets or sets the chat history.
        /// </summary>
        /// <value>
        /// The chat history.
        /// </value>
        [NotNull] public string ChatHistory
        {
            get { return _chatHistory; }
            set
            {
                if (_chatHistory == value)
                    return;

                _chatHistory = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the target user.
        /// </summary>
        /// <value>
        /// The target user.
        /// </value>
        public User TargetUser { get; set; }

        #endregion Public Properties
    }
}