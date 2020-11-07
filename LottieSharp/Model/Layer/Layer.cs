using LottieSharp.Model.Animatable;
using LottieSharp.Model.Content;
using LottieSharp.Value;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace LottieSharp.Model.Layer
{
    public class Layer : IDisposable
    {
        public enum LayerType
        {
            PreComp,
            Solid,
            Image,
            Null,
            Shape,
            Text,
            Unknown
        }

        public enum MatteType
        {
            None,
            Add,
            Invert,
            Unknown
        }

        private List<IContentModel> _shapes;
        private LottieComposition _composition;
        private readonly LayerType _layerType;
        private readonly MatteType _matteType;
        private bool disposedValue;

        public Layer(List<IContentModel> shapes, LottieComposition composition, string layerName, long layerId, LayerType layerType, long parentId, string refId, List<Mask> masks, AnimatableTransform transform, int solidWidth, int solidHeight, Color solidColor, float timeStretch, float startFrame, int preCompWidth, int preCompHeight, AnimatableTextFrame text, AnimatableTextProperties textProperties, List<Keyframe<float?>> inOutKeyframes, MatteType matteType, AnimatableFloatValue timeRemapping, bool hidden)
        {
            _shapes = shapes;
            _composition = composition;
            Name = layerName;
            Id = layerId;
            _layerType = layerType;
            ParentId = parentId;
            RefId = refId;
            Masks = masks;
            Transform = transform;
            SolidWidth = solidWidth;
            SolidHeight = solidHeight;
            SolidColor = solidColor;
            TimeStretch = timeStretch;
            StartFrame = startFrame;
            PreCompWidth = preCompWidth;
            PreCompHeight = preCompHeight;
            Text = text;
            TextProperties = textProperties;
            InOutKeyframes = inOutKeyframes;
            _matteType = matteType;
            TimeRemapping = timeRemapping;
            Hidden = hidden;
        }

        internal virtual LottieComposition Composition => _composition;

        internal virtual float TimeStretch { get; }

        internal virtual float StartFrame { get; }

        internal virtual float StartProgress => StartFrame / _composition.DurationFrames;

        internal virtual List<Keyframe<float?>> InOutKeyframes { get; }

        internal virtual long Id { get; }

        internal virtual string Name { get; }

        internal virtual string RefId { get; }

        internal virtual int PreCompWidth { get; }

        internal virtual int PreCompHeight { get; }

        internal virtual AnimatableTextFrame Text { get; }

        internal virtual AnimatableTextProperties TextProperties { get; }

        internal virtual AnimatableFloatValue TimeRemapping { get; }

        internal virtual bool Hidden { get; }

        internal virtual List<Mask> Masks { get; }

        internal virtual LayerType GetLayerType()
        {
            return _layerType;
        }

        internal virtual MatteType GetMatteType()
        {
            return _matteType;
        }

        internal virtual long ParentId { get; }

        internal virtual List<IContentModel> Shapes => _shapes;

        internal virtual AnimatableTransform Transform { get; }

        internal virtual Color SolidColor { get; }

        internal virtual int SolidHeight { get; }

        internal virtual int SolidWidth { get; }

        public override string ToString()
        {
            return ToString("");
        }

        internal virtual string ToString(string prefix)
        {
            var sb = new StringBuilder();
            sb.Append(prefix).Append(Name).Append("\n");
            var parent = _composition.LayerModelForId(ParentId);
            if (parent != null)
            {
                sb.Append("\t\tParents: ").Append(parent.Name);
                parent = _composition.LayerModelForId(parent.ParentId);
                while (parent != null)
                {
                    sb.Append("->").Append(parent.Name);
                    parent = _composition.LayerModelForId(parent.ParentId);
                }
                sb.Append(prefix).Append("\n");
            }
            if (Masks.Count > 0)
            {
                sb.Append(prefix).Append("\tMasks: ").Append(Masks.Count).Append("\n");
            }
            if (SolidWidth != 0 && SolidHeight != 0)
            {
                sb.Append(prefix).Append("\tBackground: ").Append(string.Format(CultureInfo.InvariantCulture, "{0}x{1} {2}\n", SolidWidth, SolidHeight, SolidColor));
            }
            if (_shapes.Count > 0)
            {
                sb.Append(prefix).Append("\tShapes:\n");
                foreach (var shape in _shapes)
                {
                    sb.Append(prefix).Append("\t\t").Append(shape).Append("\n");
                }
            }
            return sb.ToString();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _composition = null;

                    if (_shapes != null)
                    {
                        foreach (var shape in _shapes)
                            (shape as IDisposable)?.Dispose();

                        _shapes.Clear();
                        _shapes = null;
                    }

                    disposedValue = true;
                }
            }
        }
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}