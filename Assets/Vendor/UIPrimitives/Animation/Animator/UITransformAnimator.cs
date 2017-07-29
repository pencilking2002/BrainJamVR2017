using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UIPrimitives;

namespace UIPrimitives
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Transform))]
	public class UITransformAnimator : UIAnimator
	{
		//Queues Of Animations
		public Queue<UIAnimation_Base> myUIAnimationsPosition;
		public Queue<UIAnimation_Base> myUIAnimationsRotation;
		public Queue<UIAnimation_Base> myUIAnimationsScale;
		
		///////////////////////////////////////////////////////////////////////////
		//
		// Inherited From Monobehaviour
		//
		
		protected override void Awake ()
		{
			base.Awake();
			
			myUIAnimationsPosition = new Queue<UIAnimation_Base> ();
			myUIAnimations.Add (myUIAnimationsPosition);
			
			myUIAnimationsRotation = new Queue<UIAnimation_Base> ();
			myUIAnimations.Add (myUIAnimationsRotation);
			
			myUIAnimationsScale = new Queue<UIAnimation_Base> ();
			myUIAnimations.Add (myUIAnimationsScale);
		}
		
		protected void Update ()
		{
			UpdateAnimations();
		}
		
		///////////////////////////////////////////////////////////////////////////
		//
		// UITransformAnimator Functions
		//

		public void StartAnimation (UITransformAnimation.TransformAnimationType animationType, Action onCompleteAction, UIAnimation_Base.TweenType tweenType, Vector3 startValue, Vector3 endValue, Vector3 deltaValue, float duration, AnimationCurve easeCurve, UIAnimation_Base.LoopType loopType, int loopCount,Action<float> onUpdateAction=null)
		{
			switch (animationType) {
			case UITransformAnimation.TransformAnimationType.Position:
				AddPositionAnimation (tweenType,onCompleteAction, onUpdateAction, startValue, endValue, deltaValue, duration, easeCurve, loopType, loopCount);
				break;
			case UITransformAnimation.TransformAnimationType.Scale:
				AddScaleAnimation (tweenType,onCompleteAction, onUpdateAction, startValue, endValue, deltaValue, duration, easeCurve, loopType, loopCount);
				break;
			}
		}

		public void StartAnimation (UITransformAnimation.TransformAnimationType animationType, Action onCompleteAction, UIAnimation_Base.TweenType tweenType, Quaternion startValue, Quaternion endValue, Quaternion deltaValue, float duration, AnimationCurve easeCurve, UIAnimation_Base.LoopType loopType, int loopCount,Action<float> onUpdateAction=null)
		{
			switch (animationType) {
			case UITransformAnimation.TransformAnimationType.Rotation:
				AddRotationAnimation (tweenType,onCompleteAction, onUpdateAction, startValue, endValue, deltaValue, duration, easeCurve, loopType, loopCount);
				break;
			}
		}
		
		////////////////////////////////////////
		//
		// Position Functions
		
		protected void AddPositionAnimation (UIAnimation_Base.TweenType tweenType,Action onCompleteAction,Action<float> onUpdateAction, Vector3 startPosition, Vector3 endPosition, Vector3 deltaPositon, float duration, AnimationCurve easeCurve, UIAnimation_Base.LoopType loopType, int loopCount)
		{
			UITransformAnimation_Position newUIAnimationPosition = new UITransformAnimation_Position (this,onCompleteAction,onUpdateAction, tweenType, startPosition, endPosition, deltaPositon, duration, easeCurve, loopType, loopCount);
			myUIAnimationsPosition.Enqueue (newUIAnimationPosition);
		}
		
		//////////////////////////////
		// Position Start/End/Relative Functions
		
		public void AddPositionStartAnimation (Vector3 startPosition, Vector3 endPosition, float duration, UIAnimationUtility.EaseType easeType, Action onCompleteAction=null, Action<float> onUpdateAction=null,int loopCount=0, UIAnimation_Base.LoopType loopType=UIAnimation_Base.LoopType.PingPong)
		{
			AddPositionAnimation (UIAnimation_Base.TweenType.Start, onCompleteAction, onUpdateAction, startPosition, endPosition, Vector3.zero, duration, UIAnimationUtility.GetCurve (easeType), loopType, loopCount);
		}
		
		public void AddPositionEndAnimation (Vector3 endPosition, float duration, UIAnimationUtility.EaseType easeType, Action onCompleteAction=null, Action<float> onUpdateAction=null,int loopCount=0, UIAnimation_Base.LoopType loopType=UIAnimation_Base.LoopType.PingPong)
		{
			AddPositionAnimation (UIAnimation_Base.TweenType.End, onCompleteAction, onUpdateAction, Vector3.zero, endPosition, Vector3.zero, duration, UIAnimationUtility.GetCurve (easeType), loopType, loopCount);
		}
		
		public void AddPositionRelativeAnimation (Vector3 deltaPosition, float duration, UIAnimationUtility.EaseType easeType, Action onCompleteAction=null, Action<float> onUpdateAction=null, int loopCount=0, UIAnimation_Base.LoopType loopType=UIAnimation_Base.LoopType.PingPong)
		{
			AddPositionAnimation (UIAnimation_Base.TweenType.Relative, onCompleteAction, onUpdateAction, Vector3.zero,Vector3.zero, deltaPosition, duration, UIAnimationUtility.GetCurve (easeType), loopType, loopCount);
		}
		
		//////////////////////////////
		// Position Value Get/Set
		
		public void SetPositionValue(Vector3 position){
			transform.localPosition=position;
		}
		
		public Vector3 GetPositionValue(){
			return transform.localPosition;
		}
		
		////////////////////////////////////////
		//
		// Rotation Functions
		
		public void AddRotationAnimation (UIAnimation_Base.TweenType tweenType,Action onCompleteAction, Action<float> onUpdateAction, Quaternion startRotation, Quaternion endRotation, Quaternion deltaRotation, float duration, AnimationCurve easeCurve, UIAnimation_Base.LoopType loopType, int loopCount)
		{
			UITransformAnimation_Rotation newUIAnimationPosition = new UITransformAnimation_Rotation (this, onCompleteAction, onUpdateAction, tweenType, startRotation, endRotation, deltaRotation, duration, easeCurve, loopType, loopCount);
			myUIAnimationsRotation.Enqueue (newUIAnimationPosition);
		}
		
		//////////////////////////////
		// Rotation Start/End/Relative Functions
		
		public void AddRotationStartAnimation (Quaternion startRotation, Quaternion endRotation, float duration, UIAnimationUtility.EaseType easeType, Action onCompleteAction=null, Action<float> onUpdateAction=null, int loopCount=0, UIAnimation_Base.LoopType loopType=UIAnimation_Base.LoopType.PingPong)
		{
			AddRotationAnimation (UIAnimation_Base.TweenType.Start, onCompleteAction, onUpdateAction, startRotation, endRotation, Quaternion.identity, duration, UIAnimationUtility.GetCurve (easeType), loopType, loopCount);
		}
		
		public void AddRotationEndAnimation (Quaternion endRotation, float duration, UIAnimationUtility.EaseType easeType, Action onCompleteAction=null, Action<float> onUpdateAction=null, int loopCount=0, UIAnimation_Base.LoopType loopType=UIAnimation_Base.LoopType.PingPong)
		{
			AddRotationAnimation (UIAnimation_Base.TweenType.End, onCompleteAction, onUpdateAction, Quaternion.identity, endRotation, Quaternion.identity, duration, UIAnimationUtility.GetCurve (easeType), loopType, loopCount);
		}
		
		public void AddRotationRelativeAnimation (Quaternion deltaRotation, float duration, UIAnimationUtility.EaseType easeType, Action onCompleteAction=null, Action<float> onUpdateAction=null, int loopCount=0, UIAnimation_Base.LoopType loopType=UIAnimation_Base.LoopType.PingPong)
		{
			AddRotationAnimation (UIAnimation_Base.TweenType.Relative, onCompleteAction, onUpdateAction, Quaternion.identity,Quaternion.identity, deltaRotation, duration, UIAnimationUtility.GetCurve (easeType), loopType, loopCount);
		}
		
		//////////////////////////////
		// Rotation Value Get/Set
		
		public void SetRotationValue(Quaternion rotation){
			transform.localRotation=rotation;
		}
		
		public Quaternion GetRotationValue(){
			return transform.localRotation;
		}
		
		////////////////////////////////////////
		//
		// Scale Functions
		
		public void AddScaleAnimation (UIAnimation_Base.TweenType tweenType, Action onCompleteAction, Action<float> onUpdateAction, Vector3 startScale, Vector3 endScale, Vector3 deltaScale, float duration, AnimationCurve easeCurve, UIAnimation_Base.LoopType loopType, int loopCount)
		{
			UITransformAnimation_Scale newUIAnimationPosition = new UITransformAnimation_Scale (this,onCompleteAction, onUpdateAction, tweenType, startScale, endScale, deltaScale, duration, easeCurve, loopType, loopCount);
			myUIAnimationsScale.Enqueue (newUIAnimationPosition);
			if(myUIAnimationsScale.Count==1)
				UpdateAnimation(myUIAnimationsScale);
		}
		
		//////////////////////////////
		// Scale Start/End/Relative Functions
		
		public void AddScaleStartAnimation (Vector3 startScale, Vector3 endScale, float duration, UIAnimationUtility.EaseType easeType, Action onCompleteAction=null, Action<float> onUpdateAction=null,  int loopCount=0, UIAnimation_Base.LoopType loopType=UIAnimation_Base.LoopType.PingPong)
		{
			AddScaleAnimation (UIAnimation_Base.TweenType.Start, onCompleteAction, onUpdateAction, startScale, endScale, Vector3.zero, duration, UIAnimationUtility.GetCurve (easeType), loopType, loopCount);
		}
		
		public void AddScaleEndAnimation (Vector3 endScale, float duration, UIAnimationUtility.EaseType easeType, Action onCompleteAction=null, Action<float> onUpdateAction=null,  int loopCount=0, UIAnimation_Base.LoopType loopType=UIAnimation_Base.LoopType.PingPong)
		{
			AddScaleAnimation (UIAnimation_Base.TweenType.End, onCompleteAction, onUpdateAction, Vector3.zero, endScale, Vector3.zero, duration, UIAnimationUtility.GetCurve (easeType), loopType, loopCount);
		}
		
		public void AddScaleRelativeAnimation (Vector3 deltaScale, float duration, UIAnimationUtility.EaseType easeType, Action onCompleteAction=null, Action<float> onUpdateAction=null,  int loopCount=0, UIAnimation_Base.LoopType loopType=UIAnimation_Base.LoopType.PingPong)
		{
			AddScaleAnimation (UIAnimation_Base.TweenType.Relative, onCompleteAction, onUpdateAction, Vector3.zero,Vector3.zero, deltaScale, duration, UIAnimationUtility.GetCurve (easeType), loopType, loopCount);
		}
		
		//////////////////////////////
		// Scale Value Get/Set
		
		public void SetScaleValue(Vector3 scale){
			transform.localScale=scale;
		}
		
		public Vector3 GetScaleValue(){
			return transform.localScale;
		}
	}

	/// <summary>
	/// UITransformAnimation class for animations with Vector3 values
	/// </summary>
	public abstract class UITransformAnimation_Vector3 : UIAnimation_Vector3
	{
		public UITransformAnimator myUITransformAnimator;
		protected Action<float> onUpdateAction;

		public UITransformAnimation_Vector3 (UITransformAnimator myUITransformAnimator, Action onCompleteAction,Action<float> onUpdateAction,UIAnimation_Base.TweenType tweenType, Vector3 startValue, Vector3 endValue,  Vector3 deltaValue, float duration, AnimationCurve easeCurve, LoopType loopType, int loopCount)
			: base(onCompleteAction,tweenType, startValue, endValue, deltaValue, duration, easeCurve, loopType, loopCount)
		{
			this.myUITransformAnimator 	= myUITransformAnimator;
			this.onUpdateAction			= onUpdateAction;
		}
	}
	
	public class UITransformAnimation_Position : UITransformAnimation_Vector3
	{
		public UITransformAnimation_Position (UITransformAnimator myUITransformAnimator, Action onCompleteAction,Action<float> onUpdateAction, TweenType tweenType, Vector3 startValue, Vector3 endValue,  Vector3 deltaValue, float duration, AnimationCurve easeCurve, LoopType loopType, int loopCount)
		: base(myUITransformAnimator, onCompleteAction,onUpdateAction, tweenType, startValue, endValue, deltaValue, duration, easeCurve, loopType, loopCount){
		}
		
		public override void StartAnimation ()
		{
			SetStartValues(myUITransformAnimator.GetPositionValue ());
		}
		
		public override void UpdateAnimation ()
		{
			if(onUpdateAction!=null)
				onUpdateAction(GetPercent());
			currentValue =GetEasedValue();
			myUITransformAnimator.SetPositionValue(currentValue);
		}
		
		public override UIAnimation_Base GetNewAnimation ()
		{
			NewValues newValues = GetNewValues();
			UITransformAnimation_Position newAnimation = new UITransformAnimation_Position (myUITransformAnimator, onCompleteAction,onUpdateAction, tweenType, newValues.newStartValue, newValues.newEndValue, newValues.newDeltaValue, newValues.newDuration, easeCurve, loopType, loopCount);
			return newAnimation;
		}
		
		public override bool updateBefore {
			get {
				return true;
			}
		}
		
	}

	/// <summary>
	/// UITransformAnimation class for animations with Vector3 values
	/// </summary>
	public abstract class UITransformAnimation_Quaternion : UIAnimation_Quaternion
	{
		public UITransformAnimator myUITransformAnimator;
		protected Action<float> onUpdateAction;

		public UITransformAnimation_Quaternion (UITransformAnimator myUITransformAnimator, Action onCompleteAction,Action<float> onUpdateAction,UIAnimation_Base.TweenType tweenType, Quaternion startValue, Quaternion endValue,  Quaternion deltaValue, float duration, AnimationCurve easeCurve, LoopType loopType, int loopCount)
			: base(onCompleteAction,tweenType, startValue, endValue, deltaValue, duration, easeCurve, loopType, loopCount)
		{
			this.myUITransformAnimator 	= myUITransformAnimator;
			this.onUpdateAction			= onUpdateAction;
		}
	}
	
	public class UITransformAnimation_Rotation : UITransformAnimation_Quaternion
	{
		public UITransformAnimation_Rotation (UITransformAnimator myUITransformAnimator, Action onCompleteAction,Action<float> onUpdateAction, TweenType tweenType, Quaternion startValue, Quaternion endValue,  Quaternion deltaValue, float duration, AnimationCurve easeCurve, LoopType loopType, int loopCount)
		: base(myUITransformAnimator, onCompleteAction,onUpdateAction, tweenType, startValue, endValue, deltaValue, duration, easeCurve, loopType, loopCount){}
		
		public override void StartAnimation ()
		{
			SetStartValues(myUITransformAnimator.GetRotationValue ());
		}
		
		public override void UpdateAnimation ()
		{
			if(onUpdateAction!=null)
				onUpdateAction(GetPercent());
			currentValue =GetEasedValue();
			myUITransformAnimator.SetRotationValue(currentValue);
		}
		
		public override UIAnimation_Base GetNewAnimation ()
		{
			NewValues newValues = GetNewValues();
			UITransformAnimation_Rotation newAnimation = new UITransformAnimation_Rotation (myUITransformAnimator, onCompleteAction, onUpdateAction, tweenType, newValues.newStartValue, newValues.newEndValue, newValues.newDeltaValue, newValues.newDuration, easeCurve, loopType, loopCount);
			return newAnimation;
		}
		
		public override bool updateBefore {
			get {
				return true;
			}
		}
		
	}
	
	public class UITransformAnimation_Scale : UITransformAnimation_Vector3
	{
		public UITransformAnimation_Scale (UITransformAnimator myUITransformAnimator, Action onCompleteAction, Action<float> onUpdateAction, TweenType tweenType, Vector3 startValue, Vector3 endValue,  Vector3 deltaValue, float duration, AnimationCurve easeCurve, LoopType loopType, int loopCount) : base(myUITransformAnimator, onCompleteAction,onUpdateAction, tweenType, startValue, endValue, deltaValue, duration, easeCurve, loopType, loopCount){}
		
		public override void StartAnimation ()
		{
			SetStartValues(myUITransformAnimator.GetScaleValue ());
		}
		
		public override void UpdateAnimation ()
		{
			if(onUpdateAction!=null)
				onUpdateAction(GetPercent());
			currentValue =GetEasedValue();
			myUITransformAnimator.SetScaleValue(currentValue);
		}
		
		public override UIAnimation_Base GetNewAnimation ()
		{
			NewValues newValues = GetNewValues();
			UITransformAnimation_Scale newAnimation = new UITransformAnimation_Scale (myUITransformAnimator, onCompleteAction, onUpdateAction, tweenType, newValues.newStartValue, newValues.newEndValue, newValues.newDeltaValue, newValues.newDuration, easeCurve, loopType, loopCount);
			return newAnimation;
		}
		
		public override bool updateBefore {
			get {
				return true;
			}
		}
	}
}