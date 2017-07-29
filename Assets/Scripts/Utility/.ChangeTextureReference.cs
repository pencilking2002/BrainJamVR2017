using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Neuromancers {
	public class ChangeTextureReference : MonoBehaviour {
		//readonly

	//Serialized
		[SerializeField]
		protected Material[] materials;
		[SerializeField]
		protected Texture2D[] textures;

		
		/////Protected/////
		//References
		//Primitives
		

		///////////////////////////////////////////////////////////////////////////
		//
		// Inherited from MonoBehaviour
		//
		
		protected void Awake () {

		}

		protected void Start () {
			
		}

		protected void Update () {
			
		}

		///////////////////////////////////////////////////////////////////////////
		//
		// ChangeTextureReference Functions
		//

		[EditorButtonAttribute]
		protected void ChangeTextureReferences () {

			string path = "Assets/Vendor/Foodpack/Materials";

			for (int i = 0; i < materials.Length; ++i) {

				Material mat = materials [i];
				Texture2D tex = (Texture2D)mat.mainTexture;
				Texture2D metallicTex = (Texture2D) mat.GetTexture("_MetallicGlossMap");
				Texture2D normalTex =  (Texture2D) mat.GetTexture("_BumpMap");

				for (int j = 0; j < textures.Length; ++j) {
	
					//					Debug.Log(tex.name);
					if(tex.name == textures[j].name) {

						mat.mainTexture = textures[j];
					}
					if(metallicTex.name == textures[j].name) {

						mat.SetTexture("_MetallicGlossMap",textures[j]);
					}
					if(normalTex.name == textures[j].name) {

						mat.SetTexture("_BumpMap",textures[j]);
					}

				}
			}
		}

		
		////////////////////////////////////////
		//
		// Function Functions

	}
}
