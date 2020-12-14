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

using OpenTK;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

// DMESH - Utility
// DMesh Editor mesh-level functions

namespace OverloadLevelEditor
{
	public partial class DMesh
	{
		public void CopyMarkedPolys(DMesh src, bool all = false, bool tag_new = false)
		{
			// Tag all the src verts in marked polys
			List<int> old_vert_index = new List<int>();
			List<int> new_vert_index = new List<int>();
			src.TagAllVerts(false);
			ClearAllMarkedPoly();

			foreach (DPoly dp in src.polygon) {
				if (dp.marked || all) {
					for (int i = 0; i < dp.num_verts; i++) {
						src.vert_info[dp.vert[i]].tag = true;
					}
				}
			}
			
			// Copy all tagged verts to me
			// - And create lists for conversion
			for (int i = 0; i < src.vertex.Count; i++) {
				if (src.vert_info[i].tag) {
					old_vert_index.Add(i);
					new_vert_index.Add(vertex.Count);
					vertex.Add(src.vertex[i]);
					vert_info.Add(new DVert());
				}
			}

			// Copy all marked polys to me (converting vert idxs in process)
			foreach (DPoly old_dp in src.polygon) {
				if (old_dp.marked || all) {
					DPoly new_dp = new DPoly(old_dp);
					polygon.Add(new_dp);
					new_dp.marked = true;
					new_dp.tag = tag_new;
					for (int i = 0; i < new_dp.num_verts; i++) {
						int old_index = new_dp.vert[i];
						old_index = FindIdxInList(old_index, old_vert_index);
						if (old_index > -1 && old_index < new_vert_index.Count) {
							new_dp.vert[i] = new_vert_index[old_index];
						} else {
							Utility.DebugPopup("This is bad, copy-paste failed", "ERROR!");
						}
					}
				}
			}
		}

		public void CopyPolygonSameVerts(DPoly src_dp)
		{
			DPoly new_dp = new DPoly(src_dp);
			polygon.Add(new_dp);
			new_dp.marked = src_dp.marked;
		}

		public int FindIdxInList(int idx, List<int> int_list)
		{
			for (int i = 0; i < int_list.Count; i++) {
				if (idx == int_list[i]) {
					return i;
				}
			}

			return -1;
		}

		public void MarkAllPolys(bool mark = true)
		{
			foreach (DPoly p in polygon) {
				p.marked = mark;
			}
		}

		public void TagAllPolys(bool tag)
		{
			foreach (DPoly p in polygon) {
				p.tag = tag;
			}
		}

		public void TagAllVerts(bool tag)
		{
			foreach (DVert v in vert_info) {
				v.tag = tag;
			}
		}

		public void CombineMatchingPolys()
		{
			List<DPoly> new_polys = new List<DPoly>();
			TagAllPolys(false);
			int count = polygon.Count;
			for (int i = 0; i < polygon.Count; i++) {
				for (int j = i + 1; j < polygon.Count; j++) {
					if (!polygon[i].tag && !polygon[j].tag) {
						if (DPoly.CanCombinePolys(polygon[i], polygon[j], this)) {
							new_polys.Add(DPoly.CombinePolys(polygon[i], polygon[j]));
							polygon[i].tag = true;
							polygon[j].tag = true;
						}
					}
				}

				if (!polygon[i].tag) {
					new_polys.Add(polygon[i]);
				}
			}

			polygon.Clear();
			foreach (DPoly p in new_polys) {
				polygon.Add(p);
			}

			editor.AddOutputText("Combined " + count.ToString() + " polygons into " + polygon.Count.ToString() + " new polygons");
		}

