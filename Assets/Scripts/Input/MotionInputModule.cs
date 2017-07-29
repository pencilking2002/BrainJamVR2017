using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
using Neuromancers.UI;

namespace Neuromancers.UI {
	public abstract class MotionInputModule : InputModule {
		//structs
		protected struct TriggerEvent {

			public int deviceIndex;
			public TriggerEventType eventType;
			public RaycastResult raycastResult;

			public TriggerEvent (int deviceIndex, TriggerEventType eventType, RaycastResult raycastResult)
			{

				this.deviceIndex = deviceIndex;
				this.eventType = eventType;
				this.raycastResult = raycastResult;
			}
		}

		protected struct TriggerData {
			public bool didTrigger;
			public bool didClickSomething;
			public List<TriggerEvent> triggerEvents;

			public TriggerData (bool didTrigger, bool didClickSomething, List<TriggerEvent> triggerEvents)
			{
				this.didTrigger = didTrigger;
				this.didClickSomething = didClickSomething;
				this.triggerEvents = triggerEvents;
			}
			
		}

		//readonly
		protected readonly int MAX_CHECK_COUNT = 4;

		//public static
		public static MotionController.MotionControllerTutorialType[] tutorialTypes;

		//protected
		protected static readonly float MAX_TAP_DURATION = 1f;
		protected float lastMouseDownTime = -MAX_TAP_DURATION;

		///////////////////////////////////////////////////////////////////////////
		//
		// Inherited from Behaviour
		//

		protected virtual void Start () {

			foreach (var x in FindObjectsOfType<UnityEngine.UI.GraphicRaycaster>())
				x.enabled = false;
		}

		///////////////////////////////////////////////////////////////////////////
		//
		// Inherited from InputModule
		//

		public override void Process () {

			HandleRaycast ();
			UpdateCurrentObject ();
			HandleTrigger ();
			HandleGrip ();
			HandleScroll ();
			HandleThumbDown();
//			HandleBack ();
//			HandleTouchpad ();
		}

		///////////////////////////////////////////////////////////////////////////
		//
		// MotionInputModule Functions
		//

		protected void HandleRaycast () {
			
			if (pointerData == null) {
				pointerData = new PointerEventData (eventSystem);
			}
			m_RaycastResultCache.Clear ();
			pointerData.Reset ();
			pointerData.position = new Vector2 (0, 0);//Doesn't matter cause the MotionControlRaycaster ignores this
			eventSystem.RaycastAll (pointerData, m_RaycastResultCache);

			List<RaycastResult> resultsToRemove = new List<RaycastResult> ();

			List<InteractiveRaycastEventData> interactiveGazeEventDataList = new List<InteractiveRaycastEventData> ();

			foreach (RaycastResult rayResult in m_RaycastResultCache) {
				
				Element interactableComponent = null; 
				Transform transformToCheckForInteractableComponent = rayResult.gameObject.transform;
				int checkCount = 0;
				while (transformToCheckForInteractableComponent != null && interactableComponent == null && checkCount < MAX_CHECK_COUNT) {
					interactableComponent = transformToCheckForInteractableComponent.GetComponent<Element> ();
					checkCount++;
					transformToCheckForInteractableComponent = transformToCheckForInteractableComponent.parent;
				}

				bool didHitValidComponent = (interactableComponent != null);
				int index = GetControllerIndexFromModule (rayResult.module);

				if (didHitValidComponent) {
					interactiveGazeEventDataList.Add (new InteractiveRaycastEventData (InteractiveEventType.HitInteractable, rayResult, interactableComponent, index));	

				} else {						
					interactiveGazeEventDataList.Add (new InteractiveRaycastEventData (InteractiveEventType.HitCollider, rayResult, null, index));	

					resultsToRemove.Add (rayResult);
				}
			}
			if (m_RaycastResultCache == null || m_RaycastResultCache.Count == 0) {
				interactiveGazeEventDataList.Add (new InteractiveRaycastEventData (InteractiveEventType.HitNone, new RaycastResult (), null, -1));	
			}

			if (InteractiveRaycastAction != null) {
				InteractiveRaycastAction (interactiveGazeEventDataList);
			}

			foreach (RaycastResult rayResult in resultsToRemove) {
				m_RaycastResultCache.Remove (rayResult);
			}
			pointerData.pointerCurrentRaycast = FindFirstRaycast (m_RaycastResultCache);

//			Debug.Log(pointerData.pointerCurrentRaycast.gameObject.name);
		}

