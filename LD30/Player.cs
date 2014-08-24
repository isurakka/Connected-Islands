using SFML.Graphics;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;


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

        public float Speed = 800f;
        public float SpeedModifier = 1f;

        public bool Input = true;

        public Player(Game game, Sprite spr)
            : base(game)
        {
            this.sprite = spr;
            sprite.Scale = new Vector2f(Game.TilemapScale, Game.TilemapScale);
        }

        public override void Update(float dt)
        {
            if (!Input)
                return;

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
        }

        public override void Draw(RenderTarget target)
        {
            var oldPos = Position;
            Position = new Vector2f((float)Math.Round(Position.X), (float)Math.Round(Position.Y));

            var view = game.MainWindow.GetView();
            view.Center = Position + new Vector2f(Game.TileSize, Game.TileSize) * 0.5f;
            game.MainWindow.SetView(view);

            target.Draw(sprite);

            Position = oldPos;
        }
    }
}
