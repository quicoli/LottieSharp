using SharpDX.Direct3D9;
using System;
using System.Threading;
using System.Windows.Interop;

namespace LottieSharp.WpfSurface
{
    class Dx11ImageSource : D3DImage, IDisposable
    {

        // - field -----------------------------------------------------------------------

        private static int ActiveClients;
        private static Direct3DEx D3DContext;
        private static DeviceEx D3DDevice;

        private Texture renderTarget;

        // - property --------------------------------------------------------------------

        public int RenderWait { get; set; } = 2; // default: 2ms

        // - public methods --------------------------------------------------------------

        public Dx11ImageSource()
        {
            StartD3D();
            Dx11ImageSource.ActiveClients++;
        }

        public void Dispose()
        {
            SetRenderTarget(null);

            Disposer.SafeDispose(ref renderTarget);

            Dx11ImageSource.ActiveClients--;
            EndD3D();
        }

        public void InvalidateD3DImage()
        {
            if (renderTarget != null)
            {
                base.Lock();
                if (RenderWait != 0)
                {
                    Thread.Sleep(RenderWait);
                }
                base.AddDirtyRect(new System.Windows.Int32Rect(0, 0, base.PixelWidth, base.PixelHeight));
                base.Unlock();
            }
        }

        public void SetRenderTarget(SharpDX.Direct3D11.Texture2D target)
        {
            if (renderTarget != null)
            {
                renderTarget = null;

                base.Lock();
                base.SetBackBuffer(D3DResourceType.IDirect3DSurface9, IntPtr.Zero);
                base.Unlock();
            }

            if (target == null)
            {
                return;
            }

            var format = Dx11ImageSource.TranslateFormat(target);
            var handle = GetSharedHandle(target);

            if (!IsShareable(target))
            {
                throw new ArgumentException("Texture must be created with ResouceOptionFlags.Shared");
            }

            if (format == Format.Unknown)
            {
                throw new ArgumentException("Texture format is not compatible with OpenSharedResouce");
            }

            if (handle == IntPtr.Zero)
            {
                throw new ArgumentException("Invalid handle");
            }

            renderTarget = new Texture(Dx11ImageSource.D3DDevice, target.Description.Width, target.Description.Height, 1, Usage.RenderTarget, format, Pool.Default, ref handle);

            using (var surface = renderTarget.GetSurfaceLevel(0))
            {
                base.Lock();
                base.SetBackBuffer(D3DResourceType.IDirect3DSurface9, surface.NativePointer);
                base.Unlock();
            }
        }

        // - private methods -------------------------------------------------------------

        private void StartD3D()
        {
            if (Dx11ImageSource.ActiveClients != 0)
            {
                return;
            }

            var presentParams = GetPresentParameters();
            var createFlags = CreateFlags.HardwareVertexProcessing | CreateFlags.Multithreaded | CreateFlags.FpuPreserve;

            Dx11ImageSource.D3DContext = new Direct3DEx();
            Dx11ImageSource.D3DDevice = new DeviceEx(D3DContext, 0, DeviceType.Hardware, IntPtr.Zero, createFlags, presentParams);
        }

        private void EndD3D()
        {
            if (Dx11ImageSource.ActiveClients != 0)
            {
                return;
            }

            Disposer.SafeDispose(ref renderTarget);
            Disposer.SafeDispose(ref Dx11ImageSource.D3DDevice);
            Disposer.SafeDispose(ref Dx11ImageSource.D3DContext);
        }

        private static void ResetD3D()
        {
            if (Dx11ImageSource.ActiveClients == 0)
            {
                return;
            }

            var presentParams = GetPresentParameters();
            Dx11ImageSource.D3DDevice.ResetEx(ref presentParams);
        }

        private static PresentParameters GetPresentParameters()
        {
            var presentParams = new PresentParameters();

            presentParams.Windowed = true;
            presentParams.SwapEffect = SwapEffect.Discard;
            presentParams.DeviceWindowHandle = NativeMethods.GetDesktopWindow();
            presentParams.PresentationInterval = PresentInterval.Default;

            return presentParams;
        }

        private IntPtr GetSharedHandle(SharpDX.Direct3D11.Texture2D texture)
        {
            using (var resource = texture.QueryInterface<SharpDX.DXGI.Resource>())
            {
                return resource.SharedHandle;
            }
        }

        private static Format TranslateFormat(SharpDX.Direct3D11.Texture2D texture)
        {
            switch (texture.Description.Format)
            {
                case SharpDX.DXGI.Format.R10G10B10A2_UNorm: return SharpDX.Direct3D9.Format.A2B10G10R10;
                case SharpDX.DXGI.Format.R16G16B16A16_Float: return SharpDX.Direct3D9.Format.A16B16G16R16F;
                case SharpDX.DXGI.Format.B8G8R8A8_UNorm: return SharpDX.Direct3D9.Format.A8R8G8B8;
                default: return SharpDX.Direct3D9.Format.Unknown;
            }
        }

        private static bool IsShareable(SharpDX.Direct3D11.Texture2D texture)
        {
            return (texture.Description.OptionFlags & SharpDX.Direct3D11.ResourceOptionFlags.Shared) != 0;
        }
    }
}
