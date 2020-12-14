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
	#region Triangulation Order
	public class QuadTriangulationOrder
	{
		// For non-coplanar
		public QuadTriangulationOrder(int[] vertIndices)
		{
			DebugAssert(vertIndices.Length == 6);
			m_vertIndices = vertIndices;
		}

		// For coplanar
		public QuadTriangulationOrder()
		{
			m_vertIndices = null;
		}

		/// <summary>
		/// Get the vertex order for the quad's triangle
		/// </summary>
		/// <param name="triIndex">Which triangle of the quad (0 or 1)</param>
		/// <param name="inverted">Whether or not to invert (wind backwards) the verts, only pass true if you are working on a slave portal</param>
		/// <returns>an int[3]</returns>
		public int[] GetVertsForTriangle(int triIndex, bool inverted)
		{
			DebugAssert(triIndex >= 0 && triIndex <= 1);

			if (m_vertIndices == null) {
				// Coplanar polygon, order doesn't matter too much
				int[] res = new int[] { 0, triIndex + 1, triIndex + 2 };
				if (inverted) {
					if (triIndex == 0) {
						res[2] = 3;
					} else {
						res[0] = 1;
					}
				}
				return res;
			} else {

				int baseIndex = triIndex * 3;
				int[] res = new int[] { m_vertIndices[baseIndex + 0], m_vertIndices[baseIndex + 1], m_vertIndices[baseIndex + 2] };

				if (inverted) {
					if (triIndex == 0) {
						if (res[2] == 2) {
							res[2] = 3;
						} else {
							res[2] = 2;
						}
					} else {
						if (res[0] == 0) {
							res[0] = 1;
						} else {
							res[0] = 0;
						}
					}
				}

				return res;
			}
		}

		public bool IsCoplanar
		{
			get
			{
				return m_vertIndices == null;
			}
		}

		private int[] m_vertIndices;
	}

	public static Vector3 GetTriangleNormal(Vector3 v0, Vector3 v1, Vector3 v2)
	{
		Vector3 e0 = (v1 - v0).normalized;
		Vector3 e1 = (v2 - v0).normalized;
		return Vector3.Cross(e0, e1).normalized;
	}

	public static QuadTriangulationOrder GetTriangulationOrder(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3, bool isMasterPortal)
	{
		// There are two ways to triangulate a non-coplanar quad:
		//  1) [ (0,1,2) and (0,2,3) ] or 
		//  2) [ (0,1,3) and (1,2,3) ]
		//
		// We want to pick the triangulation that results in the two triangles pointer towards each other (instead of away)

		// Assume triangulation #1...
		Vector3 normal0 = GetTriangleNormal(v0, v1, v2);
		float planeD0 = -Vector3.Dot(normal0, v0);
		float v3DistToPlane = Vector3.Dot(normal0, v3) + planeD0;

		// If v3 is on the same plane as v0, v1, v2 ... then it is a coplanar quad
		if (Math.Abs(v3DistToPlane) <= 0.00001f) {
			// Coplanar
			return new QuadTriangulationOrder();
		}

		// This is the correct triangulation if v3 is above the plane of [v0,v1,v2]
		if (v3DistToPlane > 0.0f || isMasterPortal) {
			return new QuadTriangulationOrder(new int[] { 0, 1, 2, 0, 2, 3 });
		}

		// We need to use triangulation #2
		Vector3 normal1 = GetTriangleNormal(v0, v1, v3);
		float planeD1 = -Vector3.Dot(normal1, v0);
		float v2DistToPlane = Vector3.Dot(normal1, v2) + planeD1;
		DebugAssert(v2DistToPlane >= 0.0f);

		return new QuadTriangulationOrder(new int[] { 0, 1, 3, 1, 2, 3 });
	}

	public static QuadTriangulationOrder GetTriangulationOrder(Vector3[] verts, bool isMasterPortal)
	{
		return GetTriangulationOrder(verts[0], verts[1], verts[2], verts[3], isMasterPortal);
	}

	public static QuadTriangulationOrder GetTriangulationOrder(OverloadLevelEditor.Level level, int segmentIndex, int sideIndex)
	{
		OverloadLevelEditor.Segment segment = level.segment[segmentIndex];

		OverloadLevelEditor.Side side = segment.side[sideIndex];
		bool isMasterPortal = side.HasNeighbor;

		int[] vertIndices = side.vert;
		Vector3 v0 = level.vertex[vertIndices[0]].position.ToUnity();
		Vector3 v1 = level.vertex[vertIndices[1]].position.ToUnity();
		Vector3 v2 = level.vertex[vertIndices[2]].position.ToUnity();
		Vector3 v3 = level.vertex[vertIndices[3]].position.ToUnity();
		return GetTriangulationOrder(v0, v1, v2, v3, isMasterPortal);
	}

	public static QuadTriangulationOrder GetTriangulationOrder(LevelGeometry levelData, int segmentIndex, int sideIndex)
	{
		SegmentData segment = levelData.Segments[segmentIndex];
		bool isMasterPortal = segment.Portals[sideIndex] != -1;

		int[] vertexOrder = SegmentData.SideVertexOrder[sideIndex];
		int[] vertIndices = vertexOrder.Select(i => segment.VertIndices[i]).ToArray();
		Vector3 v0 = levelData.SegmentVerts[vertIndices[0]];
		Vector3 v1 = levelData.SegmentVerts[vertIndices[1]];
		Vector3 v2 = levelData.SegmentVerts[vertIndices[2]];
		Vector3 v3 = levelData.SegmentVerts[vertIndices[3]];
		return GetTriangulationOrder(v0, v1, v2, v3, isMasterPortal);
	}
	#endregion

	#region SubmeshKey
	public class SubmeshKey
	{
		public SubmeshKey(string materialName, MeshBuilder.GeometryType geomType)
		{
			this.MaterialName = materialName;
			this.GeometryType = geomType;
		}

		public string MaterialName { get; private set; }
		public MeshBuilder.GeometryType GeometryType { get; private set; }

		public bool Equals(SubmeshKey other)
		{
			return (this.MaterialName.Equals(other.MaterialName, StringComparison.InvariantCultureIgnoreCase)) &&
				 (this.GeometryType == other.GeometryType);
		}

		public override bool Equals(object obj)
		{
			return ((obj is SubmeshKey) && this.Equals((SubmeshKey)obj));
		}

		public override string ToString()
		{
			return string.Format("[Material='{0}' GeomType='{1}']", this.MaterialName, this.GeometryType.ToString());
		}

		public override int GetHashCode()
		{
			return this.MaterialName.ToLowerInvariant().GetHashCode() ^ this.GeometryType.GetHashCode();
		}
	}
	#endregion

	#region SubmeshKeyComparer
	class SubmeshKeyComparer : IEqualityComparer<SubmeshKey>
	{
		public bool Equals(SubmeshKey x, SubmeshKey y)
		{
			if (object.ReferenceEquals(x, y))
				return true;
			return x.Equals(y);
		}

		public int GetHashCode(SubmeshKey obj)
		{
			if (obj == null)
				return 0;
			return obj.GetHashCode();
		}
	}
	#endregion

	#region EdgeKey
	class EdgeKey
	{
		public EdgeKey(int vertIndex1, int vertIndex2)
		{
			this.VertIndex1 = vertIndex1;
			this.VertIndex2 = vertIndex2;
		}

		public int VertIndex1 { get; private set; }
		public int VertIndex2 { get; private set; }

		public bool Equals(EdgeKey other)
		{
			return (this.VertIndex1 == other.VertIndex1) && (this.VertIndex2 == other.VertIndex2);
		}

		public override bool Equals(object obj)
		{
			return ((obj is EdgeKey) && this.Equals((EdgeKey)obj));
		}

		public override string ToString()
		{
			return string.Format("[Vert Index 1 ='{0}' Vert Index 2='{1}']", this.VertIndex1, this.VertIndex2);
		}

		public override int GetHashCode()
		{
			return this.VertIndex1.GetHashCode() ^ this.VertIndex2.GetHashCode();
		}
	}
	#endregion

	#region VertexKey
	class VertexKey
	{
		public VertexKey(int vertIndex00, int vertIndex01, int vertIndex10, int vertIndex11, uint uNumerator, uint uDenominator, uint vNumerator, uint vDenominator)
		{
			this.m_vert_index_tl = vertIndex00;
			this.m_vert_index_tr = vertIndex01;
			this.m_vert_index_bl = vertIndex11;
			this.m_vert_index_br = vertIndex10;
			this.m_interp_u_numerator = uNumerator;
			this.m_interp_u_denominator = uDenominator;
			this.m_interp_v_numerator = vNumerator;
			this.m_interp_v_denominator = vDenominator;

			// Vertex keys for corner or edge verts need to do some special processing on the key, so that they will be automatically considered equal on each side/edge that
			//  shares them.
			bool top_edge = (m_interp_v_numerator == 0);
			bool bottom_edge = (m_interp_v_numerator == m_interp_v_denominator);
			bool left_edge = (m_interp_u_numerator == 0);
			bool right_edge = (m_interp_u_numerator == m_interp_u_denominator);

			if ((top_edge && left_edge) || (top_edge && right_edge) || (bottom_edge && left_edge) || (bottom_edge && right_edge)) {
				// A corner vertex - store it to the TL data
				if (top_edge && left_edge) {
					// this.m_vert_index_tl = this.m_vert_index_tl; - already OK
				} else if (top_edge && right_edge) {
					this.m_vert_index_tl = this.m_vert_index_tr;
				} else if (bottom_edge && left_edge) {
					this.m_vert_index_tl = this.m_vert_index_bl;
				} else if (bottom_edge && right_edge) {
					this.m_vert_index_tl = this.m_vert_index_br;
				}

				this.m_vert_index_tr = -1;
				this.m_vert_index_bl = -1;
				this.m_vert_index_br = -1;
				this.m_interp_u_numerator = 0;
				this.m_interp_u_denominator = 0;
				this.m_interp_v_numerator = 0;
				this.m_interp_v_denominator = 0;
			} else if (top_edge || bottom_edge) {
				// Top or bottom edge vertex - move to the top verts
				if (bottom_edge) {
					this.m_vert_index_tl = this.m_vert_index_bl;
					this.m_vert_index_tr = this.m_vert_index_br;
				}

				this.m_vert_index_bl = -1;
				this.m_vert_index_br = -1;

				this.m_interp_v_numerator = 0;
				this.m_interp_v_denominator = 0;
			} else if (left_edge || right_edge) {
				// Left or bottom edge vertex - move to the top verts
				if (left_edge) {
					this.m_vert_index_tr = this.m_vert_index_bl;
				} else {
					this.m_vert_index_tl = this.m_vert_index_tr;
					this.m_vert_index_tr = this.m_vert_index_br;
				}

				this.m_vert_index_bl = -1;
				this.m_vert_index_br = -1;

				this.m_interp_u_numerator = this.m_interp_v_numerator;
				this.m_interp_u_denominator = this.m_interp_v_denominator;
				this.m_interp_v_numerator = 0;
				this.m_interp_v_denominator = 0;
			}

			// If necessary, mirror the key around the U direction so that the smaller positive vertex index is in m_vert_index00.
			//  Because of the code above:
			//   - all edge vertices will already have been transformed into an edge along the U axis, and
			//   - all corner vertices will only have one positive vertex index, which will already be in m_vert_index00.
			if ((this.m_vert_index_tr >= 0) && (this.m_vert_index_tl > this.m_vert_index_tr)) {
				int temp = this.m_vert_index_tl;
				this.m_vert_index_tl = this.m_vert_index_tr;
				this.m_vert_index_tr = temp;
				this.m_interp_u_numerator = (this.m_interp_u_denominator - this.m_interp_u_numerator);
			}
		}

		public bool Equals(VertexKey other)
		{
			return (this.m_vert_index_tl == other.m_vert_index_tl) && (this.m_vert_index_tr == other.m_vert_index_tr)
					&& (this.m_vert_index_bl == other.m_vert_index_bl) && (this.m_vert_index_br == other.m_vert_index_br)
					&& (this.m_interp_u_numerator == other.m_interp_u_numerator) && (this.m_interp_u_denominator == other.m_interp_u_denominator)
					&& (this.m_interp_v_numerator == other.m_interp_v_numerator) && (this.m_interp_v_denominator == other.m_interp_v_denominator);
		}

		public override bool Equals(object obj)
		{
			return ((obj is VertexKey) && this.Equals((VertexKey)obj));
		}

		public override string ToString()
		{
			return string.Format("[Vert Indices =({0}, {1}, {2}, {3}), Interp values = ({4}/{5}, {6}/{7})]", this.m_vert_index_tl, this.m_vert_index_tr, this.m_vert_index_bl, this.m_vert_index_br, this.m_interp_u_numerator, this.m_interp_u_denominator, this.m_interp_v_numerator, this.m_interp_v_denominator);
		}

		public override int GetHashCode()
		{
			return this.m_vert_index_tl.GetHashCode() ^ this.m_vert_index_tr.GetHashCode() ^ this.m_vert_index_bl.GetHashCode() ^ this.m_vert_index_br.GetHashCode()
					^ this.m_interp_u_numerator.GetHashCode() ^ this.m_interp_u_denominator.GetHashCode() ^ this.m_interp_v_numerator.GetHashCode()
					^ this.m_interp_v_denominator.GetHashCode();
		}

		private int m_vert_index_tl;
		private int m_vert_index_tr;
		private int m_vert_index_bl;
		private int m_vert_index_br;

		// We store the interpolation values in rational form (numerator and denominator) rather than float form
		//  to avoid any sort of precision issues when comparing.
		private uint m_interp_u_numerator;
		private uint m_interp_u_denominator;
		private uint m_interp_v_numerator;
		private uint m_interp_v_denominator;
	}
	#endregion

	#region PortalSideKey
	class PortalSideKey
	{
		public PortalSideKey(int segment_index, int side_index)
		{
			m_segment_index = segment_index;
			m_side_index = side_index;
		}

		public bool Equals(PortalSideKey other)
		{
			return (this.m_segment_index == other.m_segment_index) && (this.m_side_index == other.m_side_index);
		}

		public override bool Equals(object obj)
		{
			return ((obj is PortalSideKey) && this.Equals((PortalSideKey)obj));
		}

		public override string ToString()
		{
			return string.Format("[Segment Index ='{0}' Side Index ='{1}']", m_segment_index, m_side_index);
		}

		public override int GetHashCode()
		{
			return this.m_segment_index.GetHashCode() ^ this.m_side_index.GetHashCode();
		}

		public int m_segment_index;
		public int m_side_index;
	}
	#endregion

	#region DeformedVertex
	class DeformedVertex
	{
		public struct DeformationEntry
		{
			public DeformationEntry(OverloadLevelEditor.DeformationModuleBaseNew deformation_module, float side_deformation_multiplier)
			{
				m_deformation_module = deformation_module;
				m_side_deformation_multiplier = side_deformation_multiplier;
			}

			public OverloadLevelEditor.DeformationModuleBaseNew m_deformation_module;
			public float m_side_deformation_multiplier;
		}

		public DeformedVertex()
		{
			m_deformation_entries = new List<DeformationEntry>();
			m_original_normals = new List<Vector3>();
			m_original_triangle_id = new List<int>();
			m_original_triangle_is_from_portal = new List<bool>();
			//m_adjacent_positions = new List<Vector3>();
			m_adjacent_vertex_keys = new List<VertexKey>();
			m_referenced_triangle_indices = new List<int>();
			m_smoothing_positions = new Vector3[2];
			m_has_cached_deform_pos = false;
		}

		public void AddVertInformation(Vector3 position, Vector3 normal, OverloadLevelEditor.DeformationModuleBaseNew deformation_module, float side_deformation_multiplier, int triangle_id, bool triangle_is_from_portal)
		{
			// Triangles from portals should not contribute to deformation or normal averaging.
			if (!triangle_is_from_portal) {
				m_deformation_entries.Add(new DeformationEntry(deformation_module, side_deformation_multiplier));
				if ((deformation_module != null) && (side_deformation_multiplier > 0.0f)) {
					m_deformed = true;
				} else {
					// Don't smooth verts for faces that are not fully tessellated
					m_smooth = false;
				}
			}
			m_original_normals.Add(normal);
			m_original_triangle_id.Add(triangle_id);
			m_original_triangle_is_from_portal.Add(triangle_is_from_portal);
			m_original_position = position;
			m_smoothing_positions[0] = position;
			m_smoothing_positions[1] = position;
			m_has_cached_deform_pos = false;
		}

		public void AddAdjacentVertex(VertexKey vertKey)
		{
			if (!m_adjacent_vertex_keys.Contains(vertKey)) {
				m_adjacent_vertex_keys.Add(vertKey);
				m_has_cached_deform_pos = false;
			}
		}

		public void AddReferencedTriangleIndex(int levelTriangleIndex)
		{
			if (!m_referenced_triangle_indices.Contains(levelTriangleIndex)) {
				m_referenced_triangle_indices.Add(levelTriangleIndex);
			}
		}

		public bool OnlyReferencedByPortals()
		{
			return !(m_original_triangle_is_from_portal.Contains(false));
		}

		public void UpdateSmoothingPosition(Vector3 new_position)
		{
			m_smoothing_positions[0] = new_position;
			m_smoothing_positions[1] = new_position;
		}

		public void DoSmoothingPass(uint smoothing_pass_index, float smooth_strength, Dictionary<VertexKey, DeformedVertex> deformed_vertices_map)
		{
			if (!m_deformed || !m_smooth || OnlyReferencedByPortals()) {
				return;
			}

			uint write_smoothing_pass_index = (smoothing_pass_index & 1);
			uint read_smoothing_pass_index = 1 - write_smoothing_pass_index;

			Vector3 total_position = m_smoothing_positions[read_smoothing_pass_index];

			int num_contributing_vertex_keys = 0;

			foreach (VertexKey vkey in m_adjacent_vertex_keys) {
				if (deformed_vertices_map[vkey].OnlyReferencedByPortals()) {
					continue;
				}
				total_position += deformed_vertices_map[vkey].GetSmoothedPosition(read_smoothing_pass_index);
				num_contributing_vertex_keys++;
			}

			Vector3 smoothed_position = Vector3.Lerp(m_smoothing_positions[read_smoothing_pass_index], total_position / (float)(num_contributing_vertex_keys + 1), smooth_strength);
			m_smoothing_positions[write_smoothing_pass_index] = smoothed_position;
		}

		public Vector3 GetFinalDeformedPosition(uint smoothing_pass_index)
		{
			if (m_has_cached_deform_pos) {
				return m_cached_deform_pos;
			}

			uint read_smoothing_pass_index = 1 - (smoothing_pass_index & 1);
			Vector3 pre_deform_position = GetSmoothedPosition(read_smoothing_pass_index);

			// Next, we need to get the average of all of the deformed positions from all of the different sides which share this vertex.
			//  Some of these sides may be undeformed, or may be in different deformation groups, so will produce different results.
			Vector3 final_deformed_position = Vector3.zero;
			if (m_deformation_entries.Count > 0) {
				foreach (var deformation_entry in m_deformation_entries) {
					final_deformed_position += LevelConvertState.DeformVertex(pre_deform_position, m_deformation_direction, deformation_entry.m_deformation_module, deformation_entry.m_side_deformation_multiplier);
				}
				final_deformed_position /= System.Convert.ToSingle(m_deformation_entries.Count);
			} else {
				final_deformed_position = pre_deform_position;
			}

			// Calculate the deformed vertex and cache it
			m_has_cached_deform_pos = true;
			m_cached_deform_pos = final_deformed_position;
			return m_cached_deform_pos;
		}

		public Vector3 GetOriginalPosition()
		{
			return m_original_position;
		}

		public Vector3 GetSmoothedPosition(uint smoothing_pass_index)
		{
			uint array_index = smoothing_pass_index & 1;
			return m_smoothing_positions[array_index];
		}

		public Vector3 GetAverageNormal(int triangle_id)
		{
			Vector3 sum = Vector3.zero;

			int num_originals = m_original_normals.Count;
			for (int i = 0; i < num_originals; ++i) {
				if (m_original_triangle_id[i] != triangle_id) {
					continue;
				}
				if (m_original_triangle_is_from_portal[i]) {
					continue;
				}
				sum += m_original_normals[i];
			}

			return sum.normalized;
		}

		/// <summary>
		/// Returns true if at least one of the verts that contribute to this have a deformation factor
		/// </summary>
		public bool IsDeformed
		{
			get { return m_deformed; }
		}

		/// <summary>
		/// Returns true if all of the verts that contribute to this vertex has a deformation factor
		/// </summary>
		public bool CanBeSmoothed
		{
			get { return m_smooth; }
		}

		public int[] ReferencedTriangleIndices
		{
			get { return m_referenced_triangle_indices.ToArray(); }
		}

		private List<VertexKey> m_adjacent_vertex_keys;
		private List<int> m_referenced_triangle_indices;
		private Vector3 m_original_position;
		private Vector3[] m_smoothing_positions;
		private List<DeformationEntry> m_deformation_entries;
		private List<Vector3> m_original_normals;
		private List<int> m_original_triangle_id;
		private List<bool> m_original_triangle_is_from_portal;
		private bool m_deformed = false;
		private bool m_smooth = true;
		private bool m_has_cached_deform_pos = false;
		private Vector3 m_cached_deform_pos;
		public Vector3 m_deformation_direction;
	}
	#endregion

	#region DecalLightInfo
	class DecalLightInfo
	{
		OverloadLevelEditor.GLight m_light;
		int m_segmentIndex;

		public DecalLightInfo(OverloadLevelEditor.GLight light, int segmentIndex)
		{
			this.m_light = light;
			this.m_segmentIndex = segmentIndex;
			this.Color = Vector3.one;
		}

		public OverloadLevelEditor.GLight Light
		{
			get { return this.m_light; }
		}

		public int EditorSegmentIndex
		{
			get { return this.m_segmentIndex; }
		}

		public Vector3 Color
		{
			get;
			set;
		}
	}
	#endregion

	#region MineMeshNormalGenerator
	// Helper class used to generate the normals for the mine vertices. This is because we want smoothing
	// on the verts depending on the materials used on the wall and the angle it forms with neighboring
	// walls. The smoothing factor depends on the angle of the faces and whether or not they share a material.
	class MineMeshNormalGenerator
	{
		struct VertIdTriangleIdPair
		{
			public int VertexId;
			public int TriangleId;
			public bool TriangleIsPortal;

			public override bool Equals(object obj)
			{
				var other = (VertIdTriangleIdPair)obj;
				return other.VertexId == VertexId && other.TriangleId == TriangleId && other.TriangleIsPortal == TriangleIsPortal;
			}

			public override int GetHashCode()
			{
				return (VertexId << 9) ^ (TriangleId << 1) ^ (TriangleIsPortal ? 1 : 0);
			}

			public override string ToString()
			{
				return string.Format("Vert: {0} TriId: {1} TriIsPortal: {2}", VertexId, TriangleId, TriangleIsPortal);
			}
		}

		float m_angle_same_material;
		float m_angle_different_material;
		int m_highest_vertex_index_encountered = -1;
		Dictionary<int, OpenTK.Vector3> m_positions = new Dictionary<int, OpenTK.Vector3>();
		Dictionary<string, int> m_material_map = new Dictionary<string, int>(StringComparer.InvariantCultureIgnoreCase);
		Dictionary<int, List<VertIdTriangleIdPair>> m_triangles = new Dictionary<int, List<VertIdTriangleIdPair>>();
		Dictionary<int, OpenTK.Vector3>[] m_final_vert_normals_by_triangle_id = null; // m_final_vert_normals_by_triangle_id[vert_index][triangle_id]

		// Internal function to register a material name, returning its "submesh index"
		int RegisterMaterial(string material_name)
		{
			int index;
			if (m_material_map.TryGetValue(material_name, out index))
				return index;

			index = m_material_map.Count;
			m_material_map.Add(material_name, index);
			return index;
		}

		// Internal function to get the vert indices for a material
		List<VertIdTriangleIdPair> GetTriangleList(string material_name)
		{
			int material_index = RegisterMaterial(material_name);

			List<VertIdTriangleIdPair> tris;
			if (!m_triangles.TryGetValue(material_index, out tris)) {
				tris = new List<VertIdTriangleIdPair>();
				m_triangles.Add(material_index, tris);
			}

			return tris;
		}

		/// <summary>
		/// Construct an instance
		/// </summary>
		/// <param name="angle_same_material">The cut-off angle (in degrees) to factor in bording triangles of the same material (any neighboring triangle within this angle)</param>
		/// <param name="angle_different_material">The cut-off angle (in degrees) to factor in bording triangles of a different material (any neighboring triangle within this angle)</param>
		public MineMeshNormalGenerator(float angle_same_material, float angle_different_material)
		{
			m_angle_different_material = angle_different_material;
			m_angle_same_material = angle_same_material;
		}

		/// <summary>
		/// Register the position value for a vertex index
		/// </summary>
		/// <param name="vidx"></param>
		/// <param name="pos"></param>
		public void SetVertexPosition(int vidx, OpenTK.Vector3 pos)
		{
			m_positions[vidx] = pos;
		}

		public static int MakeTriangleId(int segment_index, int side_index)
		{
			return (segment_index * OverloadLevelEditor.Segment.NUM_SIDES) + side_index;
		}

		/// <summary>
		/// Register a mine triangle into the system
		/// </summary>
		/// <param name="triangle_id">A unique identifier for this triangle, which is used to query the final results</param>
		/// <param name="v0"></param>
		/// <param name="v1"></param>
		/// <param name="v2"></param>
		/// <param name="material_name"></param>
		public void AddMineTriangle(int triangle_id, int v0, int v1, int v2, string material_name, bool triangle_is_portal)
		{
			var triangles = GetTriangleList(material_name);
			triangles.Add(new VertIdTriangleIdPair() { VertexId = v0, TriangleId = triangle_id, TriangleIsPortal = triangle_is_portal });
			triangles.Add(new VertIdTriangleIdPair() { VertexId = v1, TriangleId = triangle_id, TriangleIsPortal = triangle_is_portal });
			triangles.Add(new VertIdTriangleIdPair() { VertexId = v2, TriangleId = triangle_id, TriangleIsPortal = triangle_is_portal });

			m_highest_vertex_index_encountered = Math.Max(m_highest_vertex_index_encountered, v0);
			m_highest_vertex_index_encountered = Math.Max(m_highest_vertex_index_encountered, v1);
			m_highest_vertex_index_encountered = Math.Max(m_highest_vertex_index_encountered, v2);
		}

		/// <summary>
		/// Once all positions and triangles have been registered, call this so that you can access the normals
		/// </summary>
		public void CalculateNormals()
		{
			const float kEpsilon = 0.0001f;
			bool warned_about_degen_tris = false;
			bool warned_about_zero_sum_normal = false;

			float angle_same_material = m_angle_same_material * Mathf.Deg2Rad;
			float angle_different_material = m_angle_different_material * Mathf.Deg2Rad;

			int num_verts = m_highest_vertex_index_encountered + 1;
			m_final_vert_normals_by_triangle_id = new Dictionary<int, OpenTK.Vector3>[num_verts];

			// initialize the vertex positions
			OpenTK.Vector3[] vertices = new OpenTK.Vector3[num_verts];
			for (int i = 0; i < num_verts; ++i) {
				if (!m_positions.TryGetValue(i, out vertices[i])) {
					// Might not be good if we reference this.
					vertices[i] = OpenTK.Vector3.Zero;
				}

				// for any vertex index, we can have a different normal given the material/side
				m_final_vert_normals_by_triangle_id[i] = new Dictionary<int, OpenTK.Vector3>();
			}

			var point_in_space_to_ref_tris_dictionary = new Dictionary<OverloadLevelConverterExtension.VertexKey, OverloadLevelConverterExtension.VertexEntry>(vertices.Length);

			int num_submeshes = m_material_map.Count;
			int num_total_triangles = 0;
			for (int submesh_index = 0; submesh_index < num_submeshes; ++submesh_index) {
				num_total_triangles += m_triangles[submesh_index].Count / 3;
			}

			var tri_normals = new OpenTK.Vector3[num_total_triangles]; //Holds the normal of each triangle
			int tri_offset = 0;
			for (int submesh_index = 0; submesh_index < num_submeshes; ++submesh_index) {
				List<VertIdTriangleIdPair> triangles = m_triangles[submesh_index];

				// Goes through all the triangles and gathers up data to be used later
				for (var i = 0; i < triangles.Count; i += 3) {
					int i1 = triangles[i + 0].VertexId;
					int i2 = triangles[i + 1].VertexId;
					int i3 = triangles[i + 2].VertexId;

					// These should all be the same index from the way we build the m_triangles lists.
					int i1_tri_id = triangles[i + 0].TriangleId;
					int i2_tri_id = triangles[i + 1].TriangleId;
					int i3_tri_id = triangles[i + 2].TriangleId;

					bool i1_tri_is_portal = triangles[i + 0].TriangleIsPortal;
					bool i2_tri_is_portal = triangles[i + 1].TriangleIsPortal;
					bool i3_tri_is_portal = triangles[i + 2].TriangleIsPortal;

					// Calculate the normal of the triangle
					OpenTK.Vector3 p1 = vertices[i2] - vertices[i1];
					OpenTK.Vector3 p2 = vertices[i3] - vertices[i1];
					OpenTK.Vector3 normal = OpenTK.Vector3.Cross(p1, p2).Normalized();

					if (normal.Length < kEpsilon) {
						// Prevent a bad triangle from generating a bad normal
						if (!warned_about_degen_tris) {
							warned_about_degen_tris = true;
							Debug.LogWarningFormat("Degenerate triangles found during CalculateNormals for the mine");
						}
						normal = OpenTK.Vector3.UnitZ;
					}
					int tri_index = (i / 3) + tri_offset;
					tri_normals[tri_index] = normal;

					OverloadLevelConverterExtension.VertexEntry entry;
					OverloadLevelConverterExtension.VertexKey key;

					// For our purposes, each submesh is for a different material
					// But if we should ever have a material represented multiple times
					// in submeshes, then we need to adjust this as needed.
					int material_index = submesh_index;

					//For each of the three points of the triangle
					//  Add this triangle as part of the triangles they're connected to.
					if (!point_in_space_to_ref_tris_dictionary.TryGetValue(key = new OverloadLevelConverterExtension.VertexKey(vertices[i1]), out entry)) {
						entry = new OverloadLevelConverterExtension.VertexEntry();
						point_in_space_to_ref_tris_dictionary.Add(key, entry);
					}
					entry.Add(i1, i1_tri_id, tri_index, material_index, i1_tri_is_portal);

					if (!point_in_space_to_ref_tris_dictionary.TryGetValue(key = new OverloadLevelConverterExtension.VertexKey(vertices[i2]), out entry)) {
						entry = new OverloadLevelConverterExtension.VertexEntry();
						point_in_space_to_ref_tris_dictionary.Add(key, entry);
					}
					entry.Add(i2, i2_tri_id, tri_index, material_index, i2_tri_is_portal);

					if (!point_in_space_to_ref_tris_dictionary.TryGetValue(key = new OverloadLevelConverterExtension.VertexKey(vertices[i3]), out entry)) {
						entry = new OverloadLevelConverterExtension.VertexEntry();
						point_in_space_to_ref_tris_dictionary.Add(key, entry);
					}
					entry.Add(i3, i3_tri_id, tri_index, material_index, i3_tri_is_portal);
				}

				tri_offset += triangles.Count / 3;
			}

			//Foreach point in space (not necessarily the same vertex index!)
			//{
			//  Foreach triangle T1 that point belongs to
			//  {
			//    Foreach other triangle T2 (including self) that point belongs to and that meets any of the following:
			//		1) The corresponding vertex is actually the same vertex (and material)
			//		2) The angle between the two triangles is less than the smoothing angle
			//    {
			//      > Add to temporary Vector3
			//    }
			//    > Normalize temporary Vector3 to find the average
			//    > Assign the normal to corresponding vertex of T1
			//  }
			//}
			foreach (var trivdata_at_point_in_space in point_in_space_to_ref_tris_dictionary.Values) {

				// Track the tri_id/vertex pairs that have already been processed -- we will just
				// duplicate the work and get the same result.
				Dictionary<KeyValuePair<int, int>, bool> already_processed_triid_verts = new Dictionary<KeyValuePair<int, int>, bool>();

				for (var trivdata_index = 0; trivdata_index < trivdata_at_point_in_space.Count; ++trivdata_index) {

					int check_material_index = trivdata_at_point_in_space.MaterialIndex[trivdata_index];
					int check_triangle_index = trivdata_at_point_in_space.TriangleIndex[trivdata_index];
					int check_vertex_index = trivdata_at_point_in_space.VertexIndex[trivdata_index];
					int check_triangle_id = trivdata_at_point_in_space.TriangleId[trivdata_index];
					//bool check_triangle_is_portal = trivdata_at_point_in_space.TriangleIsPortal[trivdata_index];

					// Check for duplicate work
					KeyValuePair<int, int> triid_vert_key = new KeyValuePair<int, int>(check_triangle_id, check_vertex_index);
					if (already_processed_triid_verts.ContainsKey(triid_vert_key)) {
						// Already processed this one...
						continue;
					}
					already_processed_triid_verts.Add(triid_vert_key, true);

					// Build up the normal
					int sumCount = 0;
					var sum = OpenTK.Vector3.Zero;
					Dictionary<int, bool> already_processed_triangle_ids = new Dictionary<int, bool>(); // don't include a triangle_id more than once, in order to give fair weighting
					for (var test_trivdata_index = 0; test_trivdata_index < trivdata_at_point_in_space.Count; ++test_trivdata_index) {

						int test_material_index = trivdata_at_point_in_space.MaterialIndex[test_trivdata_index];
						int test_triangle_index = trivdata_at_point_in_space.TriangleIndex[test_trivdata_index];
						int test_vertex_index = trivdata_at_point_in_space.VertexIndex[test_trivdata_index];
						int test_triangle_id = trivdata_at_point_in_space.TriangleId[test_trivdata_index];
						bool test_triangle_is_portal = trivdata_at_point_in_space.TriangleIsPortal[test_trivdata_index];

						// Each triangle_id is a unique id for a side in the mine, skip over vertices on triangle_ids
						// that we have already added into the sum, we want to weight fairly
						if (already_processed_triangle_ids.ContainsKey(test_triangle_id)) {
							continue;
						}

						// Don't include triangles from portals into the normal sum.
						if (test_triangle_is_portal) {
							continue;
						}

						if ((test_triangle_id == check_triangle_id) && (test_vertex_index == check_vertex_index)) {
							// Same vertex -- the normal has to be shared
							OpenTK.Vector3 triangleNormal = tri_normals[test_triangle_index];
							sum += triangleNormal;
							++sumCount;
							already_processed_triangle_ids.Add(test_triangle_id, true);
						} else {
							// NOTE: This is still wrong, as we need to check ALL triangles against test triangle to see
							// if it is within smoothing angle. For the most part we are okay at the moment, unless we have

							// Not the same vertex, so this is a unique vert/material
							float smoothing_angle = (check_material_index == test_material_index) ? angle_same_material : angle_different_material;

							float dot = OpenTK.Vector3.Dot(tri_normals[check_triangle_index], tri_normals[test_triangle_index]);
							dot = Mathf.Clamp(dot, -0.99999f, 0.99999f);

							float acos = Mathf.Acos(dot);
							if (acos <= smoothing_angle) {
								// Within the angle, factor it in
								OpenTK.Vector3 triangleNormal = tri_normals[test_triangle_index];
								sum += triangleNormal;
								++sumCount;
								already_processed_triangle_ids.Add(test_triangle_id, true);
							}
						}
					}

					if (sum.Length <= kEpsilon || sumCount == 0) {
						// Prevent a bad normal from being created
						if (!warned_about_zero_sum_normal) {
							warned_about_zero_sum_normal = true;
							Debug.LogWarningFormat("Zero-sum normal found during CalculateNormals for the mine");
						}
						sum = OpenTK.Vector3.UnitZ;
						sumCount = 1;
					}

					sum = sum / (float)(sumCount);
					sum = sum.Normalized();

					// Assign the normal
					m_final_vert_normals_by_triangle_id[check_vertex_index].Add(check_triangle_id, sum);
				}
			}
		}

		/// <summary>
		/// Access the smoothed normal for the given vertex index
		/// </summary>
		/// <param name="vidx"></param>
		/// <returns></returns>
		public OpenTK.Vector3 GetVertexNormal(int vidx, int triangleId)
		{
			OpenTK.Vector3 normal;
			if (m_final_vert_normals_by_triangle_id[vidx].TryGetValue(triangleId, out normal)) {
				return normal;
			}

			throw new Exception(string.Format("No triangle with id ({0}) touches vertex {1}", triangleId, vidx));
		}
	}
	#endregion

	public partial class LevelConvertState
	{
		struct LevelConvertVertex
		{
			public VertexKey m_deformed_position_key;
			public Vector2 m_uv1;
			public Vector2 m_uv2;
			public Vector2 m_uv3;
			public bool m_lava_edge;
			public bool m_corner_vert;

			public LevelConvertVertex(VertexKey vertKey, Vector2 vertUV1, Vector2 vertUV2, Vector2 vertUV3, bool lavaEdge = false, bool cornerVert = false)
			{
				m_deformed_position_key = vertKey;
				m_uv1 = vertUV1;
				m_uv2 = vertUV2;
				m_uv3 = vertUV3;
				m_lava_edge = lavaEdge;
				m_corner_vert = cornerVert;
			}
		}

		struct LevelConvertTriangle
		{
			public enum WindingOrder
			{
				Default,
				Inverted
			}

			public enum FaceType
			{
				Default,
				Lava,
			}

			public LevelConvertVertex[] m_vertices;
			public int m_chunk_index;
			public int m_submesh_index;
			public MeshBuilder.GeometryType m_geom_type;
			public int m_from_segment;
			public int m_from_side;
			public FaceType m_face_type;
			public WindingOrder m_winding_order;
			public int m_virtual_edge_mask; // 3 bits, 1 for each edge. Bit is set if the corresponding edge was created for [quad] triangulation purposes, unset if artist edge. Edge0 is vert[0]->vert[1]

			public LevelConvertTriangle(LevelConvertVertex vert0, LevelConvertVertex vert1, LevelConvertVertex vert2,
				int chunkIndex, int submeshIndex, MeshBuilder.GeometryType geomType,
				int seg, int side, FaceType face_type, WindingOrder winding_order, int virtual_edge_mask)
			{
				m_vertices = new LevelConvertVertex[3];
				m_vertices[0] = vert0;
				m_vertices[1] = vert1;
				m_vertices[2] = vert2;
				m_chunk_index = chunkIndex;
				m_submesh_index = submeshIndex;
				m_geom_type = geomType;
				m_from_segment = seg;
				m_from_side = side;
				m_face_type = face_type;
				m_winding_order = winding_order;
				m_virtual_edge_mask = virtual_edge_mask;
			}
		}

		public static Vector3 DeformVertex(Vector3 initial_position, Vector3 deformation_direction, OverloadLevelEditor.DeformationModuleBaseNew deformation_module, float side_deformation_multiplier)
		{
			if (deformation_module == null) {
				return initial_position;
			} else {
				OpenTK.Vector3 deformed_position_opentk = deformation_module.GetDeformedPosition(OpenTKHelpers.ToOpenTK(initial_position), OpenTKHelpers.ToOpenTK(deformation_direction), side_deformation_multiplier);
				return OpenTKHelpers.ToUnity(deformed_position_opentk);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		static void ConstructGeometryInternal(LevelConvertStateManager convertState)
		{
			uint side_tessellation_factor = System.Convert.ToUInt32(convertState.OverloadLevelData.global_data.grid_size);
			uint num_pre_deform_smoothing_passes = System.Convert.ToUInt32(convertState.OverloadLevelData.global_data.pre_smooth);
			uint num_post_deform_smoothing_passes = System.Convert.ToUInt32(convertState.OverloadLevelData.global_data.post_smooth);
			float smoothingAngleSameMaterial = convertState.SmoothingAngleSameMaterial;
			float smoothingAngleDiffMaterial = convertState.SmoothingAngleDifferentMaterial;
			bool enableDeformation = convertState.FlagEnableDeformation;

			OverloadLevelEditor.DeformationModuleBaseNew[] deformation_modules;
			using (LogCodeExecutionTime("Building deformation modules")) {
				deformation_modules = BuildDeformationModules(convertState);
			}

			//Create a builder & material dictionary for each chunk
			using (LogCodeExecutionTime("Building chunk material dictionaries")) {
				for (int chunknum = 0; chunknum < convertState.OverloadLevelData.m_num_chunks; chunknum++) {
					convertState.BuilderGeometryByChunk[chunknum] = new MeshBuilder(MeshBuilder.GenerateMode.RenderMesh);
					convertState.BuilderCollisionsByChunk[chunknum] = new MeshBuilder(MeshBuilder.GenerateMode.CollisionMesh);

					// If this chunk has any sides that are lava in it, we want to create a mesh builder for the lava collision.
					bool chunk_has_lava = false;
					foreach (var segmentIndex1 in convertState.OverloadLevelData.EnumerateAliveSegmentIndices()) {
						var segment_base_data = convertState.OverloadLevelData.segment[segmentIndex1];

						if (segment_base_data.m_chunk_num == chunknum) {
							bool segment_has_lava = false;
							for (int sideIdx1 = 0; sideIdx1 < OverloadLevelEditor.Segment.NUM_SIDES; ++sideIdx1) {
								var side_base_data = segment_base_data.side[sideIdx1];
								if (side_base_data.is_lava) {
									segment_has_lava = true;
									break;
								}
							}

							if (segment_has_lava) {
								chunk_has_lava = true;
								break;
							}
						}
					}

					if (chunk_has_lava) {
						convertState.BuilderLavaCollisionsByChunk[chunknum] = new MeshBuilder(MeshBuilder.GenerateMode.CollisionMesh);
					} else {
						convertState.BuilderLavaCollisionsByChunk[chunknum] = null;
					}

					convertState.MaterialToSubmeshIndexByChunk[chunknum] = new Dictionary<SubmeshKey, int>(new SubmeshKeyComparer());
				}
			}

			// Set edge data for adjacent sides and also build up the mine vertex normal data
			var mine_normal_calculator = new MineMeshNormalGenerator(smoothingAngleSameMaterial, smoothingAngleDiffMaterial);
			using (LogCodeExecutionTime("Setting up adjacency and mine normal data")) {
				foreach (var segmentIndex1 in convertState.OverloadLevelData.EnumerateAliveSegmentIndices()) {
					var segment_base_data = convertState.OverloadLevelData.segment[segmentIndex1];
					for (int sideIdx1 = 0; sideIdx1 < OverloadLevelEditor.Segment.NUM_SIDES; ++sideIdx1) {
						var side_base_data = segment_base_data.side[sideIdx1];

						for (int edgeIdx = 0; edgeIdx < OverloadLevelEditor.Side.NUM_EDGES; ++edgeIdx) {
							var side_other = side_base_data.FindAdjacentSideOtherSegment(edgeIdx);
							side_base_data.edge[edgeIdx].adjacent_to_lava = side_other.is_lava;
							side_base_data.edge[edgeIdx].adjacent_to_tesselate = enableDeformation && (deformation_modules[side_other.deformation_preset] != null) && (side_other.deformation_height > 0f);
						}

						// Register the triangles for this side to the mine vertex normal calculator
						if (segment_base_data.neighbor[sideIdx1] == -1) {
							// Note: Only adding non-portals here, so we don't have to worry about inverting the winding when querying
							string side_material_name = side_base_data.tex_name;
							int triangle_id = MineMeshNormalGenerator.MakeTriangleId(segmentIndex1, sideIdx1);
							QuadTriangulationOrder tri_order = GetTriangulationOrder(convertState.OverloadLevelData, segmentIndex1, sideIdx1);
							for (int tri_idx = 0; tri_idx < 2; ++tri_idx) {
								int[] vert_order = tri_order.GetVertsForTriangle(tri_idx, false);
								int v0 = side_base_data.vert[vert_order[0]];
								int v1 = side_base_data.vert[vert_order[1]];
								int v2 = side_base_data.vert[vert_order[2]];
								mine_normal_calculator.AddMineTriangle(triangle_id, v0, v1, v2, side_material_name, false);
							}
						} else {
							// Note: Sides that are portals are ultimately defined by their master segment.
							// This is important because sides are quads, but not necessary co-planar. The master
							// side will determine the triangulation of the side, and we need to have both sides
							// of the portal agree on the triangulation. So, we need to detect if we are working
							// on a side that is a portal here, and if so, and it is for a slave side, then we
							// need to munge the vertex order to match that of the master side.
							bool is_slave_side_of_portal = segment_base_data.neighbor[sideIdx1] < segmentIndex1;

							if (is_slave_side_of_portal) {
								//
								// Special case for a slave-side portal 
								// Take the vertices of the master side
								//
								int master_segment_index = segment_base_data.neighbor[sideIdx1];
								var master_seg_data = convertState.OverloadLevelData.segment[master_segment_index];
								int master_side_index = Enumerable.Range(0, 6)
									 .First(msi => master_seg_data.neighbor[msi] == segmentIndex1);
								var master_side_data = master_seg_data.side[master_side_index];

								string side_material_name = master_side_data.tex_name;
								int triangle_id = MineMeshNormalGenerator.MakeTriangleId(segmentIndex1, sideIdx1);  // Even though we are generating the triangles from the master side, we need to register it with the actual slave side ID
								QuadTriangulationOrder tri_order = GetTriangulationOrder(convertState.OverloadLevelData, master_segment_index, master_side_index);
								for (int tri_idx = 0; tri_idx < 2; ++tri_idx) {
									int[] vert_order = tri_order.GetVertsForTriangle(tri_idx, true);
									int v0 = side_base_data.vert[vert_order[0]];
									int v1 = side_base_data.vert[vert_order[1]];
									int v2 = side_base_data.vert[vert_order[2]];
									mine_normal_calculator.AddMineTriangle(triangle_id, v0, v1, v2, side_material_name, true);
								}
							} else {
								//
								// General case for a side
								//
								string side_material_name = side_base_data.tex_name;
								int triangle_id = MineMeshNormalGenerator.MakeTriangleId(segmentIndex1, sideIdx1);
								QuadTriangulationOrder tri_order = GetTriangulationOrder(convertState.OverloadLevelData, segmentIndex1, sideIdx1);
								for (int tri_idx = 0; tri_idx < 2; ++tri_idx) {
									int[] vert_order = tri_order.GetVertsForTriangle(tri_idx, false);
									int v0 = side_base_data.vert[vert_order[0]];
									int v1 = side_base_data.vert[vert_order[1]];
									int v2 = side_base_data.vert[vert_order[2]];
									mine_normal_calculator.AddMineTriangle(triangle_id, v0, v1, v2, side_material_name, true);
								}
							}
						}
					}
				}
			}

			// Register the positions for the normal generator and generator the normals per vertex (pre-deformation)
			{
				int num_verts_total = convertState.OverloadLevelData.vertex.Length;
				for (int i = 0; i < num_verts_total; ++i) {
					if (!convertState.OverloadLevelData.vertex[i].alive) {
						continue;
					}
					mine_normal_calculator.SetVertexPosition(i, convertState.OverloadLevelData.vertex[i].position);
				}

				mine_normal_calculator.CalculateNormals();
			}

			// Next, we need to go through and actually tessellate the entire level, deforming verts where necessary.
			var deformed_vertices_map = new Dictionary<VertexKey, DeformedVertex>();
			var level_triangles = new List<LevelConvertTriangle>();
			using (LogCodeExecutionTime("Deforming segment verts")) {
				Func<VertexKey, DeformedVertex> getDeformationVertex = (key) =>
				{
					DeformedVertex deformedVert;
					if (!deformed_vertices_map.TryGetValue(key, out deformedVert)) {
						deformedVert = new DeformedVertex();
						deformed_vertices_map.Add(key, deformedVert);
					}
					return deformedVert;
				};

				Action<LevelConvertTriangle> registerConversionTriangle = (tri) =>
				{
					level_triangles.Add(tri);
				};

				foreach (var segmentIndex2 in convertState.OverloadLevelData.EnumerateAliveSegmentIndices()) {
					var segment_base_data = convertState.OverloadLevelData.segment[segmentIndex2];
					int segment_chunk_index = segment_base_data.m_chunk_num;

					// Update the chunk index -> segment index list
					List<int> segmentListForChunk;
					if (!convertState.ChunkToLevelSegmentIndices.TryGetValue(segment_chunk_index, out segmentListForChunk)) {
						segmentListForChunk = new List<int>();
						convertState.ChunkToLevelSegmentIndices.Add(segment_chunk_index, segmentListForChunk);
					}
					segmentListForChunk.Add(segmentIndex2);

					// Register neighboring sides for the segment and other preprocessing of the sides
					for (int sideIdx = 0; sideIdx < OverloadLevelEditor.Segment.NUM_SIDES; ++sideIdx) {
						bool isPortal = segment_base_data.neighbor[sideIdx] != -1;
						var side_base_data = segment_base_data.side[sideIdx];
						var segmentSubmeshKey = new SubmeshKey(side_base_data.tex_name, isPortal ? MeshBuilder.GeometryType.Portal : MeshBuilder.GeometryType.Segment);

						//Add texture to global list, if needed
						if (!convertState.TextureList.Contains(segmentSubmeshKey)) {
							convertState.TextureList.Add(segmentSubmeshKey);
						}

						//Add texture to dictionary for this chunk, if needed
						int side_submesh_index;
						if (!convertState.MaterialToSubmeshIndexByChunk[segment_chunk_index].TryGetValue(segmentSubmeshKey, out side_submesh_index)) {
							side_submesh_index = convertState.MaterialToSubmeshIndexByChunk[segment_chunk_index].Count;
							convertState.MaterialToSubmeshIndexByChunk[segment_chunk_index].Add(segmentSubmeshKey, side_submesh_index);
						}

						if (isPortal) {
							bool hasDecal = segment_base_data.side[sideIdx].decal.Any(d => d != null && !string.IsNullOrEmpty(d.mesh_name) && d.gmesh != null);
							int otherSegment = segment_base_data.neighbor[sideIdx];
							int otherSegmentSide = otherSegment == -1 ? -1 : Enumerable
							 .Range(0, OverloadLevelEditor.Segment.NUM_SIDES)
							 .Where(i => convertState.OverloadLevelData.segment[otherSegment].neighbor[i] == segmentIndex2)
							 .DefaultIfEmpty(-1)
							 .FirstOrDefault();
							bool otherHasDecal = otherSegmentSide == -1 ? false : convertState.OverloadLevelData.segment[otherSegment].side[otherSegmentSide].decal.Any(d => d != null && !string.IsNullOrEmpty(d.mesh_name) && d.gmesh != null);

							// Only connect these segments if neither side has a decal on the side
							if (hasDecal == false && otherHasDecal == false) {
								// But path finding will care
								convertState.LevelStructureBuilder.AddNeighborSegment(segmentIndex2, sideIdx, segment_base_data.neighbor[sideIdx]);
							}
						}
					}

					// This function captures the chunk index
					Func<SubmeshKey, int> lookupSubmeshIndex = (SubmeshKey segmentSubmeshKey) =>
					{
						return convertState.MaterialToSubmeshIndexByChunk[segment_chunk_index][segmentSubmeshKey];
					};

					// Note: Only getDeformationVertex and registerConversionTriangle will modify data, the rest of the arguments are read-only accesses
					DeformSegment(convertState.OverloadLevelData, side_tessellation_factor, enableDeformation, deformation_modules, mine_normal_calculator, segmentIndex2, lookupSubmeshIndex, getDeformationVertex, registerConversionTriangle);
				}
			}

			// Add all the positions to the deformed vert map from the adjacent verts
			using (LogCodeExecutionTime("Update deformed_vertices_map")) {
				for (int level_triangle_index = 0, num_level_triangles = level_triangles.Count; level_triangle_index < num_level_triangles; ++level_triangle_index) {
					var level_triangle = level_triangles[level_triangle_index];

					if (level_triangle.m_geom_type == MeshBuilder.GeometryType.Portal) {
						continue;
					}

					// This mask is 3 bits, 1 bit per edge start from the [0]->[1] edge at bit0. If the bit is
					// set, that means the edge should be considered virtual, which means it was created for triangulation
					// purposes and shouldn't be considered an 'artist edge'. We don't want to mark the two verts along a
					// virtual edge as adjacent, so that they won't influence each other.
					int level_triangle_virtual_edge_mask = level_triangle.m_virtual_edge_mask;
					for (uint vertIndex = 0; vertIndex < 3; ++vertIndex) {
						VertexKey current_vert_deformed_position_key = level_triangle.m_vertices[vertIndex].m_deformed_position_key;
						var deformed_vertex = deformed_vertices_map[current_vert_deformed_position_key];

						deformed_vertex.AddReferencedTriangleIndex(level_triangle_index);
						int edge0_index = (int)vertIndex;
						int edge1_index = (int)((vertIndex + 2) % 3);
						if ((level_triangle_virtual_edge_mask & (1 << edge0_index)) == 0) {
							deformed_vertex.AddAdjacentVertex(level_triangle.m_vertices[(vertIndex + 1) % 3].m_deformed_position_key);
						}
						if ((level_triangle_virtual_edge_mask & (1 << edge1_index)) == 0) {
							deformed_vertex.AddAdjacentVertex(level_triangle.m_vertices[(vertIndex + 2) % 3].m_deformed_position_key);
						}

						// For all lava edge verts, mark any verts that have the same position as a lava edge so they'll glow correctly (corners can be missed, currently)
						if (!level_triangle.m_vertices[vertIndex].m_corner_vert || !level_triangle.m_vertices[vertIndex].m_lava_edge) {
							continue;
						}

						Vector3 check_vert_pos = deformed_vertex.GetOriginalPosition();

						// Find all other edge verts, and compare the position
						foreach (var ltri in level_triangles) {
							for (int i = 0; i < 3; i++) {
								if (!ltri.m_vertices[i].m_corner_vert) {
									continue;
								}
								if ((deformed_vertices_map[ltri.m_vertices[i].m_deformed_position_key].GetOriginalPosition() - check_vert_pos).sqrMagnitude < 0.0001f) {
									ltri.m_vertices[i].m_lava_edge = true;
								}
							}
						}

					}
				}
			}

			// Smooth the level geometry before deformation, if desired.
			using (LogCodeExecutionTime("Smoothing passes")) {
				for (uint i = 0; i < num_pre_deform_smoothing_passes; i++) {
					foreach (DeformedVertex dv in deformed_vertices_map.Values) {
						dv.DoSmoothingPass(i, 1.0f, deformed_vertices_map);
					}
				}
			}

			Dictionary<int, Vector3> level_triangle_normal_cache = new Dictionary<int, Vector3>();
			Func<int, Vector3> calculate_level_triangle_normal = (level_tri_index) =>
			{
				Vector3 res;
				//if (level_triangle_normal_cache.TryGetValue(level_tri_index, out res)) {
				//    return res;
				//}

				LevelConvertTriangle working_tri = level_triangles[level_tri_index];

				if (working_tri.m_geom_type == MeshBuilder.GeometryType.Portal) {
					res = Vector3.zero;
				} else {
					Vector3[] post_deformed_verts = new Vector3[3];

					for (uint tri_vi = 0; tri_vi < 3; ++tri_vi) {
						post_deformed_verts[tri_vi] = deformed_vertices_map[working_tri.m_vertices[tri_vi].m_deformed_position_key].GetFinalDeformedPosition(num_pre_deform_smoothing_passes);
					}

					Vector3 normal = Vector3.Cross(post_deformed_verts[1] - post_deformed_verts[0], post_deformed_verts[2] - post_deformed_verts[0]);
					res = normal.normalized;
				}

				if (!level_triangle_normal_cache.ContainsKey(level_tri_index)) {
					level_triangle_normal_cache.Add(level_tri_index, res);
				}
				return res;
			};

			Func<int, Vector3> calculate_level_triangle_normal_smoothed = (level_tri_index) =>
			{
				Vector3 res;
				if (level_triangle_normal_cache.TryGetValue(level_tri_index, out res)) {
					return res;
				}

				LevelConvertTriangle working_tri = level_triangles[level_tri_index];

				if (working_tri.m_geom_type == MeshBuilder.GeometryType.Portal) {
					res = Vector3.zero;
				} else {
					Vector3[] post_deformed_verts = new Vector3[3];

					for (uint tri_vi = 0; tri_vi < 3; ++tri_vi) {
						post_deformed_verts[tri_vi] = deformed_vertices_map[working_tri.m_vertices[tri_vi].m_deformed_position_key].GetSmoothedPosition(num_pre_deform_smoothing_passes);
					}

					Vector3 normal = Vector3.Cross(post_deformed_verts[1] - post_deformed_verts[0], post_deformed_verts[2] - post_deformed_verts[0]);
					res = normal.normalized;
				}

				level_triangle_normal_cache.Add(level_tri_index, res);
				return res;
			};

			// Now that we've generated all of the verts, we can compute the final deformed positions for each.
			using (LogCodeExecutionTime("Calculating final deformed positions")) {
				for (int level_tri_index = 0, num_level_tris = level_triangles.Count; level_tri_index < num_level_tris; ++level_tri_index) {
					var level_triangle = level_triangles[level_tri_index];
					for (uint vertIndex = 0; vertIndex < 3; ++vertIndex) {
						DeformedVertex deformed_vertex = deformed_vertices_map[level_triangle.m_vertices[vertIndex].m_deformed_position_key];

						Vector3 pre_deform_normal;
						if (deformed_vertex.CanBeSmoothed) {
							// This vertex will participate in deformation. We'll calculate a normal for it
							int[] ref_tris = deformed_vertex.ReferencedTriangleIndices;
							Vector3 average_normal = Vector3.zero;
							foreach (var ref_tri_index in ref_tris) {
								average_normal += calculate_level_triangle_normal_smoothed(ref_tri_index);
							}
							pre_deform_normal = average_normal.normalized;

							// Save the normal as the deformation direction
							deformed_vertex.m_deformation_direction = pre_deform_normal;
						}

						Vector3 deformed_position = deformed_vertex.GetFinalDeformedPosition(num_pre_deform_smoothing_passes);
						deformed_vertex.UpdateSmoothingPosition(deformed_position);
					}
				}
			}

			// Smooth the level geometry again after deformation, if desired.
			using (LogCodeExecutionTime("Post-deformation smoothing passes")) {
				for (uint i = 0; i < num_post_deform_smoothing_passes; i++) {
					foreach (DeformedVertex dv in deformed_vertices_map.Values) {
						dv.DoSmoothingPass(i, 0.5f, deformed_vertices_map);
					}
				}
			}

			// Finally, we add the smoothed and deformed triangles to the actual submesh builders.
			using (LogCodeExecutionTime("Adding smoothed and deformed geometry to submesh builders")) {
				for (int level_tri_index = 0, num_level_tris = level_triangles.Count; level_tri_index < num_level_tris; ++level_tri_index) {
					var level_triangle = level_triangles[level_tri_index];
					int chunk_index = level_triangle.m_chunk_index;

					// Portal geometry never goes in the collision submesh builders.
					// Portal geometry only goes in the render submesh builder if debugAddPortalsToRenderGeometry is true.
					var submeshBuilderCollision = (level_triangle.m_geom_type != MeshBuilder.GeometryType.Portal) ? convertState.BuilderCollisionsByChunk[chunk_index].GetSubmeshBuilder(level_triangle.m_submesh_index, level_triangle.m_geom_type) : null;
					var submeshBuilderLavaCollision = ((level_triangle.m_geom_type != MeshBuilder.GeometryType.Portal) && (convertState.BuilderLavaCollisionsByChunk[chunk_index] != null)) ? convertState.BuilderLavaCollisionsByChunk[chunk_index].GetSubmeshBuilder(level_triangle.m_submesh_index, level_triangle.m_geom_type) : null;
					var submeshBuilderChunks = ((level_triangle.m_geom_type != MeshBuilder.GeometryType.Portal) || debugAddPortalsToRenderGeometry) ? convertState.BuilderGeometryByChunk[chunk_index].GetSubmeshBuilder(level_triangle.m_submesh_index, level_triangle.m_geom_type) : null;

					var fvt = new MeshBuilder.FaceVertex[3];

					for (uint vertIndex = 0; vertIndex < 3; ++vertIndex) {
						DeformedVertex deformed_vertex = deformed_vertices_map[level_triangle.m_vertices[vertIndex].m_deformed_position_key];

						Vector3 final_normal;
						if (deformed_vertex.CanBeSmoothed || deformed_vertex.IsDeformed) {
							// This vertex participated in deformation and/or smoothing. We'll calculate a normal for it
							// by averaging the normals of the triangles this vertex is a part of (post-deformation and smoothing)
							int[] ref_tris = deformed_vertex.ReferencedTriangleIndices;
							Vector3 average_normal = Vector3.zero;
							foreach (var ref_tri_index in ref_tris) {
								average_normal += calculate_level_triangle_normal(ref_tri_index);
							}
							final_normal = average_normal.normalized;
						} else {
							int triangle_id = MineMeshNormalGenerator.MakeTriangleId(level_triangle.m_from_segment, level_triangle.m_from_side);
							final_normal = deformed_vertex.GetAverageNormal(triangle_id);
						}

						fvt[vertIndex].Initialize();
						uint read_smoothing_pass_index = 1 - (num_post_deform_smoothing_passes & 1);
						fvt[vertIndex].SetPos(deformed_vertex.GetSmoothedPosition(read_smoothing_pass_index));
						fvt[vertIndex].SetNormal(final_normal);
						fvt[vertIndex].SetUV1(level_triangle.m_vertices[vertIndex].m_uv1);
						fvt[vertIndex].SetUV2(level_triangle.m_vertices[vertIndex].m_uv2);
						fvt[vertIndex].SetUV3(level_triangle.m_vertices[vertIndex].m_uv3);
						if (level_triangle.m_vertices[vertIndex].m_lava_edge) {
							// Lava edge is fully dark
							fvt[vertIndex].SetColor(new Vector4(1f, 1f, 1f, 0f));
						}
					}

					bool success;

					if (level_triangle.m_face_type == LevelConvertTriangle.FaceType.Lava && (submeshBuilderLavaCollision != null)) {
						success = submeshBuilderLavaCollision.AddTriangle(fvt[0], fvt[1], fvt[2]);
					} else if (submeshBuilderCollision != null) {
						success = submeshBuilderCollision.AddTriangle(fvt[0], fvt[1], fvt[2]);
					} else {
						success = true;
					}

					if (submeshBuilderChunks != null) {
						if (level_triangle.m_winding_order == LevelConvertTriangle.WindingOrder.Inverted) {
							success = submeshBuilderChunks.AddTriangle(fvt[2], fvt[1], fvt[0]) && success;
						} else {
							success = submeshBuilderChunks.AddTriangle(fvt[0], fvt[1], fvt[2]) && success;
						}
					}

					if (!success) {
						Debug.LogWarningFormat("Degenerate triangle skipped - seg:[{0}], side:[{1}]", level_triangle.m_from_segment, level_triangle.m_from_side);
					}
				}
			}

			using (LogCodeExecutionTime("Decal geometry generation")) {
				var swDecalNormalSmoothTime = new System.Diagnostics.Stopwatch();

				// Build up an AABB tree per segment, which is an AABB tree of the edges of the decals
				// within that segment. We'll use this to adjust normals of decal triangles that butt up
				// against decal geometry in another segment (on vertex or on edge)
				swDecalNormalSmoothTime.Start();
				Dictionary<int, DecalEdgeAABB> segmentToAABBQuery = BuildSegmentDecalEdgeAABB(convertState);
				swDecalNormalSmoothTime.Stop();

				foreach (var segmentIndex3 in convertState.OverloadLevelData.EnumerateAliveSegmentIndices()) {
					var segmentData = convertState.OverloadLevelData.segment[segmentIndex3];
					int chunknum = segmentData.m_chunk_num;

					// Get the neighboring segments
					swDecalNormalSmoothTime.Start();
					List<int> neighborSegments = new List<int>();
					for (int i = 0; i < OverloadLevelEditor.Segment.NUM_SIDES; ++i) {
						int neighborIndex = segmentData.neighbor[i];
						if (neighborIndex == -1) {
							continue;
						}
						neighborSegments.Add(neighborIndex);
					}
					swDecalNormalSmoothTime.Stop();

					// Go over the six sides of the segment
					for (int sideIdx3 = 0; sideIdx3 < OverloadLevelEditor.Segment.NUM_SIDES; ++sideIdx3) {
						var side = segmentData.side[sideIdx3];

						// Process geometry decals on the side.  Note that this happens whether or not the side is a portal
						foreach (var currDecal in side.decal) {
							if (currDecal == null || string.IsNullOrEmpty(currDecal.mesh_name) || currDecal.gmesh == null) {
								continue;
							}

							var decalMesh = currDecal.gmesh;

							// Get the decal color array
							var decalColors = new Vector3[4];
							for (int dcIdx = 0; dcIdx < 4 && dcIdx < decalMesh.m_color.Count; ++dcIdx) {
								decalColors[dcIdx] = decalMesh.m_color[dcIdx].ToUnity();
							}
							for (int dcIdx = decalMesh.m_color.Count; dcIdx < 4; ++dcIdx) {
								// Default to black
								decalColors[dcIdx].Set(0.0f, 0.0f, 0.0f);
							}

							// Process decal geometry
							for (int decalTriIdx = 0, decalNumTris = decalMesh.m_triangle.Count; decalTriIdx < decalNumTris; ++decalTriIdx) {
								var decalTri = decalMesh.m_triangle[decalTriIdx];

								// Resolve the submesh to use for this triangle's texture
								var textureName = decalMesh.m_tex_name[decalTri.tex_index];
								if (textureName.EndsWith(".png", StringComparison.InvariantCultureIgnoreCase)) {
									// Kill the ".png" extension on the texture name
									textureName = textureName.Substring(0, textureName.Length - ".png".Length);
								}

								var decalFvt = new MeshBuilder.FaceVertex[3];
								for (int i = 0; i < 3; ++i) {
									var vi = decalTri.vert[i];
									var vertData = decalMesh.m_vertex[vi];
									var uv1Data = decalTri.tex_uv[i];
									var normalData = decalMesh.m_triangle[decalTriIdx].normal[i];

									int colorIndex = -1;
									if ((decalTri.flags & (int)OverloadLevelEditor.FaceFlags.COLOR1) != 0) {
										colorIndex = 0;
									} else if ((decalTri.flags & (int)OverloadLevelEditor.FaceFlags.COLOR2) != 0) {
										colorIndex = 1;
									} else if ((decalTri.flags & (int)OverloadLevelEditor.FaceFlags.COLOR3) != 0) {
										colorIndex = 2;
									} else if ((decalTri.flags & (int)OverloadLevelEditor.FaceFlags.COLOR4) != 0) {
										colorIndex = 3;
									}

									var pos = vertData.ToUnity();
									var uv = uv1Data.ToUnity();
									var normal = normalData.ToUnity();

									if (flipTextureVCoordinateDecalGeom) {
										uv = new Vector2(uv.x, 1.0f - uv.y);
									}

									swDecalNormalSmoothTime.Start();
									if (convertState.FlagDecalNormalSmooth) {
										// Check to see if this point is shared to a decal in a neighboring segment
										foreach (int neighborSegIdx in neighborSegments) {
											DecalEdgeAABB neighborEdgeAABB;
											if (!segmentToAABBQuery.TryGetValue(neighborSegIdx, out neighborEdgeAABB)) {
												continue;
											}

											List<DecalEdgeAABB.QueryResult> potentialNeighborEdgeQueries = new List<DecalEdgeAABB.QueryResult>();
											foreach (DecalEdgeAABB.QueryResult neighborEdgeQuery in neighborEdgeAABB.Query(pos)) {
												if (neighborEdgeQuery == null) {
													continue;
												}

												// This decal's vert pos is on the edge of a decal triangle in another
												// segment. We probably want to average the normal.
												bool foundTexture = false;
												foreach (var neighborTexName in neighborEdgeQuery.TouchingTextures) {
													if (string.Compare(neighborTexName, textureName, true) == 0) {
														foundTexture = true;
														break;
													}
												}
												if (!foundTexture) {
													// Different texture
													continue;
												}

												// Check to see if the normal is in the general direction we are
												if (Vector3.Dot(neighborEdgeQuery.Normal, normal) <= 0.0f) {
													// Normal is not close
													continue;
												}

												// One of the potentials
												potentialNeighborEdgeQueries.Add(neighborEdgeQuery);
											}

											if (potentialNeighborEdgeQueries.Count > 0) {
												// Choose the normal that is closest to our vert's normal
												// This helps when there is a hardedge with neighboring
												// triangles with very different normals. We don't want to
												// pick some random one.
												var closestNormal = potentialNeighborEdgeQueries
													 .OrderByDescending(q => Vector3.Dot(normal, q.Normal))
													 .First();

												// Average the normal
												normal = (normal + closestNormal.Normal) * 0.5f;
												normal.Normalize();
											}

										}
									}
									swDecalNormalSmoothTime.Stop();

									decalFvt[i].Initialize();
									decalFvt[i].SetPos(pos);
									decalFvt[i].SetUV1(uv);
									decalFvt[i].SetNormal(normal);
									if (colorIndex != -1) {
										decalFvt[i].SetColor(decalColors[colorIndex]);
									}
								}

								var decalSubmeshKey = new SubmeshKey(textureName, MeshBuilder.GeometryType.Decal);

								//Add texture to global list, if needed
								if (!convertState.TextureList.Contains(decalSubmeshKey)) {
									convertState.TextureList.Add(decalSubmeshKey);
								}

								int submeshIdx;
								if (!convertState.MaterialToSubmeshIndexByChunk[chunknum].TryGetValue(decalSubmeshKey, out submeshIdx)) {
									submeshIdx = convertState.MaterialToSubmeshIndexByChunk[chunknum].Count;
									convertState.MaterialToSubmeshIndexByChunk[chunknum].Add(decalSubmeshKey, submeshIdx);
								}

								// Resolve the submesh builder for this triangle
								//  Decals can't have lava on them, so we dont need to worry about the lava collision builder here.
								var submeshBuilderCollision = convertState.BuilderCollisionsByChunk[chunknum].GetSubmeshBuilder(submeshIdx, decalSubmeshKey.GeometryType);
								var submeshBuilderChunks = convertState.BuilderGeometryByChunk[chunknum].GetSubmeshBuilder(submeshIdx, decalSubmeshKey.GeometryType);

								bool addCollision = (decalTri.flags & (int)OverloadLevelEditor.FaceFlags.NO_COLLIDE) == 0;
								bool addRender = (decalTri.flags & (int)OverloadLevelEditor.FaceFlags.NO_RENDER) == 0;

								bool success = true;
								if (addCollision) {
									success = submeshBuilderCollision.AddTriangle(decalFvt[0], decalFvt[1], decalFvt[2]) && success;
								}
								if (addRender) {
									success = submeshBuilderChunks.AddTriangle(decalFvt[0], decalFvt[1], decalFvt[2]) && success;
								}
								if (!success) {
									Debug.LogWarningFormat("Degenerate triangle skipped - decal:[{0}] seg:[{1}] side:[{2}]", currDecal.mesh_name, segmentIndex3, sideIdx3);
								}
							}
						}
					}
				}///
				Debug.LogFormat("TIME: Decal normal smoothing: {0}ms", swDecalNormalSmoothTime.ElapsedMilliseconds);
			}

			using (LogCodeExecutionTime("Automap Portal Geometry Generation")) {
				for (int level_tri_index = 0, num_level_tris = level_triangles.Count; level_tri_index < num_level_tris; ++level_tri_index) {
					var level_triangle = level_triangles[level_tri_index];

					if (level_triangle.m_geom_type != MeshBuilder.GeometryType.Portal) {
						continue;
					}

					PortalSideKey side_key = new PortalSideKey(level_triangle.m_from_segment, level_triangle.m_from_side);

					if (!convertState.PortalTriangles.ContainsKey(side_key)) {
						convertState.PortalTriangles.Add(side_key, new List<Vector3[]>());
					}

					List<Vector3[]> side_triangles = convertState.PortalTriangles[side_key];
					Vector3[] tri_vert_positions = new Vector3[3];

					for (uint vertIndex = 0; vertIndex < 3; ++vertIndex) {
						DeformedVertex deformed_vertex = deformed_vertices_map[level_triangle.m_vertices[vertIndex].m_deformed_position_key];

						uint read_smoothing_pass_index = 1 - (num_post_deform_smoothing_passes & 1);

						if (level_triangle.m_winding_order == LevelConvertTriangle.WindingOrder.Inverted) {
							tri_vert_positions[2 - vertIndex] = deformed_vertex.GetSmoothedPosition(read_smoothing_pass_index);
						} else {
							tri_vert_positions[vertIndex] = deformed_vertex.GetSmoothedPosition(read_smoothing_pass_index);
						}
					}

					side_triangles.Add(tri_vert_positions);
				}
			}
		}

		class DecalEdgeAABB
		{
			public class AABB
			{
				public class Data
				{
					public Vector3 Pos0;
					public Vector3 Pos1;
					public Vector3 Normal0;
					public Vector3 Normal1;
					public string TextureName;
					public bool IsShortEdge;
				}

				public AABB(Vector3 v0, Vector3 n0, Vector3 v1, Vector3 n1, string textureName)
				{
					const float epsilon = 0.01f;
					float min_x = Math.Min(v0.x, v1.x) - epsilon;
					float max_x = Math.Max(v0.x, v1.x) + epsilon;
					float min_y = Math.Min(v0.y, v1.y) - epsilon;
					float max_y = Math.Max(v0.y, v1.y) + epsilon;
					float min_z = Math.Min(v0.z, v1.z) - epsilon;
					float max_z = Math.Max(v0.z, v1.z) + epsilon;

					MinXYZ = new Vector3(min_x, min_y, min_z);
					MaxXYZ = new Vector3(max_x, max_y, max_z);
					LeafData = new Data();
					LeafData.Pos0 = v0;
					LeafData.Pos1 = v1;
					LeafData.Normal0 = n0;
					LeafData.Normal1 = n1;
					LeafData.TextureName = textureName;
					LeafData.IsShortEdge = Vector3.Distance(v0, v1) <= epsilon;
				}

				public AABB(Vector3 minXYZ, Vector3 maxXYZ)
				{
					MinXYZ = minXYZ;
					MaxXYZ = maxXYZ;
				}

				public Vector3 MinXYZ;
				public Vector3 MaxXYZ;
				public Data LeafData = null;

				public AABB Child0 = null;
				public AABB Child1 = null;
				public AABB Parent = null;
			}

			public AABB RootNode = null;

			public void RegisterEdge(Vector3 v0, Vector3 n0, Vector3 v1, Vector3 n1, string textureName)
			{
				var aabb = new AABB(v0, n0, v1, n1, textureName);

				if (RootNode == null) {
					// first node
					RootNode = aabb;
					return;
				}

				// find the best sibling node 
				AABB siblingNode = FindBestSiblingNode(RootNode, aabb);

				// Calculate the combined AABB of the node and best sibling
				Vector3 newParentMinXYZ, newParentMaxXYZ;
				GetCombinedAABB(aabb, siblingNode, out newParentMinXYZ, out newParentMaxXYZ);

				// Link the aabb and sibling to a new parent node
				var newParent = new AABB(newParentMinXYZ, newParentMaxXYZ);
				newParent.Parent = siblingNode.Parent;
				if (newParent.Parent != null) {
					if (newParent.Parent.Child0 == siblingNode) {
						newParent.Parent.Child0 = newParent;
					} else {
						newParent.Parent.Child1 = newParent;
					}
				}
				siblingNode.Parent = newParent;
				aabb.Parent = newParent;
				newParent.Child0 = siblingNode;
				newParent.Child1 = aabb;
				if (siblingNode == RootNode) {
					RootNode = newParent;
				}

				// Update AABB sizes on up
				UpdateParentAABB(newParent.Parent);
			}

			private void GetCombinedAABB(AABB n0, AABB n1, out Vector3 minXYZ, out Vector3 maxXYZ)
			{
				minXYZ = n0.MinXYZ;
				maxXYZ = n0.MaxXYZ;

				for (int i = 0; i < 3; ++i) {
					minXYZ[i] = Math.Min(minXYZ[i], n1.MinXYZ[i]);
					maxXYZ[i] = Math.Max(maxXYZ[i], n1.MaxXYZ[i]);
				}
			}

			private void UpdateParentAABB(AABB node)
			{
				if (node == null)
					return;

				Vector3 minXYZ;
				Vector3 maxXYZ;
				GetCombinedAABB(node.Child0, node.Child1, out minXYZ, out maxXYZ);
				node.MinXYZ = minXYZ;
				node.MaxXYZ = maxXYZ;

				UpdateParentAABB(node.Parent);
			}

			private AABB FindBestSiblingNode(AABB root, AABB testNode)
			{
				if (root.LeafData != null) {
					return root;
				}

				// Look at the child nodes, we can prune out an entire branch
				// if it is worse than the other, because there is no way any
				// of its children would be better.
				Vector3 c0MinXYZ, c0MaxXYZ;
				Vector3 c1MinXYZ, c1MaxXYZ;
				GetCombinedAABB(root.Child0, testNode, out c0MinXYZ, out c0MaxXYZ);
				GetCombinedAABB(root.Child1, testNode, out c1MinXYZ, out c1MaxXYZ);

				Vector3 c0Dim = c0MaxXYZ - c0MinXYZ;
				Vector3 c1Dim = c1MaxXYZ - c1MinXYZ;
				float c0Vol = c0Dim.x * c0Dim.y * c0Dim.z;
				float c1Vol = c1Dim.x * c1Dim.y * c1Dim.z;

				if (c0Vol < c1Vol) {
					// child0 is the better choice
					return FindBestSiblingNode(root.Child0, testNode);
				}

				// child1 is the better choice
				return FindBestSiblingNode(root.Child1, testNode);
			}

			public class QueryResult
			{
				public Vector3 Normal;
				public string[] TouchingTextures;
			}

			public IEnumerable<QueryResult> Query(Vector3 queryPos)
			{
				const float epsilon = 0.01f;
				const float epsilon2 = 0.001f;

				foreach (var aabb in FindIntersectingLeafs(RootNode, queryPos)) {
					Vector3 v0 = aabb.LeafData.Pos0;
					Vector3 v1 = aabb.LeafData.Pos1;

					if (aabb.LeafData.IsShortEdge) {
						// This edge is short, just query against the end points
						if (Vector3.Distance(queryPos, v0) <= epsilon) {
							QueryResult res = new QueryResult();
							res.Normal = aabb.LeafData.Normal0;
							res.TouchingTextures = new string[] { aabb.LeafData.TextureName };
							yield return res;
						}

						if (Vector3.Distance(queryPos, v1) <= epsilon) {
							QueryResult res = new QueryResult();
							res.Normal = aabb.LeafData.Normal1;
							res.TouchingTextures = new string[] { aabb.LeafData.TextureName };
							yield return res;
						}

						continue;
					}

					Vector3 v0ToV1 = v1 - v0;
					Vector3 v0ToQ = queryPos - v0;

					float segmentLen = v0ToV1.magnitude;
					v0ToV1 = v0ToV1 * (1.0f / segmentLen);

					// Project the v0ToQ vector onto the v0ToV1 direction
					float lenAlong = Vector3.Dot(v0ToQ, v0ToV1);
					if (lenAlong < -epsilon2 || lenAlong > (segmentLen + epsilon2)) {
						// Outside line segment
						continue;
					}
					lenAlong = Mathf.Clamp(lenAlong, 0.0f, segmentLen);

					Vector3 projectedQ = v0 + (v0ToV1 * lenAlong);
					if (Vector3.Distance(projectedQ, queryPos) > epsilon) {
						// The point is too far away from the edge line segment
						continue;
					}

					// The query pos is on the ray v0/v1
					float t = lenAlong / segmentLen;
					t = Math.Min(1.0f, Math.Max(0.0f, t));
					Vector3 interpNormal = (aabb.LeafData.Normal0 * (1.0f - t)) + (aabb.LeafData.Normal1 * t);
					interpNormal.Normalize();

					var r = new QueryResult();
					r.Normal = interpNormal;
					r.TouchingTextures = new string[] { aabb.LeafData.TextureName };
					yield return r;
				}
			}

			private IEnumerable<AABB> FindIntersectingLeafs(AABB rootNode, Vector3 queryPos)
			{
				if (rootNode == null) {
					yield break;
				}

				bool is_within = true;
				for (int i = 0; i < 3; ++i) {
					if (queryPos[i] < rootNode.MinXYZ[i] || queryPos[i] > rootNode.MaxXYZ[i]) {
						is_within = false;
						break;
					}
				}

				if (!is_within) {
					yield break;
				}

				if (rootNode.LeafData != null) {
					yield return rootNode;
				} else {
					foreach (var node in FindIntersectingLeafs(rootNode.Child0, queryPos)) {
						yield return node;
					}
					foreach (var node in FindIntersectingLeafs(rootNode.Child1, queryPos)) {
						yield return node;
					}
				}
			}
		}

		class BoundingBox
		{
			public Vector3 MinXYZ;
			public Vector3 MaxXYZ;

			public BoundingBox()
			{
				MinXYZ = MaxXYZ = Vector3.zero;
			}

			public BoundingBox(IEnumerable<Vector3> verts, float expansionLen)
			{
				bool isFirst = true;
				foreach (Vector3 v in verts) {
					if (isFirst) {
						isFirst = false;
						MinXYZ = MaxXYZ = v;
						continue;
					}

					for (int i = 0; i < 3; ++i) {
						MinXYZ[i] = Math.Min(MinXYZ[i], v[i]);
						MaxXYZ[i] = Math.Max(MaxXYZ[i], v[i]);
					}
				}

				for (int i = 0; i < 3; ++i) {
					MinXYZ[i] = MinXYZ[i] - expansionLen;
					MaxXYZ[i] = MaxXYZ[i] + expansionLen;
				}
			}

			public BoundingBox(OverloadLevelEditor.DTriangle decalTri, OverloadLevelEditor.GMesh decalMesh)
			{
				for (int i = 0; i < 3; ++i) {

					var v = decalMesh.m_vertex[decalTri.vert[i]].ToUnity();
					if (i == 0) {
						MinXYZ = MaxXYZ = v;
					} else {
						for (int c = 0; c < 3; ++c) {
							MinXYZ[c] = Math.Min(MinXYZ[c], v[c]);
							MaxXYZ[c] = Math.Max(MaxXYZ[c], v[c]);
						}
					}
				}
			}

			public bool Intersects(BoundingBox box)
			{
				for (int i = 0; i < 3; ++i) {
					if (box.MinXYZ[i] > MaxXYZ[i]) {
						// Min is beyond the max
						return false;
					}
					if (MinXYZ[i] > box.MaxXYZ[i]) {
						// Min is beyond the max
						return false;
					}
				}
				return true;
			}
		}

		private static Dictionary<int, DecalEdgeAABB> BuildSegmentDecalEdgeAABB(LevelConvertStateManager convertState)
		{
			Dictionary<int, DecalEdgeAABB> result = new Dictionary<int, LevelConvertState.DecalEdgeAABB>();
			if (!convertState.FlagDecalNormalSmooth) {
				return result;
			}

			foreach (var currSegmentIndex in convertState.OverloadLevelData.EnumerateAliveSegmentIndices()) {
				var segmentData = convertState.OverloadLevelData.segment[currSegmentIndex];
				//int chunknum = segmentData.m_chunk_num;

				// We only care about the triangles that are within range from portal polys - inner
				// triangles should not be butting up against neighboring decals. Get an AABB around
				// each portal (flared out) and only consider triangles that touch one of these AABBs.
				List<BoundingBox> portalBounds = new List<BoundingBox>();
				for (int currSideIdx = 0; currSideIdx < OverloadLevelEditor.Segment.NUM_SIDES; ++currSideIdx) {
					if (segmentData.neighbor[currSideIdx] == -1) {
						continue;
					}

					Vector3[] portalVerts = segmentData.side[currSideIdx].vert.Select(i => convertState.OverloadLevelData.vertex[i].position.ToUnity()).ToArray();
					const float expansionLen = 0.3f;
					var bbox = new BoundingBox(portalVerts, expansionLen);
					portalBounds.Add(bbox);
				}

				// Go over the six sides of the segment
				for (int currSideIdx = 0; currSideIdx < OverloadLevelEditor.Segment.NUM_SIDES; ++currSideIdx) {
					var side = segmentData.side[currSideIdx];

					foreach (var currDecal in side.decal) {
						if (currDecal == null || string.IsNullOrEmpty(currDecal.mesh_name) || currDecal.gmesh == null) {
							continue;
						}

						var decalMesh = currDecal.gmesh;

						DecalEdgeAABB decalAABB;
						if (!result.TryGetValue(currSegmentIndex, out decalAABB)) {
							decalAABB = new DecalEdgeAABB();
							result.Add(currSegmentIndex, decalAABB);
						}

						// Process decal geometry
						for (int decalTriIdx = 0, decalNumTris = decalMesh.m_triangle.Count; decalTriIdx < decalNumTris; ++decalTriIdx) {
							var decalTri = decalMesh.m_triangle[decalTriIdx];

							// Only consider this triangle if it is within bounds of a portal
							bool isNearPortal = false;
							BoundingBox decalTriBounds = new BoundingBox(decalTri, decalMesh);
							foreach (var portalBBox in portalBounds) {
								if (portalBBox.Intersects(decalTriBounds)) {
									isNearPortal = true;
									break;
								}
							}
							if (!isNearPortal) {
								continue;
							}

							string textureName = decalMesh.m_tex_name[decalTri.tex_index];
							if (textureName.EndsWith(".png", StringComparison.InvariantCultureIgnoreCase)) {
								// Kill the ".png" extension on the texture name
								textureName = textureName.Substring(0, textureName.Length - ".png".Length);
							}

							for (int i = 0; i < 3; ++i) {
								int next_i = (i + 1) % 3;

								var vertData0 = decalMesh.m_vertex[decalTri.vert[i]];
								var normalData0 = decalMesh.m_triangle[decalTriIdx].normal[i];

								var vertData1 = decalMesh.m_vertex[decalTri.vert[next_i]];
								var normalData1 = decalMesh.m_triangle[decalTriIdx].normal[next_i];

								decalAABB.RegisterEdge(vertData0.ToUnity(), normalData0.ToUnity(), vertData1.ToUnity(), normalData1.ToUnity(), textureName);
							}
						}
					}
				}
			}

			return result;
		}

		private static void DeformSegment(OverloadLevelEditor.Level levelData, uint side_tessellation_factor, bool enableDeformation, OverloadLevelEditor.DeformationModuleBaseNew[] deformation_modules, MineMeshNormalGenerator mine_normal_calculator, int segmentIndex, Func<SubmeshKey, int> lookupSubmeshIndex, Func<VertexKey, DeformedVertex> getDeformationVertex, Action<LevelConvertTriangle> registerConversionTriangle)
		{
			//var segmentBaseData = levelData.segment[segmentIndex];

			for (int sideIdx = 0; sideIdx < OverloadLevelEditor.Segment.NUM_SIDES; ++sideIdx) {
				DeformSegmentSide(levelData, side_tessellation_factor, enableDeformation, deformation_modules, mine_normal_calculator, segmentIndex, sideIdx, lookupSubmeshIndex, getDeformationVertex, registerConversionTriangle);
			}
		}

		private static void DeformSegmentSide(OverloadLevelEditor.Level levelData, uint sideTessellationFactor, bool enableDeformation, OverloadLevelEditor.DeformationModuleBaseNew[] deformationModules, MineMeshNormalGenerator mineNormalCalculator, int segmentIndex, int sideIdx, Func<SubmeshKey, int> lookupSubmeshIndex, Func<VertexKey, DeformedVertex> getDeformationVertex, Action<LevelConvertTriangle> registerConversionTriangle)
		{
			bool isPortal = levelData.segment[segmentIndex].neighbor[sideIdx] != -1;
			bool isPortalSlaveSide = isPortal && (levelData.segment[segmentIndex].neighbor[sideIdx] < segmentIndex);

			// Note: Sides that are portals are ultimately defined by their master segment.
			// This is important because sides are quads, but not necessary co-planar. The master
			// side will determine the triangulation of the side, and we need to have both sides
			// of the portal agree on the triangulation. So, we need to detect if we are working
			// on a side that is a portal here, and if so, and it is for a slave side, then we
			// need to munge the vertex order to match that of the master side.
			int geom_source_segment_index;
			int geom_source_side_index;

			if (isPortalSlaveSide) {
				// When tesselating/triangulating the slave side of a portal, we process it identically to the master side so that
				//  all of the triangulations match.  We flip the direction of the normals (since we're on the opposite side of
				//  the portal), and register the triangles with the slave side segment and side indices so we can properly
				//  retrieve them later.
				geom_source_segment_index = levelData.segment[segmentIndex].neighbor[sideIdx];
				geom_source_side_index = Enumerable.Range(0, 6).First(msi => levelData.segment[geom_source_segment_index].neighbor[msi] == segmentIndex);
			} else {
				geom_source_segment_index = segmentIndex;
				geom_source_side_index = sideIdx;
			}

			var segment_base_data = levelData.segment[segmentIndex];
			int segment_chunk_index = segment_base_data.m_chunk_num;
			var geom_source_side_base_data = levelData.segment[geom_source_segment_index].side[geom_source_side_index];

			SubmeshKey segmentSubmeshKey = new SubmeshKey(segment_base_data.side[sideIdx].tex_name, isPortal ? MeshBuilder.GeometryType.Portal : MeshBuilder.GeometryType.Segment);
			int side_submesh_index = lookupSubmeshIndex(segmentSubmeshKey);

			OverloadLevelEditor.DeformationModuleBaseNew deformation_module = null;
			float deformation_height = 0.0f;
			if (enableDeformation) {
				deformation_module = deformationModules[geom_source_side_base_data.deformation_preset];
				deformation_height = (deformation_module != null ? geom_source_side_base_data.deformation_height : 0.0f);
			}

			// For a side to be tessellated, it needs to have a non-zero deformation height, and needs to be an actual solid side, and not a portal.
			bool side_requires_tessellation = false;
			if (!isPortal) {
				side_requires_tessellation = (deformation_module != null) && (deformation_height > 0.0f);
			}

			// There are three different possible modes for a side to be in:
			//  1.) Side is non-deformed, none of its edges require tessellation.  This is the state all sides were in before we added deformation/tessellation,
			//      and should be handled the same way - side gets split diagonally and becomes two large triangles.
			//  2.) Side is non-deformed, but one or mode of its sides requires tessellation.  In this case, we add a vertex in the center of the face, and slice
			//      the face up like a pizza - for an edge that is untessellated, we generate one large triangle connecting the two endpoints to the center and covering
			//      1/4 of the side area.  For an edge that is tessellated, we generate a series of smaller slice triangles connecting each vertex in the edge to the center.
			//  3.) Side is deformed.  All edges must be tessellated in this case (we set that up in the first loop).  In this case, we subdivide the side into a square
			//      grid of smaller quads, each of which gets divided diagonally into two triangles.
			//
			//  Sides which are portals can only be of the first two types, since we do not allow deformation of portals (what would that even mean?).
			//

			bool some_edge_requires_tessellation = geom_source_side_base_data.SomeEdgeRequiresTesselation();
			bool[] edges_require_tessellation = geom_source_side_base_data.EdgesRequiringTesselation();

			Vector3[] cornerVertPositions = new Vector3[OverloadLevelEditor.Side.NUM_VERTS];
			Vector3[] cornerVertNormals = new Vector3[OverloadLevelEditor.Side.NUM_VERTS];
			Vector2[] cornerVertUV1s = new Vector2[OverloadLevelEditor.Side.NUM_VERTS];
			Vector2[] cornerVertUV2s = new Vector2[OverloadLevelEditor.Side.NUM_VERTS];
			Vector2[] cornerVertUV3s = new Vector2[OverloadLevelEditor.Side.NUM_VERTS];
			int triangle_id = MineMeshNormalGenerator.MakeTriangleId(segmentIndex, sideIdx);

			for (uint cornerIndex = 0; cornerIndex < OverloadLevelEditor.Side.NUM_VERTS; ++cornerIndex) {
				int vert_index = geom_source_side_base_data.vert[cornerIndex];

				cornerVertUV1s[cornerIndex] = geom_source_side_base_data.uv[cornerIndex].ToUnity();
				cornerVertUV2s[cornerIndex] = geom_source_side_base_data.uv2[cornerIndex].ToUnity();
				cornerVertUV3s[cornerIndex] = geom_source_side_base_data.uv3[cornerIndex].ToUnity();
				cornerVertPositions[cornerIndex] = levelData.vertex[vert_index].position.ToUnity();

				if (isPortalSlaveSide) {
					cornerVertNormals[cornerIndex] = -1.0f * mineNormalCalculator.GetVertexNormal(vert_index, triangle_id).ToUnity();
				} else {
					cornerVertNormals[cornerIndex] = mineNormalCalculator.GetVertexNormal(vert_index, triangle_id).ToUnity();
				}

				if (flipTextureVCoordinateLevelGeom) {
					cornerVertUV1s[cornerIndex] = new Vector2(cornerVertUV1s[cornerIndex].x, 1.0f - cornerVertUV1s[cornerIndex].y);
				}
			}

			if (side_requires_tessellation) {
				// TessellationFactor is the number of rows/columns of quads to tessellate into.
				// The total number of verts needed in each axis is TessellationFactor + 1, so we need that + 1 here, and <= instead of < in the for loops.
				LevelConvertVertex[,] tessellated_verts = new LevelConvertVertex[sideTessellationFactor + 1, sideTessellationFactor + 1];
				float u_divisor = 1.0f / Convert.ToSingle(sideTessellationFactor);
				float v_divisor = 1.0f / Convert.ToSingle(sideTessellationFactor);

				for (uint tessRowIndex = 0; tessRowIndex <= sideTessellationFactor; ++tessRowIndex) {
					for (uint tessColIndex = 0; tessColIndex <= sideTessellationFactor; ++tessColIndex) {

						// Calculate the tessellation UV for this tvert
						float u_lerp = Convert.ToSingle(tessColIndex) * u_divisor;
						float v_lerp = Convert.ToSingle(tessRowIndex) * v_divisor;

						// Calculate the untessellated position, normal, and uv of the mine at this tvert
						Vector3 tessVertPos = Vector3.Lerp(
							 Vector3.Lerp(cornerVertPositions[0], cornerVertPositions[1], u_lerp),
							 Vector3.Lerp(cornerVertPositions[3], cornerVertPositions[2], u_lerp),
							 v_lerp);
						Vector2 tessVertUV1 = Vector2.Lerp(
							 Vector2.Lerp(cornerVertUV1s[0], cornerVertUV1s[1], u_lerp),
							 Vector2.Lerp(cornerVertUV1s[3], cornerVertUV1s[2], u_lerp),
							 v_lerp);
						Vector2 tessVertUV2 = Vector2.Lerp(
							  Vector2.Lerp(cornerVertUV2s[0], cornerVertUV2s[1], u_lerp),
							  Vector2.Lerp(cornerVertUV2s[3], cornerVertUV2s[2], u_lerp),
							  v_lerp);
						Vector2 tessVertUV3 = Vector2.Lerp(
							  Vector2.Lerp(cornerVertUV3s[0], cornerVertUV3s[1], u_lerp),
							  Vector2.Lerp(cornerVertUV3s[3], cornerVertUV3s[2], u_lerp),
							  v_lerp);
						Vector3 tessVertNorm = Vector3.Lerp(
					 Vector3.Lerp(cornerVertNormals[0], cornerVertNormals[1], u_lerp).normalized,
					 Vector3.Lerp(cornerVertNormals[3], cornerVertNormals[2], u_lerp).normalized,
					 v_lerp).normalized;

						// This key is used so similar points of the mine match up, think of it like being the key to a point-in-space
						VertexKey tessVertKey = new VertexKey(geom_source_side_base_data.vert[0], geom_source_side_base_data.vert[1], geom_source_side_base_data.vert[2], geom_source_side_base_data.vert[3],
							 tessColIndex, sideTessellationFactor, tessRowIndex, sideTessellationFactor);

						// Calculate flags for the tvert
						bool lava_alpha = ((tessRowIndex == 0 && !geom_source_side_base_data.edge[0].adjacent_to_lava)
							 || (tessRowIndex == sideTessellationFactor && !geom_source_side_base_data.edge[2].adjacent_to_lava)
							 || (tessColIndex == 0 && !geom_source_side_base_data.edge[3].adjacent_to_lava)
							 || (tessColIndex == sideTessellationFactor && !geom_source_side_base_data.edge[1].adjacent_to_lava)
							 );
						bool corner_vert = (tessRowIndex == 0 || tessRowIndex == sideTessellationFactor) && (tessColIndex == 0 || tessColIndex == sideTessellationFactor);

						// Create the tvert
						tessellated_verts[tessRowIndex, tessColIndex] = new LevelConvertVertex(tessVertKey, tessVertUV1, tessVertUV2, tessVertUV3, lava_alpha && geom_source_side_base_data.is_lava, corner_vert);

						// Add information from this tvert to the point-in-space
						getDeformationVertex(tessVertKey).AddVertInformation(tessVertPos, tessVertNorm, deformation_module, deformation_height, triangle_id, isPortal);
					}
				}

				// For the triangles, we don't need the <=, since we're looping over the quads, not the vertices.
				for (uint tessRowIndex = 0; tessRowIndex < sideTessellationFactor; ++tessRowIndex) {
					for (uint tessColIndex = 0; tessColIndex < sideTessellationFactor; ++tessColIndex) {
						LevelConvertTriangle.FaceType face_type = geom_source_side_base_data.is_lava ? LevelConvertTriangle.FaceType.Lava : LevelConvertTriangle.FaceType.Default;
						LevelConvertTriangle.WindingOrder winding_order = isPortalSlaveSide ? LevelConvertTriangle.WindingOrder.Inverted : LevelConvertTriangle.WindingOrder.Default;
						// [0,0] [0,1] [1,1] -- virtual edge mask == 0b100 == 4
						// [0,0] [1,1] [1,0] -- virtual edge mask == 0b001 == 1
						registerConversionTriangle(new LevelConvertTriangle(tessellated_verts[tessRowIndex + 0, tessColIndex + 0], tessellated_verts[tessRowIndex + 0, tessColIndex + 1], tessellated_verts[tessRowIndex + 1, tessColIndex + 1], segment_chunk_index, side_submesh_index, segmentSubmeshKey.GeometryType, segmentIndex, sideIdx, face_type, winding_order, 4));
						registerConversionTriangle(new LevelConvertTriangle(tessellated_verts[tessRowIndex + 0, tessColIndex + 0], tessellated_verts[tessRowIndex + 1, tessColIndex + 1], tessellated_verts[tessRowIndex + 1, tessColIndex + 0], segment_chunk_index, side_submesh_index, segmentSubmeshKey.GeometryType, segmentIndex, sideIdx, face_type, winding_order, 1));
					}
				}
			} else if (some_edge_requires_tessellation) {
				// At least one edge of this side has tessellation, while at least one doesn't.
				LevelConvertVertex[,] tessellated_verts = new LevelConvertVertex[OverloadLevelEditor.Side.NUM_EDGES, sideTessellationFactor + 1];
				uint[] num_tessellated_verts = new uint[OverloadLevelEditor.Side.NUM_EDGES];

				float u_divisor = 1.0f / Convert.ToSingle(sideTessellationFactor);
				float v_divisor = 1.0f / Convert.ToSingle(sideTessellationFactor);

				// Top wedge:
				#region Top Edge
				if (edges_require_tessellation[0] == true) {
					num_tessellated_verts[0] = sideTessellationFactor + 1;

					for (uint tessIndex = 0; tessIndex <= sideTessellationFactor; ++tessIndex) {
						VertexKey tessVertKey = new VertexKey(geom_source_side_base_data.vert[0], geom_source_side_base_data.vert[1], geom_source_side_base_data.vert[2], geom_source_side_base_data.vert[3], tessIndex, sideTessellationFactor, 0, 1);

						float u_lerp = Convert.ToSingle(tessIndex) * u_divisor;
						Vector3 tessVertPos = Vector3.Lerp(cornerVertPositions[0], cornerVertPositions[1], u_lerp);
						Vector2 tessVertUV1 = Vector2.Lerp(cornerVertUV1s[0], cornerVertUV1s[1], u_lerp);
						Vector2 tessVertUV2 = Vector2.Lerp(cornerVertUV2s[0], cornerVertUV2s[1], u_lerp);
						Vector2 tessVertUV3 = Vector2.Lerp(cornerVertUV3s[0], cornerVertUV3s[1], u_lerp);
						Vector3 tessVertNorm = Vector3.Lerp(cornerVertNormals[0], cornerVertNormals[1], u_lerp).normalized;

						tessellated_verts[0, tessIndex] = new LevelConvertVertex(tessVertKey, tessVertUV1, tessVertUV2, tessVertUV3);
						getDeformationVertex(tessVertKey).AddVertInformation(tessVertPos, tessVertNorm, deformation_module, deformation_height, triangle_id, isPortal);
					}
				} else {
					num_tessellated_verts[0] = 2;

					VertexKey tessVertKey0 = new VertexKey(geom_source_side_base_data.vert[0], geom_source_side_base_data.vert[1], geom_source_side_base_data.vert[2], geom_source_side_base_data.vert[3], 0, 1, 0, 1);
					VertexKey tessVertKey1 = new VertexKey(geom_source_side_base_data.vert[0], geom_source_side_base_data.vert[1], geom_source_side_base_data.vert[2], geom_source_side_base_data.vert[3], 1, 1, 0, 1);

					tessellated_verts[0, 0] = new LevelConvertVertex(tessVertKey0, cornerVertUV1s[0], cornerVertUV2s[0], cornerVertUV3s[0]);
					tessellated_verts[0, 1] = new LevelConvertVertex(tessVertKey1, cornerVertUV1s[1], cornerVertUV2s[1], cornerVertUV3s[1]);

					getDeformationVertex(tessVertKey0).AddVertInformation(cornerVertPositions[0], cornerVertNormals[0], deformation_module, deformation_height, triangle_id, isPortal);
					getDeformationVertex(tessVertKey1).AddVertInformation(cornerVertPositions[1], cornerVertNormals[1], deformation_module, deformation_height, triangle_id, isPortal);
				}
				#endregion

				// Right wedge:
				#region Right Edge
				if (edges_require_tessellation[1] == true) {
					num_tessellated_verts[1] = sideTessellationFactor + 1;

					for (uint tessIndex = 0; tessIndex <= sideTessellationFactor; ++tessIndex) {
						VertexKey tessVertKey = new VertexKey(geom_source_side_base_data.vert[0], geom_source_side_base_data.vert[1], geom_source_side_base_data.vert[2], geom_source_side_base_data.vert[3], 1, 1, tessIndex, sideTessellationFactor);

						float v_lerp = Convert.ToSingle(tessIndex) * v_divisor;
						Vector3 tessVertPos = Vector3.Lerp(cornerVertPositions[1], cornerVertPositions[2], v_lerp);
						Vector2 tessVertUV1 = Vector2.Lerp(cornerVertUV1s[1], cornerVertUV1s[2], v_lerp);
						Vector2 tessVertUV2 = Vector2.Lerp(cornerVertUV2s[1], cornerVertUV2s[2], v_lerp);
						Vector2 tessVertUV3 = Vector2.Lerp(cornerVertUV3s[1], cornerVertUV3s[2], v_lerp);
						Vector3 tessVertNorm = Vector3.Lerp(cornerVertNormals[1], cornerVertNormals[2], v_lerp).normalized;

						tessellated_verts[1, tessIndex] = new LevelConvertVertex(tessVertKey, tessVertUV1, tessVertUV2, tessVertUV3);
						getDeformationVertex(tessVertKey).AddVertInformation(tessVertPos, tessVertNorm, deformation_module, deformation_height, triangle_id, isPortal);
					}
				} else {
					num_tessellated_verts[1] = 2;

					VertexKey tessVertKey0 = new VertexKey(geom_source_side_base_data.vert[0], geom_source_side_base_data.vert[1], geom_source_side_base_data.vert[2], geom_source_side_base_data.vert[3], 1, 1, 0, 1);
					VertexKey tessVertKey1 = new VertexKey(geom_source_side_base_data.vert[0], geom_source_side_base_data.vert[1], geom_source_side_base_data.vert[2], geom_source_side_base_data.vert[3], 1, 1, 1, 1);

					tessellated_verts[1, 0] = new LevelConvertVertex(tessVertKey0, cornerVertUV1s[1], cornerVertUV2s[1], cornerVertUV3s[1]);
					tessellated_verts[1, 1] = new LevelConvertVertex(tessVertKey1, cornerVertUV1s[2], cornerVertUV2s[2], cornerVertUV3s[2]);

					getDeformationVertex(tessVertKey0).AddVertInformation(cornerVertPositions[1], cornerVertNormals[1], deformation_module, deformation_height, triangle_id, isPortal);
					getDeformationVertex(tessVertKey1).AddVertInformation(cornerVertPositions[2], cornerVertNormals[2], deformation_module, deformation_height, triangle_id, isPortal);
				}
				#endregion

				// Bottom wedge:
				#region Bottom Edge
				if (edges_require_tessellation[2] == true) {
					num_tessellated_verts[2] = sideTessellationFactor + 1;

					for (uint tessIndex = 0; tessIndex <= sideTessellationFactor; ++tessIndex) {
						VertexKey tessVertKey = new VertexKey(geom_source_side_base_data.vert[0], geom_source_side_base_data.vert[1], geom_source_side_base_data.vert[2], geom_source_side_base_data.vert[3], (sideTessellationFactor - tessIndex), sideTessellationFactor, 1, 1);

						float u_lerp = Convert.ToSingle(sideTessellationFactor - tessIndex) * u_divisor;
						Vector3 tessVertPos = Vector3.Lerp(cornerVertPositions[3], cornerVertPositions[2], u_lerp);
						Vector2 tessVertUV1 = Vector2.Lerp(cornerVertUV1s[3], cornerVertUV1s[2], u_lerp);
						Vector2 tessVertUV2 = Vector2.Lerp(cornerVertUV2s[3], cornerVertUV2s[2], u_lerp);
						Vector2 tessVertUV3 = Vector2.Lerp(cornerVertUV3s[3], cornerVertUV3s[2], u_lerp);
						Vector3 tessVertNorm = Vector3.Lerp(cornerVertNormals[3], cornerVertNormals[2], u_lerp).normalized;

						tessellated_verts[2, tessIndex] = new LevelConvertVertex(tessVertKey, tessVertUV1, tessVertUV2, tessVertUV3);
						getDeformationVertex(tessVertKey).AddVertInformation(tessVertPos, tessVertNorm, deformation_module, deformation_height, triangle_id, isPortal);
					}
				} else {
					num_tessellated_verts[2] = 2;

					VertexKey tessVertKey0 = new VertexKey(geom_source_side_base_data.vert[0], geom_source_side_base_data.vert[1], geom_source_side_base_data.vert[2], geom_source_side_base_data.vert[3], 1, 1, 1, 1);
					VertexKey tessVertKey1 = new VertexKey(geom_source_side_base_data.vert[0], geom_source_side_base_data.vert[1], geom_source_side_base_data.vert[2], geom_source_side_base_data.vert[3], 0, 1, 1, 1);

					tessellated_verts[2, 0] = new LevelConvertVertex(tessVertKey0, cornerVertUV1s[2], cornerVertUV2s[2], cornerVertUV3s[2]);
					tessellated_verts[2, 1] = new LevelConvertVertex(tessVertKey1, cornerVertUV1s[3], cornerVertUV2s[3], cornerVertUV3s[3]);

					getDeformationVertex(tessVertKey0).AddVertInformation(cornerVertPositions[2], cornerVertNormals[2], deformation_module, deformation_height, triangle_id, isPortal);
					getDeformationVertex(tessVertKey1).AddVertInformation(cornerVertPositions[3], cornerVertNormals[3], deformation_module, deformation_height, triangle_id, isPortal);
				}
				#endregion

				// Left wedge:
				#region Left Edge
				if (edges_require_tessellation[3] == true) {
					num_tessellated_verts[3] = sideTessellationFactor + 1;

					for (uint tessIndex = 0; tessIndex <= sideTessellationFactor; ++tessIndex) {
						VertexKey tessVertKey = new VertexKey(geom_source_side_base_data.vert[0], geom_source_side_base_data.vert[1], geom_source_side_base_data.vert[2], geom_source_side_base_data.vert[3], 0, 1, (sideTessellationFactor - tessIndex), sideTessellationFactor);

						float v_lerp = Convert.ToSingle(sideTessellationFactor - tessIndex) * v_divisor;
						Vector3 tessVertPos = Vector3.Lerp(cornerVertPositions[0], cornerVertPositions[3], v_lerp);
						Vector2 tessVertUV1 = Vector2.Lerp(cornerVertUV1s[0], cornerVertUV1s[3], v_lerp);
						Vector2 tessVertUV2 = Vector2.Lerp(cornerVertUV2s[0], cornerVertUV2s[3], v_lerp);
						Vector2 tessVertUV3 = Vector2.Lerp(cornerVertUV3s[0], cornerVertUV3s[3], v_lerp);
						Vector3 tessVertNorm = Vector3.Lerp(cornerVertNormals[0], cornerVertNormals[3], v_lerp).normalized;

						tessellated_verts[3, tessIndex] = new LevelConvertVertex(tessVertKey, tessVertUV1, tessVertUV2, tessVertUV3);
						getDeformationVertex(tessVertKey).AddVertInformation(tessVertPos, tessVertNorm, deformation_module, deformation_height, triangle_id, isPortal);
					}
				} else {
					num_tessellated_verts[3] = 2;

					VertexKey tessVertKey0 = new VertexKey(geom_source_side_base_data.vert[0], geom_source_side_base_data.vert[1], geom_source_side_base_data.vert[2], geom_source_side_base_data.vert[3], 0, 1, 1, 1);
					VertexKey tessVertKey1 = new VertexKey(geom_source_side_base_data.vert[0], geom_source_side_base_data.vert[1], geom_source_side_base_data.vert[2], geom_source_side_base_data.vert[3], 0, 1, 0, 1);

					tessellated_verts[3, 0] = new LevelConvertVertex(tessVertKey0, cornerVertUV1s[3], cornerVertUV2s[3], cornerVertUV3s[3]);
					tessellated_verts[3, 1] = new LevelConvertVertex(tessVertKey1, cornerVertUV1s[0], cornerVertUV2s[0], cornerVertUV3s[0]);

					getDeformationVertex(tessVertKey0).AddVertInformation(cornerVertPositions[3], cornerVertNormals[3], deformation_module, deformation_height, triangle_id, isPortal);
					getDeformationVertex(tessVertKey1).AddVertInformation(cornerVertPositions[0], cornerVertNormals[0], deformation_module, deformation_height, triangle_id, isPortal);
				}
				#endregion

				VertexKey centerVertKey = new VertexKey(geom_source_side_base_data.vert[0], geom_source_side_base_data.vert[1], geom_source_side_base_data.vert[2], geom_source_side_base_data.vert[3], 1, 2, 1, 2);

				Vector3 centerVertPos = Vector3.Lerp(
					 Vector3.Lerp(cornerVertPositions[0], cornerVertPositions[1], 0.5f),
					 Vector3.Lerp(cornerVertPositions[3], cornerVertPositions[2], 0.5f),
					 0.5f);
				Vector2 centerVertUV1 = Vector2.Lerp(
					 Vector2.Lerp(cornerVertUV1s[0], cornerVertUV1s[1], 0.5f),
					 Vector2.Lerp(cornerVertUV1s[3], cornerVertUV1s[2], 0.5f),
					 0.5f);
				Vector2 centerVertUV2 = Vector2.Lerp(
					  Vector2.Lerp(cornerVertUV2s[0], cornerVertUV2s[1], 0.5f),
					  Vector2.Lerp(cornerVertUV2s[3], cornerVertUV2s[2], 0.5f),
					  0.5f);
				Vector2 centerVertUV3 = Vector2.Lerp(
					  Vector2.Lerp(cornerVertUV3s[0], cornerVertUV3s[1], 0.5f),
					  Vector2.Lerp(cornerVertUV3s[3], cornerVertUV3s[2], 0.5f),
					  0.5f);
				Vector3 centerVertNorm = Vector3.Lerp(
				Vector3.Lerp(cornerVertNormals[0], cornerVertNormals[1], 0.5f).normalized,
				Vector3.Lerp(cornerVertNormals[3], cornerVertNormals[2], 0.5f).normalized,
				0.5f).normalized;

				getDeformationVertex(centerVertKey).AddVertInformation(centerVertPos, centerVertNorm, deformation_module, deformation_height, triangle_id, isPortal);

				var centerVert = new LevelConvertVertex(centerVertKey, centerVertUV1, centerVertUV2, centerVertUV3);
				for (uint edgeIndex = 0; edgeIndex < OverloadLevelEditor.Side.NUM_EDGES; ++edgeIndex) {
					for (uint triangleIndex = 0; triangleIndex < num_tessellated_verts[edgeIndex] - 1; ++triangleIndex) {
						LevelConvertTriangle.FaceType face_type = geom_source_side_base_data.is_lava ? LevelConvertTriangle.FaceType.Lava : LevelConvertTriangle.FaceType.Default;
						LevelConvertTriangle.WindingOrder winding_order = isPortalSlaveSide ? LevelConvertTriangle.WindingOrder.Inverted : LevelConvertTriangle.WindingOrder.Default;
						// These triangles fan out from the centerVert out. In this case 2 of the 3 edges are 'virtual' (not artist defined) -- the two edges that
						// share the centerVert are virtual. Since vert[2] is the center vert this means edge 0 is the only non-virtual (0b110)
						registerConversionTriangle(new LevelConvertTriangle(tessellated_verts[edgeIndex, triangleIndex], tessellated_verts[edgeIndex, triangleIndex + 1], centerVert,
							segment_chunk_index, side_submesh_index, segmentSubmeshKey.GeometryType,
							segmentIndex, sideIdx, face_type, winding_order, 6));
					}
				}
			} else {
				// Just create two triangles from the side.  No additional tessellation, no deformation.
				VertexKey[] vertKeys = new VertexKey[4];
				vertKeys[0] = new VertexKey(geom_source_side_base_data.vert[0], geom_source_side_base_data.vert[1], geom_source_side_base_data.vert[2], geom_source_side_base_data.vert[3], 0, 1, 0, 1);
				vertKeys[1] = new VertexKey(geom_source_side_base_data.vert[0], geom_source_side_base_data.vert[1], geom_source_side_base_data.vert[2], geom_source_side_base_data.vert[3], 1, 1, 0, 1);
				vertKeys[2] = new VertexKey(geom_source_side_base_data.vert[0], geom_source_side_base_data.vert[1], geom_source_side_base_data.vert[2], geom_source_side_base_data.vert[3], 1, 1, 1, 1);
				vertKeys[3] = new VertexKey(geom_source_side_base_data.vert[0], geom_source_side_base_data.vert[1], geom_source_side_base_data.vert[2], geom_source_side_base_data.vert[3], 0, 1, 1, 1);

				LevelConvertVertex[] levelConvertVertices = new LevelConvertVertex[4];
				for (int i = 0; i < 4; ++i) {
					getDeformationVertex(vertKeys[i]).AddVertInformation(cornerVertPositions[i], cornerVertNormals[i], deformation_module, deformation_height, triangle_id, isPortal);
					levelConvertVertices[i] = new LevelConvertVertex(vertKeys[i], cornerVertUV1s[i], cornerVertUV2s[i], cornerVertUV3s[i]);
				}

				QuadTriangulationOrder triangulationOrder = GetTriangulationOrder(levelData, geom_source_segment_index, geom_source_side_index);
				int[] tri0Order = triangulationOrder.GetVertsForTriangle(0, false);
				int[] tri1Order = triangulationOrder.GetVertsForTriangle(1, false);
				int tr0VirtualEdge;
				int tr1VirtualEdge;
				ExtractSharedVirtualEdgeForQuadTriangles(tri0Order, tri1Order, out tr0VirtualEdge, out tr1VirtualEdge);

				LevelConvertTriangle.FaceType face_type = geom_source_side_base_data.is_lava ? LevelConvertTriangle.FaceType.Lava : LevelConvertTriangle.FaceType.Default;
				LevelConvertTriangle.WindingOrder winding_order = isPortalSlaveSide ? LevelConvertTriangle.WindingOrder.Inverted : LevelConvertTriangle.WindingOrder.Default;
				registerConversionTriangle(new LevelConvertTriangle(levelConvertVertices[tri0Order[0]], levelConvertVertices[tri0Order[1]], levelConvertVertices[tri0Order[2]], segment_chunk_index, side_submesh_index, segmentSubmeshKey.GeometryType, segmentIndex, sideIdx, face_type, winding_order, tr0VirtualEdge));
				registerConversionTriangle(new LevelConvertTriangle(levelConvertVertices[tri1Order[0]], levelConvertVertices[tri1Order[1]], levelConvertVertices[tri1Order[2]], segment_chunk_index, side_submesh_index, segmentSubmeshKey.GeometryType, segmentIndex, sideIdx, face_type, winding_order, tr1VirtualEdge));
			}
		}

		private static void ExtractSharedVirtualEdgeForQuadTriangles(int[] tri0Order, int[] tri1Order, out int tri0VirtualEdgeMask, out int tri1VirtualEdgeMask)
		{
			// One edge will be shared among the two triangles (in opposite order), find it
			for (int t0idx = 0; t0idx < 3; t0idx++) {
				int t0v0 = tri0Order[t0idx];
				int t0v1 = tri0Order[(t0idx + 1) % 3];

				for (int t1idx = 0; t1idx < 3; t1idx++) {
					int t1v0 = tri1Order[t1idx];
					int t1v1 = tri1Order[(t1idx + 1) % 3];

					if (t0v0 == t1v1 && t0v1 == t1v0) {
						// Found the matching pair
						tri0VirtualEdgeMask = 1 << t0idx;
						tri1VirtualEdgeMask = 1 << t1idx;
						return;
					}
				}
			}

			throw new Exception("Could not find virtual edge of quad");
		}

		private static OverloadLevelEditor.DeformationModuleBaseNew[] BuildDeformationModules(LevelConvertStateManager convertState)
		{
			OverloadLevelEditor.DeformationModuleBaseNew[] deformation_modules = new OverloadLevelEditor.DeformationModuleBaseNew[OverloadLevelEditor.LevelGlobalData.MAX_DEFORM_PRESETS];
			for (int deform_preset_index = 0; deform_preset_index < OverloadLevelEditor.LevelGlobalData.MAX_DEFORM_PRESETS; ++deform_preset_index) {
				switch (convertState.OverloadLevelData.global_data.deform_presets[deform_preset_index]) {
					case OverloadLevelEditor.DeformPreset.PLAIN_NOISE:
						{
							deformation_modules[deform_preset_index] = new OverloadLevelEditor.StandardDeformationModule(new OpenTK.Vector3(0.8f, 0.6f, 0.8f));

							var plain_noise_module = new OverloadLevelEditor.BasicSimplexNoiseModule(new OpenTK.Vector3(0.3f, 0.4f, 0.3f));
							plain_noise_module.AppendNoiseModifier(new OverloadLevelEditor.ScaleAndBiasNoiseModifier(1.5f, -0.5f));

							deformation_modules[deform_preset_index].AddNoiseModule(plain_noise_module);
						}
						break;
					case OverloadLevelEditor.DeformPreset.V_STRIPES:
						{
							deformation_modules[deform_preset_index] = new OverloadLevelEditor.StandardDeformationModule(new OpenTK.Vector3(0.9f, 0.8f, 0.9f));

							var v_stripes_noise_module = new OverloadLevelEditor.BasicSimplexNoiseModule(new OpenTK.Vector3(0.3f, 0.09f, 0.3f), true);
							v_stripes_noise_module.AppendNoiseModifier(new OverloadLevelEditor.ScaleAndBiasNoiseModifier(1f, 1f));
							v_stripes_noise_module.AppendNoiseModifier(new OverloadLevelEditor.ExponentNoiseModifier(2f));
							v_stripes_noise_module.AppendNoiseModifier(new OverloadLevelEditor.ScaleAndBiasNoiseModifier(1.5f, -1.6f));

							var ceiling_noise_module = new OverloadLevelEditor.BasicSimplexNoiseModule(new OpenTK.Vector3(0.22f, 0.1f, 0.22f), false, true, true);
							ceiling_noise_module.AppendNoiseModifier(new OverloadLevelEditor.ScaleAndBiasNoiseModifier(1f, 1f));
							ceiling_noise_module.AppendNoiseModifier(new OverloadLevelEditor.ExponentNoiseModifier(1.2f));
							ceiling_noise_module.AppendNoiseModifier(new OverloadLevelEditor.ScaleAndBiasNoiseModifier(1.5f, -1.5f));

							deformation_modules[deform_preset_index].AddNoiseModule(v_stripes_noise_module);
							deformation_modules[deform_preset_index].AddNoiseModule(ceiling_noise_module);
						}
						break;
					case OverloadLevelEditor.DeformPreset.POINTY:
						{
							deformation_modules[deform_preset_index] = new OverloadLevelEditor.StandardDeformationModule(new OpenTK.Vector3(1.2f, 1f, 1.2f));

							var v_stripes_noise_module = new OverloadLevelEditor.BasicSimplexNoiseModule(new OpenTK.Vector3(0.25f, 0.12f, 0.25f), true);
							v_stripes_noise_module.AppendNoiseModifier(new OverloadLevelEditor.ScaleAndBiasNoiseModifier(1f, 1f));
							v_stripes_noise_module.AppendNoiseModifier(new OverloadLevelEditor.ExponentNoiseModifier(1.3f));
							v_stripes_noise_module.AppendNoiseModifier(new OverloadLevelEditor.ScaleAndBiasNoiseModifier(2f, -1.5f));

							var ceiling_noise_module = new OverloadLevelEditor.BasicSimplexNoiseModule(new OpenTK.Vector3(0.25f, 0.25f, 0.25f), false, true, true);
							ceiling_noise_module.AppendNoiseModifier(new OverloadLevelEditor.AbsoluteValueNoiseModifier());
							ceiling_noise_module.AppendNoiseModifier(new OverloadLevelEditor.ExponentNoiseModifier(2.5f));
							ceiling_noise_module.AppendNoiseModifier(new OverloadLevelEditor.ScaleAndBiasNoiseModifier(6f, -1.5f));

							deformation_modules[deform_preset_index].AddNoiseModule(v_stripes_noise_module);
							deformation_modules[deform_preset_index].AddNoiseModule(ceiling_noise_module);
						}
						break;
					case OverloadLevelEditor.DeformPreset.H_RIDGES:
						{
							deformation_modules[deform_preset_index] = new OverloadLevelEditor.StandardDeformationModule(new OpenTK.Vector3(1f, 0.35f, 1f));

							var h_ridges_base_noise_module = new OverloadLevelEditor.BasicSimplexNoiseModule();
							h_ridges_base_noise_module.AppendNoiseModifier(new OverloadLevelEditor.AbsoluteValueNoiseModifier());
							h_ridges_base_noise_module.AppendNoiseModifier(new OverloadLevelEditor.ScaleAndBiasNoiseModifier(-1.0f, 1.0f));

							var h_ridges_noise_module = new OverloadLevelEditor.RidgedMultiFractalNoiseModule(new OpenTK.Vector3(0.06f, 0.18f, 0.06f), 3, h_ridges_base_noise_module, 1.0f, 2.0f, 0.0f);
							h_ridges_noise_module.AppendNoiseModifier(new OverloadLevelEditor.ExponentNoiseModifier(0.9f));
							h_ridges_noise_module.AppendNoiseModifier(new OverloadLevelEditor.ScaleAndBiasNoiseModifier(2f, -2f));

							deformation_modules[deform_preset_index].AddNoiseModule(h_ridges_noise_module);
						}
						break;
					case OverloadLevelEditor.DeformPreset.FRACTAL_NOISE:	// INVERTED RIDGES
						{
							deformation_modules[deform_preset_index] = new OverloadLevelEditor.StandardDeformationModule(new OpenTK.Vector3(0.8f, 0.8f, 0.8f));

							var fractal_base_noise_module = new OverloadLevelEditor.BasicSimplexNoiseModule();
							fractal_base_noise_module.AppendNoiseModifier(new OverloadLevelEditor.AbsoluteValueNoiseModifier());
							//fractal_base_noise_module.AppendNoiseModifier(new OverloadLevelEditor.ScaleAndBiasNoiseModifier(-1.0f, 1.0f));

							var fractal_noise_module = new OverloadLevelEditor.RidgedMultiFractalNoiseModule(new OpenTK.Vector3(0.12f, 0.12f, 0.12f), 3, fractal_base_noise_module, 1.0f, 2.0f, 0f);
							fractal_noise_module.AppendNoiseModifier(new OverloadLevelEditor.ExponentNoiseModifier(0.4f));
							fractal_noise_module.AppendNoiseModifier(new OverloadLevelEditor.ScaleAndBiasNoiseModifier(2f, -1.2f));

							deformation_modules[deform_preset_index].AddNoiseModule(fractal_noise_module);
						}
						break;
					case OverloadLevelEditor.DeformPreset.BILLOW_NOISE:	// BUMPY
						{
							deformation_modules[deform_preset_index] = new OverloadLevelEditor.StandardDeformationModule(new OpenTK.Vector3(0.6f, 0.6f, 0.6f));

							var billow_base_noise_module = new OverloadLevelEditor.BasicSimplexNoiseModule();
							billow_base_noise_module.AppendNoiseModifier(new OverloadLevelEditor.AbsoluteValueNoiseModifier());
							
							var billow_noise_module = new OverloadLevelEditor.StandardFractalNoiseModule(new OpenTK.Vector3(0.25f, 0.25f, 0.25f), 2, billow_base_noise_module, 1.0f, 2.0f);
							billow_noise_module.AppendNoiseModifier(new OverloadLevelEditor.ExponentNoiseModifier(1.2f));
							billow_noise_module.AppendNoiseModifier(new OverloadLevelEditor.ScaleAndBiasNoiseModifier(2.3f, -1.5f));

							deformation_modules[deform_preset_index].AddNoiseModule(billow_noise_module);
						}
						break;
					case OverloadLevelEditor.DeformPreset.RIDGED_NOISE:	// LARGE WAVES
						{
							deformation_modules[deform_preset_index] = new OverloadLevelEditor.StandardDeformationModule(new OpenTK.Vector3(0.9f, 0.9f, 0.9f));

							var h_ridges_base_noise_module = new OverloadLevelEditor.BasicSimplexNoiseModule();
							h_ridges_base_noise_module.AppendNoiseModifier(new OverloadLevelEditor.AbsoluteValueNoiseModifier());
							h_ridges_base_noise_module.AppendNoiseModifier(new OverloadLevelEditor.ScaleAndBiasNoiseModifier(1.2f, 0f));

							var h_ridges_noise_module = new OverloadLevelEditor.RidgedMultiFractalNoiseModule(new OpenTK.Vector3(0.1f, 0.1f, 0.1f), 3, h_ridges_base_noise_module, 1.0f, 2.0f, 0.0f);
							h_ridges_noise_module.AppendNoiseModifier(new OverloadLevelEditor.ScaleAndBiasNoiseModifier(2f, -1.0f));

							deformation_modules[deform_preset_index].AddNoiseModule(h_ridges_noise_module);
						}
						break;
					case OverloadLevelEditor.DeformPreset.NONE:
					default:
						deformation_modules[deform_preset_index] = null;
						break;
				}
			}

			return deformation_modules;
		}

#if OVERLOAD_LEVEL_EDITOR
        public static Dictionary<SubmeshKey, Material> BuildMaterialsDictionaryFromTextureList(List<SubmeshKey> textureList)
#else
		public static Dictionary<SubmeshKey, Material> BuildMaterialsDictionaryFromTextureList(List<SubmeshKey> textureList, Dictionary<string, ProceduralMaterial> substanceLevelLookupMap, Dictionary<string, ProceduralMaterial> substanceDecalLookupMap)
#endif
		{
			//var numMaterials = textureList.Count;
			var textureToMaterial = new Dictionary<SubmeshKey, Material>();
			foreach (var materialSubmeshKey in textureList) {
				var materialName = materialSubmeshKey.MaterialName;

				// Some materials (for decals) are getting directory names, so this fixes that if it happens
				materialName = OverloadLevelEditor.Utility.GetPathlessFilename(materialName);

				var geometryType = materialSubmeshKey.GeometryType;

				if (string.IsNullOrEmpty(materialName)) {
					materialName = "Default";
				}

#if OVERLOAD_LEVEL_EDITOR
                var material = new Material(materialName, geometryType);
                textureToMaterial.Add(materialSubmeshKey, material);
#else

				// Substance assets have top priority, so first look for the material in the substances
				var substanceLookupMap = substanceLevelLookupMap;
				if (geometryType == MeshBuilder.GeometryType.Decal) {
					substanceLookupMap = substanceDecalLookupMap;
				}

				ProceduralMaterial substanceMaterial;
				if (substanceLookupMap.TryGetValue(materialName, out substanceMaterial)) {
					// Found it as a Substance material
					textureToMaterial.Add(materialSubmeshKey, substanceMaterial);
					continue;
				}

				// Search the asset database for the material
				var materialMatchGUIDs = AssetDatabase.FindAssets(materialName + " t:Material");
				if (materialMatchGUIDs == null || materialMatchGUIDs.Length == 0) {

					if (materialName.EndsWith("_diffuse", StringComparison.InvariantCultureIgnoreCase)) {
						// Some texture names have '_diffuse' at the end - remove and look for a material again
						var adjustedMaterialName = materialName.Substring(0, materialName.Length - "_diffuse".Length);
						materialMatchGUIDs = AssetDatabase.FindAssets(adjustedMaterialName + " t:Material");
					}

					if (materialMatchGUIDs == null || materialMatchGUIDs.Length == 0) {
						UnityEngine.Debug.LogWarning(string.Format("Unable to find a material containing the name '{0}' - auto-generating a material", materialName));

						// Create a new asset as a placeholder
						var material = new Material(Shader.Find("Diffuse"));
						material.color = Color.green;
						string autoGeneratedMaterialFolder = "Assets/materials/AutoGenerated/";
						EnsureUnityAssetFolderExists(autoGeneratedMaterialFolder);
						AssetDatabase.CreateAsset(material, autoGeneratedMaterialFolder + materialName + ".mat");
						AssetDatabase.Refresh();
						textureToMaterial.Add(materialSubmeshKey, material);
						continue;
					}
				}

				// TODO(Jeff): If there are multiple options - should we pick the one that matches the name the closest?
				var materialPath = AssetDatabase.GUIDToAssetPath(materialMatchGUIDs[0]);
				textureToMaterial.Add(materialSubmeshKey, (Material)AssetDatabase.LoadAssetAtPath(materialPath, typeof(Material)));
#endif
			}

			return textureToMaterial;
		}

		Material[] BindMaterialsChunk(LevelConvertStateManager convertState, int chunknum)
		{
			var numMaterials = convertState.MaterialToSubmeshIndexByChunk[chunknum].Count;
			var materials = new Material[numMaterials];
			foreach (var kvp in convertState.MaterialToSubmeshIndexByChunk[chunknum]) {
				var materialSubmeshKey = kvp.Key;
				var submeshIndex = kvp.Value;

				materials[submeshIndex] = convertState.TextureToUnityMaterialMap[materialSubmeshKey];
			}

			return materials;
		}
	}
}
