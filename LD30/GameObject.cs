using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD30
{
    abstract class GameObject
    {
        protected Game game;

        public virtual float Depth
        {
            get
            {
                return 0f;
            }
        }

        public virtual FloatRect? VisibleRect
        {
            get
            {
                return null;
            }
        }

        private bool enabled = true;
        public virtual bool Enabled
        {
            get
            {
                return enabled;
            }
            set
            {
                enabled = value;
            }
        }

        public GameObject(Game game)
        {
            this.game = game;
        }

        public virtual void Update(float dt)
        {

        }

        public virtual void Draw(RenderTarget target)
        {

        }
    }
}
