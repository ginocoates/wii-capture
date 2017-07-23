namespace wiicapture.gui.Modules
{
    class RawGrfFile : OutputFile
    {
        public static readonly string RawBodyFileExtension = ".grf.csv";

        private string deviceID;

        protected override string Extension
        {
            get { return "." + deviceID + RawBodyFileExtension; }
        }

        public RawGrfFile(string deviceID)
        {
            this.deviceID = deviceID;           
        }

        protected override void InitHeader()
        {
            this.header = new RawGrfFileHeader();
        }

        protected override void AddFileRecord(OutputFrame data)
        {
            if (data == null || (data.LeftForce == null && data.RightForce == null)) return;

            var force = data.LeftForce;

            if (force == null || force.DeviceID != this.deviceID)
            {
                force = data.RightForce;
            }

            if (force == null || force.DeviceID != this.deviceID)
            {
                return;
            }

            this.records.Add(new RawGrfFileRecord(data.RelativeTime, force));
        }
    }
}
