using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UIPrimitives;

namespace UIPrimitives
{
	public class UILine : UIGraphic
	{
		//enum
		public enum CapType
		{
			Main,
			GlowOuter,
			GlowInner
		}

		//////Dimensions//////
		//Thickness
		[Tooltip("Thickness of line.")]
		public float
			thickness = 1f;
		//FillPercent
		[Tooltip("Fill percent of line.")]
		[Range(0F,1F)]
		public float
			fillPercent = 1f;
		//FillSubdivisions
		[Tooltip("Subdivisons along the path of the line.")]
		[Range(1,100)]
		public int
			fillSubdivisions = 30;
		//Nodes
		public List<Vector2> nodes = new List<Vector2> (){
		new Vector2(-32f,-16.5f),
		new Vector2(-14f,-16.5f),
		new Vector2(0,18f),
		new Vector2(16f,18f)};
	
	
		/////Appearance/////
		//UseGradient
		[Tooltip("Use gradient instead of solid color.")]
		public bool	useGradient;
		[Tooltip("Gradient to use.")]
		public Gradient gradient;

		/////Glow/////
		//ShouldGlow
		public bool shouldGlow = false;
		//GlowColor
		public Color glowColor = Color.cyan;
		//GlowDistance
		[Range(0.1F,10f)]
		public float
			glowDistance = 1.5f;
		
		/////Smooth/////
		//SmoothLine
		public bool smoothLine = false;
		//SmoothMultiplier
		[Range(1,10)]
		public int
			smoothMultiplier = 3;
		//StitchEdges
		public bool stitchEdges = false;
		
		/////Dotted/////
		[Range(4,100)]
		public int
			dottedLineSubdivisions = 16;
		public bool dottedLine = false;
		
		/////Cap/////
		public bool startCap = false, endCap = false;
		public float startCapWidth = .5f, endCapWidth = 1.5f;
		public Color startCapColor = Color.white, endCapColor = Color.white;
		public bool startCapGlow = false, endCapGlow = false;
		[Range(0.01F,1f)]
		public float
			startCapThickness = 1f;
		[Range(0.01F,1f)]
		public float
			endCapThickness = .15f;
		
		///////////////////////////////////////////////////////////////////////////
		//
		// Inherited from UIGraphic
		//


		protected override void OnPopulateMesh (VertexHelper vh)
		{
			List<UIVertex> verts = new List<UIVertex>();
			vh.GetUIVertexStream(verts);
			vh.Clear();
			this.vertexHelper = vh;

			FillVBO(verts);

		}

		/// <summary>
		/// Called every frame that the geometry needs to be updated, done automatically when variables are changed.
		/// You can force the call with this.UpdateGeometry()
		/// </summary>
		/// <param name="vbo">Vertex Buffer Object.</param>
		protected void FillVBO (List<UIVertex> vbo)
		{
			vbo.Clear ();
			DrawLine (vbo, 0);

		}

		
		///////////////////////////////////////////////////////////////////////////
		//
		// UILine Functions
		//
	
