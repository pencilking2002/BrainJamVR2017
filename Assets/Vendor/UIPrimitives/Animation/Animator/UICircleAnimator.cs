using UnityEngine;
using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UIPrimitives;

namespace UIPrimitives
{
	[DisallowMultipleComponent]
	[RequireComponent (typeof(UICircle))]
	public class UICircleAnimator : UIAnimator
	{
		//Queues Of Animations
		public Queue<UIAnimation_Base> myUIAnimationsRadius;
		public Queue<UIAnimation_Base> myUIAnimationsAngle;
		public Queue<UIAnimation_Base> myUIAnimationsAngleOffset;
		public Queue<UIAnimation_Base> myUIAnimationsThickness;
		public Queue<UIAnimation_Base> myUIAnimationsRepetitions;

		//References
		private UICircle myUICircle;
		
		///////////////////////////////////////////////////////////////////////////
		//
		// Inherited from MonoBehaviour
		//

		protected void Awake ()
		{
			myUICircle = GetComponent<UICircle> ();
		}

		protected void Start ()
		{
			myUIAnimations = new List<Queue<UIAnimation_Base>> ();
			
			myUIAnimationsRadius = new Queue<UIAnimation_Base> ();
			myUIAnimations.Add (myUIAnimationsRadius);

			myUIAnimationsAngle = new Queue<UIAnimation_Base> ();
			myUIAnimations.Add (myUIAnimationsAngle);

			myUIAnimationsAngleOffset = new Queue<UIAnimation_Base> ();
			myUIAnimations.Add (myUIAnimationsAngleOffset);

			myUIAnimationsThickness = new Queue<UIAnimation_Base> ();
			myUIAnimations.Add (myUIAnimationsThickness);

			myUIAnimationsRepetitions = new Queue<UIAnimation_Base> ();
			myUIAnimations.Add (myUIAnimationsRepetitions);
		}

		protected void Update ()
		{	
			bool updateGeometry=UpdateAnimations();
			//If we changed geometry we need to update the mesh of our graphic
			if (updateGeometry) {
				myUICircle.SetAllDirty_Public ();
			}
		}

		///////////////////////////////////////////////////////////////////////////
		//
		// UICircleAnimator Functions
		//

		public void StartAnimation (UICircleAnimation.CircleAnimationType animationType, UIAnimation_Base.TweenType tweenType, float startValue, float endValue, float deltaValue, float duration, AnimationCurve easeCurve, UIAnimation_Base.LoopType loopType, int loopCount)
		{
			switch (animationType) {
			case UICircleAnimation.CircleAnimationType.Radius:
				AddRadiusAnimation (tweenType, null, startValue, endValue, deltaValue, duration, easeCurve, loopType, loopCount);
				break;
				
			case UICircleAnimation.CircleAnimationType.Angle:
				AddAngleAnimation (tweenType, null, startValue, endValue, deltaValue, duration, easeCurve, loopType, loopCount);
				break;
				
			case UICircleAnimation.CircleAnimationType.AngleOffset:
				AddAngleOffsetAnimation (tweenType, null, startValue, endValue, deltaValue, duration, easeCurve, loopType, loopCount);
				break;
				
			case UICircleAnimation.CircleAnimationType.Thickness:
				AddThicknessAnimation (tweenType, null, startValue, endValue, deltaValue, duration, easeCurve, loopType, loopCount);
				break;
				
			case UICircleAnimation.CircleAnimationType.Repetitions:
				AddRepetitionsAnimation (tweenType, null, startValue, endValue, deltaValue, duration, easeCurve, loopType, loopCount);
				break;	
			}
		}

		////////////////////////////////////////
		//
		// Radius Functions
		#region RadiusFunctions
		protected void AddRadiusAnimation (UIAnimation_Base.TweenType tweenType, Action onCompleteAction, float startRadius, float endRadius, float deltaRadius, float duration, AnimationCurve easeCurve, UIAnimation_Base.LoopType loopType, int loopCount)
		{
			UICircleAnimation_Radius newUIAnimationRadius = new UICircleAnimation_Radius (this, onCompleteAction, tweenType, startRadius, endRadius, deltaRadius, duration, easeCurve, loopType, loopCount);
			myUIAnimationsRadius.Enqueue (newUIAnimationRadius);
		}

