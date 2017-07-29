using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Neuromancers {
	public class Connection : MonoBehaviour {
		//readonly

		//Serialized
		[SerializeField]
		protected Gradient strengthToColorGradient;

		/////Protected/////
		//References
		protected Neuron destinationNeuron;
		protected LineRenderer lineRenderer;
		//Primitives
		protected float strength;


		//Properties
		public float Strength {
			get {
				return this.strength;
			}
		}

		public Neuron DestinationNeuron {
			get {
				return this.destinationNeuron;
			}
		}


		///////////////////////////////////////////////////////////////////////////
		//
		// Inherited from MonoBehaviour
		//
		
		protected void Awake () {

			lineRenderer = GetComponentInChildren<LineRenderer>();
		}

		protected void Start () {

		}

		protected void Update () {
			
		}

		///////////////////////////////////////////////////////////////////////////
		//
		// Connection Functions
		//

		public void SetData (float strength, Neuron destinationNeuron) {

			this.strength = strength;
			Color color = this.strengthToColorGradient.Evaluate((strength+1f)/2f);
			lineRenderer.startColor = color.Alpha(lineRenderer.startColor.a);
			lineRenderer.endColor = color.Alpha(lineRenderer.endColor.a);

			this.destinationNeuron = destinationNeuron;
//			Debug.Log(lineRenderer.gameObject.name);
//			Debug.Log(destinationNeuron.transform.position);
			lineRenderer.SetPosition(1,transform.parent.InverseTransformPoint(destinationNeuron.transform.position));
		}

		
		////////////////////////////////////////
		//
		// Function Functions

	}
}