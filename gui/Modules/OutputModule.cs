using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BackgroundPipeline;

namespace wiicapture.gui.Modules
{
    class OutputModule : IPipelineModule<CaptureFrame>, IList<OutputFrame>
    {
        public bool IsEnabled
        {
            get;
            set;
        }
             
        public IList<OutputFrame> OutputFrames { get; set; }

        public OutputModule()
        {
            this.OutputFrames = new List<OutputFrame>();
        }

        public int IndexOf(OutputFrame item)
        {
            return this.OutputFrames.IndexOf(item);
        }

        public void Insert(int index, OutputFrame item)
        {
            this.OutputFrames.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            this.OutputFrames.RemoveAt(index);
        }

        public OutputFrame this[int index]
        {
            get
            {
                return (OutputFrame)this.OutputFrames[index];
            }
            set
            {
                this.OutputFrames[index] = value;
            }
        }

        public bool Save(IList<OutputFile> outputFiles, string path, string fileName)
        {
            if (outputFiles == null || outputFiles.Count == 0)
            {
                return false;
            }

            var items = this.OrderBy(i => i.RelativeTime.TotalMilliseconds);

            foreach (OutputFile file in outputFiles)
            {
                foreach (var item in items)
                {
                    file.Add(item);
                }

                file.Save(path, fileName);
            }

            return true;
        }

        public void Add(OutputFrame item)
        {
            this.OutputFrames.Add(item);
        }

        public void Clear()
        {
            this.OutputFrames.Clear();
        }

        public bool Contains(OutputFrame item)
        {
            return this.OutputFrames.Contains(item);
        }

        public void CopyTo(OutputFrame[] array, int arrayIndex)
        {
            this.OutputFrames.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return this.OutputFrames.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(OutputFrame item)
        {
            if (!this.OutputFrames.Contains(item))
            {
                return false;
            }

            this.OutputFrames.Remove(item);
            return true;
        }

        public IEnumerator<OutputFrame> GetEnumerator()
        {
            return new OutputModuleEnumerator(this);
        }

        IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new OutputModuleEnumerator(this);
        }

        public bool Save(IList<OutputFile> outputFiles, string path)
        {
            return this.Save(outputFiles, path, "Trial1");
        }

        public CaptureFrame Process(CaptureFrame frame)
        {
            if (this.OutputFrames == null) return frame;

            var outputFrame = new OutputFrame
            {
                AbsoluteTime = frame.AbsoluteTime,
                RelativeTime = frame.RelativeTime,
                LeftForce = frame.LeftForce,
                RightForce = frame.RightForce
            };
            
            this.OutputFrames.Add(outputFrame);

            return frame;
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (this.OutputFrames != null)
            {
                this.OutputFrames.Clear();
                this.OutputFrames = null;
            }
        }
    }
}
