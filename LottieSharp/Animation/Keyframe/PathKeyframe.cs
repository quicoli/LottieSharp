using LottieSharp.Value;
using SharpDX;

namespace LottieSharp.Animation.Keyframe
{
    public class PathKeyframe : Keyframe<Vector2?>
    {
        private readonly Keyframe<Vector2?> _pointKeyFrame;

        public PathKeyframe(LottieComposition composition, Keyframe<Vector2?> keyframe)
            : base(composition, keyframe.StartValue, keyframe.EndValue, keyframe.Interpolator, keyframe.StartFrame, keyframe.EndFrame)
        {
            _pointKeyFrame = keyframe;
            CreatePath();
        }

        internal void CreatePath()
        {
            var equals = EndValue != null && StartValue != null && StartValue.Equals(EndValue.Value);
            if (EndValue != null && !equals)
            {
                Path = Utils.Utils.CreatePath(StartValue.Value, EndValue.Value, _pointKeyFrame.PathCp1, _pointKeyFrame.PathCp2);
            }
        }

        /// <summary>
        /// This will be null if the startValue and endValue are the same.
        /// </summary>
        internal Path Path { get; private set; }
    }
}