		protected void HandleTrigger () {
			
			TriggerData triggerData = new TriggerData (false, false, new List<TriggerEvent> ());

			bool triggered = false;

			int leftMostIndex = GetLeftIndex ();
			MotionController leftMostController = GetControllerFromDeviceIndex (leftMostIndex);
			int rightMostIndex = GetRightIndex ();
			MotionController rightMostController = GetControllerFromDeviceIndex (rightMostIndex);

			bool isLeftTriggerDown = IsLeftTriggerDown () || IsLeftButtonOneDown ();
			bool isLeftTriggerHeld = IsLeftTriggerHeld () || IsLeftButtonOneHeld ();
			bool isLeftTriggerUp = IsLeftTriggerUp () || IsLeftButtonOneUp ();
			bool isRightTriggerDown = IsRightTriggerDown () || IsRightButtonOneDown ();
			bool isRightTriggerHeld = IsRightTriggerHeld () || IsRightButtonOneHeld ();
			bool isRightTriggerUp = IsRightTriggerUp () || IsRightButtonOneUp ();

			//OBJECT SPECIFIC
			HashSet<BaseRaycaster> baseRaycasters = new HashSet<BaseRaycaster> ();
			foreach (RaycastResult raycastResult in m_RaycastResultCache) {

				if (raycastResult.gameObject == null)
					continue;
				if (baseRaycasters.Contains (raycastResult.module)) { 
				//	Debug.Log ("base contains match, continuing");
					continue;
				}

				bool triggerDown = false;
				bool triggerHeld = false;
				bool triggerUp = false;

				//Trigger down
				if (isLeftTriggerDown && ControllerMatchesRaycast (leftMostController, raycastResult)) {
					
					triggerData.triggerEvents.Add (new TriggerEvent (GetLeftIndex (), TriggerEventType.TriggerDown, raycastResult));
					triggerDown = true;
				}
				if (isRightTriggerDown && ControllerMatchesRaycast (rightMostController, raycastResult)) {
					
					triggerData.triggerEvents.Add (new TriggerEvent (GetRightIndex (), TriggerEventType.TriggerDown, raycastResult));
					triggerDown = true;
				}

				//Trigger held
				if (isLeftTriggerHeld && ControllerMatchesRaycast (leftMostController, raycastResult)) {

					triggerData.triggerEvents.Add (new TriggerEvent (GetLeftIndex (), TriggerEventType.TriggerHeld, raycastResult));
					triggerHeld = true;
				}
				if (isRightTriggerHeld && ControllerMatchesRaycast (rightMostController, raycastResult)) {

					triggerData.triggerEvents.Add (new TriggerEvent (GetRightIndex (), TriggerEventType.TriggerHeld, raycastResult));
					triggerHeld = true;
				}

				//Trigger up
				if (isLeftTriggerUp && ControllerMatchesRaycast (leftMostController, raycastResult)) {
					
					triggerData.triggerEvents.Add (new TriggerEvent (GetLeftIndex (), TriggerEventType.TriggerUp, raycastResult));
					triggerUp = true;
				}
				if (isRightTriggerUp && ControllerMatchesRaycast (rightMostController, raycastResult)) {
					
					triggerData.triggerEvents.Add (new TriggerEvent (GetRightIndex (), TriggerEventType.TriggerUp, raycastResult));
					triggerUp = true;
				}

				if (isLeftTriggerDown || isRightTriggerDown || isLeftTriggerHeld || isRightTriggerHeld || isLeftTriggerUp || isRightTriggerUp)
					baseRaycasters.Add (raycastResult.module);

				if (UnityEngine.Input.GetMouseButtonDown (0) || UnityEngine.Input.GetKeyDown (KeyCode.Space) || triggerDown) {
					lastMouseDownTime = Time.time;
				}

				triggerData.didClickSomething = triggerDown;

				if (triggerDown) {

					GameObject go = raycastResult.gameObject;

					pointerData.pressPosition = pointerData.position;
					pointerData.pointerPressRaycast = raycastResult;
					pointerData.pointerPress = ExecuteEvents.ExecuteHierarchy (go, pointerData, ExecuteEvents.pointerDownHandler)
					?? ExecuteEvents.GetEventHandler<IPointerDownHandler> (go);

					ExecuteEvents.Execute (pointerData.pointerPress, pointerData, ExecuteEvents.pointerDownHandler);

					// Save the pending click state.
					pointerData.rawPointerPress = go;
				}
				if (triggerHeld) {

					GameObject go = raycastResult.gameObject;

					pointerData.button = PointerEventData.InputButton.Middle;// to throw off ScrollRect from taking input (cause it fucks everything up)
					pointerData.pressPosition = pointerData.position;
					pointerData.pointerPressRaycast = raycastResult;
					pointerData.pointerPress = ExecuteEvents.ExecuteHierarchy (go, pointerData, ExecuteEvents.dragHandler)
					?? ExecuteEvents.GetEventHandler<IDragHandler> (go);

					ExecuteEvents.Execute (pointerData.pointerPress, pointerData, ExecuteEvents.dragHandler);

					// Save the pending click state.
					pointerData.rawPointerPress = go;
				}
				if (triggerUp) {

					GameObject go = raycastResult.gameObject;

					pointerData.pressPosition = pointerData.position;
					pointerData.pointerPressRaycast = raycastResult;
					pointerData.pointerPress = ExecuteEvents.ExecuteHierarchy (go, pointerData, ExecuteEvents.pointerClickHandler)
					?? ExecuteEvents.GetEventHandler<IPointerClickHandler> (go);

					ExecuteEvents.Execute (pointerData.pointerPress, pointerData, ExecuteEvents.pointerClickHandler);

					// Save the pending click state.
					pointerData.rawPointerPress = go;
					pointerData.eligibleForClick = true;
					pointerData.clickCount++;
					if (pointerData.clickCount == 1)
						pointerData.clickTime = Time.unscaledTime;

					triggerData.didClickSomething = true;
					triggerData.didTrigger = true;
				}
			}

			/////Trigger Events
			//Trigger down
			if (isLeftTriggerDown) {
				triggerData.triggerEvents.Add (new TriggerEvent (GetLeftIndex (), TriggerEventType.TriggerDown, default(RaycastResult)));
			}
			if (isRightTriggerDown) {
				triggerData.triggerEvents.Add (new TriggerEvent (GetRightIndex (), TriggerEventType.TriggerDown, default(RaycastResult)));
			}

			//Trigger up
			if (isLeftTriggerUp) {
				triggerData.triggerEvents.Add (new TriggerEvent (GetLeftIndex (), TriggerEventType.TriggerUp, default(RaycastResult)));
			}
			if (isRightTriggerUp) {
				triggerData.triggerEvents.Add (new TriggerEvent (GetRightIndex (), TriggerEventType.TriggerUp, default(RaycastResult)));
			}

			//Global Actions
			if (isLeftTriggerDown || isRightTriggerDown) {

				if (GlobalTriggerDownAction != null)
					GlobalTriggerDownAction ();
			}
			if (isLeftTriggerHeld || isRightTriggerHeld) {

				if (GlobalTriggerHeldAction != null)
					GlobalTriggerHeldAction ();
			}
			if (isLeftTriggerUp || isRightTriggerUp) {

				if (GlobalTriggerUpAction != null)
					GlobalTriggerUpAction ();
			}

			//Motion Controller Interface Events
			foreach (TriggerEvent triggerEvent in triggerData.triggerEvents) {

				//Only block trigger downs
				//Debug.Log(triggerData.didClickSomething);
				if (!triggerData.didClickSomething || triggerEvent.eventType == TriggerEventType.TriggerUp) {

					if (TriggerAction != null)
						TriggerAction (GetTransformFromDeviceIndex (triggerEvent.deviceIndex), triggerEvent.eventType, triggerEvent.deviceIndex);
				}

				MotionController motionController = GetControllerFromDeviceIndex (triggerEvent.deviceIndex);
				IMotionControlEventHandler eventHandler = null;
				if (motionController != null)
					eventHandler = motionController.GetComponent<IMotionControlEventHandler> ();

				if (triggerEvent.eventType == TriggerEventType.TriggerDown) {

					if (eventHandler != null)
						eventHandler.OnMotionControlTriggerDown ();
				} else {

					if (eventHandler != null)
						eventHandler.OnMotionControlTriggerUp ();
				}
			}

			bool shouldDebug = false;
			if (shouldDebug)
				Debug.Log (string.Format ("DidTrigger{0}  DidHit{1}", triggerData.didTrigger, triggerData.didClickSomething));
		}


