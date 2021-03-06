﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using JetBrains.Annotations;
using SecureIM.ChatGUI.ViewModel;
using SecureIM.ChatGUI.ViewModel.interfaces;
using SecureIM.ChatGUI.ViewModel.TabClasses;

namespace SecureIM.ChatGUI.SampleData.alternativeViewData
{
    internal class SampleViewModelCustomStyleExampleWindow : ViewModelExampleBase, IViewModelCustomStyleExampleWindow
    {
        private ObservableCollection<TabBase> _itemCollection;

        [NotNull] public new ObservableCollection<TabBase> ItemCollection
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

        [CanBeNull] public new TabBase SelectedTab
        {
            get { return ItemCollection.FirstOrDefault(); }
            set { throw new NotImplementedException(); }
        }
    }
}