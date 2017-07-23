using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WiimoteLib;
using System.Diagnostics;
using System.IO;
using System.Collections;
using System.Windows;

namespace wiicapture.utils
{
    public class WiiBalanceBoardForceProvider : ForceProvider, IForceProvider
    {
        private static double InterpolationFraction = 17;

        private Wiimote balanceBoard;

        private BalanceBoardState state;

        public static List<IForceProvider> FindAll()
        {
            List<IForceProvider> providers = new List<IForceProvider>();

            try
            {
                var wiiMotes = new WiimoteLib.WiimoteCollection();

                wiiMotes.FindAllWiimotes();

                foreach (var wiiMote in wiiMotes)
                {
                    providers.Add(new WiiBalanceBoardForceProvider(wiiMote));
                }
            }
            catch (WiimoteNotFoundException ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return providers;
        }

        public WiiBalanceBoardForceProvider(Wiimote balanceBoard)
        {
            if (balanceBoard == null)
            {
                throw new ArgumentNullException("balanceboard");
            }

            this.balanceBoard = balanceBoard;
            this.balanceBoard.WiimoteChanged += (sender, args) =>
            {
                this.state = args.WiimoteState.BalanceBoardState;

                this.BatteryLevel = args.WiimoteState.Battery;
                this.BatteryLevelRaw = (int)args.WiimoteState.BatteryRaw;

                var frame = this.GetFrame();

                if (LastFrame != null && frame.AbsoluteTime.CompareTo(LastFrame.AbsoluteTime) == 0)
                {
                    return;
                }

                this.LastFrame = frame;
                this.ForceFrameArrived(frame);
            };
        }

        public override double BatteryLevel { get; protected set; }
        public override int BatteryLevelRaw { get; protected set; }

        public override IForceFrame GetFrame()
        {
            var frame = new ForceFrame
            {
                DeviceID = this.ID,
                AbsoluteTime = DateTime.Now
            };

            // Factory calibration data
            frame.FactoryCalibrationKG0 = new SensorInfo
            {
                TopLeft = this.state.CalibrationInfo.Kg0.TopLeft,
                TopRight = this.state.CalibrationInfo.Kg0.TopRight,
                BottomLeft = this.state.CalibrationInfo.Kg0.BottomLeft,
                BottomRight = this.state.CalibrationInfo.Kg0.BottomRight
            };

            frame.FactoryCalibrationKG17 = new SensorInfo
            {
                TopLeft = this.state.CalibrationInfo.Kg17.TopLeft,
                TopRight = this.state.CalibrationInfo.Kg17.TopRight,
                BottomLeft = this.state.CalibrationInfo.Kg17.BottomLeft,
                BottomRight = this.state.CalibrationInfo.Kg17.BottomRight
            };

            frame.FactoryCalibrationKG34 = new SensorInfo
            {
                TopLeft = this.state.CalibrationInfo.Kg34.TopLeft,
                TopRight = this.state.CalibrationInfo.Kg34.TopRight,
                BottomLeft = this.state.CalibrationInfo.Kg34.BottomLeft,
                BottomRight = this.state.CalibrationInfo.Kg34.BottomRight
            };

            // Factory Force DAta
            frame.Volts = new SensorInfo
            {
                TopLeft = this.state.SensorValuesRaw.TopLeft,
                TopRight = this.state.SensorValuesRaw.TopRight,
                BottomLeft = this.state.SensorValuesRaw.BottomLeft,
                BottomRight = this.state.SensorValuesRaw.BottomRight
            };

            frame.WeightKG_Factory = new SensorInfo
            {
                TopLeft = FactoryVoltsToKG(InterpolationFraction,
                       this.state.SensorValuesRaw.TopLeft,
                       this.state.CalibrationInfo.Kg0.TopLeft,
                       this.state.CalibrationInfo.Kg17.TopLeft,
                       this.state.CalibrationInfo.Kg34.TopLeft),
                TopRight = FactoryVoltsToKG(InterpolationFraction,
                       this.state.SensorValuesRaw.TopRight,
                       this.state.CalibrationInfo.Kg0.TopRight,
                       this.state.CalibrationInfo.Kg17.TopRight,
                       this.state.CalibrationInfo.Kg34.TopRight),
                BottomLeft = FactoryVoltsToKG(InterpolationFraction,
                        this.state.SensorValuesRaw.BottomLeft,
                        this.state.CalibrationInfo.Kg0.BottomLeft,
                        this.state.CalibrationInfo.Kg17.BottomLeft,
                        this.state.CalibrationInfo.Kg34.BottomLeft),
                BottomRight = FactoryVoltsToKG(InterpolationFraction,
                        this.state.SensorValuesRaw.BottomRight,
                        this.state.CalibrationInfo.Kg0.BottomRight,
                        this.state.CalibrationInfo.Kg17.BottomRight,
                        this.state.CalibrationInfo.Kg34.BottomRight)
            };

            frame.ForceN_Factory = new SensorInfo
            {
                TopLeft = KGToNewtons(frame.WeightKG_Factory.TopLeft),
                TopRight = KGToNewtons(frame.WeightKG_Factory.TopRight),
                BottomLeft = KGToNewtons(frame.WeightKG_Factory.BottomLeft),
                BottomRight = KGToNewtons(frame.WeightKG_Factory.BottomRight)
            };

            frame.COP_Factory = this.CalculateCOP(
                frame.ForceN_Factory.TopLeft,
                frame.ForceN_Factory.TopRight,
                frame.ForceN_Factory.BottomLeft,
                frame.ForceN_Factory.BottomRight,
                this.BoardX,
                this.BoardY);

            // custom calculations
            frame.ForceN_Custom = new SensorInfo
            {
                TopLeft = this.Calibration.TL.Calibrate(this.state.SensorValuesRaw.TopLeft),
                TopRight = this.Calibration.TR.Calibrate(this.state.SensorValuesRaw.TopRight),
                BottomLeft = this.Calibration.BL.Calibrate(this.state.SensorValuesRaw.BottomLeft),
                BottomRight = this.Calibration.BR.Calibrate(this.state.SensorValuesRaw.BottomRight)
            };

            var sensorTare = this.KGToNewtons(this.Calibration.Tare) / 4;

            frame.ForceN_Custom_Tare = new SensorInfo
            {
                TopLeft = frame.ForceN_Custom.TopLeft - sensorTare,
                TopRight = frame.ForceN_Custom.TopRight - sensorTare,
                BottomLeft = frame.ForceN_Custom.BottomLeft - sensorTare,
                BottomRight = frame.ForceN_Custom.BottomRight - sensorTare
            };

            frame.WeightKG_Custom = new SensorInfo
            {
                TopLeft = NewtonsToKG(frame.ForceN_Custom_Tare.TopLeft),
                TopRight = NewtonsToKG(frame.ForceN_Custom_Tare.TopRight),
                BottomLeft = NewtonsToKG(frame.ForceN_Custom_Tare.BottomLeft),
                BottomRight = NewtonsToKG(frame.ForceN_Custom_Tare.BottomRight)
            };

            frame.COP_Custom = this.CalculateCOP(
               frame.ForceN_Custom_Tare.TopLeft,
               frame.ForceN_Custom_Tare.TopRight,
               frame.ForceN_Custom_Tare.BottomLeft,
               frame.ForceN_Custom_Tare.BottomRight,
               this.BoardX,
               this.BoardY);

            frame.Torque = CalculateTorque(frame.COP_Custom,
               frame.ForceN_Custom_Tare.TopLeft,
               frame.ForceN_Custom_Tare.TopRight,
               frame.ForceN_Custom_Tare.BottomLeft,
               frame.ForceN_Custom_Tare.BottomRight);

            return frame;
        }

        private double KGToNewtons(double kilograms)
        {
            return kilograms * ForceProvider.Gravity;
        }

        private double NewtonsToKG(double newtons)
        {
            return newtons / ForceProvider.Gravity;
        }

        private double VoltsToKG(double interpolationThreshold, short volts)
        {
            return FactoryVoltsToKG(
                                   interpolationThreshold,
                                   volts,
                                   this.state.CalibrationInfo.Kg0.BottomLeft,
                                   this.state.CalibrationInfo.Kg17.BottomLeft,
                                   this.state.CalibrationInfo.Kg34.BottomLeft);
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposing)
            {
                this.Disconnect();
            }

            base.Dispose(disposing);
        }

