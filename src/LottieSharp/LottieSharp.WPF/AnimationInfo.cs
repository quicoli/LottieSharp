using System;

namespace LottieSharp.WPF
{
    public class AnimationInfo
    {
        public AnimationInfo(string version, TimeSpan duration, double fps, double inPoint, double outPoint)
        {
            Version = version;
            Duration = duration;
            Fps = fps;
            InPoint = inPoint;
            OutPoint = outPoint;
        }

        public string Version { get; private set; }
        public TimeSpan Duration { get; private set; }
        public double Fps { get; private set; }
        public double InPoint { get; private set; }
        public double OutPoint { get; private set; }
    }
}
