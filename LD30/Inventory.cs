using SFML.Graphics;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD30
{
    class Inventory : GameObject
    {
        public bool Visible = true;
        public List<Item> Items = new List<Item>();

        public Inventory(Game game)
            : base(game)
        {

        }

        public override void Draw(SFML.Graphics.RenderTarget target)
        {
            if (!Visible)
                return;

            var view = new View(target.GetView());
            target.SetView(target.DefaultView);

            const float inventoryHeight = 100f;
            var inventoryShape = new RectangleShape(new Vector2f(target.Size.X, inventoryHeight));
            inventoryShape.FillColor = new Color(0, 0, 0, 160);
            inventoryShape.Position = new Vector2f(0f, target.Size.Y - inventoryHeight);
            target.Draw(inventoryShape);

            const float itemMargin = 10f;

            float nextY = target.Size.Y - inventoryHeight + inventoryHeight / 2f - Game.TileSize / 2f;
            float nextX = itemMargin;

            foreach (var item in Items)
            {
                item.Position = new Vector2f(nextX, nextY);
                nextX += Game.TileSize + itemMargin;
                item.Draw(target);
            }

            target.SetView(view);
        }
    }
}
