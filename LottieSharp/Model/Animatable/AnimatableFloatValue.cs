using System.Collections.Generic;
using LottieSharp.Value;
using LottieSharp.Animation.Keyframe;

namespace LottieSharp.Model.Animatable
{
    public class AnimatableFloatValue : BaseAnimatableValue<float?, float?>
    {
        internal AnimatableFloatValue() : base(0f)
        {
        }

        public AnimatableFloatValue(List<Keyframe<float?>> keyframes) : base(keyframes)
        {
        }

        public override IBaseKeyframeAnimation<float?, float?> CreateAnimation()
        {
            return new FloatKeyframeAnimation(Keyframes);
        }
    }
}