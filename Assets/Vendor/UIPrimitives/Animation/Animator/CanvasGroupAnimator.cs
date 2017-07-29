using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UIPrimitives;

namespace UIPrimitives
{
	[DisallowMultipleComponent]
	[RequireComponent (typeof(CanvasGroup))]
	public class CanvasGroupAnimator : UIAnimator
	{
		protected CanvasGroup myCanvasGroup;

		//Queues Of Animations
		public Queue<UIAnimation_Base> myCanvasGroupAnimationsAlpha;

		//Properties
		public CanvasGroup MyCanvasGroup { get {return this.myCanvasGroup; } } 

		///////////////////////////////////////////////////////////////////////////
		//
		// Inherited from MonoBehaviour
		//

		protected override void Awake () {

			base.Awake ();

			myCanvasGroup = GetComponent<CanvasGroup>();

			myCanvasGroupAnimationsAlpha = new Queue<UIAnimation_Base> ();
			myUIAnimations.Add (myCanvasGroupAnimationsAlpha);
		}

		protected void Update () {	

			UpdateAnimations ();
		}

		///////////////////////////////////////////////////////////////////////////
		//
		// CanvasGroupAnimator Functions
		//

		public void StartAnimation (CanvasGroupAnimation.AnimationType animationType, UIAnimation_Base.TweenType tweenType, float startValue, float endValue, float deltaValue, float duration, AnimationCurve easeCurve, UIAnimation_Base.LoopType loopType, int loopCount)
		{
			switch (animationType) {
			case CanvasGroupAnimation.AnimationType.Alpha:
				AddAlphaAnimation (tweenType, null, startValue, endValue, deltaValue, duration, easeCurve, loopType, loopCount);
				break;
			}
		}

		////////////////////////////////////////
		//
		// CanvasGroupAnimator Functions

		protected void AddAlphaAnimation (UIAnimation_Base.TweenType tweenType, Action onCompleteAction, float startFillPercent, float endFillPercent, float deltaFillPercent, float duration, AnimationCurve easeCurve, UIAnimation_Base.LoopType loopType, int loopCount)
		{
			CanvasGroupAnimation_Alpha newCanvasGroupAnimation_Alpha = new CanvasGroupAnimation_Alpha (this, onCompleteAction, tweenType, startFillPercent, endFillPercent, deltaFillPercent, duration, easeCurve, loopType, loopCount);
			myCanvasGroupAnimationsAlpha.Enqueue (newCanvasGroupAnimation_Alpha);
		}

		//////////////////////////////
		// CanvasGroupAnimator Start/End/Relative Functions

		public void AddAlphaStartAnimation (Action onCompleteAction, float startFillPercent, float endFillPercent, float duration, UIAnimationUtility.EaseType easeType, int loopCount=0, UIAnimation_Base.LoopType loopType=UIAnimation_Base.LoopType.PingPong)
		{
			AddAlphaAnimation (UIAnimation_Base.TweenType.Start, onCompleteAction, startFillPercent, endFillPercent, 0, duration, UIAnimationUtility.GetCurve (easeType), loopType, loopCount);
		}

		public void AddAlphaEndAnimation (Action onCompleteAction, float endFillPercent, float duration, UIAnimationUtility.EaseType easeType, int loopCount=0, UIAnimation_Base.LoopType loopType=UIAnimation_Base.LoopType.PingPong)
		{
			AddAlphaAnimation (UIAnimation_Base.TweenType.End, onCompleteAction, 0, endFillPercent, 0, duration, UIAnimationUtility.GetCurve (easeType), loopType, loopCount);
		}

		public void AddAlphaRelativeAnimation (Action onCompleteAction, float deltaFillPercent, float duration, UIAnimationUtility.EaseType easeType, int loopCount=0, UIAnimation_Base.LoopType loopType=UIAnimation_Base.LoopType.PingPong)
		{
			AddAlphaAnimation (UIAnimation_Base.TweenType.Relative, onCompleteAction, 0, 0, deltaFillPercent, duration, UIAnimationUtility.GetCurve (easeType), loopType, loopCount);
		}


		//////////////////////////////
		// Alpha Value Get/Set

		public void SetAlphaValue (float value) {

			myCanvasGroup.alpha = value;
			
//			myCanvasGroup.GetComponentInChildren<Canvas>().enabled = (value>0);
		}

		public float GetAlphaValue () {

			return myCanvasGroup.alpha;
		}

	}

	public class CanvasGroupAnimation_Alpha : UIAnimation_Float
	{
		protected CanvasGroupAnimator canvasGroupAnimator;

		public CanvasGroupAnimation_Alpha (CanvasGroupAnimator materialAnimator, Action onCompleteAction, UIAnimation_Base.TweenType tweenType, float startValue, float endValue, float deltaValue, float duration, AnimationCurve easeCurve, LoopType loopType, int loopCount)
			: base (onCompleteAction, tweenType, startValue, endValue, deltaValue, duration, easeCurve, loopType, loopCount)
		{
			this.canvasGroupAnimator = materialAnimator;
		}

		public override void StartAnimation () {
			SetStartValues (canvasGroupAnimator.GetAlphaValue ());
		}

		public override void UpdateAnimation () {
			currentValue = GetEasedValue ();
			canvasGroupAnimator.SetAlphaValue (currentValue);
		}

		public override UIAnimation_Base GetNewAnimation () {
			NewValues newValues = GetNewValues ();
			CanvasGroupAnimation_Alpha newAnimation = new CanvasGroupAnimation_Alpha (canvasGroupAnimator, onCompleteAction, tweenType,  newValues.newStartValue, newValues.newEndValue, newValues.newDeltaValue, newValues.newDuration, easeCurve, loopType, loopCount);
			return newAnimation;
		}

		public override bool updateBefore {
			get {
				return true;
			}
		}
	}
}