using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD30
{
    class Scroll : Item
    {
        public Scroll(Game game)
            : base(game, new Sprite(ResourceManager.GetResource<Sprite>("scrollSpr")))
        {

        }
    }
}
