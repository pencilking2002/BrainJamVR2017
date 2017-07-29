using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
using Neuromancers.UI;

namespace Neuromancers
{

	public enum InteractiveEventType
	{
		HitInteractable,
		HitCollider,
		HitNone
	}

	public enum TriggerEventType
	{
		TriggerDown,
		TriggerHeld,
		TriggerUp,
	}

	public enum GripEventType
	{
		GripDown,
		GripUp,
		GripHover,
	}

	//To differenitaite between OnPointClick events
	public enum ClickType
	{
		Mouse,
		MotionController
	}

//	public struct MotionControlGripEventData
//	{
//		public Transform transform;
//		public GripEventType eventType;
//		public int deviceIndex;
//
//		public MotionControlGripEventData (Transform transform, GripEventType eventType, int deviceIndex)
//		{
//			this.transform = transform;
//			this.eventType = eventType;
//			this.deviceIndex = deviceIndex;
//		}
//	}

	public struct ClosestGameObjectData {

		public GameObject gameObject;
		public float squaredDistance;

		public ClosestGameObjectData (GameObject gameObject, float distance)
		{
			this.gameObject = gameObject;
			this.squaredDistance = distance;
		}
		
	}
	public struct MotionControlTriggerEventData
	{
		public Transform transform;
		public TriggerEventType eventType;
		public int deviceIndex;

		public MotionControlTriggerEventData (Transform transform, TriggerEventType eventType, int deviceIndex)
		{
			this.transform = transform;
			this.eventType = eventType;
			this.deviceIndex = deviceIndex;
		}
	}


	public struct InteractiveGazeEventData
	{
		public InteractiveEventType eventType;
		public RaycastResult raycastResult;
		public Element element;

		public InteractiveGazeEventData (InteractiveEventType eventType, RaycastResult raycastResult, Element element)
		{
			this.eventType = eventType;
			this.raycastResult = raycastResult;
			this.element = element;
		}
	}

	public struct InteractiveRaycastEventData
	{
		public InteractiveEventType eventType;
		public RaycastResult raycastResult;
		public Element element;
		public int deviceIndex;

		public InteractiveRaycastEventData (InteractiveEventType eventType, RaycastResult raycastResult, Element element, int deviceIndex)
		{
			this.eventType = eventType;
			this.raycastResult = raycastResult;
			this.element = element;
			this.deviceIndex = deviceIndex;
		}
	}

	public struct InteractiveGripEventData
	{
		public GripEventType gripEventType;
		public IMotionControlGripEnterExitHandler gripHandler;
		public GameObject gameObject;
		public string tooltipText;
		public bool isGrippable;
//		public int deviceIndex;

		public InteractiveGripEventData (GripEventType gripEventType, IMotionControlGripEnterExitHandler gripHandler, GameObject gameObject, string tooltipText, bool isGrippable)
		{
			this.gripEventType = gripEventType;
			this.gripHandler = gripHandler;
			this.gameObject = gameObject;
			this.tooltipText = tooltipText;
			this.isGrippable = isGrippable;
//			this.deviceIndex = deviceIndex;
		}
		
	}

	/// <summary>
	/// Interface for Vive event handler. This is ONLY for the scripts that FIRE events. (Like MotionController)
	/// </summary>
	public interface IMotionControlEventHandler : IEventSystemHandler
	{
		//So we can only fire events to the corresponding controller
		int GetIndex();

		void OnInteractiveGrip(List<InteractiveGripEventData> data);
			
		void OnMotionControlTouchpadDown ();

		void OnMotionControlTouchpadUp ();

		void OnMotionControlMenuDown ();

		void OnMotionControlMenuUp ();

		void OnMotionControlGripDown ();

		void OnMotionControlGripUp ();

		void OnMotionControlTriggerDown ();

		void OnMotionControlTriggerUp ();
	}

	//	public interface IMotionControlEventHandler
	//	{
	//		void OnMotionControlGripDown();
	//		void OnMotionControlGripUp();
	//	}

	public interface IViveTriggerHandler : IEventSystemHandler
	{
		void OnViveTrigger (System.Collections.Generic.List<MotionControlTriggerEventData> data);
	}

	public interface IMotionControlGripHandler : IEventSystemHandler {

		Action OnGripDown (Transform transform);
	}

	public interface IMotionControlGripEnterExitHandler : IEventSystemHandler {

		void OnGripEnter (Transform transform);
		void OnGripExit (Transform transform);
		string TooltipText {get;}
		bool IsGrippable();
	}