		public void MergeOverlappingVerts()
		{
			List<int> new_idx = new List<int>();
			for (int i = 0; i < vertex.Count; i++) {
				new_idx.Add(i);
			}

			List<Vector3> new_verts = new List<Vector3>();
			TagAllVerts(false);
			int old_count = vertex.Count;

			// Find the new index for all verts based on overlapping, and add them to the new list
			int count = 0;
			for (int i = 0; i < vertex.Count; i++) {
				if (!vert_info[i].tag) {
					new_idx[i] = count;
					new_verts.Add(vertex[i]);
					for (int j = i + 1; j < vertex.Count; j++) {
						if (!vert_info[j].tag) {
							if (Utility.AlmostOverlapping(vertex[i], vertex[j])) {
								new_idx[j] = count;
								vert_info[j].tag = true;
							}
						}
					}

					count += 1;
				}
			}

			// Set the new index for all polygons
			for (int i = 0; i < polygon.Count; i++) {
				for (int j = 0; j < polygon[i].vert.Count; j++) {
					polygon[i].vert[j] = new_idx[polygon[i].vert[j]];
				}
			}

			// Update the actual lists
			vertex.Clear();
			vert_info.Clear();
			for (int i = 0; i < count; i++) {
				vertex.Add(new_verts[i]);
				vert_info.Add(new DVert());
			}

			editor.AddOutputText("Merged " + old_count.ToString() + " verts into " + count.ToString() + " new verts");
		}

		public void RemoveUnusedVerts()
		{
			TagAllVerts(false);
			ClearAllMarkedVert();

			for (int i = 0; i < polygon.Count; i++) {
				for (int j = 0; j < polygon[i].vert.Count; j++) {
					vert_info[polygon[i].vert[j]].tag = true;
				}
			}

			int count = 0;
			for (int i = 0; i < vertex.Count; i++) {
				if (!vert_info[i].tag) {
					count += 1;
					vert_info[i].marked = true;
				}
			}

			if (count > 0) {
				editor.AddOutputText("Removed " + count + " unused verts");
			}
			DeleteMarked(EditMode.VERT);
		}

		public void RemoveExtraPolyVerts()
		{
			ClearAllMarked();

			for (int i = 0; i < polygon.Count; i++) {
				if (!polygon[i].RemoveExtraVerts()) {
					polygon[i].marked = true;
				}
			}

			DeleteMarked(EditMode.POLY);
		}

		public void DeleteMarked(EditMode em = EditMode.NUM)
		{
			em = (em == EditMode.NUM ? editor.m_edit_mode : em);

			switch (em) {
				case EditMode.POLY:
					// Create a new list, then clear and replace the current list
					List<DPoly> new_poly = new List<DPoly>();
					foreach (DPoly p in polygon) {
						if (!p.marked) {
							new_poly.Add(p);
						}
					}

					polygon.Clear();
					foreach (DPoly p in new_poly) {
						polygon.Add(p);
					}
					break;
				case EditMode.VERT:
					// Tag all the marked verts
					TagAllVerts(false);
					foreach (DVert dv in vert_info) {
						if (dv.marked) {
							dv.tag = true;
						}
					}

					// Untag any verts in polygons
					foreach (DPoly p in polygon) {
						for (int i = 0; i < p.num_verts; i++) {
							vert_info[p.vert[i]].tag = false;
						}
					}

					// Create a new list, then clear and replace the current list
					List<int> new_idx = new List<int>();
					List<DVert> new_dvert = new List<DVert>();
					List<Vector3> new_vert = new List<Vector3>();
					int count = 0;
					for (int i = 0; i < vert_info.Count; i++) {
						if (!vert_info[i].tag) {
							new_vert.Add(vertex[i]);
							new_dvert.Add(vert_info[i]);
							new_idx.Add(count);
						}
						count += 1;
					}

					// Update all the verts in polygons
					for (int i = 0; i < polygon.Count; i++) {
						for (int j = 0; j < polygon[i].num_verts; j++) {
							polygon[i].vert[j] = FindVertIndex(polygon[i].vert[j], new_idx);
						}
					}

					vertex.Clear();
					vert_info.Clear();
					for (int i = 0; i < new_dvert.Count; i++) {
						vertex.Add(new_vert[i]);
						vert_info.Add(new_dvert[i]);
					}
					break;
			}
		}

		public int FindVertIndex(int old_idx, List<int> v_list)
		{
			for (int i = 0; i < v_list.Count; i++) {
				if (v_list[i] == old_idx) {
					return i;
				}
			}

			Utility.DebugPopup("Couldn't find the vert in the list.  Bad stuff will happen.");
			return 0;
		}

