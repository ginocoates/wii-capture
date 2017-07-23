using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wiicapture.gui.Modules
{
    class OutputModuleEnumerator : IEnumerator<OutputFrame>
    {
        private OutputModule collection;
        private OutputFrame current;
        private int index;

        public OutputModuleEnumerator(OutputModule collection)
        {
            this.collection = collection;
            index = -1;
        }

        public OutputFrame Current
        {
            get { return this.current; }
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        object System.Collections.IEnumerator.Current
        {
            get { return current; }
        }

        public bool MoveNext()
        {
            if (++index >= collection.Count)
            {
                return false;
            }
            else
            {
                current = collection[index];
            }

            return true;
        }

        public void Reset()
        {
            current = null;
            index = -1;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                collection = null;
                current = null;
                index = -1;
            }
        }
    }
}