		protected void DrawLine (List<UIVertex> vbo, float zValue)
		{
			vbo.Clear ();
			if (nodes.Count == 0)
				return;

			Vector2[] lineNodes;

			storedPositions = new List<Vector3> ();
			storedColors = new List<Color> ();
			Vector3[] positions;
			Color[] colors;
			Vector3 last_TL = Vector3.zero, last_TR = Vector3.zero;
			Vector3 lastGlowTop_TL = Vector3.zero, lastGlowTop_TR = Vector3.zero;
			Vector3 lastGlowBottom_TL = Vector3.zero, lastGlowBottom_TR = Vector3.zero;

			lineNodes = nodes.ToArray ();
			if (smoothLine) {
				lineNodes = SetPath ();
			}
			if (fillSubdivisions != 1) {
				Vector2[] newLineNodes = new Vector2[(lineNodes.Length - 1) * fillSubdivisions];
				for (int i=0; i<lineNodes.Length-1; ++i) {
					for (int j=0; j<fillSubdivisions; ++j) {
						float percent = ((float)j) / (fillSubdivisions - 1);
						newLineNodes [i * fillSubdivisions + j] = Vector2.Lerp (lineNodes [i], lineNodes [i + 1], percent);
					}
				}
				lineNodes = newLineNodes;
			}
			if (dottedLine) {
				Vector2[] newLineNodes = new Vector2[(lineNodes.Length - 1) * dottedLineSubdivisions];
				for (int i=0; i<lineNodes.Length-1; ++i) {
					for (int j=0; j<dottedLineSubdivisions; ++j) {
						float percent = ((float)j) / (dottedLineSubdivisions - 1);
						newLineNodes [i * dottedLineSubdivisions + j] = Vector2.Lerp (lineNodes [i], lineNodes [i + 1], percent);
					}
				}
				lineNodes = newLineNodes;
			}
			int fillDistance = (int)((lineNodes.Length - 1) * fillPercent);
			for (int i=0; i<fillDistance; i++) {
				float percent = ((float)i) / ((float)(lineNodes.Length - 1));
				float percentNext = ((float)(i+1)) / ((float)(lineNodes.Length - 1));
				Color lineColor = color;
				Color lineColorNext = color;
				if (useGradient) {
					lineColor = gradient.Evaluate (percent);
					lineColorNext = gradient.Evaluate (percentNext);
				}
				bool doNotDraw = dottedLine && (i) % 2 == 0;
				if (doNotDraw){
					lineColor.a = 0;
					lineColorNext.a = 0;
				}
				Vector3 currentNodePosition = lineNodes [i];
				Vector3 nextNodePosition = lineNodes [i + 1];
				Vector3 newVec = nextNodePosition - currentNodePosition;
				Vector3 newVector = Vector3.Cross (newVec, Vector3.forward);
				newVector.Normalize ();
				//////// Draw Normal Line ////////
				float width = thickness;
				Vector3 rect_TL = (currentNodePosition + newVector * -width);
				Vector3 rect_TR = (currentNodePosition + newVector * width);
				Vector3 rect_BR = (nextNodePosition + newVector * width);
				Vector3 rect_BL = (nextNodePosition + newVector * -width);
				positions = new Vector3[]{rect_TL,rect_TR,rect_BR,rect_BL};
				colors = new Color[]{lineColor,lineColor,lineColorNext,lineColorNext};
				StoreQuad (positions, colors);
				//Stitch Edges
				if (!dottedLine && stitchEdges && i != 0 && (i % (fillSubdivisions) == 0)) {
					positions = new Vector3[]{rect_TL,rect_TR,last_TR,last_TL};
					StoreQuad (positions, colors);
				}
				if (i % fillSubdivisions != fillSubdivisions - 1 || fillSubdivisions == 1) {
					last_TL = rect_BL;
					last_TR = rect_BR;
				}
				if (shouldGlow) {
					//////// Draw Glow Top ////////
					Vector3 glowTopRect_TL = (rect_TL + newVector * -width * glowDistance);
					Vector3 glowTopRect_TR = rect_TL;
					Vector3 glowTopRect_BR = rect_BL;
					Vector3 glowTopRect_BL = (rect_BL + newVector * -width * glowDistance);
					positions = new Vector3[] {
				glowTopRect_TL,
				glowTopRect_TR,
				glowTopRect_BR,
				glowTopRect_BL
			};
					//Color
					Color glowColorInside = glowColor;
					if (doNotDraw)
						glowColorInside.a = 0;
					Color glowColorOutside = glowColorInside;
					glowColorOutside.a = 0;
					colors = new Color[] {
				glowColorOutside,
				glowColorInside,
				glowColorInside,
				glowColorOutside
			};
					AddQuad (vbo, positions, colors);
					//Stitch Edges
					if (!dottedLine && stitchEdges && i != 0 && (i % (fillSubdivisions) == 0)) {
						positions = new Vector3[] {
					glowTopRect_TL,
					glowTopRect_TR,
					lastGlowTop_TR,
					lastGlowTop_TL
				};
						AddQuad (vbo, positions, colors);
					}
					if (i % fillSubdivisions != fillSubdivisions - 1 || fillSubdivisions == 1) {
						lastGlowTop_TL = glowTopRect_BL;
						lastGlowTop_TR = glowTopRect_BR;
					}
			
					//////// Draw Glow Bottom ////////
					Vector3 glowBottomRect_TL = rect_TR;
					Vector3 glowBottomRect_TR = (rect_TR + newVector * width * glowDistance);
					Vector3 glowBottomRect_BR = (rect_BR + newVector * width * glowDistance);
					Vector3 glowBottomRect_BL = rect_BR;

					positions = new Vector3[] {
				glowBottomRect_TL,
				glowBottomRect_TR,
				glowBottomRect_BR,
				glowBottomRect_BL
			};
					colors = new Color[] {
				glowColorInside,
				glowColorOutside,
				glowColorOutside,
				glowColorInside
			};
					AddQuad (vbo, positions, colors);
					//Stitch Edges
					if (!dottedLine && stitchEdges && i != 0 && (i % (fillSubdivisions) == 0)) {
						positions = new Vector3[] {
					glowBottomRect_TL,
					glowBottomRect_TR,
					lastGlowBottom_TR,
					lastGlowBottom_TL
				};
						AddQuad (vbo, positions, colors);
					}
					if (i % fillSubdivisions != fillSubdivisions - 1 || fillSubdivisions == 1) {
						lastGlowBottom_TL = glowBottomRect_BL;
						lastGlowBottom_TR = glowBottomRect_BR;
					}
				}
			}

			if (startCap && fillPercent >0) {
				//Draw Start Cap
				Vector3 capOffset = nodes [0];
				float radius = 7.5f * startCapWidth;
				float radiusOutside = radius;
				float radiusInside = radiusOutside * (1f - startCapThickness);
				DrawCircle (vbo, capOffset, radiusInside, radiusOutside, startCapColor, CircleType.Main);
				//Draw Glow
				if (startCapGlow) {
					//Draw Outer Glow
					float glowRadiusOutside = radiusOutside + radius * glowDistance;
					DrawCircle (vbo, capOffset, radiusOutside, glowRadiusOutside, startCapColor, CircleType.GlowOuter);
					//Draw Inner Glow
					if (startCapThickness != 1f) {
						float glowRadiusInside = radiusInside - radius * glowDistance;
						DrawCircle (vbo, capOffset, glowRadiusInside, radiusInside, startCapColor, CircleType.GlowInner);
					}
				}
			}
			if (endCap && fillPercent >=1f) {
				//Draw End Cap
				Vector3 capOffset = nodes [nodes.Count - 1];
				float radius = 7.5f * endCapWidth;
				float radiusOutside = radius;
				float radiusInside = radiusOutside * (1f - endCapThickness);
				DrawCircle (vbo, capOffset, radiusInside, radiusOutside, endCapColor, CircleType.Main);
				//Draw Glow
				if (endCapGlow) {
					//Draw Outer Glow
					float glowRadiusOutside = radiusOutside + radius * glowDistance;
					DrawCircle (vbo, capOffset, radiusOutside, glowRadiusOutside, endCapColor, CircleType.GlowOuter);
					//Draw Inner Glow
					if (endCapThickness != 1f) {
						float glowRadiusInside = radiusInside - radius * glowDistance;
						DrawCircle (vbo, capOffset, glowRadiusInside, radiusInside, endCapColor, CircleType.GlowInner);
					}
				}
			}
			ReleaseQuads (vbo);
		}
		
