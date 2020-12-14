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
using System.Linq.Expressions;
#if OVERLOAD_LEVEL_EDITOR
using System.Security.Cryptography;
#endif

public class SerializationFailureException : Exception
{
}

public class OverloadLevelConvertSerializer : IDisposable
{
	public delegate object SerializerDelegate(object obj, Type type, OverloadLevelConvertSerializer serializer);
	static Dictionary<Type, SerializerDelegate> s_registeredTypes = new Dictionary<Type, SerializerDelegate>();

#if OVERLOAD_LEVEL_EDITOR
	static UInt64 CalculateHashForType(Type type)
	{
		using (SHA1 sha = new SHA1CryptoServiceProvider()) {

			foreach (var f in type.GetFields()) {

				byte[] fieldNameBytes = System.Text.UTF8Encoding.UTF8.GetBytes(f.Name);
				byte[] fieldTypeNameBytes = System.Text.UTF8Encoding.UTF8.GetBytes(f.FieldType.FullName);

				sha.TransformBlock(fieldNameBytes, 0, fieldNameBytes.Length, fieldNameBytes, 0);
				sha.TransformBlock(fieldTypeNameBytes, 0, fieldTypeNameBytes.Length, fieldTypeNameBytes, 0);
			}

			foreach (var p in type.GetProperties()) {

				byte[] fieldNameBytes = System.Text.UTF8Encoding.UTF8.GetBytes(p.Name);
				byte[] fieldTypeNameBytes = System.Text.UTF8Encoding.UTF8.GetBytes(p.PropertyType.FullName);

				sha.TransformBlock(fieldNameBytes, 0, fieldNameBytes.Length, fieldNameBytes, 0);
				sha.TransformBlock(fieldTypeNameBytes, 0, fieldTypeNameBytes.Length, fieldTypeNameBytes, 0);
			}


			byte[] typeNameBytes = System.Text.UTF8Encoding.UTF8.GetBytes(type.FullName);
			sha.TransformFinalBlock(typeNameBytes, 0, typeNameBytes.Length);
			byte[] hash = sha.Hash;

			for (uint i = 8; i < hash.Length; ++i) {
				hash[i & 7] %= hash[i];
			}

			return BitConverter.ToUInt64(hash, 0);
		}
	}
#endif

	public static void RegisterSerializer(Type type, SerializerDelegate serializer, UInt64 hash)
	{
#if OVERLOAD_LEVEL_EDITOR
		UInt64 currHash = CalculateHashForType(type);
		// If you hit this assert, it means that the type has changed since it the serialize code
		// was written for it. You need to update the serializer function and the hash. I put this
		// in here to catch if anyone subtley changes a type I serialize out.
		System.Diagnostics.Debug.Assert(currHash == hash, "Type has changed", "The type {0} has changed since its serializer was written - got {1} expected {2}", type.FullName, hash.ToString("X16"), currHash.ToString("X16"));
#endif

		s_registeredTypes[type] = serializer;
	}

	public static bool HasRegisteredSerializer(Type type)
	{
		return s_registeredTypes.ContainsKey(type);
	}


	System.IO.Stream m_stream;
	uint m_version = 0;
	bool m_writing;
	byte[] m_tempReadBuffer;
	byte[] m_tempReadBuffer16;
	Dictionary<Type, System.Reflection.MethodInfo> m_serializeBaseTypeMethodLookup = new Dictionary<Type, System.Reflection.MethodInfo>();

	public bool IsWriting
	{
		get { return m_writing; }
	}

	public uint Version
	{
		get { return m_version; }
		private set
		{
			if (m_version == 0) {
				m_version = value;
			}
		}
	}

	public object Context
	{
		get; set;
	}

	public void SetVersion(uint version)
	{
		this.Version = version;
	}

