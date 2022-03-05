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
using System.Linq;
using Overload;

public static class Assert
{
    public static void True(bool comp)
    {
        System.Diagnostics.Debug.Assert(comp);
    }

    public static void True(bool comp, string message)
    {
        System.Diagnostics.Debug.Assert(comp, message);
    }
}

public static class OpenTKHelpers
{
    public static UnityEngine.Vector2 ToUnity( this OpenTK.Vector2 v )
    {
        return new UnityEngine.Vector2( v.X, v.Y );
    }

    public static UnityEngine.Vector3 ToUnity( this OpenTK.Vector3 v)
    {
        return new UnityEngine.Vector3( v.X, v.Y, v.Z );
    }

    public static UnityEngine.Quaternion ToUnity( this OpenTK.Quaternion q )
    {
        return new UnityEngine.Quaternion( q.X, q.Y, q.Z, q.W );
    }

    public static OpenTK.Vector3 ToOpenTK( this UnityEngine.Vector3 v )
    {
        return new OpenTK.Vector3( v.x, v.y, v.z );
    }

    public static UnityEngine.Vector3 ToUnity( this System.Drawing.Color c )
    {
        float scale = 1.0f / 255.0f;
        float r = (float)c.R * scale;
        float g = (float)c.G * scale;
        float b = (float)c.B * scale;
        return new UnityEngine.Vector3( r, g, b );
    }
}

namespace UnityEditor
{
}

namespace UnityEngine
{
	namespace Rendering
	{
		public enum ReflectionProbeMode
		{
			Baked = 0,
			Realtime = 1,
		}

		public enum ReflectionProbeRefreshMode
		{
			OnAwake = 0,
			ViaScripting = 2
		}

		public enum ReflectionProbeClearFlags
		{
			SolidColor = 2
		}
	}

	public enum LightType
	{
		Spot,
		Directional,
		Point,
		Area
	}

	public enum LightShadows
	{
		None,
		Hard
	}

	public class HideInInspectorAttribute : System.Attribute
	{

	}

	public class Debug
	{
		public delegate void OutputLogFunction(int level, string msg);
		public static OutputLogFunction LogCallback = null;

		public static void Log(string msg)
		{
			OverloadLevelEditor.Utility.DebugLog(string.Format("INFO: {0}", msg));
			if (LogCallback != null) {
				LogCallback(0, msg);
				return;
			}
		}

		public static void LogWarning(string msg)
		{
			OverloadLevelEditor.Utility.DebugLog(string.Format("WARN: {0}", msg));
			if (LogCallback != null) {
				LogCallback(1, msg);
			}
		}

		public static void LogError(string msg)
		{
			OverloadLevelEditor.Utility.DebugLog(string.Format("ERROR: {0}", msg));
			if (LogCallback != null) {
				LogCallback(2, msg);
			}
		}

		public static void LogFormat(string msg, params object[] args)
		{
			Log(string.Format(msg, args));
		}

		public static void LogWarningFormat(string msg, params object[] args)
		{
			LogWarning(string.Format(msg, args));
		}

		public static void LogErrorFormat(string msg, params object[] args)
		{
			LogError(string.Format(msg, args));
		}
	}

	public class Application
	{
		public static string dataPath;
	}

	public class Object
	{
	}

	public class GameObject
	{
	}

	public class Material
	{
		public Material(string materialLookupName, OverloadLevelConverter.MeshBuilder.GeometryType geomType)
		{
			m_materialLookupName = materialLookupName;
			m_geomType = geomType;
		}

		public string m_materialLookupName;
		public OverloadLevelConverter.MeshBuilder.GeometryType m_geomType;
	}

	public struct Bounds
	{
		public Vector3 center;
		public Vector3 extents;

		public Bounds(Vector3 _center, Vector3 _extents)
		{
			this.center = _center;
			this.extents = _extents;
		}

		public Vector3 max
		{
			get { return center + extents; }
		}

		public Vector3 min
		{
			get { return center - extents; }
		}

		public Vector3 size
		{
			get { return extents * 2.0f; }
		}

		public bool IntersectRay(Vector3 origin, Vector3 direction)
		{
			Vector3 invDirection = 1.0f / direction;
			int rSign0 = invDirection.x < 0.0f ? 1 : 0;
			int rSign1 = invDirection.y < 0.0f ? 1 : 0;
			int rSign2 = invDirection.z < 0.0f ? 1 : 0;

			Vector3[] bounds = new Vector3[2] { min, max };

			float tmin = (bounds[rSign0].x - origin.x) * invDirection.x;
			float tmax = (bounds[1 - rSign0].x - origin.x) * invDirection.x;
			float tymin = (bounds[rSign1].y - origin.y) * invDirection.y;
			float tymax = (bounds[1 - rSign1].y - origin.y) * invDirection.y;

			if ((tmin > tymax) || (tymin > tmax)) {
				return false;
			}
			if (tymin > tmin) {
				tmin = tymin;
			}
			if (tymax < tmax) {
				tmax = tymax;
			}

			float tzmin = (bounds[rSign2].z - origin.z) * invDirection.z;
			float tzmax = (bounds[1 - rSign2].z - origin.z) * invDirection.z;

			if ((tmin > tzmax) || (tzmin > tmax)) {
				return false;
			}
			if (tzmin > tmin) {
				tmin = tzmin;
			}
			if (tzmax < tmax) {
				tmax = tzmax;
			}

			if (tmin < 0.0f && tmax < 0.0f) {
				return false;
			}

			return true;
		}
	}

    public enum TextureFormat
    {
        ARGB32,
        RGB24,
        Alpha8
    }

    public enum FilterMode
    {
        Bilinear,
        Point
    }

