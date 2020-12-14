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

using UnityEngine;
using Overload;
using System.Reflection;
using System.Globalization;
using System.Collections.Generic;

// Some useful general-purpose functions for math/strings/etc
public static class RUtility
{
	public const float RADIANS_360 = ((float)Mathf.PI) * 2.0f;
	public const float RADIANS_270 = ((float)Mathf.PI) * 1.5f;
	public const float RADIANS_180 = ((float)Mathf.PI);
	public const float RADIANS_135 = ((float)Mathf.PI) * 3.0f / 4.0f;
	public const float RADIANS_120 = ((float)Mathf.PI) / 1.5f;
	public const float RADIANS_90 = ((float)Mathf.PI) / 2.0f;
	public const float RADIANS_60 = ((float)Mathf.PI) / 3.0f;
	public const float RADIANS_45 = ((float)Mathf.PI) / 4.0f;
	public const float RADIANS_30 = ((float)Mathf.PI) / 6.0f;
	public const float RADIANS_15 = ((float)Mathf.PI) / 12.0f;
	public const float RADIANS_5 = ((float)Mathf.PI) / 36.0f;
	public const float RADIANS_1 = ((float)Mathf.PI) / 180.0f;
	public const float RADIANS_0 = 0.0f;
	public const float RADIANS_TO_DEGREES = 360f / RADIANS_360;
	public const float DEGREES_TO_RADIANS = RADIANS_360 /360f;

	public static float FRAMETIME_GAME;
	public static float FRAMETIME_UI;
	public static float FRAMETIME_FIXED;
	public static float FIXED_FT_RATIO; // FRAMETIME_FIXED / 120
	public static float FIXED_FT_INVERTED; // 120 / FRAMETIME_FIXED

	public static string DIR_SEP = "/"; //Path.DirectorySeparatorChar.ToString();
	public const string TAG_DATA = "OVERLOAD_DATA";
	public const string TAG_ENTRY_END = "END_ENTRY";

	// Delayed object destroy
	public static void DestroyGameObjectDelayed(GameObject go, bool detach_particles, float secs = 3f)
	{
		if (detach_particles) {
			// All attached muzzle flashes must have an explosion lite
			ExplosionLite[] exp_lite = go.GetComponentsInChildren<ExplosionLite>();
			int count = exp_lite.Length;
         for (int i = 0; i < count; i++) {
				exp_lite[i].c_transform.parent = null;
			}
		}
		go.SetActive(false);
		Object.Destroy(go, secs);
	}
		
	// Round up the floating point value to the nearest increment
	public static float RoundUpToIncrement(float value, float inc)
	{
		float div = value / inc;
		int div_int = (int)div;
		if (div - div_int > 0.0f) {
			return (div_int + 1) * inc;
		} else {
			return div_int * inc;
		}
	}

	// Returns positive value between 0 and 1 for UV comparisons
	public static float UVFraction(float value)
	{
		if (value < 0f) {
			value -= (int)(value);
		}

		return (value % 1f);
	}

	public static Vector2 UVFraction(Vector2 uv)
	{
		return new Vector2(UVFraction(uv.x), UVFraction(uv.y));
	}

	public static Vector2 UVFractionSnapped(Vector2 uv, float snap_amt = 0.001f)
	{
		return UVFraction(SnapValue(uv, snap_amt));
	}

	public static Vector3 SnapValue(Vector3 v, float snap_inc)
	{
		v.x = SnapValue(v.x, snap_inc);
		v.y = SnapValue(v.y, snap_inc);
		v.z = SnapValue(v.z, snap_inc);
		return v;
	}

