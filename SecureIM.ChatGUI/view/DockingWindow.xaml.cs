using System.Windows;
using System.Windows.Media.Animation;
using JetBrains.Annotations;

namespace SecureIM.ChatGUI.view
{
    /// <summary>
    ///     Interaction logic for DockingWindow.xaml
    /// </summary>
    public partial class DockingWindow
    {
        public DockingWindow() { this.Dispatcher.InvokeAsync(InitializeComponent); }

        private void Window_Loaded([NotNull] object sender, [NotNull] RoutedEventArgs e)
        {
            var sb = Resources["FadeInContentAnim"] as Storyboard;
            sb?.Begin();
        }
    }
}