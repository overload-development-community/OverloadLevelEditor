/*
THE COMPUTER CODE CONTAINED HEREIN IS THE SOLE PROPERTY OF REVIVAL
PRODUCTIONS, LLC ("REVIVAL").  REVIVAL, IN DISTRIBUTING THE CODE TO
END-USERS, AND SUBJECT TO ALL OF THE TERMS AND CONDITIONS HEREIN, GRANTS A
ROYALTY-FREE, PERPETUAL LICENSE TO SUCH END-USERS FOR USE BY SUCH END-USERS
IN USING, DISPLAYING,  AND CREATING DERIVATIVE WORKS THEREOF, SO LONG AS
SUCH USE, DISPLAY OR CREATION IS FOR NON-COMMERCIAL, ROYALTY OR REVENUE
FREE PURPOSES.  IN NO EVENT SHALL THE END-USER USE THE COMPUTER CODE
CONTAINED HEREIN FOR REVENUE-BEARING PURPOSES.  THE END-USER UNDERSTANDS
AND AGREES TO THE TERMS HEREIN AND ACCEPTS THE SAME BY USE OF THIS FILE.  
COPYRIGHT 2015-2020 REVIVAL PRODUCTIONS, LLC.  ALL RIGHTS RESERVED.
*/

using System;
using OpenTK;
using OpenTK.Graphics;
using System.IO;
using Newtonsoft.Json.Linq;

// UTILITY
// Editor-specific utility functions
// (see also UtilityGame)

#pragma warning disable 1690 // warning CS1690: Accessing a member on 'OverloadLevelEditor.GLView.control_sz' may cause a runtime exception because it is a field of a marshal-by-reference class

namespace OverloadLevelEditor
{
	public enum DecalAlign
	{
		TEXTURE,
		CENTER,
		EDGE_RIGHT,
		EDGE_DOWN,
		EDGE_LEFT,
		EDGE_UP,

		NUM,
	}

	public enum DecalMirror
	{
		OFF,
		MIRROR_U,
		MIRROR_V,

		NUM,
	}

	public enum DecalClip
	{
		NONE, // Cannot use CAP (all others can)
		WHOLE,
		PERP,
		SEGMENT,
		AVERAGE,
		ADJACENT,

		NUM,
	}

	public enum EdgeOrder
	{
		RIGHT,
		DOWN,
		LEFT,
		UP,

		NUM,
	}


	public enum SideOrder
	{
		// Left (7623)
		LEFT,
		// Top (0473)
		TOP,
		// Right (0154)
		RIGHT,
		// Bottom (2651)
		BOTTOM,
		// Front (3210)
		FRONT,
		// Back (4567)
		BACK,
	}

	public static partial class Utility
	{
		// FROM UTILITY GAME
		public static void DebugPopup(string s, string s2 = "Warning")
		{
			System.Windows.Forms.MessageBox.Show(s, s2);
			DebugLog(s);
		}

		public static void DebugLog(string s)
		{
			System.Diagnostics.Debug.WriteLine(s);
		}

		public class HermiteCurveEvaluator
		{
			// start_pt       : The starting position of the curve (the value at t=0.0f)
			// start_velocity : The direction and speed the curve should have coming out from the start_pt. This is the direction the
			//                  curve will initially start moving towards until it advances
			// end_pt         : The ending position of the curve (the value at t=1.0f)
			// end_velocity   : The direction and speed the curve should end up with when it reach end_pt. If you were to attach two
			//                  hermite curves end-to-start, for a continuous curve this would be the start_velocity of the next segment
			public HermiteCurveEvaluator(Vector3 start_pt, Vector3 start_velocity, Vector3 end_pt, Vector3 end_velocity)
			{
				this.StartPt = start_pt;
				this.EndPt = end_pt;
				this.StartVelocity = start_velocity;
				this.EndVelocity = end_velocity;
			}

			public Vector3 Eval(float t)
			{
				if (t <= 0.0f) {
					return this.StartPt;
				}
				if (t >= 1.0f) {
					return this.EndPt;
				}

				float t2 = t * t;
				float t3 = t2 * t;

				float h1 = 2.0f * t3 - 3 * t2 + 1.0f;
				float h2 = -2.0f * t3 + 3 * t2;
				float h3 = t3 - 2.0f * t2 + t;
				float h4 = t3 - t2;

				return (this.StartPt * h1) + (this.EndPt * h2) + (this.StartVelocity * h3) + (this.EndVelocity * h4);
			}

