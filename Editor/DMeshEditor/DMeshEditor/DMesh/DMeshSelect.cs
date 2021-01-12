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
using System;
using System.Collections.Generic;

namespace OverloadLevelEditor
{
	// DMESH - A decal mesh object (the base piece that gets tiled)
	// - Saved in local space, facing Y up, tiles every 4 in XZ
	public partial class DMesh
	{
		public const int MAX_NEAR_OBJS = 32;
		public int[] near_obj_list = new int[MAX_NEAR_OBJS];
		public int[] near_obj_side = new int[MAX_NEAR_OBJS];
		public float[] near_obj_z = new float[MAX_NEAR_OBJS];
		public const float VERT_CYCLE_RANGE_SQ = 100f; // In PIXELS, not distance

		public bool MousePickSelect(GLView view, Vector2 mouse_pos, bool silent = false, bool ignore_z = false)
		{
			Vector2 v;
			Vector3 center_pos;
			Vector3 normal;

			float closest_sq = 1000f;
			int closest_obj = -1;
			int near_objs = 0;
			int sel_obj_idx = -1;
			int sel_side_idx = -1;

			float dist_sq;

			vert_scrn_pos = new Vector3[vertex.Count];
			for (int i = 0; i < vertex.Count; i++) {
				vert_scrn_pos[i] = WorldToScreenPos(vertex[i], view);
			}

			// Apply the matrices to the vertices, find which vert is the closest, and a list of all near verts (within a certain range)
			switch (editor.m_edit_mode) {
				case EditMode.VERT:
					sel_obj_idx = selected_vert;

					for (int i = 0; i < vertex.Count; i++) {
						if (near_objs < MAX_NEAR_OBJS) {
							// Test verts for distance from mouse_pos
							v.X = vert_scrn_pos[i].X;
							v.Y = vert_scrn_pos[i].Y;
							dist_sq = (mouse_pos - v).LengthSquared;
							if (dist_sq < closest_sq) {
								closest_obj = i;
								closest_sq = dist_sq;
							}
							if (view.m_view_type != ViewType.PERSP || vert_scrn_pos[i].Z > 0f) {
								if (dist_sq < VERT_CYCLE_RANGE_SQ) {
									near_obj_list[near_objs] = i;
									near_obj_z[near_objs] = vert_scrn_pos[i].Z;
									near_objs += 1;
								}
							}
						}
					}
					break;
				case EditMode.POLY:
					sel_obj_idx = selected_poly;
					List<DTriangle> tri_list = new List<DTriangle>();

					// Go through all the triangles of each poly, first check normal, then test for being inside
					for (int i = 0; i < polygon.Count; i++) {
						if (!IsPolyVisible(i)) {
							continue;
						}

						tri_list = polygon[i].DeconvertPoly();
						foreach (DTriangle tri in tri_list) {

							normal = Utility.FindNormal(vert_scrn_pos[tri.vert[0]], vert_scrn_pos[tri.vert[1]], vert_scrn_pos[tri.vert[2]]);
							if (normal.Z > 0f || ignore_z) {
								bool inside = Utility.PointInsideTri(mouse_pos, vert_scrn_pos[tri.vert[0]], vert_scrn_pos[tri.vert[1]], vert_scrn_pos[tri.vert[2]]);
								if (inside) {
									// Find the relative distance to the opposite edges
									float e0 = Utility.FindDistanceToEdge(mouse_pos, vert_scrn_pos[tri.vert[1]], vert_scrn_pos[tri.vert[2]]) / Utility.FindDistanceToEdge(vert_scrn_pos[tri.vert[0]], vert_scrn_pos[tri.vert[1]], vert_scrn_pos[tri.vert[2]]);
									float e1 = Utility.FindDistanceToEdge(mouse_pos, vert_scrn_pos[tri.vert[2]], vert_scrn_pos[tri.vert[0]]) / Utility.FindDistanceToEdge(vert_scrn_pos[tri.vert[1]], vert_scrn_pos[tri.vert[2]], vert_scrn_pos[tri.vert[0]]);
									float e2 = Utility.FindDistanceToEdge(mouse_pos, vert_scrn_pos[tri.vert[0]], vert_scrn_pos[tri.vert[1]]) / Utility.FindDistanceToEdge(vert_scrn_pos[tri.vert[2]], vert_scrn_pos[tri.vert[0]], vert_scrn_pos[tri.vert[1]]);

									float total = e0 + e1 + e2;

									//center_pos = (vert_scrn_pos[tri.vert[0]] + vert_scrn_pos[tri.vert[1]] + vert_scrn_pos[tri.vert[2]]) * 0.3333f;
									center_pos = vert_scrn_pos[tri.vert[0]] * e0 + vert_scrn_pos[tri.vert[1]] * e1 + vert_scrn_pos[tri.vert[2]] * e2;

                           if (view.m_view_type != ViewType.PERSP || center_pos.Z > 0f) {
										near_obj_list[near_objs] = i;
										near_obj_z[near_objs] = center_pos.Z;
										near_objs += 1;
									}
								}
							}
						}
					}
					break;
			}

			if (near_objs < 2 && closest_obj > -1 && editor.m_edit_mode == EditMode.VERT) {
				sel_obj_idx = closest_obj;
			} else if (near_objs >= 1) {
				// Sort the objects by Z order (have to copy to arrays of the correct size first)
				int[] sort_list = new int[near_objs];
				int[] sort_side = new int[near_objs];
				float[] sort_z = new float[near_objs];

				for (int i = 0; i < near_objs; i++) {
					sort_list[i] = near_obj_list[i];
					sort_side[i] = near_obj_side[i];
					sort_z[i] = near_obj_z[i];
				}

				Array.Sort(sort_z, sort_list);
				for (int i = 0; i < near_objs; i++) {
					sort_z[i] = near_obj_z[i];
				}
				Array.Sort(sort_z, sort_side);

				// See if the current selected object is in the array
				int sel_index = -1;
				for (int i = 0; i < near_objs; i++) {
					// Extra testing for segment/sides, but not verts/entities
					if (sort_list[i] == sel_obj_idx) {
						sel_index = i;
						break;
					}
				}

				// If it's in the list, and not the last in the list
				if (sel_index > -1 && sel_index < near_objs - 1) {
					// Find the next in the list
					sel_obj_idx = sort_list[sel_index + 1];
					sel_side_idx = sort_side[sel_index + 1];
				} else {
					// Choose the first in the list
					sel_obj_idx = sort_list[0];
					sel_side_idx = sort_side[0];
				}
			}

			// Select the actual object
			if (sel_obj_idx > -1 && near_objs > 0) {
				switch (editor.m_edit_mode) {
					case EditMode.VERT:
						selected_vert = sel_obj_idx;
						break;
					case EditMode.POLY:
						SelectPolyIndex(sel_obj_idx);
						break;
				}

				if (!silent) {
					editor.RefreshGeometry();
				}

				return true;
			} else {
				return false;
			}
		}

