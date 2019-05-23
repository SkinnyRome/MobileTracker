using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;
using WebSocketSharp.Server;
using System.IO;


namespace Server
{

    class ServerSocket : WebSocketBehavior
    {
        protected override void OnMessage(MessageEventArgs e)
        {
            Console.Write("\nHa llegado un mensaje\n");
            //FileInfo f = new FileInfo(e.Data);
            Console.Write("DATA: " + "\n");
            byte[] read = Tracker.Utilities.Instance.Decompress(e.RawData);
            File.WriteAllBytes("D:/USABILIDAD/Proyecto/MobileTracker/TrackerInfoCSV.csv", read);
            for (int i = 0; i < e.RawData.Length; i++)
            {
                Console.Write(e.RawData[i].ToString() + "\n");
            }
            //Console.Write(f.ToString());
            
        }
    }

    public class TrackerServer
    {

        public TrackerServer()
        {
            wssv = new WebSocketServer("ws://localhost:4649");
            wssv.AddWebSocketService<ServerSocket>("/server");
            _running = true;

        }

        public void Close() { wssv.Stop();
            Console.Write("Closing server");
        }
        public void Start() { wssv.Start();
            Console.Write("Opening server");
        }

        public bool Running() { return _running; }

        WebSocketServer wssv;
        private bool _running;



    }
}
