using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
using OvrTouch.Controllers;

namespace Neuromancers.UI
{
	public class RiftInputModule : MotionInputModule
	{
		//readonly
		protected readonly float INDEX_TRIGGER_THRESHOLD = .5f;

		//Serialized
		
		/////Protected/////
		//References
		//Primitives

		//Properties
		public override InputModuleType InputModuleType {
			get {
				return InputModuleType.Rift;
			}
		}

		protected Dictionary<int,MotionController> deviceIndexToControllerDict = new Dictionary<int, MotionController>();

		///////////////////////////////////////////////////////////////////////////
		//
		// Inherited from Behaviour
		//
		
		protected void Awake()
		{

		}
		
		protected override void Start ()
		{
			base.Start ();

		}
		
		protected void Update ()
		{
			
		}

		///////////////////////////////////////////////////////////////////////////
		//
		// Inherited from MotionInputModule
		//

		public override MotionController[] GetControllers () {

			return new MotionController[] { GetLeftController(), GetRightController() };
		}

		public override MotionController GetLeftController () {

			return deviceIndexToControllerDict [GetLeftIndex ()];
		}

		public override MotionController GetRightController () {

			return deviceIndexToControllerDict [GetRightIndex ()];
		}

		protected override int GetControllerIndexFromModule(BaseRaycaster module) {

			RiftController controller = GetControllerFromModule (module);
			if (controller == null)
				return -1;
			return controller.GetIndex();
		}

		protected override int GetLeftIndex () {
			
			return (int) HandednessId.Left;
		}

		protected override int GetRightIndex () {
			
			return (int) HandednessId.Right;
		}

		protected override Vector2 GetLeftScroll() {

			return OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick,OVRInput.Controller.LTouch);
		}

		protected override Vector2 GetRightScroll() {

			return OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick,OVRInput.Controller.RTouch);
		}

		protected override bool IsLeftTriggerDown () {

			return OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger,OVRInput.Controller.LTouch);
		}

		protected override bool IsLeftTriggerHeld () {

			return OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger,OVRInput.Controller.LTouch);
		}

		protected override bool IsLeftTriggerUp () {

			return OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger,OVRInput.Controller.LTouch);
		}

		protected override bool IsRightTriggerDown () {

			return OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger,OVRInput.Controller.RTouch);
		}

		protected override bool IsRightTriggerHeld () {

			return OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger,OVRInput.Controller.RTouch);
		}

		protected override bool IsRightTriggerUp () {

			return OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger,OVRInput.Controller.RTouch);
		}


		//Button One

		protected override bool IsLeftButtonOneDown () {

			return OVRInput.GetDown(OVRInput.Button.One,OVRInput.Controller.LTouch);
		}

		protected override bool IsLeftButtonOneHeld () {

			return OVRInput.Get(OVRInput.Button.One,OVRInput.Controller.LTouch);
		}

		protected override bool IsLeftButtonOneUp () {

			return OVRInput.GetUp(OVRInput.Button.One,OVRInput.Controller.LTouch);
		}

		protected override bool IsRightButtonOneDown () {

			return OVRInput.GetDown(OVRInput.Button.One,OVRInput.Controller.RTouch);
		}

		protected override bool IsRightButtonOneHeld () {

			return OVRInput.Get(OVRInput.Button.One,OVRInput.Controller.RTouch);
		}

		protected override bool IsRightButtonOneUp () {

			return OVRInput.GetUp(OVRInput.Button.One,OVRInput.Controller.RTouch);
		}

		//grip

		protected override bool IsLeftGripDown () {

			return OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger,OVRInput.Controller.LTouch);
		}

		protected override bool IsLeftGripHeld () {

			return OVRInput.Get(OVRInput.Button.PrimaryHandTrigger,OVRInput.Controller.LTouch);
		}

		protected override bool IsLeftGripUp () {

			return OVRInput.GetUp(OVRInput.Button.PrimaryHandTrigger,OVRInput.Controller.LTouch);
		}

		protected override bool IsRightGripDown () {

			return OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger,OVRInput.Controller.RTouch);
		}

		protected override bool IsRightGripHeld () {

			return OVRInput.Get(OVRInput.Button.PrimaryHandTrigger,OVRInput.Controller.RTouch);
		}

		protected override bool IsRightGripUp () {

			return OVRInput.GetUp(OVRInput.Button.PrimaryHandTrigger,OVRInput.Controller.RTouch);
		}

		//thumb 

		protected override bool IsLeftThumbDown () {

			return OVRInput.GetDown(OVRInput.Button.PrimaryThumbstick,OVRInput.Controller.LTouch);
		}

		protected override bool IsRightThumbDown () {

			return OVRInput.GetDown(OVRInput.Button.PrimaryThumbstick,OVRInput.Controller.RTouch);
		}


		
		///////////////////////////////////////////////////////////////////////////
		//
		// RiftInputModule Functions
		//

		protected RiftController GetControllerFromModule (BaseRaycaster module) {

			//Debug.Log (module.gameObject.name);
			Transform parent = module.gameObject.transform.parent;
			if (parent == null)
				return null;
			return parent.GetComponent<RiftController> ();
		}

		protected override MotionController GetControllerFromDeviceIndex(int index) {

			MotionController motionController = null;

			if(deviceIndexToControllerDict.ContainsKey(index))
				motionController = deviceIndexToControllerDict[index];
			else  {
				
				motionController = (MotionController) TrackedController.FindOrCreate((HandednessId) index).RiftController;
				deviceIndexToControllerDict.Add(index,motionController);
			}

			return motionController;
		}

		
		////////////////////////////////////////
		//
		// Function Functions

	}
}
