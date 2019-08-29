using System;
using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace SpriteFactory.Json
{
    public class FlatObservableCollectionIntConverter : JsonConverter<ObservableCollection<int>>
    {
        public override void WriteJson(JsonWriter writer, ObservableCollection<int> value, JsonSerializer serializer)
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

        public override ObservableCollection<int> ReadJson(JsonReader reader, Type objectType, ObservableCollection<int> existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            throw new NotSupportedException();
        }
    }
}