	private void InternalConstruct(System.IO.Stream stream, bool writing)
	{
		Type thisType = typeof(OverloadLevelConvertSerializer);
		System.Reflection.BindingFlags bindingFlags = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public;

		m_writing = writing;
		m_stream = stream;

		if (writing) {
			m_tempReadBuffer = null;

			m_serializeBaseTypeMethodLookup[typeof(string)] = thisType.GetMethod("SerializeOut_string", bindingFlags);
			m_serializeBaseTypeMethodLookup[typeof(bool)] = thisType.GetMethod("SerializeOut_bool", bindingFlags);
			m_serializeBaseTypeMethodLookup[typeof(byte)] = thisType.GetMethod("SerializeOut_byte", bindingFlags);
			m_serializeBaseTypeMethodLookup[typeof(sbyte)] = thisType.GetMethod("SerializeOut_sbyte", bindingFlags);
			m_serializeBaseTypeMethodLookup[typeof(char)] = thisType.GetMethod("SerializeOut_char", bindingFlags);
			m_serializeBaseTypeMethodLookup[typeof(UInt16)] = thisType.GetMethod("SerializeOut_uint16", bindingFlags);
			m_serializeBaseTypeMethodLookup[typeof(Int16)] = thisType.GetMethod("SerializeOut_int16", bindingFlags);
			m_serializeBaseTypeMethodLookup[typeof(UInt32)] = thisType.GetMethod("SerializeOut_uint32", bindingFlags);
			m_serializeBaseTypeMethodLookup[typeof(Int32)] = thisType.GetMethod("SerializeOut_int32", bindingFlags);
			m_serializeBaseTypeMethodLookup[typeof(UInt64)] = thisType.GetMethod("SerializeOut_uint64", bindingFlags);
			m_serializeBaseTypeMethodLookup[typeof(Int64)] = thisType.GetMethod("SerializeOut_int64", bindingFlags);
			m_serializeBaseTypeMethodLookup[typeof(float)] = thisType.GetMethod("SerializeOut_float", bindingFlags);
			m_serializeBaseTypeMethodLookup[typeof(double)] = thisType.GetMethod("SerializeOut_double", bindingFlags);
			m_serializeBaseTypeMethodLookup[typeof(Guid)] = thisType.GetMethod("SerializeOut_guid", bindingFlags);
			m_serializeBaseTypeMethodLookup[typeof(UnityEngine.Vector2)] = thisType.GetMethod("SerializeOut_vector2", bindingFlags);
			m_serializeBaseTypeMethodLookup[typeof(UnityEngine.Vector3)] = thisType.GetMethod("SerializeOut_vector3", bindingFlags);
			m_serializeBaseTypeMethodLookup[typeof(UnityEngine.Vector4)] = thisType.GetMethod("SerializeOut_vector4", bindingFlags);
			m_serializeBaseTypeMethodLookup[typeof(UnityEngine.Color)] = thisType.GetMethod("SerializeOut_color", bindingFlags);
			m_serializeBaseTypeMethodLookup[typeof(UnityEngine.Color32)] = thisType.GetMethod("SerializeOut_color32", bindingFlags);
			m_serializeBaseTypeMethodLookup[typeof(UnityEngine.Quaternion)] = thisType.GetMethod("SerializeOut_quat", bindingFlags);
			m_serializeBaseTypeMethodLookup[typeof(UnityEngine.Matrix4x4)] = thisType.GetMethod("SerializeOut_mat4x4", bindingFlags);
			m_serializeBaseTypeMethodLookup[typeof(UnityEngine.BoneWeight)] = thisType.GetMethod("SerializeOut_boneweight", bindingFlags);
#if OVERLOAD_LEVEL_EDITOR
			m_serializeBaseTypeMethodLookup[typeof(OpenTK.Vector3)] = thisType.GetMethod("SerializeOut_otk_vector3", bindingFlags);
#endif
		} else {
			m_tempReadBuffer = new byte[1024];
			m_tempReadBuffer16 = new byte[16];

			m_serializeBaseTypeMethodLookup[typeof(string)] = thisType.GetMethod("SerializeIn_string", bindingFlags);
			m_serializeBaseTypeMethodLookup[typeof(bool)] = thisType.GetMethod("SerializeIn_bool", bindingFlags);
			m_serializeBaseTypeMethodLookup[typeof(byte)] = thisType.GetMethod("SerializeIn_byte", bindingFlags);
			m_serializeBaseTypeMethodLookup[typeof(sbyte)] = thisType.GetMethod("SerializeIn_sbyte", bindingFlags);
			m_serializeBaseTypeMethodLookup[typeof(char)] = thisType.GetMethod("SerializeIn_char", bindingFlags);
			m_serializeBaseTypeMethodLookup[typeof(UInt16)] = thisType.GetMethod("SerializeIn_uint16", bindingFlags);
			m_serializeBaseTypeMethodLookup[typeof(Int16)] = thisType.GetMethod("SerializeIn_int16", bindingFlags);
			m_serializeBaseTypeMethodLookup[typeof(UInt32)] = thisType.GetMethod("SerializeIn_uint32", bindingFlags);
			m_serializeBaseTypeMethodLookup[typeof(Int32)] = thisType.GetMethod("SerializeIn_int32", bindingFlags);
			m_serializeBaseTypeMethodLookup[typeof(UInt64)] = thisType.GetMethod("SerializeIn_uint64", bindingFlags);
			m_serializeBaseTypeMethodLookup[typeof(Int64)] = thisType.GetMethod("SerializeIn_int64", bindingFlags);
			m_serializeBaseTypeMethodLookup[typeof(float)] = thisType.GetMethod("SerializeIn_float", bindingFlags);
			m_serializeBaseTypeMethodLookup[typeof(double)] = thisType.GetMethod("SerializeIn_double", bindingFlags);
			m_serializeBaseTypeMethodLookup[typeof(Guid)] = thisType.GetMethod("SerializeIn_guid", bindingFlags);
			m_serializeBaseTypeMethodLookup[typeof(UnityEngine.Vector2)] = thisType.GetMethod("SerializeIn_vector2", bindingFlags);
			m_serializeBaseTypeMethodLookup[typeof(UnityEngine.Vector3)] = thisType.GetMethod("SerializeIn_vector3", bindingFlags);
			m_serializeBaseTypeMethodLookup[typeof(UnityEngine.Vector4)] = thisType.GetMethod("SerializeIn_vector4", bindingFlags);
			m_serializeBaseTypeMethodLookup[typeof(UnityEngine.Color)] = thisType.GetMethod("SerializeIn_color", bindingFlags);
			m_serializeBaseTypeMethodLookup[typeof(UnityEngine.Color32)] = thisType.GetMethod("SerializeIn_color32", bindingFlags);
			m_serializeBaseTypeMethodLookup[typeof(UnityEngine.Quaternion)] = thisType.GetMethod("SerializeIn_quat", bindingFlags);
			m_serializeBaseTypeMethodLookup[typeof(UnityEngine.Matrix4x4)] = thisType.GetMethod("SerializeIn_mat4x4", bindingFlags);
			m_serializeBaseTypeMethodLookup[typeof(UnityEngine.BoneWeight)] = thisType.GetMethod("SerializeIn_boneweight", bindingFlags);
#if OVERLOAD_LEVEL_EDITOR
			m_serializeBaseTypeMethodLookup[typeof(OpenTK.Vector3)] = thisType.GetMethod("SerializeIn_otk_vector3", bindingFlags);
#endif
		}
	}

