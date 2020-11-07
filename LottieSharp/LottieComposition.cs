using LottieSharp.Model;
using LottieSharp.Model.Layer;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LottieSharp
{
    /// <summary>
    /// After Effects/Bodymovin composition model. This is the serialized model from which the
    /// animation will be created.
    /// 
    /// To create one, use <see cref="LottieCompositionFactory"/>.
    /// 
    /// It can be used with a <seealso cref="LottieAnimationView"/> or
    /// <seealso cref="LottieDrawable"/>.
    /// </summary>
    public class LottieComposition : IDisposable
    {
        public bool Disposed { get; set; }

        private readonly PerformanceTracker _performanceTracker = new PerformanceTracker();
        private readonly HashSet<string> _warnings = new HashSet<string>();
        private Dictionary<string, List<Layer>> _precomps;
        private Dictionary<string, LottieImageAsset> _images;
        /** Map of font names to fonts */
        public virtual Dictionary<string, Font> Fonts { get; private set; }
        public virtual Dictionary<int, FontCharacter> Characters { get; private set; }
        private Dictionary<long, Layer> _layerMap;
        public List<Layer> Layers { get; private set; }

        // This is stored as a set to avoid duplicates.
        public virtual RectangleF Bounds { get; private set; }
        public float StartFrame { get; private set; }
        public float EndFrame { get; private set; }
        public float FrameRate { get; private set; }

        internal void AddWarning(string warning)
        {
            Debug.WriteLine(warning, LottieLog.Tag);
            _warnings.Add(warning);
        }

        public List<string> Warnings => _warnings.ToList();

        public virtual bool PerformanceTrackingEnabled
        {
            set => _performanceTracker.Enabled = value;
        }

        public virtual PerformanceTracker PerformanceTracker => _performanceTracker;

        internal virtual Layer LayerModelForId(long id)
        {
            _layerMap.TryGetValue(id, out Layer layer);
            return layer;
        }

        public virtual float Duration
        {
            get
            {
                return (long)(DurationFrames / FrameRate * 1000);
            }
        }

        public void Init(RectangleF bounds, float startFrame, float endFrame, float frameRate, List<Layer> layers, Dictionary<long, Layer> layerMap, Dictionary<string, List<Layer>> precomps, Dictionary<string, LottieImageAsset> images, Dictionary<int, FontCharacter> characters, Dictionary<string, Font> fonts)
        {
            Bounds = bounds;
            StartFrame = startFrame;
            EndFrame = endFrame;
            FrameRate = frameRate;
            Layers = layers;
            _layerMap = layerMap;
            _precomps = precomps;
            _images = images;
            Characters = characters;
            Fonts = fonts;
        }

        internal virtual List<Layer> GetPrecomps(string id)
        {
            return _precomps[id];
        }

        public virtual bool HasImages => _images.Count > 0;

        public virtual Dictionary<string, LottieImageAsset> Images => _images;

        internal virtual float DurationFrames => EndFrame - StartFrame;

        public override string ToString()
        {
            var sb = new StringBuilder("LottieComposition:\n");
            foreach (var layer in Layers)
            {
                sb.Append(layer.ToString("\t"));
            }
            return sb.ToString();
        }

        public void Dispose()
        {
            Disposed = true;

            foreach (var item in _images)
            {
                item.Value.Bitmap.Dispose();
                item.Value.Bitmap = null;
            }
            _images.Clear();

            foreach (var item in _layerMap)
            {
                item.Value.Dispose();
            }
            _layerMap.Clear();

            foreach (var item in Layers)
            {
                item.Dispose();
            }
            Layers.Clear();
        }
    }
}