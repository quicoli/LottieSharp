
using SharpDX;
using SharpDX.Direct2D1;
using System;

namespace LottieSharp.Animation.Content
{
    public class Paint: IDisposable
    {
        public static int AntiAliasFlag = 0b01;
        public static int FilterBitmapFlag = 0b10;
        private bool disposedValue;

        public int Flags { get; }

        public Paint(int flags)
        {
            Flags = flags;
        }

        public Paint()
            : this(0)
        {
        }

        public enum PaintStyle
        {
            Fill,
            Stroke
        }

        public byte Alpha
        {
            get => Color.A;
            set
            {
                var color = Color;
                color.A = value;
                Color = color;
            }
        }

        public Color Color { get; set; } = Color.Transparent;
        public PaintStyle Style { get; set; }
        public ColorFilter ColorFilter { get; set; }
        public CapStyle StrokeCap { get; set; }
        public LineJoin StrokeJoin { get; set; }
        public float StrokeMiter { get; set; }
        public float StrokeWidth { get; set; }
        public PathEffect PathEffect { get; set; }
        public PorterDuffXfermode Xfermode { get; set; }
        public Shader Shader { get; set; }
        public Typeface Typeface { get; set; }
        public float TextSize { get; set; }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    (ColorFilter as IDisposable)?.Dispose();
                    ColorFilter = null;

                    (PathEffect as IDisposable)?.Dispose();
                    PathEffect = null;
                    
                    (Xfermode as IDisposable)?.Dispose();
                    Xfermode = null;
                    
                    (Shader as IDisposable)?.Dispose();
                    Shader = null;
                    
                    (Typeface as IDisposable)?.Dispose();
                    Typeface = null;
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}