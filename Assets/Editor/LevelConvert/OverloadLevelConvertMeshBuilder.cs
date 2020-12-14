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
using System.Linq;

public partial class OverloadLevelConverter
{
	public class MeshBuilder
	{
		public enum GenerateMode
		{
			RenderMesh,
			CollisionMesh,
		}

		public enum GeometryType
		{
			Segment,
			Decal,
			Portal,
		}

		public struct FaceVertex
		{
			Vector3 m_pos;
			Vector3 m_normal;
			Vector4 m_color;
			Vector2 m_uv1;
            Vector2 m_uv2;
            Vector2 m_uv3;
            bool m_posSet;
			bool m_normalSet;
			bool m_uv1Set;
            bool m_uv2Set;
            bool m_uv3Set;

            public void Initialize()
			{
				m_pos = new Vector3(0.0f, 0.0f, 0.0f);
				m_normal = new Vector3(0.0f, 0.0f, 1.0f);
				m_color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
				m_uv1 = new Vector2(0.0f, 0.0f);
                m_uv2 = new Vector2(0.0f, 0.0f);
                m_uv3 = new Vector2(0.0f, 0.0f);
                m_posSet = false;
				m_uv1Set = false;
                m_uv2Set = false;
                m_uv3Set = false;
                m_normalSet = false;
			}

			public Vector3 GetPos()
			{
				DebugAssert( m_posSet);
				return m_pos;
			}

			public void SetPos(Vector3 pos)
			{
				m_posSet = true;
				m_pos = pos;
			}

			public bool HasPos { get { return m_posSet; } }

			public Vector3 GetNormal()
			{
				DebugAssert( m_normalSet);
				return m_normal;
			}

			public void SetNormal(Vector3 normal)
			{
				m_normalSet = true;
				m_normal = normal;
			}

			public bool HasNormal { get { return m_normalSet; } }
			public void ClearNormal()
			{
				m_normalSet = false;
			}

			public Vector2 GetUV1()
			{
				DebugAssert( m_uv1Set);
				return m_uv1;
			}

			public void SetUV1(Vector2 uv)
			{
				m_uv1Set = true;
				m_uv1 = uv;
			}

			public bool HasUV1 { get { return m_uv1Set; } }

			public void ClearUV1()
			{
				m_uv1Set = false;
			}

            public Vector2 GetUV2()
            {
                DebugAssert(m_uv2Set);
                return m_uv2;
            }

            public void SetUV2(Vector2 uv)
            {
                m_uv2Set = true;
                m_uv2 = uv;
            }

            public bool HasUV2 { get { return m_uv2Set; } }

            public void ClearUV2()
            {
                m_uv2Set = false;
            }

            public Vector2 GetUV3()
            {
                DebugAssert(m_uv3Set);
                return m_uv3;
            }

            public void SetUV3(Vector2 uv)
            {
                m_uv3Set = true;
                m_uv3 = uv;
            }

            public bool HasUV3 { get { return m_uv3Set; } }

            public void ClearUV3()
            {
                m_uv3Set = false;
            }

            public Vector4 GetColor()
			{
				return m_color;
			}

			public Color GetColorAsColor()
			{
				return new Color(m_color.x, m_color.y, m_color.z, m_color.w);
			}

			public void SetColor(Vector4 color)
			{
				m_color = color;
			}

			public ulong CalculateHash()
			{
				const float position_granularity = 0.001f; // millimeter
				const float normal_granularity = 0.00001f;
				const float color_granularity = 0.001f;
				const float uv_granularity = 0.0001f;

				var hasher = new Murmur3();

				hasher.AddHash(m_posSet);
				hasher.AddHash(m_normalSet);
				hasher.AddHash(m_uv1Set);
                hasher.AddHash(m_uv2Set);
                hasher.AddHash(m_uv3Set);

                if (m_posSet) {
					hasher.AddHash(m_pos, position_granularity);
				}

				if (m_normalSet) {
					hasher.AddHash(m_normal, normal_granularity);
				}

				hasher.AddHash(m_color, color_granularity);

				if (m_uv1Set) {
					hasher.AddHash(m_uv1, uv_granularity);
				}

                if (m_uv2Set)
                {
                    hasher.AddHash(m_uv2, uv_granularity);
                }

                if (m_uv3Set)
                {
                    hasher.AddHash(m_uv3, uv_granularity);
                }

                hasher.Finish();

				return hasher.Hash64Bit;
			}
		}

		public class SubmeshBuilder
		{
			public struct AddVertexResult
			{
				public int CombinedVertexIndex;
				public int SubmeshVertexIndex;
			}

			public delegate AddVertexResult AddVertexDelegate(FaceVertex fv);
			AddVertexDelegate m_addVertex;
			List<int> m_combined_vertlist = new List<int>();
			List<int> m_submesh_vertlist = new List<int>();

			public SubmeshBuilder(AddVertexDelegate addVertex, GeometryType geomType)
			{
				this.m_addVertex = addVertex;
				this.GeomType = geomType;
			}

