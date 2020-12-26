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
using OpenTK;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

// DPOLY - Editor
// More general functions/variables for polygons

namespace OverloadLevelEditor
{
	public partial class DPoly
	{
		public List<int> export_list_idx = new List<int>();
		public bool should_normalize = false;

		public static DPoly CombinePolys(DPoly p1, DPoly p2)
		{
			DPoly p_out = new DPoly(p1);

			// Find the first matching vert
			int match_1st1 = -1;
			int match_1st2 = -1;
			for (int i = 0; i < p1.num_verts; i++) {
				for (int j = 0; j < p2.num_verts; j++) {
					if (p1.vert[i] == p2.vert[j] && match_1st1 < 0) {
						match_1st1 = i;
						match_1st2 = j;
					}
				}
			}

			// Find the second matching vert
			int match_2nd1 = -1;
			int match_2nd2 = -1;
			for (int i = match_1st1 + 1; i < p1.num_verts; i++) {
				for (int j = 0; j < p2.num_verts; j++) {
					if (p1.vert[i] == p2.vert[j] && match_2nd1 < 0) {
						match_2nd1 = i;
						match_2nd2 = j;
					}
				}
			}

			if ((match_1st1 + 1) % p1.num_verts == match_2nd1) {
				// Use it
			} else {
				// Use the other
				match_1st1 = match_2nd1;
				match_1st2 = match_2nd2;
			}

			// Insert the new verts
			int idx1, idx2;
			for (int i = 0; i < p2.num_verts - 2; i++) {
				idx1 = (match_1st1 + i + 1) % p1.num_verts;
				idx2 = (match_1st2 + i + 1) % p2.num_verts;
				p_out.vert.Insert(idx1, p2.vert[idx2]);
				p_out.normal.Insert(idx1, p2.normal[idx2]);
				p_out.tex_uv.Insert(idx1, p2.tex_uv[idx2]);
			}

			p_out.num_verts += (p2.num_verts - 2);

			return p_out;
		}

		// Can you combine this poly with the passed in one
		public static bool CanCombinePolys(DPoly p1, DPoly p2, DMesh dm)
		{
			// First check for two shared verts
			int count = 0;
			for (int i = 0; i < p1.num_verts; i++) {
				for (int j = 0; j < p2.num_verts; j++) {
					if (p1.vert[i] == p2.vert[j]) {
						count += 1;
					}
				}
			}

			if (count != 2) {
				return false;
			}

			// Check for normals that align
			Vector3 n1 = CalculatePolyNormal(p1, dm);
			Vector3 n2 = CalculatePolyNormal(p2, dm);

			if (Vector3.Dot(n1, n2) >= 0.995f) {
				return true;
			} else {
				return false;
			}
		}

		public static bool HasTwoOrMoreSharedVerts(DPoly p1, DPoly p2)
		{
			int count = 0;
			for (int i = 0; i < p1.num_verts; i++) {
				for (int j = 0; j < p2.num_verts; j++) {
					if (p1.vert[i] == p2.vert[j]) {
						count += 1;
					}
				}
			}

			return (count >= 2);
		}

		public static bool HasThreeOrMoreCloseVerts(DPoly p1, DPoly p2, DMesh dm)
		{
			int count = 0;
			for (int i = 0; i < p1.num_verts; i++) {
				for (int j = 0; j < p2.num_verts; j++) {
					if (Utility.AlmostOverlapping(dm.vertex[p1.vert[i]], dm.vertex[p2.vert[j]])) {
						count += 1;
					}
				}
			}

			return (count >= 3);
		}
		
		public static Vector3 CalculatePolyNormal(DPoly p, DMesh dm) 
		{
			Vector3 n = Vector3.Zero;

			n = Utility.FindNormal(dm.vertex[p.vert[0]], dm.vertex[p.vert[1]], dm.vertex[p.vert[2]]);

			return n;
		}

		public void RecalculateNormal(DMesh dm)
		{
			should_normalize = false;
			face_normal = CalculatePolyNormal(this, dm);
			for (int i = 0; i < num_verts; i++) {
				normal[i] = face_normal;
			}
		}