        public override string ID
        {
            get
            {
                return this.balanceBoard.HIDSerial;
            }
        }

        public override double BoardX
        {
            get { return WIIMetrics.BoardSensorsX; }
        }

        public override double BoardY
        {
            get { return WIIMetrics.BoardSensorsY; }
        }

        public override bool Connect()
        {
            if (this.Connected) return true;

            try
            {
                if (this.balanceBoard != null)
                {
                    this.balanceBoard.Connect();
                    this.balanceBoard.SetLEDs(1);
                    this.Connected = true;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Wii Balance Board Not Found! {0}", ex.Message);
                this.Connected = false;
            }

            return this.Connected;
        }

        public override void Disconnect()
        {
            if (!this.Connected) return;

            try
            {
                if (this.balanceBoard != null)
                {
                    this.balanceBoard.Disconnect();
                    this.balanceBoard.SetLEDs(0);
                    this.Connected = false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error during Wii disconnect. {0}", ex.Message);
            }
        }

        public static IForceProvider FindByID(string deviceID)
        {
            var collection = FindAll();
            var found = collection.FirstOrDefault(b => b.ID.Equals(deviceID));
            if (found == null)
            {
                throw new InvalidOperationException("WII not found with device ID " + deviceID);
            }

            return found;
        }

        private double FactoryVoltsToKG(double threshold, short sensor, short min, short mid, short max)
        {
            if (max == mid || mid == min)
                return 0;

            if (sensor < mid)
                return threshold * ((double)(sensor - min) / (mid - min));
            else
                return threshold * ((double)(sensor - mid) / (max - mid)) + threshold;
        }
    }
}
