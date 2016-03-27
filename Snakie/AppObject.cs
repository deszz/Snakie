using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snakie
{
    public abstract class AppObject
    {
        public virtual void OnLoad() { }
        public virtual void OnUpdate() { }
        public virtual void OnDraw(SpriteBatch sBatch) { }

        public virtual void OnDestroy() { }
        public virtual void OnDestroy(ref bool cancel)
        {
            OnDestroy();
        }
    }
}
