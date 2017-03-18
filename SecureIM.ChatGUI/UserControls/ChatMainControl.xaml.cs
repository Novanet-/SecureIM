using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using JetBrains.Annotations;
using PostSharp.Patterns.Diagnostics;
using SecureIM.ChatBackend.model;
using SecureIM.ChatGUI.view.alternativeViews;
using SecureIM.ChatBackend;

namespace SecureIM.ChatGUI.UserControls
{
    /// <summary>
    ///     Interaction logic for ChatWindowControl.xaml
    /// </summary>
    internal partial class ChatMainControl
    {
        #region Public Properties

        private IChatBackend Backend { get; }

        #endregion Public Properties

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ChatWindowControl" /> class.
        /// </summary>
        [Log("MyProf")]
        public ChatMainControl()
        {
            InitializeComponent();
            ScrollToEnd();

            Backend = ChatBackend.ChatBackend.Instance;
            Backend.DisplayMessageDelegate = DisplayMessage;

//            ToggleBaseControls(false);
//            ToggleRestrictedControls(false);

            if (!Backend.ServiceStarted)
            {
                Dispatcher.InvokeAsync(() =>
                {
                    Backend.StartService();
                    ToggleRestrictedControls(Backend.IsRegistered);
                    LblCurrentName.Content = Backend.CurrentUser.Name;
                    ToggleBaseControls(true);
                });
            }
            else
            {
                ToggleBaseControls(true);
                ToggleRestrictedControls(Backend.IsRegistered);
            }
        }

        #endregion Public Constructors

        #region Private Methods

        /// <summary>
        /// Handles the Click event of the BtnAddFriend control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        [Log("MyProf")]
        private void BtnAddFriend_Click([NotNull] object sender, [NotNull] RoutedEventArgs e)
        {
            string friendAlias = TxtAlias.Text;
            string friendPubKey = TxtFriendPublicKey.Text;
            SendCommand($"addfriend:{friendAlias}:{friendPubKey}");
            TxtAlias.Clear();
            TxtFriendPublicKey.Clear();
            TxtEntryField.Focus();
        }

        /// <summary>
        /// Handles the Click event of the BtnGenKeyPair control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        [Log("MyProf")]
        private void BtnGenKeyPair_Click([NotNull] object sender, [NotNull] RoutedEventArgs e)
        {
            SendCommand("genkey:");
            TxtEntryField.Focus();
        }

        /// <summary>
        /// Handles the Click event of the BtnGetPubKey control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        [Log("MyProf")]
        private void BtnGetPubKey_Click([NotNull] object sender, [NotNull] RoutedEventArgs e)
        {
            SendCommand("getpub:");
            TxtEntryField.Focus();
        }

        /// <summary>
        /// Handles the Click event of the BtnRegPubKey control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        [Log("MyProf")]
        private void BtnRegPubKey_Click([NotNull] object sender, [NotNull] RoutedEventArgs e)
        {
            SendCommand("regpub:");
            TxtEntryField.Focus();
        }

        /// <summary>
        /// Handles the Click event of the BtnSetName control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        [Log("MyProf")]
        private void BtnSetName_Click([NotNull] object sender, [NotNull] RoutedEventArgs e)
        {
            string newName = TxtSetName.Text;
            SendCommand($"setname:{newName}");
            LblCurrentName.Content = Backend.CurrentUser.Name;
            TxtSetName.Clear();
            TxtEntryField.Focus();
        }

        /// <summary>
        /// Handles the Click event of the BtnStartChat control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        [Log("MyProf")]
        private void BtnStartChat_Click([NotNull] object sender, [NotNull] RoutedEventArgs e)
        {
            IEnumerable<User> userMatches = Backend.FriendsList.Where(x => x.Name.Equals(TxtStartChatFriendName.Text));
            IList<User> userMatchesList = userMatches as IList<User> ?? userMatches.ToList();
            if (userMatchesList.Count > 1)
            {
                var messageComposite = new MessageComposite(Backend.EventUser, Backend.CurrentUser, "Duplicate friends with this name found");
                Backend.DisplayMessageDelegate(messageComposite);
            }
            else if (!userMatchesList.Any())
            {
                var messageComposite = new MessageComposite(Backend.EventUser, Backend.CurrentUser, "Friend not found");
                Backend.DisplayMessageDelegate(messageComposite);
            }
            else
            {
                User user = userMatchesList.First();

                var parent = Application.Current.MainWindow as PinnedTabExampleWindow;
                parent?.MyChromeTabControlWithPinnedTabs.AddTabCommand.Execute(user);
            }
        }

