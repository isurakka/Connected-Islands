﻿using SFML.Graphics;
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
                return new FloatRect(Position.X, Position.Y, Game.TileSize, Game.TileSize);
            }
        }

        public Item(Game game, Sprite spr)
            : base(game)
        {
            this.sprite = spr;
            sprite.Scale = new Vector2f(Game.TilemapScale, Game.TilemapScale);
        }

        public override void Draw(RenderTarget target)
        {
            target.Draw(sprite);
        }
    }
}
