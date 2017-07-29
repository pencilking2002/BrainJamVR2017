using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ITHB.Utility
{
	public class Utility_Cloner : MonoBehaviour
	{
		//readonly
		
		//Serialized
		[SerializeField]
		protected string gameObjectTag;
		[SerializeField]
		protected GameObject prefab;
		[SerializeField]
		protected string cloneTag;
		
		/////Protected/////
		//References
		//Primitives
		
		
		///////////////////////////////////////////////////////////////////////////
		//
		// Inherited from MonoBehaviour
		//
		
		protected void Awake()
		{
		}
		
		protected void Start ()
		{
			
		}
		
		protected void Update ()
		{
			
		}
		
		///////////////////////////////////////////////////////////////////////////
		//
		// Utility_Cloner Functions
		//
		
		public void CreateObjects()
		{
			CreateObjects(GameObject.FindGameObjectsWithTag(this.gameObjectTag));
		}
		
		protected void CreateObjects(GameObject[] gameObjects)
		{
			if(!gameObjects.IsValid())
			{
				Debug.Log ("Utility_Cloner could not find any GameObjects with tag "+gameObjectTag);
				return;
			}
			
			for (int i = 0; i < gameObjects.Length; ++i)
			{
#if UNITY_EDITOR
				GameObject newGameObject = UnityEditor.PrefabUtility.InstantiatePrefab(prefab) as GameObject;

				newGameObject.name = gameObjects[i].name +"_CloneInstance_"+i;
				if(cloneTag.IsValid())
					newGameObject.tag = cloneTag;

				newGameObject.transform.parent = gameObjects[i].transform;
				newGameObject.transform.Recenter();
#endif
			}
		}

		public void DeleteObjects()
		{
			DeleteObjects(GameObject.FindGameObjectsWithTag(this.cloneTag));
		}

		protected void DeleteObjects(GameObject[] gameObjects)
		{
			if(!gameObjects.IsValid())
			{
				Debug.Log ("Utility_Cloner could not find any GameObjects with tag "+cloneTag);
				return;
			}
			
			for (int i = 0; i < gameObjects.Length; ++i)
			{
				DestroyImmediate(gameObjects[i]);
			}
		}
		
		////////////////////////////////////////
		//
		// Function Functions
		
	}
}