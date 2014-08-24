using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace LD30
{
    class Scroll : Item
    {
        public override List<string> RightClickOptions
        {
            get 
            {
                var baseOptions = base.RightClickOptions;
                var myOptions = new List<string>() { "View" };
                myOptions.AddRange(baseOptions);
                return myOptions;
            }
        }

        public Message Message;
        public bool ReceiveOnOpen = false;

        public Scroll(Game game, Message message)
            : base(game, new Sprite(ResourceManager.GetResource<Sprite>("scrollSpr")))
        {
            this.Message = message;
        }
    }
}
