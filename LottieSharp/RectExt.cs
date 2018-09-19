using SharpDX;
using System;

namespace LottieSharp
{
    public static class RectExt
    {
        public static void Set(ref RectangleF rect, float left, float top, float right, float bottom)
        {
            rect.X = left;
            rect.Y = top;
            rect.Width = Math.Abs(right - left);
            rect.Height = Math.Abs(bottom - top);
        }

        public static void Set(ref RectangleF rect, RectangleF newRect)
        {
            rect.X = newRect.X;
            rect.Y = newRect.Y;
            rect.Width = newRect.Width;
            rect.Height = newRect.Height;
        }
    }
}
