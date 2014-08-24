using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LD30
{
    class Message
    {
        public int Id = -1;
        public string[] Text = new string[3];
        public string Regards = "Regards ";
        public DateTime Time = DateTime.Now;
        public long Views;

        public void NetSend()
        {
            using (var client = new WebClient())
            {
                client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                var values = new NameValueCollection();
                values.Add("id", Id.ToString());
                values.Add("message", Text[0] + "\n" + Text[1] + "\n" + Text[2]);
                values.Add("regards", Regards);
                var response = client.UploadValues("http://37.139.17.207/LD30_Server/index.php", "POST", values);
                Debug.WriteLine("Response: " + Encoding.UTF8.GetString(response));
            }
        }

        public static Message ReceiveRandom()
        {
            var msg = new Message();

            string response = null;
            using (var client = new WebClient())
            {
                client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                var values = new NameValueCollection();
                values.Add("MessageRequest", Game.MessageRequest);
                var raw = client.UploadValues("http://37.139.17.207/LD30_Server/index.php", "POST", values);
                response = Encoding.UTF8.GetString(raw);
                Debug.WriteLine("Response: " + response);
            }

            var split = response.Split(new string[] { Game.NetSeparator }, StringSplitOptions.None);
            msg.Id = int.Parse(split[0]);
            msg.Text = split[1].Split('\n');
            msg.Regards = split[2];
            msg.Time = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc).AddSeconds(double.Parse(split[3])).ToLocalTime();
            msg.Views = int.Parse(split[4]);

            return msg;
        }
    }
}