		protected void HandleGrip () {

			List<GripEvent> gripEvents = new List<GripEvent> ();
//			GripData gripData = new GripData();
//			gripData.didGrip = false;
//			gripData.gripEvents = new List<GripEvent>();

			int leftMostIndex = GetLeftIndex ();
			MotionController leftMostController = GetControllerFromDeviceIndex (leftMostIndex);
			int rightMostIndex = GetRightIndex ();
			MotionController rightMostController = GetControllerFromDeviceIndex (rightMostIndex);


			bool isLeftGripDown = IsLeftGripDown ();
			bool isLeftGripHeld = IsLeftGripHeld ();
			bool isLeftGripUp = IsLeftGripUp ();
			bool isRightGripDown = IsRightGripDown ();
			bool isRightGripHeld = IsRightGripHeld ();
			bool isRightGripUp = IsRightGripUp ();

			//Global Actions
			if (isLeftGripDown || isRightGripDown) {

				if (GlobalGripDownAction != null)
					GlobalGripDownAction ();
			}
			if (isLeftGripHeld || isRightGripHeld) {

				if (GlobalGripHeldAction != null)
					GlobalGripHeldAction ();
			}
			if (isLeftGripUp || isRightGripUp) {

				if (GlobalGripUpAction != null)
					GlobalGripUpAction ();
			}

			/////Grip Events
			//Grip down
			if (isLeftGripDown) {
				gripEvents.Add (new GripEvent (GetLeftIndex (), GripEventType.GripDown, default(RaycastResult)));
			}
			if (isRightGripDown) {
				gripEvents.Add (new GripEvent (GetRightIndex (), GripEventType.GripDown, default(RaycastResult)));
			}

			//Grip up
			if (isLeftGripUp) {
				gripEvents.Add (new GripEvent (GetLeftIndex (), GripEventType.GripUp, default(RaycastResult)));
			}
			if (isRightGripUp) {
				gripEvents.Add (new GripEvent (GetRightIndex (), GripEventType.GripUp, default(RaycastResult)));
			}

			//Grip Move
			if (leftMostController.IsActive ()) {

				if (GripMoveAction != null)
					GripMoveAction (leftMostController.transform, leftMostIndex);
			}
			if (rightMostController.IsActive ()) {

				if (GripMoveAction != null)
					GripMoveAction (rightMostController.transform, rightMostIndex);
			}

			foreach (GripEvent gripEvent in gripEvents) {

				if (GripAction != null)
					GripAction (GetTransformFromDeviceIndex (gripEvent.deviceIndex), gripEvent.eventType, gripEvent.deviceIndex);

//				GameObject pointerCurrentRaycastGameObject = gripEvent.raycastResult.gameObject;
//
//				if(gripEvent.eventType== GripEventType.GripDown) {
//
//					IViveGripDownHandler gripDownHandler = pointerCurrentRaycastGameObject.GetComponentInParent<IViveGripDownHandler>();
//					if(gripDownHandler!=null){
//
//						Action callback = gripDownHandler.OnGripDown(GetTransformFromDevice(gripEvent.device),pointerData.pointerCurrentRaycast);
//						AddGripUpCallBack(gripEvent.device,callback);
//					}
//
//					IViveEventHandler eventHandler = GetViveControllerFromModule(gripEvent.raycastResult.module).GetComponent<IViveEventHandler>();
//					if(eventHandler!=null)
//						eventHandler.OnViveGripDown();
//
//				}else {
//					IViveEventHandler eventHandler = GetViveControllerFromModule(gripEvent.raycastResult.module).GetComponent<IViveEventHandler>();
//					if(eventHandler!=null)
//						eventHandler.OnViveGripUp();
//				}
			}
//
//			HashSet<BaseRaycaster> baseRaycasters = new HashSet<BaseRaycaster>();
//			foreach (RaycastResult raycastResult in m_RaycastResultCache) {
//
//				if(raycastResult.gameObject==null)
//					continue;
//				if(baseRaycasters.Contains(raycastResult.module))
//					continue;
//
//				//Vive grip down
//				if(leftMostDevice!=null && leftMostDevice.GetPressDown(EVRButtonId.k_EButton_Grip) && DeviceMatchesRaycast(leftMostDevice,raycastResult)) {
//					gripData.gripEvents.Add(new GripEvent(leftMostDevice,ViveGripEventType.GripDown,raycastResult));
//					baseRaycasters.Add(raycastResult.module);
//				}
//				if(rightMostDevice!=null && (rightMostDevice != leftMostDevice) && rightMostDevice.GetPressDown(EVRButtonId.k_EButton_Grip) && DeviceMatchesRaycast(rightMostDevice,raycastResult)) {
//					gripData.gripEvents.Add(new GripEvent(rightMostDevice,ViveGripEventType.GripDown,raycastResult));
//					baseRaycasters.Add(raycastResult.module);
//				}
//
//
//				//Vive grip up
//				if(leftMostDevice!=null && leftMostDevice.GetPressUp(EVRButtonId.k_EButton_Grip) && DeviceMatchesRaycast(leftMostDevice,raycastResult)) {
//					gripData.gripEvents.Add(new GripEvent(leftMostDevice,ViveGripEventType.GripUp,raycastResult));
//					baseRaycasters.Add(raycastResult.module);
//					ClearGripUpCallBacks(leftMostDevice);
//				}
//				if(rightMostDevice!=null && (rightMostDevice != leftMostDevice) && rightMostDevice.GetPressUp(EVRButtonId.k_EButton_Grip) && DeviceMatchesRaycast(rightMostDevice,raycastResult)) {
//					gripData.gripEvents.Add(new GripEvent(rightMostDevice,ViveGripEventType.GripUp,raycastResult));
//					baseRaycasters.Add(raycastResult.module);
//					ClearGripUpCallBacks(rightMostDevice);
//				}
//			}
//			//Vive grip up
//			if(leftMostDevice!=null && leftMostDevice.GetPressUp(EVRButtonId.k_EButton_Grip)) {
//				ClearGripUpCallBacks(leftMostDevice);
//			}
//			if(rightMostDevice!=null && (rightMostDevice != leftMostDevice) && rightMostDevice.GetPressUp(EVRButtonId.k_EButton_Grip)) {
//				ClearGripUpCallBacks(rightMostDevice);
//			}
//			//			Debug.Log("n1: "+name+" _ pcr: "+pointerData.pointerCurrentRaycast.gameObject.name);
//
		}


