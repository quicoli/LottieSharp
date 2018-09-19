using System.Collections.Generic;
using LottieSharp.Value;
using LottieSharp.Animation.Keyframe;
using SharpDX;

namespace LottieSharp.Model.Animatable
{
    public class AnimatableColorValue : BaseAnimatableValue<Color?, Color?>
    {
        public AnimatableColorValue(List<Keyframe<Color?>> keyframes) : base(keyframes)
        {
        }

        public override IBaseKeyframeAnimation<Color?, Color?> CreateAnimation()
        {
            return new ColorKeyframeAnimation(Keyframes);
        }
    }
}