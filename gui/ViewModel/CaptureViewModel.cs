
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using wiicapture.gui.Properties;
using BackgroundPipeline;
using wiicapture.gui.Modules;
using wiicapture.gui.Messages;

namespace wiicapture.gui.ViewModel
{
    public class CaptureViewModel : ViewModelBase, IDisposable
    {
        public static readonly string DefaultDataFolder = "WIIData";
        public static readonly string DefaultSubjectName = "DefaultSubject";
        public static readonly string DefaultTrialName = "Trial1";
        public static readonly string CaptureFilePrefix = "wii";

        public static readonly int MaxWaitTime = 300;

        private bool capture = false;
        private bool waitingForCapturePipeline;
        private DateTime captureStartTime;
        private double fpsPipeline;
        private int bodyCount;

        // capture settings form       
        private TimeSpan captureTime;

        // module for creating file output
        private OutputModule dataOutputModule;

        // A repository of bodies captured    
        private BackgroundPipeline<CaptureFrame> capturePipeline;

        // module for detecting custom markers using OpenCV
        private int queueCount;
        private string captureFileName;

        public TimeSpan CaptureTime
        {
            get { return captureTime; }
            set { captureTime = value; RaisePropertyChanged(() => this.CaptureTime); }
        }

        /// <summary>
        /// The number of bodies captured in the session
        /// </summary>
        public int FrameCount
        {
            get { return bodyCount; }
            set { bodyCount = value; RaisePropertyChanged(() => this.FrameCount); }
        }

        public int QueueCount
        {
            get { return queueCount; }
            set { queueCount = value; RaisePropertyChanged(() => this.QueueCount); }
        }

        /// <summary>
        /// Capture in Progress
        /// </summary>
        public bool CaptureInProgress
        {
            get { return capture; }
            set { capture = value; RaisePropertyChanged(() => this.CaptureInProgress); }
        }

        /// <summary>
        /// The FPS of the capture background processing pipeline
        /// </summary>
        public double FPSPipeline
        {
            get { return fpsPipeline; }
            set
            {
                fpsPipeline = value;
                RaisePropertyChanged(() => this.FPSPipeline);
            }
        }

        /// <summary>
        /// Provides data and management of WII A
        /// </summary>
        public WIIViewModel WIILeftViewModel
        {
            get;
            private set;
        }

        /// <summary>
        /// Provides data and management of WII B
        /// </summary>
        public WIIViewModel WIIRightViewModel
        {
            get;
            private set;
        }

        /// <summary>
        /// Can capture command be executed
        /// </summary>
        public bool CanCapture
        {
            get
            {
                return !this.CaptureInProgress
                    && (this.WIILeftViewModel.Connected || this.WIIRightViewModel.Connected)
                    && !string.IsNullOrEmpty(Settings.Default.OutputPath)
                    && !string.IsNullOrEmpty(Settings.Default.SubjectName)
                    && !string.IsNullOrEmpty(Settings.Default.TrialName);
            }
        }

        /// <summary>
        /// Can the end capture button be clicked
        /// </summary>
        public bool CanEndCapture
        {
            get
            {
                // can only use the end capture button if the capture is in progress and 
                // we are not waiting for the capture pipeline to complete
                return this.CaptureInProgress == true && !this.waitingForCapturePipeline;
            }
        }

        /// <summary>
        /// Start capture command
        /// </summary>
        public RelayCommand CaptureCommand { get; private set; }

        /// <summary>
        /// End capture command
        /// </summary>
        public RelayCommand EndCaptureCommand { get; private set; }

        /// <summary>
        /// Command to kinect to available WII boards
        /// </summary>
        public RelayCommand ConnectWIICommand { get; private set; }

        /// <summary>
        /// Command to capture weight from both WII boards
        /// </summary>
        public RelayCommand CaptureWeightCommand { get; private set; }

        /// <summary>
        /// Command to capture a photo from the color stream
        /// </summary>
        public RelayCommand CapturePhotoCommand { get; private set; }

        /// <summary>
        /// Command to capture the position of WII A in the frame
        /// </summary>
        public RelayCommand<ObservableCollection<Point>> SetWIILeftPositionCommand { get; private set; }

        /// <summary>
        /// Command to capture the position of WII B in the frame
        /// </summary>
        public RelayCommand<ObservableCollection<Point>> SetWIIRightPositionCommand { get; private set; }


        public CaptureViewModel(ViewModelLocator locator)
        {
            this.CaptureInProgress = false;

            this.WIILeftViewModel = locator.WIIViewModelLeft;
            this.WIIRightViewModel = locator.WIIViewModelRight;

            this.CaptureCommand = new RelayCommand(this.StartCapture, () => this.CanCapture);
            this.EndCaptureCommand = new RelayCommand(this.EndCapture, () => this.CanEndCapture);
           
            this.ConnectWIICommand = new RelayCommand(() => this.ConnectWII());
            this.CaptureWeightCommand = new RelayCommand(() => this.CaptureWeight());
                       
            this.dataOutputModule = new OutputModule();
        }
        
        private void ConnectWII()
        {
            this.WIILeftViewModel.ConnectCommand.Execute(true);
            this.WIIRightViewModel.ConnectCommand.Execute(true);
        }

        /// <summary>
        /// Set the weight of the subject when the comand is executed
        /// </summary>
        private void CaptureWeight()
        {
            this.WIILeftViewModel.CaptureWeightCommand.Execute(null);
            this.WIIRightViewModel.CaptureWeightCommand.Execute(null);

            Settings.Default.Weight = (this.WIILeftViewModel.WeightKG + this.WIIRightViewModel.WeightKG);
        }

