using System.Windows;
using ChromeTabs;
using GalaSoft.MvvmLight.Command;
using PostSharp.Patterns.Diagnostics;
using SecureIM.ChatGUI.ViewModel.TabClasses;

namespace SecureIM.ChatGUI.view.alternativeViews
{
    /// <summary>
    ///     Interaction logic for PinnedTabExample.xaml
    /// </summary>
    public partial class PinnedTabExampleWindow : Window
    {


        /// <summary>
        /// Initializes a new instance of the <see cref="PinnedTabExampleWindow"/> class.
        /// </summary>
        [Log("MyProf")]
        public PinnedTabExampleWindow()
        {
            this.Dispatcher.InvokeAsync(InitializeComponent);
            var sCardStartupWindow = new SmartcardStartupWindow();
            sCardStartupWindow.Show();
            this.Hide();
        }


        /// <summary>
        /// Handles the ContainerItemPreparedForOverride event of the TabControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ContainerOverrideEventArgs"/> instance containing the event data.</param>
        private void TabControl_ContainerItemPreparedForOverride(object sender, ContainerOverrideEventArgs e)
        {
            e.Handled = true;
            var viewModel = e.Model as TabBase;
            if (e.TabItem != null && viewModel != null) e.TabItem.IsPinned = viewModel.IsPinned;
        }
    }
}