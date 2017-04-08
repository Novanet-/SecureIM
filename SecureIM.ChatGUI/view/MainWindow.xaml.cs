using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using ChromeTabs;
using JetBrains.Annotations;
using SecureIM.ChatGUI.Utilities;
using SecureIM.ChatGUI.view.alternativeViews;
using SecureIM.ChatGUI.ViewModel;
using SecureIM.ChatGUI.ViewModel.TabClasses;

namespace SecureIM.ChatGUI.view
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        #region Public Properties

        //We use this collection to keep track of what windows we have open
        public List<DockingWindow> OpenWindows { get; }

        #endregion Public Properties


        #region Public Constructors

        public MainWindow()
        {
            this.Dispatcher.InvokeAsync(InitializeComponent);
            OpenWindows = new List<DockingWindow>();
            Backend = ChatBackend.ChatBackend.Instance;

        }

        public ChatBackend.ChatBackend Backend { get; set; }

        #endregion Public Constructors


        #region Private Methods

        private static bool CanInsertTabItem([NotNull] FrameworkElement element)
        {
            if (element is ChromeTabItem) return true;
            if (element is ChromeTabPanel) return true;

            object child = LogicalTreeHelper.GetChildren(element)
                                            .Cast<object>()
                                            .FirstOrDefault(x => x is ChromeTabPanel);

            if (child != null) return true;

            FrameworkElement localElement = element;

            while (true)
            {
                object obj = localElement?.TemplatedParent;

                if (obj == null) break;

                if (obj is ChromeTabItem) return true;

                localElement = localElement.TemplatedParent as FrameworkElement;
            }

            return false;
        }

        /// <summary>
        ///     Used P/Invoke to find and return the top window under the cursor position
        /// </summary>
        /// <param name="source"></param>
        /// <param name="screenPoint"></param>
        /// <returns></returns>
        [CanBeNull]
        private static Window FindWindowUnderThisAt([NotNull] Window source, Point screenPoint)
                // WPF units (96dpi), not device units
        {
            IEnumerable<Window> allWindows = SortWindowsTopToBottom(Application.Current.Windows.OfType<Window>());
            IEnumerable<Window> windowsUnderCurrent = from win in allWindows
                                                      where
                                                      (win.WindowState == WindowState.Maximized
                                                       || new Rect(win.Left, win.Top, win.Width, win.Height).Contains(
                                                                                                                   screenPoint))
                                                      && !Equals(win, source)
                                                      select win;
            return windowsUnderCurrent.FirstOrDefault();
        }

        /// <summary>
        ///     We need to do some P/Invoke magic to get the windows on screen
        /// </summary>
        /// <param name="unsorted"></param>
        /// <returns></returns>
        [ItemNotNull]
        private static IEnumerable<Window> SortWindowsTopToBottom([NotNull] IEnumerable<Window> unsorted)
        {
            var byHandle = new Dictionary<IntPtr, Window>();
            foreach (Window window1 in unsorted)
            {
                var fromVisual = (HwndSource) PresentationSource.FromVisual(window1);
                if (fromVisual != null) byHandle.Add(fromVisual.Handle, window1);
            }

            for (IntPtr hWnd = Win32.GetTopWindow(IntPtr.Zero);
                 hWnd != IntPtr.Zero;
                 hWnd = Win32.GetWindow(hWnd, Win32.GW_HWNDNEXT))
            {
                if (byHandle.ContainsKey(hWnd)) yield return byHandle[hWnd];
            }
        }

        private void BnOpenCustomStyleExample_Click([NotNull] object sender, [NotNull] RoutedEventArgs e)
        {
        }

        private void BnOpenPinnedTabExample_Click([NotNull] object sender, [NotNull] RoutedEventArgs e)
        {
            var newWindow = new PinnedTabExampleWindow();
            newWindow.Show();
        }

        /// <summary>
        ///     This event triggers when a tab is dragged outside the bonds of the tab control panel.
        ///     We can use it to create a docking tab control.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChromeTabControl_TabDraggedOutsideBonds([NotNull] object sender, [NotNull] TabDragEventArgs e)
        {
            var draggedTab = e.Tab as TabBase;

            DockingWindow win = OpenWindows.FirstOrDefault(x => x.DataContext == draggedTab);
            //check if it's already open

            if (win == null) //If not, create a new one
            {
                win = new DockingWindow
                {
                    Title = draggedTab?.TabName,
                    DataContext = draggedTab
                };

                win.Closed += win_Closed;
                win.Loaded += win_Loaded;
                win.LocationChanged += win_LocationChanged;
                win.Tag = e.CursorPosition;
                win.Left = e.CursorPosition.X - win.Width + 200;
                win.Top = e.CursorPosition.Y - 20;
                win.Show();
            }
            else
            {
                Debug.WriteLine(DateTime.Now.ToShortTimeString() + " got window");
                MoveWindow(win, e.CursorPosition);
            }
            OpenWindows.Add(win);
            Debug.WriteLine(e.CursorPosition);
        }

        private void MoveWindow([NotNull] Window win, Point pt)
        {
            //Use a BeginInvoke to delay the execution slightly, else we can have problems grabbing the newly opened window.
            Dispatcher.BeginInvoke(new Action(() =>
            {
                win.Topmost = true;
                //We position the window at the mouse position
                win.Left = pt.X - win.Width + 200;
                win.Top = pt.Y - 20;
                Debug.WriteLine(DateTime.Now.ToShortTimeString() + " dragging window");

                if (Mouse.LeftButton == MouseButtonState.Pressed) win.DragMove(); //capture the movement to the mouse, so it can be dragged around

                win.Topmost = false;
            }));
        }

        private void MyChromeTabControl_SelectionChanged([NotNull] object sender, [NotNull] SelectionChangedEventArgs e) { }
        //remove the window from the open windows collection when it is closed.
        private void win_Closed([NotNull] object sender, [NotNull] EventArgs e)
        {
            OpenWindows.Remove(sender as DockingWindow);
            Debug.WriteLine(DateTime.Now.ToShortTimeString() + " closed window");
        }

        private void win_Loaded([NotNull] object sender, [NotNull] RoutedEventArgs e)
        {
            var win = (Window) sender;
            win.Loaded -= win_Loaded;
            var cursorPosition = (Point) win.Tag;
            MoveWindow(win, cursorPosition);
        }

        //We use this to keep track of where the window is on the screen, so we can dock it later
        private void win_LocationChanged([NotNull] object sender, [NotNull] EventArgs e)
        {
            var win = (Window) sender;
            if (!win.IsLoaded) return;

            var pt = new W32Point();
            if (!Win32.GetCursorPos(ref pt)) Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());

            var absoluteScreenPos = new Point(pt.X, pt.Y);

            Window windowUnder = FindWindowUnderThisAt(win, absoluteScreenPos);

            if (windowUnder == null || !windowUnder.Equals(this)) return;

            //The screen position relative to the main window
            Point relativePoint = PointFromScreen(absoluteScreenPos);

            //Hit test against the tab control
            var element = MyChromeTabControl.InputHitTest(relativePoint) as FrameworkElement;

            if (element == null || !CanInsertTabItem(element)) return;

            var dockedWindowVm = (TabBase) win.DataContext;
            var mainWindowVm = (ViewModelMainWindow) DataContext;

            mainWindowVm.ItemCollection.Add(dockedWindowVm);

            win.Close();

            mainWindowVm.SelectedTab = dockedWindowVm;

            //We run this method on the tab control for it to grab the tab and position it at the mouse, ready to move again.
            MyChromeTabControl.GrabTab(dockedWindowVm);
        }

        #endregion Private Methods
    }
}