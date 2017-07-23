using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace wiicapture.gui.Utils
{
    public class COPRenderData
    {
        public COPRenderData(object[] values)
        {
            if (values == null || values.Length < 4)
            {
                throw new ArgumentException(
                    "COPRenderData expects 4 or 5 double values to be passed" +
                    " in this order -> BoardDimensionX | Y, copX | copY, canvasWidth | Height, ellipseWidth | Height, [Invert]",
                    "values");
            }
            
            this.BoardDimensionMtrs = (float)values[0];
            this.COPValue = (double)values[1];
            this.CanvasDimensionDIPs = (double)values[2];
            this.IndicatorDimensionDIPs = (double)values[3];
            this.Scale = this.CanvasDimensionDIPs / this.BoardDimensionMtrs;

            if (values.Length == 5 && (bool)values[4])
            {
                this.COPValue *= -1;
            }
        }

        public double COPValue { get; set; }

        public double CanvasDimensionDIPs { get; set; }

        public double IndicatorDimensionDIPs { get; set; }

        public float BoardDimensionMtrs { get; set; }

        public double IndicatorPosition
        {
            get
            {
                var scaled = this.COPValue * this.Scale;
                return (this.CanvasDimensionDIPs / 2 - this.IndicatorDimensionDIPs / 2) + scaled;
            }
        }

        public double Scale { get; private set; }
    }
}
