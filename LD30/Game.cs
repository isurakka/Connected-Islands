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
        public const string NetSeparator = "bJqjhyNsvA3wffogBGL5qpxoQ3mNemK7";
        public const string MessageRequest = "i5tbjtHCXa0fGCvZW98wrrWzAHPRRw88";

        SortedDictionary<int, List<GameObject>> gameObjects = new SortedDictionary<int, List<GameObject>>();
        OverWorld overWorld;
        Player player;
        Inventory inventory;
        MouseMenu mouseMenu;
        MessageModal messageModal;
        MessageLog messageLog;

        public Game()
            : base(null)
        {
            MainWindow = new RenderWindow(new VideoMode(1600, 900), "LD30", Styles.Close, new ContextSettings() { AntialiasingLevel = 8 });
            MainWindow.SetFramerateLimit(180u);
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
            ResourceManager.DeriveResource<Texture, Sprite>("tilemapTex", "scrollinbottleSpr", s => Utility.CreateSubSprite(s, TilemapSize, TilemapSize, 4, 6));
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
            inventory.Items.Add(new Scroll(this, new Message()));
            inventory.Items.Add(new Scroll(this, new Message()));

            messageLog = new MessageLog(this);
            messageLog.Position = new Vector2f(4f, MainWindow.Size.Y - 300f);
            Add(messageLog, 1001);

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

            refreshBottles();
            processUI();
        }

#if DEBUG
        const int maxNetBottles = 10;
#else
        const int maxNetBottles = 2;
