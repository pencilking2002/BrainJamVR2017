using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Neuromancers
{
	[CustomEditor(typeof(AnimatedTextureUtility))]
	public class AnimatedTextureUtilityEditor : Editor
	{
		//readonly

		//Serialized
		
		/////Protected/////
		//References
		//Primitives
		

		///////////////////////////////////////////////////////////////////////////
		//
		// Inherited from Editor
		//

		SerializedProperty frames;

		protected  void OnEnable()
		{
			frames = serializedObject.FindProperty("frames");	
		}

		static readonly float disableColorValue=.85f;
		Color disabledColor = new Color(disableColorValue,disableColorValue,disableColorValue);
		public override void OnInspectorGUI ()
		{
			DrawDefaultInspector();

			AnimatedTextureUtility myPodMeshScript = ((AnimatedTextureUtility) target);
			GUILayout.Space(10);
			if(GUILayout.Button("Organize",EditorStyles.miniButton)){
				Organize(myPodMeshScript);
			}
		}


		// Use this for initialization
		void Organize (AnimatedTextureUtility podMeshScript)
		{
			Texture2D[] array=podMeshScript.frames;
			Texture2D[] arrayNew=new Texture2D[array.Length];
			for(int i=0;i<array.Length;++i){
				string name=array[i].name;
				name = name.Replace(" at frame 1","");
				name = name.Replace("Image ","");
				int index=int.Parse(name);
				arrayNew[index-1]=array[i];
			}
			for(int i=0;i<array.Length;++i){
				array[i]=arrayNew[i];
			}
			podMeshScript.frames=array;
		}
		
		///////////////////////////////////////////////////////////////////////////
		//
		// AnimatedTextureUtilityEditor Functions
		//

		
		////////////////////////////////////////
		//
		// Function Functions

	}
}
