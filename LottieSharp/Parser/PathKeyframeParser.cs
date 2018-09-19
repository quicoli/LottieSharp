using SharpDX;
using LottieSharp.Value;
using LottieSharp.Animation.Keyframe;
using Newtonsoft.Json;

namespace LottieSharp.Parser
{
    static class PathKeyframeParser
    {
        internal static PathKeyframe Parse(JsonReader reader, LottieComposition composition)
        {
            bool animated = reader.Peek() == JsonToken.StartObject;
            Keyframe<Vector2?> keyframe = KeyframeParser.Parse(reader, composition, Utils.Utils.DpScale(), PathParser.Instance, animated);

            return new PathKeyframe(composition, keyframe);
        }
    }
}
