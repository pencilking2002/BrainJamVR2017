using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Neuromancers {

	public enum ConnectionType {

		Unknown,
		Excitory,
		Inhibitory,
	}

	public class NeuronManager : MonoBehaviour {

		//readonly
		protected readonly int TUTORIAL_FINISH_FIRE_THRESHOLD = 10;
		protected readonly int TUTORIAL_NEURON_COUNT = 3;
		protected readonly int NEURON_COUNT = 50;
		protected readonly float MAX_CONNECTION_RANGE = 3f;
		protected readonly float MIN_CONNECTION_STRENGTH = -1f;
		protected readonly float MAX_CONNECTION_STRENGTH = 1f;
		protected readonly int MAX_NEURON_CONNECTIONS = 2;

		//serialized
		public List<Neuron> neurons = new List<Neuron> ();
		public float neuronColliderRadius = .1f;
		public float radiusMin = 4;
		public float radiusMax = 10;
		[SerializeField]
		protected bool showTutorial;
		[SerializeField]
		protected Button resetButton;

		//protected
		protected GameObject neuronPrefab;
		protected GameObject connectionPrefab;
		//primtiive
		protected int currentTutorialFireCount;

		///////////////////////////////////////////////////////////////////////////
		//
		// Inherited from MonoBehaviour
		//

		protected void Awake () {

			neuronPrefab = Resources.Load ("Prefabs/Neuron") as GameObject;
			connectionPrefab = Resources.Load ("Prefabs/Connection") as GameObject;
			resetButton.SelectedActionSimple+=OnResetButtonSelected;
		}

		protected void Start () {

			if (showTutorial) {

				StartCoroutine(StartTutorial());
			} else {
				
				CreateNeurons (NEURON_COUNT);
				ConnectNeurons ();
			}
		}

		protected void Update () {

		}

		///////////////////////////////////////////////////////////////////////////
		//
		// NeuronManager Functions
		//

		protected IEnumerator StartTutorial() {

			//initial wait time
			yield return new WaitForSeconds(1f);

			//spawna  few neurons, one per second
			for (int i = 0; i < TUTORIAL_NEURON_COUNT; ++i) {

				Neuron neuron = CreateNeuron();
				neuron.SetParticlesEnabled(true);
				neuron.FireAction += OnNeuronFired;
				yield return new WaitForSeconds(.1f);
			}

			//wait time before we start making connections
			yield return new WaitForSeconds(1f);

			//now we make the connections, one every second
			for (int i = 0; i < neurons.Count; ++i) {

				ConnectNeuron(i,true,true);
				yield return new WaitForSeconds(.1f);
			}

			yield return new WaitUntil (() => currentTutorialFireCount >= TUTORIAL_FINISH_FIRE_THRESHOLD);

			CreateNeurons(NEURON_COUNT - TUTORIAL_NEURON_COUNT);
			ConnectNeurons();
		}

		protected void OnNeuronFired() {

			currentTutorialFireCount++;
		}

		protected void CreateNeurons (int count) {
			
			// Place nodes
			for (int i = 0; i < count; i++) {

				CreateNeuron();
			}
		}

		protected Neuron CreateNeuron() {

			GameObject newNeuronGO = Instantiate (neuronPrefab) as GameObject;

			Neuron neuron = newNeuronGO.GetComponent<Neuron> ();
			neuron.GetComponentInChildren<SphereCollider> ().radius = neuronColliderRadius;
			neurons.Add (neuron);

			newNeuronGO.transform.position = GetPositionInNightSky (); //GetRandomNeuronPosition ();
			newNeuronGO.transform.parent = this.transform;

			return neuron;
		}

		protected void ConnectNeurons () {

			for (int i = 0; i < neurons.Count; ++i) {

				ConnectNeuron(i);
			}
		}

		protected void ConnectNeuron(int i, bool shouldIgnoreDistance = false, bool shouldAnimateConnection = true) {

			for (int j = 0; j < neurons.Count; ++j) {

				//don't want to check against itself
				if (j == i)
					continue;

				Neuron sourceNeuron = neurons [i];
				Neuron destinationNeuron = neurons [j];

				//distance check
				float distance = Vector3.Distance (sourceNeuron.gameObject.transform.position, destinationNeuron.gameObject.transform.position);
				bool isWithinMaxDistance = distance < MAX_CONNECTION_RANGE;
//				Debug.Log("isWithinMaxDistance:" +isWithinMaxDistance);
				//max connections check
				bool isAboveMaxConnections = sourceNeuron.GetConnectionCount () >= MAX_NEURON_CONNECTIONS;
				//also check if we're already connected to it (done last for performance reasons)
				if ((isWithinMaxDistance||shouldIgnoreDistance) && !isAboveMaxConnections && !sourceNeuron.IsConnectedTo(destinationNeuron)) {

					ConnectionType connectionType = ConnectionType.Unknown;
					//We want each neuron's first connection to be excitory
					if(sourceNeuron.GetConnectionCount() == 0)
						connectionType = ConnectionType.Excitory;
					//if the destination neuron is already connected to me and its type is excitory, then force my connection to him to be inhibitory
					if(destinationNeuron.IsConnectedTo(sourceNeuron) && destinationNeuron.GetConnectionTypeToNeuron(sourceNeuron) == ConnectionType.Excitory)
						connectionType = ConnectionType.Inhibitory;
					Connection newConnection = CreateConnection (sourceNeuron, destinationNeuron, shouldAnimateConnection,connectionType);

					sourceNeuron.AddConnection (newConnection);
				}

			}
		}

		protected Connection CreateConnection (Neuron sourceNeuron, Neuron destinationNeuron,bool shouldAnimateConnection, ConnectionType connectionType = ConnectionType.Unknown) {

			float connectionStrength = Random.Range (MIN_CONNECTION_STRENGTH, MAX_CONNECTION_STRENGTH);

			GameObject newConnectionGO = Instantiate (connectionPrefab) as GameObject;
			newConnectionGO.transform.SetParent (sourceNeuron.transform);
			newConnectionGO.transform.Recenter ();

			Connection newConnection = newConnectionGO.GetComponent<Connection> ();
			newConnection.SetData (connectionStrength, destinationNeuron,shouldAnimateConnection,connectionType);


			return newConnection;
		}

		protected void OnResetButtonSelected() {

			UnityEngine.Application.LoadLevel(Application.loadedLevel);
		}


		Vector3 GetRandomNeuronPosition () {
			Vector3 centerVec = new Vector3 (0, 0, 0);
		
			float rangeX = 4;
			float rangeY = 4;
			float rangeZ = 4;

			Vector3 randomPos = new Vector3 (
				                    Random.Range (rangeX * -1, rangeX),
				                    Random.Range (rangeY * -1, rangeY), 
				                    Random.Range (rangeZ * -1, rangeZ));

			randomPos += centerVec; // add center point offset

			return randomPos;
		}

		Vector3 GetPositionInNightSky () {

//			Debug.Log ("Get Night Position ");

			bool goodDist = false;
			bool goodVerticalAngle = false;

		

			Vector3 centerVec = new Vector3 (0, 0, -10);
			Vector3 pnt = Vector3.zero;

			int conditions = 0;

			while (conditions <= 1) {
				conditions = 0;
			
				pnt = Random.insideUnitSphere * radiusMax;
		
				goodVerticalAngle = false;

				float angle;
				angle =	CalculateAngle (pnt, centerVec);
				if (130 > angle && angle > 50) {
					

					conditions++;
				}

				/*
				float hAngle;
				hAngle = CalculateAngleHorizon(pnt, centerVec);
				if(180 > hAngle && hAngle > 90)
				{


					conditions++;
				}*/

				// change
			
				float distFromCenter = Vector3.Distance (pnt, centerVec);

				if (distFromCenter > radiusMin) {
					conditions++;
				}
			}

			return pnt;
		}

				
		public static float CalculateAngle (Vector3 from, Vector3 to) {
			return Quaternion.FromToRotation (Vector3.up, to - from).eulerAngles.z;
		}

		public static float CalculateAngleHorizon (Vector3 from, Vector3 to) {
			return Quaternion.FromToRotation (Vector3.left, to - from).eulerAngles.z;
		}



		void DrawConnections () {

			// Each connection made has a line render and a precent 

			/*	foreach (Neuron n in nodes) {

	
				n.gameObject.AddComponent<ConnectionLine>();
				n.gameObject.AddComponent<LineRenderer>();

				ConnectionLine line = n.gameObject.GetComponent<ConnectionLine>();

				Neuron randNeuron = nodes[Random.Range( 0, nodes.Count )];

				line.SetUp(n, randNeuron);
			}*/

			foreach (Neuron n in neurons) {

				List<Neuron> neighbors = getNodeInRange (n, 3f);

			

				foreach (Neuron neighbor in neighbors) {

					n.gameObject.AddComponent<ConnectionLine> ();
					n.gameObject.AddComponent<LineRenderer> ();

					ConnectionLine line = n.gameObject.GetComponent<ConnectionLine> ();
					line.SetUp (n, neighbor);

				}
			}
		}

		//		void DrawConnectionsOfEachNeuron () {
		//
		//			// Each connection made has a line render and a precent
		//
		//			/*	foreach (Neuron n in nodes) {
		//
		//
		//                    n.gameObject.AddComponent<ConnectionLine>();
		//                    n.gameObject.AddComponent<LineRenderer>();
		//
		//                    ConnectionLine line = n.gameObject.GetComponent<ConnectionLine>();
		//
		//                    Neuron randNeuron = nodes[Random.Range( 0, nodes.Count )];
		//
		//                    line.SetUp(n, randNeuron);
		//                }*/
		//
		//			foreach (Neuron n in neurons) {
		//
		//
		//				List<Neuron> neighbors = n.neighbors;
		//
		//
		//				foreach (Neuron neighbor in neighbors) {
		//
		//					n.gameObject.AddComponent<ConnectionLine> ();
		//					n.gameObject.AddComponent<LineRenderer> ();
		//
		//					ConnectionLine line = n.gameObject.GetComponent<ConnectionLine> ();
		//					line.SetUp (n, neighbor);
		//
		//				}
		//			}
		//		}

		public List<Neuron> getNodeInRange (Neuron nodeToCheck, float range) {

			List<Neuron> neighbors = new List<Neuron> ();

			foreach (Neuron n in neurons) {

				float dist = Vector3.Distance (nodeToCheck.gameObject.transform.position, n.gameObject.transform.position);

				if (dist < range) {

					neighbors.Add (n);
				}
			}

			// pick random close object


			return neighbors;
		}

	}
}
