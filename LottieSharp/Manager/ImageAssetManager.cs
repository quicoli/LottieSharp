using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SharpDX.Direct2D1;

namespace LottieSharp.Manager
{
    internal class ImageAssetManager : IDisposable
    {
        private readonly string _imagesFolder;
        private IImageAssetDelegate _delegate;
        private readonly Dictionary<string, LottieImageAsset> _imageAssets;
        private readonly RenderTarget _context;

        internal ImageAssetManager(string imagesFolder, IImageAssetDelegate @delegate, Dictionary<string, LottieImageAsset> imageAssets, RenderTarget context)
        {
            _imagesFolder = imagesFolder;
            _context = context;
            if (!string.IsNullOrEmpty(imagesFolder) && _imagesFolder[_imagesFolder.Length - 1] != '/')
            {
                _imagesFolder += '/';
            }

            //if (!(callback is UIElement)) // TODO: Makes sense on UWP?
            //{
            //    Debug.WriteLine("LottieDrawable must be inside of a view for images to work.", L.TAG);
            //    this.imageAssets = new Dictionary<string, LottieImageAsset>();
            //    return;
            //}

            _imageAssets = imageAssets;
            Delegate = @delegate;
        }

        internal virtual IImageAssetDelegate Delegate
        {
            set
            {
                lock (this)
                {
                    _delegate = value;
                }
            }
        }

        /// <summary>
        /// Returns the previously set bitmap or null.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        internal Bitmap UpdateBitmap(string id, Bitmap bitmap)
        {
            lock (this)
            {
                if (bitmap == null)
                {
                    if (_imageAssets.TryGetValue(id, out var asset))
                    {
                        var ret = asset.Bitmap;
                        asset.Bitmap = null;
                        return ret;
                    }
                    return null;
                }
                PutBitmap(id, bitmap);
                return bitmap;
            }
        }

        internal virtual Bitmap BitmapForId(RenderTarget renderTarget, string id)
        {
            lock (this)
            {
                if (!_imageAssets.TryGetValue(id, out var imageAsset))
                {
                    return null;
                }
                else if (imageAsset.Bitmap != null)
                {
                    return imageAsset.Bitmap;
                }

                Bitmap bitmap;

                if (_delegate != null)
                {
                    bitmap = _delegate.FetchBitmap(imageAsset);
                    if (bitmap != null)
                    {
                        PutBitmap(id, bitmap);
                    }
                    return bitmap;
                }

                var filename = imageAsset.FileName;

                if (filename.StartsWith("data:") && filename.IndexOf("base64,") > 0)
                {
                    // Contents look like a base64 data URI, with the format data:image/png;base64,<data>.
                    byte[] data;
                    try
                    {
                        data = Convert.FromBase64String(filename.Substring(filename.IndexOf(',') + 1));
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine($"data URL did not have corRectangleF base64 format. {e}", LottieLog.Tag);
                        return null;
                    }

                    //TODO: OID: Check this way is ok or not
                    bitmap = new Bitmap(renderTarget, new SharpDX.Size2(0, 0));
                    bitmap.CopyFromMemory(data, 160);

                    PutBitmap(id, bitmap);
                    return bitmap;
                }

                Stream @is;
                try
                {
                    if (string.IsNullOrEmpty(_imagesFolder))
                    {
                        throw new InvalidOperationException("You must set an images folder before loading an image. Set it with LottieDrawable.ImageAssetsFolder");
                    }
                    @is = File.OpenRead(_imagesFolder + imageAsset.FileName);
                }
                catch (IOException e)
                {
                    Debug.WriteLine($"Unable to open asset. {e}", LottieLog.Tag);
                    return null;
                }

                //TODO: OID: Check this way is ok or not
                bitmap = new Bitmap(renderTarget, new SharpDX.Size2(0, 0));
                bitmap.CopyFromStream(@is, 160, (int)@is.Length);

                @is.Dispose();

                PutBitmap(id, bitmap);

                return bitmap;
            }
        }

        internal virtual void RecycleBitmaps()
        {
            lock (this)
            {
                for (var i = _imageAssets.Count - 1; i >= 0; i--)
                {
                    var entry = _imageAssets.ElementAt(i);
                    entry.Value.Bitmap?.Dispose();
                    entry.Value.Bitmap = null;
                    _imageAssets.Remove(entry.Key);
                }
            }
        }

        public bool HasSameContext(RenderTarget context)
        {
            return context == null && _context == null || _context.Equals(context);
        }

        private Bitmap PutBitmap(string key, Bitmap bitmap)
        {
            lock (this)
            {
                _imageAssets[key].Bitmap = bitmap;
                return bitmap;
            }
        }

        private void Dispose(bool disposing)
        {
            RecycleBitmaps();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~ImageAssetManager()
        {
            Dispose(false);
        }
    }
}