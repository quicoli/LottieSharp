using SharpDX;

/* Unmerged change from project 'LottieSharp (netcoreapp3.0)'
Before:
using System;
After:
using SharpDX;
*/
using SharpDX.Direct2D1;
using System;

namespace LottieSharp.Animation.Content
{
    internal class RadialGradient : Gradient, IDisposable
    {
        private readonly float _x0;
        private readonly float _y0;
        private readonly float _r;
        private readonly GradientStop[] _canvasGradientStopCollection;
        private RadialGradientBrush _canvasRadialGradientBrush;

        public RadialGradient(float x0, float y0, float r, Color[] colors, float[] positions)
        {
            _x0 = x0;
            _y0 = y0;
            _r = r;
            _canvasGradientStopCollection = new GradientStop[colors.Length];
            for (var i = 0; i < colors.Length; i++)
            {
                _canvasGradientStopCollection[i] = new GradientStop
                {
                    Color = colors[i],
                    Position = positions[i]
                };
            }
        }

        public override Brush GetBrush(RenderTarget renderTarget, byte alpha)
        {
            if (_canvasRadialGradientBrush == null || _canvasRadialGradientBrush.IsDisposed)
            {
                var center = new Vector2(_x0, _y0);
                center = LocalMatrix.Transform(center);

                var properties = new RadialGradientBrushProperties
                {
                    RadiusX = _r,
                    RadiusY = _r,
                    Center = center
                };

                var collection = new GradientStopCollection(renderTarget, _canvasGradientStopCollection, Gamma.Linear, ExtendMode.Clamp);
                //TODO: OID: property missed, Same for Linear 
                _canvasRadialGradientBrush = new RadialGradientBrush(renderTarget, properties, collection);
            }

            _canvasRadialGradientBrush.Opacity = alpha / 255f;

            return _canvasRadialGradientBrush;
        }

        private void Dispose(bool disposing)
        {
            if (_canvasRadialGradientBrush != null)
            {
                _canvasRadialGradientBrush.Dispose();
                _canvasRadialGradientBrush = null;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~RadialGradient()
        {
            Dispose(false);
        }
    }
}