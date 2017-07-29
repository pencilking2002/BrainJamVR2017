using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace UIPrimitives
{
	public class UIAnimatorEditor : Editor
	{
	
		//readonly
		protected static readonly int SECTION_SPACE = 5;
	
		///////////////////////////////////////////////////////////////////////////
		//
		// UIAnimatorEditor Functions
		//

		/// <summary>
		/// Draws the enabled disable buttons.
		/// </summary>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		protected void DrawEnabledDisableButtons<T> () where T : UIAnimation
		{
			GUILayout.Space (SECTION_SPACE);
			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Enable All Animations", EditorStyles.miniButtonLeft)) {
				SetAnimationsEnabled<T> (true);
			}
			if (GUILayout.Button ("Disable All Animations", EditorStyles.miniButtonRight)) {
				SetAnimationsEnabled<T> (false);
			}
			GUILayout.EndHorizontal ();
		}
		
		/// <summary>
		/// Sets the animations enabled.
		/// </summary>
		/// <param name="value">If set to <c>true</c> value.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		protected void SetAnimationsEnabled<T> (bool value) where T : UIAnimation
		{
			for (int i=0; i<targets.Length; ++i) {
				foreach (T animation in  ((UIAnimator)targets[i]).gameObject.GetComponents<T>()) {
					animation.enabled = value;
				}
			}
		}
	}

}