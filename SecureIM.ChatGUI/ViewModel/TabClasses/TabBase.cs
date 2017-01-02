using System.Windows.Media;
using GalaSoft.MvvmLight;

namespace SecureIM.ChatGUI.ViewModel.TabClasses
{
    public abstract class TabBase : ViewModelBase
    {
        private int _tabNumber;

        public int TabNumber
        {
            get { return _tabNumber; }
            set
            {
                if (_tabNumber != value) Set(propertyExpression: () => TabNumber, field: ref _tabNumber, newValue: value);
            }
        }

        private string _tabName;

        public string TabName
        {
            get { return _tabName; }
            set
            {
                if (_tabName != value) Set(propertyExpression: () => TabName, field: ref _tabName, newValue: value);
            }
        }


        private bool _isPinned;

        public bool IsPinned
        {
            get { return _isPinned; }
            set
            {
                if (_isPinned != value) Set(propertyExpression: () => IsPinned, field: ref _isPinned, newValue: value);
            }
        }


        private ImageSource _tabIcon;

        public ImageSource TabIcon
        {
            get { return _tabIcon; }
            set
            {
                if (!Equals(_tabIcon, value)) Set(propertyExpression: () => TabIcon, field: ref _tabIcon, newValue: value);
            }
        }
    }
}