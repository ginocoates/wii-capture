using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wiicapture.utils
{
    public class CalibrationData
    {
        public double X3 { get; set; }
        public double X2 { get; set; }
        public double X1 { get; set; }
        public double C { get; set; }

        public CalibrationData()
        {
            //--default calibration data outputs the raw signal
            this.X3 = 0;
            this.X2 = 0;
            this.X1 = 1;
            this.C = 0;
        }

        public double Calibrate(double input) {
            var output = input;

            output = (this.X3 * Math.Pow(input, 3))
                + (this.X2 * Math.Pow(input, 2))
                + (this.X1 * input)
                + this.C;

            return output;
        }
    }
}
