using LottieSharp.Model.Animatable;
using LottieSharp.Model.Content;

namespace LottieSharp.Parser
{
    static class RepeaterParser
    {
        internal static Repeater Parse(JsonReader reader, LottieComposition composition)
        {
            string name = null;
            AnimatableFloatValue copies = null;
            AnimatableFloatValue offset = null;
            AnimatableTransform transform = null;

            while (reader.HasNext())
            {
                switch (reader.NextName())
                {
                    case "nm":
                        name = reader.NextString();
                        break;
                    case "c":
                        copies = AnimatableValueParser.ParseFloat(reader, composition, false);
                        break;
                    case "o":
                        offset = AnimatableValueParser.ParseFloat(reader, composition, false);
                        break;
                    case "tr":
                        transform = AnimatableTransformParser.Parse(reader, composition);
                        break;
                    default:
                        reader.SkipValue();
                        break;
                }
            }

            return new Repeater(name, copies, offset, transform);
        }
    }
}
