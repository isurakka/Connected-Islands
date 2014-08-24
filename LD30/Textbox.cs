using SFML.Graphics;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD30
{
    class Textbox : GameObject
    {
        public Vector2f Position;
        public float Margin = 4f;
        public Vector2f LineSize = new Vector2f(460f, 30f);
        private int lineCount;
        public int LineCount
        {
            get
            {
                return lineCount;
            }
            set
            {
                lineCount = value;
                Array.Resize(ref Lines, value);
            }
        }
        public string[] Lines;
        public int WriteIndex = -1;

        public Textbox(Game game)
            : base(game)
        {
            LineCount = 1;
        }

        public FloatRect GetRectForLine(int index)
        {
            return new FloatRect(Position.X, Position.Y + (index * LineSize.Y + index * Margin), LineSize.X, LineSize.Y);
        }

        public Text GetTextForString(string str)
        {
            var text = new Text(str, ResourceManager.GetResource<Font>("font"), 32u);
            text.Color = Color.Black;
            return text;
        }

        public override void Draw(SFML.Graphics.RenderTarget target)
        {
            var view = new View(target.GetView());
            target.SetView(target.DefaultView);

            float nextX = Position.X;
            float nextY = Position.Y;

            for (int i = 0; i < LineCount; i++)
            {
                var rect = new RectangleShape(LineSize);
                rect.FillColor = new Color(188, 171, 94);
                rect.Position = new Vector2f(nextX, nextY);
                target.Draw(rect);

                var text = GetTextForString(Lines[i]);
                text.Position = rect.Position + new Vector2f(Margin, -LineSize.Y / 3f);
                target.Draw(text);

                nextY += LineSize.Y + Margin;
            }

            target.SetView(view);
        }
    }
}
