using LottieSharp.Animation.Content;
using SharpDX.Direct2D1;

namespace LottieSharp
{
    public abstract class ColorFilter
    {
        public abstract Brush Apply(BitmapCanvas dst, Brush brush);
    }
}