	public OverloadLevelConvertSerializer(System.IO.Stream readStream)
	{
		InternalConstruct(readStream, false);
	}

	public OverloadLevelConvertSerializer(string filePath, bool writing)
	{
		if (writing) {
			InternalConstruct(System.IO.File.Open(filePath, System.IO.FileMode.Create, System.IO.FileAccess.ReadWrite), writing);
		} else {
			InternalConstruct(System.IO.File.OpenRead(filePath), writing);
		}
	}

	public void Dispose()
	{
		// TODO: Do Dispose correctly
		m_stream.Dispose();
	}

	private void WriteBytes(byte[] data)
	{
		m_stream.Write(data, 0, data.Length);
	}

	private byte[] ReadBytes(int count)
	{
		if (count < 0) {
			throw new SerializationFailureException();
		}

		if (count >= m_tempReadBuffer.Length) {
			byte[] buffer = new byte[count];
			int result = m_stream.Read(buffer, 0, count);
			if (result != count) {
				throw new SerializationFailureException();
			}
			return buffer;
		} else if (count == 16) {
			// We have a specific 16-byte temp array because we read a lot of GUIDs
			// and the Guid constructor requires an exact sized 16-byte array.
			int result = m_stream.Read(m_tempReadBuffer16, 0, count);
			if (result != count) {
				throw new SerializationFailureException();
			}
			return m_tempReadBuffer16;
		} else {
			int result = m_stream.Read(m_tempReadBuffer, 0, count);
			if (result != count) {
				throw new SerializationFailureException();
			}
			return m_tempReadBuffer;
		}
	}

	public void StartCmd(UInt16 commandId)
	{
		if (m_writing) {
			WriteBytes(BitConverter.GetBytes(commandId));
		}
	}

	public void FinishCmd(UInt16 commandId)
	{
	}

	enum UnknownType : byte
	{
		String = 0,
		Bool,
		Byte,
		SByte,
		Char,
		UInt16,
		Int16,
		UInt32,
		Int32,
		UInt64,
		Int64,
		Float,
		Double,
		Guid,
		Vector2,
		Vector3,
		Vector4,
		Color,
		Quaternion,
		Enum,
		Object,
		OpenTKVector3,
		Matrix4x4,
		Color32,
		BoneWeight,
	}

	private byte GetUnknownEncoding(Type type, out bool needExtraTypeInfo)
	{
		byte arrayModifier = 0;
		Type elementType = type;

		if (type.IsArray) {
			elementType = type.GetElementType();
			arrayModifier = 0x80;
		}

		needExtraTypeInfo = false;

		UnknownType elemTypeVal;

		if (elementType.IsEnum) {
			elemTypeVal = UnknownType.Enum;
			needExtraTypeInfo = true;
		} else if (elementType == typeof(string)) {
			elemTypeVal = UnknownType.String;
		} else if (elementType == typeof(bool)) {
			elemTypeVal = UnknownType.Bool;
		} else if (elementType == typeof(byte)) {
			elemTypeVal = UnknownType.Byte;
		} else if (elementType == typeof(sbyte)) {
			elemTypeVal = UnknownType.SByte;
		} else if (elementType == typeof(char)) {
			elemTypeVal = UnknownType.Char;
		} else if (elementType == typeof(UInt16)) {
			elemTypeVal = UnknownType.UInt16;
		} else if (elementType == typeof(Int16)) {
			elemTypeVal = UnknownType.Int16;
		} else if (elementType == typeof(UInt32)) {
			elemTypeVal = UnknownType.UInt32;
		} else if (elementType == typeof(Int32)) {
			elemTypeVal = UnknownType.Int32;
		} else if (elementType == typeof(UInt64)) {
			elemTypeVal = UnknownType.UInt64;
		} else if (elementType == typeof(Int64)) {
			elemTypeVal = UnknownType.Int64;
		} else if (elementType == typeof(float)) {
			elemTypeVal = UnknownType.Float;
		} else if (elementType == typeof(double)) {
			elemTypeVal = UnknownType.Double;
		} else if (elementType == typeof(System.Guid)) {
			elemTypeVal = UnknownType.Guid;
		} else if (elementType == typeof(UnityEngine.Vector2)) {
			elemTypeVal = UnknownType.Vector2;
		} else if (elementType == typeof(UnityEngine.Vector3)) {
			elemTypeVal = UnknownType.Vector3;
		} else if (elementType == typeof(UnityEngine.Vector4)) {
			elemTypeVal = UnknownType.Vector4;
		} else if (elementType == typeof(UnityEngine.Color)) {
			elemTypeVal = UnknownType.Color;
		} else if (elementType == typeof(UnityEngine.Color32)) {
			elemTypeVal = UnknownType.Color32;
		} else if (elementType == typeof(UnityEngine.Quaternion)) {
			elemTypeVal = UnknownType.Quaternion;
		} else if (elementType == typeof(UnityEngine.Matrix4x4)) {
			elemTypeVal = UnknownType.Matrix4x4;
		} else if( elementType == typeof(UnityEngine.BoneWeight)) {
			elemTypeVal = UnknownType.BoneWeight;
#if OVERLOAD_LEVEL_EDITOR
		} else if (elementType == typeof(OpenTK.Vector3)) {
			elemTypeVal = UnknownType.OpenTKVector3;
#endif
		} else {
			elemTypeVal = UnknownType.Object;
			needExtraTypeInfo = true;
		}

		return (byte)((byte)elemTypeVal | arrayModifier);
	}

