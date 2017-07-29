using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace UIPrimitives
{
	public abstract class Button : UIElement
	{
		public enum ButtonState
		{
			Normal,
			Hovered,
			Selected
		}

		protected Action<bool> hoveredAction;
		protected Action<Button> selectedAction;
		protected Action selectedActionSimple;
		protected Collider collider;

		public abstract bool IsDisabled {get;set;}
		
		////////////////////////////////////////
		//
		// Properties
		
		public Action<bool> HoveredAction
		{
			get {
				return this.hoveredAction;
			}
			set {
				this.hoveredAction=value;
			}
		}
		
		public Action<Button> SelectedAction
		{
			get {
				return this.selectedAction;
			}
			set {
				this.selectedAction=value;
			}
		}
		
		public Action SelectedActionSimple
		{
			get {
				return this.selectedActionSimple;
			}
			set {
				this.selectedActionSimple=value;
			}
		}
		
//		public Collider Collider
//		{
//			get {
//				return this.collider;
//			}
//		}

		////////////////////////////////////////
		//
		// Button Functions

		public abstract void SetButtonState(ButtonState state);
		public abstract void ResetState();

		public abstract bool IsToggled();

		public virtual Vector3 GetPivotPosition()
		{
			if(this.collider==null)
				return this.transform.position;

			return this.collider.transform.position;
		}
	}
}