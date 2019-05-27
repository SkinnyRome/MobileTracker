using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;
using WebSocketSharp.Server;
using System.IO;
using UnityEngine;



namespace Server
{

    class ServerSocket : WebSocketBehavior
    {
        protected override void OnMessage(MessageEventArgs e)
        {
            //byte[] read = Tracker.Utilities.Instance.Decompress(e.RawData);
            File.WriteAllBytes(Application.persistentDataPath + "/TrackerInfoServer.data", e.RawData);
            string[] file = File.ReadAllLines(Application.persistentDataPath + "/TrackerInfoServer.data");
            for (int i = 0; i < file.Length; i++)
            {
                string a = Tracker.Utilities.Instance.BinaryToString(file[i]);
                File.AppendAllText(Application.persistentDataPath + "/TrackerInfoServer.csv", a + "\n");
            }
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