#endif

        private void refreshBottles()
        {
            while (true)
            {
                var netBottles = gameObjects.Aggregate(new List<GameObject>(), (acc, pair) => { acc.AddRange(pair.Value); return acc; }).Where(obj => obj is ScrollInBottle && (obj as Item).Dropped && (obj as ScrollInBottle).Scroll.ReceiveOnOpen);
                if (netBottles.Count() >= maxNetBottles)
                    break;
                
                foreach (var bottlePos in overWorld.FindAllWorldPositionsForColor(new Color(0, 127, 100)))
                {
                    var topLeftView = MainWindow.MapPixelToCoords(new Vector2i());
                    var botRightView = MainWindow.MapPixelToCoords(new Vector2i((int)MainWindow.Size.X - 1, (int)MainWindow.Size.Y - 1));
                    var viewRect = new FloatRect(topLeftView.X, topLeftView.Y, botRightView.X - topLeftView.X, botRightView.Y - topLeftView.Y);

                    if (viewRect.Contains(bottlePos.X, bottlePos.Y))
                        continue;

                    if (netBottles.Any(obj =>
                    {
                        var bottle = obj as ScrollInBottle;
                        return (bottle.Position - bottlePos).Length() < 100;
                    }))
                    {
                        continue;
                    }

                    var newBottle = new ScrollInBottle(this);
                    newBottle.Bottle = new Bottle(this);
                    newBottle.Scroll = new Scroll(this, null);
                    newBottle.Scroll.ReceiveOnOpen = true;
                    newBottle.Dropped = true;
                    newBottle.Position = bottlePos;
                    Add(newBottle);
                }
            }
        }

        bool lastMouseRightDown = false;
        bool lastMouseLeftDown = false;
        Item clickedItem = null;
        Item combiningItem = null;

        private void processUI()
        {
            var mousePosLocal = MainWindow.MapPixelToCoords(Mouse.GetPosition(MainWindow), MainWindow.DefaultView);
            var mousePosWorld = MainWindow.MapPixelToCoords(Mouse.GetPosition(MainWindow));
            //var mousePosF = new Vector2f(mousePos.X, mousePos.Y);
            bool mouseRightDown = Mouse.IsButtonPressed(Mouse.Button.Right);
            bool mouseLeftDown = Mouse.IsButtonPressed(Mouse.Button.Left);

            // Just pressed right mouse button
            if (mouseRightDown && !lastMouseRightDown && messageModal == null)
            {
                bool openedMouseMenu = false;
                foreach (var item in inventory.Items)
                {
                    if (item.WorldRect.Contains(mousePosLocal.X, mousePosLocal.Y))
                    {
                        if ((mouseMenu != null && mouseMenu.HoverIndex != -1))
                            continue;

                        openedMouseMenu = true;
                        clickedItem = item;
                        break;
                    }
                }

                if (!openedMouseMenu)
                {
                    foreach (var item in gameObjects.Aggregate(new List<GameObject>(), (acc, pair) => { acc.AddRange(pair.Value); return acc; }).Where(obj => obj is Item && (obj as Item).Dropped))
	                {
                        var castedItem = item as Item;
                        if (castedItem.WorldRect.Contains(mousePosWorld.X, mousePosWorld.Y))
                        {
                            if (mouseMenu != null)
                            {
                                Remove(mouseMenu);
                            }

                            clickedItem = castedItem;

                            mouseMenu = new MouseMenu(game);
                            mouseMenu.Options = new List<string>() { "Pick up" };
                            mouseMenu.Position = mousePosLocal;
                            Add(mouseMenu, 1000);
                        }
	                }
                }

                if (openedMouseMenu && clickedItem != null)
                {
                    if (mouseMenu != null)
                    {
                        Remove(mouseMenu);
                    }

                    mouseMenu = new MouseMenu(game);
                    mouseMenu.Options = clickedItem.RightClickOptions;
                    mouseMenu.Position = mousePosLocal;
                    Add(mouseMenu, 1000);
                }
            }

            // Close mouse menu if too far away
            if (mouseMenu != null && clickedItem != null && clickedItem.Dropped)
            {
                if ((clickedItem.Position - player.Position).Length() > 100)
                {
                    Remove(mouseMenu);
                    mouseMenu = null;
                }
            }

            if (mouseMenu != null)
            {
                mouseMenu.HoverIndex = -1;
                for (int i = 0; i < mouseMenu.Options.Count; i++)
                {
                    if (mouseMenu.GetRectForOption(i).Contains(mousePosLocal.X, mousePosLocal.Y))
                    {
                        mouseMenu.HoverIndex = i;

                        // Pressed left mouse button on option
                        if (mouseLeftDown && !lastMouseLeftDown)
                        {
                            Scroll clickedScroll = clickedItem as Scroll;

                            if (clickedScroll != null && mouseMenu.Options[mouseMenu.HoverIndex] == "View")
                            {
                                var messageImg = ResourceManager.GetResource<Image>("messageImg");
                                messageModal = new MessageModal(this, ResourceManager.GetResource<Sprite>("messageSpr"), new Vector2f(messageImg.Size.X, messageImg.Size.Y) * Game.TilemapScale);
                                if (clickedScroll.ReceiveOnOpen)
                                {
                                    clickedScroll.Message = Message.ReceiveRandom();
                                    clickedScroll.ReceiveOnOpen = false;
                                    messageModal.TimeText.DisplayedString = clickedScroll.Message.Time.ToString();
                                }
                                else
                                {
                                    messageModal.TimeText.DisplayedString = DateTime.Now.ToString();
                                }
                                for (int j = 0; j < 3; j++)
                                {
                                    messageModal.MessageBox.Lines[j] = clickedScroll.Message.Text[j];
                                }
                                messageModal.RegardsBox.Lines[0] = clickedScroll.Message.Regards;
                                Add(messageModal, 1001);
                                player.Input = false;
                            }
                            else if (mouseMenu.Options[mouseMenu.HoverIndex] == "Drop")
                            {
                                clickedItem.Dropped = true;
                                inventory.Items.ForEach(item => item.Combining = false);
                                inventory.Items.Remove(clickedItem);
                                clickedItem.Position = player.Position;
                                Add(clickedItem);
                            }
                            else if (mouseMenu.Options[mouseMenu.HoverIndex] == "Combine")
                            {
                                if (combiningItem != null)
                                {
                                    if ((combiningItem is Bottle && clickedItem is Scroll) || (clickedItem is Bottle && combiningItem is Scroll))
                                    {
                                        Scroll scroll = (combiningItem as Scroll) ?? (clickedItem as Scroll);
                                        Bottle bottle = (combiningItem as Bottle) ?? (clickedItem as Bottle);

                                        var bottleIndex = inventory.Items.IndexOf(bottle);
                                        ScrollInBottle scrollInBottle = new ScrollInBottle(this);
                                        scrollInBottle.Scroll = scroll;
                                        scrollInBottle.Bottle = bottle;
                                        inventory.Items.Insert(bottleIndex, scrollInBottle);

                                        inventory.Items.Remove(scroll);
                                        inventory.Items.Remove(bottle);

                                        messageLog.AddMessage("Put scroll in bottle.");
                                    }
                                    else
                                    {
                                        messageLog.AddMessage("This does nothing.");
                                    }

                                    combiningItem = null;
                                }
                                else
                                {
                                    clickedItem.Combining = true;
                                    combiningItem = clickedItem;
                                }
                            }
                            else if (mouseMenu.Options[mouseMenu.HoverIndex] == "Separate")
                            {
                                ScrollInBottle scrollInBottle = clickedItem as ScrollInBottle;
                                if (scrollInBottle != null)
                                {
                                    var index = inventory.Items.IndexOf(scrollInBottle);
                                    inventory.Items.Insert(index, scrollInBottle.Scroll);
                                    inventory.Items.Insert(index, scrollInBottle.Bottle);
                                    inventory.Items.Remove(scrollInBottle);
                                    messageLog.AddMessage("Took the scroll out of the bottle.");
                                }
                            }
                            else if (mouseMenu.Options[mouseMenu.HoverIndex] == "Send bottle")
                            {
                                var color = overWorld.GetColorAtWorldPosition(player.WorldCenter);
                                if (Utility.ColorEquals(color, Color.Blue))
                                {
                                    var scrollInBottle = clickedItem as ScrollInBottle;
                                    scrollInBottle.Scroll.Message.NetSend();
                                    messageLog.AddMessage("Bottle sent.");
                                    inventory.Items.Remove(scrollInBottle);
                                }
                                else
                                {
                                    messageLog.AddMessage("You need to be in sea.");
                                }
                            }
                            else if (mouseMenu.Options[mouseMenu.HoverIndex] == "Pick up")
                            {
                                Remove(clickedItem);
                                clickedItem.Dropped = false;
                                inventory.Items.Add(clickedItem);
                            }

                            if (mouseMenu.Options[mouseMenu.HoverIndex] != "View")
                            {
                                clickedItem = null;
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

                        if (rect.Contains(mousePosLocal.X, mousePosLocal.Y))
                        {
                            messageModal.RegardsBox.WriteIndex = -1;
                            messageModal.MessageBox.WriteIndex = i;
                            messageModal.CurrentTextbox = messageModal.MessageBox;
                            break;
                        }
                    }

                    for (int i = 0; i < messageModal.RegardsBox.LineCount; i++)
                    {
                        var rect = messageModal.RegardsBox.GetRectForLine(i);

                        if (rect.Contains(mousePosLocal.X, mousePosLocal.Y))
                        {
                            messageModal.MessageBox.WriteIndex = -1;
                            messageModal.RegardsBox.WriteIndex = i;
                            messageModal.CurrentTextbox = messageModal.RegardsBox;
                            break;
                        }
                    }

                    // Pressed close on message modal
                    if (messageModal.CloseButton.WorldRect.Contains(mousePosLocal.X, mousePosLocal.Y))
                    {
                        Scroll clickedScroll = clickedItem as Scroll;
                        for (int j = 0; j < 3; j++)
                        {
                            clickedScroll.Message.Text[j] = messageModal.MessageBox.Lines[j];
                        }
                        clickedScroll.Message.Regards = messageModal.RegardsBox.Lines[0];
                        Remove(messageModal);
                        messageModal = null;
                        player.Input = true;
                        clickedItem = null;
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

                messageModal.TimeText.DisplayedString = DateTime.Now.ToString();

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
