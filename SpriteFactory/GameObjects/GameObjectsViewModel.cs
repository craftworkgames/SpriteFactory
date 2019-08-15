using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace SpriteFactory.GameObjects
{
    public class GameObjectsViewModel : ViewModel
    {
        public GameObjectsViewModel()
        {
            const string hardCodedPath = @"D:\Github\IdleInvestments\Assets";
            
            var gameObjects = GetFilesRecursive(hardCodedPath)
                .Select(f => new GameObject(f));

            Items = new ObservableCollection<GameObject>(gameObjects);
        }

        private IEnumerable<string> GetFilesRecursive(string path)
        {
            foreach (var directory in Directory.GetDirectories(path))
            {
                foreach (var subFile in GetFilesRecursive(directory))
                    yield return subFile;
            } 

            foreach (var file in Directory.GetFiles(path))
                yield return file;
        }


        public ObservableCollection<GameObject> Items { get; }
        public GameObject SelectedItem { get; set; }
    }
}