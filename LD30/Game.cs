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
        Inventory inventory;
        MouseMenu mouseMenu;
        MessageModal messageModal;

        public Game()
            : base(null)
        {
            MainWindow = new RenderWindow(new VideoMode(1600, 900), "LD30", Styles.Close, new ContextSettings() { AntialiasingLevel = 8 });
            MainWindow.SetFramerateLimit(240u);
            MainWindow.TextEntered += MainWindow_TextEntered;
        }

        public Game Init()
        {
            ResourceManager.LoadResource<Image>("assets/tilemap.png", "tilemapImg");
            ResourceManager.DeriveResource<Image, Texture>("tilemapImg", "tilemapTex", s => new Texture(s));
            ResourceManager.DeriveResource<Texture, Sprite>("tilemapTex", "playerSpr", s => Utility.CreateSubSprite(s, TilemapSize, TilemapSize, 0, 4));
            ResourceManager.DeriveResource<Texture, Sprite>("tilemapTex", "rockSpr", s => Utility.CreateSubSprite(s, TilemapSize, TilemapSize, 1, 4));
            ResourceManager.DeriveResource<Texture, Sprite>("tilemapTex", "flowerSpr", s => Utility.CreateSubSprite(s, TilemapSize, TilemapSize, 2, 4));
            ResourceManager.DeriveResource<Texture, Sprite>("tilemapTex", "bottleSpr", s => Utility.CreateSubSprite(s, TilemapSize, TilemapSize, 2, 6));
            ResourceManager.DeriveResource<Texture, Sprite>("tilemapTex", "scrollSpr", s => Utility.CreateSubSprite(s, TilemapSize, TilemapSize, 3, 6));
            ResourceManager.DeriveResource<Texture, Sprite>("tilemapTex", "treeSpr", s => Utility.CreateSubSprite(s, TilemapSize, TilemapSize, 0, 5, 2, 3));

            ResourceManager.LoadResource<Font>("assets/HelvetiPixel.ttf", "font");

            ResourceManager.LoadResource<Image>("assets/message.png", "messageImg");
            ResourceManager.DeriveResource<Image, Texture>("messageImg", "messageTex", s => new Texture(s));
            ResourceManager.DeriveResource<Texture, Sprite>("messageTex", "messageSpr", s => new Sprite(s));

            ResourceManager.LoadResource<Image>("assets/overworld.png", "overworldImg");

            overWorld = new OverWorld(this, ResourceManager.GetResource<Image>("overworldImg"));
            Add(overWorld);

            player = new Player(this, ResourceManager.GetResource<Sprite>("playerSpr"));
            Add(player, 1);

            inventory = new Inventory(this);
            Add(inventory, 2);

            inventory.Items.Add(new Bottle(this));
            inventory.Items.Add(new Bottle(this));
            inventory.Items.Add(new Bottle(this));
            inventory.Items.Add(new Scroll(this));
            inventory.Items.Add(new Scroll(this));

            player.Position = overWorld.SpawnPosition;

            return this;
        }

        public void Add(GameObject obj, int layer = 0)
        {
            if (!gameObjects.ContainsKey(layer))
                gameObjects.Add(layer, new List<GameObject>());

            gameObjects[layer].Add(obj);
        }

        public void Remove(GameObject obj)
        {
            foreach (var pair in gameObjects)
            {
                if (pair.Value.Contains(obj))
                {
                    pair.Value.Remove(obj);
                    return;
                }
            }

            throw new ArgumentException("Couldn't find the specified object");
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

            processUI();
        }

        bool lastMouseRightDown = false;
        bool lastMouseLeftDown = false;

        private void processUI()
        {
            var mousePos = MainWindow.MapPixelToCoords(Mouse.GetPosition(MainWindow), MainWindow.DefaultView);
            //var mousePosF = new Vector2f(mousePos.X, mousePos.Y);
            bool mouseRightDown = Mouse.IsButtonPressed(Mouse.Button.Right);
            bool mouseLeftDown = Mouse.IsButtonPressed(Mouse.Button.Left);

            // Just pressed right mouse button
            if (mouseRightDown && !lastMouseRightDown)
            {
                Item clickedItem = null;
                foreach (var item in inventory.Items)
                {
                    if (item.WorldRect.Contains(mousePos.X, mousePos.Y) && (mouseMenu == null || mouseMenu.HoverIndex == -1) && messageModal == null)
                    {
                        clickedItem = item;
                        break;
                    }
                }

                if (clickedItem != null)
                {
                    if (mouseMenu != null)
                    {
                        Remove(mouseMenu);
                    }

                    mouseMenu = new MouseMenu(game);
                    mouseMenu.Options = clickedItem.RightClickOptions;
                    mouseMenu.Position = mousePos;
                    Add(mouseMenu, 1000);
                }
            }

            if (mouseMenu != null)
            {
                mouseMenu.HoverIndex = -1;
                for (int i = 0; i < mouseMenu.Options.Count; i++)
                {
                    if (mouseMenu.GetRectForOption(i).Contains(mousePos.X, mousePos.Y))
                    {
                        mouseMenu.HoverIndex = i;

                        // Pressed left mouse button on option
                        if (mouseLeftDown && !lastMouseLeftDown)
                        {
                            if (mouseMenu.Options[mouseMenu.HoverIndex] == "Write a message")
                            {
                                var messageImg = ResourceManager.GetResource<Image>("messageImg");
                                messageModal = new MessageModal(this, ResourceManager.GetResource<Sprite>("messageSpr"), new Vector2f(messageImg.Size.X, messageImg.Size.Y) * Game.TilemapScale);
                                Add(messageModal, 1001);
                                player.Input = false;
                            }

                            Remove(mouseMenu);
                            mouseMenu = null;
                        }

                        break;
                    }
                }
            }

            if (messageModal != null)
            {
                if (mouseLeftDown && !lastMouseLeftDown)
                {
                    for (int i = 0; i < messageModal.MessageBox.LineCount; i++)
                    {
                        var rect = messageModal.MessageBox.GetRectForLine(i);

                        if (rect.Contains(mousePos.X, mousePos.Y))
                        {
                            messageModal.MessageBox.WriteIndex = i;
                            messageModal.CurrentTextbox = messageModal.MessageBox;
                            break;
                        }
                    }

                    for (int i = 0; i < messageModal.RegardsBox.LineCount; i++)
                    {
                        var rect = messageModal.RegardsBox.GetRectForLine(i);

                        if (rect.Contains(mousePos.X, mousePos.Y))
                        {
                            messageModal.RegardsBox.WriteIndex = i;
                            messageModal.CurrentTextbox = messageModal.RegardsBox;
                            break;
                        }
                    }

                    if (messageModal.SendButton.WorldRect.Contains(mousePos.X, mousePos.Y))
                    {

                    }
                }
            }

            lastMouseRightDown = mouseRightDown;
        }

        void MainWindow_TextEntered(object sender, TextEventArgs e)
        {
            if (messageModal != null && messageModal.CurrentTextbox != null && messageModal.CurrentTextbox.WriteIndex != -1)
            {
                const string allowedChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890.,-_:;!\"#¤%&/()=?+@£$€{[]}\'`´^* \b";

                if (!allowedChars.Contains(e.Unicode))
                    return;

                if (e.Unicode == "\b" && messageModal.CurrentTextbox.Lines[messageModal.CurrentTextbox.WriteIndex].Length > 0)
                {
                    messageModal.CurrentTextbox.Lines[messageModal.CurrentTextbox.WriteIndex] = messageModal.CurrentTextbox.Lines[messageModal.CurrentTextbox.WriteIndex].Remove(messageModal.CurrentTextbox.Lines[messageModal.CurrentTextbox.WriteIndex].Length - 1, 1);
                }
                else
                {
                    messageModal.CurrentTextbox.Lines[messageModal.CurrentTextbox.WriteIndex] += e.Unicode;
                }

                var text = messageModal.CurrentTextbox.GetTextForString(messageModal.CurrentTextbox.Lines[messageModal.CurrentTextbox.WriteIndex]);
                if (text.GetLocalBounds().Width > messageModal.CurrentTextbox.LineSize.X - messageModal.CurrentTextbox.Margin * 2f)
                {
                    messageModal.CurrentTextbox.Lines[messageModal.CurrentTextbox.WriteIndex] = messageModal.CurrentTextbox.Lines[messageModal.CurrentTextbox.WriteIndex].Remove(messageModal.CurrentTextbox.Lines[messageModal.CurrentTextbox.WriteIndex].Length - 1, 1);
                    if (messageModal.CurrentTextbox.WriteIndex != messageModal.CurrentTextbox.Lines.Length - 1)
                        messageModal.CurrentTextbox.WriteIndex++;
                }
            }
        }

        public override void Draw(RenderTarget target)
        {
            var topLeftView = target.MapPixelToCoords(new Vector2i());
            var botRightView = target.MapPixelToCoords(new Vector2i((int)MainWindow.Size.X - 1, (int)MainWindow.Size.Y - 1));
            var viewRect = new FloatRect(topLeftView.X, topLeftView.Y, botRightView.X - topLeftView.X, botRightView.Y - topLeftView.Y);

            foreach (var pair in gameObjects)
            {
                pair.Value.Sort(new Comparison<GameObject>((obj1, obj2) => (int)(obj1.Depth - obj2.Depth)));
                foreach (var obj in pair.Value)
                {
                    if (obj.VisibleRect != null && !viewRect.Intersects(obj.VisibleRect.Value))
                        continue;

                    obj.Draw(target);
                }
            }
        }
    }
}
