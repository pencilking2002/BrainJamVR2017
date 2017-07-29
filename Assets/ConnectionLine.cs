using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionLine : MonoBehaviour {

	public Neuron self;
	public Neuron other;

	List<Transform> pathNodes;// = new List<Transform>();

	public float precent = 0;

	LineRenderer lr;

	// Use this for initialization
	void Awake () {
		
	}

	public ConnectionLine SetUp(Neuron setSelf, Neuron setOther ){

		lr  = GetComponent<LineRenderer>();
		lr.startWidth = 0.1f;
		lr.endWidth = 0.1f;

		self = setSelf;
		other = setOther;
		precent = 1;


		pathNodes = new List<Transform>();

		pathNodes.Add(self.gameObject.transform);
		pathNodes.Add(other.gameObject.transform);

		lr.positionCount = pathNodes.Count;

		for (int i = 0; i < pathNodes.Count; i++){
			lr.SetPosition(i, pathNodes[i].transform.position);
		}


		return this;
	}

	
	// Update is called once per frame
	void Update () {

		if(lr){

			for (int i = 0; i < pathNodes.Count; i++){
				lr.SetPosition(i, pathNodes[i].transform.position);
			}
		}

	}
}
