
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;
using Neuromancers.UI;

namespace Neuromancers.UI
{
	public class GazeInputModule : InputModule
	{

		/// <summary>
		/// VRB STUFF
		/// </summary>
		protected readonly int MAX_CHECK_COUNT = 4;
		//how many parents to check for IInteractable before giving up
		protected readonly float MIN_SWIPE_DISTANCE = 50f;


//		public Action GlobalTriggerAction { get; set; }

		//Tap
		protected static readonly float MAX_TAP_DURATION = .35f;
		protected float lastMouseDownTime = -MAX_TAP_DURATION;

		//Hold
		public static bool isHoldingMouse;


		//Swipe
		protected Vector2 lastMouseDownPosition;
		public static bool isSwiping;
		protected Vector2 lastMousePosition;
		public static Vector2 deltaMousePosition;


		//Properties
		public override InputModuleType InputModuleType {
			get {
				return InputModuleType.Gaze;
			}
		}

		/// <summary>
		/// GOOGLE STUFF
		/// </summary>

		[Tooltip ("Whether gaze input is active in VR Mode only (true), or all the time (false).")]
		public bool
			vrModeOnly = false;
		[Tooltip ("Optional object to place at raycast intersections as a 3D cursor. " +
		"Be sure it is on a layer that raycasts will ignore.")]
		public GameObject
			cursor;

		// Time in seconds between the pointer down and up events sent by a magnet click.
		// Allows time for the UI elements to make their state transitions.
		[HideInInspector]
		public float
			clickTime = 0.1f;
		// Based on default time for a button to animate to Pressed.

		// The pixel through which to cast rays, in viewport coordinates.  Generally, the center
		// pixel is best, assuming a monoscopic camera is selected as the Canvas' event camera.
		[SerializeField]
		public Vector2
			hotspot = new Vector2 (0.5f, .5f);
		private PointerEventData pointerData;

		public override bool ShouldActivateModule () {
			return true;
		}

		public override void DeactivateModule () {
			base.DeactivateModule ();
			if (pointerData != null) {
				HandlePointerExitAndEnter (pointerData, null);
				pointerData = null;
			}
			eventSystem.SetSelectedGameObject (null, GetBaseEventData ());
			if (cursor != null) {
				cursor.SetActive (false);
			}
		}

		public override bool IsPointerOverGameObject (int pointerId) {
			return pointerData != null && pointerData.pointerEnter != null;
		}

		public override void Process () {
			CastRayFromGaze ();
			UpdateCurrentObject ();
			PlaceCursor ();
			TriggerData triggerData = HandleTrigger ();
			HandleSwipe ();

			//			Debug.Log (string.Format ("DidTrigger{0}  DidHit{1}", triggerData.didTrigger, triggerData.didClickSomething));
			if (triggerData.didTrigger && !triggerData.didClickSomething) {

				if (GlobalTriggerAction != null)
					GlobalTriggerAction ();
			}
		}

		private void CastRayFromGaze () {
			if (pointerData == null) {
				pointerData = new PointerEventData (eventSystem);
			}
			m_RaycastResultCache.Clear ();
			pointerData.Reset ();
			pointerData.position = new Vector2 (hotspot.x * Screen.width, hotspot.y * Screen.height);
			eventSystem.RaycastAll (pointerData, m_RaycastResultCache);

			List<RaycastResult> resultsToRemove = new List<RaycastResult> ();

			List<InteractiveGazeEventData> interactiveGazeEventDataList = new List<InteractiveGazeEventData> ();

			foreach (RaycastResult rayResult in m_RaycastResultCache) {
				Element interactableComponent = null; 
				Transform transformToCheckForInteractableComponent = rayResult.gameObject.transform;
				int checkCount = 0;
				while (transformToCheckForInteractableComponent != null && interactableComponent == null && checkCount < MAX_CHECK_COUNT) {
					interactableComponent = transformToCheckForInteractableComponent.GetComponent<Element> ();
					checkCount++;
					transformToCheckForInteractableComponent = transformToCheckForInteractableComponent.parent;
				}


				bool didHitValidComponent = interactableComponent != null;// && (rayResult.distance < interactableComponent.MaxInteractDistance);


				if (didHitValidComponent) {
					interactiveGazeEventDataList.Add (new InteractiveGazeEventData (InteractiveEventType.HitInteractable, rayResult, interactableComponent));	

				} else {						
					interactiveGazeEventDataList.Add (new InteractiveGazeEventData (InteractiveEventType.HitCollider, rayResult, null));	

					resultsToRemove.Add (rayResult);
				}
			}
			if (m_RaycastResultCache == null || m_RaycastResultCache.Count == 0) {
				interactiveGazeEventDataList.Add (new InteractiveGazeEventData (InteractiveEventType.HitNone, new RaycastResult (),null));	
			}

			if (InteractiveGazeAction != null) {
				InteractiveGazeAction (interactiveGazeEventDataList);
			}

			foreach (RaycastResult rayResult in resultsToRemove) {
				m_RaycastResultCache.Remove (rayResult);
			}
			pointerData.pointerCurrentRaycast = FindFirstRaycast (m_RaycastResultCache);
		}