        /// <summary>
        ///     Displays the given message in the user's gui
        /// </summary>
        /// <param name="messageComposite">
        ///     The delegate method that tells the backend how to display messages received from other
        ///     users
        /// </param>
        [Log("MyProf")]
        private void DisplayMessage([NotNull] MessageComposite messageComposite)
        {
            string username = messageComposite.Sender.Name;
            string message = messageComposite.Message.Text;
            Dispatcher.InvokeAsync(() =>
            {
                TxtChatPane.AppendText(username + ": " + message + Environment.NewLine);

                BindingExpression exp = TxtChatPane.GetBindingExpression(TextBox.TextProperty);
                exp?.UpdateSource();
            });
        }

        /// <summary>
        /// Scrolls to end.
        /// </summary>
        private void ScrollToEnd()
        {
            if (TxtChatPane == null) return;

            TxtChatPane.SelectionStart = TxtChatPane.Text.Length;
            TxtChatPane.ScrollToEnd();
            TxtEntryField.Focus();
        }

        /// <summary>
        /// Sends the command.
        /// </summary>
        /// <param name="commandString">The command string.</param>
        [Log("MyProf")]
        private void SendCommand([NotNull] string commandString)
        {
            Backend.SendMessage(commandString);
            TxtEntryField.Clear();
            TxtEntryField.Focus();

            ToggleRestrictedControls(Backend.IsRegistered);
        }

        /// <summary>
        /// Handles the OnKeyDown event of the TextBoxEntryField control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="KeyEventArgs"/> instance containing the event data.</param>
        [Log("MyProf")]
        private void TextBoxEntryField_OnKeyDown([NotNull] object sender, [NotNull] KeyEventArgs e)
        {
            if (!(e.Key == Key.Return || e.Key == Key.Enter)) return;

            Dispatcher.InvokeAsync(() =>
            {
                SendCommand(TxtEntryField.Text);
                TxtEntryField.Clear();
            });

            TxtEntryField.Focus();
        }

        /// <summary>
        /// Toggles the base controls.
        /// </summary>
        /// <param name="enabled">if set to <c>true</c> [enabled].</param>
        [Log("MyProf")]
        private void ToggleBaseControls(bool enabled)
        {
            BtnSetName.IsEnabled = enabled;
            TxtSetName.IsEnabled = enabled;
            BtnGenKeyPair.IsEnabled = enabled;
            BtnGetPubKey.IsEnabled = enabled;
        }

        /// <summary>
        /// Toggles the restricted controls.
        /// </summary>
        /// <param name="enabled">if set to <c>true</c> [enabled].</param>
        [Log("MyProf")]
        private void ToggleRestrictedControls(bool enabled)
        {
            BtnAddFriend.IsEnabled = enabled;
            TxtAlias.IsEnabled = enabled;
            TxtFriendPublicKey.IsEnabled = enabled;
            BtnStartChat.IsEnabled = enabled;
            TxtStartChatFriendName.IsEnabled = enabled;
        }

        /// <summary>
        /// Handles the IsVisibleChanged event of the TxtChatPane control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private void TxtChatPane_IsVisibleChanged([NotNull] object sender, DependencyPropertyChangedEventArgs e) => ScrollToEnd();

        /// <summary>
        /// Handles the TextChanged event of the TxtChatPane control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="TextChangedEventArgs"/> instance containing the event data.</param>
        private void TxtChatPane_TextChanged([NotNull] object sender, [NotNull] TextChangedEventArgs e) => ScrollToEnd();

        #endregion Private Methods
    }
}