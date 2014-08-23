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
        private Vector2f position;
        public Vector2f Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
            }
        }

        public float Speed = 1000f;

        public Player(Game game)
            : base(game)
        {

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

            var view = game.MainWindow.GetView();
            view.Center = Position;
            game.MainWindow.SetView(view);
        }
    }
}
