using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;


class main
{
    static void Main(string[] args)
    {
        Tracker.TrackerEvent[] e = new Tracker.TrackerEvent[150];
        
        for(int i = 0; i < e.Length; i++)
        {
            e[i] = new Tracker.TrackerEvent("asas", 1, i);
        }
        for (int i = 0; i < e.Length; i++)
        {
            Tracker.Tracker.Instance.AddEvent(e[i]);
        }

        /*Tracker.SerializerInterface b = new Tracker.CSVSerializer();
        Tracker.Tracker.Instance.AddSerializer(b, true);
        Tracker.Tracker.Instance.SetPath("D:/USABILIDAD/Proyecto/MobileTracker/");
        Tracker.Tracker.Instance.DumpData();*/

        /*//Compress process
         FileStream fs = File.Open("D:/USABILIDAD/Proyecto/MobileTracker/TrackerInfoCSV.csv", FileMode.Open);
         byte[] reads = new byte[fs.Length];
        //Save data in reads
         int s = fs.Read(reads, 0, (int)fs.Length);
         byte [] compress = Tracker.Utilities.Instance.Compress(reads);
         File.WriteAllBytes("D:/USABILIDAD/Proyecto/MobileTracker/TrackerInfoCSV.gz", compress);*/


        /*//Decompress process
        byte[] file = File.ReadAllBytes("D:/USABILIDAD/Proyecto/MobileTracker/TrackerInfoCSV.gz");
        byte[] decompressed = Tracker.Utilities.Instance.Decompress(file);
        File.WriteAllBytes("D:/USABILIDAD/Proyecto/MobileTracker/TrackerInfoCSV.csv", decompressed);*/
       
    }
}