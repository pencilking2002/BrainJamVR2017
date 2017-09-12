using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Neuromancers
{
   
    public class VideoPlayer : MonoBehaviour
    {
        

      
       
       // Use this for initialization
       protected void Awake()
       {

        }

        protected void Start()
        {

           
            PlayVideoVR();
        }


        public void PlayVideoVR()
        {
            Destroy(this.gameObject, 10f);
            this.gameObject.GetComponent<MediaPlayerCtrl>().Play();


        }
   
    }
}
