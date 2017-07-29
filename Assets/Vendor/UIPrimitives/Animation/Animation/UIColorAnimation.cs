using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UIPrimitives;

namespace UIPrimitives
{
	[RequireComponent (typeof(UIColorAnimator))]
	public class UIColorAnimation : UIAnimation
	{
		//enum
		public enum ColorAnimationType
		{
			Color
		}
		
		//Animation type
		public ColorAnimationType animationType = ColorAnimationType.Color;
		
		//My animator
		protected UIColorAnimator myUIColorAnimator;
		//My animated component
		protected Graphic myGraphic;
		
		/////Value/////
		public Color startColor = Color.white;
		public Color endColor = Color.white;
		public Color deltaColor = Color.white;
		
		///////////////////////////////////////////////////////////////////////////
		//
		// Inherited from MonoBehaviour
		//

		protected void Awake ()
		{
			myUIColorAnimator = GetComponent<UIColorAnimator> ();
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
			switch(animationType){
			case ColorAnimationType.Color:
				myUIColorAnimator.SetColorValue(startColor);
				break;
			}
		}

		/// <summary>
		/// Starts the animation.
		/// </summary>
		public override void StartAnimation ()
		{
			myUIColorAnimator.StartAnimation (animationType,null, tweenType,  startColor, endColor, deltaColor, duration, easeCurve, loopType, loopCount);
		}

		public override void StartAnimationReversed ()
		{
			myUIColorAnimator.StartAnimation (animationType,null, tweenType,  endColor, startColor, deltaColor, duration, easeCurve, loopType, loopCount);
		}
		
		///////////////////////////////////////////////////////////////////////////
		//
		// UIColorAnimation Functions
		//

		/// <summary>
		/// Resets the colors.
		/// </summary>
		public void ResetColors ()
		{
			myGraphic = GetComponent<Graphic> ();
			startColor = myGraphic.color;
			endColor = myGraphic.color;
			if (tweenType == UIAnimation_Base.TweenType.Start)
				startColor.a = 0;
			else
				endColor.a = 0;
		}
	}
}



