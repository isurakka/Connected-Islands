using SFML.Graphics;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD30
{
    class Game
    {
        static void Main(string[] args)
        {
            new Game().Run();
        }

        RenderWindow rw;

        public Game()
        {
            rw = new RenderWindow(new VideoMode(1024, 768), "LD30", Styles.Close, new ContextSettings() { AntialiasingLevel = 8 });
        }

        public void Run()
        {
            while (rw.IsOpen)
            {
                rw.DispatchEvents();

                rw.Clear();
                rw.Display();
            }
        }
    }
}
