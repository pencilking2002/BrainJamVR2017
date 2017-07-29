using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Neuromancers.Utility
{
	[ExecuteInEditMode]
	public class Utility_SpriteScale : MonoBehaviour
	{
		//readonly

		//Serialized
		public float updateInterval = .013f;
		public Transform anchor;
		
		/////Protected/////
		//References
		protected Material material;
		//Primitives
		protected Vector2 offsetInterval;
		protected int totalIntervals;
		protected int resetIndex;
		public int myIndex = 0;
		
		

		///////////////////////////////////////////////////////////////////////////
		//
		// Inherited from MonoBehaviour
		//
		
		protected void Awake()
		{
			Init();
		}
		
		protected void Start ()
		{
			
		}
		
		protected void Update ()
		{
			if(anchor==null)
				return;
				
			this.myIndex = (int) ((1f-anchor.rotation.eulerAngles.y/360f)*resetIndex);

				int x = 0;
				int y = 0;

				if(this.myIndex!=0)
				{
					x = this.myIndex%(totalIntervals);
					y = this.myIndex/(totalIntervals);
				}

				SetTextureOffset(Vector2.Scale(offsetInterval,new Vector2(x,-y)));

			
		}

		protected void OnEnable()
		{
			Init();
		}
		
		///////////////////////////////////////////////////////////////////////////
		//
		// Utility_AnimatedSprite Functions
		//

		protected void Init()
		{
			material = GetComponentInChildren<MeshRenderer>().material;
			offsetInterval = material.GetTextureScale("_MainTex");
			totalIntervals = (int) (1f/offsetInterval.x);
			resetIndex = totalIntervals*totalIntervals;
		}

		protected void SetTextureOffset(Vector2 offset) {
			material.SetTextureOffset("_MainTex", offset);
		}
		
		////////////////////////////////////////
		//
		// Function Functions

	}
}
