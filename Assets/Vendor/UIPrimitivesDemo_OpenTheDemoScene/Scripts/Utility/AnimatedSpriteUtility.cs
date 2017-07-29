using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace VRB.Utility
{
	public class AnimatedSpriteUtility : MonoBehaviour
	{
		//readonly

		//Serialized
		public float updateInterval = .013f;
		public bool sharedMaterial;
		public bool pingPong;
		
		/////Protected/////
		//References
		protected Material material;
		//Primitives
		protected Vector2 offsetInterval;
		protected int totalIntervals;
		protected float lastUpdateTime;
		protected int resetIndex;
		protected int direction = 1;
		public int myIndex = 0;
		
		

		///////////////////////////////////////////////////////////////////////////
		//
		// Inherited from MonoBehaviour
		//
		
		protected void Awake()
		{
			if(sharedMaterial)
			material = GetComponent<MeshRenderer>().sharedMaterial;
			else
			material = GetComponent<MeshRenderer>().material;

			offsetInterval = material.GetTextureScale("_MainTex");
			totalIntervals = (int) (1f/offsetInterval.x);
			resetIndex = totalIntervals*totalIntervals;
		}
		
		protected void Start ()
		{

		}
		
		protected void Update ()
		{
			
			if (Time.time - lastUpdateTime > updateInterval) {

				float timeElapsed = Time.time - this.lastUpdateTime;
				
				this.myIndex += direction*((int) (timeElapsed/updateInterval));
				
				if (this.myIndex >= this.resetIndex && direction > 0) {
					if(this.pingPong)
					{
						this.direction = -1;
					}
					else
					{
						this.myIndex = 0;
					}
				}
				if (this.myIndex <= 0 && direction < 0) {
					if(this.pingPong)
					{
						this.direction = 1;
					}
				}

				int x = 0;
				int y = 0;

				if(this.myIndex!=0)
				{
					x = this.myIndex%(totalIntervals);
					y = this.myIndex/(totalIntervals);
				}

				SetTextureOffset(Vector2.Scale(offsetInterval,new Vector2(x,-y)));

				this.lastUpdateTime = Time.time;
			}
			
		}
		
		///////////////////////////////////////////////////////////////////////////
		//
		// Utility_AnimatedSprite Functions
		//

		protected void SetTextureOffset(Vector2 offset) {
			this.material.SetTextureOffset("_MainTex", offset);
		}
	}
}