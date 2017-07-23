/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:wiicapture.gui"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using System;
using System.IO;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;

namespace wiicapture.gui.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        public static readonly string WIIViewModelLeftKey = "LeftFoot";
        public static readonly string WIIViewModelRightKey = "RightFoot";
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            ////if (ViewModelBase.IsInDesignModeStatic)
            ////{
            ////    // Create design time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DesignDataService>();
            ////}
            ////else
            ////{
            ////    // Create run time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DataService>();
            ////}

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register(() => new WIIViewModel(Properties.Settings.Default.WIIDeviceIDLeft, Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"calibration", Properties.Settings.Default.WIIDeviceIDLeft + ".xml")), WIIViewModelLeftKey);
            SimpleIoc.Default.Register(() => new WIIViewModel(Properties.Settings.Default.WIIDeviceIDRight, Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"calibration", Properties.Settings.Default.WIIDeviceIDRight + ".xml")), WIIViewModelRightKey);
            SimpleIoc.Default.Register<CaptureViewModel>();
            SimpleIoc.Default.Register<SettingsViewModel>();
            SimpleIoc.Default.Register<ViewModelLocator>(() => this);
        }

        public MainViewModel MainViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();                
            }
        }

        public WIIViewModel WIIViewModelLeft
        {
            get
            {
                return ServiceLocator.Current.GetInstance<WIIViewModel>(WIIViewModelLeftKey);
            }
        }

        public WIIViewModel WIIViewModelRight
        {
            get
            {
                return ServiceLocator.Current.GetInstance<WIIViewModel>(WIIViewModelRightKey);
            }
        }

        public CaptureViewModel CaptureViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<CaptureViewModel>();
            }
        }

        public SettingsViewModel SettingsViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<SettingsViewModel>();
            }
        }

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}