	public interface IViveGripDownHandler : IEventSystemHandler
	{
		Action OnGripDown (Transform transform, RaycastResult raycastResult);
	}

	public interface IViveGripUpHandler : IEventSystemHandler
	{
		void OnGripUp ();
	}

	public interface IInteractiveGazeHandler : IEventSystemHandler
	{
		void OnInteractiveGaze (System.Collections.Generic.List<InteractiveGazeEventData> data);
	}

	public interface IInteractiveRaycastHandler : IEventSystemHandler
	{
		void OnInteractiveRaycast (System.Collections.Generic.List<InteractiveRaycastEventData> data);
	}

	/// <summary>
	/// An element that scales (size/color/etc) based on distance from the crosshair.
	/// </summary>
	public interface IDistScaleElement : IEventSystemHandler
	{
		/// <summary>
		/// The position of the crosshair has changed, this is where the object would scale.
		/// </summary>
		/// <param name="position">Position.</param>
		void OnDistScalePositionChanged (Vector3 position);
	}

	public class InputManager : Singleton<Neuromancers.InputManager>
	{
		//readonly
		protected readonly float MAXIMUM_GRIP_SQUARE_MAGNITUDE = .03f;

		//Serialized
		[SerializeField]
		protected GameObject standaloneInputModule;
		[SerializeField]
		protected InputModuleType activeInputModuleType;
		[SerializeField]
		protected InputModule[] inputModules;
		
		/////Protected/////
		//References
		protected InputModule activeInputModule;
		protected Crosshair crosshair;
		protected List<GameObject> viveTriggerListeners = new List<GameObject> ();
		protected List<GameObject> motionControlEventListeners = new List<GameObject> ();
		protected List<GameObject> motionControlGripListeners = new List<GameObject> ();
		protected List<GameObject> motionControlGripEnterExitListeners = new List<GameObject> ();
		protected List<GameObject> interactiveGazeListeners = new List<GameObject> ();
		protected List<GameObject> interactiveRaycastListeners = new List<GameObject> ();
		protected Audio audio;
		protected Action<bool> buttonHoveredAction;
		protected Action<Button> buttonSelectedAction;
		protected Dictionary<int,Queue<Action>> motionControlGripUpCallbacks = new Dictionary<int,Queue<Action>> ();

		//Primitives
		protected Vector2[] motionControlScrollValues = new Vector2[2];
		protected bool motionControlScrollValuesSet = false;
		protected bool[] motionControlThumbDown = new bool[]{false,false};//do something with this
		protected bool motionControlThumbDownValuesSet = false;
		protected bool motionControlTriggerDownSet = false;
		protected bool motionControlTriggerHeldSet = false;
		protected bool motionControlTriggerUpSet = false;
		protected bool motionControlGripDownSet = false;
		protected bool motionControlGripHeldSet = false;
		protected bool motionControlGripUpSet = false;
		//public static
		public static bool isMotionControlTriggerDown = false;
		public static bool isMotionControlTriggerHeld = false;
		public static bool isMotionControlTriggerUp = false;
		public static bool isMotionControlGripDown = false;
		public static bool isMotionControlGripHeld = false;
		public static bool isMotionControlGripUp = false;

		public static bool allowButtonHoverFill  { get; set; }

		//Actions
		public Action GlobalTriggerAction { get; set; }
		//		public Action GlobalTriggerDownAction { get; set; }
		//		public Action GlobalTriggerHeldAction { get; set; }
		//		public Action GlobalTriggerUpAction { get; set; }
		public Action GlobalBackAction { get; set; }

//		public Action<Neuromancers.Application.QualityLevel> QualityLevelSelectedAction;

		////////////////////////////////////////
		//
		// Properties
		
		public Action<bool> ButtonHoveredAction {
			set {
				this.buttonHoveredAction = value;
			}
		}

		public Action<Button> ButtonSelectedAction {
			set {
				this.buttonSelectedAction = value;
			}
		}

		public Vector2[] MotionControlScrollValues {
			get {
				return this.motionControlScrollValues;
			}
		}

		public bool[] MotionControlThumbDown {
			get {
				return this.motionControlThumbDown;
			}
		}

		///////////////////////////////////////////////////////////////////////////
		//
		// Inherited from MonoBehaviour
		//
		
