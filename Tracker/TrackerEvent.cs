using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Tracker
{
    public class TrackerEvent
    {

        private int type;
        private string idSession;
        private float timeStamp;

        public TrackerEvent(string idS, int t, float tS){

            type = t;
            idSession = idS;
            timeStamp = tS;
        }


        public float TimeStamp { get => timeStamp; set => timeStamp = value; }
        public string IdSession { get => idSession; set => idSession = value; }
        public int Type { get => type; set => type = value; }


    }

    

      

}