		public void NormalizeNormals()
		{
			for (int i = 0; i < num_verts; i++) {
				normal[i] = normal[i].Normalized();
			}
		}

		public void ReplaceVert(int old_vert, int new_vert)
		{
			for (int i = 0; i < num_verts; i++) {
				if (vert[i] == old_vert) {
					vert[i] = new_vert;
					return;
				}
			}
		}

		public bool HasAVert(int v)
		{
			for (int i = 0; i < num_verts; i++) {
				if (vert[i] == v) {
					return true;
				}
			}

			return false;
		}

		public bool HasBothVerts(int v1, int v2)
		{
			int count = 0;
			for (int i = 0; i < num_verts; i++) {
				if (vert[i] == v1 || vert[i] == v2) {
					count += 1;
				}
			}

			return (count == 2);
		}

		public bool HasBothVertsAdjacent(int v1, int v2)
		{
			if (HasBothVerts(v1, v2)) {
				for (int i = 0; i < num_verts; i++) {
					if (vert[i] == v1) {
						if (vert[(i + 1) % num_verts] == v2) {
							return true;
						}
					} else if (vert[i] == v2) {
						if (vert[(i + 1) % num_verts] == v1) {
							return true;
						}
					}
				}
				return false;
			} else {
				return false;
			}
		}

		public bool HasBothVertsNonAdjacent(int v1, int v2)
		{
			if (HasBothVerts(v1, v2)) {
				for (int i = 0; i < num_verts; i++) {
					if (vert[i] == v1) {
						if (vert[(i + 1) % num_verts] == v2) {
							return false;
						}
					} else if (vert[i] == v2) {
						if (vert[(i + 1) % num_verts] == v1) {
							return false;
						}
					}
				}
				return true;
			} else {
				return false;
			}
		}

		public List<int> GetVertsStartUpTo(int start_v, int upto_v)
		{
			List<int> vert_list = new List<int>();
			int start_idx = FindVertByIndex(start_v);
			int end_idx = FindVertByIndex(upto_v);

			for (int i = start_idx + 1; i < num_verts + start_idx; i++) {
				if (i % num_verts == end_idx) {
					return vert_list;
				} else {
					vert_list.Add(i % num_verts);
				}
			}

			return null;
		}

		public void RemoveVertList(List<int> vert_list)
		{
			int high_idx;
			int list_idx;
			int count = vert_list.Count;
			for (int i = 0; i < count; i++) {
				// Remove the highest idx vert
				high_idx = -1;
				list_idx = -1;

				for (int j = 0; j < vert_list.Count; j++) {
					if (vert_list[j] > high_idx) {
						high_idx = vert_list[j];
						list_idx = j;
					}
				}

				if (high_idx > -1) {
					RemoveVert(high_idx);
					vert_list.RemoveAt(list_idx);
				}
			}
		}

		public void RemoveVert(int idx)
		{
			num_verts -= 1;
			vert.RemoveAt(idx);
			tex_uv.RemoveAt(idx);
			normal.RemoveAt(idx);
		}

		public static int AddedVert = -1;

		// Make sure you reset AddedVert to -1 if doing a new cut (it's meant to cache a vert for cutting multiple edges at once)
		public void MaybeAddVertBetween(int v1, int v2, DMesh dmesh)
		{
			for (int i = 0; i < num_verts; i++) {
				if (v1 == vert[i]) {
					if (i > 0 && (vert[i - 1] == v2)) {
						AddVertBetween(i - 1, i, dmesh);
						return;
					} else if (i < num_verts - 1 && vert[i + 1] == v2) {
						AddVertBetween(i, i + 1, dmesh);
						return;
					} else if (i == 0 && v2 == vert[num_verts - 1]) {
						AddVertBetween(num_verts - 1, 0, dmesh);
						return;
					} else if (v2 == vert[0] && i == num_verts - 1) {
						AddVertBetween(num_verts - 1, 0, dmesh);
						return;
					}
				}
			}
		}

