using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


class main
{
    static void Main(string[] args)
    {
        Tracker.TrackerEvent e = new Tracker.TrackerEvent("12312", 454, 324.0f);
        Tracker.Tracker.Instance.AddEvent(e);
        Tracker.SerializerInterface csv = new Tracker.JsonSerializer();
        Tracker.Tracker.Instance.DumpData(csv);
    }
}