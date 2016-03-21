using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Snakie.Scenes;
using System;
using System.Diagnostics;

namespace Snakie
{
    public class App : Game
    {
        #region Debug

        public static GraphicsDevice __Debug_GraphicsDevice
        {
            get
            {
                return instance.GraphicsDevice;
            }
        }
        public static void __Debug_DrawRect(SpriteBatch sBatch, Rectangle rect, Color? color = null)
        {
            if (color == null)
                color = new Color(Color.Red, 0.7f);

            var tex = new Texture2D(__Debug_GraphicsDevice, rect.Width, rect.Height);
            Color[] data = new Color[rect.Width * rect.Height];

            for (int i = 0; i < data.Length; ++i)
                data[i] = color.Value;
            tex.SetData(data);

            sBatch.Draw(tex, rect, Color.White);
        }

        #endregion

        public const int TargetFPS = 60;

        public static float FrameTime
        { get; private set; }
        public static float TotalGameTime
        { get; private set; }

        public static Viewport Viewport
        {
            get
            {
                return instance.GraphicsDevice.Viewport;
            }
        }

        private static Random rand;
        private static App instance;

        public static int GetRandom(int min, int max)
        {
            if (rand == null)
                rand = new Random();

            return rand.Next(min, max);
        }

        public static T LoadRes<T>(string resName)
        {
            return instance.Content.Load<T>(resName);
        }

        public static void LoadScene(Scene scene)
        {
            instance.SetNextScene(scene);
        }

        public static void ExitGame()
        {
            instance.exitRequested = true;
        }

        public GraphicsDeviceManager graphics;
        public SpriteBatch spriteBatch;

        private Scene currentScene;
        private Scene nextScene;

        private bool exitRequested;

        public App()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            instance = this;
        }

        protected override void Initialize()
        {
            base.Initialize();

            IsMouseVisible = true;
            TargetElapsedTime = TimeSpan.FromSeconds(1f / TargetFPS);

            SetNextScene(new GameScene());
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
            if (exitRequested)
                Exit();

            if (nextScene != null)
            {
                if (currentScene != null)
                    currentScene.OnDestroy();

                currentScene = nextScene;
                currentScene.OnLoad();

                nextScene = null;
            }

            TotalGameTime = (float)gameTime.TotalGameTime.TotalSeconds;
            FrameTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (currentScene != null)
                currentScene.UpdateScene();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            spriteBatch.Begin();

            if (currentScene != null)
                currentScene.DrawScene(spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void SetNextScene(Scene scene)
        {
            nextScene = scene;
        }
    }
}