		protected void HandleScroll () {


			Vector2 leftScroll = GetLeftScroll ();
			Vector2 rightScroll = GetRightScroll ();

			if (ScrollAction != null)
				ScrollAction (new Vector2[]{ leftScroll, rightScroll });
		}

		protected void HandleThumbDown() {

			if(IsLeftThumbDown()){
				if(ThumbDownAction!=null)
					ThumbDownAction(0);
			}
			if(IsRightThumbDown()){
				if(ThumbDownAction!=null)
					ThumbDownAction(1);
			}
		}

		protected abstract Vector2 GetLeftScroll ();

		protected abstract Vector2 GetRightScroll ();

		protected abstract int GetLeftIndex ();

		protected abstract int GetRightIndex ();
		//Trigger
		protected abstract bool IsLeftTriggerDown ();

		protected abstract bool IsLeftTriggerHeld ();

		protected abstract bool IsLeftTriggerUp ();

		protected abstract bool IsRightTriggerDown ();

		protected abstract bool IsRightTriggerHeld ();

		protected abstract bool IsRightTriggerUp ();
		//Grip
		protected abstract bool IsLeftGripDown ();

		protected abstract bool IsLeftGripHeld ();

		protected abstract bool IsLeftGripUp ();

		protected abstract bool IsRightGripDown ();

