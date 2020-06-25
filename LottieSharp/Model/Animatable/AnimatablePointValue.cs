using LottieSharp.Animation.Keyframe;

/* Unmerged change from project 'LottieSharp (netcoreapp3.0)'
Before:
using SharpDX;
using LottieSharp.Value;
After:
using LottieSharp.Value;
using SharpDX;
*/
using LottieSharp.Value;
using SharpDX;
using System.Collections.Generic;

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