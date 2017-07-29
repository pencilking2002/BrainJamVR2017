using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UIPrimitives;

namespace UIPrimitives
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(UILine))]
	public class UILineAnimator : UIAnimator
	{
		//MyUILine
		protected UILine myUILine;

		//Queues Of Animations
		public Queue<UIAnimation_Base> myUIAnimationsFillPercent;
		
		///////////////////////////////////////////////////////////////////////////
		//
		// Inherited from MonoBehaviour
		//

		protected override void Awake ()
		{
			base.Awake();
			
			myUILine = GetComponent<UILine> ();

			myUIAnimationsFillPercent = new Queue<UIAnimation_Base> ();
			myUIAnimations.Add(myUIAnimationsFillPercent);
		}

		protected void Update ()
		{
			bool changeGeometry = UpdateAnimations();
		
			if (changeGeometry) {
				myUILine.SetAllDirty_Public ();
			}
		}
		
		///////////////////////////////////////////////////////////////////////////
		//
		// UILineAnimator Functions
		//

		public void StartAnimation (UILineAnimation.LineAnimationType animationType, UIAnimation_Base.TweenType tweenType, float startValue, float endValue, float deltaValue, float duration, AnimationCurve easeCurve, UIAnimation_Base.LoopType loopType, int loopCount)
		{
			switch (animationType) {
			case UILineAnimation.LineAnimationType.FillPercent:
				AddFillPercentAnimation (tweenType, null, startValue, endValue, deltaValue, duration, easeCurve, loopType, loopCount);
				break;
			}
		}

		////////////////////////////////////////
		//
		// FillPercent Functions
		#region FillPercentFunctions
		protected void AddFillPercentAnimation (UIAnimation_Base.TweenType tweenType, Action onCompleteAction, float startFillPercent, float endFillPercent, float deltaFillPercent, float duration, AnimationCurve easeCurve, UIAnimation_Base.LoopType loopType, int loopCount)
		{
			UILineAnimation_FillPercent newUIAnimationFillPercent = new UILineAnimation_FillPercent (this, onCompleteAction, tweenType, startFillPercent, endFillPercent, deltaFillPercent, duration, easeCurve, loopType, loopCount);
			myUIAnimationsFillPercent.Enqueue (newUIAnimationFillPercent);
		}
		
		//////////////////////////////
		// FillPercent Start/End/Relative Functions
		
		public void AddFillPercentStartAnimation (Action onCompleteAction, float startFillPercent, float endFillPercent, float duration, UIAnimationUtility.EaseType easeType, int loopCount=0, UIAnimation_Base.LoopType loopType=UIAnimation_Base.LoopType.PingPong)
		{
			AddFillPercentAnimation (UIAnimation_Base.TweenType.Start, onCompleteAction, startFillPercent, endFillPercent, 0, duration, UIAnimationUtility.GetCurve (easeType), loopType, loopCount);
		}
		
		public void AddFillPercentEndAnimation (Action onCompleteAction, float endFillPercent, float duration, UIAnimationUtility.EaseType easeType, int loopCount=0, UIAnimation_Base.LoopType loopType=UIAnimation_Base.LoopType.PingPong)
		{
			AddFillPercentAnimation (UIAnimation_Base.TweenType.End, onCompleteAction, 0, endFillPercent, 0, duration, UIAnimationUtility.GetCurve (easeType), loopType, loopCount);
		}
		
		public void AddFillPercentRelativeAnimation (Action onCompleteAction, float deltaFillPercent, float duration, UIAnimationUtility.EaseType easeType, int loopCount=0, UIAnimation_Base.LoopType loopType=UIAnimation_Base.LoopType.PingPong)
		{
			AddFillPercentAnimation (UIAnimation_Base.TweenType.Relative, onCompleteAction, 0, 0, deltaFillPercent, duration, UIAnimationUtility.GetCurve (easeType), loopType, loopCount);
		}
		
		//////////////////////////////
		// FillPercent Value Get/Set

		public float GetFillPercentValue(){
			return myUILine.fillPercent;
		}

		public void SetFillPercentValue(float value){
			myUILine.fillPercent=value;
		}
		#endregion
	}
	
	///////////////////////////////////////////////////////////////////////////
	//
	// UILineAnimation Classes
	//
	
	/// <summary>
	/// Base class for UILineAnimations with Float values
	/// </summary>
	public abstract class UILineAnimation_Float : UIAnimation_Float
	{
		protected UILineAnimator myUILineAnimator;
		public  UILine myUILine;
		
		public UILineAnimation_Float (UILineAnimator myUILineAnimator, Action onCompleteAction, TweenType tweenType, float startValue, float endValue, float deltaValue, float duration, AnimationCurve easeCurve, LoopType loopType, int loopCount)
			: base(onCompleteAction, tweenType, startValue, endValue, deltaValue, duration, easeCurve, loopType, loopCount)
		{
			this.myUILineAnimator = myUILineAnimator;
		}
		
	}
	/// <summary>
	/// UILineAnimation class for FillPercent animations
	/// </summary>
	public class UILineAnimation_FillPercent : UILineAnimation_Float
	{
		public float  currentValue;
		public float newFillPercent;
		
		public UILineAnimation_FillPercent (UILineAnimator myUILineAnimator, Action onCompleteAction, TweenType tweenType, float startFillPercent, float endFillPercent, float deltaFillPercent, float duration, AnimationCurve easeCurve, LoopType loopType, int loopCount) 
			: base(myUILineAnimator, onCompleteAction, tweenType, startFillPercent, endFillPercent, deltaFillPercent, duration, easeCurve, loopType, loopCount)
		{
		}
		
		public override void StartAnimation ()
		{
			SetStartValues (myUILineAnimator.GetFillPercentValue ());
		}
		
		public override void UpdateAnimation ()
		{
			currentValue = Mathf.Clamp01(GetEasedValue());
			myUILineAnimator.SetFillPercentValue (currentValue);
		}
		
		public override UIAnimation_Base GetNewAnimation ()
		{
			NewValues newValues = GetNewValues ();
			UILineAnimation_FillPercent newAnimation = new UILineAnimation_FillPercent (myUILineAnimator, onCompleteAction, tweenType, newValues.newStartValue, newValues.newEndValue, newValues.newDeltaValue, newValues.newDuration, easeCurve, loopType, loopCount);
			return newAnimation;
		}
		
		public override bool updateBefore {
			get {
				return false;
			}
		}
	}
}