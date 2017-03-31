using System.Windows;

namespace SecureIM.ChatGUI.view.alternativeViews
{
    /// <summary>
    ///     Interaction logic for CustomStyleExampleWindow.xaml
    /// </summary>
    public partial class CustomStyleExampleWindow : Window
    {
        public CustomStyleExampleWindow() { this.Dispatcher.InvokeAsync(InitializeComponent); }
    }
}