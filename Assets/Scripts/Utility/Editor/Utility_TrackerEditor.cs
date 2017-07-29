
using UnityEngine;
using UnityEditor;
using System.Collections;

namespace ITHB.Utility
{
	[ExecuteInEditMode]
	[CustomEditor (typeof(Utility_Tracker))]
	class Utility_TrackerEditor : Editor {
		
		Transform[] children ;
		void OnEnable(){
			Transform myTransform = ((Utility_Tracker) target).transform;
			children = new Transform[myTransform.childCount];
			for (int i=0; i<children.Length; ++i) {
				children [i] = myTransform.GetChild (i);
			}
		}
		static readonly float disableColorValue=.85f;
		Color disabledColor = new Color(disableColorValue,disableColorValue,disableColorValue);
		public override void OnInspectorGUI ()
		{
			GUILayout.Space(10);
			GUILayout.BeginHorizontal ();

			Utility_Tracker tracker = (Utility_Tracker) target;

			for(int i=0;i<children.Length;++i)
			{
				GUIStyle currentStyle = EditorStyles.miniButtonMid;
				if(i==0)
					currentStyle=EditorStyles.miniButtonLeft;
				if(i==children.Length-1)
					currentStyle=EditorStyles.miniButtonRight;
				if(!children[i].gameObject.activeSelf){
					GUI.backgroundColor=disabledColor;
						GUI.color=disabledColor;
				}
				if(GUILayout.Button(children[i].gameObject.name,currentStyle)){
					tracker.SetActive(i);
				}
				GUI.backgroundColor=Color.white;
				GUI.color=Color.white;
			}

			GUILayout.EndHorizontal ();
			GUILayout.Space(10);
		}
		
		protected void OnSceneGUI()
		{
			Event e = Event.current;
			KeyCode keyCode = e.keyCode;
			if(e.type!=EventType.KeyDown)
				return;
			
			Utility_Tracker tracker = (Utility_Tracker) target;
			Transform transform = tracker.transform;
			
			for(int i=0;i<transform.childCount;++i)
			{
				if(transform.IsActiveTransform() && (keyCode == Utility.ALPHA_KEY_CODES[i]))
				{
					tracker.SetActive(i);
				}
			}
		}

#if !UNITY_EDITOR_WIN
		
		[MenuItem("GameObject/Select Tracker Camera #t", false, 0)]
		static void SelectTrackerCamera ()
		{
			Transform tracker = FindObjectOfType<Utility_Tracker>().transform;
			for(int i=0;i<tracker.childCount;++i)
			{
				if(tracker.GetChild(i).gameObject.activeSelf)
					Selection.activeTransform = tracker.GetChild(i);
			}
		}
		
		[MenuItem("GameObject/Tracker/Select Tracker #%t", false, 0)]
		static void SelectTracker ()
		{
			Transform tracker = FindObjectOfType<Utility_Tracker>().transform;

			Selection.activeTransform = tracker;
		}
		
		[MenuItem("GameObject/Tracker/Set Tracker Selection 1 _1", false, 0)]
		static void SelectTracker1 (){Utility_TrackerEditor.NumberPressed(1);}
		
		[MenuItem("GameObject/Tracker/Set Tracker Selection 2 _2", false, 0)]
		static void SelectTracker2 (){Utility_TrackerEditor.NumberPressed(2);}
		
		[MenuItem("GameObject/Tracker/Set Tracker Selection 3 _3", false, 0)]
		static void SelectTracker3 (){Utility_TrackerEditor.NumberPressed(3);}
		
		[MenuItem("GameObject/Tracker/Set Tracker Selection 4 _4", false, 0)]
		static void SelectTracker4 (){Utility_TrackerEditor.NumberPressed(4);}
#endif
		public static void NumberPressed(int number)
		{
			if(UnityEngine.Application.isPlaying)
				return;

			bool canAcceptNumberPress = false;
			bool setFocus = false;
			Transform focusTransform = null;
			Transform transform = FindObjectOfType<Utility_Tracker>().transform;

				
			if(transform.IsActiveTransform())
				canAcceptNumberPress = true;

			Object[] childObjects = new Object[transform.childCount];

			for(int i=0;i<transform.childCount;++i)
			{
				Transform child = transform.GetChild(i);
				childObjects[i] = (Object) child.gameObject;
				if(child.IsActiveTransform())
				{
					canAcceptNumberPress=true;
					setFocus=true;
				}
			}

			Undo.RecordObjects(childObjects,"Undo Set Active Tracker");

			if(canAcceptNumberPress)
			{
				transform.GetComponent<Utility_Tracker>().SetActive(number-1);
				if(setFocus)
					Selection.activeTransform = transform.GetChild(number-1);;
			}
		}
	}
}