using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Neuromancers
{
public class BrainController : MonoBehaviour {

	public List<Neuron> nodes = new List<Neuron>();

	void SetUp(){

		GameObject go = Instantiate(Resources.Load("Neuron")) as GameObject;
		nodes.Add( go.AddComponent<Neuron>();
		


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
		singleton.TryInit ();
		singleton = this;

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
