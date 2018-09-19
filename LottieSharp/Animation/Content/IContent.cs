using System.Collections.Generic;

namespace LottieSharp.Animation.Content
{
    public interface IContent
    {
        string Name { get; }

        void SetContents(List<IContent> contentsBefore, List<IContent> contentsAfter);
    }
}