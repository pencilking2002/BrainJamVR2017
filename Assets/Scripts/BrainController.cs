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
