using UnityEditor;
using UnityEngine;
using System.Collections;
using UIPrimitives;

namespace UIPrimitives
{
	[CanEditMultipleObjects]
	[CustomEditor (typeof(CanvasGroupAnimation))]
	class CanvasGroupAnimationEditor : UIAnimationEditor
	{
		//Value
		protected SerializedProperty classProperty;
		//Uses the same values for all the animations
		protected SerializedProperty startValue;
		protected SerializedProperty endValue;
		protected SerializedProperty deltaValue;

		//Target
		protected CanvasGroupAnimation myCanvasGroupAnimation;
		
		///////////////////////////////////////////////////////////////////////////
		//
		// Inherited from Editor
		//

		protected override void OnEnable ()
		{
			base.OnEnable ();

			//Value
			startValue = serializedObject.FindProperty ("startValue");
			endValue = serializedObject.FindProperty ("endValue");
			deltaValue = serializedObject.FindProperty ("deltaValue");
		}

		public override void OnInspectorGUI ()
		{
			serializedObject.Update ();
			
			/////Target/////
			myCanvasGroupAnimation = ((CanvasGroupAnimation)target);
			myUIAnimation = myCanvasGroupAnimation;


			/////Sequence/////
			GUILayout.Space (SECTION_SPACE);
			DrawSequence ();

			//////Value/////
			GUILayout.Space (SECTION_SPACE);
			DrawValues ();
			
			/////Ease////
			GUILayout.Space (SECTION_SPACE);
			DrawEase ();
			
			/////Curve/////
			DrawCurve ();
			
			/////Apply/////
			ApplyValues ();
		}
		
		///////////////////////////////////////////////////////////////////////////
		//
		// UICircleAnimationEditor Functions
		//
		
		protected override void DrawValues ()
		{
			base.DrawValues();
		}
		
		protected override void DrawStartValues ()
		{
			DrawStartValue();
			DrawEndValue();
		}
		
		protected override void DrawEndValues ()
		{
			DrawEndValue();
		}
		
		protected override void DrawRelativeValues ()
		{
			DrawRelativeValue();
		}
		
		////////////////////////////////////////
		//
		// Draw Functions
		
		//////////////////////////////
		// Draw Value Functions
		
		protected void DrawStartValue(){
			EditorGUILayout.PropertyField (startValue, new GUIContent ("Start Value"));
		}
		
		protected void DrawEndValue(){
			EditorGUILayout.PropertyField (endValue, new GUIContent ("End Value"));
		}
		
		protected void DrawRelativeValue(){
			EditorGUILayout.PropertyField (deltaValue, new GUIContent ("Change In Value"));
		}

	}
}