			public float CalculateDistance(float t0, float t1)
			{
				if (t0 > t1) {
					// swap
					float temp = t0;
					t0 = t1;
					t1 = temp;
				}

				if (t1 <= 0.0f || t0 >= 1.0f) {
					return 0.0f;
				}

				if (t0 < 0.0f) {
					t0 = 0.0f;
				}
				if (t1 > 1.0f) {
					t1 = 1.0f;
				}

				// TODO: Always room for improvement here
				Vector3 last_pt = Eval(t0);
				int num_steps = 30;
				float delta_step = (t1 - t0) / (float)num_steps;
				float t = t0 + delta_step;
				float len = 0.0f;
				for (int i = 1; i < num_steps - 1; ++i) {
					Vector3 this_pt = Eval(t);
					t += delta_step;
					Vector3 delta_v = this_pt - last_pt;
					len += delta_v.Length;
					last_pt = this_pt;
				}
				Vector3 final_pt = Eval(t1);
				Vector3 final_delta_v = final_pt - last_pt;
				len += final_delta_v.Length;
				return len;
			}

			public float CalculateNextTByDistance(float from_t, float curve_dist, out float real_dist, out Vector3 new_t_pos)
			{
				real_dist = 0.0f;

				if (from_t < 0.0f) {
					from_t = 0.0f;
				} else if (from_t >= 1.0f) {
					new_t_pos = this.EndPt;
					return 1.0f;
				}

				if (curve_dist <= 0.0f) {
					new_t_pos = this.StartPt;
					return 0.0f;
				}

				// Note: This is a horrible way to do this, but I'm just needing something at the moment
				// so we'll just binary search for the next t
				float min_t = from_t;
				float max_t = 1.0f;
				float test_t = from_t;
				int num_iterations_left = 20;
				while (num_iterations_left >= 0) {
					--num_iterations_left;

					test_t = (min_t + max_t) * 0.5f;
					real_dist = CalculateDistance(from_t, test_t);
					if (Math.Abs(real_dist - curve_dist) <= 0.01f) {
						new_t_pos = Eval(test_t);
						return test_t;
					}
					if (real_dist < curve_dist) {
						// Didn't go far enough, set the lower bounds to this
						min_t = test_t;
					} else {
						// Went too far, set the upper bounds to this
						max_t = test_t;
					}
				}

				new_t_pos = Eval(test_t);
				return test_t;
			}

			public Vector3 EvalDerivative(float t)
			{
				// TODO: Replace this with a proper function?
				if (t <= 0.0f) {
					return this.StartVelocity;
				}
				if (t >= 1.0f) {
					return this.EndVelocity;
				}

				float dt = 0.001f;
				Vector3 at_t0 = Eval(t);
				Vector3 at_t1 = Eval(t + dt);
				return (at_t1 - at_t0) / dt;
			}

			public Vector3 StartPt { get; private set; }
			public Vector3 EndPt { get; private set; }
			public Vector3 StartVelocity { get; private set; }
			public Vector3 EndVelocity { get; private set; }
		}

		// Math helpers
		public const float PI = (float)Math.PI;
		public const float RAD_360 = PI * 2f;
		public const float RAD_180 = PI;
		public const float RAD_90 = PI / 2f;
		public const float RAD_60 = PI / 3f;
		public const float RAD_45 = PI / 4f;
		public const float RAD_30 = PI / 6f;
		public const float RAD_15 = PI / 12f;

		public static Vector3 V3Min(Vector3 v1, Vector3 v2)
		{
			Vector3 min;
			min.X = Math.Min(v1.X, v2.X);
			min.Y = Math.Min(v1.Y, v2.Y);
			min.Z = Math.Min(v1.Z, v2.Z);
			return min;
		}

		public static Vector3 V3Max(Vector3 v1, Vector3 v2)
		{
			Vector3 max;
			max.X = Math.Max(v1.X, v2.X);
			max.Y = Math.Max(v1.Y, v2.Y);
			max.Z = Math.Max(v1.Z, v2.Z);
			return max;
		}

		public static Vector2 V2Min(Vector2 v1, Vector2 v2)
		{
			Vector2 min;
			min.X = Math.Min(v1.X, v2.X);
			min.Y = Math.Min(v1.Y, v2.Y);
			return min;
		}

		public static Vector2 V2Max(Vector2 v1, Vector2 v2)
		{
			Vector2 max;
			max.X = Math.Max(v1.X, v2.X);
			max.Y = Math.Max(v1.Y, v2.Y);
			return max;
		}

