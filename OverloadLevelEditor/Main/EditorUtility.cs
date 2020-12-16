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
using System.Windows.Forms;
using System.IO;
using OpenTK;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Diagnostics;

// EDITOR - Utility functions
// Mostly used for consolidating or redirecting editor commands to the specific steps
// Fairly random collection, so moving groups of functions to new files/etc is fine

namespace OverloadLevelEditor
{
	public partial class Editor : EditorDockContent
	{
		public int m_decal_active = 0;

		public Vector3 m_src_side_pos = Vector3.Zero;
		public Matrix4 m_src_side_rotation = Matrix4.Identity;
		public int m_src_seg_count = 0;
		public Vector3 m_dest_side_pos = Vector3.Zero;
		public Matrix4 m_dest_side_rotation = Matrix4.Identity;

		public void CopyMarkedSegments(bool silent = false)
		{
			Vector3 src_side_normal;
			if (m_level.GetSelectedSide() != null) {
				m_level.GetSelectedSidePosAndNormal(out m_src_side_pos, out src_side_normal);
				m_src_side_rotation = this.m_level.GetSelectedSideOrientation();
			} else {
				m_src_side_pos = Vector3.Zero;
				src_side_normal = Vector3.UnitX;
				m_src_side_rotation = Matrix4.Identity;
			}

			m_src_seg_count = m_lvl_buffer.CopyMarkedSegments(m_level);

			if (!silent) {
				AddOutputText("Copied " + m_src_seg_count.ToString() + " marked segments");
			}
		}

		public bool PasteBufferSegments(bool aligned, bool paste_entites, bool refresh = true)
		{
			if (m_src_seg_count > 0) {
				if (refresh) {
					SaveStateForUndo("Paste copied segments");
				}
				if (aligned) {
					Vector3 dest_side_normal;
					m_level.GetSelectedSidePosAndNormal(out m_dest_side_pos, out dest_side_normal);
					m_dest_side_rotation = this.m_level.GetSelectedSideOrientation();
				}
				m_level.PasteSegments(m_lvl_buffer, aligned);

				if (paste_entites) {
					m_level.PasteEntities(m_lvl_buffer, aligned);
				} else {
					//If not pasting entities, must go through and clear Door value on pasted sides
					m_level.ClearDoorReferences(m_lvl_buffer);
				}

				if (refresh) {
					AddOutputText("Pasted buffer segments (" + (aligned ? "at side" : "at origin") + ")" + (paste_entites ? " with entities" : ""));
					RefreshGeometry();
				}
				return true;
			} else {
				AddOutputText("Buffer has 0 segments");
				return false;
			}
		}

		public void PasteMirror(Axis axis)
		{
			SaveStateForUndo("Paste copied segments (mirror)");
			if (PasteBufferSegments(false, false)) {
				m_level.FlipMarkedSegments(axis, Vector3.Zero);
				AddOutputText("Pasted buffer segments (mirrored)");
				RefreshGeometry();
			}
		}

		public Matrix4 ComputeRotationFromNormal(Vector3 normal)
		{
			// Rotate to align to the normal
			if (Math.Abs(normal.Y) < (0.99f)) {
				return Matrix4.LookAt(Vector3.Zero, normal, Vector3.UnitY);
			} else {
				return Matrix4.LookAt(Vector3.Zero, normal, Vector3.UnitX);
			}
		}

		// Rotate and offset the position based on the src and dest side
		public Vector3 AlignPasteVert(Vector3 v)
		{
			// Remove src side position
			v -= m_src_side_pos;

			// Rotate to align to src normal
			v = Vector3.Transform(v, m_src_side_rotation.Inverted());

			v = Vector3.Transform(v, Matrix4.CreateScale(-1, 1, -1));

			// Rotate to align to dest normal
			v = Vector3.Transform(v, m_dest_side_rotation);

			// Add the dest side position
			v += m_dest_side_pos;

			return v;
		}

		public void MaybeFlipMarkedSegments(Axis axis)
		{
			if (!m_level.MarkedSegmentsAreIsolated()) {
				AddOutputText("Marked segments must be isolated to flip them");
			} else if (m_level.GetSelectedSide() == null) {
				AddOutputText("Must have selected segment/side to flip marked segments");
			} else {
				SaveStateForUndo("Flip marked segments");
				m_level.FlipMarkedSegments(axis, m_level.GetSelectedSidePos());
				AddOutputText("Flipped the marked segments");
				RefreshMarkedSegmentGMeshes();
				RefreshGeometry();
			}
		}

		public void MaybeRotateMarkedSegments(Axis axis, float rot, bool origin)
		{
			if (!m_level.MarkedSegmentsAreIsolated()) {
				AddOutputText("Marked segments must be isolated to rotate them");
			} else if (m_level.GetSelectedSide() == null) {
				AddOutputText("Must have selected segment/side to rotate marked segments");
			} else { 
				SaveStateForUndo("Rotate marked segments");
				m_level.RotateMarkedSegments(axis, rot, (origin ? Vector3.Zero : m_level.GetSelectedSidePos()));
				RefreshMarkedSegmentGMeshes();
				RefreshGeometry();
			}
		}

		public void IsolateMarkedSegments()
		{
			SaveStateForUndo("Isolate marked segments");
			CopyMarkedSegments(true);
			m_level.DeleteMarked(true);
			PasteBufferSegments(false, false);
			AddOutputText("Isolated the marked segments");
			RefreshGeometry();
		}

		// TODO: Maybe make this a setting?
		public const float JOIN_RANGE = 3f;

		public void JoinSelectedSideToMarked()
		{
			if (!m_level.SelectedSegmentAlive()) {
				AddOutputText("Must have a selected side for this operation");
				return;
			}
			if (m_level.num_marked_sides == 1) {
				if (!m_level.SelectedSideMarked()) {
					SaveStateForUndo("Join sides");

					// Move the selected side's verts to match the marked one's
					Side selected = m_level.GetSelectedSide();
					Side marked = m_level.FindFirstMarkedSide();
					if (marked == null) {
						Utility.DebugPopup("Could not find a marked side", "ERROR");
						return;
					}
					m_level.JoinSides(selected, marked);
					AddOutputText("Joined the selected side to the marked side");
					RefreshGeometry();
				} else {
					AddOutputText("Can't join a side to itself");
				}
			} else {
				SaveStateForUndo("Join sides nearby");
				if (m_level.MaybeJoinSelectedSideToNearest(JOIN_RANGE)) {
					AddOutputText("Joined the selected side to a nearby side");
					RefreshGeometry();
				} else {
					AddOutputText("Could not find a side to join to.");
				}
			}
		}

		public void JoinMarkedSides()
		{
			if (m_level.num_marked_sides > 1) {
				SaveStateForUndo("Join marked sides");
				int count = m_level.JoinMarkedSides();
				AddOutputText("Joined " + count + " pairs of marked sides");
				RefreshGeometry();
			} else {
				AddOutputText("Must mark 2 or more adjacent sides");
			}
		}


		//Returns true if at least two verts on check_side are in front of base_side, and none are behind
		public bool SideInFrontOfSide(Side base_side, Side check_side)
		{
			Vector3 center = base_side.FindCenter();
			Vector3 normal = -base_side.FindNormal();
			int zero_count = 0;

			for (int i = 0; i < Side.NUM_VERTS; i++) {

				float dot = Vector3.Dot(m_level.vertex[check_side.vert[i]].position - center, normal);

				if (dot < 0f) {
					return false;
				} else if (dot < 0.0001) {
					zero_count++;
				}
			}

			return (zero_count <= 2);
		}

		public void ConnectSelectedSideToMarkedWithSegment()
		{
			Side selected = m_level.GetSelectedSide();

			if (selected == null) {
				MessageBox.Show("Must have a selected side for this operation.");
				return;
			}

			if (m_level.num_marked_sides == 0) {
				MessageBox.Show("Must have a marked side for this operation.");
				return;
			}

			if (m_level.num_marked_sides > 1) {
				MessageBox.Show("Must have only one marked side for this operation.");
				return;
			}

			Side marked = m_level.FindFirstMarkedSide();

			if ((selected.segment.neighbor[selected.num] != -1) || (marked.segment.neighbor[marked.num] != -1)) {
				MessageBox.Show("Selected and Marked sides must not have connections for this operaion.");
				return;
			}

			//Check for any points on one face behind the other
			if (!SideInFrontOfSide(selected, marked) || !SideInFrontOfSide(marked, selected)) {
				MessageBox.Show("Selected and Marked sides must face each other for this operation.");
				return;
			}

			SaveStateForUndo("Connect with segment");

			//Insert the segment
			int new_segnum = m_level.InsertSegmentBetween(selected, marked);

			// Select the new segment (depending on the option)
			if (m_insert_advance) {
				m_level.selected_segment = new_segnum;
			}

			//Done
			RefreshGeometry();
		}

