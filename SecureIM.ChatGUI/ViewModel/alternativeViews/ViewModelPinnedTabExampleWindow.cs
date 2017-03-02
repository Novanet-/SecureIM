using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using GalaSoft.MvvmLight.Command;
using SecureIM.ChatBackend;
using SecureIM.ChatBackend.model;
using SecureIM.ChatGUI.ViewModel.interfaces;
using SecureIM.ChatGUI.ViewModel.TabClasses;

namespace SecureIM.ChatGUI.ViewModel.alternativeViews
{
    public sealed class ViewModelPinnedTabExampleWindow : ViewModelExampleBase, IViewModelPinnedTabExampleWindow
    {
        public RelayCommand<TabBase> PinTabCommand { get; set; }
        public RelayCommand<User> AddChatTabCommand { get; set; }


        public ViewModelPinnedTabExampleWindow()
        {
            TabBase vm1 = CreateTabChatMain();
            vm1.IsPinned = true;
            ItemCollection.Add(vm1);
            ItemCollection.Add(CreateTabChatWindow(new User("")));
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
            AddChatTabCommand = new RelayCommand<User>(AddTabCommandAction);
            ChatBackend.ChatBackend.Instance.ProcessMessageDelegate = ProcessMessage;
        }

        private void ProcessMessage(MessageComposite message, DisplayMessageDelegate dmd)
        {
            //TODO: do stuff
        }

        private void PinTabCommandAction(TabBase tab)
        {
            tab.IsPinned = !tab.IsPinned;
            var view = CollectionViewSource.GetDefaultView(ItemCollection) as ICollectionView;
            view.Refresh();
        }

        //Adds a random tab
        public void AddTabCommandAction(User user)
        {
            var newTab = CreateTabChatWindow(user);


            Binding bind = new Binding();
            bind.Source = newTab;
            bind.Path = new PropertyPath("TargetUser");
            newTab.TargetUser = user;
            bind.Mode = BindingMode.TwoWay;
            bind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;


            ItemCollection.Add(newTab);
        }
    }
}