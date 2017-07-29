using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//namespace App
//{
	public class Utility_LightColor : MonoBehaviour
	{
		//readonly

		//Serialized
		[SerializeField]
		protected float speed=1f;
		[SerializeField]
		protected float saturation=1f;
		[SerializeField]
		protected float value=1f;
		[SerializeField]
		protected float minHue;
		[SerializeField]
		protected float maxHue=1f;
//	[SerializeField]
//	protected float hueOffset;

	[SerializeField]
	protected float intensityMultiplier=1f;
		
		/////Protected/////
		//References
		protected Light light;
		//Primitives
		

		///////////////////////////////////////////////////////////////////////////
		//
		// Inherited from MonoBehaviour
		//
		
		protected void Awake()
		{
			light = GetComponent<Light>();
		}
		
		protected void Start ()
		{
			
		}
		
		protected void Update ()
		{
		float hue = (Mathf.PingPong(Time.time*speed,maxHue-minHue)+minHue)%1f;
			light.color = Color.HSVToRGB(	hue,saturation,value);

		light.intensity += (UnityEngine.Random.value-.5f)*intensityMultiplier;

		}
		
		///////////////////////////////////////////////////////////////////////////
		//
		// Utility_LightColor Functions
		//

		
		////////////////////////////////////////
		//
		// Function Functions

	}
//}
