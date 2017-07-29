using UnityEngine;
using UIPrimitives;
using System;
using System.Collections;
using System.Collections.Generic;
using OvrTouch.Controllers;

namespace Neuromancers
{
	public class RiftController : MotionController
	{
		//readonly
		protected readonly Dictionary<HandednessId,Handedness> HANDEDNESS_ID_TO_HANDEDNESS = new Dictionary<HandednessId, Handedness>() {

			{HandednessId.Left,Handedness.Left},
			{HandednessId.Right,Handedness.Right},
		};

		//Serialized
		[SerializeField]
		protected Material thumbstickNormalMaterial;
		[SerializeField]
		protected Renderer thumbstickRenderer;
		[SerializeField]
		protected MaterialAnimator primaryTriggerMaterialAnimator;
		[SerializeField]
		protected MaterialAnimator sideTriggerMaterialAnimator;
		
		/////Protected/////
		//References
		protected TouchController touchController;
		protected Material thumbstickMenuMaterial;
		//Primitives
		protected bool isAnimatingPrimaryTrigger;
		protected bool isAnimatingSideTrigger;
		protected OVRInput.Controller controllerType;
		

		///////////////////////////////////////////////////////////////////////////
		//
		// Inherited from MonoBehaviour
		//
		
		protected override void Awake () {

			base.Awake ();

			touchController = GetComponent<TouchController> ();
			controllerType = GetControllerType();

			if(thumbstickRenderer!=null)
				thumbstickMenuMaterial = thumbstickRenderer.sharedMaterial;
		}

		protected override void Start () {

			base.Start ();
		}

		protected override void Update () {

			base.Update ();

			bool shouldBeAnimatingSideTrigger = InputManager.Instance.IsMotionControlGripHovering(GetIndex()) && !IsMotionControlGripHeld() && !sideTriggerMaterialAnimator.IsAnimating();
			if(shouldBeAnimatingSideTrigger!=isAnimatingSideTrigger) {

				isAnimatingSideTrigger = shouldBeAnimatingSideTrigger;
				if(isAnimatingSideTrigger)
					sideTriggerMaterialAnimator.AddColorEndAnimation(Color.green,"_EmissionColor",.5f, UIAnimationUtility.EaseType.easeOutSine,1,onCompleteAction: OnAnimationComplete);
			}

			bool shouldBeAnimatingPrimaryTrigger = hitInteractable && !IsMotionControlTriggerHeld() && !primaryTriggerMaterialAnimator.IsAnimating();
			if(shouldBeAnimatingPrimaryTrigger!=isAnimatingPrimaryTrigger) {

				isAnimatingPrimaryTrigger = shouldBeAnimatingPrimaryTrigger;
				if(isAnimatingPrimaryTrigger)
					primaryTriggerMaterialAnimator.AddColorEndAnimation(Color.green,"_EmissionColor",.5f, UIAnimationUtility.EaseType.easeOutSine,1,onCompleteAction: OnAnimationComplete);
			}

		}

		///////////////////////////////////////////////////////////////////////////
		//
		// Inherited from MotionController
		//

		protected override float GetJoystickDirectionPercent (Direction direction) {

			float percent = 0;
			OVRInput.Controller controller = GetControllerType();
			Vector2 axis = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick,controller);

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
			return this.HANDEDNESS_ID_TO_HANDEDNESS[this.touchController.GetHandedness()];
		}

		public override int GetIndex () {

			return touchController.GetIndex ();
		}

		public override bool IsActive () {

			return (OVRInput.GetConnectedControllers () & controllerType) != OVRInput.Controller.None;
		}

		protected override bool IsJoystickDown () {

			return OVRInput.GetDown(OVRInput.Button.PrimaryThumbstick,controllerType);
		}

		protected override bool IsMotionControlGripHeld() {

			return OVRInput.Get(OVRInput.Button.PrimaryHandTrigger,controllerType);
		}

		protected override void SetThumbstickNormal() {

			thumbstickRenderer.sharedMaterial = thumbstickNormalMaterial;
		}

		protected override void SetThumbstickMenu() {

			thumbstickRenderer.sharedMaterial = thumbstickMenuMaterial;
		}

		///////////////////////////////////////////////////////////////////////////
		//
		// RiftController Functions
		//


		protected bool IsMotionControlTriggerHeld() {

			return OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger,controllerType);
		}

		protected OVRInput.Controller GetControllerType () {

			return touchController.GetIndex () == 0 ? OVRInput.Controller.LTouch : OVRInput.Controller.RTouch;
		}

		////////////////////////////////////////
		//
		// Event functions

		protected void OnAnimationComplete() {

			isAnimatingSideTrigger = false;
		}
	}
}
