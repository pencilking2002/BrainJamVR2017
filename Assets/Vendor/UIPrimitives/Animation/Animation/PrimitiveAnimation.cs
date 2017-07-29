using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace UIPrimitives
{
	public abstract class PrimitiveAnimation : MonoBehaviour
	{
		//Serialized
		
		/////Protected/////
		//References
		//Primitives

		/////Timing/////
		//Duration
		[UnityEngine.Serialization.FormerlySerializedAs("animationDuration")]//in UIText
		public float duration = 2f;


		//StartDelay
		public float startDelay = 0;
	}
}