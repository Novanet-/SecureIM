using System;
using System.Windows.Input;
using SecureIM.ChatBackend;

namespace SecureIM.ChatGUI
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly ChatBackend.ChatBackend _backend;

        public MainWindow()
        {
            InitializeComponent();
            _backend = new ChatBackend.ChatBackend(DisplayMessage);
        }

        public void DisplayMessage(CompositeType composite)
        {
            string username = composite.Username ?? "";
            string message = composite.Message ?? "";
            textBoxChatPane.Text += username + ": " + message + Environment.NewLine;
        }

        private void TextBoxEntryField_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (!(e.Key == Key.Return || e.Key == Key.Enter)) return;

            _backend.SendMessage(textBoxEntryField.Text);
            textBoxEntryField.Clear();
        }
    }
}