		//////////////////////////////
		// Radius Start/End/Relative Functions
		
		public void AddRadiusStartAnimation (Action onCompleteAction, float startRadius, float endRadius, float duration, UIAnimationUtility.EaseType easeType, int loopCount=0, UIAnimation_Base.LoopType loopType=UIAnimation_Base.LoopType.PingPong)
		{
			AddRadiusAnimation (UIAnimation_Base.TweenType.Start, onCompleteAction, startRadius, endRadius, 0, duration, UIAnimationUtility.GetCurve (easeType), loopType, loopCount);
		}
		
		public void AddRadiusEndAnimation (Action onCompleteAction, float endRadius, float duration, UIAnimationUtility.EaseType easeType, int loopCount=0, UIAnimation_Base.LoopType loopType=UIAnimation_Base.LoopType.PingPong)
		{
			AddRadiusAnimation (UIAnimation_Base.TweenType.End, onCompleteAction, 0, endRadius, 0, duration, UIAnimationUtility.GetCurve (easeType), loopType, loopCount);
		}
		
		public void AddRadiusRelativeAnimation (Action onCompleteAction, float deltaRadius, float duration, UIAnimationUtility.EaseType easeType, int loopCount=0, UIAnimation_Base.LoopType loopType=UIAnimation_Base.LoopType.PingPong)
		{
			AddRadiusAnimation (UIAnimation_Base.TweenType.Relative, onCompleteAction, 0, 0, deltaRadius, duration, UIAnimationUtility.GetCurve (easeType), loopType, loopCount);
		}
		
		//////////////////////////////
		// Radius Value Get/Set

		public float GetRadiusValue ()
		{
			return myUICircle.radius;
		}

		public void SetRadiusValue (float value)
		{
			myUICircle.radius = value;
		}
		#endregion

		////////////////////////////////////////
		//
		// Angle Functions
		#region AngleFunctions
		protected void AddAngleAnimation (UIAnimation_Base.TweenType tweenType, Action onCompleteAction, float startAngle, float endAngle, float deltaAngle, float duration, AnimationCurve easeCurve, UIAnimation_Base.LoopType loopType, int loopCount)
		{
			UICircleAnimation_Angle newUIAnimationAngle = new UICircleAnimation_Angle (this, onCompleteAction, tweenType, startAngle, endAngle, deltaAngle, duration, easeCurve, loopType, loopCount);
			myUIAnimationsAngle.Enqueue (newUIAnimationAngle);
		}
		
		//////////////////////////////
		// Angle Start/End/Relative Functions

		public void AddAngleStartAnimation (Action onCompleteAction, float startAngle, float endAngle, float duration, UIAnimationUtility.EaseType easeType, int loopCount=0, UIAnimation_Base.LoopType loopType=UIAnimation_Base.LoopType.PingPong)
		{
			AddAngleAnimation (UIAnimation_Base.TweenType.Start, onCompleteAction, startAngle, endAngle, 0, duration, UIAnimationUtility.GetCurve (easeType), loopType, loopCount);
		}
		
		public void AddAngleEndAnimation (Action onCompleteAction, float endAngle, float duration, UIAnimationUtility.EaseType easeType, int loopCount=0, UIAnimation_Base.LoopType loopType=UIAnimation_Base.LoopType.PingPong)
		{
			AddAngleAnimation (UIAnimation_Base.TweenType.End, onCompleteAction, 0, endAngle, 0, duration, UIAnimationUtility.GetCurve (easeType), loopType, loopCount);
		}
		
		public void AddAngleRelativeAnimation (Action onCompleteAction, float deltaAngle, float duration, UIAnimationUtility.EaseType easeType, int loopCount=0, UIAnimation_Base.LoopType loopType=UIAnimation_Base.LoopType.PingPong)
		{
			AddAngleAnimation (UIAnimation_Base.TweenType.Relative, onCompleteAction, 0, 0, deltaAngle, duration, UIAnimationUtility.GetCurve (easeType), loopType, loopCount);
		}
		
		//////////////////////////////
		// Angle Value Get/Set
		
