using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LottieSharp.WpfSurface
{
    public static class Disposer
    {
        public static void SafeDispose<T>(ref T resource) where T : class
        {
            if (resource == null)
            {
                return;
            }

            var disposer = resource as IDisposable;
            if (disposer != null)
            {
                try
                {
                    disposer.Dispose();
                }
                catch
                {
                }
            }

            resource = null;
        }
    }
}
