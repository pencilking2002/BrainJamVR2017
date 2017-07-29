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

        public Renderer render;

        Vector3 strScale;
        /////Protected/////
        //References
        //Primitives
        private bool neighborNode = false;
        private float currentTemperature = 0f;

        public float CurrentTemperature
        {
            get { return currentTemperature; }
            set  {  currentTemperature = value; }
        }
        public bool NeighborNode
        {
            get { return neighborNode; }
            set
            {
                neighborNode = value;
               
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
            SetupNeighbors();

            strScale = transform.localScale;
    }

    private void SetupNeighbors()
        {
            neighbors = BrainController.getSingleton().getNodeInRange(this, 3f);
        }





        /// <summary>
        /// Selecting a node for input
        /// </summary>
        /// 
        public GameObject spherePrefab;
    public void SelectNode( Neuron parent)
    {

            LeanTween.scale(gameObject, strScale * 1.5f, 0.2f).setLoopPingPong(1);
            //render.material.color = Color.red;
            UIPrimitives.MaterialAnimator matAnim = render.GetComponent<UIPrimitives.MaterialAnimator>();
            matAnim.ClearAllAnimations();
            matAnim.AddColorStartAnimation(Color.white, Color.red, loopCount:  1, duration: .5f);
            //  if (!neighborNode)
            //   {
            /* foreach (Neuron neighbor in neighbors)
             {

                 neighbor.ImpluseTrigger(1.1f);
                 neighborNode = true;
             }*/

            Neuron n = neighbors[UnityEngine.Random.Range(0, neighbors.Count)];

            if(parent != n || true)
            {
                GameObject pathGO = Instantiate(spherePrefab) as GameObject;
                pathGO.transform.position = parent.transform.position;
                pathGO.GetComponent<UIPrimitives.UITransformAnimator>().AddPositionEndAnimation(n.transform.position, 1.1f, UIPrimitives.UIAnimationUtility.EaseType.easeInOutSine);
                Destroy(pathGO, 1.1f);
                n.ImpluseTrigger(1.1f);
            }
                           // }

        }

    public void ImpluseTrigger(float delta)
    {
            CurrentTemperature += delta;
            //gameObject.transform.localScale *= delta;
            // gameObject.GetComponent<Renderer>().material.color = Color.red;

            StartCoroutine("Wait", 0.6f);
    }

        private void OnMouseDown()
        {
          

            neighborNode = false;
            SelectNode(this);
            Debug.Log("WE ARE WORKING AT POWER LEVEL 9000");
        }





    
        IEnumerator Wait(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            SelectNode(this);
        }

    }
}