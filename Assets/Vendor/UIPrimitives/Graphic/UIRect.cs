using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UIPrimitives;

namespace UIPrimitives
{
	/// <summary>
	/// uGUI Rect Primitive
	/// Uses RectTransform's scale, rotation, width/height, and pivot values.
	/// </summary>
	public class UIRect : UIFillableGraphic
	{
		//////Dimensions//////
		//Thickness
		[Tooltip ("Percent of how filled in the rect is.")]
		[Range (0.001F, 1.0F)]
		public float
			thickness = 1F;
		//XSubdivisions
		[Tooltip ("Amount of horizontal subdivisions, increase if yu see gradient banding.")]
		[Range (1, 100)]
		public int
			xSubdivisions = 1;
		//YSubdivisions
		[Tooltip ("Amount of vertical subdivisions, increase if yu see gradient banding.")]
		[Range (1, 100)]
		public int
			ySubdivisions = 1;
		//FillPercentX
		[Tooltip ("Horizontal fill percent.")]
		[Range (0.0F, 1.0F)]
		public float
			fillPercentX = 1F;
		//FillPercentY
		[Tooltip ("Vertical fill percent.")]
		[Range (0.0F, 1.0F)]
		public float
			fillPercentY = 1F;
		[Tooltip ("Border radius.")]
		[Range (0f, 1f)]
		public float
			borderRadius;
	
		//////Appearance//////
		//UseGradient
		//UseGradient
		[Tooltip ("Use gradient coloring.")]
		public bool
			useGradient;
		//GradientStyle
		[Tooltip ("Gradient calculation style, Horizontal or Vertical.")]
		public GradientStyleRect
			gradientStyle;
		//Gradient
		[Tooltip ("Gradient to use, only uses front and end colors.")]
		public Gradient
			gradient;
		//GradientStyleRect
		public enum GradientStyleRect
		{
			Horizontal,
			Vertical
		}

		//////Glow//////
		//ShouldGlow
		[Tooltip ("Add a glow to the circle.")]
		public bool
			shouldGlow = false;
		//UseCircleColor
		[Tooltip ("Use rect color for the glow.")]
		public bool
			useRectColor = true;
		//GlowColor
		[Tooltip ("Color for the rect's glow.")]
		public Color
			glowColor = Color.white;
		//GlowDistance
		[Tooltip ("Distance from the edge of the circle for the glow.")]
		[Range (0.01F, 1f)]
		public float
			glowDistance = .2f;

		//////Structs//////
		//Struct for rect coordinates
		protected struct RectCoords
		{
			public Vector3 TL;
			public Vector3 TR;
			public Vector3 BR;
			public Vector3 BL;
		}
		//Struct for glow colors
		protected class GlowColors
		{
			public Color innerGlowColorTL;
			public Color outerGlowColorTL;
			public Color innerGlowColorTR;
			public Color outerGlowColorTR;
			public Color innerGlowColorBR;
			public Color outerGlowColorBR;
			public Color innerGlowColorBL;
			public Color outerGlowColorBL;

			public void SetInnerColors(Color color) {

				innerGlowColorBL = color;
				innerGlowColorBR = color;
				innerGlowColorTL = color;
				innerGlowColorTR = color;
			}

			public void SetOuterColors(Color color) {

				outerGlowColorBL = color;
				outerGlowColorBR = color;
				outerGlowColorTL = color;
				outerGlowColorTR = color;
			}

			public GlowColors Reverse() {
				GlowColors glowColors = new GlowColors();
				glowColors.innerGlowColorBL = outerGlowColorBL;
				glowColors.innerGlowColorTL = outerGlowColorTL;
				glowColors.innerGlowColorTR = outerGlowColorTR;
				glowColors.innerGlowColorBR = outerGlowColorBR;
				glowColors.outerGlowColorBL = innerGlowColorBL;
				glowColors.outerGlowColorTL = innerGlowColorTL;
				glowColors.outerGlowColorTR = innerGlowColorTR;
				glowColors.outerGlowColorBR = innerGlowColorBR;
				return glowColors;
			}
		}

	
		///////////////////////////////////////////////////////////////////////////
		//
		// Inherited from UIGraphic
		//

