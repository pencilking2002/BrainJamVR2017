using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace VRB.Utility
{
	public class TextureOffsetUtility : MonoBehaviour
	{
		//readonly/const

		//Serialized
		[SerializeField]
		protected Vector2 offset;
		[SerializeField]
		protected float speed = 1f;
		[SerializeField]
		protected string propertyName = "_MainTex";
		[SerializeField]
		protected bool useSharedMaterial;
		
		/////Protected/////
		//References
		protected Material material;
		//Primitives
		protected Vector2 currentOffset;
		protected Vector2 startOffset;
		
		//Actions

		///////////////////////////////////////////////////////////////////////////
		//
		// Inherited from MonoBehaviour
		//
		
		protected void Awake()	{
			if(useSharedMaterial)
				material = GetComponentInChildren<Renderer> ().sharedMaterial;
			else
				material = GetComponentInChildren<Renderer> ().material;
			currentOffset = material.GetTextureOffset (propertyName);

			startOffset = currentOffset;
		}
		
		protected void Start () {
			
		}
		
		protected void Update () {

			currentOffset += Time.deltaTime * speed * offset;
			material.SetTextureOffset (propertyName, currentOffset);
		}

		protected void OnApplicationQuit() {

			if(useSharedMaterial)
				material.SetTextureOffset(propertyName, startOffset);
		}
		
		///////////////////////////////////////////////////////////////////////////
		//
		// TextureOffsetUtility Functions
		//

		
		////////////////////////////////////////
		//
		// Function Functions

	}
}
