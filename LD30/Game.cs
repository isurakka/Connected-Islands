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
        }

        public Game Init()
        {
            ResourceManager.LoadResource<Image>("assets/tilemap.png", "tilemapImg");
            ResourceManager.DeriveResource<Image, Texture>("tilemapImg", "tilemapTex", s => new Texture(s));
            ResourceManager.DeriveResource<Texture, Sprite>("tilemapTex", "playerSpr", s => Utility.CreateSubSprite(s, TilemapSize, TilemapSize, 0, 4));
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
            foreach (var pair in gameObjects)
            {
                foreach (var obj in pair.Value)
                {
                    obj.Update(dt);
                }
            }

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