		public void SnapMarkedToGrid()
		{
			TagAllVerts(false);

			switch (editor.m_edit_mode) {
				case EditMode.POLY:
					for (int i = 0; i < polygon.Count; i++) {
						if (polygon[i].marked) {
							TagPolyVerts(polygon[i]);
						}
					}
					break;
				case EditMode.VERT:
					for (int i = 0; i < vert_info.Count; i++) {
						if (vert_info[i].marked) {
							vert_info[i].tag = true;
						}
					}
					break;
			}

			for (int i = 0; i < vert_info.Count; i++) {
				if (vert_info[i].tag) {
					vertex[i] = Utility.SnapValue(vertex[i], editor.m_grid_snap);
				}
			}
		}

		public void MoveMarked(GLView view, Vector3 dir, float dist = 1f)
		{
			if (dist == 0f) return;
			Vector3 original_dir = dir;

			if (view.m_view_type == ViewType.PERSP) {
				dir = Vector3.Transform(dir, view.m_cam_mat.ExtractRotation().Inverted());
				dir.Z *= -1f;
				dir = Utility.CardinalVector(dir);
			} else {
				if (view.m_view_type == ViewType.TOP) {
					dir = Vector3.Transform(dir, Matrix4.CreateRotationX(Utility.RAD_90));
				} else if (view.m_view_type == ViewType.RIGHT) {
					dir = Vector3.Transform(dir, Matrix4.CreateRotationY(-Utility.RAD_90));
				}
			}
			dir.Normalize();
			dir *= dist;

			// Tag all the verts to move, then move them
			TagAllVerts(false);

			switch (editor.m_edit_mode) {
				case EditMode.POLY:
					for (int i = 0; i < polygon.Count; i++) {
						if (polygon[i].marked) {
							TagPolyVerts(polygon[i]);
						}
					}
					break;
				case EditMode.VERT:
					for (int i = 0; i < vert_info.Count; i++) {
						if (vert_info[i].marked) {
							vert_info[i].tag = true;
						}
					}
					break;
			}

			for (int i = 0; i < vert_info.Count; i++) {
				if (vert_info[i].tag) {
					vertex[i] += dir;
				}
			}

			editor.RefreshGeometry();
		}

		public void MoveMarkedRaw(GLView view, Vector3 dir)
		{
			if (dir == Vector3.Zero) return;

			if (view.m_view_type == ViewType.PERSP) {
				dir = Vector3.Transform(dir, view.m_cam_mat.ExtractRotation().Inverted());
				dir.Z *= -1f;
			} else {
				if (view.m_view_type == ViewType.TOP) {
					dir = Vector3.Transform(dir, Matrix4.CreateRotationX(Utility.RAD_90));
				} else if (view.m_view_type == ViewType.RIGHT) {
					dir = Vector3.Transform(dir, Matrix4.CreateRotationY(-Utility.RAD_90));
				}
			}
			
			// Tag all the verts to move, then move them
			TagAllVerts(false);

			switch (editor.m_edit_mode) {
				case EditMode.POLY:
					for (int i = 0; i < polygon.Count; i++) {
						if (polygon[i].marked) {
							TagPolyVerts(polygon[i]);
						}
					}
					break;
				case EditMode.VERT:
					for (int i = 0; i < vert_info.Count; i++) {
						if (vert_info[i].marked) {
							vert_info[i].tag = true;
						}
					}
					break;
			}

			for (int i = 0; i < vert_info.Count; i++) {
				if (vert_info[i].tag) {
					vertex[i] += dir;
				}
			}

			editor.RefreshGeometry();
		}

