using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using SpriteFactory.Json;

namespace SpriteFactory
{
    public class Document<T> where T : new()
    {
        public Document()
            : this(new T())
        {
        }

        public Document(T content)
        {
            Content = content;
            IsSaved = false;
        }

        public T Content { get; }
        public bool IsSaved { get; set; }
        public string FullPath { get; set; }

        public void Save(string fullPath, T content)
        {
            var jsonSerializer = CreateJsonSerializer();

            using (var streamWriter = new StreamWriter(fullPath))
            using (var jsonWriter = new JsonTextWriter(streamWriter))
            {
                jsonSerializer.Serialize(jsonWriter, content);
            }
        }

        public static Document<T> Load(string fullPath)
        {
            var jsonSerializer = CreateJsonSerializer();

            using (var streamReader = new StreamReader(fullPath))
            using (var jsonReader = new JsonTextReader(streamReader))
            {
                var content = jsonSerializer.Deserialize<T>(jsonReader);
                return new Document<T>(content);
            }
        }

        private static JsonSerializer CreateJsonSerializer()
        {
            var jsonSerializer = new JsonSerializer
            {
                Formatting = Formatting.Indented,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Converters =
                {
                    new StringEnumConverter(),
                    new FlatObservableCollectionIntConverter()
                }
            };
            return jsonSerializer;
        }
    }
}