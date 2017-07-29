using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Linq;
using UIPrimitives;

namespace UIPrimitives
{
	[CanEditMultipleObjects]
	[CustomEditor (typeof(UIButton))]
	class UIButtonEditor : Editor
	{
		//readonly
		protected readonly int SECTION_SPACE = 5;

		//Timing
		protected SerializedProperty fillSpeed;
		protected SerializedProperty cooldownTime ;
		//Fill References
		protected SerializedProperty fillType;
//		protected SerializedProperty fillRenderer;
//		protected SerializedProperty fillMaterial;
//		protected SerializedProperty fillShaderFloatName;
		protected SerializedProperty fillUIRect;
		protected SerializedProperty fillUICircle;
		//Info
		protected SerializedProperty tooltipName;
		protected SerializedProperty isToggle;
		protected SerializedProperty shouldResetOnStart;

		//Graphic
		protected SerializedProperty graphic;
		protected SerializedProperty normalColor;
		protected SerializedProperty disabledColor;
		protected SerializedProperty hoverColor;
		protected SerializedProperty selectedColor;

		//Animator
		protected SerializedProperty hoverRootAnimator;
		protected SerializedProperty selectRootAnimator;

		protected void OnEnable ()
		{
			//Timing
			this.fillSpeed 				= this.serializedObject.FindProperty ("fillSpeed");
			this.cooldownTime 			= this.serializedObject.FindProperty ("cooldownTime");
			//Fill References
			this.fillType 				= this.serializedObject.FindProperty ("fillType");
//			this.fillRenderer 			= this.serializedObject.FindProperty ("fillRenderer");
//			this.fillMaterial 			= this.serializedObject.FindProperty ("fillMaterial");
//			this.fillShaderFloatName 	= this.serializedObject.FindProperty ("fillShaderFloatName");
			this.fillUIRect 			= this.serializedObject.FindProperty ("fillUIRect");
			this.fillUICircle 			= this.serializedObject.FindProperty ("fillUICircle");
			//Info
			this.tooltipName 			= this.serializedObject.FindProperty ("tooltipName");
			this.isToggle 			= this.serializedObject.FindProperty ("isToggle");
			this.shouldResetOnStart 			= this.serializedObject.FindProperty ("shouldResetOnStart");
			//graphic
			this.graphic 			= this.serializedObject.FindProperty ("graphic");
			this.normalColor 			= this.serializedObject.FindProperty ("normalColor");
			this.disabledColor 			= this.serializedObject.FindProperty ("disabledColor");
			this.hoverColor 			= this.serializedObject.FindProperty ("hoverColor");
			this.selectedColor 			= this.serializedObject.FindProperty ("selectedColor");
			//animator
			this.hoverRootAnimator 			= this.serializedObject.FindProperty ("hoverRootAnimator");
			this.selectRootAnimator 			= this.serializedObject.FindProperty ("selectRootAnimator");
		}


		public override void OnInspectorGUI ()
		{
			serializedObject.Update ();

			//Script
			GUILayout.Space (SECTION_SPACE);
			SerializedProperty scriptProperty = serializedObject.GetIterator ();
			scriptProperty.NextVisible (true);
			EditorGUILayout.PropertyField (scriptProperty, new GUIContent ("Script"));

			//Timing
			GUILayout.Space (SECTION_SPACE);
			EditorGUILayout.LabelField ("Timing", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField (this.fillSpeed, new GUIContent ("Trigger Time"));
			EditorGUILayout.PropertyField (this.cooldownTime, new GUIContent ("Cooldown Time"));

			//Fill references
			GUILayout.Space (SECTION_SPACE);
			EditorGUILayout.LabelField ("Fill", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField (this.fillType, new GUIContent ("Fill Type"));

			switch((UIButton.FillType) this.fillType.enumValueIndex)
			{
//			case Button.FillType.Renderer:
//				
//				EditorGUILayout.PropertyField (this.fillRenderer, new GUIContent ("Renderer"));
//				EditorGUILayout.PropertyField (this.fillShaderFloatName, new GUIContent ("Shader Float Name"));
//				
//				break;
//				
//			case UIButton.FillType.Material:
//				
//				EditorGUILayout.PropertyField (this.fillMaterial, new GUIContent ("Material"));
//				EditorGUILayout.PropertyField (this.fillShaderFloatName, new GUIContent ("Shader Float Name"));
//				
//				break;
			case UIButton.FillType.UIRect:
				
				EditorGUILayout.PropertyField (this.fillUIRect, new GUIContent ("UIRect"));
				
				break;
			case UIButton.FillType.UICircle:
				
				EditorGUILayout.PropertyField (this.fillUICircle, new GUIContent ("UICircle"));
				
				break;
			}
			
			//Info
			GUILayout.Space (SECTION_SPACE);
			EditorGUILayout.LabelField ("Info", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField (this.tooltipName, new GUIContent ("Tooltip Name"));
			EditorGUILayout.PropertyField (this.isToggle, new GUIContent ("Is Toggle"));
			EditorGUILayout.PropertyField (this.shouldResetOnStart, new GUIContent ("Should Reset On Start"));
			
			//Graphic
			GUILayout.Space (SECTION_SPACE);
			EditorGUILayout.LabelField ("Graphic", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField (this.graphic, new GUIContent ("graphic"));
			EditorGUILayout.PropertyField (this.normalColor, new GUIContent ("normalColor"));
			EditorGUILayout.PropertyField (this.disabledColor, new GUIContent ("disabledColor"));
			EditorGUILayout.PropertyField (this.hoverColor, new GUIContent ("hoverColor"));
			EditorGUILayout.PropertyField (this.selectedColor, new GUIContent ("selectedColor"));
			
			//Animator
			GUILayout.Space (SECTION_SPACE);
			EditorGUILayout.LabelField ("Animator", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField (this.hoverRootAnimator, new GUIContent ("Hover Root Animator"));
			EditorGUILayout.PropertyField (this.selectRootAnimator, new GUIContent ("Select Root Animator"));


			this.serializedObject.ApplyModifiedProperties ();
		}
	}
}