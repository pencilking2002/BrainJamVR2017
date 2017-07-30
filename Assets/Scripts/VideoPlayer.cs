using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Neuromancers
{
    [RequireComponent(typeof(UnityEngine.Video.VideoPlayer))]
    public class VideoPlayer : MonoBehaviour
    {

        [Tooltip("Put Youtube 360 live stream link here")]
        public string youtube360;
        [Tooltip("Turn into test offline video")]
        public bool testMode = false;

        private UnityEngine.Video.VideoPlayer vrPlayer;
       
       // Use this for initialization
       protected void Awake()
       {
        }

        protected void Start()
        {
            vrPlayer = GetComponent<UnityEngine.Video.VideoPlayer>();
            if(!testMode)
            vrPlayer.url = youtube360;
        }


        public void PlayVideoVR()
        {
            vrPlayer.Play();
        }
   
    }
}
