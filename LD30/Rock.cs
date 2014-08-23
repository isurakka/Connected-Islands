using SFML.Graphics;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD30
{
    class Rock : GameObject
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

        public override float Depth
        {
            get
            {
                return Position.Y + Game.TileSize;
            }
        }

        public override FloatRect? VisibleRect
        {
            get
            {
                return new FloatRect(Position.X, Position.Y, Game.TileSize, Game.TileSize);
            }
        }

        public Rock(Game game, Sprite spr)
            : base(game)
        {
            this.sprite = spr;
        }

        public override void Draw(RenderTarget target)
        {
            target.Draw(sprite);
        }
    }
}
