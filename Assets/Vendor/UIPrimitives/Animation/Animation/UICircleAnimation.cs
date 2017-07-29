using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UIPrimitives;

namespace UIPrimitives
{
	[RequireComponent (typeof(UICircleAnimator))]
	public class UICircleAnimation : UIAnimation
	{
		//enum
		public enum CircleAnimationType
		{
			Radius,
			Angle,
			AngleOffset,
			Thickness,
			Repetitions
		}
		
		//Animation type
		public UICircleAnimation.CircleAnimationType animationType = UICircleAnimation.CircleAnimationType.AngleOffset;
		
		//My animator
		protected UICircleAnimator myUICircleAnimator;
		//My animated component
		protected UICircle myUICircle;
		
		/////Value/////
		public float startValue = 0f, endValue = 360f, deltaValue = 360f;

		
		///////////////////////////////////////////////////////////////////////////
		//
		// Inherited from MonoBehaviour
		//

		protected void Awake ()
		{
			myUICircleAnimator = GetComponent<UICircleAnimator> ();
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
		/// Starts the animation.
		/// </summary>
		public override void StartAnimation ()
		{
			myUICircleAnimator.StartAnimation (animationType, tweenType, startValue, endValue, deltaValue, duration, easeCurve, loopType, loopCount);
		}	
		
		public override void StartAnimationReversed ()
		{
			myUICircleAnimator.StartAnimation (animationType, tweenType, endValue, startValue, -deltaValue, duration, easeCurve, loopType, loopCount);
		}

		/// <summary>
		/// Sets the start values.
		/// </summary>
		public override void SetStartValues ()
		{
			switch (animationType) {
			case UICircleAnimation.CircleAnimationType.Radius:
				myUICircleAnimator.SetRadiusValue (startValue);
				break;
				
			case UICircleAnimation.CircleAnimationType.Angle:
				myUICircleAnimator.SetAngleValue ((int)startValue);
				break;
				
			case UICircleAnimation.CircleAnimationType.AngleOffset:
				myUICircleAnimator.SetAngleOffsetValue ((int)startValue);
				break;
				
			case UICircleAnimation.CircleAnimationType.Thickness:
				myUICircleAnimator.SetThicknessValue (startValue);
				break;
				
			case UICircleAnimation.CircleAnimationType.Repetitions:
				myUICircleAnimator.SetRepetitionsValue ((int)startValue);
				break;	
			}
		}
	}
}



