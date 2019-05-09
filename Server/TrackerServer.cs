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
            Console.Write("Ha llegado un mensaje\n");
            //FileInfo f = new FileInfo(e.Data);
            Console.Write("DATA: " + e.RawData.ToString() + "\n");
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

        public bool Running() { return _running;
            
        }

        WebSocketServer wssv;
        private bool _running;



    }
}
