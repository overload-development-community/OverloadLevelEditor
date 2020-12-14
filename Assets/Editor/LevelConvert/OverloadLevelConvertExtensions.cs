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
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

public static class OverloadLevelConverterExtension
{
	public static Dictionary<string, TItem> SafeConvertToDictionary<T, TItem>(this IEnumerable<T> e, Func<T, string> getKey, Func<T, TItem> getItem, Func<T, string> getInfo)
	{
		var dict = new Dictionary<string, TItem>(StringComparer.InvariantCultureIgnoreCase);
		var dictInfo = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
		foreach (var item in e) {
			var key = getKey(item);
			if (key == null) {
				UnityEngine.Debug.LogError("Invalid object provided - internal error");
				return null;
			}

			var info = getInfo(item);
			if (dict.ContainsKey(key)) {
				var existingInfo = dictInfo[key];
				UnityEngine.Debug.LogError(string.Format("The items \"{0}\" and \"{1}\" both form \"{2}\"", info, existingInfo, key));
				return null;
			}

			dictInfo.Add(key, info);
			dict.Add(key, getItem(item));
		}

		return dict;
	}

	public static void RecalculateTangents(this Mesh mesh)
	{
		if (mesh.uv.Length == 0)
			return;

		const double kUVEpsilon = 0.000001;
		const float kEpsilon = 0.0001f;
		bool warnedAboutBadUVData = false;
		bool warnedAboutBadNormalData = false;
		bool warnedAboutBadTangentData = false;

		//speed up math by copying the mesh arrays
		Vector3[] vertices = mesh.vertices;
		Vector2[] uv = mesh.uv;
		Vector3[] normals = mesh.normals;
		int vertex_count = vertices.Length;

		Vector3[] tan1 = new Vector3[vertex_count];
		Vector3[] tan2 = new Vector3[vertex_count];
		Vector4[] tangents = new Vector4[vertex_count];

		int num_submeshes = mesh.subMeshCount;
		for (int submesh_index = 0; submesh_index < num_submeshes; ++submesh_index) {
			
			int[] triangle_indices = mesh.GetTriangles(submesh_index);
			int triangle_index_count = triangle_indices.Length;

			for (int a = 0; a < triangle_index_count; a += 3) {
				int i1 = triangle_indices[a + 0];
				int i2 = triangle_indices[a + 1];
				int i3 = triangle_indices[a + 2];

				Vector3 v1 = vertices[i1];
				Vector3 v2 = vertices[i2];
				Vector3 v3 = vertices[i3];

				Vector2 w1 = uv[i1];
				Vector2 w2 = uv[i2];
				Vector2 w3 = uv[i3];

				double x1 = v2.x - v1.x;
				double x2 = v3.x - v1.x;
				double y1 = v2.y - v1.y;
				double y2 = v3.y - v1.y;
				double z1 = v2.z - v1.z;
				double z2 = v3.z - v1.z;

				double s1 = w2.x - w1.x;
				double s2 = w3.x - w1.x;
				double t1 = w2.y - w1.y;
				double t2 = w3.y - w1.y;

				double den = ( s1 * t2 - s2 * t1 );
				if( Math.Abs( den ) <= kUVEpsilon ) {
					// Prevent divide by 0
					if( !warnedAboutBadUVData ) {
						warnedAboutBadUVData = true;
						Debug.LogWarningFormat( "Bad UV coordinates found during RecalculateTangents ({0})", mesh.name );
					}
					den = 1.0;
				}

				double r = 1.0 / den;

				Vector3 sdir = new Vector3((float)((t2 * x1 - t1 * x2) * r), (float)((t2 * y1 - t1 * y2) * r), (float)((t2 * z1 - t1 * z2) * r));
				Vector3 tdir = new Vector3((float)((s1 * x2 - s2 * x1) * r), (float)((s1 * y2 - s2 * y1) * r), (float)((s1 * z2 - s2 * z1) * r));

				tan1[i1] += sdir;
				tan1[i2] += sdir;
				tan1[i3] += sdir;

				tan2[i1] += tdir;
				tan2[i2] += tdir;
				tan2[i3] += tdir;
			}
		}

		for (int a = 0; a < vertex_count; ++a) {
			Vector3 n = normals[a];
			Vector3 t = tan1[a];

			if( n.magnitude <= kEpsilon ) {
				// Prevent a bad normal
				if( !warnedAboutBadNormalData ) {
					warnedAboutBadNormalData = true;
					Debug.LogWarningFormat( "Bad normal found during RecalculateTangents ({0})", mesh.name );
				}
				n = Vector3.forward;
			}

			if( t.magnitude <= kEpsilon ) {
				// Prevent a bad tangent basis by picking something
				if( !warnedAboutBadTangentData ) {
					warnedAboutBadTangentData = true;
					Debug.LogWarningFormat( "Bad tangent calculated during RecalculateTangents ({0})", mesh.name );
				}
				tan2[ a ] = Vector3.Cross( n, Vector3.right ).normalized;
				t = Vector3.Cross( tan2[ a ], n ).normalized;
			}

			//Vector3 tmp = (t - n * Vector3.Dot(n, t)).normalized;
			//tangents[a] = new Vector4(tmp.x, tmp.y, tmp.z);
			Vector3.OrthoNormalize(ref n, ref t);
			tangents[a].x = t.x;
			tangents[a].y = t.y;
			tangents[a].z = t.z;

			tangents[a].w = (Vector3.Dot(Vector3.Cross(n, t), tan2[a]) < 0.0f) ? -1.0f : 1.0f;
		}

		mesh.tangents = tangents;
	}

