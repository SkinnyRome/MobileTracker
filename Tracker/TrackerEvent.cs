using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Runtime.Serialization;


namespace Tracker
{
    [Serializable]
    public class TrackerEvent
    {

        protected int type;
        protected string idSession;
        protected float timeStamp;
        protected string moreInfo;

        public TrackerEvent(string idS, int t, float tS, string mI = ""){

            type = t;
            idSession = idS;
            timeStamp = tS;
            moreInfo = mI;
        }

        public float TimeStamp { get => timeStamp; set => timeStamp = value; }
        public string IdSession { get => idSession; set => idSession = value; }
        public int Type { get => type; set => type = value; }
        public string MoreInfo { get => moreInfo; set => moreInfo = value; }
    }
    [Serializable]
    public class GyroEvent : TrackerEvent
    {
        public GyroEvent(string idS, int t, float tS) : base(idS, t, tS)
        {
            moreInfo = SystemInfo.supportsGyroscope.ToString();
            if (SystemInfo.supportsGyroscope)
            {
                if (Input.gyro.enabled)
                    moreInfo += Input.gyro.attitude.w.ToString() + " " + Input.gyro.attitude.x.ToString() + " " + Input.gyro.attitude.y.ToString() + " " + Input.gyro.attitude.z.ToString();
                else
                    moreInfo += "NotEnabled";
            }
        }
    }
    [Serializable]
    public class RunInBackgroundEvent : TrackerEvent
    {
        public RunInBackgroundEvent(string idS, int t, float tS) : base(idS, t, tS)
        {
            moreInfo = Application.runInBackground.ToString();
        }
    }
    [Serializable]
    public class CameraEvent : TrackerEvent
    {
        public CameraEvent(string idS, int t, float tS) : base (idS, t, tS)
        {
            foreach (var device in WebCamTexture.devices)
            {
                moreInfo += device.ToString() + " ";
            }
        }

    }
    [Serializable]
    public class MicrophoneEvent : TrackerEvent
    {
        public MicrophoneEvent(string idS, int t, float tS) : base(idS, t, tS)
        { 
            foreach (var device in Microphone.devices)
            {
               moreInfo += device.ToString() + " " + Microphone.IsRecording(device).ToString() + " ";
            }
        }
    }
    [Serializable]
    public class SystemEvent : TrackerEvent
    {
        public SystemEvent(string idS, int t, float tS) : base(idS, t, tS)
        {
            moreInfo = SystemInfo.deviceModel.ToString() + " " + SystemInfo.graphicsDeviceName.ToString() + " " + SystemInfo.graphicsDeviceType.ToString() + " " + SystemInfo.operatingSystem.ToString() + " " + SystemInfo.processorCount.ToString() + " " + SystemInfo.processorFrequency.ToString() + " " + SystemInfo.supportsVibration.ToString();
        }
    }







}
