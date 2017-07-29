using UnityEngine;
using System.Collections;

/// <summary>
/// Utility class for UIPrimitives
/// </summary>
public class UIUtility {

	/// <summary>
	/// Catmull-Rom interpolation between points for percent t
	/// </summary>
	/// <param name="pts">Pts.</param>
	/// <param name="t">T.</param>
	public static Vector2 Interp(Vector2[] pts, float percent){
		int numSections = pts.Length - 3;
		int currPt = Mathf.Min(Mathf.FloorToInt(percent * (float) numSections), numSections - 1);
		float u = percent * (float) numSections - (float) currPt;
		
		Vector2 a = pts[currPt];
		Vector2 b = pts[currPt + 1];
		Vector2 c = pts[currPt + 2];
		Vector2 d = pts[currPt + 3];
		
		return .5f * (
			(-a + 3f * b - 3f * c + d) * (u * u * u)
			+ (2f * a - 5f * b + 4f * c - d) * (u * u)
			+ (-a + c) * u
			+ 2f * b
			);
	}

	/// <summary>
	/// Helper function to normalize a list of points
	/// </summary>
	/// <returns>The control point generator.</returns>
	/// <param name="path">Path.</param>
	public static Vector2[] PathControlPointGenerator(Vector2[] path){
		Vector2[] suppliedPath;
		Vector2[] vector3s;
		
		//create and store path points:
		suppliedPath = path;
		
		//populate calculate path;
		int offset = 2;
		vector3s = new Vector2[suppliedPath.Length+offset];
		System.Array.Copy(suppliedPath,0,vector3s,1,suppliedPath.Length);
		
		//populate start and end control points:
		//vector3s[0] = vector3s[1] - vector3s[2];
		vector3s[0] = vector3s[1] + (vector3s[1] - vector3s[2]);
		vector3s[vector3s.Length-1] = vector3s[vector3s.Length-2] + (vector3s[vector3s.Length-2] - vector3s[vector3s.Length-3]);
		
		//is this a closed, continuous loop? yes? well then so let's make a continuous Catmull-Rom spline!
		if(vector3s[1] == vector3s[vector3s.Length-2]){
			Vector2[] tmpLoopSpline = new Vector2[vector3s.Length];
			System.Array.Copy(vector3s,tmpLoopSpline,vector3s.Length);
			tmpLoopSpline[0]=tmpLoopSpline[tmpLoopSpline.Length-3];
			tmpLoopSpline[tmpLoopSpline.Length-1]=tmpLoopSpline[2];
			vector3s=new Vector2[tmpLoopSpline.Length];
			System.Array.Copy(tmpLoopSpline,vector3s,tmpLoopSpline.Length);
		}	
		
		return(vector3s);
	}

	//Subdivisions that are factors of 360 
	public static readonly int[] CIRCLE_SUBDIVISIONS = new int[]{1,2,3,4,5,6,8,9,10,12,15,18,20,24,30,36,40,45,60,72,90,120};
	public static readonly int CIRCLE_SUBDIVIONS_COUNT=CIRCLE_SUBDIVISIONS.Length;

	/// <summary>
	/// Gets the adjusted subdivisions.
	/// </summary>
	/// <returns>The adjusted subdivisions.</returns>
	/// <param name="subdivisions">Subdivisions.</param>
	public static int GetAdjustedSubdivisions(int subdivisions){
		return UIUtility.CIRCLE_SUBDIVISIONS [UIUtility.CIRCLE_SUBDIVIONS_COUNT - subdivisions];
	}
}
