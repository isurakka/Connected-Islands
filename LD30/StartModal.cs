using SFML.Graphics;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace LD30
{
    class StartModal : GameObject
    {
        Sprite bgSprite;
        Vector2f size;

        public Button CloseButton;

        public StartModal(Game game, Sprite bgSprite, Vector2f size)
            : base(game)
        {
            this.bgSprite = bgSprite;
            this.bgSprite.Scale = new Vector2f(Game.TilemapScale, Game.TilemapScale);
            this.size = size;

            CloseButton = new Button(game);
            CloseButton.Text = "Close";
        }

        public override void Draw(RenderTarget target)
        {
            var view = new View(target.GetView());
            target.SetView(target.DefaultView);

            bgSprite.Position = new Vector2f(game.MainWindow.Size.X / 2f, game.MainWindow.Size.Y / 2f) - new Vector2f(size.X / 2f, size.Y / 2f);
            target.Draw(bgSprite);

            var text1 = new Text("You are on a stranded island but you don't\nremember how you got here.\n\nYou can send messages to other people\nusing bottles and scrolls. There are also\n7 guestbooks you can sign.\n\nWASD to move. Right click to open context menu\non items. Left click to choose option.", ResourceManager.GetResource<Font>("font"), 28u);
            text1.Color = Color.Black;
            text1.Position = bgSprite.Position + new Vector2f(60f, 60f);
            target.Draw(text1);

            CloseButton.Position = bgSprite.Position + size * 0.5f - new Vector2f(CloseButton.Size.X, CloseButton.Size.Y) * 0.5f + new Vector2f(0f, 130f);
            CloseButton.Draw(target);

            target.SetView(view);
        }
    }
}
