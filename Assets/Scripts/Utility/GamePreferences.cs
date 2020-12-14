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
using System.Collections;
using System.IO;

namespace Overload
{
	public static class GamePreferences
	{
		private static Hashtable m_prefs_hashtable = new Hashtable();

		private static string m_serialized_data = "";

		private const string PARAMETERS_SEPERATOR = ";";
		private const string KEY_VALUE_SEPERATOR = ":";

		public static void ClearPilot()
		{
			m_serialized_data = "";
		}

		public static void Load(string filename)
		{
			DeleteAll();

			try {
				//load previous settings
				m_serialized_data = Platform.ReadTextUserData(filename).TrimEnd('\r', '\n');		//Remove newline from older files
				Deserialize();
			}
			catch (System.Exception ex) {
				GameManager.DebugOut(ex.GetType() + " in GamePreferences.Load: " + ex.Message);
			}
		}

		public static bool HasKey(string key)
		{
			return m_prefs_hashtable.ContainsKey(key);
		}

		public static void SetString(string key, string value)
		{
			if (!m_prefs_hashtable.ContainsKey(key)) {
				m_prefs_hashtable.Add(key, value);
			} else {
				m_prefs_hashtable[key] = value;
			}
		}

		public static void SetInt(string key, int value)
		{
			if (!m_prefs_hashtable.ContainsKey(key)) {
				m_prefs_hashtable.Add(key, value);
			} else {
				m_prefs_hashtable[key] = value;
			}
		}

		public static void SetLong(string key, long value)
		{
			if (!m_prefs_hashtable.ContainsKey(key)) {
				m_prefs_hashtable.Add(key, value);
			} else {
				m_prefs_hashtable[key] = value;
			}
		}

		public static void SetFloat(string key, float value)
		{
			if (!m_prefs_hashtable.ContainsKey(key)) {
				m_prefs_hashtable.Add(key, value);
			} else {
				m_prefs_hashtable[key] = value;
			}
		}

		public static void SetBool(string key, bool value)
		{
			if (!m_prefs_hashtable.ContainsKey(key)) {
				m_prefs_hashtable.Add(key, value);
			} else {
				m_prefs_hashtable[key] = value;
			}
		}

		public static string GetString(string key)
		{
			if (m_prefs_hashtable.ContainsKey(key)) {
				return m_prefs_hashtable[key].ToString();
			}

			return null;
		}

		public static string GetString(string key, string defaultValue)
		{
			if (m_prefs_hashtable.ContainsKey(key)) {
				return m_prefs_hashtable[key].ToString();
			} else {
				m_prefs_hashtable.Add(key, defaultValue);
				return defaultValue;
			}
		}

		public static int GetInt(string key)
		{
			if (m_prefs_hashtable.ContainsKey(key)) {
				return (int)m_prefs_hashtable[key];
			}

			return 0;
		}

		public static int GetInt(string key, int defaultValue)
		{
			if (m_prefs_hashtable.ContainsKey(key)) {
				return (int)m_prefs_hashtable[key];
			} else {
				m_prefs_hashtable.Add(key, defaultValue);
				return defaultValue;
			}
		}

		public static long GetLong(string key)
		{
			if (m_prefs_hashtable.ContainsKey(key)) {
				return (long)m_prefs_hashtable[key];
			}

			return 0;
		}

		public static long GetLong(string key, long defaultValue)
		{
			if (m_prefs_hashtable.ContainsKey(key)) {
				return (long)m_prefs_hashtable[key];
			} else {
				m_prefs_hashtable.Add(key, defaultValue);
				return defaultValue;
			}
		}

		public static float GetFloat(string key)
		{
			if (m_prefs_hashtable.ContainsKey(key)) {
				return (float)m_prefs_hashtable[key];
			}

			return 0.0f;
		}

		public static float GetFloat(string key, float defaultValue)
		{
			if (m_prefs_hashtable.ContainsKey(key)) {
				return (float)m_prefs_hashtable[key];
			} else {
				m_prefs_hashtable.Add(key, defaultValue);
				return defaultValue;
			}
		}

		public static bool GetBool(string key)
		{
			if (m_prefs_hashtable.ContainsKey(key)) {
				return (bool)m_prefs_hashtable[key];
			}

			return false;
		}

		public static bool GetBool(string key, bool defaultValue)
		{
			if (m_prefs_hashtable.ContainsKey(key)) {
				return (bool)m_prefs_hashtable[key];
			} else {
				m_prefs_hashtable.Add(key, defaultValue);
				return defaultValue;
			}
		}

