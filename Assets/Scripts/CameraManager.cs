using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Neuromancers
{
	public class CameraManager : Singleton<CameraManager>
	{
		//readonly

		//Serialized
		
		/////Protected/////
		//References
		protected Transform activeChild;
		//Primitives

		public Transform ActiveChild {
			get {
				return this.activeChild;
			}
		}
		

		///////////////////////////////////////////////////////////////////////////
		//
		// Inherited from MonoBehaviour
		//
		
		protected void Awake () {

			for (int i = 0; i < transform.childCount; ++i) {

				if(transform.GetChild(i).gameObject.activeSelf)
					activeChild = transform.GetChild(i);
			}
		}

		protected void Start () {


			Transform prototypeCameraTransform = transform.Find ("PrototypeCamera");
			if (prototypeCameraTransform == null)
				return;
			Camera prototypeCamera = prototypeCameraTransform.GetComponent<Camera> ();
			Camera mainCamera = GetMainCamera();

			ApplyCameraComponents (prototypeCamera,mainCamera);
			MoveChildren (prototypeCamera,mainCamera);
		}

		protected void Update () {
			
		}

		///////////////////////////////////////////////////////////////////////////
		//
		// CameraManager Functions
		//

		protected void MoveChildren(Camera prototypeCamera, Camera mainCamera) {

			while(prototypeCamera.transform.childCount>0) {
				
				prototypeCamera.transform.GetChild(0).SetParent(mainCamera.transform);
			}
		}

		protected void ApplyCameraComponents (Camera prototypeCamera, Camera mainCamera) {
			

//			foreach (Component component in prototypeCamera.gameObject.GetComponents<Component>()) {
//
//				if(component!=prototypeCamera && component.GetType() != typeof(Transform)) 
//					CopyComponent(component,mainCamera.gameObject);
//			}
//			ColorSuite prototypeColorSuite = prototypeCamera.GetComponent<ColorSuite>();
//			if(prototypeColorSuite!=null) {
//
////				CopyComponent<ColorSuite>(prototypeColorSuite,mainCamera.gameObject);
//				ColorSuite colorSuite = mainCamera.gameObject.AddComponent<ColorSuite>();
//
//				colorSuite.colorTemp = prototypeColorSuite.colorTemp;
//				colorSuite.colorTint = prototypeColorSuite.colorTint;
//
//				colorSuite.saturation = prototypeColorSuite.saturation;
//
//				colorSuite.redCurve = prototypeColorSuite.redCurve;
//				colorSuite.blueCurve = prototypeColorSuite.blueCurve;
//				colorSuite.greenCurve = prototypeColorSuite.greenCurve;
//				colorSuite.rgbCurve = prototypeColorSuite.rgbCurve;
//
//				colorSuite.ditherMode = prototypeColorSuite.ditherMode;
//			}
		}

		protected T CopyComponent<T>(T original, GameObject destination) where T : Component
		{
			System.Type type = original.GetType();
			var dst = destination.GetComponent(type) as T;
			if (!dst) dst = destination.AddComponent(type) as T;
			var fields = type.GetFields();
			foreach (var field in fields)
			{
				if (field.IsStatic) continue;
				field.SetValue(dst, field.GetValue(original));
			}
			var props = type.GetProperties();
			foreach (var prop in props)
			{
				if (!prop.CanWrite || !prop.CanWrite || prop.Name == "name") continue;
				prop.SetValue(dst, prop.GetValue(original, null), null);
			}
			return dst as T;
		}

		protected Component CopyComponent (Component original, GameObject destination) {
			
			System.Type type = original.GetType ();
			Component copy = destination.AddComponent (type);
			// Copied fields can be restricted with BindingFlags
			System.Reflection.FieldInfo[] fields = type.GetFields (); 
			foreach (System.Reflection.FieldInfo field in fields) {
				field.SetValue (copy, field.GetValue (original));
			}
			return copy;
		}

		protected Camera GetMainCamera () {

			Camera mainCamera = Camera.main;
			//SteamVR camera hack
			if(mainCamera.gameObject.name == "Camera (head)")
				mainCamera = mainCamera.transform.Find("Camera (eye)").GetComponent<Camera>();

			return mainCamera;
		}
		
		////////////////////////////////////////
		//
		// Function Functions

	}
}