using Snakie.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snakie.Scenes
{
    public partial class GameScene
    {
        public int ScoreMultiplier
        { get; set; }

        private void LoadScoreManager()
        {
            ScoreMultiplier = 1;
            FoodEaten += FoodEatenEventHandler;
        }

        private void UpdateScoreManager()
        {
            // ...
        }

        private void FoodEatenEventHandler(object sender, FoodEatenEventArgs e)
        {
            Score += ScoreMultiplier * CalculateScoreValue(e.Food);
        }

        private int CalculateScoreValue(Food food)
        {
            if (food.Type == FoodType.LiveBonus ||
                food.Type == FoodType.ScoreBonus)
                return 5;

            if (food.Type == FoodType.SlowDownBonus ||
                food.Type == FoodType.SpeedUpBonus)
                return 3;

            return 1;
        }
    }
}