	// Returns true if the type is fully known, returns false if it is 'object' or 'enum'
	private bool ParseUnknownType(byte unknownType, out Type valueType, out bool isArray)
	{
		isArray = (unknownType & 0x80) == 0x80;
		switch (((UnknownType)(unknownType & 0x7F))) {
			case UnknownType.String:
				valueType = typeof(string);
				break;
			case UnknownType.Bool:
				valueType = typeof(bool);
				break;
			case UnknownType.Byte:
				valueType = typeof(byte);
				break;
			case UnknownType.SByte:
				valueType = typeof(sbyte);
				break;
			case UnknownType.Char:
				valueType = typeof(char);
				break;
			case UnknownType.UInt16:
				valueType = typeof(UInt16);
				break;
			case UnknownType.Int16:
				valueType = typeof(Int16);
				break;
			case UnknownType.UInt32:
				valueType = typeof(UInt32);
				break;
			case UnknownType.Int32:
				valueType = typeof(Int32);
				break;
			case UnknownType.UInt64:
				valueType = typeof(UInt64);
				break;
			case UnknownType.Int64:
				valueType = typeof(Int64);
				break;
			case UnknownType.Float:
				valueType = typeof(float);
				break;
			case UnknownType.Double:
				valueType = typeof(double);
				break;
			case UnknownType.Guid:
				valueType = typeof(Guid);
				break;
			case UnknownType.Vector2:
				valueType = typeof(UnityEngine.Vector2);
				break;
			case UnknownType.Vector3:
				valueType = typeof(UnityEngine.Vector3);
				break;
			case UnknownType.Vector4:
				valueType = typeof(UnityEngine.Vector4);
				break;
			case UnknownType.Color:
				valueType = typeof(UnityEngine.Color);
				break;
			case UnknownType.Color32:
				valueType = typeof(UnityEngine.Color32);
				break;
			case UnknownType.Quaternion:
				valueType = typeof(UnityEngine.Quaternion);
				break;
			case UnknownType.Matrix4x4:
				valueType = typeof(UnityEngine.Matrix4x4);
				break;
			case UnknownType.BoneWeight:
				valueType = typeof(UnityEngine.BoneWeight);
				break;
			case UnknownType.Enum:
				valueType = typeof(Enum);
				return false;
			case UnknownType.OpenTKVector3:
#if OVERLOAD_LEVEL_EDITOR
				valueType = typeof(OpenTK.Vector3);
#else
				valueType = typeof(UnityEngine.Vector3);
#endif
				break;
			case UnknownType.Object:
				valueType = typeof(object);
				return false;
			default:
				throw new SerializationFailureException();
		}

		return true;
	}

	public void SerializeOut_unknown(object obj, Type underlyingType)
	{
		Type elementType = underlyingType;
		if (underlyingType.IsArray) {
			elementType = underlyingType.GetElementType();
		}

		bool needExtraTypeInfo;
		byte encodingByte = GetUnknownEncoding(underlyingType, out needExtraTypeInfo);
		SerializeOut_byte(encodingByte);
		if (needExtraTypeInfo) {
			SerializeOut_string(elementType.FullName);
		}

		if (underlyingType.IsArray) {
			//
			// Special case handling for type[]
			//
			SerializeOut_array(elementType, obj as Array);
			return;
		}

		if (underlyingType.IsEnum) {
			//
			// Special case handling for enums
			//
			SerializeOut_int32(Convert.ToInt32(obj));
			return;
		}

		System.Reflection.MethodInfo baseTypeMethod;
		if (m_serializeBaseTypeMethodLookup.TryGetValue(underlyingType, out baseTypeMethod)) {
			//
			// Fundamental/base type
			//
			System.Diagnostics.Debug.Assert(((UnknownType)(encodingByte & 0x7F)) != UnknownType.Object);
			baseTypeMethod.Invoke(this, new object[1] { obj });
			return;
		}

		// Fallback to the generic object
		System.Diagnostics.Debug.Assert(((UnknownType)(encodingByte & 0x7F)) == UnknownType.Object);
		SerializeOut_object(obj);
	}

	public object SerializeIn_unknown()
	{
		byte encodingByte = SerializeIn_byte();
		Type elementType;
		bool isArray;
		bool isFullyKnownType = ParseUnknownType(encodingByte, out elementType, out isArray);

		if (!isFullyKnownType) {
			// Resolve the real element type
			string fullTypeName = SerializeIn_string();
			elementType = Type.GetType(fullTypeName);
			if (elementType == null) {
				elementType = typeof(UnityEngine.Vector2).Assembly.GetType(fullTypeName);
			}
			if (elementType == null)
				throw new SerializationFailureException();
		}

		if (isArray) {
			//
			// Special case handling for type[]
			//
			return SerializeIn_array(elementType);
		}

		if (((UnknownType)(encodingByte & 0x7F)) == UnknownType.Enum) {
			//
			// Special case handling for enums
			//
			Int32 intVal = SerializeIn_int32();
			return Enum.ToObject(elementType, intVal);
		}

		System.Reflection.MethodInfo baseTypeMethod;
		if (m_serializeBaseTypeMethodLookup.TryGetValue(elementType, out baseTypeMethod)) {
			//
			// Fundamental/base type
			//
			return baseTypeMethod.Invoke(this, null);
		}

		// Fallback to the generic object
		return SerializeIn_object(elementType);
	}

