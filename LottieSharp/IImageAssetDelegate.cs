using SharpDX.Direct2D1;

namespace LottieSharp
{
    /// <summary>
    /// Delegate to handle the loading of bitmaps that are not packaged in the assets of your app.
    /// </summary>
    public interface IImageAssetDelegate
    {
        Bitmap FetchBitmap(LottieImageAsset asset);
    }
}