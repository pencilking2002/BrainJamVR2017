using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace YouVisit
{
	[ExecuteInEditMode]
	public class Utility_MatchMaterialColor : MonoBehaviour
	{
		//readonly

		//Serialized
		[SerializeField]
		protected string propertyName = "_Color";
		[SerializeField]
		protected Material material;
		[SerializeField]
		protected Color color = Color.white;
		
		/////Protected/////
		//References
		//Primitives
		

		///////////////////////////////////////////////////////////////////////////
		//
		// Inherited from MonoBehaviour
		//

		protected void OnEnable()
		{
			Debug.Log (this.material==null);
			if(this.material==null)
			{
				Renderer renderer = GetComponentInChildren<Renderer>();
				if(renderer!=null)
					this.material = renderer.sharedMaterial;

				if(this.material!=null)
					this.color = this.material.GetColor(propertyName);
			}
		}
		
		protected void Awake()
		{

		}
		
		protected void Start ()
		{
			
		}
		
		protected void Update ()
		{
			
			if(this.material!=null && !UnityEngine.Application.isPlaying)
			{
				this.material.SetColor(this.propertyName,this.color);
			}
			
		}
		
		///////////////////////////////////////////////////////////////////////////
		//
		// Utility_MatchMaterialColor Functions
		//

		
		////////////////////////////////////////
		//
		// Function Functions

	}
}
