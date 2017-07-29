using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UIPrimitives;

namespace UIPrimitives
{

/// <summary>
///  Sorry for the ugly code, the glow requires some lengthy sections.
/// </summary>
public class UIGrid : Graphic
{
	public bool drawGrid = true;
	public bool fadeEdges = true;
	[Range(0.05F,1.0F)]
	public float
		fadeDistance = .25f;
	[Range(0,100)]
	public int
		horizontalLineCount = 4;
	[Range(0,100)]
	public int
		verticalLineCount = 8;
	[Range(0.1F,10.0F)]
	public float
		lineWidth = 1.5f;
	Color edgeColor;

	//Glow
	public bool shouldGlow = true;
	public Color glowColor = Color.white;
	[Range(0.01F,1.0F)]
	public float
		glowDistance = .3f;


	float width, height;

	protected override void OnFillVBO (List<UIVertex> vbo)
	{
		vbo.Clear ();
		edgeColor = color;
		edgeColor.a = 0;

		width = rectTransform.rect.width;
		height = rectTransform.rect.height;
		float glowWidth = glowDistance * 2f * lineWidth;
		
		Vector3 left = new Vector3 (-width / 2f, 0, 0);
		Vector3 right = new Vector3 (width / 2f, 0, 0);
		Vector3 top = new Vector3 (0, height / 2f, 0);
		Vector3 bottom = new Vector3 (0, -height / 2f, 0);
		
		float yInterval = height / (horizontalLineCount + 1);
		float xInterval = (width) / (verticalLineCount + 1);


		//Store Circle
		
		storedPositions = new List<Vector3> ();
		storedColors = new List<Color> ();
		if (drawCircles) {
			for (int i=0; i<horizontalLineCount; ++i) {
				for (int j=0; j<verticalLineCount; ++j) {
					Vector3 newCirclePosition = new Vector3 ((j + 1) * xInterval * 1f - width * .5f, (i + 1) * 1f * yInterval - height * .5f, 0);
					DrawCircle (newCirclePosition, color);
				}
			}
		} else if (drawSquares) {
			for (int i=0; i<horizontalLineCount; ++i) {
				for (int j=0; j<verticalLineCount; ++j) {
					Vector3 newSquarePosition = new Vector3 ((j + 1) * xInterval * 1f - width * .5f, (i + 1) * 1f * yInterval - height * .5f, 0);
					DrawSquare (newSquarePosition, color);
				}
			}
		}
		if (drawGrid) {
			#region Grid
			for (int i=1; i<horizontalLineCount+1; ++i) {
				float startY = yInterval * (i) - height / 2f;
				//SetCornerPositions
				Vector3 TL = left;
				TL.y = startY;
				Vector3 TR = right;
				TR.y = startY;
				Vector3 BL = left;
				BL.y = startY - lineWidth;
				Vector3 BR = right;
				BR.y = startY - lineWidth;
				//Draw
				UIVertex vert = UIVertex.simpleVert;
			
				#region HorizontalLines
				///// Horizontal Lines //////
				if (fadeEdges) {
					float percent = ((float)i) / ((float)(horizontalLineCount + 1));
					percent -= .5f;
					percent = Mathf.Abs (percent);
					percent *= 2f;
					percent = 1f - percent;
					Color centerColor = color;
					centerColor.a *= percent;
				
					////// LEFT FADE ///////
				
					Vector3 L_BR = BL;
					Vector3 L_TL = TR;
					L_TL.x *= (fadeDistance * 1f - 1f);
					Vector3 L_TR = TL;
					Vector3 L_BL = BR;
					L_BL.x *= (fadeDistance * 1f - 1f);

					//Top Left
					vert.color = edgeColor;
					vert.position = L_TR;
					vbo.Add (vert);
					//Top Right
					vert.color = centerColor;
					vert.position = L_TL;
					vbo.Add (vert);
					//BottomRight
					vert.color = centerColor;
					vert.position = L_BL;
					vbo.Add (vert);
					//BottomLeft
					vert.color = edgeColor;
					vert.position = L_BR;
					vbo.Add (vert);

					if (shouldGlow) {
						//// GLOW ////
						Color fadedGlowColor = glowColor;
						fadedGlowColor.a = centerColor.a;
						Color alphaGlowColor = fadedGlowColor;
						alphaGlowColor.a = 0;
					
						//Glow Bottom
					
						//Top Left
						vert.color = fadedGlowColor;
						vert.position = L_BL;
						vbo.Add (vert);
						//Top Right
						vert.color = alphaGlowColor;
						vert.color.a = (byte)edgeColor.a;
						vert.position = L_BR;
						vbo.Add (vert);
						//Bottom Right
						vert.color = alphaGlowColor;
						vert.position = L_BR;
						vbo.Add (vert);
						//Bottom Left
						vert.color = alphaGlowColor;
						vert.color.a = (byte)edgeColor.a;
						vert.position = L_BL - Vector3.up * glowWidth;
						vbo.Add (vert);
						//Glow Top
					
						//Top Left
						vert.color = alphaGlowColor;
						vert.position = L_TL + Vector3.up * glowWidth;
						vbo.Add (vert);
						//Top Right
						vert.color = alphaGlowColor;
						vert.color.a = (byte)edgeColor.a;
						vert.position = L_TR;
						vbo.Add (vert);
						//Bottom Right
						vert.color = fadedGlowColor;
						vert.color.a = (byte)edgeColor.a;
						vert.position = L_TR;
						vbo.Add (vert);
						//Bottom Left
						vert.color = fadedGlowColor;
						vert.position = L_TL;
						vbo.Add (vert);
					}
				
					////// CENTER ///////
				
					vert.color = centerColor;
					Vector3 C_TL = L_TL;
					Vector3 C_TR = L_TL;
					C_TR.x *= -1f;
					Vector3 C_BR = L_BL;
					C_BR.x *= -1f;
					Vector3 C_BL = L_BL;

					//Top Left
					vert.position = C_TL;
					vbo.Add (vert);
					//Top Right
					vert.position = C_TR;
					vbo.Add (vert);
					//Bottom Right
					vert.position = C_BR;
					vbo.Add (vert);
					//Bottom Left
					vert.position = C_BL;
					vbo.Add (vert);

					if (shouldGlow) {
						//// GLOW ////
						Color fadedGlowColor = glowColor;
						fadedGlowColor.a = centerColor.a;
						Color alphaGlowColor = fadedGlowColor;
						alphaGlowColor.a = 0;
					
						//Glow Bottom
					
						//Top Left
						vert.color = fadedGlowColor;
						vert.position = C_BL;
						vbo.Add (vert);
						//Top Right
						vert.color = fadedGlowColor;
						vert.position = C_BR;
						vbo.Add (vert);
						//Bottom Right
						vert.color = alphaGlowColor;
						vert.position = C_BR;
						vert.position -= Vector3.up * glowWidth;
						vbo.Add (vert);
						//Bottom Left
						vert.color = alphaGlowColor;
						vert.position = C_BL - Vector3.up * glowWidth;
						vbo.Add (vert);
					
						//Glow Top
					
						//Top Left
						vert.color = alphaGlowColor;
						vert.position = C_TL + Vector3.up * glowWidth;
						vbo.Add (vert);
						//Top Right
						vert.color = alphaGlowColor;
						vert.position = C_TR;
						vert.position += Vector3.up * glowWidth;
						vbo.Add (vert);
						//Bottom Right
						vert.color = fadedGlowColor;
						vert.position = C_TR;
						vbo.Add (vert);
						//Bottom Left
						vert.color = fadedGlowColor;
						vert.position = C_TL;
						vbo.Add (vert);
					}
				
					////// RIGHT FADE ///////
					Vector3 R_BR = BR;
					Vector3 R_TL = L_TL;
					R_TL.x *= -1f;
					Vector3 R_TR = TR;
					Vector3 R_BL = L_BL;
					R_BL.x *= -1f;
				
					//Top Left
					vert.color = centerColor;
					vert.position = R_TL;
					vbo.Add (vert);
					//Top Right
					vert.color = edgeColor;
					vert.position = R_TR;
					vbo.Add (vert);
					//Bottom Right
					vert.color = edgeColor;
					vert.position = R_BR;
					vbo.Add (vert);
					//Bottom Left
					vert.color = centerColor;
					vert.position = R_BL;
					vbo.Add (vert);
				
					if (shouldGlow) {
						//// GLOW ////
						Color fadedGlowColor = glowColor;
						fadedGlowColor.a = centerColor.a;
						Color alphaGlowColor = fadedGlowColor;
						alphaGlowColor.a = 0;
					
						//Glow Bottom
					
						//Top Left
						vert.color = fadedGlowColor;
						vert.position = R_BL;
						vbo.Add (vert);
						//Top Right
						vert.color = alphaGlowColor;
						vert.color.a = (byte)edgeColor.a;
						vert.position = R_BR;
						vbo.Add (vert);
						//Bottom Right
						vert.color = alphaGlowColor;
						vert.position = R_BR;
						vbo.Add (vert);
						//Bottom Left
						vert.color = alphaGlowColor;
						vert.color.a = (byte)edgeColor.a;
						vert.position = R_BL - Vector3.up * glowWidth;
						vbo.Add (vert);
						//Glow Top
					
						//Top Left
						vert.color = alphaGlowColor;
						vert.position = R_TL + Vector3.up * glowWidth;
						vbo.Add (vert);
						//Top Right
						vert.color = alphaGlowColor;
						vert.color.a = (byte)edgeColor.a;
						vert.position = R_TR;
						vbo.Add (vert);
						//Bottom Right
						vert.color = fadedGlowColor;
						vert.color.a = (byte)edgeColor.a;
						vert.position = R_TR;
						vbo.Add (vert);
						//Bottom Left
						vert.color = fadedGlowColor;
						vert.position = R_TL;
						vbo.Add (vert);
					}

				} else {
					vert.color = color;
					vert.position = TL;
					vbo.Add (vert);
					vert.position = TR;
					vbo.Add (vert);
					vert.position = BR;
					vbo.Add (vert);
					vert.position = BL;
					vbo.Add (vert);

					if (shouldGlow) {
						//Glow Bottom
				
						//Top Left
						vert.color = glowColor;
						vert.position = BL;
						vbo.Add (vert);
						//Top Right
						vert.color = glowColor;
						vert.position = BR;
						vbo.Add (vert);
						//Bottom Right
						vert.color = glowColor;
						vert.color.a = 0;
						vert.position = BR - Vector3.up * glowWidth;
						vbo.Add (vert);
						//Bottom Left
						vert.color = glowColor;
						vert.color.a = 0;
						vert.position = BL - Vector3.up * glowWidth;
						vbo.Add (vert);
				
						//Glow Top
				
						//Top Left
						vert.color = glowColor;
						vert.color.a = 0;
						vert.position = TL + Vector3.up * glowWidth;
						vbo.Add (vert);
						//Top Right
						vert.color = glowColor;
						vert.color.a = 0;
						vert.position = TR + Vector3.up * glowWidth;
						vbo.Add (vert);
						//Bottom Right
						vert.color = glowColor;
						vert.position = TR;
						vbo.Add (vert);
						//Bottom Left
						vert.color = glowColor;
						vert.position = TL;
						vbo.Add (vert);
					}
				}

				//Right

			}
			#endregion
			#region VerticalLines
			///// Vertical Lines //////
			for (int i=1; i<verticalLineCount+1; ++i) {
				float startX = xInterval * (i) - width / 2f;
				//SetCornerPositions
				Vector3 TL = top;
				TL.x = startX;
				Vector3 TR = bottom;
				TR.x = startX;
				Vector3 BL = top;
				BL.x = startX + lineWidth;
				Vector3 BR = bottom;
				BR.x = startX + lineWidth;
				//Draw
				UIVertex vert = UIVertex.simpleVert;
			
				if (fadeEdges) {
					float percent = ((float)i) / ((float)(verticalLineCount + 1));
					percent -= .5f;
					percent = Mathf.Abs (percent);
					percent *= 2f;
					percent = 1f - percent;
					Color centerColor = color;
					centerColor.a *= percent;
				
					////// LEFT FADE ///////

					Vector3 L_BR = BL;
					Vector3 L_TL = TR;
					L_TL.y *= (fadeDistance * 1f - 1f);
					Vector3 L_TR = TL;
					Vector3 L_BL = BR;
					L_BL.y *= (fadeDistance * 1f - 1f);

					//Top Left
					vert.color = edgeColor;
					vert.position = TL;
					vbo.Add (vert);
					//Top Right
					vert.color = centerColor;
					vert.position = L_TL;
					vbo.Add (vert);
					//Bottom Right
					vert.color = centerColor;
					vert.position = L_BL;
					vbo.Add (vert);
					//Bottom Left
					vert.color = edgeColor;
					vert.position = BL;
					vbo.Add (vert);
				
					if (shouldGlow) {
						//// GLOW ////
						Color fadedGlowColor = glowColor;
						fadedGlowColor.a = centerColor.a;
						Color alphaGlowColor = fadedGlowColor;
						alphaGlowColor.a = 0;
					
						//Glow Bottom
					
						//Top Left
						vert.color = fadedGlowColor;
						vert.position = L_BL;
						vbo.Add (vert);
						//Top Right
						vert.color = alphaGlowColor;
						vert.color.a = (byte)edgeColor.a;
						vert.position = L_BR;
						vbo.Add (vert);
						//Bottom Right
						vert.color = alphaGlowColor;
						vert.position = L_BR;
						vbo.Add (vert);
						//Bottom Left
						vert.color = alphaGlowColor;
						vert.color.a = (byte)edgeColor.a;
						vert.position = L_BL - Vector3.left * glowWidth;
						vbo.Add (vert);
						//Glow Top
					
						//Top Left
						vert.color = alphaGlowColor;
						vert.position = L_TL + Vector3.left * glowWidth;
						vbo.Add (vert);
						//Top Right
						vert.color = alphaGlowColor;
						vert.color.a = (byte)edgeColor.a;
						vert.position = L_TR;
						vbo.Add (vert);
						//Bottom Right
						vert.color = fadedGlowColor;
						vert.color.a = (byte)edgeColor.a;
						vert.position = L_TR;
						vbo.Add (vert);
						//Bottom Left
						vert.color = fadedGlowColor;
						vert.position = L_TL;
						vbo.Add (vert);
					}
				
					////// CENTER ///////
				
					vert.color = centerColor;
					Vector3 C_TL = L_TL;
					Vector3 C_TR = L_TL;
					C_TR.y *= -1f;
					Vector3 C_BR = L_BL;
					C_BR.y *= -1f;
					Vector3 C_BL = L_BL;


					//Top Left
					vert.position = C_TL;
					vbo.Add (vert);
					//Top Right
					vert.position = C_TR;
					vbo.Add (vert);
					//Bottom Right
					vert.position = C_BR;
					vbo.Add (vert);
					//Bottom Left
					vert.position = C_BL;
					vbo.Add (vert);

					if (shouldGlow) {
						//// GLOW ////
						Color fadedGlowColor = glowColor;
						fadedGlowColor.a = centerColor.a;
						Color alphaGlowColor = fadedGlowColor;
						alphaGlowColor.a = 0;

						//Glow Bottom

						//Top Left
						vert.color = fadedGlowColor;
						vert.position = C_BL;
						vbo.Add (vert);
						//Top Right
						vert.color = fadedGlowColor;
						vert.position = C_BR;
						vbo.Add (vert);
						//Bottom Right
						vert.color = alphaGlowColor;
						vert.position = C_BR;
						vert.position -= Vector3.left * glowWidth;
						vbo.Add (vert);
						//Bottom Left
						vert.color = alphaGlowColor;
						vert.position = C_BL - Vector3.left * glowWidth;
						vbo.Add (vert);
				
						//Glow Top
				
						//Top Left
						vert.color = alphaGlowColor;
						vert.position = C_TL + Vector3.left * glowWidth;
						vbo.Add (vert);
						//Top Right
						vert.color = alphaGlowColor;
						vert.position = C_TR;
						vert.position += Vector3.left * glowWidth;
						vbo.Add (vert);
						//Bottom Right
						vert.color = fadedGlowColor;
						vert.position = C_TR;
						vbo.Add (vert);
						//Bottom Left
						vert.color = fadedGlowColor;
						vert.position = C_TL;
						vbo.Add (vert);
					}

					////// RIGHT FADE ///////
					Vector3 R_BR = BR;
					Vector3 R_TL = L_TL;
					R_TL.y *= -1f;
					Vector3 R_TR = TR;
					Vector3 R_BL = L_BL;
					R_BL.y *= -1f;
					//Make the edge pointy by averaging the X of both endpoints
//				R_TR.x=(R_TR.x+R_BR.x)/2f;
//				R_BR.x=R_TR.x;

					//Top Left
					vert.color = centerColor;
					vert.position = R_TL;
					vbo.Add (vert);
					//Top Right
					vert.color = edgeColor;
					vert.position = R_TR;
					vbo.Add (vert);
					//Bottom Right
					vert.color = edgeColor;
					vert.position = R_BR;
					vbo.Add (vert);
					//Bottom Left
					vert.color = centerColor;
					vert.position = R_BL;
					vbo.Add (vert);

					if (shouldGlow) {
						//// GLOW ////
						Color fadedGlowColor = glowColor;
						fadedGlowColor.a = centerColor.a;
						Color alphaGlowColor = fadedGlowColor;
						alphaGlowColor.a = 0;
					
						//Glow Bottom
					
						//Top Left
						vert.color = fadedGlowColor;
						vert.position = R_BL;
						vbo.Add (vert);
						//Top Right
						vert.color = alphaGlowColor;
						vert.color.a = (byte)edgeColor.a;
						vert.position = R_BR;
						vbo.Add (vert);
						//Bottom Right
						vert.color = alphaGlowColor;
						vert.position = R_BR;
						vbo.Add (vert);
						//Bottom Left
						vert.color = alphaGlowColor;
						vert.color.a = (byte)edgeColor.a;
						vert.position = R_BL - Vector3.left * glowWidth;
						vbo.Add (vert);
						//Glow Top
					
						//Top Left
						vert.color = alphaGlowColor;
						vert.position = R_TL + Vector3.left * glowWidth;
						vbo.Add (vert);
						//Top Right
						vert.color = alphaGlowColor;
						vert.color.a = (byte)edgeColor.a;
						vert.position = R_TR;
						vbo.Add (vert);
						//Bottom Right
						vert.color = fadedGlowColor;
						vert.color.a = (byte)edgeColor.a;
						vert.position = R_TR;
						vbo.Add (vert);
						//Bottom Left
						vert.color = fadedGlowColor;
						vert.position = R_TL;
						vbo.Add (vert);
					}
				
				} else {
					vert.color = color;
					vert.position = TL;
					vbo.Add (vert);
					vert.position = TR;
					vbo.Add (vert);
					vert.position = BR;
					vbo.Add (vert);
					vert.position = BL;
					vbo.Add (vert);


				
					if (shouldGlow) {
						//Glow Bottom
				
						//Top Left
						vert.color = glowColor;
						vert.position = BL;
						vbo.Add (vert);
						//Top Right
						vert.color = glowColor;
						vert.position = BR;
						vbo.Add (vert);
						//Bottom Right
						vert.color = glowColor;
						vert.color.a = 0;
						vert.position = BR - Vector3.left * glowWidth;
						vbo.Add (vert);
						//Bottom Left
						vert.color = glowColor;
						vert.color.a = 0;
						vert.position = BL - Vector3.left * glowWidth;
						vbo.Add (vert);
				
						//Glow Top
				
						//Top Left
						vert.color = glowColor;
						vert.color.a = 0;
						vert.position = TL + Vector3.left * glowWidth;
						vbo.Add (vert);
						//Top Right
						vert.color = glowColor;
						vert.color.a = 0;
						vert.position = TR + Vector3.left * glowWidth;
						vbo.Add (vert);
						//Bottom Right
						vert.color = glowColor;
						vert.position = TR;
						vbo.Add (vert);
						//Bottom Left
						vert.color = glowColor;
						vert.position = TL;
						vbo.Add (vert);
					}
				}
			}
			#endregion
			#endregion
		}
		ReleaseQuads (vbo);

	}