		public int GetAngleValue ()
		{
			return myUICircle.angle;
		}
		
		public void SetAngleValue (int value)
		{
			myUICircle.angle = value;
		}
		#endregion

		////////////////////////////////////////
		//
		// AngleOffset Functions
		#region AngleOffsetFunctions
		protected void AddAngleOffsetAnimation (UIAnimation_Base.TweenType tweenType, Action onCompleteAction, float startAngleOffset, float endAngleOffset, float deltaAngleOffset, float duration, AnimationCurve easeCurve, UIAnimation_Base.LoopType loopType, int loopCount)
		{
			UICircleAnimation_AngleOffset newUIAnimationAngleOffset = new UICircleAnimation_AngleOffset (this, onCompleteAction, tweenType, startAngleOffset, endAngleOffset, deltaAngleOffset, duration, easeCurve, loopType, loopCount);
			myUIAnimationsAngleOffset.Enqueue (newUIAnimationAngleOffset);
		}
		
		//////////////////////////////
		// AngleOffset Start/End/Relative Functions
		
		public void AddAngleOffsetStartAnimation (Action onCompleteAction, float startAngleOffset, float endAngleOffset, float duration, UIAnimationUtility.EaseType easeType, int loopCount=0, UIAnimation_Base.LoopType loopType=UIAnimation_Base.LoopType.PingPong)
		{
			AddAngleOffsetAnimation (UIAnimation_Base.TweenType.Start, onCompleteAction, startAngleOffset, endAngleOffset, 0, duration, UIAnimationUtility.GetCurve (easeType), loopType, loopCount);
		}
		
		public void AddAngleOffsetEndAnimation (Action onCompleteAction, float endAngleOffset, float duration, UIAnimationUtility.EaseType easeType, int loopCount=0, UIAnimation_Base.LoopType loopType=UIAnimation_Base.LoopType.PingPong)
		{
			AddAngleOffsetAnimation (UIAnimation_Base.TweenType.End, onCompleteAction, 0, endAngleOffset, 0, duration, UIAnimationUtility.GetCurve (easeType), loopType, loopCount);
		}
		
		public void AddAngleOffsetRelativeAnimation (Action onCompleteAction, float deltaAngleOffset, float duration, UIAnimationUtility.EaseType easeType, int loopCount=0, UIAnimation_Base.LoopType loopType=UIAnimation_Base.LoopType.PingPong)
		{
			AddAngleOffsetAnimation (UIAnimation_Base.TweenType.Relative, onCompleteAction, 0, 0, deltaAngleOffset, duration, UIAnimationUtility.GetCurve (easeType), loopType, loopCount);
		}
		
		//////////////////////////////
		// AngleOffset Value Get/Set
		
		public int GetAngleOffsetValue ()
		{
			return myUICircle.angleOffset;
		}
		
		public void SetAngleOffsetValue (int value)
		{
			myUICircle.angleOffset = value;
		}
		#endregion

		////////////////////////////////////////
		//
		// Thickness Functions
		#region ThicknessFunctions
		protected void AddThicknessAnimation (UIAnimation_Base.TweenType tweenType, Action onCompleteAction, float startThickness, float endThickness, float deltaThickness, float duration, AnimationCurve easeCurve, UIAnimation_Base.LoopType loopType, int loopCount)
		{
			UICircleAnimation_Thickness newUIAnimationThickness = new UICircleAnimation_Thickness (this, onCompleteAction, tweenType, startThickness, endThickness, deltaThickness, duration, easeCurve, loopType, loopCount);
			myUIAnimationsThickness.Enqueue (newUIAnimationThickness);
		}
		
		//////////////////////////////
		// Thickness Start/End/Relative Functions
		
		public void AddThicknessStartAnimation (Action onCompleteAction, float startThickness, float endThickness, float duration, UIAnimationUtility.EaseType easeType, int loopCount=0, UIAnimation_Base.LoopType loopType=UIAnimation_Base.LoopType.PingPong)
		{
			AddThicknessAnimation (UIAnimation_Base.TweenType.Start, onCompleteAction, startThickness, endThickness, 0, duration, UIAnimationUtility.GetCurve (easeType), loopType, loopCount);
		}
		
