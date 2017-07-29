using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace VRB
{
	public class ObjectGeneratorUtility : MonoBehaviour
	{
		//readonly/const

		//Serialized
		[SerializeField]
		protected GameObject objectPrefab;
		[SerializeField]
		protected int objectCount = 10;
		[SerializeField]
		protected Vector3 spawnDimensions = new Vector3 (10f, 10f, 10f);

		/////Protected/////
		//References
		//Primitives
		
		//Actions

		///////////////////////////////////////////////////////////////////////////
		//
		// Inherited from MonoBehaviour
		//
		
		protected void Awake () {

		}

		protected void Start () {
			GenerateObjects ();
		}

		protected void Update () {
			
		}

		///////////////////////////////////////////////////////////////////////////
		//
		// ObjectGenerator Functions
		//

		protected void GenerateObjects () {
			if (objectPrefab == null)
			{
				Debug.Log ("Object prefab is null");
				return;
			}
			if (objectCount <= 0)
			{
				Debug.Log ("Object count is less than or equal to zero");
				return;
			}
			for (int i = 0; i < objectCount; i++)
			{
				GameObject generatedObject = Instantiate (objectPrefab, Vector3.zero, Quaternion.identity) as GameObject;

				generatedObject.transform.SetParent (transform);

				Vector3 localPosition = new Vector3 ();
				localPosition.x = UnityEngine.Random.Range (-spawnDimensions.x, spawnDimensions.x);
				localPosition.y = UnityEngine.Random.Range (-spawnDimensions.y, spawnDimensions.y);
				localPosition.z = UnityEngine.Random.Range (-spawnDimensions.z, spawnDimensions.z);

				generatedObject.transform.localPosition = localPosition;
			}
		}
		
		////////////////////////////////////////
		//
		// Function Functions

	}
}