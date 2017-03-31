using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using GalaSoft.MvvmLight.Command;
using JetBrains.Annotations;
using PostSharp.Patterns.Diagnostics;
using SecureIM.ChatBackend;
using SecureIM.ChatBackend.model;
using SecureIM.ChatGUI.ViewModel.interfaces;
using SecureIM.ChatGUI.ViewModel.TabClasses;

namespace SecureIM.ChatGUI.ViewModel.alternativeViews
{
    public sealed class ViewModelPinnedTabExampleWindow : ViewModelExampleBase, IViewModelPinnedTabExampleWindow
    {
        #region Public Properties

        public RelayCommand<User> AddChatTabCommand { get; set; }

        public RelayCommand<TabBase> PinTabCommand { get; set; }

        #endregion Public Properties

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelPinnedTabExampleWindow"/> class.
        /// </summary>
        [Log("MyProf")]
        public ViewModelPinnedTabExampleWindow()
        {
            TabBase vm1 = CreateTabChatMain();
            vm1.IsPinned = true;
            ItemCollection.Add(vm1);
            ItemCollection.Add(CreateTabChatWindow(new User("")));
            //            ItemCollection.Add(CreateTab3());
            //            ItemCollection.Add(CreateTabLoremIpsum());
            SelectedTab = ItemCollection.FirstOrDefault();
            ICollectionView view = CollectionViewSource.GetDefaultView(ItemCollection);
            //This sort description is what keeps the source collection sorted, based on tab number.
            //You can also use the sort description to manually sort the tabs, based on your own criterias,
            //as show below by sorting both by tab number and Pinned status.
            view.SortDescriptions.Add(new SortDescription("IsPinned", ListSortDirection.Descending));
            view.SortDescriptions.Add(new SortDescription("TabNumber", ListSortDirection.Ascending));

            PinTabCommand = new RelayCommand<TabBase>(PinTabCommandAction);
            AddChatTabCommand = new RelayCommand<User>(AddTabCommandAction);
            ChatBackend.ChatBackend.Instance.ProcessMessageDelegate = ProcessMessage;
        }

        #endregion Public Constructors

        #region Public Methods

        //Adds a random tab
        /// <summary>
        /// Adds the tab command action.
        /// </summary>
        /// <param name="user">The user.</param>
        [Log("MyProf")]
        private void AddTabCommandAction([NotNull] User user)
        {
            TabChatWindow newTab = CreateTabChatWindow(user);

            var bind = new Binding
            {
                Source = newTab,
                Path = new PropertyPath("TargetUser")
            };
            newTab.TargetUser = user;
            bind.Mode = BindingMode.TwoWay;
            bind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

            ItemCollection.Add(newTab);
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Adds the message to chat.
        /// </summary>
        /// <param name="messageComposite">The message composite.</param>
        /// <param name="chatTab">The chat tab.</param>
        [Log("MyProf")]
        private static void AddMessageToChat([NotNull] MessageComposite messageComposite, [NotNull] TabChatWindow chatTab)
        {
            string username = messageComposite.Sender.Name ?? "";
            string message = messageComposite.Message.Text ?? "";

            string newChatLog = $"{chatTab?.ChatHistory}{username}: {message}{Environment.NewLine}";
            chatTab.ChatHistory = newChatLog;
            chatTab.NewMessages = chatTab.NewMessages + 1;
            chatTab.TabName = $"{chatTab.TargetUser.Name} [{chatTab.NewMessages}]";
        }

        /// <summary>
        /// Adds the message to chat.
        /// </summary>
        /// <param name="messageComposite">The message composite.</param>
        /// <param name="chatMain">The chat main.</param>
        [Log("MyProf")]
        private static void AddMessageToChat([NotNull] MessageComposite messageComposite, [NotNull] TabChatMain chatMain)
        {
            string username = messageComposite.Sender.Name ?? "";
            string message = messageComposite.Message.Text ?? "";

            string newChatLog = $"{chatMain?.ChatHistory}{username}: {message}{Environment.NewLine}";
            chatMain.ChatHistory = newChatLog;
        }

        /// <summary>
        /// Matches the tab to target user.
        /// </summary>
        /// <param name="messageComposite">The message composite.</param>
        /// <param name="tab">The tab.</param>
        /// <returns></returns>
        [Log("MyProf")]
        private static bool MatchTabToTargetUser([NotNull] User targetUser, [NotNull] TabBase tab)
        {
            if (tab.IsPinned)
            {
                return targetUser.PublicKey.Equals("event") || targetUser.PublicKey.Equals("info");
            }

            var chatTab = tab as TabChatWindow;
            return chatTab != null && chatTab.TargetUser.PublicKey.Equals(targetUser.PublicKey);
        }

        /// <summary>
        /// Pins the tab command action.
        /// </summary>
        /// <param name="tab">The tab.</param>
        [Log("MyProf")]
        private void PinTabCommandAction([NotNull] TabBase tab)
        {
            tab.IsPinned = !tab.IsPinned;
            ICollectionView view = CollectionViewSource.GetDefaultView(ItemCollection);
            view.Refresh();
        }

        /// <summary>
        /// Processes the message.
        /// </summary>
        /// <param name="messageComposite">The message composite.</param>
        /// <param name="dmd">The DMD.</param>
        /// <param name="targetUserForDisplay"></param>
        [Log("MyProf")]
        private void ProcessMessage([NotNull] MessageComposite messageComposite, [NotNull] DisplayMessageDelegate dmd, [NotNull] User targetUserForDisplay)
        {
            //TODO: do stuff

            IEnumerable<TabBase> matchedTab = ItemCollection.Where(tab => MatchTabToTargetUser(targetUserForDisplay, tab));

            IEnumerable<TabBase> tabBases = matchedTab as TabBase[] ?? matchedTab.ToArray();
            if (!tabBases.Any())
            {
                //TODO: create new tab

                TabChatWindow newChatTab = CreateTabChatWindow(messageComposite.Sender);
                ItemCollection.Add(newChatTab);
                AddMessageToChat(messageComposite, newChatTab);
            }
            else
            {
                //TODO: add to chat history of tab
                if (tabBases.First().IsPinned)
                {
                    var matchedChatMain = tabBases.First() as TabChatMain;
                    if (matchedChatMain != null) AddMessageToChat(messageComposite, matchedChatMain);
                }
                else
                {
                    var matchedChatTab = tabBases.First() as TabChatWindow;
                    if (matchedChatTab != null) AddMessageToChat(messageComposite, matchedChatTab);
                }
            }
        }

        #endregion Private Methods
    }
}