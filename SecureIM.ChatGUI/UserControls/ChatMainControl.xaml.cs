using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using SecureIM.ChatBackend.model;

namespace SecureIM.ChatGUI.UserControls
{
    /// <summary>
    ///     Interaction logic for ChatWindowControl.xaml
    /// </summary>
    public partial class ChatMainControl
    {
        #region Public Properties

        public ChatBackend.ChatBackend Backend { get; }

        #endregion Public Properties

        #region Public Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ChatWindowControl" /> class.
        /// </summary>
        public ChatMainControl()
        {
            InitializeComponent();
            ScrollToEnd();

            Backend = ChatBackend.ChatBackend.Instance;
            Backend.DisplayMessageDelegate = DisplayMessage;

            ToggleBaseControls(false);
            ToggleRestrictedControls(false);

            if (!Backend.ServiceStarted)
                this.Dispatcher.InvokeAsync(() =>
                {
                    Backend.StartService();
                    ToggleRestrictedControls(Backend.IsRegistered);
                    LblCurrentName.Content = Backend.CurrentUser.Name;
                    ToggleBaseControls(true);
                });
        }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        ///     Displays the given message in the user's gui
        /// </summary>
        /// <param name="messageComposite">
        ///     The delegate method that tells the backend how to display messages received from other
        ///     users
        /// </param>
        public void DisplayMessage(MessageComposite messageComposite)
        {
            string username = messageComposite.Sender.Name ?? "";
            string message = messageComposite.Message.Text ?? "";
            Dispatcher.InvokeAsync(() =>
            {
                TxtChatPane.AppendText(username + ": " + message + Environment.NewLine);

                BindingExpression exp = TxtChatPane.GetBindingExpression(TextBox.TextProperty);
                exp?.UpdateSource();
            });
        }

        #endregion Public Methods

        #region Private Methods

        private void BtnAddFriend_Click(object sender, RoutedEventArgs e)
        {
            string friendAlias = TxtAlias.Text;
            string friendPubKey = TxtFriendPublicKey.Text;
            SendCommand($"addfriend:{friendAlias}:{friendPubKey}");
            TxtAlias.Clear();
            TxtFriendPublicKey.Clear();
            TxtEntryField.Focus();
        }

        private void BtnGenKeyPair_Click(object sender, RoutedEventArgs e)
        {
            SendCommand("genkey:");
            SendCommand("regpub:");
            TxtEntryField.Focus();
        }

        private void BtnGetPubKey_Click(object sender, RoutedEventArgs e)
        {
            SendCommand("getpub:");
            SendCommand("regpub:");
            TxtEntryField.Focus();
        }

        private void BtnSetName_Click(object sender, RoutedEventArgs e)
        {
            string newName = TxtSetName.Text;
            SendCommand($"setname:{newName}");
            LblCurrentName.Content = Backend.CurrentUser.Name;
            TxtSetName.Clear();
            TxtEntryField.Focus();
        }

        private void ScrollToEnd()
        {
            if (TxtChatPane == null) return;

            TxtChatPane.SelectionStart = TxtChatPane.Text.Length;
            TxtChatPane.ScrollToEnd();
            TxtEntryField.Focus();
        }

        private void SendCommand(string commandString)
        {
            Backend.SendMessage(commandString);
            TxtEntryField.Clear();
            TxtEntryField.Focus();

            ToggleRestrictedControls(Backend.IsRegistered);
        }

        private void TextBoxEntryField_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (!(e.Key == Key.Return || e.Key == Key.Enter)) return;

            Dispatcher.InvokeAsync(() =>
            {
                SendCommand(TxtEntryField.Text);
                TxtEntryField.Clear();
            });

            TxtEntryField.Focus();
        }

        private void TxtChatPane_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e) => ScrollToEnd();

        private void TxtChatPane_TextChanged(object sender, TextChangedEventArgs e) => ScrollToEnd();

        private void ToggleBaseControls(bool enabled)
        {
            BtnSetName.IsEnabled = enabled;
            TxtSetName.IsEnabled = enabled;
            BtnGenKeyPair.IsEnabled = enabled;
            BtnGetPubKey.IsEnabled = enabled;
        }

        private void ToggleRestrictedControls(bool enabled)
        {
            BtnAddFriend.IsEnabled = enabled;
            TxtAlias.IsEnabled = enabled;
            TxtFriendPublicKey.IsEnabled = enabled;
            BtnStartChat.IsEnabled = enabled;
            TxtStartChatFriendName.IsEnabled = enabled;
        }

        #endregion Private Methods
    }
}