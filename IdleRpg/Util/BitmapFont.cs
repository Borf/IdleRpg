using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace IdleRpg.Util;

public class BitmapFont : IDisposable
{
    public Image Image;
    public int cellWidth { get; init; }
    public int cellHeight { get; init; }
    public char startCharacter { get; init; }
    public List<int> widths { get; init; } = [];

    public BitmapFont(string path)
    {
        Image = Image.Load(path + ".png");
        byte[] data = File.ReadAllBytes(path + ".dat");
        cellWidth = BitConverter.ToInt32(data, 8);
        cellHeight = BitConverter.ToInt32(data, 12);
        startCharacter = (char)data[16];
        for(int i = 17; i < data.Length; i++)
            widths.Add(data[i]);
    }




    public void Dispose() => Image.Dispose();

    public int CalculateWidth(string name)
    {
        int width = 0;
        for(int i = 0; i < name.Length; i++)
        {
            int charIndex = (int)(name[i] - startCharacter);
            if(charIndex < 0 || charIndex >= widths.Count)
                continue;
            width += widths[charIndex];
        }
        return width;
    }
}


public static class BitmapFontDrawExtensions
{
    public static void DrawBitmapText(this IImageProcessingContext ip, BitmapFont font, string text, SixLabors.ImageSharp.Point position, Color color)
    {
        int x = position.X;
        int y = position.Y;
        var startChar = font.startCharacter;
        int charsPerRow = font.Image.Width / font.cellWidth;
        for (int i = 0; i < text.Length; i++)
        {
            int charIndex = (int)(text[i] - startChar);
            if(charIndex < 0 || charIndex >= font.widths.Count)
                continue;
            int charWidth = font.widths[charIndex+startChar];
            var sourceRect = new Rectangle((charIndex % charsPerRow) * font.cellWidth, (charIndex / charsPerRow) * font.cellHeight, charWidth, font.cellHeight);
            var destRect = new Rectangle(x, y, charWidth+1, font.cellHeight);
            ip.DrawImage(font.Image, destRect.Location, sourceRect, 1.0f);
            x += charWidth;
        }
    }
}