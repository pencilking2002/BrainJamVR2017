using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UIPrimitives;

namespace UIPrimitives
{
/// <summary>
/// uGUI Circle Primitive
/// Uses RectTransform's scale, rotation, and pivot values.
/// Ignores RectTransform's width/height values, use radius instead. 
/// </summary>
	public class UICircle :  UIFillableGraphic
	{	

		//////Dimensions//////
		//Radius
		[Tooltip("Radius of circle.")]
		public float
			radius = 100;
		//Angle
		[Tooltip("Angle of circle in degrees.")]
		[Range(0,360)]
		public int
			angle = 360;
		//AngleOffset
		[Tooltip("Starting offset for angle in degrees.")]
		[Range(0.0F,360.0F)]
		public int
			angleOffset = 0;
		//RepeatInterval
		[Tooltip("Angle in degrees between repititions.")]
		[Range(2,360)]
		public int
			repeatInterval = 360;
		//Repetitions
		[Tooltip("Maximum amount of repititions.")]
		[Range(0,180)]
		public int
			repetitions = 180;
		//Thickness
		[Tooltip("Percent of how filled in the circle is.")]
		[Range(0.0F,1.0F)]
		public float
			thickness = 1F;
		//Subdivisions
		[Tooltip("Tiered level of subdivisions, increase if you see banding.")]
		[Range(1,21)]
		public int
			subdivisions = 13;

		//////Appearance//////
		//UseGradient
		[Tooltip("Use gradient coloring.")]
		public bool
			useGradient;
		//GradientStyle
		[Tooltip("Gradient calculation style, Radial or Angle.")]
		public GradientStyleCircle
			gradientStyle;
		//Gradient
		[Tooltip("Gradient to use, only uses front and end colors.")]
		public Gradient
			gradient;
		//GradientStretch
		[Tooltip("Stretch gradient to fit the circle.")]
		public bool
			gradientStretch;
		//WorldSpace
		[Tooltip("Gradient will be rendered in world space, ignoring circle rotation.")]
		public bool
			worldSpace;
		//GradientStyleCircle
		public enum GradientStyleCircle
		{
			Radial,//From center to outside
			Angle//Clockwise from start of circle
		}

		//////Dots//////
		//DrawDots
		[Tooltip("Draw dots instead of circle.")]
		public bool
			drawDots = false;
		//DotRadius
		[Tooltip("Radius of each dot.")]
		public float
			dotRadius = .25f;
		//DotSubdivisions
		[Tooltip("Subdivisions for each dot.")]
		[Range(1,10)]
		public int
			dotSubdivisions = 3;

		//////Glow//////
		//ShouldGlow
		[Tooltip("Add a glow to the circle.")]
		public bool
			shouldGlow = false;
		//UseCircleColor
		[Tooltip("Use circle color for the glow.")]
		public bool
			useCircleColor = true;
		//GlowColor
		[Tooltip("Color for the circle's glow.")]
		public Color
			glowColor = Color.white;
		//GlowDistance
		[Tooltip("Distance from the edge of the circle for the glow.")]
		[Range(0.001F,.25F)]
		public float
			glowDistance = .1f;

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

			//Draw Main Circle
			float radiusInside = (1f - thickness) * radius;
			float radiusOutside = radius;
			DrawCircle (vbo, radiusInside, radiusOutside, CircleType.Main);
			//Draw Glow
			if (shouldGlow && !drawDots) {
				//Draw Outer Glow
				float glowRadiusOutside = radiusOutside + radius * glowDistance;
				DrawCircle (vbo, radiusOutside, glowRadiusOutside, CircleType.GlowOuter);
				//Draw Inner Glow
				if (thickness != 1f) {
					float glowRadiusInside = radiusInside - radius * glowDistance;
					DrawCircle (vbo, glowRadiusInside, radiusInside, CircleType.GlowInner);
				}
			}
		}

		///////////////////////////////////////////////////////////////////////////
		//
		// UICircle Functions
		//

