using LottieSharp.Animation.Content;
using SharpDX.Direct2D1;

namespace LottieSharp
{
    internal class DashPathEffect : PathEffect
    {
        private readonly float[] _intervals;
        private readonly float _phase;

        public DashPathEffect(float[] intervals, float phase)
        {
            _intervals = intervals;
            _phase = phase;
        }

        public override void Apply(StrokeStyle StrokeStyle, Paint paint)
        {
            if (paint.Style == Paint.PaintStyle.Stroke)
            {
                //TODO: OID: Custom dash style is not exists in SharpDX
                //StrokeStyle..CustomDashStyle = _intervals;
                //StrokeStyle.DashOffset = _phase;
            }
        }
    }
}