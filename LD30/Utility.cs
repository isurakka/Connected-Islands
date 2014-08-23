using SFML.Graphics;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD30
{
    static class Utility
    {
        public static Sprite CreateSubSprite(Texture tex, int subWidth, int subHeight, int x, int y, int xTiles = 1, int yTiles = 1)
        {
            int texX = x * subWidth;
            int texY = y * subHeight;
            return new Sprite(tex, new IntRect(texX, texY, subWidth * xTiles, subHeight * yTiles));
        }

        public static bool ColorEquals(Color a, Color b)
        {
            return 
                a.R == b.R && 
                a.G == b.G && 
                a.B == b.B && 
                a.A == b.A;
        }

        public static Vector2f Normalize(this Vector2f v)
        {
            float len = (float)Math.Sqrt(Math.Pow((double)v.X, 2) + Math.Pow((double)v.Y, 2));
            return new Vector2f(len != 0f ? v.X / len : 0f, len != 0f ? v.Y / len : 0f);
        }

        public static Vector2f GetTilemapPositionForCoords(int x, int y)
        {
            return new Vector2f(x * Game.TilemapSize, y * Game.TilemapSize);
        }
    }
}
