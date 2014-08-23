using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD30
{
    class Bottle : Item
    {
        public Bottle(Game game)
            : base(game, new Sprite(ResourceManager.GetResource<Sprite>("bottleSpr")))
        {

        }
    }
}
