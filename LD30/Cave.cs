using SFML.Graphics;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD30
{
    class Cave : World
    {
        public Cave(Game game, Image worldImage)
            : base(game, worldImage)
        {
            PositionCache.Add("CaveEntrance", findFirstWorldPositionForColor(new Color(255, 0, 255)));
        }

        protected override SFML.Window.Vector2f getTilemapPositionForColor(Color color)
        {
            if (Utility.ColorEquals(color, new Color(200, 100, 0)))
                return Utility.GetTilemapPositionForCoords(0, 1);
            else if (Utility.ColorEquals(color, new Color(64, 32, 0)))
                return Utility.GetTilemapPositionForCoords(1, 1);
            else if (Utility.ColorEquals(color, new Color(255, 0, 255)))
                return Utility.GetTilemapPositionForCoords(2, 1);

            throw new ArgumentException("No position found for such color.");
        }

        protected override bool getCollisionForColor(Color color)
        {
            if (Utility.ColorEquals(color, new Color(64, 32, 0)))
                return true;

            return false;
        }
    }
}
