using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Neuromancers
{
public class BrainController : MonoBehaviour {

	public List<Neuron> nodes = new List<Neuron>();

	private GameObject nodeGo;

	void SetUp(){

			nodeGo = new GameObject();
			nodeGo.name = "| Neurons |";

			GameObject go;
		
			//float offSetX = 3;
			//float offSetY = 3;
		
			// Place nodes
			for( int i = 0; i < 200 ; i++){

				go = Instantiate(Resources.Load("Neuron")) as GameObject;

				Neuron node = go.GetComponent<Neuron>();
				nodes.Add(node);

				go.transform.position = nodeRandomPlacement();//new Vector3(i * offSetX, 0, 0 );

				go.transform.parent = nodeGo.transform;
			}


			DrawConnections();

	}

	Vector3 nodeRandomPlacement()
	{
		Vector3 centerVec = new Vector3( 0,0,0);
		
		float rangeX = 10;
		float rangeY = 10;
		float rangeZ = 10;

		Vector3 randomPos = new Vector3(
				Random.Range(rangeX * -1, rangeX),
				Random.Range(rangeY * -1, rangeY), 
				Random.Range(rangeZ * -1, rangeZ));

		randomPos += centerVec; // add center point offset

		return randomPos;
	}

	
	void DrawConnections(){

			// Each connection made has a line render and a precent 

		/*	foreach (Neuron n in nodes) {

	
				n.gameObject.AddComponent<ConnectionLine>();
				n.gameObject.AddComponent<LineRenderer>();

				ConnectionLine line = n.gameObject.GetComponent<ConnectionLine>();

				Neuron randNeuron = nodes[Random.Range( 0, nodes.Count )];

				line.SetUp(n, randNeuron);
			}*/

			foreach (Neuron n in nodes) {

				List<Neuron> neighbors = getNodeInRange(n, 5f);

			

				foreach (Neuron neighbor in neighbors) {

					n.gameObject.AddComponent<ConnectionLine>();
					n.gameObject.AddComponent<LineRenderer>();

					ConnectionLine line = n.gameObject.GetComponent<ConnectionLine>();
					line.SetUp(n, neighbor);

				}
			}
	}

	public List<Neuron> getNodeInRange( Neuron nodeToCheck, float range){

			List<Neuron> neighbors = new List<Neuron>();

			foreach (Neuron n in nodes) {

				float dist = Vector3.Distance(nodeToCheck.gameObject.transform.position, n.gameObject.transform.position);

				if(dist < range){

					neighbors.Add(n);
				}
			}

			// pick random close object


			return neighbors;
	}



	/*
	 * START SINGLETON
	 */
	private static BrainController singleton;

	public static BrainController getSingleton ()
	{
		if (singleton == null) {

			GameObject go = new GameObject ("| Brain |");
			singleton = go.AddComponent<BrainController> ();
			singleton.TryInit ();
		}

		return singleton;
	}

	void Start ()
	{
			singleton = this;

			singleton.TryInit ();
		
		/*if (singleton != this) {
			Debug.LogError ("Brain " + gameObject.name, gameObject);
			Destroy (this);
		}*/

		SetUp();
	}

	void TryInit ()
	{
		DontDestroyOnLoad (gameObject);
		singleton = this;
	}
	/*
	 * END SINGLETON
	 */


	}
}
