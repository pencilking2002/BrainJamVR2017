using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Neuromancers
{
	public abstract class MotionController : MonoBehaviour, IInteractiveRaycastHandler, IMotionControlEventHandler
	{
		//enums

		/////Enums
		public enum MenuType {

			Application,
			Appearance,
			Render,
			Volume,
			Rotate,
			Scale,
		}
		public enum Handedness {

			Unknown,
			Left,
			Right
		}
		public enum Direction {
			Top,
			Right,
			Bottom,
			Left
		}
		public enum MotionControllerState
		{
			Active,
			Inactive
		}
		public enum MotionControllerTutorialType
		{
			Teleport,
			Grab,
			Action,
			Select,
		}
		//structs
		[System.Serializable]
		public struct MotionControllerTutorial
		{
			public MotionControllerTutorialType type;
			public GameObject gameObject;
		}
		[System.Serializable]
		public class MotionControllerMenu 
		{
			public MenuType menuType;
			public Direction direction;
			public GameObject gameObject;
			public Renderer discRenderer;
			[System.NonSerializedAttribute]
			public Material discMaterial;
			[System.NonSerializedAttribute]
			public bool enabled;
			[System.NonSerializedAttribute]
			public bool isVisible;
			[System.NonSerializedAttribute]
			public bool isJoystickSelected;
		}
		//readonly
		protected static readonly float LINE_RENDERER_FADE_DURATION = .5f;
		protected readonly float OFFSET_DISTANCE = 0;
		protected readonly int VELOCITY_COUNT = 5;
		protected static readonly float VELOCITY_INTERVAL = .1f;//in seconds
		protected static readonly float TUTORIAL_APPEAR_TIME = 30f;
		protected static readonly float TUTORIAL_FADE_DURATION = 1f;
		protected readonly Color CANVAS_GRAPHIC_INTERACTABLE_COLOR = new Color(.5f,1f,.5f,.5f);
		protected readonly Color CANVAS_GRAPHIC_COLLIDER_COLOR = new Color(1f,1f,1f,.35f);

		//Serialized
		[SerializeField]
		protected UnityEngine.EventSystems.MotionControlRaycaster motionControlRaycaster;
		[SerializeField]
		protected UnityEngine.UI.Graphic canvasGraphic;
		[SerializeField]
		protected float lineRendererStartAlpha=.45f;
		[SerializeField]
		protected float lineRendererEndAlpha=.15f;
		[SerializeField]
		protected Canvas canvasLeft;
		[SerializeField]
		protected Canvas canvasRight;
		[SerializeField]
		protected Canvas canvasTutorial;
		[SerializeField]
		protected MotionControllerTutorial[] tutorials;
		[Header("Grip Connector")]
		[SerializeField]
		protected Transform gripStartTransform;
		[SerializeField]
		protected Transform gripConnectorTransform;
		[SerializeField]
		protected Transform gripConnectorDiscOneTransform;
		[SerializeField]
		protected Transform gripConnectorDiscTwoTransform;
		[SerializeField]
		protected Transform gripConnectorTextQuadTransform;
		[SerializeField]
		protected UnityEngine.UI.Text gripConnectorText;
		[Header("MenuCanvases")]
		[SerializeField]
		protected MotionControllerMenu[] menus;
		
		/////Protected/////
		//References
		protected Dictionary<MotionControllerTutorialType,bool> tutorialEnabledDict;
		protected Queue<Vector3> recentPositions = new Queue<Vector3>();
//		protected SteamVR_TrackedObject trackedObject;
		protected LineRenderer lineRenderer;
		protected Transform glowQuadTransform;
		protected MeshRenderer glowQuadMeshRenderer;
		protected UnityEngine.UI.Text canvasLeftText;
		protected UnityEngine.UI.Text canvasRightText;
		protected UIPrimitives.CanvasGroupAnimator tutorialCanvasGroupAnimator;
		protected Material gripConnectorMaterial;
		protected Vector3 raycasterLocalPosition;
		//Primitives
		protected int selectedPropertyID;
		protected int tilingMultiplierXPropertyID;
		protected float canvasGraphicStartAlpha;
		protected float lastInteractionTime;
		protected string currentTooltipText = "";
		protected bool isCanvasEnabled = false;
		protected bool isTutorialEnabled = true;
		protected bool hitInteractable = false;
		protected bool isShowingGripConnector = false;
		protected float lastVelocityUpdateTime = - VELOCITY_INTERVAL;
		protected float lastValidCollisionTime = -LINE_RENDERER_FADE_DURATION;

		///////////////////////////////////////////////////////////////////////////
		//
		// Inherited from MonoBehaviour
		//
		
		protected virtual void Awake () {
			
			canvasLeftText = canvasLeft.GetComponentInChildren<UnityEngine.UI.Text>();
			canvasRightText = canvasRight.GetComponentInChildren<UnityEngine.UI.Text>();

			if(canvasTutorial!=null)
				tutorialCanvasGroupAnimator = canvasTutorial.GetComponent<UIPrimitives.CanvasGroupAnimator>();

			lineRenderer = GetComponentInChildren<LineRenderer> ();

			canvasGraphicStartAlpha = canvasGraphic.color.a;
			glowQuadTransform = lineRenderer.transform.GetChild (0);
			glowQuadMeshRenderer = glowQuadTransform.GetComponent<MeshRenderer> ();

			//Grip Connector
			if(gripConnectorTransform!=null)
				gripConnectorMaterial = gripConnectorTransform.GetComponentInChildren<Renderer>().material;

			this.selectedPropertyID = Shader.PropertyToID("_Selected");
			this.tilingMultiplierXPropertyID = Shader.PropertyToID ("_TilingMultiplierX");

			StartCoroutine(InitializeMenus());
		}

		protected virtual void Start () {


			InputManager.Instance.RegisterMotionControlEventGameObject(this.gameObject);
			InputManager.Instance.RegisterInteractiveRaycastGameObject (this.gameObject);
			SetEnabledTutorialTypes (Neuromancers.UI.MotionInputModule.tutorialTypes);
			HideTutorial();

			ClearGripConnector();

			raycasterLocalPosition = motionControlRaycaster.transform.localPosition;
			lineRenderer.SetPosition (0,raycasterLocalPosition );
		}

		protected virtual void Update () {

			UpdateVelocity();

			UpdateLineRenderer();

			if(isCanvasEnabled)
				UpdateCanvas();

			UpdateTutorial();

			if (isShowingGripConnector) {

				UpdateGripConnector();
			}

			UpdateMenu();
		}

		protected void OnCollisionEnter(Collision collision) {

            Vector3 force = GetVelocity() * 50f;
           // Debug.Log("adding force: " + force);
			collision.rigidbody.AddForce(force);
		}

		protected void OnEnable() {

			SetEnabledTutorialTypes (Neuromancers.UI.MotionInputModule.tutorialTypes);
		}

		///////////////////////////////////////////////////////////////////////////
		//
		// MotionController Functions
		//

		////////////////////////////////////////
		//
		// Menu Functions

		public void SetMenuEnabled(MenuType menuType, bool value) {

			for (int i = 0; i < menus.Length; ++i) {

				if(menus[i].menuType==menuType) {

					MotionControllerMenu menu = menus[i];
					menu.enabled = value;
					menu.discRenderer.enabled = value;
					if(menu.isVisible && !menu.enabled)
						HideMenu(menu);
				}
			}

			bool allMenusEnabled = true;
			for (int i = 0; i < menus.Length; ++i) {
				
				allMenusEnabled &= menus[i].enabled;
			}

			if(allMenusEnabled)
				SetThumbstickMenu();
			else
				SetThumbstickNormal();
		}

		protected readonly float MENU_JOYSTICK_THRESHOLD = .5f;

		protected void UpdateMenu() {
			
			for (int i = 0; i < menus.Length; ++i) {

				MotionControllerMenu menu = menus[i];

				if(!menu.enabled)
					continue;

				float joystickPercent = GetJoystickDirectionPercent(menu.direction);

				float selectedPercent = (joystickPercent * .82f)/MENU_JOYSTICK_THRESHOLD;//.82 is the max because of the way i UV'ed the disc
				menu.discMaterial.SetFloat(selectedPropertyID,selectedPercent);

				bool shouldJoystickBeSelected = joystickPercent > MENU_JOYSTICK_THRESHOLD;
				if(shouldJoystickBeSelected && shouldJoystickBeSelected!=menu.isJoystickSelected) {

					bool shouldShowMenu = !menu.isVisible;
					HideAllMenus();
					if(shouldShowMenu)
						ShowMenu(menu);
				}

				menu.isJoystickSelected = shouldJoystickBeSelected;
			}

			if(IsJoystickDown()) {

				HideAllMenus();
			}
		}

		protected IEnumerator InitializeMenus() {

			for (int i = 0; i < menus.Length; ++i) {

				menus[i].discMaterial = menus[i].discRenderer.material;
			}
			yield return null;
			HideAllMenus();
		}

		protected void ShowMenu(MotionControllerMenu menu) {

			if(menu.gameObject!=null)
				menu.gameObject.SetActive(true);
			menu.isVisible = true;
		}

		protected void HideAllMenus() {

			for (int i = 0; i < menus.Length; ++i) {
				
				HideMenu(menus[i]);
			}
		}

		protected void HideMenu(MotionControllerMenu menu) {

			if(menu.gameObject!=null)
				menu.gameObject.SetActive(false);
			menu.isVisible = false;
		}

		////////////////////////////////////////
		//
		// Appearance Functions

		protected void UpdateLineRenderer() {

			float lineRendererVisibilityPercent = 1f- (Time.time - lastValidCollisionTime) / LINE_RENDERER_FADE_DURATION;
			lineRenderer.enabled = lineRendererVisibilityPercent > 0;
			if(lineRenderer.enabled) {
				Color startColor = Color.white.Alpha(lineRendererStartAlpha*lineRendererVisibilityPercent);
				Color endColorBase = hitInteractable ? CANVAS_GRAPHIC_INTERACTABLE_COLOR : CANVAS_GRAPHIC_COLLIDER_COLOR;
				Color endColor = endColorBase.Alpha(lineRendererEndAlpha*lineRendererVisibilityPercent);
				lineRenderer.SetColors(startColor,endColor);

				//canvasGraphic.color = canvasGraphic.color.Alpha (canvasGraphicStartAlpha * lineRendererVisibilityPercent);
			}
			canvasGraphic.enabled = lineRendererVisibilityPercent == 1f;
			canvasGraphic.color = hitInteractable ? CANVAS_GRAPHIC_INTERACTABLE_COLOR : CANVAS_GRAPHIC_COLLIDER_COLOR;
		}

		////////////////////////////////////////
		//
		// Tutorial Functions

		public void SetEnabledTutorialTypes(MotionControllerTutorialType[] tutorials) {

			tutorialEnabledDict = new Dictionary<MotionControllerTutorialType, bool>();

			//add the sent ones as true
			if (tutorials != null) {
				for (int i = 0; i < tutorials.Length; ++i) {
			
					tutorialEnabledDict.Add (tutorials [i], true);
				}
			}
			//add the rest false
			foreach(MotionControllerTutorialType tutorial in Enum.GetValues(typeof(MotionControllerTutorialType))) {

				if(!tutorialEnabledDict.ContainsKey(tutorial))
					tutorialEnabledDict.Add(tutorial,false);
			}
			UpdateEnabledTutorials();
		}

		protected void UpdateEnabledTutorials() {

			for (int i = 0; i < tutorials.Length; ++i) {

				tutorials[i].gameObject.SetActive(tutorialEnabledDict[tutorials[i].type]);
			}
		}

		protected void UpdateTutorial() {

			if(isTutorialEnabled) {
				return;
			}

			if(Time.time-lastInteractionTime > TUTORIAL_APPEAR_TIME) {

//				ShowTutorial();
			}
		}

		protected void ShowTutorial() {

			if(tutorialCanvasGroupAnimator==null)
				return;
			
			tutorialCanvasGroupAnimator.AddAlphaEndAnimation(null,1f,TUTORIAL_FADE_DURATION,UIPrimitives.UIAnimationUtility.EaseType.easeOutSine);
			isTutorialEnabled = true;
		}

		protected void HideTutorial() {

			if(tutorialCanvasGroupAnimator==null)
				return;

			tutorialCanvasGroupAnimator.AddAlphaEndAnimation(null,0f,TUTORIAL_FADE_DURATION,UIPrimitives.UIAnimationUtility.EaseType.easeInSine);
			isTutorialEnabled = false;
		}

		////////////////////////////////////////
		//
		// Grip Particle Functions

		protected void StartGripConnector(Transform targetTransform,string displayText) {

			gripConnectorTargetTransform = targetTransform;

			if(gripStartTransform!=null) {

//				gripParticleSystem.SetEmissionEnabled (true);
				SetGripConnectorVisible(true);
				gripConnectorText.text = displayText;
//				ParticleSystem.VelocityOverLifetimeModule module = gripParticleSystem.velocityOverLifetime;

			}
		}

		protected void ClearGripConnector() {

			gripConnectorTargetTransform = null;
			if (gripStartTransform != null) {

				SetGripConnectorVisible(false);
//				gripParticleSystem.SetEmissionEnabled (false);

			}
		}

		protected void SetGripConnectorVisible(bool value) {

			foreach (var renderer in gripConnectorTransform.parent.GetComponentsInChildren<Renderer>()) {

				renderer.enabled = value;
			}
			foreach (var graphic in gripConnectorTransform.parent.GetComponentsInChildren<UnityEngine.UI.Graphic>()) {

				graphic.enabled = value;
			}
		}

		protected void UpdateGripConnector () {

			if(this.gripConnectorTransform==null)
				return;

			Transform mainCameraTransform = Camera.main.transform;
			
			Vector3 startPosition = this.gripStartTransform.position;
			Vector3 endPosition = this.gripConnectorTargetTransform.position;

			this.gripConnectorTransform.position = Vector3.Lerp (startPosition, endPosition, .5f);

			float localScaleX = Vector3.Distance (startPosition, endPosition);

			Vector3 connectorLocalScale = this.gripConnectorTransform.localScale;
			connectorLocalScale.z = localScaleX;
			this.gripConnectorTransform.localScale = connectorLocalScale;

			this.gripConnectorTransform.LookAt (startPosition, mainCameraTransform.up);

			this.gripConnectorDiscOneTransform.position = startPosition;
			this.gripConnectorDiscOneTransform.forward = mainCameraTransform.forward;
			this.gripConnectorDiscTwoTransform.position = endPosition;
			this.gripConnectorDiscTwoTransform.forward = mainCameraTransform.forward;
			this.gripConnectorTextQuadTransform.transform.position = Vector3.Lerp (startPosition, endPosition, .75f);
			this.gripConnectorTextQuadTransform.forward = mainCameraTransform.forward;
			//this.gripConnectorTransform.up = Camera.main.transform.up;
			//			this.gripConnectorMaterial.SetFloat (tilingMultiplierXPropertyID, connectorLocalScale.z / connectorLocalScale.y);

		}

		////////////////////////////////////////
		//
		// Velocity Functions

		protected void UpdateVelocity() {

			if(Time.time-lastVelocityUpdateTime > VELOCITY_INTERVAL) {

				recentPositions.Enqueue(transform.parent.localPosition);
				if(recentPositions.Count>VELOCITY_COUNT)
					recentPositions.Dequeue();

				lastVelocityUpdateTime = Time.time;
			}
		}

		public Vector3 GetVelocity() {

			Vector3[] differences = new Vector3[recentPositions.Count-1];
			Vector3[] positions = recentPositions.ToArray();

			for (int i = 1; i < positions.Length; ++i) {

				differences[i-1] = positions[i] - positions[i-1];
			}

			Vector3 averageDifference = Utility.Utility.GetMeanVector(differences);
            averageDifference = transform.parent.TransformDirection(averageDifference);

			Vector3 velocity = averageDifference / (VELOCITY_INTERVAL);

			// Debug.Log("Vel:" + velocity);
			return velocity;
		}

		////////////////////////////////////////
		//
		// Tooltip Functions

		protected void SetTooltip(string text)
		{
			if(text=="") {
				ClearTooltip();
				return;
			}
			SetCanvasEnabled (true);
			currentTooltipText = text;
		}

		protected void ClearTooltip()
		{
			SetCanvasEnabled (false);
			currentTooltipText = "";
		}

		protected void SetCanvasEnabled(bool value) {

			isCanvasEnabled = value;

			if(value) {

				UpdateCanvas();
			} else {

				this.canvasLeft.enabled = false;
				this.canvasRight.enabled = false;
			}
		}

		protected void UpdateCanvas() {

			Transform mainCameraTransform = Camera.main.transform;
			float dot = Vector3.Dot(mainCameraTransform.right,transform.right);

			float angleDir = AngleDir(mainCameraTransform.forward,transform.forward,Vector3.up);

			canvasLeft.enabled = (angleDir>0);
			canvasRight.enabled = !canvasLeft.enabled;

			if(canvasLeft.enabled)
				canvasLeftText.text= currentTooltipText;
			if(canvasRight.enabled)
				canvasRightText.text= currentTooltipText;
		}

		//returns -1 when to the left, 1 to the right, and 0 for forward/backward
		public float AngleDir(Vector3 fwd, Vector3 targetDir, Vector3 up)
		{
			Vector3 perp = Vector3.Cross(fwd, targetDir);
			float dir = Vector3.Dot(perp, up);

			if (dir > 0.0f) {
				return 1.0f;
			} else if (dir < 0.0f) {
				return -1.0f;
			} else {
				return 0.0f;
			}
		}  

		////////////////////////////////////////
		//
		// Event Functions

		protected void OnControllerInteraction() {

			lastInteractionTime = Time.time;

			if(isTutorialEnabled)
				HideTutorial();
		}

		protected Transform gripConnectorTargetTransform;

		protected virtual void OnGripHover(bool isHovering, Transform transform, string text) {

			bool shouldShowGripConnector = isHovering && !IsMotionControlGripHeld();
			if(isShowingGripConnector != shouldShowGripConnector){

				if(shouldShowGripConnector)
					StartGripConnector(transform,text);
				else
					ClearGripConnector();
				
				isShowingGripConnector = shouldShowGripConnector;
			}

			if (isShowingGripConnector && transform != gripConnectorTargetTransform)
				StartGripConnector (transform,text);
		}
		
		////////////////////////////////////////
		//
		// Get/Set Functions

		protected void SetState (MotionControllerState state) {

		}


		protected void UpdateHitObjects (float distance, Transform element) {

//			Debug.Log ("dist:" + distance);
			Vector3 position = Vector3.forward * distance + raycasterLocalPosition;
			lineRenderer.SetPosition (1, position);
			glowQuadTransform.localPosition = position;
			canvasGraphic.transform.position = transform.TransformPoint(position);
			canvasGraphic.transform.forward = element.forward;
		}

		protected void SetLastValidCollisionTime () {
			lastValidCollisionTime = Time.time;
		}

		public abstract Handedness GetHandedness();

		public abstract int GetIndex();
		public abstract bool IsActive();
//		public abstract void OnHandednessUpdated();

		protected abstract bool IsMotionControlGripHeld();
		protected abstract bool IsJoystickDown();
		protected abstract void SetThumbstickNormal();
		protected abstract void SetThumbstickMenu();

		protected abstract float GetJoystickDirectionPercent(Direction direction);

		///////////////////////////////////////////////////////////////////////////
		//
		// Inherited from IInteractiveRaycastHandler
		//

		public void OnInteractiveRaycast (List<InteractiveRaycastEventData> dataList) {

			hitInteractable = false;
			bool hitCollider = false;
			float minDistance = float.MaxValue;
			Transform minElement = null;
			float minDistanceCollider = float.MaxValue;
			Transform minElementCollider = null;
			string text = null;

			foreach (InteractiveRaycastEventData data in dataList) {
				
				if(data.deviceIndex != GetIndex()) {

					continue;
				}

				if (data.eventType != InteractiveEventType.HitNone) {
					if (data.eventType == InteractiveEventType.HitInteractable && data.raycastResult.distance < minDistance){ 
						minDistance = data.raycastResult.distance;
						text = data.element.TooltipName;
						minElement = data.raycastResult.gameObject.transform;
					}
					if (data.eventType == InteractiveEventType.HitCollider && data.raycastResult.distance < minDistanceCollider) {
						
						minDistanceCollider = data.raycastResult.distance;
						minElementCollider = data.raycastResult.gameObject.transform;
					}

					hitInteractable |= data.eventType == InteractiveEventType.HitInteractable;
					hitCollider |= data.eventType == InteractiveEventType.HitCollider;
				}
			}

			if (!hitInteractable) {
				minDistance = minDistanceCollider;
				minElement = minElementCollider;
				ClearTooltip();
			}

			bool hitNothing = (!hitInteractable && !hitCollider);

			if (hitInteractable) {

				SetTooltip(text);
				UpdateHitObjects (minDistance - OFFSET_DISTANCE,minElement);
				SetLastValidCollisionTime ();
				SetState (MotionControllerState.Active);
			} else if (hitCollider) {
//				Debug.Log("setting hit collider"+minDistance);
				UpdateHitObjects (minDistance - OFFSET_DISTANCE,minElement);
				SetLastValidCollisionTime ();
				SetState (MotionControllerState.Inactive);
			} else {
//				SetHitDistance (currentCrosshairDistance);
				SetState (MotionControllerState.Inactive);
			}

            glowQuadMeshRenderer.enabled = false;// hitInteractable;
		}

		///////////////////////////////////////////////////////////////////////////
		//
		// Inherited from IMotionEventHandler
		//


		public void OnInteractiveGrip (List<InteractiveGripEventData> dataList) {

			bool didGripHover = false;
			Transform hoverTransform = null;
			string hoverText = null;

			foreach (InteractiveGripEventData data in dataList) {

				if(data.isGrippable==false)
					continue;

				if (data.gripEventType == GripEventType.GripHover) {

					didGripHover = true;
					hoverTransform = data.gameObject.transform;
					hoverText = data.tooltipText;
				}else {

				}
			}

			OnGripHover(didGripHover,hoverTransform,hoverText);
		}

		public void OnMotionControlTouchpadDown () {

			OnControllerInteraction();
		}

		public void OnMotionControlTouchpadUp () {

			OnControllerInteraction();
		}

		public void OnMotionControlMenuDown () {

			OnControllerInteraction();
		}

		public void OnMotionControlMenuUp () {

			OnControllerInteraction();
		}

		public void OnMotionControlGripDown () {

			OnControllerInteraction();
		}

		public void OnMotionControlGripUp () {

			OnControllerInteraction();
		}

		public void OnMotionControlTriggerDown () {

			OnControllerInteraction();
		}

		public void OnMotionControlTriggerUp () {

			OnControllerInteraction();
		}
	}
}