		//Rotate the marked segments around the center point of the selected side
		public void RotateAtSelectedSide(bool backwards)
		{
			Side selected_side = m_level.GetSelectedSide();

			if (selected_side == null) {
				MessageBox.Show("Must have a selected side for this operation.");
				return;
			}

			if (!m_level.MarkedSegmentsAreIsolated()) {
				MessageBox.Show("Marked segments must be isolated to rotate them");
				return;
			}
			Segment selected_seg = selected_side.segment;
			System.Diagnostics.Debug.Assert(selected_seg != null);

			SaveStateForUndo("Rotate at Selected Side");
			float angle = (float)m_rotate_angle / 180f * (float)Math.PI;
			m_level.RotateMarkedSegments(selected_side.FindNormal(), backwards ? -angle : angle, selected_side.FindCenter());
			RefreshGeometry();
		}

		public void MarkCoplanarSides()
		{
			if (m_level.selected_segment > -1 && m_level.selected_side > -1) {
				SaveStateForUndo("Mark coplanar sides", false);
				SetEditModeSilent(EditMode.SIDE);

				m_level.UnTagAllSides();
				m_level.TagSelectedSide();
				m_level.TagCoplanarConnectedSides(m_level.segment[m_level.selected_segment].side[m_level.selected_side], ((float)m_coplanar_tol) * Utility.RAD_90 / 90f, true);
				m_level.MarkTaggedSides();

				AddOutputText("Marked coplanar sides");
				RefreshGeometry();
			}
		}

		public void MarkCoTextureSides()
		{
			if (m_level.selected_segment > -1 && m_level.selected_side > -1) {
				SaveStateForUndo("Mark coplanar sides", false);
				SetEditModeSilent(EditMode.SIDE);

				m_level.UnTagAllSides();
				m_level.TagSelectedSide();
				m_level.TagCoTextureConnectedSides(m_level.segment[m_level.selected_segment].side[m_level.selected_side], true);
				m_level.MarkTaggedSides();

				AddOutputText("Marked coplanar sides");
				RefreshGeometry();
			}
		}

		public void MarkConnectedTextureSides()
		{
			if (m_level.selected_segment > -1 && m_level.selected_side > -1) {
				SaveStateForUndo("Mark same-textured connected sides", false);
				SetEditModeSilent(EditMode.SIDE);

				m_level.UnTagAllSides();
				m_level.TagSelectedSide();
				m_level.TagCoplanarConnectedSides(m_level.segment[m_level.selected_segment].side[m_level.selected_side], ((float)m_coplanar_tol) * Utility.RAD_90 / 90f, true);
				m_level.MarkTaggedSides();

				AddOutputText("Marked coplanar sides");
				RefreshGeometry();
			}
		}

		public void MarkWallSides()
		{
			if (m_level.selected_segment > -1 && m_level.selected_side > -1) {
				SaveStateForUndo("Mark wall sides", false);
				SetEditModeSilent(EditMode.SIDE);

				m_level.UnTagAllSides();
				m_level.TagSelectedSide();
				m_level.TagConnectedWallSides(m_level.segment[m_level.selected_segment].side[m_level.selected_side], 0.4f);
				m_level.MarkTaggedSides();

				AddOutputText("Marked wall sides");
				RefreshGeometry();
			}
		}

		public void MarkWallSidesStraight()
		{
			if (m_level.selected_segment > -1 && m_level.selected_side > -1) {
				SaveStateForUndo("Mark wall sides straight", false);
				SetEditModeSilent(EditMode.SIDE);

				m_level.UnTagAllSides();
				m_level.TagSelectedSide();
				m_level.TagConnectedWallSidesStraight(m_level.segment[m_level.selected_segment].side[m_level.selected_side], 0.4f);
				m_level.MarkTaggedSides();

				AddOutputText("Marked wall sides on same XY plane");
				RefreshGeometry();
			}
		}

		public void FitFrameLevel(bool all, GLView view)
		{
			List<Vector3> pos_list = pos_list = m_level.AllVertexPositions();

			if (all) {
				gl_panel.FitFrame(pos_list);
			} else {
				view.FitFrame(pos_list);
			}
		}

		public void FitFrameMarkedSelected(bool all, GLView view, bool dont_change_zoom = false)
		{
			List<Vector3> pos_list = m_level.AllMarkedVertexPositions(true);
			if (all) {
				gl_panel.FitFrame(pos_list, dont_change_zoom);
			} else {
				view.FitFrame(pos_list, dont_change_zoom);
			}
		}

		public const float UV_SHIFT = 0.25f;
		public const float UV_SHIFT_MINOR = UV_SHIFT / 8f;

		public void MoveMarkedSideTextures(Vector2 dir, bool minor)
		{
			SaveStateForUndo("Move textures");
			m_level.UVMoveMarkedSide(dir * (minor ? UV_SHIFT_MINOR : UV_SHIFT));
			RefreshGeometry();
		}

		public const float UV_ROT = Utility.RAD_45;
		public const float UV_ROT_MINOR = UV_ROT / 8f;

		public void RotateMarkedSideTextures(float angle, bool minor)
		{
			SaveStateForUndo("Rotate textures");
			m_level.UVRotateMarkedSide(angle * (minor ? UV_ROT_MINOR : UV_ROT));
			RefreshGeometry();
		}

		public Axis AxisFromViewType(ViewType vt)
		{
			switch (vt) {
				case ViewType.FRONT:
					return Axis.Z;
				case ViewType.RIGHT:
					return Axis.X;
				default:
				case ViewType.TOP:
					return Axis.Y;
			}
		}

		public void SetActiveDecal(int idx)
		{
			m_decal_active = idx;
			GeometryDecalsPane.UpdateDecalLabels();
		}

		public void CycleDecalAlignment(bool reverse)
		{
			Decal d = m_level.GetSelectedDecal(m_decal_active);
			if (d != null) {
				SaveStateForUndo("Decal cycle alignment");
				d.CycleAlign(reverse);
				GeometryDecalsPane.UpdateDecalLabels();
				RefreshSelectedGMeshes();
				RefreshGeometry();
			}
		}

		public void CycleDecalMirror()
		{
			Decal d = m_level.GetSelectedDecal(m_decal_active);
			if (d != null) {
				SaveStateForUndo("Decal cycle mirror");
				d.CycleMirror();
				GeometryDecalsPane.UpdateDecalLabels();
				RefreshSelectedGMeshes();
				RefreshGeometry();
			}
		}

		public void CycleDecalClip(int idx, bool reverse)
		{
			Decal d = m_level.GetSelectedDecal(m_decal_active);
			if (d != null) {
				SaveStateForUndo("Decal cycle clip");
				d.CycleClip(idx, reverse);
				GeometryDecalsPane.UpdateDecalLabels();
				RefreshSelectedGMeshes();
				RefreshGeometry();
			}
		}

		public void SetDecalPresetClip(DecalClip dc1, DecalClip dc2, DecalClip dc3, DecalClip dc4)
		{
			Decal d = m_level.GetSelectedDecal(m_decal_active);
			if (d != null) {
				SaveStateForUndo("Decal set all 4 preset");
				d.Set4Clips(dc1, dc2, dc3, dc4);
				GeometryDecalsPane.UpdateDecalLabels();
				RefreshSelectedGMeshes();
				RefreshGeometry();
			}
		}

		public void ResetDecalSettings()
		{
			Decal d = m_level.GetSelectedDecal(m_decal_active);
			if (d != null) {
				SaveStateForUndo("Decal reset repeat/rotation/offset settings");
				d.ResetSettings();
            GeometryDecalsPane.UpdateDecalLabels();
				RefreshSelectedGMeshes();
				RefreshGeometry();
			}
		}

		public void ChangeDecalRepeat(int u, int v)
		{
			Decal d = m_level.GetSelectedDecal(m_decal_active);
			if (d != null) {
				SaveStateForUndo("Decal change repeat");
				d.ChangeRepeat(u, v);
				GeometryDecalsPane.UpdateDecalLabels();
				RefreshSelectedGMeshes();
				RefreshGeometry();
			}
		}

		public void ChangeDecalOffset(int u, int v)
		{
			Decal d = m_level.GetSelectedDecal(m_decal_active);
			if (d != null) {
				SaveStateForUndo("Decal change offset");
				d.ChangeOffset(u, v);
				GeometryDecalsPane.UpdateDecalLabels();
				RefreshSelectedGMeshes();
				RefreshGeometry();
			}
		}

		public void ChangeDecalRotation(int inc)
		{
			Decal d = m_level.GetSelectedDecal(m_decal_active);
			if (d != null) {
				SaveStateForUndo("Decal rotation");
				d.ChangeRotation(inc);
				GeometryDecalsPane.UpdateDecalLabels();
				RefreshSelectedGMeshes();
				RefreshGeometry();
			}
		}

		public void ChangeSideDeformationHeight(int increment)
		{
			Side s = m_level.GetSelectedSide();
			if (s != null) {
				s.deformation_height = Utility.SnapValue(Utility.Clamp(s.deformation_height + increment * 0.05f, 0f, 2f), 0.05f);
				TexturingPane.UpdateTextureLabels();
			}
		}

		public void ChangeSplitPlaneOrder(int increment)
		{
			Side s = m_level.GetSelectedSide();
			if (s != null) {
				s.chunk_plane_order += increment;
				if (s.chunk_plane_order < -1) {
					s.chunk_plane_order = -1;
				}
				//if (false) {
				//	// DEBUG FUNCTION: Set the same split on all marked sides to make chunks segment-sized
				//	List<Side> side_list = m_level.GetMarkedSides(false);
				//	foreach (Side s1 in side_list) {
				//		if (s1.ConnectedSide() != null && s1.ConnectedSide().chunk_plane_order == -1) {
				//			s1.chunk_plane_order = s.chunk_plane_order;
				//		}
				//	}
				//}
				RefreshGeometry();
				TexturingPane.UpdateTextureLabels();
			}
		}

