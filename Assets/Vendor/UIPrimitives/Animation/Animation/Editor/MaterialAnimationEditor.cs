using UnityEditor;
using UnityEngine;
using System.Collections;
using UIPrimitives;

namespace UIPrimitives
{
	[CanEditMultipleObjects]
	[CustomEditor (typeof(MaterialAnimation))]
	class MaterialAnimationEditor : UIAnimationEditor
	{
		//Value
		protected SerializedProperty animationType;
		//Color
		protected SerializedProperty colorData;
		protected SerializedProperty floatData;

		//Target
		protected MaterialAnimation myMaterialAnimation;
		
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
			colorData = serializedObject.FindProperty ("colorData");
			//float
			floatData = serializedObject.FindProperty ("floatData");
			
			//If the values are the same we reset them
//			if (Color.Equals (((MaterialAnimation.ColorData) colorData.objectReferenceValue).startColor, endColor.colorValue)) {
//				ResetColors ();
//			}
		}

		public override void OnInspectorGUI ()
		{
			serializedObject.Update ();

			//Target
			myMaterialAnimation = ((MaterialAnimation)target);
			myUIAnimation = myMaterialAnimation;

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

			switch (myMaterialAnimation.animationType) {
			case (MaterialAnimation.AnimationType.Color):
				DrawPropertyColor();
				break;
			case (MaterialAnimation.AnimationType.Float):
				DrawPropertyFloat();
				break;
			}
//			if (myMaterialAnimation.animationType == MaterialAnimation.AnimationType.Color) {
//				if (GUILayout.Button ("Reset Colors", EditorStyles.miniButton)) {
//					ResetColors ();
//				}
//			} 
		}
		
		protected override void DrawStartValues ()
		{
			switch (myMaterialAnimation.animationType) {
			case (MaterialAnimation.AnimationType.Color):
				DrawStartColor();
				DrawEndColor();
				break;
			case (MaterialAnimation.AnimationType.Float):
				DrawStartFloat();
				DrawEndFloat();
				break;
			}
		}
		
		protected override void DrawEndValues ()
		{
			switch (myMaterialAnimation.animationType) {
			case (MaterialAnimation.AnimationType.Color):
				DrawEndColor();
				break;
			case (MaterialAnimation.AnimationType.Float):
				DrawEndFloat();
				break;
			}
		}
		
		protected override void DrawRelativeValues ()
		{
			switch (myMaterialAnimation.animationType) {
			case (MaterialAnimation.AnimationType.Color):
				DrawRelativeColor();
				break;
			case (MaterialAnimation.AnimationType.Float):
				DrawRelativeFloat();
				break;
			}
		}
		
		////////////////////////////////////////
		//
		// Draw Functions

		//////////////////////////////
		// Draw Color Functions

		protected void DrawStartColor(){
			GUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("Start Color");
			((MaterialAnimation)target).colorData.startColor = EditorGUILayout.ColorField (((MaterialAnimation)target).colorData.startColor);
			GUILayout.EndHorizontal ();
		}

		protected void DrawEndColor(){
			GUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("End Color");
			((MaterialAnimation)target).colorData.endColor = EditorGUILayout.ColorField (((MaterialAnimation)target).colorData.endColor);
			GUILayout.EndHorizontal ();
		}

		protected void DrawRelativeColor(){
			GUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("Relative Color");
			((MaterialAnimation)target).colorData.deltaColor = EditorGUILayout.ColorField (((MaterialAnimation)target).colorData.deltaColor);
			GUILayout.EndHorizontal ();
		}

		protected void DrawPropertyColor(){
			GUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("Property Name");
			((MaterialAnimation)target).colorData.propertyName = EditorGUILayout.TextField (((MaterialAnimation)target).colorData.propertyName);
			GUILayout.EndHorizontal ();
		}

		//////////////////////////////
		// Draw Float Functions

		protected void DrawStartFloat(){
			GUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("Start Value");
			((MaterialAnimation)target).floatData.startValue = EditorGUILayout.FloatField (((MaterialAnimation)target).floatData.startValue);
			GUILayout.EndHorizontal ();
		}

		protected void DrawEndFloat(){
			GUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("End Value");
			((MaterialAnimation)target).floatData.endValue = EditorGUILayout.FloatField (((MaterialAnimation)target).floatData.endValue);
			GUILayout.EndHorizontal ();
		}

		protected void DrawRelativeFloat(){
			GUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("Relative Value");
			((MaterialAnimation)target).floatData.deltaValue = EditorGUILayout.FloatField (((MaterialAnimation)target).floatData.deltaValue);
			GUILayout.EndHorizontal ();
		}

		protected void DrawPropertyFloat(){
			GUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("Property Name");
			((MaterialAnimation)target).floatData.propertyName = EditorGUILayout.TextField (((MaterialAnimation)target).floatData.propertyName);
			GUILayout.EndHorizontal ();
		}

		/// <summary>
		/// Resets the colors.
		/// </summary>
//		protected void ResetColors ()
//		{		
//			for (int i=0; i<targets.Length; ++i) {
//				((MaterialAnimation)targets [i]).ResetColors ();
//			}
//		}
	}
}