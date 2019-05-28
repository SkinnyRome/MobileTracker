using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Collections.Concurrent;
using WebSocketSharp;
using WebSocketSharp.Server;
using System.Threading;
using System.IO.Compression;

namespace Tracker
{


    public sealed class Tracker
    {

        private static Tracker _instance;
        private WebSocket _socket;
        private ConcurrentQueue<TrackerEvent> _eventQueue;
        private int MAX_ELEMS = 500;
        //path where it will be saved
        private string _path;
        //list of all serializers
        private List<Serializer> _serializerList;

        //Thread
        private Thread _thread;
        private bool _running = true;


        ///time which the program must wait between piece data deliveries
        private float _deliveryTime;

        public enum MODE { LOW, NORMAL, ULTRA};
        private float _mode = 1;

        /// <summary>
        /// Struct for controlling the Tracker delivery rate in order to optimize device battery
        /// FULL: Increase the data delivery frequency to the maximum
        /// HIGH: Increase the data delivery frequency to a high level
        /// MODERATE: Increase the data delivery frequency to moderate level
        /// LOW: Makes a minor increment in the data delivery frequency
        /// NONE: The frequency of delivery isn't affected
        /// </summary>
        enum Optimize { FULL, HIGH, MODERATE, LOW, NONE };

        private Optimize CheckBatteryStatus()
        {
            float batteryLevel = SystemInfo.batteryLevel;
            BatteryStatus batteryStatus = SystemInfo.batteryStatus;

            if (batteryLevel == -1)
            {
#if DEBUG
                Debug.Log("Current platform doesn´t support battery level info");

#endif
                return Optimize.NONE;
            }
            if (batteryStatus == BatteryStatus.Discharging || batteryStatus == BatteryStatus.NotCharging)
            {
                if (batteryLevel >= 0.75f)// LOW
                {
                    return Optimize.LOW;
                }
                else if (batteryLevel >= 0.5f)// MODERATE
                {
                    return Optimize.MODERATE;
                }
                else if (batteryLevel >= 0.3f)// HIGH
                {
                    return Optimize.HIGH;
                }
                else if (batteryLevel < 0.3f)// FULL
                {
                    return Optimize.FULL;
                }
                else
                    return Optimize.NONE;// NONE
            }
            else
            {
                return Optimize.NONE;// NONE
            }
        }

        //Change delivery frequency in function of battery
        private void OptimizeTracker(Optimize level)
        {
            switch (level)
            {
                case Optimize.FULL:
                    _deliveryTime = 90.0f * _mode;
                    break;
                case Optimize.HIGH:
                    _deliveryTime = 60.0f * _mode;
                    break;
                case Optimize.MODERATE:
                    _deliveryTime = 30.0f * _mode;
                    break;
                case Optimize.LOW:
                    _deliveryTime = 15.0f * _mode;
                    break;
                case Optimize.NONE:
                    _deliveryTime = 7.0f * _mode;
                    break;
            }
        }

        //Types of connectivity
        enum Connectivity { NOINTERNET, CARRIERDATA, WIFI }
        //Check device connection
        private Connectivity ConnectivityStatus()
        {
            //Not reachable
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                return Connectivity.NOINTERNET;
            }
            //Check if the device can reach the internet via a carrier data network
            else if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
            {
                return Connectivity.CARRIERDATA;
            }
            //Check if the device can reach the internet via a LAN (WIFI...)
            else 
            {
                return Connectivity.WIFI;
            }
           
        }


        //struct serializer with the serializer and his control bool
        struct Serializer
        {
            public SerializerInterface _serializer;
            public bool _active;
        }


        private Tracker()
        {
            _eventQueue = new ConcurrentQueue<TrackerEvent>();
            _serializerList = new List<Serializer>();
            _socket = new WebSocket("ws://localhost:4649/server");
            _thread = new Thread(ProcessData);
        }

        //Init the thread
        public void Init()
        {
            _running = true;
            _thread.Start();
        }

