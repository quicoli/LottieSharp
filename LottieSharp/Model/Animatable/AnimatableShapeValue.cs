using System.Collections.Generic;
using LottieSharp.Value;
using LottieSharp.Animation.Keyframe;
using LottieSharp.Model.Content;

namespace LottieSharp.Model.Animatable
{
    public class AnimatableShapeValue : BaseAnimatableValue<ShapeData, Path>
    {
        public AnimatableShapeValue(List<Keyframe<ShapeData>> keyframes) : base(keyframes)
        {
        }

        public override IBaseKeyframeAnimation<ShapeData, Path> CreateAnimation()
        {
            return new ShapeKeyframeAnimation(Keyframes);
        }
    }
}