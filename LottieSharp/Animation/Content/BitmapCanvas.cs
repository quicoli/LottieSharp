using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using SharpDX.Mathematics.Interop;
using System;
using System;
/* Unmerged change from project 'LottieSharp (netcoreapp3.0)'
Before:
using System;
After:
using System.Collections.Generic;
*/


namespace LottieSharp.Animation.Content
{
    public class BitmapCanvas : IDisposable
    {
        private Matrix3X3 _matrix = Matrix3X3.CreateIdentity();
        private readonly Stack<Matrix3X3> _matrixSaves = new Stack<Matrix3X3>();
        private readonly Stack<int> _flagSaves = new Stack<int>();
        private readonly Dictionary<int, RenderTargetHolder> _renderTargets = new Dictionary<int, RenderTargetHolder>();
        //private DeviceContext _renderTarget;

        class RenderTargetHolder
        {
            public DeviceContext RenderTarget { get; set; }
            public Bitmap1 Bitmap { get; set; }
        }

        class ClipSave
        {
            public ClipSave(RectangleF rect, IDisposable layer)
            {
                RectangleF = rect;
                Layer = layer;
            }

            public RectangleF RectangleF { get; }
            public IDisposable Layer { get; }
        }

        private readonly Stack<ClipSave> _clipSaves = new Stack<ClipSave>();
        private RectangleF _currentClip;

        class RenderTargetSave
        {
            public RenderTargetSave(DeviceContext renderTarget, int paintFlags, PorterDuffXfermode paintXfermode, byte paintAlpha)
            {
                RenderTarget = renderTarget;
                PaintFlags = paintFlags;
                PaintXfermode = paintXfermode;
                PaintAlpha = paintAlpha;
            }

            public DeviceContext RenderTarget { get; }
            public int PaintFlags { get; }
            public PorterDuffXfermode PaintXfermode { get; }
            public byte PaintAlpha { get; }
        }

        private readonly Stack<RenderTargetSave> _renderTargetSaves = new Stack<RenderTargetSave>();
        private readonly Stack<RenderTargetHolder> _canvasDrawingSessions = new Stack<RenderTargetHolder>();

        //internal RenderTarget OutputRenderTarget { get; private set; }
        internal DeviceContext CurrentRenderTarget =>
            _canvasDrawingSessions.Count > 0 ?
                _canvasDrawingSessions.Peek()?.RenderTarget :
                null;

        public BitmapCanvas(float width, float height)
        {
            //OutputRenderTarget = renderTarget;
            UpdateClip(width, height);
        }

        private void UpdateClip(float width, float height)
        {
            if (Math.Abs(width - Width) > 0 || Math.Abs(height - Height) > 0)
            {
                Dispose(false);
            }

            Width = width;
            Height = height;
            _currentClip = new RectangleF(0, 0, Width, Height);
        }

        public float Width { get; private set; }
        public float Height { get; private set; }

        public static int MatrixSaveFlag = 0b00001;
        public static int ClipSaveFlag = 0b00010;
        //public static int HasAlphaLayerSaveFlag = 0b00100;
        //public static int FullColorLayerSaveFlag = 0b01000;
        public static int ClipToLayerSaveFlag = 0b10000;
        public static int AllSaveFlag = 0b11111;


        internal IDisposable CreateSession(float width, float height, DeviceContext drawingSession)
        {
            _canvasDrawingSessions.Clear();
            //_renderTarget = drawingSession;
            _canvasDrawingSessions.Push(new RenderTargetHolder { RenderTarget = drawingSession });

            UpdateClip(width, height);

            return PushMask(_currentClip, 1f);
            //return new Disposable(() => { });
        }

        public void DrawRect(double x1, double y1, double x2, double y2, Paint paint)
        {
            UpdateDrawingSessionWithFlags(paint.Flags);
            CurrentRenderTarget.Transform = GetCurrentTransform();
            using (var brush = new SolidColorBrush(CurrentRenderTarget, paint.Color))
            {
                if (paint.Style == Paint.PaintStyle.Stroke)
                {
                    CurrentRenderTarget.DrawRectangle(new RectangleF((float)x1, (float)y1, (float)(x2 - x1), (float)(y2 - y1)), brush, paint.StrokeWidth, GetStrokeStyle(paint));
                }
                else
                {
                    CurrentRenderTarget.FillRectangle(new RectangleF((float)x1, (float)y1, (float)(x2 - x1), (float)(y2 - y1)), brush);
                }
            }

            if (paint.Xfermode.Mode == PorterDuff.Mode.Clear)
            {
                CurrentRenderTarget.Flush();
            }
        }

