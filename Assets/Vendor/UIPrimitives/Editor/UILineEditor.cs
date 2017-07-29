using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Linq;
using UIPrimitives;

namespace UIPrimitives
{
	[CanEditMultipleObjects]
	[CustomEditor (typeof(UILine))]
	class UILineEditor : Editor
	{
		//Dimensions
		SerializedProperty thickness;
		SerializedProperty fillPercent;
		SerializedProperty fillSubdivisions;
		//Smooth
		SerializedProperty smoothLine;
		SerializedProperty smoothSubdivisions;
		//Dotted
		SerializedProperty dottedLine;
		SerializedProperty dottedLineSubdivisions;
		/////// Caps ///////
		//Start
		SerializedProperty startCap;
		SerializedProperty startCapWidth;
		SerializedProperty startCapColor;
		SerializedProperty startCapThickness;
		SerializedProperty startCapGlow;
		//End
		SerializedProperty endCap;
		SerializedProperty endCapWidth;
		SerializedProperty endCapColor;
		SerializedProperty endCapThickness;
		SerializedProperty endCapGlow;
		//Appearance
		SerializedProperty useGradient;
		SerializedProperty gradient;
		SerializedProperty m_Color;
		SerializedProperty material;
		//Glow
		SerializedProperty shouldGlow;
		SerializedProperty glowColor;
		SerializedProperty glowDistance;
		SerializedProperty stitchEdges;
		//Variables
		UILine myUILine;

		void OnEnable ()
		{
			//Dimensions
			thickness = serializedObject.FindProperty ("thickness");
			stitchEdges = serializedObject.FindProperty ("stitchEdges");
			fillPercent = serializedObject.FindProperty ("fillPercent");
			fillSubdivisions = serializedObject.FindProperty ("fillSubdivisions");
			//Smooth
			smoothLine = serializedObject.FindProperty ("smoothLine");
			smoothSubdivisions = serializedObject.FindProperty ("smoothMultiplier");
			//Dotted
			dottedLine = serializedObject.FindProperty ("dottedLine");
			dottedLineSubdivisions = serializedObject.FindProperty ("dottedLineSubdivisions");
			////Caps
			//Start
			startCap = serializedObject.FindProperty ("startCap");
			startCapWidth = serializedObject.FindProperty ("startCapWidth");
			startCapColor = serializedObject.FindProperty ("startCapColor");
			startCapThickness = serializedObject.FindProperty ("startCapThickness");
			startCapGlow = serializedObject.FindProperty ("startCapGlow");
			//End
			endCap = serializedObject.FindProperty ("endCap");
			endCapWidth = serializedObject.FindProperty ("endCapWidth");
			endCapColor = serializedObject.FindProperty ("endCapColor");
			endCapThickness = serializedObject.FindProperty ("endCapThickness");
			endCapGlow = serializedObject.FindProperty ("endCapGlow");
			//Appearance
			useGradient = serializedObject.FindProperty ("useGradient");
			gradient = serializedObject.FindProperty ("gradient");
			m_Color = serializedObject.FindProperty ("m_Color");
			material = serializedObject.FindProperty ("m_Material");
			//Glow
			shouldGlow = serializedObject.FindProperty ("shouldGlow");
			glowColor = serializedObject.FindProperty ("glowColor");
			glowDistance = serializedObject.FindProperty ("glowDistance");
			//MyLine
			myUILine = (UILine)target;
		}

		AnimationCurve currentCurve;
		static readonly int SECTION_SPACE = 5;
		static string amount = "1";
		int intAmount;
		readonly Vector3 incrementAmount = new Vector3 (40f, 40f, 0);
		readonly float incrementThicknessAmount = .001f;

