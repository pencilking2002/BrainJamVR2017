using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Neuromancers
{
public class BrainController : MonoBehaviour {

	public List<Neuron> nodes = new List<Neuron>();

	void SetUp(){

			GameObject go;
		
			float offSetX = 10;
			float offSetY = 10;
		
			// Place nodes
			for( int i = 0; i < 10 ; i++){

				go = Instantiate(Resources.Load("Neuron")) as GameObject;

				Neuron node = go.AddComponent<Neuron>();
				nodes.Add(node);

				go.transform.position = new Vector3(i * offSetX, 0, 0 );
			}


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