        /// <summary>
        /// Start the capture process
        /// </summary>
        public void StartCapture()
        {
            //--Start Capture immediately
            //--create the processing pipeline
            this.capturePipeline = new BackgroundPipeline<CaptureFrame>(Settings.Default.WIIFrequencyHz);

            this.captureStartTime = DateTime.Now;

            this.incrementTrialNumber();

            this.captureFileName = Settings.Default.TrialName;

            this.CaptureTime = TimeSpan.Zero;
            
            this.dataOutputModule = new OutputModule { IsEnabled = true };
            this.capturePipeline.Modules.Add(this.dataOutputModule);

            // setup and start the capture            
            this.waitingForCapturePipeline = false;

            this.QueueCount = 0;
            this.FrameCount = 0;
            this.capturePipeline.Timer.Tick += Timer_Tick;
            this.capturePipeline.Start();
            this.CaptureInProgress = true;

            Debug.WriteLine("Capture started.");
            SystemMessage.Send("Capture Started - " + Path.Combine(Properties.Settings.Default.SubjectName, this.captureFileName));
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (!this.CaptureInProgress || this.capturePipeline == null || this.capturePipeline.IsCompleted || this.capturePipeline.IsAddingCompleted)
                return;

            this.CaptureTime = this.capturePipeline.Timer.ElapsedTime;

            this.FPSPipeline = this.capturePipeline.Timer.FPS;

            var frame = new CaptureFrame
            {
                AbsoluteTime = DateTime.Now,
                RelativeTime = this.capturePipeline.Timer.ElapsedTime,
                LeftForce = WIILeftViewModel.ForceFrame,
                RightForce = WIIRightViewModel.ForceFrame
            };
            
            if (this.CanCaptureWII)
            {
                this.capturePipeline.Enqueue(frame);
                this.FrameCount++;
            }

            this.QueueCount = this.capturePipeline.Count;
        }

        public async void EndCapture()
        {
            if (!this.CaptureInProgress || this.waitingForCapturePipeline) return;

            try
            {
                this.capturePipeline.Stop();
                this.capturePipeline.Timer.Tick -= this.Timer_Tick;

                this.waitingForCapturePipeline = true;

                await CompleteCapture(MaxWaitTime);

                //--setup the output files     
                var fileCollection = new List<OutputFile>();

                if (this.CanCaptureWII)
                {
                    if (this.WIILeftViewModel.Connected)
                    {
                        var wiiLeftRawFile = new RawGrfFile(this.WIILeftViewModel.DeviceID);
                        fileCollection.Add(wiiLeftRawFile);
                    }

                    if (this.WIIRightViewModel.Connected)
                    {
                        var wiiRightRawFile = new RawGrfFile(this.WIIRightViewModel.DeviceID);
                        fileCollection.Add(wiiRightRawFile);
                    }
                }

                this.dataOutputModule.Save(fileCollection, Path.Combine(Settings.Default.OutputPath, Settings.Default.SubjectName), this.captureFileName);
                this.dataOutputModule.Clear();
            }
            catch (Exception ex)
            {
                SystemError.Send(ex.Message, ex);
            }
            finally
            {
                // try to clean up memory
                GC.Collect();
                GC.WaitForPendingFinalizers();

                this.CaptureInProgress = false;
                this.dataOutputModule.Dispose();
                this.capturePipeline.Dispose();
                Debug.WriteLine("Capture complete.");
                SystemMessage.Send("Capture Complete!");
                this.waitingForCapturePipeline = false;

                CommandManager.InvalidateRequerySuggested();
            }
        }

        private void incrementTrialNumber()
        {
            var fullPath = Path.Combine(Properties.Settings.Default.OutputPath, Properties.Settings.Default.SubjectName);

            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
            }

            var pattern = @"^(?<trialname>[\w\-.]+)_(?<trialnumber>[\d]+)$";
            var trialName = Settings.Default.TrialName ?? DefaultTrialName;
            var trialNumber = 1;
            // if the trial name doesn't have a number, add one
            if (!Regex.IsMatch(trialName, pattern))
            {
                trialName += "_" + trialNumber.ToString("D2");
            }

            var match = Regex.Match(trialName, pattern);

            trialName = match.Groups["trialname"].Value;
            trialNumber = int.Parse(match.Groups["trialnumber"].Value);

            // keep going until we find the next trial number
            while (Directory.EnumerateFiles(fullPath, trialName + "_" + trialNumber.ToString("D2") + "*.*").Any())
            {
                trialNumber++;
            }

            Settings.Default.TrialName = trialName + "_" + trialNumber.ToString("D2");
            Settings.Default.Save();
        }

        /// <summary>
        /// Wait async for the capture pipeline to complete processing frames
        /// </summary>
        /// <returns>An async task that we can wait for</returns>
        private async Task CompleteCapture(int maxWaitTime)
        {
            const int WaitMs = 1000;

            //--wait for queue to finish processing
            var waited = 0;

            while (!this.capturePipeline.IsCompleted && waited < maxWaitTime)
            {
                waited++;
                SystemMessage.Send(string.Format("Please wait. {1} frames remaining...", waited, this.capturePipeline.Count));
                this.QueueCount = this.capturePipeline.Count;
                await Task.Delay(WaitMs);
            }

            this.QueueCount = this.capturePipeline.Count;

            if (waited >= maxWaitTime && !this.capturePipeline.IsCompleted)
                throw new InvalidOperationException("Failed to complete capture in a timely manner. Data was not saved!");
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.dataOutputModule != null) this.dataOutputModule.Dispose();
                if (this.capturePipeline != null) this.capturePipeline.Dispose();
            }
        }

        public bool CanCaptureWII
        {
            get
            {
                return (this.WIILeftViewModel.Connected || this.WIIRightViewModel.Connected);
            }
        }
    }
}
