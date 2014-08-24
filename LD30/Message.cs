using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD30
{
    class Message
    {
        public int Id = -1;
        public string[] Text = new string[3];
        public string Regards = "Regards ";
        public DateTime Time;
        public long Views;
    }
}