	/// <summary>
	///     Recalculate the normals of a mesh based on an angle threshold. This takes
	///     into account distinct vertices that have the same position.
	/// </summary>
	/// <param name="mesh"></param>
	/// <param name="angleSameMaterial">
	///     The smoothing angle. Note that triangles that already share the same vertex will be smooth regardless of the angle!
	///     If the vertex is not shared then triangle with normals within this angle and with the same material, are smoothed
	/// </param>
	/// <param name="angleDifferentMaterial">
	///     See angleSameMaterial. But this is if the triangles are of different materials.
	/// </param>
	/// <param name="normalLock">
	///		If provided, then for each normal in normals[], this says whether or not the vertex for this normal should
	///		be locked to that provided by normals[]. Otherwise, if false, that normal will be recalculated.
	/// </param>
	/// <param name="providedNormals">
	///		Paired with normalLock to decide whether we should calculate the normal
	/// </param>
	public static void RecalculateNormals( this Mesh mesh, float angleSameMaterial, float angleDifferentMaterial, bool[] normalLock, Vector3[] providedNormals )
	{
		const float kEpsilon = 0.0001f;
		bool warnedAboutDegenTris = false;
		bool warnedAboutZeroSumNormal = false;

		angleSameMaterial = angleSameMaterial * Mathf.Deg2Rad;
		angleDifferentMaterial = angleDifferentMaterial * Mathf.Deg2Rad;

		if( providedNormals == null ) {
			// If we were not given provided normals, then we don't do normal lock checking
			normalLock = null;
		}
		
		var vertices = mesh.vertices;
		var normals = new Vector3[ vertices.Length ];
		var pointInSpaceToRefTrisDictionary = new Dictionary<VertexKey, VertexEntry>( vertices.Length );

		if( normalLock != null ) {
			// Make sure all the arrays are the expected length
			Assert.True( normalLock.Length == providedNormals.Length );
			Assert.True( normalLock.Length == vertices.Length );
		}

		int numSubmeshes = mesh.subMeshCount;
		int numTotalTriangles = 0;
		for( int submeshIndex = 0; submeshIndex < numSubmeshes; ++submeshIndex ) {
			int[] triangles = mesh.GetTriangles( submeshIndex );
			numTotalTriangles += triangles.Length / 3;
		}

		var triNormals = new Vector3[ numTotalTriangles ]; //Holds the normal of each triangle
		int triOffset = 0;
		for( int submesh_index = 0; submesh_index < numSubmeshes; ++submesh_index ) {

			int[] triangles = mesh.GetTriangles( submesh_index );

			// Goes through all the triangles and gathers up data to be used later
			for( var i = 0; i < triangles.Length; i += 3 ) {
				int i1 = triangles[ i + 0 ];
				int i2 = triangles[ i + 1 ];
				int i3 = triangles[ i + 2 ];

				int i1_tri_id = 0;
				int i2_tri_id = 0;
				int i3_tri_id = 0;

				//Calculate the normal of the triangle
				Vector3 p1 = vertices[ i2 ] - vertices[ i1 ];
				Vector3 p2 = vertices[ i3 ] - vertices[ i1 ];
				Vector3 normal = Vector3.Cross( p1, p2 ).normalized;
				if( normal.magnitude < kEpsilon ) {
					// Prevent a bad triangle from generating a bad normal
					if( !warnedAboutDegenTris ) {
						warnedAboutDegenTris = true;
						Debug.LogWarningFormat( "Degenerate triangles found during RecalculateNormals ({0})", mesh.name );
					}
					normal = Vector3.forward;
				}
				int triIndex = ( i / 3 ) + triOffset;
				triNormals[ triIndex ] = normal;

				VertexEntry entry;
				VertexKey key;

				// For our purposes, each submesh is for a different material
				// But if we should ever have a material represented multiple times
				// in submeshes, then we need to adjust this as needed.
				int materialIndex = submesh_index;

				//For each of the three points of the triangle
				//  Add this triangle as part of the triangles they're connected to.
                
                // NOTE!!!! We pass false here for triangleIsPortal, because we assume that by the time this gets called, there are no portal triangles in the mesh.
                //  This is not necessarily the case if debugAddPortalsToRenderGeometry is true.  If we want to support that, we will need some way to feed the flag
                //  of whether each triangle is from a portal or not through the UnityEngine.Mesh and into this function somehow. -MFlavin 3/1/2017
				if( !pointInSpaceToRefTrisDictionary.TryGetValue( key = new VertexKey( vertices[ i1 ] ), out entry ) ) {
					entry = new VertexEntry();
					pointInSpaceToRefTrisDictionary.Add( key, entry );
				}
				entry.Add( i1, i1_tri_id, triIndex, materialIndex, false );

				if( !pointInSpaceToRefTrisDictionary.TryGetValue( key = new VertexKey( vertices[ i2 ] ), out entry ) ) {
					entry = new VertexEntry();
					pointInSpaceToRefTrisDictionary.Add( key, entry );
				}
				entry.Add( i2, i2_tri_id, triIndex, materialIndex, false );

				if( !pointInSpaceToRefTrisDictionary.TryGetValue( key = new VertexKey( vertices[ i3 ] ), out entry ) ) {
					entry = new VertexEntry();
					pointInSpaceToRefTrisDictionary.Add( key, entry );
				}
				entry.Add( i3, i3_tri_id, triIndex, materialIndex, false );
			}

			triOffset += triangles.Length / 3;
		}

		//Foreach point in space (not necessarily the same vertex index!)
		//{
		//  Foreach triangle T1 that point belongs to
		//  {
		//    Foreach other triangle T2 (including self) that point belongs to and that
		//    meets any of the following:
		//    1) The corresponding vertex is actually the same vertex
		//    2) The angle between the two triangles is less than the smoothing angle
		//    {
		//      > Add to temporary Vector3
		//    }
		//    > Normalize temporary Vector3 to find the average
		//    > Assign the normal to corresponding vertex of T1
		//  }
		//}
		foreach( var refTriangles in pointInSpaceToRefTrisDictionary.Values ) {
			for( var i = 0; i < refTriangles.Count; ++i ) {
				var sum = new Vector3();

				int materialI = refTriangles.MaterialIndex[ i ];
				int vertexIndexI = refTriangles.VertexIndex[ i ];
				int triangleIndexI = refTriangles.TriangleIndex[ i ];

				for( var j = 0; j < refTriangles.Count; ++j ) {

					int materialJ = refTriangles.MaterialIndex[ j ];
					int vertexIndexJ = refTriangles.VertexIndex[ j ];
					int triangleIndexJ = refTriangles.TriangleIndex[ j ];

					if( vertexIndexI == vertexIndexJ ) {
						// This is the same vertex, it is going to be shared no matter what because
						// the indices point to the same normal.
						sum += triNormals[ triangleIndexJ ];
					} else {
						float dot = Vector3.Dot( triNormals[ triangleIndexI ], triNormals[ triangleIndexJ ] );
						dot = Mathf.Clamp( dot, -0.99999f, 0.99999f );
						float acos = Mathf.Acos( dot );
						float smoothingAngle = ( materialI == materialJ ) ? angleSameMaterial : angleDifferentMaterial;
						if( acos <= smoothingAngle ) {
							// Within the angle, factor it in
							sum += triNormals[ triangleIndexJ ];
						}
					}
				}

				if( sum.magnitude <= kEpsilon ) {
					// Prevent a bad normal from being created
					if( !warnedAboutZeroSumNormal ) {
						warnedAboutZeroSumNormal = true;
						Debug.LogWarningFormat( "Zero-sum normal found during RecalculateNormals ({0})", mesh.name );
					}
					sum = Vector3.forward;
				}

				if( normalLock != null && normalLock[ vertexIndexI ] ) {
					// we must take the provided normal
					normals[ vertexIndexI ] = providedNormals[ vertexIndexI ];
				} else {
					normals[ vertexIndexI ] = sum.normalized;
				}
			}
		}

		mesh.normals = normals;
	}

