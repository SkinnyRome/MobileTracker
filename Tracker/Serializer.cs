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

    public class BinarySerializer : SerializerInterface
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream fs;
        public void DumpEvent(TrackerEvent e, string path)
        {
            // TODO: PROBAR SERIALIZAR UNA TRAZA Y A PARTE COMPRIMIRLA Y ESCRIBIRLA PARA COMPARAR CUAL OCUPA MAS
            // ORDENAR TODO ESTO 

            // Open the file if it exits or create a new one
            fs = File.Open(path + "TrackerInfo.data", FileMode.Append);

            string eventString = string.Format("{0},{1},{2}\n", e.IdSession, e.Type, e.TimeStamp);

            // Convert a string to byte array.
            byte[] text = Encoding.ASCII.GetBytes(eventString);

            // Use compress method.
            byte[] compress = Compress(text);

            for (int i = 0; i < compress.Length; i++)
            {
                Console.Write(compress[i]);
            }
            Console.WriteLine();

            // Close the file and release any resources
            fs.Close();

            File.WriteAllBytes(path + "TrackerInfo.data", compress);

            fs = File.Open(path + "TrackerInfo.data", FileMode.Open);

           /**************************/

            byte[] bytes = new byte[fs.Length];
            int numBytesToRead = (int)fs.Length;
            int numBytesRead = 0;
            while (numBytesToRead > 0)
            {
                // Read may return anything from 0 to numBytesToRead.
                int n = fs.Read(bytes, numBytesRead, numBytesToRead);

                // Break when the end of the file is reached.
                if (n == 0)
                    break;

                numBytesRead += n;
                numBytesToRead -= n;
            }
            numBytesToRead = bytes.Length;
            for (int i = 0; i < bytes.Length; i++)
            {
                Console.Write(bytes[i]);
            }

            byte[] d = Decompress(bytes);

            string tet = Encoding.ASCII.GetString(d);

            /*****************/
            // Save the binary serialized event in the file
            //bf.Serialize(fs, compress);

        }

        /// <summary>
        /// Compresses byte array to new byte array.
        /// </summary>
        public byte[] Compress(byte[] raw)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                using (GZipStream gzip = new GZipStream(memory, CompressionMode.Compress, true))
                {
                    gzip.Write(raw, 0, raw.Length);
                }
                return memory.ToArray();
            }
        }

        /// <summary>
        /// Decompresses byte array to new byte array.
        /// </summary>
        public static byte[] Decompress(byte[] raw)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                using (GZipStream gzip = new GZipStream(memory, CompressionMode.Decompress, true))
                {
                    gzip.Write(raw, 0, raw.Length);
                }
                return memory.ToArray();
            }
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
