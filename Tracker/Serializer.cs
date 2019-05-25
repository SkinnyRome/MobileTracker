using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.IO;
using UnityEngine;
using System.Xml;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO.Compression;
using System.Xml.Serialization;

namespace Tracker
{
    public abstract class SerializerInterface
    {
        
        //Save an event
        public abstract void DumpEvent(TrackerEvent e, string path);

        public const string _fileName = "TrackerInfo";

        public string GetFileName() { return _fileName; }
    }

    //JSON
    public class JsonSerializer : SerializerInterface
    {

        public override void DumpEvent(TrackerEvent e, string path)
        { 
            string result = JsonConvert.SerializeObject(e);
            File.AppendAllText(path + _fileName + ".json", result);
        }
    }

    //CSV
    public class CSVSerializer : SerializerInterface
    {
        public override void DumpEvent(TrackerEvent e, string path)
        { 
            string csv = string.Format("{0},{1},{2},{3}\n", e.IdSession, e.Type, e.TimeStamp, e.MoreInfo);
            File.AppendAllText(path + _fileName + ".csv", csv);
        }
    }

    //XML
    public class XMLSerializer : SerializerInterface
    {
        public override void DumpEvent(TrackerEvent e, string path)
        {
            throw new NotImplementedException();
        }
    }

    //BinaryFormatter
    public class BinaryFormatterSerializer : SerializerInterface
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream fs;
        public override void DumpEvent(TrackerEvent e, string path)
        {
            // Open the file if it exits or create a new one
            fs = File.Open(path + _fileName, FileMode.Append);

            bf.Serialize(fs, e);

            // Close the file and release any resources
            fs.Close();
        }
    }

    //Binary
    public class BinarySerializer : SerializerInterface
    {
        public override void DumpEvent(TrackerEvent e, string path)
        {
            StringBuilder sb = new StringBuilder();
            string data = string.Format("{0},{1},{2},{3}", e.IdSession, e.Type, e.TimeStamp, e.MoreInfo);

            foreach (char c in data.ToCharArray())
            {
                sb.Append(Convert.ToString(c, 2).PadLeft(8, '0'));
            }
            File.AppendAllText(path + _fileName + ".data", sb.ToString() + "\n");
        }
    }        
}
