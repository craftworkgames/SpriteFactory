using System.IO;

namespace SpriteFactory.GameObjects
{
    public class GameObject
    {
        public GameObject(string fullPath)
        {
            FullPath = fullPath;
        }

        public string Name => Path.GetFileName(FullPath);
        public string FullPath { get; }

        public override string ToString() => Name;
    }
}