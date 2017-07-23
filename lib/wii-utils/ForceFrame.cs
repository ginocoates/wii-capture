using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows;

namespace wiicapture.utils
{
    [Serializable]
    public class ForceFrame : IForceFrame
    {
        /// <summary>
        /// The WII HID serial device ID that generated this frame
        /// </summary>
        public string DeviceID { get; set; }

        /// <summary>
        /// Sensor output for calibration purposes, assuming the board is inverted
        /// </summary>     
        public ISensorInfo Volts { get; set; }

        /// <summary>
        /// Sensor output using the factory calibration mechanism
        /// </summary>
        public ISensorInfo WeightKG_Factory { get; set; }
        public ISensorInfo ForceN_Factory { get; set; }

        /// <summary>
        /// Sensor output using our custom calibration mechanism
        /// </summary>
        public ISensorInfo WeightKG_Custom { get; set; }
        public ISensorInfo ForceN_Custom { get; set; }
        public ISensorInfo ForceN_Custom_Tare { get; set; }

        /// <summary>
        /// Calibrated and uncalibrated COP values
        /// </summary>
        public Vector COP_Custom { get; set; }
        public Vector COP_Factory { get; set; }

        public Torque Torque { get; set; }

        public DateTime AbsoluteTime { get; set; }

        public object Clone()
        {
            return new ForceFrame
            {
                DeviceID = this.DeviceID,
                AbsoluteTime = this.AbsoluteTime,
                ForceN_Custom = this.ForceN_Custom,    
                ForceN_Custom_Tare = this.ForceN_Custom_Tare,
                WeightKG_Custom = this.WeightKG_Custom,              
                COP_Custom = this.COP_Custom,
                COP_Factory = this.COP_Factory,
                ForceN_Factory = this.ForceN_Factory,
                Volts = this.Volts,
                WeightKG_Factory = this.WeightKG_Factory,
                FactoryCalibrationKG0 = this.FactoryCalibrationKG0,
                FactoryCalibrationKG17 = this.FactoryCalibrationKG17,
                FactoryCalibrationKG34 = this.FactoryCalibrationKG34,
                Torque = this.Torque
            };
        }
        
        public ISensorInfo FactoryCalibrationKG0
        {
            get;
            set;
        }

        public ISensorInfo FactoryCalibrationKG17
        {
            get;
            set;
        }

        public ISensorInfo FactoryCalibrationKG34
        {
            get;
            set;
        }
    }
}
