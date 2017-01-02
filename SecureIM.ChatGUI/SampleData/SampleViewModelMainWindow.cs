using System;
using System.Collections.ObjectModel;
using System.Linq;
using Demo.ViewModel;

namespace Demo.SampleData
{
    public class SampleViewModelMainWindow : ViewModelExampleBase, IViewModelMainWindow
    {
        public bool CanMoveTabs { get { return true; } set { throw new NotImplementedException(); } }

        private ObservableCollection<TabBase> _itemCollection;

        public new ObservableCollection<TabBase> ItemCollection
        {
            get
            {
                if (_itemCollection == null)
                {
                    _itemCollection = new ObservableCollection<TabBase>();

                    _itemCollection.Add(CreateTab1());
                    _itemCollection.Add(CreateTab2());
                    _itemCollection.Add(CreateTab3());
                    _itemCollection.Add(CreateTabLoremIpsum());
                }
                return _itemCollection;
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