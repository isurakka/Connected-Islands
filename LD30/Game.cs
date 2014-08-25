using SFML.Graphics;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;


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
        public const string GuestbookRequest = "CQrxp2zpbWUNRDYaLeoLOpQait2rHk2N";

        SortedDictionary<int, List<GameObject>> gameObjects = new SortedDictionary<int, List<GameObject>>();
        OverWorld overWorld;
        Cave cave;
        World currentWorld;
        Player player;
        Inventory inventory;
        MouseMenu mouseMenu;
        MessageModal messageModal;
        MessageLog messageLog;
        StartModal startModal;

        Guestbook MainIslandGuestbook;
        Guestbook EastIslandGuestbook;
        Guestbook WestIslandGuestbook;
        Guestbook SouthIslandGuestbook;
        Guestbook NorthIslandGuestbook;
        Guestbook CaveEntranceGuestbook;
        Guestbook CaveLDGuestbook;
        Guestbook CaveBigRoomGuestbook;
        GuestbookModal guestbookModal;

        public Random Random = new Random();

        public Game()
            : base(null)
        {
            MainWindow = new RenderWindow(new VideoMode(1024, 768), "Connected Islands", Styles.Default, new ContextSettings() { AntialiasingLevel = 8 });
            //MainWindow.SetFramerateLimit(180u);
            MainWindow.Closed += (s, a) =>
            {
                MainWindow.Close();
            };
            MainWindow.Resized += (s, a) =>
            {
                var view = MainWindow.GetView();
                //view.Size = new Vector2f(MainWindow.Size.X, MainWindow.Size.Y);
                view.Reset(new FloatRect(0f, 0f, MainWindow.Size.X, MainWindow.Size.Y));
                //MainWindow.DefaultView.Size = view.Size;
                MainWindow.DefaultView.Reset(new FloatRect(0f, 0f, MainWindow.Size.X, MainWindow.Size.Y));
                //MainWindow.DefaultView
                MainWindow.SetView(view);
            };
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
            ResourceManager.DeriveResource<Texture, Sprite>("tilemapTex", "guestbookSpr", s => Utility.CreateSubSprite(s, TilemapSize, TilemapSize, 3, 4));
            ResourceManager.DeriveResource<Texture, Sprite>("tilemapTex", "scrollinbottleSpr", s => Utility.CreateSubSprite(s, TilemapSize, TilemapSize, 4, 6));
            ResourceManager.DeriveResource<Texture, Sprite>("tilemapTex", "treeSpr", s => Utility.CreateSubSprite(s, TilemapSize, TilemapSize, 0, 5, 2, 3));

            ResourceManager.LoadResource<Font>("assets/HelvetiPixel.ttf", "font");

            ResourceManager.LoadResource<Image>("assets/message.png", "messageImg");
            ResourceManager.DeriveResource<Image, Texture>("messageImg", "messageTex", s => new Texture(s));
            ResourceManager.DeriveResource<Texture, Sprite>("messageTex", "messageSpr", s => new Sprite(s));

            ResourceManager.LoadResource<Image>("assets/overworld.png", "overworldImg");
            ResourceManager.LoadResource<Image>("assets/cave.png", "caveImg");

            overWorld = new OverWorld(this, ResourceManager.GetResource<Image>("overworldImg"));
            Add(overWorld);

            var mainIslandGuestbookWorldPos = overWorld.FindFirstWorldPositionForColor(new Color(127, 0, 255));
            var mainIslandGuestbookLocalPos = overWorld.FindAllLocalPositionsForColor(new Color(127, 0, 255))[0];
            overWorld.Collisions[mainIslandGuestbookLocalPos.X, mainIslandGuestbookLocalPos.Y] = true;
            MainIslandGuestbook = new Guestbook(this, new Sprite(ResourceManager.GetResource<Sprite>("guestbookSpr")));
            MainIslandGuestbook.Position = mainIslandGuestbookWorldPos;
            MainIslandGuestbook.MyWorld = overWorld;
            MainIslandGuestbook.Name = "Center island guestbook";
            Add(MainIslandGuestbook);

            var eastIslandGuestbookWorldPos = overWorld.FindFirstWorldPositionForColor(new Color(128, 0, 255));
            var eastIslandGuestbookLocalPos = overWorld.FindAllLocalPositionsForColor(new Color(128, 0, 255))[0];
            overWorld.Collisions[eastIslandGuestbookLocalPos.X, eastIslandGuestbookLocalPos.Y] = true;
            EastIslandGuestbook = new Guestbook(this, new Sprite(ResourceManager.GetResource<Sprite>("guestbookSpr")));
            EastIslandGuestbook.Position = eastIslandGuestbookWorldPos;
            EastIslandGuestbook.MyWorld = overWorld;
            EastIslandGuestbook.Name = "East island guestbook";
            Add(EastIslandGuestbook);

            var westIslandGuestbookWorldPos = overWorld.FindFirstWorldPositionForColor(new Color(129, 0, 255));
            var westIslandGuestbookLocalPos = overWorld.FindAllLocalPositionsForColor(new Color(129, 0, 255))[0];
            overWorld.Collisions[westIslandGuestbookLocalPos.X, westIslandGuestbookLocalPos.Y] = true;
            WestIslandGuestbook = new Guestbook(this, new Sprite(ResourceManager.GetResource<Sprite>("guestbookSpr")));
            WestIslandGuestbook.Position = westIslandGuestbookWorldPos;
            WestIslandGuestbook.MyWorld = overWorld;
            WestIslandGuestbook.Name = "West island guestbook";
            Add(WestIslandGuestbook);

            var southIslandGuestbookWorldPos = overWorld.FindFirstWorldPositionForColor(new Color(130, 0, 255));
            var southIslandGuestbookLocalPos = overWorld.FindAllLocalPositionsForColor(new Color(130, 0, 255))[0];
            overWorld.Collisions[southIslandGuestbookLocalPos.X, southIslandGuestbookLocalPos.Y] = true;
            SouthIslandGuestbook = new Guestbook(this, new Sprite(ResourceManager.GetResource<Sprite>("guestbookSpr")));
            SouthIslandGuestbook.Position = southIslandGuestbookWorldPos;
            SouthIslandGuestbook.MyWorld = overWorld;
            SouthIslandGuestbook.Name = "South island guestbook";
            Add(SouthIslandGuestbook);

            var northIslandGuestbookWorldPos = overWorld.FindFirstWorldPositionForColor(new Color(131, 0, 255));
            var northIslandGuestbookLocalPos = overWorld.FindAllLocalPositionsForColor(new Color(131, 0, 255))[0];
            overWorld.Collisions[northIslandGuestbookLocalPos.X, northIslandGuestbookLocalPos.Y] = true;
            NorthIslandGuestbook = new Guestbook(this, new Sprite(ResourceManager.GetResource<Sprite>("guestbookSpr")));
            NorthIslandGuestbook.Position = northIslandGuestbookWorldPos;
            NorthIslandGuestbook.MyWorld = overWorld;
            NorthIslandGuestbook.Name = "North island guestbook";
            Add(NorthIslandGuestbook);

            cave = new Cave(this, ResourceManager.GetResource<Image>("caveImg"));
            cave.Enabled = false;
            Add(cave);

            var caveEntranceGuestbookWorldPos = cave.FindFirstWorldPositionForColor(new Color(132, 0, 255));
            var caveEntranceGuestbookLocalPos = cave.FindAllLocalPositionsForColor(new Color(132, 0, 255))[0];
            cave.Collisions[caveEntranceGuestbookLocalPos.X, caveEntranceGuestbookLocalPos.Y] = true;
            CaveEntranceGuestbook = new Guestbook(this, new Sprite(ResourceManager.GetResource<Sprite>("guestbookSpr")));
            CaveEntranceGuestbook.Position = caveEntranceGuestbookWorldPos;
            CaveEntranceGuestbook.MyWorld = cave;
            CaveEntranceGuestbook.Name = "Cave entrance guestbook";
            Add(CaveEntranceGuestbook);

            var caveLDGuestbookWorldPos = cave.FindFirstWorldPositionForColor(new Color(133, 0, 255));
            var caveLDGuestbookLocalPos = cave.FindAllLocalPositionsForColor(new Color(133, 0, 255))[0];
            cave.Collisions[caveLDGuestbookLocalPos.X, caveLDGuestbookLocalPos.Y] = true;
            CaveLDGuestbook = new Guestbook(this, new Sprite(ResourceManager.GetResource<Sprite>("guestbookSpr")));
            CaveLDGuestbook.Position = caveLDGuestbookWorldPos;
            CaveLDGuestbook.MyWorld = cave;
            CaveLDGuestbook.Name = "Cave LD30 guestbook";
            Add(CaveLDGuestbook);

            var caveBigRoomGuestbookWorldPos = cave.FindFirstWorldPositionForColor(new Color(134, 0, 255));
            var caveBigRoomGuestbookLocalPos = cave.FindAllLocalPositionsForColor(new Color(134, 0, 255))[0];
            cave.Collisions[caveBigRoomGuestbookLocalPos.X, caveBigRoomGuestbookLocalPos.Y] = true;
            CaveBigRoomGuestbook = new Guestbook(this, new Sprite(ResourceManager.GetResource<Sprite>("guestbookSpr")));
            CaveBigRoomGuestbook.Position = caveBigRoomGuestbookWorldPos;
            CaveBigRoomGuestbook.MyWorld = cave;
            CaveBigRoomGuestbook.Name = "Cave big room guestbook";
            Add(CaveBigRoomGuestbook);

            currentWorld = overWorld;

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

            var messageImg = ResourceManager.GetResource<Image>("messageImg");
            startModal = new StartModal(this, ResourceManager.GetResource<Sprite>("messageSpr"), new Vector2f(messageImg.Size.X, messageImg.Size.Y) * Game.TilemapScale);
            Add(startModal, 1000);
            player.Input = false;

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
                    if (obj.MyWorld != null && obj.MyWorld != currentWorld)
                        continue;

                    obj.Update(dt);
                }
            }

            var newPlayerPos = player.Position;
            var playerDir = newPlayerPos - oldPlayerPos;

            var playerLocalPos = new Vector2i(
                (int)Math.Floor(player.WorldCenter.X * (1f / Game.TileSize)),
                (int)Math.Floor(player.WorldCenter.Y * (1f / Game.TileSize)));

            var upLocal = playerLocalPos + new Vector2i(0, -1);
            var upRect = currentWorld.GetWorldFloatRectForTile(playerLocalPos + new Vector2i(0, -1));
            var downLocal = playerLocalPos + new Vector2i(0, 1);
            var downRect = currentWorld.GetWorldFloatRectForTile(playerLocalPos + new Vector2i(0, 1));

            var leftLocal = playerLocalPos + new Vector2i(-1, 0);
            var leftRect = currentWorld.GetWorldFloatRectForTile(playerLocalPos + new Vector2i(-1, 0));
            var rightLocal = playerLocalPos + new Vector2i(1, 0);
            var rightRect = currentWorld.GetWorldFloatRectForTile(playerLocalPos + new Vector2i(1, 0));

            var playerRect = new FloatRect(player.Position.X, player.Position.Y, Game.TileSize, Game.TileSize);
            if (currentWorld.Collisions[upLocal.X, upLocal.Y] && playerRect.Intersects(upRect))
                player.Position += new Vector2f(0f, -(player.Position.Y - (upRect.Top + upRect.Height)));
            if (currentWorld.Collisions[downLocal.X, downLocal.Y] && playerRect.Intersects(downRect))
                player.Position += new Vector2f(0f, -(player.Position.Y + Game.TileSize - downRect.Top));
            if (currentWorld.Collisions[leftLocal.X, leftLocal.Y] && playerRect.Intersects(leftRect))
                player.Position += new Vector2f(-(player.Position.X - (leftRect.Left + leftRect.Width)), 0f);
            if (currentWorld.Collisions[rightLocal.X, rightLocal.Y] && playerRect.Intersects(rightRect))
                player.Position += new Vector2f(-(player.Position.X + Game.TileSize - rightRect.Left), 0f);

            var color = currentWorld.GetColorAtWorldPosition(player.WorldCenter);
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

            processPortals();
            refreshBottles();
            processUI();
        }

        private void processPortals()
        {
            World otherWorld = null;
            if (overWorld.Enabled)
            {
                otherWorld = cave;
            }
            else
            {
                otherWorld = overWorld;
            }

            var portalPos = currentWorld.PositionCache["CaveEntrance"];

            if ((player.Position - portalPos).Length() < Game.TileSize / 4f)
            {
                currentWorld.Enabled = false;
                otherWorld.Enabled = true;
                currentWorld = otherWorld;
                player.Position = otherWorld.PositionCache["CaveEntrance"] + new Vector2f(0f, Game.TileSize);
            }
        }

