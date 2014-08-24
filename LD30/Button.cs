using SFML.Graphics;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD30
{
    class Button : GameObject
    {
        public Vector2f Position;
        public Vector2f Size = new Vector2f(100f, 30f);
        public string Text = "";

        public FloatRect WorldRect
        {
            get
            {
                return new FloatRect(Position.X, Position.Y, Size.X, Size.Y);
            }
        }

        public Button(Game game)
            : base(game)
        {

        }

        public override void Draw(RenderTarget target)
        {
            var view = new View(target.GetView());
            target.SetView(target.DefaultView);

            var rect = new RectangleShape(Size);
            rect.FillColor = new Color(165, 151, 81);
            rect.Position = Position;
            target.Draw(rect);

            var text = new Text(Text, ResourceManager.GetResource<Font>("font"), 32u);
            text.Color = Color.Black;
            text.Position = rect.Position + new Vector2f(rect.Size.X, 0f) * 0.5f - new Vector2f(text.GetLocalBounds().Width, text.GetLocalBounds().Height) * 0.5f;
            target.Draw(text);

            target.SetView(view);
        }
    }
}
