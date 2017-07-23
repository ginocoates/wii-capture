using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace wiicapture.utils
{
    class ForceUnsubscriber : IDisposable
    {
        private List<IObserver<IForceFrame>> observers;
        private IObserver<IForceFrame> observer;

        public ForceUnsubscriber(List<IObserver<IForceFrame>> observers, IObserver<IForceFrame> observer)
        {
            // TODO: Complete member initialization
            this.observers = observers;
            this.observer = observer;
        }

        public void Dispose()
        {
            if (observer != null) observers.Remove(observer);
        }
    }
}
