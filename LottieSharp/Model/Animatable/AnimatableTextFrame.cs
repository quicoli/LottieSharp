using LottieSharp.Animation.Keyframe;
using LottieSharp.Value;
using System.Collections.Generic;

namespace LottieSharp.Model.Animatable
{
    public class AnimatableTextFrame : BaseAnimatableValue<DocumentData, DocumentData>
    {
        public AnimatableTextFrame(List<Keyframe<DocumentData>> keyframes) : base(keyframes)
        {
        }

        public override IBaseKeyframeAnimation<DocumentData, DocumentData> CreateAnimation()
        {
            return new TextKeyframeAnimation(Keyframes);
        }
    }
}