		protected override void OnPopulateMesh (VertexHelper vh)
		{
			List<UIVertex> verts = new List<UIVertex> ();
			vh.GetUIVertexStream (verts);
			vh.Clear ();
			this.vertexHelper = vh;

			FillVBO (verts);

		}

		/// <summary>
		/// Called every frame that the geometry needs to be updated, done automatically when variables are changed.
		/// You can force the call with this.UpdateGeometry()
		/// </summary>
		/// <param name="vbo">Vertex Buffer Object.</param>
		protected void FillVBO (List<UIVertex> vbo)
		{
			vbo.Clear ();
			DrawRect (vbo);
		}

	
		///////////////////////////////////////////////////////////////////////////
		//
		// UIRect Functions
		//

		/// <summary>
		/// Draws the rect.
		/// </summary>
		/// <param name="vbo">Vertex Buffer Object.</param>
		void DrawRect (List<UIVertex> vbo)
		{
			vbo.Clear ();
		
			float rectWidth = rectTransform.sizeDelta.x;
			float rectHeight = rectTransform.sizeDelta.y;
			float xDistance;
			float yDistance;
			Vector3 xDirection = Vector3.right;
			Vector3 yDirection = Vector3.up;
			Vector3 center = Vector2.Scale ((rectTransform.pivot - (Vector2.one * .5f)), rectTransform.sizeDelta);
			Vector3[] positions;
			Color[] colors;
			
			this.fillPercentX = Mathf.Clamp01 (this.fillPercentX);
			this.fillPercentY = Mathf.Clamp01 (this.fillPercentY);

			//////// Draw Normal Rect ////////
			if (thickness == 1f) {
				//Dont draw at all if either fill percents are 0
				bool shouldFillPercent = (xSubdivisions == 1 && ySubdivisions == 1);
				if (shouldFillPercent && (fillPercentX == 0 || fillPercentY == 0)) {
					return;
				}
				//No subdivisions
				if (xSubdivisions == 1 && ySubdivisions == 1) {

					//Draw the rounded rectangle
					if (borderRadius != 0) {
						

						//Calculate distance to inner rect
						xDistance = rectWidth * .5f * fillPercentX * (1f -borderRadius);
						yDistance = rectHeight * .5f * fillPercentY * (1f- borderRadius);

						float xOffset = rectWidth * .5f * (1f - fillPercentX);
						float yOffset = rectHeight * .5f * (1f - fillPercentY);

						//Calculates offset to inner rect to make the stroke even
						if (fillPercentX == 1f && fillPercentY == 1f) {
							if (rectWidth > rectHeight) {
								xDistance += (((float)rectHeight) / ((float)rectWidth)) * rectWidth * borderRadius * (((rectWidth - rectHeight) / (2f * rectHeight)));
							} else {
								yDistance += (((float)rectWidth) / ((float)rectHeight)) * rectHeight  * borderRadius * (((rectHeight - rectWidth) / (2f * rectWidth)));
							}
						} else {
							if (rectWidth > rectHeight) {
								xDistance += (((float)rectHeight) / ((float)rectWidth)) * rectWidth * (fillPercentX) * (borderRadius) * (((rectWidth - rectHeight) / (2f * rectHeight)));
							} else {
								yDistance += (((float)rectWidth) / ((float)rectHeight)) * rectHeight * (fillPercentY) *(borderRadius) * (((rectHeight - rectWidth) / (2f * rectWidth)));
							}
						}

						//Rect coordinates
						RectCoords rectCoords = GetRectCoords (xDistance, yDistance, xDirection, yDirection, center);
						rectCoords = OffsetRectCoords (rectCoords, xDirection, yDirection, xOffset, yOffset);

						if (fillPercentX == 1f && fillPercentY == 1f) {
							//Calculate distances for size of stroke, we take the smaller stroke and use it for both so it doesn't look strange
							xDistance = Mathf.Min (rectWidth, rectHeight) * .5f * borderRadius;
							yDistance = xDistance;
						} else {

							xDistance = Mathf.Min (rectWidth, rectHeight) * .5f * borderRadius * fillPercentX;
							yDistance = Mathf.Min (rectWidth, rectHeight) * .5f * borderRadius * fillPercentY;
						}
						xDistance = Mathf.Min (rectWidth, rectHeight) * .5f * borderRadius;
						yDistance = xDistance;
						//Get the colors
//						colors = new Color[]{ color, color, color, color };
						colors = GetColors (0, 1f, 0, 1f);

						DrawRoundedRect (vbo, rectCoords, xDirection, yDirection, xDistance, yDistance, colors);

					} else {//Draw the a normal one quad rect if border radius is 0

						//Calculate size
						xDistance = rectWidth * .5f * fillPercentX;
						yDistance = rectHeight * .5f * fillPercentY;
						//Calculate our fill percent
						float xOffset = rectWidth * .5f * (1f - fillPercentX);
						float yOffset = rectHeight * .5f * (1f - fillPercentY);
						//Get the coordinates
						RectCoords rectCoords = GetRectCoords (xDistance, yDistance, xDirection, yDirection, center);
						rectCoords = OffsetRectCoords (rectCoords, xDirection, yDirection, xOffset, yOffset);
						//Get the colors
						colors = GetColors (0, fillPercentX, 0, fillPercentY);
						//Draw the quad
						AddQuad (vbo, rectCoords, colors);
					}
				} else {//Subdivided Rect
					xDistance = rectWidth * .5f;
					yDistance = rectHeight * .5f;
					//Save bottom left corner to help us do relative subdivisions
					Vector3 rect_BL = -xDistance * xDirection + -yDistance * yDirection - center;

					for (int i = 0; i < xSubdivisions; ++i) {
						//Calculate vertical percent to draw
						float xPercent = (i + 0) / ((float)(xSubdivisions));
						float xPercentNext = (i + 1) / ((float)(xSubdivisions));
						for (int j = 0; j < ySubdivisions; ++j) {
							//Calculate horizontal percent to draw
							float yPercent = (j + 0) / ((float)(ySubdivisions));
							float yPercentNext = (j + 1) / ((float)(ySubdivisions));

							//Make sure we should render this part
							if (xPercent >= fillPercentX)
								return;
							if (yPercent >= fillPercentY)
								return;

							//Calculate next distance to draw
							xDistance = rectWidth * xPercent;
							float xDistanceNext = rectWidth * xPercentNext;
							yDistance = rectHeight * yPercent;
							float yDistanceNext = rectHeight * yPercentNext;
							//Calculate the coordinates for the next portion
							Vector3 BL = rect_BL + xDistance * xDirection + yDistance * yDirection;
							Vector3 BR = rect_BL + xDistanceNext * xDirection + yDistance * yDirection;
							Vector3 TL = rect_BL + xDistance * xDirection + yDistanceNext * yDirection;
							Vector3 TR = rect_BL + xDistanceNext * xDirection + yDistanceNext * yDirection;
							positions = new Vector3[]{ TL, TR, BR, BL };
							//Get Colors
							colors = GetColors (xPercent, xPercentNext, yPercent, yPercentNext);
							//Draw the quad
							AddQuad (vbo, positions, colors);
						}
					}
				}
				//Draw the glow
				if (shouldGlow && borderRadius == 0) {
					xDistance = rectWidth * .5f;
					yDistance = rectHeight * .5f;
					float xOffset = 0;
					float yOffset = 0;
					//Don't draw the entire glow
					if (shouldFillPercent) {
						xDistance *= fillPercentX;
						yDistance *= fillPercentY;
						xOffset = rectWidth * .5f * (1f - fillPercentX);
						yOffset = rectHeight * .5f * (1f - fillPercentY);
					}
					//Get the coordinates
					RectCoords rectCoords = GetRectCoords (xDistance, yDistance, xDirection, yDirection, center);
					//If we aren't drawing the entire rect offset the glow
					if (shouldFillPercent) {
						rectCoords = OffsetRectCoords (rectCoords, xDirection, yDirection, xOffset, yOffset);
					}
					float distance = .5f * (glowDistance) * Mathf.Min (rectWidth, rectHeight);
					//Color
					GlowColors glowColors = GetGlowColors (0, fillPercentX, 0, fillPercentY);
					//Draw the inner glow
					DrawRectOutline (vbo, rectCoords, xDirection, yDirection, distance, distance, glowColors);
				}
			} else {//Draw Rect Outline
				//Calculate distance to inner rect
				xDistance = rectWidth * .5f * (1f - thickness);
				yDistance = rectHeight * .5f * (1f - thickness);
				//Calculates offset to inner rect to make the stroke even
				if (rectWidth > rectHeight) {
					xDistance += (((float)rectHeight) / ((float)rectWidth)) * rectWidth * thickness * (((rectWidth - rectHeight) / (2f * rectHeight)));
				} else {
					yDistance += (((float)rectWidth) / ((float)rectHeight)) * rectHeight * thickness * (((rectHeight - rectWidth) / (2f * rectWidth)));
				}
				//GlowDistance
				float actualGlowDistance = .5f * (glowDistance) * Mathf.Min (rectWidth, rectHeight);
				//Save inner rect for glow
				RectCoords innerRectCoords = GetRectCoords (xDistance - actualGlowDistance, yDistance - actualGlowDistance, xDirection, yDirection, center);
				//Rect coordinates
				RectCoords rectCoords = GetRectCoords (xDistance, yDistance, xDirection, yDirection, center);
				//Calculate distances for size of stroke, we take the smaller stroke and use it for both so it doesn't look strange
				xDistance = Mathf.Min (rectWidth, rectHeight) * .5f * thickness;
				yDistance = xDistance;
				//Draw the stroke rect
				GlowColors glowColors = new GlowColors();
				glowColors.SetInnerColors(color);
				glowColors.SetOuterColors(color);
				DrawRectOutline (vbo, rectCoords, xDirection, yDirection, xDistance, yDistance, glowColors);
				//Draw the glow
				if (shouldGlow && borderRadius == 0) {
					//Color
					glowColors = GetGlowColors (0, 1f, 0, 1f);
					GlowColors innerGlowColors = glowColors.Reverse();
					//Draw inner glow
					DrawRectOutline (vbo, innerRectCoords, xDirection, yDirection, actualGlowDistance, actualGlowDistance, innerGlowColors);
					//Calculate glow distance
					xDistance = rectWidth * .5f * 1f;
					yDistance = rectHeight * .5f * 1f;
					//Get the rect coordinates for the glow
					rectCoords = GetRectCoords (xDistance, yDistance, xDirection, yDirection, center);
					//Draw outer glow
					DrawRectOutline (vbo, rectCoords, xDirection, yDirection, actualGlowDistance, actualGlowDistance, glowColors);
				}
			}
		}

