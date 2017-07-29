using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UIPrimitives;

namespace UIPrimitives
{
	[RequireComponent (typeof(MaterialAnimator))]
	public class MaterialAnimation : UIAnimation
	{
		//enum
		public enum AnimationType
		{
			Color,
			Float
		}

		//structs
		[System.Serializable]
		public class ColorData
		{
			public string propertyName = "_Color";
			public Color startColor = Color.white;
			public Color endColor = Color.white;
			public Color deltaColor = Color.white;
		}
		[System.Serializable]
		public class FloatData
		{
			public string propertyName = "_AlphaMultiplier";
			public float startValue = 0;
			public float endValue = 1f;
			public float deltaValue = .5f;
		}
		
		//Animation type
		public AnimationType animationType = AnimationType.Color;
		
		//My animator
		protected MaterialAnimator materialAnimator;
		protected Renderer myRenderer;

		/////Value/////
		//Color
		[SerializeField]
		public ColorData colorData;
		//Float
		[SerializeField]
		public FloatData floatData;
		
		///////////////////////////////////////////////////////////////////////////
		//
		// Inherited from MonoBehaviour
		//

		protected void Awake ()
		{
			materialAnimator = GetComponent<MaterialAnimator> ();
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
			case AnimationType.Color:
				materialAnimator.SetColorValue(colorData.startColor,colorData.propertyName);
				break;
			case AnimationType.Float:
				materialAnimator.SetFloatValue(floatData.startValue,floatData.propertyName);
				break;
			}
		}

		/// <summary>
		/// Starts the animation.
		/// </summary>
		public override void StartAnimation ()
		{
			materialAnimator.StartAnimation (animationType, tweenType, colorData, floatData, duration, easeCurve, loopType, loopCount);
		}

		public override void StartAnimationReversed ()
		{
			materialAnimator.StartAnimation (animationType, tweenType, Reverse(colorData), Reverse(floatData), duration, easeCurve, loopType, loopCount);
		}
		
		///////////////////////////////////////////////////////////////////////////
		//
		// UIColorAnimation Functions
		//

		/// <summary>
		/// Resets the colors.
		/// </summary>
//		public void ResetColors ()
//		{
//			myRenderer = GetComponent<Renderer> ();
//			colorData.startColor = myRenderer.sharedMaterial.color;
//			colorData.endColor = myRenderer.sharedMaterial.color;
//			if (tweenType == UIAnimation_Base.TweenType.Start)
//				colorData.startColor.a = 0;
//			else
//				colorData.endColor.a = 0;
//		}

		protected ColorData Reverse(ColorData colorData) {

			ColorData reversedColorData = new ColorData ();

			reversedColorData.startColor = colorData.endColor;
			reversedColorData.endColor = colorData.startColor;
			reversedColorData.deltaColor = colorData.deltaColor;
			reversedColorData.propertyName = colorData.propertyName;

			return reversedColorData;
		}

		protected FloatData Reverse(FloatData floatData) {

			FloatData reversedFloatData = new FloatData ();

			reversedFloatData.startValue = floatData.endValue;
			reversedFloatData.endValue = floatData.startValue;
			reversedFloatData.deltaValue = -floatData.deltaValue;
			reversedFloatData.propertyName = floatData.propertyName;

			return reversedFloatData;
		}
	}
}