		public override void OnInspectorGUI ()
		{
			serializedObject.Update ();
			//Dimensions
			GUILayout.Space (SECTION_SPACE);
			EditorGUILayout.LabelField ("Dimensions", EditorStyles.boldLabel);
			//Thickness
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.PropertyField (thickness, new GUIContent ("Thickness"));
			//Increment buttons, no longer used
//			if (GUILayout.Button ("-", EditorStyles.miniButtonLeft)) {
//				myUILine.thickness -= incrementThicknessAmount;
//			}
//			if (GUILayout.Button ("+", EditorStyles.miniButtonRight)) {
//				myUILine.thickness += incrementThicknessAmount;
//			}
			EditorGUILayout.EndHorizontal ();
			EditorGUILayout.PropertyField (stitchEdges, new GUIContent ("Stitch Joints"));
			EditorGUILayout.PropertyField (fillPercent, new GUIContent ("Fill Percent"));
			EditorGUILayout.PropertyField (fillSubdivisions, new GUIContent ("Subdivisions"));
			//Smooth
			GUILayout.Space (SECTION_SPACE);
//		EditorGUILayout.LabelField ("Smooth", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField (smoothLine, new GUIContent ("Smooth Line"));
			if (smoothLine.boolValue) {
				EditorGUILayout.PropertyField (smoothSubdivisions, new GUIContent ("Subdivisions"));
			}
			//dotted
			GUILayout.Space (SECTION_SPACE);
//		EditorGUILayout.LabelField ("Dotted", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField (dottedLine, new GUIContent ("Dotted Line"));
			if (dottedLine.boolValue) {
				EditorGUILayout.PropertyField (dottedLineSubdivisions, new GUIContent ("Subdivisions"));
			}
			//Caps
			GUILayout.Space (SECTION_SPACE);
			EditorGUILayout.LabelField ("Caps", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField (startCap, new GUIContent ("Start Cap"));
			if (startCap.boolValue) {
				EditorGUILayout.PropertyField (startCapWidth, new GUIContent ("Start Cap Radius"));
				EditorGUILayout.PropertyField (startCapThickness, new GUIContent ("Start Cap Thickness"));
				EditorGUILayout.PropertyField (startCapColor, new GUIContent ("Start Cap Color"));;
				EditorGUILayout.PropertyField (startCapGlow, new GUIContent ("Start Cap Glow"));
			}
			GUILayout.Space (SECTION_SPACE);
			EditorGUILayout.PropertyField (endCap, new GUIContent ("End Cap"));
			if (endCap.boolValue) {
				EditorGUILayout.PropertyField (endCapWidth, new GUIContent ("End Cap Radius"));
				EditorGUILayout.PropertyField (endCapThickness, new GUIContent ("End Cap Thickness"));
				EditorGUILayout.PropertyField (endCapColor, new GUIContent ("End Cap Color"));
				EditorGUILayout.PropertyField (endCapGlow, new GUIContent ("End Cap Glow"));
			}
			//Appearance
			GUILayout.Space (SECTION_SPACE);
			EditorGUILayout.LabelField ("Appearance", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField (material, new GUIContent ("Material"));
			EditorGUILayout.PropertyField (useGradient, new GUIContent ("Use Gradient"));
			if (useGradient.boolValue) {
				EditorGUILayout.PropertyField (gradient, new GUIContent ("Gradient"));
			} else {
				EditorGUILayout.PropertyField (m_Color, new GUIContent ("Line Color"));
			}
			//Glow
			GUILayout.Space (SECTION_SPACE);
			EditorGUILayout.LabelField ("Glow", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField (shouldGlow, new GUIContent ("Glow"));
			if (shouldGlow.boolValue) {
				EditorGUILayout.PropertyField (glowColor, new GUIContent ("Glow Color"));
				EditorGUILayout.PropertyField (glowDistance, new GUIContent ("Glow Thickness"));
			}

			//Dimensions
			GUILayout.Space (SECTION_SPACE);
			EditorGUILayout.LabelField ("Nodes", EditorStyles.boldLabel);
			int nodesCount = myUILine.nodes.Count;
			//Add
			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Add ", EditorStyles.label, GUILayout.ExpandWidth (false));
			amount = GUILayout.TextField (amount, EditorStyles.textField, GUILayout.Width (40));
			GUILayout.Label (" Nodes", EditorStyles.label, GUILayout.ExpandWidth (false));
			if (GUILayout.Button ("Add", EditorStyles.miniButton)) {
				if (int.TryParse (amount, out intAmount)) {
					for (int i=0; i<intAmount; ++i) {
//						Vector3 offset = Vector3.Scale (incrementAmount, myUILine.transform.lossyScale);
//						if (nodesCount != 0 && nodesCount != 1)
//							offset = Vector3.Scale (myUILine.transform.lossyScale,));
						Vector3 offset;
						if(myUILine.nodes.Count>=2){
						offset= incrementAmount.x * Vector3.Normalize (myUILine.nodes [nodesCount - 1] - myUILine.nodes [nodesCount - 2]);
						}else{
							offset=new Vector3(30f,30f,0);
						}
						Vector3 nodePosition = Vector3.zero;
						if (nodesCount != 0)
							nodePosition = ((Vector3)myUILine.nodes [nodesCount - 1]);
						myUILine.nodes.Add (nodePosition + offset);
					
						nodesCount = myUILine.nodes.Count;
					}	
				}
			}
			GUILayout.EndHorizontal ();
			//node display:
			EditorGUI.indentLevel = 1;
			for (int i = 0; i < nodesCount; i++) {
				GUILayout.Label ("Node " + (i + 1), EditorStyles.label);
				GUILayout.BeginHorizontal ();
				myUILine.nodes [i] = EditorGUILayout.Vector2Field ("", myUILine.nodes [i], GUILayout.Height (10));
				if (GUILayout.Button ("X", EditorStyles.miniButtonRight)) {
					myUILine.nodes.RemoveAt (i);
				}
				GUILayout.EndHorizontal ();
				nodesCount = myUILine.nodes.Count;
			}
			GUILayout.Space (SECTION_SPACE);
			EditorGUI.indentLevel = 0;
		
			serializedObject.ApplyModifiedProperties ();
			if (GUI.changed) {
				EditorUtility.SetDirty (myUILine);
				myUILine.SetAllDirty_Public ();
			}	
		}

		void OnSceneGUI ()
		{		
			int nodesCount = myUILine.nodes.Count;
			if (nodesCount > 0) {
				//allow path adjustment undo:
				Undo.RecordObject (myUILine, "Adjust Line Path");

				Transform myUILineTransform = myUILine.transform;

				//node handle display:
				for (int i = 0; i < nodesCount; i++) {
					Vector3 nodePosition = Vector3.Scale (myUILine.nodes [i], Vector3.one);
					nodePosition = myUILineTransform.TransformPoint (myUILine.nodes [i]);
					Vector3 myPosition = Vector3.Scale (myUILine.transform.position, Vector3.one);
					Handles.lighting=false;
					Handles.Label (nodePosition , (i + 1) + "/" + (nodesCount), EditorStyles.whiteLargeLabel);
//					nodePosition.x = Handles.ScaleValueHandle (nodePosition.x, nodePosition, Quaternion.Euler (myUILineTransform.right), 1f, Handles.ArrowCap, 1f);
					myUILine.nodes [i] = myUILineTransform.InverseTransformPoint(Handles.PositionHandle (nodePosition,myUILineTransform.localRotation));
//					myUILine.nodes [i] = myUILineTransform.InverseTransformPoint (nodePosition);
					Handles.lighting=true;
				}	
			}	
			if (GUI.changed) {
				EditorUtility.SetDirty (target);
			
				myUILine.SetAllDirty_Public ();
			}
		}
	}
}