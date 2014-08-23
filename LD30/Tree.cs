using SFML.Graphics;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD30
{
    class Tree : GameObject
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
                sprite.Position = value + new Vector2f(0f, -Game.TileSize * 2);
            }
        }

        public override float Depth
        {
            get
            {
                return Position.Y + Game.TileSize * 3;
            }
        }

        public Tree(Game game, Sprite spr)
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
