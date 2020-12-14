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

// UTILITY
// Editor-specific utility functions
// (see also UtilityGame)

#pragma warning disable 1690 // warning CS1690: Accessing a member on 'OverloadLevelEditor.GLView.control_sz' may cause a runtime exception because it is a field of a marshal-by-reference class

namespace OverloadLevelEditor
{
	public static partial class Utility
	{
#if !OVERLOAD_LEVEL_EDITOR
		public static System.Drawing.Color ColorFromUnity(UnityEngine.Color c)
		{
			return System.Drawing.Color.FromArgb(F2B(c.a), F2B(c.r), F2B(c.g), F2B(c.b));
		}

		public static UnityEngine.Color ColorToUnity(System.Drawing.Color c)
		{
			return new UnityEngine.Color(B2F(c.R), B2F(c.G), B2F(c.B), B2F(c.A));
		}
#endif

		public static float B2F(int b)
		{
			return (float)b / 255f;
		}

		// Convert a float to a byte (input range 0-1.0 output range 0-255)
		public static int F2B(float f)
		{
			return (int)(255.001f * f);
		}

		public static Vector3 GetEntitySize(Entity e)
		{
			return GetEntityTypeSize(e.Type);
		}

		// Copy one object's public properties to another object of the same type
		public static void CopyTo<T>(this T src, T dest) where T : class
		{
			foreach (var prop_info in src.GetType().GetProperties()) {
				if (prop_info.PropertyType.IsArray) {                    //Array, so create new array and copy elements
					(prop_info.GetSetMethod()).Invoke(dest, new object[] { ((Array)prop_info.GetGetMethod().Invoke(src, null)).Clone() });
				} else if (!prop_info.PropertyType.IsValueType) {        //A (non-array) refernece type.  Put up a warning.
					Utility.DebugPopup("CopyTo: Copying reference type:" + prop_info.Name);
				} else {                                              //A value type, so copy it
					(prop_info.GetSetMethod()).Invoke(dest, new object[] { prop_info.GetGetMethod().Invoke(src, null) });
				}
			}
		}

		public static Vector3 GetEntityTypeSize(EntityType et)
		{
			switch (et) {
				default:
				case EntityType.ENEMY: return new Vector3(0.5f, 0.25f, 0.75f);
				case EntityType.PROP: return new Vector3(0.5f, 0.5f, 0.5f);
				case EntityType.ITEM: return new Vector3(0.5f, 0.25f, 0.5f);
				case EntityType.DOOR: return new Vector3(1f, 1f, 0.125f);
				case EntityType.SCRIPT: return new Vector3(0.25f, 0.25f, 0.25f);
				case EntityType.TRIGGER: return new Vector3(0.5f, 0.5f, 0.5f);
				case EntityType.LIGHT: return new Vector3(0.5f, 0.5f, 0.5f);
				case EntityType.SPECIAL: return new Vector3(0.5f, 0.5f, 0.75f);
			}
		}

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

		public static float DistanceFromPlane(Vector3 point, Vector3 plane_pos, Vector3 plane_norm)
		{
			float sb, sn, sd;

			sn = -Vector3.Dot(plane_norm, (point - plane_pos));
			sd = Vector3.Dot(plane_norm, plane_norm);
			sb = sn / sd;

			Vector3 result = point + sb * plane_norm;
			return (point - result).Length * (sn > 0f ? 1f : -1f);
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

		public static Axis VectorToAxis(Vector3 v, out float sign)
		{
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

		public static float Lerp(float start, float end, float value)
		{
			return ((end - start) * value) + start;
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