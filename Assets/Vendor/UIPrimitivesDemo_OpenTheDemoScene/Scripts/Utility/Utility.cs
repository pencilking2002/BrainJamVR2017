using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;
using System;

namespace VRB.Utility {
	public class Utility : MonoBehaviour {


		public static Color GetColorFromTexture(Texture2D texture, float saturation = .65f, float value = 1f) {
			Color[] pixels = texture.GetPixels();
			Vector3 averageColorValues = Vector3.zero;

			int interval = 100000;
			for (int i = 0; i < pixels.Length; i += interval) {
				averageColorValues += new Vector3(pixels[i].r, pixels[i].g, pixels[i].b);
			}

			averageColorValues /= (pixels.Length / interval);

			Color averageColor = new Color(averageColorValues.x, averageColorValues.y, averageColorValues.z);

			float h, s, v;
			Color.RGBToHSV(averageColor, out h, out s, out v);

			s = saturation;
			v = value;

			averageColor = Color.HSVToRGB(h, s, v);

			return averageColor;
		}


		public static Color CreateColor255(float red, float green, float blue) {
			return new Color(red / 255f, green / 255f, blue / 255f);
		}

		public static Color CreateColor255(int red, int green, int blue) {
			return new Color(((float) red) / 255f, ((float) green) / 255f, ((float) blue) / 255f);
		}

		public static Vector2 GetRandomVector2(float range) {
			return new Vector2(UnityEngine.Random.Range(-range, range), UnityEngine.Random.Range(-range, range));
		}

		public static Vector3 GetRandomVector3(float range) {
			return new Vector3(UnityEngine.Random.Range(-range, range), UnityEngine.Random.Range(-range, range), UnityEngine.Random.Range(-range, range));
		}

		public static Vector3 Interp(Vector3[] pts, float t) {
			int numSections = pts.Length - 3;
			int currPt = Mathf.Min(Mathf.FloorToInt(t * (float) numSections), numSections - 1);
			float u = t * (float) numSections - (float) currPt;
		
			Vector3 a = pts[currPt];
			Vector3 b = pts[currPt + 1];
			Vector3 c = pts[currPt + 2];
			Vector3 d = pts[currPt + 3];
		
			return .5f * (
			     (-a + 3f * b - 3f * c + d) * (u * u * u)
			     + (2f * a - 5f * b + 4f * c - d) * (u * u)
			     + (-a + c) * u
			     + 2f * b
			);
		}

		public static Vector3[] PathControlPointGenerator(Vector3[] path) {
			Vector3[] suppliedPath;
			Vector3[] vector3s;
		
			//create and store path points:
			suppliedPath = path;
		
			//populate calculate path;
			int offset = 2;
			vector3s = new Vector3[suppliedPath.Length + offset];
			System.Array.Copy(suppliedPath, 0, vector3s, 1, suppliedPath.Length);
		
			//populate start and end control points:
			//vector3s[0] = vector3s[1] - vector3s[2];
			vector3s[0] = vector3s[1] + (vector3s[1] - vector3s[2]);
			vector3s[vector3s.Length - 1] = vector3s[vector3s.Length - 2] + (vector3s[vector3s.Length - 2] - vector3s[vector3s.Length - 3]);
		
			//is this a closed, continuous loop? yes? well then so let's make a continuous Catmull-Rom spline!
			if (vector3s[1] == vector3s[vector3s.Length - 2]) {
				Vector3[] tmpLoopSpline = new Vector3[vector3s.Length];
				System.Array.Copy(vector3s, tmpLoopSpline, vector3s.Length);
				tmpLoopSpline[0] = tmpLoopSpline[tmpLoopSpline.Length - 3];
				tmpLoopSpline[tmpLoopSpline.Length - 1] = tmpLoopSpline[2];
				vector3s = new Vector3[tmpLoopSpline.Length];
				System.Array.Copy(tmpLoopSpline, vector3s, tmpLoopSpline.Length);
			}	
		
			return(vector3s);
		}

		public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles) {
			Vector3 dir = point - pivot; // get point direction relative to pivot
			dir = Quaternion.Euler(angles) * dir; // rotate it
			point = dir + pivot; // calculate rotated point
			return point; // return it
		}

		public static Color GetRandomColor() {
			return new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
		}

		public static AnimationCurve GetLinearCurve(float duration) {
			return AnimationCurve.Linear(Time.time, 0, Time.time + duration, 1f);
		}

		public static AnimationCurve GetEaseInOutCurve(float duration) {
			return AnimationCurve.EaseInOut(Time.time, 0, Time.time + duration, 1f);
		}


		public static string Truncate(string source, int length) {
			if (source == null)
				return null;

			if (source.Length > length) {
				int index = source.Substring(0, length).LastIndexOf(' ');
				if (index == -1) {
					index = length - 1;
				}
				source = source.Insert(index, "\n");

				if (source.Substring(index).Length > length) {
					source = source.Substring(0, index + length) + "...";
				}
			}
			return source;
		}

