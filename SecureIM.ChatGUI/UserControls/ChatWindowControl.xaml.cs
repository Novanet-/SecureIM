using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using PostSharp.Patterns.Diagnostics;
using SecureIM.ChatBackend.model;
using SecureIM.ChatGUI.ViewModel.TabClasses;

namespace SecureIM.ChatGUI.UserControls
{
    /// <summary>
    ///     Interaction logic for ChatWindowControl.xaml
    /// </summary>
    public partial class ChatWindowControl
    {
        #region Public Properties

        public ChatBackend.ChatBackend Backend { get; }
        public User TargetUser { get; set; }

        #endregion Public Properties

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
        }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// Displays the given message in the user's gui
        /// </summary>
        /// <param name="messageComposite">The delegate method that tells the backend how to display messages recieved from other
        /// users</param>
        [Log("MyProf")]
        public void DisplayMessage(MessageComposite messageComposite)
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

        #endregion Public Methods

        #region Private Methods

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
        /// Handles the OnKeyDown event of the TextBoxEntryField control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="KeyEventArgs"/> instance containing the event data.</param>
        [Log("MyProf")]
        private void TextBoxEntryField_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (!(e.Key == Key.Return || e.Key == Key.Enter)) return;

            var vm = (TabChatWindow) this.DataContext;
            TargetUser = vm.TargetUser;

            Backend.SendMessage(TxtEntryField.Text);
            TxtEntryField.Clear();
            TxtEntryField.Focus();
        }

        /// <summary>
        /// Handles the IsVisibleChanged event of the TxtChatPane control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private void TxtChatPane_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e) => ScrollToEnd();

        /// <summary>
        /// Handles the TextChanged event of the TxtChatPane control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="TextChangedEventArgs"/> instance containing the event data.</param>
        private void TxtChatPane_TextChanged(object sender, TextChangedEventArgs e) => ScrollToEnd();

        #endregion Private Methods
    }
}