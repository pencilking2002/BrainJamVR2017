using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace VRB.Utility
{
	public class PathSweepUtility : MonoBehaviour
	{
		//readonly/const

		//Serialized
		
		/////Protected/////
		//References
		//Primitives
		public int radialSegments = 8;
		private float radialSegmentsFloat;
		public float radius = .2f;
		public bool shouldLoop = false;
		
		//Actions
		public int smoothMultiplier = 3;
		public static int pathLength;

		Vector3[] vertices;
		int[] triangles;
		Vector2[] uvs;
		int pathLengthAdjusted;

		public Vector3[] smoothedPath { get; set; }


		///////////////////////////////////////////////////////////////////////////
		//
		// Inherited from MonoBehaviour
		//
		
		protected void Awake () {

			radialSegmentsFloat = (float)radialSegments;
			SetPath ();
		}

		protected void Start () {
			CreateMesh ();
			
		}

		protected void Update () {
			
		}

		///////////////////////////////////////////////////////////////////////////
		//
		// PathSweepUtility Functions
		//

		void SetPath () {
			Vector3[] vector3s = Utility.PathControlPointGenerator (GetComponentInChildren<PathUtility> ().nodes.ToArray ());
			int SmoothAmount = vector3s.Length * smoothMultiplier;
			pathLength = SmoothAmount;
			smoothedPath = new Vector3[SmoothAmount];
			pathLengthAdjusted = SmoothAmount;
			for (int i = 0; i < SmoothAmount; i++) {
				float pm = (float)i / SmoothAmount;
				Vector3 currPt = Utility.Interp (vector3s, pm);
				smoothedPath [i] = currPt;
			}
		}

		void CreateMesh () {
			vertices = new Vector3[(smoothedPath.Length) * radialSegments];
			int polygonCount = (smoothedPath.Length) * radialSegments;
			int triangleCount = polygonCount * 2;
			triangles = new int[triangleCount * 3];
			//triangles = new int[1344];
			int triangleIndex = 0;
			uvs = new Vector2[vertices.Length];
			int vertexIndex = 0;
			for (int i = 0; i < pathLengthAdjusted; ++i) {
				Vector3 currentCenter = smoothedPath [i];
				float myIndexPercent = ((float)i) / ((float)pathLength);
				Vector3 direction = (smoothedPath [(i + 1) % smoothedPath.Length] - smoothedPath [i]).normalized;
				for (int j = 0; j < radialSegments; ++j) {
					float percent = ((float)j) / radialSegmentsFloat;
					float angle = percent * 6.28318531f;
					Vector3 localPosition = new Vector3 (radius * Mathf.Cos (angle), radius * Mathf.Sin (angle), 0);
					float theta = SignedAngle (Vector3.forward, direction);
					Vector3 position = Utility.RotatePointAroundPivot (localPosition, Vector3.zero, new Vector3 (0, theta, 0));
					position = Utility.RotatePointAroundPivot (position, Vector3.zero, new Vector3 (0, 0,  Vector3.Angle (-Vector3.right, direction)));
					Vector3 vertexPosition = currentCenter + position - transform.localPosition;
					if (i < smoothedPath.Length - 1) {
						if (j == radialSegments - 1) {
							triangles [triangleIndex] = vertexIndex;
							triangles [triangleIndex + 1] = (vertexIndex - 7);
							triangles [triangleIndex + 2] = vertexIndex + radialSegments;
							triangles [triangleIndex + 3] = vertexIndex + radialSegments;
							triangles [triangleIndex + 4] = (vertexIndex - 7);
							triangles [triangleIndex + 5] = (vertexIndex + radialSegments - 7);
						} else {
							triangles [triangleIndex] = vertexIndex;
							triangles [triangleIndex + 1] = (vertexIndex + 1);
							triangles [triangleIndex + 2] = vertexIndex + radialSegments;
							triangles [triangleIndex + 3] = vertexIndex + radialSegments;
							triangles [triangleIndex + 4] = (vertexIndex + 1);
							triangles [triangleIndex + 5] = (vertexIndex + radialSegments + 1);
						}
						triangleIndex += 6;
					}

					Vector2 newUV = new Vector2 ();
					if (i % 2 == 0) {
						newUV.x = percent;
						newUV.y = 0;
					} else {
						newUV.x = percent;
						newUV.y = 1;
					}
					uvs [vertexIndex] = newUV;

					vertices [vertexIndex] = vertexPosition;
					vertexIndex++;
				}
			}
			Mesh newMesh = new Mesh ();
			newMesh.vertices = vertices;
			newMesh.uv = uvs;
			newMesh.triangles = triangles;
			newMesh.RecalculateNormals ();
			GetComponent<MeshFilter> ().mesh = newMesh;

			#if UNITY_EDITOR
			UnityEditor.AssetDatabase.CreateAsset(newMesh as UnityEngine.Object,"Assets/Meshes/"+gameObject.name+".asset");
			UnityEditor.AssetDatabase.SaveAssets();
			#endif
		}

		public static float SignedAngle (Vector3 vector1, Vector3 vector2) {
			float angle = Vector3.Angle (vector1, vector2); // calculate angle
			// assume the sign of the cross product's Y component:
			return angle * Mathf.Sign (Vector3.Cross (vector1, vector2).y);
		}

		public Vector3 GetPoint (float percent) {
			int index = (int)(percent * ((float)smoothedPath.Length));
			return smoothedPath [index];
		}

		////////////////////////////////////////
		//
		// Function Functions

	}
}