		public static string TruncateFromBack(string source, int length) {
			if (source == null)
				return null;

			int sourceLength = source.Length;

			if (sourceLength > length) {
				source = "..." + source.Substring(sourceLength - (length), length);
			}
			return source;
		}

		public static Regex passrgx = new Regex(@"(.)\1{2,}", RegexOptions.None);
		public static Regex userrgx = new Regex(@"[^a-zA-Z0-9-_.]", RegexOptions.None);
		public static Regex emailrgx = new Regex(@"\A[a-z0-9!#$%&'*+\/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+\/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\z", RegexOptions.IgnoreCase);

		public static int GetUnixTimestamp(DateTime date) {
			return (int) (date.ToUniversalTime().Subtract(new DateTime(1970, 1, 1).ToUniversalTime())).TotalSeconds;
		}

		public static int GetUnixTimestampNow() {
			return (int) (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
		}

		public static DateTime GetDateTimeFromUnixTimestamp(int timestamp) {
			// Unix timestamp is seconds past epoch
			DateTime dateTime = new DateTime(1970,1,1,0,0,0,0,System.DateTimeKind.Utc);
			dateTime = dateTime.AddSeconds( timestamp ).ToUniversalTime();
			return dateTime;
		}

		public static bool ValidatePassword(string pass) {
			return ValidatePassword(pass, "dfsdlkjfldjsljf", "dfsldkjflsjfls@dlkfjsld.com");
		}

		public static bool ValidatePassword(string pass, string username, string email) {
			if (username == null)
				username = "";
			if (email == null)
				email = "";
			if (pass == null || pass.Length == 0)
				return false;
			if (pass.Contains(username) || pass.Contains(email)) { //cannot contain the username or password
				return false;
			}
			if (pass.Length < 6) { //has to be at least 6 chars
				return false;
			}
			if (passrgx.IsMatch(pass)) { //don't allow more than 2 repeating chars
				return false;
			}
			return true;
		}

		public static bool ValidateEmail(string email) {
			if (email.Length < 64 && emailrgx.IsMatch(email)) { //valid email address, less than 64 chars
				return true;
			}
			return false;
		}

		public static bool ValidateUsername(string username) {
			if (username.Length < 2) { //has to be at least 2 chars
				return false;
			}
			if (userrgx.IsMatch(username)) { //only allow numbers, letters, underscore, dash, and dot
				return false;
			}
			return true;
		}

		public static string GetFormattedTimeAgo(long timestamp) {
			DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
			dt = dt.AddSeconds(timestamp).ToLocalTime();
			if (dt > DateTime.Now)
				return "Just now";
			TimeSpan span = DateTime.Now - dt;

			if (span.Days > 365) {
				int years = (span.Days / 365);
				if (span.Days % 365 != 0)
					years += 1;
				return String.Format("about {0} {1} ago", years, years == 1 ? "year" : "years");
			}

			if (span.Days > 30) {
				int months = (span.Days / 30);
				if (span.Days % 31 != 0)
					months += 1;
				return String.Format("{0} {1} ago", months, months == 1 ? "month" : "months");
			}

			if (span.Days > 0)
				return String.Format("{0} {1} ago", span.Days, span.Days == 1 ? "day" : "days");

			if (span.Hours > 0)
				return String.Format("{0} {1} ago", span.Hours, span.Hours == 1 ? "hour" : "hours");

			if (span.Minutes > 0)
				return String.Format("{0} {1} ago", span.Minutes, span.Minutes == 1 ? "minute" : "minutes");

			if (span.Seconds > 5)
				return String.Format("{0} seconds ago", span.Seconds);

			if (span.Seconds <= 5)
				return "just now";

			return string.Empty;
		}

		public static string GetFormattedTimeAgo(DateTime dt) {
			if (dt > DateTime.Now)
				return "Just now";
			TimeSpan span = DateTime.Now - dt;

			if (span.Days > 365) {
				int years = (span.Days / 365);
				if (span.Days % 365 != 0)
					years += 1;
				return String.Format("about {0} {1} ago", years, years == 1 ? "year" : "years");
			}

			if (span.Days > 30) {
				int months = (span.Days / 30);
				if (span.Days % 31 != 0)
					months += 1;
				return String.Format("{0} {1} ago", months, months == 1 ? "month" : "months");
			}

			if (span.Days > 0)
				return String.Format("{0} {1} ago", span.Days, span.Days == 1 ? "day" : "days");

			if (span.Hours > 0)
				return String.Format("{0} {1} ago", span.Hours, span.Hours == 1 ? "hour" : "hours");

			if (span.Minutes > 0)
				return String.Format("{0} {1} ago", span.Minutes, span.Minutes == 1 ? "minute" : "minutes");

			if (span.Seconds > 5)
				return String.Format("{0} seconds ago", span.Seconds);

			if (span.Seconds <= 5)
				return "just now";

			return string.Empty;
		}
	}
}