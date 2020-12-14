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
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using Newtonsoft.Json.Linq;

// SEGMENT - Editor
// Editor-specific functions for modifying/etc a segment

namespace OverloadLevelEditor
{
	public partial class Segment
	{
		//List of segement vertex indices for each side
		public static int[,] SegmentSideVerts = new int[,] { { 7, 6, 2, 3 }, { 0, 4, 7, 3 }, { 0, 1, 5, 4 }, { 2, 6, 5, 1 }, { 4, 5, 6, 7 }, { 3, 2, 1, 0 } };
																				//	LEFT				TOP				RIGHT					BOTTOM			FRONT				BACK

		public void Delete()
		{
			Segment nb_seg;
			// Remove me as a neighbor from my neighbors
			for (int i = 0; i < NUM_SIDES; i++) {
				if (neighbor[i] > -1) {
					nb_seg = level.segment[neighbor[i]];
					for (int j = 0; j < NUM_SIDES; j++) {
						if (nb_seg.neighbor[j] == num) {
							nb_seg.neighbor[j] = -1;
						}
					}
				}
			}

			// Mark me as dead
			m_alive = false;
			marked = false;
		}

		public void Create(int[] vrt, int[] nbr, bool mrk = false)
		{
			for (int i = 0; i < NUM_VERTS; i++) {
				vert[i] = vrt[i];
			}

			if (nbr != null) {
				for (int i = 0; i < NUM_SIDES; i++) {
					neighbor[i] = nbr[i];
				}
			} else {
				for (int i = 0; i < NUM_SIDES; i++) {
					neighbor[i] = -1;
				}
			}

			// TODO: Assign side properties?
			for (int i = 0; i < NUM_SIDES; i++) {
				side[i].Init();
			}
			AssignSideVerts();

			m_hidden = false;
			marked = mrk;
			m_alive = true;
		}

		public void AssignSideVerts()
		{
			for (int i = 0; i < NUM_SIDES; i++) {
				side[i].SetVerts(Utility.SideVertsFromSegVerts(vert, i));
			}
		}

		public int NumNeighbors()
		{
			int count = 0;
			for (int i = 0; i < NUM_SIDES; i++) {
				if (neighbor[i] > -1) {
					count += 1;
				}
			}

			return count;
		}

		// Clean up neighbors (for copy/paste)
		public void RemoveDeadNeighbors()
		{
			for (int i = 0; i < NUM_SIDES; i++) {
				if (neighbor[i] > -1) {
					if (!level.segment[neighbor[i]].Alive) {
						neighbor[i] = -1;
					}
				}
			}
		}

		/*public void Copy(Segment src, bool full)
		{
			alive = src.alive;
			marked = (full ? src.marked : true);

			for (int i = 0; i < NUM_VERTS; i++) {
				vert[i] = src.vert[i];
			}

			for (int i = 0; i < NUM_SIDES; i++) {
				side[i].Copy(src.side[i], full);
				neighbor[i] = src.neighbor[i];
			}
		}*/

		public void Invert()
		{
			int tmp;
			// Swap the verts of the left/right sides
			// Left (7623) Right (0154)
			tmp = vert[0];
			vert[0] = vert[3];
			vert[3] = tmp;

			tmp = vert[1];
			vert[1] = vert[2];
			vert[2] = tmp;

			tmp = vert[5];
			vert[5] = vert[6];
			vert[6] = tmp;

			tmp = vert[4];
			vert[4] = vert[7];
			vert[7] = tmp;

			// Swap the neighbors of the left/right sides
			tmp = neighbor[(int)SideOrder.LEFT];
			neighbor[(int)SideOrder.LEFT] = neighbor[(int)SideOrder.RIGHT];
			neighbor[(int)SideOrder.RIGHT] = tmp;

			// Swap the textures and decals of the left/right sides
			Side s = new Side(this, (int)SideOrder.LEFT);
			s.Copy(side[(int)SideOrder.LEFT], false);
			side[(int)SideOrder.LEFT].Copy(side[(int)SideOrder.RIGHT], false);
			side[(int)SideOrder.RIGHT].Copy(s, false);

			for (int i = 0; i < NUM_SIDES; i++) {
				// Reset the verts for the sides
				side[i].SetVerts(Utility.SideVertsFromSegVerts(vert, i));
			}
		}