		public void AddVertBetween(int idx1, int idx2, DMesh dmesh)
		{
			int new_vert_idx;
			if (AddedVert < 0) {
				Vector3 pos = (dmesh.vertex[vert[idx1]] + dmesh.vertex[vert[idx2]]) * 0.5f;
				dmesh.AddVertexEditor(pos, false);
				new_vert_idx = dmesh.vertex.Count - 1;
				AddedVert = new_vert_idx;
			} else {
				new_vert_idx = AddedVert;
			}
			Vector2 new_uv = (tex_uv[idx1] + tex_uv[idx2]) * 0.5f;

			vert.Insert(idx2, new_vert_idx);
			normal.Insert(idx2, face_normal);
			tex_uv.Insert(idx2, new_uv);
			num_verts += 1;
		}

		public float ClosestTwoVertsArea(Vector3 pos, int original_vert, DMesh dmesh)
		{
			int v1 = -1, v2 = -1;
			for (int i = 0; i < num_verts; i++) {
				if (vert[(i + 1) % num_verts] == original_vert) {
					v1 = i;
				}
				if (vert[(i + num_verts - 1) % num_verts] == original_vert) {
					v2 = i;
				}
			}

			if (v1 > -1 && v2 > -1) {
				return Utility.AreaOfTriangle(pos, dmesh.vertex[vert[v1]], dmesh.vertex[vert[v2]]);
			} else {
				return 9999f;
			}
		}

		public float ClosestTwoVertsDot(Vector3 pos, int original_vert, DMesh dmesh)
		{
			int v1 = -1, v2 = -1;
			for (int i = 0; i < num_verts; i++) {
				if (vert[(i + 1) % num_verts] == original_vert) {
					v1 = i;
				}
				if (vert[(i + num_verts - 1) % num_verts] == original_vert) {
					v2 = i;
				}
			}

			if (v1 > -1 && v2 > -1) {
				return Vector3.Dot((dmesh.vertex[vert[v1]] - pos).Normalized(), (dmesh.vertex[vert[v2]] - pos).Normalized());
			} else {
				return 9999f;
			}
		}

		public Vector3 ClosestTwoVertsDirection(Vector3 pos, int original_vert, DMesh dmesh)
		{
			int v1 = -1, v2 = -1;
			for (int i = 0; i < num_verts; i++) {
				if (vert[(i + 1) % num_verts] == original_vert) {
					v1 = i;
				}
				if (vert[(i + num_verts - 1) % num_verts] == original_vert) {
					v2 = i;
				}
			}

			if (v1 > -1 && v2 > -1) {
				return ((dmesh.vertex[vert[v1]] - pos).Normalized() + (dmesh.vertex[vert[v2]] - pos).Normalized()).Normalized();
			} else {
				return Vector3.UnitY;
			}
		}

		// Find the closest edge (avg pos between two verts) on the selected polygon for the view/pos, returns the first of the verts in the edge
		public int GetClosestEdgeFV(GLView view, DMesh dmesh, Vector3 pos, int exclude_vert, bool update_cut_pos = false)
		{
			int closest_edge = -1;
			float closest_dist = 9999f;
			float dist;
			Vector3 edge_pos;

			for (int i = 0; i < num_verts; i++) {
				if (i != exclude_vert) {
					edge_pos = dmesh.vertex[vert[i]] + dmesh.vertex[vert[(i + 1) % num_verts]];
					edge_pos *= 0.5f;
					edge_pos = Utility.NullifyAxisForView(edge_pos, view.m_view_type);

					dist = (pos - edge_pos).Length;
					if (dist < closest_dist) {
						if (update_cut_pos) {
							Editor.POLYCUT_POS = edge_pos;
						}
						closest_dist = dist;
						closest_edge = i;
					}
				}
			}

			return closest_edge;
		}

		public bool IsPlanar(DMesh dmesh)
		{
			Vector3 first_normal = Utility.FindNormal(dmesh.vertex[vert[0]], dmesh.vertex[vert[1]], dmesh.vertex[vert[2]]);
			// For verts above 2, see if they're 0 distance from the plane of the first triangle
			for (int i = 3; i < num_verts; i++) {
				if (Math.Abs(Utility.DistanceFromPlane(dmesh.vertex[vert[i]], dmesh.vertex[vert[0]], first_normal)) > 0.0005f) {
					return false;
				}
			}

			return true;
		}