		public void SelectPolyIndex(int idx)
		{
			selected_poly = idx;
			if (polygon[idx].tex_index > 0) {
				editor.uv_editor.SetTexture(editor.tm_decal.m_bitmap[m_tex_gl_id[polygon[idx].tex_index] - 1], true); // Not entirely sure how the m_tex_gl_ids are off by 1, but they are
			}
			editor.uv_editor.Refresh();
		}

		public void CycleSelectedPoly(bool reverse)
		{
			if (polygon.Count > 0) {
				if (reverse) {
					SelectPolyIndex((selected_poly + polygon.Count - 1) % polygon.Count);
				} else {
					SelectPolyIndex((selected_poly + 1) % polygon.Count);
				}
			}
		}

		public void CycleSelectedVert(bool reverse)
		{
			if (vertex.Count > 0) {
				if (reverse) {
					selected_vert = (selected_vert + vertex.Count - 1) % vertex.Count;
				} else {
					selected_vert = (selected_vert + 1) % vertex.Count;
				}
			}
		}

		public void CycleSelectedPolyWithVert(bool reverse)
		{
			// Get a list of the polygons containing selected vert
			// Choose the next/prev one in list
		}

		public void CycleSelectedVertInPoly(bool reverse)
		{
			// Find the list verts in the selected poly
			// Choose the next/prev one
		}

		public void MouseDragMark(GLView view, Vector2 mouse_pos_down, Vector2 mouse_pos, bool add)
		{
			int count;

			vert_scrn_pos = new Vector3[vertex.Count];
			for (int i = 0; i < vertex.Count; i++) {
				vert_scrn_pos[i] = WorldToScreenPos(vertex[i], view);
			}

			// Apply the matrices to the vertices, find which vert is the closest, and a list of all near verts (within a certain range)
			switch (editor.m_edit_mode) {
				case EditMode.VERT:
					for (int i = 0; i < vertex.Count; i++) {
						if (!add) {
							vert_info[i].marked = false;
						}
						if (Utility.PointInsideAABB(vert_scrn_pos[i].Xy, mouse_pos_down, mouse_pos)) {
							vert_info[i].marked = true;
						}
					}
					break;
				case EditMode.POLY:
					for (int i = 0; i < polygon.Count; i++) {
						if (!add) {
							polygon[i].marked = false;
						}
						if (!IsPolyVisible(i)) {
							continue;
						}

						if (editor.m_drag_mode == DragMode.ALL) {
							count = 0;
							for (int j = 0; j < polygon[i].num_verts; j++) {
								if (Utility.PointInsideAABB(vert_scrn_pos[polygon[i].vert[j]].Xy, mouse_pos_down, mouse_pos)) {
									count += 1;
								}
							}
							if (count == polygon[i].num_verts) {
								polygon[i].marked = true;
							}
						} else if (editor.m_drag_mode == DragMode.ANY) {
							for (int j = 0; j < polygon[i].num_verts; j++) {
								if (Utility.PointInsideAABB(vert_scrn_pos[polygon[i].vert[j]].Xy, mouse_pos_down, mouse_pos)) {
									polygon[i].marked = true;
									break;
								}
							}
						}
					}
					break;
			}

			UpdateMarkedCounts();
			editor.RefreshGeometry();
		}

