using UnityEngine;
using System.Collections.Generic;

namespace Neuromancers.UI
{
	public class Crosshair : Singleton<Crosshair>, IInteractiveGazeHandler
	{
		//readonly
		private readonly float CROSSHAIR_MIN_DISTANCE = .62f;
		protected readonly float OFFSET_DISTANCE = .2f;
		protected readonly int MAX_CHECK_COUNT = 2;
//How many parents to check after hitting a collider for a UIElement
		protected readonly float SCALE_MULTIPLIER = 0.02741935484f;
		protected static readonly float FADE_DURATION = 1f;

		//enum
		public enum CrosshairState
		{
			Open,
			Closed,
			Connecting,
			Disabled
		}

		//Serialized
		[Header ("Textures")]
		[SerializeField]
		protected Sprite openTexture;
		[SerializeField]
		protected Sprite closedTexture;
		[SerializeField]
		protected Transform crosshairTransform;
		//		[Header("Materials")]
		//		[SerializeField]
		//		protected Material staticMaterial;
		//		[SerializeField]
		//		protected Material connectingMaterial;
  
		/////Protected//
		//References
		protected Transform mainCameraTransform;
		protected SpriteRenderer crosshairSpriteRenderer;
		protected CrosshairState crosshairState = CrosshairState.Open;
		protected Canvas canvas;
		protected UnityEngine.UI.Text canvasText;
		//Primitives
		/////Protected_Connector//
		//References
		protected Transform connectorTransform;
		protected Material connectorMaterial;
		protected AnimationCurve fadeAnimationCurve;
		//Primitives
		protected bool isConnecting;
		protected Vector3 connectingStartPosition;
		protected float currentCrosshairDistance;
		protected int tilingMultiplierXPropertyID;
		protected int alphaMultiplierPropertyID;
		protected float lastValidCollisionTime = -FADE_DURATION;
		protected bool isVisible;



		///////////////////////////////////////////////////////////////////////////
		//
		// Inherited from MonoBehaviour
		//

		protected void Awake () {
			base.SaveInstance (this);

			this.mainCameraTransform = GameObject.FindGameObjectWithTag ("MainCamera").transform;

//			this.crosshairTransform = this.transform.FindChild("Crosshair_Mesh");
			this.crosshairSpriteRenderer = this.crosshairTransform.GetComponent<SpriteRenderer> ();

			this.connectorTransform = this.transform.Find ("Crosshair_Connector_Mesh");
			this.connectorMaterial = this.connectorTransform.GetComponent<MeshRenderer> ().sharedMaterial;

			this.tilingMultiplierXPropertyID = Shader.PropertyToID ("_TilingMultiplierX");
			this.alphaMultiplierPropertyID = Shader.PropertyToID ("_AlphaMultiplier");

			this.canvas = GetComponentInChildren<Canvas> ();
			this.canvasText = this.canvas.GetComponentInChildren<UnityEngine.UI.Text> ();

			this.currentCrosshairDistance = CROSSHAIR_MIN_DISTANCE;

			this.fadeAnimationCurve = UIPrimitives.UIAnimationUtility.GetCurve (UIPrimitives.UIAnimationUtility.EaseType.easeOutSine);
		}

		protected void Start () {
			SetState (CrosshairState.Open);

			InputManager.Instance.RegisterInteractiveGazeGameObject (this.gameObject);
		}

		protected void Update () {
//			UpdateRaycast();

			if (this.isConnecting)
				UpdateConnector ();

			UpdateFadePercent ();

		}

		///////////////////////////////////////////////////////////////////////////
		//
		// Crosshair functions
		//

		public void ForceDisable () {

			SetState (CrosshairState.Disabled);
		}

