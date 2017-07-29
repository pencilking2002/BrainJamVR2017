
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;

public class Utility_RenderCubemapWizard : ScriptableWizard {
	public Transform renderFromPosition;
	public Cubemap cubemap;
	
	void OnWizardUpdate () {

	}
	
	void OnWizardCreate () {
		// create temporary camera for rendering
		GameObject go = new GameObject( "CubemapCamera");
		go.AddComponent<Camera>();
		// place it on the object
		go.transform.position = renderFromPosition.position;
		go.transform.rotation = Quaternion.identity;
		// render into cubemap	
		go.GetComponent<Camera>().farClipPlane=9999999f;	
		if(Camera.main!=null)
		go.GetComponent<Camera>().backgroundColor = Camera.main.backgroundColor;
		go.GetComponent<Camera>().RenderToCubemap( cubemap );
		
		// destroy temporary camera
		DestroyImmediate( go );
	}
	
	[MenuItem("Custom/Render into Cubemap")]
	static void RenderCubemap () {
		ScriptableWizard.DisplayWizard<Utility_RenderCubemapWizard>(
			"Render cubemap", "Render!");
	}
}
#endif
