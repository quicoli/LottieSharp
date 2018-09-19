using System.Collections.Generic;
using SharpDX;
using LottieSharp.Value;
using LottieSharp.Animation.Keyframe;
using SharpDX;

namespace LottieSharp.Model.Animatable
{
    public class AnimatablePointValue : BaseAnimatableValue<Vector2?, Vector2?>
    {
        public AnimatablePointValue(List<Keyframe<Vector2?>> keyframes) : base(keyframes)
        {
        }

        public override IBaseKeyframeAnimation<Vector2?, Vector2?> CreateAnimation()
        {
            return new PointKeyframeAnimation(Keyframes);
        }
    }
}