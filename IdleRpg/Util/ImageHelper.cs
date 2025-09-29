using SixLabors.ImageSharp;

namespace IdleRpg.Util;

public static class ImageHelper
{
    public static Stream AsPngStream(this Image image)
    {
        var memoryStream = new MemoryStream();
        image.SaveAsPng(memoryStream);
        memoryStream.Position = 0;
        return memoryStream;
    }
}
