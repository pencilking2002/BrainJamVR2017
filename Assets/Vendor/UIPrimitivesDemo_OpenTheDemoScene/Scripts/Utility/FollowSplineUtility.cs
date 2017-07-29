using UnityEngine;
using System;
using System.Collections;
using VRB.Utility;

namespace ITHB.Utility
{
	public class FollowSplineUtility : MonoBehaviour
	{
		public enum MovementState
		{
			Idle,
			Moving
		}


		///Serialized
		public string pathName;
		[Header("Speed")]
		public int smoothMultiplier = 20;
		public float updateInterval = .013f;
		[Header("Timing")]
		public bool playOnStart=true;
		public float startDelay;
		public bool setValueOnStart;
		[Header("Loop")]
		public bool closedLoop;
		public bool autoLoop=true;
		[Header("Up Vector")]
		public bool shouldRotate = true;
		public bool overrideUpVector;
		public Vector3 upVector;
		public bool lerpUpVector;
		public Vector3 endUpVector = new Vector3(.3f,.6f,.7f);
		public float endUpVectorPercent = 1f;
		[Header("Debug")]
		public bool showSlider=true;
		public int myIndex = 0;
		public MovementState myMovementState = MovementState.Moving;
		public int lookOffset = 5;

		/////Protected
		//References
		public static Action<float> pathPercentUpdated;
		public Action pathCompleted;
		//Primitives
		protected bool movementPaused;
		protected int pathLength;
		protected int resetIndex;
		protected float lastUpdateTime;
		protected Vector3 myPosition;
		protected Vector3 startUpVector;
		//public
		public Vector3[] smoothedPath { get; set; }

		///////////////////////////////////////////////////////////////////////////
		//
		// Inherited from MonoBehaviour
		//

		protected void Start ()
		{
			PathUtility path = PathUtility.GetPath(pathName);

			if(this.lerpUpVector)
				this.startUpVector = this.upVector;

			if(this.pathName!=null)
			{
				this.lastUpdateTime=Time.time;
				SetPath (path);

				if(playOnStart)
				{
					if(startDelay==0){
						StartMoving ();
					}
					else
					{
						Invoke("StartMoving",startDelay);
					}
					
					UpdatePosition();
				}
				if(!playOnStart&&setValueOnStart)
					UpdatePosition();

			}
			else
				Debug.Log("No Path Found");
				
		}
//		protected Vector3 endUpVector = new Vector3(.3f,.6f,.7f);

		protected void Update ()
		{

		
			switch (myMovementState) {
			case MovementState.Idle:
				
				break;
			case MovementState.Moving:
				float timeElapsed = Time.time - this.lastUpdateTime;
				if (timeElapsed > updateInterval && !this.movementPaused) {
					int increment = (int) (timeElapsed/updateInterval);

					UpdatePosition (increment);
					lastUpdateTime = Time.time;
				}
				break;
			}
		}

		protected void OnGUI()
		{
			if(this.showSlider && this.myMovementState == MovementState.Moving)
			{
				float oldPercent= GetPathPercent();
				float newPercent = GUI.HorizontalSlider(new Rect(25, 15, 400, 30), oldPercent, 0.0F, 1.0F);

				GUI.Label(new Rect(25, 15+30, 400, 30), newPercent.ToString());


				if (oldPercent != newPercent )
					SetPathPercent(newPercent);
			}
		}

		public void OnIncrement()
		{
			int increment = GetSkipAmount();
			UpdatePosition(increment,true);
		}

		public void OnDecrement()
		{
			int increment = -GetSkipAmount();
			UpdatePosition(increment,true);
		}

		///////////////////////////////////////////////////////////////////////////
		//
		// Utility_FollowSpline Functions
		//
		
		
		public void StartMoving ()
		{
			this.lastUpdateTime		= Time.time;
			this.myPosition 		= this.transform.position;
			this.myMovementState 	= MovementState.Moving;
			
			this.UpdatePosition();
			
		}
		
		public void StopMoving ()
		{
			this.myMovementState	= MovementState.Idle;
		}

		public void PauseMovement()
		{
			this.movementPaused = true;
		}

		public void ResumeMovement()
		{
			this.movementPaused = false;
			this.lastUpdateTime = Time.time;
		}

		public void Reset()
		{
			this.myIndex=0;
			this.Start();
		}

		public void SetPathNode(int index, Vector3 value)
		{
			PathUtility path = PathUtility.GetPath(pathName);
			path.Nodes[index] = value;
			SetPath(path);
		}

		protected void TogglePause()
		{
			this.movementPaused = !this.movementPaused;

			if(!this.movementPaused)
			{
				this.lastUpdateTime = Time.time;
			}
		}

		protected void SetPath (PathUtility path)
		{
			Vector3[] vector3s = VRB.Utility.Utility.PathControlPointGenerator (path.Nodes);
			int SmoothAmount = vector3s.Length * smoothMultiplier;
			this.pathLength = SmoothAmount;
			if (this.closedLoop) {
				this.resetIndex = this.pathLength;
			} else {
				this.resetIndex = this.pathLength - this.lookOffset;
			}

			this.smoothedPath = new Vector3[SmoothAmount];

			for (int i = 0; i < SmoothAmount; i++) {
				float pm = (float)i / SmoothAmount;
				Vector3 currPt = VRB.Utility.Utility.Interp (vector3s, pm);
				this.smoothedPath [i] = currPt;
			}
		}


		protected void UpdatePosition (int increment = 0, bool catchOutOfRange = false)
		{
			this.myIndex+=increment;
			if(catchOutOfRange && this.myIndex >= this.resetIndex)
				this.myIndex = this.resetIndex-1;

			float percent = GetPathPercent();

			//Calculate up vector
			if(this.lerpUpVector)
			{
				float upVectorPercent = percent/endUpVectorPercent;
				upVectorPercent = Mathf.Clamp01(upVectorPercent);

				this.upVector = Vector3.Lerp(startUpVector,endUpVector,upVectorPercent);
			}



			if(pathPercentUpdated!=null)
				pathPercentUpdated(percent);

			if (this.myIndex >= this.resetIndex) {
				if(this.autoLoop)
				{
					this.myIndex = 0;
				}
				else
				{
					if(this.pathCompleted!=null)
						this.pathCompleted();

					this.myMovementState = MovementState.Idle;
					return;
				}
			}

			this.transform.position = smoothedPath [myIndex];

			int lookAtIndex = this.myIndex + this.lookOffset;

			if (this.closedLoop)
				lookAtIndex %= pathLength;

			if (this.shouldRotate)
				this.transform.rotation = GetRotation(myIndex,lookAtIndex);
		}

		public Quaternion GetRotation(int startIndex, int endIndex)
		{
			Vector3 forward = this.smoothedPath [endIndex]-this.smoothedPath[startIndex];

			if(this.overrideUpVector)
				return Quaternion.LookRotation(forward,this.upVector);
			else
				return Quaternion.LookRotation (forward);
		}
		
		public float GetPathPercent()
		{			
			if(!this.isActiveAndEnabled)
				return 0;

			return ((float) this.myIndex)/((float) this.resetIndex);
		}

		public void SetPathPercent(float percent){
			this.myIndex = (int) (percent*this.resetIndex);
		}

		protected int GetSkipAmount()
		{
			return (int) (resetIndex*.05f*GetSkipMultiplier());
		}

		protected float GetSkipMultiplier()
		{
			float multiplier = 1f;
			return multiplier;
		}
	}
}