        private StrokeStyle GetStrokeStyle(Paint paint)
        {
            var styleProperties = new StrokeStyleProperties()
            {
                StartCap = paint.StrokeCap,
                DashCap = paint.StrokeCap,
                EndCap = paint.StrokeCap,
                LineJoin = paint.StrokeJoin,
                MiterLimit = paint.StrokeMiter
            };
            //TODO: OID: Check Factory() is ok or not
            var style = new StrokeStyle(CurrentRenderTarget.Factory, styleProperties);
            paint.PathEffect?.Apply(style, paint);
            return style;
        }

        internal void DrawRect(RectangleF rect, Paint paint)
        {
            UpdateDrawingSessionWithFlags(paint.Flags);
            CurrentRenderTarget.Transform = GetCurrentTransform();

            using (var brush = new SolidColorBrush(CurrentRenderTarget, paint.Color))
            {
                if (paint.Style == Paint.PaintStyle.Stroke)
                {
                    CurrentRenderTarget.DrawRectangle(rect, brush, paint.StrokeWidth, GetStrokeStyle(paint));
                }
                else
                {
                    CurrentRenderTarget.FillRectangle(rect, brush);
                }
            }
        }

        public void DrawPath(Path path, Paint paint, bool fromMask = false)
        {
            UpdateDrawingSessionWithFlags(paint.Flags);

            CurrentRenderTarget.Transform = GetCurrentTransform();

            var gradient = paint.Shader as Gradient;
            var brush = gradient != null ? gradient.GetBrush(CurrentRenderTarget, paint.Alpha) : new SolidColorBrush(CurrentRenderTarget, paint.Color);
            var finalBrush = paint.ColorFilter?.Apply(this, brush) ?? brush;

            using (var geometry = path.GetGeometry(CurrentRenderTarget.Factory))
            {
                if (paint.Style == Paint.PaintStyle.Stroke)
                    CurrentRenderTarget.DrawGeometry(geometry, finalBrush, paint.StrokeWidth, GetStrokeStyle(paint));
                else
                    CurrentRenderTarget.FillGeometry(geometry, finalBrush);
            }

            if (gradient == null)
            {
                brush?.Dispose();
                finalBrush?.Dispose();
            }
        }

        public Disposable PushMask(RectangleF rect, float alpha, Path path = null)
        {
            if (alpha >= 1 && path == null)
            {
                CurrentRenderTarget.PushAxisAlignedClip(rect, CurrentRenderTarget.AntialiasMode);

                return new Disposable(() =>
                {
                    CurrentRenderTarget.PopAxisAlignedClip();
                });
            }
            else
            {
                var geometery = path?.GetGeometry(CurrentRenderTarget.Factory);

                var parameters = new LayerParameters
                {
                    ContentBounds = rect,
                    Opacity = alpha,
                    MaskTransform = GetCurrentTransform(),
                    GeometricMask = geometery
                };

                var layer = new Layer(CurrentRenderTarget);

                CurrentRenderTarget.PushLayer(ref parameters, layer);

                return new Disposable(() =>
                {
                    this.CurrentRenderTarget.PopLayer();
                    layer.Dispose();
                    geometery?.Dispose();
                });
            }
        }

        private Matrix3x2 GetCurrentTransform()
        {
            return new Matrix3x2
            {
                M11 = _matrix.M11,
                M12 = _matrix.M21,
                M21 = _matrix.M12,
                M22 = _matrix.M22,
                M31 = _matrix.M13,
                M32 = _matrix.M23
            };
        }

        public bool ClipRect(RectangleF rect)
        {
            _currentClip.Intersects(rect);
            return _currentClip.IsEmpty == false;
        }

        public void ClipReplaceRect(RectangleF rect)
        {
            _currentClip = rect;
        }

        public void Concat(Matrix3X3 parentMatrix)
        {
            _matrix = MatrixExt.PreConcat(_matrix, parentMatrix);
        }

        public void Save()
        {
            _flagSaves.Push(MatrixSaveFlag | ClipSaveFlag);
            SaveMatrix();
            SaveClip(255);
        }

        public void SaveLayer(RectangleF bounds, Paint paint, int flags, Path path = null)
        {
            _flagSaves.Push(flags);
            if ((flags & MatrixSaveFlag) == MatrixSaveFlag)
            {
                SaveMatrix();
            }
            var isClipToLayer = (flags & ClipToLayerSaveFlag) == ClipToLayerSaveFlag;

            if (isClipToLayer)
            {
                UpdateDrawingSessionWithFlags(paint.Flags);

                var rendertarget = CreateRenderTarget(bounds, _renderTargetSaves.Count);
                _renderTargetSaves.Push(new RenderTargetSave(rendertarget.RenderTarget, paint.Flags, paint.Xfermode, paint.Xfermode != null ? (byte)255 : paint.Alpha));

                //var drawingSession = rendertarget.CreateDrawingSession();
                rendertarget.RenderTarget.Clear(Color.Transparent);
                _canvasDrawingSessions.Push(rendertarget);
            }

            if ((flags & ClipSaveFlag) == ClipSaveFlag)
            {
                SaveClip(isClipToLayer ? (byte)255 : paint.Alpha, path);
            }

        }

