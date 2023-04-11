using System.Windows;
using System.Windows.Controls;

namespace LottieSharp.WPF.Transforms
{
    public class AnimationTransformBase : Control
    {
        public float ScaleX
        {
            get { return (float)GetValue(ScaleXProperty); }
            set { SetValue(ScaleXProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ScaleX.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ScaleXProperty =
            DependencyProperty.Register("ScaleX", typeof(float), typeof(AnimationTransformBase), new PropertyMetadata(0.0f));

        public float ScaleY
        {
            get { return (float)GetValue(ScaleYProperty); }
            set { SetValue(ScaleYProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ScaleY.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ScaleYProperty =
            DependencyProperty.Register("ScaleY", typeof(float), typeof(AnimationTransformBase), new PropertyMetadata(0.0f));

        public float CenterX
        {
            get { return (float)GetValue(CenterXProperty); }
            set { SetValue(CenterXProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CenterX.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CenterXProperty =
            DependencyProperty.Register("CenterX", typeof(float), typeof(AnimationTransformBase), new PropertyMetadata(0.0f));

        public float CenterY
        {
            get { return (float)GetValue(CenterYProperty); }
            set { SetValue(CenterYProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CenterY.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CenterYProperty =
            DependencyProperty.Register("CenterY", typeof(float), typeof(AnimationTransformBase), new PropertyMetadata(0.0f));

    }
}
