using System;
using System.Collections.ObjectModel;
using System.Linq;
using SecureIM.ChatGUI.ViewModel;
using SecureIM.ChatGUI.ViewModel.interfaces;
using SecureIM.ChatGUI.ViewModel.TabClasses;

namespace SecureIM.ChatGUI.SampleData
{
    public class SampleViewModelMainWindow : ViewModelExampleBase, IViewModelMainWindow
    {
        public bool CanMoveTabs { get { return true; } set { throw new NotImplementedException(); } }

        private ObservableCollection<TabBase> _itemCollection;

        public new ObservableCollection<TabBase> ItemCollection
        {
            get
            {
                return _itemCollection ?? (_itemCollection = new ObservableCollection<TabBase>
                {
                    CreateTab1(),
                    CreateTab2(),
                    CreateTab3(),
                    CreateTabLoremIpsum()
                });
            }
            set { _itemCollection = value; }
        }


        public new TabBase SelectedTab
        {
            get { return ItemCollection.FirstOrDefault(); }
            set { throw new NotImplementedException(); }
        }

        public bool ShowAddButton { get { return true; } set { throw new NotImplementedException(); } }
    }
}