		public static float AreaOfTriangle(Vector3 pt1, Vector3 pt2, Vector3 pt3)
		{
			float a = (pt1  - pt2).Length;
			float b = (pt2-pt3).Length;
			float c = (pt3-pt1).Length;
			float s = (a + b + c) / 2;
			return (float)Math.Sqrt(s * (s - a) * (s - b) * (s - c));
		}

		public static Vector3 FindNormal(Vector3 p1, Vector3 p2, Vector3 p3)
		{
			Vector3 normal = Vector3.Cross(p2 - p1, p3 - p1);
			return normal.Normalized();
		}

		public static float AlignSign(Vector2 p1, Vector2 p2, Vector2 p3)
		{
			return (p1.X - p3.X) * (p2.Y - p3.Y) - (p2.X - p3.X) * (p1.Y - p3.Y);
		}

		public static float AlignSign(Vector2 p1, Vector3 p2, Vector3 p3)
		{
			return (p1.X - p3.X) * (p2.Y - p3.Y) - (p2.X - p3.X) * (p1.Y - p3.Y);
		}

		// For testing a 3D-transformed triangle (Z doesn't matter)
		public static bool PointInsideTri(Vector2 pt, Vector3 v1, Vector3 v2, Vector3 v3)
		{
			bool b1, b2, b3;

			b1 = AlignSign(pt, v1, v2) < 0.0f;
			b2 = AlignSign(pt, v2, v3) < 0.0f;
			b3 = AlignSign(pt, v3, v1) < 0.0f;

			return ((b1 == b2) && (b2 == b3));
		}

		// The default 2D version
		public static bool PointInsideTri(Vector2 pt, Vector2 v1, Vector2 v2, Vector2 v3)
		{
			bool b1, b2, b3;

			b1 = AlignSign(pt, v1, v2) < 0.0f;
			b2 = AlignSign(pt, v2, v3) < 0.0f;
			b3 = AlignSign(pt, v3, v1) < 0.0f;

			return ((b1 == b2) && (b2 == b3));
		}

		public static Vector3 WorldToScreen(Vector3 pos, Matrix4 cam_mat, Matrix4 persp_mat, float scr_w, float scr_h, bool persp)
		{
			Vector3 v = pos;
			v = Vector3.Transform(v, cam_mat);
			persp_mat.M43 = 0f; // This is used by OpenGL for Z-clip I think, but it throws off calculations up close, so zero it out
			v = Vector3.Transform(v, persp_mat);

			if (persp) {
				v.X /= Math.Max(1f, v.Z);
				v.Y /= Math.Max(1f, v.Z);
			}

			v.X = scr_w * (v.X + 1.0f) / 2.0f;
			v.Y = scr_h * (1.0f - ((v.Y + 1.0f) / 2.0f));

			return v;
		}

		public static bool PointInsideAABBSort(Vector2 pt, Vector2 v1, Vector2 v2)
		{
			if (v1.X > v2.X) {
				float t = v1.X;
				v1.X = v2.X;
				v2.X = t;
			}
			if (v1.Y > v2.Y) {
				float t = v1.Y;
				v1.Y = v2.Y;
				v2.Y = t;
			}

			if ((pt.X >= v1.X && pt.X <= v2.X) || (pt.X >= v2.X && pt.X <= v1.X)) {
				if ((pt.Y >= v1.Y && pt.Y <= v2.Y) || (pt.Y >= v2.Y && pt.Y <= v1.Y)) {
					return true;
				}
			}
			return false;
		}

		public static bool PointInsideAABB(Vector2 pt, Vector2 v1, Vector2 v2)
		{
			if ((pt.X >= v1.X && pt.X <= v2.X) || (pt.X >= v2.X && pt.X <= v1.X)) {
				if ((pt.Y >= v1.Y && pt.Y <= v2.Y) || (pt.Y >= v2.Y && pt.Y <= v1.Y)) {
					return true;
				}
			}
			return false;
		}

		public static Vector3 CardinalVector(Vector3 v)
		{
			if (v.X > Math.Abs(v.Z) && v.X > Math.Abs(v.Y)) {
				return Vector3.UnitX;
			} else if (-v.X > Math.Abs(v.Z) && -v.X > Math.Abs(v.Y)) {
				return -Vector3.UnitX;
			} else if (v.Y > Math.Abs(v.X) && v.Y > Math.Abs(v.Z)) {
				return Vector3.UnitY;
			} else if (-v.Y > Math.Abs(v.X) && -v.Y > Math.Abs(v.Z)) {
				return -Vector3.UnitY;
			} else if (v.Z > Math.Abs(v.X) && v.Z > Math.Abs(v.Y)) {
				return Vector3.UnitZ;
			} else if (-v.Z > Math.Abs(v.X) && -v.Z > Math.Abs(v.Y)) {
				return -Vector3.UnitZ;
			} else {
				return Vector3.UnitY;
			}
		}