	public static Vector2 SnapValue(Vector2 v, float snap_inc)
	{
		v.x = SnapValue(v.x, snap_inc);
		v.y = SnapValue(v.y, snap_inc);
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

	// This is here for reference.  Better to copy-paste it for speed
	public static float FindVec3Distance(Vector3 vec)
	{
		return (float)System.Math.Sqrt(vec.x * vec.x + vec.y * vec.y + vec.z * vec.z);
	}

	public static void Vec3Normalize(ref Vector3 vec)
	{
		float len = (float)System.Math.Sqrt(vec.x * vec.x + vec.y * vec.y + vec.z * vec.z);
		len = (len > 0.001f ? len : 0.001f);
		vec.x /= len;
		vec.y /= len;
		vec.z /= len;
	}

	public static Vector3 Vec3Normalized(Vector3 vec)
	{
		float len = (float)System.Math.Sqrt(vec.x * vec.x + vec.y * vec.y + vec.z * vec.z);
		len = (len > 0.001f ? len : 0.001f);
		return new Vector3(vec.x / len, vec.y / len, vec.z / len);
	}

	// Map range and value to another range (return the new value)
	public static float MapRange(float range_1_max, float range_1_min, float value_1, float range_2_max, float range_2_min)
	{
		float diff = range_1_max - range_1_min;
		float offset = (value_1 - range_1_min) / diff;
		float value = (offset * (range_2_max - range_2_min)) + range_2_min;
		return value;
	}

	// For lerping (probably redundant with Mathf.Lerp)
	public static float WeightedAverage(float a, float b, float b_weight)
	{
		return (a * (1.0f - b_weight) + b * (b_weight));
	}

	// True and false
	public static bool RandomBool()
	{
		float value = Random.Range(0.0f, 1.0f);
		return ((value < 0.5f) ? true : false);
	}

	public static bool RandomBool(float chance_of_true)
	{
		float value = Random.Range(0.0f, 1.0f);
		return ((value < chance_of_true) ? true : false);
	}

	public static float RandomSign()
	{
		return (Random.value > 0.5f ? 1f : -1f);
	}

	public static Vector2 RandomUnitVector()
	{
		float rot = Random.Range(0, RADIANS_360);
		return AngleToVector(rot);
	}

	public static Vector2 AngleToVector(float angle)
	{
		Vector2 vec = Vector2.zero;
		vec.x = Mathf.Sin(angle);
		vec.y = -Mathf.Cos(angle);

		return vec;
	}

	public static float VectorToAngle(Vector2 vec)
	{
		return Mathf.Atan2(-vec.x, vec.y);
	}

	public static Quaternion AngleRandomize(Quaternion rot, float angle)
	{
		Vector2 random_vec = Random.insideUnitCircle;

		return AngleSpreadY(AngleSpreadX(rot, angle * random_vec.x), angle * random_vec.y);
	}

	public static Quaternion AngleSpreadX(Quaternion rot, float angle)
	{
		Quaternion rot_x = Quaternion.AngleAxis(angle, rot * Vector3.up);

		return rot_x * rot;
	}

	public static Quaternion AngleSpreadY(Quaternion rot, float angle)
	{
		Quaternion rot_y = Quaternion.AngleAxis(angle, rot * Vector3.right);

		return rot_y * rot;
	}

	public static Quaternion AngleSpreadXZ(Quaternion rot, float angle_x, float angle_z)
	{
		Quaternion rot_x = Quaternion.AngleAxis(angle_x, rot * Vector3.up);
		Quaternion rot_z = Quaternion.AngleAxis(angle_z, rot * Vector3.forward);

		return rot_z * (rot_x * rot);
	}

	// Assumes normalized vectors, returns angle in degrees
	public static float AngleBetweenVectorsDegrees(Vector3 v1, Vector3 v2)
	{
		float dot = Vector3.Dot(v1, v2);
		//Get the arc cosin of the angle, you now have your angle in radians 
		float angle = Mathf.Acos(dot);
		
		return angle * RADIANS_TO_DEGREES;
	}

	public static Quaternion QuaternionFromMatrix(Matrix4x4 m)
	{
		// Adapted from: http://www.euclideanspace.com/maths/geometry/rotations/conversions/matrixToQuaternion/index.htm
		Quaternion q = new Quaternion();
		q.w = (float)System.Math.Sqrt(Mathf.Max(0, 1 + m[0, 0] + m[1, 1] + m[2, 2])) * 0.5f;
		q.x = (float)System.Math.Sqrt(Mathf.Max(0, 1 + m[0, 0] - m[1, 1] - m[2, 2])) * 0.5f;
		q.y = (float)System.Math.Sqrt(Mathf.Max(0, 1 - m[0, 0] + m[1, 1] - m[2, 2])) * 0.5f;
		q.z = (float)System.Math.Sqrt(Mathf.Max(0, 1 - m[0, 0] - m[1, 1] + m[2, 2])) * 0.5f;
		q.x *= Mathf.Sign(q.x * (m[2, 1] - m[1, 2]));
		q.y *= Mathf.Sign(q.y * (m[0, 2] - m[2, 0]));
		q.z *= Mathf.Sign(q.z * (m[1, 0] - m[0, 1]));
		return q;
	}

	public static Quaternion QuaternionFromMatrixAlt(Matrix4x4 m)
	{
		return Quaternion.LookRotation(m.GetColumn(2), m.GetColumn(1));
	}

	// Convert a number into a score string (with commas)
	public static string ConvertToScore(long i)
	{
		string comma = Loc.m_language_digits_separator[(int)Loc.CurrentLanguage];
      string s = "";
		if (i >= 1000000000) {
			s += ((i - (i % 1000000000)) / 1000000000).ToString();
			s += comma;
			s += PadThousandString((i % 1000000000 - (i % 1000000)) / 1000000);
			s += comma;
			s += PadThousandString((i % 1000000 - (i % 1000)) / 1000);
			s += comma;
			s += PadThousandString(i % 1000);
			return s;
		} else if (i >= 1000000) {
			s += ((i - (i % 1000000)) / 1000000).ToString();
			s += comma;
			s += PadThousandString((i % 1000000 - (i % 1000)) / 1000);
			s += comma;
			s += PadThousandString(i % 1000);
			return s;
		} else if (i >= 1000) {
			s += ((i - (i % 1000)) / 1000).ToString();
			s += comma;
			s += PadThousandString(i % 1000);
			return s;
		} else {
			return i.ToString();
		}
	}

	// Return a two-digit string for an int
	public static string TwoDigits(int i)
	{
		if (i < 10) {
			return "0" + i.ToString();
		} else {
			return i.ToString();
		}
	}

	// Pad a number to fit in a thousand string
	public static string PadThousandString(long i)
	{
		if (i >= 100) {
			return i.ToString();
		} else if (i >= 10) {
			return "0" + i.ToString();
		} else {
			return "00" + i.ToString();
		}
	}

	// Convert a floating number to a whole-number percentage string (0-1 = "0%" to "100%)
	public static string FloatToPercent(float pct)
	{
		pct = pct * 100.0f + 0.001f;
		return ((int)pct).ToString() + "%";
	}

	// Round a value to the nearest step within a range (starting from min, going up steps * step_value)
	public static float NearestStep(float cur_value, float min, float step_value, int steps)
	{
		float nearest = min;
		float nearest_diff = Mathf.Abs(cur_value - min);
		float diff;
		float temp;

		for (int i = 1; i <= steps; i++) {
			temp = (min + i * step_value);
			diff = Mathf.Abs(cur_value - temp);
			if (diff < nearest_diff) {
				nearest = temp;
				nearest_diff = diff;
			}
		}

		return nearest;
	}

	// Convert a floating point number to a 2 decimal string (if negative, can display -.-- instead)
	public static string ConvertFloatTo2Dec(float time, bool neg = true)
	{
		if (time < 0.0f) {
			if (neg) {
				return "-" + ConvertFloatTo2Dec(-time);
			} else {
				return "-.--";
			}
		}
		string s = ((int)time).ToString();
		float temp = 100.0f * (float)(time - (float)((int)time));
		int hundredths = (int)temp;
		s += "." + (hundredths < 10 ? "0" : "") + hundredths.ToString();
		return s;
	}

	// Convert a floating point number to a 1 decimal string (if negative, can display -.- instead)
	public static string ConvertFloatTo1Dec(float time, bool neg = true)
	{
		if (time < 0.0f) {
			if (neg) {
				return "-" + ConvertFloatTo1Dec(-time);
			} else {
				return "-.-";
			}
		}
		string s = ((int)time).ToString();
		float temp = 10.0f * (float)(time - (float)((int)time));
		int tenths = (int)temp;
		s += "." + tenths.ToString();
		return s;
	}

	// Convert a floating point number time string (if negative, can display 0:00 instead)
	public static string ConvertFloatToSeconds(float time, bool show_hundredths = false)
	{
		if (time < 0.0f) {
			return "0:00";
		}

		int minutes = (int)(time / 60f);
		int seconds = (int)time - minutes * 60;
		string s = minutes.ToString() + ":" + (seconds < 10 ? "0" : "") + seconds.ToString();
		if (show_hundredths) {
			int hundredths = (int) ((time - minutes * 60 - seconds) * 100);
         s = s + Loc.m_language_decimal_separator[(int)Loc.CurrentLanguage] + hundredths.ToString("D2");
		}
		return s;
	}

	// Supposedly the build version is super slow.  This is not.
	public static bool StringStartsWith(string original, string start)
	{
		int len1 = original.Length;
		int len2 = start.Length;
		if (len1 < len2) return false;

		for (int i = 0; i < len2; i++) {
			if (original[i] != start[i]) {
				return false;
			}
		}

		return true;
	}

	// Component quick copy functions
	public static void QuickCopyLight(Light src, Light dst)
	{
		dst.color = src.color;
		dst.range = src.range;
		dst.intensity = dst.intensity;
	}

	public static void QuickCopyPhysMaterial(PhysicMaterial src, PhysicMaterial dst)
	{
		dst.bounciness = src.bounciness;
		dst.bounceCombine = src.bounceCombine;
		dst.dynamicFriction = src.dynamicFriction;
		dst.staticFriction = src.staticFriction;
		dst.frictionCombine = src.frictionCombine;
	}

	public static void QuickCopyRigidbody(Rigidbody src, Rigidbody dst)
	{
		dst.angularDrag = src.angularDrag;
		dst.centerOfMass = src.centerOfMass;
		dst.drag = src.drag;
		dst.collisionDetectionMode = src.collisionDetectionMode;
		dst.freezeRotation = src.freezeRotation;
		dst.mass = src.mass;
		dst.maxAngularVelocity = src.maxAngularVelocity;
		dst.useGravity = src.useGravity;
	}

	public static void QuickCopyCapsuleCollider(CapsuleCollider src, CapsuleCollider dst)
	{
		dst.center = src.center;
		dst.height = src.height;
		dst.radius = src.radius;
		dst.direction = src.direction;
	}

	public static Component CopyComponent(Component original, GameObject destination)
	{
		System.Type type = original.GetType();
		Component copy = destination.AddComponent(type);
		// Copied fields can be restricted with BindingFlags
		System.Reflection.FieldInfo[] fields = type.GetFields();
		foreach (System.Reflection.FieldInfo field in fields) {
			field.SetValue(copy, field.GetValue(original));
		}
		return copy;
	}

	public static T ParseEnumSafe<T>( System.Type enumType, string valueStr, T defaultValue )
	{
		try {
			return (T)System.Enum.Parse( enumType, valueStr, true );
		} catch( System.Exception ex ) {
			Debug.LogException( ex );
			return defaultValue;
		}
	}

	public static void ReadField(System.Object obj, string name, string value)
	{
		try {
			FieldInfo field = obj.GetType().GetField(name);
			if (field == null) {
				Debug.LogError("Tried to read a field of name: " + name + " but that doesn't exist");
			}
			System.Type field_type = field.FieldType;
			if (field_type == typeof(int)) {
					field.SetValue(obj, int.Parse(value,CultureInfo.InvariantCulture));
			} else if (field_type == typeof(float)) {
				field.SetValue(obj, value.ToFloat());
			} else if (field_type == typeof(bool)) {
				field.SetValue(obj, bool.Parse(value));
			} else if (field_type == typeof(string)) {
				field.SetValue(obj, value);
			} else if (field_type == typeof(FallOffType)) {
				FallOffType fot = ParseEnumSafe( typeof( FallOffType ), value, FallOffType.LINEAR );
				field.SetValue(obj, fot);
			} else if (field_type == typeof(BounceBehavior)) {
				BounceBehavior bb = ParseEnumSafe( typeof( BounceBehavior ), value, BounceBehavior.none );
				field.SetValue(obj, bb);
			} else if (field_type == typeof(FXWeaponExplosion)) {
				FXWeaponExplosion wxp = ParseEnumSafe( typeof( FXWeaponExplosion ), value, FXWeaponExplosion.num );
				field.SetValue(obj, wxp);
			} else if (field_type == typeof(ProjPrefab)) {
				ProjPrefab proj = ParseEnumSafe( typeof( ProjPrefab ), value, ProjPrefab.num );
				field.SetValue(obj, proj);
			} else if (field_type == typeof(ItemPrefab)) {
				ItemPrefab item = ParseEnumSafe(typeof(ItemPrefab), value, ItemPrefab.num);
				field.SetValue(obj, item);
			} else if (field_type == typeof(ProjSpawnPattern)) {
				ProjSpawnPattern psp = ParseEnumSafe( typeof( ProjSpawnPattern ), value, ProjSpawnPattern.RANDOM );
				field.SetValue(obj, psp);
			} else if (field_type == typeof(SoundEffect)) {
				SoundEffect sfx = ParseEnumSafe( typeof( SoundEffect ), value, SoundEffect.none );
				field.SetValue(obj, sfx);
			} else if (field_type == typeof(FXWeaponEffect)) {
				FXWeaponEffect wfx = ParseEnumSafe( typeof( FXWeaponEffect ), value, FXWeaponEffect.num );
				field.SetValue(obj, wfx);
			} else if (field_type == typeof(FXTrailRenderer)) {
				FXTrailRenderer trr = ParseEnumSafe( typeof( FXTrailRenderer ), value, FXTrailRenderer.num );
				field.SetValue(obj, trr);
			} else if (field_type == typeof(FiringDistribution)) {
				FiringDistribution fd = ParseEnumSafe( typeof( FiringDistribution ), value, FiringDistribution.SEQUENTIAL );
				field.SetValue(obj, fd);
			} else if (field_type == typeof(WeaponUnlock)) {
				WeaponUnlock wu = ParseEnumSafe( typeof( WeaponUnlock ), value, WeaponUnlock.LEVEL_0 );
				field.SetValue(obj, wu);
			} else if (field_type == typeof(SFXCue)) {
				SFXCue cue = ParseEnumSafe( typeof( SFXCue ), value, SFXCue.none );
				field.SetValue(obj, cue);
			} else {
				Debug.LogError("Unrecognized field type: " + field_type.ToString() + " for variable: " + name);
			}
		}
		catch( System.Exception ex ) {
			Debug.LogException( ex );
		}
	}

	/// <summary>Check if this object or its parent can open the door</summary>
	public static bool CanObjectOpenDoor(GameObject go, bool robot_can_use)
	{
		if (go != null && (go.layer == (int)UnityObjectLayers.PLAYER_PROJ || go.layer == (int)UnityObjectLayers.PLAYER_LEVEL) || (go.layer == (int)UnityObjectLayers.ITEM_LEVEL && go.CompareTag("GuideBot"))) {
			return true;
		}

		if (go != null && (go.layer == (int)UnityObjectLayers.ENEMY_LEVEL || go.layer == (int)UnityObjectLayers.ENEMY_MESH)) {
			return robot_can_use;
		}

		if (go.transform.parent != null && go.transform.parent.gameObject != null) {
			return CanObjectOpenDoor(go.transform.parent.gameObject, robot_can_use);
		} else {
			return false;
		}
	}

	public static bool CanSeePoint(Vector3 pos1, Vector3 pos2, out RaycastHit raycast_info)
	{
		bool cast_result;
		int layer_mask;

		// Want to check only level.  For now, don't assume level layer has been correctly set, so see comments below.
		layer_mask = (1 << 0) | (1 << (int)UnityObjectLayers.LEVEL) | (1 << (int)UnityObjectLayers.LAVA) | (1 << (int)UnityObjectLayers.DOOR);

		cast_result = Physics.Linecast(pos1, pos2, out raycast_info, layer_mask);
		if (cast_result == true) {
			return !cast_result;
		} else { // Need to cast in opposite direction.
			cast_result = Physics.Linecast(pos2, pos1, out raycast_info, layer_mask);
		}

		return !cast_result;
	}

	//public static bool CanSeePoint(Vector3 pos1, Vector3 pos2)
	//{
	//	bool raycast_result;
	//	int layer_mask;

		//	RaycastHit hit;

		//	// Want to check only level.  For now, don't assume level layer has been correctly set.
		//	layer_mask = (1 << 0) | (1 << (int)UnityObjectLayers.LEVEL) | (1 << (int)UnityObjectLayers.LAVA) | (1 << (int)UnityObjectLayers.DOOR);

		//	Vector3 td_vec = RUtility.Vec3Normalized(pos2 - pos1);
		//	raycast_result = Physics.Raycast(pos1, td_vec, out hit, (pos2 - pos1).magnitude, layer_mask);

		//	return !raycast_result;
		//}

	public static bool CanSeePointSphere(Vector3 pos1, Vector3 pos2, float radius)
	{
		bool raycast_result;
		int layer_mask;

		RaycastHit hit;

		// Want to check only level.  For now, don't assume level layer has been correctly set.
		layer_mask = (1 << 0) | (1 << (int)UnityObjectLayers.LEVEL) | (1 << (int)UnityObjectLayers.LAVA) | (1 << (int)UnityObjectLayers.DOOR);

		Vector3 td_vec = RUtility.Vec3Normalized(pos2 - pos1);
		raycast_result = Physics.SphereCast(pos1, radius, td_vec, out hit, (pos2 - pos1).magnitude, layer_mask);

		return !raycast_result;
	}

	public static int BoolArrayToBitmask(bool[] ba)
	{
		if (ba.Length > 31) {
			Debug.LogError("Can't convert a bool array with 32+ elements");
		}
		int n = 0;
		for (int i = 0; i < ba.Length; i++) {
			n += (ba[i] ? 1 << i : 0);
		}

		return n;
	}

	public static void BitmaskToBoolArray(int bm, ref bool[] ba)
	{
		if (ba.Length > 31) {
			Debug.LogError("Can't handle a bool array with 32+ elements");
		}
		for (int i = 0; i < ba.Length; i++) {
			if ((bm & (1 << i)) == (1 << i)) {
				ba[i] = true;
			} else {
				ba[i] = false;
			}
		}
	}

	// Return the index of a random true value in the array (-1 for none)
	public static int GetRandomTrueInBoolArray(bool[] ba)
	{
		int count = 0;
		for (int i = 0; i < ba.Length; i++) {
			if (ba[i]) count += 1;
		}

		if (count > 0) {
			int idx1 = Random.Range(0, count);
			int idx2 = 0;
			for (int i = 0; i < ba.Length; i++) {
				if (ba[i]) {
					if (idx2 == idx1) {
						return i;
					} else {
						idx2 += 1;
					}
				}
			}

			// Should never get here
			return -1;
		} else {
			return -1;
		}
	}

	public static List<Material> GetMaterialsFromObject(GameObject go)
	{
		List<Material> materials = new List<Material>();

		// Find the render materials
		Material m;
		foreach (MeshRenderer mr in go.GetComponentsInChildren<MeshRenderer>()) {
			for (int j = 0; j < mr.sharedMaterials.Length; j++) {
				m = mr.sharedMaterials[j];
				if (!materials.Contains(m)) {
					materials.Add(m);
				}
			}
		}

		return materials;
	}
}