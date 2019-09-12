using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using SpriteFactory.Json;

namespace SpriteFactory.Documents
{
    public class Document<T> where T : new()
    {
        private Document(string fullPath, T content)
        {
            FullPath = fullPath;
            Content = content;
            IsSaved = true;
        }

        public string FullPath { get; private set; }
        public bool IsNew => string.IsNullOrEmpty(FullPath);
        public bool IsSaved { get; set; }
        public T Content { get; }

        public string Directory => Path.GetDirectoryName(FullPath);
        public string GetRelativePath(string path) => Catel.IO.Path.GetRelativePath(path, Directory);
        public string GetFullPath(string path) => Path.Combine(Directory, path);

        public static Document<T> New() => new Document<T>(null, new T());

        public void Save(Func<Document<T>, T> getContent) => Save(FullPath, getContent);

        public void Save(string fullPath, Func<Document<T>, T> getContent)
        {
            var oldFullPath = FullPath;

            try
            {
                FullPath = fullPath;

                var jsonSerializer = CreateJsonSerializer();

                using (var streamWriter = new StreamWriter(fullPath))
                using (var jsonWriter = new JsonTextWriter(streamWriter))
                {
                    var content = getContent(this);
                    jsonSerializer.Serialize(jsonWriter, content);
                }

                IsSaved = true;
            }
            catch
            {
                FullPath = oldFullPath;
            }
        }

        public static Document<T> Load(string fullPath)
        {
            var jsonSerializer = CreateJsonSerializer();

            using (var streamReader = new StreamReader(fullPath))
            using (var jsonReader = new JsonTextReader(streamReader))
            {
                var content = jsonSerializer.Deserialize<T>(jsonReader);
                return new Document<T>(fullPath, content);
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
                    new FlatIntArrayConverter()
                }
            };
            return jsonSerializer;
        }
    }
}