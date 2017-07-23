using System;
using System.Linq;
using System.Windows;
using wiicapture.utils;

namespace wiicapture.gui.Modules
{
    class RawGrfFileRecord
    {
        public static readonly string AbsoluteTimeFormat = @"dd-MM-yyyy hh:mm:ss:ffff";
        public static readonly string RelativeTimeFormat = @"hh\:mm\:ss\:ff";

        IForceFrame ForceFrame { get; set; }

        TimeSpan RelativeTime { get; set; }

        public RawGrfFileRecord(TimeSpan relativeTime, IForceFrame frame)
        {
            this.RelativeTime = relativeTime;
            this.ForceFrame = frame;
        }

        /// <summary>
        /// Return the timestamp and joint data as a single string.
        /// Used when writing data to file
        /// </summary>
        /// <returns>The complete record as a string</returns>
        public override string ToString()
        {
            return string.Join(",",
              new string[] {
                    this.ForceFrame.AbsoluteTime.ToString(AbsoluteTimeFormat),
                     this.RelativeTime.ToString(RelativeTimeFormat)
                }) + "," + string.Join(",", new[]{
                    this.ForceFrame.ForceN_Factory.BottomLeft,
                    this.ForceFrame.ForceN_Factory.TopLeft,
                    this.ForceFrame.ForceN_Factory.BottomRight,
                    this.ForceFrame.ForceN_Factory.TopRight,
                    this.ForceFrame.ForceN_Custom.BottomLeft,
                    this.ForceFrame.ForceN_Custom.TopLeft,
                    this.ForceFrame.ForceN_Custom.BottomRight,
                    this.ForceFrame.ForceN_Custom.TopRight,
                    this.ForceFrame.COP_Custom.X,
                    this.ForceFrame.COP_Custom.Y,
                    this.ForceFrame.Torque.X,
                    this.ForceFrame.Torque.Y,
                    this.ForceFrame.Torque.Z,
                    this.ForceFrame.WeightKG_Custom.BottomLeft
                        + this.ForceFrame.WeightKG_Custom.BottomRight
                        + this.ForceFrame.WeightKG_Custom.TopLeft
                        + this.ForceFrame.WeightKG_Custom.TopRight
                    });
        }


        /// <summary>
        /// Write the record to disk
        /// </summary>
        /// <param name="writer">The output file writer</param>
        public void Save(System.IO.StreamWriter writer)
        {
            writer.WriteLine(this.ToString());
        }
    }
}
