using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snakie.Scenes;
using Microsoft.Xna.Framework.Graphics;

namespace Snakie.World
{
    public class SlowDownBonus : Bonus
    {
        private static Texture2D texture;

        static SlowDownBonus()
        {
            texture = App.LoadRes<Texture2D>("UI/SlowDownBonus");
        }

        public override Texture2D Icon
        {
            get
            {
                return texture;
            }
        }

        public override bool IsOneTimeBonus
        {
            get
            {
                return false;
            }
        }

        public override float BonusTime
        {
            get
            {
                return 15;
            }
        }

        protected override void OnApply(GameScene scene)
        {
            scene.Snake.BonusSpeed -= 100;
            scene.Snake.BonusMovement -= 0.375f;
        }

        protected override void OnCancel(GameScene scene)
        {
            scene.Snake.BonusSpeed += 100;
            scene.Snake.BonusMovement += 0.375f;
        }
    }
}