		public static Axis VectorToAxis(Vector3 v, out float sign) {
			// Note: This gets flipped for some views because of rotation (see DMeshUtility)
			sign = 1f;
			if (v.X > Math.Abs(v.Z) && v.X > Math.Abs(v.Y)) {
				return Axis.X;
			} else if (-v.X > Math.Abs(v.Z) && -v.X > Math.Abs(v.Y)) {
				sign = -1f;
				return Axis.X;
			} else if (v.Y > Math.Abs(v.X) && v.Y > Math.Abs(v.Z)) {
				sign = -1f;
				return Axis.Y;
			} else if (-v.Y > Math.Abs(v.X) && -v.Y > Math.Abs(v.Z)) {
				return Axis.Y;
			} else if (v.Z > Math.Abs(v.X) && v.Z > Math.Abs(v.Y)) {
				sign = -1f;
				return Axis.Z;
			} else if (-v.Z > Math.Abs(v.X) && -v.Z > Math.Abs(v.Y)) {
				return Axis.Z;
			} else {
				return Axis.Y;
			}
		}

		public static ViewType VectorToViewType(Vector3 v)
		{
			if (v.X > Math.Abs(v.Z) && v.X > Math.Abs(v.Y)) {
				return ViewType.RIGHT;
			} else if (-v.X > Math.Abs(v.Z) && -v.X > Math.Abs(v.Y)) {
				return ViewType.RIGHT;
			} else if (v.Y > Math.Abs(v.X) && v.Y > Math.Abs(v.Z)) {
				return ViewType.TOP;
			} else if (-v.Y > Math.Abs(v.X) && -v.Y > Math.Abs(v.Z)) {
				return ViewType.TOP;
			} else if (v.Z > Math.Abs(v.X) && v.Z > Math.Abs(v.Y)) {
				return ViewType.FRONT;
			} else if (-v.Z > Math.Abs(v.X) && -v.Z > Math.Abs(v.Y)) {
				return ViewType.FRONT;
			} else {
				return ViewType.FRONT;
			}
		}

		public static Axis ViewTypeToAxis(ViewType vt)
		{
			switch (vt) {
				default: return Axis.Y;
				case ViewType.FRONT: return Axis.Z;
				case ViewType.RIGHT: return Axis.X;
			}
		}

		public static Vector3 NullifyAxisForView(Vector3 pos, ViewType vt)
		{
			switch (vt) {
				default: pos.Y *= 0f; break;
				case ViewType.FRONT: pos.Z *= 0f; break;
				case ViewType.RIGHT: pos.X *= 0f; break;
			}

			return pos;
		}

		public const float CARDINAL_TOL = 0.995f;

		public static bool AlmostCardinal(Vector3 v)
		{
			if (Math.Abs(v.X) > CARDINAL_TOL || Math.Abs(v.Y) > CARDINAL_TOL || Math.Abs(v.Z) > CARDINAL_TOL) {
				return true;
			} else {
				return false;
			}
		}

		public static float DistanceFromPlane(Vector3 point, Vector3 plane_pos, Vector3 plane_norm)
		{
			float sb, sn, sd;

			sn = -Vector3.Dot(plane_norm, (point - plane_pos));
			sd = Vector3.Dot(plane_norm, plane_norm);
			sb = sn / sd;

			Vector3 result = point + sb * plane_norm;
			return (point - result).Length * (sn > 0f ? 1f : -1f);
		}

		public static float FindDistanceToEdge(Vector3 pt, Vector3 p1, Vector3 p2)
		{
			return FindDistanceToEdge(new Vector2(pt.X, pt.Y), p1, p2);
		}

