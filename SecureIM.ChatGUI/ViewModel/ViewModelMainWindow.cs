using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using GalaSoft.MvvmLight.Command;
using SecureIM.ChatBackend.model;
using SecureIM.ChatGUI.ViewModel.interfaces;
using SecureIM.ChatGUI.ViewModel.TabClasses;

namespace SecureIM.ChatGUI.ViewModel
{
    public class ViewModelMainWindow : ViewModelExampleBase, IViewModelMainWindow
    {
        //this property is to show you can lock the tabs with a binding
        private bool _canMoveTabs;
        public RelayCommand<TabBase> PinTabCommand { get; set; }

        public bool CanMoveTabs
        {
            get { return _canMoveTabs; }
            set
            {
                if (_canMoveTabs != value) Set(() => CanMoveTabs, ref _canMoveTabs, value);
            }
        }

        //this property is to show you can bind the visibility of the add button
        private bool _showAddButton;

        public bool ShowAddButton
        {
            get { return _showAddButton; }
            set
            {
                if (_showAddButton != value) Set(() => ShowAddButton, ref _showAddButton, value);
            }
        }


        public ViewModelMainWindow()
        {
            //Adding items to the collection creates a tab
            TabChatMain tabChatMain = CreateTabChatMain();
            ItemCollection.Add(tabChatMain);
//            ItemCollection.Add(CreateTab2());
//            ItemCollection.Add(CreateTab3());
//            ItemCollection.Add(CreateTabLoremIpsum());
            ItemCollection.Add(CreateTabChatWindow(new User("")));

            SelectedTab = ItemCollection.FirstOrDefault();
            var view = CollectionViewSource.GetDefaultView(ItemCollection);

            //This sort description is what keeps the source collection sorted, based on tab number.
            //You can also use the sort description to manually sort the tabs, based on your own criterias.
            view.SortDescriptions.Add(new SortDescription("IsPinned", ListSortDirection.Descending));
            view.SortDescriptions.Add(new SortDescription("TabNumber", ListSortDirection.Ascending));

            CanMoveTabs = true;
            ShowAddButton = true;

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