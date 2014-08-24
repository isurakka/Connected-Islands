using SFML.Graphics;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace LD30
{
    class GuestbookModal : GameObject
    {
        Sprite bgSprite;
        Vector2f size;

        public Textbox SignBox;
        public Button SignButton;
        public Button CloseButton;
        public Button UpButton;
        public Button DownButton;

        public Guestbook Guestbook;
        public int NamesAtOnce = 6;
        public int NameIndex = 0;

        public GuestbookModal(Game game, Sprite bgSprite, Vector2f size)
            : base(game)
        {
            this.bgSprite = bgSprite;
            this.bgSprite.Scale = new Vector2f(Game.TilemapScale, Game.TilemapScale);
            this.size = size;

            SignBox = new Textbox(game);
            SignBox.LineCount = 1;
            SignBox.LineSize = new Vector2f(260f, 30f);
            SignBox.Lines[0] = "";

            SignButton = new Button(game);
            SignButton.Text = "Sign";

            CloseButton = new Button(game);
            CloseButton.Text = "Close";

            UpButton = new Button(game);
            UpButton.Text = "^";
            UpButton.Size = new Vector2f(20f, 40f);

            DownButton = new Button(game);
            DownButton.Text = "v";
            DownButton.Size = new Vector2f(20f, 40f);
        }

        public override void Update(float dt)
        {
            if (SignBox != null)
            {
                SignBox.Update(dt);
            }
        }

        public override void Draw(RenderTarget target)
        {
            var view = new View(target.GetView());
            target.SetView(target.DefaultView);

            bgSprite.Position = new Vector2f(game.MainWindow.Size.X / 2f, game.MainWindow.Size.Y / 2f) - new Vector2f(size.X / 2f, size.Y / 2f);
            target.Draw(bgSprite);

            if (Guestbook != null)
            {
                var titleText = new Text(Guestbook.Name, ResourceManager.GetResource<Font>("font"), 32u);
                titleText.Color = Color.Black;
                titleText.Position = bgSprite.Position + new Vector2f(65f, 40f);
                target.Draw(titleText);
            }

            UpButton.Position = bgSprite.Position + new Vector2f(65f + 460f, 85f);
            UpButton.Draw(target);

            DownButton.Position = bgSprite.Position + new Vector2f(65f + 460f, 40f + 180f);
            DownButton.Draw(target);

            var nextY = bgSprite.Position.Y + 70f;
            for (int i = NameIndex; i < NameIndex + NamesAtOnce; i++)
            {
                if (i < 0 || i > Guestbook.Rows.Count - 1)
                    continue;

                var nameText = new Text(Guestbook.Rows[i].Name, ResourceManager.GetResource<Font>("font"), 32u);
                nameText.Color = Color.Black;
                nameText.Position = new Vector2f(bgSprite.Position.X + 65f, nextY);
                var timeText = new Text(Guestbook.Rows[i].Time.ToString(), ResourceManager.GetResource<Font>("font"), 32u);
                timeText.Color = Color.Black;
                timeText.Position = nameText.Position + new Vector2f(270f, 0f);
                nextY += 28f;

                target.Draw(nameText);
                target.Draw(timeText);
            }

            if (SignBox != null)
            {
                SignBox.Position = bgSprite.Position + new Vector2f(65f, 70f) + new Vector2f(0f, 200f);
                SignBox.Draw(target);
            }

            if (SignBox != null)
            {
                SignButton.Position = SignBox.Position + new Vector2f(360f, 0f);
                SignButton.Draw(target);
            }

            CloseButton.Position = bgSprite.Position + new Vector2f(65f, 70f) + new Vector2f(0f, 200f) + new Vector2f(360f, 0f) + new Vector2f(0f, 34f);
            CloseButton.Draw(target);

            target.SetView(view);
        }
    }
}
