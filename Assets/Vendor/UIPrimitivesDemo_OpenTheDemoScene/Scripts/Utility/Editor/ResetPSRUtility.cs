
using UnityEngine;
using UnityEditor;
using System.Collections;

namespace VRB.Utility
{
	public static class ResetPSRUtility
	{

		[MenuItem("GameObject/Reset PSR #%r", false, 0)]
		static void Init ()
		{
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
