using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Neuromancers {
	
	public class NeuronManager : MonoBehaviour {

		//readonly 
		protected readonly int NEURON_COUNT = 70;
		protected readonly float MAX_CONNECTION_RANGE = 3f;
		protected readonly float MIN_CONNECTION_STRENGTH = -1f;
		protected readonly float MAX_CONNECTION_STRENGTH = 1f;

		//serialized
		public List<Neuron> neurons = new List<Neuron> ();

		//protected
		protected GameObject neuronPrefab;
		protected GameObject connectionPrefab;

		///////////////////////////////////////////////////////////////////////////
		//
		// Inherited from MonoBehaviour
		//

		protected void Awake () {

			neuronPrefab = Resources.Load ("Prefabs/Neuron") as GameObject;
			connectionPrefab = Resources.Load ("Prefabs/Connection") as GameObject;
		}

		protected void Start () {

			CreateNeurons();
			ConnectNeurons();
		}

		protected void Update () {

		}

		///////////////////////////////////////////////////////////////////////////
		//
		// NeuronManager Functions
		//

		protected void CreateNeurons () {
			

		
			// Place nodes
			for (int i = 0; i < NEURON_COUNT; i++) {

				GameObject newNeuronGO = Instantiate (neuronPrefab) as GameObject;

				Neuron node = newNeuronGO.GetComponent<Neuron> ();
				neurons.Add (node);

				newNeuronGO.transform.position = GetPositionInNightSky(); //GetRandomNeuronPosition ();
				newNeuronGO.transform.parent = this.transform;
			}

		}

		protected void ConnectNeurons() {

			for (int i = 0; i < neurons.Count; ++i) {

				for(int j=0; j < neurons.Count; ++j) {

					//don't want to check against itself
					if(j==i)
						continue;

					Neuron sourceNeuron = neurons[i];
					Neuron destinationNeuron = neurons[j];


					float distance = Vector3.Distance (sourceNeuron.gameObject.transform.position, destinationNeuron.gameObject.transform.position);

					if (distance < MAX_CONNECTION_RANGE) {

						Connection newConnection = CreateConnection(sourceNeuron,destinationNeuron);
						sourceNeuron.AddConnection (newConnection);
					}
				}
			}
		}

		protected Connection CreateConnection(Neuron sourceNeuron, Neuron destinationNeuron) {

			float connectionStrength = Random.Range(MIN_CONNECTION_STRENGTH,MAX_CONNECTION_STRENGTH);

			GameObject newConnectionGO = Instantiate (connectionPrefab) as GameObject;
			newConnectionGO.transform.SetParent(sourceNeuron.transform);
			newConnectionGO.transform.Recenter();

			Connection newConnection = newConnectionGO.GetComponent<Connection>();
			newConnection.SetData(connectionStrength,destinationNeuron);


			return newConnection;
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

		Vector3 GetPositionInNightSky(){

			Debug.Log("Get Night Position " );

			bool goodDist = false;
			bool goodVerticalAngle = false;

			float radiusMin = 6;
			float radiusMax = 10;
		

			Vector3 centerVec = new Vector3 (0, 0, 0);
			Vector3 pnt = Vector3.zero;

			int conditions = 0;

			while(conditions <= 1)
			{
				conditions = 0;
			
				pnt = Random.insideUnitSphere * radiusMax;
		
				goodVerticalAngle = false;

				float angle;
				angle =	CalculateAngle(pnt, centerVec);
				if(150 > angle && angle > 70)
				{
					

					conditions++;
				}

				/*
				float hAngle;
				hAngle = CalculateAngleHorizon(pnt, centerVec);
				if(180 > hAngle && hAngle > 90)
				{


					conditions++;
				}*/


			
				float distFromCenter = Vector3.Distance(pnt, centerVec);

				if(distFromCenter > radiusMin)
				{
					conditions++;
				}
			}

			return pnt;
		}

				
		public static float CalculateAngle(Vector3 from, Vector3 to)
		{
			return Quaternion.FromToRotation(Vector3.up, to - from).eulerAngles.z;
		}

		public static float CalculateAngleHorizon(Vector3 from, Vector3 to)
		{
			return Quaternion.FromToRotation(Vector3.left, to - from).eulerAngles.z;
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
