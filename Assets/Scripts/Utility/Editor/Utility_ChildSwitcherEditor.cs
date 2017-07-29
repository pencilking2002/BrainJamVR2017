#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor (typeof(Utility_ChildSwitcher))]
class Utility_ChildSwitcherEditor : Editor {
	
	static readonly float disableColorValue=.85f;

	Color disabledColor = new Color(disableColorValue,disableColorValue,disableColorValue);
	Transform[] children ;

	protected void OnEnable()
	{
		Transform myTransform = ((Utility_ChildSwitcher) target).transform;
		children = new Transform[myTransform.childCount];
		for (int i=0; i<children.Length; ++i) {
			children [i] = myTransform.GetChild (i);
		}
	}

	public override void OnInspectorGUI ()
	{
		GUILayout.Space(10);
//		GUILayout.BeginHorizontal ();

		for(int i=0;i<children.Length;++i){
//			GUIStyle currentStyle = EditorStyles.;
//			if(i==0)
//				currentStyle=EditorStyles.miniButtonLeft;
//			if(i==children.Length-1)
//				currentStyle=EditorStyles.miniButtonRight;
			if(!children[i].gameObject.activeSelf){
				GUI.backgroundColor=disabledColor;
					GUI.color=disabledColor;
			}
			if(GUILayout.Button(children[i].gameObject.name)){
				SetActive(i);
			}
			GUI.backgroundColor=Color.white;
			GUI.color=Color.white;
		}
		GUILayout.Space(20);

		if(GUILayout.Button("None")){
			SetNoneActive();
		}
		GUILayout.Space(10);
		if(GUILayout.Button("All")){
			SetAllActive();
		}
	}

	protected void SetActive (int index)
	{
		for (int i=0; i<children.Length; ++i) {
			Undo.RecordObject( children[i].gameObject as Object, "Undo Set Active "+children[i].gameObject.name);
			children[i].gameObject.SetActive(index==i);
		}
	}
	protected void SetNoneActive ()
	{
		for (int i=0; i<children.Length; ++i) {
			Undo.RecordObject( children[i].gameObject as Object, "Undo Set Active "+children[i].gameObject.name);
			children[i].gameObject.SetActive(false);
		}
	}
	protected void SetAllActive ()
	{
		for (int i=0; i<children.Length; ++i) {
			Undo.RecordObject( children[i].gameObject as Object, "Undo Set Active "+children[i].gameObject.name);
			children[i].gameObject.SetActive(true);
		}
	}
}
#endif