		protected virtual void Awake () {
			
			this.audio = Audio.Instance;

			SetActiveInputModule ();
			RegisterActiveInputModuleEvents ();


//			if (gazeInputModule == null)
//				gazeInputModule = FindObjectOfType<GazeInputModule> ();
//
//			if (viveInputModule == null)
//				viveInputModule = FindObjectOfType<ViveInputModule> ();
//
//			if (gazeInputModule != null) {
//				gazeInputModule.InteractiveGazeAction += OnInteractiveGaze;
//				gazeInputModule.GlobalTriggerAction += OnGlobalTrigger;
//			}
//
//			if (viveInputModule != null) {
//				viveInputModule.InteractiveRaycastAction += OnInteractiveRaycast;
//				viveInputModule.TriggerAction += OnViveTrigger;
//				viveInputModule.GlobalBackAction += OnGlobalBack;
//			}
			//if rift input module

			crosshair = Crosshair.Instance;

			
		}

		protected virtual void Start () {


		}

		protected virtual void Update () {

			//Back button on OVR Remote, B button on Xbox One Controller
			if (OVRInput.GetDown (OVRInput.Button.Two))
				OnGlobalBack ();

			if (motionControlScrollValuesSet == false)
				ClearMotionControlScrollValues ();
			if (motionControlThumbDownValuesSet == false)
				ClearMotionControlThumbDownValues ();

			if (motionControlTriggerDownSet == false)
				isMotionControlTriggerDown = false;
			if (motionControlTriggerHeldSet == false)
				isMotionControlTriggerHeld = false;
			if (motionControlTriggerUpSet == false)
				isMotionControlTriggerUp = false;
			if (motionControlGripDownSet == false)
				isMotionControlGripDown = false;
			if (motionControlGripHeldSet == false)
				isMotionControlGripHeld = false;
			if (motionControlGripUpSet == false)
				isMotionControlGripUp = false;

			motionControlScrollValuesSet = false;
			motionControlThumbDownValuesSet = false;
			motionControlTriggerDownSet = false;
			motionControlTriggerHeldSet = false;
			motionControlTriggerUpSet = false;
			motionControlGripDownSet = false;
			motionControlGripHeldSet = false;
			motionControlGripUpSet = false;
		}

		protected void LateUpdate () {

		}

		protected void ClearMotionControlScrollValues () {

			this.motionControlScrollValues [0] = Vector2.zero;
			this.motionControlScrollValues [1] = Vector2.zero;
		}

		protected void ClearMotionControlThumbDownValues () {

			this.motionControlThumbDown [0] = false;
			this.motionControlThumbDown [1] = false;
		}

		///////////////////////////////////////////////////////////////////////////
		//
		// Input Functions
		//

		public MotionController[] GetMotionControllers() {

			if(activeInputModule==null)
				return null;
			MotionInputModule motionInputModule = activeInputModule.GetComponent<MotionInputModule> ();
			if (motionInputModule == null)
				return null;

			return motionInputModule.GetControllers ();
		}

		public MotionController GetLeftMotionController() {

			MotionInputModule motionInputModule = activeInputModule.GetComponent<MotionInputModule> ();
			if (motionInputModule == null)
				return null;

			return motionInputModule.GetLeftController ();
		}

		public MotionController GetRightMotionController() {

			MotionInputModule motionInputModule = activeInputModule.GetComponent<MotionInputModule> ();
			if (motionInputModule == null)
				return null;

			return motionInputModule.GetRightController ();
		}

		protected void SetActiveInputModule () {

			for (int i = 0; i < inputModules.Length; ++i) {

				bool matchesType = (inputModules [i].InputModuleType == activeInputModuleType);
				if (matchesType) {
				
					this.activeInputModule = inputModules [i];
					Debug.Log ("Setting active input module for type: " + this.activeInputModuleType);
//					return;
				}

				inputModules [i].gameObject.SetActive (matchesType);
			}

			this.standaloneInputModule.SetActive (activeInputModuleType == InputModuleType.Standalone);

//			Debug.Log("Could not find input module to set active for type: "+this.activeInputModuleType);
		}