		protected abstract bool IsRightGripHeld ();

		protected abstract bool IsRightGripUp ();
		//Thumb
		protected abstract bool IsLeftThumbDown();
		protected abstract bool IsRightThumbDown();
		//ButtonOne
		protected virtual bool IsLeftButtonOneDown () {
			return false;
		}

		protected virtual bool IsLeftButtonOneHeld () {
			return false;
		}

		protected virtual bool IsLeftButtonOneUp () {
			return false;
		}

		protected virtual bool IsRightButtonOneDown () {
			return false;
		}

		protected virtual bool IsRightButtonOneHeld () {
			return false;
		}

		protected virtual bool IsRightButtonOneUp () {
			return false;
		}

		protected abstract int GetControllerIndexFromModule (BaseRaycaster module);

		protected abstract MotionController GetControllerFromDeviceIndex (int index);

		public abstract MotionController[] GetControllers ();
		public abstract MotionController GetLeftController ();
		public abstract MotionController GetRightController ();

		protected virtual Transform GetTransformFromDeviceIndex (int index) {

			MotionController controller = GetControllerFromDeviceIndex (index);

			return (controller == null) ? null : controller.transform;
		}

		//Saving for Vive
		protected int GetViveControllerIndexFromModule (BaseRaycaster module) {

			return GetViveControllerFromModule (module).GetIndex ();
		}

		protected ViveController GetViveControllerFromModule (BaseRaycaster module) {

			return module.gameObject.transform.parent.GetComponent<ViveController> ();
		}

		protected bool IsLeftViveTriggerDown () {

			int leftMostIndex = GetViveLeftIndex ();
			SteamVR_Controller.Device leftMostDevice = (leftMostIndex == -1) ? null : SteamVR_Controller.Input (leftMostIndex);

			return leftMostDevice != null && leftMostDevice.GetHairTriggerDown ();
		}

		protected bool IsRightViveTriggerDown () {

			int leftMostIndex = GetViveLeftIndex ();
			SteamVR_Controller.Device leftMostDevice = (leftMostIndex == -1) ? null : SteamVR_Controller.Input (leftMostIndex);
			int rightMostIndex = GetViveRightIndex ();
			SteamVR_Controller.Device rightMostDevice = (rightMostIndex == -1) ? null : SteamVR_Controller.Input (rightMostIndex);

			return (rightMostDevice != null && (rightMostDevice != leftMostDevice) && rightMostDevice.GetHairTriggerDown ());
		}

		protected bool IsLeftViveTriggerUp () {

			int leftMostIndex = GetViveLeftIndex ();
			SteamVR_Controller.Device leftMostDevice = (leftMostIndex == -1) ? null : SteamVR_Controller.Input (leftMostIndex);

			return leftMostDevice != null && leftMostDevice.GetHairTriggerUp ();
		}

		protected bool IsRightViveTriggerUp () {

			int leftMostIndex = GetViveLeftIndex ();
			SteamVR_Controller.Device leftMostDevice = (leftMostIndex == -1) ? null : SteamVR_Controller.Input (leftMostIndex);
			int rightMostIndex = GetViveRightIndex ();
			SteamVR_Controller.Device rightMostDevice = (rightMostIndex == -1) ? null : SteamVR_Controller.Input (rightMostIndex);

			return (rightMostDevice != null && (rightMostDevice != leftMostDevice) && rightMostDevice.GetHairTriggerUp ());
		}

		protected int GetViveLeftIndex () {

			return SteamVR_Controller.GetDeviceIndex (SteamVR_Controller.DeviceRelation.Leftmost);
		}

