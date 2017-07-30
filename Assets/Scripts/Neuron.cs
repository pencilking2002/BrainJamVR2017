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

		//Actions
		public Action FireAction;


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

		protected void Update() {

			this.neuronRenderer.SetEnergyLevel(this.energyLevel);
			this.energyLevel -= Time.deltaTime*.2f;
			this.energyLevel = Mathf.Clamp01(this.energyLevel);
		}


		///////////////////////////////////////////////////////////////////////////
		//
		// Neuron Functions
		//

		public void SetParticlesEnabled(bool value) {

			GetComponentInChildren<ParticleSystem>().SetEmissionEnabled(value);
		}

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
				c.Fire();
				Neuron n = c.DestinationNeuron;
	
				GameObject pathGO = Instantiate (spherePrefab) as GameObject;
				pathGO.transform.position = c.GetWorldStartPosition();
				pathGO.GetComponent<Renderer>().material.SetColor("_TintColor",c.GetColor());
				pathGO.GetComponent<UIPrimitives.UITransformAnimator> ().AddPositionEndAnimation (n.transform.position, FIRE_DELAY, UIPrimitives.UIAnimationUtility.EaseType.easeInOutSine);
				Destroy (pathGO, FIRE_DELAY);

				StartCoroutine(n.ImpluseTrigger (c.Strength));
			}

			RandomSound();

			if(FireAction!=null)
				FireAction();
		}

		void RandomSound(){


			Audio.SoundEffect[] MyArray = { 
				Audio.SoundEffect.Neuron_Fire1, 
				Audio.SoundEffect.Neuron_Fire2, 
				Audio.SoundEffect.Neuron_Fire3,
				Audio.SoundEffect.Neuron_Fire4, 
				Audio.SoundEffect.Neuron_Fire5,
				};
			
			int MyIndex = UnityEngine.Random.Range(0,(MyArray.Length - 1));
//			Debug.Log(MyArray[MyIndex]);

			Audio.Instance.PlaySoundEffect(MyArray[MyIndex]);
		}

		public int GetConnectionCount() {

			return this.allConnections.Count;
		}

		public bool IsConnectedTo(Neuron destinationNeuron) {

			for (int i = 0; i < allConnections.Count; ++i) {

				if(allConnections[i].DestinationNeuron == destinationNeuron)
					return true;
			}

			return false;
		}

		public ConnectionType GetConnectionTypeToNeuron(Neuron destinationNeuron) {

			for (int i = 0; i < allConnections.Count; ++i) {

				if(allConnections[i].DestinationNeuron == destinationNeuron)
					return allConnections[i].MyConnectionType;
			}

			return ConnectionType.Unknown;
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