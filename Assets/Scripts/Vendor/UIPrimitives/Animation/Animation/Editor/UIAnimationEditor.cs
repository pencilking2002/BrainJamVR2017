using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace UIPrimitives
{
	public abstract class UIAnimationEditor : Editor
	{
		//Readonly
		protected readonly int SECTION_SPACE = 5;
		protected readonly Color curveColor = new Color (.4f, 1f, 1f);

		/////SerializedProperties/////

		//Ease
		protected SerializedProperty easeType;
		
		//Loop
		protected SerializedProperty loopCount;
		protected SerializedProperty loopType;
		
		//Timing
		protected SerializedProperty duration;

		//Value
		protected SerializedProperty tweenType;
		protected SerializedProperty animationType;

		//Sequence
		protected SerializedProperty playOnAwake;
		protected SerializedProperty startDelay;
		protected SerializedProperty setValueAtStart;
		
		/////Protected/////

		protected UIAnimation myUIAnimation;
		protected UIAnimationUtility.EaseType currentEaseType;
		protected AnimationCurve currentCurve;

		///////////////////////////////////////////////////////////////////////////
		//
		// Inherited from Editor
		//
 	
		protected virtual void OnEnable ()
		{
			//Ease
			easeType = serializedObject.FindProperty ("easeType");

			//Loop
			loopCount = serializedObject.FindProperty ("loopCount");
			loopType = serializedObject.FindProperty ("loopType");

			//Timing
			duration = serializedObject.FindProperty ("duration");

			//Value
			animationType = serializedObject.FindProperty ("animationType");
			tweenType = serializedObject.FindProperty ("tweenType");

			//Sequence
			playOnAwake = serializedObject.FindProperty ("playOnAwake");
			startDelay = serializedObject.FindProperty ("startDelay");
			setValueAtStart = serializedObject.FindProperty ("setValueAtStart");
		}
	
		///////////////////////////////////////////////////////////////////////////
		//
		// UIAnimationEditor Functions
		//

		protected void ApplyValues ()
		{	
			myUIAnimation.easeCurve = currentCurve;
			serializedObject.ApplyModifiedProperties ();
		}

		////////////////////////////////////////
		//
		// OnGUI Functions

		protected virtual void DrawValues ()
		{	
			EditorGUILayout.LabelField ("Value", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField (animationType, new GUIContent ("Animation Type"));
			EditorGUILayout.PropertyField (tweenType, new GUIContent ("Tween Type"));
			//If we are in a situation where we are automatically starting the animation after a delay we might want to set the start value on before the animation starts
			if ((!playOnAwake.boolValue||(playOnAwake.boolValue && startDelay.floatValue != 0)) && myUIAnimation.tweenType == UIAnimation_Base.TweenType.Start)
				EditorGUILayout.PropertyField (setValueAtStart, new GUIContent ("Set Value At Start"));
			else//Set it false otherwise so we can just check if its true and not have to check the other conditions
				setValueAtStart.boolValue=false;
			switch (myUIAnimation.tweenType) {
			case UIAnimation_Base.TweenType.Start:
				DrawStartValues ();
				break;
			case UIAnimation_Base.TweenType.End:
				DrawEndValues ();
				break;
			case UIAnimation_Base.TweenType.Relative:
				DrawRelativeValues ();
				break;
			}
		}
		
		protected abstract void DrawStartValues ();

		protected abstract void DrawEndValues ();

		protected abstract void DrawRelativeValues ();

		/// <summary>
		/// Draws the sequence related GUI elements.
		/// </summary>
		protected void DrawSequence ()
		{
			EditorGUILayout.LabelField ("Animation", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField (playOnAwake, new GUIContent ("Play On Awake"));
			EditorGUILayout.PropertyField (startDelay, new GUIContent ("Start Delay"));
			EditorGUILayout.PropertyField (duration, new GUIContent ("Duration"));
			EditorGUILayout.PropertyField (loopCount, new GUIContent ("Loop Count"));
			if (loopCount.intValue != 0) {
				EditorGUILayout.PropertyField (loopType, new GUIContent ("Loop Type"));
			}
		}

		/// <summary>
		/// Draws the ease related GUI elements.
		/// </summary>
		protected void DrawEase ()
		{
			EditorGUILayout.LabelField ("Ease", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField (easeType, new GUIContent ("Ease Type"));
			currentEaseType = myUIAnimation.easeType;
			if (currentEaseType != myUIAnimation.lastEaseType) {
				currentCurve = UIAnimationUtility.GetCurve (currentEaseType);
			}

			myUIAnimation.lastEaseType = currentEaseType;
		}

		/// <summary>
		/// Draws the curve related GUI elements.
		/// </summary>
		protected void DrawCurve ()
		{
			if (currentCurve == null) {
				currentCurve = myUIAnimation.easeCurve;
				if (currentCurve == null)
					currentCurve = UIAnimationUtility.GetCurve (currentEaseType);
			}

			int curveHeight = Screen.width - 20;
			if (myUIAnimation.isCurveMinimized)
				curveHeight = 40;
			currentCurve = EditorGUILayout.CurveField (currentCurve, curveColor, new Rect (), GUILayout.Height (curveHeight));
			GUILayout.BeginHorizontal ();
			//Reset curve to chosen animation type
			if (GUILayout.Button ("Reset", EditorStyles.miniButtonLeft)) {
				currentCurve = UIAnimationUtility.GetCurve (currentEaseType);
			}
			string resizeText = "Minimize";
			//Minimize/Maximize curve window
			if (myUIAnimation.isCurveMinimized)
				resizeText = "Maximize";
			if (GUILayout.Button (resizeText, EditorStyles.miniButtonRight)) {
				myUIAnimation.isCurveMinimized = !myUIAnimation.isCurveMinimized;
			}
			GUILayout.EndHorizontal ();

		}
	}
}
