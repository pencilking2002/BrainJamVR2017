using UnityEngine;
using System.Collections;

namespace ITHB.Utility
{
	[ExecuteInEditMode]
	public class Utility_Tracker : MonoBehaviour
	{
		
		public void SetActive (int index)
		{
			for (int i=0; i<transform.childCount; ++i) {
				#if UNITY_EDITOR
				UnityEditor.Undo.RecordObject((Object) transform.GetChild(i).gameObject, "Undo Tracker Set Active");
				#endif
				transform.GetChild(i).gameObject.SetActive(index==i);
			}
		}
	}
}