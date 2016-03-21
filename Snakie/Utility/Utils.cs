using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snakie.Utility
{
    public static class Utils
    {
        public static Vector2 Normalized(this Vector2 vec2)
        {
            var temp = new Vector2(vec2.X, vec2.Y);
            temp.Normalize();
            return temp;
        }

        public static double DegToRad(float degrees)
        {
            return (Math.PI / 180) * degrees;
        }
        
        public static float Cap(float value, float min, float max)
        {
            return value > max ? max : value < min ? min : value;
        }
        
        public static Point GetScreenCenter(this Viewport viewport)
        {
            return new Point(viewport.Width / 2, viewport.Height / 2);
        }
    }
}
