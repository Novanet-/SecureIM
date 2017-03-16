using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using JetBrains.Annotations;
using PostSharp.Patterns.Diagnostics;
using SecureIM.ChatBackend.model;
using SecureIM.ChatGUI.ViewModel.TabClasses;

namespace SecureIM.ChatGUI.UserControls
{
    /// <summary>
    ///     Interaction logic for ChatWindowControl.xaml
    /// </summary>
    internal partial class ChatWindowControl
    {
        #region Private Properties

        private ChatBackend.ChatBackend Backend { get; }
        private User TargetUser { get; set; }

        #endregion Private Properties

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ChatWindowControl" /> class.
        /// </summary>
        [Log("MyProf")]
        public ChatWindowControl()
        {
            InitializeComponent();
            Backend = ChatBackend.ChatBackend.Instance;
            Backend.DisplayMessageDelegate = DisplayMessage;

            //            BindingExpression exp = LblTargetUser.GetBindingExpression(ContentProperty);
            //            exp?.UpdateSource();

            ScrollToEnd();
            TxtEntryField.Focus();
        }

        #endregion Public Constructors

        #region Private Methods

        /// <summary>
        /// Displays the given message in the user's gui
        /// </summary>
        /// <param name="messageComposite">The delegate method that tells the backend how to display messages recieved from other
        /// users</param>
        [Log("MyProf")]
        private void DisplayMessage([NotNull] MessageComposite messageComposite)
        {
            string username = messageComposite.Sender.Name ?? "";
            string message = messageComposite.Message.Text ?? "";
            Dispatcher.InvokeAsync(() =>
            {
                TxtChatPane.AppendText($"{username}: {message}{Environment.NewLine}");

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

            var vm = (TabChatWindow)DataContext;
            TargetUser = vm.TargetUser;

            //            Backend.SendMessage(TxtEntryField.Text);
            SendCommand($"encrypt:{LblTargetUser.Content}:{TxtEntryField.Text}");

            TxtEntryField.Clear();
            TxtEntryField.Focus();
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