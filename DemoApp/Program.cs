using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tracker;

namespace DemoApp
{
    class Program
    {
        static void Main(string[] args)
        {
            TrackerEvent e1 = new TrackerEvent("prueba", 10, 0.5f);
            Tracker.Tracker.Instance.AddEvent(e1);
            Tracker.Tracker.Instance.Send();
        }
    }
}