		protected void DrawRoundedRect (List<UIVertex> vbo, RectCoords rectCoords, Vector3 xDirection, Vector3 yDirection, float borderRadiusX, float borderRadiusY, Color[] colors)
		{
			Vector3 rect_TL = rectCoords.TL;
			Vector3 rect_TR = rectCoords.TR;
			Vector3 rect_BR = rectCoords.BR;
			Vector3 rect_BL = rectCoords.BL;

			Vector3 TL, TR, BR, BL;
			Vector3[] positions;

			//Top Center
			TL = rect_TL + yDirection * borderRadiusX;
			TR = rect_TR + yDirection * borderRadiusX;
			BR = rect_TR;
			BL = rect_TL;
			positions = new Vector3[]{ TL, TR, BR, BL };
			AddQuad (vbo, positions, new Color[] { colors[0],colors[1],colors[1],colors[0] });


			//Bottom Center
			BL = rect_BL - yDirection * borderRadiusX;
			BR = rect_BR - yDirection * borderRadiusX;
			TR = rect_BR;
			TL = rect_BL;
			positions = new Vector3[]{ TL, TR, BR, BL };
			AddQuad (vbo, positions, new Color[] { colors[3],colors[2],colors[2],colors[3] });


//			//Normal triangles, use this if I ever try to figure out border radius for partial rects
//			if (fillPercentX != 1f || fillPercentY !=1f) {
//				//Top Right
//				TL = rect_TR + yDirection * borderRadiusX;
//				TR = rect_TR + borderRadiusX * xDirection + yDirection * borderRadiusX;
//				BR = rect_TR + borderRadiusX * xDirection;
//				BL = rect_TR;
//				positions = new Vector3[]{ TR, BR, BL, TL };
//				AddQuad (vbo, positions, colors);
//			}
//			if (fillPercentX != 1f ) {
//				//Bottom Right
//				BL = rect_BR - yDirection * borderRadiusX;
//				BR = rect_BR + borderRadiusX * xDirection - yDirection * borderRadiusX;
//				TR = rect_BR + borderRadiusX * xDirection;
//				TL = rect_BR;
//				positions = new Vector3[]{ BR, BL, TL, TR };
//				AddQuad (vbo, positions, colors);
//			}
//			if (fillPercentY != 1f ) {
//				//Top Left
//				TL = rect_TL - borderRadiusX * xDirection + yDirection * borderRadiusX;
//				TR = rect_TL + yDirection * borderRadiusX;
//				BR = rect_TL;
//				BL = rect_TL - borderRadiusX * xDirection;
//				positions = new Vector3[]{ BR, BL, TL, TR };
//				AddQuad (vbo, positions, colors);
//			}
//

			//Right Center
			TR = rect_TR + borderRadiusX * xDirection;
			BR = rect_BR + borderRadiusX * xDirection;
			TL = rect_TR;
			BL = rect_BR;
			positions = new Vector3[]{ TR, BR, BL, TL };
			AddQuad (vbo, positions, new Color[] { colors[1],colors[2],colors[2],colors[1] });

			//Left Center
			TL = rect_TL - borderRadiusX * xDirection;
			BL = rect_BL - borderRadiusX * xDirection;
			TR = rect_TL;
			BR = rect_BL;
			positions = new Vector3[]{ TR, BR, BL, TL };
			AddQuad (vbo, positions, new Color[] { colors[0],colors[3],colors[3],colors[0] });

			//Middle Center
			TL = rect_TL;
			BL = rect_BL;
			TR = rect_TR;
			BR = rect_BR;
			positions = new Vector3[]{ TR, BR, BL, TL };
			AddQuad (vbo, positions, new Color[] { colors[1],colors[2],colors[3],colors[0] });

			int subdivisions = 8;
			if (this.borderRadius > .5f)
				subdivisions = 12;
			else if (this.borderRadius > .2f)
				subdivisions = 11;

			//TopLeft
			if (fillPercentY == 1f)
				DrawRoundedCorner (vbo, TL, colors [0], 90, borderRadiusX, subdivisions);

			//TopRight
			if (fillPercentX == 1f && fillPercentY == 1f)
				DrawRoundedCorner (vbo, TR, colors [1], 0, borderRadiusX, subdivisions);

			//BottomRight
			if (fillPercentX == 1f)
				DrawRoundedCorner (vbo, BR, colors [2], 270, borderRadiusX, subdivisions);

			//BottomLeft
			DrawRoundedCorner (vbo, BL, colors [3], 180, borderRadiusX, subdivisions);
		}

