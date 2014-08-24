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
        public override List<string> RightClickOptions
        {
            get 
            {
                var baseOptions = base.RightClickOptions;
                var myOptions = new List<string>() { "Write a message" };
                myOptions.AddRange(baseOptions);
                return myOptions;
            }
        }

        public Message Message;

        public Scroll(Game game, Message message)
            : base(game, new Sprite(ResourceManager.GetResource<Sprite>("scrollSpr")))
        {
            this.Message = message;
        }
    }
}
