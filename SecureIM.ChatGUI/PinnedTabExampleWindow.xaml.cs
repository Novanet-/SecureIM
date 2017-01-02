using System.Windows;
using ChromeTabs;
using Demo.ViewModel;

namespace Demo
{
    /// <summary>
    ///     Interaction logic for PinnedTabExample.xaml
    /// </summary>
    public partial class PinnedTabExampleWindow : Window
    {
        public PinnedTabExampleWindow() { InitializeComponent(); }

        private void TabControl_ContainerItemPreparedForOverride(object sender, ContainerOverrideEventArgs e)
        {
            e.Handled = true;
            var viewModel = e.Model as TabBase;
            if (e.TabItem != null && viewModel != null) e.TabItem.IsPinned = viewModel.IsPinned;
        }
    }
}