		protected void DrawRoundedCorner (List<UIVertex> vbo, Vector3 center, Color dotColor, int startAngle, float dotRadius, int dotSubdivisions)
		{
			float outer = dotRadius;
			float inner = 0;
			//Positon
			Vector3 prevX = Vector3.zero;
			Vector3 prevY = Vector3.zero;

			int adjustedCircleSubdivisions = UIUtility.GetAdjustedSubdivisions (dotSubdivisions);
			for (int i = startAngle; i < startAngle + 91; i += adjustedCircleSubdivisions) {
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
				Color[] colors = new Color[]{ dotColor, dotColor, dotColor, dotColor };
				//Outer
				positions [0] = prevX;
				if (i == startAngle)
					positions [0] = outerPosition;
				prevX = outerPosition;
				positions [1] = prevX;
				//Inner
				positions [2] = innerPosition;
				positions [3] = prevY;
				if (i == startAngle)
					positions [3] = innerPosition;
				prevY = innerPosition;

				AddQuad (vbo, positions, colors);
			}
		}

		/// <summary>
		/// Draws the rect outline.
		/// </summary>
		/// <param name="vbo">Vbo.</param>
		/// <param name="rect_TL">Rect_ T.</param>
		/// <param name="rect_TR">Rect_ T.</param>
		/// <param name="rect_BR">Rect_ B.</param>
		/// <param name="rect_BL">Rect_ B.</param>
		/// <param name="xDirection">X direction.</param>
		/// <param name="yDirection">Y direction.</param>
		/// <param name="xDistance">X distance.</param>
		/// <param name="yDistance">Y distance.</param>
		/// <param name="innerColor">Inner color.</param>
		/// <param name="outerColor">Outer color.</param>
		protected void DrawRectOutline (List<UIVertex> vbo, Vector3 rect_TL, Vector3 rect_TR, Vector3 rect_BR, Vector3 rect_BL, Vector3 xDirection, Vector3 yDirection, float xDistance, float yDistance, GlowColors glowColors)
		{
			Vector3 TL, TR, BR, BL;
			Vector3[] positions;
			Color[] colors;
			Color innerColor, outerColor;
			//Top Left
			innerColor = glowColors.innerGlowColorTL;
			outerColor = glowColors.outerGlowColorTL;
			TL = rect_TL - xDistance * xDirection + yDirection * yDistance;
			TR = rect_TL + yDirection * yDistance;
			BR = rect_TL;
			BL = rect_TL - xDistance * xDirection;
			positions = new Vector3[]{ BR, BL, TL, TR };
			colors = new Color[]{ innerColor, outerColor, outerColor, outerColor };
			AddQuad (vbo, positions, colors);
			//Top Center
			TL = rect_TL + yDirection * yDistance;
			TR = rect_TR + yDirection * yDistance;
			BR = rect_TR;
			BL = rect_TL;
			positions = new Vector3[]{ TL, TR, BR, BL };
			colors = new Color[]{ glowColors.outerGlowColorTL, glowColors.outerGlowColorTR, glowColors.innerGlowColorTR, glowColors.innerGlowColorTL };
			AddQuad (vbo, positions, colors);
			//Top Right
			innerColor = glowColors.innerGlowColorTR;
			outerColor = glowColors.outerGlowColorTR;
			TL = rect_TR + yDirection * yDistance;
			TR = rect_TR + xDistance * xDirection + yDirection * yDistance;
			BR = rect_TR + xDistance * xDirection;
			BL = rect_TR;
			positions = new Vector3[]{ TR, BR, BL, TL };
			colors = new Color[]{ outerColor, outerColor, innerColor, outerColor };
			AddQuad (vbo, positions, colors);
			//Bottom Left
			innerColor = glowColors.innerGlowColorBL;
			outerColor = glowColors.outerGlowColorBL;
			BL = rect_BL - xDistance * xDirection - yDirection * yDistance;
			BR = rect_BL - yDirection * yDistance;
			TR = rect_BL;
			TL = rect_BL - xDistance * xDirection;
			positions = new Vector3[]{ TR, BR, BL, TL };
			colors = new Color[]{ innerColor, outerColor, outerColor, outerColor };
			AddQuad (vbo, positions, colors);
			//Bottom Center
			BL = rect_BL - yDirection * yDistance;
			BR = rect_BR - yDirection * yDistance;
			TR = rect_BR;
			TL = rect_BL;
			positions = new Vector3[]{ TL, TR, BR, BL };
			colors = new Color[]{ glowColors.innerGlowColorBL, glowColors.innerGlowColorBR, glowColors.outerGlowColorBR, glowColors.outerGlowColorBL };
			AddQuad (vbo, positions, colors);
			//Bottom Right
			innerColor = glowColors.innerGlowColorBR;
			outerColor = glowColors.outerGlowColorBR;
			BL = rect_BR - yDirection * yDistance;
			BR = rect_BR + xDistance * xDirection - yDirection * yDistance;
			TR = rect_BR + xDistance * xDirection;
			TL = rect_BR;
			positions = new Vector3[]{ BR, BL, TL, TR };
			colors = new Color[]{ outerColor, outerColor, innerColor, outerColor };
			AddQuad (vbo, positions, colors);
			//Right Center
			TR = rect_TR + xDistance * xDirection;
			BR = rect_BR + xDistance * xDirection;
			TL = rect_TR;
			BL = rect_BR;
			positions = new Vector3[]{ TR, BR, BL, TL };
			colors = new Color[]{ glowColors.outerGlowColorTR, glowColors.outerGlowColorBR, glowColors.innerGlowColorBR, glowColors.innerGlowColorTR };
			AddQuad (vbo, positions, colors);
			//Left Center
			TL = rect_TL - xDistance * xDirection;
			BL = rect_BL - xDistance * xDirection;
			TR = rect_TL;
			BR = rect_BL;
			positions = new Vector3[]{ TR, BR, BL, TL };
			colors = new Color[]{ glowColors.innerGlowColorTL, glowColors.innerGlowColorBL, glowColors.outerGlowColorBL, glowColors.outerGlowColorTL };
			AddQuad (vbo, positions, colors);
		}

