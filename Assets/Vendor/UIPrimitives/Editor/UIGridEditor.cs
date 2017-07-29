using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Linq;
using UIPrimitives;

namespace UIPrimitives
{

[CanEditMultipleObjects]
[CustomEditor (typeof(UIGrid))]
class UIGridEditor : Editor
{
	
	

	
	//Dimensions
	SerializedProperty horizontalLineCount;
	SerializedProperty verticalLineCount;
	//Lines
	SerializedProperty drawGrid;
	SerializedProperty lineWidth;
	SerializedProperty fadeEdges;
	SerializedProperty fadeDistance;
	//Circles
	SerializedProperty drawCircles;
	SerializedProperty circleRadius;
	SerializedProperty circleSubdivisions;
	//Squares
	SerializedProperty drawSquares;
	SerializedProperty squareSize;
	//Appearance
	SerializedProperty m_Color;
	SerializedProperty material;
	//Glow
	SerializedProperty shouldGlow;
	SerializedProperty glowColor;
	SerializedProperty glowDistance;
	
	void OnEnable ()
	{
		//Dimensions
		horizontalLineCount = serializedObject.FindProperty ("horizontalLineCount");
		verticalLineCount = serializedObject.FindProperty ("verticalLineCount");
		//Lines
		drawGrid = serializedObject.FindProperty ("drawGrid");
		lineWidth = serializedObject.FindProperty ("lineWidth");
		fadeEdges = serializedObject.FindProperty ("fadeEdges");
		fadeDistance = serializedObject.FindProperty ("fadeDistance");
		//Circles
		drawCircles = serializedObject.FindProperty ("drawCircles");
		circleRadius = serializedObject.FindProperty ("circleRadius");
		circleSubdivisions = serializedObject.FindProperty ("circleSubdivisions");
		//Squares
		drawSquares = serializedObject.FindProperty ("drawSquares");
		squareSize = serializedObject.FindProperty ("squareSize");
		//Appearance
		m_Color = serializedObject.FindProperty ("m_Color");
		material = serializedObject.FindProperty ("m_Material");
		//Glow
		shouldGlow = serializedObject.FindProperty ("shouldGlow");
		glowColor = serializedObject.FindProperty ("glowColor");
		glowDistance = serializedObject.FindProperty ("glowDistance");
	}

	AnimationCurve currentCurve;
	static readonly int SECTION_SPACE = 5;

	public override void OnInspectorGUI ()
	{
		serializedObject.Update ();
		//Appearance
		GUILayout.Space (SECTION_SPACE);
		EditorGUILayout.LabelField ("Appearance", EditorStyles.boldLabel);
		EditorGUILayout.PropertyField (material, new GUIContent ("Material"));
		EditorGUILayout.PropertyField (m_Color, new GUIContent ("Color"));
		//Dimensions
		GUILayout.Space (SECTION_SPACE);
		EditorGUILayout.LabelField ("Dimensions", EditorStyles.boldLabel);
		EditorGUILayout.PropertyField (horizontalLineCount, new GUIContent ("Horizontal Line Count"));
		EditorGUILayout.PropertyField (verticalLineCount, new GUIContent ("Vertical Line Count"));
		
		//Lines
		GUILayout.Space (SECTION_SPACE);
		EditorGUILayout.LabelField ("Lines", EditorStyles.boldLabel);
		EditorGUILayout.PropertyField (drawGrid, new GUIContent ("Draw Lines"));
		if (drawGrid.boolValue) {
			EditorGUILayout.PropertyField (lineWidth, new GUIContent ("Line Width"));
			EditorGUILayout.PropertyField (fadeEdges, new GUIContent ("Fade Edges"));
			if(fadeEdges.boolValue){
			EditorGUILayout.PropertyField (fadeDistance, new GUIContent ("Fade Distance"));
			}
			
			//Glow
			GUILayout.Space (SECTION_SPACE);
			EditorGUILayout.LabelField ("Glow", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField (shouldGlow, new GUIContent ("Glow"));
			if (shouldGlow.boolValue) {
				EditorGUILayout.PropertyField (glowColor, new GUIContent ("Glow Color"));
				EditorGUILayout.PropertyField (glowDistance, new GUIContent ("Glow Thickness"));
			}
		}
		if (!drawSquares.boolValue) {
			//Circles
			GUILayout.Space (SECTION_SPACE);
			EditorGUILayout.LabelField ("Circles", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField (drawCircles, new GUIContent ("Draw Circles"));
			if (drawCircles.boolValue) {
				EditorGUILayout.PropertyField (circleRadius, new GUIContent ("Circle Radius"));
				EditorGUILayout.PropertyField (circleSubdivisions, new GUIContent ("Circle Subdivisions"));
			}
		}
		if (!drawCircles.boolValue) {
			//Squares
			GUILayout.Space (SECTION_SPACE);
			EditorGUILayout.LabelField ("Squares", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField (drawSquares, new GUIContent ("Draw Squares"));
			if (drawSquares.boolValue) {
				EditorGUILayout.PropertyField (squareSize, new GUIContent ("Square Size"));
			}
		}

		serializedObject.ApplyModifiedProperties ();
	}
}
}