		// Calculate the distance between
		// point pt and the edge p1 --> p2 (NOTE: This ignores .Z coordinates)
		public static float FindDistanceToEdge(Vector2 pt, Vector3 p1, Vector3 p2)
		{
			Vector2 closest;
			float dx = p2.X - p1.X;
			float dy = p2.Y - p1.Y;
			if ((dx == 0) && (dy == 0)) {
				// It's a point not a line segment.
				closest = new Vector2(p1.X, p1.Y);
				dx = pt.X - p1.X;
				dy = pt.Y - p1.Y;
				return (float)System.Math.Sqrt(dx * dx + dy * dy);
			}

			// Calculate the t that minimizes the distance.
			float t = ((pt.X - p1.X) * dx + (pt.Y - p1.Y) * dy) /
				 (dx * dx + dy * dy);

			// See if this represents one of the segment's
			// end points or a point in the middle.
			if (t < 0) {
				closest = new Vector2(p1.X, p1.Y);
				dx = pt.X - p1.X;
				dy = pt.Y - p1.Y;
			} else if (t > 1) {
				closest = new Vector2(p2.X, p2.Y);
				dx = pt.X - p2.X;
				dy = pt.Y - p2.Y;
			} else {
				closest = new Vector2(p1.X + t * dx, p1.Y + t * dy);
				dx = pt.X - closest.X;
				dy = pt.Y - closest.Y;
			}

			return (float)System.Math.Sqrt(dx * dx + dy * dy);
		}

		public const float CLOSE_TOL = 0.0001f;

		public static bool AlmostOverlapping(Vector3 v1, Vector3 v2, float close_tol = CLOSE_TOL)
		{
			Vector3 diff = (v1 - v2);
			return (diff.LengthSquared < (close_tol * close_tol));
		}

		public static Vector3 SnapValue(Vector3 v, float snap_inc)
		{
			v.X = SnapValue(v.X, snap_inc);
			v.Y = SnapValue(v.Y, snap_inc);
			v.Z = SnapValue(v.Z, snap_inc);
			return v;
		}

		public static Vector2 SnapValue(Vector2 v, float snap_inc)
		{
			v.X = SnapValue(v.X, snap_inc);
			v.Y = SnapValue(v.Y, snap_inc);
			return v;
		}

		public static float SnapValue(float value, float snap_inc)
		{
			if (value > 0.0f) {
				return snap_inc * ((int)((value / snap_inc) + 0.5f));
			} else {
				return -snap_inc * ((int)((-value / snap_inc) + 0.5f));
			}
		}

		public static Vector3 RotateAroundPivot(Vector3 pos, Vector3 pivot, Axis axis, float angle)
		{
			Vector3 diff = (pos - pivot);
			Vector2 v2;
			switch (axis) {
				case Axis.X:
					v2 = Vector2Rotate(diff.Yz, angle);
					diff.Y = v2.X;
					diff.Z = v2.Y;
					break;
				case Axis.Y:
					v2 = Vector2Rotate(diff.Xz, angle);
					diff.X = v2.X;
					diff.Z = v2.Y;
					break;
				case Axis.Z:
					v2 = Vector2Rotate(diff.Xy, angle);
					diff.X = v2.X;
					diff.Y = v2.Y;
					break;
			}

			return pivot + diff;
		}

		public static Vector3 ScaleFromPivot(Vector3 pos, Vector3 pivot, Axis axis, float amt)
		{
			Vector3 diff = (pos - pivot);
			switch (axis) {
				case Axis.X:
					diff.X *= amt;
					break;
				case Axis.Y:
					diff.Y *= amt;
					break;
				case Axis.Z:
					diff.Z *= amt;
					break;
				case Axis.XY:
					diff.X *= amt;
					diff.Y *= amt;
					break;
				case Axis.XZ:
					diff.X *= amt;
					diff.Z *= amt;
					break;
				case Axis.YZ:
					diff.Y *= amt;
					diff.Z *= amt;
					break;
				case Axis.ALL:
					diff.X *= amt;
					diff.Y *= amt;
					diff.Z *= amt;
					break;
			}

			return pivot + diff;
		}

		/// <summary>
		/// A robust method for getting the relative path of a file under a directory
		/// </summary>
		/// <param name="directory_name"></param>
		/// <param name="file_under_directory"></param>
		/// <returns></returns>
		public static string GetRelativeFilenameFromDirectory(string directory_name, string file_under_directory)
		{
			// with Uris, directories must end with a path separator
			Uri dir_uri = new Uri(directory_name.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar, UriKind.Absolute);

			if (!Path.IsPathRooted(file_under_directory)) {
				// Assume the file was given relative to the working directory
				file_under_directory = Path.GetFullPath(file_under_directory);
			}

			Uri absolute_file_uri = new Uri(file_under_directory.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar), UriKind.Absolute);
			Uri relative_file_uri = dir_uri.MakeRelativeUri(absolute_file_uri);
			if (relative_file_uri.IsAbsoluteUri) {
				throw new Exception(string.Format("The directory \"{0}\" is not a root of \"{1}\"", directory_name, file_under_directory));
			}

