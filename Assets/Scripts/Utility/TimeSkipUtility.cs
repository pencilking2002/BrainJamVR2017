using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Neuromancers {
	public class TimeSkipUtility : MonoBehaviour {
		//readonly

		//Serialized
		public float time = 1f;
		public bool skipOnStart = false;

		
		/////Protected/////
		//References
		//Primitives
		

		///////////////////////////////////////////////////////////////////////////
		//
		// Inherited from MonoBehaviour
		//
		
		protected void Awake () {

		}

		protected void Start () {
			
			if(skipOnStart)
				Skip();
		}

		protected void Update () {
			
		}

		///////////////////////////////////////////////////////////////////////////
		//
		// TimeSkipUtility Functions
		//

		[EditorButtonAttribute]
		protected void Skip () {

			StartCoroutine(SkipCoroutine());
		}

		protected IEnumerator SkipCoroutine() {

			float oldTimeScale = Time.timeScale;

			Time.timeScale = Mathf.Clamp(time,1f,30f);

			yield return new WaitForSeconds(time);

			Time.timeScale = oldTimeScale;
		}
		
		////////////////////////////////////////
		//
		// Function Functions

	}
}
