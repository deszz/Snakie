using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snakie.Scenes;
using Microsoft.Xna.Framework.Graphics;

namespace Snakie.World
{
    public class ScoreBonus : Bonus
    {
        private static Texture2D texture;

        static ScoreBonus()
        {
            texture = App.LoadRes<Texture2D>("UI/ScoreBonus");
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
                return 25;
            }
        }

        protected override void OnApply(GameScene scene)
        {
            scene.ScoreMultiplier += 1;
        }

        protected override void OnCancel(GameScene scene)
        {
            scene.ScoreMultiplier -= 1;
        }
    }
}
