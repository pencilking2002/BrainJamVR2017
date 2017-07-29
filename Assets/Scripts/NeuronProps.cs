using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NeuronProps {
	
	private float _strength = 0;
	public float strength {
		get 
		{
			return _strength;
		}

		set
		{
			_strength = Mathf.Clamp (value, -1, 1);
		}
	}

	public Transform targetNeuron;

}
