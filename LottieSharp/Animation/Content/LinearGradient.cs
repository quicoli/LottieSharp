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
    internal class LinearGradient : Gradient, IDisposable
    {
        private readonly float _x0;
        private readonly float _y0;
        private readonly float _x1;
        private readonly float _y1;
        private readonly GradientStop[] _canvasGradientStopCollection;
        private LinearGradientBrush _canvasLinearGradientBrush;

        public LinearGradient(float x0, float y0, float x1, float y1, Color[] colors, float[] positions)
        {
            _x0 = x0;
            _y0 = y0;
            _x1 = x1;
            _y1 = y1;
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
            if (_canvasLinearGradientBrush == null || _canvasLinearGradientBrush.IsDisposed)
            {
                var startPoint = new Vector2(_x0, _y0);
                var endPoint = new Vector2(_x1, _y1);

                startPoint = LocalMatrix.Transform(startPoint);
                endPoint = LocalMatrix.Transform(endPoint);

                _canvasLinearGradientBrush = new LinearGradientBrush(renderTarget, new LinearGradientBrushProperties
                {
                    StartPoint = startPoint,
                    EndPoint = endPoint,
                }
                , new GradientStopCollection(renderTarget, _canvasGradientStopCollection, Gamma.Linear, ExtendMode.Clamp));

            }

            _canvasLinearGradientBrush.Opacity = alpha / 255f;

            return _canvasLinearGradientBrush;
        }

        private void Dispose(bool disposing)
        {
            if (_canvasLinearGradientBrush != null)
            {
                _canvasLinearGradientBrush.Dispose();
                _canvasLinearGradientBrush = null;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~LinearGradient()
        {
            Dispose(false);
        }
    }
}