using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.unimelb.aimss.WIIUtils
{
    public class DemoForceProvider : ForceProvider, IForceProvider
    {
        private Random random;

        public DemoForceProvider(int frequencyHertz) : base(frequencyHertz) 
        {
        
        }

        public override float WeightLeftKg
        {
            get { return randomForce(1); }
        }

        public override float WeightRightKg
        {
            get { return randomForce(2); }
        }

        public override float WeightTopLeftKg
        {
            get { return randomForce(3); }
        }

        public override float WeightTopRightKg
        {
            get { return randomForce(4); }
        }

        public override float WeightBottomLeftKg
        {
            get { return randomForce(5); }
        }

        public override float WeightBottomRightKg
        {
            get { return randomForce(6); }
        }

        public override float ForceBottomLeft { get { return randomForce(6); } }
        public override float ForceBottomRight { get { return randomForce(6); } }
        public override float ForceTopLeft { get { return randomForce(6); } }
        public override float ForceTopRight { get { return randomForce(6); } }

        public override float Length
        {
            get { return WiiBalanceBoardForceProvider.BoardLength; }
        }

        public override float Height
        {
            get { return WiiBalanceBoardForceProvider.BoardHeight; }
        }

        public override bool Connect() { return true; }

        public override void Disconnect()
        {            
        }

        public override bool Calibrate(string calibrationFilePath)
        {
            throw new NotImplementedException();
        }

        private float randomForce(int sensor)
        {
            if (null == random)
            {
                this.random = new Random();
            }

            if (sensor == 1) return 30.0f;
            if (sensor == 2) return 15.0f;

            var currentForce = new[] { 30.0f, 15.0f, 2.5f, 0.0f }[random.Next(0, 3)];
            return currentForce;
        }

        public override System.Drawing.PointF CentreOfPressure
        {
            get
            {
                return new System.Drawing.Point(-2, -6);
            }
        }

        public override System.Drawing.PointF CentreOfGravity
        {
            get
            {
                return new System.Drawing.Point(-2, 5);
            }
        }
       
    }
}