		public bool IsPolyVisible(int idx)
		{
			if (polygon[idx].flags == (int)FaceFlags.NO_COLLIDE) {
				if (editor.m_vis_type == VisibilityType.NO_RENDER || editor.m_vis_type == VisibilityType.NORMAL_ONLY) {
					return false;
				}
			} else if (polygon[idx].flags == (int)FaceFlags.NO_RENDER) {
				if (editor.m_vis_type == VisibilityType.NO_COLLIDE || editor.m_vis_type == VisibilityType.NORMAL_ONLY) {
					return false;
				}
			}
			return true;
      }

		public Vector3 WorldToScreenPos(Vector3 obj_pos, GLView view)
		{
			if (view.m_view_type == ViewType.TOP) {
				obj_pos = Vector3.Transform(obj_pos, Matrix4.CreateRotationX(-Utility.RAD_90));
			} else if (view.m_view_type == ViewType.RIGHT) {
				obj_pos = Vector3.Transform(obj_pos, Matrix4.CreateRotationY(Utility.RAD_90));
			} else if (view.m_view_type == ViewType.PERSP) {
				//obj_pos.Z *= -1f;
			}
			obj_pos.Z *= -1f;

			return Utility.WorldToScreen(obj_pos, view.m_cam_mat, view.m_persp_mat, view.m_control_sz.X, view.m_control_sz.Y, (view.m_view_type == ViewType.PERSP));
		}

		public List<DPoly> GetMarkedPolys(bool or_selected = true)
		{
			List<DPoly> poly_list = new List<DPoly>();
			for (int i = 0; i < polygon.Count; i++) {
				if (polygon[i].marked) {
					poly_list.Add(polygon[i]);
				}
			}

			if (poly_list.Count < 1 && or_selected) {
				if (selected_poly > -1 && selected_poly < polygon.Count) {
					poly_list.Add(polygon[selected_poly]);
				}
			}

			return poly_list;
		}

		public List<DPoly> GetTaggedPolys()
		{
			List<DPoly> poly_list = new List<DPoly>();
			for (int i = 0; i < polygon.Count; i++) {
				if (polygon[i].tag) {
					poly_list.Add(polygon[i]);
				}
			}

			return poly_list;
		}

		// NOTE: Returns index #s, not Vector3 or DVerts
		public List<int> GetMarkedVerts(bool or_selected = true)
		{
			List<int> vert_list = new List<int>();
			for (int i = 0; i < vert_info.Count; i++) {
				if (vert_info[i].marked) {
					vert_list.Add(i);
				}
			}

			if (vert_list.Count < 1 && or_selected) {
				if (selected_vert > -1 && selected_vert < vertex.Count) {
					vert_list.Add(selected_poly);
				}
			}

			return vert_list;
		}

		public void TagCoplanarConnectedPolys(DPoly p, float angle_tol, bool recursive = false)
		{
			float angle;
			for (int i = 0; i < polygon.Count; i++) {
				if (!polygon[i].tag) {
					if (DPoly.HasTwoOrMoreSharedVerts(polygon[i], p)) {
						angle = Vector3.CalculateAngle(DPoly.CalculatePolyNormal(polygon[i], this), DPoly.CalculatePolyNormal(p, this));
						if (angle <= angle_tol) {
							polygon[i].tag = true;
							if (recursive) {
								TagCoplanarConnectedPolys(polygon[i], angle_tol, true);
							}
						}
					}
				}
			}
		}

		public void MarkTaggedPolys()
		{
			for (int i = 0; i < polygon.Count; i++) {
				if (polygon[i].tag && !polygon[i].marked) {
					polygon[i].marked = true;
					num_marked_polys += 1;
				}
			}
		}

		public void MarkDuplicatePolys()
		{
			TagAllPolys(false);
			ClearAllMarkedPoly();

			for (int i = 0; i < polygon.Count; i++) {
				for (int j = i + 1; j < polygon.Count; j++) {
					if (DPoly.HasThreeOrMoreCloseVerts(polygon[i], polygon[j], this)) {
						polygon[i].marked = true;
					}
				}
			}
		}

		public Vector3 AverageMarkedPolyVertPosition()
		{
			TagAllVerts(false);
			foreach (DPoly dp in GetMarkedPolys(false)) {
				TagPolyVerts(dp);
			}

			Vector3 avg_pos = Vector3.Zero;
			for (int i = 0; i < vert_info.Count; i++) {
				if (vert_info[i].tag) {
					avg_pos += vertex[i];
				}
			}

			if (vert_info.Count > 0) {
				return avg_pos / vert_info.Count;
			} else {
				return avg_pos;
			}
		}
	}
}