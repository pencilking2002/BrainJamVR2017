using UnityEditor;
using UnityEngine;
using System.Collections;
using UIPrimitives;

namespace UIPrimitives
{
	[CanEditMultipleObjects]
	[CustomEditor (typeof(UIColorAnimation))]
	class UIColorAnimationEditor : UIAnimationEditor
	{
		//Value
		protected SerializedProperty animationType;
		//Color
		protected SerializedProperty startColor;
		protected SerializedProperty endColor;
		protected SerializedProperty deltaColor;

		//Target
		protected UIColorAnimation myUIColorAnimation;
		
		///////////////////////////////////////////////////////////////////////////
		//
		// Inherited from Editor
		//

		protected override void OnEnable ()
		{
			base.OnEnable ();
			//Value
			animationType = serializedObject.FindProperty ("animationType");
			//Color
			startColor = serializedObject.FindProperty ("startColor");
			endColor = serializedObject.FindProperty ("endColor");
			deltaColor = serializedObject.FindProperty ("deltaColor");
			
			//If the values are the same we reset them
			if (Color.Equals (startColor.colorValue, endColor.colorValue)) {
				ResetColors ();
			}
		}

		public override void OnInspectorGUI ()
		{
			serializedObject.Update ();

			//Target
			myUIColorAnimation = ((UIColorAnimation)target);
			myUIAnimation = myUIColorAnimation;

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
		// UIColorAnimationEditor Functions
		//

		protected override void DrawValues ()
		{
			base.DrawValues();
			if (myUIColorAnimation.animationType == UIColorAnimation.ColorAnimationType.Color) {
				if (GUILayout.Button ("Reset Colors", EditorStyles.miniButton)) {
					ResetColors ();
				}
			} 
		}
		
		protected override void DrawStartValues ()
		{
			switch (myUIColorAnimation.animationType) {
			case (UIColorAnimation.ColorAnimationType.Color):
				DrawStartColor();
				DrawEndColor();
				break;
			}
		}
		
		protected override void DrawEndValues ()
		{
			switch (myUIColorAnimation.animationType) {
			case (UIColorAnimation.ColorAnimationType.Color):
				DrawEndColor();
				break;
			}
		}
		
		protected override void DrawRelativeValues ()
		{
			switch (myUIColorAnimation.animationType) {
			case (UIColorAnimation.ColorAnimationType.Color):
				DrawRelativeColor();
				break;
			}
		}
		
		////////////////////////////////////////
		//
		// Draw Functions
		
		//////////////////////////////
		// Draw Color Functions
		
		protected void DrawStartColor(){
			EditorGUILayout.PropertyField (startColor, new GUIContent ("Start Color"));
		}
		
		protected void DrawEndColor(){
			EditorGUILayout.PropertyField (endColor, new GUIContent ("End Color"));
		}
		
		protected void DrawRelativeColor(){
			EditorGUILayout.PropertyField (deltaColor, new GUIContent ("Change In Color"));
		}

		/// <summary>
		/// Resets the colors.
		/// </summary>
		protected void ResetColors ()
		{		
			for (int i=0; i<targets.Length; ++i) {
				((UIColorAnimation)targets [i]).ResetColors ();
			}
		}
	}
}