	public struct VertexKey
	{
		readonly long _x;
		readonly long _y;
		readonly long _z;

		//Change this if you require a different precision.
		const int Tolerance = 100000;

		public VertexKey( Vector3 position )
		{
			_x = (long)( Mathf.Round( position.x * Tolerance ) );
			_y = (long)( Mathf.Round( position.y * Tolerance ) );
			_z = (long)( Mathf.Round( position.z * Tolerance ) );
		}

		public VertexKey( OpenTK.Vector3 position )
		{
			_x = (long)( Mathf.Round( position.X * Tolerance ) );
			_y = (long)( Mathf.Round( position.Y * Tolerance ) );
			_z = (long)( Mathf.Round( position.Z * Tolerance ) );
		}

		public override bool Equals( object obj )
		{
			var key = (VertexKey)obj;
			return _x == key._x && _y == key._y && _z == key._z;
		}

		public override int GetHashCode()
		{
			return ( _x * 7 ^ _y * 13 ^ _z * 27 ).GetHashCode();
		}
	}

	public sealed class VertexEntry
	{
		public int[] TriangleIndex = new int[ 4 ];
		public int[] VertexIndex = new int[ 4 ];
		public int[] TriangleId = new int[ 4 ];
		public int[] MaterialIndex = new int[ 4 ];
        public bool[] TriangleIsPortal = new bool[ 4 ];

		private int _reserved = 4;
		private int _count;

		public int Count { get { return _count; } }

		public void Add( int vertIndex, int triangleId, int triIndex, int materialIndex, bool triangleIsPortal )
		{
			//Auto-resize the arrays when needed
			if( _reserved == _count ) {
				_reserved *= 2;
				Array.Resize( ref TriangleIndex, _reserved );
				Array.Resize( ref VertexIndex, _reserved );
				Array.Resize( ref TriangleId, _reserved );
				Array.Resize( ref MaterialIndex, _reserved );
                Array.Resize( ref TriangleIsPortal, _reserved );
			}
			TriangleIndex[ _count ] = triIndex;
			VertexIndex[ _count ] = vertIndex;
			TriangleId[ _count ] = triangleId;
			MaterialIndex[ _count ] = materialIndex;
            TriangleIsPortal[ _count ] = triangleIsPortal;
			++_count;
		}
	}
}
