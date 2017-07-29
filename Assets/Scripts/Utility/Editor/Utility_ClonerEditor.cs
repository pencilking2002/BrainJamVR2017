
using UnityEngine;
using UnityEditor;
using System.Collections;

namespace ITHB.Utility
{
	[CustomEditor (typeof(Utility_Cloner))]
	class Utility_ClonerEditor : Editor {


		public override void OnInspectorGUI ()
		{
			this.DrawDefaultInspector();

			GUILayout.Space(10);

			if(GUILayout.Button("Clone")){
				((Utility_Cloner) target).CreateObjects();
			}
			GUILayout.Space(20);
			
			if(GUILayout.Button("Delete")){
				((Utility_Cloner) target).DeleteObjects();
			}

			GUILayout.Space(10);
		}
	}
}