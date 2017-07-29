using UnityEditor;
using UnityEngine;
using System;
using System.Collections;
using System.Linq;
using UIPrimitives;

namespace UIPrimitives
{
	[CanEditMultipleObjects]
	[CustomEditor (typeof(UILineAnimator))]
	class UILineAnimatorEditor : UIAnimatorEditor
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
				DrawEnabledDisableButtons<UILineAnimation> ();
			} 
		}

		protected void DrawAddAnimationButtons(){
			GUILayout.Space (SECTION_SPACE);
			foreach (UILineAnimation.LineAnimationType animationType in Enum.GetValues(typeof(UILineAnimation.LineAnimationType))) {
				if (GUILayout.Button ("Add " + animationType + " Animation")) {
					for (int i=0; i<targets.Length; ++i) {
						UILineAnimation newUIAnimation = ((UIAnimator)targets [i]).gameObject.AddComponent<UILineAnimation> ();
						newUIAnimation.animationType = animationType;
					}
				}
			}
		}
	}
}