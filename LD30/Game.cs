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
        public const int TileSize = TilemapSize * 3;

        List<GameObject> gameObjects = new List<GameObject>();
        OverWorld overWorld;
        Player player;

        public Game()
            : base(null)
        {
            MainWindow = new RenderWindow(new VideoMode(1024, 768), "LD30", Styles.Close, new ContextSettings() { AntialiasingLevel = 0 });
        }

        public Game Init()
        {
            ResourceManager.LoadResource<Image>("assets/tilemap.png", "tilemapImg");
            ResourceManager.DeriveResource<Image, Texture>("tilemapImg", "tilemapTex", s => new Texture(s));
            ResourceManager.DeriveResource<Texture, Sprite>("tilemapTex", "grassSpr", s => Utility.CreateSubSprite(s, TilemapSize, TilemapSize, 0, 0));
            ResourceManager.DeriveResource<Texture, Sprite>("tilemapTex", "seaSpr", s => Utility.CreateSubSprite(s, TilemapSize, TilemapSize, 1, 0));

            ResourceManager.LoadResource<Image>("assets/overworld.png", "overworldImg");

            overWorld = new OverWorld(this, ResourceManager.GetResource<Image>("overworldImg"));
            gameObjects.Add(overWorld);

            player = new Player(this);
            gameObjects.Add(player);

            player.Position = overWorld.SpawnPosition;

            return this;
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
            foreach (var obj in gameObjects)
            {
                obj.Update(dt);
            }
        }

        public override void Draw(RenderTarget target)
        {
            foreach (var obj in gameObjects)
            {
                obj.Draw(target);
            }
        }
    }
}