	public bool drawCircles = false;
	public float circleRadius = .25f;
	[Range(1,10)]
	public int
		circleSubdivisions = 3;
	Vector3 prevX2, prevY2;
	float zValue = 0;
	
	void DrawCircle (Vector3 center, Color color)
	{
		float outer = 7.5f * circleRadius;
		float inner = 0;
		Vector3 circle_TL = center;
		
		int adjustedCircleSubdivisions = UIUtility.GetAdjustedSubdivisions (circleSubdivisions);
		for (int i=0; i<361; i+=adjustedCircleSubdivisions) {
			//position
			float rad = Mathf.Deg2Rad * (i);
			float c = Mathf.Cos (rad);
			float s = Mathf.Sin (rad);
			Vector3 innerPosition = new Vector3 (inner * c, inner * s, zValue);
			Vector3 outerPosition = new Vector3 (outer * c, outer * s, zValue);
			innerPosition += circle_TL;
			outerPosition += circle_TL;
			//Vertices
			Vector3[] positions = new Vector3[4];
			Color[] colors = new Color[]{color,color,color,color};
			//Outer
			positions [0] = prevX2;
			if (i == 0)
				positions [0] = outerPosition;
			prevX2 = outerPosition;
			positions [1] = prevX2;
			//Inner
			positions [2] = innerPosition;
			positions [3] = prevY2;
			if (i == 0)
				positions [3] = innerPosition;
			prevY2 = innerPosition;
			
			StoreQuad (positions, colors, false);
		}
	}

