using System.IO;

namespace SpriteFactory.Documents
{
    public class DocumentContext
    {
        public DocumentContext()
            : this(null)
        {
        }

        public DocumentContext(string filePath)
        {
            FilePath = filePath;
        }

        public bool IsNewFile => string.IsNullOrEmpty(FilePath);
        public string FilePath { get; }
        public string Directory => Path.GetDirectoryName(FilePath);
        public string GetRelativePath(string path) => Catel.IO.Path.GetRelativePath(path, Directory);
        public string GetFullPath(string path) => Path.Combine(Directory, path);
    }
}