		// Create a polygon from a list of verts and the DMesh reference
		public DPoly(List<int> vrts, int tex_idx, DMesh dmesh, bool sort = false)
		{
			if (sort) {
				vrts = SortVertsForPlanar(vrts, dmesh);
			}

			num_verts = vrts.Count;
			ClearLists();
			for (int i = 0; i < num_verts; i++) {
				vert.Add(vrts[i]);
				normal.Add(Vector3.UnitY);
				tex_uv.Add(Vector2.Zero);
			}

			tex_index = tex_idx;
			flags = 0;
			marked = false;

			// Calculate the properties
			Vector3 n = CalculatePolyNormal(this, dmesh);
			for (int i = 0; i < num_verts; i++) {
				normal[i] = n;
			}
			DefaultAlignment(dmesh);
		}

		// Note: Assumes a (mostly?) convex polygon
		public List<int> SortVertsForPlanar(List<int> orig_list, DMesh dmesh)
		{
			Vector3 center = FindCenter(orig_list, dmesh);
			Vector3 normal = Utility.FindNormal(dmesh.vertex[orig_list[0]], dmesh.vertex[orig_list[1]], dmesh.vertex[orig_list[2]]);
			Vector3 v0 = (Utility.ProjectOntoPlane(dmesh.vertex[orig_list[0]], center, normal) - center).Normalized();
			Vector3 cross = Vector3.Cross(normal, v0);

			// We're basically getting polar co-ordinates for each vertex relative to the center of the face
			// and the first vertex of the face. We can then sort the vertex list in ascending order of angle.
			// (The previous system was not resilient to "spiral" vertex orders.)
			var vertAngles = new SortedDictionary<int, float>();
			foreach (int vertex in orig_list)
			{
				Vector3 v = (Utility.ProjectOntoPlane(dmesh.vertex[vertex], center, normal) - center).Normalized();
				float x = Vector3.Dot(v, v0);
				float y = Vector3.Dot(v, cross);
				float angle = (float)Math.Atan2(y, x);
				if (angle < 0)
				{
					angle += (float)(2 * Math.PI);
				}
				vertAngles[vertex] = angle;
			}

			return orig_list.OrderBy(vert => vertAngles[vert]).ToList();
		}

		public Axis FindShortestAxis(List<int> orig_list, DMesh dmesh)
		{
			Vector3 min, max;
			dmesh.GetVertBounds(orig_list, out min, out max);
			Vector3 diff = max - min;
			if (diff.X < diff.Y && diff.X < diff.Z) {
				return Axis.X;
			} else if (diff.Z < diff.Y && diff.Z < diff.X) {
				return Axis.Z;
			} else {
				return Axis.Y;
			}
		}

		public void ReSortVerts(DMesh dmesh)
		{
			List<int> sort_vrt = new List<int>();
			List<Vector2> sort_uv = new List<Vector2>();
			List<Vector3> sort_norm = new List<Vector3>();

			// Sort clockwise
			for (int i = 0; i < vert.Count; i++) {
				sort_vrt.Add(vert[i]);
			}
			sort_vrt = SortVertsForPlanar(sort_vrt, dmesh);
			
			// Get the other properties sorted
			for (int i = 0; i < sort_vrt.Count; i++) {
				int idx = FindVertByIndex(sort_vrt[i]);
				sort_uv.Add(tex_uv[idx]);
				sort_norm.Add(normal[idx]);
			}

			// Copy to the real lists
			ClearLists();
			for (int i = 0; i < sort_vrt.Count; i++) {
				vert.Add(sort_vrt[i]);
				tex_uv.Add(sort_uv[i]);
				normal.Add(sort_norm[i]);
			}
		}

		public int FindVertByIndex(int idx)
		{
			for (int i = 0; i < vert.Count; i++) {
				if (vert[i] == idx) {
					return i;
				}
			}

			return 0;
		}

		public void AddVert(int idx, Vector3 norm, Vector2 uv)
		{
			if (num_verts < MAX_VERTS) {
				vert.Add(idx);
				normal.Add(norm);
				tex_uv.Add(uv);
				num_verts += 1;
			}
		}