		protected int GetViveRightIndex () {

			return SteamVR_Controller.GetDeviceIndex (SteamVR_Controller.DeviceRelation.Rightmost);
		}










		//Handles hover and select
		protected override void UpdateCurrentObject () {

			// Send enter events and update the highlight.
			//			var go = pointerData.pointerCurrentRaycast.gameObject;

			HashSet<int> controllerIndices = new HashSet<int> ();
			
			foreach (RaycastResult raycastResult in m_RaycastResultCache) {

				if (raycastResult.gameObject == null)
					continue;
				int index = GetControllerIndexFromModule (raycastResult.module);
				if (controllerIndices.Contains (index))
					continue;

				GameObject go = raycastResult.gameObject;
				HandlePointerExitAndEnter (pointerData, go, index);

				controllerIndices.Add (index);

				// Update the current selection, or clear if it is no longer the current object.
				var selected = ExecuteEvents.GetEventHandler<ISelectHandler> (go);
				if (selected == eventSystem.currentSelectedGameObject) {
					ExecuteEvents.Execute (eventSystem.currentSelectedGameObject, GetBaseEventData (),
						ExecuteEvents.updateSelectedHandler);

				} else {
					eventSystem.SetSelectedGameObject (null, pointerData);
				}
			}

			List<int> keys = new List<int> (currentTargets.Keys);
			foreach (var Key in keys) {

				if (controllerIndices.Contains (Key))
					continue;
				HandlePointerExitAndEnter (pointerData, null, Key);
			}

			if (m_RaycastResultCache.Count == 0) {

				// Send enter events and update the highlight.
				var go = pointerData.pointerCurrentRaycast.gameObject;
				// Update the current selection, or clear if it is no longer the current object.
				var selected = ExecuteEvents.GetEventHandler<ISelectHandler> (go);
				if (selected == eventSystem.currentSelectedGameObject) {
					ExecuteEvents.Execute (eventSystem.currentSelectedGameObject, GetBaseEventData (),
						ExecuteEvents.updateSelectedHandler);
				} else {
					eventSystem.SetSelectedGameObject (null, pointerData);
				}
			}

			List<GameObject> newHover = new List<GameObject> ();
			foreach (var kvp in currentHoveredDict) {

				foreach (var go in kvp.Value) {

					if (!newHover.Contains (go))
						newHover.Add (go);
				}
			}

			List<GameObject> oldHover = pointerData.hovered;
			List<GameObject> newHoverAndOldHover = new List<GameObject> (newHover);
			newHoverAndOldHover.AddRange (oldHover);
			for (int i = 0; i < newHoverAndOldHover.Count; ++i) {

				if (newHover.Contains (newHoverAndOldHover [i]) && !oldHover.Contains (newHoverAndOldHover [i]))
					ExecuteEvents.Execute (newHoverAndOldHover [i], pointerData, ExecuteEvents.pointerEnterHandler);
				else if (oldHover.Contains (newHoverAndOldHover [i]) && !newHover.Contains (newHoverAndOldHover [i]))
					ExecuteEvents.Execute (newHoverAndOldHover [i], pointerData, ExecuteEvents.pointerExitHandler);
			}
			pointerData.hovered = newHover;
		}


		Dictionary<int,GameObject> currentTargets = new Dictionary<int, GameObject> ();
		Dictionary<int,List<GameObject>> currentHoveredDict = new Dictionary<int, List<GameObject>> ();

		public struct ExitEnterData {

			public List<GameObject> enterGameObjects;
			public List<GameObject> exitGameObjects;

			public ExitEnterData (List<GameObject> enterGameObjects, List<GameObject> exitGameObjects)
			{
				this.enterGameObjects = enterGameObjects;
				this.exitGameObjects = exitGameObjects;
			}
			
		}
		// walk up the tree till a common root between the last entered and the current entered is foung
		// send exit events up to (but not inluding) the common root. Then send enter events up to
		// (but not including the common root).
		protected void HandlePointerExitAndEnter (PointerEventData currentPointerData, GameObject newEnterTarget, int index) {

			if (!currentTargets.ContainsKey (index))
				currentTargets.Add (index, null);
			if (!currentHoveredDict.ContainsKey (index))
				currentHoveredDict.Add (index, new List<GameObject> ());
			
			GameObject currentTarget = currentTargets [index];
			List<GameObject> currentHovered = currentHoveredDict [index];

			// if we have no target / pointerEnter has been deleted
			// just send exit events to anything we are tracking
			// then exit
			if (newEnterTarget == null || currentTarget == null) {

				currentHovered.Clear ();

				if (newEnterTarget == null) {
					currentTarget = newEnterTarget;
					currentTargets [index] = currentTarget;
					currentHoveredDict [index] = currentHovered;
					return ;
				}
			}

			// if we have not changed hover target
			if (currentTarget == newEnterTarget && newEnterTarget) {
				currentTargets [index] = currentTarget;
				currentHoveredDict [index] = currentHovered;
				return;
			}

			GameObject commonRoot = FindCommonRoot (currentTarget, newEnterTarget);

			// and we already an entered object from last time
			if (currentTarget != null) {
				// send exit handler call to all elements in the chain
				// until we reach the new target, or null!
				Transform t = currentTarget.transform;

				while (t != null) {
					// if we reach the common root break out!
					if (commonRoot != null && commonRoot.transform == t)
						break;
					
					currentHovered.Remove (t.gameObject);
					t = t.parent;
				}
			}

			// now issue the enter call up to but not including the common root
			currentTarget = newEnterTarget;
			if (newEnterTarget != null) {
				Transform t = newEnterTarget.transform;

				while (t != null && t.gameObject != commonRoot) {
					
					currentHovered.Add (t.gameObject);
					t = t.parent;
				}
			}
			currentTargets [index] = currentTarget;
			currentHoveredDict [index] = currentHovered;

			return;
		}


























