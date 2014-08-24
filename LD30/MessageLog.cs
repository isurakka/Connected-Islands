using SFML.Graphics;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace LD30
{
    class MessageLog : GameObject
    {
        public Vector2f Position;
        public int MessageLimit = 6;
        public float MessageLifetime = 5f;
        public int MessageSize = 32;
        List<LogMessage> Messages = new List<LogMessage>();

        public MessageLog(Game game)
            : base(game)
        {

        }

        public void AddMessage(string msg)
        {
            Messages.Insert(0, new LogMessage() { Message = msg });
            while (Messages.Count > MessageLimit)
                Messages.RemoveAt(Messages.Count - 1);
        }

        public override void Update(float dt)
        {
            foreach (var msg in Messages)
            {
                msg.TimeAlive += dt;
            }

            Messages.RemoveAll(msg => msg.TimeAlive > MessageLifetime);
        }

        public override void Draw(SFML.Graphics.RenderTarget target)
        {
            var view = new View(target.GetView());
            target.SetView(target.DefaultView);

            float nextY = Position.Y;
            foreach (var msg in Messages)
            {
                var text = new Text(msg.Message, ResourceManager.GetResource<Font>("font"), (uint)MessageSize);
                text.Color = new Color(0, 0, 0, (byte)(int)((MessageLifetime - msg.TimeAlive) / MessageLifetime * 255f));
                text.Position = new Vector2f(Position.X, nextY);
                nextY += MessageSize;
                target.Draw(text);
            }

            target.SetView(view);
        }
    }

    class LogMessage
    {
        public string Message = "";
        public float TimeAlive = 0f;
    }
}