		public void CyclePathfinding(int increment)
		{
			Segment seg = m_level.GetSelectedSegment();
			if (seg != null) {
				seg.m_pathfinding = (PathfindingType)(((int)seg.m_pathfinding + (int)PathfindingType.NUM + increment) % (int)PathfindingType.NUM);
                RefreshGeometry();
				TexturingPane.UpdateTextureLabels();
			}
		}

		public void CycleExitSegment(int increment)
		{
			Segment seg = m_level.GetSelectedSegment();
			if (seg != null) {
				seg.m_exit_segment_type = (ExitSegmentType)(((int)seg.m_exit_segment_type + (int)ExitSegmentType.NUM + increment) % (int)ExitSegmentType.NUM);
				RefreshGeometry();
				TexturingPane.UpdateTextureLabels();
			}
		}

		public void ToggleDark()
		{
			Segment seg = m_level.GetSelectedSegment();
			if (seg != null) {
				seg.m_dark = !seg.m_dark;
				RefreshGeometry();
				TexturingPane.UpdateTextureLabels();
			}
		}

        public void CopySideDeformationHeightToMarked()
		{
			Side s = m_level.GetSelectedSide();
			if (s != null) {
				List<Side> s_list = m_level.GetMarkedSides();

				for (int i = 0; i < s_list.Count; i++) {
					s_list[i].deformation_height = s.deformation_height;
				}
			}
		}

		public void SetSplitPlaneOrder(int order)
		{
			Side s = m_level.GetSelectedSide();
			if (s != null) {
				s.chunk_plane_order = order;
				
				RefreshGeometry();
			}
		}

		public void CopyDecalPropertiesToMarked(int idx)
		{
			Decal d = m_level.GetSelectedDecal(idx);
			if (d != null) {
				SaveStateForUndo("Decal copy properties");
				m_level.CopyDecalPropertiesToMarked(d, idx);
				RefreshMarkedSideGMeshes();
				RefreshGeometry();
			}
		}

		public void ToggleMarkedDecalHidden(int idx)
		{
			//first determine if any are hidden
			bool any_hidden = false;
			foreach (Decal d in m_level.GetMarkedAppliedDecals(idx)) {
				if (d.hidden) {
					any_hidden = true;
					break;
				}
			}

			//Now hide or unhide based on whether any were hidden
			foreach (Decal d in m_level.GetMarkedAppliedDecals(idx)) {
				d.hidden = !any_hidden;
			}

			GeometryDecalsPane.UpdateDecalLabels();
			RefreshMarkedSideGMeshes();
			RefreshGeometry();
		}

		public void ToggleDecalHidden(int idx)
		{
			Decal d = m_level.GetSelectedDecal(idx);
			if (d != null) {
				SaveStateForUndo("Decal hidden toggle");
				d.hidden = !d.hidden;
				GeometryDecalsPane.UpdateDecalLabels();
				RefreshSelectedGMeshes();
				RefreshGeometry();
			}
		}

		public void SnapMarkedElementsToGrid()
		{
			SaveStateForUndo("Snap marked elements to grid");
			m_level.SnapMarkedToGrid();
		}

		public void RotatedMarkedEntities(Axis axis, float dir, bool minor)
		{
			SaveStateForUndo("Rotate marked entites");
			m_level.RotateMarkedEntities(axis, dir * Utility.RAD_45 * (minor ? 0.25f : 1f));
		}

		public void ScaleMarkedElements(GLView view, bool up, bool minor)
		{
			SaveStateForUndo("Scale marked elements");
			float amt = 1f;
			if (up) {
				amt = (minor ? 1.1f : 2f);
			} else {
				amt = (minor ? 1f / 1.1f : 0.5f);
			}
			m_level.ScaleMarked(view, m_scale_mode, amt);
			RefreshGeometry();
		}

		public ScaleMode m_scale_mode = ScaleMode.VIEW_XY;

		public void ScaleMarkedElementsRaw(GLView view, float amtx = 1f, float amty = 1f)
		{
			if (m_scale_mode == ScaleMode.FREE_XY) {
				m_level.ScaleMarked(view, ScaleMode.VIEW_X, amtx);
				m_level.ScaleMarked(view, ScaleMode.VIEW_X, amty);
			} else {
				m_level.ScaleMarked(view, m_scale_mode, amtx);
			}
			RefreshGeometry();
		}

		public void RotatedMarkedElements(GLView view, float amt, bool minor)
		{
			SaveStateForUndo("Rotate marked elements");
			m_level.RotateMarked(view, Utility.RAD_45 * amt * (minor ? 0.125f : 1f));
			RefreshGeometry();
		}

		public void RotateMarkedElementsRaw(GLView view, float amt)
		{
			m_level.RotateMarked(view, amt);
			RefreshGeometry();
		}

		public void MoveMarkedElements(GLView view, Vector3 dir, bool minor)
		{
			SaveStateForUndo("Move marked elements");
			m_level.MoveMarked(view, dir, m_grid_snap * (minor ? 0.25f : 1f));
		}

		public void InsertMarkedSides()
		{
			SaveStateForUndo("Insert segment(s)");
			if (m_level.InsertMarkedSides()) {
				RefreshTaggedSegmentGMeshes();
				RefreshGeometry();
				AddOutputText("Inserted segments on marked sides");
			}
		}

		public void InsertSelectedSide(bool regular)
		{
			SaveStateForUndo("Insert segment");
			if (m_level.InsertSegmentSelectedSide(regular) != -1) {
				RefreshTaggedSegmentGMeshes();
				RefreshGeometry();
				AddOutputText("Inserted new segment");
			}
		}

		public void DeleteMarked()
		{
			SaveStateForUndo("Delete marked");
			m_level.DeleteMarked();
			m_level.CompactLevelSegments();
			RefreshLevelGmesh();
			RefreshGeometry();
		}

		public void ClearAllMarked()
		{
			SaveStateForUndo("Clear all marked", false);
			m_level.ClearAllMarked();
			RefreshGeometry();
		}

		public void ToggleMarkAll()
		{
			SaveStateForUndo("Toggle mark all", false);
			m_level.ToggleMarkAll();
			RefreshGeometry();
		}

		public void ToggleMarkSelected()
		{
			SaveStateForUndo("Toggle mark selected", false);
			m_level.ToggleMarkSelected();
			RefreshGeometry();
		}

		public void SetMarkedRobotsStation(bool station)
		{
			SaveStateForUndo("Marked robots set to station: " + station, false);
			m_level.SetMarkedRobotsStation(station);
			EntityEditPane.UpdateEntityLabels();
			RefreshGeometry();
		}

		public void SetMarkedRobotsNGP(bool ngp)
		{
			SaveStateForUndo("Marked robots set to New Game Plus: " + ngp, false);
			m_level.SetMarkedRobotsNGP(ngp);
			EntityEditPane.UpdateEntityLabels();
			RefreshGeometry();
		}

		public void SelectAdjacentSegment()
		{
			SaveStateForUndo("Select adjacent segment", false);
			m_level.SelectAdjacentSegment();
			RefreshGeometry();
		}

		public void SelectOppositeSegment()
		{
			SaveStateForUndo("Select opposite segment", false);
			m_level.SelectOppositeSegment();
			RefreshGeometry();
		}

		public void CycleSelectedSegment(bool prev)
		{
			SaveStateForUndo("Cycle selected segment", false);
			m_level.CycleSelectedSegment(prev);
			RefreshGeometry();
		}

		public void CycleSelectedSide(bool prev)
		{
			SaveStateForUndo("Cycle selected side", false);
			m_level.CycleSelectedSide(prev);
			RefreshGeometry();
		}

		public void CycleSelectedEntity(bool prev)
		{
			SaveStateForUndo("Cycle selected entity", false);
			m_level.CycleSelectedEntity(prev);
			RefreshGeometry();
		}

		public void SetSelectedEntitySubType(int index)
		{
			SaveStateForUndo("Change entity subtype");
			m_level.SetSelectedEntitySubType(index);
			EntityEditPane.UpdateEntityLabels();
			RefreshGeometry();
		}

		//Returns entity number
		private int DoCreateEntity(int segnum, Vector3 pos, Vector3 dir)
		{
			SaveStateForUndo("Place new entity");

			//Get type & subtype
			EntityType type = (EntityType)EntityEditPane.comboBox_entity_type.SelectedIndex;
			if (type == EntityType.ITEM) {
				dir = Vector3.UnitZ;
			}

			Entity selected_entity = m_level.GetSelectedEntity();
			int subtype = ((selected_entity != null) && (selected_entity.Type == type)) ? selected_entity.SubType : 0;

			//Create the entity
			int entity_num = m_level.CreateEntity(segnum, pos, dir, type, subtype);

			//Leave just this new entity marked
			m_level.MarkAllEntities(false);
			m_level.entity[entity_num].marked = true;

			//Update the windows
			RefreshGeometry();

			//Done
			return entity_num;
		}

