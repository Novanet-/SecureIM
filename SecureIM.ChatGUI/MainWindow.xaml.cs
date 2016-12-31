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
        #region Private Fields


        private readonly ChatBackend.ChatBackend _backend;


        #endregion Private Fields




        #region Public Constructors


        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            _backend = new ChatBackend.ChatBackend(DisplayMessage);
        }


        #endregion Public Constructors




        #region Public Methods


        /// <summary>
        /// Displays the message.
        /// </summary>
        /// <param name="composite">The composite.</param>
        public void DisplayMessage(CompositeType composite)
        {
            string username = composite.Username ?? "";
            string message = composite.Message ?? "";
            textBoxChatPane.Text += username + ": " + message + Environment.NewLine;
        }


        #endregion Public Methods




        #region Private Methods


        /// <summary>
        /// Handles the OnKeyDown event of the TextBoxEntryField control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="KeyEventArgs"/> instance containing the event data.</param>
        private void TextBoxEntryField_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (!(e.Key == Key.Return || e.Key == Key.Enter)) return;

            _backend.SendMessage(textBoxEntryField.Text);
            textBoxEntryField.Clear();
        }


        #endregion Private Methods
    }
}