		public void AddThicknessEndAnimation (Action onCompleteAction, float endThickness, float duration, UIAnimationUtility.EaseType easeType, int loopCount=0, UIAnimation_Base.LoopType loopType=UIAnimation_Base.LoopType.PingPong)
		{
			AddThicknessAnimation (UIAnimation_Base.TweenType.End, onCompleteAction, 0, endThickness, 0, duration, UIAnimationUtility.GetCurve (easeType), loopType, loopCount);
		}

		public void AddThicknessRelativeAnimation (Action onCompleteAction, float deltaThickness, float duration, UIAnimationUtility.EaseType easeType, int loopCount=0, UIAnimation_Base.LoopType loopType=UIAnimation_Base.LoopType.PingPong)
		{
			AddThicknessAnimation (UIAnimation_Base.TweenType.Relative, onCompleteAction, 0, 0, deltaThickness, duration, UIAnimationUtility.GetCurve (easeType), loopType, loopCount);
		}
		
		//////////////////////////////
		// Thickness Value Get/Set
		
		public float GetThicknessValue ()
		{
			return myUICircle.thickness;
		}
		
		public void SetThicknessValue (float value)
		{
			myUICircle.thickness = value;
		}
		#endregion

		////////////////////////////////////////
		//
		// Repetitions Functions
		#region RepetitionsFunctions
		protected void AddRepetitionsAnimation (UIAnimation_Base.TweenType tweenType, Action onCompleteAction, float startRepetitions, float endRepetitions, float deltaRepetitions, float duration, AnimationCurve easeCurve, UIAnimation_Base.LoopType loopType, int loopCount)
		{
			UICircleAnimation_Repetitions newUIAnimationRepetitions = new UICircleAnimation_Repetitions (this, onCompleteAction, tweenType, startRepetitions, endRepetitions, deltaRepetitions, duration, easeCurve, loopType, loopCount);
			myUIAnimationsRepetitions.Enqueue (newUIAnimationRepetitions);
		}
		
		//////////////////////////////
		// Repetitions Start/End/Relative Functions

		public void AddRepetitionsStartAnimation (Action onCompleteAction, float startRepetitions, float endRepetitions, float duration, UIAnimationUtility.EaseType easeType, int loopCount=0, UIAnimation_Base.LoopType loopType=UIAnimation_Base.LoopType.PingPong)
		{
			AddRepetitionsAnimation (UIAnimation_Base.TweenType.Start, onCompleteAction, startRepetitions, endRepetitions, 0, duration, UIAnimationUtility.GetCurve (easeType), loopType, loopCount);
		}
		
		public void AddRepetitionsEndAnimation (Action onCompleteAction, float endRepetitions, float duration, UIAnimationUtility.EaseType easeType, int loopCount=0, UIAnimation_Base.LoopType loopType=UIAnimation_Base.LoopType.PingPong)
		{
			AddRepetitionsAnimation (UIAnimation_Base.TweenType.End, onCompleteAction, 0, endRepetitions, 0, duration, UIAnimationUtility.GetCurve (easeType), loopType, loopCount);
		}

		public void AddRepetitionsRelativeAnimation (Action onCompleteAction, float deltaRepetitions, float duration, UIAnimationUtility.EaseType easeType, int loopCount=0, UIAnimation_Base.LoopType loopType=UIAnimation_Base.LoopType.PingPong)
		{
			AddRepetitionsAnimation (UIAnimation_Base.TweenType.Relative, onCompleteAction, 0, 0, deltaRepetitions, duration, UIAnimationUtility.GetCurve (easeType), loopType, loopCount);
		}
		
		//////////////////////////////
		// Repetitions Value Get/Set
		
		public int GetRepetitionsValue ()
		{
			return myUICircle.repetitions;
		}
		
		public void SetRepetitionsValue (int value)
		{
			myUICircle.repetitions = value;
		}

		#endregion

	}
	
	///////////////////////////////////////////////////////////////////////////
	//
	// UICircleAnimation Classes
	//
	
	/// <summary>
	/// Base class for UICircleAnimations
	/// </summary>
	public abstract class UICircleAnimation_Float : UIAnimation_Float
	{
		protected UICircleAnimator myUICircleAnimator;

