using UnityEditor;
using UnityEngine;
using System;
using System.Collections;
using System.Linq;
using UIPrimitives;

namespace UIPrimitives
{
	[CanEditMultipleObjects]
	[CustomEditor (typeof(MaterialAnimator))]
	class MaterialAnimatorEditor : UIAnimatorEditor
	{
		
		///////////////////////////////////////////////////////////////////////////
		//
		// Inherited from Editor
		//

		public override void OnInspectorGUI ()
		{
			serializedObject.Update ();

			DrawDefaultInspector ();
			GUILayout.Label ("You can overwrite the material used here ^^^");

			//If we are not in play mode display the add, enable, and disable animation buttons
			if (!Application.isPlaying) {
				//Allow user to add animations using buttons
				DrawAddAnimationButtons ();
				//Draw Enable/Disable all animation buttons
				DrawEnabledDisableButtons<MaterialAnimation> ();
			} 
		}

		protected void DrawAddAnimationButtons(){
			GUILayout.Space (SECTION_SPACE);
			foreach (MaterialAnimation.AnimationType animationType in Enum.GetValues(typeof(MaterialAnimation.AnimationType))) {
				if (GUILayout.Button ("Add " + animationType + " Animation")) {
					for (int i=0; i<targets.Length; ++i) {
						MaterialAnimation newUIAnimation = ((UIAnimator)targets [i]).gameObject.AddComponent<MaterialAnimation> ();
						newUIAnimation.animationType = animationType;
					}
				}
			}
		}
	}
}