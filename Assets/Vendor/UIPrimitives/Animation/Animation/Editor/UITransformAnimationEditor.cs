using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Linq;
using System.Runtime.InteropServices;
using UIPrimitives;

namespace UIPrimitives
{
	[CanEditMultipleObjects]
	[CustomEditor (typeof(UITransformAnimation))]
	class UITransformAnimationEditor : UIAnimationEditor
	{
		/////Value/////
		//Position
		protected SerializedProperty startPosition;
		protected SerializedProperty endPosition;
		protected SerializedProperty deltaPosition;
		//Rotation
		protected SerializedProperty startRotation;
		protected SerializedProperty endRotation;
		protected SerializedProperty deltaRotation;
		//Scale
		protected SerializedProperty startScale;
		protected SerializedProperty endScale;
		protected SerializedProperty deltaScale;
		//Vector lock
		protected SerializedProperty lockX;
		protected SerializedProperty lockY;
		protected SerializedProperty lockZ;

		//Target
		protected UITransformAnimation myUITransformAnimation;
		
		///////////////////////////////////////////////////////////////////////////
		//
		// Inherited from Editor
		//

		protected override void OnEnable ()
		{
			base.OnEnable ();
			
			/////Value/////
			//Position
			startPosition = serializedObject.FindProperty ("startPosition");
			endPosition = serializedObject.FindProperty ("endPosition");
			deltaPosition = serializedObject.FindProperty ("deltaPosition");
			//Rotation
			startRotation = serializedObject.FindProperty ("startRotation");
			endRotation = serializedObject.FindProperty ("endRotation");
			deltaRotation = serializedObject.FindProperty ("deltaRotation");
			//Scale
			startScale = serializedObject.FindProperty ("startScale");
			endScale = serializedObject.FindProperty ("endScale");
			deltaScale = serializedObject.FindProperty ("deltaScale");
			//Vector lock
			lockX = serializedObject.FindProperty ("lockX");
			lockY = serializedObject.FindProperty ("lockY");
			lockZ = serializedObject.FindProperty ("lockZ");

			//If all the end values are the same we reset them
			if (Vector3.Equals (endPosition.vector3Value, endRotation.vector3Value) && Vector3.Equals (endRotation.vector3Value, endScale.vector3Value)) {
				ResetValues ();
			}
		}

		/// <summary>
		/// Raises the inspector GUI event.
		/// Draw everything we need to the Editor window.
		/// </summary>
		public override void OnInspectorGUI ()
		{
			serializedObject.Update ();

			/////Target/////
			myUITransformAnimation = ((UITransformAnimation)target);
			myUIAnimation = myUITransformAnimation;

			/////Sequence/////
			GUILayout.Space (SECTION_SPACE);
			DrawSequence ();

			/////Value/////
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
		// UITransformAnimationEditor Functions
		//

		////////////////////////////////////////
		//
		// Draw Value Functions

		/// <summary>
		/// Draws the values.
		/// </summary>
		protected override void DrawValues ()
		{
			base.DrawValues ();
			//Lock
			EditorGUILayout.PropertyField (lockX, new GUIContent ("Lock X"));
			EditorGUILayout.PropertyField (lockY, new GUIContent ("Lock Y"));
			EditorGUILayout.PropertyField (lockZ, new GUIContent ("Lock Z"));
			//Reset
			if (GUILayout.Button ("Reset Values", EditorStyles.miniButton)) {
				ResetValues ();
			}
		}

		protected override void DrawStartValues ()
		{
			switch (myUITransformAnimation.animationType) {
			case (UITransformAnimation.TransformAnimationType.Position):
				DrawStartPosition();
				DrawEndPosition();
				break;
			case (UITransformAnimation.TransformAnimationType.Rotation):
				DrawStartRotation();
				DrawEndRotation();
				break;
			case (UITransformAnimation.TransformAnimationType.Scale):
				DrawStartScale();
				DrawEndScale();
				break;
			}
		}

		protected override void DrawEndValues ()
		{
			switch (myUITransformAnimation.animationType) {
			case (UITransformAnimation.TransformAnimationType.Position):
				DrawEndPosition();
				break;
			case (UITransformAnimation.TransformAnimationType.Rotation):
				DrawEndRotation();
				break;
			case (UITransformAnimation.TransformAnimationType.Scale):
				DrawEndScale();
				break;
			}
		}

		protected override void DrawRelativeValues ()
		{
			switch (myUITransformAnimation.animationType) {
			case (UITransformAnimation.TransformAnimationType.Position):
				DrawRelativePosition();
				break;
			case (UITransformAnimation.TransformAnimationType.Rotation):
				DrawRelativeRotation();
				break;
			case (UITransformAnimation.TransformAnimationType.Scale):
				DrawRelativeScale();
				break;
			}
		}
		
		////////////////////////////////////////
		//
		// Draw Functions
		
		//////////////////////////////
		// Draw Position Functions
		
		protected void DrawStartPosition(){
			EditorGUILayout.PropertyField (startPosition, new GUIContent ("Start Position (Local)"));
		}
		
		protected void DrawEndPosition(){
			EditorGUILayout.PropertyField (endPosition, new GUIContent ("End Position (Local)"));
		}
		
		protected void DrawRelativePosition(){
			EditorGUILayout.PropertyField (deltaPosition, new GUIContent ("Delta Position (Local)"));
		}
		
		//////////////////////////////
		// Draw Rotation Functions
		
		protected void DrawStartRotation(){
			EditorGUILayout.PropertyField (startRotation, new GUIContent ("Start Rotation (Local)"));
		}
		
		protected void DrawEndRotation(){
			EditorGUILayout.PropertyField (endRotation, new GUIContent ("End Rotation (Local)"));
		}
		
		protected void DrawRelativeRotation(){
			EditorGUILayout.PropertyField (deltaRotation, new GUIContent ("Delta Rotation (Local)"));
		}
		
		//////////////////////////////
		// Draw Scale Functions
		
		protected void DrawStartScale(){
			EditorGUILayout.PropertyField (startScale, new GUIContent ("Start Scale (Local)"));
		}
		
		protected void DrawEndScale(){
			EditorGUILayout.PropertyField (endScale, new GUIContent ("End Scale (Local)"));
		}
		
		protected void DrawRelativeScale(){
			EditorGUILayout.PropertyField (deltaScale, new GUIContent ("Delta Scale (Local)"));
		}
		
		/// <summary>
		/// Resets the values.
		/// </summary>
		protected void ResetValues ()
		{		
			UnityEditor.Undo.RecordObjects(targets,"Undo Reset Values");
			for (int i=0; i<targets.Length; ++i) {
				((UITransformAnimation)targets [i]).ResetValues ();
			}
		}

	}
}