		public void CreateEntityInSegment()
		{
			DoCreateEntity(m_level.selected_segment, m_level.GetSelectedSegmentPos(), -m_level.GetSelectedSideNormal());
		}

		public void CreateEntityOnSide()
		{
			Debug.Assert((m_level.GetSelectedSide().Door == -1) || ((EntityType)EntityEditPane.comboBox_entity_type.SelectedIndex != EntityType.DOOR));

			int entity_num = DoCreateEntity(m_level.selected_segment, m_level.GetSelectedSidePos(), m_level.GetSelectedSideNormal());

			//If placing a door, point the side to the door & set the split plane (if not already split)
			if (m_level.entity[entity_num].Type == EntityType.DOOR) {
				m_level.GetSelectedSide().Door = entity_num;
				if (m_level.GetSelectedSide().chunk_plane_order == -1) {
					m_level.GetSelectedSide().chunk_plane_order = 0;
				}
			}

			RefreshGeometry();
		}

		public void EntityResetRotation()
		{
			SaveStateForUndo("Reset entity rotation");
			m_level.EntityResetRotation();
			this.Refresh();
		}

		public int EntityMarkedResetRotation()
		{
			SaveStateForUndo("Reset entity rotation");
			int num = m_level.EntityMarkedResetRotation();
			this.Refresh();
			return num;
		}

		public void EntityFaceSelectedSide()
		{
			SaveStateForUndo("Entity face selected side");
			m_level.EntityFaceSelectedSide();
			this.Refresh();
		}

		public int EntityMarkedFaceSelectedSide()
		{
			SaveStateForUndo("Entity face selected side");
			int num = m_level.EntityMarkedFaceSelectedSide();
			this.Refresh();
			return num;
		}

		public void EntityCycleTeam()
		{
			SaveStateForUndo("Entity face selected side");
			m_level.EntityCycleTeam();
			EntityEditPane.UpdateEntityLabels();
		}

		public int DuplicateMarkedEntities()
		{
			SaveStateForUndo("Duplicate marked entities");
			int num = m_level.DuplicateMarkedEntities();
			RefreshGeometry();
			return num;
		}

		public void EntityMoveToSide()
		{
			Debug.Assert((m_level.GetSelectedSide().Door == -1) || (m_level.GetSelectedEntity().Type != EntityType.DOOR) || (m_level.GetSelectedSide().Door == m_level.GetSelectedEntity().num));
			SaveStateForUndo("Entity move to selected side");
			m_level.EntityMoveToSide();
			EntityEditPane.UpdateEntityLabels();
			this.Refresh();
		}

		public void EntityAlignToSide()
		{
			SaveStateForUndo("Entity align to selected side");
			m_level.EntityAlignToSide();
			this.Refresh();
		}

		public void EntityMoveToSegment()
		{
			SaveStateForUndo("Entity move to selected segment");
			m_level.EntityMoveToSegment();
			EntityEditPane.UpdateEntityLabels();
			this.Refresh();
		}

		public void ApplyTexture(string s, int idx)
		{
			SaveStateForUndo("Apply texture to side(s)");
			m_level.ApplyTexture(s, idx);
			RefreshGeometry();
		}

		public void MarkSidesWithTexture(string s)
		{
			SaveStateForUndo("Mark sides with texture");
			m_level.MarkSidesWithTexture(s);
			RefreshGeometry();
		}

		public string GetSelectedSideTexture()
		{
			return m_level.GetSelectedSideTexture();
		}

		public void MarkSidesWithCavePreset(int idx, bool same_height = false)
		{
			SaveStateForUndo("Mark sides with cave preset");
			m_level.MarkSidesWithCavePreset(idx, same_height);
			RefreshGeometry();
		}

		public int GetSelectedSideCavePreset()
		{
			return m_level.GetSelectedSideCavePreset();
		}

		public void SetSideCavePreset(int idx)
		{
			SaveStateForUndo("Set cave preset");
			m_level.SetSideCavePreset(idx);
			RefreshGeometry();
		}

		public void ApplyDecal(DMesh dm, int idx)
		{
			SaveStateForUndo("Apply decal to side(s)");

			List<Side> side_list = m_level.GetMarkedSides();

			//Apply to selected if no marked
			if (side_list.Count == 0) {
				Side side = m_level.GetSelectedSide();
				if (side != null) { 
					m_level.ApplyDecal(side, dm, idx);
					RefreshSelectedGMeshes();
				}
			} else {
				//Apply to all marked
				foreach (Side side in side_list) {
					m_level.ApplyDecal(side, dm, idx);
				}
				RefreshMarkedSideGMeshes();
			}

			RefreshGeometry();
		}

		public Guid m_entity_prev_guid = Guid.Empty;

		public void ELLFeedback(object sender, EntityLinkLabelArgs e)
		{
			Entity entity = m_level.GetSelectedEntity();
			if (entity != null) {
				switch (e.Button) {
					case MouseButtons.Left:
						if (e.Shift) {
							entity.ClearLink(e.Index);
							m_entity_prev_guid = Guid.Empty;
						} else {
							List<Entity> e_list = m_level.GetMarkedEntities();
							if (e_list.Count > 0) {
								entity.AssignLink(e.Index, e_list[0].guid);
								e_list[0].marked = false;
								m_entity_prev_guid = Guid.Empty;
							}
						}
						break;
					case MouseButtons.Right:
					case MouseButtons.Middle:
						Guid guid = entity.GetLinkGUID(e.Index);
						Entity linked_e = m_level.FindEntityWithGUID(guid);
						if (linked_e != null) {
							if (e.Button == MouseButtons.Right) {
								linked_e.marked = true;
							} else {
								m_level.selected_entity = linked_e.num;
							}
						}
						break;
				}
			}

			RefreshGeometry();
		}

		public string EntityGUIDString(Guid guid)
		{
			if (guid == Guid.Empty) {
				return " - ";
			}

			Entity e = m_level.FindEntityWithGUID(guid);
			if (e != null) {
				return string.Format("{0} - {1}", e.Type, guid.ToPrettyString());
			}

			return "<missing>";
		}

		public void UpdateCountLabels()
		{
			var level = m_level;
			label_count_total.Text = "Total: " + level.num_segments.ToString() + "/" + level.num_sides.ToString() + "/" + level.num_vertices.ToString();
			label_count_marked.Text = "Marked: " + level.num_marked_segments.ToString() + "/" + level.num_marked_sides.ToString() + "/" + level.num_marked_vertices.ToString();
			label_count_selected.Text = "Selected: " + (level.selected_segment < 0 ? "--" : level.selected_segment.ToString()) + "/" +
				(level.selected_side < 0 ? "--" : level.selected_side.ToString()) + "/" +
				(level.selected_vertex < 0 ? "--" : level.selected_vertex.ToString());
		}

		public void AddOutputText(string s)
		{
			OutputPane.AddText(s);
		}

