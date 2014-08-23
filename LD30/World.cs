using SFML.Graphics;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD30
{
    abstract class World : GameObject
    {
        Tile[,] tiles;
        VertexArray tileVA;
        Image worldImage;

        public World(Game game, Image worldImage)
            : base(game)
        {
            this.worldImage = worldImage;
            tileVA = new VertexArray(PrimitiveType.Quads, worldImage.Size.X * worldImage.Size.Y * 4);
            for (int x = 0; x < worldImage.Size.X; x++)
            {
                for (int y = 0; y < worldImage.Size.Y; y++)
                {
                    var color = worldImage.GetPixel((uint)x, (uint)y);
                    var pos = getTilemapPositionForColor(color);
                    var topLeft = new Vertex(
                        new Vector2f(x * Game.TilemapSize, y * Game.TilemapSize), 
                        pos);
                    var topRight = new Vertex(
                        new Vector2f((x + 1) * Game.TilemapSize, y * Game.TilemapSize), 
                        pos + new Vector2f(Game.TilemapSize, 0f));
                    var botRight = new Vertex(
                        new Vector2f((x + 1) * Game.TilemapSize, (y + 1) * Game.TilemapSize), 
                        pos + new Vector2f(Game.TilemapSize, Game.TilemapSize));
                    var botLeft = new Vertex(
                        new Vector2f(x * Game.TilemapSize, (y + 1) * Game.TilemapSize), 
                        pos + new Vector2f(0f, Game.TilemapSize));

                    tileVA.Append(topLeft);
                    tileVA.Append(topRight);
                    tileVA.Append(botRight);
                    tileVA.Append(botLeft);
                }
            }
        }

        protected Vector2f findFirstWorldPositionForColor(Color findColor)
        {
            for (int x = 0; x < worldImage.Size.X; x++)
            {
                for (int y = 0; y < worldImage.Size.Y; y++)
                {
                    var color = worldImage.GetPixel((uint)x, (uint)y);
                    if (Utility.ColorEquals(color, findColor))
                        return new Vector2f(x * Game.TileSize, y * Game.TileSize);
                }
            }

            throw new ArgumentException("Couldn't find the specified color");
        }

        public override void Draw(RenderTarget target)
        {
            var states = RenderStates.Default;
            states.Transform.Scale(Game.TilemapScale, Game.TilemapScale);
            states.Texture = ResourceManager.GetResource<Texture>("tilemapTex");
            target.Draw(tileVA, states);
        }

        protected abstract Vector2f getTilemapPositionForColor(Color color);
    }
}
