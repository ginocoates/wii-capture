using System.Collections.Generic;
using System.Linq;

namespace wiicapture.gui.Modules
{
    class RawGrfFileHeader
    {
        public static readonly string[] Headers = new[] { "Absolute", "Relative", "BL", "TL", "BR", "TR", "BLC", "TLC", "BRC", "TRC", "COPX", "COPY", "MX", "MY", "MZ", "Weight" };
        /// <summary>
        /// Headers for the file, will be populated in the constructor
        /// </summary>
        static List<string> headers;

        public RawGrfFileHeader()
        {

            headers = new List<string>();

            headers.AddRange(Headers.ToList());
        }

        public void Save(System.IO.StreamWriter writer)
        {
            //--write the headers to file
            writer.WriteLine(string.Join(",", headers));
        }
    }
}
