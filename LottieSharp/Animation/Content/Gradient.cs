using SharpDX.Direct2D1;

namespace LottieSharp.Animation.Content
{
    internal abstract class Gradient : Shader
    {
        public abstract Brush GetBrush(RenderTarget renderTarget, byte alpha);
    }
}