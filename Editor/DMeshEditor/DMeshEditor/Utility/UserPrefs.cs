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

// Adapted from PlayerPrefs (for Unity) from PreviewLabs
using System.Collections;
using System.IO;
using System;
using System.Text.RegularExpressions;
using System.Drawing;

namespace OverloadLevelEditor
{
	public static class UserPrefs
	{
		private static Hashtable m_prefs_hashtable = new Hashtable();

		private static bool m_hashtable_changed = false;
		private static string m_serialized_output = "";
		private static string m_serialized_input = "";

		private const string PARAMETERS_SEPERATOR = ";";
		private const string KEY_VALUE_SEPERATOR = "|";

		private static readonly string file_name = "overload_dmesh_editor.userprefs";

		static UserPrefs()
		{
			//load previous settings
			StreamReader file_reader = null;

			if (File.Exists(file_name)) {
				file_reader = new StreamReader(file_name);

				m_serialized_input = file_reader.ReadLine();

				Deserialize();

				file_reader.Close();

				Utility.DebugLog("Preferences loaded from file");
			} else {
				Utility.DebugLog("No .userprefs file loaded");
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

			m_hashtable_changed = true;
		}

		public static void SetInt(string key, int value)
		{
			if (!m_prefs_hashtable.ContainsKey(key)) {
				m_prefs_hashtable.Add(key, value);
			} else {
				m_prefs_hashtable[key] = value;
			}

			m_hashtable_changed = true;
		}

		public static void SetLong(string key, long value)
		{
			if (!m_prefs_hashtable.ContainsKey(key)) {
				m_prefs_hashtable.Add(key, value);
			} else {
				m_prefs_hashtable[key] = value;
			}

			m_hashtable_changed = true;
		}

		public static void SetFloat(string key, float value)
		{
			if (!m_prefs_hashtable.ContainsKey(key)) {
				m_prefs_hashtable.Add(key, value);
			} else {
				m_prefs_hashtable[key] = value;
			}

			m_hashtable_changed = true;
		}

		public static void SetBool(string key, bool value)
		{
			if (!m_prefs_hashtable.ContainsKey(key)) {
				m_prefs_hashtable.Add(key, value);
			} else {
				m_prefs_hashtable[key] = value;
			}

			m_hashtable_changed = true;
		}

		public static void SetPoint(string key, Point value)
		{
			SetInt(key + ".x", value.X);
			SetInt(key + ".y", value.Y);
		}

		public static Point GetPoint(string key, Point defaultvalue)
		{
			Point p = new Point();
			p.X = GetInt(key + ".x", defaultvalue.X);
			p.Y = GetInt(key + ".y", defaultvalue.Y);
			return p;
		}

		public static string GetString(string key)
		{
			if (m_prefs_hashtable.ContainsKey(key)) {
				return m_prefs_hashtable[key].ToString();
			}

			return null;
		}

		public static string GetString(string key, string defaultvalue)
		{
			if (m_prefs_hashtable.ContainsKey(key)) {
				return m_prefs_hashtable[key].ToString();
			} else {
				m_prefs_hashtable.Add(key, defaultvalue);
				m_hashtable_changed = true;
				return defaultvalue;
			}
		}

		public static int GetInt(string key)
		{
			if (m_prefs_hashtable.ContainsKey(key)) {
				return (int)m_prefs_hashtable[key];
			}

			return 0;
		}

		public static int GetInt(string key, int defaultvalue)
		{
			if (m_prefs_hashtable.ContainsKey(key)) {
				return (int)m_prefs_hashtable[key];
			} else {
				m_prefs_hashtable.Add(key, defaultvalue);
				m_hashtable_changed = true;
				return defaultvalue;
			}
		}

		public static long GetLong(string key)
		{
			if (m_prefs_hashtable.ContainsKey(key)) {
				return (long)m_prefs_hashtable[key];
			}

			return 0;
		}

		public static long GetLong(string key, long defaultvalue)
		{
			if (m_prefs_hashtable.ContainsKey(key)) {
				return (long)m_prefs_hashtable[key];
			} else {
				m_prefs_hashtable.Add(key, defaultvalue);
				m_hashtable_changed = true;
				return defaultvalue;
			}
		}

		public static float GetFloat(string key)
		{
			if (m_prefs_hashtable.ContainsKey(key)) {
				return (float)m_prefs_hashtable[key];
			}

			return 0.0f;
		}

		public static float GetFloat(string key, float defaultvalue)
		{
			if (m_prefs_hashtable.ContainsKey(key)) {
				return (float)m_prefs_hashtable[key];
			} else {
				m_prefs_hashtable.Add(key, defaultvalue);
				m_hashtable_changed = true;
				return defaultvalue;
			}
		}

		public static bool GetBool(string key)
		{
			if (m_prefs_hashtable.ContainsKey(key)) {
				return (bool)m_prefs_hashtable[key];
			}

			return false;
		}

		public static bool GetBool(string key, bool defaultvalue)
		{
			if (m_prefs_hashtable.ContainsKey(key)) {
				return (bool)m_prefs_hashtable[key];
			} else {
				m_prefs_hashtable.Add(key, defaultvalue);
				m_hashtable_changed = true;
				return defaultvalue;
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

		public static void Flush()
		{
			if (m_hashtable_changed) {
				Serialize();

				StreamWriter file_writer = null;
				file_writer = File.CreateText(file_name);

				if (file_writer == null) {
					Utility.DebugLog("UserPrefs::Flush() opening file for writing failed: " + file_name);
				}

				file_writer.WriteLine(m_serialized_output);

				file_writer.Close();

				m_serialized_output = "";
			}
		}

		private static void Serialize()
		{
			IDictionaryEnumerator my_enumerator = m_prefs_hashtable.GetEnumerator();

			while (my_enumerator.MoveNext()) {
				if (m_serialized_output != "") {
					m_serialized_output += PARAMETERS_SEPERATOR;
				}

				string type_name = my_enumerator.Value.GetType().ToString();
				m_serialized_output += EscapeNonSeperators(my_enumerator.Key.ToString()) + KEY_VALUE_SEPERATOR + ValueToString(my_enumerator.Value, type_name) + KEY_VALUE_SEPERATOR + SimpleTypeSymbol(type_name);
			}
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
				Utility.DebugLog("Unsupported type: " + type_name);
				return "U";
			}
		}

		private static void Deserialize()
		{
			string[] parameters = m_serialized_input.Split(new string[] { PARAMETERS_SEPERATOR }, System.StringSplitOptions.None);// StringSplitOptions.None);

			foreach (string parameter in parameters) {
				string[] parameterContent = parameter.Split(new string[] { KEY_VALUE_SEPERATOR }, System.StringSplitOptions.None);

				m_prefs_hashtable.Add(DeEscapeNonSeperators(parameterContent[0]), GetTypevalueSimple(parameterContent[2], parameterContent[1]));

				if (parameterContent.Length > 3) {
					Utility.DebugLog("UserPrefs::Deserialize() parameterContent has " + parameterContent.Length + " elements");
				}
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

		public static object GetTypevalueSimple(string type_name, string value)
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
				Utility.DebugLog("Unsupported type: " + type_name);
			}

			return null;
		}
	}
}