		public void RotateMarked(GLView view, float amt = Utility.RAD_90)
		{
			if (amt == 0f) return;
			Axis axis = Axis.X;

			switch (view.m_view_type) {
				case ViewType.FRONT:
					axis = Axis.Z;
					amt *= -1f;
					break;
				case ViewType.RIGHT:
					axis = Axis.X;
					break;
				case ViewType.TOP:
					axis = Axis.Y;
					amt *= -1f;
					break;
				case ViewType.PERSP:
					Vector3 dir = Vector3.Transform(Vector3.UnitZ, view.m_cam_mat.ExtractRotation().Inverted());
					dir = Utility.CardinalVector(dir);
					float sign;
					axis = Utility.VectorToAxis(dir, out sign);
					amt *= sign;
					break;
			}

			// Tag all the verts to rotate, then rotate them
			TagAllVerts(false);

			switch (editor.m_edit_mode) {
				case EditMode.POLY:
					for (int i = 0; i < polygon.Count; i++) {
						if (polygon[i].marked) {
							TagPolyVerts(polygon[i]);
						}
					}
					break;
				case EditMode.VERT:
					for (int i = 0; i < vert_info.Count; i++) {
						if (vert_info[i].marked) {
							vert_info[i].tag = true;
						}
					}
					break;
			}

			Vector3 pivot = GetPivotPosition();
			for (int i = 0; i < vert_info.Count; i++) {
				if (vert_info[i].tag) {
					vertex[i] = Utility.RotateAroundPivot(vertex[i], pivot, axis, amt); 
				}
			}
		}

		public void ScaleUVs(float amt)
		{
			List<DPoly> poly_list = GetMarkedPolys();
			foreach (DPoly dp in poly_list) {
				dp.ScaleUVs(amt);
			}
		}

		public void ScaleMarked(GLView view, ScaleMode scale_mode, float amt = 2f)
		{
			if (amt == 1f) return;
			Axis axis = Axis.ALL;

			// Figure which axis to scale
			if (scale_mode != ScaleMode.ALL) {
				ViewType vt = view.m_view_type;
				if (vt == ViewType.PERSP) {
					Vector3 dir = Vector3.Transform(Vector3.UnitZ, view.m_cam_mat.ExtractRotation().Inverted());
					dir = Utility.CardinalVector(dir);
					vt = Utility.VectorToViewType(dir);
				}
				switch (vt) {
					case ViewType.FRONT:
						switch (scale_mode) {
							case ScaleMode.VIEW_X: axis = Axis.X; break;
							case ScaleMode.VIEW_Y: axis = Axis.Y; break;
							case ScaleMode.VIEW_XY: axis = Axis.XY; break;
						}
						break;
					case ViewType.RIGHT:
						switch (scale_mode) {
							case ScaleMode.VIEW_X: axis = Axis.Z; break;
							case ScaleMode.VIEW_Y: axis = Axis.Y; break;
							case ScaleMode.VIEW_XY: axis = Axis.YZ; break;
						}
						break;
					case ViewType.TOP:
						switch (scale_mode) {
							case ScaleMode.VIEW_X: axis = Axis.X; break;
							case ScaleMode.VIEW_Y: axis = Axis.Z; break;
							case ScaleMode.VIEW_XY: axis = Axis.XZ; break;
						}
						break;
				}
			}

			// Tag all the verts to scale, then scale them
			TagAllVerts(false);

			switch (editor.m_edit_mode) {
				case EditMode.POLY:
					for (int i = 0; i < polygon.Count; i++) {
						if (polygon[i].marked) {
							TagPolyVerts(polygon[i]);
						}
					}
					break;
				case EditMode.VERT:
					for (int i = 0; i < vert_info.Count; i++) {
						if (vert_info[i].marked) {
							vert_info[i].tag = true;
						}
					}
					break;
			}

			Vector3 pivot = GetPivotPosition();
			for (int i = 0; i < vert_info.Count; i++) {
				if (vert_info[i].tag) {
					vertex[i] = Utility.ScaleFromPivot(vertex[i], pivot, axis, amt);
				}
			}
		}