			// Returns true on success, false if it was a degenerate triangle
			public bool AddTriangle(FaceVertex v0, FaceVertex v1, FaceVertex v2)
			{
				AddVertexResult vb_id_0 = m_addVertex(v0);
				AddVertexResult vb_id_1 = m_addVertex(v1);
				AddVertexResult vb_id_2 = m_addVertex(v2);

				// Degenerate check
				if (vb_id_0.CombinedVertexIndex == vb_id_1.CombinedVertexIndex ||
					 vb_id_0.CombinedVertexIndex == vb_id_2.CombinedVertexIndex ||
					 vb_id_1.CombinedVertexIndex == vb_id_2.CombinedVertexIndex) {
					return false;
				}

				m_combined_vertlist.Add(vb_id_0.CombinedVertexIndex);
				m_combined_vertlist.Add(vb_id_1.CombinedVertexIndex);
				m_combined_vertlist.Add(vb_id_2.CombinedVertexIndex);

				m_submesh_vertlist.Add(vb_id_0.SubmeshVertexIndex);
				m_submesh_vertlist.Add(vb_id_1.SubmeshVertexIndex);
				m_submesh_vertlist.Add(vb_id_2.SubmeshVertexIndex);

				return true;
			}

			// Get the triangle's vert indices into the combined mesh
			public int[] CombinedTriangleList
			{
				get { return m_combined_vertlist.ToArray(); }
			}

			// Get the triangle's vert indices into the individual submesh
			public int[] SubmeshTriangleList
			{
				get { return m_submesh_vertlist.ToArray(); }
			}

			public GeometryType GeomType { get; private set; }
		}

		Dictionary<int, List<FaceVertex>> m_verts_by_submesh = new Dictionary<int, List<FaceVertex>>();
		List<FaceVertex> m_combined_verts = new List<FaceVertex>();
		Dictionary<int, Dictionary<UInt64, int>> m_vert_hash_by_submesh_to_index = new Dictionary<int, Dictionary<UInt64, int>>();
		Dictionary<UInt64, int> m_combined_vert_hash_to_index = new Dictionary<UInt64, int>();
		Dictionary<int, SubmeshBuilder> m_submeshesBySubmeshIndex = new Dictionary<int, SubmeshBuilder>();

		public GenerateMode Mode { get; private set; }

		public MeshBuilder(GenerateMode mode)
		{
			Mode = mode;
		}

		public SubmeshBuilder GetSubmeshBuilder(int submeshIndex, GeometryType geomType)
		{
			SubmeshBuilder builder;
			if (!m_submeshesBySubmeshIndex.TryGetValue(submeshIndex, out builder)) {
				builder = new SubmeshBuilder(new SubmeshBuilder.AddVertexDelegate((fv) => {

					if (Mode == GenerateMode.CollisionMesh) {
						// Collision meshes ignore UV, Normals, and Color data
						fv.ClearUV1();
                        fv.ClearUV2();
                        fv.ClearUV3();
                        fv.ClearNormal();
						fv.SetColor(new Vector4(1.0f, 1.0f, 1.0f, 1.0f));
					}

					UInt64 vertHash = fv.CalculateHash();

					Dictionary<UInt64, int> submesh_verthash_to_index;
					if (!m_vert_hash_by_submesh_to_index.TryGetValue(submeshIndex, out submesh_verthash_to_index)) {
						submesh_verthash_to_index = new Dictionary<UInt64, int>();
						m_vert_hash_by_submesh_to_index.Add(submeshIndex, submesh_verthash_to_index);
					}

					// For combined mesh
					int combined_index;
					if (!m_combined_vert_hash_to_index.TryGetValue(vertHash, out combined_index)) {
						combined_index = m_combined_verts.Count;
						m_combined_vert_hash_to_index.Add(vertHash, combined_index);
						m_combined_verts.Add(fv);
					}

					// For individual submesh
					int submesh_index;
					if (!submesh_verthash_to_index.TryGetValue(vertHash, out submesh_index)) {

						List<FaceVertex> submesh_vert_list;
						if (!m_verts_by_submesh.TryGetValue(submeshIndex, out submesh_vert_list)) {
							submesh_vert_list = new List<FaceVertex>();
							m_verts_by_submesh.Add(submeshIndex, submesh_vert_list);
						}

						submesh_index = submesh_vert_list.Count;
						submesh_verthash_to_index.Add(vertHash, submesh_index);
						submesh_vert_list.Add(fv);
					}

					return new SubmeshBuilder.AddVertexResult() { CombinedVertexIndex = combined_index, SubmeshVertexIndex = submesh_index };
				}), geomType);
				m_submeshesBySubmeshIndex.Add(submeshIndex, builder);
			}
			return builder;
		}

		class BuildMeshWorkerResult
		{
			public Vector3[] m_vertices;
			public Vector2[] m_uv1s;
			public Vector2[] m_uv2s;
			public Vector2[] m_uv3s;
			public Color[] m_colors;
			public Vector3[] m_normals;
			public bool[] m_normal_lock;
			public bool m_has_translucent;
			public bool m_has_uv2 = false;
			public bool m_has_uv3 = false;

