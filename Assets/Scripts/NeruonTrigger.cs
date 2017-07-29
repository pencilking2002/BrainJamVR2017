using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


namespace Neuromancers
{
    [RequireComponent(typeof(SphereCollider))]
    public class NeruonTrigger : MonoBehaviour
    {
        //readonly
        //Serialized
        [Tooltip("Put Parent Neuron object here")]
        public GameObject Neuron; 

        /////Protected/////
        //References
        //Primitives

        ///////////////////////////////////////////////////////////////////////////
        //
        // Inherited from MonoBehaviour
        //

        /// <summary>
        /// add all neuron found when trigger is enabled in the range of the trigger
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerEnter(Collider other)
        {
            
            this.gameObject.GetComponent<Neuron>().neighbors.Add(other.gameObject);
        }


    }
}