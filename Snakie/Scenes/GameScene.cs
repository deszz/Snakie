using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Snakie.World;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snakie.Scenes
{
    public partial class GameScene : Scene
    {
        private const int defaultScore = 0;
        private const int defaultLives = 3;

        private static Random rand = new Random();

        public bool IsPaused
        { get; private set; }

        public int Lives
        { get; set; }
        public int Score
        { get; set; }
        public Snake Snake
        { get; private set; }

        public GameScene(int score = defaultScore, int lives = defaultLives)
        {
            Lives = lives;
            Score = score;
        }

        public override void OnLoad()
        {
            Init();

            LoadFoodManager();
            LoadScoreManager();

            LoadUI();

            base.OnLoad();
        }

        public override void OnUpdate()
        {
            if (!IsPaused)
            {
                UpdateFoodManager();
                UpdateScoreManager();
            }

            UpdateUI();

            base.OnUpdate();
        }

        public override void OnDraw(SpriteBatch sBatch)
        {
            DrawUI(sBatch);

            base.OnDraw(sBatch);
        }

        public void AddLive()
        {
            Lives++;
        }

        private void Init()
        {
            Snake = new Snake();
            AddToScene(Snake);
        }

        private void ReloadScene(int initScore = defaultScore, int initLives = defaultLives)
        {
            App.LoadScene(new GameScene(initScore, initLives));
        }

        private void Resume()
        {
            IsPaused = false;
            Snake.IsPaused = false;
        }

        private void Pause()
        {
            IsPaused = true;
            Snake.IsPaused = true;
        }

        #region Utils

        private Point GetRandomPointOnScreen(int bounds = 0)
        {
            return new Point(rand.Next(bounds, App.Viewport.Width - bounds),
                             rand.Next(bounds, App.Viewport.Height - bounds));
        }

        #endregion
    }
}
