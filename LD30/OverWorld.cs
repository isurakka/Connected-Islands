using SFML.Graphics;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD30
{
    class OverWorld : World
    {
        public readonly Vector2f SpawnPosition;

        public OverWorld(Game game, Image worldImage)
            : base(game, worldImage)
        {
            SpawnPosition = findFirstWorldPositionForColor(Color.Black);
        }

        protected override SFML.Window.Vector2f getTilemapPositionForColor(Color color)
        {
            if (Utility.ColorEquals(color, Color.Green) || Utility.ColorEquals(color, Color.Black))
                return Utility.GetTilemapPositionForCoords(0, 0);
            else if (Utility.ColorEquals(color, Color.Blue))
                return Utility.GetTilemapPositionForCoords(1, 0);
            else if (Utility.ColorEquals(color, Color.Yellow))
                return Utility.GetTilemapPositionForCoords(2, 0);

            throw new ArgumentException("No position found for such color.");
        }
    }
}
