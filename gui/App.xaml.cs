using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Threading;
using wiicapture.gui.Messages;
using wiicapture.gui.Properties;
using wiicapture.gui.ViewModel;

namespace wiicapture.gui
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            DispatcherHelper.Initialize();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            base.OnStartup(e);

            if (string.IsNullOrWhiteSpace(Settings.Default.SubjectName))
            {
                Settings.Default.SubjectName = CaptureViewModel.DefaultSubjectName;
                Settings.Default.Save();
            }

            if (string.IsNullOrWhiteSpace(Settings.Default.TrialName))
            {
                Settings.Default.TrialName = CaptureViewModel.DefaultTrialName;
                Settings.Default.Save();
            }

            if (string.IsNullOrWhiteSpace(Settings.Default.OutputPath))
            {
                Settings.Default.OutputPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), CaptureViewModel.DefaultDataFolder);
                Settings.Default.Save();
            }
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            SystemError.Send("Unhandled Error", e.ExceptionObject as Exception);
        }

        void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            // Process unhandled exception
            SystemError.Send("Unhandled Error", e.Exception);

            // Prevent default unhandled exception processing
            e.Handled = true;
        }
    }
}
