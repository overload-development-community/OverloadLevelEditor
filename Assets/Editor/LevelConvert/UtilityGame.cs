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
using System.IO;
using OpenTK;
using Newtonsoft.Json.Linq;

// UTILITY - Game
// Utility functions that need to bee accessible in the game/Unity

namespace OverloadLevelEditor
{
	public static partial class Utility
	{
#if !OVERLOAD_LEVEL_EDITOR
		public static void DebugPopup(string s, string s2 = "Warning")
		{
			var s2Lower = s2.ToLowerInvariant();
			if (s2Lower.Contains("error")) {
				UnityEngine.Debug.LogError(s);
			}
			else if (s2Lower.Contains("warning")) {
				UnityEngine.Debug.LogWarning(s);
			}
			else {
				UnityEngine.Debug.Log(s);
			}
		}
#else
		public static void DebugPopup(string s, string s2 = "Warning")
		{
			System.Windows.Forms.MessageBox.Show(s, s2);
			DebugLog(s);
		}

		public static void DebugLog(string s)
		{
			System.Diagnostics.Debug.WriteLine(s);
		}
#endif

		public class HermiteCurveEvaluator
		{
			// start_pt       : The starting position of the curve (the value at t=0.0f)
			// start_velocity : The direction and speed the curve should have coming out from the start_pt. This is the direction the
			//                  curve will initially start moving towards until it advances
			// end_pt         : The ending position of the curve (the value at t=1.0f)
			// end_velocity   : The direction and speed the curve should end up with when it reach end_pt. If you were to attach two
			//                  hermite curves end-to-start, for a continuous curve this would be the start_velocity of the next segment
			public HermiteCurveEvaluator( Vector3 start_pt, Vector3 start_velocity, Vector3 end_pt, Vector3 end_velocity )
			{
				this.StartPt = start_pt;
				this.EndPt = end_pt;
				this.StartVelocity = start_velocity;
				this.EndVelocity = end_velocity;
			}

			public Vector3 Eval(float t)
			{
				if( t <= 0.0f ) {
					return this.StartPt;
				}
				if( t >= 1.0f ) {
					return this.EndPt;
				}

				float t2 = t * t;
				float t3 = t2 * t;

				float h1 = 2.0f * t3 - 3 * t2 + 1.0f;
				float h2 = -2.0f * t3 + 3 * t2;
				float h3 = t3 - 2.0f * t2 + t;
				float h4 = t3 - t2;

				return ( this.StartPt * h1 ) + ( this.EndPt * h2 ) + ( this.StartVelocity * h3 ) + ( this.EndVelocity * h4 );
			}

