using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snakie
{
    public abstract class Scene : AppObject
    {
        private List<GameObject> sceneObjects = new List<GameObject>();

        public void UpdateScene()
        {
            for (int i = sceneObjects.Count - 1; i >= 0; --i)
            {
                if (sceneObjects[i].IsDestroyed)
                {
                    sceneObjects.RemoveAt(i);
                    continue;
                }

                if (sceneObjects[i].IsEnabled)
                    sceneObjects[i].OnUpdate();
            }

            OnUpdate();
        }

        public void DrawScene(SpriteBatch sBatch)
        {
            for (int i = sceneObjects.Count - 1; i >= 0; --i)
            {
                if (sceneObjects[i].IsEnabled)
                    sceneObjects[i].OnDraw(sBatch);
            }

            OnDraw(sBatch);
        }

        public IEnumerable<GameObject> GetSceneObjects()
        {
            for (int i = 0; i < sceneObjects.Count; ++i)
                yield return sceneObjects[i];
        }

        public void AddToScene(GameObject gameObject)
        {
            if (gameObject == null)
                throw new ArgumentNullException(nameof(gameObject));

            sceneObjects.Add(gameObject);
        }

        public void RemoveFromScene(GameObject gameObject)
        {
            if (gameObject == null)
                throw new ArgumentNullException(nameof(gameObject));

            gameObject.Destroy();
        }
    }
}