		protected void UpdateRaycast () {
			Vector3 raycastOrigin = this.mainCameraTransform.position;
			Vector3 raycastDirection = this.mainCameraTransform.forward;
			Ray ray = new Ray (raycastOrigin, raycastDirection);
			
			RaycastHit raycastInfo;
			if (Physics.Raycast (ray, out raycastInfo, 100f)) {
				Element hitUIElement = null; 
				Transform transformToCheckForUIElement = raycastInfo.collider.transform;
				int checkCount = 0;
				while (transformToCheckForUIElement != null && hitUIElement == null && checkCount < MAX_CHECK_COUNT) {
					hitUIElement = transformToCheckForUIElement.GetComponent<Element> ();
					checkCount++;
					transformToCheckForUIElement = transformToCheckForUIElement.parent;
				}
				
				LerpCrosshair (raycastInfo.distance - OFFSET_DISTANCE);

				bool didHitValidUIElement = hitUIElement != null && hitUIElement;
				if (didHitValidUIElement) {
					SetTooltip (hitUIElement.TooltipName);
					SetLastValidCollisionTime ();
					
				} else {	
					ClearTooltip ();
				}

			} else {
				LerpCrosshair (currentCrosshairDistance);
				ClearTooltip ();
			}
		}


		public void OnInteractiveGaze (List<InteractiveGazeEventData> dataList) {
			
			bool hitInteractable = false;
			bool hitCollider = false;
			float minDistance = float.MaxValue;
			float minDistanceCollider = float.MaxValue;
			string text = null;

			foreach (InteractiveGazeEventData data in dataList) {
				if (data.eventType != InteractiveEventType.HitNone) {
					if (data.eventType == InteractiveEventType.HitInteractable && data.raycastResult.distance < minDistance) { 
						minDistance = data.raycastResult.distance;
						text = data.element.TooltipName;
					}
					if (data.eventType == InteractiveEventType.HitCollider && data.raycastResult.distance < minDistanceCollider)
						minDistanceCollider = data.raycastResult.distance;

					hitInteractable |= data.eventType == InteractiveEventType.HitInteractable;
					hitCollider |= data.eventType == InteractiveEventType.HitCollider;
				}
			}

			if (!hitInteractable) { 
				minDistance = minDistanceCollider;
				ClearTooltip ();
			}

			bool hitNothing = (!hitInteractable && !hitCollider);

			if (hitInteractable) {

				SetTooltip (text);
				LerpCrosshair (minDistance - OFFSET_DISTANCE);
				SetLastValidCollisionTime ();
				if (!IsStateLocked ())
					SetState (CrosshairState.Closed);
			} else if (hitCollider) {
				LerpCrosshair (minDistance - OFFSET_DISTANCE);
				SetLastValidCollisionTime ();
				if (!IsStateLocked ())
					SetState (CrosshairState.Open);
			} else {
				LerpCrosshair (currentCrosshairDistance);
				if (!IsStateLocked ())
					SetState (CrosshairState.Open);
			}
		}

		protected void UpdateFadePercent () {
			float fadePercent = 1f - Mathf.Clamp01 ((Time.time - lastValidCollisionTime) / FADE_DURATION);
			if (fadePercent != 0 && fadePercent != 1f)
				fadePercent = this.fadeAnimationCurve.Evaluate (fadePercent);

			this.isVisible = (fadePercent > 0);

			this.crosshairSpriteRenderer.enabled = this.isVisible;
			this.crosshairSpriteRenderer.color = Color.white.Alpha (fadePercent);
			
		}

		//These states can't just be changed depending on gaze data
		protected bool IsStateLocked () {

			return (crosshairState == CrosshairState.Disabled);
		}

		////////////////////////////////////////
		//
		// Transform Functions
		
		protected void LerpCrosshair (float distance) {
			Vector3 targetPosition = GetTargetPosition (distance);
			Vector3 targetScale = GetTargetScale (distance);
			if (this.isVisible) {
				Vector3 targetPositionDirection = targetPosition - transform.position;
				this.transform.position += targetPositionDirection * 10f * Time.deltaTime;
				
				Vector3 targetScaleDirection = targetScale - this.crosshairTransform.localScale;
				this.crosshairTransform.localScale += targetScaleDirection * 10f * Time.deltaTime;
			} else {
				this.transform.position = targetPosition;
				this.crosshairTransform.localScale = targetScale;
			}
			
			this.transform.forward = GetForwardDirection ();
			
			this.currentCrosshairDistance = distance;
		}

