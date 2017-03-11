using System.Windows.Media;
using GalaSoft.MvvmLight;
using JetBrains.Annotations;

namespace SecureIM.ChatGUI.ViewModel.TabClasses
{
    public abstract class TabBase : ViewModelBase
    {
        private int _tabNumber;

        /// <summary>
        /// Gets or sets the tab number.
        /// </summary>
        /// <value>
        /// The tab number.
        /// </value>
        public int TabNumber
        {
            get { return _tabNumber; }
            set
            {
                if (_tabNumber != value) Set(propertyExpression: () => TabNumber, field: ref _tabNumber, newValue: value);
            }
        }

        private string _tabName;

        /// <summary>
        /// Gets or sets the name of the tab.
        /// </summary>
        /// <value>
        /// The name of the tab.
        /// </value>
        [NotNull] public string TabName
        {
            get { return _tabName; }
            set
            {
                if (_tabName != value) Set(propertyExpression: () => TabName, field: ref _tabName, newValue: value);
            }
        }


        private bool _isPinned;

        /// <summary>
        /// Gets or sets a value indicating whether this instance is pinned.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is pinned; otherwise, <c>false</c>.
        /// </value>
        public bool IsPinned
        {
            get { return _isPinned; }
            set
            {
                if (_isPinned != value) Set(propertyExpression: () => IsPinned, field: ref _isPinned, newValue: value);
            }
        }


        private ImageSource _tabIcon;

        /// <summary>
        /// Gets or sets the tab icon.
        /// </summary>
        /// <value>
        /// The tab icon.
        /// </value>
        [NotNull] public ImageSource TabIcon
        {
            get { return _tabIcon; }
            set
            {
                if (!Equals(_tabIcon, value)) Set(propertyExpression: () => TabIcon, field: ref _tabIcon, newValue: value);
            }
        }
    }
}