			return Uri.UnescapeDataString(relative_file_uri.ToString()).Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
		}

		/// <summary>
		/// Returns the relative filename, sans extension, of a file under a directory
		/// </summary>
		/// <param name="directory_name"></param>
		/// <param name="file_under_directory"></param>
		/// <returns></returns>
		public static string GetRelativeExtensionlessFilenameFromDirectory(string directory_name, string file_under_directory)
		{
			return Path.ChangeExtension(Utility.GetRelativeFilenameFromDirectory(directory_name, file_under_directory), null);
		}

		public static string GetPathlessFilename(string cur_file_name)
		{
			string dir_name = Path.GetDirectoryName(cur_file_name);
			if (dir_name != "") {
				return Utility.GetRelativeFilenameFromDirectory(dir_name, cur_file_name);
			} else {
				return cur_file_name;
			}
		}

		public static bool IsValid(this JToken tok)
		{
			return tok != null && tok.Type != JTokenType.None;
		}

		public static TEnum GetEnum<TEnum>(this JToken tok, TEnum defaultValue) where TEnum : struct, IConvertible
		{
			if (!tok.IsValid())
				return defaultValue;

			if (!typeof(TEnum).IsEnum) {
				throw new ArgumentException("TEnum must be an enumerated type");
			}

			if (tok.Type != JTokenType.String) {
				return defaultValue;
			}

			try {
				return (TEnum)Enum.Parse(typeof(TEnum), (string)tok);
			}
			catch {
				return defaultValue;
			}
		}

		public static int GetInt(this JToken tok, int defaultValue = 0)
		{
			if (!tok.IsValid())
				return defaultValue;

			if (tok.Type == JTokenType.Integer) {
				return (int)tok;
			}
			if (tok.Type == JTokenType.Boolean) {
				return ((bool)tok) ? 1 : 0;
			}
			if (tok.Type == JTokenType.Float) {
				return (int)((float)tok);
			}
			if (tok.Type == JTokenType.String) {
				int res;
				if (int.TryParse((string)tok, out res)) {
					return res;
				}
			}
			return defaultValue;
		}

		public static float GetFloat(this JToken tok, float defaultValue = 0.0f)
		{
			if (!tok.IsValid())
				return defaultValue;

			if (tok.Type == JTokenType.Float) {
				return (float)tok;
			}
			if (tok.Type == JTokenType.Integer) {
				return (float)((int)tok);
			}
			if (tok.Type == JTokenType.Boolean) {
				return ((bool)tok) ? 1.0f : 0.0f;
			}
			if (tok.Type == JTokenType.String) {
				float res;
				if (float.TryParse((string)tok, out res)) {
					return res;
				}
			}
			return defaultValue;
		}

		public static bool GetBool(this JToken tok, bool defaultValue = false)
		{
			if (!tok.IsValid())
				return defaultValue;

			if (tok.Type == JTokenType.Boolean) {
				return (bool)tok;
			}
			if (tok.Type == JTokenType.Integer) {
				return ((int)tok) != 0;
			}
			if (tok.Type == JTokenType.Float) {
				return ((float)tok) != 0.0f;
			}
			if (tok.Type == JTokenType.String) {
				bool res;
				if (bool.TryParse((string)tok, out res)) {
					return res;
				}

				int resInt;
				if (int.TryParse((string)tok, out resInt)) {
					return resInt != 0;
				}

				float resFloat;
				if (float.TryParse((string)tok, out resFloat)) {
					return resFloat != 0.0f;
				}
			}
			return defaultValue;
		}

		public static string GetString(this JToken tok, string defaultValue = null)
		{
			if (!tok.IsValid())
				return defaultValue;

			if (tok.Type == JTokenType.String) {
				return (string)tok;
			}
			if (tok.Type == JTokenType.Null) {
				return null;
			}
			if (tok.Type == JTokenType.Float) {
				return ((float)tok).ToString();
			}
			if (tok.Type == JTokenType.Integer) {
				return ((int)tok).ToString();
			}
			if (tok.Type == JTokenType.Boolean) {
				return ((bool)tok).ToString();
			}
			return defaultValue;
		}

