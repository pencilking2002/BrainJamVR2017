using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UIPrimitives;

namespace UIPrimitives
{
	[DisallowMultipleComponent]
//	[RequireComponent(typeof(Graphic))]
	public class UIColorAnimator : UIAnimator
	{
		//MyGraphic
		protected Graphic myGraphic;
		protected MeshRenderer myMeshRenderer;
		protected Material myMaterial;
		protected int myMaterialPropertyID = -1;

		//Queues Of Animations
		public Queue<UIAnimation_Base> myUIAnimationsColor;

		///////////////////////////////////////////////////////////////////////////
		//
		// Inherited from MonoBehaviour
		//

		protected void Awake ()
		{
			base.Awake();
			
			myGraphic = GetComponent<Graphic> ();
			if(myGraphic==null)
				myMeshRenderer = GetComponent<MeshRenderer> ();
			if(myMeshRenderer!=null)
				myMaterial = myMeshRenderer.sharedMaterial;
			if(myMaterial!=null)
			{
				string[] properties = new string[]{"_Color","_EmissionColor","_RimColor"};
				Stack<string> propertiesToTry = new Stack<string>(properties);
				while(propertiesToTry.Count!=0 && myMaterialPropertyID == -1)
				{
					string propertyToTry = propertiesToTry.Pop();
					if(myMaterial.HasProperty(propertyToTry))
						myMaterialPropertyID = Shader.PropertyToID(propertyToTry);
				}
			}

			myUIAnimationsColor = new Queue<UIAnimation_Base> ();
			myUIAnimations.Add(myUIAnimationsColor);
		}
	
		protected void Update ()
		{	
			UpdateAnimations();
		}

		///////////////////////////////////////////////////////////////////////////
		//
		// UIColorAnimator Functions
		//

		public void StartAnimation (UIColorAnimation.ColorAnimationType animationType, Action onCompleteAction, UIAnimation_Base.TweenType tweenType, Color startColor, Color endColor, Color deltaColor, float duration, AnimationCurve easeCurve, UIAnimation_Base.LoopType loopType, int loopCount)
		{
			switch (animationType) {
			case UIColorAnimation.ColorAnimationType.Color:
				AddColorAnimation (tweenType, onCompleteAction, startColor, endColor, deltaColor, duration, easeCurve, loopType, loopCount);
				break;
			}
		}

		////////////////////////////////////////
		//
		// StartColor Functions
		
		protected void AddColorAnimation (UIAnimation_Base.TweenType tweenType, Action onCompleteAction, Color startColor, Color endColor, Color deltaColor, float duration, AnimationCurve easeCurve, UIAnimation_Base.LoopType loopType, int loopCount)
		{
			UIColorAnimation_Color newUIAnimationColor = new UIColorAnimation_Color (this, onCompleteAction, tweenType, startColor, endColor, deltaColor, duration, easeCurve, loopType, loopCount);
			myUIAnimationsColor.Enqueue (newUIAnimationColor);
		}
		
		//////////////////////////////
		// Color Start/End/Relative Functions

		public void AddColorStartAnimation (Action onCompleteAction, Color startColor, Color endColor, float duration, UIAnimationUtility.EaseType easeType, int loopCount=0, UIAnimation_Base.LoopType loopType=UIAnimation_Base.LoopType.PingPong)
		{
			AddColorAnimation (UIAnimation_Base.TweenType.Start, onCompleteAction, startColor, endColor, Color.white, duration, UIAnimationUtility.GetCurve (easeType), loopType, loopCount);
		}
		
		public void AddColorEndAnimation (Action onCompleteAction, Color endColor, float duration, UIAnimationUtility.EaseType easeType, int loopCount=0, UIAnimation_Base.LoopType loopType=UIAnimation_Base.LoopType.PingPong)
		{
			AddColorAnimation (UIAnimation_Base.TweenType.End, onCompleteAction,Color.white, endColor, Color.white, duration, UIAnimationUtility.GetCurve (easeType), loopType, loopCount);
		}
		
		public void AddColorRelativeAnimation (Action onCompleteAction, Color deltaColor, float duration, UIAnimationUtility.EaseType easeType, int loopCount=0, UIAnimation_Base.LoopType loopType=UIAnimation_Base.LoopType.PingPong)
		{
			AddColorAnimation (UIAnimation_Base.TweenType.Relative, onCompleteAction, Color.white,Color.white, deltaColor, duration, UIAnimationUtility.GetCurve (easeType), loopType, loopCount);
		}
		
		//////////////////////////////
		// Color Value Get/Set
		
		public void SetColorValue (Color color)
		{
			if(myGraphic!=null)
				myGraphic.color = color;
			if(myMaterial!=null)
				myMaterial.SetColor(myMaterialPropertyID,color);
		}
		
		public Color GetColorValue ()
		{
			if(myGraphic!=null)
				return myGraphic.color;
			if(myMaterial!=null)
				return myMaterial.GetColor(myMaterialPropertyID);
			else
			{
				Debug.Log ("No graphic or material found");
				return Color.white;
			}
		}
	}

	public class UIColorAnimation_Color : UIAnimation_Color
	{
		protected UIColorAnimator myUIColorAnimator;
	
		public UIColorAnimation_Color (UIColorAnimator myUIColorAnimator, Action onCompleteAction, UIAnimation_Base.TweenType tweenType, Color startColor, Color endColor, Color deltaColor, float duration, AnimationCurve easeCurve, LoopType loopType, int loopCount)
			:base( onCompleteAction,  tweenType,  startColor,  endColor,  deltaColor,  duration,  easeCurve,  loopType,  loopCount)
		{
			this.myUIColorAnimator = myUIColorAnimator;
		}
	
		public override void StartAnimation ()
		{
			SetStartValues(myUIColorAnimator.GetColorValue());
		}
	
		public override void UpdateAnimation ()
		{
			newColor = GetEasedValue();
			myUIColorAnimator.SetColorValue (newColor);
		}
		
		public override UIAnimation_Base GetNewAnimation ()
		{
			NewValues newValues=GetNewValues();
			UIColorAnimation_Color newAnimation = new UIColorAnimation_Color (myUIColorAnimator, onCompleteAction,tweenType, newValues.newStartValue, newValues.newEndValue, newValues.newDeltaValue, newValues.newDuration, easeCurve, loopType, loopCount);
			return newAnimation;
		}

		public override bool updateBefore {
			get {
				return true;
			}
		}
	}
}