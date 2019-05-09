using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


class main
{
    static void Main(string[] args)
    {
        Tracker.TrackerEvent e = new Tracker.TrackerEvent("12312", 454, 324.0f);
        Tracker.TrackerEvent ee = new Tracker.TrackerEvent("12312", 54, 324.0f);
        Tracker.TrackerEvent eee = new Tracker.TrackerEvent("12312", 44, 324.0f);

        Tracker.Tracker.Instance.AddEvent(e);
        Tracker.Tracker.Instance.AddEvent(ee);
        Tracker.Tracker.Instance.AddEvent(eee);

        Tracker.SerializerInterface b = new Tracker.BinarySerializer();
        Tracker.Tracker.Instance.AddSerializer(b, true);
        Tracker.Tracker.Instance.SetPath("C:/hlocal/");
        Tracker.Tracker.Instance.DumpData();

    }
}