		/// <summary>
		/// Offsets the rect coords.
		/// </summary>
		/// <returns>The rect coords.</returns>
		/// <param name="rectCoords">Rect coords.</param>
		/// <param name="xDirection">X direction.</param>
		/// <param name="yDirection">Y direction.</param>
		/// <param name="xOffset">X offset.</param>
		/// <param name="yOffset">Y offset.</param>
		protected RectCoords OffsetRectCoords (RectCoords rectCoords, Vector3 xDirection, Vector3 yDirection, float xOffset, float yOffset)
		{
			rectCoords.TL += -xDirection * xOffset + -yDirection * yOffset;
			rectCoords.TR += -xDirection * xOffset + -yDirection * yOffset;
			rectCoords.BR += -xDirection * xOffset + -yDirection * yOffset;
			rectCoords.BL += -xDirection * xOffset + -yDirection * yOffset;
			return rectCoords;
		}

		/// <summary>
		/// Overload that takes RectCoords instead of Vector3[]
		/// </summary>
		/// <param name="vbo">Vertex Buffer Object.</param>
		/// <param name="positions">Positions.</param>
		/// <param name="colors">Colors.</param>
		/// <param name="rectCoords">Rect coords.</param>
		protected void AddQuad (List<UIVertex> vbo, RectCoords rectCoords, Color[] colors)
		{
			Vector3[] positions = new Vector3[] {
				rectCoords.TL,
				rectCoords.TR,
				rectCoords.BR,
				rectCoords.BL
			};
			AddQuad (vbo, positions, colors);
		}

