using LottieSharp.Animation.Content;
using LottieSharp.Model.Animatable;
using LottieSharp.Model.Layer;
using SharpDX.Direct2D1;
using System.Collections.Generic;

namespace LottieSharp.Model.Content
{
    public class ShapeStroke : IContentModel
    {
        public enum LineCapType
        {
            Butt,
            Round,
            Unknown
        }

        internal static CapStyle LineCapTypeToPaintCap(LineCapType lineCapType)
        {
            switch (lineCapType)
            {
                case LineCapType.Butt:
                    return CapStyle.Flat;
                case LineCapType.Round:
                    return CapStyle.Round;
                case LineCapType.Unknown:
                default:
                    return CapStyle.Square;
            }
        }

        public enum LineJoinType
        {
            Miter,
            Round,
            Bevel
        }

        internal static LineJoin LineJoinTypeToPaintLineJoin(LineJoinType lineJoinType)
        {
            switch (lineJoinType)
            {
                case LineJoinType.Bevel:
                    return LineJoin.Bevel;
                case LineJoinType.Miter:
                    return LineJoin.Miter;
                case LineJoinType.Round:
                default:
                    return LineJoin.Round;
            }
        }

        public ShapeStroke(string name, AnimatableFloatValue offset, List<AnimatableFloatValue> lineDashPattern, AnimatableColorValue color, AnimatableIntegerValue opacity, AnimatableFloatValue width, LineCapType capType, LineJoinType joinType, float miterLimit)
        {
            Name = name;
            DashOffset = offset;
            LineDashPattern = lineDashPattern;
            Color = color;
            Opacity = opacity;
            Width = width;
            CapType = capType;
            JoinType = joinType;
            MiterLimit = miterLimit;
        }

        public IContent ToContent(LottieDrawable drawable, BaseLayer layer)
        {
            return new StrokeContent(drawable, layer, this);
        }

        internal virtual string Name { get; }

        internal virtual AnimatableColorValue Color { get; }

        internal virtual AnimatableIntegerValue Opacity { get; }

        internal virtual AnimatableFloatValue Width { get; }

        internal virtual List<AnimatableFloatValue> LineDashPattern { get; }

        internal virtual AnimatableFloatValue DashOffset { get; }

        internal virtual LineCapType CapType { get; }

        internal virtual LineJoinType JoinType { get; }

        internal virtual float MiterLimit { get; }
    }
}