		/// <summary>
		/// Draws the circle, outer glow, and inner glow.
		/// </summary>
		/// <param name="vbo">Vertex Buffer Object.</param>
		/// <param name="radiusInside">Radius inside.</param>
		/// <param name="radiusOutside">Radius outside.</param>
		/// <param name="circleType">Circle type.</param>
		protected void DrawCircle (List<UIVertex> vbo, Vector3 offset, float radiusInside, float radiusOutside, Color mainColor, CircleType circleType)
		{
			int angle = 360;
			int subdivisions = 7;
			int angleOffset = 0;
			//Calculate the actual amount of subdivisions
			int adjustedSubdivisions = UIUtility.GetAdjustedSubdivisions (subdivisions);
			//Positon
			Vector3 prevX = Vector3.zero;
			Vector3 prevY = Vector3.zero;
			Vector3 center = Vector2.Scale ((rectTransform.pivot - (Vector2.one * .5f)), rectTransform.sizeDelta);
			center -= offset;
			//Angle
			float anglePercent = ((float)angle) / 360f;
			bool resetPositions = true;
			//Circle	
			for (int i=0; i<361; i+=adjustedSubdivisions) {
				float absolutePercent = ((float)i) / 360f;
				float relativePercent = absolutePercent / anglePercent;
				//Position 
				float radians = Mathf.Deg2Rad * (i + angleOffset);
				float cosValue = Mathf.Cos (radians);
				float sinValue = Mathf.Sin (radians);
				Vector3 innerPosition = new Vector3 (radiusInside * cosValue, radiusInside * sinValue, 0) - center;
				Vector3 outerPosition = new Vector3 (radiusOutside * cosValue, radiusOutside * sinValue, 0) - center;
				//We want to prevent weird stretching when there are gaps
				if (resetPositions) {
					prevX = outerPosition;
					prevY = innerPosition;
					resetPositions = false;
				}
				//Get colors
				CircleColors circleColors = GetCircleColors (circleType, mainColor, absolutePercent, relativePercent);
				//Draw

				//InnerLeft,OuterLeft,OuterRight,InnerRight
				Vector3[] positions = new Vector3[]
					{
						prevX,
						prevY,
						outerPosition,
						innerPosition
					};
				Color[] colors = new Color[]{
						circleColors.innerColor,
						circleColors.outerColor,
						circleColors.outerColor,
						circleColors.innerColor
					};
				//Actually add the quads to the VBO for drawing
				StoreQuad (positions, colors);
				//Save values for next quad
				prevX = innerPosition;
				prevY = outerPosition;

			}
		}
		