		public Vector3 FindCenter(List<int> v_list, DMesh dmesh)
		{
			Vector3 center = Vector3.Zero;
			for (int i = 0; i < v_list.Count; i++) {
				center += dmesh.vertex[v_list[i]];
			}
			if (v_list.Count > 0) {
				center /= v_list.Count;
			}

			return center;
		}

		public Vector3 FindCenter(DMesh dmesh)
		{
			Vector3 center = Vector3.Zero;
			for (int i = 0; i < vert.Count; i++) {
				center += dmesh.vertex[vert[i]];
			}
			if (vert.Count > 0) {
				center /= vert.Count;
			}

			return center;
		}

		public int HasVertsInListCount(List<int> vert_list)
		{
			int count = 0;
			for (int i = 0; i < vert_list.Count; i++) {
				if (vert.Contains(vert_list[i])) {
					count += 1;
				}
			}

			return count;
		}

		public bool SharesAllVerts(DPoly dp)
		{
			for (int i = 0; i < dp.num_verts; i++) {
				if (!vert.Contains(dp.vert[i])) {
					return false;
				}
			}

			return true;
		}

		public bool SharesSomeVerts(DPoly dp)
		{
			int count = 0;
			for (int i = 0; i < dp.num_verts; i++) {
				if (vert.Contains(dp.vert[i])) {
					count += 1;
				}
			}

			return (count >= 1);
		}

		public bool SharesOneVertPlusOne(DPoly dp, int vert_idx)
		{
			int count = 0;
			int v1 = -1;
			for (int i = 0; i < dp.num_verts; i++) {
				if (vert.Contains(dp.vert[i])) {
					count += 1;
					v1 = dp.vert[i];
				}
			}

			if (count == 1) {
				if (vert_idx != v1 && vert.Contains(vert_idx)) {
					return true;
				}
			}

			return false;
		}

		public int FindSharedVertNotThisOne(DPoly dp, int idx)
		{
			for (int i = 0; i < num_verts; i++) {
				if (dp.vert.Contains(vert[i]) && vert[i] != idx) {
					return vert[i];
				}
			}

			return -1;
		}

		public void SmoothSharedVerts(DPoly dp)
		{
			should_normalize = true;
			dp.should_normalize = true;
			for (int i = 0; i < num_verts; i++) {
				for (int j = 0; j < dp.num_verts; j++) {
					// Verts match
					if (vert[i] == dp.vert[j]) {
						// Add the normals, these will get normalized after all are smoothed
						normal[i] += dp.face_normal;
						dp.normal[j] += face_normal;
					}
				}
			}
		}

		public void MaybeShiftVerts(int unshared_vert)
		{
			if (vert[0] == unshared_vert || vert[2] == unshared_vert) {
				// Shift everything by 1
				vert.Add(vert[0]);
				tex_uv.Add(tex_uv[0]);
				normal.Add(normal[0]);

				vert.RemoveAt(0);
				tex_uv.RemoveAt(0);
				normal.RemoveAt(0);
			}
		}


		// TEXTURING FUNCTIONS
		public void PlanarMap(Axis axis, float scl, Vector3 offset, DMesh dmesh)
		{
			Vector3 v;
			for (int i = 0; i < num_verts; i++) {
				v = dmesh.vertex[vert[i]];
				switch (axis) {
					case Axis.X:
						tex_uv[i] = (v.Yz - offset.Yz) * scl;
						break;
					case Axis.Y:
						tex_uv[i] = (v.Xz - offset.Xz) * scl;
						break;
					case Axis.Z:
						tex_uv[i] = (v.Xy - offset.Xy) * scl;
						break;
				}
			}
		}

		public void BoxMap(float scl, Vector3 offset, DMesh dmesh)
		{
			// Choose the planar direction based on the most significant part of the normal
			Vector3 norm = CalculatePolyNormal(this, dmesh);
			if (Math.Abs(norm.X) >= Math.Abs(norm.Y) && Math.Abs(norm.X) >= Math.Abs(norm.Z)) {
				PlanarMap(Axis.X, scl, offset, dmesh);
			} else if (Math.Abs(norm.Y) >= Math.Abs(norm.X) && Math.Abs(norm.Y) >= Math.Abs(norm.Z)) {
				PlanarMap(Axis.Y, scl, offset, dmesh);
			} else {
				PlanarMap(Axis.Z, scl, offset, dmesh);
			}
		}