			public float CalculateDistance(float t0, float t1)
			{
				if (t0 > t1) {
					// swap
					float temp = t0;
					t0 = t1;
					t1 = temp;
				}

				if (t1 <= 0.0f || t0 >= 1.0f ) {
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
				}
				else if (from_t >= 1.0f) {
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
					}
					else {
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

			public Vector3 StartPt{ get; private set; }
			public Vector3 EndPt{ get; private set; }
			public Vector3 StartVelocity{ get; private set; }
			public Vector3 EndVelocity{ get; private set; }

		}


		// Math helpers
		public const float PI = (float)Math.PI;
		public const float RAD_360 = PI * 2f;
		public const float RAD_180 = PI;
		public const float RAD_90 = PI * 0.5f;
		public const float RAD_45 = PI * 0.25f;
		public const float RAD_15 = PI / 12f;
		public const float RAD_1 = PI / 180f;

		// Side-Vertices order: Front (4567) Back (3210) Left (7623) Right (0154) Top (0473) Bottom (2651) 
		// Order of sides: Left, Top, Right, Bottom, Front, Back

		public static int TopLeftVertForSide(int side_idx)
		{
			switch (side_idx) {
				case (int)SideOrder.LEFT: return 3; // Lower Left = 2
				case (int)SideOrder.RIGHT: return 3; // Lower Left = 2
				case (int)SideOrder.TOP: return 1; // Upper Right = 2
				case (int)SideOrder.BOTTOM: return 3; // Lower Left = 2
				case (int)SideOrder.FRONT: return 3; // Lower Left = 2
				case (int)SideOrder.BACK: return 3; // Lower Left = 2
				default: return (int)SideOrder.FRONT;
			}
		}

		public static Vector3 DefaultSVOffset(int idx)
		{
			Vector3 v;
			switch (idx) {
				case 0: v.X = 1f; v.Y = 1f; v.Z = -1f; return v;
				case 1: v.X = 1f; v.Y = -1f; v.Z = -1f; return v;
				case 2: v.X = -1f; v.Y = -1f; v.Z = -1f; return v;
				case 3: v.X = -1f; v.Y = 1f; v.Z = -1f; return v;
				case 4: v.X = 1f; v.Y = 1f; v.Z = 1f; return v;
				case 5: v.X = 1f; v.Y = -1f; v.Z = 1f; return v;
				case 6: v.X = -1f; v.Y = -1f; v.Z = 1f; return v;
				case 7: v.X = -1f; v.Y = 1f; v.Z = 1f; return v;
				default:
					return Vector3.Zero;
			}
		}

		public static int OppositeSide(int side_idx)
		{
			switch (side_idx) {
				case (int)SideOrder.LEFT: return (int)SideOrder.RIGHT;
				case (int)SideOrder.RIGHT: return (int)SideOrder.LEFT;
				case (int)SideOrder.TOP: return (int)SideOrder.BOTTOM;
				case (int)SideOrder.BOTTOM: return (int)SideOrder.TOP;
				case (int)SideOrder.FRONT: return (int)SideOrder.BACK;
				case (int)SideOrder.BACK: return (int)SideOrder.FRONT;
				default: return (int)SideOrder.FRONT;
			}
		}

		public static Vector3 TwoVectorCorners(Vector3 right, Vector3 up, int corner)
		{
			switch (corner) {
				default:
				case 0: return right * 1f + up * -1f;
				case 1: return right * -1f + up * -1f;
				case 2: return right * -1f + up * 1f;
				case 3: return right * 1f + up * 1f;
			}
		}

		public static int[] SideVertsFromSegVerts(int[] seg_verts, int side_num)
		{
			int[] side_verts = new int[Side.NUM_VERTS];

			switch (side_num) {
				default:
				case (int)SideOrder.LEFT:
					side_verts[0] = seg_verts[7];
					side_verts[1] = seg_verts[6];
					side_verts[2] = seg_verts[2];
					side_verts[3] = seg_verts[3];
					break;
				case (int)SideOrder.RIGHT:
					side_verts[0] = seg_verts[0];
					side_verts[1] = seg_verts[1];
					side_verts[2] = seg_verts[5];
					side_verts[3] = seg_verts[4];
					break;
				case (int)SideOrder.TOP:
					side_verts[0] = seg_verts[0];
					side_verts[1] = seg_verts[4];
					side_verts[2] = seg_verts[7];
					side_verts[3] = seg_verts[3];
					break;
				case (int)SideOrder.BOTTOM:
					side_verts[0] = seg_verts[2];
					side_verts[1] = seg_verts[6];
					side_verts[2] = seg_verts[5];
					side_verts[3] = seg_verts[1];
					break;
				case (int)SideOrder.FRONT:
					side_verts[0] = seg_verts[4];
					side_verts[1] = seg_verts[5];
					side_verts[2] = seg_verts[6];
					side_verts[3] = seg_verts[7];
					break;
				case (int)SideOrder.BACK:
					side_verts[0] = seg_verts[3];
					side_verts[1] = seg_verts[2];
					side_verts[2] = seg_verts[1];
					side_verts[3] = seg_verts[0];
					break;
			}

			return side_verts;
		}

		public static Vector3[] SideVertsFromSegVerts(Vector3[] seg_verts, int side_num)
		{
			Vector3[] side_verts = new Vector3[Side.NUM_VERTS];

			switch (side_num) {
				default:
				case (int)SideOrder.LEFT:
					side_verts[0] = seg_verts[7];
					side_verts[1] = seg_verts[6];
					side_verts[2] = seg_verts[2];
					side_verts[3] = seg_verts[3];
					break;
				case (int)SideOrder.RIGHT:
					side_verts[0] = seg_verts[0];
					side_verts[1] = seg_verts[1];
					side_verts[2] = seg_verts[5];
					side_verts[3] = seg_verts[4];
					break;
				case (int)SideOrder.TOP:
					side_verts[0] = seg_verts[0];
					side_verts[1] = seg_verts[4];
					side_verts[2] = seg_verts[7];
					side_verts[3] = seg_verts[3];
					break;
				case (int)SideOrder.BOTTOM:
					side_verts[0] = seg_verts[2];
					side_verts[1] = seg_verts[6];
					side_verts[2] = seg_verts[5];
					side_verts[3] = seg_verts[1];
					break;
				case (int)SideOrder.FRONT:
					side_verts[0] = seg_verts[4];
					side_verts[1] = seg_verts[5];
					side_verts[2] = seg_verts[6];
					side_verts[3] = seg_verts[7];
					break;
				case (int)SideOrder.BACK:
					side_verts[0] = seg_verts[3];
					side_verts[1] = seg_verts[2];
					side_verts[2] = seg_verts[1];
					side_verts[3] = seg_verts[0];
					break;
			}

			return side_verts;
		}

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
			}
			else if (-v.X > Math.Abs(v.Z) && -v.X > Math.Abs(v.Y)) {
				return -Vector3.UnitX;
			}
			else if (v.Y > Math.Abs(v.X) && v.Y > Math.Abs(v.Z)) {
				return Vector3.UnitY;
			}
			else if (-v.Y > Math.Abs(v.X) && -v.Y > Math.Abs(v.Z)) {
				return -Vector3.UnitY;
			}
			else if (v.Z > Math.Abs(v.X) && v.Z > Math.Abs(v.Y)) {
				return Vector3.UnitZ;
			}
			else if (-v.Z > Math.Abs(v.X) && -v.Z > Math.Abs(v.Y)) {
				return -Vector3.UnitZ;
			}
			else {
				return Vector3.UnitY;
			}
		}

