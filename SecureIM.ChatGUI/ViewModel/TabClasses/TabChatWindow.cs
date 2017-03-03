using SecureIM.ChatBackend.model;

namespace SecureIM.ChatGUI.ViewModel.TabClasses
{
    public class TabChatWindow : TabBase
    {
        #region Private Fields

        private string _chatHistory;

        #endregion Private Fields

        #region Public Properties

        public string ChatEntry { get; set; }

        public string ChatHistory
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

        public User TargetUser { get; set; }

        #endregion Public Properties
    }
}