		public static void DeleteKey(string key)
		{
			m_prefs_hashtable.Remove(key);
		}

		public static void DeleteAll()
		{
			m_prefs_hashtable.Clear();
		}

		public static void Flush(string filename)
		{
			string new_data = Serialize();

			if (m_serialized_data != new_data) {
				m_serialized_data = new_data;

				try {
					Platform.WriteTextUserData(filename, m_serialized_data);
				}
				catch (System.Exception ex) {
					GameManager.DebugOut(ex.GetType() + " in GamePreferences.Flush: " + ex.Message);
				}
			}
		}

		private static string Serialize()
		{
			string data = "";

			IDictionaryEnumerator my_enumerator = m_prefs_hashtable.GetEnumerator();

			while (my_enumerator.MoveNext()) {
				if (data != "") {
					data += PARAMETERS_SEPERATOR;
				}
				//serializedOutput += EscapeNonSeperators(myEnumerator.Key.ToString()) + " " + KEY_VALUE_SEPERATOR + " " + EscapeNonSeperators(myEnumerator.Value.ToString()) + " " + KEY_VALUE_SEPERATOR + " " + myEnumerator.Value.GetType();
				string typeName = my_enumerator.Value.GetType().ToString();
				data += EscapeNonSeperators(my_enumerator.Key.ToString()) + KEY_VALUE_SEPERATOR + ValueToString(my_enumerator.Value, typeName) + KEY_VALUE_SEPERATOR + SimpleTypeSymbol(typeName);
			}

			return data;
		}

		public static string ValueToString(object value, string type_name)
		{
			if (type_name == "System.Boolean") {
				return ((bool)value ? "T" : "F");
			} else {
				return value.ToString();
			}
		}

		private static string SimpleTypeSymbol(string type_name)
		{
			if (type_name == "System.String") {
				return "S";
			}
			if (type_name == "System.Int32") {
				return "I";
			}
			if (type_name == "System.Int64") {
				return "L";
			}
			if (type_name == "System.Boolean") {
				return "B";
			}
			if (type_name == "System.Single") {	// -> single = float
				return "F";
			} else {
				if (Debug.isDebugBuild) Debug.LogError("Unsupported type: " + type_name);
				return "U";
			}
		}

		private static void Deserialize()
		{
			string[] parameters = m_serialized_data.Split(new string[] { PARAMETERS_SEPERATOR }, System.StringSplitOptions.None);// StringSplitOptions.None);

			foreach (string parameter in parameters) {
				string[] parameterContent = parameter.Split(new string[] { KEY_VALUE_SEPERATOR }, System.StringSplitOptions.None);

				if (parameterContent.Length != 3) {
					Debug.LogWarning("PlayerPrefs::Deserialize() parameterContent has " + parameterContent.Length + " elements");
					return;
				}
				m_prefs_hashtable.Add(DeEscapeNonSeperators(parameterContent[0]), GetTypeValueSimple(parameterContent[2], parameterContent[1]));
			}
		}

		private static string EscapeNonSeperators(string input_to_escape)
		{
			input_to_escape = input_to_escape.Replace(KEY_VALUE_SEPERATOR, "\\" + KEY_VALUE_SEPERATOR);
			input_to_escape = input_to_escape.Replace(PARAMETERS_SEPERATOR, "\\" + PARAMETERS_SEPERATOR);
			return input_to_escape;
		}

		private static string DeEscapeNonSeperators(string input_to_deescape)
		{
			input_to_deescape = input_to_deescape.Replace("\\" + KEY_VALUE_SEPERATOR, KEY_VALUE_SEPERATOR);
			input_to_deescape = input_to_deescape.Replace("\\" + PARAMETERS_SEPERATOR, PARAMETERS_SEPERATOR);
			return input_to_deescape;
		}

		public static object GetTypeValueSimple(string type_name, string value)
		{
			if (type_name == "S") {
				return (object)value.ToString();
			}
			if (type_name == "I") {
				return (object)System.Convert.ToInt32(value);
			}
			if (type_name == "L") {
				return (object)System.Convert.ToInt64(value);
			}
			if (type_name == "B") {
				return (object)(value.ToString() == "T" ? true : false);
			}
			if (type_name == "F") {	// -> single = float
				return (object)System.Convert.ToSingle(value);
			} else {
				if (Debug.isDebugBuild) Debug.LogError("Unsupported type: " + type_name);
			}

			return null;
		}
	}
}