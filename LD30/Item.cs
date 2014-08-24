using SFML.Graphics;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD30
{
    class Item : GameObject
    {
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

        public virtual List<string> RightClickOptions
        {
            get { return new List<string>() { "Combine", "Drop" }; }
        }

        public FloatRect WorldRect
        {
            get
            {
                if (Dropped)
                {
                    return new FloatRect(Position.X, Position.Y, Game.TileSize * (2f / 3f), Game.TileSize * (2f / 3f));
                }
                else
                {
                    return new FloatRect(Position.X, Position.Y, Game.TileSize, Game.TileSize);
                }
            }
        }

        public bool Combining = false;
        public bool Dropped = false;

        public Item(Game game, Sprite spr)
            : base(game)
        {
            this.sprite = spr;
        }

        public override void Draw(RenderTarget target)
        {
            if (Dropped)
            {
                sprite.Scale = new Vector2f(Game.TilemapScale, Game.TilemapScale) * (2f / 3f);
            }
            else
            {
                sprite.Scale = new Vector2f(Game.TilemapScale, Game.TilemapScale);
            }

            target.Draw(sprite);
            
            if (Combining)
            {
                var rect = new RectangleShape(new Vector2f(Game.TileSize, Game.TileSize));
                rect.FillColor = Color.Transparent;
                rect.OutlineColor = Color.Red;
                rect.OutlineThickness = -4f;
                rect.Position = sprite.Position;
                target.Draw(rect);
            }
        }
    }
}
