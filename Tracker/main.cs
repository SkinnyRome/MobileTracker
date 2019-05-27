using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;


class main
{
    static void Main(string[] args)
    {
        Tracker.TrackerEvent[] e = new Tracker.TrackerEvent[10];

        for (int i = 0; i < e.Length; i++)
        {
            e[i] = new Tracker.TrackerEvent("asas", 1, i);
        }
        for (int i = 0; i < e.Length; i++)
        {
            Tracker.Tracker.Instance.AddEvent(e[i]);
        }

        Tracker.SerializerInterface b = new Tracker.BinarySerializer();
        Tracker.Tracker.Instance.AddSerializer(b, true);
        Tracker.Tracker.Instance.SetPath("D:/USABILIDAD/Proyecto/MobileTracker/");
        //Tracker.Tracker.Instance.SaveTmpData();
        string[] file = File.ReadAllLines("D:/USABILIDAD/Proyecto/MobileTracker" + "/TrackerInfo.data");
        for(int i = 0; i < file.Length; i++)
        {
            string a = Tracker.Utilities.Instance.BinaryToString(file[i]);
            File.AppendAllText("D:/USABILIDAD/Proyecto/MobileTracker" + "/TrackerInfoServer.csv", a + "\n");
        }

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



        //Tracker.Tracker.Instance.ServerDumpData();
        //Tracker.Tracker.Instance.ProcessData();
        //Tracker.Tracker.Instance.Init();

    }
}