using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Neuromancers
{
    [RequireComponent(typeof(UnityEngine.Video.VideoPlayer))]
    public class VideoPlayer : MonoBehaviour
    {

        UnityEngine.Video.VideoPlayer vrPlayer;

        public string youtube360;
        public bool testMode = false;
       // Use this for initialization
       protected void Awake()
       {
        }

        protected void Start()
        {
            vrPlayer = GetComponent<UnityEngine.Video.VideoPlayer>();
          //  if(!testMode)
       //     vrPlayer.url = youtube360;
        }


        public void PlayVideoVR()
        {
            vrPlayer.Play();
        }
   
    }
}
