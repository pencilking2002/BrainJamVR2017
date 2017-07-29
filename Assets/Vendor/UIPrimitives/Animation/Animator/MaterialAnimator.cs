using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UIPrimitives;

namespace UIPrimitives
{
	[DisallowMultipleComponent]
//	[RequireComponent (typeof(Renderer))]
	public class MaterialAnimator : UIAnimator
	{
		[SerializeField]
		protected Material myMaterial;

		//Queues Of Animations
		public Queue<UIAnimation_Base> myMaterialAnimationsColor;
		public Queue<UIAnimation_Base> myMaterialAnimationsFloat;

		//Properties
		public Material MyMaterial { get {return this.myMaterial; } } 

		///////////////////////////////////////////////////////////////////////////
		//
		// Inherited from MonoBehaviour
		//

		protected override void Awake () {

			base.Awake ();

			RefreshMaterial ();

			myMaterialAnimationsColor = new Queue<UIAnimation_Base> ();
			myUIAnimations.Add (myMaterialAnimationsColor);

			myMaterialAnimationsFloat = new Queue<UIAnimation_Base> ();
			myUIAnimations.Add (myMaterialAnimationsFloat);
		}

		protected void Update () {	

			UpdateAnimations ();
		}

		///////////////////////////////////////////////////////////////////////////
		//
		// UIColorAnimator Functions
		//

		public void StartAnimation (MaterialAnimation.AnimationType animationType, UIAnimation_Base.TweenType tweenType, MaterialAnimation.ColorData colorData, MaterialAnimation.FloatData floatData, float duration, AnimationCurve easeCurve, UIAnimation_Base.LoopType loopType, int loopCount,Action onCompleteAction = null) {

			switch (animationType) {
			case MaterialAnimation.AnimationType.Color:
				AddColorAnimation (tweenType, onCompleteAction, colorData,  duration, easeCurve, loopType, loopCount);
				break;
			case MaterialAnimation.AnimationType.Float:
				AddFloatAnimation (tweenType, onCompleteAction, floatData,  duration, easeCurve, loopType, loopCount);
				break;
			}
		}

		////////////////////////////////////////
		//
		// StartColor Functions

		protected void AddColorAnimation (UIAnimation_Base.TweenType tweenType, Action onCompleteAction, MaterialAnimation.ColorData colorData, float duration, AnimationCurve easeCurve, UIAnimation_Base.LoopType loopType, int loopCount) {

			MaterialAnimation_Color newMaterialAnimationColor = new MaterialAnimation_Color (this, onCompleteAction, tweenType, Shader.PropertyToID (colorData.propertyName), colorData.startColor, colorData.endColor, colorData.deltaColor, duration, easeCurve, loopType, loopCount);
			myMaterialAnimationsColor.Enqueue (newMaterialAnimationColor);
		}

		//////////////////////////////
		// Color Start/End/Relative Functions

		public void AddColorStartAnimation (Color startColor, Color endColor, string propertyName = "_Color", float duration = 2f, UIAnimationUtility.EaseType easeType = UIAnimationUtility.EaseType.easeInSine, int loopCount = 0, UIAnimation_Base.LoopType loopType = UIAnimation_Base.LoopType.PingPong, Action onCompleteAction = null) {

			MaterialAnimation.ColorData colorData = new MaterialAnimation.ColorData ();
			colorData.startColor = startColor;
			colorData.endColor = endColor;
			colorData.propertyName = propertyName;

			AddColorAnimation (UIAnimation_Base.TweenType.Start, onCompleteAction, colorData, duration, UIAnimationUtility.GetCurve (easeType), loopType, loopCount);
		}

		public void AddColorEndAnimation (Color endColor, string propertyName = "_Color", float duration = 2f, UIAnimationUtility.EaseType easeType = UIAnimationUtility.EaseType.easeInSine, int loopCount = 0, UIAnimation_Base.LoopType loopType = UIAnimation_Base.LoopType.PingPong, Action onCompleteAction = null) {

			MaterialAnimation.ColorData colorData = new MaterialAnimation.ColorData ();
			colorData.endColor = endColor;
			colorData.propertyName = propertyName;

			AddColorAnimation (UIAnimation_Base.TweenType.End, onCompleteAction, colorData, duration, UIAnimationUtility.GetCurve (easeType), loopType, loopCount);
		}

