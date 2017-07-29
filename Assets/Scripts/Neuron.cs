using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Neuromancers {
	
	public class Neuron : MonoBehaviour {
       
		//readonly 
		protected readonly float FIRE_DELAY = 1f;


		//Serialized
//		public List<Neuron> neighbors = new List<Neuron> ();
//		public Renderer render;

		/////Protected/////
		//References
		protected List<Connection> allConnections;
		protected NeuronRenderer neuronRenderer;
		//Primitives
		Vector3 strScale;
		private bool neighborNode = false;
		private float energyLevel = 0f;

		//properties
//		public float EnergyLevel {
//			get { return energyLevel; }
//			set  { energyLevel = value; }
//		}
//
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
			this.neuronRenderer = GetComponentInChildren<NeuronRenderer>();
		}

		protected void Start () {
			
//			SetupNeighbors ();

			strScale = transform.localScale;
		}

		protected void Update() {

			this.energyLevel -= Time.deltaTime*.2f;
			this.energyLevel = Mathf.Clamp01(this.energyLevel);
			this.neuronRenderer.SetEnergyLevel(this.energyLevel);
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

		public void Fire () {
			
//			UIPrimitives.MaterialAnimator matAnim = render.GetComponent<UIPrimitives.MaterialAnimator> ();
//			matAnim.ClearAllAnimations ();
//			matAnim.AddColorStartAnimation (Color.white, Color.red, loopCount: 1, duration: .5f);
		
			for (int i = 0; i < allConnections.Count; ++i) {

				Neuron n = allConnections[i].DestinationNeuron;
	
				GameObject pathGO = Instantiate (spherePrefab) as GameObject;
				pathGO.transform.position = this.transform.position;
				pathGO.GetComponent<UIPrimitives.UITransformAnimator> ().AddPositionEndAnimation (n.transform.position, FIRE_DELAY, UIPrimitives.UIAnimationUtility.EaseType.easeInOutSine);
				Destroy (pathGO, FIRE_DELAY);

				StartCoroutine(n.ImpluseTrigger (allConnections[i].Strength));
			}
			// }

		}

		public IEnumerator ImpluseTrigger (float delta) {

			yield return new WaitForSeconds (FIRE_DELAY);

			this.energyLevel += delta;
			this.energyLevel = Mathf.Clamp01(this.energyLevel);

			if(this.energyLevel == 1f) {

				Fire ();
			}
		}

		private void OnMouseDown () {
          
			this.energyLevel = 1f;
			neighborNode = false;
			Fire ();
//			Debug.Log ("WE ARE WORKING AT POWER LEVEL 9000");
		}


	}
}