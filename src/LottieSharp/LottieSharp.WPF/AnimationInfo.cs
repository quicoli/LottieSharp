using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LottieSharp.WPF
{
    public class AnimationInfo : INotifyPropertyChanged
    {
        private string version;
        private TimeSpan duration;
        private double fps;
        private double inPoint;
        private double outPoint;

        public AnimationInfo(string version, TimeSpan duration, double fps, double inPoint, double outPoint)
        {
            Version = version;
            Duration = duration;
            Fps = fps;
            InPoint = inPoint;
            OutPoint = outPoint;
        }

        public string Version
        {
            get => version; private
            set
            {
                version = value;
                OnPropertyChanged();
            }
        }
        public TimeSpan Duration
        {
            get => duration;
            private set
            {
                duration = value;
                OnPropertyChanged();
            }
        }
        public double Fps
        {
            get => fps;
            private set
            {
                fps = value;
                OnPropertyChanged();
            }
        }
        public double InPoint
        {
            get => inPoint;
            private set
            {
                inPoint = value;
                OnPropertyChanged();
            }
        }
        public double OutPoint
        {
            get => outPoint;
            private set
            {
                outPoint = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        // Create the OnPropertyChanged method to raise the event
        // The calling member's name will be used as the parameter.
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
