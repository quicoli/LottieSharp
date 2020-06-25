using LottieSharp.Animation.Keyframe;
using SharpDX;

namespace LottieSharp.Model.Animatable
{
    internal class AnimatableSplitDimensionPathValue : IAnimatableValue<Vector2?, Vector2?>
    {
        private readonly AnimatableFloatValue _animatableXDimension;
        private readonly AnimatableFloatValue _animatableYDimension;

        public AnimatableSplitDimensionPathValue(AnimatableFloatValue animatableXDimension, AnimatableFloatValue animatableYDimension)
        {
            _animatableXDimension = animatableXDimension;
            _animatableYDimension = animatableYDimension;
        }

        public IBaseKeyframeAnimation<Vector2?, Vector2?> CreateAnimation()
        {
            return new SplitDimensionPathKeyframeAnimation(_animatableXDimension.CreateAnimation(), _animatableYDimension.CreateAnimation());
        }
    }
}