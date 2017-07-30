using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Neuromancers
{
    [RequireComponent(typeof(UnityEngine.Video.VideoPlayer))]
    public class VideoPlayer : MonoBehaviour
    {


       
        [Tooltip("Put Hotsprings Video here ")]
        public UnityEngine.Video.VideoClip hotSprings;
        [Tooltip("Put River Video here ")]
        public UnityEngine.Video.VideoClip river;
        

        private UnityEngine.Video.VideoPlayer vrPlayer;
       
       // Use this for initialization
       protected void Awake()
       {

        }

        protected void Start()
        {

            vrPlayer = GetComponent<UnityEngine.Video.VideoPlayer>();
            if (GameManager.instance.neuronHitCounter >= 10)
                vrPlayer.clip = river;
            else
                vrPlayer.clip = river;
          
            PlayVideoVR();
        }


        public void PlayVideoVR()
        {
            vrPlayer.Play();
        }
   
    }
}