		protected void RegisterActiveInputModuleEvents () {

			if (this.activeInputModule == null)
				return;
			
			this.activeInputModule.InteractiveRaycastAction += OnInteractiveRaycast;
			this.activeInputModule.ScrollAction += OnMotionControlScroll;
			this.activeInputModule.ThumbDownAction += OnMotionControlThumbDown;
			this.activeInputModule.TriggerAction += OnMotionControlTrigger;
			this.activeInputModule.GripAction += OnMotionControlGrip;
			this.activeInputModule.GripMoveAction += OnMotionControlGripMove;
			this.activeInputModule.GlobalTriggerAction += OnGlobalTrigger;
			this.activeInputModule.GlobalTriggerDownAction += OnGlobalTriggerDown;
			this.activeInputModule.GlobalTriggerHeldAction += OnGlobalTriggerHeld;
			this.activeInputModule.GlobalTriggerUpAction += OnGlobalTriggerUp;
			this.activeInputModule.GlobalGripDownAction += OnGlobalGripDown;
			this.activeInputModule.GlobalGripHeldAction += OnGlobalGripHeld;
			this.activeInputModule.GlobalGripUpAction += OnGlobalGripUp;
			this.activeInputModule.GlobalBackAction += OnGlobalBack;
		}


		////////////////////////////////////////
		//
		// Crosshair Functions

		public void ForceDisableCrosshair () {

			this.crosshair.ForceDisable ();
		}


		////////////////////////////////////////
		//
		// Register Functions

		public virtual void RegisterSlider (UIPrimitives.UISlider slider) {
			if (slider == null)
				return;
			slider.HoveredAction += OnButtonHovered;
			slider.SelectedAction += OnSliderSelected;
		}

		public virtual void RegisterButton (Button button) {
			if (button == null)
				return;
			button.HoveredAction	+=	OnButtonHovered;
			button.SelectedAction	+=	OnButtonSelected;
		}

		public virtual void RegisterButtons (Button[] buttons) {
			if (buttons == null)
				return;
			for (int i = 0; i < buttons.Length; ++i)
				RegisterButton (buttons [i]);
		}

		////////////////////////////////////////
		//
		// Event Functions

		protected virtual void OnButtonHovered (bool value) {
			if (this.buttonHoveredAction != null)
				this.buttonHoveredAction (value);
			
			
			//PlayHoverSound (value);
		}

		protected virtual void OnButtonSelected (Button button) {
			if (this.buttonSelectedAction != null)
				this.buttonSelectedAction (button);

			PlaySelectSound ();
		}

		protected virtual void OnSliderSelected (UIPrimitives.UISlider slider) {

			PlaySelectSound ();
		}

		protected void OnQualityLevelSelected (int index) {

//			if (QualityLevelSelectedAction != null)
//				QualityLevelSelectedAction ((Neuromancers.Application.QualityLevel)index);
		}

		////////////////////////////////////////
		//
		// Audio Functions

		public void PlayHoverSound (bool value) {
			
			if (value)
				this.audio.PlaySoundEffect (Audio.SoundEffect.UIHoverOn);
//			else
//				this.audio.PlaySoundEffect (Audio.SoundEffect.UIHoverOff);
		}

		public void PlaySelectSound () {
			
			this.audio.PlaySoundEffect (Audio.SoundEffect.UISelect);
		}

		////////////////////////////////////////
		//
		// Gaze Functions

		public void RegisterViveTriggerGameObject (GameObject gameObjectToRegister) {
			if (!viveTriggerListeners.Contains (gameObjectToRegister))
				viveTriggerListeners.Add (gameObjectToRegister);
		}

		public void RegisterInteractiveGazeGameObject (GameObject gameObjectToRegister) {
			if (!interactiveGazeListeners.Contains (gameObjectToRegister))
				interactiveGazeListeners.Add (gameObjectToRegister);
		}

		public void RegisterInteractiveRaycastGameObject (GameObject gameObjectToRegister) {
			if (!interactiveRaycastListeners.Contains (gameObjectToRegister))
				interactiveRaycastListeners.Add (gameObjectToRegister);
		}

		public void RegisterMotionControlEventGameObject (GameObject gameObjectToRegister) {
			if (!motionControlEventListeners.Contains (gameObjectToRegister))
				motionControlEventListeners.Add (gameObjectToRegister);
		}

		public void RegisterMotionControlGripGameObject (GameObject gameObjectToRegister) {
			if (!motionControlGripListeners.Contains (gameObjectToRegister))
				motionControlGripListeners.Add (gameObjectToRegister);
		}

		public void DeregisterMotionControlGripGameObject (GameObject gameObjectToDeregister) {
			if (motionControlGripListeners.Contains (gameObjectToDeregister))
				motionControlGripListeners.Remove (gameObjectToDeregister);
		}

