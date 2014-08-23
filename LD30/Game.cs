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
    class Game : GameObject
    {
        static void Main(string[] args)
        {
            new Game().Init().Run();
        }

        public RenderWindow MainWindow;
        public const int TilemapSize = 24;
        public const int TilemapScale = 3;
        public const int TileSize = TilemapSize * TilemapScale;

        SortedDictionary<int, List<GameObject>> gameObjects = new SortedDictionary<int, List<GameObject>>();
        OverWorld overWorld;
        Player player;

        public Game()
            : base(null)
        {
            MainWindow = new RenderWindow(new VideoMode(1600, 900), "LD30", Styles.Close, new ContextSettings() { AntialiasingLevel = 0 });
            MainWindow.SetFramerateLimit(240u);
        }

        public Game Init()
        {
            ResourceManager.LoadResource<Image>("assets/tilemap.png", "tilemapImg");
            ResourceManager.DeriveResource<Image, Texture>("tilemapImg", "tilemapTex", s => new Texture(s));
            ResourceManager.DeriveResource<Texture, Sprite>("tilemapTex", "playerSpr", s => Utility.CreateSubSprite(s, TilemapSize, TilemapSize, 0, 4));
            ResourceManager.DeriveResource<Texture, Sprite>("tilemapTex", "rockSpr", s => Utility.CreateSubSprite(s, TilemapSize, TilemapSize, 1, 4));
            ResourceManager.DeriveResource<Texture, Sprite>("tilemapTex", "treeSpr", s => Utility.CreateSubSprite(s, TilemapSize, TilemapSize, 0, 5, 2, 3));

            ResourceManager.LoadResource<Image>("assets/overworld.png", "overworldImg");

            overWorld = new OverWorld(this, ResourceManager.GetResource<Image>("overworldImg"));
            Add(overWorld);

            player = new Player(this, ResourceManager.GetResource<Sprite>("playerSpr"));
            Add(player, 1);

            player.Position = overWorld.SpawnPosition;

            return this;
        }

        public void Add(GameObject obj, int layer = 0)
        {
            if (!gameObjects.ContainsKey(layer))
                gameObjects.Add(layer, new List<GameObject>());

            gameObjects[layer].Add(obj);
        }

        public void Run()
        {
            var sw = new Stopwatch();
            sw.Start();
            while (MainWindow.IsOpen())
            {
                MainWindow.DispatchEvents();

                sw.Stop();
                float dt = (float)TimeSpan.FromTicks(sw.ElapsedTicks).TotalSeconds;
                sw.Reset();
                sw.Start();

                Update(dt);
                MainWindow.Clear();
                Draw(MainWindow);
                MainWindow.Display();
            }
        }

        public override void Update(float dt)
        {
            var oldPlayerPos = player.Position;

            foreach (var pair in gameObjects)
            {
                foreach (var obj in pair.Value)
                {
                    obj.Update(dt);
                }
            }

            var newPlayerPos = player.Position;
            var playerDir = newPlayerPos - oldPlayerPos;

            var playerLocalPos = new Vector2i(
                (int)Math.Floor(player.WorldCenter.X * (1f / Game.TileSize)),
                (int)Math.Floor(player.WorldCenter.Y * (1f / Game.TileSize)));

            var upLocal = playerLocalPos + new Vector2i(0, -1);
            var upRect = overWorld.GetWorldFloatRectForTile(playerLocalPos + new Vector2i(0, -1));
            var downLocal = playerLocalPos + new Vector2i(0, 1);
            var downRect = overWorld.GetWorldFloatRectForTile(playerLocalPos + new Vector2i(0, 1));

            var leftLocal = playerLocalPos + new Vector2i(-1, 0);
            var leftRect = overWorld.GetWorldFloatRectForTile(playerLocalPos + new Vector2i(-1, 0));
            var rightLocal = playerLocalPos + new Vector2i(1, 0);
            var rightRect = overWorld.GetWorldFloatRectForTile(playerLocalPos + new Vector2i(1, 0));

            var playerRect = new FloatRect(player.Position.X, player.Position.Y, Game.TileSize, Game.TileSize);
            if (overWorld.Collisions[upLocal.X, upLocal.Y] && playerRect.Intersects(upRect))
                player.Position += new Vector2f(0f, -(player.Position.Y - (upRect.Top + upRect.Height)));
            if (overWorld.Collisions[downLocal.X, downLocal.Y] && playerRect.Intersects(downRect))
                player.Position += new Vector2f(0f, -(player.Position.Y + Game.TileSize - downRect.Top));
            if (overWorld.Collisions[leftLocal.X, leftLocal.Y] && playerRect.Intersects(leftRect))
                player.Position += new Vector2f(-(player.Position.X - (leftRect.Left + leftRect.Width)), 0f);
            if (overWorld.Collisions[rightLocal.X, rightLocal.Y] && playerRect.Intersects(rightRect))
                player.Position += new Vector2f(-(player.Position.X + Game.TileSize - rightRect.Left), 0f);
            
            var color = overWorld.GetColorAtWorldPosition(player.WorldCenter);
            if (Utility.ColorEquals(color, Color.Blue))
            {
                player.SpeedModifier = 0.5f;
                player.HalfVertical = true;
            }
            else
            {
                player.SpeedModifier = 1f;
                player.HalfVertical = false;
            }
        }

        public override void Draw(RenderTarget target)
        {
            foreach (var pair in gameObjects)
            {
                pair.Value.Sort(new Comparison<GameObject>((obj1, obj2) => (int)(obj1.Depth - obj2.Depth)));
                foreach (var obj in pair.Value)
                {
                    obj.Draw(target);
                }
            }
        }
    }
}
