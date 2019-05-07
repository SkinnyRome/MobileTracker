using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.IO;
using UnityEngine;
using System.Xml;

namespace Tracker
{
    public interface SerializerInterface
    {
        //Save an event
        void DumpEvent(TrackerEvent e, string path);

    }

    //JSON
    public class JsonSerializer : SerializerInterface
    {

        public void DumpEvent(TrackerEvent e, string path)
        { 
            string result = JsonConvert.SerializeObject(e);
            File.AppendAllText(path + "TrackerInfo.json", result);
        }
    }

    //CSV
    public class CSVSerializer : SerializerInterface
    {

        public void DumpEvent(TrackerEvent e, string path)
        { 
            string csv = string.Format("{0},{1},{2}\n", e.IdSession, e.Type, e.TimeStamp);
            File.AppendAllText(path + "TrackerInfo.csv", csv);

        }
    }


    /* class XMLSerializer : SerializerInterface
     {

         string _path = "D:/UNIVERSIDAD/2018-2019/Ánalisis y Usabilidad de Videojuegos/Tracker/TrackerInfo.xml";
         XmlDocument xml_document;


         public void DumpEvent(Queue<TrackerEvent> queue)
         {

             while (queue.Count > 0)
             {

                 // Format the XML text.
                 StringWriter string_writer = new StringWriter();
                 XmlTextWriter xml_text_writer = new XmlTextWriter(string_writer);
                 xml_text_writer.Formatting = System.Xml.Formatting.Indented;
                 xml_document.WriteTo(xml_text_writer);


                 // Display the result.
                 txtResult.Text = string_writer.ToString();

             }
         }
     }*/



}