		public void SetEnabledMotionControllerTutorialTypes (MotionController.MotionControllerTutorialType[] tutorials) {

			tutorialTypes = tutorials;
			//doesn't work (returns null array)
			//			ViveController[] viveControllers = GetViveControllers();
			//			for (int i = 0; i < viveControllers.Length; ++i) {
			//
			//				viveControllers[i].SetEnabledTutorialTypes(tutorials);
			//			}
		}

		protected struct GripEvent {

			public int deviceIndex;
			public GripEventType eventType;
			public RaycastResult raycastResult;

			public GripEvent (int deviceIndex, GripEventType eventType, RaycastResult raycastResult)
			{

				this.deviceIndex = deviceIndex;
				this.eventType = eventType;
				this.raycastResult = raycastResult;
			}
		}

		protected struct GripData {
			public bool didGrip;

			public List<GripEvent> gripEvents;
		}

		protected Dictionary<SteamVR_Controller.Device,Queue<Action>> gripUpCallbacks = new Dictionary<SteamVR_Controller.Device,Queue<Action>> ();


		protected void AddGripUpCallBack (SteamVR_Controller.Device device, Action callback) {

			Queue<Action> gripUpCallbacksQueue = null;
			if (gripUpCallbacks.ContainsKey (device))
				gripUpCallbacksQueue = gripUpCallbacks [device];
			else {
				gripUpCallbacksQueue = new Queue<Action> ();
				gripUpCallbacks.Add (device, gripUpCallbacksQueue);
			}

			gripUpCallbacksQueue.Enqueue (callback);
		}

		protected void ClearGripUpCallBacks (SteamVR_Controller.Device device) {

			if (!gripUpCallbacks.ContainsKey (device))
				return;

			Queue<Action> gripUpCallbacksQueue = gripUpCallbacks [device];

			while (gripUpCallbacksQueue.Count > 0) {

				Action action = gripUpCallbacksQueue.Dequeue ();
				if (action != null)
					action ();
			}
		}

		protected void HandleBack () {

//			bool menuDown = false;
//
//			int leftMostIndex = GetViveLeftIndex();
//			int rightMostIndex = GetViveRightIndex();
//			SteamVR_Controller.Device leftMostDevice = (leftMostIndex == -1) ? null : SteamVR_Controller.Input(leftMostIndex);
//			SteamVR_Controller.Device rightMostDevice = (rightMostIndex == -1) ? null : SteamVR_Controller.Input(rightMostIndex);
//
//			if(leftMostDevice != null && leftMostDevice.GetPressDown(EVRButtonId.k_EButton_ApplicationMenu)) {
//				menuDown = true;
//
//				IViveEventHandler eventHandler = GetViveControllerFromDevice(leftMostDevice).GetComponent<IViveEventHandler>();
//				if(eventHandler!=null)
//					eventHandler.OnViveMenuDown();
//			}
//
//			if(rightMostDevice != null && rightMostDevice.GetPressDown(EVRButtonId.k_EButton_ApplicationMenu)) { 
//				menuDown = true;
//
//				IViveEventHandler eventHandler = GetViveControllerFromDevice(rightMostDevice).GetComponent<IViveEventHandler>();
//				if(eventHandler!=null)
//					eventHandler.OnViveMenuDown();
//			}
//
//
//
//			if(menuDown && GlobalBackAction != null)
//				GlobalBackAction();
		}