		// whole is the fraction of 1, so 8 = 1/8ths
		public void UVSnapToFraction(int whole)
		{
			float snap = 1f / (float)(Math.Max(1, whole));

			for (int i = 0; i < num_verts; i++) {
				tex_uv[i] = Utility.SnapValue(tex_uv[i], snap);
			}
		}

		public void ScaleUVs(float amt)
		{
			for (int i = 0; i < num_verts; i++) {
				tex_uv[i] *= amt;
			}
		}

		public void DefaultAlignment(DMesh dmesh)
		{
			// Get matrix to rotate face to align with XY plane (first rotate to be vertical, then to face XY)
			// Planar Map with vert0 as 0,0
			Vector3[] v = new Vector3[num_verts];

			// Offset all the verts by vert2 to get a better frame of reference
			for (int i = 0; i < num_verts; i++) {
				v[i] = dmesh.vertex[vert[i]] - dmesh.vertex[vert[0]];
			}
			Vector3 normal = CalculatePolyNormal(this, dmesh);

			v = GetXYAlignedPositions(v, normal);

			// Assign the UVs based on the XY positions
			for (int i = 0; i < num_verts; i++) {
				tex_uv[i] = v[i].Xy * -DMesh.PLANAR_SCL;
			}
		}

		public Vector3[] GetXYAlignedPositions(Vector3[] v, Vector3 normal)
		{
			Matrix4 rot_mat;
			if (Math.Abs(normal.Y) < 0.999f) {
				Vector3 base_uvec = FindBestTextureUVec(normal);
				Vector3 base_rvec = Vector3.Cross(base_uvec, normal);
				Matrix4 base_rot = (Matrix4.LookAt(Vector3.Zero, normal, base_uvec));
				for (int i = 0; i < v.Length; i++) {
					v[i] = Vector3.Transform(v[i], base_rot);
					v[i].X *= -1f;
				}

				return v;
			} else {
				// Just need to rotate the verts to be vertical
				rot_mat = Matrix4.CreateFromAxisAngle(Vector3.UnitX, Utility.RAD_90 * normal.Y);

				// Rotate the verts
				for (int i = 0; i < v.Length; i++) {
					v[i] = Vector3.Transform(v[i], rot_mat);
				}
			}

			return v;
		}

		public Vector3 FindBestTextureUVec(Vector3 normal)
		{
			Vector3 temp_uvec;
			Vector3 temp_rvec;
			if (normal.Y > 0.999f) {
				return Vector3.UnitZ;
			} else if (normal.Y < -0.999f) {
				return -Vector3.UnitZ;
			} else if (normal.Y < -0.001f) {
				temp_uvec = -Vector3.UnitY;
				temp_rvec = Vector3.Cross(temp_uvec, normal);
				return Vector3.Cross(normal, temp_rvec);
			} else {
				temp_uvec = Vector3.UnitY;
				temp_rvec = Vector3.Cross(temp_uvec, normal);
				return Vector3.Cross(normal, temp_rvec);
			}
		}