		bool NewOrLoadLevelHelper(Func<Tuple<Level, string>> acquire_level)
		{
			try {
				var level_path_tuple = acquire_level();
				if (level_path_tuple == null || level_path_tuple.Item1 == null) {
					return false;
				}

				this.m_level = level_path_tuple.Item1;
				string path_to_file = level_path_tuple.Item2;

				Shell.CurrentLevelFilePath = path_to_file;

				m_level.GetTextureSet(false);      //This will set to a valid set if the default is invalid

				gl_panel.DeleteGMesh();
				EntityListPane.Populate();
				LevelGlobalPane.UpdateLabels();
				LevelCustomInfoPane.Populate();

				if (string.IsNullOrWhiteSpace(path_to_file)) {
					this.Text = "<New Level>";
					AddOutputText("Created new level");
				} else {
					this.Text = Path.GetFileName(path_to_file);
					AddOutputText(string.Format("Loaded level: {0}", path_to_file));
					AddRecentFile(path_to_file);
				}

				return true;
			}
			catch (Exception ex) {
				MessageBox.Show(string.Format("Error initializing level: {0}", ex.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return false;
			}
		}

		public void NewLevel()
		{
			NewOrLoadLevelHelper(() => new Tuple<Level, string>(new Level(this), null));
		}

		public bool LoadLevel(string path_to_file)
		{
			return NewOrLoadLevelHelper(() => {
				var fileData = System.IO.File.ReadAllText(path_to_file);
				var root = JObject.Parse(fileData);
				var newLevel = new Level(this);
				newLevel.Deserialize(root);
				newLevel.dirty = false;

				//Look for entites without segment & entites in segments that are not alive
				string bad_segnums = "";
				foreach (Entity entity in newLevel.EnumerateAliveEntities()) {
					if ((entity.m_segnum == -1) || (!newLevel.segment[entity.m_segnum].Alive)) {
						entity.m_segnum = newLevel.FindSegmentForPoint(entity.position);

						if (entity.m_segnum == -1) {
							bad_segnums += entity.num + " ";
						}
					}
				}
				if (bad_segnums != "") {
					AddOutputText("These entities are not in a segment " + bad_segnums);
				}

				// Compact the segment array down to remove dead segment slots
				newLevel.CompactLevelSegments();

				return new Tuple<Level, string>(newLevel, path_to_file);
			});
		}

		public bool SaveLevel(string path_to_file, bool silent = false)
		{
			try {
				// Compact the segment array down to remove dead segment slots
				this.m_level.CompactLevelSegments();

				var root = new JObject();
				this.m_level.Serialize(root);

				var fileData = root.ToString(Formatting.Indented);
				System.IO.File.WriteAllText(path_to_file, fileData);

				if (!silent) {
					Shell.CurrentLevelFilePath = path_to_file;
					this.Text = Path.GetFileName(path_to_file);

					AddOutputText(string.Format("Saved level: {0}", path_to_file));

					AddRecentFile(path_to_file);
					m_level.dirty = false;
				}

				return true;
			}
			catch (Exception ex) {
				if (!silent) {
					MessageBox.Show(string.Format("Failed to save level: {0}", ex.Message));
				}
				return false;
			}
		}

		public bool ExportLevel(string inputLevelPath, string outputPath, OverloadLevelExport.LevelType exportLevelType)
		{
			try {
				try {
					UnityEngine.Debug.LogCallback = (int level, string msg) => {
						string tag = "INFO";
						switch (level) {
							case 1:
								tag = "WARN";
								break;
							case 2:
								tag = "ERROR";
								break;
						}
						AddOutputText(string.Format("{0}: {1}", tag, msg));
					};

					using (var serializer = new OverloadLevelConvertSerializer(outputPath, true)) {
						// Serializer header
						OverloadLevelConverter.WriteSerializationLevelHeader(serializer, (uint)exportLevelType);

						using (OverloadLevelExport.SceneBroker sceneBroker = new OverloadLevelExport.SceneBroker(serializer, exportLevelType)) {
							serializer.Context = sceneBroker;

							OverloadLevelExport.SceneBroker.ActiveSceneBrokerInstance = sceneBroker;
							try {
								OverloadLevelConverter.ConvertLevel(inputLevelPath, Path.GetFileNameWithoutExtension(outputPath), this.m_filepath_root, sceneBroker);

								// Last packet is the 'Done' packet
								var doneCmd = new OverloadLevelExport.CmdDone();
								doneCmd.Serialize(serializer);
							} finally {
								serializer.Context = null;
								OverloadLevelExport.SceneBroker.ActiveSceneBrokerInstance = null;
							}
						}
					}
				} finally {
					UnityEngine.Debug.LogCallback = null;
					GC.Collect();
				}

				return true;
			} catch (Exception ex) {
				MessageBox.Show(string.Format("Failed to export level: {0}", ex.Message));
				return false;
			}
		}

		public void ExportSimpleMission(string output_path, string output_name, string level_name)
		{
			if (output_name != null && output_name != "" && output_path != null && output_path != "") {
				output_name = Path.Combine(output_path, output_name);
				if (File.Exists(output_name)) {
					AddOutputText("The mission file already exists: " + output_name);
					return;
				}
				TextWriter w = new StreamWriter(output_name);

				w.WriteLine("# Default Mission File for custom mission (" + level_name + ")");
				w.WriteLine("display_name: # MISSION NAME");
				string visible_name = level_name.Replace('_', ' ').ToUpper();
            w.WriteLine(" EN: " + visible_name);
				w.WriteLine(" ES: " + visible_name);
				w.WriteLine(" FR: " + visible_name);
				w.WriteLine(" DE: " + visible_name);
				w.WriteLine(" RU: " + visible_name);

				w.WriteLine("levels:");
				w.WriteLine(" - name: # LEVEL NAME");
				w.WriteLine("    EN: " + visible_name);
				w.WriteLine("    ES: " + visible_name);
				w.WriteLine("    FR: " + visible_name);
				w.WriteLine("    DE: " + visible_name);
				w.WriteLine("    RU: " + visible_name);
				w.WriteLine("   scene: " + level_name);
				w.WriteLine("   music: outer_01");
				w.WriteLine("   # BRIEFING? weapons: [impulse]");
				w.WriteLine("   # BRIEFING? missiles: [falcon, missile_pod]");
				w.WriteLine("   # BRIEFING?  robots: [grunta, gruntb, recoila]");
				w.WriteLine("   upgrade_points: 0");
				w.WriteLine("   super_upgrade_points: 0");
				w.WriteLine("   ammo: 0");
				w.WriteLine("   loadout:");
				w.WriteLine("    impulse: 0 # Number is [unlock level]");
				w.WriteLine("    falcon: [0, 10] # Numbers are [unlock level, ammo]");
				w.WriteLine("# ADDITIONAL LEVELS GO HERE");

				w.Close();

				AddOutputText("Created a default mission file for: " + level_name);
			} else {
				AddOutputText("Could not find the directory or filename to create a mission file");
			}
		}

		public void GenerateCMData(string output_path, string output_name, string level_name)
		{
			if (output_name != null && output_name != "" && output_path != null && output_path != "") {
				output_name = Path.Combine(output_path, output_name);
				if (File.Exists(output_name)) {
					AddOutputText("The CM data file already exists: " + output_name);
					return;
				}
				TextWriter w = new StreamWriter(output_name);

				w.WriteLine("CHALLENGE_DATA");
				w.WriteLine("// Challenge mode data for custom level (" + level_name + ")");
				w.WriteLine("");
				w.WriteLine("$desc_english CUSTOM LEVEL DESCRIPTION");
				w.WriteLine("$desc_spanish CUSTOM LEVEL DESCRIPTION");
				w.WriteLine("$desc_french CUSTOM LEVEL DESCRIPTION");
				w.WriteLine("$desc_german CUSTOM LEVEL DESCRIPTION");
				w.WriteLine("$desc_russian CUSTOM LEVEL DESCRIPTION");
				w.WriteLine("");

				w.WriteLine("$music; outer_01   // outer_01 - 05, titan_06 - 10, inner_11 - 12, alien_13 - 16, outer/titan/alien_boss");
				w.WriteLine("// More or less bots (additive) in countdown/infinite for this level (0 is default, can go higher than 1 for very large levels)");
				w.WriteLine("$robot_count_booster_infinite; 0.0");
				w.WriteLine("$robot_count_booster_countdown; 0.0");
				w.WriteLine("");

				w.WriteLine("$group1_robots; 2	// Up to 5 groups allowed, with any number of robots per group");
				w.WriteLine("$robot; hulkb; 0.25; 0.1  // First number is the relative frequency a certain robot type will spawn (overall, not at any given time)");
				w.WriteLine("$robot; hulka; 0.25; 0.1  // Second number is a modifier on how hard robot is considered for the level (higher = harder, 0 = default)");
				w.WriteLine("$robot; vipera; 0.25; 0.1   // Some robot types have additonal restrictions in code that cannot be altered (max count at once, for instance)");
				w.WriteLine("");

				w.WriteLine("$group2_robots; 2");
				w.WriteLine("$robot; cannonb; 0.5; 0");
				w.WriteLine("$robot; recoilb; 0.6; 0");
				w.WriteLine("$robot; gruntb; 0.5; 0");
				w.WriteLine("$robot; droneb; 0.3; 0");
				w.WriteLine("");

				w.WriteLine("$group3_robots; 2");
				w.WriteLine("$robot; bladesa; 0.3; 0");
				w.WriteLine("$robot; detonatora; 0.3; 0");
				w.WriteLine("$robot; clawbota; 0.3; 0");
				w.WriteLine("");

				w.WriteLine("$super_robots; 3 // Number of super robot types and their appearance rate");
				w.WriteLine("$super_robot; hulka; 0.8");
				w.WriteLine("$super_robot; cannonb; 1.0");
				w.WriteLine("$super_robot; recoila; 1.0");
				w.WriteLine("$super_robot; gruntb; 1.0");
				w.WriteLine("$super_robot; cannona; 1.0");
				w.WriteLine("$super_robot; grunta; 1.0");
				w.WriteLine("");

				w.WriteLine("$robot_variant; recoila; 0.5 // Chance a certain robot type will be a variant (during a playthrough).  Only 7 bots have variants");
				w.WriteLine("$robot_variant; recoilb; 0.5");
				w.WriteLine("$robot_variant; hulka; 0.5");
				w.WriteLine("$robot_variant; dronea; 0.5");
				w.WriteLine("$robot_variant; droneb; 0.5");
				w.WriteLine("$robot_variant; grunta; 0.5");
				w.WriteLine("$robot_variant; gruntb; 0.5");
				w.WriteLine("");

				w.WriteLine("CHALLENGE_DATA_END");
				w.Close();

				AddOutputText("Created default CM data file for: " + level_name);
			} else {
				AddOutputText("Could not find the directory or filename to create a CM data file");
			}
		}

		public DMesh GetDMeshByName(string name)
		{
			foreach (DMesh dmesh in decal_list.m_dmesh) {
				if (dmesh.name == name) {
					return dmesh;
				}
			}

			return null;
		}

		public void AddRecentFile(string filename)
		{
			if (MaybeRecentFilesSwap(filename)) {
				// Already swapped inside MaybeRecentFilesSwap
			} else {
				for (int i = NumRecentFiles - 1; i > 0; i--) {
					SetRecentFile(i, GetRecentFile(i - 1));
				}
				SetRecentFile(0, filename);
			}

			Shell.UpdateRecentFileMenu();
		}

		public bool MaybeRecentFilesSwap(string filename)
		{
			for (int i = 0; i < NumRecentFiles; i++) {
				if (GetRecentFile(i) != filename) {
					continue;
				}

				// Swap with the first item if it's in the list
				string s = GetRecentFile(0);
				SetRecentFile(0, filename);
				SetRecentFile(i, s);

				return true;
			}

			return false;
		}

		//Returns the number of errors found
		public int CheckLevelValidity(bool mark_bad_segements = false)
		{
			int error_count = 0;

			if (mark_bad_segements) {
				ClearAllMarked();
			}

			//Check for the right number of verts in segment sides (not sure exactly what error this is checking for -Matt)
			foreach (Segment seg in m_level.EnumerateAliveSegments()) {
				int old_error_count = error_count;

				List<int> seg_verts = new List<int>();
				int sv_count = 0;
				for (int j = 0; j < Segment.NUM_SIDES; j++) {
					for (int k = 0; k < Side.NUM_VERTS; k++) {
						if (!seg_verts.Contains(seg.side[j].vert[k])) {
							seg_verts.Add(seg.side[j].vert[k]);
							sv_count += 1;
						}

						if (seg.side[j].FindAdjacentSideSameSegment(k) == null) {
							AddOutputText("Could not find the adjacent side for a segment " + seg.num + " side " + j);
							error_count++;
						}
					}
				}
				if (sv_count != 8) {
					AddOutputText("A segment had the wrong number of verts within the sides (" + sv_count + "), segment #: " + seg.num);
					error_count++;
				}

				//Check for two verts in a segment that are very close together
				bool overlapping = false;
				for (int h = 0; h < Segment.NUM_VERTS; h++) {
					for (int g = h + 1; g < Segment.NUM_VERTS; g++) {
						Vector3 diff = (m_level.vertex[seg.vert[h]].position - m_level.vertex[seg.vert[g]].position);
						if (diff.LengthSquared < 0.0001f) {
							overlapping = true;
						}
					}
				}
				if (overlapping) {
					AddOutputText("Two verts are overlapping in segment #: " + seg.num);
					error_count++;
				}

				//Check for degenerate faces
				if (SegmentDegenerateFacesNear(seg)) {
					error_count++;
				}

				//Check for reciprocal connections
				for (int sidenum = 0; sidenum < 6; sidenum++) {
					int neighbor = seg.neighbor[sidenum];
					if (neighbor > -1) {
						try {
							m_level.segment[neighbor].FindConnectingSide(seg.num);
						}
						catch {
							AddOutputText("Segment " + seg.num + " side " + sidenum + " connects to segment " + neighbor + " but that segment doesn't connect back.");
							error_count++;
						}
					}
				}

				//Mark this segment if errors found
				if (mark_bad_segements && (error_count != old_error_count)) {
					seg.marked = true;
				}
			}

			//Check for missing decals
			foreach (Segment segment in m_level.EnumerateAliveSegments()) {
				foreach (Side side in segment.side) {
					foreach (Decal decal in side.decal) { 
						if (!string.IsNullOrEmpty(decal.mesh_name) && !decal.hidden) {
							if (side.level.editor.GetDMeshByName(decal.mesh_name) == null) {
								AddOutputText("Segment " + segment.num + " Side " + side.num + " specifies invalid decal '" + decal.mesh_name + "'.");
								error_count++;
								if (mark_bad_segements) {
									segment.marked = side.marked = true;
								}
							}
						}
					}
				}
			}

			//Clear tag for all doors
			foreach (Entity door in m_level.EnumerateAliveEntities(EntityType.DOOR)) {
				door.tag = false;
			}

			//Make sure segment -> door connections are right
			foreach (Segment segment in m_level.EnumerateAliveSegments()) {
				int old_error_count = error_count;

				foreach (Side side in segment.side) {

					//Check doors
					if (side.Door != -1) {

						Entity door = m_level.entity[side.Door];

						//Make sure the entity pointed to is a valid door
						if (!door.alive) {
							AddOutputText("Segment " + segment.num + " Side " + side.num + " specifies entity " + door.num + " as a door, but that entity is not alive.");
							error_count++;
						} else if (door.Type != EntityType.DOOR) {
							AddOutputText("Segment " + segment.num + " Side " + side.num + " specifies entity " + door.num + " as a door, but it not.");
							if (mark_bad_segements) {
								door.marked = true;
							}
							error_count++;
						} else {

							//Flag this door as having been found
							door.tag = true;

							//Make sure door and segment point at each other
							if (door.m_segnum != segment.num) {
								AddOutputText("Segment " + segment.num + " Side " + side.num + " contains door " + door.num + " but door says it's in segment " + door.m_segnum);
								if (mark_bad_segements) {
									door.marked = true;
								}
								error_count++;
							}

							//Make sure the door is properly located
							//AddOutputText("Door distance from center: " + (door.position - side.FindCenter()).Length);
							if ((door.position - side.FindCenter()).Length > 0.1f) {              //Need to check this constant
								AddOutputText("Door (entity " + door.num + ") is not at center of side (segment = " + segment.num + ", side = " + side.num + ").");
								if (mark_bad_segements) {
									door.marked = true;
								}
								error_count++;
							}

							//Make sure there's a connection on the other side of this door
							if (segment.neighbor[side.num] == -1) {
								AddOutputText("Door (entity " + door.num + " in segment " + segment.num + ") does not connect to another segment.");
								error_count++;
							}

							//Make sure the side has a separation plane
							if (side.chunk_plane_order == -1) {
								AddOutputText("Segment " + segment.num + " side " + side.num + " contains a door but has its sort order set to OFF.");
								error_count++;
							}
						}
					}
				}

				//Mark segment if errors found
				if (mark_bad_segements && (error_count != old_error_count)) {
					segment.marked = true;
				}
			}

			//Make sure we found a segment side for each door
			foreach (Entity door in m_level.EnumerateAliveEntities(EntityType.DOOR)) {
				if (!door.tag) { 
					AddOutputText("Door " + door.num + ": There is no segment/side that owns this door.  The door says it belongs to segment " + door.m_segnum + ".");
					if (mark_bad_segements) {
						door.marked = true;
					}
					error_count++;
				}
			}

			//Make sure each entity (except scripts & doors) is in the segment it thinks it's in
			foreach (Entity entity in m_level.EnumerateAliveEntities()) {
				if ((entity.Type != EntityType.SCRIPT) && (entity.Type != EntityType.DOOR)) {
					if (!entity.InSegment()) {
						if (entity.m_segnum == -1) {
							AddOutputText("Entity " + entity.num + " is marked as being in segment -1");
						} else {
							AddOutputText("Entity " + entity.num + " thinks it's in segment " + entity.m_segnum + " but it is not.");
						}
						if (mark_bad_segements) {
							entity.marked = true;
						}
						error_count++;
					}
				}
			}

			//Update display if we've marked bad segments
			if (mark_bad_segements && (error_count > 0)) {
				RefreshGeometry();
			}

			AddOutputText("Done testing validity");

			return error_count;
		}

		public void RemoveInvalidDoorReferences()
		{
			int count = 0;
			foreach (Segment segment in m_level.EnumerateAliveSegments()) {
				foreach (Side side in segment.side) {

					if (side.Door != -1) {

						Entity door = m_level.entity[side.Door];

						//Make sure the entity pointed to is a valid door
						if (!door.alive) {
							count++;
							side.Door = -1;
						} else if (door.Type != EntityType.DOOR) {
							count++;
							side.Door = -1;
						}
					}
				}
			}

			AddOutputText("Removed " + count + " invalid door references");
		}

		public void MakeAllDoorsBeSplitPlanes(int order = 5)
		{
			foreach (Segment segment in m_level.EnumerateAliveSegments()) {
				foreach (Side side in segment.side) {
					if (side.Door != -1) {
						if (side.chunk_plane_order == -1) {
							side.chunk_plane_order = 5;
						}
					}
				}
			}
		}

		public void ClearSplitPlanes()
		{
			foreach (Segment segment in m_level.EnumerateAliveSegments()) {
				foreach (Side side in segment.side) {
					side.chunk_plane_order = -1;
				}
			}
		}

		//Find all segments that are too small for a robot to pass through, and mark it as no pathfinding
		public void FindImpassibleSegments()
		{
			string impassible_segments = "";

			foreach (Segment segment in m_level.EnumerateAliveSegments()) {
				Vector3 segment_center = segment.FindCenter();

				bool impassible = false;

				//Check for a solid wall too close to the center
				foreach (Side side in segment.side) {
					Vector3 side_center = side.FindCenter();

					if (!side.HasNeighbor) {                        //If side is solid, check that it's not too close to the segment center
						if (Vector3.Subtract(segment_center, side_center).LengthSquared < 0.64f) {
							impassible = true;
						}
					} 
					//else {         //If side is open, check that the opening is big enough
					//	for (int edge_num = 0; edge_num < 4; edge_num++) {
					//		Side adjacent_side = side.FindAdjacentSideOtherSegment(edge_num);
					//		if (!adjacent_side.HasNeighbor) {
					//			Vector3 edge_center = side.FindEdgeCenter(edge_num);
					//			if (Vector3.Subtract(side_center, edge_center).LengthSquared < 0.64f) {
					//				impassible = true;
					//				break;   //break out of edge loop
					//			}
					//		}
					//	}
					//}

					if (impassible) {
                        segment.m_pathfinding = PathfindingType.None;
						impassible_segments += segment.num + ",";
						break;	//break of out of side loop
					}
				}
			}

			if (impassible_segments == "") {
				AddOutputText("No impassible segments found.");
			} else {
				AddOutputText("Marked segments as impassible: " + impassible_segments.Substring(0, impassible_segments.Length - 1));
			}
		}

		// Mark all impassible segments (GB or No PF)
		public void MarkImpassibleSegments()
		{
			bool impassible;
			m_level.ClearAllMarked();
			SetEditModeSilent(EditMode.SEGMENT);

			foreach (Segment segment in m_level.EnumerateAliveSegments()) {
				impassible = ((segment.m_pathfinding == PathfindingType.None) || (segment.m_pathfinding == PathfindingType.GB));
				if (impassible) {
					segment.marked = true;
				}
			}

			RefreshGeometry();
		}

		public void AllRobotsDropPowerups()
		{
			int count_switch = 0;
			int count_total = 0;

			foreach (Entity entity in m_level.EnumerateAliveEntities()) {
				if (entity.Type == EntityType.ENEMY) {
					if (((Overload.EntityPropsRobot)entity.entity_props).replace_default_drop) {
						((Overload.EntityPropsRobot)entity.entity_props).replace_default_drop = false;
						count_switch += 1;
					}
					count_total += 1;
				}
			}

			AddOutputText("Switched this many robots to drop powerups: " + count_switch + " out of " + count_total);
			RefreshGeometry();
		}

		public void CopyEntityStatsClipboard()
		{
			string output_text = "";

			output_text += "--Enemies--\n";
			int[] count_normal = new int[(int)Overload.EnemyType.NUM];
			int[] count_variant = new int[(int)Overload.EnemyType.NUM];
			int[] count_super = new int[(int)Overload.EnemyType.NUM];
			int count_ngp = 0;
			int count_station = 0;
			int count_dropped = 0;
			foreach (Entity entity in m_level.EnumerateAliveEntities()) {
				if (entity.Type == EntityType.ENEMY) {
					if (((Overload.EntityPropsRobot)entity.entity_props).super) {
						count_super[entity.SubType] += 1;
					} else if (((Overload.EntityPropsRobot)entity.entity_props).variant) {
						count_variant[entity.SubType] += 1;
					} else {
						count_normal[entity.SubType] += 1;
					}
					if (((Overload.EntityPropsRobot)entity.entity_props).bonus_drop1 == Overload.ItemPrefab.entity_item_log_entry) {
						count_dropped += 1;
               }
					if (((Overload.EntityPropsRobot)entity.entity_props).ng_plus) {
						count_ngp += 1;
					}
					if (((Overload.EntityPropsRobot)entity.entity_props).station) {
						count_station += 1;
					}
				}
			}
			for (int i = 0; i < count_normal.Length; i++) {
				if (count_normal[i] > 0) {
					output_text += " " + ((Overload.EnemyType)i).ToString() + " NORMAL\t" + count_normal[i] + "\n";
				}
				if (count_variant[i] > 0) {
					output_text += " " + ((Overload.EnemyType)i).ToString() + " VARIANT\t" + count_variant[i] + "\n";
				}
				if (count_super[i] > 0) {
					output_text += " " + ((Overload.EnemyType)i).ToString() + " SUPER\t" + count_super[i] + "\n";
				}
			}
			output_text += " DROPPED LOG ENTRY\t" + count_dropped + "\n";
			output_text += " STATION BOTS\t" + count_station + "\n";
			output_text += " NG+ BOTS\t" + count_ngp + "\n";

			output_text += "\n";
			output_text += "--Items--\n";
			int[] count_items = new int[(int)ItemSubType.NUM];
			int[] count_super_items = new int[(int)ItemSubType.NUM];
			int count_secrets = 0;
			foreach (Entity entity in m_level.EnumerateAliveEntities()) {
				if (entity.Type == EntityType.ITEM) {
					if (((Overload.EntityPropsItem)entity.entity_props).secret) {
						count_secrets += 1;
					}
					if (((Overload.EntityPropsItem)entity.entity_props).super) {
						count_super_items[entity.SubType] += 1;
					} else {
						count_items[entity.SubType] += 1;
					}
				}
			}
			for (int i = 0; i < count_items.Length; i++) {
				if (count_items[i] > 0) {
					output_text += " " + ((ItemSubType)i).ToString() + "\t" + count_items[i] + "\n";
				}
			}
			for (int i = 0; i < count_items.Length; i++) {
				if (count_super_items[i] > 0) {
					output_text += " " + ((ItemSubType)i).ToString() + " SUPER\t" + count_super_items[i] + "\n";
				}
			}
			output_text += " SECRET ITEMS\t" + count_secrets + "\n";

			output_text += "\n";
			output_text += "--Props--\n";
			int[] count_props = new int[(int)PropSubType.NUM];
			foreach (Entity entity in m_level.EnumerateAliveEntities()) {
				if (entity.Type == EntityType.PROP) {
					count_props[entity.SubType] += 1;
				}
			}
			for (int i = 0; i < count_props.Length; i++) {
				if (count_props[i] > 0) {
					output_text += " " + ((PropSubType)i).ToString() + "\t" + count_props[i] + "\n";
				}
			}

			output_text += "\n";
			output_text += "--Scripts--\n";
			int[] count_scripts = new int[(int)ScriptSubType.NUM];
			foreach (Entity entity in m_level.EnumerateAliveEntities()) {
				if (entity.Type == EntityType.SCRIPT) {
					count_scripts[entity.SubType] += 1;
				}
			}
			for (int i = 0; i < count_scripts.Length; i++) {
				if (count_scripts[i] > 0) {
					output_text += " " + ((ScriptSubType)i).ToString() + "\t" + count_scripts[i] + "\n";
				}
			}

			output_text += "\n";
			output_text += "--Matcens--\n";
			int count_matcens = 0;
			int count_shielded = 0;
			foreach (Entity entity in m_level.EnumerateAliveEntities()) {
				if (entity.Type == EntityType.SPECIAL) {
					Overload.EntityPropsSpecial eps = (Overload.EntityPropsSpecial)entity.entity_props;
					if (entity.SubType == (int)SpecialSubType.MATCEN) {
						count_matcens += 1;
						if (eps.Invulnerable) {
							count_shielded += 1;
						}
					}
				}
			}
			output_text += " TOTAL MATCENS\t" + count_matcens + "\n";
			output_text += " TOTAL SHIELDED\t" + count_shielded + "\n";

			Clipboard.SetText(output_text);
		}

		public void CopyTextureStatsToClipboard()
		{
			string output_text = "";
			
			// Keep a list and count of all textures in the level
			List<string> tex_names = new List<string>();
			List<int> tex_count = new List<int>();

			
			// sides first
			foreach (Segment seg in m_level.EnumerateAliveSegments()) {
				for (int i = 0; i < 6; i++) {
					if (!seg.side[i].HasNeighbor) {
						AddToListPair(tex_names, tex_count, seg.side[i].tex_name);
					}
				}
			}

			int decal_start = tex_names.Count;
			
			// decals second
			foreach (Segment seg in m_level.EnumerateAliveSegments()) {
				for (int i = 0; i < 6; i++) {
					if (!seg.side[i].decal[0].hidden && seg.side[i].decal[0].dmesh != null) {
						for (int j = 0; j < seg.side[i].decal[0].dmesh.tex_name.Count; j++) {
							bool used = false;
							for (int k = 0; k < seg.side[i].decal[0].dmesh.triangle.Count; k++) {
								if (seg.side[i].decal[0].dmesh.triangle[k].tex_index == j) {
									used = true;
									break;
								}
							}
							if (used) {
								AddToListPair(tex_names, tex_count, Utility.GetPathlessFilename(seg.side[i].decal[0].dmesh.tex_name[j]));
							}
						}
					}
					if (!seg.side[i].decal[1].hidden && seg.side[i].decal[1].dmesh != null) {
						for (int j = 0; j < seg.side[i].decal[1].dmesh.tex_name.Count; j++) {
							bool used = false;
							for (int k = 0; k < seg.side[i].decal[1].dmesh.triangle.Count; k++) {
								if (seg.side[i].decal[1].dmesh.triangle[k].tex_index == j) {
									used = true;
									break;
								}
							}
							if (used) {
								AddToListPair(tex_names, tex_count, Utility.GetPathlessFilename(seg.side[i].decal[1].dmesh.tex_name[j]));
							}
						}
					}
				}
			}

			for (int i = 0; i < tex_names.Count; i++) {
				if (i == 0) {
					output_text += "--LEVEL TEXTURES--\n";
				} else if (i == decal_start) {
					output_text += "\n";
					output_text += "--DECAL-SPECIFIC TEXTURES--\n";
				}
				output_text += tex_names[i] + "\t" + tex_count[i].ToString() + "\n";
			}

			output_text += "\n";
			output_text += "--DECALS--\n";

			List<string> decal_names = new List<string>();
			List<int> decal_count = new List<int>();

			foreach (Segment seg in m_level.EnumerateAliveSegments()) {
				for (int i = 0; i < 6; i++) {
					if (!seg.side[i].decal[0].hidden && seg.side[i].decal[0].dmesh != null) {
						AddToListPair(decal_names, decal_count, (seg.side[i].decal[0].dmesh.name));
					}
					if (!seg.side[i].decal[1].hidden && seg.side[i].decal[1].dmesh != null) {
						AddToListPair(decal_names, decal_count, (seg.side[i].decal[1].dmesh.name));
					}
				}
			}

			for (int i = 0; i < decal_names.Count; i++) {
				output_text += decal_names[i] + "\t" + decal_count[i].ToString() + "\n";
			}

			Clipboard.SetText(output_text);
		}

		public void AddToListPair(List<string> name_list, List<int> count_list, string name)
		{
			if (name_list.Contains(name)) {
				int idx = name_list.IndexOf(name);
				count_list[idx] += 1;
			} else {
				name_list.Add(name);
				count_list.Add(1);
			}
		}


		//void DegenTest(Vector3 norm1,Vector3 norm2, float threshold, int segnum, ref bool degenerate)
		//{
		//	if (Math.Abs(Vector3.Dot(norm1, norm2)) > threshold) {
		//		degenerate = true;

		//		AddOutputText("Segment " + segnum + " is degenerate, threshold = " + Vector3.Dot(norm1, norm2));
		//	}

		//}

		// Check for degenerate segment due to:
		//	 - Given a triangle and the four vertices on the opposite side, two verticies lie on opposite sides of the triangle's plane
		//	 - Two sides are very close together.
		//	 - Adjacent sides have nearly identical normals.  Specifically: If sides A and B are adjacent and either triangle in A has a nearly identical normal to either triangle in B.
		// Print errors and return true if any of the above occur.
		public bool SegmentDegenerateFacesNear(Segment segdata)
		{
			Vector3 a, b, c;
			float[] distances = new float[4];
			float distance_sum = 0.0f;
			int other_side_index;

			//// First, check for nearly identical normals on adjacent sides.
			//bool degenerate = false;
			//for (int sidenum = 0; sidenum < 6; sidenum++) {
			//	for (int side2 = 0; side2 < 6; side2++) {

			//		if (sidenum == side2)
			//			continue;

			//		if (sidenum == Utility.OppositeSide(side2))
			//			continue;

			//		Vector3 norm1, norm2, norm3, norm4;
			//		Vector3 edge1, edge2, edge3;

			//		edge1 = (m_level.vertex[segdata.vert[Segment.SegmentSideVerts[sidenum, 1]]].position - m_level.vertex[segdata.vert[Segment.SegmentSideVerts[sidenum, 0]]].position).Normalized();
			//		edge2 = (m_level.vertex[segdata.vert[Segment.SegmentSideVerts[sidenum, 2]]].position - m_level.vertex[segdata.vert[Segment.SegmentSideVerts[sidenum, 0]]].position).Normalized();
			//		edge3 = (m_level.vertex[segdata.vert[Segment.SegmentSideVerts[sidenum, 3]]].position - m_level.vertex[segdata.vert[Segment.SegmentSideVerts[sidenum, 0]]].position).Normalized();

			//		norm1 = Vector3.Cross(edge1, edge2).Normalized();
			//		norm2 = Vector3.Cross(edge2, edge3).Normalized();

			//		edge1 = (m_level.vertex[segdata.vert[Segment.SegmentSideVerts[side2, 1]]].position - m_level.vertex[segdata.vert[Segment.SegmentSideVerts[side2, 0]]].position).Normalized();
			//		edge2 = (m_level.vertex[segdata.vert[Segment.SegmentSideVerts[side2, 2]]].position - m_level.vertex[segdata.vert[Segment.SegmentSideVerts[side2, 0]]].position).Normalized();
			//		edge3 = (m_level.vertex[segdata.vert[Segment.SegmentSideVerts[side2, 3]]].position - m_level.vertex[segdata.vert[Segment.SegmentSideVerts[side2, 0]]].position).Normalized();

			//		norm3 = Vector3.Cross(edge1, edge2).Normalized();
			//		norm4 = Vector3.Cross(edge2, edge3).Normalized();

			//		float threshold = 0.995f;
			//		//DegenTest(norm1, norm3, threshold, segdata.num, ref degenerate);
			//		//DegenTest(norm1, norm4, threshold, segdata.num, ref degenerate);
			//		//DegenTest(norm2, norm3, threshold, segdata.num, ref degenerate);
			//		//DegenTest(norm2, norm4, threshold, segdata.num, ref degenerate);

			//		if (Math.Abs(Vector3.Dot(norm1, norm3)) > threshold) degenerate = true;
			//		if (Math.Abs(Vector3.Dot(norm1, norm4)) > threshold) degenerate = true;
			//		if (Math.Abs(Vector3.Dot(norm2, norm3)) > threshold) degenerate = true;
			//		if (Math.Abs(Vector3.Dot(norm2, norm4)) > threshold) degenerate = true;
			//		if (degenerate) {
			//			AddOutputText("Segment " + segdata.num + " is degenerate because adjacent sides " + sidenum + " & " + side2 +" are nearly coplanar.");
			//			return true;
			//		}
			//	}
			//}

			for (int sidenum = 0; sidenum < 6; sidenum++) {
				int degen_count = 0;
				for (int trinum = 0; trinum < 2; trinum++) {
					a = m_level.vertex[segdata.vert[Segment.SegmentSideVerts[sidenum, 0]]].position;
					if (trinum == 0) {
						b = m_level.vertex[segdata.vert[Segment.SegmentSideVerts[sidenum, 1]]].position;
						c = m_level.vertex[segdata.vert[Segment.SegmentSideVerts[sidenum, 2]]].position;
					} else {
						b = m_level.vertex[segdata.vert[Segment.SegmentSideVerts[sidenum, 2]]].position;
						c = m_level.vertex[segdata.vert[Segment.SegmentSideVerts[sidenum, 3]]].position;
					}

					Vector3 ab = b - a;
					Vector3 ac = c - a;
					Vector3 normal = Vector3.Cross(ab, ac).Normalized();

					other_side_index = Utility.OppositeSide(sidenum);

					for (int j = 0; j < 4; j++) {    // Check all vertices on opposite side
						distances[j] = Vector3.Dot(m_level.vertex[segdata.vert[Segment.SegmentSideVerts[other_side_index, j]]].position - a, normal);
					}

					distance_sum = Math.Abs(distances[0]);

					// Have all distances.  If one is negative and any are positive, or vice versa, we have a degneracy.
					if (distances[0] < 0.0f) {
						for (int k = 1; k < 4; k++) {
							distance_sum += Math.Abs(distances[k]);

							if (distances[k] > 0.0f) {
								degen_count++;
							}
						}
					} else {
						for (int k = 1; k < 4; k++) {
							distance_sum += Math.Abs(distances[k]);

							if (distances[k] < 0.0f) {
								degen_count++;
							}
						}
					}
				}

				if (degen_count == 2) {		// Requiring both triangles to partition vertices on opposite side.  Requiring only one flagged too many not so bad segments.
					AddOutputText("Segment " + segdata.num + " is degenerate.  Side #" + Utility.OppositeSide(sidenum) + " has vertices on both sides of side #" + sidenum + ".  Distances = " + distances[0] + " " + distances[1] + " " + distances[2] + " " + distances[3]);
					return true;
				}

				if (distance_sum < 0.2f) {
					AddOutputText("Segment " + segdata.num + " is degenerate.  Sides " + Utility.OppositeSide(sidenum) + " and " + sidenum + " are very, very close together");
					return true;
				}
			}

			return false;
		}


		public static string CleanupName(string s)
		{
			//Must convert to lower case before converting to title case, because ToTitleCase() won't change all-caps words on the assumption they're acronyms
			var text_info = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo;
			return (s == null) ? "" : text_info.ToTitleCase(text_info.ToLower(s.Replace('_', ' ')));
		}

		public void Split2Way()
		{
			m_level.SplitSegment2Way(m_level.selected_segment, m_level.selected_side);
			RefreshGeometry(true);
		}

		public void Split5Way()
		{
			m_level.SplitSegment5Way(m_level.selected_segment, m_level.selected_side);
			RefreshGeometry(true);
		}

		public void Split7Way()
		{
			m_level.SplitSegment7Way(m_level.selected_segment);
			RefreshGeometry(true);
		}

		public void AverageMarkedSideVertsY()
		{
			m_level.AverageMarkedSideVertsY();
			RefreshGeometry(true);
      }

		public void HideMarkedSegments()
		{
			m_level.HideMarkedSegments();
			if (m_level.selected_segment > -1) {
				if (m_level.segment[m_level.selected_segment].m_hidden) {
					m_level.selected_segment = -1;
				}
			}
			RefreshAllGMeshes();
			RefreshGeometry(true);
		}

		public void HideUnmarkedSegments()
		{
			m_level.HideUnmarkedSegments();
			if (m_level.selected_segment > -1) {
				if (m_level.segment[m_level.selected_segment].m_hidden) {
					m_level.selected_segment = -1;
				}
			}
			RefreshAllGMeshes();
			RefreshGeometry(true);
		}

		public void UnhideHiddenSegments()
		{
			m_level.UnhideHiddenSegments();
			RefreshAllGMeshes();
			RefreshGeometry(true);
		}
	}
}