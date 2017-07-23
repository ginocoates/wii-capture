using System;
using System.Windows;
namespace wiicapture.utils
{
    public interface IForceFrame : ICloneable
    {
        string DeviceID { get; set; }
        DateTime AbsoluteTime { get; set; }
        Torque Torque { get; set; }
                
        /// <summary>
        /// Calibrated and uncalibrated COP values
        /// </summary>
        Vector COP_Factory { get; set; }
        Vector COP_Custom { get; set; }

        /// <summary>
        /// Sensor output for calibration purposes, assuming the board is inverted
        /// </summary>
        ISensorInfo Volts { get; set; }
        
        /// <summary>
        /// Sensor output using the factory calibration mechanism
        /// </summary>
        ISensorInfo WeightKG_Factory { get; set; }
        ISensorInfo ForceN_Factory { get; set; }

        /// <summary>
        /// Sensor output using our custom calibration mechanism
        /// </summary>
        ISensorInfo WeightKG_Custom { get; set; }
        ISensorInfo ForceN_Custom { get; set; }
        ISensorInfo ForceN_Custom_Tare { get; set; }

        /// <summary>
        /// Factory calibration data for the sensor
        /// </summary>
        ISensorInfo FactoryCalibrationKG0 { get; set; }
        ISensorInfo FactoryCalibrationKG17 { get; set; }
        ISensorInfo FactoryCalibrationKG34 { get; set; }
    }
}
