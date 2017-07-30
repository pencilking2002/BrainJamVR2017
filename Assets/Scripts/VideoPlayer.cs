using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Neuromancers
{
    [RequireComponent(typeof(UnityEngine.Video.VideoPlayer))]
    public class VideoPlayer : MonoBehaviour
    {
        

        private UnityEngine.Video.VideoPlayer vrPlayer;
       
       // Use this for initialization
       protected void Awake()
       {

        }

        protected void Start()
        {

            vrPlayer = GetComponent<UnityEngine.Video.VideoPlayer>();
            PlayVideoVR();
        }


        public void PlayVideoVR()
        {
            Destroy(this.gameObject, 10f);
            vrPlayer.Play();
            
        }
   
    }
}
