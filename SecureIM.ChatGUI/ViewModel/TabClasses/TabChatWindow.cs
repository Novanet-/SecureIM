using System.ComponentModel;
using SecureIM.ChatBackend.model;

namespace SecureIM.ChatGUI.ViewModel.TabClasses
{
    public class TabChatWindow : TabBase
    {
        #region Private Fields

        private string _chatHistory;

        #endregion Private Fields

        #region Public Events

        public new event PropertyChangedEventHandler PropertyChanged;

        #endregion Public Events

        #region Public Properties

        public string ChatEntry { get; set; }

        public string ChatHistory

        {
            get { return _chatHistory; }
            set
            {
                _chatHistory = value;
                OnPropertyChanged("ChatHistory");
            }
        }

        public User TargetUser { get; set; }

        #endregion Public Properties

        #region Protected Methods

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion Protected Methods
    }
}