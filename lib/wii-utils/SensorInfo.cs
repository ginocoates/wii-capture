using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wiicapture.utils
{
    public class SensorInfo : wiicapture.utils.ISensorInfo
    {
        public double TopLeft { get; set; }
        public double TopRight { get; set; }
        public double BottomLeft { get; set; }
        public double BottomRight { get; set; }
    }
}
