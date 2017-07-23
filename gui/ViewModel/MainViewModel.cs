using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using wiicapture.gui.Messages;

namespace wiicapture.gui.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {  
        private string statusText;       
        private ViewModelLocator viewModelLocator;
        private ConcurrentQueue<string> statusMessages;
        private DateTime lastStatusMessage;
        private Timer toastTimer;
        private string statusMessage;

        public string StatusText
        {
            get { return this.statusText; }
            set
            {
                this.statusText = value;
                RaisePropertyChanged(() => this.StatusText);
            }
        }

        public CaptureViewModel CaptureViewModel
        {
            get;
            private set;
        }
        
        public RelayCommand ShowSettingsCommand { get; private set; }
        
        public RelayCommand QuitApplicationCommand { get; private set; }

        public MainViewModel(CaptureViewModel captureViewModel)
        {
            this.CaptureViewModel = captureViewModel;

            this.ShowSettingsCommand = new RelayCommand(this.ShowSettings);
           
            this.QuitApplicationCommand = new RelayCommand(this.QuitApplication);
            
            Messenger.Default.Register<SystemMessage>(this, (message) => this.statusMessages.Enqueue(message.Message));
            Messenger.Default.Register<SystemError>(this, (message) => this.statusMessages.Enqueue(message.Message));
           
            this.statusMessages = new ConcurrentQueue<string>();

            this.toastTimer = new Timer(new TimerCallback((o) =>
            {
                var message = string.Empty;

                if (this.statusMessages.TryDequeue(out message))
                {
                    this.StatusMessage = message;
                }
                RaisePropertyChanged(() => this.ShowToast);
            }), null, 0, 1000);
        }

        private void QuitApplication()
        {
            Application.Current.Shutdown(0);
        }

        private void ShowSettings()
        {
            new SettingsWindow().ShowDialog();
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.toastTimer != null) this.toastTimer.Dispose();
            }
        }

        public string StatusMessage
        {
            get { return this.statusMessage; }
            set
            {
                this.statusMessage = value;
                this.lastStatusMessage = DateTime.Now;
                RaisePropertyChanged(() => this.StatusMessage);
            }
        }

        public bool ShowToast
        {
            get
            {
                return DateTime.Now.Subtract(this.lastStatusMessage).TotalSeconds <= 5;
            }
        }
     }
}