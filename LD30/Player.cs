using SFML.Graphics;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD30
{
    class Player : GameObject
    {
        Sprite sprite;

        private Vector2f position;
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

        public Vector2f WorldCenter
        {
            get
            {
                return Position + new Vector2f(Game.TileSize / 2f, Game.TileSize / 2f);
            }
        }

        bool halfVertical = false;
        public bool HalfVertical
        {
            get
            {
                return halfVertical;
            }
            set
            {
                if (halfVertical == value)
                    return;

                var rect = sprite.TextureRect;
                if (value)
                {
                    rect.Height = Game.TilemapSize / 2;
                }
                else
                {
                    rect.Height = Game.TilemapSize;
                }
                sprite.TextureRect = rect;

                halfVertical = value;
            }
        }

        public override float Depth
        {
            get
            {
                return Position.Y + Game.TileSize;
            }
        }

        public float Speed = 1000f;
        public float SpeedModifier = 1f;

        public Player(Game game, Sprite spr)
            : base(game)
        {
            this.sprite = spr;
            sprite.Scale = new Vector2f(Game.TilemapScale, Game.TilemapScale);
        }

        public override void Update(float dt)
        {
            var dir = new Vector2f();
            if (Keyboard.IsKeyPressed(Keyboard.Key.W))
                dir.Y -= 1f;
            if (Keyboard.IsKeyPressed(Keyboard.Key.S))
                dir.Y += 1f;
            if (Keyboard.IsKeyPressed(Keyboard.Key.A))
                dir.X -= 1f;
            if (Keyboard.IsKeyPressed(Keyboard.Key.D))
                dir.X += 1f;

            dir = dir.Normalize() * Speed * SpeedModifier * dt;
            Position += dir;

            Debug.WriteLine(Position);
        }

        public override void Draw(RenderTarget target)
        {
            var oldPos = Position;
            Position = new Vector2f((float)Math.Round(Position.X), (float)Math.Round(Position.Y));

            var view = game.MainWindow.GetView();
            view.Center = Position;
            game.MainWindow.SetView(view);

            target.Draw(sprite);

            Position = oldPos;
        }
    }
}
