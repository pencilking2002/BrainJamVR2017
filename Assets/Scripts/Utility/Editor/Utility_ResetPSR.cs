
using UnityEngine;
using UnityEditor;
using System.Collections;

namespace ITHB.Utility
{
	public static class Utility_ResetPSR
	{

		[MenuItem("GameObject/Reset PSR #%r", false, 0)]
		static void Init ()
		{
			// create temporary camera for rendering
			Transform activeTransform = Selection.activeTransform;
			Undo.RecordObject (activeTransform, "Edit Transform");
			if (activeTransform != null) {
				activeTransform.localPosition = Vector3.zero;;
				activeTransform.localRotation = Quaternion.identity;
				activeTransform.localScale = Vector3.one;
			}
		}
	}
}