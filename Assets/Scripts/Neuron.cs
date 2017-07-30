using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Neuromancers {
	
	public class Neuron : MonoBehaviour {
       
		//readonly 
		protected readonly float FIRE_DELAY = 1f;
		protected readonly float MIN_FIRE_INTERVAL = .2f;

		//Serialized
//		public List<Neuron> neighbors = new List<Neuron> ();
//		public Renderer render;

		/////Protected/////
		//References
		protected List<Connection> allConnections;
		protected NeuronRenderer neuronRenderer;
		//Primitives
		protected float lastFireTime;
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
			GetComponent<Button>().SelectedActionSimple += OnMouseDown;
		}

		protected void Start () {
			
//			SetupNeighbors ();

			strScale = transform.localScale;
		}
//		[EditorButtonAttribute]
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
            GameManager.instance.neuronHitCounter++;
			lastFireTime = Time.time;
//			UIPrimitives.MaterialAnimator matAnim = render.GetComponent<UIPrimitives.MaterialAnimator> ();
//			matAnim.ClearAllAnimations ();
//			matAnim.AddColorStartAnimation (Color.white, Color.red, loopCount: 1, duration: .5f);
		
			for (int i = 0; i < allConnections.Count; ++i) {

				Connection c = allConnections[i];
				Neuron n = c.DestinationNeuron;
	
				GameObject pathGO = Instantiate (spherePrefab) as GameObject;
				pathGO.transform.position = c.GetWorldStartPosition();
				pathGO.GetComponent<Renderer>().material.SetColor("_TintColor",c.GetColor());
				pathGO.GetComponent<UIPrimitives.UITransformAnimator> ().AddPositionEndAnimation (n.transform.position, FIRE_DELAY, UIPrimitives.UIAnimationUtility.EaseType.easeInOutSine);
				Destroy (pathGO, FIRE_DELAY);

				StartCoroutine(n.ImpluseTrigger (c.Strength));
			}

		}

		public int GetConnectionCount() {

			return this.allConnections.Count;
		}

		public IEnumerator ImpluseTrigger (float delta) {

			yield return new WaitForSeconds (FIRE_DELAY);

			this.energyLevel += delta;
			this.energyLevel = Mathf.Clamp01(this.energyLevel);

			if(this.energyLevel == 1f && (Time.time - lastFireTime) > MIN_FIRE_INTERVAL) {

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