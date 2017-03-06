using System.Windows;
using System.Windows.Controls;
using PostSharp.Patterns.Diagnostics;
using SecureIM.Smartcard.controller.smartcard;

namespace SecureIM.ChatGUI.view
{
    /// <summary>
    /// Interaction logic for SmartcardStartupWindow.xaml
    /// </summary>
    public partial class SmartcardStartupWindow : Window
    {
        public SmartcardController SCardController { get; }

        #region Public Properties

        public ChatBackend.ChatBackend Backend { get; set; }

        #endregion Public Properties

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SmartcardStartupWindow"/> class.
        /// </summary>
        [Log("MyProf")]
        public SmartcardStartupWindow()
        {
            InitializeComponent();
            Backend = ChatBackend.ChatBackend.Instance;
            var smartcardCryptoHandler = Backend.CryptoHandler as SmartcardCryptoHandler;

            SCardController = smartcardCryptoHandler?.SmartcardController;
            string[] readernames = SCardController?.GetSCardReaders();
            if (readernames != null) lstChooseReader.ItemsSource = readernames;
        }

        #endregion Public Constructors

        #region Private Methods

        /// <summary>
        /// Handles the SelectionChanged event of the lstChooseReader control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs"/> instance containing the event data.</param>
        [Log("MyProf")]
        private void lstChooseReader_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            {
                var readerName = lstChooseReader.SelectedItem as string;
                if (readerName != null) SCardController.ConnectToSCardReader(readerName);

                this.Hide();
                Application.Current.MainWindow.Show();
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Window.Closing" /> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.ComponentModel.CancelEventArgs" /> that contains the event data.</param>
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            //do my stuff before closing

            base.OnClosing(e);

            Application.Current.Shutdown();
        }

        #endregion Private Methods
    }
}