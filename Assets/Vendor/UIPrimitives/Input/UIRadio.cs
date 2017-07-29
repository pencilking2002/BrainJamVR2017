using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Neuromancers;

namespace UIPrimitives
{
	public class UIRadio : MonoBehaviour
	{
		//readonly

		//Serialized
		[SerializeField]
		protected UIPrimitives.UIButton[] buttons;
		
		/////Protected/////
		//References
		//Primitives
		

		///////////////////////////////////////////////////////////////////////////
		//
		// Inherited from MonoBehaviour
		//
		
		protected void Awake()
		{
			for (int i = 0; i < buttons.Length; ++i) {
				buttons[i].SelectedAction+=OnButtonSelected;
			}
		}
		
		protected void Start ()
		{
			
		}
		
		protected void Update ()
		{
			
		}
		
		///////////////////////////////////////////////////////////////////////////
		//
		// UIRadio Functions
		//

		protected void OnButtonSelected(Button button)
		{
			for (int i = 0; i < buttons.Length; ++i)
			{
				if(buttons[i]!=button)
					buttons[i].ResetState();
			}
		}
		
		////////////////////////////////////////
		//
		// Function Functions

	}
}
