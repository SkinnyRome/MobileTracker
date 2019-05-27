using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {

            TrackerServer server = new TrackerServer();
            server.Start();

            while (server.Running()) ;

            server.Close();


        }
    }
}
