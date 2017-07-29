using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UIPrimitives;

namespace UIPrimitives
{
	[RequireComponent (typeof(UITransformAnimator))]
	public class UITransformAnimation : UIAnimation
	{
		//enum
		public enum TransformAnimationType
		{
			Position,
			Rotation,
			Scale
		}

		//structs 

		//Animation type
		public TransformAnimationType animationType = TransformAnimationType.Position;
		
		//My animator
		protected UITransformAnimator myUITransformAnimator;
		//My animated component
		protected Transform myTransform;
		
		/////Value/////
		//Position
		public Vector3 startPosition = Vector3.zero;
		public Vector3 endPosition = Vector3.zero;
		public Vector3 deltaPosition = Vector3.zero;
		//Rotation
		public Vector3 startRotation = Vector3.zero;
		public Vector3 endRotation = Vector3.zero;
		public Vector3 deltaRotation = Vector3.zero;
		//Scale
		public Vector3 startScale = Vector3.one;
		public Vector3 endScale = Vector3.one;
		public Vector3 deltaScale = Vector3.one;
		//Lock
		public bool lockX = false;
		public bool lockY = false;
		public bool lockZ = false;

		///////////////////////////////////////////////////////////////////////////
		//
		// Inherited from MonoBehaviour
		//

		protected void Awake ()
		{
			this.myUITransformAnimator = GetComponent<UITransformAnimator> ();
		}
			
		protected override void Start ()
		{
			base.Start ();
		}

		///////////////////////////////////////////////////////////////////////////
		//
		// Inherited from UIAnimation
		//
		
		/// <summary>
		/// Sets the start values.
		/// </summary>
		public override void SetStartValues ()
		{
			myTransform = GetComponent<Transform> ();
			switch (animationType) {
			case TransformAnimationType.Position:
				myUITransformAnimator.SetPositionValue(startPosition);
				break;
			case TransformAnimationType.Rotation:
				myUITransformAnimator.SetRotationValue(Quaternion.Euler(startRotation));
				break;
			case TransformAnimationType.Scale:
				myUITransformAnimator.SetScaleValue(startScale);
				break;
			}
		}

		/// <summary>
		/// Starts the animation depending on the animationType.
		/// </summary>
		public override void StartAnimation ()
		{
			StartAnimation(null);
		}
		
		
		public override void StartAnimationReversed ()
		{
			switch (animationType) {
			case TransformAnimationType.Position:
				myUITransformAnimator.StartAnimation (animationType,null, tweenType, endPosition,  startPosition, deltaPosition, duration, easeCurve, loopType, loopCount);
				break;
			case TransformAnimationType.Rotation:
				myUITransformAnimator.StartAnimation (animationType,null, tweenType, Quaternion.Euler(endRotation), Quaternion.Euler(startRotation), Quaternion.Euler(deltaRotation), duration,  easeCurve, loopType, loopCount);
				break;
			case TransformAnimationType.Scale:
				myUITransformAnimator.StartAnimation (animationType,null, tweenType, endScale, startScale, deltaScale, duration, easeCurve, loopType, loopCount);
				break;
			}
		}

		
		///////////////////////////////////////////////////////////////////////////
		//
		// UITransformAnimation Functions
		//
		
		
		public void StartAnimation(Action<float> updateCallback,Action completeCallback=null)
		{
			switch (animationType) {
			case TransformAnimationType.Position:
				myUITransformAnimator.StartAnimation (animationType,completeCallback, tweenType,  startPosition, endPosition, deltaPosition, duration, easeCurve, loopType, loopCount, updateCallback);
				break;
			case TransformAnimationType.Rotation:
				myUITransformAnimator.StartAnimation (animationType,completeCallback, tweenType, Quaternion.Euler(startRotation), Quaternion.Euler(endRotation), Quaternion.Euler(deltaRotation), duration,  easeCurve, loopType, loopCount, updateCallback);
				break;
			case TransformAnimationType.Scale:
				myUITransformAnimator.StartAnimation (animationType,completeCallback, tweenType, startScale, endScale, deltaScale, duration, easeCurve, loopType, loopCount, updateCallback);
				break;
				
			}
		}

		public void ResetValues ()
		{
			myTransform = GetComponent<Transform> ();
			endPosition = myTransform.localPosition;
			endRotation = myTransform.localRotation.eulerAngles;
			endScale = myTransform.localScale;
			startPosition = LockVector (endPosition);
			startRotation = LockVector (endRotation);
			startScale = LockVector (endScale);
		}

		/// <summary>
		/// Locks the vector when you want to prevent it from being reset.
		/// </summary>
		/// <returns>The vector.</returns>
		/// <param name="vector">Vector.</param>
		Vector3 LockVector (Vector3 vector)
		{
			if (!lockX)
				vector.x = 0;
			if (!lockY)
				vector.y = 0;
			if (!lockZ)
				vector.z = 0;
			return vector;
		}

		public void GetStartValues ()
		{
			startPosition = transform.localPosition;
			startRotation = transform.localRotation.eulerAngles;
			startScale = transform.localScale;
		}
	}
}



