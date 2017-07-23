using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using wiicapture.gui.Messages;
using wiicapture.gui.Views;

namespace wiicapture.gui.ViewModel
{
    public class SettingsViewModel : ViewModelBase, IModalCommands
    {
        private string dialogTitle;
        private int captureBufferSize;
        private ObservableCollection<string> markerSetupFiles = new ObservableCollection<string>();

        public ObservableCollection<string> MarkerSetupFiles
        {
            get { return this.markerSetupFiles; }
            set { this.markerSetupFiles = value; RaisePropertyChanged(() => this.MarkerSetupFiles); }
        }

        public Properties.Settings Settings
        {
            get
            {
                return Properties.Settings.Default;
            }
        }

        public int CaptureBufferSize
        {
            get
            {
                return this.captureBufferSize;
            }

            set
            {
                this.captureBufferSize = value;
                RaisePropertyChanged(() => this.CaptureBufferSize);
            }
        }

        public CaptureViewModel CaptureViewModel { get; set; }

        public RelayCommand BufferSizeSettingChangedCommand { get; private set; }

        public string DialogTitle
        {
            get { return dialogTitle; }
            set { dialogTitle = value; RaisePropertyChanged(() => this.DialogTitle); }
        }

        /// <summary>
        /// Command when the OK button is clicked
        /// </summary>
        public RelayCommand OKCommand { get; private set; }

        /// <summary>
        /// Command when the cancel button is clicked
        /// </summary>
        public RelayCommand CancelCommand { get; private set; }

        /// <summary>
        /// Command to set the output path
        /// </summary>
        public RelayCommand SetOutputPathCommand { get; private set; }
        public RelayCommand SetupCustomMarkersCommand { get; private set; }

        public bool CanExecuteOKCommand { get { return this.SettingsIsValid(); } }

        private bool SettingsIsValid()
        {
            return !string.IsNullOrEmpty(this.Settings.OutputPath);
        }

        public SettingsViewModel(ViewModelLocator locator)
        {
            this.CaptureViewModel = locator.CaptureViewModel;
            this.DialogTitle = "Settings";
            this.SetOutputPathCommand = new RelayCommand(() => this.SetOutputPath());
            this.OKCommand = new RelayCommand(() => this.OnOK(), () => this.CanExecuteOKCommand);
            this.CancelCommand = new RelayCommand(() => this.Cancel());
                      
        }
              
        private void SetOutputPath()
        {
            OpenFolderMessage.Send("Capture Folder", (path) =>
            {
                this.Settings.OutputPath = path;
            });
        }

        private void OnOK()
        {
            this.Settings.Save();
        }

        public void Cancel()
        {
            Properties.Settings.Default.Reload();
        }
    }
}
