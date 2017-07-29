using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UIPrimitives;

namespace UIPrimitives
{
	public class UIElement : Element//, IPointerEnterHandler, IPointerExitHandler
	{

		public string tooltipName;

		////////////////////////////////////////
		//
		// Properties
		
		public override string TooltipName
		{
			get {
				return this.tooltipName;
			}
			set {
				this.tooltipName = value;
			}
		}
	}
}