	public void SerializeX<TObject,TPropType>(TObject obj, Expression<Func<TObject, TPropType>> propertyRefExpr) where TObject : class
	{
		var expr = (MemberExpression)propertyRefExpr.Body;
		System.Reflection.PropertyInfo prop = expr.Member as System.Reflection.PropertyInfo;
		System.Reflection.FieldInfo field = null;
		Type underlyingType;

		if (prop != null) {
			underlyingType = prop.PropertyType;
		} else {
			field = expr.Member as System.Reflection.FieldInfo;
			underlyingType = field.FieldType;
		}

		if (underlyingType.IsArray) {
			//
			// Special case handling for type[]
			//
			Type elementType = underlyingType.GetElementType();
			if (m_writing) {
				object arrayValue = (prop != null) ? prop.GetValue(obj, null) : field.GetValue(obj);
				SerializeOut_array(elementType, arrayValue as Array);
			} else {
				Array arrayValue = SerializeIn_array(elementType);
				if (typeof(TObject).IsValueType) {
					object objBoxed = obj;
					if (prop != null) {
						prop.SetValue(objBoxed, arrayValue, null);
					} else {
						field.SetValue(objBoxed, arrayValue);
					}
					obj = (TObject)objBoxed;
				} else {
					if (prop != null) {
						prop.SetValue(obj, arrayValue, null);
					} else {
						field.SetValue(obj, arrayValue);
					}
				}
			}
			return;
		}

		if (underlyingType.IsEnum) {
			//
			// Special case handling for enums
			//
			if (m_writing) {
				object enumValue = (prop != null) ? prop.GetValue(obj, null) : field.GetValue(obj);
				SerializeOut_int32(Convert.ToInt32(enumValue));
			} else {
				Int32 intVal = SerializeIn_int32();
				if (typeof(TObject).IsValueType) {
					object objBoxed = obj;
					if (prop != null) {
						prop.SetValue(objBoxed, Enum.ToObject(underlyingType, intVal), null);
					} else {
						field.SetValue(objBoxed, Enum.ToObject(underlyingType, intVal));
					}
					obj = (TObject)objBoxed;
				} else {
					if (prop != null) {
						prop.SetValue(obj, Enum.ToObject(underlyingType, intVal), null);
					} else {
						field.SetValue(obj, Enum.ToObject(underlyingType, intVal));
					}
				}
			}
			return;
		}

		System.Reflection.MethodInfo baseTypeMethod;
		if (m_serializeBaseTypeMethodLookup.TryGetValue(underlyingType, out baseTypeMethod)) {
			//
			// Fundamental/base type
			//
			if (m_writing) {
				object val = (prop != null) ? prop.GetValue(obj, null) : field.GetValue(obj);
				baseTypeMethod.Invoke(this, new object[1] { val });
			} else {
				object val = baseTypeMethod.Invoke(this, null);
				if (typeof(TObject).IsValueType) {
					object objBoxed = obj;
					if (prop != null) {
						prop.SetValue(objBoxed, val, null);
					} else {
						field.SetValue(objBoxed, val);
					}
					obj = (TObject)objBoxed;
				} else {
					if (prop != null) {
						prop.SetValue(obj, val, null);
					} else {
						field.SetValue(obj, val);
					}
				}
			}
			return;
		}

		// Fallback to the generic object
		if (m_writing) {
			object val = (prop != null) ? prop.GetValue(obj, null) : field.GetValue(obj);
			SerializeOut_object(val);
		} else {
			// Construct the object
			object inValue = SerializeIn_object(underlyingType);
			if (typeof(TObject).IsValueType) {
				object objBoxed = obj;
				if (prop != null) {
					prop.SetValue(objBoxed, inValue, null);
				} else {
					field.SetValue(objBoxed, inValue);
				}
				obj = (TObject)objBoxed;
			} else {
				if (prop != null) {
					prop.SetValue(obj, inValue, null);
				} else {
					field.SetValue(obj, inValue);
				}
			}
		}
	}

	public void SerializeOut(byte[] data, uint offset, uint count)
	{
		m_stream.Write(data, (int)offset, (int)count);
	}

	public void SerializeIn(byte[] data, uint offset, uint count)
	{
		int result = m_stream.Read(data, (int)offset, (int)count);
		if (result != (int)count) {
			throw new SerializationFailureException();
		}
	}

	public void SerializeOut_string(string data)
	{
		if (data == null) {
			// Special case for null
			SerializeOut_int32(-1);
			return;
		}

		byte[] dataBytes = System.Text.UTF8Encoding.UTF8.GetBytes(data);
		SerializeOut_int32(dataBytes.Length);

		m_stream.Write(dataBytes, 0, dataBytes.Length);
	}

	public string SerializeIn_string()
	{
		Int32 stringLen = SerializeIn_int32();
		if (stringLen == -1) {
			// Special case for null
			return null;
		}
		byte[] buffer = ReadBytes(stringLen);
		return System.Text.UTF8Encoding.UTF8.GetString(buffer, 0, stringLen);
	}

	public void SerializeOut_bool(bool data)
	{
		m_stream.WriteByte(data ? (byte)1 : (byte)0);
	}

	public bool SerializeIn_bool()
	{
		return m_stream.ReadByte() != 0;
	}

	public void SerializeOut_byte(byte data)
	{
		m_stream.WriteByte(data);
	}

	public byte SerializeIn_byte()
	{
		int result = m_stream.ReadByte();
		if (result == -1) {
			throw new SerializationFailureException();
		}
		return (byte)result;
	}

	public void SerializeOut_sbyte(sbyte data)
	{
		m_stream.WriteByte((byte)data);
	}