		public Vector3 GetPivotPosition()
		{
			switch (editor.m_pivot_mode) {
				case PivotMode.ORIGIN:
					return Vector3.Zero;
				case PivotMode.ALL_MARKED:
					if (editor.m_edit_mode == EditMode.VERT) {
						List<int> vert_list = GetMarkedVerts(true);
						Vector3 avg_pos = Vector3.Zero;

						if (vert_list.Count > 0) {
							for (int i = 0; i < vert_list.Count; i++) {
								avg_pos += vertex[vert_list[i]];
							}
							avg_pos /= vert_list.Count;

							return avg_pos;
						}
					} else {
						List<DPoly> poly_list = GetMarkedPolys(true);
						Vector3 avg_pos = Vector3.Zero;

						if (poly_list.Count > 0) {
							for (int i = 0; i < poly_list.Count; i++) {
								avg_pos += poly_list[i].FindCenter(this);
							}
							avg_pos /= poly_list.Count;

							return avg_pos;
						}
					}
					break;
				case PivotMode.SEL_POLY:
					if (selected_poly > -1) {
						return polygon[selected_poly].FindCenter(this);
					}
					break;
				case PivotMode.SEL_VERT:
					if (selected_vert > -1) {
						return vertex[selected_vert];
					}
					break;
			}

			// Fall-thru case (can happen legitimately)
			return Vector3.Zero;
		}

		public void MarkVertByIndex(int idx)
		{
			if (!vert_info[idx].marked) {
				vert_info[idx].marked = true;
				num_marked_verts += 1;
			}
		}

		public void TagMarkedPolyVerts()
		{
			for (int i = 0; i < polygon.Count; i++) {
				if (polygon[i].marked) {
					TagPolyVerts(polygon[i]);
				}
			}
		}

		public void TagTaggedPolyVerts()
		{
			for (int i = 0; i < polygon.Count; i++) {
				if (polygon[i].tag) {
					TagPolyVerts(polygon[i]);
				}
			}
		}

		public void TagPolyVerts(DPoly dp)
		{
			for (int i = 0; i < dp.vert.Count; i++) {
				vert_info[dp.vert[i]].tag = true;
			}
		}

		public void ClearAllMarked()
		{
			ClearAllMarkedPoly();
			ClearAllMarkedVert();
		}

		public void ClearAllMarkedPoly()
		{
			for (int i = 0; i < polygon.Count; i++) {
				polygon[i].marked = false;
			}
			num_marked_polys = 0;
		}

		public void ClearAllMarkedVert()
		{
			for (int i = 0; i < vert_info.Count; i++) {
				vert_info[i].marked = false;
			}
			num_marked_verts = 0;
		}

		public void ToggleMarkAll(bool force_clear = false)
		{
			bool clear = force_clear;

			switch (editor.m_edit_mode) {
				case EditMode.POLY:
					for (int i = 0; i < polygon.Count; i++) {
						if (polygon[i].marked) {
							clear = true;
							break;
						} else {
							polygon[i].marked = true;
							num_marked_polys += 1;
						}
					}

					if (clear) {
						for (int i = 0; i < polygon.Count; i++) {
							polygon[i].marked = false;
						}
						num_marked_polys = 0;
					}
					break;
				case EditMode.VERT:
					for (int i = 0; i < vert_info.Count; i++) {
						if (vert_info[i].marked) {
							clear = true;
							break;
						} else {
							vert_info[i].marked = true;
							num_marked_verts += 1;
						}
					}

					if (clear) {
						for (int i = 0; i < vert_info.Count; i++) {
							vert_info[i].marked = false;
						}
						num_marked_verts = 0;
					}
					break;
			}
		}

		public void ToggleMarkSelected()
		{
			switch (editor.m_edit_mode) {
				case EditMode.POLY:
					if (selected_poly > -1 && polygon.Count > selected_poly) {
						polygon[selected_poly].marked = !polygon[selected_poly].marked;
					}
					break;
				case EditMode.VERT:
					if (selected_vert > -1 && vert_info.Count > selected_vert) {
						vert_info[selected_vert].marked = !vert_info[selected_vert].marked;
					}
					break;
			}
		}

		public void UpdateMarkedCounts()
		{
			num_marked_polys = 0;
			for (int i = 0; i < polygon.Count; i++) {
				if (polygon[i].marked) {
					num_marked_polys += 1;
				}
			}

			num_marked_verts = 0;
			for (int i = 0; i < vert_info.Count; i++) {
				if (vert_info[i].marked) {
					num_marked_verts += 1;
				}
			}
		}