			public void Build(List<FaceVertex> verts)
			{
				int numVerts = verts.Count;

				m_vertices = new Vector3[numVerts];
				m_uv1s = new Vector2[numVerts];
				m_uv2s = new Vector2[numVerts];
				m_uv3s = new Vector2[numVerts];
				m_colors = new Color[numVerts];
				m_normals = new Vector3[numVerts];
				m_normal_lock = new bool[numVerts];

				m_has_translucent = false;
				for (int i = 0; i < numVerts; ++i) {
					var fv = verts[i];
					m_vertices[i] = fv.GetPos();
					m_uv1s[i] = fv.GetUV1();

					if (fv.HasUV2) {
						Vector2 uv2 = fv.GetUV2();
						if (uv2.x != 0.0f || uv2.y != 0.0f) {
							m_has_uv2 = true;
						}
						m_uv2s[i] = uv2;
					} else {
						m_uv2s[i] = new Vector2(0.0f, 0.0f);
					}

					if (fv.HasUV3) {
						Vector2 uv3 = fv.GetUV3();
						if (uv3.x != 0.0f || uv3.y != 0.0f) {
							m_has_uv3 = true;
						}
						m_uv3s[i] = uv3;
					} else {
						m_uv3s[i] = new Vector2(0.0f, 0.0f);
					}

					var col = fv.GetColorAsColor();
					m_colors[i] = col;

					if (col.a < 1f) {
						m_has_translucent = true;
					}

					if (fv.HasNormal) {
						// Normal already specified
						m_normals[i] = fv.GetNormal();
						m_normal_lock[i] = true;
					} else {
						// No normal specified
						m_normals[i] = Vector3.forward;
						m_normal_lock[i] = false;
					}
				}
			}
		}

		public void GenerateMeshObject(GenerateMode generateMode, float smoothingAngleSameMaterial, float smoothingAngleDiffMaterial, List<Mesh> output_mesh_list, List<int> output_submesh_index_list)
		{
			if (generateMode == GenerateMode.RenderMesh) {
				//
				// Building a render mesh -- materials matter
				//
				foreach (var kvp in m_submeshesBySubmeshIndex) {
					int submesh_index = kvp.Key;
					SubmeshBuilder submesh_builder = kvp.Value;

                    // Sometimes we may create a submesh builder but not add any verts
                    // to it. This could be for no-render decal face flags.
                    List<FaceVertex> faceVerts;
                    if( !m_verts_by_submesh.TryGetValue( submesh_index, out faceVerts ) ) {
                        continue;
                    }

					BuildMeshWorkerResult submesh_mesh_data = new BuildMeshWorkerResult();
                    submesh_mesh_data.Build( faceVerts );

					Mesh m = new Mesh();
					m.vertices = submesh_mesh_data.m_vertices;

					// Needs UVs and colors
					m.uv = submesh_mesh_data.m_uv1s;
					if (submesh_mesh_data.m_has_uv2) {
						m.uv2 = submesh_mesh_data.m_uv2s;
					}
					if (submesh_mesh_data.m_has_uv3) {
						m.uv3 = submesh_mesh_data.m_uv3s;
					}
					if (submesh_mesh_data.m_has_translucent) {
						// Only take colors if there was a non-white color
						m.colors = submesh_mesh_data.m_colors;
					}

					// As an optimization - each material gets its own mesh (instead of using submeshes within the mesh)
					m.subMeshCount = 1;
					m.SetTriangles(submesh_builder.SubmeshTriangleList, 0);

					m.RecalculateNormals(smoothingAngleSameMaterial, smoothingAngleDiffMaterial, submesh_mesh_data.m_normal_lock, submesh_mesh_data.m_normals);
					// Recalculate tangents (AFTER normals, as RecalculateNormals will destroy tangents)
					m.RecalculateTangents();
					m.RecalculateBounds();
					
					output_mesh_list.Add(m);
                    output_submesh_index_list.Add(submesh_index);
				}
			} else {
				//
				// Building a collision mesh -- combine the verts
				//
				BuildMeshWorkerResult combined_mesh_data = new BuildMeshWorkerResult();
				combined_mesh_data.Build(m_combined_verts);

				Mesh m = new Mesh();
				m.vertices = combined_mesh_data.m_vertices;

				// Collision mesh is one submesh
				m.subMeshCount = 1;

				var all_combined_triangles = new List<int>();
				foreach (var kvp in m_submeshesBySubmeshIndex) {
					int submeshIndex = kvp.Key;
					if (submeshIndex < 0) {
						continue;
					}
					var submeshBuilder = kvp.Value;
					if (submeshBuilder.GeomType == GeometryType.Portal) {
						// don't include the debug portal geometry into the collision data
						continue;
					}
					all_combined_triangles.AddRange(submeshBuilder.CombinedTriangleList);
				}
				m.SetTriangles(all_combined_triangles.ToArray(), 0);

				m.RecalculateNormals(smoothingAngleSameMaterial, smoothingAngleDiffMaterial, combined_mesh_data.m_normal_lock, combined_mesh_data.m_normals);
				m.RecalculateBounds();
				;
				output_mesh_list.Add(m);
                output_submesh_index_list.Add(0);
			}
		}
	}
}