    public class Texture2D : UnityEngine.Object
    {
        public Texture2D(int _width, int _height, TextureFormat _format, bool _mipmap)
        {
            this.width = _width;
            this.height = _height;
            this.format = _format;
            this.mipmap = _mipmap;
        }

        public FilterMode filterMode = FilterMode.Bilinear;
        public string name = string.Empty;
        public Color32[] pixelData = new Color32[0];
        public int width;
        public int height;
        public TextureFormat format;
        public bool mipmap;

        public void SetPixels32(Color32[] colorData)
        {
            pixelData = colorData;
        }

        public Color32[] GetPixels32()
        {
            return pixelData;
        }

        public void Apply()
        {
        }
    }

	public class Mesh : UnityEngine.Object
	{
		public string name = string.Empty;

		public Vector2[] uv
		{
			get { return m_uv; }
			set
			{
				m_uv = value;
			}
		}

		public Vector2[] uv2
		{
			get { return m_uv2; }
			set
			{
				m_uv2 = value;
			}
		}

		public Vector2[] uv3
		{
			get { return m_uv3; }
			set
			{
				m_uv3 = value;
			}
		}

		public Vector3[] vertices
		{
			get { return m_vertices; }
			set
			{
				m_vertices = value;
			}
		}

		public Vector3[] normals
		{
			get { return m_normals; }
			set
			{
				m_normals = value;
			}
		}

		public Vector4[] tangents
		{
			get { return m_tangents; }
			set
			{
				m_tangents = value;
			}
		}

		public Color[] colors
		{
			get { return m_colors; }
			set
			{
				m_colors = value;
			}
		}

		public Color32[] colors32
		{
			get { return m_colors32; }
			set
			{
				m_colors32 = value;
			}
		}

		public BoneWeight[] boneWeights
		{
			get { return m_boneWeights; }
			set
			{
				m_boneWeights = value;
			}
		}

		public Matrix4x4[] bindposes
		{
			get { return m_bindposes; }
			set
			{
				m_bindposes = value;
			}
		}

		public Bounds bounds { get; private set; }

		private Vector2[] m_uv = new Vector2[0];
		private Vector2[] m_uv2 = new Vector2[0];
		private Vector2[] m_uv3 = new Vector2[0];
		private Vector3[] m_vertices = new Vector3[0];
		private Vector3[] m_normals = new Vector3[0];
		private Vector4[] m_tangents = new Vector4[0];
		private Color[] m_colors = new Color[0];
		private Color32[] m_colors32 = new Color32[0];
		private BoneWeight[] m_boneWeights = new BoneWeight[0];
		private Matrix4x4[] m_bindposes = new Matrix4x4[0];
		private List<int[]> m_triangleIndicesBySubmesh = new List<int[]>();

		public Mesh()
		{
			this.bounds = new Bounds(Vector3.zero, Vector3.zero);
		}

		public int subMeshCount
		{
			get { return m_triangleIndicesBySubmesh.Count; }
			set
			{
				if (value == m_triangleIndicesBySubmesh.Count) {
					return;
				}

				if (value < m_triangleIndicesBySubmesh.Count) {
					// Downsize
					m_triangleIndicesBySubmesh.RemoveRange(value, m_triangleIndicesBySubmesh.Count - value);
					return;
				}

				// Upsize
				for (int i = m_triangleIndicesBySubmesh.Count; i < value; ++i) {
					m_triangleIndicesBySubmesh.Add(new int[0]);
				}
			}
		}

		public int[] GetTriangles(int submesh_index)
		{
			if (submesh_index < 0 || submesh_index >= m_triangleIndicesBySubmesh.Count) {
				throw new ArgumentOutOfRangeException();
			}

			return m_triangleIndicesBySubmesh[submesh_index];
		}

		public void SetTriangles(int[] triangles, int submesh_index)
		{
			if (submesh_index < 0 || submesh_index >= m_triangleIndicesBySubmesh.Count) {
				throw new ArgumentOutOfRangeException();
			}

			if (triangles == null) {
				triangles = new int[0];
			}

			m_triangleIndicesBySubmesh[submesh_index] = triangles;
			RecalculateBounds();
		}

		public void RecalculateBounds()
		{
			Vector3 minXYZ = new Vector3(System.Single.MaxValue, System.Single.MaxValue, System.Single.MaxValue);
			Vector3 maxXYZ = -minXYZ;

			for (int submeshIndex = 0; submeshIndex < m_triangleIndicesBySubmesh.Count; ++submeshIndex) {
				int[] tris = m_triangleIndicesBySubmesh[submeshIndex];
				int numTriVerts = tris.Length;
				for (int tv = 0; tv < numTriVerts; ++tv) {
					Vector3 v = vertices[tris[tv]];

					minXYZ = Vector3.Min(minXYZ, v);
					maxXYZ = Vector3.Max(maxXYZ, v);
				}
			}

			Vector3 center = (maxXYZ + minXYZ) * 0.5f;
			Vector3 extents = maxXYZ - center;
			this.bounds = new Bounds(center, extents);
		}

		public void RecalculateNormals()
		{
		}
	}

	public class MonoBehaviour
	{
	}

	public class ScriptableObject : UnityEngine.Object
	{
		public string name { get; set; }

		public static T CreateInstance<T>()
		{
			return (T)Activator.CreateInstance<T>();
		}
	}

	public class RaycastHit
	{
	}

	public static class Physics
	{
		public static bool Linecast(Vector3 p1, Vector3 p2, int layer_mask)
		{
			return OverloadLevelExport.SceneBroker.ActiveSceneBrokerInstance.GetPhysicsScene().Linecast(p1, p2, layer_mask);
		}

		public static bool Raycast(Vector3 pos, Vector3 dir, out RaycastHit hit, float length, int layer_mask)
		{
			hit = null;
			return false;
		}
	}
}
