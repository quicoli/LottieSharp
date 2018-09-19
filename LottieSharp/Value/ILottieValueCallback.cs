using LottieSharp.Animation.Keyframe;

namespace LottieSharp.Value
{
    public interface ILottieValueCallback<T>
    {
        void SetAnimation(IBaseKeyframeAnimation animation);

        T GetValue(LottieFrameInfo<T> frameInfo);

        T GetValueInternal(
            float startFrame,
            float endFrame,
            T startValue,
            T endValue,
            float linearKeyframeProgress,
            float interpolatedKeyframeProgress,
            float overallProgress
        );
    }
}