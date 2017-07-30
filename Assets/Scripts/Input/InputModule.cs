using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Neuromancers.UI
{
	public enum InputModuleType {

		Standalone,
		Gaze,
		Rift,
		Vive
	}
	public abstract class InputModule : BaseInputModule
	{
		//readonly

		//Serialized
		
		/////Protected/////
		//References
		protected PointerEventData pointerData;
		//Primitives

		//Actions
		public Action<List<InteractiveGazeEventData>> InteractiveGazeAction  { get; set; }
		public Action<List<InteractiveRaycastEventData>> InteractiveRaycastAction;
		public Action<Vector2[]> ScrollAction;
		public Action<int> ThumbDownAction;
		public Action<Transform, TriggerEventType, int> TriggerAction;
		public Action<Transform, GripEventType, int> GripAction;
		public Action<Transform, int> GripMoveAction;
		public Action GlobalTriggerAction;
		public Action GlobalGripAction;
		public Action GlobalTriggerDownAction;
		public Action GlobalTriggerHeldAction;
		public Action GlobalTriggerUpAction;
		public Action GlobalGripDownAction;
		public Action GlobalGripHeldAction;
		public Action GlobalGripUpAction;
		public Action GlobalBackAction;

		//Properties
		public abstract InputModuleType InputModuleType {get;} 

		

		///////////////////////////////////////////////////////////////////////////
		//
		// Inherited from BaseInputModule
		//

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
		}

		public override bool IsPointerOverGameObject (int pointerId) {
			return pointerData != null && pointerData.pointerEnter != null;
		}
		
		//		protected void Awake()
		//		{
		//
		//		}
		//
		//		protected void Start ()
		//		{
		//
		//		}
		//
		//		protected void Update ()
		//		{
		//
		//		}
		
		///////////////////////////////////////////////////////////////////////////
		//
		// InputModule Functions
		//


		//Handles hover and select
		protected virtual void UpdateCurrentObject () {

			// Send enter events and update the highlight.
			//			var go = pointerData.pointerCurrentRaycast.gameObject;

			HashSet<BaseRaycaster> baseRaycasters = new HashSet<BaseRaycaster> ();
			foreach (RaycastResult raycastResult in m_RaycastResultCache) {

				if (raycastResult.gameObject == null)
					continue;
				if (baseRaycasters.Contains (raycastResult.module))
					continue;

				GameObject go = raycastResult.gameObject;

				HandlePointerExitAndEnter (pointerData, go);
				// Update the current selection, or clear if it is no longer the current object.
				var selected = ExecuteEvents.GetEventHandler<ISelectHandler> (go);
				if (selected == eventSystem.currentSelectedGameObject) {
					ExecuteEvents.Execute (eventSystem.currentSelectedGameObject, GetBaseEventData (),
						ExecuteEvents.updateSelectedHandler);

					baseRaycasters.Add (raycastResult.module);
				} else {
					eventSystem.SetSelectedGameObject (null, pointerData);
				}
			}

			if (m_RaycastResultCache.Count == 0) {

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
		}

		////////////////////////////////////////
		//
		// Function Functions

	}
}
