using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows;

namespace wiicapture.utils
{
    public interface IForceProvider : IObservable<IForceFrame>, IDisposable
    {
        string ID { get; }
        int FrequencyHertz { get; }
        IForceFrame LastFrame { get; }
        double BoardX { get; }
        double BoardY { get; }
        IForceFrame GetFrame();
        bool Connected { get; }
        bool Connect();
        void Disconnect();
        bool Calibrate(string calibrationFilePath);
        double BatteryLevel { get; }
        int BatteryLevelRaw { get; }
    }
}
