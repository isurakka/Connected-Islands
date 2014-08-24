using SFML.Graphics;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;


namespace LD30
{
    class Guestbook : GameObject
    {
        public List<GuestbookRow> Rows = new List<GuestbookRow>();
        public string Name;
        public bool LoadOnOpen = true;

        public static List<string> CantSign = new List<string>();

        Sprite sprite;
        public Vector2f Position
        {
            get
            {
                return sprite.Position;
            }
            set
            {
                sprite.Position = value;
            }
        }

        public override float Depth
        {
            get
            {
                return Position.Y + Game.TileSize;
            }
        }

        public FloatRect WorldRect
        {
            get
            {
                return new FloatRect(Position.X, Position.Y, Game.TileSize, Game.TileSize);
            }
        }

        public Guestbook(Game game, Sprite spr)
            : base(game)
        {
            this.sprite = spr;
            this.sprite.Scale = new Vector2f(Game.TilemapScale, Game.TilemapScale);
        }

        public void NetAddName(GuestbookRow row)
        {
            using (var client = new WebClient())
            {
                client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                var values = new NameValueCollection();
                values.Add("where", Name);
                values.Add("name", row.Name);
                var response = client.UploadValues("http://37.139.17.207/LD30_Server/index.php", "POST", values);
                Debug.WriteLine("Response: " + Encoding.UTF8.GetString(response));
            }

            Rows.Insert(0, row);
        }

        public void NetReceiveNames()
        {
            string response = null;
            using (var client = new WebClient())
            {
                client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                var values = new NameValueCollection();
                values.Add("GuestbookRequest", Game.GuestbookRequest);
                values.Add("where", Name);
                var raw = client.UploadValues("http://37.139.17.207/LD30_Server/index.php", "POST", values);
                response = Encoding.UTF8.GetString(raw);
                Debug.WriteLine("Response: " + response);
            }

            var split = response.Split(new string[] { Game.NetSeparator }, StringSplitOptions.None).ToList();
            while (split.Count() % 2 != 0)
                split.RemoveAt(split.Count - 1);
            for (int i = 0; i < split.Count; i+=2)
            {
                Rows.Add(new GuestbookRow() { Name = split[i], Time = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc).AddSeconds(double.Parse(split[i + 1])).ToLocalTime() });
            }
        }

        public override void Draw(RenderTarget target)
        {
            target.Draw(sprite);
        }
    }

    struct GuestbookRow
    {
        public string Name;
        public DateTime Time;
    }
}
