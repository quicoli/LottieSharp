using LottieSharp.WPF.Transforms;
using SkiaSharp;
using SkiaSharp.Skottie;
using SkiaSharp.Views.Desktop;
using SkiaSharp.Views.WPF;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace LottieSharp.WPF
{
    /// <summary>
    /// A WPF control for displaying and controlling Lottie (JSON-based) animations using SkiaSharp.
    /// </summary>
    /// <example>
    /// Example usage in XAML:
    /// <code>
    /// <lottie:LottieAnimationView FileName="Assets/animation.json" AutoPlay="True" RepeatCount="-1" />
    /// </code>
    /// </example>
    public class LottieAnimationView : SKElement, IDisposable
    {
        private readonly Stopwatch? watch = new();
        private int _frameCount;
        private Animation animation;
        private Storyboard storyboard;
        System.Windows.Resources.StreamResourceInfo? resourceInfo;
        private bool disposedValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="LottieAnimationView"/> class.
        /// </summary>
        public LottieAnimationView()
        {

        }

        /// <summary>
        /// Handles property changes to update animation playback based on visibility and enabled state.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property == VisibilityProperty || e.Property == IsEnabledProperty || e.Property == IsVisibleProperty)
            {
                if (EnsureVisibleAndEnabled())
                {
                    if (animation != null && (AutoPlay || IsPlaying))
                    {
                        PlayAnimation();
                    }
                }
                else
                {
                    ResumeAnimation();
                }
            }

        }

        /// <summary>
        /// Checks if the control is visible and enabled.
        /// </summary>
        /// <returns>True if visible and enabled; otherwise, false.</returns>
        private bool EnsureVisibleAndEnabled()
        {
            return Visibility == Visibility.Visible && IsEnabled && IsVisible;
        }

        /// <summary>
        /// Gets or sets information about the loaded animation (version, duration, fps, etc).
        /// </summary>
        public AnimationInfo Info
        {
            get { return (AnimationInfo)GetValue(InfoProperty); }
            set { SetValue(InfoProperty, value); }
        }

        /// <summary>
        /// Occurs when the animation is stopped.
        /// </summary>
        public event EventHandler OnStop;

        /// <summary>
        /// Gets or sets the file path to the Lottie JSON animation.
        /// </summary>
        /// <example>
        /// <code>
        /// lottieView.FileName = "Assets/animation.json";
        /// </code>
        /// </example>
        public string FileName
        {
            get => (string)GetValue(FileNameProperty);
            set => SetValue(FileNameProperty, value);
        }

        /// <summary>
        /// Gets or sets the resource URI to the Lottie JSON animation (for embedded resources).
        /// </summary>
        /// <example>
        /// <code>
        /// lottieView.ResourcePath = "pack://application:,,,/YourAssembly;component/Assets/animation.json";
        /// </code>
        /// </example>
        public string ResourcePath
        {
            get => (string)GetValue(ResourcePathProperty);
            set => SetValue(ResourcePathProperty, value);
        }


        public double CurrentFrame
        {
            get { return (double)GetValue(CurrentFrameProperty); }
            private set { SetValue(CurrentFrameProperty, value); }
        }

        public static readonly DependencyProperty CurrentFrameProperty =
            DependencyProperty.Register("CurrentFrame", typeof(double), typeof(LottieAnimationView), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender));


        public int CurrentFps
        {
            get { return (int)GetValue(CurrentFpsProperty); }
            private set { SetValue(CurrentFpsProperty, value); }
        }

        public static readonly DependencyProperty CurrentFpsProperty =
            DependencyProperty.Register("CurrentFps", typeof(int), typeof(LottieAnimationView), new PropertyMetadata(0));

        /// <summary>
        /// Starts or resumes the animation playback.
        /// </summary>
        /// <example>
        /// <code>
        /// lottieView.PlayAnimation();
        /// </code>
        /// </example>
        public virtual void PlayAnimation()
        {
            if (EnsureVisibleAndEnabled())
            {
                if (storyboard != null)
                {
                    if (_isResume)
                    {
                        storyboard.Resume();
                        watch.Restart();
                    }
                    else
                    {
                        storyboard.Begin();
                        watch.Start();
                    }
                    IsPlaying = true;
                    _isResume = false;
                }
            }
            else
            {
                storyboard?.Pause();
            }
        }

        private bool _isResume;
        public void ResumeAnimation()
        {
            storyboard?.Resume();
            watch.Stop();
            _isResume = true;
        }

        /// <summary>
        /// Stops the animation playback and resets the timer.
        /// </summary>
        /// <example>
        /// <code>
        /// lottieView.StopAnimation();
        /// </code>
        /// </example>
        public virtual void StopAnimation()
        {
            storyboard?.Stop();
            watch?.Stop();
            IsPlaying = false;
            _isResume = false;
            OnStop?.Invoke(this, null);
        }

        /// <summary>
        /// Gets or sets the number of times the animation should repeat.
        /// </summary>
        /// <example>
        /// <code>
        /// lottieView.RepeatBehavior = RepeatBehavior.Forever; // Infinite loop
        /// </code>
        /// </example>
        public RepeatBehavior RepeatBehavior
        {
            get { return (RepeatBehavior)GetValue(RepeatBehaviorProperty); }
            set { SetValue(RepeatBehaviorProperty, value); }
        }

        /// <summary>
        /// Identifies the RepeatBehavior dependency property.
        /// </summary>
        public static readonly DependencyProperty RepeatBehaviorProperty =
            DependencyProperty.Register("RepeatBehavior", typeof(RepeatBehavior), typeof(LottieAnimationView), new PropertyMetadata(new RepeatBehavior(1), RepeatBehaviorChangedCallback));

        private static void RepeatBehaviorChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is LottieAnimationView lottieAnimationView && lottieAnimationView.storyboard != null)
            {
                lottieAnimationView.storyboard.RepeatBehavior = (RepeatBehavior)e.NewValue;
            }
        }

        /// <summary>
        /// Gets or sets the transform used to scale or center the animation.
        /// </summary>
        /// <example>
        /// <code>
        /// lottieView.AnimationScale = new CenterTransform { ScaleX = 2, ScaleY = 2 };
        /// </code>
        /// </example>
        public AnimationTransformBase AnimationScale
        {
            get { return (AnimationTransformBase)GetValue(AnimationScaleProperty); }
            set { SetValue(AnimationScaleProperty, value); }
        }

        /// <summary>
        /// Identifies the AnimationScale dependency property.
        /// </summary>
        public static readonly DependencyProperty AnimationScaleProperty =
            DependencyProperty.Register("AnimationScale", typeof(AnimationTransformBase), typeof(LottieAnimationView), new PropertyMetadata(default(AnimationTransformBase)));

        /// <summary>
        /// Gets or sets whether the animation should start playing automatically when loaded.
        /// </summary>
        /// <example>
        /// <code>
        /// lottieView.AutoPlay = true;
        /// </code>
        /// </example>
        public bool AutoPlay
        {
            get { return (bool)GetValue(AutoStartProperty); }
            set { SetValue(AutoStartProperty, value); }
        }

        /// <summary>
        /// Identifies the AutoPlay dependency property.
        /// </summary>
        public static readonly DependencyProperty AutoStartProperty =
            DependencyProperty.Register("AutoPlay", typeof(bool), typeof(LottieAnimationView), new PropertyMetadata(false, AutoPlayPropertyChangedCallback));

        /// <summary>
        /// Identifies the Info dependency property.
        /// </summary>
        public static readonly DependencyProperty InfoProperty =
            DependencyProperty.Register("Info", typeof(AnimationInfo), typeof(LottieAnimationView));

        private static void AutoPlayPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            // Not in use at the moment
        }

        /// <summary>
        /// Gets or sets whether the animation is currently playing.
        /// </summary>
        public bool IsPlaying
        {
            get { return (bool)GetValue(IsPlayingProperty); }
            set { SetValue(IsPlayingProperty, value); }
        }

        /// <summary>
        /// Identifies the IsPlaying dependency property.
        /// </summary>
        public static readonly DependencyProperty IsPlayingProperty =
            DependencyProperty.Register("IsPlaying", typeof(bool), typeof(LottieAnimationView), new PropertyMetadata(false));

        /// <summary>
        /// Identifies the FileName dependency property.
        /// </summary>
        public static readonly DependencyProperty FileNameProperty =
            DependencyProperty.Register("FileName", typeof(string), typeof(LottieAnimationView), new PropertyMetadata(null, FileNamePropertyChangedCallback));

        /// <summary>
        /// Identifies the ResourcePath dependency property.
        /// </summary>
        public static readonly DependencyProperty ResourcePathProperty =
            DependencyProperty.Register("ResourcePath", typeof(string), typeof(LottieAnimationView), new PropertyMetadata(null, ResourcePathPropertyChangedCallback));

        private static void FileNamePropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (dependencyObject is LottieAnimationView lottieAnimationView && e.NewValue is string assetName)
            {
                lottieAnimationView.SetAnimationFromFile(assetName);
            }
        }

        private static void ResourcePathPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (dependencyObject is LottieAnimationView lottieAnimationView && e.NewValue is string assetName)
            {
                lottieAnimationView.SetAnimationFromResource(assetName);
            }
        }

        /// <summary>
        /// Loads a Lottie animation from a file path.
        /// </summary>
        /// <param name="assetName">The file path to the animation JSON.</param>
        private void SetAnimationFromFile(string assetName)
        {
            try
            {
                using FileStream stream = File.OpenRead(assetName);
                SetAnimation(stream);
            }
            catch (IOException)
            {
                Debug.WriteLine($"Failed to load {assetName}");
                throw;
            }
            catch (Exception)
            {
                Debug.WriteLine($"Unexpected error when loading {assetName}");
                throw;
            }
        }

        /// <summary>
        /// Loads a Lottie animation from a resource URI.
        /// </summary>
        /// <param name="assetUri">The resource URI to the animation JSON.</param>
        private void SetAnimationFromResource(string assetUri)
        {
            if (DesignerProperties.GetIsInDesignMode(this))
            {
                return;
            }

            try
            {
                Uri resourceUri = new Uri(assetUri);
                resourceInfo = Application.GetResourceStream(resourceUri);

                SetAnimation(resourceInfo?.Stream);
            }
            catch (IOException)
            {
                Debug.WriteLine($"Failed to load resource {assetUri}");
                throw;
            }
            catch (UriFormatException)
            {
                Debug.WriteLine($"Resource URI failure for resource {assetUri}");
                throw;
            }
            catch (Exception)
            {
                Debug.WriteLine($"Unexpected error when loading resource {assetUri}");
                throw;
            }

        }

        /// <summary>
        /// Loads and prepares the animation from a stream.
        /// </summary>
        /// <param name="stream">The stream containing the animation JSON.</param>
        private void SetAnimation(Stream stream)
        {
            using SKManagedStream fileStream = new(stream);

            if (Animation.TryCreate(fileStream, out animation))
            {
                animation.Seek(0);
                Info = new AnimationInfo(animation.Version, animation.Duration, animation.Fps, animation.InPoint,
                    animation.OutPoint);
            }
            else
            {
                Info = new AnimationInfo(string.Empty, TimeSpan.Zero, 0, 0, 0);
                throw new InvalidOperationException("Failed to load animation");
            }

            if (storyboard == null)
            {
                storyboard = new Storyboard() { RepeatBehavior = this.RepeatBehavior };
                storyboard.Completed += (s, e) =>
                {
                    this.StopAnimation();
                };
                var ani = new DoubleAnimation(animation.InPoint, animation.OutPoint, animation.Duration);
                Storyboard.SetTarget(ani, this);
                Storyboard.SetTargetProperty(ani, new PropertyPath(CurrentFrameProperty));
                storyboard.Children.Add(ani);
                Timeline.SetDesiredFrameRate(storyboard, (int)animation.Fps);
            }
            else
            {
                if (Timeline.GetDesiredFrameRate(storyboard) != (int)animation.Fps)
                {
                    storyboard.Stop();
                    this.BeginAnimation(CurrentFrameProperty, null);
                    Timeline.SetDesiredFrameRate(storyboard, (int)animation.Fps);
                }
                if (storyboard.RepeatBehavior != this.RepeatBehavior)
                {
                    storyboard.RepeatBehavior = this.RepeatBehavior;
                }
            }


            if (AutoPlay || IsPlaying)
            {
                PlayAnimation();
            }
        }

        /// <summary>
        /// Renders the animation on the control's surface.
        /// </summary>
        /// <param name="e">The paint surface event arguments.</param>
        protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
        {
            base.OnPaintSurface(e);

            SKCanvas canvas = e.Surface.Canvas;
            canvas.Clear(SKColor.Empty);
            SKImageInfo info = e.Info;
            if (!EnsureVisibleAndEnabled())
            {
                StopAnimation();
            }
            else if (animation != null)
            {
                animation.SeekFrame(this.CurrentFrame);

                if (AnimationScale is CenterTransform)
                {
                    canvas.Scale(AnimationScale.ScaleX, AnimationScale.ScaleY, info.Width / 2, info.Height / 2);
                }
                else if (AnimationScale != null)
                {
                    canvas.Scale(AnimationScale.ScaleX, AnimationScale.ScaleY, AnimationScale.CenterX, AnimationScale.CenterY);
                }

                animation.Render(canvas, new SKRect(0, 0, info.Width, info.Height));

                _frameCount++;
                if (watch.ElapsedMilliseconds >= 1000)
                {
                    this.CurrentFps = (int)(_frameCount / watch.Elapsed.TotalSeconds);
                    watch.Restart();
                    _frameCount = 0;
                }
            }

        }

        /// <summary>
        /// Releases the resources used by the control.
        /// </summary>
        /// <param name="disposing">True to release managed resources; false otherwise.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    resourceInfo?.Stream?.Dispose();
                }

                storyboard.Stop();
                this.BeginAnimation(CurrentFrameProperty, null);
                storyboard = null;
                resourceInfo = null;
                disposedValue = true;
            }
        }

        /// <summary>
        /// Finalizer for the LottieAnimationView.
        /// </summary>
        ~LottieAnimationView()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        /// <summary>
        /// Disposes the control and releases resources.
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
