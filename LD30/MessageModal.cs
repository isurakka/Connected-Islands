using SFML.Graphics;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD30
{
    class MessageModal : GameObject
    {
        Sprite bgSprite;
        Vector2f size;
        public Textbox MessageBox;
        public Textbox RegardsBox;
        public Text TimeText;
        public Textbox CurrentTextbox = null;

        public Button CloseButton;
        public bool CanEdit = true;

        public MessageModal(Game game, Sprite bgSprite, Vector2f size)
            : base(game)
        {
            this.bgSprite = bgSprite;
            this.bgSprite.Scale = new Vector2f(Game.TilemapScale, Game.TilemapScale);
            this.size = size;

            MessageBox = new Textbox(game);
            MessageBox.LineCount = 3;

            RegardsBox = new Textbox(game);
            RegardsBox.LineCount = 1;
            RegardsBox.LineSize = new Vector2f(260f, 30f);
            RegardsBox.Lines[0] = "Regards ";

            TimeText = new Text("", ResourceManager.GetResource<Font>("font"), 28u);
            TimeText.Color = Color.Black;

            CloseButton = new Button(game);
            CloseButton.Text = "Close";
        }

        public override void Update(float dt)
        {
            MessageBox.Update(dt);
            RegardsBox.Update(dt);
        }

        public override void Draw(RenderTarget target)
        {
            var view = new View(target.GetView());
            target.SetView(target.DefaultView);

            bgSprite.Position = new Vector2f(game.MainWindow.Size.X / 2f, game.MainWindow.Size.Y / 2f) - new Vector2f(size.X / 2f, size.Y / 2f);
            target.Draw(bgSprite);

            MessageBox.Position = bgSprite.Position + new Vector2f(65f, 70f);
            MessageBox.Draw(target);

            RegardsBox.Position = MessageBox.Position + new Vector2f(0f, 200f);
            RegardsBox.Draw(target);

            TimeText.Position = RegardsBox.Position + new Vector2f(300f, -12f);
            target.Draw(TimeText);

            CloseButton.Position = RegardsBox.Position + new Vector2f(360f, 34f);
            CloseButton.Draw(target);

            target.SetView(view);
        }
    }
}
