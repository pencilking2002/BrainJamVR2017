using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Neuromancers {
//	[ExecuteInEditMode]
	public class NeuronRenderer : MonoBehaviour {
		
		//readonly
		protected readonly float MAX_EMISSION_INTENSITY = 1f;
		protected readonly float MAX_RIM_INTENSITY = 2.82f;

		//Serialized
		[SerializeField]
		protected Transform glowTransform;
		[SerializeField]
		[Range (0, 1f)]
		protected float energyLevel = .5f;
		[SerializeField]
		protected Renderer sphereRenderer;
		[SerializeField]
		protected Color unclickedRimColor;
		[SerializeField]
		protected Color unclickedEmissionColor;

		/////Protected/////
		//References
		protected CanvasGroup canvasGroup;
		protected Material sphereMaterial;
		//Primitives
		protected Vector3 startLocalScale;
		protected Color startEmissionColor;
		protected Color startRimColor;

		///////////////////////////////////////////////////////////////////////////
		//
		// Inherited from MonoBehaviour
		//
		
		protected void Awake () {

			sphereMaterial = sphereRenderer.material;
			startEmissionColor = sphereMaterial.GetColor("_Color");
			startRimColor = sphereMaterial.GetColor("_RimColor");
			sphereMaterial.SetColor("_Color",unclickedEmissionColor);
			sphereMaterial.SetColor("_RimColor",unclickedRimColor);

			canvasGroup = GetComponent<CanvasGroup>();
			startLocalScale = this.transform.localScale;
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

			this.energyLevel = Mathf.Clamp(newEnergyLevel,.3f,1f);

			if(energyLevel==1f) {


				sphereMaterial.SetColor("_Color",startEmissionColor);
				sphereMaterial.SetColor("_RimColor",startRimColor);
			}
		}

		protected void UpdateAppearance() {

//			this.energyLevel = 1f;
//			UpdateGlowScale ();

			Vector3 newScale =  startLocalScale*(1f + energyLevel*1f);
			this.transform.localScale = newScale;

			sphereMaterial.SetFloat("_EmissionIntensity",MAX_EMISSION_INTENSITY*energyLevel);
			sphereMaterial.SetFloat("_RimIntensity",MAX_RIM_INTENSITY*energyLevel);
			sphereMaterial.SetFloat("_VertexOffsetIntensity",energyLevel);
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