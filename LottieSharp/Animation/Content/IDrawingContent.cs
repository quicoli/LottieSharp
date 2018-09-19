using SharpDX;

namespace LottieSharp.Animation.Content
{
    internal interface IDrawingContent : IContent
    {
        void Draw(BitmapCanvas canvas, Matrix3X3 parentMatrix, byte alpha);
        void GetBounds(ref RectangleF outBounds, Matrix3X3 parentMatrix);
    }
}