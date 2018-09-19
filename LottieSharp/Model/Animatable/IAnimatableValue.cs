using LottieSharp.Animation.Keyframe;

namespace LottieSharp.Model.Animatable
{
    public interface IAnimatableValue<out TK, TA>
    {
        IBaseKeyframeAnimation<TK, TA> CreateAnimation();
    }
}