		public void AddColorRelativeAnimation (Color deltaColor, string propertyName = "_Color", float duration = 2f, UIAnimationUtility.EaseType easeType = UIAnimationUtility.EaseType.easeInSine, int loopCount = 0, UIAnimation_Base.LoopType loopType = UIAnimation_Base.LoopType.PingPong, Action onCompleteAction = null) {

			MaterialAnimation.ColorData colorData = new MaterialAnimation.ColorData ();
			colorData.deltaColor = deltaColor;
			colorData.propertyName = propertyName;

			AddColorAnimation (UIAnimation_Base.TweenType.Relative, onCompleteAction, colorData, duration, UIAnimationUtility.GetCurve (easeType), loopType, loopCount);
		}

		//////////////////////////////
		// Color Value Get/Set

		public void SetColorValue (Color color, string propertyName) {

			SetColorValue (color, Shader.PropertyToID (propertyName));
		}

		public void SetColorValue (Color color, int propertyID) {

			Material material = GetMaterial();
			if (material != null && material.HasProperty (propertyID))
				material.SetColor (propertyID, color);
			else
				Debug.Log ("Material is either null or doesn't have the property");
		}

		public Color GetColorValue (string propertyName) {

			return GetColorValue (Shader.PropertyToID (propertyName));
		}

		public Color GetColorValue (int propertyID) {

			if (myMaterial != null) {
				if (myMaterial.HasProperty (propertyID))
					return myMaterial.GetColor (propertyID);
				else {
					Debug.Log ("Property not found on material");
					return Color.white;
				}
					
			}else {
				Debug.Log ("No material found");
				return Color.white;
			}
		}

		////////////////////////////////////////
		//
		// StartFloat Functions

		protected void AddFloatAnimation (UIAnimation_Base.TweenType tweenType, Action onCompleteAction, MaterialAnimation.FloatData floatData, float duration, AnimationCurve easeCurve, UIAnimation_Base.LoopType loopType, int loopCount) {

			MaterialAnimation_Float newMaterialAnimationFloat = new MaterialAnimation_Float (this, onCompleteAction, tweenType, Shader.PropertyToID (floatData.propertyName), floatData.startValue, floatData.endValue, floatData.deltaValue, duration, easeCurve, loopType, loopCount);
			myMaterialAnimationsFloat.Enqueue (newMaterialAnimationFloat);
		}

		//////////////////////////////
		// Float Start/End/Relative Functions

		public void AddFloatStartAnimation (float startValue, float endValue, string propertyName = "_AlphaMultiplier", float duration = 2f, UIAnimationUtility.EaseType easeType = UIAnimationUtility.EaseType.easeInSine, int loopCount = 0, UIAnimation_Base.LoopType loopType = UIAnimation_Base.LoopType.PingPong, Action onCompleteAction = null) {

			MaterialAnimation.FloatData floatData = new MaterialAnimation.FloatData ();
			floatData.startValue = startValue;
			floatData.endValue = endValue;
			floatData.propertyName = propertyName;

			AddFloatAnimation (UIAnimation_Base.TweenType.Start, onCompleteAction, floatData, duration, UIAnimationUtility.GetCurve (easeType), loopType, loopCount);
		}

		public void AddFloatEndAnimation (float endValue, string propertyName = "_AlphaMultiplier", float duration = 2f, UIAnimationUtility.EaseType easeType = UIAnimationUtility.EaseType.easeInSine, int loopCount = 0, UIAnimation_Base.LoopType loopType = UIAnimation_Base.LoopType.PingPong, Action onCompleteAction = null) {

			MaterialAnimation.FloatData floatData = new MaterialAnimation.FloatData ();
			floatData.endValue = endValue;
			floatData.propertyName = propertyName;

			AddFloatAnimation (UIAnimation_Base.TweenType.End, onCompleteAction, floatData, duration, UIAnimationUtility.GetCurve (easeType), loopType, loopCount);
		}

		public void AddFloatRelativeAnimation (float deltaValue, string propertyName = "_AlphaMultiplier", float duration = 2f, UIAnimationUtility.EaseType easeType = UIAnimationUtility.EaseType.easeInSine, int loopCount = 0, UIAnimation_Base.LoopType loopType = UIAnimation_Base.LoopType.PingPong, Action onCompleteAction = null) {

			MaterialAnimation.FloatData floatData = new MaterialAnimation.FloatData ();
			floatData.deltaValue = deltaValue;
			floatData.propertyName = propertyName;

			AddFloatAnimation (UIAnimation_Base.TweenType.Relative, onCompleteAction, floatData, duration, UIAnimationUtility.GetCurve (easeType), loopType, loopCount);
		}