        private RenderTargetHolder CreateRenderTarget(RectangleF bounds, int index)
        {
            if (!_renderTargets.TryGetValue(index, out var rendertarget))
            {
                //TODO: OID: Some properties are missed
                //rendertarget = new DeviceContext(
                //    CurrentRenderTarget,
                //    CompatibleRenderTargetOptions.None,
                //    new Size2F((float)bounds.Width, (float)bounds.Height),
                //    new Size2((int)Utils.Utils.Dpi(), (int)Utils.Utils.Dpi()),
                //    new PixelFormat(SharpDX.DXGI.Format.B8G8R8A8_UNorm, AlphaMode.Premultiplied));


                var rt = new DeviceContext(CurrentRenderTarget.Device, DeviceContextOptions.EnableMultithreadedOptimizations);

                var bitmap = new Bitmap1(
                    rt,
                    new Size2((int)bounds.Width, (int)bounds.Height),
                    null, 0,
                    new BitmapProperties1
                    {
                        DpiX = CurrentRenderTarget.DotsPerInch.Width,
                        DpiY = CurrentRenderTarget.DotsPerInch.Height,
                        PixelFormat = CurrentRenderTarget.PixelFormat,
                        BitmapOptions = BitmapOptions.Target
                    });

                rt.Target = bitmap;
                rendertarget = new RenderTargetHolder
                {
                    RenderTarget = rt,
                    Bitmap = bitmap
                };
                _renderTargets.Add(index, rendertarget);
            }
            rendertarget.RenderTarget.BeginDraw();
            return rendertarget;
        }

        private void SaveMatrix()
        {
            var copy = new Matrix3X3();
            copy.Set(_matrix);
            _matrixSaves.Push(copy);
        }

        private void SaveClip(byte alpha, Path path = null)
        {
            var currentLayer = PushMask(_currentClip, alpha / 255f, path);

            _clipSaves.Push(new ClipSave(_currentClip, currentLayer));
        }

        public void RestoreAll()
        {
            while (_flagSaves.Count > 0)
                Restore();
        }

        public void Restore()
        {
            if (_flagSaves.Count < 1) return;

            var flags = _flagSaves.Pop();

            //if ((flags & ClipToLayerSaveFlag) == ClipToLayerSaveFlag)
            //{
            //    using (var brush = new SolidColorBrush(CurrentRenderTarget, new RawColor4(0, 0, 0, 0.3f)))
            //        CurrentRenderTarget.FillRectangle(new RectangleF(0, 0, 100, 100), brush);
            //}


            if ((flags & MatrixSaveFlag) == MatrixSaveFlag)
            {
                _matrix = _matrixSaves.Pop();
            }

            if ((flags & ClipSaveFlag) == ClipSaveFlag)
            {
                var clipSave = _clipSaves.Pop();
                _currentClip = clipSave.RectangleF;
                clipSave.Layer.Dispose();
            }

            if ((flags & ClipToLayerSaveFlag) == ClipToLayerSaveFlag)
            {
                var drawingSession = _canvasDrawingSessions.Pop();
                drawingSession.RenderTarget.Flush();
                drawingSession.RenderTarget.EndDraw();

                var renderTargetSave = _renderTargetSaves.Pop();

                UpdateDrawingSessionWithFlags(renderTargetSave.PaintFlags);
                CurrentRenderTarget.Transform = GetCurrentTransform();


                var canvasComposite = CompositeMode.SourceOver;
                if (renderTargetSave.PaintXfermode != null)
                {
                    canvasComposite = PorterDuff.ToCanvasComposite(renderTargetSave.PaintXfermode.Mode);
                }

                CurrentRenderTarget.DrawImage(drawingSession.Bitmap,
                    new RawVector2(0, 0),
                    new RectangleF(0, 0, renderTargetSave.RenderTarget.Size.Width, renderTargetSave.RenderTarget.Size.Height),
                    //renderTargetSave.PaintAlpha / 255f,
                    InterpolationMode.Linear,
                    canvasComposite);

                //CurrentRenderTarget.DrawBitmap(drawingSession.Bitmap, 255f, InterpolationMode.Linear);


                //var rect = new RawRectangleF(0, 0, rt.Size.Width, rt.Size.Height);
                ////using (var brush = new SolidColorBrush(CurrentRenderTarget, Color.Black))
                ////    CurrentRenderTarget.FillOpacityMask(rt, brush, OpacityMaskContent.Graphics, rect, rect);
                //CurrentRenderTarget.DrawImage(rt, rect, renderTargetSave.PaintAlpha / 255f, BitmapInterpolationMode.Linear, rect);

                //rt.Dispose();
                //renderTargetSave.RenderTarget.Dispose();
            }

            CurrentRenderTarget.Flush();


        }

