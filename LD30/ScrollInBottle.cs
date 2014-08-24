using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD30
{
    class ScrollInBottle : Item
    {
        public Scroll Scroll;
        public Bottle Bottle;
        
        public override List<string> RightClickOptions
        {
            get
            {
                var baseOptions = base.RightClickOptions;
                var myOptions = new List<string>() { "Send bottle", "Separate" };
                myOptions.AddRange(baseOptions);
                return myOptions;
            }
        }

        public ScrollInBottle(Game game)
            : base(game, new Sprite(ResourceManager.GetResource<Sprite>("scrollinbottleSpr")))
        {

        }
    }
}
