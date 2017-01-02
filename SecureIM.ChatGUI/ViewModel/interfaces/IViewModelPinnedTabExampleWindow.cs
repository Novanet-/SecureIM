using GalaSoft.MvvmLight.Command;
using SecureIM.ChatGUI.ViewModel.TabClasses;

namespace SecureIM.ChatGUI.ViewModel.interfaces
{
    public interface IViewModelPinnedTabExampleWindow
    {
        RelayCommand<TabBase> PinTabCommand { get; set; }
    }
}