		public void CopyTexturesAndDecalsFromSegmentAtBack(Segment back_seg, int back_seg_front_side)
		{
			if (level.editor.ActiveInsertDecal != InsertDecal.NONE) {
				for (int i = 0; i < Segment.NUM_SIDES; i++) {
					if (level.editor.ActiveInsertDecal == InsertDecal.ALL || i == back_seg_front_side) {
						side[i].CopyTexturesAndDecals(back_seg.side[i]);
					} else {
						side[i].CopyTextures(back_seg.side[i]);
					}
				}
			} else {
				for (int i = 0; i < Segment.NUM_SIDES; i++) {
					side[i].CopyTextures(back_seg.side[i]);
				}
			}
			side[Utility.OppositeSide(back_seg_front_side)].DisableDecals();
		}

		public int FindSideWithTwoMatchingVerts(Side s, int exclude_side)
		{
			for (int i = 0; i < NUM_SIDES; i++) {
				if (i != exclude_side && side[i].HasTwoSharedVerts(s)) {
					return i;
				}
			}

			return exclude_side;
		}

		//Returns the side number of the current segement that connects to the specified segment
		public int FindConnectingSide(int connected_segnum)
		{
			for (int sidenum = 0; sidenum < NUM_SIDES; sidenum++) {
				if (this.neighbor[sidenum] == connected_segnum) {
					return sidenum;
				}
			}
			throw new System.Exception("FindConnectingSide: Can't find connecting side.");
		}

		public void DetatchSide(int side)
		{
			if (neighbor[side] > -1) {
				// Detach my neighbor first
				level.segment[neighbor[side]].neighbor[level.segment[neighbor[side]].FindConnectingSide(num)] = -1;
				neighbor[side] = -1;
			}
		}

		public void Scale(float scl)
		{
			Vector3 center = FindCenter();
			for (int i = 0; i < NUM_VERTS; i++) {
				level.vertex[vert[i]].position = center * (1f - scl) + level.vertex[vert[i]].position * scl;
			}
		}

		// Find the highest (measued in Y) side
		public int FindTopSide()
		{
			int top_side = 0;
			float top_y = side[0].FindCenter().Y;

			float y;

			for (int i = 1; i < NUM_SIDES; i++) {
				y = side[i].FindCenter().Y;
				if (y > top_y) {
					top_y = y;
					top_side = i;
				}	
			}

			return top_side;
		}

		public int FindLevelVertNotOnSide(int lvl_vert, int side_idx)
		{
			int seg_vert = 0;
			for (int i = 0; i < NUM_VERTS; i++) {
				if (vert[i] == lvl_vert) {
					seg_vert = i;
					break;
				}
			}

			return vert[FindSegVertNotOnSide(seg_vert, side_idx)];
		}

		public int FindSegVertNotOnSide(int seg_vert, int side_idx)
		{
			switch (side_idx) {
				case (int)SideOrder.LEFT:
					switch (seg_vert) {
						case 2: return 1;
						case 3: return 0;
						case 6: return 5;
						case 7: return 4;
					}
					break;
				case (int)SideOrder.RIGHT:
					switch (seg_vert) {
						case 1: return 2;
						case 0: return 3;
						case 5: return 6;
						case 4: return 7;
					}
					break;
				case (int)SideOrder.TOP:
					switch (seg_vert) {
						case 0: return 1;
						case 3: return 2;
						case 7: return 6;
						case 4: return 5;
					}
					break;
				case (int)SideOrder.BOTTOM:
					switch (seg_vert) {
						case 1: return 0;
						case 2: return 3;
						case 6: return 7;
						case 5: return 4;
					}
					break;
				case (int)SideOrder.FRONT:
					switch (seg_vert) {
						case 4: return 0;
						case 5: return 1;
						case 6: return 2;
						case 7: return 3;
					}
					break;
				case (int)SideOrder.BACK:
					switch (seg_vert) {
						case 0: return 4;
						case 1: return 5;
						case 2: return 6;
						case 3: return 7;
					}
					break;
			}

			// This should never happen
			return -1;
		}

		public int FindSideWithSameVerts(Side s)
		{
			for (int i = 0; i < NUM_SIDES; i++) {
				if (side[i].HasFourSharedVerts(s)) {
					return i;
				}
			}

			return -1;
		}
	}
}