		// Assumes the passed in side also uses default mapping
		public void UVAlignToPoly(DPoly dp, DMesh dmesh)
		{
			// First set the default mapping
			DefaultAlignment(dmesh);

			// Find matching pair of verts
			int v1 = -1;
			int v2 = -1;
			int other_v1 = -1;
			int other_v2 = -1;
			for (int i = 0; i < num_verts; i++) {
				for (int j = 0; j < dp.num_verts; j++) {
					if (vert[i] == dp.vert[j]) {
						if (v1 < 0) {
							v1 = i;
							other_v1 = j;
						} else {
							v2 = i;
							other_v2 = j;
						}
					}
				}
			}

			// Make sure the verts are valid
			if (v2 < 0) {
				Utility.DebugLog("Tried to align UVs on two sides that don't seem to have two matching verts");
				return;
			}

			// Double check the first one in the current side
			if (v1 == 0 && v2 == dp.num_verts - 1) {
				v1 = num_verts - 1;
				v2 = 0;
				int tmp = other_v1;
				other_v1 = other_v2;
				other_v2 = tmp;
			}

			// Find the angle difference between my UV edge and the other side, and rotate the UVs to compensate
			Vector2 uv_edge = (tex_uv[v2] - tex_uv[v1]).Normalized();
			Vector2 uv_edge_other = (dp.tex_uv[other_v2] - dp.tex_uv[other_v1]).Normalized();
			Vector2 uv_first = tex_uv[v1];
			Vector2 uv_first_other = dp.tex_uv[other_v1];
			float angle = (float)Math.Atan2(uv_edge.Y, uv_edge.X) - (float)Math.Atan2(uv_edge_other.Y, uv_edge_other.X); //(float)Math.Atan2(uv_edge.Y - uv_edge_other.Y, uv_edge.X - uv_edge_other.X);

			// Offset the UVs by the other side's UV
			for (int i = 0; i < num_verts; i++) {
				tex_uv[i] = tex_uv[i] - uv_first + uv_first_other;
			}

			// Updat first UV
			uv_first = tex_uv[v1];

			for (int i = 0; i < num_verts; i++) {
				tex_uv[i] = Utility.Vector2Rotate(tex_uv[i] - uv_first, -angle) + uv_first;
			}
		}

		public Vector2 FindUVCenter()
		{
			Vector2 center = Vector2.Zero;
			for (int i = 0; i < num_verts; i++) {
				center += tex_uv[i];
			}

			return center / (float)num_verts;
		}

		public void CenterU()
		{
			Vector2 center = FindUVCenter();
			for (int i = 0; i < num_verts; i++) {
				tex_uv[i] -= Vector2.UnitX * center.X;
			}
		}

		public void CenterV()
		{
			Vector2 center = FindUVCenter();
			for (int i = 0; i < num_verts; i++) {
				tex_uv[i] -= Vector2.UnitY * center.Y;
			}
		}

		public void FitUVsToQuarter(float offset_x, float offset_y)
		{
			Vector2 center = FindUVCenter();
			// Center it
			for (int i = 0; i < num_verts; i++) {
				tex_uv[i] -= Vector2.UnitY * center.Y;
				tex_uv[i] -= Vector2.UnitX * center.X;
			}

			// Find the largest extent
			float largest_extent = 0.25f;
			for (int i = 0; i < num_verts; i++) {
				largest_extent = Math.Max(largest_extent, Math.Abs(tex_uv[i].X));
				largest_extent = Math.Max(largest_extent, Math.Abs(tex_uv[i].Y));
			}

			float scale = 0.25f / largest_extent;

			// Scale it and center back on the quarter
			for (int i = 0; i < num_verts; i++) {
				tex_uv[i] *= scale;
				Vector2 v = new Vector2(-0.25f + offset_x, -0.25f + offset_y);
				tex_uv[i] += v;
			}
		}

		public bool RemoveExtraVerts()
		{
			// Save indices of unique verts
			List<int> unique_verts = new List<int>();
			List<int> new_vert_idx = new List<int>();
			for (int i = 0; i < num_verts; i++) {
				if (!unique_verts.Contains(vert[i])) {
					unique_verts.Add(vert[i]);
					new_vert_idx.Add(i);
				}
			}

			if (unique_verts.Count == num_verts) {
				// Do nothing
				return true;
			} else if (unique_verts.Count < 3) {
				// Delete this polygon
				return false;
			} else {
				// Remake all the lists with unique verts only
				List<Vector3> unique_normals = new List<Vector3>();
				List<Vector2> unique_uvs = new List<Vector2>();

				for (int i = 0; i < new_vert_idx.Count; i++) {
					unique_normals.Add(normal[i]);
					unique_uvs.Add(tex_uv[i]);
				}

				num_verts = new_vert_idx.Count;
				ClearLists();
				for (int i = 0; i < num_verts; i++) {
					vert.Add(unique_verts[i]);
					normal.Add(unique_normals[i]);
					tex_uv.Add(unique_uvs[i]);
				}

				return true;
			}
		}
	}
}
