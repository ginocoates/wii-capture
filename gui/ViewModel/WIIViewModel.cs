using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows.Media.Imaging;
using wiicapture.gui.Messages;
using wiicapture.utils;
using Media = System.Windows.Media;

namespace wiicapture.gui.ViewModel
{
    public class WIIViewModel : ViewModelBase, IObserver<IForceFrame>
    {
        // Max length of path to capture when displaying on screen    
        public static readonly int MaxCOPPath = 25000;

        private IForceProvider forceProvider;

        private float calibrationX;

        private float calibrationY;

        private IForceFrame forceFrame;

        private bool capture;

        private bool connected;

        private ObservableCollection<PointF> copPath;

        private WriteableBitmap copPathImage;
        private double prevY;
        private double prevX;

        Media.Color backgroundColor = (Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#FF595959");
        Media.Color foregroundColor = Media.Colors.Yellow;

        private string deviceID;
        private string calibrationFilePath;
        private double weight;
        private double forceN;


        public string DeviceID
        {
            get { return this.deviceID; }
            set
            {
                this.deviceID = value;
                RaisePropertyChanged(() => this.DeviceID);
            }
        }

        public Media.ImageSource COPPathImage { get { return this.copPathImage; } }

        public bool Capture
        {
            get { return capture; }
            set
            {
                capture = value;

                RaisePropertyChanged(() => this.Capture);
            }
        }

        public IForceFrame ForceFrame
        {
            get { return this.forceFrame; }
            set
            {
                this.forceFrame = value;
                RaisePropertyChanged(() => this.ForceFrame);
            }
        }

        public float CalibrationX
        {
            get { return calibrationX; }
            set
            {
                calibrationX = value;
                RaisePropertyChanged(() => this.CalibrationX);
            }
        }

        public float CalibrationY
        {
            get { return calibrationY; }
            set
            {
                calibrationY = value;
                RaisePropertyChanged(() => this.CalibrationY);
            }
        }

        public bool Connected
        {
            get { return this.connected; }
            set
            {
                this.connected = value;
                RaisePropertyChanged(() => this.Connected);
            }
        }


        public ObservableCollection<PointF> COPPath
        {
            get
            {
                return this.copPath;
            }

            set
            {
                this.copPath = value;
                RaisePropertyChanged(() => this.COPPath);
            }
        }

        public double WeightKG
        {
            get
            {
                return this.weight;
            }

            set
            {
                this.weight = value;
                RaisePropertyChanged(() => this.WeightKG);
            }
        }

        public double ForceN
        {
            get
            {
                return this.forceN;
            }

            set
            {
                this.forceN = value;
                RaisePropertyChanged(() => this.ForceN);
            }
        }


        public RelayCommand<bool> ConnectCommand { get; private set; }
        public RelayCommand ClearCommand { get; set; }
        public RelayCommand CaptureWeightCommand { get; private set; }

        public double BoardRenderHeight { get { return WIIMetrics.BoardSensorsY * 1000; } }
        public double BoardRenderWidth { get { return WIIMetrics.BoardSensorsX * 1000; } }

        public WIIViewModel(string deviceID, string calibrationFilePath)
        {
            // TODO: Complete member initialization
            this.DeviceID = deviceID;
            this.calibrationFilePath = calibrationFilePath;

            this.ForceFrame = new ForceFrame();

            this.copPathImage = BitmapFactory.New((int)this.BoardRenderWidth, (int)this.BoardRenderHeight);
            this.copPathImage.Clear(backgroundColor);

            this.prevX = (this.BoardRenderWidth / 2);
            this.prevY = (this.BoardRenderHeight / 2);

            this.Connected = false;
            this.Capture = false;

            this.ConnectCommand = new RelayCommand<bool>((b) => this.Connect(b));
            this.ClearCommand = new RelayCommand(() => this.ClearCopPath());

            this.copPath = new ObservableCollection<PointF>();
            this.CaptureWeightCommand = new RelayCommand(() => this.CaptureWeight());

            this.Connect(true);
      }

        /// <summary>
        /// Capture the weight of the subject on the board by adding up the sensor values
        /// </summary>
        private void CaptureWeight()
        {
            if (this.ForceFrame == null || this.ForceFrame.WeightKG_Custom == null)
            {
                this.WeightKG = 0;
                return;
            }

            this.WeightKG = this.ForceFrame.WeightKG_Custom.BottomLeft
                + this.ForceFrame.WeightKG_Custom.BottomRight
                + this.ForceFrame.WeightKG_Custom.TopLeft
                + this.ForceFrame.WeightKG_Custom.TopRight;
        }

        /// <summary>
        /// Clear the render on screen
        /// </summary>
        private void ClearCopPath()
        {
            this.copPathImage.Clear(backgroundColor);
        }

        /// <summary>
        /// Force provider completed sending frames
        /// </summary>
        public void OnCompleted()
        {
            SystemMessage.Send("Force frame provider completed sending frames");
        }

        /// <summary>
        /// Error occured in the force provider
        /// </summary>
        /// <param name="error">The error that occured</param>
        public void OnError(Exception error)
        {
            SystemError.Send(string.Format("Error occured processing force frame:{0}", error.ToString()));
        }

        /// <summary>
        /// A force frame arrived
        /// </summary>
        /// <param name="frame">The frame</param>
        public void OnNext(IForceFrame frame)
        {
            this.ForceFrame = (IForceFrame)frame.Clone();

            if (App.Current == null) return;

            // process on UI frame as we will be drawing on screen
            App.Current.Dispatcher.Invoke((Action)delegate
            {
                this.ProcessFrame(this.ForceFrame);
            });
        }

        /// <summary>
        /// Process a force frame
        /// </summary>
        /// <param name="forceFrame"></param>
        private void ProcessFrame(IForceFrame forceFrame)
        {
            var copX = this.ForceFrame.COP_Custom.X * 1000;
            var copY = this.ForceFrame.COP_Custom.Y * 1000;

            //--calculate total force
            this.ForceN = forceFrame.ForceN_Custom_Tare.BottomLeft
                + forceFrame.ForceN_Custom_Tare.BottomRight
                + forceFrame.ForceN_Custom_Tare.TopLeft
                + forceFrame.ForceN_Custom_Tare.TopRight;

            this.WeightKG = forceFrame.WeightKG_Custom.BottomLeft
                + forceFrame.WeightKG_Custom.BottomRight
                + forceFrame.WeightKG_Custom.TopLeft
                + forceFrame.WeightKG_Custom.TopRight;

            //--transform y and x and draw on screen
            var xPos = (this.BoardRenderWidth / 2) + copX;
            var yPos = (this.BoardRenderHeight / 2) - copY;

            this.copPathImage.DrawLineAa((int)this.prevX, (int)this.prevY, (int)xPos, (int)yPos, foregroundColor);

            this.prevX = xPos;
            this.prevY = yPos;
        }

        /// <summary>
        /// Connect to the force provider
        /// </summary>
        /// <param name="connect">True means connect, false means disconnect</param>
        private void Connect(bool connect)
        {
            if (this.forceProvider == null)
            {
                try
                {
                    this.forceProvider = WiiBalanceBoardForceProvider.FindByID(this.deviceID);

                    if (!this.forceProvider.Calibrate(this.calibrationFilePath))
                    {
                        SystemMessage.Send("Failed to calibrate WII " + this.deviceID + " with file " + this.calibrationFilePath);
                    }
                    else
                    {
                        SystemMessage.Send("Calibrated WII " + this.deviceID + " with file " + this.calibrationFilePath);
                    }

                    // subscribe to events published from the WII board
                    this.forceProvider.Subscribe(this);
                }
                catch (InvalidOperationException ex)
                {
                    SystemMessage.Send("WII " + this.deviceID + " is not available! " + ex.Message);
                    this.Connected = false;
                    return;
                }
            }

            if (connect)
            {
                this.copPath.Clear();
                this.Connected = this.forceProvider.Connect();
            }
            else
            {
                this.forceProvider.Disconnect();
                this.Connected = false;
                SystemMessage.Send("WII disconnected!");
            }

            if (connect && this.Connected)
            {
                SystemMessage.Send("WII Connected Successfully - " + this.forceProvider.ID);
            }
            else if (connect && !this.Connected)
            {
                SystemMessage.Send("Failed to connect to WII!");
            }
        }
    }
}
