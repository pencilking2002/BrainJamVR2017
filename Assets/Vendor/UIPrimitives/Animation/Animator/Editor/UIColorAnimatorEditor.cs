using UnityEditor;
using UnityEngine;
using System;
using System.Collections;
using System.Linq;
using UIPrimitives;

namespace UIPrimitives
{
	[CanEditMultipleObjects]
	[CustomEditor (typeof(UIColorAnimator))]
	class UIColorAnimatorEditor : UIAnimatorEditor
	{
		
		///////////////////////////////////////////////////////////////////////////
		//
		// Inherited from Editor
		//

		public override void OnInspectorGUI ()
		{
			serializedObject.Update ();
			//If we are not in play mode display the add, enable, and disable animation buttons
			if (!Application.isPlaying) {
				//Allow user to add animations using buttons
				DrawAddAnimationButtons ();
				//Draw Enable/Disable all animation buttons
				DrawEnabledDisableButtons<UIColorAnimation> ();
			} 
		}

		protected void DrawAddAnimationButtons(){
			GUILayout.Space (SECTION_SPACE);
			foreach (UIColorAnimation.ColorAnimationType animationType in Enum.GetValues(typeof(UIColorAnimation.ColorAnimationType))) {
				if (GUILayout.Button ("Add " + animationType + " Animation")) {
					for (int i=0; i<targets.Length; ++i) {
						UIColorAnimation newUIAnimation = ((UIAnimator)targets [i]).gameObject.AddComponent<UIColorAnimation> ();
						newUIAnimation.animationType = animationType;
					}
				}
			}
		}
	}
}