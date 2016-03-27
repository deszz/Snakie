using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snakie.World
{
    public class Food : GameObject
    {
        private const float animationTime = 0.25f; // in seconds

        private static Color[] colorsList = { Color.Red/*, Color.Blue,
                                              Color.Green*/ };

        public Rectangle Collider
        {
            get
            {
                return collider;
            }
        }

        public bool IsExpired
        {
            get
            {
                return App.TotalGameTime - CreationTime > ShelfLife;
            }
        }

        public double ShelfLife
        { get; private set; }
        public double CreationTime
        { get; private set; }
        public Bonus Bonus
        { get; private set; }
        public FoodType Type
        { get; private set; }

        private float alpha;
        private Color color;
        private Rectangle rect;
        private Rectangle collider;
        private Texture2D texture;

        private bool destroying;
        private bool destroyed;

        public Food(FoodType type, Point position)
        {
            CreationTime = App.TotalGameTime;
            Type = type;

            alpha = 0;
            color = (type == FoodType.Figure) ? GetRandomColor() : Color.White;
            rect = new Rectangle(position, new Point(32, 32));
            collider = new Rectangle(position.X - 2, position.Y - 2, 
                                     rect.Width - 4, rect.Height - 4);

            switch (type)
            {
                case FoodType.LiveBonus:
                    texture = App.LoadRes<Texture2D>("World/Food/LiveBonus");
                    Bonus = new LiveBonus();
                    ShelfLife = 3.5;
                    break;
                case FoodType.SpeedUpBonus:
                    texture = App.LoadRes<Texture2D>("World/Food/SpeedUpBonus");
                    Bonus = new SpeedUpBonus();
                    ShelfLife = 6;
                    break;
                case FoodType.SlowDownBonus:
                    texture = App.LoadRes<Texture2D>("World/Food/SlowDownBonus");
                    Bonus = new SlowDownBonus();
                    ShelfLife = 6;
                    break;
                case FoodType.ScoreBonus:
                    texture = App.LoadRes<Texture2D>("World/Food/ScoreBonus");
                    Bonus = new ScoreBonus();
                    ShelfLife = 4;
                    break;
                default:
                    texture = GetRandomFigureTexture();
                    Bonus = null;
                    ShelfLife = 10;
                    break;
            }
        }

        public override void OnUpdate()
        {
            var factor = GetFactor();

            if (destroying)
            {
                if (alpha > 0)
                {
                    rect = ResizeRect(rect, factor);
                    alpha -= factor;
                }
                else
                {
                    destroyed = true;
                }
            }
            else
            {
                if (alpha < 1)
                    alpha += factor;
            }

            base.OnUpdate();
        }

        public override void OnDraw(SpriteBatch sBatch)
        {
            sBatch.Draw(texture, rect, new Color(color, alpha));

            base.OnDraw(sBatch);
        }

        public override void OnDestroy(ref bool cancel)
        {
            if (destroyed)
                return;

            cancel = true;
            if (!destroying)
            {
                destroying = true;
                collider = Rectangle.Empty;
            }
        }

        private Color GetRandomColor()
        {
            return colorsList[App.GetRandom(0, colorsList.Length)];
        }

        private Texture2D GetRandomFigureTexture()
        {
            return App.LoadRes<Texture2D>("World/Food/" + ("Figure_" + App.GetRandom(1, 4)));
        }

        private float GetFactor()
        {
            return (1 / animationTime) * App.FrameTime;
        }

        private Rectangle ResizeRect(Rectangle rect, float factor)
        {
            return new Rectangle(rect.X - (int)((rect.Width * factor) / 2),
                                 rect.Y - (int)((rect.Height * factor) / 2),
                                 rect.Width + (int)(rect.Width * factor),
                                 rect.Height + (int)(rect.Height * factor));
        }
    }
}
