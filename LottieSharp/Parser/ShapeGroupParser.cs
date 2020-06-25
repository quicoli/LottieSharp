using LottieSharp.Model.Content;
using System.Collections.Generic;

namespace LottieSharp.Parser
{
    static class ShapeGroupParser
    {
        internal static ShapeGroup Parse(JsonReader reader, LottieComposition composition)
        {
            string name = null;
            List<IContentModel> items = new List<IContentModel>();

            while (reader.HasNext())
            {
                switch (reader.NextName())
                {
                    case "nm":
                        name = reader.NextString();
                        break;
                    case "it":
                        reader.BeginArray();
                        while (reader.HasNext())
                        {
                            IContentModel newItem = ContentModelParser.Parse(reader, composition);
                            if (newItem != null)
                            {
                                items.Add(newItem);
                            }
                        }
                        reader.EndArray();
                        break;
                    default:
                        reader.SkipValue();
                        break;
                }
            }

            return new ShapeGroup(name, items);
        }
    }
}