		public static JArray GetArray(this JToken tok, JArray defaultValue = null)
		{
			if (tok.IsValid() && tok.Type == JTokenType.Array) {
				return (JArray)tok;
			}

			if (defaultValue == null) {
				return new JArray();
			}

			return defaultValue;
		}

		public static JObject GetObject(this JToken tok, JObject defaultValue = null)
		{
			if (tok.IsValid() && tok.Type == JTokenType.Object) {
				return (JObject)tok;
			}

			if (defaultValue == null) {
				return new JObject();
			}

			return defaultValue;
		}

		public static JToken Serialize(this OpenTK.Matrix4 mat)
		{
			var res = new JArray();
			for (int r = 0; r < 4; ++r) {
				for (int c = 0; c < 4; ++c) {
					var val = mat[r, c];
					res.Add(val);
				}
			}

			return res;
		}

		public static OpenTK.Matrix4 GetMatrix4(this JToken tok, OpenTK.Matrix4? defaultValue = null)
		{
			if (!tok.IsValid() || tok.Type != JTokenType.Array || ((JArray)tok).Count != 16) {
				if (defaultValue.HasValue) {
					return defaultValue.Value;
				}
				return OpenTK.Matrix4.Identity;
			}

			var mat = new OpenTK.Matrix4();
			var asArray = (JArray)tok;
			for (int r = 0; r < 4; ++r) {
				for (int c = 0; c < 4; ++c) {
					var val = asArray[r * 4 + c];
					if (val == null || (val.Type != JTokenType.Integer && val.Type != JTokenType.Float)) {
						if (defaultValue.HasValue) {
							return defaultValue.Value;
						}
						return OpenTK.Matrix4.Identity;
					}
					mat[r, c] = (float)val;
				}
			}
			return mat;
		}

		public static JToken Serialize(this OpenTK.Vector3 vec)
		{
			var res = new JArray();
			res.Add(vec.X);
			res.Add(vec.Y);
			res.Add(vec.Z);
			return res;
		}

		public static OpenTK.Vector3 GetVector3(this JToken tok, OpenTK.Vector3? defaultValue = null)
		{
			if (!tok.IsValid() || tok.Type != JTokenType.Array || ((JArray)tok).Count != 3) {
				if (defaultValue.HasValue) {
					return defaultValue.Value;
				}
				return OpenTK.Vector3.Zero;
			}

			OpenTK.Vector3 vec = new OpenTK.Vector3();
			var asArray = (JArray)tok;
			for (int i = 0; i < 3; ++i) {
				var val = asArray[i];
				if (val == null || (val.Type != JTokenType.Integer && val.Type != JTokenType.Float)) {
					if (defaultValue.HasValue) {
						return defaultValue.Value;
					}
					return OpenTK.Vector3.Zero;
				}
				vec[i] = (float)val;
			}
			return vec;
		}

		public static JToken Serialize(this OpenTK.Vector2 vec)
		{
			var res = new JArray();
			res.Add(vec.X);
			res.Add(vec.Y);
			return res;
		}

		public static OpenTK.Vector2 GetVector2(this JToken tok, OpenTK.Vector2? defaultValue = null)
		{
			if (!tok.IsValid() || tok.Type != JTokenType.Array || ((JArray)tok).Count != 2) {
				if (defaultValue.HasValue) {
					return defaultValue.Value;
				}
				return OpenTK.Vector2.Zero;
			}

			OpenTK.Vector2 vec = new OpenTK.Vector2();
			var asArray = (JArray)tok;
			for (int i = 0; i < 2; ++i) {
				var val = asArray[i];
				if (val == null || (val.Type != JTokenType.Integer && val.Type != JTokenType.Float)) {
					if (defaultValue.HasValue) {
						return defaultValue.Value;
					}
					return OpenTK.Vector2.Zero;
				}
				vec[i] = (float)val;
			}
			return vec;
		}

		public static float Lerp(float start, float end, float value)
		{
			if (value <= 0.0f)
				return start;
			if (value >= 1.0f)
				return end;
			return ((end - start) * value) + start;
		}

		// END UTILITY GAME

		public static Random random = new Random();

		public static int RandomRange(int min, int max)
		{
			return random.Next(max - min) + min;
		}

		public const int BIG_NUMBER = 1000000;
		public const float BIG_NUMBER_INVERT = 1f / (float)BIG_NUMBER;

		public static float RandomRange(float min, float max)
		{
			return (float)random.Next(BIG_NUMBER) * (max - min) * BIG_NUMBER_INVERT + min;
		}

