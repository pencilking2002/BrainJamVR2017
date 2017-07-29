using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Neuromancers {
//	[ExecuteInEditMode]
	public class NeuronRenderer : MonoBehaviour {
		
		//readonly
		protected readonly float MAX_RIM_INTENSITY = 2.82f;

		//Serialized
		[SerializeField]
		protected Transform glowTransform;
		[SerializeField]
		[Range (0, 1f)]
		protected float energyLevel = .5f;
		[SerializeField]
		protected Renderer sphereRenderer;

		/////Protected/////
		//References
		protected CanvasGroup canvasGroup;
		protected Material sphereMaterial;
		//Primitives
		

		///////////////////////////////////////////////////////////////////////////
		//
		// Inherited from MonoBehaviour
		//
		
		protected void Awake () {

			sphereMaterial = sphereRenderer.material;
			canvasGroup = GetComponent<CanvasGroup>();
		}

		protected void Start () {
			
		}

		protected void Update () {


			UpdateAppearance();
		}

		///////////////////////////////////////////////////////////////////////////
		//
		// NeuronRenderer Functions
		//

		public void SetEnergyLevel(float newEnergyLevel) {

			this.energyLevel = Mathf.Clamp(newEnergyLevel,.03f,1f);
		}

		protected void UpdateAppearance() {

//			this.energyLevel = 1f;
			UpdateGlowScale ();

			sphereMaterial.SetFloat("_RimIntensity",MAX_RIM_INTENSITY*energyLevel);
			canvasGroup.alpha = energyLevel;
		}

		protected void UpdateGlowScale () {

			float scale = Mathf.Abs (Mathf.Sin (Time.time * .5f));
			scale = 0.9f + 0.2f * scale;
			glowTransform.localScale = Vector3.one * scale;
		}
		
		////////////////////////////////////////
		//
		// Function Functions

	}
}