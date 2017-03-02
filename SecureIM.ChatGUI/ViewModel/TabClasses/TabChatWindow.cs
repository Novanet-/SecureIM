using SecureIM.ChatBackend.model;

namespace SecureIM.ChatGUI.ViewModel.TabClasses
{
    public class TabChatWindow : TabBase
    {
        public string ChatHistory { get; set; }
        public string ChatEntry { get; set; }

        public User TargetUser { get; set; }
    }
}