	public bool drawSquares = false;
	public float squareSize = 3f;
	
	protected void DrawSquare (Vector3 center, Color color)
	{
	
		float rectWidth = squareSize;
		float rectHeight = squareSize;
			
		float xDistance = rectWidth;
		float yDistance = rectHeight;
		Vector3 xDirection = Vector3.right;
		Vector3 yDirection = Vector3.up;
		Vector3[] positions;
		Color[] colors;
			
			
		Vector3 rect_TL = -xDistance * xDirection + yDistance * yDirection + center;
		Vector3 rect_TR = xDistance * xDirection + yDistance * yDirection + center;
		Vector3 rect_BR = xDistance * xDirection + -yDistance * yDirection + center;
		Vector3 rect_BL = -xDistance * xDirection + -yDistance * yDirection + center;
		
		positions = new Vector3[]{rect_TL,rect_TR,rect_BR,rect_BL};
		colors = new Color[]{color,color,color,color};
			
		StoreQuad (positions, colors, false);
		
	}
	
	protected List<Vector3> storedPositions;
	protected List<Color> storedColors;
	
	void StoreQuad (Vector3[] positions, Color[] colors, bool shouldScale)
	{
		for (int i=0; i<4; ++i) {
			Vector3 positionToStore = positions [i];
			if (shouldScale)
				positionToStore = Vector3.Scale (positionToStore, new Vector3 (1f / transform.lossyScale.x, 1f / transform.lossyScale.y, 0));
			storedPositions.Add (positionToStore);
			storedColors.Add (colors [i]);
		}
	}
	
	void ReleaseQuads (List<UIVertex> vbo)
	{
		UIVertex vert = UIVertex.simpleVert;
		for (int i=0; i<storedPositions.Count; ++i) {
			vert.position = storedPositions [i];
			vert.color = storedColors [i];
			vbo.Add (vert);
		}
	}

	void DrawRect (List<UIVertex> vbo, Vector3 TL, Vector3 TR, Vector3 BL, Vector3 BR, Color vertColor)
	{
		UIVertex vert = UIVertex.simpleVert;
		vert.color = vertColor;

		vert.position = TL;
		vbo.Add (vert);
		vert.position = TR;
		vbo.Add (vert);
		vert.position = BR;
		vbo.Add (vert);
		vert.position = BL;
		vbo.Add (vert);
	}	
}
}