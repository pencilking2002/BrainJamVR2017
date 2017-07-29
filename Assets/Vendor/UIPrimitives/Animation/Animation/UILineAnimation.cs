using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UIPrimitives;

namespace UIPrimitives
{
	[RequireComponent (typeof(UILineAnimator))]
	public class UILineAnimation : UIAnimation
	{
		//enum
		public enum LineAnimationType
		{
			FillPercent
		}

		//Animation type
		public LineAnimationType animationType = LineAnimationType.FillPercent;
		
		//My animator
		protected UILineAnimator myUILineAnimator;
		//My animated component
		protected UILine myUILine;
		
		/////Value/////
		//float
		public float startValue, endValue = 1f, deltaValue = 1f;
			
		///////////////////////////////////////////////////////////////////////////
		//
		// Inherited from MonoBehaviour
		//

		protected void Awake ()
		{
			myUILineAnimator = GetComponent<UILineAnimator> ();
		}

		protected override void Start ()
		{
			base.Start();
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
			myUILine=GetComponent<UILine>();

			switch (animationType) {
			case  LineAnimationType.FillPercent:
				myUILine.SetFillPercent(startValue);
				break;
			}
		}
		
		/// <summary>
		/// Starts the animation.
		/// </summary>
		public override void StartAnimation ()
		{
			myUILineAnimator.StartAnimation (animationType, tweenType, startValue, endValue, deltaValue, duration, easeCurve, loopType, loopCount);
		}
		/// <summary>
		/// Starts the animation reversed.
		/// </summary>
		public override void StartAnimationReversed ()
		{
			myUILineAnimator.StartAnimation (animationType, tweenType, endValue, startValue, -deltaValue, duration, easeCurve, loopType, loopCount);
		}
	}
}


	