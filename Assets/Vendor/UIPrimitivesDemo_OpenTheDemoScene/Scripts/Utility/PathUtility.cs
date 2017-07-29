// Copyright (c) 2010 Bob Berkebile
// Please direct any bugs/comments/suggestions to http://www.pixelplacement.com
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using UnityEngine;
using System.Collections.Generic;

namespace VRB.Utility
{
	public class PathUtility : MonoBehaviour
	{
		public string pathName = "";
		public Color pathColor = Color.cyan;
		public List<Vector3> nodes = new List<Vector3> (){Vector3.zero, Vector3.zero};
		public int nodeCount;
		public static Dictionary<string, PathUtility> paths = new Dictionary<string, PathUtility> ();
		public bool initialized = false;
		public string initialName = "";
		public bool pathVisible = true;

		public Vector3[] Nodes {
			get {
				Vector3[] nodeArray = nodes.ToArray();
				for(int i=0;i<nodeArray.Length;++i){
					nodeArray[i]+=transform.position;
				}
				return nodeArray;
			}
		}

		
		void OnEnable ()
		{

			Debug.Log ("pathName: " + pathName );
			if (!paths.ContainsKey (pathName)) {
				paths.Add (pathName.ToLower (), this);
			}
		}
	
		void OnDisable ()
		{
			paths.Remove (pathName.ToLower ());
		}
	
		void OnDrawGizmosSelected ()
		{
			if (pathVisible) {
				if (nodes.Count > 0) {
					DrawPathHelper (Nodes, pathColor, "gizmos");
				}	
			}
		}

		public static PathUtility GetPath (string requestedName)
		{
			requestedName = requestedName.ToLower ();
			if (paths.ContainsKey (requestedName)) {
				return paths [requestedName];
			} else {
				Debug.Log ("No path with that name (" + requestedName + ") exists! Are you sure you wrote it correctly?");
				return null;
			}
		}
		
		/// <summary>
		/// Returns the visually edited path as a Vector3 array.
		/// </summary>
		/// <param name="requestedName">
		/// A <see cref="System.String"/> the requested name of a path.
		/// </param>
		/// <returns>
		/// A <see cref="Vector3[]"/>
		/// </returns>
		public static Vector3[] GetPathNodes (string requestedName)
		{
			//			foreach (string s in paths.Keys){
			//				Debug.Log (s);
			//			}
			requestedName = requestedName.ToLower ();
			if (paths.ContainsKey (requestedName)) {
				return paths [requestedName].Nodes;
			} else {
				Debug.Log ("No path with that name (" + requestedName + ") exists! Are you sure you wrote it correctly?");
				return null;
			}
		}
		
		/// <summary>
		/// Returns the visually edited path as a Vector3 array.
		/// </summary>
		/// <param name="requestedName">
		/// A <see cref="System.String"/> the requested name of a path.
		/// </param>
		/// <returns>
		/// A <see cref="Vector3[]"/>
		/// </returns>
		public static Vector3[] GetPathNodesLocal (string requestedName)
		{
			//			foreach (string s in paths.Keys){
			//				Debug.Log (s);
			//			}
			requestedName = requestedName.ToLower ();
			if (paths.ContainsKey (requestedName)) {
				return paths [requestedName].nodes.ToArray();
			} else {
				Debug.Log ("No path with that name (" + requestedName + ") exists! Are you sure you wrote it correctly?");
				return null;
			}
		}


	
		private static void DrawPathHelper (Vector3[] path, Color color, string method)
		{
			Vector3[] vector3s = VRB.Utility.Utility.PathControlPointGenerator (path);
		
			//Line Draw:
			Vector3 prevPt = VRB.Utility.Utility.Interp (vector3s, 0);
			Gizmos.color = color;
			int SmoothAmount = path.Length * 20;
			for (int i = 1; i <= SmoothAmount; i++) {
				float pm = (float)i / SmoothAmount;
				Vector3 currPt = VRB.Utility.Utility.Interp (vector3s, pm);
				if (method == "gizmos") {
					Gizmos.DrawLine (currPt, prevPt);
				} else if (method == "handles") {
					Debug.LogError ("iTween Error: Drawing a path with Handles is temporarily disabled because of compatability issues with Unity 2.6!");
					//UnityEditor.Handles.DrawLine(currPt, prevPt);
				}
				prevPt = currPt;
			}
		}	

		public void Offset(int offset) {

			List<Vector3> newNodes = new List<Vector3>();
			for (int i = offset; i < nodes.Count+offset; ++i) {

				newNodes.Add(nodes[i%nodes.Count]);
			}
			nodes = newNodes;
		}

		public void Reverse() {

			List<Vector3> newNodes = new List<Vector3>();
			for (int i = nodes.Count-1; i >= 0; --i) {

				newNodes.Add(nodes[i]);
			}
			nodes = newNodes;
		}
	}
}