using System.Collections.Generic;
using System.IO;

namespace wiicapture.gui.Modules
{
    abstract class OutputFile
    {
        public static readonly string DefaultFilePrefix = "wiicapture.gui";

        private string defaultFileName;

        public OutputFile()
            : this(DefaultFilePrefix)
        {
            this.records = new List<RawGrfFileRecord>();
        }

        public OutputFile(string filename)
        {
            this.defaultFileName = filename;
        }
        
        public OutputFile(string fileName, RawGrfFileHeader header, List<RawGrfFileRecord> records)
            : this(fileName)
        {
            this.header = header;
            this.records = records;
        }


        public string Filename { get; protected set; }

        public RawGrfFileHeader header { get; protected set; }

        public List<RawGrfFileRecord> records { get; private set; }

        protected abstract string Extension
        {
            get;
        }

        public virtual bool Save(string path, string fileName)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            this.Filename = Path.Combine(path, fileName + this.Extension);

            this.InitHeader();

            if (this.header == null || this.records == null)
            {
                return false;
            }

            //--delete any existing file to avoid a clash
            if (File.Exists(this.Filename))
            {
                File.Delete(this.Filename);
            }

            //--create the stream writer
            using (var writer = new StreamWriter(new FileStream(this.Filename, FileMode.CreateNew)))
            {
                if (this.header != null)
                {
                    this.header.Save(writer);
                }

                if (this.records != null)
                {
                    //--save each record
                    foreach (var record in records)
                    {
                        record.Save(writer);
                    }
                }

                writer.Flush();
            }

            return true;
        }

        protected abstract void InitHeader();

        public void Add(OutputFrame data)
        {
            this.AddFileRecord(data);
        }

        protected abstract void AddFileRecord(OutputFrame data);
    }
}
