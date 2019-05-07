using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Collections.Concurrent;

namespace Tracker
{


    public sealed class Tracker
    {
        private static Tracker _instance;
        private ConcurrentQueue<TrackerEvent> _eventQueue;
        const int MAX_ELEMS = 500;
        //path where it will be saved
        private string _path;
        //list of all serializers
        private List<Serializer> _serializerList;

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

        //Proccess event queue
        public void DumpData()
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
    }
}