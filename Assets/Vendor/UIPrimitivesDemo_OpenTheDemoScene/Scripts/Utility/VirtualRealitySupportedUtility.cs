
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

namespace VRB
{
	[ExecuteInEditMode]
	public class VirtualRealitySupportedUtility : MonoBehaviour
	{
		//readonly/const

		//Serialized
		
		/////Protected/////
		//References
		//Primitives
		
		//Actions

		///////////////////////////////////////////////////////////////////////////
		//
		// Inherited from MonoBehaviour
		//
		
		protected void Awake()	{

		}
		
		protected void Start () {
			
		}
		
		protected void Update () {
			
		}
		
		///////////////////////////////////////////////////////////////////////////
		//
		// VirtualRealitySupportedUtility Functions
		//

		[EditorButtonAttribute]
		protected void VRSupportOn()
		{
			PlayerSettings.virtualRealitySupported = true;
		}

		[EditorButtonAttribute]
		protected void VRSupportOff()
		{
			PlayerSettings.virtualRealitySupported = true;
		}

		
		////////////////////////////////////////
		//
		// Function Functions

	}
}
#endif
