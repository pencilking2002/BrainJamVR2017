using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UIPrimitives;

namespace UIPrimitives
{
	[RequireComponent (typeof(CanvasGroupAnimator))]
	public class CanvasGroupAnimation : UIAnimation
	{
		//enum
		public enum AnimationType
		{
			Alpha
		}

		
		//Animation type
		public AnimationType animationType = AnimationType.Alpha;
		
		//My animator
		protected CanvasGroupAnimator canvasGroupAnimator;
		protected Renderer myRenderer;

		/////Value/////
		//float
		public float startValue, endValue = 1f, deltaValue = 1f;
		
		///////////////////////////////////////////////////////////////////////////
		//
		// Inherited from MonoBehaviour
		//

		protected void Awake ()
		{
			canvasGroupAnimator = GetComponent<CanvasGroupAnimator> ();
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
		public override void SetStartValues(){
			
			canvasGroupAnimator=GetComponent<CanvasGroupAnimator>();

			switch(animationType){
			case AnimationType.Alpha:
				canvasGroupAnimator.SetAlphaValue(startValue);
				break;
			}
		}

		/// <summary>
		/// Starts the animation.
		/// </summary>
		public override void StartAnimation ()
		{
			canvasGroupAnimator.StartAnimation (animationType, tweenType, startValue, endValue, deltaValue, duration, easeCurve, loopType, loopCount);
		}

		public override void StartAnimationReversed ()
		{
			canvasGroupAnimator.StartAnimation (animationType, tweenType, endValue, startValue, -deltaValue, duration, easeCurve, loopType, loopCount);
		}

	}
}



