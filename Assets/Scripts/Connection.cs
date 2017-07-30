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
		protected ConnectionType myConnectionType;
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
		public ConnectionType MyConnectionType {
			get {
				return this.myConnectionType;
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

		public void SetData (float strength, Neuron destinationNeuron, bool shouldAnimateConnection, ConnectionType connectionType) {

			if(connectionType == ConnectionType.Unknown) {

				connectionType = (UnityEngine.Random.value < .5f) ? ConnectionType.Excitory : ConnectionType.Inhibitory;
			}
			this.myConnectionType = connectionType;

			if(myConnectionType == ConnectionType.Excitory) 
				strength = UnityEngine.Random.Range(.5f,1f);
			else 
				strength = UnityEngine.Random.Range(-1f,-.5f);
			
			this.strength = strength;
			Color color = this.strengthToColorGradient.Evaluate((strength+1f)/2f);
			lineRenderer.startColor = color.Alpha(lineRenderer.startColor.a);
			lineRenderer.endColor = color.Alpha(lineRenderer.endColor.a);

			this.destinationNeuron = destinationNeuron;
//			Debug.Log(lineRenderer.gameObject.name);
//			Debug.Log(destinationNeuron.transform.position);
			lineRenderer.SetPosition(0, Utility.Utility.GetRandomVector3(.05f));

			Vector3 lineRendererEndPosition = transform.parent.InverseTransformPoint(destinationNeuron.transform.position);
			if(shouldAnimateConnection)
				StartCoroutine(AnimateLineEndPosition(lineRendererEndPosition));
			else
				lineRenderer.SetPosition(1,lineRendererEndPosition);
		}

		protected IEnumerator AnimateLineEndPosition(Vector3 position) {

			float duration = 2f;
			float startTime = Time.time;
			float percent = 0;
			AnimationCurve curve = UIPrimitives.UIAnimationUtility.GetCurve(UIPrimitives.UIAnimationUtility.EaseType.easeOutSine);

			while (percent<1f) {

				float easedPercent = curve.Evaluate(percent);
				Vector3 currentPosition = Vector3.Lerp(Vector3.zero,position,easedPercent);
				lineRenderer.SetPosition(1,currentPosition);

				percent = (Time.time - startTime)/duration;
				yield return null;
			}
		}

		public Vector3 GetWorldStartPosition() {

			return transform.TransformPoint(lineRenderer.GetPosition(0));
		}

		public Color GetColor() {

			Color color = this.strengthToColorGradient.Evaluate((strength+1f)/2f);
			return color;
		}

		
		////////////////////////////////////////
		//
		// Function Functions

	}
}