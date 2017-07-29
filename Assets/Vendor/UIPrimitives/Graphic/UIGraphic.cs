using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UIPrimitives;

namespace UIPrimitives
{
	public class UIGraphic : Graphic
	{
	
		//readonly

		//Serialized
	
		//Protected
		protected VertexHelper vertexHelper;
	
		//Delegates

		//Singleton
		
		//////Structs//////
		//Parts of the circle for drawing
		public enum CircleType
		{
			Main,
			GlowOuter,
			GlowInner
		}
		//Struct for circle colors
		protected struct CircleColors
		{
			public Color innerColor;
			public Color outerColor;
		}



		///////////////////////////////////////////////////////////////////////////
		//
		// Inherited from Graphic
		//
 
		///////////////////////////////////////////////////////////////////////////
		//
		// UIGraphic Functions
		//

		/// <summary>
		/// Allows our animator classes to update our geometry
		/// </summary>
		public void SetAllDirty_Public ()
		{
//			this.UpdateGeometry ();
			this.SetAllDirty();
		}
	
		///////////////////////////////////////////////////////////////////////////
		//
		// Drawing Functions
		//
		
		/// <summary>
		/// Draws the dot.
		/// </summary>
		/// <param name="vbo">Vbo.</param>
		/// <param name="center">Center position.</param>
		/// <param name="dotColor">Dot color.</param>
		protected void DrawDot (List<UIVertex> vbo, Vector3 center, Color dotColor,float dotRadius,int dotSubdivisions)
		{
			float outer = 7.5f * dotRadius;
			float inner = 0;
			//Positon
			Vector3 prevX = Vector3.zero;
			Vector3 prevY = Vector3.zero;
			
			int adjustedCircleSubdivisions = UIUtility.GetAdjustedSubdivisions (dotSubdivisions);
			for (int i=0; i<361; i+=adjustedCircleSubdivisions) {
				//position
				float rad = Mathf.Deg2Rad * (i);
				float c = Mathf.Cos (rad);
				float s = Mathf.Sin (rad);
				Vector3 innerPosition = new Vector3 (inner * c, inner * s, 0);
				Vector3 outerPosition = new Vector3 (outer * c, outer * s, 0);
				innerPosition += center;
				outerPosition += center;
				//Vertices
				Vector3[] positions = new Vector3[4];
				Color[] colors = new Color[]{dotColor,dotColor,dotColor,dotColor};
				//Outer
				positions [0] = prevX;
				if (i == 0)
					positions [0] = outerPosition;
				prevX = outerPosition;
				positions [1] = prevX;
				//Inner
				positions [2] = innerPosition;
				positions [3] = prevY;
				if (i == 0)
					positions [3] = innerPosition;
				prevY = innerPosition;
				
				AddQuad (vbo, positions, colors);
			}
		}

		///////////////////////////////////////////////////////////////////////////
		//
		// Helper Functions
		//
		
		protected List<Vector3> storedPositions;
		protected List<Color> storedColors;
		
		protected void StoreQuad (Vector3[] positions, Color[] colors)
		{
			for (int i=0; i<4; ++i) {
				Vector3 positionToStore = positions [i];
				storedPositions.Add (positionToStore);
				storedColors.Add (colors [i]);
			}
		}
		
		protected void ReleaseQuads (List<UIVertex> vbo)
		{
			UIVertex vert = UIVertex.simpleVert;
			for (int i=0; i<storedPositions.Count; i+=4) {
				UIVertex[] verts = new UIVertex[4];
				for (int k = 0; k < 4; ++k) 
				{
					vert.position = storedPositions [i+k];
					vert.color = storedColors [i+k];
					verts[k]=vert;
					vbo.Add (vert);
				}
				this.vertexHelper.AddUIVertexQuad(verts);
			}
		}

		/// <summary>
		/// Helper class to add the quad to the VBO
		/// </summary>
		/// <param name="vbo">Vertex Buffer Object.</param>
		/// <param name="positions">Positions.</param>
		/// <param name="colors">Colors.</param>
		protected void AddQuad (List<UIVertex> vbo, Vector3[] positions, Color[] colors)
		{
			UIVertex vert = UIVertex.simpleVert;
			UIVertex[] verts = new UIVertex[4];
			for (int i=0; i<4; ++i) {
				vert.position = positions [i];
				vert.color = colors [i];
				verts[i]=vert;
				vbo.Add (vert);

			}
			this.vertexHelper.AddUIVertexQuad(verts);
		}

		#if UNITY_EDITOR
		public static void CreateObject<T>(string name) where T: UIGraphic
		{
			Transform activeTransform = UnityEditor.Selection.activeTransform;
			GameObject newUIObject = new GameObject(name);
			newUIObject.transform.SetParent(activeTransform);
			newUIObject.transform.localPosition = Vector3.zero;
			newUIObject.transform.localRotation = Quaternion.identity;
			newUIObject.transform.localScale = Vector3.one;
			newUIObject.AddComponent<T>();
			UnityEditor.Selection.activeTransform = newUIObject.transform;
		}
		#endif
	}
}