	public sbyte SerializeIn_sbyte()
	{
		int result = m_stream.ReadByte();
		if (result == -1) {
			throw new SerializationFailureException();
		}
		return (sbyte)((byte)result);
	}

	public void SerializeOut_char(char data)
	{
		WriteBytes(BitConverter.GetBytes(data));
	}

	public char SerializeIn_char()
	{
		return BitConverter.ToChar(ReadBytes(2), 0);
	}

	public void SerializeOut_uint16(UInt16 data)
	{
		WriteBytes(BitConverter.GetBytes(data));
	}

	public UInt16 SerializeIn_uint16()
	{
		return BitConverter.ToUInt16(ReadBytes(2), 0); ;
	}

	public void SerializeOut_int16(Int16 data)
	{
		WriteBytes(BitConverter.GetBytes(data));
	}

	public Int16 SerializeIn_int16()
	{
		return BitConverter.ToInt16(ReadBytes(2), 0);
	}

	public void SerializeOut_uint32(UInt32 data)
	{
		WriteBytes(BitConverter.GetBytes(data));
	}

	public UInt32 SerializeIn_uint32()
	{
		return BitConverter.ToUInt32(ReadBytes(4), 0);
	}

	public void SerializeOut_int32(Int32 data)
	{
		WriteBytes(BitConverter.GetBytes(data));
	}

	public Int32 SerializeIn_int32()
	{
		return BitConverter.ToInt32(ReadBytes(4), 0);
	}

	public void SerializeOut_uint64(UInt64 data)
	{
		WriteBytes(BitConverter.GetBytes(data));
	}

	public UInt64 SerializeIn_uint64()
	{
		return BitConverter.ToUInt64(ReadBytes(8), 0);
	}

	public void SerializeOut_int64(Int64 data)
	{
		WriteBytes(BitConverter.GetBytes(data));
	}

	public Int64 SerializeIn_int64()
	{
		return BitConverter.ToInt64(ReadBytes(8), 0);
	}

	public void SerializeOut_float(float data)
	{
		WriteBytes(BitConverter.GetBytes(data));
	}

	public float SerializeIn_float()
	{
		return BitConverter.ToSingle(ReadBytes(4), 0);
	}

	public void SerializeOut_double(double data)
	{
		WriteBytes(BitConverter.GetBytes(data));
	}

	public double SerializeIn_double()
	{
		return BitConverter.ToDouble(ReadBytes(8), 0);
	}

	public void SerializeOut_guid(Guid data)
	{
		WriteBytes(data.ToByteArray());
	}

	public Guid SerializeIn_guid()
	{
		return new Guid(ReadBytes(16));
	}

	public void SerializeOut_vector2(UnityEngine.Vector2 v)
	{
		SerializeOut_float(v.x);
		SerializeOut_float(v.y);
	}

	public UnityEngine.Vector2 SerializeIn_vector2()
	{
		UnityEngine.Vector2 v = new UnityEngine.Vector2();
		v.x = SerializeIn_float();
		v.y = SerializeIn_float();
		return v;
	}

	public void SerializeOut_vector3(UnityEngine.Vector3 v)
	{
		SerializeOut_float(v.x);
		SerializeOut_float(v.y);
		SerializeOut_float(v.z);
	}

	public UnityEngine.Vector3 SerializeIn_vector3()
	{
		UnityEngine.Vector3 v = new UnityEngine.Vector3();
		v.x = SerializeIn_float();
		v.y = SerializeIn_float();
		v.z = SerializeIn_float();
		return v;
	}

	public void SerializeOut_vector4(UnityEngine.Vector4 v)
	{
		SerializeOut_float(v.x);
		SerializeOut_float(v.y);
		SerializeOut_float(v.z);
		SerializeOut_float(v.w);
	}

	public UnityEngine.Vector4 SerializeIn_vector4()
	{
		UnityEngine.Vector4 v = new UnityEngine.Vector4();
		v.x = SerializeIn_float();
		v.y = SerializeIn_float();
		v.z = SerializeIn_float();
		v.w = SerializeIn_float();
		return v;
	}

	public void SerializeOut_color(UnityEngine.Color c)
	{
		SerializeOut_float(c.r);
		SerializeOut_float(c.g);
		SerializeOut_float(c.b);
		SerializeOut_float(c.a);
	}

	public UnityEngine.Color SerializeIn_color()
	{
		UnityEngine.Color c = new UnityEngine.Color();
		c.r = SerializeIn_float();
		c.g = SerializeIn_float();
		c.b = SerializeIn_float();
		c.a = SerializeIn_float();
		return c;
	}

    public void SerializeOut_color32(UnityEngine.Color32 c)
    {
        SerializeOut_byte(c.r);
        SerializeOut_byte(c.g);
        SerializeOut_byte(c.b);
        SerializeOut_byte(c.a);
    }

    public UnityEngine.Color32 SerializeIn_color32()
    {
        UnityEngine.Color32 c = new UnityEngine.Color32();
        c.r = SerializeIn_byte();
        c.g = SerializeIn_byte();
        c.b = SerializeIn_byte();
        c.a = SerializeIn_byte();
        return c;
    }

    public void SerializeOut_quat(UnityEngine.Quaternion q)
	{
		SerializeOut_float(q.x);
		SerializeOut_float(q.y);
		SerializeOut_float(q.z);
		SerializeOut_float(q.w);
	}

	public UnityEngine.Quaternion SerializeIn_quat()
	{
		UnityEngine.Quaternion q = new UnityEngine.Quaternion();
		q.x = SerializeIn_float();
		q.y = SerializeIn_float();
		q.z = SerializeIn_float();
		q.w = SerializeIn_float();
		return q;
	}

