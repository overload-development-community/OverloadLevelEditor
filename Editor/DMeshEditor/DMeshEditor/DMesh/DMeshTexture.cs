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
		public const float PLANAR_SCL = 0.25f;
		public static Vector3 PLANAR_OFFSET = Vector3.Zero;
		
		public void UVPlanarMapMarkedSides(Axis axis)
		{
			List<DPoly> poly_list = GetMarkedPolys();

			for (int i = 0; i < poly_list.Count; i++) {
				poly_list[i].PlanarMap(axis, PLANAR_SCL, PLANAR_OFFSET, this);
			}
		}

		public void UVBoxMapMarkedSides()
		{
			List<DPoly> poly_list = GetMarkedPolys();

			for (int i = 0; i < poly_list.Count; i++) {
				poly_list[i].BoxMap(PLANAR_SCL, PLANAR_OFFSET, this);
			}
		}

		public void UVDefaultMapMarkedPolys()
		{
			List<DPoly> poly_list = GetMarkedPolys();

			for (int i = 0; i < poly_list.Count; i++) {
				poly_list[i].DefaultAlignment(this);
			}
		}

		public void UVSnapToFraction(int whole)
		{
			List<DPoly> poly_list = GetMarkedPolys();

			for (int i = 0; i < poly_list.Count; i++) {
				poly_list[i].UVSnapToFraction(whole);
			}
		}

		public void UVCenterU()
		{
			List<DPoly> poly_list = GetMarkedPolys();

			for (int i = 0; i < poly_list.Count; i++) {
				poly_list[i].CenterU();
			}
		}

		public void UVCenterV()
		{
			List<DPoly> poly_list = GetMarkedPolys();

			for (int i = 0; i < poly_list.Count; i++) {
				poly_list[i].CenterV();
			}
		}

		public void FitUVsToQuarter(float offset_x, float offset_y)
		{
			List<DPoly> poly_list = GetMarkedPolys();

			for (int i = 0; i < poly_list.Count; i++) {
				poly_list[i].FitUVsToQuarter(offset_x, offset_y);
			}
		}

		public void UVAlignToPoly()
		{
			if (selected_poly > -1) {
				TagAllPolys(false);

				DPoly dp = polygon[selected_poly];
				dp.tag = true;

				UVAlignAdjacentMarkedPolys(dp);
			}
		}

		// Recursive alignment
		public void UVAlignAdjacentMarkedPolys(DPoly dp)
		{
			List<DPoly> poly_list = GetMarkedPolys();

			for (int i = 0; i < poly_list.Count; i++) {
				if (!poly_list[i].tag) {
					if (DPoly.HasTwoOrMoreSharedVerts(dp, poly_list[i])) {
						poly_list[i].tag = true;
						poly_list[i].UVAlignToPoly(dp, this);
						UVAlignAdjacentMarkedPolys(poly_list[i]);
					}
				}
			}
		}

		public void UVMoveMarkedPoly(Vector2 dir)
		{
			List<DPoly> poly_list = GetMarkedPolys();

			for (int i = 0; i < poly_list.Count; i++) {
				for (int j = 0; j < poly_list[i].num_verts; j++) {
					poly_list[i].tex_uv[j] += dir;
				}
			}
		}

		public void UVRotateMarkedPoly(float angle)
		{
			List<DPoly> poly_list = GetMarkedPolys();

			// Find the center point from the selected side
			Vector2 uv_center = GetSelectedPolyUVCenter();

			for (int i = 0; i < poly_list.Count; i++) {
				for (int j = 0; j < poly_list[i].num_verts; j++) {
					// ROTATE
					poly_list[i].tex_uv[j] = Utility.Vector2Rotate(poly_list[i].tex_uv[j] - uv_center, angle) + uv_center;
				}
			}
		}

		public Vector2 GetSelectedPolyUVCenter()
		{
			if (selected_poly > -1) {
				return polygon[selected_poly].FindUVCenter();
			} else {
				return Vector2.Zero;
			}
		}

		public void ApplyTexture(string s, int idx)
		{
			List<DPoly> poly_list = GetMarkedPolys();

			int tex_idx = FindTextureIndex(s);
			if (tex_idx > -1) {
				idx = tex_idx;
			} else {
				AddTexture(idx, s);
				idx = tex_name.Count - 1;
			}
			for (int i = 0; i < poly_list.Count; i++) {
				poly_list[i].tex_index = idx;	
			}
		}

		public void MarkPolysWithTexture(string s)
		{
			editor.m_edit_mode = EditMode.POLY;
			ToggleMarkAll(true);

			int tex_idx = FindTextureIndex(s);
			for (int i = 0; i < polygon.Count; i++) {
				if (polygon[i].tex_index == tex_idx) {
					polygon[i].marked = true;
				}
			}
		}

		public string GetSelectedPolyTexture()
		{
			if (selected_poly > -1 && selected_poly < polygon.Count && polygon[selected_poly].tex_index > -1) {
				return tex_name[polygon[selected_poly].tex_index];
			} else {
				return "";
			}
		}
	}
}