		protected void UpdateConnector () {
			Vector3 startPosition = this.connectingStartPosition;
			Vector3 endPosition = this.transform.position;

			this.connectorTransform.position = Vector3.Lerp (startPosition, endPosition, .5f);

			float localScaleX = Vector3.Distance (startPosition, endPosition);

			Vector3 connectorLocalScale = this.connectorTransform.localScale;
			connectorLocalScale.x = localScaleX;
			this.connectorTransform.localScale = connectorLocalScale;

			this.connectorTransform.right = (endPosition - startPosition);

			this.connectorMaterial.SetFloat (tilingMultiplierXPropertyID, connectorLocalScale.x / connectorLocalScale.y);
		}

		protected void SetLastValidCollisionTime () {

			if (this.crosshairState == CrosshairState.Disabled)
				return;
			
			lastValidCollisionTime = Time.time;
		}

		////////////////////////////////////////
		//
		// Connecting Functions

		public void StartConnecting (Vector3 position) {
			SetState (CrosshairState.Connecting);
			this.connectingStartPosition = position - mainCameraTransform.forward * OFFSET_DISTANCE * .5f;
			this.isConnecting = true;
			//Center it
			this.connectorTransform.localScale = Vector3.one * this.connectorTransform.localScale.y;
			this.connectorTransform.localPosition = this.crosshairTransform.localPosition;
		}

		public void StopConnecting () {
			this.isConnecting = false;
			SetState (CrosshairState.Open);
		}

		////////////////////////////////////////
		//
		// State Functions

		public void SetState (CrosshairState state) {
			if (this.isConnecting)
				return;

			this.crosshairState = state;

			SetConnectorVisible (state == CrosshairState.Connecting);

			switch (this.crosshairState) {
			case CrosshairState.Open:

//				SetMaterial(this.staticMaterial);
//				SetTexture(this.openTexture);
				
				break;
			case CrosshairState.Closed:

//				SetMaterial(this.staticMaterial);
//				SetTexture(this.closedTexture);

				break;
			case CrosshairState.Connecting:

				//				SetMaterial(this.connectingMaterial);

				break;
			case CrosshairState.Disabled:



				break;

			}
		}

		////////////////////////////////////////
		//
		// Tooltip Functions
		
		protected void SetTooltip (string text) {
			if (text == "") {
				ClearTooltip ();
				return;
			}
			this.canvas.enabled = true;
			canvasText.text = text;
		}

		protected void ClearTooltip () {
			canvasText.text = "";
			this.canvas.enabled = false;
		}

		////////////////////////////////////////
		//
		// Get Functions

		protected Vector3 GetTargetPosition (float distance) {
			return this.mainCameraTransform.position + this.mainCameraTransform.forward * distance;
		}

		protected Vector3 GetTargetScale (float distance) {
			return Vector3.one * (distance / SCALE_MULTIPLIER) * (GetMeshScalePercent () / 1.5f + .5f);
		}

		protected Vector3 GetForwardDirection () {
			return this.mainCameraTransform.forward;
		}

		protected float GetMeshScalePercent () {
			switch (this.crosshairState) {
			case CrosshairState.Open:

				return 0;

				break;
			case CrosshairState.Closed:

				return 1f;

				break;

			}

			return 0;
		}

		////////////////////////////////////////
		//
		// Set Functions
		
		public void SetVisible (bool value) {
			this.crosshairSpriteRenderer.enabled = value;
		}

		protected void SetTexture (Sprite sprite) {
			this.crosshairSpriteRenderer.sprite = sprite;
		}

		protected void SetMaterial (Material material) {
//			this.crosshairSpriteRenderer.sharedMaterial = material;
		}

		protected void SetConnectorVisible (bool value) {
			this.connectorTransform.GetComponent<MeshRenderer> ().enabled = value;
		}
	}
}