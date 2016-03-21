using Microsoft.Xna.Framework.Graphics;
using Snakie.Scenes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snakie.World
{
    public abstract class Bonus
    {
        public bool IsExpired
        {
            get
            {
                return TimeLeft < 0;
            }
        }

        public float TimeLeft
        {
            get
            {
                return BonusTime - (App.TotalGameTime - AppliedTime);
            }
        }

        public float AppliedTime
        { get; private set; }

        public virtual Texture2D Icon
        {
            get
            {
                return null;
            }
        }

        public abstract bool IsOneTimeBonus
        { get; }
        public abstract float BonusTime
        { get; }

        public void Apply(GameScene scene)
        {
            AppliedTime = App.TotalGameTime;

            OnApply(scene);
        }

        public void Cancel(GameScene scene)
        {
            OnCancel(scene);
        }

        protected abstract void OnApply(GameScene scene);
        protected virtual void OnCancel(GameScene scene) { }
    }
}
