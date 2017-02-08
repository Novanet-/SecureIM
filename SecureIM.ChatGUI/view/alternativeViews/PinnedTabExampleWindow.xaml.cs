using System.Windows;
using ChromeTabs;
using SecureIM.ChatGUI.ViewModel.TabClasses;

namespace SecureIM.ChatGUI.view.alternativeViews
{
    /// <summary>
    ///     Interaction logic for PinnedTabExample.xaml
    /// </summary>
    public partial class PinnedTabExampleWindow : Window
    {
        public PinnedTabExampleWindow() { this.Dispatcher.InvokeAsync(InitializeComponent); }

        private void TabControl_ContainerItemPreparedForOverride(object sender, ContainerOverrideEventArgs e)
        {
            e.Handled = true;
            var viewModel = e.Model as TabBase;
            if (e.TabItem != null && viewModel != null) e.TabItem.IsPinned = viewModel.IsPinned;
        }
    }
}