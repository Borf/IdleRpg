using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace IdleRpg.Game.Core;

public interface IImageGenerator<T>
{
    Image<Rgba32> GetImage(T source);
}

public interface IImageGenerator<T, R>
{
    Image<Rgba32> GetImage(T source, R options);
}
public interface IImageGenerator<T, R, S>
{
    Image<Rgba32> GetImage(T source, R options, S option2);
}