		public static Vector2 Vector2Rotate(Vector2 v, float angle)
		{
			float sin = (float)Math.Sin(angle);
			float cos = (float)Math.Cos(angle);
			float tx = v.X;
			float ty = v.Y;
			v.X = (cos * tx) - (sin * ty);
			v.Y = (sin * tx) + (cos * ty);

			return v;
		}

		public static Vector3 WorldToScreenPos(Vector3 obj_pos, GLView view)
		{
			if (view.m_view_type == ViewType.TOP) {
				obj_pos = Vector3.Transform(obj_pos, Matrix4.CreateRotationX(-Utility.RAD_90));
			} else if (view.m_view_type == ViewType.RIGHT) {
				obj_pos = Vector3.Transform(obj_pos, Matrix4.CreateRotationY(Utility.RAD_90));
			} else if (view.m_view_type == ViewType.PERSP) {
				//obj_pos.Z *= -1f;
			}
			obj_pos.Z *= -1f;

			return Utility.WorldToScreen(obj_pos, view.m_cam_mat, view.m_persp_mat, view.m_control_sz.X, view.m_control_sz.Y, (view.m_view_type == ViewType.PERSP));
		}

		public static float[] ColorToFloats(Color4 color, float mult)
		{
			float[] c_floats = new float[4];
			c_floats[0] = mult * ((float)color.R) / 255f;
			c_floats[1] = mult * ((float)color.G) / 255f;
			c_floats[2] = mult * ((float)color.B) / 255f;
			c_floats[3] = ((float)color.A) / 255f;
			return c_floats;
		}

		public static int Pow2(int n)
		{
			return (int)Math.Pow(2, n);
		}

		public static float Clamp01(float f)
		{
			return Math.Min(1f, Math.Max(0f, f));
		}

		public static float Clamp(float f, float min, float max)
		{
			return Math.Min(max, Math.Max(min, f));
		}

		public static int Clamp(int i, int min, int max)
		{
			return Math.Min(max, Math.Max(min, i));
		}

		public static string ConvertFloatTo1Dec(float f, bool neg = true)
		{
			if (f < 0.0f) {
				if (neg) {
					return "-" + ConvertFloatTo1Dec(-f);
				} else {
					return "-.-";
				}
			}
			string s = ((int)f).ToString();
			float temp = 10.0001f * (float)(f - (float)((int)f));
			int tenths = (int)temp;
			s += "." + tenths.ToString();
			return s;
		}

		public static string ConvertFloatTo2Dec(float f, bool neg = true)
		{
			if (f < 0.0f) {
				if (neg) {
					return "-" + ConvertFloatTo2Dec(-f);
				} else {
					return "-.--";
				}
			}
			string s = ((int)f).ToString();
			float temp = 100.0001f * (float)(f - (float)((int)f));
			int hundredths = (int)temp;
			s += "." + (hundredths < 10 ? "0" : "") + hundredths.ToString();
			return s;
		}

		public static string ConvertFloatTo1Thru3Dec(float f)
		{
			string s = f.ToString();
			if (s.Contains(".")) {
				if (s.Contains("-")) {
					return s.Substring(0, Math.Min(s.Length, 6));
				} else {
					return s.Substring(0, Math.Min(s.Length, 5));
				}
			} else {
				return s + ".0";
			}
		}

		// MODIFIED FROM MATHFX (Unity extra math library)
		// -----
		public static float Hermite(float start, float end, float value)
		{
			return Lerp(start, end, value * value * (3.0f - 2.0f * value));
		}

		public static float Sinerp(float start, float end, float value)
		{
			return Lerp(start, end, (float)Math.Sin(value * (float)Math.PI * 0.5f));
		}

		public static float Coserp(float start, float end, float value)
		{
			return Lerp(start, end, 1.0f - (float)Math.Cos(value * (float)Math.PI * 0.5f));
		}

		public static float Berp(float start, float end, float value)
		{
			value = Clamp01(value);
			value = ((float)Math.Sin(value * (float)Math.PI * (0.2f + 2.5f * value * value * value)) * (float)Math.Pow(1f - value, 2.2f) + value) * (1f + (1.2f * (1f - value)));
			return start + (end - start) * value;
		}

		public static float SmoothStep(float x, float min, float max)
		{
			x = Clamp(x, min, max);
			float v1 = (x - min) / (max - min);
			float v2 = (x - min) / (max - min);
			return -2 * v1 * v1 * v1 + 3 * v2 * v2;
		}
	}
}