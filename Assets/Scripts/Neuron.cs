using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Neuromancers
{
    public class Neuron : MonoBehaviour
    {
       
        [Tooltip("Put child trigger object here")]
        public GameObject neuronTigger;
        public List<Neuron> neighbors = new List<Neuron>();


        /////Protected/////
        //References
        //Primitives
        private bool neighborNode = false;

        public bool NeighborNode
        {
            get { return neighborNode; }
            set
            {
                neighborNode = value;
                NeigborNodesSelectable(neighborNode);
            }
        }

        ///////////////////////////////////////////////////////////////////////////
        //
        // Inherited from MonoBehaviour
        //

    protected void Awake()
    {
    }

    protected void Start()
    {
            if (!neuronTigger)
                Debug.LogError("Missing Trigger object");
    }

    protected void Update()
    {

    }

   /// <summary>
   /// TURN on/off Highligher for neighbor nodes
   /// </summary>
   /// <param name="turnOn"></param>
    private void NeigborNodesSelectable(bool turnOn)
    {

            if (turnOn)
            {
                //TODO: TURN ON HIGHTLIGHTER
            }
            else
            {
                //TODO: TURN OFF HightLIGHTER
            }
           
    }
    

        /// <summary>
        /// Selecting a node for input
        /// </summary>
    public void SelectNode()
    {
            //Check to see if we are in neighbor selection
            if (neighborNode)
            {

            }
           else //normal selection
            {
                if (!neuronTigger)
                {
                    neuronTigger.SetActive(true);
                    Destroy(neuronTigger, 1f);
                }

            }

    }

   public void MakeNeighborsList(Neuron )
   {

   }







    ///////////////////////////////////////////////////////////////////////////
    //
    // #SCRIPTNAME# Functions
    //

        ////////////////////////////////////////
        //
        // Function Functions
    }
}