		//////////////////////////////
		// Float Value Get/Set

		public void SetFloatValue (float value, string propertyName) {

			SetFloatValue (value, Shader.PropertyToID (propertyName));
		}

		public void SetFloatValue (float value, int propertyID) {

			Material material = GetMaterial();
			if (material != null)
				material.SetFloat (propertyID, value);

		}

		public float GetFloatValue (string propertyName) {

			return GetFloatValue (Shader.PropertyToID (propertyName));
		}

		public float GetFloatValue (int propertyID) {

			if (myMaterial != null)
				return myMaterial.GetFloat (propertyID);
			else {
				Debug.Log ("No material found");
				return 1f;
			}
		}

		////////////////////////////////////////
		//
		// Material Functions

		protected Material GetMaterial() {

			if (this.myMaterial != null)
				return myMaterial;

			if (Application.isPlaying) {
				RefreshMaterial ();
			} else {
				this.myMaterial = this.GetComponent<Renderer> ().sharedMaterial;
			}

			return this.myMaterial;
		}

		protected void RefreshMaterial() {

			if(myMaterial==null)
				myMaterial = GetComponent<Renderer> ().material;
		}
	}

	public class MaterialAnimation_Color : UIAnimation_Color
	{
		protected MaterialAnimator materialAnimator;
		protected int propertyID;

		public MaterialAnimation_Color (MaterialAnimator materialAnimator, Action onCompleteAction, UIAnimation_Base.TweenType tweenType, int propertyID, Color startColor, Color endColor, Color deltaColor, float duration, AnimationCurve easeCurve, LoopType loopType, int loopCount)
			: base (onCompleteAction, tweenType, startColor, endColor, deltaColor, duration, easeCurve, loopType, loopCount)
		{
			this.materialAnimator = materialAnimator;
			this.propertyID = propertyID;
		}

		public override void StartAnimation () {
			SetStartValues (materialAnimator.GetColorValue (propertyID));
		}

		public override void UpdateAnimation () {
			newColor = GetEasedValue ();
			materialAnimator.SetColorValue (newColor,propertyID);
		}

		public override UIAnimation_Base GetNewAnimation () {
			NewValues newValues = GetNewValues ();
			MaterialAnimation_Color newAnimation = new MaterialAnimation_Color (materialAnimator, onCompleteAction, tweenType, this.propertyID,  newValues.newStartValue, newValues.newEndValue, newValues.newDeltaValue, newValues.newDuration, easeCurve, loopType, loopCount);
			return newAnimation;
		}

		public override bool updateBefore {
			get {
				return true;
			}
		}
	}

	public class MaterialAnimation_Float : UIAnimation_Float
	{
		protected MaterialAnimator materialAnimator;
		protected int propertyID;

		public MaterialAnimation_Float (MaterialAnimator materialAnimator, Action onCompleteAction, UIAnimation_Base.TweenType tweenType, int propertyID, float startValue, float endValue, float deltaValue, float duration, AnimationCurve easeCurve, LoopType loopType, int loopCount)
			: base (onCompleteAction, tweenType, startValue, endValue, deltaValue, duration, easeCurve, loopType, loopCount)
		{
			this.materialAnimator = materialAnimator;
			this.propertyID = propertyID;
		}

		public override void StartAnimation () {
			SetStartValues (materialAnimator.GetFloatValue (propertyID));
		}

		public override void UpdateAnimation () {
			currentValue = GetEasedValue ();
			materialAnimator.SetFloatValue (currentValue,propertyID);
		}

		public override UIAnimation_Base GetNewAnimation () {
			NewValues newValues = GetNewValues ();
			MaterialAnimation_Float newAnimation = new MaterialAnimation_Float (materialAnimator, onCompleteAction, tweenType, this.propertyID,  newValues.newStartValue, newValues.newEndValue, newValues.newDeltaValue, newValues.newDuration, easeCurve, loopType, loopCount);
			return newAnimation;
		}

		public override bool updateBefore {
			get {
				return true;
			}
		}
	}
}