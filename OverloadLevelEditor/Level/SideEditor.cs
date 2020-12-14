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

// SIDE - Editor
// Editor-specific functions for modifying/etc a side (also see SideTexture)

namespace OverloadLevelEditor
{
	public partial class Side
	{
		public int m_tex_gl_id = -1;
		public bool m_tag;
		public Vector3 avg_normal = Vector3.UnitY;
		public Vector3 avg_position = Vector3.UnitY;
		public bool[] uv_mark = new bool[NUM_VERTS]; // This is for UV editing only, never saved

		public void SetVerts(int[] vrt)
		{
			for (int i = 0; i < NUM_VERTS; i++) {
				vert[i] = vrt[i];
			}
		}

		public Vector3 FindVertPos(int idx)
		{
			return level.vertex[vert[idx]].position;
		}

		public Vector3 FindBestEdgeDir4()
		{
			Vector3 e1 = FindEdgeDir(0);
			Vector3 e2 = FindEdgeDir(1);
			Vector3 e3 = FindEdgeDir(2);
			Vector3 e4 = FindEdgeDir(3);

			if (Utility.AlmostCardinal(e1)) {
				return e1;
			} else if (Utility.AlmostCardinal(e2)) {
				return e2;
			} else if (Utility.AlmostCardinal(e3)) {
				return e3;
			} else if (Utility.AlmostCardinal(e4)) {
				return e4;
			} else {
				return e1;
			}
		}

		public void AlignToSide(Side dst)
		{
			float close_dist_sq = 999999f;
			int close_vert = 0;
			int close_vert_dst = 0;
			float dist_sq;
			Vector3 diff;

			// Find the closest pair of verts
			for (int i = 0; i < Side.NUM_VERTS; i++) {
				for (int j = 0; j < Side.NUM_VERTS; j++) {
					diff = level.vertex[vert[i]].position - level.vertex[dst.vert[j]].position;
					dist_sq = diff.LengthSquared;
					if (dist_sq < close_dist_sq) {
						close_dist_sq = dist_sq;
						close_vert = i;
						close_vert_dst = j;
					}
				}
			}

			// Align all the verts
			for (int i = 0; i < Side.NUM_VERTS; i++) {
				level.vertex[vert[(close_vert + i) % Side.NUM_VERTS]].position = level.vertex[dst.vert[(close_vert_dst + (Side.NUM_VERTS - i)) % Side.NUM_VERTS]].position;
			}
		}

		/*public void Copy(Side src, bool full)
		{
			for (int i = 0; i < NUM_VERTS; i++) {
				vert[i] = src.vert[i];
				uv[i] = src.uv[i];
			}

			CopyTexturesAndDecals(src, false);
		}*/

		public void CopyTexturesAndDecals(Side src, bool align = true)
		{
			m_tex_gl_id = src.m_tex_gl_id;
			tex_name = src.tex_name;
			if (tex_name == "") {
				tex_name = DEFAULT_TEX_NAME;
				m_tex_gl_id = level.editor.tm_level.FindTextureIDByName(tex_name);
			}

			deformation_preset = src.deformation_preset;
			deformation_height = src.deformation_height;

			if (align) {
				DefaultAlignment();
			}

			for (int i = 0; i < NUM_DECALS; i++) {
				decal[i].Copy(src.decal[i]);
			}
		}

		public void CopyTextures(Side src, bool align = true)
		{
			m_tex_gl_id = src.m_tex_gl_id;
			tex_name = src.tex_name;
			if (tex_name == "") {
				tex_name = DEFAULT_TEX_NAME;
				m_tex_gl_id = level.editor.tm_level.FindTextureIDByName(tex_name);
			}

			deformation_preset = src.deformation_preset;
			deformation_height = src.deformation_height;

			if (align) {
				DefaultAlignment();
			}
		}

		public bool HasTaggedVerts()
		{
			for (int i = 0; i < Side.NUM_VERTS; i++) {
				if (level.vertex[vert[i]].m_tag) {
					return true;
				}
			}

			return false;
		}

		public void DisableDecals()
		{
			for (int i = 0; i < NUM_DECALS; i++) {
				decal[i].hidden = true;
			}
		}

		// Scale my size away/towards my centerpoint
		public void Scale(float scl)
		{
			Vector3 center = FindCenter();
			for (int i = 0; i < NUM_VERTS; i++) {
				level.vertex[vert[i]].position = center * (1f - scl) + level.vertex[vert[i]].position * scl;
			}
		}

		public bool HasFourSharedVerts(Side s)
		{
			int shared = 0;
			for (int i = 0; i < Side.NUM_VERTS; i++) {
				for (int j = 0; j < Side.NUM_VERTS; j++) {
					if (vert[i] == s.vert[j]) {
						shared += 1;
						if (shared >= 4) {
							return true;
						}
					}
				}
			}

			return false;
		}

		public Side ConnectedSide()
		{
			int neighbor = segment.neighbor[num];
			if (neighbor > -1) {
				int side_num = segment.level.segment[neighbor].FindConnectingSide(segment.num);
				return segment.level.segment[neighbor].side[side_num];
			} else {
				return null;
			}
		}
	}
}