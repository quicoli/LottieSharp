using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace LottieSharp.WpfSurface
{
    public abstract class D2dControl : System.Windows.Controls.Image
    {
        // - field -----------------------------------------------------------------------

        private SharpDX.Direct3D11.Device device;
        private Texture2D renderTarget;
        private Dx11ImageSource d3DSurface;
        private RenderTarget d2DRenderTarget;
        private SharpDX.Direct2D1.Factory d2DFactory;

        private readonly Stopwatch renderTimer = new Stopwatch();

        protected ResourceCache resCache = new ResourceCache();

        private long lastFrameTime = 0;
        private long lastRenderTime = 0;
        private int frameCount = 0;
        private int frameCountHistTotal = 0;
        private Queue<int> frameCountHist = new Queue<int>();

        // - property --------------------------------------------------------------------

        public static bool IsInDesignMode
        {
            get
            {
                var prop = DesignerProperties.IsInDesignModeProperty;
                var isDesignMode = (bool)DependencyPropertyDescriptor.FromProperty(prop, typeof(FrameworkElement)).Metadata.DefaultValue;
                return isDesignMode;
            }
        }

        private static readonly DependencyPropertyKey FpsPropertyKey = DependencyProperty.RegisterReadOnly(
            "Fps",
            typeof(int),
            typeof(D2dControl),
            new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.None)
            );

        public static readonly DependencyProperty FpsProperty = FpsPropertyKey.DependencyProperty;

        public int Fps
        {
            get { return (int)GetValue(FpsProperty); }
            protected set { SetValue(FpsPropertyKey, value); }
        }

        public static DependencyProperty RenderWaitProperty = DependencyProperty.Register(
            "RenderWait",
            typeof(int),
            typeof(D2dControl),
            new FrameworkPropertyMetadata(2, OnRenderWaitChanged)
            );

        public int RenderWait
        {
            get { return (int)GetValue(RenderWaitProperty); }
            set { SetValue(RenderWaitProperty, value); }
        }

        public RenderTarget RenderTarget { get => d2DRenderTarget; }
        public static RenderTarget SharedRenderTarget { get; set; }



        // - public methods --------------------------------------------------------------

        public D2dControl()
        {
            base.Loaded += Window_Loaded;
            base.Unloaded += Window_Closing;

            base.Stretch = System.Windows.Media.Stretch.Fill;
        }

        public abstract void Render(RenderTarget target);

        // - event handler ---------------------------------------------------------------

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (D2dControl.IsInDesignMode)
            {
                return;
            }

            StartD3D();
            StartRendering();
        }

        private void Window_Closing(object sender, RoutedEventArgs e)
        {
            if (D2dControl.IsInDesignMode)
            {
                return;
            }

            StopRendering();
            EndD3D();
        }

        private void OnRendering(object sender, EventArgs e)
        {
            if (!renderTimer.IsRunning)
            {
                return;
            }

            PrepareAndCallRender();
            d3DSurface.InvalidateD3DImage();

            lastRenderTime = renderTimer.ElapsedMilliseconds;
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            CreateAndBindTargets();
            base.OnRenderSizeChanged(sizeInfo);
        }

        private void OnIsFrontBufferAvailableChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (d3DSurface.IsFrontBufferAvailable)
            {
                StartRendering();
            }
            else
            {
                StopRendering();
            }
        }

        private static void OnRenderWaitChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (D2dControl)d;
            control.d3DSurface.RenderWait = (int)e.NewValue;
        }

        // - private methods -------------------------------------------------------------

        private void StartD3D()
        {
            device = new SharpDX.Direct3D11.Device(DriverType.Hardware, DeviceCreationFlags.BgraSupport);

            d3DSurface = new Dx11ImageSource();
            d3DSurface.IsFrontBufferAvailableChanged += OnIsFrontBufferAvailableChanged;

            CreateAndBindTargets();

            base.Source = d3DSurface;
        }

        private void EndD3D()
        {
            d3DSurface.IsFrontBufferAvailableChanged -= OnIsFrontBufferAvailableChanged;
            base.Source = null;

            Disposer.SafeDispose(ref d2DRenderTarget);
            Disposer.SafeDispose(ref d2DFactory);
            Disposer.SafeDispose(ref d3DSurface);
            Disposer.SafeDispose(ref renderTarget);
            Disposer.SafeDispose(ref device);
        }

        private void CreateAndBindTargets()
        {
            if (d3DSurface == null)
            {
                return;
            }

            d3DSurface.SetRenderTarget(null);

            Disposer.SafeDispose(ref d2DRenderTarget);
            Disposer.SafeDispose(ref d2DFactory);
            Disposer.SafeDispose(ref renderTarget);

            var width = Math.Max((int)ActualWidth, 100);
            var height = Math.Max((int)ActualHeight, 100);

            var renderDesc = new Texture2DDescription
            {
                BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
                Format = Format.B8G8R8A8_UNorm,
                Width = width,
                Height = height,
                MipLevels = 1,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default,
                OptionFlags = ResourceOptionFlags.Shared,
                CpuAccessFlags = CpuAccessFlags.None,
                ArraySize = 1
            };

            renderTarget = new Texture2D(device, renderDesc);

            var surface = renderTarget.QueryInterface<Surface>();

            d2DFactory = new SharpDX.Direct2D1.Factory();
            //var rtp = new RenderTargetProperties(new PixelFormat(Format.Unknown, SharpDX.Direct2D1.AlphaMode.Premultiplied));
            //d2DRenderTarget = new SharpDX.Direct2D1.DeviceContext(d2DFactory, surface, rtp);
            d2DRenderTarget = new SharpDX.Direct2D1.DeviceContext(surface);
            SharedRenderTarget = d2DRenderTarget;
            resCache.RenderTarget = RenderTarget;

            d3DSurface.SetRenderTarget(renderTarget);

            device.ImmediateContext.Rasterizer.SetViewport(0, 0, width, height, 0.0f, 1.0f);
        }

        private void StartRendering()
        {
            if (renderTimer.IsRunning)
            {
                return;
            }

            System.Windows.Media.CompositionTarget.Rendering += OnRendering;
            renderTimer.Start();
        }

        private void StopRendering()
        {
            if (!renderTimer.IsRunning)
            {
                return;
            }

            System.Windows.Media.CompositionTarget.Rendering -= OnRendering;
            renderTimer.Stop();
        }

        private void PrepareAndCallRender()
        {
            if (device == null)
            {
                return;
            }

            RenderTarget.BeginDraw();
            Render(RenderTarget);
            RenderTarget.EndDraw();

            CalcFps();

            device.ImmediateContext.Flush();
        }

        private void CalcFps()
        {
            frameCount++;
            if (renderTimer.ElapsedMilliseconds - lastFrameTime > 1000)
            {
                frameCountHist.Enqueue(frameCount);
                frameCountHistTotal += frameCount;
                if (frameCountHist.Count > 5)
                {
                    frameCountHistTotal -= frameCountHist.Dequeue();
                }

                var fps = frameCountHistTotal / frameCountHist.Count;
                Fps = fps;
                
                frameCount = 0;
                lastFrameTime = renderTimer.ElapsedMilliseconds;
            }
        }
    }
}