		public void RegisterMotionControlGripEnterExitGameObject (GameObject gameObjectToRegister) {
			if (!motionControlGripEnterExitListeners.Contains (gameObjectToRegister))
				motionControlGripEnterExitListeners.Add (gameObjectToRegister);
		}

		public void DeregisterMotionControlGripEnterExitGameObject (GameObject gameObjectToDeregister) {
			if (motionControlGripEnterExitListeners.Contains (gameObjectToDeregister))
				motionControlGripEnterExitListeners.Remove (gameObjectToDeregister);
		}

		protected void OnMotionControlTrigger (List<MotionControlTriggerEventData> viveTriggerDataList) {

			//Fire the interactive gaze events
			foreach (GameObject listenerGameObject in viveTriggerListeners)
				ExecuteEvents.Execute<IViveTriggerHandler> (listenerGameObject, null, (x, y) => x.OnViveTrigger (viveTriggerDataList));
		}

//		protected void OnMotionControlGrip (GameObject listener, MotionControlGripEventData motionControlGripData) {
//			
//			ExecuteEvents.Execute<IMotionControlGripHandler> (listener, null, (x, y) => x.OnGripDown (motionControlGripData.transform));
//		}

		protected void OnInteractiveGaze (List<InteractiveGazeEventData> interactiveGazeEventDataList) {

			//Fire the interactive gaze events
			foreach (GameObject gazeListenerGameObject in interactiveGazeListeners)
				ExecuteEvents.Execute<IInteractiveGazeHandler> (gazeListenerGameObject, null, (x, y) => x.OnInteractiveGaze (interactiveGazeEventDataList));
		}
		protected void OnMotionControlEvent (List<InteractiveGripEventData> eventDataList,int index) {

			//Fire the interactive gaze events
			foreach (GameObject listenerGameObject in motionControlEventListeners) {

				if(listenerGameObject.GetComponent<IMotionControlEventHandler>().GetIndex() == index)
					ExecuteEvents.Execute<IMotionControlEventHandler> (listenerGameObject, null, (x, y) => x.OnInteractiveGrip (eventDataList));
			}
		}

		protected void OnInteractiveRaycast (List<InteractiveRaycastEventData> interactiveRaycastEventDataList) {
			
//			Fire the interactive gaze events
			foreach (GameObject raycastListenerGameObject in interactiveRaycastListeners)
				ExecuteEvents.Execute<IInteractiveRaycastHandler> (raycastListenerGameObject, null, (x, y) => x.OnInteractiveRaycast (interactiveRaycastEventDataList));
			
		}

		protected void OnGlobalBack () {

			if (GlobalBackAction != null)
				GlobalBackAction ();
		}

		protected void OnGlobalTrigger () {

			if (GlobalTriggerAction != null)
				GlobalTriggerAction ();
		}

		protected void OnGlobalTriggerDown () {


			this.motionControlTriggerDownSet = true;
			isMotionControlTriggerDown = true;
		}

		protected void OnGlobalTriggerHeld () {

			this.motionControlTriggerHeldSet = true;
			isMotionControlTriggerHeld = true;
		}

		protected void OnGlobalTriggerUp () {

			this.motionControlTriggerUpSet = true;
			isMotionControlTriggerUp = true;
		}

		protected void OnGlobalGripDown () {
			
			this.motionControlGripDownSet = true;
			isMotionControlGripDown = true;
		}

		protected void OnGlobalGripHeld () {

			this.motionControlGripHeldSet = true;
			isMotionControlGripHeld = true;
		}

		protected void OnGlobalGripUp () {

			this.motionControlGripUpSet = true;
			isMotionControlGripUp = true;
		}

		protected void OnMotionControlScroll (Vector2[] scrollValues) {

			//Debug.Log (scrollValues[1]);
			this.motionControlScrollValuesSet = true;
			this.motionControlScrollValues = scrollValues;
		}

		protected void OnMotionControlThumbDown(int index) {

			this.motionControlThumbDownValuesSet = true;
			this.motionControlThumbDown[index] = true;
		}

		protected void OnMotionControlTrigger (Transform transform, TriggerEventType viveTriggerEventType, int deviceIndex) {

			OnMotionControlTrigger (new List<MotionControlTriggerEventData> (){ new MotionControlTriggerEventData (transform, viveTriggerEventType, deviceIndex) });
		}