        //Stop the thread
        public void Stop(bool ShowSystemEvents)
        {
            if (ShowSystemEvents)
            {
                AddEvent(new SystemEvent("-1", -1, -1));
                AddEvent(new GyroEvent("-1", -1, -1));
                AddEvent(new CameraEvent("-1", -1, -1));
                AddEvent(new MicrophoneEvent("-1", -1, -1));
            }
            _running = false;
            _thread.Join();
            //Do a final save for last events
            if (ConnectivityStatus() == Connectivity.NOINTERNET)
            {
                SaveTmpData();
            }
            else
            {
                if (File.Exists(_path + _serializerList[0]._serializer.GetFileName() + ".data"))
                {
                    ServerDumpData();
                }
                SaveTmpData();
                ServerDumpData();
            }
        }

        public static Tracker Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Tracker();
                return _instance;
            }
        }

        //Set a path
        public void SetPath(string path)
        {
            _path = path;
        }

        //Change the opt of battery
        public void SetOptMode (MODE mode)
        {
            switch (mode)
            {
                case MODE.LOW:
                    _mode = 0.5f;
                    break;
                case MODE.NORMAL:
                    _mode = 1.0f;
                    break;
                case MODE.ULTRA:
                    _mode = 2.0f;
                    break;
                default:
                    _mode = 1.0f;
                    break;
            }
        }

        //Set max elem of events (500-2000)
        public void SetMAX_ELEM(int maxElem)
        {
            if (maxElem >= 500 && maxElem <= 2000)
            {
                MAX_ELEMS = maxElem;
            }
        }

        //Add new serializers
        public void AddSerializer(SerializerInterface sI, bool active)
        {
            Serializer s;
            s._serializer = sI;
            s._active = active;
            _serializerList.Add(s);
        }


        //Add an event
        public void AddEvent(TrackerEvent e)
        {

            if (_eventQueue.Count < MAX_ELEMS)
            {
                _eventQueue.Enqueue(e);
            }
        }

        //Force add an event
        public void ForceAddEvent(TrackerEvent e)
        {

            if (_eventQueue.Count < MAX_ELEMS)
            {
                _eventQueue.Enqueue(e);
            }
            else
            {
                TrackerEvent aux;
                _eventQueue.TryDequeue(out aux);
                _eventQueue.Enqueue(e);
            }
        }


        public void ProcessData()//TODO: mirar el tema de guardar cuando no ay conexion y luego enviar lo guardado y comprir todo
        {
            float tIni = DateTime.Now.Second;
            float tEnd;
            while (_running)
            {
                tEnd = DateTime.Now.Second;
                OptimizeTracker(CheckBatteryStatus());
                if (tEnd - tIni >= _deliveryTime)
                {
                    if (ConnectivityStatus() == Connectivity.NOINTERNET)
                    {
                        SaveTmpData();
                    }
                    else 
                    {
                        if (File.Exists(_path + _serializerList[0]._serializer.GetFileName() + ".data"))
                        {
                            ServerDumpData();
                        }
                        SaveTmpData();
                        ServerDumpData();
                    }
                    tIni = DateTime.Now.Second;
                }
            }
        }

        //Proccess event queue
        //Save the info in local for all desired serializers
        public void LocalDumpData()
        {
            while (_eventQueue.Count > 0)
            {
                foreach (var s in _serializerList)
                {
                    if (s._active)
                    {
                        s._serializer.DumpEvent(_eventQueue.First(), _path);
                    }
                }

                TrackerEvent aux;
                _eventQueue.TryDequeue(out aux);
            }
        }

        //Proccess event queue
        //Save the info temporally in local with binary serializer
        public void SaveTmpData()
        {
            while (_eventQueue.Count > 0)
            {
                _serializerList[0]._serializer.DumpEvent(_eventQueue.First(), _path);
                TrackerEvent aux;
                _eventQueue.TryDequeue(out aux);
            }
        }
       

        //Proccess event queue
        //Send information to server
        public void ServerDumpData()
        {

            _socket.Connect();

            FileStream fs = File.Open(_path + _serializerList[0]._serializer.GetFileName() + ".data", FileMode.Open);
            byte[] reads = new byte[fs.Length];
            //Save data in reads
            int s = fs.Read(reads, 0, (int)fs.Length);
            fs.Close();
            //byte[] compress = Utilities.Instance.Compress(reads); If you do a dll of the project, GZipStream of Unity doesn´t work

            _socket.Send(reads);
            _socket.Close();

            FileInfo f = new FileInfo(_path + _serializerList[0]._serializer.GetFileName() + ".data");
            f.Delete();
        }

    }
}