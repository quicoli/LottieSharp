using System;
using SharpDX;
using LottieSharp.Animation.Content;
using LottieSharp.Animation.Keyframe;
using LottieSharp.Value;
using SharpDX.Direct2D1;

namespace LottieSharp.Model.Layer
{
    internal class ImageLayer : BaseLayer
    {
        private readonly Paint _paint = new Paint(Paint.AntiAliasFlag | Paint.FilterBitmapFlag);
        private RectangleF _src;
        private RectangleF _dst;
        private IBaseKeyframeAnimation<ColorFilter, ColorFilter> _colorFilterAnimation;

        internal ImageLayer(LottieDrawable lottieDrawable, Layer layerModel) : base(lottieDrawable, layerModel)
        {
        }

        public override void DrawLayer(BitmapCanvas canvas, Matrix3X3 parentMatrix, byte parentAlpha)
        {
            var bitmap = Bitmap;
            if (bitmap == null)
            {
                return;
            }
            var density = Utils.Utils.DpScale();

            _paint.Alpha = parentAlpha;
            if (_colorFilterAnimation != null)
            {
                _paint.ColorFilter = _colorFilterAnimation.Value;
            }
            canvas.Save();
            canvas.Concat(parentMatrix);
            RectExt.Set(ref _src, 0, 0, PixelWidth, PixelHeight);
            RectExt.Set(ref _dst, 0, 0, (int)(PixelWidth * density), (int)(PixelHeight * density));
            canvas.DrawBitmap(bitmap, _src, _dst, _paint);
            canvas.Restore();
        }

        public override void GetBounds(ref RectangleF outBounds, Matrix3X3 parentMatrix)
        {
            base.GetBounds(ref outBounds, parentMatrix);
            var bitmap = Bitmap;
            if (bitmap != null)
            {
                RectExt.Set(ref outBounds, outBounds.Left, outBounds.Top, Math.Min(outBounds.Right, PixelWidth), Math.Min(outBounds.Bottom, PixelHeight));
                BoundsMatrix.MapRect(ref outBounds);
            }
        }
        private int PixelWidth => (int)Bitmap.PixelSize.Width;

        private int PixelHeight => (int)Bitmap.PixelSize.Height;

        private Bitmap Bitmap
        {
            get
            {
                var refId = LayerModel.RefId;
                return LottieDrawable.GetImageAsset(refId);
            }
        }

        public override void AddValueCallback<T>(LottieProperty property, ILottieValueCallback<T> callback)
        {
            base.AddValueCallback(property, callback);
            if (property == LottieProperty.ColorFilter)
            {
                if (callback == null)
                {
                    _colorFilterAnimation = null;
                }
                else
                {
                    _colorFilterAnimation = new ValueCallbackKeyframeAnimation<ColorFilter, ColorFilter>((ILottieValueCallback<ColorFilter>)callback);
                }
            }
        }
    }
}