		public UICircleAnimation_Float (UICircleAnimator myUICircleAnimator, Action onCompleteAction, TweenType tweenType, float startValue, float endValue, float deltaValue, float duration, AnimationCurve easeCurve, LoopType loopType, int loopCount)
			: base(onCompleteAction, tweenType, startValue, endValue, deltaValue, duration, easeCurve, loopType, loopCount)
		{
			this.myUICircleAnimator = myUICircleAnimator;
		}

	}
	/// <summary>
	/// UICircleAnimation class for Radius animations
	/// </summary>
	public class UICircleAnimation_Radius : UICircleAnimation_Float
	{
		public float  currentValue;

		public UICircleAnimation_Radius (UICircleAnimator myUICircleAnimator, Action onCompleteAction, TweenType tweenType, float startAngle, float endAngle, float deltaAngle, float duration, AnimationCurve easeCurve, LoopType loopType, int loopCount) 
		: base(myUICircleAnimator, onCompleteAction, tweenType, startAngle, endAngle, deltaAngle, duration, easeCurve, loopType, loopCount)
		{
		}
		
		public override void StartAnimation ()
		{
			SetStartValues (myUICircleAnimator.GetRadiusValue ());
		}
		
		public override void UpdateAnimation ()
		{
			currentValue = GetEasedValue();
			myUICircleAnimator.SetRadiusValue (currentValue);
		}
		
		public override UIAnimation_Base GetNewAnimation ()
		{
			NewValues newValues = GetNewValues ();
			UICircleAnimation_Radius newAnimation = new UICircleAnimation_Radius (myUICircleAnimator, onCompleteAction, tweenType, newValues.newStartValue, newValues.newEndValue, newValues.newDeltaValue, newValues.newDuration, easeCurve, loopType, loopCount);
			return newAnimation;
		}
		
		public override bool updateBefore {
			get {
				return false;
			}
		}
	}
	
	/// <summary>
	/// UICircleAnimation class for Angle animations
	/// </summary>
	public class UICircleAnimation_Angle : UICircleAnimation_Float
	{	
		public int currentValue;

		public UICircleAnimation_Angle (UICircleAnimator myUICircleAnimator, Action onCompleteAction, TweenType tweenType, float startAngle, float endAngle, float deltaAngle, float duration, AnimationCurve easeCurve, LoopType loopType, int loopCount) 
		: base(myUICircleAnimator, onCompleteAction, tweenType, startAngle, endAngle, deltaAngle, duration, easeCurve, loopType, loopCount)
		{
		}
		
		public override void StartAnimation ()
		{
			SetStartValues (myUICircleAnimator.GetAngleValue ());
		}
		
		public override void UpdateAnimation ()
		{
			currentValue = (int)GetEasedValue();
			int adjustedAngle = currentValue;
			if (adjustedAngle > 360)
				adjustedAngle -= 360;
			if (adjustedAngle < 0)
				adjustedAngle = 0;
			myUICircleAnimator.SetAngleValue (adjustedAngle);
		}
	
		public override UIAnimation_Base GetNewAnimation ()
		{
			NewValues newValues = GetNewValues ();
			UICircleAnimation_Angle newAnimation = new UICircleAnimation_Angle (myUICircleAnimator, onCompleteAction, tweenType, newValues.newStartValue, newValues.newEndValue, newValues.newDeltaValue, newValues.newDuration, easeCurve, loopType, loopCount);
			return newAnimation;
		}
		
		public override bool updateBefore {
			get {
				return true;
			}
		}
	}
	/// <summary>
	/// UICircle animation class for Angle Offset animations
	/// </summary>
	public class UICircleAnimation_AngleOffset : UICircleAnimation_Float
	{
		public int currentValue;

		public UICircleAnimation_AngleOffset (UICircleAnimator myUICircleAnimator, Action onCompleteAction, TweenType tweenType, float startAngle, float endAngle, float deltaAngle, float duration, AnimationCurve easeCurve, LoopType loopType, int loopCount)
		: base(myUICircleAnimator, onCompleteAction, tweenType, startAngle, endAngle, deltaAngle, duration, easeCurve, loopType, loopCount)
		{
		}
		
