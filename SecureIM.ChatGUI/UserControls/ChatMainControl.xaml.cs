using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using PostSharp.Patterns.Diagnostics;
using SecureIM.ChatBackend.model;
using SecureIM.ChatGUI.view.alternativeViews;
using PostSharp.Extensibility;

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
        [Log("MyProf")]
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
        [Log("MyProf")]
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

        [Log("MyProf")]
        private void BtnAddFriend_Click(object sender, RoutedEventArgs e)
        {
            string friendAlias = TxtAlias.Text;
            string friendPubKey = TxtFriendPublicKey.Text;
            SendCommand($"addfriend:{friendAlias}:{friendPubKey}");
            TxtAlias.Clear();
            TxtFriendPublicKey.Clear();
            TxtEntryField.Focus();
        }

        [Log("MyProf")]
        private void BtnGenKeyPair_Click(object sender, RoutedEventArgs e)
        {
            SendCommand("genkey:");
            TxtEntryField.Focus();
        }

        [Log("MyProf")]
        private void BtnGetPubKey_Click(object sender, RoutedEventArgs e)
        {
            SendCommand("getpub:");
            TxtEntryField.Focus();
        }

        [Log("MyProf")]
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

        [Log("MyProf")]
        private void SendCommand(string commandString)
        {
            Backend.SendMessage(commandString);
            TxtEntryField.Clear();
            TxtEntryField.Focus();

            ToggleRestrictedControls(Backend.IsRegistered);
        }

        [Log("MyProf")]
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

        [Log("MyProf")]
        private void ToggleBaseControls(bool enabled)
        {
            BtnSetName.IsEnabled = enabled;
            TxtSetName.IsEnabled = enabled;
            BtnGenKeyPair.IsEnabled = enabled;
            BtnGetPubKey.IsEnabled = enabled;
        }

        [Log("MyProf")]
        private void ToggleRestrictedControls(bool enabled)
        {
            BtnAddFriend.IsEnabled = enabled;
            TxtAlias.IsEnabled = enabled;
            TxtFriendPublicKey.IsEnabled = enabled;
            BtnStartChat.IsEnabled = enabled;
            TxtStartChatFriendName.IsEnabled = enabled;
        }

        #endregion Private Methods

        [Log("MyProf")]
        private void BtnRegPubKey_Click(object sender, RoutedEventArgs e)
        {
            SendCommand("regpub:");
            TxtEntryField.Focus();
        }

        [Log("MyProf")]
        private void BtnStartChat_Click(object sender, RoutedEventArgs e)
        {
            IEnumerable<User> userMatches = Backend.FriendsList.Where(x => x.Name.Equals(TxtStartChatFriendName.Text));
            var userMatchesList = userMatches as IList<User> ?? userMatches.ToList();
            if (userMatchesList.Count() > 1)
            {
                var messageComposite = new MessageComposite(Backend.EventUser, Backend.CurrentUser, "Duplicate friends with this name found");
                Backend.DisplayMessageDelegate?.Invoke(messageComposite);
            }

            else if (!userMatchesList.Any())
            {
                var messageComposite = new MessageComposite(Backend.EventUser, Backend.CurrentUser, "Friend not found");
                Backend.DisplayMessageDelegate?.Invoke(messageComposite);
            }
            else
            {
                User user = userMatchesList.First();

                var parent = Application.Current.MainWindow as PinnedTabExampleWindow;
                parent?.MyChromeTabControlWithPinnedTabs.AddTabCommand.Execute(user);
            }
        }
    }
}