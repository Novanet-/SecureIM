/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:SecureIM.ChatGUI"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using SecureIM.ChatGUI.SampleData;
using SecureIM.ChatGUI.ViewModel;
using SecureIM.ChatGUI.ViewModel.alternativeViews;
using SecureIM.ChatGUI.ViewModel.interfaces;

namespace SecureIM.ChatGUI.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            if (ViewModelBase.IsInDesignModeStatic)
            {
                // Create design time view services and models
                SimpleIoc.Default.Register<IViewModelMainWindow, SampleViewModelMainWindow>();
                SimpleIoc.Default.Register<IViewModelPinnedTabExampleWindow, SampleViewModelPinnedTabExampleWindow>();
                SimpleIoc.Default.Register<IViewModelCustomStyleExampleWindow, SampleViewModelCustomStyleExampleWindow>();
            }
            else
            {
                // Create run time view services and models
                SimpleIoc.Default.Register<IViewModelMainWindow, ViewModelMainWindow>();
                SimpleIoc.Default.Register<IViewModelPinnedTabExampleWindow, ViewModelPinnedTabExampleWindow>();
                SimpleIoc.Default.Register<IViewModelCustomStyleExampleWindow, ViewModelCustomStyleExampleWindow>();
            }

        }

        public IViewModelCustomStyleExampleWindow VieWModelCustomStyleExampleWindow
        {
             get
            {
                return ServiceLocator.Current.GetInstance<IViewModelCustomStyleExampleWindow>();
            }
        }

        public IViewModelMainWindow ViewModelMainWindow
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IViewModelMainWindow>();
            }
        }

        public IViewModelPinnedTabExampleWindow ViewModelPinnedTabExampleWindow
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IViewModelPinnedTabExampleWindow>();
            }

        }

        
        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}