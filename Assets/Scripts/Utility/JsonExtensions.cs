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
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using CodeStage.AntiCheat.ObscuredTypes;

public static class JsonExtensions
{

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
			if (float.TryParse((string)tok, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out res)) {
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

	//There's another version of this function elsewhere that works with OpenTK.Vector3.
	//I don't know how they're both able to exist, but everything seems fine.
	public static Vector3 GetVector3(this JToken tok, Vector3? defaultValue = null)
	{
		if (!tok.IsValid() || (tok.Type != JTokenType.Array) || (((JArray)tok).Count != 3)) {
			if (defaultValue.HasValue) {
				return defaultValue.Value;
			}
			return Vector3.zero;
		}

		Vector3 vec = new Vector3();
		var asArray = (JArray)tok;
		for (int i = 0; i < 3; ++i) {
			var val = asArray[i];
			if (val == null || ((val.Type != JTokenType.Integer) && (val.Type != JTokenType.Float))) {
				if (defaultValue.HasValue) {
					return defaultValue.Value;
				}
				return Vector3.zero;
			}
			vec[i] = (float)val;
		}
		return vec;
	}

	public static Vector3 GetVector2(this JToken tok, Vector3? defaultValue = null)
	{
		if (!tok.IsValid() || (tok.Type != JTokenType.Array) || (((JArray)tok).Count != 2)) {
			if (defaultValue.HasValue) {
				return defaultValue.Value;
			}
			return Vector3.zero;
		}

		Vector3 vec = new Vector2();
		var asArray = (JArray)tok;
		for (int i = 0; i < 2; ++i) {
			var val = asArray[i];
			if (val == null || ((val.Type != JTokenType.Integer) && (val.Type != JTokenType.Float))) {
				if (defaultValue.HasValue) {
					return defaultValue.Value;
				}
				return Vector2.zero;
			}
			vec[i] = (float)val;
		}
		return vec;
	}

	public static Quaternion GetQuat(this JToken tok, Quaternion? defaultValue = null)
	{
		if (!tok.IsValid() || (tok.Type != JTokenType.Array) || (((JArray)tok).Count != 4)) {
			if (defaultValue.HasValue) {
				return defaultValue.Value;
			}
			return Quaternion.identity;
		}

		Quaternion quat = new Quaternion();
		var asArray = (JArray)tok;
		for (int i = 0; i < 4; ++i) {
			var val = asArray[i];
			if (val == null || ((val.Type != JTokenType.Integer) && (val.Type != JTokenType.Float))) {
				if (defaultValue.HasValue) {
					return defaultValue.Value;
				}
				return Quaternion.identity;
			}
			quat[i] = (float)val;
		}
		return quat;
	}

	public static void GetArray(this JToken tok, int[] array, int defaultValue = 0, int length = -1)
	{
		if (length == -1) {
			length = array.Length;
		}

		if (!tok.IsValid() || (tok.Type != JTokenType.Array) || (((JArray)tok).Count != length)) {
			goto error_return;
		}

		var asArray = (JArray)tok;
		for (int i = 0; i < length; ++i) {
			JToken val = asArray[i];
			if (val == null || (val.Type != JTokenType.Integer)) {
				goto error_return;
			}
			array[i] = (int)val;
		}
		return;

		error_return:
		Assert.True(false);
		for (int i = 0; i < length; ++i) {
			array[i] = defaultValue;
		}
		return;
	}

	public static void GetArray(this JToken tok, ObscuredInt[] array, int defaultValue = 0, int length = -1)
	{
		if (length == -1) {
			length = array.Length;
		}

		if (!tok.IsValid() || (tok.Type != JTokenType.Array) || (((JArray)tok).Count != length)) {
			goto error_return;
		}

		var asArray = (JArray)tok;
		for (int i = 0; i < length; ++i) {
			JToken val = asArray[i];
			if (val == null || (val.Type != JTokenType.Integer)) {
				goto error_return;
			}
			array[i] = (int)val;
		}
		return;

		error_return:
		Assert.True(false);
		for (int i = 0; i < length; ++i) {
			array[i] = defaultValue;
		}
		return;
	}

	public static void GetArray(this JToken tok, float[] array, float defaultValue = 0.0f, int length = -1)
	{
		if (length == -1) {
			length = array.Length;
		}

		if (!tok.IsValid() || (tok.Type != JTokenType.Array) || (((JArray)tok).Count != length)) {
			goto error_return;
		}

		var asArray = (JArray)tok;
		for (int i = 0; i < length; ++i) {
			JToken val = asArray[i];
			if (val == null || ((val.Type != JTokenType.Float) && (val.Type != JTokenType.Integer))) {
				goto error_return;
			}
			array[i] = (int)val;
		}
		return;

		error_return:
		Assert.True(false);
		for (int i = 0; i < length; ++i) {
			array[i] = defaultValue;
		}
		return;
	}

	public static void GetArray(this JToken tok, Vector3[] array, Vector3? defaultValue = null, int length = -1)
	{
		if (length == -1) {
			length = array.Length;
		}

		Vector3 value = defaultValue.HasValue ? defaultValue.Value : Vector3.zero;

		if (!tok.IsValid() || (tok.Type != JTokenType.Array) || (((JArray)tok).Count != length)) {
			goto error_return;
		}

		var asArray = (JArray)tok;
		for (int i = 0; i < length; ++i) {
			JToken val = asArray[i];
			if (!val.IsValid() || (val.Type != JTokenType.Array) || (((JArray)val).Count != 3)) {
				goto error_return;
			}
			array[i] = val.GetVector3(value);
		}
		return;

		error_return:
		Assert.True(false);
		for (int i = 0; i < length; ++i) {
			array[i] = value;
		}
		return;
	}

	public static void GetArray(this JToken tok, string[] array, string defaultValue = "", int length = -1)
	{
		if (length == -1) {
			length = array.Length;
		}

		if (!tok.IsValid() || (tok.Type != JTokenType.Array) || (((JArray)tok).Count != length)) {
			goto error_return;
		}

		var asArray = (JArray)tok;
		for (int i = 0; i < length; ++i) {
			JToken val = asArray[i];
			if (!val.IsValid() || (val.Type != JTokenType.String)) {
				goto error_return;
			}
			array[i] = val.GetString(defaultValue);
		}
		return;

		error_return:
		Assert.True(false);
		for (int i = 0; i < length; ++i) {
			array[i] = defaultValue;
		}
		return;
	}

	public static void GetArray(this JToken tok, bool[] array, bool defaultValue = false, int length = -1)
	{
		if (length == -1) {
			length = array.Length;
		}

		if (!tok.IsValid() || (tok.Type != JTokenType.Array) || (((JArray)tok).Count != length)) {
			goto error_return;
		}

		var asArray = (JArray)tok;
		for (int i = 0; i < length; ++i) {
			JToken val = asArray[i];
			if (!val.IsValid() || (val.Type != JTokenType.Boolean)) {
				goto error_return;
			}
			array[i] = val.GetBool(defaultValue);
		}
		return;

		error_return:
		Assert.True(false);
		for (int i = 0; i < length; ++i) {
			array[i] = defaultValue;
		}
		return;
	}

	public static bool[] GetBoolArray(this JToken tok, bool defaultValue = false, int length = -1)
	{
		if (!tok.IsValid() || (tok.Type != JTokenType.Array)) {
			return null;
		}

		bool[] array = new bool[((JArray)tok).Count];

		tok.GetArray(array, defaultValue, length);

		return array;
	}


	//This version meant for enums.  If gets called for other types (such as Vector3) will generate an error.
	public static void GetArray<T>(this JToken tok, T[] array, int? defaultValue = null, int length = -1)
	{
		if (length == -1) {
			length = array.Length;
		}

		if (!tok.IsValid() || (tok.Type != JTokenType.Array) || (((JArray)tok).Count != length)) {
			goto error_return;
		}

		JArray asArray = (JArray)tok;
		for (int i = 0; i < length; ++i) {
			JToken val = asArray[i];
			if (val == null || (val.Type != JTokenType.Integer)) {
				goto error_return;
			}
			array.SetValue((int)val, i);
		}
		return;

		error_return:
		Assert.True(false);
		int value = defaultValue.HasValue ? defaultValue.Value : 0;
		for (int i = 0; i < length; ++i) {
			array.SetValue(value, i);
		}
		return;
	}

	public static void GetList<T>(this JToken tok, List<T> list, Func<JToken,T> deserializer)
	{
		list.Clear();

		if (!tok.IsValid() || (tok.Type != JTokenType.Array)) {
			return;
		}

		foreach (JToken item in (JArray)tok) {
			if (item == null || (item.Type != JTokenType.Object)) {
				return;
			}
			list.Add(deserializer(item));
		}
	}

	public static void GetList(this JToken tok, List<int> list)
	{
		list.Clear();

		if (!tok.IsValid() || (tok.Type != JTokenType.Array)) {
			return;
		}

		foreach (JToken item in (JArray)tok) {
			if (item == null || (item.Type != JTokenType.Integer)) {
				return;
			}
			list.Add(item.GetInt());
		}
	}

	public static void GetList(this JToken tok, List<float> list)
	{
		list.Clear();

		if (!tok.IsValid() || (tok.Type != JTokenType.Array)) {
			return;
		}

		foreach (JToken item in (JArray)tok) {
			if (item == null || (item.Type != JTokenType.Float)) {
				return;
			}
			list.Add(item.GetFloat());
		}
	}
		
	public static JToken Serialize(this Vector3 v)
	{
		return new JArray(v[0], v[1], v[2]);
	}

	public static JToken Serialize(this Vector2 v)
	{	
		return new JArray(v[0], v[1]);
	}

	public static JToken Serialize(this Quaternion q)
	{
		return new JArray(q[0], q[1], q[2], q[3]);
	}
}