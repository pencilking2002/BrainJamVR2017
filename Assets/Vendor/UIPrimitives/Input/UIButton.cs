using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UIPrimitives;

namespace UIPrimitives
{
	public class UIButton : Neuromancers.Button, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
	{
		//enum
		public enum FillType
		{
			UIRect,
			UICircle
		}
		public enum ToggleState
		{
			Enabled,
			Disabled
		}

		/////Serialized/////
		/// 
		//Timing
		public float fillSpeed = 1f;
		public float cooldownTime=1f;
		//Fill
		public FillType fillType;
		public UIRect fillUIRect;
		public UICircle fillUICircle;
		//Info
		public bool isToggle = false;
		public bool shouldResetOnStart = true;
//		public string tooltipName;
		//Graphic
		public UnityEngine.UI.Graphic graphic;
		public Color normalColor = new Color(0,0,0,51f/255f);
		public Color disabledColor = new Color(0,0,0,20f/255f);
		public Color hoverColor  = new Color(27f/255f,53f/255f,56f/255f,87f/255f);
		public Color selectedColor = new Color(41/255f,86/255f,56f/255f,122f/255f);
		//Animation
		public UIRootAnimator hoverRootAnimator;
		public UIRootAnimator selectRootAnimator;
		//Transform
//		public Transform pivotTransform;

		/////Protected/////
		//References
		//Primitives
		protected ButtonState buttonState;
		protected ToggleState toggleState  = ToggleState.Enabled;
		protected bool isDisabled;
		protected bool isFocused;
		protected bool expandButtonsOnHover;
		protected float capsuleColliderStartRadius;
		protected float fillPercent;
		protected float maxCircleAngle;
		
		////////////////////////////////////////
		//
		// Properties
		
//		public override string TooltipName
//		{
//			get {
//				return this.tooltipName;
//			}
//			set {
//				this.tooltipName = value;
//			}
//		}

		public override bool IsDisabled
		{
			get {
				return this.isDisabled;
			}
			set {
				this.isDisabled=value;
			}
		}
		///////////////////////////////////////////////////////////////////////////
		//
		// Inherited from MonoBehaviour
		//
		
		protected void Awake()
		{
//			if(this.pivotTransform == null)
//				this.pivotTransform = this.transform;
			this.collider = GetComponentInChildren<Collider>();

			this.fillSpeed = 1f/this.fillSpeed;

			if(this.fillType == FillType.UICircle)
			{
				if(this.fillUICircle!=null)
					this.maxCircleAngle = this.fillUICircle.angle;

			}
		}
		
		protected void OnEnable ()
		{
			if(shouldResetOnStart)
				StartCoroutine(this.Reset ());

			if(this.gameObject.name.StartsWith("Menu_Button"))
			{
				expandButtonsOnHover = true;
				capsuleColliderStartRadius = GetComponentInChildren<CapsuleCollider>().radius;
			}
		}
		
		protected void Update ()
		{
			if(this.isFocused && !this.isDisabled && this.buttonState != ButtonState.Selected)
			{
				if(this.fillPercent<1f )
				{
					Increase();
				}
			}else
			{
				if(this.fillPercent>0)
				{
					Decrease();
				}
			}
			
		}
		
		///////////////////////////////////////////////////////////////////////////
		//
		// UIButton Functions
		//

		public override void ResetState()
		{
			StartCoroutine(this.Reset());
		}

		protected void Increase()
		{
			this.fillPercent+=Time.deltaTime*fillSpeed;
			PercentChanged();
		}

		protected void Decrease()
		{
			this.fillPercent-=Time.deltaTime*fillSpeed;
			PercentChanged();
		}

		public IEnumerator Reset(float delay = 0)
		{
//			this.OnPointerExit(null);
			this.fillPercent = 0;
			PercentChanged();

			//if the delay is negative it means it doesn't want us to go back to normal state
			if(Mathf.Sign(delay)==-1)
				yield break;

			yield return new WaitForSeconds(delay);

			SetButtonState(ButtonState.Hovered);
		}

		////////////////////////////////////////
		//
		// Event Functions
		