		public const float CARDINAL_TOL = 0.995f;

		public static bool AlmostCardinal(Vector3 v)
		{
			if (Math.Abs(v.X) > CARDINAL_TOL || Math.Abs(v.Y) > CARDINAL_TOL || Math.Abs(v.Z) > CARDINAL_TOL) {
				return true;
			}
			else {
				return false;
			}
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
			}
			else {
				return -snap_inc * ((int)((-value / snap_inc) + 0.5f));
			}
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

		public static Guid GetGuid(this JToken tok, Guid defaultValue)
		{
			if (!tok.IsValid())
				return defaultValue;

			if (tok.Type == JTokenType.String) {
				return new Guid((string)tok);
			}
			if (tok.Type == JTokenType.Null) {
				return Guid.Empty;
			}
			return defaultValue;
		}

		public static Overload.EntityGuid GetEntityGuid(this JToken tok)
		{
			if (!tok.IsValid())
				return Overload.EntityGuid.Empty;

			if (tok.Type == JTokenType.String) {
				return new Overload.EntityGuid((string)tok);
			}
			if (tok.Type == JTokenType.Null) {
				return Overload.EntityGuid.Empty;
			}
			return Overload.EntityGuid.Empty;
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

		// Shorten to string for editor display
		public static string ToPrettyString(this Guid guid)
		{
			string s = guid.ToString();
			return s.Substring(s.Length - 4).ToUpper();
		}
	}
}