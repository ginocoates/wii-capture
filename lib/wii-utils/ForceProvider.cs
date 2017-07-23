using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using System.Windows;

namespace wiicapture.utils
{
    public abstract class ForceProvider : IForceProvider
    {
        public static double Gravity = 9.80665f;

        System.Threading.Timer timer;

        List<IObserver<IForceFrame>> observers;

        public ForceProvider()
        {
            observers = new List<IObserver<IForceFrame>>();
            this.Calibration = ForceProviderCalibration.Default();
        }

        ~ForceProvider()
        {
            this.Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.timer.Dispose();
            }
        }

        protected void ForceFrameArrived(IForceFrame frame)
        {
            if (!this.Connected) return;

            foreach (var observer in this.observers)
            {
                observer.OnNext(frame);
            }
        }

        public virtual string ID { get { return string.Empty; } }

        public int FrequencyHertz
        {
            get;
            private set;
        }


        public IForceFrame LastFrame { get; protected set; }

        public abstract double BoardX
        {
            get;
        }

        public abstract double BoardY
        {
            get;
        }

        public bool Connected
        {
            get;
            protected set;
        }

        public ForceProviderCalibration Calibration { get; private set; }

        public abstract bool Connect();

        public abstract void Disconnect();

        public bool Calibrate(string calibrationFilePath)
        {
            this.Calibration = ForceProviderCalibration.FromFile(calibrationFilePath);
            return true;
        }

        public abstract IForceFrame GetFrame();

        /// <summary>
        /// Calculate COP 
        /// Based on 1. Bartlett HL, Ting LH, Bingham JT (2014) Accuracy of force and center of pressure measures of the Wii Balance Board. Gait Posture 39:224–8. doi: 10.1016/j.gaitpost.2013.07.010
        /// </summary>
        /// <param name="forceTR">Top right sensor value</param>
        /// <param name="forceBR">Bottom Right sensor value</param>
        /// <param name="forceTL">Top Left sensor value</param>
        /// <param name="forceBL">Bottom Left Sensor VAlue</param>
        /// <param name="l">Length between wii board sensors on the x plane</param>
        /// <param name="w">Width between the board sensors on the y plane</param>
        /// <returns>The calculated COP</returns>
        protected Vector CalculateCOP(double forceTL, double forceTR, double forceBL, double forceBR, double l, double w)
        {
            var f = forceTR + forceBR + forceTL + forceBL;
            //--see https://www.ncbi.nlm.nih.gov/pmc/articles/PMC3842432/
            var copX = ((l / 2) * (((forceTR + forceBR) - (forceTL + forceBL)) / f)) + this.Calibration.Origin.X;
            var copY = ((w / 2) * (((forceTR + forceTL) - (forceBR + forceBL)) / f)) + this.Calibration.Origin.Y;

            return new Vector(copX, copY);
        }

        /// <summary>
        /// Calculate moments around the board center.
        /// Based on http://link.springer.com.ezp.lib.unimelb.edu.au/book/10.1007%2F978-3-662-04732-3
        /// </summary>
        /// <param name="cop">The current center of pressure</param>
        /// <param name="forceTR">The top right force</param>
        /// <param name="forceBR">The bottom right force</param>
        /// <param name="forceTL">The top left force</param>
        /// <param name="forceBL">The bottom left force</param> 
        /// <returns>The calculated torque</returns>
        protected Torque CalculateTorque(Vector cop, double forceTL, double forceTR, double forceBL, double forceBR)
        {
            var aX = cop.X;
            var aY = cop.Y;
            var aZ = 0;
            var Fx = 0;
            var Fy = 0;
            var Fz = (forceTR + forceBR + forceBL + forceTL) * ForceProvider.Gravity;

            //--mX is along the X axis (parallel with long edge of board).
            var mX = -(aY * Fz) + (aZ * Fx);

            //--mY is along the Y axis (parallel with short edge of board)           
            var mY = (aX * Fz) - (aZ * Fy);

            var mZ = (aX * Fy) - (aY * Fx);

            return new Torque { X = (double)mX, Y = (double)mY, Z = (double)mZ };
        }


        public IDisposable Subscribe(IObserver<IForceFrame> observer)
        {
            if (!this.observers.Contains(observer))
            {
                this.observers.Add(observer);
            }

            return new ForceUnsubscriber(this.observers, observer);
        }


        public abstract double BatteryLevel
        {
            get;
            protected set;
        }

        public abstract int BatteryLevelRaw
        {
            get;
            protected set;
        }
    }
}
