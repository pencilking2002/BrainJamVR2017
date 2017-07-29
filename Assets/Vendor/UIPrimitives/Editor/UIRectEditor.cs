using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Linq;
using UIPrimitives;

[CanEditMultipleObjects]
[CustomEditor (typeof(UIRect))]
class UIRectEditor : Editor
{
	//Dimensions
	SerializedProperty thickness;
	SerializedProperty xSubdivisions;
	SerializedProperty ySubdivisions;
	SerializedProperty fillPercentX;
	SerializedProperty fillPercentY;
	SerializedProperty borderRadius;
	//Appearance
	SerializedProperty useGradient;
	SerializedProperty gradientStyle;
	SerializedProperty gradient;
	SerializedProperty m_Color;
	SerializedProperty material;
	//Glow
	SerializedProperty shouldGlow;
	SerializedProperty useRectColor;
	SerializedProperty glowColor;
	SerializedProperty glowDistance;

	void OnEnable ()
	{
		//Dimensions
		thickness = serializedObject.FindProperty ("thickness");
		xSubdivisions = serializedObject.FindProperty ("xSubdivisions");
		ySubdivisions = serializedObject.FindProperty ("ySubdivisions");
		fillPercentX = serializedObject.FindProperty ("fillPercentX");
		fillPercentY = serializedObject.FindProperty ("fillPercentY");
		borderRadius = serializedObject.FindProperty ("borderRadius");
		//Appearance
		useGradient = serializedObject.FindProperty ("useGradient");
		gradientStyle = serializedObject.FindProperty ("gradientStyle");
		gradient = serializedObject.FindProperty ("gradient");
		m_Color = serializedObject.FindProperty ("m_Color");
		material = serializedObject.FindProperty ("m_Material");
		//Glow
		shouldGlow = serializedObject.FindProperty ("shouldGlow");
		useRectColor = serializedObject.FindProperty ("useRectColor");
		glowColor = serializedObject.FindProperty ("glowColor");
		glowDistance = serializedObject.FindProperty ("glowDistance");
	}

	static readonly int SECTION_SPACE = 5;

	public override void OnInspectorGUI ()
	{
		serializedObject.Update ();
		
		//Script
		SerializedProperty scriptProperty = serializedObject.GetIterator ();
		scriptProperty.NextVisible (true);
		EditorGUILayout.PropertyField (scriptProperty, new GUIContent ("Script"));

		//Dimensions
		GUILayout.Space (SECTION_SPACE);
		EditorGUILayout.LabelField ("Dimensions", EditorStyles.boldLabel);
		EditorGUILayout.PropertyField (thickness, new GUIContent ("Thickness"));
		if (thickness.floatValue == 1f) {
			EditorGUILayout.PropertyField (xSubdivisions, new GUIContent ("Subdivisions X"));
			EditorGUILayout.PropertyField (ySubdivisions, new GUIContent ("Subdivisions Y"));
			if (xSubdivisions.intValue == 1 && ySubdivisions.intValue == 1) {
				EditorGUILayout.PropertyField (fillPercentX, new GUIContent ("Fill Percent X"));
				EditorGUILayout.PropertyField (fillPercentY, new GUIContent ("Fill Percent Y"));

				if (fillPercentX.floatValue == 1f && fillPercentY.floatValue == 1f) {

					EditorGUILayout.PropertyField (borderRadius, new GUIContent ("Border Radius"));
				} else {
					borderRadius.floatValue = 0;
				}
			} else {
				borderRadius.floatValue = 0;
			}
		} else {
			borderRadius.floatValue = 0;
		}
		if (thickness.floatValue != 1f) {
			useGradient.boolValue = false;
		}
		//Appearance
		GUILayout.Space (SECTION_SPACE);
		EditorGUILayout.LabelField ("Appearance", EditorStyles.boldLabel);
		EditorGUILayout.PropertyField (material, new GUIContent ("Material"));
		if (thickness.floatValue == 1f) {
			EditorGUILayout.PropertyField (useGradient, new GUIContent ("Use Gradient"));
		}
		if (useGradient.boolValue) {
			EditorGUILayout.PropertyField (gradientStyle, new GUIContent ("Gradient Style"));
			EditorGUILayout.PropertyField (gradient, new GUIContent ("Gradient"));
		} else {
			EditorGUILayout.PropertyField (m_Color, new GUIContent ("Color"));
		}
		//Glow
		if (borderRadius.floatValue == 0) {
			GUILayout.Space (SECTION_SPACE);
			EditorGUILayout.LabelField ("Glow", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField (shouldGlow, new GUIContent ("Glow"));
			if (shouldGlow.boolValue) {
				EditorGUILayout.PropertyField (useRectColor, new GUIContent ("Use Rect Color"));
				if (!useRectColor.boolValue) {
					EditorGUILayout.PropertyField (glowColor, new GUIContent ("Glow Color"));
				}
				EditorGUILayout.PropertyField (glowDistance, new GUIContent ("Glow Thickness"));
			}
		} else {

			GUILayout.Space (SECTION_SPACE);
			EditorGUILayout.LabelField ("Glow not available when border radius is not zero.", EditorStyles.boldLabel);
			GUILayout.Space (SECTION_SPACE);
		}

		serializedObject.ApplyModifiedProperties ();
	}
}
