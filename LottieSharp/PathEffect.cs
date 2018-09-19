using LottieSharp.Animation.Content;
using SharpDX.Direct2D1;

namespace LottieSharp
{
    public abstract class PathEffect
    {
        public abstract void Apply(StrokeStyle StrokeStyle, Paint paint);
    }
}