using SFML.Graphics;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD30
{
    abstract class World : GameObject
    {
        public bool[,] Collisions;
        VertexArray tileVA;
        Image worldImage;
        RenderStates states = RenderStates.Default;
        public Dictionary<string, Vector2f> PositionCache = new Dictionary<string, Vector2f>();

        public override bool Enabled
        {
            get
            {
                return base.Enabled;
            }
            set
            {
                base.Enabled = value;

                if (!value)
                {
                    tileVA.Clear();
                }
            }
        }

        public World(Game game, Image worldImage)
            : base(game)
        {
            this.worldImage = worldImage;
            tileVA = new VertexArray(PrimitiveType.Quads, worldImage.Size.X * worldImage.Size.Y * 4);
            
            Collisions = new bool[worldImage.Size.X, worldImage.Size.Y];

            states.Transform.Scale(Game.TilemapScale, Game.TilemapScale);
            states.Texture = ResourceManager.GetResource<Texture>("tilemapTex");

            for (int x = 0; x < worldImage.Size.X; x++)
            {
                for (int y = 0; y < worldImage.Size.Y; y++)
                {
                    if (getCollisionForColor(worldImage.GetPixel((uint)x, (uint)y)))
                        Collisions[x, y] = true;
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

        public List<Vector2i> FindAllLocalPositionsForColor(Color findColor)
        {
            var finds = new List<Vector2i>();
            for (int x = 0; x < worldImage.Size.X; x++)
            {
                for (int y = 0; y < worldImage.Size.Y; y++)
                {
                    var color = worldImage.GetPixel((uint)x, (uint)y);
                    if (Utility.ColorEquals(color, findColor))
                        finds.Add(new Vector2i(x, y));
                }
            }
            return finds;
        }

        public List<Vector2f> FindAllWorldPositionsForColor(Color findColor)
        {
            var findsLocal = FindAllLocalPositionsForColor(findColor);
            var finds = new List<Vector2f>();
            for (int i = 0; i < findsLocal.Count; i++)
            {
                finds.Add(new Vector2f(findsLocal[i].X * Game.TileSize, findsLocal[i].Y * Game.TileSize));
            }
            return finds;
        }

        public Color GetColorAtWorldPosition(Vector2f worldPos)
        {
            var localPos = worldPos * (1f / Game.TileSize);
            var color = worldImage.GetPixel((uint)Math.Floor(localPos.X), (uint)Math.Floor(localPos.Y));
            return color;
        }

        public FloatRect GetWorldFloatRectForTile(Vector2i coords)
        {
            return new FloatRect(coords.X * Game.TileSize, coords.Y * Game.TileSize, Game.TileSize, Game.TileSize);
        }

        public override void Draw(RenderTarget target)
        {
            if (!Enabled)
                return;

            var topLeftView = target.MapPixelToCoords(new Vector2i());
            var botRightView = target.MapPixelToCoords(new Vector2i((int)game.MainWindow.Size.X - 1, (int)game.MainWindow.Size.Y - 1));
            var viewRect = new FloatRect(topLeftView.X, topLeftView.Y, botRightView.X - topLeftView.X, botRightView.Y - topLeftView.Y);

            var topLeftLocal = new Vector2i((int)Math.Floor(topLeftView.X / Game.TileSize), (int)Math.Floor(topLeftView.Y / Game.TileSize));
            var botRightLocal = new Vector2i((int)Math.Ceiling(botRightView.X / Game.TileSize), (int)Math.Ceiling(botRightView.Y / Game.TileSize));

            tileVA.Clear();
            for (int x = topLeftLocal.X; x < botRightLocal.X; x++)
            {
                for (int y = topLeftLocal.Y; y < botRightLocal.Y; y++)
                {
                    if (x < 0 || y < 0 || x > worldImage.Size.X - 1 || y > worldImage.Size.Y - 1)
                        continue;

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

            target.Draw(tileVA, states);
        }

        protected abstract Vector2f getTilemapPositionForColor(Color color);
        protected abstract bool getCollisionForColor(Color color);
    }
}