		protected void HandleTouchpad () {

//			int leftMostIndex = GetViveLeftIndex();
//			int rightMostIndex = GetViveRightIndex();
//			SteamVR_Controller.Device leftMostDevice = (leftMostIndex == -1) ? null : SteamVR_Controller.Input(leftMostIndex);
//			SteamVR_Controller.Device rightMostDevice = (rightMostIndex == -1) ? null : SteamVR_Controller.Input(rightMostIndex);
//
//			if(leftMostDevice != null && leftMostDevice.GetPressDown(EVRButtonId.k_EButton_SteamVR_Touchpad)) {
//
//				IViveEventHandler eventHandler = GetViveControllerFromDevice(leftMostDevice).GetComponent<IViveEventHandler>();
//				if(eventHandler!=null)
//					eventHandler.OnViveTouchpadDown();
//			}
//
//			if(rightMostDevice != null && rightMostDevice.GetPressDown(EVRButtonId.k_EButton_SteamVR_Touchpad)) { 
//
//				IViveEventHandler eventHandler = GetViveControllerFromDevice(rightMostDevice).GetComponent<IViveEventHandler>();
//				if(eventHandler!=null)
//					eventHandler.OnViveTouchpadDown();
//			}
		}

		////////////////////////////////////////
		//
		// Haptic Functions

		protected Dictionary<SteamVR_Controller.Device,Coroutine> activeHapticCoroutines = new Dictionary<SteamVR_Controller.Device, Coroutine> ();

		public void StartHapticVibration (SteamVR_Controller.Device device, float length, float strength) {

			if (activeHapticCoroutines.ContainsKey (device)) {
				Debug.Log ("This device is already vibrating");
				return;
			}

			Coroutine coroutine = StartCoroutine (StartHapticVibrationCoroutine (device, length, strength));
			activeHapticCoroutines.Add (device, coroutine);

		}

		public void StopHapticVibration (SteamVR_Controller.Device device) {

			if (!activeHapticCoroutines.ContainsKey (device)) {
				Debug.Log ("Could not find this device");
				return;
			}
			StopCoroutine (activeHapticCoroutines [device]);
			activeHapticCoroutines.Remove (device);
		}

		protected IEnumerator StartHapticVibrationCoroutine (SteamVR_Controller.Device device, float length, float strength) {

			for (float i = 0; i < length; i += Time.deltaTime) {
				device.TriggerHapticPulse ((ushort)Mathf.Lerp (0, 3999, strength));
				yield return null;
			}

			activeHapticCoroutines.Remove (device);
		}

		////////////////////////////////////////
		//
		// Helper Functions

		protected bool ControllerMatchesRaycast (MotionController controller, RaycastResult raycastResult) {

			BaseRaycaster module = raycastResult.module;
			int index = GetControllerIndexFromModule (module);
			int deviceIndex = controller.GetIndex ();

			return (index == deviceIndex);
		}


		public Transform[] GetControllerTransforms () {

			int leftMostIndex = SteamVR_Controller.GetDeviceIndex (SteamVR_Controller.DeviceRelation.Leftmost);
			int rightMostIndex = SteamVR_Controller.GetDeviceIndex (SteamVR_Controller.DeviceRelation.Rightmost);
			SteamVR_Controller.Device leftMostDevice = (leftMostIndex == -1) ? null : SteamVR_Controller.Input (leftMostIndex);
			SteamVR_Controller.Device rightMostDevice = (rightMostIndex == -1) ? null : SteamVR_Controller.Input (rightMostIndex);

			if (leftMostDevice == null && rightMostDevice == null) {
				return null;
			}
			Transform leftTransform = null;
			if (leftMostDevice != null)
				leftTransform = GetTransformFromDevice (leftMostDevice);
			Transform rightTransform = null;
			if (rightMostDevice != null)
				rightTransform = GetTransformFromDevice (rightMostDevice);

			if (leftTransform == null && rightTransform == null) {
				return null;
			}

			if (leftTransform == rightTransform) {

				return new Transform[] { leftTransform };
			}

			List<Transform> transforms = new List<Transform> ();
			if (leftTransform != null)
				transforms.Add (leftTransform);
			if (rightTransform != null)
				transforms.Add (rightTransform);

			return transforms.ToArray ();
		}

		protected ViveController[] GetViveControllers () {

			Transform[] controllerTransforms = GetControllerTransforms ();
			if (controllerTransforms == null)
				return null;

			ViveController[] viveControllers = new ViveController[controllerTransforms.Length];

			for (int i = 0; i < controllerTransforms.Length; ++i) {

				viveControllers [i] = controllerTransforms [i].GetComponentInChildren<ViveController> ();
			}
			return viveControllers;
		}


		protected Transform GetTransformFromDevice (SteamVR_Controller.Device device) {

			SteamVR_TrackedObject[] trackedControllers = FindObjectsOfType<SteamVR_TrackedObject> ();
			foreach (SteamVR_TrackedObject tracked in trackedControllers) {
				if (((int)tracked.index) == device.index) {
					return tracked.transform;
				}
			}
			return null;
		}

		protected ViveController GetViveControllerFromDevice (SteamVR_Controller.Device device) {

			Transform transform = GetTransformFromDevice (device);
			if (transform == null)
				return null;
			return transform.GetComponentInChildren<ViveController> ();
		}
	}
}