    public void SerializeOut_mat4x4(UnityEngine.Matrix4x4 v)
    {
        SerializeOut_vector4(v.GetRow(0));
        SerializeOut_vector4(v.GetRow(1));
        SerializeOut_vector4(v.GetRow(2));
        SerializeOut_vector4(v.GetRow(3));
    }

    public UnityEngine.Matrix4x4 SerializeIn_mat4x4()
    {
        var v = new UnityEngine.Matrix4x4();
        v.SetRow(0, SerializeIn_vector4());
        v.SetRow(1, SerializeIn_vector4());
        v.SetRow(2, SerializeIn_vector4());
        v.SetRow(3, SerializeIn_vector4());
        return v;
    }

	public void SerializeOut_boneweight(UnityEngine.BoneWeight bw)
	{
		SerializeOut_int32(bw.boneIndex0);
		SerializeOut_int32(bw.boneIndex1);
		SerializeOut_int32(bw.boneIndex2);
		SerializeOut_int32(bw.boneIndex3);
		SerializeOut_float(bw.weight0);
		SerializeOut_float(bw.weight1);
		SerializeOut_float(bw.weight2);
		SerializeOut_float(bw.weight3);
	}

	public UnityEngine.BoneWeight SerializeIn_boneweight()
	{
		var bw = new UnityEngine.BoneWeight();
		bw.boneIndex0 = SerializeIn_int32();
		bw.boneIndex1 = SerializeIn_int32();
		bw.boneIndex2 = SerializeIn_int32();
		bw.boneIndex3 = SerializeIn_int32();
		bw.weight0 = SerializeIn_float();
		bw.weight1 = SerializeIn_float();
		bw.weight2 = SerializeIn_float();
		bw.weight3 = SerializeIn_float();
		return bw;
	}

#if OVERLOAD_LEVEL_EDITOR
	public void SerializeOut_otk_vector3(OpenTK.Vector3 v)
	{
		SerializeOut_float(v.X);
		SerializeOut_float(v.Y);
		SerializeOut_float(v.Z);
	}

	public OpenTK.Vector3 SerializeIn_otk_vector3()
	{
		OpenTK.Vector3 v = new OpenTK.Vector3();
		v.X = SerializeIn_float();
		v.Y = SerializeIn_float();
		v.Z = SerializeIn_float();
		return v;
	}
#endif

	public void Serialize(byte[] data, uint offset, uint count)
	{
		if (m_writing) {
			SerializeOut(data, offset, count);
		} else {
			SerializeIn(data, offset, count);
		}
	}

	public void Serialize(ref string data)
	{
		if (m_writing) {
			SerializeOut_string(data);
		} else {
			data = SerializeIn_string();
		}
	}

	public void Serialize(ref bool data)
	{
		if (m_writing) {
			SerializeOut_bool(data);
		} else {
			data = SerializeIn_bool();
		}
	}

	public void Serialize(ref byte data)
	{
		if (m_writing) {
			SerializeOut_byte(data);
		} else {
			data = SerializeIn_byte();
		}
	}

	public void Serialize(ref sbyte data)
	{
		if (m_writing) {
			SerializeOut_sbyte(data);
		} else {
			data = SerializeIn_sbyte();
		}
	}

	public void Serialize(ref char data)
	{
		if (m_writing) {
			SerializeOut_char(data);
		} else {
			data = SerializeIn_char();
		}
	}

	public void Serialize(ref UInt16 data)
	{
		if (m_writing) {
			SerializeOut_uint16(data);
		} else {
			data = SerializeIn_uint16();
		}
	}

	public void Serialize(ref Int16 data)
	{
		if (m_writing) {
			SerializeOut_int16(data);
		} else {
			data = SerializeIn_int16();
		}
	}

	public void Serialize(ref UInt32 data)
	{
		if (m_writing) {
			SerializeOut_uint32(data);
		} else {
			data = SerializeIn_uint32();
		}
	}

	public void Serialize(ref Int32 data)
	{
		if (m_writing) {
			SerializeOut_int32(data);
		} else {
			data = SerializeIn_int32();
		}
	}

	public void Serialize(ref UInt64 data)
	{
		if (m_writing) {
			SerializeOut_uint64(data);
		} else {
			data = SerializeIn_uint64();
		}
	}

	public void Serialize(ref Int64 data)
	{
		if (m_writing) {
			SerializeOut_int64(data);
		} else {
			data = SerializeIn_int64();
		}
	}

	public void Serialize(ref float data)
	{
		if (m_writing) {
			SerializeOut_float(data);
		} else {
			data = SerializeIn_float();
		}
	}

	public void Serialize(ref double data)
	{
		if (m_writing) {
			SerializeOut_double(data);
		} else {
			data = SerializeIn_double();
		}
	}

	public void Serialize(ref Guid guid)
	{
		if (m_writing) {
			SerializeOut_guid(guid);
		} else {
			guid = SerializeIn_guid();
		}
	}

	public void Serialize(ref UnityEngine.Vector2 v)
	{
		if (m_writing) {
			SerializeOut_vector2(v);
		} else {
			v = SerializeIn_vector2();
		}
	}

	public void Serialize(ref UnityEngine.Vector3 v)
	{
		if (m_writing) {
			SerializeOut_vector3(v);
		} else {
			v = SerializeIn_vector3();
		}
	}

	public void Serialize(ref UnityEngine.Vector4 v)
	{
		if (m_writing) {
			SerializeOut_vector4(v);
		} else {
			v = SerializeIn_vector4();
		}
	}

