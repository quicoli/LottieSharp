using LottieSharp.Value;
using System.Collections.Generic;

namespace LottieSharp.Animation.Keyframe
{
    internal abstract class KeyframeAnimation<T> : BaseKeyframeAnimation<T, T>
    {
        internal KeyframeAnimation(List<Keyframe<T>> keyframes) : base(keyframes)
        {
        }
    }
}
