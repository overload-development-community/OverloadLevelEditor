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
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

// LEVEL - Select and Modify
// Functions that select or modify the level (outside of adding or deleting)
// (These are editor-only, not for game)
// (Some functions could possibly be moved into other Level sub-files)

#pragma warning disable 1690 // warning CS1690: Accessing a member on 'OverloadLevelEditor.GLView.control_sz' may cause a runtime exception because it is a field of a marshal-by-reference class

namespace OverloadLevelEditor
{
	public partial class Level
	{
		public Vector3[] vert_scrn_pos = new Vector3[MAX_VERTICES];
		public const int MAX_NEAR_OBJS = 32;
		public int[] near_obj_list = new int[MAX_NEAR_OBJS];
		public int[] near_obj_side = new int[MAX_NEAR_OBJS];
		public float[] near_obj_z = new float[MAX_NEAR_OBJS];

		public const float VERT_CYCLE_RANGE_SQ = 100f; // In PIXELS, not distance

		// Shell for mouse pick to do some shennanigans when in entity mode
		public void MousePickSelect(GLView view, Vector2 mouse_pos)
		{
			if (MousePickSelectObject(view, mouse_pos)) {
				// We picked something, done!
			} else {
				// If we are entity mode, pick a segment/side if we didn't pick and entity
				if (editor.ActiveEditMode == EditMode.ENTITY) {
					editor.ActiveEditMode = EditMode.SEGMENT;
					MousePickSelectObject(view, mouse_pos, true);
					editor.ActiveEditMode = EditMode.ENTITY;
					editor.RefreshGeometry();
				}
			}
		}

		public bool MousePickSelectObject(GLView view, Vector2 mouse_pos, bool silent = false)
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

			// We don't need this for entities
			if (editor.ActiveEditMode != EditMode.ENTITY) {
				for (int i = 0; i < MAX_VERTICES; i++) {
					if (vertex[i].alive) {
						vert_scrn_pos[i] = WorldToScreenPos(vertex[i].position, view);
					}
				}
			}

