using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;


namespace UIPrimitives
{
	public class UISlider : UIElement, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
	{
		//readonly

		//Serialized
		[SerializeField]
		protected float startValue=.5f;
		[SerializeField]
		protected float width = 600f;
		[SerializeField]
		protected Transform hoverTransform;
		[SerializeField]
		protected Transform currentTransform;
		[SerializeField]
		protected Text percentText;
		
		/////Protected/////
		//References
		protected PointerEventData activePointerEventData;
		//Primitives
		protected bool isFocused;
		protected float currentHoverPercent;
		protected float goalHoverPercent;
		protected float actualPercent;
		
		//Actions
		public Action<float> ValueChangedAction;

		public float ActualPercent {

			get {
				return this.actualPercent;
			}
		}

		///////////////////////////////////////////////////////////////////////////
		//
		// Inherited from MonoBehaviour
		//
		
		protected void Awake()
		{
			this.actualPercent = startValue;
		}
		
		protected void Start ()
		{
			UpdateText();
			SetHoverVisible(false);	
		}
		
		protected void Update ()
		{
			if(isFocused)
				OnHovered();


			if(isFocused) {
				currentHoverPercent = Mathf.Lerp(currentHoverPercent,goalHoverPercent,5f*Time.deltaTime);
				UpdateHoverPosition();
			}
		}
		
		///////////////////////////////////////////////////////////////////////////
		//
		// UISlider Functions
		//

		protected void UpdateHoverPosition() {

			Vector3 localPosition = hoverTransform.localPosition;
			localPosition.x = width*(currentHoverPercent-.5f);
			hoverTransform.localPosition =localPosition;
		}

		protected void UpdateCurrentPosition() {

			Vector3 localPosition = currentTransform.localPosition;
			localPosition.x = width*(actualPercent-.5f);
			currentTransform.localPosition =localPosition;

		}

		protected void UpdateText() {

			if(percentText!=null)
				percentText.text = ((int) (this.actualPercent*100f))+"%";
		}

		public void SetPercent(float percent) {

			this.currentHoverPercent = percent;
			this.goalHoverPercent = percent;
			this.actualPercent = percent;
			UpdateText();
			UpdateCurrentPosition();
		}

		protected void OnClicked(PointerEventData eventData) {

			if(activePointerEventData==null)
				return;

			Vector3 position = activePointerEventData.pointerCurrentRaycast.worldPosition;

			this.actualPercent = WorldPositionToPercent(position);
			UpdateText();

			UpdateCurrentPosition();

			if(ValueChangedAction!=null)
				ValueChangedAction(this.actualPercent);

            if (this.selectedAction != null)
                this.selectedAction(this);

            if (this.selectedActionSimple != null)
                this.selectedActionSimple();
        }

		protected void OnHovered() {
			
			if(activePointerEventData==null)
				return;

			Vector3 position = activePointerEventData.pointerCurrentRaycast.worldPosition;

			goalHoverPercent = WorldPositionToPercent(position);
		}

		protected float WorldPositionToPercent(Vector3 position) {

			return Mathf.Clamp01(hoverTransform.parent.InverseTransformPoint(position).x/width + .5f);
		}

		////////////////////////////////////////
		//
		// Function Functions

		protected void SetHoverVisible(bool value) {

			hoverTransform.gameObject.SetActive(value);
		}

		///////////////////////////////////////////////////////////////////////////
		//
		// Implementation for IPointer
		//

		public void OnPointerEnter(PointerEventData eventData)
		{
            Hovered(true);
			isFocused=true;
			activePointerEventData = eventData;

			SetHoverVisible(true);
			OnHovered();
			currentHoverPercent = goalHoverPercent;

		}

		public void OnPointerExit(PointerEventData eventData)
		{
            Hovered(false);
			isFocused=false;
			activePointerEventData = null;
			SetHoverVisible(false);
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			OnClicked(eventData);

        }

        //event

        protected void Hovered(bool value)
        {
            if (this.hoveredAction != null)
            {
                this.hoveredAction(value);
            }
        }

        //ACtion

        protected Action<bool> hoveredAction;
        protected Action<UISlider> selectedAction;
        protected Action selectedActionSimple;
        ////////////////////////////////////////
        //
        // Properties

        public Action<bool> HoveredAction
        {
            get
            {
                return this.hoveredAction;
            }
            set
            {
                this.hoveredAction = value;
            }
        }

        public Action<UISlider> SelectedAction
        {
            get
            {
                return this.selectedAction;
            }
            set
            {
                this.selectedAction = value;
            }
        }

        public Action SelectedActionSimple
        {
            get
            {
                return this.selectedActionSimple;
            }
            set
            {
                this.selectedActionSimple = value;
            }
        }
    }
}