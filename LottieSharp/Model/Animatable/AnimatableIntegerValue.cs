using LottieSharp.Animation.Keyframe;
using LottieSharp.Value;
using System.Collections.Generic;

namespace LottieSharp.Model.Animatable
{
    public class AnimatableIntegerValue : BaseAnimatableValue<int?, int?>
    {
        public AnimatableIntegerValue() : base(100)
        {
        }

        public AnimatableIntegerValue(List<Keyframe<int?>> keyframes) : base(keyframes)
        {
        }

        public override IBaseKeyframeAnimation<int?, int?> CreateAnimation()
        {
            return new IntegerKeyframeAnimation(Keyframes);
        }
    }
}