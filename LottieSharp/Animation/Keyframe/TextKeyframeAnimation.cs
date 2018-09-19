using System.Collections.Generic;
using LottieSharp.Model;
using LottieSharp.Value;

namespace LottieSharp.Animation.Keyframe
{
    internal class TextKeyframeAnimation : KeyframeAnimation<DocumentData>
    {
        internal TextKeyframeAnimation(List<Keyframe<DocumentData>> keyframes) : base(keyframes)
        {
        }

        public override DocumentData GetValue(Keyframe<DocumentData> keyframe, float keyframeProgress)
        {
            return keyframe.StartValue;
        }
    }
}