		private void UpdateCurrentObject () {
			// Send enter events and update the highlight.
			var go = pointerData.pointerCurrentRaycast.gameObject;
			HandlePointerExitAndEnter (pointerData, go);
			// Update the current selection, or clear if it is no longer the current object.
			var selected = ExecuteEvents.GetEventHandler<ISelectHandler> (go);
			if (selected == eventSystem.currentSelectedGameObject) {
				ExecuteEvents.Execute (eventSystem.currentSelectedGameObject, GetBaseEventData (),
					ExecuteEvents.updateSelectedHandler);
			} else {
				eventSystem.SetSelectedGameObject (null, pointerData);
			}
		}

		private void PlaceCursor () {
			if (cursor == null)
				return;
			var go = pointerData.pointerCurrentRaycast.gameObject;
			cursor.SetActive (go != null);
			if (cursor.activeInHierarchy) {
				Camera cam = pointerData.enterEventCamera;
				// Note: rays through screen start at near clipping plane.
				float dist = pointerData.pointerCurrentRaycast.distance + cam.nearClipPlane;
				cursor.transform.position = cam.transform.position + cam.transform.forward * dist;
			}
		}

		protected struct TriggerData
		{

			public bool didTrigger;
			public bool didClickSomething;
		}

		protected TriggerData HandleTrigger () {

			TriggerData triggerData = new TriggerData ();
			triggerData.didClickSomething = false;
			triggerData.didTrigger = false;

			bool triggered = false;
			bool holding = false;

            if (UnityEngine.Input.GetMouseButtonDown (0) || UnityEngine.Input.GetKeyDown (KeyCode.Space) || OVRInput.GetDown (OVRInput.Button.One) || OVRInput.GetDown(OVRInput.RawButton.A)) {
				lastMouseDownTime = Time.time;
				//				triggered = true;
			}

			//			if(UnityEngine.Input.GetMouseButton(0))
			//			{
			//				if(Time.time-lastMouseDownTime>MAX_TAP_DURATION)
			//					holding = true;
			//			}
			//
			if (!isSwiping && (UnityEngine.Input.GetMouseButtonUp (0) || UnityEngine.Input.GetKeyDown (KeyCode.Space) || OVRInput.GetUp (OVRInput.Button.One) || OVRInput.GetUp(OVRInput.RawButton.A))) {
				if (Time.time - lastMouseDownTime < MAX_TAP_DURATION)
					triggered = true;
			}

			isHoldingMouse = holding;


			if (triggered) {


				//				foreach (RaycastResult rayResult in m_RaycastResultCache) {
				if (pointerData.pointerCurrentRaycast.gameObject != null) {
					//					GameObject go = rayResult.gameObject;
					GameObject go = pointerData.pointerCurrentRaycast.gameObject;

					// Send pointer down event.
					pointerData.pressPosition = pointerData.position;
					pointerData.pointerPressRaycast = pointerData.pointerCurrentRaycast;
					pointerData.pointerPress = ExecuteEvents.ExecuteHierarchy (go, pointerData, ExecuteEvents.pointerDownHandler)
					?? ExecuteEvents.GetEventHandler<IPointerClickHandler> (go);

					ExecuteEvents.Execute (pointerData.pointerPress, pointerData, ExecuteEvents.pointerClickHandler);

					// Save the pending click state.
					pointerData.rawPointerPress = go;
					pointerData.eligibleForClick = true;
					pointerData.clickCount++;
					if (pointerData.clickCount == 1)
						pointerData.clickTime = Time.unscaledTime;

					triggerData.didClickSomething = true;

				}
				triggerData.didTrigger = true;
			}
			return triggerData;
		}