		protected void OnMotionControlGrip (Transform transform, GripEventType gripEventType, int deviceIndex) {

//			MotionControlGripEventData eventdata = new MotionControlGripEventData(transform,gripEventType,deviceIndex);

			if (gripEventType == GripEventType.GripDown) {

				ClosestGameObjectData closestGameObjectData =  GetClosestGameObjectInList(motionControlGripListeners,transform);
					
				if(closestGameObjectData.gameObject!=null && closestGameObjectData.squaredDistance < MAXIMUM_GRIP_SQUARE_MAGNITUDE) {

					AddGripUpCallBack(deviceIndex,closestGameObjectData.gameObject.GetComponent<IMotionControlGripHandler>().OnGripDown(transform));
				}

			} else {//Grip Up

				ClearGripUpCallBacks(deviceIndex);
			}
		}

		protected Dictionary<int,IMotionControlGripEnterExitHandler> activeMotionControlGripHoverDict = new Dictionary<int, IMotionControlGripEnterExitHandler>();

		public bool IsMotionControlGripHovering(int deviceIndex) {

			return activeMotionControlGripHoverDict.ContainsKey(deviceIndex);
		}

		protected void OnMotionControlGripMove(Transform transform, int deviceIndex) {
			
			ClosestGameObjectData closestGameObjectData =  GetClosestGameObjectInList(motionControlGripEnterExitListeners,transform);
			List<InteractiveGripEventData> gripEventData = new List<InteractiveGripEventData>();
            if (closestGameObjectData.gameObject == null)
                return;
			IMotionControlGripEnterExitHandler closestHandler = closestGameObjectData.gameObject.GetComponent<IMotionControlGripEnterExitHandler>();

			if(closestGameObjectData.gameObject!=null && closestGameObjectData.squaredDistance < MAXIMUM_GRIP_SQUARE_MAGNITUDE && closestHandler.IsGrippable()) {

				if(activeMotionControlGripHoverDict.ContainsKey(deviceIndex)) {

					if(activeMotionControlGripHoverDict[deviceIndex]==closestHandler) {

						//nothing to do
					} else {

						activeMotionControlGripHoverDict[deviceIndex].OnGripExit(transform);
						activeMotionControlGripHoverDict[deviceIndex] = closestHandler;
						activeMotionControlGripHoverDict[deviceIndex].OnGripEnter(transform);
					}

				} else {

					activeMotionControlGripHoverDict.Add(deviceIndex,closestHandler);
					activeMotionControlGripHoverDict[deviceIndex].OnGripEnter(transform);
				}

				gripEventData.Add(new InteractiveGripEventData(GripEventType.GripHover,closestHandler,closestGameObjectData.gameObject,closestHandler.TooltipText,closestHandler.IsGrippable()));
				
			}else {

				if(activeMotionControlGripHoverDict.ContainsKey(deviceIndex)) {

					activeMotionControlGripHoverDict[deviceIndex].OnGripExit(transform);
					activeMotionControlGripHoverDict.Remove(deviceIndex);
				}
			}

			OnMotionControlEvent(gripEventData,deviceIndex);
//			activeMotionControlGripHoverDict
		}


		protected ClosestGameObjectData GetClosestGameObjectInList(List<GameObject> list, Transform transform) {

			float minSquareMagnitude = float.MaxValue;
			GameObject closestListener = null;
			for (int i = 0; i < list.Count; ++i) {

				float squareMagnitude = (list[i].transform.position - transform.position).sqrMagnitude;
				if(squareMagnitude<minSquareMagnitude) {

					minSquareMagnitude = squareMagnitude;
					closestListener = list[i];
				}
			}

			return new ClosestGameObjectData(closestListener,minSquareMagnitude);
		}


		protected void AddGripUpCallBack(int index, Action callback) {

			Queue<Action> gripUpCallbacksQueue = null;
			if(motionControlGripUpCallbacks.ContainsKey(index))
				gripUpCallbacksQueue = motionControlGripUpCallbacks[index];
			else {
				gripUpCallbacksQueue = new Queue<Action>();
				motionControlGripUpCallbacks.Add(index,gripUpCallbacksQueue);
			}

			gripUpCallbacksQueue.Enqueue(callback);
		}

		protected void ClearGripUpCallBacks(int index) {

			if(!motionControlGripUpCallbacks.ContainsKey(index))
				return;

			Queue<Action> gripUpCallbacksQueue = motionControlGripUpCallbacks[index];

			while(gripUpCallbacksQueue.Count>0) {

				Action action = gripUpCallbacksQueue.Dequeue();
				if(action!=null)
					action();
			}
		}


	}
}
