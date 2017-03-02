using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
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
        ///     Initializes a new instance of the <see cref="ChatWindowControl" /> class.
        /// </summary>
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
        ///     Displays the given message in the user's gui
        /// </summary>
        /// <param name="messageComposite">
        ///     The delegate method that tells the backend how to display messages recieved from other
        ///     users
        /// </param>
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

        private void ScrollToEnd()
        {
            if (TxtChatPane == null) return;

            TxtChatPane.SelectionStart = TxtChatPane.Text.Length;
            TxtChatPane.ScrollToEnd();
            TxtEntryField.Focus();
        }

        private void TextBoxEntryField_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (!(e.Key == Key.Return || e.Key == Key.Enter)) return;

            var vm = (TabChatWindow) this.DataContext;
            TargetUser = vm.TargetUser;

            Backend.SendMessage(TxtEntryField.Text);
            TxtEntryField.Clear();
            TxtEntryField.Focus();
        }

        private void TxtChatPane_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e) => ScrollToEnd();

        private void TxtChatPane_TextChanged(object sender, TextChangedEventArgs e) => ScrollToEnd();

        #endregion Private Methods
    }
}