using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using System.Xml.Linq;

namespace wiicapture.utils
{
    public class ForceProviderCalibration
    {
        //--elements
        private const string CurveElement = "Curve";
        private const string TLElement = "TL";
        private const string TRElement = "TR";
        private const string BLElement = "BL";
        private const string BRElement = "BR";
        private const string OriginElement = "Origin";
        private const string TareElement = "Tare";

        //--attriutes
        private const string X3Attribute = "x3";
        private const string X2Attribute = "x2";
        private const string X1Attribute = "x1";
        private const string CAttribute = "c";
        private const string XAttribute = "x";
        private const string YAttribute = "y";
        private const string ZAttribute = "z";
        private const string WeightAttribute = "weight";

        public CalibrationData TL { get; set; }
        public CalibrationData TR { get; set; }
        public CalibrationData BL { get; set; }
        public CalibrationData BR { get; set; }
        public Point3D Origin { get; set; }
        public double Tare { get; set; }

        public static ForceProviderCalibration Default()
        {
            return new ForceProviderCalibration
            {
                TL = new CalibrationData(),
                TR = new CalibrationData(),
                BL = new CalibrationData(),
                BR = new CalibrationData(),
                Origin = new Point3D()
            };
        }

        public static ForceProviderCalibration FromFile(string calibrationFilePath)
        {
            if (!File.Exists(calibrationFilePath))
            {
                throw new FileNotFoundException("File not found:" + calibrationFilePath);
            }

            ForceProviderCalibration calibration = new ForceProviderCalibration();

            var xdocument = XDocument.Load(calibrationFilePath);

            ParseSettings(calibration, xdocument);

            return calibration;
        }

        public static void ParseSettings(ForceProviderCalibration calibration, XDocument xdocument)
        {
            //--read individual configurations
            var curveNode = xdocument.Descendants(CurveElement).First();
            var tareNode = xdocument.Descendants(TareElement).First();
            var originNode = xdocument.Descendants(OriginElement).First();

            //--get sensor values
            var topLeftNode = curveNode.Descendants(TLElement).First();
            var topRightNode = curveNode.Descendants(TRElement).First();
            var bottomLeftNode = curveNode.Descendants(BLElement).First();
            var bottomRightNode = curveNode.Descendants(BRElement).First();

            calibration.TL = ProcessSensorCalibration(topLeftNode);
            calibration.TR = ProcessSensorCalibration(topRightNode);
            calibration.BL = ProcessSensorCalibration(bottomLeftNode);
            calibration.BR = ProcessSensorCalibration(bottomRightNode);

            calibration.Origin = ProcessOrigin(originNode);

            calibration.Tare = Double.Parse(tareNode.Attributes(WeightAttribute).First().Value.ToString());
        }

        private static Point3D ProcessOrigin(XElement originNode)
        {
            var x = Double.Parse(originNode.Attributes(XAttribute).First().Value);
            var y = Double.Parse(originNode.Attributes(YAttribute).First().Value);
            var z = Double.Parse(originNode.Attributes(ZAttribute).First().Value);

            return new Point3D
            {
                X = x,
                Y = y,
                Z = z
            };
        }

        private static CalibrationData ProcessSensorCalibration(XElement sensor)
        {
            var x3 = Double.Parse(sensor.Attributes(X3Attribute).First().Value);
            var x2 = Double.Parse(sensor.Attributes(X2Attribute).First().Value);
            var x = Double.Parse(sensor.Attributes(XAttribute).First().Value);
            var c = Double.Parse(sensor.Attributes(CAttribute).First().Value);

            var calibrationData = new CalibrationData
            {
                X3 = x3,
                X2 = x2,
                X1 = x,
                C = c
            };

            return calibrationData;
        }
    }
}