#if DEBUG
        const int maxNetBottles = 12;
#else
        const int maxNetBottles = 4;
#endif

        private void refreshBottles()
        {
            List<Vector2f> positions = null;

            while (true)
            {
                var netBottles = gameObjects.Aggregate(new List<GameObject>(), (acc, pair) => { acc.AddRange(pair.Value); return acc; }).Where(obj => obj is ScrollInBottle && (obj as Item).Dropped && (obj as ScrollInBottle).Scroll.ReceiveOnOpen);
                if (netBottles.Count() >= maxNetBottles)
                    break;

                if (positions == null)
                {
                    positions = currentWorld.FindAllWorldPositionsForColor(new Color(0, 127, 100));
                }

                var index = Random.Next(0, positions.Count);
                var bottlePos = positions[index];

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

        bool lastMouseRightDown = false;
        bool lastMouseLeftDown = false;
        Item clickedItem = null;
        Item combiningItem = null;
        Guestbook openingGuestbook = null;
        bool pressedButtonThisFrame = false;

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
                            openedMouseMenu = true;

                            mouseMenu = new MouseMenu(game);
                            mouseMenu.Options = new List<string>() { "Pick up" };
                            mouseMenu.Position = mousePosLocal;
                            Add(mouseMenu, 1000);
                        }
	                }
                }

                if (!openedMouseMenu)
                {
                    foreach (var item in gameObjects.Aggregate(new List<GameObject>(), (acc, pair) => { acc.AddRange(pair.Value); return acc; }).Where(obj => obj is Guestbook))
                    {
                        var guestbook = item as Guestbook;
                        if (guestbook.WorldRect.Contains(mousePosWorld.X, mousePosWorld.Y))
                        {
                            if (mouseMenu != null)
                            {
                                Remove(mouseMenu);
                            }

                            openedMouseMenu = true;
                            openingGuestbook = guestbook;

                            mouseMenu = new MouseMenu(game);
                            mouseMenu.Options = new List<string>() { "Open guestbook" };
                            mouseMenu.Position = mousePosLocal;
                            Add(mouseMenu, 1000);
                        }
                    }
                }

                if (openedMouseMenu && clickedItem != null && mouseMenu == null)
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
                            else if (mouseMenu.Options[mouseMenu.HoverIndex] == "Open guestbook")
                            {
                                var messageImg = ResourceManager.GetResource<Image>("messageImg");
                                guestbookModal = new GuestbookModal(this, ResourceManager.GetResource<Sprite>("messageSpr"), new Vector2f(messageImg.Size.X, messageImg.Size.Y) * Game.TilemapScale);
                                if (openingGuestbook.LoadOnOpen)
                                {
                                    openingGuestbook.NetReceiveNames();
                                    openingGuestbook.LoadOnOpen = false;
                                }
                                guestbookModal.Guestbook = openingGuestbook;
                                if (Guestbook.CantSign.Contains(openingGuestbook.Name))
                                {
                                    guestbookModal.SignBox = null;
                                    guestbookModal.SignButton = null;
                                }
                                Add(guestbookModal, 1001);
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
                                var color = currentWorld.GetColorAtWorldPosition(player.WorldCenter);
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

                // Pressed mouse outside an option
                if (mouseMenu != null && mouseLeftDown && !lastMouseLeftDown)
                {
                    Remove(mouseMenu);
                    mouseMenu = null;
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

            if (!mouseLeftDown && lastMouseLeftDown)
                pressedButtonThisFrame = false;

            if (guestbookModal != null)
            {
                if (mouseLeftDown && !lastMouseLeftDown)
                {
                    if (guestbookModal.SignBox != null)
                    {
                        for (int i = 0; i < guestbookModal.SignBox.LineCount; i++)
                        {
                            var rect = guestbookModal.SignBox.GetRectForLine(i);

                            if (rect.Contains(mousePosLocal.X, mousePosLocal.Y))
                            {
                                guestbookModal.SignBox.WriteIndex = i;
                                break;
                            }
                        }
                    }

                    // Pressed up on guestbook modal
                    if (guestbookModal.UpButton != null && !pressedButtonThisFrame && guestbookModal.UpButton.WorldRect.Contains(mousePosLocal.X, mousePosLocal.Y))
                    {
                        if (guestbookModal.NameIndex > 0)
                        {
                            guestbookModal.NameIndex--;
                            pressedButtonThisFrame = true;
                        }
                    }

                    // Pressed up on guestbook modal
                    if (guestbookModal.DownButton != null && !pressedButtonThisFrame && guestbookModal.DownButton.WorldRect.Contains(mousePosLocal.X, mousePosLocal.Y))
                    {
                        if (guestbookModal.NameIndex + 1 + guestbookModal.NamesAtOnce <= guestbookModal.Guestbook.Rows.Count)
                        {
                            guestbookModal.NameIndex++;
                            pressedButtonThisFrame = true;
                        }
                    }

                    // Pressed sign on guestbook modal
                    if (guestbookModal.SignButton != null && guestbookModal.SignButton.WorldRect.Contains(mousePosLocal.X, mousePosLocal.Y))
                    {
                        guestbookModal.Guestbook.NetAddName(new GuestbookRow() { Name = guestbookModal.SignBox.Lines[0], Time = DateTime.Now });
                        Guestbook.CantSign.Add(guestbookModal.Guestbook.Name);
                        guestbookModal.SignButton = null;
                        guestbookModal.SignBox = null;
                        guestbookModal.NameIndex = 0;
                    }

                    // Pressed close on guestbook modal
                    if (guestbookModal.CloseButton.WorldRect.Contains(mousePosLocal.X, mousePosLocal.Y))
                    {
                        Remove(guestbookModal);
                        guestbookModal = null;
                        player.Input = true;
                    }
                }
            }

            if (startModal != null)
            {
                if (mouseLeftDown && !lastMouseLeftDown)
                {
                    // Pressed close on guestbook modal
                    if (startModal.CloseButton.WorldRect.Contains(mousePosLocal.X, mousePosLocal.Y))
                    {
                        Remove(startModal);
                        startModal = null;
                        player.Input = true;
                    }
                }
            }

            lastMouseRightDown = mouseRightDown;
            lastMouseLeftDown = mouseLeftDown;
        }

        void MainWindow_TextEntered(object sender, TextEventArgs e)
        {
            if (messageModal != null && messageModal.CurrentTextbox != null && messageModal.CurrentTextbox.WriteIndex != -1)
            {
                const string allowedChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890.,-_:;!\"#¤%&/()=?+@£$€{[]}'`´^* \b";

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

            if (guestbookModal != null && guestbookModal.SignBox != null && guestbookModal.SignBox.WriteIndex != -1)
            {
                const string allowedChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890.,-_:;!\"#¤%&/()=?+@£$€{[]}'`´^* \b";

                if (!allowedChars.Contains(e.Unicode))
                    return;

                if (e.Unicode == "\b" && guestbookModal.SignBox.Lines[guestbookModal.SignBox.WriteIndex].Length > 0)
                {
                    guestbookModal.SignBox.Lines[guestbookModal.SignBox.WriteIndex] = guestbookModal.SignBox.Lines[guestbookModal.SignBox.WriteIndex].Remove(guestbookModal.SignBox.Lines[guestbookModal.SignBox.WriteIndex].Length - 1, 1);
                }
                else
                {
                    guestbookModal.SignBox.Lines[guestbookModal.SignBox.WriteIndex] += e.Unicode;
                }

                var text = guestbookModal.SignBox.GetTextForString(guestbookModal.SignBox.Lines[guestbookModal.SignBox.WriteIndex]);
                if (text.GetLocalBounds().Width > guestbookModal.SignBox.LineSize.X - guestbookModal.SignBox.Margin * 2f)
                {
                    guestbookModal.SignBox.Lines[guestbookModal.SignBox.WriteIndex] = guestbookModal.SignBox.Lines[guestbookModal.SignBox.WriteIndex].Remove(guestbookModal.SignBox.Lines[guestbookModal.SignBox.WriteIndex].Length - 1, 1);
                    if (guestbookModal.SignBox.WriteIndex != guestbookModal.SignBox.Lines.Length - 1)
                        guestbookModal.SignBox.WriteIndex++;
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
                var visible = pair.Value.Where(obj => obj.VisibleRect == null || viewRect.Intersects(obj.VisibleRect.Value)).ToList();

                visible.Sort(new Comparison<GameObject>((obj1, obj2) => (int)(obj1.Depth - obj2.Depth)));
                foreach (var obj in visible)
                {
                    if (obj.MyWorld != null && obj.MyWorld != currentWorld)
                        continue;

                    obj.Draw(target);
                }
            }
        }
    }
}
