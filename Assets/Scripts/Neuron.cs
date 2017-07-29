using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Neuromancers {
	
	public class Neuron : MonoBehaviour {
       
		public List<Neuron> neighbors = new List<Neuron> ();

		public Renderer render;

		Vector3 strScale;
		/////Protected/////
		//References
		protected List<Connection> allConnections;
		//Primitives
		private bool neighborNode = false;
		private float currentTemperature = 0f;

		//properties
		public float CurrentTemperature {
			get { return currentTemperature; }
			set  { currentTemperature = value; }
		}

		public bool NeighborNode {
			get { return neighborNode; }
			set {
				neighborNode = value;
               
			}
		}

		///////////////////////////////////////////////////////////////////////////
		//
		// Inherited from MonoBehaviour
		//

		protected void Awake () {

			this.allConnections = new List<Connection>();
		}

		protected void Start () {
			
//			SetupNeighbors ();

			strScale = transform.localScale;
		}



		///////////////////////////////////////////////////////////////////////////
		//
		// Neuron Functions
		//

		public void AddConnection(Connection newConnection) {

			this.allConnections.Add(newConnection);
		}





		/// <summary>
		/// Selecting a node for input
		/// </summary>
		/// 
		public GameObject spherePrefab;

		public void SelectNode (Neuron parent) {

//			LeanTween.scale (transform.Find("View").gameObject, strScale * 1.5f, 0.2f).setLoopPingPong (1);
			//render.material.color = Color.red;
			UIPrimitives.MaterialAnimator matAnim = render.GetComponent<UIPrimitives.MaterialAnimator> ();
			matAnim.ClearAllAnimations ();
			matAnim.AddColorStartAnimation (Color.white, Color.red, loopCount: 1, duration: .5f);
			//  if (!neighborNode)
			//   {
			/* foreach (Neuron neighbor in neighbors)
             {

                 neighbor.ImpluseTrigger(1.1f);
                 neighborNode = true;
             }*/

			for (int i = 0; i < allConnections.Count; ++i) {

				Neuron n = allConnections[i].DestinationNeuron;
	
				GameObject pathGO = Instantiate (spherePrefab) as GameObject;
				pathGO.transform.position = parent.transform.position;
				pathGO.GetComponent<UIPrimitives.UITransformAnimator> ().AddPositionEndAnimation (n.transform.position, 1.1f, UIPrimitives.UIAnimationUtility.EaseType.easeInOutSine);
				Destroy (pathGO, 1.1f);
				n.ImpluseTrigger (1.1f);
			}
			// }

		}

		public void ImpluseTrigger (float delta) {
			CurrentTemperature += delta;
			//gameObject.transform.localScale *= delta;
			// gameObject.GetComponent<Renderer>().material.color = Color.red;

			StartCoroutine ("Wait", 0.6f);
		}

		private void OnMouseDown () {
          

			neighborNode = false;
			SelectNode (this);
			Debug.Log ("WE ARE WORKING AT POWER LEVEL 9000");
		}





    
		IEnumerator Wait (float seconds) {
			yield return new WaitForSeconds (seconds);
			SelectNode (this);
		}

	}
}