		private void HandleSwipe () {
			//Swipe Complete
			bool swipedUp = false;
			bool swipedDown = false;
			bool swipedLeft = false;
			bool swipedRight = false;
			//Mid swipe
			bool swipingUp = false;
			bool swipingDown = false;
			bool swipingLeft = false;
			bool swipingRight = false;
			//Keep track of signed distance from origin
			float xSwipeDistance = 0;
			float ySwipeDistance = 0;

			//Swipe initialized
			if (UnityEngine.Input.GetMouseButtonDown (0))
				lastMouseDownPosition = UnityEngine.Input.mousePosition;

			Vector2 currentMousePosition = UnityEngine.Input.mousePosition;

			deltaMousePosition = currentMousePosition - lastMousePosition;

			xSwipeDistance = currentMousePosition.x - lastMouseDownPosition.x;
			ySwipeDistance = currentMousePosition.y - lastMouseDownPosition.y;

			if (UnityEngine.Input.GetMouseButton (0)) {
				swipingUp = ySwipeDistance > MIN_SWIPE_DISTANCE;
				swipingDown = ySwipeDistance < (-MIN_SWIPE_DISTANCE);
				swipingRight = xSwipeDistance > MIN_SWIPE_DISTANCE;
				swipingLeft = xSwipeDistance < (-MIN_SWIPE_DISTANCE);
			}

			isSwiping = swipingUp || swipingDown || swipingRight || swipingLeft;

			if (UnityEngine.Input.GetMouseButtonUp (0)) {
				swipedUp = ySwipeDistance > MIN_SWIPE_DISTANCE;
				swipedDown = ySwipeDistance < (-MIN_SWIPE_DISTANCE);
				swipedRight = xSwipeDistance > MIN_SWIPE_DISTANCE;
				swipedLeft = xSwipeDistance < (-MIN_SWIPE_DISTANCE);
			}

			lastMousePosition = currentMousePosition;
			//			foreach(GameObject swipeListenerGameObject in swipeListeners)
			//			{
			//				//Up
			//				if(swipedUp)
			//					ExecuteEvents.Execute<ISwipeUpHandler>(swipeListenerGameObject, null, (x,y)=>x.OnSwipeUp(ySwipeDistance));
			//				if(swipingUp)
			//					ExecuteEvents.Execute<ISwipeUpHandler>(swipeListenerGameObject, null, (x,y)=>x.OnSwipingUp(ySwipeDistance));
			//				//Down
			//				if(swipedDown)
			//					ExecuteEvents.Execute<ISwipeDownHandler>(swipeListenerGameObject, null, (x,y)=>x.OnSwipeDown(ySwipeDistance));
			//				if(swipingDown)
			//					ExecuteEvents.Execute<ISwipeDownHandler>(swipeListenerGameObject, null, (x,y)=>x.OnSwipingDown(ySwipeDistance));
			//				//Right
			//				if(swipedRight)
			//					ExecuteEvents.Execute<ISwipeRightHandler>(swipeListenerGameObject, null, (x,y)=>x.OnSwipeRight(xSwipeDistance));
			//				if(swipingRight)
			//					ExecuteEvents.Execute<ISwipeRightHandler>(swipeListenerGameObject, null, (x,y)=>x.OnSwipingRight(xSwipeDistance));
			//				//Left
			//				if(swipedLeft)
			//					ExecuteEvents.Execute<ISwipeLeftHandler>(swipeListenerGameObject, null, (x,y)=>x.OnSwipeLeft(xSwipeDistance));
			//				if(swipingLeft)
			//					ExecuteEvents.Execute<ISwipeLeftHandler>(swipeListenerGameObject, null, (x,y)=>x.OnSwipingLeft(xSwipeDistance));
			//			}
		}
	}

}
