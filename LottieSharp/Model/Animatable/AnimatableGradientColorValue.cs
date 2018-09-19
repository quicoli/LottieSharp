using System.Collections.Generic;
using LottieSharp.Value;
using LottieSharp.Animation.Keyframe;
using LottieSharp.Model.Content;

namespace LottieSharp.Model.Animatable
{
    public class AnimatableGradientColorValue : BaseAnimatableValue<GradientColor, GradientColor>
    {
        public AnimatableGradientColorValue(List<Keyframe<GradientColor>> keyframes) : base(keyframes)
        {
        }

        public override IBaseKeyframeAnimation<GradientColor, GradientColor> CreateAnimation()
        {
            return new GradientColorKeyframeAnimation(Keyframes);
        }
    }
}