		/// <summary>
		/// Overload that takes RectCoords instead of individual Vector3s.
		/// </summary>
		/// <param name="vbo">Vbo.</param>
		/// <param name="rectCoords">Rect coords.</param>
		/// <param name="xDirection">X direction.</param>
		/// <param name="yDirection">Y direction.</param>
		/// <param name="xDistance">X distance.</param>
		/// <param name="yDistance">Y distance.</param>
		/// <param name="innerColor">Inner color.</param>
		/// <param name="outerColor">Outer color.</param>
		protected void DrawRectOutline (List<UIVertex> vbo, RectCoords rectCoords, Vector3 xDirection, Vector3 yDirection, float xDistance, float yDistance, GlowColors glowColors)
		{
			DrawRectOutline (vbo, rectCoords.TL, rectCoords.TR, rectCoords.BR, rectCoords.BL, xDirection, yDirection, xDistance, yDistance, glowColors);
		}

		/// <summary>
		/// Helper function to generate rect coordinates.
		/// </summary>
		/// <returns>The rect coords.</returns>
		/// <param name="xDistance">X distance.</param>
		/// <param name="yDistance">Y distance.</param>
		/// <param name="xDirection">X direction.</param>
		/// <param name="yDirection">Y direction.</param>
		/// <param name="center">Center.</param>
		protected RectCoords GetRectCoords (float xDistance, float yDistance, Vector3 xDirection, Vector3 yDirection, Vector3 center)
		{
			RectCoords rectCoords;
			rectCoords.TL = -xDistance * xDirection + yDistance * yDirection - center;
			rectCoords.TR = xDistance * xDirection + yDistance * yDirection - center;
			rectCoords.BR = xDistance * xDirection + -yDistance * yDirection - center;
			rectCoords.BL = -xDistance * xDirection + -yDistance * yDirection - center;
			return rectCoords;
		}