		/// <summary>
		/// Draws the circle, outer glow, and inner glow.
		/// </summary>
		/// <param name="vbo">Vertex Buffer Object.</param>
		/// <param name="radiusInside">Radius inside.</param>
		/// <param name="radiusOutside">Radius outside.</param>
		/// <param name="circleType">Circle type.</param>
		protected void DrawCircle (List<UIVertex> vbo, float radiusInside, float radiusOutside, CircleType circleType)
		{
			//Calculate the actual amount of subdivisions
			int adjustedSubdivisions = UIUtility.GetAdjustedSubdivisions (subdivisions);
			//Positon
			Vector3 prevX = Vector3.zero;
			Vector3 prevY = Vector3.zero;
			Vector3 center = Vector2.Scale ((rectTransform.pivot - (Vector2.one * .5f)), rectTransform.sizeDelta);
			//Angle
			float anglePercent = ((float)angle) / 360f;
			bool resetPositions = true;
			//Circle	
			for (int i=0; i<361; i+=adjustedSubdivisions) {
				float absolutePercent = ((float)i) / 360f;
				float absolutePercentNext = ((float)(i+adjustedSubdivisions)) / 360f;
				//Adjust for world space coloring
				if (worldSpace)
					absolutePercent = ((((float)i) + angleOffset) % 359) / 360f;
				float relativePercent = absolutePercent / anglePercent;
				float relativePercentNext = absolutePercentNext / anglePercent;

				//Check if we maxed out our repetitions
				if (i >= ((angle + repeatInterval) * repetitions))
					break;
				//Check if it is within our angle limit
				int remainder = i % (angle + repeatInterval);
				if (remainder <= angle) {
					//Set the percent relative to the piece of the circle we are drawing
					if (repeatInterval != 360) {
						relativePercent = ((float)remainder) / angle;
					}
				} else {
					//If it should not be drawn we want to reset the prevX/prevY variables on the next draw
					resetPositions = true;
					continue;
				} 
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
				CircleColors circleColors = GetCircleColors (circleType, absolutePercent, relativePercent);
				CircleColors circleColorsNext = GetCircleColors (circleType, absolutePercentNext, relativePercentNext);
				//Draw
				if (drawDots) {//Draw the dots
					DrawDot (vbo, outerPosition, circleColors.outerColor,dotRadius,dotSubdivisions);
				} else {
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
						circleColorsNext.outerColor,
						circleColorsNext.innerColor
				};
					//Actually add the quads to the VBO for drawing
					AddQuad (vbo, positions, colors);
					//Save values for next quad
					prevX = innerPosition;
					prevY = outerPosition;
				}
			}
		}

		/// <summary>
		/// Gets the circle colors.
		/// </summary>
		/// <returns>The circle colors.</returns>
		/// <param name="circleType">Circle type.</param>
		/// <param name="absolutePercent">Absolute percent.</param>
		/// <param name="relativePercent">Relative percent.</param>
		protected CircleColors GetCircleColors (CircleType circleType, float absolutePercent, float relativePercent)
		{
			CircleColors circleColors;
			//Determine what part of the circle we are drawing
			switch (circleType) {
			case CircleType.Main://Body of the circle
				circleColors.outerColor = GetOuterColor (absolutePercent, relativePercent);
				circleColors.innerColor = GetInnerColor (absolutePercent, relativePercent);
				break;
			case CircleType.GlowOuter://Outer glow of the circle
				if (useCircleColor) {
					circleColors.innerColor = GetInnerColor (absolutePercent, relativePercent);
				} else {
					circleColors.innerColor = glowColor;
					circleColors.innerColor.a = color.a * glowColor.a;
				}
				circleColors.outerColor = circleColors.innerColor;
				circleColors.outerColor.a = 0;
				break;
			case CircleType.GlowInner://Inner glow of the circle
				if (useCircleColor) {
					circleColors.outerColor = GetOuterColor (absolutePercent, relativePercent);
				} else {
					circleColors.outerColor = glowColor;
					circleColors.outerColor.a = color.a * glowColor.a;
				}
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
		
		/// <summary>
		/// Gets the color of the inner part of the circle.
		/// </summary>
		/// <returns>The inner color.</returns>
		/// <param name="absolutePercent">Absolute percent.</param>
		/// <param name="relativePercent">Relative percent.</param>
		protected Color GetInnerColor (float absolutePercent, float relativePercent)
		{
			if (useGradient) {
				if (gradientStyle == GradientStyleCircle.Angle) {
					if (gradientStretch) {
						return gradient.Evaluate (relativePercent);
					} else {
						return gradient.Evaluate (absolutePercent);
					}
				} else if (gradientStyle == GradientStyleCircle.Radial) {
					return gradient.Evaluate (1f);
				}
			} else {
				return color;
			}
			return Color.white;
		}

		/// <summary>
		/// Gets the color of the outer part of the circle.
		/// </summary>
		/// <returns>The outer color.</returns>
		/// <param name="absolutePercent">Absolute percent.</param>
		/// <param name="relativePercent">Relative percent.</param>
		protected Color GetOuterColor (float absolutePercent, float relativePercent)
		{
			if (useGradient) {
				if (gradientStyle == GradientStyleCircle.Angle) {
					if (gradientStretch) {
						return gradient.Evaluate (relativePercent);
					} else {
						return gradient.Evaluate (absolutePercent);
					}
				} else if (gradientStyle == GradientStyleCircle.Radial) {
					return gradient.Evaluate (0);
				}
			} else {
				return  color;
			}
			return Color.white;
		}

		#if UNITY_EDITOR
		[UnityEditor.MenuItem("GameObject/UIPrimitives/UICircle",false, 10)]
		public static void CreateUICircle()
		{
			CreateObject<UICircle>("UICircle");
		}
		#endif

		///////////////////////////////////////////////////////////////////////////
		//
		// Inherited from UIFillable Graphic
		//

		public override void SetFillPercent(float percent) {

			this.angle =(int) (percent * 360f);
			this.SetAllDirty ();
		}
	}
}