		public override void StartAnimation ()
		{
			SetStartValues (myUICircleAnimator.GetAngleOffsetValue ());
		}
		
		public override void UpdateAnimation ()
		{
			currentValue = (int)GetEasedValue ();
			int adjustedAngleOffset = currentValue;
			if (adjustedAngleOffset > 360)
				adjustedAngleOffset -= 360;
			if (adjustedAngleOffset < 0)
				adjustedAngleOffset += 360;
			myUICircleAnimator.SetAngleOffsetValue (adjustedAngleOffset);
		}
		
		public override UIAnimation_Base GetNewAnimation ()
		{
			NewValues newValues = GetNewValues ();
			UICircleAnimation_AngleOffset newAnimation = new UICircleAnimation_AngleOffset (myUICircleAnimator, onCompleteAction, tweenType, newValues.newStartValue, newValues.newEndValue, newValues.newDeltaValue, newValues.newDuration, easeCurve, loopType, loopCount);
			return newAnimation;
		}
		
		public override bool updateBefore {
			get {
				return false;
			}
		}
	}
	/// <summary>
	/// UICircleAnimation class for Thickness animations
	/// </summary>
	public class UICircleAnimation_Thickness : UICircleAnimation_Float
	{
		public float  currentValue;

		public UICircleAnimation_Thickness (UICircleAnimator myUICircleAnimator, Action onCompleteAction, TweenType tweenType, float startThickness, float endThickness, float deltaThickness, float duration, AnimationCurve easeCurve, LoopType loopType, int loopCount) 
		: base(myUICircleAnimator, onCompleteAction, tweenType, startThickness, endThickness, deltaThickness, duration, easeCurve, loopType, loopCount)
		{
		}
		
		public override void StartAnimation ()
		{
			SetStartValues (myUICircleAnimator.GetThicknessValue ());
		}
		
		public override void UpdateAnimation ()
		{
			currentValue = Mathf.Clamp01 (GetEasedValue ());
			myUICircleAnimator.SetThicknessValue (currentValue);
		}
		
		public override UIAnimation_Base GetNewAnimation ()
		{
			NewValues newValues = GetNewValues ();
			UICircleAnimation_Thickness newAnimation = new UICircleAnimation_Thickness (myUICircleAnimator, onCompleteAction, tweenType, newValues.newStartValue, newValues.newEndValue, newValues.newDeltaValue, newValues.newDuration, easeCurve, loopType, loopCount);
			return newAnimation;
		}
		
		public override bool updateBefore {
			get {
				return false;
			}
		}
	}
	/// <summary>
	/// UICircleAnimation class for Repetitions animations
	/// </summary>
	public class UICircleAnimation_Repetitions : UICircleAnimation_Float
	{
		public int currentValue;

		public UICircleAnimation_Repetitions (UICircleAnimator myUICircleAnimator, Action onCompleteAction, TweenType tweenType, float startAngle, float endAngle, float deltaAngle, float duration, AnimationCurve easeCurve, LoopType loopType, int loopCount) : base(myUICircleAnimator, onCompleteAction, tweenType, startAngle, endAngle, deltaAngle, duration, easeCurve, loopType, loopCount)
		{
		}
	
		public override void StartAnimation ()
		{
			SetStartValues (myUICircleAnimator.GetRepetitionsValue ());
		}
		
		public override void UpdateAnimation ()
		{
			currentValue = (int)GetEasedValue ();
			int adjustedRepitions = Mathf.Clamp (currentValue, 0, 360);
			myUICircleAnimator.SetRepetitionsValue (adjustedRepitions);
		}
		
		public override UIAnimation_Base GetNewAnimation ()
		{
			NewValues newValues = GetNewValues ();
			UICircleAnimation_Repetitions newAnimation = new UICircleAnimation_Repetitions (myUICircleAnimator, onCompleteAction, tweenType, newValues.newStartValue, newValues.newEndValue, newValues.newDeltaValue, newValues.newDuration, easeCurve, loopType, loopCount);
			return newAnimation;
		}
		
		public override bool updateBefore {
			get {
				return true;
			}
		}
	}
}