        public void DrawBitmap(Bitmap bitmap, RectangleF src, RectangleF dst, Paint paint)
        {
            UpdateDrawingSessionWithFlags(paint.Flags);
            CurrentRenderTarget.Transform = GetCurrentTransform();

            //var canvasComposite = CanvasComposite.SourceOver;
            // TODO paint.ColorFilter
            //if (paint.ColorFilter is PorterDuffColorFilter porterDuffColorFilter)
            //    canvasComposite = PorterDuff.ToCanvasComposite(porterDuffColorFilter.Mode);

            CurrentRenderTarget.DrawBitmap(bitmap, dst, paint.Alpha / 255f, BitmapInterpolationMode.NearestNeighbor, src);
        }

        public void GetClipBounds(ref RectangleF bounds)
        {
            RectExt.Set(ref bounds, _currentClip.X, _currentClip.Y, _currentClip.Width, _currentClip.Height);
        }

        public void Clear(Color color)
        {
            UpdateDrawingSessionWithFlags(0);

            CurrentRenderTarget.Clear(color);

            _matrixSaves.Clear();
            _flagSaves.Clear();
            _clipSaves.Clear();
        }

        private void UpdateDrawingSessionWithFlags(int flags)
        {
            CurrentRenderTarget.AntialiasMode = (flags & Paint.AntiAliasFlag) == Paint.AntiAliasFlag
                ? AntialiasMode.PerPrimitive
                : AntialiasMode.Aliased;
        }

        private AntialiasMode GetDrawingSessionMode(int flags)
        {
            return (flags & Paint.AntiAliasFlag) == Paint.AntiAliasFlag
                ? AntialiasMode.PerPrimitive
                : AntialiasMode.Aliased;
        }

        public void Translate(float dx, float dy)
        {
            _matrix = MatrixExt.PreTranslate(_matrix, dx, dy);
        }

        public void Scale(float sx, float sy, float px, float py)
        {
            _matrix = MatrixExt.PreScale(_matrix, sx, sy, px, py);
        }

        public void SetMatrix(Matrix3X3 matrix)
        {
            _matrix.Set(matrix);
        }

        public RectangleF DrawText(char character, Paint paint)
        {
            var gradient = paint.Shader as Gradient;
            var brush = gradient != null ? gradient.GetBrush(CurrentRenderTarget, paint.Alpha) : new SolidColorBrush(CurrentRenderTarget, paint.Color);
            var finalBrush = paint.ColorFilter?.Apply(this, brush) ?? brush;

            UpdateDrawingSessionWithFlags(paint.Flags);
            CurrentRenderTarget.Transform = GetCurrentTransform();

            var text = new string(character, 1);

            //TODO: OID: Check for global factory
            using (var factory = new SharpDX.DirectWrite.Factory())
            {
                var textFormat = new TextFormat(factory, paint.Typeface.FontFamily, paint.Typeface.Weight, paint.Typeface.Style, paint.TextSize)
                {
                    //FontSize = paint.TextSize,
                    //FontFamily = paint.Typeface.FontFamily,
                    //FontStyle = paint.Typeface.Style,
                    //FontWeight = paint.Typeface.Weight,
                    //VerticalAlignment = CanvasVerticalAlignment.Center,
                    //HorizontalAlignment = CanvasHorizontalAlignment.Left,
                    //LineSpacingBaseline = 0,
                    //LineSpacing = 0
                };
                var textLayout = new TextLayout(factory, text, textFormat, 0.0f, 0.0f);
                CurrentRenderTarget.DrawText(text, textFormat, new RectangleF(0, 0, 0, 0), finalBrush);

                if (gradient == null)
                {
                    brush?.Dispose();
                    finalBrush?.Dispose();
                }

                //TODO: OID: LayoutBound is not exists in text layout
                return new RectangleF(0, 0, textLayout.DetermineMinWidth(), 0);
            }
        }

        private void Dispose(bool disposing)
        {
            foreach (var renderTarget in _renderTargets)
            {
                renderTarget.Value.RenderTarget.Dispose();
                renderTarget.Value.RenderTarget = null;

                renderTarget.Value.Bitmap.Dispose();
                renderTarget.Value.Bitmap = null;
            }
            _renderTargets.Clear();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

    }
}