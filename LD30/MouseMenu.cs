using SFML.Graphics;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace LD30
{
    class MouseMenu : GameObject
    {
        public bool Visible = true;
        public List<string> Options = new List<string>();
        public Vector2f Position;
        public Vector2f OptionSize = new Vector2f(200f, 30f);
        public float OptionMargin = 4f;
        public int HoverIndex = -1;

        public MouseMenu(Game game)
            : base(game)
        {

        }

        public FloatRect GetRectForOption(int index)
        {
            var left = Position.X + OptionMargin;
            var top = Position.Y - (Options.Count * OptionSize.Y + Options.Count * OptionMargin) + (index * OptionSize.Y + index * OptionMargin);
            return new FloatRect(left, top, OptionSize.X, OptionSize.Y);
        }

        public override void Draw(SFML.Graphics.RenderTarget target)
        {
            var view = new View(target.GetView());
            target.SetView(target.DefaultView);

            var bgRect = new RectangleShape();
            bgRect.FillColor = new Color(150, 113, 57);
            bgRect.Size = new Vector2f(OptionSize.X + 2f * OptionMargin, Options.Count * OptionSize.Y + Options.Count * OptionMargin + OptionMargin);
            bgRect.Position = new Vector2f(Position.X, Position.Y - bgRect.Size.Y);
            target.Draw(bgRect);

            float nextX = bgRect.Position.X + OptionMargin;
            float nextY = bgRect.Position.Y + OptionMargin;

            for (int i = 0; i < Options.Count; i++)
            {
                var fgRect = new RectangleShape();
                if (HoverIndex == i)
                {
                    fgRect.FillColor = new Color(175, 132, 66);
                }
                else
                {
                    fgRect.FillColor = new Color(201, 151, 76);
                }
                fgRect.Size = OptionSize;
                fgRect.Position = new Vector2f(nextX, nextY);
                target.Draw(fgRect);

                var text = new Text(Options[i], ResourceManager.GetResource<Font>("font"), 32u);
                text.DisplayedString = Options[i];
                text.Position = new Vector2f(nextX + OptionMargin, nextY - OptionSize.Y / 3f);
                target.Draw(text);

                nextY += OptionSize.Y + OptionMargin;
            }

            target.SetView(view);
        }
    }
}
