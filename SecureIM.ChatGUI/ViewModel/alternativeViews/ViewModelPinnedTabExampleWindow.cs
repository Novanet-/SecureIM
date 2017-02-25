using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using GalaSoft.MvvmLight.Command;
using SecureIM.ChatGUI.ViewModel.interfaces;
using SecureIM.ChatGUI.ViewModel.TabClasses;

namespace SecureIM.ChatGUI.ViewModel.alternativeViews
{
    public class ViewModelPinnedTabExampleWindow : ViewModelExampleBase, IViewModelPinnedTabExampleWindow
    {
        public RelayCommand<TabBase> PinTabCommand { get; set; }


        public ViewModelPinnedTabExampleWindow()
        {
            TabBase vm1 = CreateTabChatMain();
            vm1.IsPinned = true;
            ItemCollection.Add(vm1);
            ItemCollection.Add(CreateTabChatWindow());
//            ItemCollection.Add(CreateTab3());
//            ItemCollection.Add(CreateTabLoremIpsum());
            SelectedTab = ItemCollection.FirstOrDefault();
            var view = CollectionViewSource.GetDefaultView(ItemCollection) as ICollectionView;
            //This sort description is what keeps the source collection sorted, based on tab number.
            //You can also use the sort description to manually sort the tabs, based on your own criterias,
            //as show below by sorting both by tab number and Pinned status.
            view.SortDescriptions.Add(new SortDescription("IsPinned", ListSortDirection.Descending));
            view.SortDescriptions.Add(new SortDescription("TabNumber", ListSortDirection.Ascending));

            PinTabCommand = new RelayCommand<TabBase>(PinTabCommandAction);
        }

        private void PinTabCommandAction(TabBase tab)
        {
            tab.IsPinned = !tab.IsPinned;
            var view = CollectionViewSource.GetDefaultView(ItemCollection) as ICollectionView;
            view.Refresh();
        }
    }
}