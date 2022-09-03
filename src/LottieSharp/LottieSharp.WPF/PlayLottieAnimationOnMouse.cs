using Microsoft.Xaml.Behaviors;
using System.Linq;
using System.Windows;

namespace LottieSharp.WPF
{
    public class PlayLottieAnimationOnMouse : Behavior<UIElement>
    {
        public string LottieView { get; set; }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.MouseEnter += AssociatedObject_MouseEnter;
            AssociatedObject.MouseLeave += AssociatedObject_MouseLeave;
            AssociatedObject.TouchUp += AssociatedObject_TouchUp;
        }

        private void AssociatedObject_TouchUp(object sender, System.Windows.Input.TouchEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(LottieView)) return;

            LottieAnimationView lottieView = AssociatedObject.FindChildren<LottieAnimationView>().FirstOrDefault(x => x.Name.ToLower() == LottieView.ToLower());
            if (lottieView == null) return;

            if (!lottieView.IsPlaying)
            {
                lottieView.PlayAnimation();
            }
        }

        protected override void OnDetaching()
        {
            AssociatedObject.MouseEnter -= AssociatedObject_MouseEnter;
            AssociatedObject.MouseLeave -= AssociatedObject_MouseLeave;
            AssociatedObject.TouchUp -= AssociatedObject_TouchUp;
            base.OnDetaching();
        }

        private void AssociatedObject_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(LottieView)) return;

            LottieAnimationView lottieView = AssociatedObject.FindChildren<LottieAnimationView>().FirstOrDefault(x => x.Name.ToLower() == LottieView.ToLower());
            if (lottieView == null) return;

            lottieView.StopAnimation();
        }

        private void AssociatedObject_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(LottieView)) return;

            LottieAnimationView lottieView = AssociatedObject.FindChildren<LottieAnimationView>().FirstOrDefault(x => x.Name.ToLower() == LottieView.ToLower());
            if (lottieView == null) return;

            if (!lottieView.IsPlaying)
            {
                lottieView.PlayAnimation();
            }
        }
    }
}
