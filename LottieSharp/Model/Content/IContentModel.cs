using LottieSharp.Animation.Content;
using LottieSharp.Model.Layer;

namespace LottieSharp.Model.Content
{
    public interface IContentModel
    {
        IContent ToContent(LottieDrawable drawable, BaseLayer layer);
    }
}
