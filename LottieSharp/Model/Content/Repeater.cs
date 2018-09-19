using LottieSharp.Animation.Content;
using LottieSharp.Model.Animatable;
using LottieSharp.Model.Layer;

namespace LottieSharp.Model.Content
{
    public class Repeater : IContentModel
    {
        public Repeater(string name, AnimatableFloatValue copies, AnimatableFloatValue offset, AnimatableTransform transform)
        {
            Name = name;
            Copies = copies;
            Offset = offset;
            Transform = transform;
        }

        internal virtual string Name { get; }

        internal virtual AnimatableFloatValue Copies { get; }

        internal virtual AnimatableFloatValue Offset { get; }

        internal virtual AnimatableTransform Transform { get; }

        public IContent ToContent(LottieDrawable drawable, BaseLayer layer)
        {
            return new RepeaterContent(drawable, layer, this);
        }
    }
}
