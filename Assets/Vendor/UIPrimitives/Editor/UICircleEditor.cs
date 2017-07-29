using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Linq;
using UIPrimitives;

namespace UIPrimitives
{
	[CanEditMultipleObjects]
	[CustomEditor (typeof(UICircle))]
	class UICircleEditor : Editor
	{

		SerializedProperty radius;
		SerializedProperty angle ;
		SerializedProperty angleOffset ;
		SerializedProperty repeatInterval ;
		SerializedProperty repetitions ;
		SerializedProperty thickness ;
		SerializedProperty subdivisions;
		//Dots
		SerializedProperty drawDots;
		SerializedProperty dotRadius;
		SerializedProperty dotSubdivisions;
		//Appearance
		SerializedProperty useGradient;
		SerializedProperty gradientStretch;
		SerializedProperty worldSpace;
		SerializedProperty gradientStyle;
		SerializedProperty gradient;
		SerializedProperty m_Color;
		SerializedProperty material;
		//Glow
		SerializedProperty shouldGlow;
		SerializedProperty useCircleColor;
		SerializedProperty glowColor;
		SerializedProperty glowDistance;

		void OnEnable ()
		{
			//Dimensions
			radius = serializedObject.FindProperty ("radius");
			angle = serializedObject.FindProperty ("angle");
			angleOffset = serializedObject.FindProperty ("angleOffset");
			repeatInterval = serializedObject.FindProperty ("repeatInterval");
			repetitions = serializedObject.FindProperty ("repetitions");
			thickness = serializedObject.FindProperty ("thickness");
			subdivisions = serializedObject.FindProperty ("subdivisions");
			//dots
			drawDots = serializedObject.FindProperty ("drawDots");
			dotRadius = serializedObject.FindProperty ("dotRadius");
			dotSubdivisions = serializedObject.FindProperty ("dotSubdivisions");
			//Appearance
			useGradient = serializedObject.FindProperty ("useGradient");
			gradientStretch = serializedObject.FindProperty ("gradientStretch");
			worldSpace = serializedObject.FindProperty ("worldSpace");
			gradientStyle = serializedObject.FindProperty ("gradientStyle");
			gradient = serializedObject.FindProperty ("gradient");
			m_Color = serializedObject.FindProperty ("m_Color");
			material = serializedObject.FindProperty ("m_Material");
			//Glow
			shouldGlow = serializedObject.FindProperty ("shouldGlow");
			useCircleColor = serializedObject.FindProperty ("useCircleColor");
			glowColor = serializedObject.FindProperty ("glowColor");
			glowDistance = serializedObject.FindProperty ("glowDistance");
		}

		AnimationCurve currentCurve;
		static readonly int SECTION_SPACE = 5;

		public override void OnInspectorGUI ()
		{
			serializedObject.Update ();

			//Script
			SerializedProperty scriptProperty = serializedObject.GetIterator ();
			scriptProperty.NextVisible (true);
			EditorGUILayout.PropertyField (scriptProperty, new GUIContent ("Script"));

			//Appearance
			GUILayout.Space (SECTION_SPACE);
			EditorGUILayout.LabelField ("Appearance", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField (material, new GUIContent ("Material"));
			EditorGUILayout.PropertyField (useGradient, new GUIContent ("Use Gradient"));
			if (useGradient.boolValue) {
				EditorGUILayout.PropertyField (gradientStyle, new GUIContent ("Gradient Style"));
				if (gradientStyle.intValue == 0)
					EditorGUILayout.LabelField ("Radial uses end colors", EditorStyles.miniLabel);
//			else if (gradientStyle.intValue == 1)
//				EditorGUILayout.LabelField ("Increase subdivisions if you see banding", EditorStyles.miniLabel);
				if (gradientStyle.intValue == 1) {
					if (!worldSpace.boolValue)
						EditorGUILayout.PropertyField (gradientStretch, new GUIContent ("Fit Gradient"));
					if (!gradientStretch.boolValue)
						EditorGUILayout.PropertyField (worldSpace, new GUIContent ("World Space"));
				}
				EditorGUILayout.PropertyField (gradient, new GUIContent ("Gradient"));
			} else {
				EditorGUILayout.PropertyField (m_Color, new GUIContent ("Color"));
			}
			//Dimensions
			GUILayout.Space (SECTION_SPACE);
			EditorGUILayout.LabelField ("Dimensions", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField (radius, new GUIContent ("Radius"));
			EditorGUILayout.PropertyField (angle, new GUIContent ("Angle"));
			EditorGUILayout.PropertyField (angleOffset, new GUIContent ("Angle Offset"));
			EditorGUILayout.PropertyField (repeatInterval, new GUIContent ("Repeat Interval"));
			EditorGUILayout.PropertyField (repetitions, new GUIContent ("Repetitions"));
			EditorGUILayout.PropertyField (thickness, new GUIContent ("Thickness"));
			EditorGUILayout.PropertyField (subdivisions, new GUIContent ("Subdivisions"));
			//Dots
			GUILayout.Space (SECTION_SPACE);
			EditorGUILayout.LabelField ("Dots", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField (drawDots, new GUIContent ("Draw Dots"));
			if (drawDots.boolValue) {
				EditorGUILayout.PropertyField (dotRadius, new GUIContent ("Dot Radius"));
				EditorGUILayout.PropertyField (dotSubdivisions, new GUIContent ("Dot Subdivisions"));
			}
		
			//Glow
			GUILayout.Space (SECTION_SPACE);
			EditorGUILayout.LabelField ("Glow", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField (shouldGlow, new GUIContent ("Glow"));
			if (shouldGlow.boolValue) {
				EditorGUILayout.PropertyField (useCircleColor, new GUIContent ("Use Circle Color"));
				if (!useCircleColor.boolValue) {
					EditorGUILayout.PropertyField (glowColor, new GUIContent ("Glow Color"));
				}
				EditorGUILayout.PropertyField (glowDistance, new GUIContent ("Glow Thickness"));
			}

			serializedObject.ApplyModifiedProperties ();
		}
	}
}