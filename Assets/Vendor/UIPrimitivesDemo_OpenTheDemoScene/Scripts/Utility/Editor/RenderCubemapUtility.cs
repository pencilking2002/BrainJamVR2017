
using UnityEngine;
using UnityEditor;
using System.Collections;

public class RenderCubemapUtility : ScriptableWizard {
	public Transform renderFromPosition;
	public Cubemap cubemap;
	
	void OnWizardUpdate () {

	}
	
	void OnWizardCreate () {
		// create temporary camera for rendering
		Camera mainCamera = Camera.main;
		GameObject go = new GameObject( "CubemapCamera");
		go.AddComponent<Camera>();
		// place it on the object
		go.transform.position = renderFromPosition.position;
		go.transform.rotation = Quaternion.identity;
		// render into cubemap	
		go.GetComponent<Camera>().farClipPlane=9999999f;	
		go.GetComponent<Camera>().backgroundColor = mainCamera.backgroundColor;
		go.GetComponent<Camera>().RenderToCubemap( cubemap );
		// destroy temporary camera
		DestroyImmediate( go );
	}
	
	[MenuItem("Custom/Render into Cubemap")]
	static void RenderCubemap () {
		ScriptableWizard.DisplayWizard<RenderCubemapUtility>(
			"Render cubemap", "Render!");
	}
}