    public void Serialize(ref UnityEngine.Matrix4x4 v)
    {
        if (m_writing) {
            SerializeOut_mat4x4(v);
        }
        else {
            v = SerializeIn_mat4x4();
        }
    }

    public void Serialize(ref UnityEngine.Color c)
	{
		if (m_writing) {
			SerializeOut_color(c);
		} else {
			c = SerializeIn_color();
		}
	}

    public void Serialize(ref UnityEngine.Color32 c)
    {
        if (m_writing) {
            SerializeOut_color32(c);
        }
        else {
            c = SerializeIn_color32();
        }
    }

	public void Serialize(ref UnityEngine.Quaternion q)
	{
		if (m_writing) {
			SerializeOut_quat(q);
		} else {
			q = SerializeIn_quat();
		}
	}

	public void Serialize(ref UnityEngine.BoneWeight bw)
	{
		if (m_writing) {
			SerializeOut_boneweight(bw);
		} else {
			bw = SerializeIn_boneweight();
		}
	}

#if OVERLOAD_LEVEL_EDITOR
	public void Serialize(ref OpenTK.Vector3 v)
	{
		if (m_writing) {
			SerializeOut_otk_vector3(v);
		} else {
			v = SerializeIn_otk_vector3();
		}
	}
#endif

	public void SerializeOut_object(object obj)
	{
		if (obj == null) {
			// Special case handling for null
			SerializeOut_byte(0);
			return;
		}

		Type objectType = obj.GetType();
		bool isScriptableObject = objectType.IsSubclassOf(typeof(UnityEngine.ScriptableObject));

		// Not null.
		// 1 -- normal object
		// 2 -- object derived from ScriptableObject
		SerializeOut_byte((byte)(isScriptableObject ? 2 : 1));

		SerializerDelegate serializer;
		if (!s_registeredTypes.TryGetValue(objectType, out serializer)) {
			throw new SerializationFailureException();
		}

		serializer(obj, objectType, this);
	}

	public object SerializeIn_object(Type objectType)
	{
		byte objectTypeInfo = SerializeIn_byte();
		if (objectTypeInfo == 0) {
			// Special case handling for null
			return null;
		}

		SerializerDelegate serializer;
		if (!s_registeredTypes.TryGetValue(objectType, out serializer)) {
			throw new SerializationFailureException();
		}

		// Create the object
		object resultObject;
		if (objectTypeInfo == 2) {
			// Object derived from ScriptableObject
#if OVERLOAD_LEVEL_EDITOR
			resultObject = Activator.CreateInstance(objectType);
#else
			resultObject = UnityEngine.ScriptableObject.CreateInstance(objectType);
#endif
		} else {
			// Normal object
			resultObject = Activator.CreateInstance(objectType);
		}
		resultObject = serializer(resultObject, objectType, this);

		return resultObject;
	}

	public void SerializeOut_ExistingObject(Guid id, object obj)
	{
		if (obj == null) {
			throw new SerializationFailureException();
		}

		Type objectType = obj.GetType();

		SerializerDelegate serializer;
		if (!s_registeredTypes.TryGetValue(objectType, out serializer)) {
			throw new SerializationFailureException();
		}

		serializer(obj, objectType, this);
	}

	public object SerializeIn_ExistingObject(object resultObject)
	{
		Type objectType = resultObject.GetType();
		System.Diagnostics.Debug.Assert(objectType.IsValueType == false); // Only can be done for reference types

		SerializerDelegate serializer;
		if (!s_registeredTypes.TryGetValue(objectType, out serializer)) {
			throw new SerializationFailureException();
		}

		return serializer(resultObject, objectType, this);
	}

	public void SerializeOut_array(Type elementType, Array arrayData)
	{
		if (arrayData == null) {
			// Write out a length of -1 to indicate null
			SerializeOut_int32(-1);
			return;
		}

		// Write out the real length
		int numToSerialize = arrayData.Length;
		SerializeOut_int32(numToSerialize);
		if (numToSerialize == 0) {
			return;
		}

		// Do we have a serialize function for the base type?
		Type serializerType = typeof(OverloadLevelConvertSerializer);
		Type[] serializeTypeArray = new Type[1] { elementType.MakeByRefType() };
		var methodInfo = serializerType.GetMethod("Serialize", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance, null, serializeTypeArray, null);
		var argArray = new object[1];
		
		for (int i = 0; i < numToSerialize; ++i) {

			if (methodInfo != null) {
				argArray[0] = arrayData.GetValue(i);
				methodInfo.Invoke(this, argArray);
			} else {
				SerializeOut_object(arrayData.GetValue(i));
			}
		}
	}

	public Array SerializeIn_array(Type elementType)
	{
		Int32 len = SerializeIn_int32();
		if (len == -1) {
			// Used to indicate a null array
			return null;
		}

		// Create the array
		Array arrayData = Array.CreateInstance(elementType, len);
		if (len == 0) {
			return arrayData;
		}

		// Do we have a serialize function for the base type?
		Type serializerType = typeof(OverloadLevelConvertSerializer);
		Type[] serializeTypeArray = new Type[1] { elementType.MakeByRefType() };
		var methodInfo = serializerType.GetMethod("Serialize", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance, null, serializeTypeArray, null);
		var argArray = new object[1];

		for (int i = 0; i < len; ++i) {
			if (methodInfo != null) {
				argArray[0] = arrayData.GetValue(i);
				methodInfo.Invoke(this, argArray);
				arrayData.SetValue(argArray[0], i);
			} else {
				arrayData.SetValue(SerializeIn_object(elementType), i);
			}
		}

		return arrayData;
	}
}
