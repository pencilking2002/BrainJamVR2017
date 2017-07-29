using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Neuromancers
{
	public class ViveController : MotionController
	{
		//readonly

		//Serialized

		/////Protected/////
		//References
		protected SteamVR_ControllerManager controllerManager;
		protected SteamVR_TrackedObject trackedObject;
		protected SteamVR_Controller.Device device;
		//Primitives

		///////////////////////////////////////////////////////////////////////////
		//
		// Inherited from MonoBehaviour
		//
		
		protected override void Awake () {

			base.Awake();

//			if(transform.parent!=null)
				trackedObject = transform.parent.GetComponent<SteamVR_TrackedObject>();

			controllerManager = GetComponentInParent<SteamVR_ControllerManager>();
		}

		protected override void Start () {

			base.Start();

			device = SteamVR_Controller.Input(GetIndex());
		}

		protected override void Update () {

			base.Update ();

		}

		///////////////////////////////////////////////////////////////////////////
		//
		// Inherited from MotionController
		//

		protected override float GetJoystickDirectionPercent (Direction direction) {

			float percent = 0;
			Vector2 axis = device.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad);

			switch (direction) {

			case Direction.Top:

				percent = axis.y;
				break;
			case Direction.Bottom:

				percent = -axis.y;
				break;

			case Direction.Right:

				percent = axis.x;
				break;
			case Direction.Left:

				percent = -axis.x;
				break;
			}

			percent = Mathf.Clamp01(percent);

			return percent;
		}

		public override Handedness GetHandedness () {

			Handedness handedness = Handedness.Unknown;
			if (((uint)trackedObject.index) == controllerManager.leftIndex)
				handedness = Handedness.Left;
			if (((uint)trackedObject.index) == controllerManager.rightIndex)
				handedness = Handedness.Right;
			
			return handedness;
		}

		public override int GetIndex () {

			if(trackedObject==null)
				return -1;
			return (int) trackedObject.index;
		}

		public override bool IsActive () {

			if(trackedObject==null)
				return false;
			return trackedObject.gameObject.activeSelf;
		}

		protected override bool IsJoystickDown () {

			return device.GetPressDown(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad);
//			return OVRInput.GetDown(OVRInput.Button.PrimaryThumbstick,controllerType);
		}

		protected override bool IsMotionControlGripHeld() {

			return device.GetPress(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger);
//			return OVRInput.Get(OVRInput.Button.PrimaryHandTrigger,controllerType);
		}

		protected override void SetThumbstickNormal() {

//			thumbstickRenderer.sharedMaterial = thumbstickNormalMaterial;
		}

		protected override void SetThumbstickMenu() {

//			thumbstickRenderer.sharedMaterial = thumbstickMenuMaterial;
		}


		///////////////////////////////////////////////////////////////////////////
		//
		// ViveController Functions
		//


		///////////////////////////////////////////////////////////////////////////
		//
		// Inherited from IViveEventHandler
		//

//		public int GetIndex () {
//
//			return GetTrackedObjectIndex();
//		}
//
//		public void OnInteractiveGrip (List<InteractiveGripEventData> data) {
//			
//		}

//		public void OnMotionControlTouchpadDown () {
//
//			OnControllerInteraction();
//		}
//
//		public void OnMotionControlTouchpadUp () {
//
//			OnControllerInteraction();
//		}
//
//		public void OnMotionControlMenuDown () {
//
//			OnControllerInteraction();
//		}
//
//		public void OnMotionControlMenuUp () {
//
//			OnControllerInteraction();
//		}
//
//		public void OnMotionControlGripDown () {
//
//			OnControllerInteraction();
//		}
//
//		public void OnMotionControlGripUp () {
//
//			OnControllerInteraction();
//		}
//
//		public void OnMotionControlTriggerDown () {
//
//			OnControllerInteraction();
//		}
//
//		public void OnMotionControlTriggerUp () {
//
//			OnControllerInteraction();
//		}
	}
}
