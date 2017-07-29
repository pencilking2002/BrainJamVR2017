using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ITHB.Utility
{
	public class Utility_AnimatedSprite : MonoBehaviour
	{
		//readonly

		//Serialized
		public float updateInterval = .013f;
		public bool sharedMaterial;
		public bool pingPong;
		public bool playOnAwake=true;
		public bool loop=true;
		public string propertyName = "_MainTex";
		
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
		protected int propertyID;
		protected bool isPlaying;
		
		

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

			propertyID = Shader.PropertyToID(propertyName);
			offsetInterval = material.GetTextureScale(propertyName);
			totalIntervals = (int) (1f/offsetInterval.x);
			resetIndex = totalIntervals*totalIntervals;

			if(playOnAwake)
				Play();
		}
		
		protected void Start ()
		{
		}
		
		protected void Update ()
		{
			if(isPlaying)
				UpdateSprite();
			
		}
		
		///////////////////////////////////////////////////////////////////////////
		//
		// Utility_AnimatedSprite Functions
		//

		protected void UpdateSprite() {

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
					if(!loop) {
						isPlaying = false;
						return;
					}
				}
				if (this.myIndex <= 0 && direction < 0) {
					if(this.pingPong)
					{
						this.direction = 1;
					}
					if(!loop) {
						isPlaying = false;
						return;
					}
				}

				OnIndexUpdated();

				this.lastUpdateTime = Time.time;
			}
		}

		protected void OnIndexUpdated() {

			int x = 0;
			int y = 0;

			if(this.myIndex!=0)
			{
				x = this.myIndex%(totalIntervals);
				y = this.myIndex/(totalIntervals);
			}

			int totalYIntervals = (int) (1f/offsetInterval.y);

			SetTextureOffset(Vector2.Scale(offsetInterval,new Vector2(x,totalYIntervals- (y+1))));
		}


		[EditorButtonAttribute]
		public void Play() {

			GetComponent<MeshRenderer>().enabled = true;
			
			isPlaying = true;
			lastUpdateTime = Time.time - updateInterval;
			this.myIndex = 0;
		}

		protected void SetTextureOffset(Vector2 offset) {
			this.material.SetTextureOffset(propertyName, offset);
		}

		public void SetPercent(float percent) {
			
			this.myIndex = (int) (percent*((float) (this.resetIndex-1)));
			OnIndexUpdated();
		}
	}
}