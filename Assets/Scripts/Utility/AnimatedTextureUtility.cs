using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Neuromancers
{
	public class AnimatedTextureUtility : MonoBehaviour
	{
		//readonly

		//Serialized
		public Texture2D[] frames;
		public bool isAnimating;
		
		/////Protected/////
		//References
		Material myMaterialGlow;
		//Primitives
		readonly float frameInterval = .07f;
		float lastFrameTime;
		int myFrameIndex = 5;


		///////////////////////////////////////////////////////////////////////////
		//
		// Inherited from MonoBehaviour
		//
		
		protected void Awake () {

		}

		protected void Start () {
			myMaterialGlow = GetComponent<Renderer> ().material;
			
		}

		protected void Update () {

			if (isAnimating && (Time.time - lastFrameTime > frameInterval)) {
				myMaterialGlow.mainTexture = frames [myFrameIndex];
				myFrameIndex++;
				if (myFrameIndex == frames.Length)
					myFrameIndex = 6;
				lastFrameTime = Time.time;
			}
		}

		///////////////////////////////////////////////////////////////////////////
		//
		// AnimatedTextureUtility Functions
		//

		
		////////////////////////////////////////
		//
		// Function Functions

		public void StartAnimating () {
			
			isAnimating = true;
		}

		public void StopAnimating () {
			
			isAnimating = false;
			myMaterialGlow.mainTexture = null;
		}

	}
}
