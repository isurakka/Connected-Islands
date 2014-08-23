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

            foreach (var treePos in findAllWorldPositionForColor(new Color(127, 64, 0)))
            {
                var spr = new Sprite(ResourceManager.GetResource<Sprite>("treeSpr"));
                spr.Scale = new Vector2f(Game.TilemapScale, Game.TilemapScale);
                var tree = new Tree(game, spr);
                tree.Position = treePos;
               
                game.Add(tree, 1);
            }
        }

        protected override SFML.Window.Vector2f getTilemapPositionForColor(Color color)
        {
            if (Utility.ColorEquals(color, Color.Green) || Utility.ColorEquals(color, Color.Black) || Utility.ColorEquals(color, new Color(127, 64, 0)))
                return Utility.GetTilemapPositionForCoords(0, 0);
            else if (Utility.ColorEquals(color, Color.Blue))
                return Utility.GetTilemapPositionForCoords(1, 0);
            else if (Utility.ColorEquals(color, Color.Yellow))
                return Utility.GetTilemapPositionForCoords(2, 0);

            throw new ArgumentException("No position found for such color.");
        }
    }
}
