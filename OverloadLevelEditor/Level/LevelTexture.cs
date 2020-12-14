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

// LEVEL - Texture
// UVs, texture assignment, and decal functions
// (These are editor-only, not for game)
// (Some functions could possibly be moved into other Level sub-files)

namespace OverloadLevelEditor
{
	public partial class Level
	{
		public const float PLANAR_SCL = 0.25f;
		public static Vector3 PLANAR_OFFSET = Vector3.One * 2f;

		public void UVPlanarMapMarkedSides(Axis axis)
		{
			List<Side> side_list = GetMarkedSides(false, true);

			for (int i = 0; i < side_list.Count; i++) {
				side_list[i].PlanarMap(axis, PLANAR_SCL, PLANAR_OFFSET);
			}
		}

		public void UVBoxMapMarkedSides()
		{
			List<Side> side_list = GetMarkedSides(false, true);

			for (int i = 0; i < side_list.Count; i++) {
				side_list[i].BoxMap(PLANAR_SCL, PLANAR_OFFSET);
			}
		}

		public void UVDefaultMapMarkedSides()
		{
			List<Side> side_list = GetMarkedSides(false, true);

			for (int i = 0; i < side_list.Count; i++) {
				side_list[i].DefaultAlignment();
			}
		}

		public void UVSnapToFraction(int whole)
		{
			List<Side> side_list = GetMarkedSides(false, true);

			for (int i = 0; i < side_list.Count; i++) {
				side_list[i].UVSnapToFraction(whole);
			}
		}

		public void UVAlignToSide()
		{
			if (selected_segment > -1 && selected_side > -1) {
				UnTagAllSides();

				Side s = segment[selected_segment].side[selected_side];
				s.m_tag = true;

				UVAlignAdjacentMarkedSides(s);
			}
		}

		public void UnTagAllSides()
		{
			for (int i = 0; i < MAX_SEGMENTS; i++) {
				if (segment[i].Alive) {
					for (int j = 0; j < Segment.NUM_SIDES; j++) {
						segment[i].side[j].m_tag = false;
					}
				}
			}
		}

		// Recursive alignment
		public void UVAlignAdjacentMarkedSides(Side s)
		{
			List<Side> side_list = GetMarkedSides();

			for (int i = 0; i < side_list.Count; i++) {
				if (!side_list[i].m_tag) {
					if (s.HasTwoSharedVerts(side_list[i])) {
						side_list[i].m_tag = true;
						side_list[i].UVAlignToSide(s);
						UVAlignAdjacentMarkedSides(side_list[i]);
					}
				}
			}
		}

		public void UVMoveMarkedSide(Vector2 dir)
		{
			List<Side> side_list = GetMarkedSides(false, true);

			for (int i = 0; i < side_list.Count; i++) {
				for (int j = 0; j < Side.NUM_VERTS; j++) {
					side_list[i].uv[j] += dir;
				}
			}
		}

		public void UVRotateMarkedSide(float angle)
		{
			List<Side> side_list = GetMarkedSides(false, true);

			// Find the center point from the selected side
			Vector2 uv_center = GetSelectedSideUVCenter();

			for (int i = 0; i < side_list.Count; i++) {
				for (int j = 0; j < Side.NUM_VERTS; j++) {
					// ROTATE
					side_list[i].uv[j] = Utility.Vector2Rotate(side_list[i].uv[j] - uv_center, angle) + uv_center;
				}
			}
		}

		public void UVCenterU()
		{
			List<Side> side_list = GetMarkedSides(false, true);

			for (int i = 0; i < side_list.Count; i++) {
				side_list[i].CenterU();
			}
		}

		public void UVCenterV()
		{
			List<Side> side_list = GetMarkedSides(false, true);

			for (int i = 0; i < side_list.Count; i++) {
				side_list[i].CenterV();
			}
		}

		public Vector2 GetSelectedSideUVCenter()
		{
			if (selected_segment > -1 && selected_side > -1) {
				return segment[selected_segment].side[selected_side].FindUVCenter();
			} else {
				return Vector2.Zero;
			}
		}

		public void CopyDecalPropertiesToMarked(Decal src, int idx)
		{
			List<Side> side_list = GetMarkedSides();

			for (int i = 0; i < side_list.Count; i++) {
				side_list[i].decal[idx].Copy(src);
			}
		}

		public void ApplyTexture(string s, int idx)
		{
			List<Side> side_list = GetMarkedSides(false, true);
			
			for (int i = 0; i < side_list.Count; i++) {
				side_list[i].m_tex_gl_id = idx;
				side_list[i].tex_name = s;
			}
		}

		public void SetSideCavePreset(int idx)
		{
			List<Side> side_list = GetMarkedSides(false, true);

			for (int i = 0; i < side_list.Count; i++) {
				side_list[i].deformation_preset = idx;
			}
		}

		public void MarkSidesWithTexture(string s)
		{
			s = s.ToLower();
			for (int i = 0; i < MAX_SEGMENTS; i++) {
				if (segment[i].Visible) {
					for (int j = 0; j < Segment.NUM_SIDES; j++) {
						if (segment[i].side[j].tex_name.ToLower() == s) {
							segment[i].side[j].marked = true;
						}
					}
				}
			}
		}

		public int GetSelectedSideCavePreset()
		{
			if (selected_segment > -1 && selected_side > -1) {
				return segment[selected_segment].side[selected_side].deformation_preset;
			}
			return 0;
		}

		public string GetSelectedSideTexture()
		{
			if (selected_segment > -1 && selected_side > -1) {
				return segment[selected_segment].side[selected_side].tex_name;
			}
			return "";
		}

		public void MarkSidesWithCavePreset(int idx, bool same_height)
		{
			float height = 0f;
			if (same_height) {
				height = GetSelectedSide().deformation_height;
			}

			for (int i = 0; i < MAX_SEGMENTS; i++) {
				if (segment[i].Visible) {
					for (int j = 0; j < Segment.NUM_SIDES; j++) {
						if (segment[i].side[j].deformation_preset == idx && (height > 0f ? segment[i].side[j].deformation_height == height : segment[i].side[j].deformation_height > 0f)) {
							segment[i].side[j].marked = true;
						}
					}
				}
			}
		}

		public void ApplyDecal(Side side, DMesh dm, int idx)
		{
			side.decal[idx].mesh_name = dm.name;
			side.decal[idx].hidden = false;
		}

		public void UpdateSideTextures()
		{
			// Go through every side, change the tex_gl_id to match the gl_id of the loaded texture
			// If you don't do this, when you add new textures, the level textures will get messed up next time you load the editor
			// WARNING: This is not optimized
            if(editor.IsHeadlessProxyEditor) {
                // Not the real editor, it is a proxy (headless), so just do nothing.
                return;
            }

            for (int i = 0; i < MAX_SEGMENTS; i++) {
				var curr_seg = segment[i];
				if (!curr_seg.Alive) {
					continue;
				}

				for (int j = 0; j < Segment.NUM_SIDES; j++) {
					var curr_side = curr_seg.side[j];
					curr_side.m_tex_gl_id = editor.tm_level.FindTextureIDByName(curr_side.tex_name);
				}
			}
		}

	}
}