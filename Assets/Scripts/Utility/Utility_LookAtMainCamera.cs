using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Neuromancers.Utility
{
	[ExecuteInEditMode]
	public class Utility_LookAtMainCamera : MonoBehaviour
	{
		//readonly

		//Serialized
		[SerializeField]
		protected bool onlyRotateYAxis;
		
		/////Protected/////
		//References
		protected Transform mainCameraTransform;
		//Primitives
		

		///////////////////////////////////////////////////////////////////////////
		//
		// Inherited from MonoBehaviour
		//
		
		protected void Awake()
		{
			this.mainCameraTransform = Camera.main.transform;
		}
		
		protected void Start ()
		{
			
		}
		
		protected void Update ()
		{
			Vector3 direction = this.transform.position - this.mainCameraTransform.position;

			if(transform.parent!=null) {
				Vector3 localDirection = transform.parent.InverseTransformDirection(direction);

				if(onlyRotateYAxis)
					localDirection.y = 0;

				this.transform.localRotation = Quaternion.LookRotation(localDirection);
			}else {
				if(onlyRotateYAxis)
					direction.y = 0;
				
				this.transform.rotation = Quaternion.LookRotation(direction);
			}
		}
		
		///////////////////////////////////////////////////////////////////////////
		//
		// Utility_LookAtMainCamera Functions
		//

		
		////////////////////////////////////////
		//
		// Function Functions

	}
}
