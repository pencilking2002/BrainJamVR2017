using UnityEngine;
using System.Collections;

namespace ITHB.Utility
{
	public class Utility_FloatingTransform : MonoBehaviour
	{
		//References
		Transform myTransform;
		Transform myMeshTransform;
		public enum PodState
		{
			Idle,
			Path
		}
		PodState myPodState = PodState.Idle;
		// Use this for initialization
		void Awake ()
		{
			myTransform = transform;
		}
		
		void Start ()
		{
			DIRECTION_PIVOT_POINT = transform.position;
			
			switch (myPodState) {
			case PodState.Idle:
				StartIdleMovement ();
				break;
			}
		}
		
		// Update is called once per frame
		void Update ()
		{
			
			switch (myPodState) {
			case PodState.Idle:
				UpdateIdleMovement ();
				break;
			}
		}
		//////////////// MOVEMENT  ////////////////
		// Direction Change //
		readonly float DIRECTION_CHANGE_TIME_MIN = 4f;
		readonly float DIRECTION_CHANGE_TIME_MAX = 6f;
		Vector3 DIRECTION_PIVOT_POINT = new Vector3 (1.08f, 2.67f, 1.1f);
		float directionChangeTime;
		AnimationCurve directionCurve;
		AnimationCurve easeInEaseOut;
		// Velocity //
		readonly float VELOCITY_MAX = .1f;
		Vector3 direction;
		//////////////// ROTATION  ////////////////
		Vector3 spinDirection;
		float spinSpeed = 1f;
		
		void StartIdleMovement ()
		{
			SetIdleDirection ();
		}
		
		void SetIdleDirection ()
		{
			direction = (DIRECTION_PIVOT_POINT - myTransform.position).normalized + Utility.GetRandomVector3 (1f);
			directionChangeTime = Random.Range (DIRECTION_CHANGE_TIME_MIN, DIRECTION_CHANGE_TIME_MAX);
			directionCurve = AnimationCurve.EaseInOut (Time.time, 0, Time.time + directionChangeTime, 0);
			directionCurve.AddKey (Time.time + directionChangeTime * Random.Range (.3f, .7f), 1f);
			Invoke ("SetIdleDirection", directionChangeTime);
			spinDirection = new Vector3 (Random.Range (-1f, 1f), Random.Range (-1f, 1f), 0);
		}
		
		void UpdateIdleMovement ()
		{
			//position
			myTransform.position += direction * VELOCITY_MAX * (1f / directionChangeTime) * Time.deltaTime * directionCurve.Evaluate (Time.time);
			//rotate
			myTransform.Rotate (spinSpeed * Time.deltaTime * spinDirection * directionCurve.Evaluate (Time.time), Space.World);
		}
		
	}
}
