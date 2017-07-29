using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace UIPrimitives
{
	public class UICanvas : MonoBehaviour
	{
		//readonly

		//Serialized
		
		/////Protected/////
		//References
		//Primitives
		

		///////////////////////////////////////////////////////////////////////////
		//
		// Inherited from MonoBehaviour
		//
		
		protected virtual void Awake()
		{

		}
		
		protected virtual void Start ()
		{
			
		}
		
		protected virtual void Update ()
		{
			
		}
		
		///////////////////////////////////////////////////////////////////////////
		//
		// UICanvas Functions
		//

		public void SetVisible(bool value)
		{
			this.gameObject.SetActive(value);
		}
		
		////////////////////////////////////////
		//
		// Function Functions

	}
}