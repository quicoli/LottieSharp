using System.Collections.Generic;
using LottieSharp.Value;
using LottieSharp.Animation.Keyframe;

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
