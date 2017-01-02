﻿using System;
using System.Windows.Controls;
using System.Windows.Input;
using SecureIM.ChatBackend;

namespace Demo.UserControls
{
    /// <summary>
    ///     Interaction logic for ChatWindowControl.xaml
    /// </summary>
    public partial class ChatWindowControl
    {
        #region Private Fields

        public ChatBackend Backend { get; }

        #endregion Private Fields


        #region Public Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ChatWindowControl" /> class.
        /// </summary>
        public ChatWindowControl()
        {
            InitializeComponent();
            Backend = new ChatBackend(DisplayMessage);
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
            string username = messageComposite.Username ?? "";
            string message = messageComposite.Message ?? "";
            textBoxChatPane.Text += username + ": " + message + Environment.NewLine;
        }

        #endregion Public Methods


        #region Private Methods

        private void TextBoxEntryField_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (!(e.Key == Key.Return || e.Key == Key.Enter)) return;

            Backend.SendMessage(textBoxEntryField.Text);
            textBoxEntryField.Clear();
        }

        #endregion Private Methods
    }
}