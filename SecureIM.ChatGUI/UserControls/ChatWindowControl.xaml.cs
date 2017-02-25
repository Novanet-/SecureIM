using System;
using System.Threading.Tasks;
using System.Windows.Input;
using SecureIM.ChatBackend;
using SecureIM.ChatBackend.model;

namespace SecureIM.ChatGUI.UserControls
{
    /// <summary>
    ///     Interaction logic for ChatWindowControl.xaml
    /// </summary>
    public partial class ChatWindowControl
    {
        #region Private Fields

        public ChatBackend.ChatBackend Backend { get; }

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ChatWindowControl" /> class.
        /// </summary>
        public ChatWindowControl()
        {
            this.Dispatcher.InvokeAsync(InitializeComponent);
            Backend = ChatBackend.ChatBackend.Instance;
            Backend.DisplayMessageDelegate = DisplayMessage;

//            Task.Run(() => Backend.StartService());
//                Backend.StartService();
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
            this.Dispatcher.InvokeAsync(() => TextBoxChatPane.Text += username + ": " + message + Environment.NewLine);
        }

        #endregion Public Methods

        #region Private Methods

        private void TextBoxEntryField_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (!(e.Key == Key.Return || e.Key == Key.Enter)) return;

            this.Dispatcher.InvokeAsync(() =>
            {
//                var oldDelegate = Backend.DisplayMessageDelegate.Clone() as DisplayMessageDelegate;
//                Backend.DisplayMessageDelegate = DisplayMessage;

                Backend.SendMessage(TextBoxEntryField.Text);
//                TextBoxEntryField.Clear();
//
//                if (oldDelegate != null) Backend.DisplayMessageDelegate = oldDelegate;
            });
        }

        #endregion Private Methods
    }
}