using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace SpriteFactory.Scenes
{
    public class Scene
    {
        public List<SceneEntity> Entities = new List<SceneEntity>();

        public SceneEntity EntityAt(Vector2 position)
        {
            return Entities.LastOrDefault(e => e.BoundingRectangle.Contains(position));
        }
    }
}