		/// <summary>
		/// Gets the circle colors.
		/// </summary>
		/// <returns>The circle colors.</returns>
		/// <param name="circleType">Circle type.</param>
		/// <param name="absolutePercent">Absolute percent.</param>
		/// <param name="relativePercent">Relative percent.</param>
		protected CircleColors GetCircleColors (CircleType circleType, Color capColor, float absolutePercent, float relativePercent)
		{
			CircleColors circleColors;
			//Determine what part of the circle we are drawing
			switch (circleType) {
			case CircleType.Main://Body of the circle
				circleColors.outerColor = capColor;
				circleColors.innerColor = capColor;
				break;
			case CircleType.GlowOuter://Outer glow of the circle
				circleColors.innerColor = glowColor;
				circleColors.innerColor.a = color.a * glowColor.a;

				circleColors.outerColor = circleColors.innerColor;
				circleColors.outerColor.a = 0;
				break;
			case CircleType.GlowInner://Inner glow of the circle
				circleColors.outerColor = glowColor;
				circleColors.outerColor.a = color.a * glowColor.a;

				circleColors.innerColor = circleColors.outerColor;
				circleColors.innerColor.a = 0;
				break;
			default:
				circleColors.outerColor = Color.white;
				circleColors.innerColor = Color.white;
				break;
			}
			return circleColors;
		}
	
		protected Vector2[] SetPath ()
		{
			Vector3[] nodes3D = new Vector3[nodes.Count];
			for (int i=0; i<nodes3D.Length; ++i) {
				nodes3D [i] = nodes [i];
			}
			Vector2[] vector2s = UIUtility.PathControlPointGenerator (nodes.ToArray ());
			int SmoothAmount = vector2s.Length * smoothMultiplier;
			Vector2[] smoothedPath = new Vector2[SmoothAmount];
			for (int i = 0; i < SmoothAmount; i++) {
				float pm = (float)i / (SmoothAmount - 1);
				Vector2 currPt = UIUtility.Interp (vector2s, pm);
				smoothedPath [i] = currPt;
			}
			return smoothedPath;
		}

		
		
		/// <summary>
		/// Sets the fill percent.
		/// </summary>
		/// <param name="percent">Percent.</param>
		public void SetFillPercent (float percent)
		{
			fillPercent = percent;
			this.UpdateGeometry ();
		}
		
		/// <summary>
		/// Deprecated, sets the width to x*2, works for two point lines.
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		public void SetX (float x)
		{
			Vector2 node0 = nodes [0];
			node0.x = -x;
			nodes [0] = node0;
			Vector2 node1 = nodes [1];
			node1.x = x;
			nodes [1] = node1;
			this.UpdateGeometry ();
		}

		#if UNITY_EDITOR
		[UnityEditor.MenuItem("GameObject/UIPrimitives/UILine",false, 10)]
		public static void CreateUILine()
		{
			CreateObject<UILine>("UILine");
		}
		#endif

	}
}




