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
using Newtonsoft.Json.Linq;
using UnityEngine;

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

#if !OVERLOAD_LEVEL_EDITOR
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
#endif

}