		/// <summary>
		/// Gets the colors.
		/// </summary>
		/// <returns>The colors.</returns>
		/// <param name="xPercent">X percent.</param>
		/// <param name="xPercentNext">X percent next.</param>
		/// <param name="yPercent">Y percent.</param>
		/// <param name="yPercentNext">Y percent next.</param>
		protected Color[] GetColors (float xPercent, float xPercentNext, float yPercent, float yPercentNext)
		{
			Color[] colors;
			if (useGradient && gradientStyle == GradientStyleRect.Horizontal) {
				Color colorLeft = gradient.Evaluate (xPercent);
				Color colorRight = gradient.Evaluate (xPercentNext);
				colors = new Color[]{ colorLeft, colorRight, colorRight, colorLeft };
			} else if (useGradient && gradientStyle == GradientStyleRect.Vertical) {
				Color colorBottom = gradient.Evaluate (yPercent);
				Color colorTop = gradient.Evaluate (yPercentNext);
				colors = new Color[]{ colorTop, colorTop, colorBottom, colorBottom };
			} else {
				colors = new Color[]{ color, color, color, color };
			}
			return colors;
		}

		/// <summary>
		/// Gets the glow colors.
		/// </summary>
		/// <returns>The glow colors.</returns>
		protected GlowColors GetGlowColors (float xPercent, float xPercentNext, float yPercent, float yPercentNext)
		{
			GlowColors glowColors = new GlowColors();
		
			if(useRectColor)
			{
				Color[] colors = GetColors(xPercent,xPercentNext,yPercent,yPercentNext);
				glowColors.innerGlowColorTL = colors[0];
				glowColors.outerGlowColorTL = GetOuterGlowColor(glowColors.innerGlowColorTL);
				glowColors.innerGlowColorTR = colors[1];
				glowColors.outerGlowColorTR = GetOuterGlowColor(glowColors.innerGlowColorTR);
				glowColors.innerGlowColorBR = colors[2];
				glowColors.outerGlowColorBR = GetOuterGlowColor(glowColors.innerGlowColorBR);
				glowColors.innerGlowColorBL = colors[3];
				glowColors.outerGlowColorBL = GetOuterGlowColor(glowColors.innerGlowColorBL);
			}
			else
			{
				Color innerGlowColor = GetInnerGlowColor(glowColor);
				glowColors.SetInnerColors(glowColor);

				Color outerGlowColor = GetOuterGlowColor(innerGlowColor);
				glowColors.SetOuterColors(outerGlowColor);
			}

			return glowColors;
		}

		protected Color GetInnerGlowColor(Color glowColor) {
			glowColor.a = glowColor.a * color.a;
			return glowColor;
		}

		protected Color GetOuterGlowColor(Color glowColor) {
			glowColor.a = 0;
			return glowColor;
		}

		#if UNITY_EDITOR
		[UnityEditor.MenuItem("GameObject/UIPrimitives/UIRect",false, 10)]
		public static void CreateUIRect()
		{
			CreateObject<UIRect>("UIRect");
		}
		#endif

		///////////////////////////////////////////////////////////////////////////
		//
		// Inherited from UIFillable Graphic
		//

		public override void SetFillPercent(float percent) {

			this.fillPercentX = percent;
			this.SetAllDirty ();
		}
	}
}