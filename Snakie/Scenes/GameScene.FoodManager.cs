using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Snakie.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snakie.Scenes
{
    public class FoodEatenEventArgs : EventArgs
    {
        public Food Food;

        public FoodEatenEventArgs(Food food)
        {
            Food = food;
        }
    }

    public partial class GameScene
    {
        private static readonly Dictionary<FoodType, int> foodTypeChances = new Dictionary<FoodType, int>()
        {
            [FoodType.Figure] = 1000,
            [FoodType.LiveBonus] = 25,
            [FoodType.ScoreBonus] = 25,
            [FoodType.SlowDownBonus] = 75,
            [FoodType.SpeedUpBonus] = 75,
        };

        private event EventHandler<FoodEatenEventArgs> FoodEaten;

        private float addingDelay;
        private float elapsedTime;

        private Bonus currentBonus;

        private void LoadFoodManager()
        {
            addingDelay = 1.5f;
            elapsedTime = 0;
        }

        private void UpdateFoodManager()
        {
            elapsedTime += App.FrameTime;
            if (elapsedTime > addingDelay)
            {
                AddToScene(GetRandomFoodObject());
                elapsedTime = 0;
            }

            if (currentBonus != null)
            {
                if (currentBonus.IsExpired)
                {
                    currentBonus.Cancel(this);
                    currentBonus = null;
                }
            }

            CheckExpiryDates();
            CheckCollisions();
        }

        private void CheckExpiryDates()
        {
            var currentTime = App.TotalGameTime;

            foreach (var food in GetAllFoodObjects())
            {
                if (food.IsExpired)
                    food.Destroy();
            }
        }

        private void CheckCollisions()
        {
            foreach (var food in GetAllFoodObjects())
            {
                if (Snake.IsHeadIntersects(food.Collider))
                    OnFoodEaten(food);
            }
        }

        private Food GetRandomFoodObject()
        {
            return new Food(GetRandomFoodType(), GetRandomPointOnScreen(75));
        }

        private FoodType GetRandomFoodType()
        {
            var weightsSum = foodTypeChances.Sum((x) => x.Value);
            var randWeight = App.GetRandom(0, weightsSum);

            foreach (var f in foodTypeChances)
            {
                if (randWeight < f.Value)
                    return f.Key;

                randWeight -= f.Value;
            }

            throw new Exception("Unreachable code.");
        }

        private void OnFoodEaten(Food food)
        {
            elapsedTime += 0.375f;

            Snake.AddSegment();
            if (food.Bonus != null)
                ApplyBonus(food.Bonus);

            FoodEaten?.Invoke(this, new FoodEatenEventArgs(food));
            food.Destroy();
        }

        private void ApplyBonus(Bonus bonus)
        {
            if (bonus.IsOneTimeBonus)
            {
                bonus.Apply(this);
                return;
            }

            if (currentBonus != null)
                currentBonus.Cancel(this);

            currentBonus = bonus;
            currentBonus.Apply(this);
        }

        private IEnumerable<Food> GetAllFoodObjects()
        {
            foreach (var gameObject in GetSceneObjects())
            {
                var food = gameObject as Food;
                if (food != null)
                    yield return food;
            }
        }
    }
}
