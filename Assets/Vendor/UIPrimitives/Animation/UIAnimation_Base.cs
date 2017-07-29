using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace UIPrimitives
{
	public abstract class UIAnimation_Base
	{
	
		public enum LoopType
		{
			Loop,
			PingPong
		}
	
		public enum TweenType
		{
			Start,
			End,
			Relative
		}

	
		//My tween type
		protected TweenType tweenType;
		//My loop type
		protected LoopType loopType;
		//To call when animation is complete
		public Action onCompleteAction;
		//AnimationValues
		public AnimationCurve easeCurve;
		public float startTime;
		public float endTime;
		public float duration;
		public int loopCount;
		public bool hasStarted;
		//Decides whether or not to update before or after checking if its complete
		public abstract bool updateBefore { get; }
	
		public abstract void StartAnimation ();
	
		public abstract void UpdateAnimation ();

		public abstract UIAnimation_Base GetNewAnimation ();
	
		public bool IsComplete ()
		{
			return Time.time > endTime;
		}

		protected float GetPercent()
		{
			return (Time.time - startTime) / duration;
		}

		protected float GetEasedPercent(){
			float percent = GetPercent();
			return easeCurve.Evaluate (percent);
		}
	
		public float GetRemainingTime ()
		{
			if (hasStarted)
				return (endTime - Time.time);
			else
				return duration;
		}
	}

	public abstract class UIAnimation_Float : UIAnimation_Base
	{
		public float currentValue;
		public float startValue, endValue, deltaValue;
		
		public UIAnimation_Float(Action onCompleteAction, TweenType tweenType, float startValue, float endValue, float deltaValue, float duration, AnimationCurve easeCurve, LoopType loopType, int loopCount)
		{
			this.onCompleteAction = onCompleteAction;
			this.tweenType = tweenType;
			//Value
			this.startValue = startValue;
			this.endValue = endValue;
			this.deltaValue = deltaValue;
			//Animation
			this.duration = duration;
			this.easeCurve = easeCurve;
			//Loop
			this.loopType = loopType;
			this.loopCount = loopCount;
		}
		
		protected struct NewValues
		{
			public float newStartValue;
			public float newEndValue;
			public float newDeltaValue;
			public float newDuration;
		};
		
		protected void SetStartValues(float currentObjectValue){
			hasStarted = true;
			startTime = Time.time;
			endTime = startTime + duration;
			//Value
			switch (tweenType) {
			case UIAnimation_Base.TweenType.Start:
				//Already set correctly
				break;
			case UIAnimation_Base.TweenType.End:
				this.startValue = currentObjectValue;
				break;
			case UIAnimation_Base.TweenType.Relative:
				this.startValue = currentObjectValue;
				this.endValue = startValue + deltaValue;
				break;
			}
		}
		
		protected NewValues GetNewValues ()
		{
			NewValues newValues;
			newValues.newDuration = endTime - startTime;
			newValues.newStartValue=0;
			newValues.newEndValue=0;
			newValues.newDeltaValue=0;
			
			switch (tweenType) {
			case UIAnimation_Base.TweenType.Start:
				if (loopType == LoopType.PingPong) {
					newValues.newStartValue = endValue;
					newValues.newEndValue = startValue;
				} else {
					newValues.newStartValue = startValue;
					newValues.newEndValue = endValue;
				}
				break;
			case UIAnimation_Base.TweenType.End:
				if (loopType == LoopType.PingPong) {
					newValues.newEndValue = startValue;
				} else {
					newValues.newEndValue = endValue;
				}
				break;
			case UIAnimation_Base.TweenType.Relative:
				if (loopType == LoopType.PingPong) {
					newValues.newDeltaValue = startValue - endValue;
				} else {
					newValues.newDeltaValue = endValue - startValue;
				}
				break;
			}
			return newValues;
		}

		protected float GetEasedValue(){
			float easedPercent=GetEasedPercent();
			return startValue + ((endValue - startValue) * easedPercent);
		}
	}

	public abstract class UIAnimation_Int
	{


	}

	public abstract class UIAnimation_Vector3 : UIAnimation_Base
	{
		public Vector3 startValue, endValue, deltaValue;
		public Vector3 currentValue;

		protected struct NewValues
		{
			public Vector3 newStartValue;
			public Vector3 newEndValue;
			public Vector3 newDeltaValue;
			public float newDuration;
		};

		public UIAnimation_Vector3 (Action onCompleteAction, UIAnimation_Base.TweenType tweenType, Vector3 startValue, Vector3 endValue, Vector3 deltaValue, float duration, AnimationCurve easeCurve, LoopType loopType, int loopCount)
		{
			this.onCompleteAction = onCompleteAction;
			this.tweenType = tweenType;
			//Value
			this.startValue = startValue;
			this.endValue = endValue;
			this.deltaValue = deltaValue;
			//Animation
			this.duration = duration;
			this.easeCurve = easeCurve;
			//Loop
			this.loopType = loopType;
			this.loopCount = loopCount;
		}

		protected void SetStartValues (Vector3 currentObjectValue)
		{
			hasStarted = true;
			startTime = Time.time;
			endTime = startTime + duration;
			//Value
			switch (tweenType) {
			case UIAnimation_Base.TweenType.Start:
				//Already set correctly
				break;
			case UIAnimation_Base.TweenType.End:
				this.startValue = currentObjectValue;
				break;
			case UIAnimation_Base.TweenType.Relative:
				this.startValue = currentObjectValue;
				this.endValue = startValue + deltaValue;
				break;
			}
		}

		protected NewValues GetNewValues ()
		{
			NewValues newValues;
			newValues.newDuration = endTime - startTime;
			newValues.newStartValue = Vector3.zero;
			newValues.newEndValue = Vector3.zero;
			newValues.newDeltaValue = Vector3.zero;

			switch (tweenType) {
			case UIAnimation_Base.TweenType.Start:
				if (loopType == LoopType.PingPong) {
					newValues.newStartValue = endValue;
					newValues.newEndValue = startValue;
				} else {
					newValues.newStartValue = startValue;
					newValues.newEndValue = endValue;
				}
				break;
			case UIAnimation_Base.TweenType.End:
				if (loopType == LoopType.PingPong) {
					newValues.newEndValue = startValue;
				} else {
					newValues.newEndValue = endValue;
				}
				break;
			case UIAnimation_Base.TweenType.Relative:
				if (loopType == LoopType.PingPong) {
					newValues.newDeltaValue = startValue - endValue;
				} else {
					newValues.newDeltaValue = endValue - startValue;
				}
				break;
			}
			return newValues;
		}

		protected Vector3 GetEasedValue(){
			float easedPercent = GetEasedPercent();
			return Vector3.Lerp (startValue, endValue, easedPercent);
		}
	}

	public abstract class UIAnimation_Quaternion : UIAnimation_Base
	{
		public Quaternion startValue, endValue, deltaValue;
		public Quaternion currentValue;

		protected struct NewValues
		{
			public Quaternion newStartValue;
			public Quaternion newEndValue;
			public Quaternion newDeltaValue;
			public float newDuration;
		};

		public UIAnimation_Quaternion (Action onCompleteAction, UIAnimation_Base.TweenType tweenType, Quaternion startValue, Quaternion endValue, Quaternion deltaValue, float duration, AnimationCurve easeCurve, LoopType loopType, int loopCount)
		{
			this.onCompleteAction = onCompleteAction;
			this.tweenType = tweenType;
			//Value
			this.startValue = startValue;
			this.endValue = endValue;
			this.deltaValue = deltaValue;
			//Animation
			this.duration = duration;
			this.easeCurve = easeCurve;
			//Loop
			this.loopType = loopType;
			this.loopCount = loopCount;
		}

		protected void SetStartValues (Quaternion currentObjectValue)
		{
			hasStarted = true;
			startTime = Time.time;
			endTime = startTime + duration;
			//Value
			switch (tweenType) {
			case UIAnimation_Base.TweenType.Start:
				//Already set correctly
				break;
			case UIAnimation_Base.TweenType.End:
				this.startValue = currentObjectValue;
				break;
			case UIAnimation_Base.TweenType.Relative:
				this.startValue = currentObjectValue;
				this.endValue = Quaternion.Euler(startValue.eulerAngles + deltaValue.eulerAngles);
				break;
			}
		}

		protected NewValues GetNewValues ()
		{
			NewValues newValues;
			newValues.newDuration = endTime - startTime;
			newValues.newStartValue = Quaternion.identity;
			newValues.newEndValue = Quaternion.identity;
			newValues.newDeltaValue = Quaternion.identity;

			switch (tweenType) {
			case UIAnimation_Base.TweenType.Start:
				if (loopType == LoopType.PingPong) {
					newValues.newStartValue = endValue;
					newValues.newEndValue = startValue;
				} else {
					newValues.newStartValue = startValue;
					newValues.newEndValue = endValue;
				}
				break;
			case UIAnimation_Base.TweenType.End:
				if (loopType == LoopType.PingPong) {
					newValues.newEndValue = startValue;
				} else {
					newValues.newEndValue = endValue;
				}
				break;
			case UIAnimation_Base.TweenType.Relative:
				if (loopType == LoopType.PingPong) {
					newValues.newDeltaValue =  Quaternion.Euler(startValue.eulerAngles - endValue.eulerAngles);
				} else {
					newValues.newDeltaValue =  Quaternion.Euler(endValue.eulerAngles - startValue.eulerAngles);
				}
				break;
			}
			return newValues;
		}

		protected Quaternion GetEasedValue(){
			float easedPercent = GetEasedPercent();
			return Quaternion.Lerp (startValue, endValue, easedPercent);
		}
	}

	public abstract class UIAnimation_Color : UIAnimation_Base
	{
		public Color startColor, endColor,deltaColor, newColor;
		protected Gradient myGradient;
		
		protected struct NewValues
		{
			public Color newStartValue;
			public Color newEndValue;
			public Color newDeltaValue;
			public float newDuration;
		};

		public UIAnimation_Color(Action onCompleteAction, UIAnimation_Base.TweenType tweenType, Color startColor, Color endColor, Color deltaColor, float duration, AnimationCurve easeCurve, LoopType loopType, int loopCount){
			this.onCompleteAction = onCompleteAction;
			this.tweenType = tweenType;
			//Value
			this.startColor = startColor;
			this.endColor = endColor;
			this.deltaColor = deltaColor;
			//Animation
			this.duration = duration;
			this.easeCurve = easeCurve;
			//Loop
			this.loopType = loopType;
			this.loopCount = loopCount;
		}

		protected void SetStartValues(Color currentObjectColor){
			hasStarted = true;
			startTime = Time.time;
			endTime = startTime + duration;
			myGradient = new Gradient ();
			//Value
			switch (tweenType) {
			case UIAnimation_Base.TweenType.Start:
				//Already set correctly
				break;
			case UIAnimation_Base.TweenType.End:
				this.startColor = currentObjectColor;
				break;
			case UIAnimation_Base.TweenType.Relative:
				this.startColor = currentObjectColor;
				this.endColor = startColor + deltaColor;
				break;
			}
			//Make the gradient for easier evaluation than lerping each frame
			GradientColorKey[] gradientColorKeys = new GradientColorKey[]{
				new GradientColorKey (startColor, 0),
				new GradientColorKey (endColor, 1f)
			};
			GradientAlphaKey[] gradientAlphaKeys = new GradientAlphaKey[]{
				new GradientAlphaKey (startColor.a, 0),
				new GradientAlphaKey (endColor.a, 1f)
			};
			myGradient.SetKeys (gradientColorKeys, gradientAlphaKeys);
		}

		protected NewValues GetNewValues(){
			NewValues newValues;
			newValues.newDuration = endTime - startTime;
			
			//Calculate new value
			Color newStartColor = Color.white;
			Color newEndColor = Color.white;
			Color newDeltaColor = Color.white;
			switch (tweenType) {
			case UIAnimation_Base.TweenType.Start:
			case UIAnimation_Base.TweenType.End:
				if (loopType == LoopType.PingPong) {
					newStartColor = endColor;
					newEndColor = startColor;
				} else {
					newStartColor = startColor;
					newEndColor = endColor;
				}
				break;
			case UIAnimation_Base.TweenType.Relative:
				if (loopType == LoopType.PingPong) {
					newDeltaColor = startColor - endColor;
				} else {
					newDeltaColor = endColor - startColor;
				}
				break;
			}
			newValues.newStartValue = newStartColor;
			newValues.newEndValue = newEndColor;
			newValues.newDeltaValue = newDeltaColor;
			return newValues;
		}
		
		
		protected Color GetEasedValue(){
			float easedPercent = GetEasedPercent();
			return myGradient.Evaluate (easedPercent);
		}
	}
}