		public List<Vector3> AllVertexPositions()
		{
			List<Vector3> pos_list = new List<Vector3>();
			for (int i = 0; i < vertex.Count; i++) {
				pos_list.Add(vertex[i]);
			}

			return pos_list;
		}

		public List<Vector3> AllMarkedVertexPositions()
		{
			List<Vector3> pos_list = new List<Vector3>();

			TagAllVerts(false);

			for (int i = 0; i < polygon.Count; i++) {
				if (polygon[i].marked) {
					TagPolyVerts(polygon[i]);
				}
			}
			for (int i = 0; i < vert_info.Count; i++) {
				if (vert_info[i].marked || vert_info[i].tag) {
					pos_list.Add(vertex[i]);
				}
			}

			if (pos_list.Count < 1) {
				if (selected_vert > -1 && selected_vert < vertex.Count) {
					pos_list.Add(vertex[selected_vert]);
				}
			}

			return pos_list;
		}

		public void ReverseMarkedPolys()
		{
			List<DPoly> poly_list = GetMarkedPolys();
			for (int i = 0; i < poly_list.Count; i++) {
				poly_list[i].ReverseWindingOrder();
				poly_list[i].RecalculateNormal(this);
			}
		}

		public void ReverseTaggedPolys()
		{
			List<DPoly> poly_list = GetTaggedPolys();
			for (int i = 0; i < poly_list.Count; i++) {
				poly_list[i].ReverseWindingOrder();
				poly_list[i].RecalculateNormal(this);
			}
		}

		public void RecalculateNormals()
		{
			for (int i = 0; i < polygon.Count; i++) {
				polygon[i].RecalculateNormal(this);
			}

			float angle;
			float angle_same = MathHelper.DegreesToRadians(smooth_angle_same);
			float angle_diff = MathHelper.DegreesToRadians(smooth_angle_diff);

			if (angle_same > 0.001f || angle_diff > 0.001f) {
				// Smooth anything that should be smoothed
				for (int i = 0; i < polygon.Count; i++) {
					// Find adjacent faces (higher index only)
					for (int j = i + 1; j < polygon.Count; j++) {
						// Shares two verts, so adjacent
						if (i != j && polygon[i].SharesSomeVerts(polygon[j])) {
							// Within tolerance?  Then smooth the shared verts
							angle = Vector3.CalculateAngle(polygon[i].face_normal, polygon[j].face_normal);
							// Same texture, or different
							if (polygon[i].tex_index == polygon[j].tex_index) {
								if (angle < angle_same) {
									// Smooth the two verts
									polygon[i].SmoothSharedVerts(polygon[j]);
								}
							} else {
								if (angle < angle_diff) {
									// Smooth the two verts
									polygon[i].SmoothSharedVerts(polygon[j]);
								}
							}
						}
					}
				}
			}

			// Normalize anything that got smoothed
			for (int i = 0; i < polygon.Count; i++) {
				if (polygon[i].should_normalize) {
					polygon[i].NormalizeNormals();
				}
			}
		}

		public void AlignVerts(Vector3 pos, Axis axis)
		{
			List<int> verts = GetMarkedVerts(false);
			Vector3 v;
			switch (axis) {
				case Axis.X:
					for (int i = 0; i < verts.Count; i++) {
						v = vertex[verts[i]];
						v.X = pos.X;
						vertex[verts[i]] = v;
					}
					break;
				case Axis.Y:
					for (int i = 0; i < verts.Count; i++) {
						v = vertex[verts[i]];
						v.Y = pos.Y;
						vertex[verts[i]] = v;
					}
					break;
				case Axis.Z:
					for (int i = 0; i < verts.Count; i++) {
						v = vertex[verts[i]];
						v.Z = pos.Z;
						vertex[verts[i]] = v;
					}
					break;
			}
		}

		public System.Drawing.Color GetPolyColor(DPoly dp)
		{
			int mask = dp.flags / 4;
			if (color.Count > 0) {
				switch (mask) {
					case 1:
						return color[0];
					case 2:
						return color[1];
					case 4:
						return color[2];
					case 8:
						return color[3];
				}
			}
			return System.Drawing.Color.White;
		}

	}
}