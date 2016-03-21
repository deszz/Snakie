using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.BitmapFonts;
using Snakie.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snakie.Scenes
{
    public partial class GameScene
    {
        public enum Screen
        {
            Game,
            Pause,
            GetReady,
            Lose
        }

        public event EventHandler ScreenChanged;

        private BitmapFont cyrbit32Font;
        private BitmapFont cyrbit24Font;
        private BitmapFont cyrbit16Font;

        private Texture2D liveTexture;
        private Point liveSize;

        private Texture2D pauseButtonTexture;
        private Texture2D pausedBannerTexture;
        private Texture2D getReadyBannerTexture;
        private Texture2D loseBannerTexture;

        private Rectangle pauseButtonRect;
        private Rectangle pausedBannerRect;
        private Rectangle getReadyBannerRect;
        private Rectangle loseBannerRect;

        private Rectangle bonusIconRect;
        private Point bonusTimeleftLocation;

        private Screen screen;

        private SpriteBatch sBatch;
        private KeyboardState keyboardState;
        private MouseState mouseState;
        private List<Keys> keyboardPrevStates = new List<Keys>();

        private SoundEffect pickUpSound;
        private SoundEffect deathSound;

        public void LoadUI()
        {
            cyrbit32Font = App.LoadRes<BitmapFont>("Fonts/Cyrbit32");
            cyrbit24Font = App.LoadRes<BitmapFont>("Fonts/Cyrbit24");
            cyrbit16Font = App.LoadRes<BitmapFont>("Fonts/Cyrbit16");

            liveTexture = App.LoadRes<Texture2D>("UI/Hearth");
            liveSize = new Point(16, 16);

            pauseButtonTexture = App.LoadRes<Texture2D>("UI/PauseButton");
            pausedBannerTexture = App.LoadRes<Texture2D>("UI/PausedBanner");
            getReadyBannerTexture = App.LoadRes<Texture2D>("UI/GetReadyBanner");
            loseBannerTexture = App.LoadRes<Texture2D>("UI/LoseBanner");

            pauseButtonRect = new Rectangle(App.Viewport.Width - 42, 10, 32, 32);
            pausedBannerRect = GetCentredRect(pausedBannerTexture);
            getReadyBannerRect = GetCentredRect(getReadyBannerTexture);
            loseBannerRect = GetCentredRect(loseBannerTexture);

            var biSize = new Point(32, 32);
            bonusIconRect = new Rectangle(pauseButtonRect.X - biSize.X - 10, 
                                          pauseButtonRect.Center.Y - (biSize.Y / 2), 
                                          biSize.X, 
                                          biSize.Y);
            bonusTimeleftLocation = bonusIconRect.Location + new Point(-30, (biSize.Y / 2) - (cyrbit16Font.LineHeight / 2));

            pickUpSound = App.LoadRes<SoundEffect>("World/Food/PickUpSound");
            deathSound = App.LoadRes<SoundEffect>("World/Snake/DeathSound");

            FoodEaten += FoodEatenHandler;
            Snake.Death += SnakeDeathHandler;

            Pause();
            ChangeScreen(Screen.GetReady);
        }

        #region Update

        public void UpdateUI()
        {
            keyboardState = Keyboard.GetState();
            mouseState = Mouse.GetState();

            switch (screen)
            {
                case Screen.Game:
                    UpdateGameScreen();
                    break;
                case Screen.Pause:
                    UpdatePauseScreen();
                    break;
                case Screen.GetReady:
                    UpdateGetReadyScreen();
                    break;
                case Screen.Lose:
                    UpdateLoseScreen();
                    break;
            }

            keyboardPrevStates = new List<Keys>(keyboardState.GetPressedKeys());
        }

        private void UpdateGameScreen()
        {
            if (IsMouseClicked(pauseButtonRect) || IsKeyReleased(Keys.Space))
            {
                Pause();
                ChangeScreen(Screen.Pause);
            }
        }

        private void UpdatePauseScreen()
        {
            if (IsKeyReleased(Keys.Space))
            {
                Resume();
                ChangeScreen(Screen.Game);
            }
        }

        private void UpdateGetReadyScreen()
        {
            if (IsKeyReleased(Keys.Space))
            {
                Resume();
                ChangeScreen(Screen.Game);
            }
        }

        private void UpdateLoseScreen()
        {
            if (IsKeyReleased(Keys.Space))
                ReloadScene();
            else if (IsKeyReleased(Keys.Escape))
                App.ExitGame();
        }

        #endregion

        #region Draw

        private void DrawUI(SpriteBatch sBatch)
        {
            this.sBatch = sBatch;

            switch (screen)
            {
                case Screen.Game:
                    DrawGameScreen();
                    break;
                case Screen.Pause:
                    DrawPauseScreen();
                    break;
                case Screen.GetReady:
                    DrawGetReadyScreen();
                    break;
                case Screen.Lose:
                    DrawLoseScreen();
                    break;
            }
        }

        private void DrawGameScreen()
        {
            Draw(pauseButtonTexture, pauseButtonRect);
            Print(cyrbit24Font, "SCORE: " + Score, new Vector2(20, 10));

            for (int i = 0; i < Lives; ++i)
            {
                Draw(liveTexture, new Rectangle(175 + (i * (int)(liveSize.X * 1.5f)), 13,
                                                liveSize.X, liveSize.Y), Color.Black);
            }

            if (currentBonus?.Icon != null)
            {
                Draw(currentBonus.Icon, bonusIconRect);
                Print(cyrbit16Font, currentBonus.TimeLeft.ToString("0.0"), bonusTimeleftLocation);
            }
        }

        private void DrawPauseScreen()
        {
            Draw(pausedBannerTexture, pausedBannerRect);
        }

        private void DrawGetReadyScreen()
        {
            Draw(getReadyBannerTexture, getReadyBannerRect);
        }

        private void DrawLoseScreen()
        {
            Draw(loseBannerTexture, loseBannerRect);

            var scoreRect = cyrbit16Font.GetStringRectangle("SCORE: " + Score, App.Viewport.GetScreenCenter().ToVector2());
            var scorePos = new Vector2(scoreRect.Location.X - (scoreRect.Width / 2), scoreRect.Location.Y - (scoreRect.Height / 2) - 40);

            Print(cyrbit16Font, "SCORE: " + Score, scorePos);
        }

        #endregion

        private void ChangeScreen(Screen screen)
        {
            this.screen = screen;
            ScreenChanged?.Invoke(this, EventArgs.Empty);
        }

        private void FoodEatenHandler(object sender, FoodEatenEventArgs e)
        {
            pickUpSound.Play(0.15f, 0, 1);
        }

        private void SnakeDeathHandler(object sender, EventArgs e)
        {
            deathSound.Play();

            if (Lives == 0)
            {
                Pause();
                ChangeScreen(Screen.Lose);
            }
            else
            {
                ReloadScene(Score, Lives - 1);
            }
        }

        private void Draw(Texture2D texture, Rectangle rect, Color? color = null)
        {
            if (color == null)
                color = Color.White;

            sBatch.Draw(texture, rect, color.Value);
        }

        private void Print(BitmapFont font, string text, Point position, Color? color = null)
        {
            Print(font, text, position.ToVector2(), color);
        }

        private void Print(BitmapFont font, string text, Vector2 position, Color? color = null)
        {
            if (color == null)
                color = Color.Black;

            sBatch.DrawString(font, text, position, color.Value);
        }

        private bool IsMouseClicked(Rectangle rect)
        {
            if (mouseState.LeftButton == ButtonState.Pressed)
                return rect.Contains(mouseState.Position);

            return false;
        }

        private bool IsKeyReleased(Keys key)
        {
            return keyboardState.IsKeyUp(key) && 
                   keyboardPrevStates.IndexOf(key) != -1;
        }

        private bool IsKeyUp(Keys key)
        {
            return keyboardState.IsKeyUp(key);
        }

        private bool IsKeyDown(Keys key)
        {
            return keyboardState.IsKeyDown(key);
        }

        private Rectangle GetCentredRect(Texture2D texture)
        {
            return new Rectangle((App.Viewport.Width - texture.Width) / 2,
                                 (App.Viewport.Height - texture.Height) / 2,
                                 texture.Width, 
                                 texture.Height);
        }
    }
}
