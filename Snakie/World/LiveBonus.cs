using Snakie.Scenes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snakie.World
{
    public class LiveBonus : Bonus
    {
        public override bool IsOneTimeBonus
        {
            get
            {
                return true;
            }
        }

        public override float BonusTime
        {
            get
            {
                return 0;
            }
        }

        protected override void OnApply(GameScene scene)
        {
            scene.Lives++;
        }
    }
}
