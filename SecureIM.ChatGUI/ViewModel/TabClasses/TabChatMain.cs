using System.ComponentModel;
using SecureIM.ChatBackend.model;

namespace SecureIM.ChatGUI.ViewModel.TabClasses
{
    public class TabChatMain : TabBase
    {
        #region Private Fields

        private string _chatHistory;

        #endregion Private Fields

        #region Public Events


        #endregion Public Events

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

        #region Protected Methods

        #endregion Protected Methods
    }
}