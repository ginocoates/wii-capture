using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wiicapture.utils
{
    public interface IForceListener
    {
        void ForceFrameArrived(ForceFrame frame);
    }
}