		protected void Hovered(bool value)
		{
			if(this.buttonState == ButtonState.Selected)
				return;
			if(this.isDisabled)
				return;
			
			if(value)
				SetButtonState(ButtonState.Hovered);
			else
				SetButtonState(ButtonState.Normal);
			
			if(this.hoveredAction!=null)
			{
				this.hoveredAction(value);
			}
		}
		
		protected void Selected()
		{
			SetButtonState(ButtonState.Selected);

			//Flip the toggle state
			if(isToggle)
				SetToggleState((ToggleState) (1-((int) this.toggleState)));
			
			StartCoroutine(this.Reset(cooldownTime));
			
			if(this.selectedAction!=null)
				this.selectedAction(this);
			
			if(this.selectedActionSimple!=null)
				this.selectedActionSimple();
		}

		protected void PercentChanged()
		{
			SetFillPercent(Mathf.Clamp01(this.fillPercent));

			if(this.fillPercent>=1f)
			{
				Selected();
			}
		}
		
		////////////////////////////////////////
		//
		// State Functions
		
		public override bool IsToggled()
		{
			return this.toggleState == ToggleState.Enabled;
		}

		public void RefreshButtonState() {

			SetButtonState(this.buttonState);
		}

		public override void SetButtonState(ButtonState state)
		{
			this.buttonState = state;

			switch(this.buttonState)
			{
			case ButtonState.Normal:

				if(this.isToggle)
					SetToggleState(this.toggleState);
				else
				{
					if(this.isDisabled)
						SetColor(this.disabledColor);
					else
						SetColor(this.normalColor);
				}

				break;
			case ButtonState.Hovered:
				
				SetColor(this.hoverColor);
				StartRootAnimator(this.hoverRootAnimator);

				break;
			case ButtonState.Selected:
				
				SetColor(this.selectedColor);
				StartRootAnimator(this.selectRootAnimator);

				break;


			}
		}

		public void SetToggleState(ToggleState state)
		{
			this.toggleState = state;
			
			switch(this.toggleState)
			{
			case ToggleState.Enabled:
				
				SetColor(this.normalColor);
				
				break;
			case ToggleState.Disabled:

				SetColor(this.disabledColor);

				break;
			}
		}


		////////////////////////////////////////
		//
		// Appearance Functions
		
		protected void SetFillPercent(float t)
		{
			switch(fillType)
			{
			case FillType.UIRect:
				
				if(fillUIRect != null)
				{
					fillUIRect.fillPercentX = t;
					fillUIRect.SetAllDirty();
				}
				
				break;
			case FillType.UICircle:
				
				if(fillUICircle != null)
				{
					fillUICircle.angle = (int) (t*maxCircleAngle);
					fillUICircle.SetAllDirty();
				}
				
				break;
			default:
				
				Debug.Log ("No fill type found");
				break;
			}
		}

		protected void SetColor(Color color)
		{
			if(this.graphic!=null)
			{
				this.graphic.color = color;
			}
		}

		protected void StartRootAnimator(UIRootAnimator rootAnimator)
		{
			if(rootAnimator!=null)
				rootAnimator.Animate();
		}
		
		///////////////////////////////////////////////////////////////////////////
		//
		// Implementation for IPointer
		//
		
		public void OnPointerEnter(PointerEventData eventData)
		{
//			Debug.Log("OnPointerEnter");
			Hovered(true);

			if(expandButtonsOnHover)
			{
				GetComponentInChildren<CapsuleCollider>().radius = capsuleColliderStartRadius*2f;
			}
		}
		
		public void OnPointerExit(PointerEventData eventData)
		{
//			Debug.Log("OnPointerExit");
			Hovered(false);

			if(expandButtonsOnHover)
			{
				GetComponentInChildren<CapsuleCollider>().radius = capsuleColliderStartRadius;
			}
		}

		public void OnPointerClick(PointerEventData eventData) {
			
//			Debug.Log("OnPointerClick");
			if (buttonState == ButtonState.Hovered) {
				this.fillPercent = 1f;
				PercentChanged ();
			}
		}
	}
}
