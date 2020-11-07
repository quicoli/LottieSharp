using System;

namespace LottieSharp.Utils
{
    public abstract class BaseLottieAnimator : ValueAnimator
    {
        public long StartDelay
        {
            get => throw new Exception("LottieAnimator does not support getStartDelay.");
            set => throw new Exception("LottieAnimator does not support setStartDelay.");
        }

        public override long Duration
        {
            set => throw new Exception("LottieAnimator does not support setDuration.");
        }

        public override IInterpolator Interpolator
        {
            set => throw new Exception("LottieAnimator does not support setInterpolator.");
        }

        public event EventHandler<LottieAnimatorStartEventArgs> AnimationStart;
        public event EventHandler<LottieAnimatorEndEventArgs> AnimationEnd;
        public event EventHandler AnimationCancel;
        public event EventHandler AnimationRepeat;

        public class LottieAnimatorStartEventArgs : EventArgs
        {
            public bool IsReverse { get; }

            public LottieAnimatorStartEventArgs(bool isReverse)
            {
                IsReverse = isReverse;
            }
        }

        public class LottieAnimatorEndEventArgs : EventArgs
        {
            public bool IsReverse { get; }

            public LottieAnimatorEndEventArgs(bool isReverse)
            {
                IsReverse = isReverse;
            }
        }

        public virtual void OnAnimationStart(bool isReverse)
        {
            AnimationStart?.Invoke(this, new LottieAnimatorStartEventArgs(isReverse));
        }

        public virtual void OnAnimationRepeat()
        {
            AnimationRepeat?.Invoke(this, EventArgs.Empty);
        }

        public virtual void OnAnimationEnd(bool isReverse)
        {
            AnimationEnd?.Invoke(this, new LottieAnimatorEndEventArgs(isReverse));
        }

        public virtual void OnAnimationCancel()
        {
            AnimationCancel?.Invoke(this, EventArgs.Empty);
        }

        protected override void Disposing(bool disposing)
        {
            base.Disposing(disposing);

            if (AnimationStart != null)
                foreach (EventHandler<LottieAnimatorStartEventArgs> handler in AnimationStart.GetInvocationList())
                    AnimationStart -= handler;

            if (AnimationEnd != null)
                foreach (EventHandler<LottieAnimatorEndEventArgs> handler in AnimationEnd.GetInvocationList())
                    AnimationEnd -= handler;

            if (AnimationCancel != null)
                foreach (EventHandler handler in AnimationCancel.GetInvocationList())
                    AnimationCancel -= handler;

            if (AnimationRepeat != null)
                foreach (EventHandler handler in AnimationRepeat.GetInvocationList())
                    AnimationRepeat -= handler;
        }
    }
}
