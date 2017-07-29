using UnityEditor;
using UnityEngine;
using System;
using System.Collections;
using System.Linq;
using UIPrimitives;

namespace UIPrimitives
{
	[CanEditMultipleObjects]
	[CustomEditor (typeof(UICircleAnimator))]
	class UICircleAnimatorEditor : UIAnimatorEditor
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
				DrawEnabledDisableButtons<UICircleAnimation> ();
			} 
		}

		protected void DrawAddAnimationButtons(){
			GUILayout.Space (SECTION_SPACE);
			foreach (UICircleAnimation.CircleAnimationType animationType in Enum.GetValues(typeof(UICircleAnimation.CircleAnimationType))) {
				if (GUILayout.Button ("Add " + animationType + " Animation")) {
					for (int i=0; i<targets.Length; ++i) {
						UICircleAnimation newUICircleAnimation = ((UIAnimator)targets [i]).gameObject.AddComponent<UICircleAnimation> ();
						newUICircleAnimation.animationType = animationType;
					}
				}
			}
		}
	}
}