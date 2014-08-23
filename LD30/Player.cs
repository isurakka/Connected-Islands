using SFML.Graphics;
using SFML.Window;
using System;
using System.Collections.Generic;
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

        public float Speed = 1000f;

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

            dir = dir.Normalize() * Speed * dt;
            Position += dir;
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
