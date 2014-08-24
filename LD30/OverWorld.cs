using SFML.Graphics;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace LD30
{
    class OverWorld : World
    {
        public readonly Vector2f SpawnPosition;

        public OverWorld(Game game, Image worldImage)
            : base(game, worldImage)
        {
            SpawnPosition = FindFirstWorldPositionForColor(Color.Black);
            PositionCache.Add("CaveEntrance", FindFirstWorldPositionForColor(new Color(255, 0, 255)));

            foreach (var treePos in FindAllLocalPositionsForColor(new Color(127, 64, 0)))
            {
                var spr = new Sprite(ResourceManager.GetResource<Sprite>("treeSpr"));
                spr.Scale = new Vector2f(Game.TilemapScale, Game.TilemapScale);
                var tree = new Tree(game, spr);
                tree.Position = new Vector2f(treePos.X * Game.TileSize, treePos.Y * Game.TileSize);
                tree.MyWorld = this;
               
                game.Add(tree, 1);
                Collisions[treePos.X, treePos.Y] = true;
                Collisions[treePos.X + 1, treePos.Y] = true;
            }

            foreach (var rockPos in FindAllLocalPositionsForColor(new Color(64, 64, 64)))
            {
                var spr = new Sprite(ResourceManager.GetResource<Sprite>("rockSpr"));
                spr.Scale = new Vector2f(Game.TilemapScale, Game.TilemapScale);
                var rock = new Rock(game, spr);
                rock.Position = new Vector2f(rockPos.X * Game.TileSize, rockPos.Y * Game.TileSize);
                rock.MyWorld = this;

                game.Add(rock, 1);
                Collisions[rockPos.X, rockPos.Y] = true;
            }

            foreach (var flowerPos in FindAllLocalPositionsForColor(new Color(64, 127, 0)))
            {
                var spr = new Sprite(ResourceManager.GetResource<Sprite>("flowerSpr"));
                spr.Scale = new Vector2f(Game.TilemapScale, Game.TilemapScale);
                var flower = new Flower(game, spr);
                flower.Position = new Vector2f(flowerPos.X * Game.TileSize, flowerPos.Y * Game.TileSize);
                flower.MyWorld = this;

                game.Add(flower, 1);
            }
        }

        protected override SFML.Window.Vector2f getTilemapPositionForColor(Color color)
        {
            if (Utility.ColorEquals(color, Color.Green) || 
                Utility.ColorEquals(color, Color.Black) || 
                Utility.ColorEquals(color, new Color(127, 64, 0)) || 
                Utility.ColorEquals(color, new Color(64, 64, 64)) || 
                Utility.ColorEquals(color, new Color(64, 127, 0)) ||
                Utility.ColorEquals(color, new Color(127, 0, 255)) ||
                Utility.ColorEquals(color, new Color(128, 0, 255)) ||
                Utility.ColorEquals(color, new Color(129, 0, 255)) ||
                Utility.ColorEquals(color, new Color(130, 0, 255)) ||
                Utility.ColorEquals(color, new Color(131, 0, 255)))
                return Utility.GetTilemapPositionForCoords(0, 0);
            else if (
                Utility.ColorEquals(color, Color.Blue) ||
                Utility.ColorEquals(color, new Color(0, 127, 100)))
                return Utility.GetTilemapPositionForCoords(1, 0);
            else if (Utility.ColorEquals(color, Color.Yellow))
                return Utility.GetTilemapPositionForCoords(2, 0);
            else if (Utility.ColorEquals(color, new Color(200, 100, 0)))
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
