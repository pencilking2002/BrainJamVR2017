using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Neuromancers.Utility
{
	public class Utility : MonoBehaviour
	{
		public static readonly KeyCode[] ALPHA_KEY_CODES = new KeyCode[]
		{
			KeyCode.Alpha1,
			KeyCode.Alpha2,
			KeyCode.Alpha3,
			KeyCode.Alpha4,
			KeyCode.Alpha5,
			KeyCode.Alpha6,
			KeyCode.Alpha7,
			KeyCode.Alpha8,
			KeyCode.Alpha9,
			KeyCode.Alpha0
		};
		
		public static GameObject FindGameObjectContainingName(string objectName)
		{
			GameObject[] gos = (GameObject[])FindObjectsOfType(typeof(GameObject));

			int shallowestDepth = int.MaxValue;
			GameObject shallowestGameObject = null;

			for(int i=0;i<gos.Length;i++) {
				if(gos[i].name.Contains(objectName)) {

					return gos[i];
				}
			}

			return null;
		}

		public static GameObject FindShallowestGameObjectContainingName(string objectName)
		{
			GameObject[] gos = (GameObject[])FindObjectsOfType(typeof(GameObject));

			int shallowestDepth = int.MaxValue;
			GameObject shallowestGameObject = null;

			for(int i=0;i<gos.Length;i++) {
				if(gos[i].name.Contains(objectName)) {

					GameObject go = gos[i];
					int depth = go.transform.GetDepth();

					if(depth==0)
						return go;
					
					if(depth<shallowestDepth) {

						shallowestDepth = depth;
						shallowestGameObject = go;
					}
				}
			}
			return shallowestGameObject;
		}
		public static Vector2 GetRandomVector2 (float range)
		{
			return new Vector2 (Random.Range (-range, range), Random.Range (-range, range));
		}

		public static Vector3 GetRandomVector3 (float range)
		{
			return new Vector3 (Random.Range (-range, range), Random.Range (-range, range), Random.Range (-range, range));
		}

		public static Vector3 GetMeanVector(Vector3[] positions)
		{
			if (positions.Length == 0)
				return Vector3.zero;
			float x = 0f;
			float y = 0f;
			float z = 0f;
			foreach (Vector3 pos in positions)
			{
				x += pos.x;
				y += pos.y;
				z += pos.z;
			}
			return new Vector3(x / positions.Length, y / positions.Length, z / positions.Length);
		}

		public static Vector3 Interp (Vector3[] pts, float t)
		{
			int numSections = pts.Length - 3;
			int currPt = Mathf.Min (Mathf.FloorToInt (t * (float)numSections), numSections - 1);
			float u = t * (float)numSections - (float)currPt;
		
			Vector3 a = pts [currPt];
			Vector3 b = pts [currPt + 1];
			Vector3 c = pts [currPt + 2];
			Vector3 d = pts [currPt + 3];
		
			return .5f * (
			(-a + 3f * b - 3f * c + d) * (u * u * u)
				+ (2f * a - 5f * b + 4f * c - d) * (u * u)
				+ (-a + c) * u
				+ 2f * b
			);
		}

		public static Vector3[] PathControlPointGenerator (Vector3[] path)
		{
			Vector3[] suppliedPath;
			Vector3[] vector3s;
		
			//create and store path points:
			suppliedPath = path;
		
			//populate calculate path;
			int offset = 2;
			vector3s = new Vector3[suppliedPath.Length + offset];
			System.Array.Copy (suppliedPath, 0, vector3s, 1, suppliedPath.Length);
		
			//populate start and end control points:
			//vector3s[0] = vector3s[1] - vector3s[2];
			vector3s [0] = vector3s [1] + (vector3s [1] - vector3s [2]);
			vector3s [vector3s.Length - 1] = vector3s [vector3s.Length - 2] + (vector3s [vector3s.Length - 2] - vector3s [vector3s.Length - 3]);
		
			//is this a closed, continuous loop? yes? well then so let's make a continuous Catmull-Rom spline!
			if (vector3s [1] == vector3s [vector3s.Length - 2]) {
				Vector3[] tmpLoopSpline = new Vector3[vector3s.Length];
				System.Array.Copy (vector3s, tmpLoopSpline, vector3s.Length);
				tmpLoopSpline [0] = tmpLoopSpline [tmpLoopSpline.Length - 3];
				tmpLoopSpline [tmpLoopSpline.Length - 1] = tmpLoopSpline [2];
				vector3s = new Vector3[tmpLoopSpline.Length];
				System.Array.Copy (tmpLoopSpline, vector3s, tmpLoopSpline.Length);
			}	
		
			return(vector3s);
		}

		public static Vector3 RotatePointAroundPivot (Vector3 point, Vector3 pivot, Vector3 angles)
		{
			Vector3 dir = point - pivot; // get point direction relative to pivot
			dir = Quaternion.Euler (angles) * dir; // rotate it
			point = dir + pivot; // calculate rotated point
			return point; // return it
		}

		public static AnimationCurve GetCurve_Linear (float duration)
		{
			return AnimationCurve.Linear (Time.time, 0, Time.time + duration, 1f);
		}

		public static AnimationCurve GetCurve_Ease (float duration)
		{
			return AnimationCurve.EaseInOut (Time.time, 0, Time.time + duration, 1f);
		}
	
		public static string Truncate (string source, int length)
		{
			if (source == null)
				return null;
		
			if (source.Length > length) {
				int index = source.Substring (0, length).LastIndexOf (' ');
				if (index == -1) {
					index = length - 1;
				}
				source = source.Insert (index, "\n");
			
				if (source.Substring (index).Length > length) {
					source = source.Substring (0, index + length) + "...";
				}
			}
			return source;
		}
	
		//Utility function: print Dictionary of List of Dictionary
		public static void printDictList (Dictionary<string,  List<Dictionary<string, string>>> d)
		{
			foreach (KeyValuePair<string, List<Dictionary<string, string>>> kvp in d) {
				Debug.Log (kvp.Key + "------------------------------");
				printList (kvp.Value);
			}
		}
	
		//Utility function: print List of Dictionary
		public static void printList (List<Dictionary<string, string>> list)
		{
			foreach (Dictionary<string, string> i in list) {
				foreach (string k in i.Keys) {
					Debug.Log (k + " " + i [k]);
				}
			}
		}

		/// <summary>
		/// Converts HSV color to RGB color.
		/// </summary>
		/// <returns>The to RG.</returns>
		/// <param name="H">H.</param>
		/// <param name="S">S.</param>
		/// <param name="V">V.</param>
		public static Color HSVToRGB(float H, float S, float V)
		{
			if (S == 0f)
				return new Color(V,V,V);
			else if (V == 0f)
				return Color.black;
			else
			{
				Color col = Color.black;
				float Hval = H * 6f;
				int sel = Mathf.FloorToInt(Hval);
				float mod = Hval - sel;
				float v1 = V * (1f - S);
				float v2 = V * (1f - S * mod);
				float v3 = V * (1f - S * (1f - mod));
				switch (sel + 1)
				{
				case 0:
					col.r = V;
					col.g = v1;
					col.b = v2;
					break;
				case 1:
					col.r = V;
					col.g = v3;
					col.b = v1;
					break;
				case 2:
					col.r = v2;
					col.g = V;
					col.b = v1;
					break;
				case 3:
					col.r = v1;
					col.g = V;
					col.b = v3;
					break;
				case 4:
					col.r = v1;
					col.g = v2;
					col.b = V;
					break;
				case 5:
					col.r = v3;
					col.g = v1;
					col.b = V;
					break;
				case 6:
					col.r = V;
					col.g = v1;
					col.b = v2;
					break;
				case 7:
					col.r = V;
					col.g = v3;
					col.b = v1;
					break;
				}
				col.r = Mathf.Clamp(col.r, 0f, 1f);
				col.g = Mathf.Clamp(col.g, 0f, 1f);
				col.b = Mathf.Clamp(col.b, 0f, 1f);
				return col;
			}
		}


	}
}
