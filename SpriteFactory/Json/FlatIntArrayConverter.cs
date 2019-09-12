using System;
using Newtonsoft.Json;

namespace SpriteFactory.Json
{
    public class FlatIntArrayConverter : JsonConverter<int[]>
    {
        public override void WriteJson(JsonWriter writer, int[] value, JsonSerializer serializer)
        {
            var formatting = writer.Formatting;

            writer.Formatting = Formatting.None;
            writer.WriteWhitespace(" ");
            writer.WriteStartArray();

            foreach (var v in value)
                writer.WriteValue(v);

            writer.WriteEndArray();
            writer.Formatting = formatting;
        }

        public override bool CanRead => false;

        public override int[] ReadJson(JsonReader reader, Type objectType, int[] existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            throw new NotSupportedException();
        }
    }
}