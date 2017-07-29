using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Neuromancers.UI {
	public class ViveInputModule : MotionInputModule {
		//readonly
		protected readonly float INDEX_TRIGGER_THRESHOLD = .5f;

		//Serialized
		[SerializeField]
		protected SteamVR_ControllerManager controllerManager;
		[SerializeField]
		protected ViveController leftController;
		[SerializeField]
		protected ViveController rightController;
		
		/////Protected/////
		//References
		protected Dictionary<int,MotionController> deviceIndexToControllerDict = new Dictionary<int, MotionController> ();
		//Primitives

		//Properties
		public override InputModuleType InputModuleType {
			get {
				return InputModuleType.Vive;
			}
		}


		///////////////////////////////////////////////////////////////////////////
		//
		// Inherited from Behaviour
		//
		
		protected void Awake () {

		}

		protected override void Start () {
			base.Start ();

		}

		protected void Update () {

		}

		///////////////////////////////////////////////////////////////////////////
		//
		// Inherited from MotionInputModule
		//

		protected override int GetControllerIndexFromModule (BaseRaycaster module) {

			ViveController controller = GetControllerFromModule (module);
			if (controller == null)
				return -1;
			return controller.GetIndex ();
		}

		public override MotionController[] GetControllers () {

			return new MotionController[] { GetLeftController(), GetRightController() };
		}

		public override MotionController GetLeftController () {

			return (MotionController)leftController;
		}

		public override MotionController GetRightController () {

			return (MotionController)rightController;
		}

		protected override int GetLeftIndex () {
			
			return leftController.GetIndex ();
		}

		protected override int GetRightIndex () {
			
			return rightController.GetIndex ();
		}

		protected SteamVR_Controller.Device GetLeftDevice () {

			return SteamVR_Controller.Input (GetLeftIndex ());
		}

		protected SteamVR_Controller.Device GetRightDevice () {

			return SteamVR_Controller.Input (GetRightIndex ());
		}

		protected override Vector2 GetLeftScroll () {

//			return OVRInput.Get (OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.LTouch);
			return GetScroll (GetLeftDevice ());
		}

		protected override Vector2 GetRightScroll () {

//			return OVRInput.Get (OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.RTouch);
			return GetScroll (GetRightDevice ());
		}

		protected Vector2 GetScroll (SteamVR_Controller.Device device) {

			if (device == null)
				return Vector2.zero;
			return device.GetAxis (Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad);
		}

		protected override bool IsLeftTriggerDown () {
			
			return IsTriggerDown (GetLeftDevice ());
		}

		protected override bool IsLeftTriggerHeld () {
			
			return IsTriggerHeld (GetLeftDevice ());
		}

		protected override bool IsLeftTriggerUp () {
			
			return IsTriggerUp (GetLeftDevice ());
		}

		protected override bool IsRightTriggerDown () {
			
			return IsTriggerDown (GetRightDevice ());
		}

		protected override bool IsRightTriggerHeld () {
			
			return IsTriggerHeld (GetRightDevice ());
		}

		protected override bool IsRightTriggerUp () {
			
			return IsTriggerUp (GetRightDevice ());
		}

		protected bool IsTriggerDown (SteamVR_Controller.Device device) {

			if (device == null)
				return false;
			return device.GetPressDown (Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger);
		}

		protected bool IsTriggerHeld (SteamVR_Controller.Device device) {

			if (device == null)
				return false;
			return device.GetPress (Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger);
		}

		protected bool IsTriggerUp (SteamVR_Controller.Device device) {

			if (device == null)
				return false;
			return device.GetPressUp (Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger);
		}

		//Button One

		protected override bool IsLeftButtonOneDown () {

			return IsButtonOneDown (GetLeftDevice ());
		}

		protected override bool IsLeftButtonOneHeld () {

			return IsButtonOneHeld (GetLeftDevice ());
		}

		protected override bool IsLeftButtonOneUp () {

			return IsButtonOneUp (GetLeftDevice ());
		}

		protected override bool IsRightButtonOneDown () {

			return IsButtonOneDown (GetRightDevice ());
		}

		protected override bool IsRightButtonOneHeld () {

			return IsButtonOneHeld (GetRightDevice ());
		}

		protected override bool IsRightButtonOneUp () {

			return IsButtonOneUp (GetRightDevice ());
		}

		protected bool IsButtonOneDown (SteamVR_Controller.Device device) {

			if (device == null)
				return false;
			return device.GetPressDown (Valve.VR.EVRButtonId.k_EButton_ApplicationMenu);
		}

		protected bool IsButtonOneHeld (SteamVR_Controller.Device device) {

			if (device == null)
				return false;
			return device.GetPress (Valve.VR.EVRButtonId.k_EButton_ApplicationMenu);
		}

		protected bool IsButtonOneUp (SteamVR_Controller.Device device) {

			if (device == null)
				return false;
			return device.GetPressUp (Valve.VR.EVRButtonId.k_EButton_ApplicationMenu);
		}

		//grip

		protected override bool IsLeftGripDown () {

			return IsGripDown (GetLeftDevice ());
		}

		protected override bool IsLeftGripHeld () {

			return IsGripHeld (GetLeftDevice ());
		}

		protected override bool IsLeftGripUp () {

			return IsGripUp (GetLeftDevice ());
		}

		protected override bool IsRightGripDown () {

			return IsGripDown (GetRightDevice ());
		}

		protected override bool IsRightGripHeld () {

			return IsGripHeld (GetRightDevice ());
		}

		protected override bool IsRightGripUp () {

			return IsGripUp (GetRightDevice ());
		}

		protected bool IsGripDown (SteamVR_Controller.Device device) {

			if (device == null)
				return false;
			return device.GetPressDown (Valve.VR.EVRButtonId.k_EButton_Grip);
		}

		protected bool IsGripHeld (SteamVR_Controller.Device device) {

			if (device == null)
				return false;
			return device.GetPress (Valve.VR.EVRButtonId.k_EButton_Grip);
		}

		protected bool IsGripUp (SteamVR_Controller.Device device) {

			if (device == null)
				return false;
			return device.GetPressUp (Valve.VR.EVRButtonId.k_EButton_Grip);
		}

		//thumb

		protected override bool IsLeftThumbDown () {

//			return OVRInput.GetDown (OVRInput.Button.PrimaryThumbstick, OVRInput.Controller.LTouch);
			return IsThumbDown (GetLeftDevice ());
		}

		protected override bool IsRightThumbDown () {

//			return OVRInput.GetDown (OVRInput.Button.PrimaryThumbstick, OVRInput.Controller.RTouch);
			return IsThumbDown (GetRightDevice ());
		}


		protected bool IsThumbDown (SteamVR_Controller.Device device) {

			if (device == null)
				return false;
			return device.GetPressDown (Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad);
		}

		
		///////////////////////////////////////////////////////////////////////////
		//
		// ViveInputModule Functions
		//

		protected ViveController GetControllerFromModule (BaseRaycaster module) {

			//Debug.Log (module.gameObject.name);
			Transform parent = module.gameObject.transform.parent;
			if (parent == null)
				return null;
			return parent.GetComponent<ViveController> ();
		}

		protected override MotionController GetControllerFromDeviceIndex (int index) {

			MotionController motionController = null;

			if (deviceIndexToControllerDict.ContainsKey (index))
				motionController = deviceIndexToControllerDict [index];
			else {

				GameObject controllerGameObject = (index == (int)controllerManager.leftIndex) ? controllerManager.left : controllerManager.right;
				motionController = controllerGameObject.GetComponentInChildren<MotionController> ();
				deviceIndexToControllerDict.Add (index, motionController);
			}

			return motionController;
		}


		//		protected Transform GetTransformFromDevice(SteamVR_Controller.Device device) {
		//
		//			SteamVR_TrackedObject[] trackedControllers = FindObjectsOfType<SteamVR_TrackedObject>();
		//			foreach (SteamVR_TrackedObject tracked in trackedControllers)
		//			{
		//				if (((int) tracked.index) == device.index)
		//				{
		//					return tracked.transform;
		//				}
		//			}
		//			return null;
		//		}
		//
		//		protected ViveController GetViveControllerFromDevice(SteamVR_Controller.Device device) {
		//
		//			Transform transform = GetTransformFromDevice(device);
		//			if(transform==null)
		//				return null;
		//			return transform.GetComponentInChildren<ViveController>();
		//		}

		////////////////////////////////////////
		//
		// Function Functions

	}
}