			// Apply the matrices to the vertices, find which vert is the closest, and a list of all near verts (within a certain range)
			switch (editor.ActiveEditMode) {
				case EditMode.VERTEX:
					sel_obj_idx = selected_vertex;

					for (int i = 0; i < MAX_VERTICES; i++) {
						if (near_objs < MAX_NEAR_OBJS && vertex[i].alive) {
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
				case EditMode.SEGMENT:
				case EditMode.SIDE:
					sel_obj_idx = selected_segment;
					sel_side_idx = selected_side;

					for (int i = 0; i < MAX_SEGMENTS; i++) {
						if (near_objs < MAX_NEAR_OBJS && segment[i].Visible) {
							for (int j = 0; j < Segment.NUM_SIDES; j++) {
								if (segment[i].neighbor[j] < 0 || editor.ActiveSideSelect == SideSelect.ALL || editor.ActiveSideSelect == SideSelect.FRONT) {
									// Test both triangles for mouse_pos inside
									normal = Utility.FindNormal(vert_scrn_pos[segment[i].side[j].vert[0]], vert_scrn_pos[segment[i].side[j].vert[1]], vert_scrn_pos[segment[i].side[j].vert[2]]);
									if (normal.Z > 0.0f || editor.ActiveSideSelect == SideSelect.SOLID || editor.ActiveSideSelect == SideSelect.ALL) {
										bool inside_quad = Utility.PointInsideTri(mouse_pos, vert_scrn_pos[segment[i].side[j].vert[0]], vert_scrn_pos[segment[i].side[j].vert[1]], vert_scrn_pos[segment[i].side[j].vert[2]]);
										inside_quad = inside_quad || Utility.PointInsideTri(mouse_pos, vert_scrn_pos[segment[i].side[j].vert[0]], vert_scrn_pos[segment[i].side[j].vert[2]], vert_scrn_pos[segment[i].side[j].vert[3]]);

										if (inside_quad) {
											center_pos = (vert_scrn_pos[segment[i].side[j].vert[0]] + vert_scrn_pos[segment[i].side[j].vert[1]]
												+ vert_scrn_pos[segment[i].side[j].vert[2]] + vert_scrn_pos[segment[i].side[j].vert[3]]) * 0.25f;

											// Make sure we don't select stuff behind the camera
											if (view.m_view_type != ViewType.PERSP || center_pos.Z > 0f) { // Get lots of false positives below 4 or so (need to figure out why some day)
												near_obj_list[near_objs] = i;
												near_obj_side[near_objs] = j;
												near_obj_z[near_objs] = center_pos.Z;
												near_objs += 1;
											}
										}
									}
								}
							}
						}
					}
					break;
				case EditMode.ENTITY:
					sel_obj_idx = selected_entity;

					for (int i = 0; i < MAX_ENTITIES; i++) {
						if (near_objs < MAX_NEAR_OBJS && entity[i].alive) {
							// Test mouse in bounding box
							if (MouseInBoundingBox(mouse_pos, view, entity[i].position, entity[i].m_rotation, Utility.GetEntitySize(entity[i]))) {
								center_pos = WorldToScreenPos(entity[i].position, view);
								if (view.m_view_type != ViewType.PERSP || center_pos.Z > 2f) {
									near_obj_list[near_objs] = i;
									near_obj_z[near_objs] = center_pos.Z;
									near_objs += 1;
								}
							}
						}
					}
					break;
			}

			if (near_objs < 2 && closest_obj > -1 && editor.ActiveEditMode == EditMode.VERTEX) {
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
					if (sort_list[i] == sel_obj_idx && (sort_side[i] == sel_side_idx || editor.ActiveEditMode == EditMode.VERTEX || editor.ActiveEditMode == EditMode.ENTITY)) {
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

			if (sel_obj_idx > -1 && near_objs > 0) {
				switch (editor.ActiveEditMode) {
					case EditMode.VERTEX:
						selected_vertex = sel_obj_idx;
						break;
					case EditMode.ENTITY:
						selected_entity = sel_obj_idx;
						break;
					case EditMode.SEGMENT:
						selected_segment = sel_obj_idx;
						selected_side = sel_side_idx;
						break;
					case EditMode.SIDE:
						selected_segment = sel_obj_idx;
						selected_side = sel_side_idx;
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

		//Unmark any verts that are only used by non-visible segments
		private void UnmarkHiddenVerts()
		{
			//Clear tags
			foreach (Vertex vert in EnumerateAliveVertices()) {
				vert.m_tag = false;
			}

			//Tag each vert that's used by a visible segment
			foreach (Segment seg in EnumerateVisibleSegments()) {
				for (int vn = 0; vn < seg.vert.Length; vn++) {
					vertex[seg.vert[vn]].m_tag = true;
				}
			}

			//Unmark any verts not tagged
			foreach (Vertex vert in EnumerateMarkedVertices()) {
				if (!vert.m_tag) {
					vert.marked = false;
				}
			}
		}

		public void MouseDragMark(GLView view, Vector2 mouse_pos_down, Vector2 mouse_pos, bool add)
		{
			int count;

			if (editor.ActiveEditMode != EditMode.ENTITY) {
				for (int i = 0; i < MAX_VERTICES; i++) {
					if (vertex[i].alive) {
						vert_scrn_pos[i] = WorldToScreenPos(vertex[i].position, view);
					}
				}
			} else {
				// Use this for entity positions instead of vert positions
				for (int i = 0; i < MAX_ENTITIES; i++) {
					if (entity[i].alive) {
						vert_scrn_pos[i] = WorldToScreenPos(entity[i].position, view);
					}
				}
			}

			switch (editor.ActiveEditMode) {
				case EditMode.ENTITY:
					for (int i = 0; i < MAX_ENTITIES; i++) {
						if (entity[i].Visible) {
							if (!add) {
								entity[i].marked = false;
							}
							if (Utility.PointInsideAABB(vert_scrn_pos[i].Xy, mouse_pos_down, mouse_pos)) {
								entity[i].marked = true;
							}
						}
					}
					break;
				case EditMode.VERTEX:
					for (int i = 0; i < MAX_VERTICES; i++) {
						if (vertex[i].alive) {
							if (!add) {
								vertex[i].marked = false;
							}
							if (Utility.PointInsideAABB(vert_scrn_pos[i].Xy, mouse_pos_down, mouse_pos)) {
								vertex[i].marked = true;
							}
						}
					}
					UnmarkHiddenVerts();
					break;
				case EditMode.SEGMENT:
					for (int i = 0; i < MAX_SEGMENTS; i++) {
						if (segment[i].Visible) {
							if (!add) {
								segment[i].marked = false;
							}
							if (editor.ActiveDragMode == DragMode.ALL) {
								count = 0;
								for (int j = 0; j < (int)Segment.NUM_VERTS; j++) {
									if (Utility.PointInsideAABB(vert_scrn_pos[segment[i].vert[j]].Xy, mouse_pos_down, mouse_pos)) {
										count += 1;
									}
								}
								if (count == (int)Segment.NUM_VERTS) {
									segment[i].marked = true;
								}
							} else if (editor.ActiveDragMode == DragMode.ANY) {
								for (int j = 0; j < (int)Segment.NUM_VERTS; j++) {
									if (Utility.PointInsideAABB(vert_scrn_pos[segment[i].vert[j]].Xy, mouse_pos_down, mouse_pos)) {
										segment[i].marked = true;
										break;
									}
								}
							}
						}
					}
					break;
				case EditMode.SIDE:
					for (int i = 0; i < MAX_SEGMENTS; i++) {
						if (segment[i].Visible) {
							for (int j = 0; j < (int)Segment.NUM_SIDES; j++) {
								if (!add) {
									segment[i].side[j].marked = false;
								}

								if (editor.ActiveDragMode == DragMode.ALL) {
									count = 0;
									for (int k = 0; k < Side.NUM_VERTS; k++) {
										if (Utility.PointInsideAABB(vert_scrn_pos[segment[i].side[j].vert[k]].Xy, mouse_pos_down, mouse_pos)) {
											count += 1;
										}
									}
									if (count == (int)Side.NUM_VERTS) {
										segment[i].side[j].marked = true;
									}
								} else if (editor.ActiveDragMode == DragMode.ANY) {
									for (int k = 0; k < Side.NUM_VERTS; k++) {
										if (Utility.PointInsideAABB(vert_scrn_pos[segment[i].side[j].vert[k]].Xy, mouse_pos_down, mouse_pos)) {
											segment[i].side[j].marked = true;
											break;
										}
									}
								}
							}

						}
					}
					break;
			}

			editor.RefreshGeometry();
		}

		public bool MouseInBoundingBox(Vector2 mouse_pos, GLView view, Vector3 pos, Matrix4 rot, Vector3 bbox_size)
		{
			// Create the bounding box verts, in screen space
			Vector3[] v = new Vector3[8];
			for (int i = 0; i < 8; i++) {
				// Base bounding box size
				v[i] = GLView.m_box_verts[i] * bbox_size;

				// Rotated
				v[i] = Vector3.Transform(v[i], rot);

				// Center position of bounding box
				v[i] = v[i] + pos;

				// Transform to screen space
				v[i] = WorldToScreenPos(v[i], view);
			}

			// Test all 6 sides of the box (2 triangles each)
			Vector3[] side_verts = new Vector3[4];
			for (int i = 0; i < 6; i++) {
				side_verts = Utility.SideVertsFromSegVerts(v, i);
				bool inside_quad = Utility.PointInsideTri(mouse_pos, side_verts[0], side_verts[1], side_verts[2]);
				inside_quad = inside_quad || Utility.PointInsideTri(mouse_pos, side_verts[0], side_verts[2], side_verts[3]);

				if (inside_quad) {
					return true;
				}
			}

			return false;
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

		public void DoMark(EditMode edit_mode, OperationMode op, IEnumerable<int> indices)
		{
			switch (edit_mode) {
				case EditMode.ENTITY:
					{
						foreach (int index in indices) {
							if (index < 0) {
								continue;
							}

							bool new_value = false;
							if (this.entity[index].alive) {
								switch (op) {
									case OperationMode.TOGGLE:
										new_value = !this.entity[index].marked;
										break;
									case OperationMode.ADD:
										new_value = true;
										break;
									case OperationMode.REMOVE:
										new_value = false;
										break;
								}
							}

							this.entity[index].marked = new_value;
						}
					}
					break;

				case EditMode.VERTEX:
					{
						foreach (int index in indices) {
							if (index < 0) {
								continue;
							}

							bool new_value = false;
							if (this.vertex[index].alive) {
								switch (op) {
									case OperationMode.TOGGLE:
										new_value = !this.vertex[index].marked;
										break;
									case OperationMode.ADD:
										new_value = true;
										break;
									case OperationMode.REMOVE:
										new_value = false;
										break;
								}
							}

							this.vertex[index].marked = new_value;
						}
					}
					break;

				case EditMode.SEGMENT:
					{
						foreach (int index in indices) {
							if (index < 0) {
								continue;
							}

							bool new_value = false;
							if (this.segment[index].Alive) {
								switch (op) {
									case OperationMode.TOGGLE:
										new_value = !this.segment[index].marked;
										break;
									case OperationMode.ADD:
										new_value = true;
										break;
									case OperationMode.REMOVE:
										new_value = false;
										break;
								}
							}

							this.segment[index].marked = new_value;
						}
					}
					break;

				case EditMode.SIDE:

					int segment_index = this.selected_segment;

					if (segment_index >= 0) {

						foreach (int index in indices) {
							if (index < 0) {
								continue;
							}

							bool new_value = false;
							if (this.segment[segment_index].Alive) {
								switch (op) {
									case OperationMode.TOGGLE:
										new_value = !this.segment[segment_index].side[index].marked;
										break;
									case OperationMode.ADD:
										new_value = true;
										break;
									case OperationMode.REMOVE:
										new_value = false;
										break;
								}
							}

							this.segment[segment_index].side[index].marked = new_value;
						}
					}
					break;
			}
		}

		int[] GetActiveSelectedIndicesForEditMode()
		{
			switch (editor.ActiveEditMode) {
				case EditMode.ENTITY:
					return new int[] { this.selected_entity };
				case EditMode.VERTEX:
					return new int[] { this.selected_vertex };
				case EditMode.SEGMENT:
					return new int[] { this.selected_segment };
				case EditMode.SIDE:
					return new int[] { this.selected_side };
			}

			return new int[0];
		}

		public void ToggleMarkSelected()
		{
			DoMark(editor.ActiveEditMode, OperationMode.TOGGLE, GetActiveSelectedIndicesForEditMode());
		}

		public void ForceMarkSelected(bool sel = true)
		{
			DoMark(editor.ActiveEditMode, sel ? OperationMode.ADD : OperationMode.REMOVE, GetActiveSelectedIndicesForEditMode());
		}

		public void ClearAllMarked()
		{
			EditMode em = editor.ActiveEditMode;
			try {
				editor.SetEditModeSilent(EditMode.VERTEX, false);
				ToggleMarkAll(true);
				editor.SetEditModeSilent(EditMode.SEGMENT, false);
				ToggleMarkAll(true);
				editor.SetEditModeSilent(EditMode.SIDE, false);
				ToggleMarkAll(true);
				editor.SetEditModeSilent(EditMode.ENTITY, false);
				ToggleMarkAll(true);
			} finally {
				editor.SetEditModeSilent(em, false);
			}
		}

		public void ToggleMarkAll(bool force_clear = false)
		{
			bool clear = force_clear;

			switch (editor.ActiveEditMode) {
				case EditMode.ENTITY:
					if (!clear) {
						for (int i = 0; i < MAX_ENTITIES; i++) {
							if (entity[i].Visible) {
								if (entity[i].marked) {
									clear = true;
									break;
								} else {
									entity[i].marked = true;
								}
							}
						}
					}

					if (clear) {
						for (int i = 0; i < MAX_ENTITIES; i++) {
							if (entity[i].alive) {
								entity[i].marked = false;
							}
						}
					}
					break;
				case EditMode.VERTEX:
					if (!clear) {
						for (int i = 0; i < MAX_VERTICES; i++) {
							if (vertex[i].alive) {
								if (vertex[i].marked) {
									clear = true;
									break;
								} else {
									vertex[i].marked = true;
								}
							}
						}
					}

					if (clear) {
						for (int i = 0; i < MAX_VERTICES; i++) {
							if (vertex[i].alive) {
								vertex[i].marked = false;
							}
						}
					} else {
						UnmarkHiddenVerts();
					}
					break;
				case EditMode.SEGMENT:
					if (!clear) {
						for (int i = 0; i < MAX_SEGMENTS; i++) {
							if (segment[i].Alive) {
								if (segment[i].marked) {
									clear = true;
									break;
								} else {
									if (segment[i].Visible) {
										segment[i].marked = true;
									}
								}
							}
						}
					}

					if (clear) {
						for (int i = 0; i < MAX_SEGMENTS; i++) {
							if (segment[i].Alive) {
								segment[i].marked = false;
							}
						}
					}
					break;
				case EditMode.SIDE:
					if (!clear) {
						for (int i = 0; i < MAX_SEGMENTS; i++) {
							if (segment[i].Alive) {
								for (int j = 0; j < Segment.NUM_SIDES; j++) {
									if (segment[i].side[j].marked) {
										clear = true;
										break;
									} else {
										segment[i].side[j].marked = true;
									}
								}
							}
						}
					}

					if (clear) {
						for (int i = 0; i < MAX_SEGMENTS; i++) {
							if (segment[i].Alive) {
								for (int j = 0; j < Segment.NUM_SIDES; j++) {
									segment[i].side[j].marked = false;
								}
							}
						}
					}
					break;
			}
		}

		public Vector3 GetPivotPosition()
		{
			return GetPivotPosition(editor.ActivePivotMode);
		}

		public Vector3 GetPivotPosition(PivotMode pivot_mode)
		{
			switch (pivot_mode) {

				case PivotMode.ORIGIN:
					return Vector3.Zero;

				case PivotMode.LOCAL:
					switch (editor.ActiveEditMode) {
						case EditMode.ENTITY: return GetPivotPosition(PivotMode.SELECTED_ENTITY);
						case EditMode.SEGMENT: return GetPivotPosition(PivotMode.SELECTED_SEG);
						case EditMode.SIDE: return GetPivotPosition(PivotMode.SELECTED_SIDE);
						case EditMode.VERTEX: return GetPivotPosition(PivotMode.SELECTED_VERT);
						default:
							System.Diagnostics.Debug.Assert(false);
							break;
					}
					break;

				case PivotMode.MARKED:
					Vector3 sum = Vector3.Zero;
					int count = 0;
					if (editor.ActiveEditMode == EditMode.ENTITY) {
						foreach (Entity entity in EnumerateMarkedEntities()) {
							sum += entity.position;
							count++;
						}
					} else {
						TagMarkedElementVertices();
						foreach (Vertex vert in EnumerateTaggedVertices()) {
							sum += vert.position;
							count++;
						}
					}
					if (count > 0) {
						return sum / count;
					}
					break;

				case PivotMode.SELECTED_SEG:
					return GetSelectedSegmentPos();

				case PivotMode.SELECTED_SIDE:
					return GetSelectedSidePos();

				case PivotMode.SELECTED_VERT:
					if (selected_vertex > -1) {
						return vertex[selected_vertex].position;
					}
					break;

				case PivotMode.SELECTED_ENTITY:
					if (GetSelectedEntity() != null) {
						return GetSelectedEntity().position;
					}
					break;

				default:
					System.Diagnostics.Debug.Assert(false);
					break;
			}

			// Fall-thru case (can happen legitimately)
			return Vector3.Zero;
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

			Vector3 pivot = GetPivotPosition();

			if (editor.ActiveEditMode == EditMode.ENTITY) {

				foreach (Entity entity in EnumerateMarkedEntities()) {
					entity.SetPosition(Utility.ScaleFromPivot(entity.position, pivot, axis, amt));
				}

			} else {
				// Tag all the verts to scale, then scale them
				TagMarkedElementVertices();

				foreach (Vertex vert in EnumerateTaggedVertices()) {
					vert.position = Utility.ScaleFromPivot(vert.position, pivot, axis, amt);
				}
			}
		}

		public void RotateMarkedEntities(Axis axis, float amt)
		{
			if (amt == 0f) return;

			//System.Diagnostics.Debug.Assert(editor.m_edit_mode == EditMode.ENTITY);

			if (editor.ActivePivotMode == PivotMode.LOCAL) {
				foreach (Entity entity in EnumerateMarkedEntities()) { 
					entity.RotateLocal(axis, amt);
				}
			} else {
				Vector3 center = GetPivotPosition();
				foreach (Entity entity in EnumerateMarkedEntities()) {
					entity.RotateAroundPosition(center, axis, amt);
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

			if (editor.ActiveEditMode == EditMode.ENTITY) {

				RotateMarkedEntities(axis, amt);

			} else {
				Vector3 pivot = GetPivotPosition();

				// Tag all the verts to rotate, then rotate them
				TagMarkedElementVertices();

				foreach (Vertex vert in EnumerateTaggedVertices()) {
					vert.position = Utility.RotateAroundPivot(vert.position, pivot, axis, amt);
				}
			}
		}

		public void MoveMarked(GLView view, Vector3 dir, float dist = 1.0f)
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

			if (editor.ActiveEditMode == EditMode.ENTITY) {
				for (int i = 0; i < MAX_ENTITIES; i++) {
					if (entity[i].marked) {
						if (editor.ActivePivotMode == PivotMode.LOCAL) {
							entity[i].Move(original_dir, editor.ActivePivotMode);
						} else {
							entity[i].Move(dir, editor.ActivePivotMode);
						}
					}
				}
			} else {
				TagMarkedElementVertices();

				for (int i = 0; i < MAX_VERTICES; i++) {
					if (vertex[i].m_tag) {
						vertex[i].position += dir;
					}
				}

				if (editor.ActiveEditMode == EditMode.SEGMENT) {
					//Move all the entities int the marked segments
					foreach (Entity entity in EnumerateAliveEntities()) {
						if ((entity.m_segnum != -1) && segment[entity.m_segnum].marked) {
							entity.SetPosition(entity.position + dir, entity.m_segnum);
						}
					}
				}
			}

			editor.RefreshGeometry();
			editor.Refresh();
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
			TagMarkedElementVertices();

			foreach (Vertex vert in EnumerateTaggedVertices()) {
				vert.position += dir;
			}

			editor.RefreshGeometry();
		}

		public void SnapMarkedToGrid()
		{
			if (editor.ActiveEditMode == EditMode.ENTITY) {
				for (int i = 0; i < MAX_ENTITIES; i++) {
					if (entity[i].marked) {
						entity[i].SetPosition(Utility.SnapValue(entity[i].position, editor.CurrGridSnap));
					}
				}
			} else {
				TagMarkedElementVertices();

				for (int i = 0; i < MAX_VERTICES; i++) {
					if (vertex[i].m_tag) {
						vertex[i].SnapToGrid(editor.CurrGridSnap);
					}
				}
			}

			editor.RefreshGeometry();
			editor.Refresh();
		}

		public void UnTagAllVertices()
		{
			for (int i = 0; i < MAX_VERTICES; i++) {
				vertex[i].m_tag = false;
			}
		}

		public void TagAllVertices()
		{
			for (int i = 0; i < MAX_VERTICES; i++) {
				vertex[i].m_tag = true;
			}
		}

		public void TagMarkedElementVertices()
		{
			UnTagAllVertices();

			switch (editor.ActiveEditMode) {
				case EditMode.VERTEX:
					for (int i = 0; i < MAX_VERTICES; i++) {
						if (vertex[i].alive && vertex[i].marked) {
							vertex[i].m_tag = true;
						}
					}
					break;
				case EditMode.SEGMENT:
					for (int i = 0; i < MAX_SEGMENTS; i++) {
						if (segment[i].Alive && segment[i].marked) {
							for (int j = 0; j < Segment.NUM_VERTS; j++) {
								vertex[segment[i].vert[j]].m_tag = true;
							}
						}
					}
					break;
				case EditMode.SIDE:
					for (int i = 0; i < MAX_SEGMENTS; i++) {
						if (segment[i].Alive) {
							for (int j = 0; j < Segment.NUM_SIDES; j++) {
								if (segment[i].side[j].marked) {
									for (int k = 0; k < Side.NUM_VERTS; k++) {
										vertex[segment[i].side[j].vert[k]].m_tag = true;
									}
								}
							}
						}
					}
					break;
			}
		}

		public void TagSelectedSide()
		{
			if (selected_side > -1 && selected_segment > -1) {
				segment[selected_segment].side[selected_side].m_tag = true;
			}
		}

		public int JoinSidesWithTaggedVertices()
		{
			List<Side> side_list = new List<Side>();

			// Get the list of sides with tagged verts	
			for (int i = 0; i < MAX_SEGMENTS; i++) {
				if (segment[i].Alive) {
					for (int j = 0; j < Segment.NUM_SIDES; j++) {
						if (segment[i].neighbor[j] < 0) {
							for (int k = 0; k < Side.NUM_VERTS; k++) {
								if (vertex[segment[i].side[j].vert[k]].m_tag) {
									side_list.Add(segment[i].side[j]);
									break;
								}
							}
						}
					}
				}
			}

			return JoinListOfSides(side_list);
		}

		public int JoinMarkedSides()
		{
			List<Side> side_list = GetMarkedSides();

			return JoinListOfSides(side_list);
		}

		public List<Segment> GetMarkedSegments(bool get_selected = true)
		{
			List<Segment> seg_list = new List<Segment>();

			for (int i = 0; i < MAX_SEGMENTS; i++) {
				if (segment[i].Alive && segment[i].marked) {
					seg_list.Add(segment[i]);
				}
			}

			if (seg_list.Count == 0 && get_selected) {
				if (selected_segment > -1 && segment[selected_segment].Alive) {
					seg_list.Add(segment[selected_segment]);
				}
			}

			return seg_list;
		}


		public List<Segment> GetTaggedSegments()
		{
			List<Segment> seg_list = new List<Segment>();

			for (int i = 0; i < MAX_SEGMENTS; i++) {
				if (segment[i].Alive && segment[i].m_tag) {
					seg_list.Add(segment[i]);
				}
			}

			return seg_list;
		}

		public List<Side> GetMarkedSides(bool ignore_open = false, bool get_selected = false)
		{
			List<Side> side_list = new List<Side>();

			// Get the list of marked sides		
			for (int i = 0; i < MAX_SEGMENTS; i++) {
				if (segment[i].Alive) {
					for (int j = 0; j < Segment.NUM_SIDES; j++) {
						if (segment[i].neighbor[j] < 0 || !ignore_open) {
							if (segment[i].side[j].marked) {
								side_list.Add(segment[i].side[j]);
							}
						}
					}
				}
			}

			if (side_list.Count == 0 && get_selected) {
				Side s = GetSelectedSide();
				if (s != null) {
					side_list.Add(s);
				}
			}

			return side_list;
		}

		public List<Side> GetSidesWithTaggedVertices()
		{
			List<Side> side_list = new List<Side>();

			// Get the list of sides with tagged verts
			for (int i = 0; i < MAX_SEGMENTS; i++) {
				if (segment[i].Alive) {
					for (int j = 0; j < Segment.NUM_SIDES; j++) {
						if (segment[i].side[j].HasTaggedVerts()) {
							side_list.Add(segment[i].side[j]);
						}
					}
				}
			}

			return side_list;
		}

		public int JoinListOfSides(List<Side> side_list)
		{
			Side s1;
			Side s2;

			int join_count = 0;
			for (int i = 0; i < side_list.Count; i++) {
				for (int j = i + 1; j < side_list.Count; j++) {
					s1 = side_list[i];
					s2 = side_list[j];

					// See if these sides match (via centerpoint), and join them if so
					if ((s1.FindCenter() - s2.FindCenter()).LengthSquared < MATCH_TOL) {
						s1.AlignToSide(s2);
						SetSidesAsNeighbors(s1, s2);
						RemoveDuplicateVerts(s1.vert);
						join_count += 1;
					}
				}
			}

			return join_count;
		}

		//src moves to match up with dst
		public void JoinSides(Side src, Side dst)
		{
			src.AlignToSide(dst);
			SetSidesAsNeighbors(src, dst);
			RemoveDuplicateVerts(src.vert);
		}

		public bool MaybeJoinSelectedSideToNearest(float range)
		{
			// Move the selected side's verts to match the marked one's
			Side s1 = segment[selected_segment].side[selected_side];
			Side s2 = FindClosestSide(s1, range);
			if (s2 != null) {
				s1.AlignToSide(s2);
				SetSidesAsNeighbors(s1, s2);
				RemoveDuplicateVerts(s1.vert);
				return true;
			} else {
				// No nearby side
				return false;
			}
		}

		// Find the closest side that is not in the same segment with the Side "s"
		public Side FindClosestSide(Side s, float range)
		{
			Side closest_side = null;
			float closest_dist = range;
			float dist = 0.0f;
			Vector3 s_center = s.FindCenter();
			Vector3 center;

			for (int i = 0; i < MAX_SEGMENTS; i++) {
				if (segment[i].Visible && i != s.segment.num) {
					for (int j = 0; j < Segment.NUM_SIDES; j++) {
						// Only consider sides with no neighbors
						if (segment[i].neighbor[j] < 0) {
							center = segment[i].side[j].FindCenter();
							dist = (center - s_center).Length;
							if (dist < closest_dist) {
								closest_dist = dist;
								closest_side = segment[i].side[j];
							}
							
						}
					}
				}
			}

			return closest_side;
		}

		public bool SetSidesAsNeighbors(Side s1, Side s2)
		{
			if (s1.segment.num != s2.segment.num) {
				segment[s1.segment.num].neighbor[s1.num] = s2.segment.num;
				segment[s2.segment.num].neighbor[s2.num] = s1.segment.num;
				return true;
			} else {
				// Can't join a segment to itself
				return false;
			}
		}

		public Side FindFirstMarkedSide(bool neighborless = true)
		{
			for (int i = 0; i < MAX_SEGMENTS; i++) {
				if (segment[i].Alive) {
					for (int j = 0; j < Segment.NUM_SIDES; j++) {
						if (segment[i].side[j].marked) {
							if (!neighborless || segment[i].neighbor[j] < 0) {
								return segment[i].side[j];
							}
						}
					}
				}
			}

			return null;
		}

		public void MergeAllOverlappingVerts()
		{
			List<int> verts = new List<int>();
			for (int i = 0; i < MAX_VERTICES; i++) {
				if (vertex[i].alive) {
					verts.Add(i);
				}
			}

			RemoveDuplicateVerts(verts.ToArray());

			RemoveOrphanVerts();
		}

		public void RemoveOrphanVerts()
		{
			UnTagAllVertices();

			for (int i = 0; i < MAX_SEGMENTS; i++) {
				if (segment[i].Alive) {
					for (int j = 0; j < Segment.NUM_VERTS; j++) {
						vertex[segment[i].vert[j]].m_tag = true;
					}
				}
			}

			int count = 0;
			for (int i = 0; i < MAX_VERTICES; i++) {
				if (!vertex[i].m_tag) {
					if (vertex[i].alive) {
						vertex[i].alive = false;
						count += 1;
					}
				}
			}

			Utility.DebugLog("Removed " + count.ToString() + " orphan vertices");
		}

		public void RemoveDuplicateVerts(int[] vrts)
		{
			// For each vertex in the list, merge it with another vert (possibly more than 1)
			for (int i = 0; i < vrts.Length; i++) {
				for (int j = 0; j < MAX_VERTICES; j++) {
					if (vertex[j].alive && j != vrts[i]) {
						if (VerticeMatch(vrts[i], j)) {
							MergeVertex(vrts[i], j);
						}
					}
				}
			}
		}

		public const float MATCH_TOL = 0.01f;

		public bool VerticeMatch(int v1, int v2)
		{
			Vector3 diff = vertex[v1].position - vertex[v2].position;
			return (diff.LengthSquared < MATCH_TOL);
		}

		public void MergeVertex(int v1, int v2)
		{
			bool merge = false;
			// For every reference of v1 in the level, set it to v2
			for (int i = 0; i < MAX_SEGMENTS; i++) {
				if (segment[i].Alive) {
					merge = false;

					// Adjust the segment reference
					for (int j = 0; j < Segment.NUM_VERTS; j++) {
						if (segment[i].vert[j] == v1) {
							segment[i].vert[j] = v2;
							merge = true;
							break;
						}
					}

					// Adjust the side references
					if (merge) {
						for (int j = 0; j < Segment.NUM_SIDES; j++) {
							for (int k = 0; k < Side.NUM_VERTS; k++) {
								if (segment[i].side[j].vert[k] == v1) {
									segment[i].side[j].vert[k] = v2;
									break;
								}
							}
						}
					}
				}
			}
		}

		public bool SelectAdjacentSegment()
		{
			if (selected_segment > -1 && selected_side > -1 && segment[selected_segment].neighbor[selected_side] > -1) {
				int prev_seg = selected_segment;
				selected_segment = segment[selected_segment].neighbor[selected_side];

				// Select the side that is away from the previous side
				for (int i = 0; i < Segment.NUM_SIDES; i++) {
					if (segment[selected_segment].neighbor[i] == prev_seg) {
						selected_side = Utility.OppositeSide(i);
						break;
					}
				}

				return true;
			} else {
				return false;
			}
		}

		public bool SelectOppositeSegment()
		{
			if (selected_segment > -1 && selected_side > -1 && segment[selected_segment].neighbor[Utility.OppositeSide(selected_side)] > -1) {
				int prev_seg = selected_segment;
				selected_segment = segment[selected_segment].neighbor[Utility.OppositeSide(selected_side)];

				// Select the side that is away from the previous side
				for (int i = 0; i < Segment.NUM_SIDES; i++) {
					if (segment[selected_segment].neighbor[i] == prev_seg) {
						selected_side = i;
						break;
					}
				}

				return true;
			} else {
				return false;
			}
		}

		public void CycleSelectedSegment(bool prev)
		{
			for (int i = 0; i < MAX_SEGMENTS; i++) {
				selected_segment += (prev ? -1 : 1);
				if (selected_segment < 0) {
					selected_segment = MAX_SEGMENTS - 1;
				} else if (selected_segment > MAX_SEGMENTS - 1) {
					selected_segment = 0;
				}
				if (segment[selected_segment].Visible) {
					return;
				}
			}
		}

		public void CycleSelectedSide(bool prev)
		{
			if (selected_side < 0) {
				selected_side = (prev ? Segment.NUM_SIDES - 1 : 0);
			} else {
				if (prev) {
					selected_side = (selected_side - 1 + Segment.NUM_SIDES) % Segment.NUM_SIDES;
				} else {
					selected_side = (selected_side + 1) % Segment.NUM_SIDES;
				}
			}
		}

		public void CycleSelectedEntity(bool prev)
		{
			int new_selected_entity = selected_entity;
			for (int i = 0; i < MAX_ENTITIES; i++) {
				new_selected_entity += (prev ? -1 : 1);
				if (new_selected_entity < 0) {
					new_selected_entity = MAX_ENTITIES - 1;
				} else if (new_selected_entity > MAX_ENTITIES - 1) {
					new_selected_entity = 0;
				}
				if (entity[new_selected_entity].alive) {
					selected_entity = new_selected_entity;
					return;
				}
			}
		}

		//public void MarkCoplanarConnectedSides(Side s, float angle_tol, bool recursive = false)
		//{
		//	float angle;
		//	for (int i = 0; i < MAX_SEGMENTS; i++) {
		//		if (segment[i].alive) {
		//			for (int j = 0; j < Segment.NUM_SIDES; j++) {
		//				if (!segment[i].side[j].marked && segment[i].neighbor[j] < 0) {
		//					// Check for two common verts
		//					if (segment[i].side[j].HasTwoSharedVerts(s)) {
		//						angle = Vector3.CalculateAngle(segment[i].side[j].FindNormal(), s.FindNormal());
		//						//Utility.DebugLog("Found an angle of " + angle.ToString() + " between adjacent sides with normals: " + segment[i].side[j].FindNormal().ToString() + " and " + s.FindNormal().ToString());
		//						if (angle <= angle_tol) {
		//							segment[i].side[j].marked = true;
		//							if (recursive) {
		//								MarkCoplanarConnectedSides(segment[i].side[j], angle_tol, true);
		//							}
		//						}
		//					}
		//				}
		//			}
		//		}
		//	}
		//}

		//Used to use Vector3.CalculateAngle() but it was returning NaN for some nearly-identical vectors, presumable because precision errors were causing the dot product to be > 1
		private float CalculateAngle(Vector3 n1, Vector3 n2)
		{
			float dot = Vector3.Dot(n1, n2);
			return (dot > 1.0f) ? 0 : (float)Math.Acos(dot);		//If >1 (due to precision errors) say angle is zero
		}

		public void TagCoplanarConnectedSides(Side s, float angle_tol, bool recursive = false)
		{
			float angle;
			for (int i = 0; i < MAX_SEGMENTS; i++) {
				if (segment[i].Visible) {
					for (int j = 0; j < Segment.NUM_SIDES; j++) {
						if (!segment[i].side[j].m_tag && segment[i].neighbor[j] < 0) {
							// Check for two common verts
							if (segment[i].side[j].HasTwoSharedVerts(s)) {
								angle = CalculateAngle(segment[i].side[j].FindNormal(), s.FindNormal());
								//Utility.DebugLog("Found an angle of " + angle.ToString() + " between adjacent sides with normals: " + segment[i].side[j].FindNormal().ToString() + " and " + s.FindNormal().ToString());
								if (angle <= angle_tol) {
									segment[i].side[j].m_tag = true;
									if (recursive) {
										TagCoplanarConnectedSides(segment[i].side[j], angle_tol, true);
									}
								}
							}
						}
					}
				}
			}
		}

		public void TagCoTextureConnectedSides(Side s, bool recursive = false)
		{
			for (int i = 0; i < MAX_SEGMENTS; i++) {
				if (segment[i].Visible) {
					for (int j = 0; j < Segment.NUM_SIDES; j++) {
						if (!segment[i].side[j].m_tag && segment[i].neighbor[j] < 0) {
							// Check for two common verts
							if (segment[i].side[j].HasTwoSharedVerts(s)) {
								if (s.m_tex_gl_id == segment[i].side[j].m_tex_gl_id) {
									segment[i].side[j].m_tag = true;
									if (recursive) {
										TagCoTextureConnectedSides(segment[i].side[j], true);
									}
								}
							}
						}
					}
				}
			}
		}

		public void TagConnectedWallSides(Side s, float y_normal_tol)
		{
			for (int i = 0; i < MAX_SEGMENTS; i++) {
				var curr_seg = segment[i];
				if (!curr_seg.Visible)
					continue;

				for (int j = 0; j < Segment.NUM_SIDES; j++) {
					var curr_seg_side = curr_seg.side[j];
					if (curr_seg_side.m_tag || curr_seg.neighbor[j] >= 0)
						continue;

					// Check for two common verts
					if (!curr_seg_side.HasTwoSharedVerts(s))
						continue;

					if (Math.Abs(curr_seg_side.FindNormal().Y) >= y_normal_tol)
						continue;

					curr_seg_side.m_tag = true;
					TagConnectedWallSides(curr_seg_side, y_normal_tol);
				}
			}
		}

		public void TagConnectedWallSidesStraight(Side s, float y_normal_tol)
		{
			for (int i = 0; i < MAX_SEGMENTS; i++) {
				var curr_seg = segment[i];
				if (!curr_seg.Visible)
					continue;

				for (int j = 0; j < Segment.NUM_SIDES; j++) {
					var curr_seg_side = curr_seg.side[j];
					if (curr_seg_side.m_tag || curr_seg.neighbor[j] >= 0)
						continue;

					// Check for two common verts
					if (!curr_seg_side.HasTwoSharedVerts(s))
						continue;

					// Check for being moslty vertical
					if (Math.Abs(curr_seg_side.FindNormal().Y) >= y_normal_tol)
						continue;

					// Check for being in XZ plane
					if (Math.Abs(curr_seg_side.FindCenter().Y - s.FindCenter().Y) > 0.4f)
						continue;

					curr_seg_side.m_tag = true;
					TagConnectedWallSidesStraight(curr_seg_side, y_normal_tol);
				}
			}
		}

		public void MarkTaggedSides()
		{
			for (int i = 0; i < MAX_SEGMENTS; i++) {
				if (segment[i].Alive) {
					for (int j = 0; j < Segment.NUM_SIDES; j++) {
						if (segment[i].side[j].m_tag) {
							segment[i].side[j].marked = true;
						}
					}
				}
			}
		}

		public List<Vector3> AllVertexPositions()
		{
			List<Vector3> pos_list = new List<Vector3>();

			for (int i = 0; i < MAX_VERTICES; i++) {
				if (vertex[i].alive) {
					pos_list.Add(vertex[i].position);
				}
			}

			return pos_list;
		}

		public List<Vertex> GetTaggedVerts()
		{
			List<Vertex> vert_list = new List<Vertex>();

			for (int i = 0; i < MAX_VERTICES; i++) {
				if (vertex[i].alive && vertex[i].m_tag) {
					vert_list.Add(vertex[i]);
				}
			}

			return vert_list;
		}

		public List<Vector3> AllMarkedVertexPositions(bool maybe_selected = false)
		{
			List<Vector3> pos_list = new List<Vector3>();

			TagMarkedElementVertices();

			for (int i = 0; i < MAX_VERTICES; i++) {
				if (vertex[i].m_tag) {
					pos_list.Add(vertex[i].position);
				}
			}

			// If you have 1 or fewer verts so far, add the selected segment's verts (maybe)
			if (maybe_selected) {
				if (pos_list.Count < 2 && selected_segment > -1) {
					for (int i = 0; i < Segment.NUM_VERTS; i++) {
						pos_list.Add(vertex[segment[selected_segment].vert[i]].position);
					}
				}
			}

			return pos_list;
		}

		public void SelectAllSidesWithNumber(int num)
		{
			for (int i = 0; i < MAX_SEGMENTS; i++) {
				segment[i].side[num].marked = true;
			}
		}

		public void SelectAllSidesWithNumberInMarkedSegments(int num)
		{
			for (int i = 0; i < MAX_SEGMENTS; i++) {
				if (segment[i].marked) {
					segment[i].side[num].marked = true;
				}
			}
		}

		public void SelectAllSidesWithDeformationInMarkedSegments()
		{
			for (int i = 0; i < MAX_SEGMENTS; i++) {
				if (segment[i].marked) {
					for (int j=0; j < Segment.NUM_SIDES; j++) {
						if (segment[i].side[j].deformation_height > 0f) {
							segment[i].side[j].marked = true;
						}
					}
				}
			}
		}

		public void UnmarkSegmentSides(Segment seg)
		{
			for (int j = 0; j < Segment.NUM_SIDES; j++) {
				seg.side[j].marked = false;
			}
		}

		public void UnmarkAllSides()
		{
			foreach (Segment seg in EnumerateAliveSegments()) {
				UnmarkSegmentSides(seg);
			}
		}

		public void AlignAllMarkedSides()
		{
			// Warning: This is not fast
			for (int i = 0; i < MAX_SEGMENTS; i++) {
				if (segment[i].Visible) {
					for (int j = 0; j < Segment.NUM_SIDES; j++) {
						UVAlignAdjacentMarkedSides(segment[i].side[j]);
					}
				}
			}
		}

		public int lava_tex_id = -1;

		public void QuickTextureMarkedSegments(bool restore_lava)
		{
			TextureSet texture_set = GetTextureSet();
			List<Side> lava_sides = null;
			if (restore_lava) {
				lava_sides = FindSidesWithLava();
			}

			// This is hacky but useful
			UnmarkAllSides();
			SelectAllSidesWithNumberInMarkedSegments((int)SideOrder.BOTTOM);
			ApplyTexture(texture_set.Floor, editor.tm_level.FindTextureIDByName(texture_set.Floor));
			UVDefaultMapMarkedSides();
			//AlignAllMarkedSides();

			UnmarkAllSides();
			SelectAllSidesWithNumberInMarkedSegments((int)SideOrder.TOP);
			ApplyTexture(texture_set.Ceiling, editor.tm_level.FindTextureIDByName(texture_set.Ceiling));
			UVDefaultMapMarkedSides();
			//AlignAllMarkedSides();

			UnmarkAllSides();
			SelectAllSidesWithNumberInMarkedSegments((int)SideOrder.BACK);
			SelectAllSidesWithNumberInMarkedSegments((int)SideOrder.FRONT);
			SelectAllSidesWithNumberInMarkedSegments((int)SideOrder.LEFT);
			SelectAllSidesWithNumberInMarkedSegments((int)SideOrder.RIGHT);
			ApplyTexture(texture_set.Wall, editor.tm_level.FindTextureIDByName(texture_set.Wall));
			UVDefaultMapMarkedSides();
			//UnmarkAllSides();

			//Overwrite cave sides with cave texture
			UnmarkAllSides();
			SelectAllSidesWithDeformationInMarkedSegments();
			if (num_marked_sides > 0) {
				ApplyTexture(texture_set.Cave, editor.tm_level.FindTextureIDByName(texture_set.Cave));
				UVDefaultMapMarkedSides();
				AlignAllMarkedSides();
			}

			if (restore_lava) {
				UnmarkAllSides();
				for (int i = 0; i < lava_sides.Count; i++) {
					lava_sides[i].tex_name = "lava_v2";
					lava_sides[i].m_tex_gl_id = lava_tex_id;
				}
			}
		}

		public List<Side> FindSidesWithLava()
		{
			List<Side> sides = new List<Side>();
			for (int i = 0; i < MAX_SEGMENTS; i++) {
				if (segment[i].Alive && segment[i].marked) {
					for (int j = 0; j < Segment.NUM_SIDES; j++) {
						if (segment[i].side[j].m_tex_gl_id > -1 && ((lava_tex_id == -1 && segment[i].side[j].tex_name.ToLower() == "lava_v2") || lava_tex_id == segment[i].side[j].m_tex_gl_id)) {
							lava_tex_id = segment[i].side[j].m_tex_gl_id;
							sides.Add(segment[i].side[j]);
						}
					}
				}
			}

			return sides;
		}

		// Amount is how much to move the verts
		public void ApplyNoiseToMarkedSides(float amt)
		{
			// Tag the verts in those sides
			UnTagAllVertices();
			TagVertsInMarkedSides();

			for (int i = 0; i < MAX_VERTICES; i++) {
				if (vertex[i].alive && vertex[i].m_tag) {
					vertex[i].position.X += Utility.RandomRange(-amt, amt);
					vertex[i].position.Y += Utility.RandomRange(-amt, amt);
					vertex[i].position.Z += Utility.RandomRange(-amt, amt);
				}
			}
		}

		// Amount should be between 0 and 0.5 (or so)
		public void SmoothMarkedSides(float amt)
		{
			// This is not a good way to do this, but it does something...
			List<Side> side_list = GetMarkedSides();

			Vector3 center;
			for (int i = 0; i < side_list.Count; i++) {
				center = side_list[i].FindCenter();
				for (int j = 0; j < Side.NUM_VERTS; j++) {
					vertex[side_list[i].vert[j]].position = vertex[side_list[i].vert[j]].position * (1f - amt) + center * amt;
				}
			}
		}

		public void TagVertsInMarkedSides()
		{
			List<Side> side_list = GetMarkedSides();

			for (int i = 0; i < side_list.Count; i++) {
				for (int j = 0; j < Side.NUM_VERTS; j++) {
					vertex[side_list[i].vert[j]].m_tag = true;
				}
			}
		}

		public string GetDecalFromSide(int idx)
		{
			if (selected_side > -1 && selected_segment > -1) {
				Side s = segment[selected_segment].side[selected_side];
				return s.decal[idx].mesh_name;
			} else {
				return "";
			}
		}

		public void MarkSidesWithDecal(int idx, string dcl_name)
		{
			dcl_name = dcl_name.ToLower();
			for (int i = 0; i < MAX_SEGMENTS; i++) {
				if (segment[i].Visible) {
					for (int j = 0; j < Segment.NUM_SIDES; j++) {
						if (!segment[i].side[j].decal[idx].hidden && segment[i].side[j].decal[idx].mesh_name.ToLower() == dcl_name) {
							segment[i].side[j].marked = true;
						}
					}
				}
			}
		}

		public void FlattenAllNonFlatSides()
		{
			TagAllVertices();

			// Untag anything we don't want to flatten
			for (int i = 0; i < MAX_SEGMENTS; i++) {
				if (segment[i].Visible) {
					for (int j = 0; j < Segment.NUM_SIDES; j++) {
						if (segment[i].neighbor[j] < 0) {
							if (segment[i].side[j].deformation_height < 0.05f) {
								vertex[segment[i].side[j].vert[0]].m_tag = false;
								vertex[segment[i].side[j].vert[1]].m_tag = false;
								vertex[segment[i].side[j].vert[2]].m_tag = false;
								vertex[segment[i].side[j].vert[3]].m_tag = false;
							}
						}
					}
				}
			}

			/*for (int i = 0; i < MAX_VERTICES; i++) {
				if (vertex[i].m_tag) {
					vertex[i].marked = true;
				}
			}*/
			
			UnTagAllSides();
			int count;
			// Tag all sides that have 4 untagged verts
			for (int i = 0; i < MAX_SEGMENTS; i++) {
				if (segment[i].Visible) {
					for (int j = 0; j < Segment.NUM_SIDES; j++) {
						count = 0;
						for (int k = 0; k < 4; k++) {
							if (vertex[segment[i].side[j].vert[k]].m_tag) {
								count += 1;
							}
						}
						if (count == 4 && segment[i].neighbor[j] > -1) {
							segment[i].side[j].m_tag = true;
							//segment[i].side[j].marked = true;
						}
					}
				}
			}
			
			// Find the normals for all 4 tris of a side, and average them
			Vector3[] norms = new Vector3[4];
			for (int i = 0; i < MAX_SEGMENTS; i++) {
				if (segment[i].Visible) {
					for (int j = 0; j < Segment.NUM_SIDES; j++) {
						if (segment[i].side[j].m_tag) {
							norms[0] = Utility.FindNormal(vertex[segment[i].side[j].vert[0]].position, vertex[segment[i].side[j].vert[1]].position, vertex[segment[i].side[j].vert[2]].position);
							norms[1] = Utility.FindNormal(vertex[segment[i].side[j].vert[1]].position, vertex[segment[i].side[j].vert[2]].position, vertex[segment[i].side[j].vert[3]].position);
							norms[2] = Utility.FindNormal(vertex[segment[i].side[j].vert[2]].position, vertex[segment[i].side[j].vert[3]].position, vertex[segment[i].side[j].vert[0]].position);
							norms[3] = Utility.FindNormal(vertex[segment[i].side[j].vert[3]].position, vertex[segment[i].side[j].vert[0]].position, vertex[segment[i].side[j].vert[1]].position);
							segment[i].side[j].avg_normal = ((norms[0] + norms[1] + norms[2] + norms[3]) * 0.25f).Normalized();
							segment[i].side[j].avg_position = (vertex[segment[i].side[j].vert[0]].position + vertex[segment[i].side[j].vert[1]].position + vertex[segment[i].side[j].vert[2]].position + vertex[segment[i].side[j].vert[3]].position) * 0.25f;
						}
					}
				}
			}

			// Move all verts along the normal (a bit), based on their distance
			float dist_from_plane = 0f;
			for (int i = 0; i < MAX_SEGMENTS; i++) {
				if (segment[i].Visible) {
					for (int j = 0; j < Segment.NUM_SIDES; j++) {
						if (segment[i].side[j].m_tag) {
							for (int k = 0; k < 4; k++) {
								dist_from_plane = Utility.DistanceFromPlane(vertex[segment[i].side[j].vert[k]].position, segment[i].side[j].avg_position, segment[i].side[j].avg_normal);
								vertex[segment[i].side[j].vert[k]].position = vertex[segment[i].side[j].vert[k]].position + segment[i].side[j].avg_normal * 0.2f * dist_from_plane;
							}
						}
					}
				}
			}
		}

		// Split the segment into 2 segments
		// Side given determines the direction of the split
		// Any attached segments along the split are detached
		public void SplitSegment2Way(int seg_idx, int side_idx)
		{
			// Detach the neighbors
			for (int i = 0; i < Segment.NUM_SIDES; i++) {
				if (i == side_idx || i == Utility.OppositeSide(side_idx)) {
					// Skip it
				} else {
					segment[seg_idx].DetatchSide(i);
				}
			}

			// Add new verts between the correct verts
			List<int> new_verts = AddHalfwayVertsFromSide(seg_idx, side_idx);

			// Create a new segment with my side verts and the new verts
			Side s = segment[seg_idx].side[side_idx];

			int[] seg_verts = new int[Segment.NUM_VERTS];
			// Create 4 new vertices, and use the 4 from the current side
			for (int i = 0; i < Side.NUM_VERTS; i++) {
				seg_verts[i] = new_verts[i];
				seg_verts[i + 4] = s.vert[i];
			}

			// Reorder verts so the new segment is oriented the same
			seg_verts = ReorderVertsForSide(seg_verts, (SideOrder)s.num);

			// Set the neighbors
			int[] neighbors_new = new int[Segment.NUM_SIDES];
			for (int i = 0; i < Segment.NUM_SIDES; i++) {
				neighbors_new[i] = -1;
			}
			neighbors_new[Utility.OppositeSide(s.num)] = s.segment.num;

			// Create the new segment
			int new_seg_idx = CreateSegment(seg_verts, neighbors_new);

			// TODO: Adjust the UVs to match the segments

			// Fix up the new segment's neighbor
			segment[new_seg_idx].neighbor[side_idx] = s.segment.neighbor[side_idx];
			if (s.segment.neighbor[side_idx] > -1) {
				Segment old_nseg = segment[s.segment.neighbor[side_idx]];
				old_nseg.neighbor[old_nseg.FindConnectingSide(seg_idx)] = new_seg_idx;
			}

			// Assign the old segments side neighbor
			s.segment.neighbor[side_idx] = new_seg_idx;

			// Copy all the stuff to the new segment
			segment[new_seg_idx].CopyTexturesAndDecalsFromSegmentAtBack(segment[seg_idx], side_idx);

			// Set my old verts to the new verts
			ReplaceVertsInSegment(seg_idx, Utility.SideVertsFromSegVerts(segment[seg_idx].vert, side_idx), new_verts.ToArray());
		}

		public void ReplaceVertsInSegment(int seg, int[] old_verts, int[] new_verts)
		{
			for (int i = 0; i < Segment.NUM_SIDES; i++) {
				for (int j = 0; j < Side.NUM_VERTS; j++) {
					for (int k = 0; k < new_verts.Length; k++) {
						if (segment[seg].side[i].vert[j] == old_verts[k]) {
							segment[seg].side[i].vert[j] = new_verts[k];
                  }
					}
				}
			}

			for (int i = 0; i < Segment.NUM_VERTS; i++) {
				for (int k = 0; k < new_verts.Length; k++) {
					if (segment[seg].vert[i] == old_verts[k]) {
						segment[seg].vert[i] = new_verts[k];
					}
				}
			}
		}

		public List<int> AddHalfwayVertsFromSide(int seg, int side)
		{
			List<int> new_verts = new List<int>();

			int[] v0 = Utility.SideVertsFromSegVerts(segment[seg].vert, side);
			int[] v1 = Utility.SideVertsFromSegVerts(segment[seg].vert, Utility.OppositeSide(side));
         for (int i = 0; i < Side.NUM_VERTS; i++) {
				new_verts.Add(CreateVertexBetween(v0[i], v1[3 - i]));
         }

			return new_verts;
		}

		public int CreateVertexBetween(int v0, int v1)
		{
			return CreateVertex((vertex[v0].position + vertex[v1].position) * 0.5f);
		}

		// Split the segment into 5 segments, with the original segment at the center
		// Side given (and the opposite side) do not get a new segment attached
		// Any attached segments along the split are detached
		public void SplitSegment5Way(int seg_idx, int side_idx)
		{
			UnTagAllVertices();

			// 1. Detach the selected segment (save the neighbors of the 4 reconnecting sides
			int[] old_neighbor = new int[6];
			for (int i = 0; i < Segment.NUM_SIDES; i++) {
				old_neighbor[i] = segment[seg_idx].neighbor[i];
				segment[seg_idx].DetatchSide(i);
			}

			int[] dup_verts = DuplicateSegmentVerts(segment[seg_idx]);
			int[] old_verts = new int[8];
			// Tag all these verts for use later in joining sides
			for (int i = 0; i < Segment.NUM_VERTS; i++) {
				old_verts[i] = segment[seg_idx].vert[i];
            vertex[segment[seg_idx].vert[i]].m_tag = true;
				vertex[dup_verts[i]].m_tag = true;
			}
			ReplaceVertsInSegment(seg_idx, segment[seg_idx].vert, dup_verts);

			// 2. Move verts inward (do once for both split sides)
			segment[seg_idx].side[side_idx].Scale(0.5f);
			segment[seg_idx].side[Utility.OppositeSide(side_idx)].Scale(0.5f);

			// 3. Insert new connecting segments on the other sides
			for (int i = 0; i < Segment.NUM_SIDES; i++) {
				if (i == side_idx || i == Utility.OppositeSide(side_idx)) {
					// Nothing
				} else {
					int idx = InsertSegment4Verts(segment[seg_idx].side[i], Utility.SideVertsFromSegVerts(old_verts, i));
					segment[idx].CopyTexturesAndDecalsFromSegmentAtBack(segment[seg_idx], i);
				}
			}

			// 4. Join the 4 connecting segments with everything else
			JoinSidesWithTaggedVertices();
		}

		public int[] DuplicateSegmentVerts(Segment seg)
		{
			int[] v = new int[Segment.NUM_VERTS];
			for (int i = 0; i < Segment.NUM_VERTS; i++) {
				v[i] = CreateVertex(vertex[seg.vert[i]].position);
			}

			return v;
		}

		// Split the segment into 7 segments, with the original segment at the center
		public void SplitSegment7Way(int seg_idx)
		{
			UnTagAllVertices();

			// 1. Detach the selected segment (save the neighbors of the 4 reconnecting sides
			int[] old_neighbor = new int[6];
			for (int i = 0; i < Segment.NUM_SIDES; i++) {
				old_neighbor[i] = segment[seg_idx].neighbor[i];
				segment[seg_idx].DetatchSide(i);
			}

			int[] dup_verts = DuplicateSegmentVerts(segment[seg_idx]);
			int[] old_verts = new int[8];
			// Tag all these verts for use later in joining sides
			for (int i = 0; i < Segment.NUM_VERTS; i++) {
				old_verts[i] = segment[seg_idx].vert[i];
				vertex[segment[seg_idx].vert[i]].m_tag = true;
				vertex[dup_verts[i]].m_tag = true;
			}
			ReplaceVertsInSegment(seg_idx, segment[seg_idx].vert, dup_verts);

			// 2. Move verts inward
			segment[seg_idx].Scale(0.5f);
			
			// 3. Insert new connecting segments on the other sides
			for (int i = 0; i < Segment.NUM_SIDES; i++) {
				int idx = InsertSegment4Verts(segment[seg_idx].side[i], Utility.SideVertsFromSegVerts(old_verts, i));
				segment[idx].CopyTexturesAndDecalsFromSegmentAtBack(segment[seg_idx], i);
			}

			// 4. Join the 4 connecting segments with everything else
			JoinSidesWithTaggedVertices();

		}

		public void AverageMarkedSideVertsY()
		{
			UnTagAllVertices();
			List<Side> side_list = GetMarkedSides(false);

			// Tag all the verts in the marked sides
			foreach (Side side in side_list) {
				for (int i = 0; i < Side.NUM_VERTS; i++) {
					vertex[side.vert[i]].m_tag = true;
				}
			}

			float y_value = 0f;
			int count = 0;

			// Add the Y values
			for (int i = 0; i < MAX_VERTICES; i++) {
				if (vertex[i].m_tag) {
					y_value += vertex[i].position.Y;
					count += 1;
				}
			}

			if (count > 0) {
				y_value /= count;

				// Average them
				for (int i = 0; i < MAX_VERTICES; i++) {
					if (vertex[i].m_tag) {
						vertex[i].position.Y = y_value;
               }
				}
			}
		}

		// Reorient the marked segments so they are upward
		public void ReorientMarkedSegments()
		{
			List<Segment> seg_list = GetMarkedSegments();

			// Reorient the marked segments
			foreach (Segment seg in seg_list) {
				ReorientSegment(seg);
			}

			// Mark the segment if there's only 1
			if (seg_list.Count == 1) {
				seg_list[0].marked = true;
			}

			// Delete the marked segments
			editor.ActiveEditMode = EditMode.SEGMENT;
			DeleteMarked(true);

			// Connect sides
			JoinMarkedSides();
		}

		public void ReorientSegment(Segment seg)
		{
			// 1. Figure out which side is most upward
			int upside = seg.FindTopSide();
			int bottomside = Utility.OppositeSide(upside);

			// 2. Create a new segment, attach the verts of the top and bottom appropriately
			int[] new_verts = new int[Segment.NUM_VERTS];
			new_verts[0] = seg.side[upside].vert[0];
			new_verts[1] = seg.FindLevelVertNotOnSide(seg.side[upside].vert[0], upside);
			new_verts[2] = seg.FindLevelVertNotOnSide(seg.side[upside].vert[1], upside);
			new_verts[3] = seg.side[upside].vert[1];
			new_verts[4] = seg.side[upside].vert[3];
			new_verts[5] = seg.FindLevelVertNotOnSide(seg.side[upside].vert[3], upside);
			new_verts[6] = seg.FindLevelVertNotOnSide(seg.side[upside].vert[2], upside);
			new_verts[7] = seg.side[upside].vert[2];

			// Set the neighbors to none (for now)
			int[] neighbors = new int[Segment.NUM_SIDES];
			neighbors[0] = neighbors[1] = neighbors[2] = neighbors[3] = neighbors[4] = neighbors[5] = -1;

			// Create the new segment
			int new_seg_idx = CreateSegment(new_verts, neighbors);
			Segment new_seg = segment[new_seg_idx];

			// Flip it back to normal
			new_seg.Invert();

			// 3. Copy the side properties and neighbor from the original sides which have the same verts (including decals, even if it messes up orientation)
			int close_side = 0;
			for (int i = 0; i < Segment.NUM_SIDES; i++) {
				close_side = seg.FindSideWithSameVerts(new_seg.side[i]);
				new_seg.side[i].CopyTexturesAndDecals(seg.side[close_side]);

				// Mark it so we can join it later
				new_seg.side[i].marked = true;
			}
		}

		public bool m_segments_are_hidden = false;

		private void UnmarkSegmentVerts(Segment seg)
		{
			foreach (int vertnum in seg.vert) {
				this.vertex[vertnum].marked = false;
			}
		}

		private void HideSegment(Segment seg)
		{
			seg.m_hidden = m_segments_are_hidden = true;
			UnmarkSegmentSides(seg);
			UnmarkSegmentVerts(seg);
		}

		public void HideMarkedSegments()
		{
			foreach (Segment seg in EnumerateMarkedSegments()) {
				HideSegment(seg);
				seg.marked = false;
			}
		}

		public void HideUnmarkedSegments()
		{
			foreach (Segment seg in EnumerateUnmarkedSegments()) {
				HideSegment(seg);
			}
		}

		public void UnhideHiddenSegments()
		{
			foreach (Segment seg in EnumerateHiddenSegments()) {
				seg.m_hidden = false;
			}
			m_segments_are_hidden = false;
		}


		// POSSIBLE FUTURE FUNCTIONS
		// Mark marked segments' sides
		// CycleFaceVert
		// SelectNextVert
		// SelectPreviousVert
	}
}