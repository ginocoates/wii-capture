using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wiicapture.utils;

namespace wiicapture.gui.Modules
{
    class CaptureFrame
    {
        public IForceFrame LeftForce
        {
            get;
            set;
        }

        public IForceFrame RightForce
        {
            get;
            set;
        }

        public DateTime AbsoluteTime { get; set; }
        public TimeSpan RelativeTime { get; set; }
    }
}
