using System;
namespace wiicapture.utils
{
    public interface ISensorInfo
    {
        double BottomLeft { get; set; }
        double BottomRight { get; set; }
        double TopLeft { get; set; }
        double TopRight { get; set; }
    }
}
