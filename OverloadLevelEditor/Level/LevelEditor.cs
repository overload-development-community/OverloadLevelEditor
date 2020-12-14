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
using System.Collections.Generic;

// LEVEL - Editor
// General level functions that the editor uses
// (Some functions could possibly be moved into other Level sub-files)

namespace OverloadLevelEditor
{
	public partial class Level
	{
		public void CopyLevelProperties(Level src)
		{
			next_segment = src.next_segment;
			next_vertex = src.next_vertex;
			next_entity = src.next_entity;
			selected_segment = src.selected_segment;
			selected_side = src.selected_side;
			selected_vertex = src.selected_vertex;
			selected_entity = src.selected_entity;
			num_segments = src.num_segments;
			num_vertices = src.num_vertices;
			num_entities = src.num_entities;
			num_marked_segments = src.num_marked_segments;
			num_marked_sides = src.num_marked_sides;
			num_marked_vertices = src.num_marked_vertices;
			num_marked_entities = src.num_marked_entities;
		}

		public int CreateVertex(Vector3 pos)
		{
			if (next_vertex > -1) {
				int idx = next_vertex;
				vertex[idx].Create(pos);
				GetNextVertexIndex();
				return idx;
			} else {
				Utility.DebugPopup("Max level vertices reached");
				return -1;
			}
		}

		public void GetNextVertexIndex()
		{
			for (int i = 0; i < MAX_VERTICES; i++) {
				next_vertex = (next_vertex + 1) % MAX_VERTICES;
				if (!vertex[next_vertex].alive) {
					return;
				}
			}

			// Nothing found
			next_vertex = -1;
		}

		public int CreateSegment(int[] verts, int[] neighbors)
		{
			if (next_segment > -1) {
				int idx = next_segment;
				segment[idx].Create(verts, neighbors);
				GetNextFreeSegmentIndex();
				return idx;
			} else {
				Utility.DebugPopup("Max level segments reached");
				return -1;
			}
		}

		public void GetNextFreeSegmentIndex()
		{
			for (int i = 0; i < MAX_SEGMENTS; i++) {
				next_segment = (next_segment + 1) % MAX_SEGMENTS;
				if (!segment[next_segment].Alive) {
					return;
				}
			}

			// Nothing found
			next_segment = -1;
		}

		public Segment GetSelectedSegment()
		{
			return (selected_segment > -1) ? segment[selected_segment] : null;
		}

		public Side GetSelectedSide()
		{
			if (selected_side > -1 && selected_segment > -1) {
				return segment[selected_segment].side[selected_side];
			} else {
				return null;
			}
		}

		public void GetSelectedSidePosAndNormal(out Vector3 pos, out Vector3 normal)
		{
			if (selected_side > -1 && selected_segment > -1) {
				Side s = segment[selected_segment].side[selected_side];
				pos = s.FindCenter();
				normal = s.FindNormal();
			} else {
				editor.AddOutputText("WARNING: No selected segment side");
				pos = Vector3.Zero;
				normal = Vector3.Zero;
			}
		}

		public Vector3 GetSelectedSideNormal()
		{
			if (selected_side > -1 && selected_segment > -1) {
				Side s = segment[selected_segment].side[selected_side];
				return s.FindNormal();
			} else {
				editor.AddOutputText("WARNING: No selected segment side");
				return Vector3.UnitZ;
			}
		}

		public Matrix4 GetSelectedSideOrientation()
		{
			if (selected_side > -1 && selected_segment > -1) {
				Side s = segment[selected_segment].side[selected_side];
				return s.FindOrientation();
			} else {
				editor.AddOutputText("WARNING: No selected segment side");
				return Matrix4.Identity;
			}
		}

		public Vector3 GetSelectedSidePos()
		{
			if (selected_side > -1 && selected_segment > -1) {
				Side s = segment[selected_segment].side[selected_side];
				return s.FindCenter();
			} else {
				editor.AddOutputText("WARNING: No selected segment side");
				return Vector3.Zero;
			}
		}

		public Vector3 GetSelectedSegmentPos()
		{
			if (selected_segment > -1) {
				Segment seg = segment[selected_segment];
				return seg.FindCenter();
			} else {
				editor.AddOutputText("WARNING: No selected segment");
				return Vector3.Zero;
			}
		}

		public void UpdateCounts()
		{
			ResetCounts();
			for (int i = 0; i < MAX_VERTICES; i++) {
				if (vertex[i].alive) {
					num_vertices += 1;
					if (vertex[i].marked) {
						num_marked_vertices += 1;
					}
				}
			}

			for (int i = 0; i < MAX_SEGMENTS; i++) {
				if (segment[i].Alive) {
					num_segments += 1;
					if (segment[i].Visible) {
						num_visible_segments += 1;
					}
					if (segment[i].marked) {
						num_marked_segments += 1;
					}
					for (int j = 0; j < Segment.NUM_SIDES; j++) {
						if (segment[i].side[j].marked) {
							num_marked_sides += 1;
						}
						if (segment[i].neighbor[j] < 0) {
							num_sides += 1;
						}
					}
				}
			}

			for (int i = 0; i < MAX_ENTITIES; i++) {
				if (entity[i].alive) {
					num_entities += 1;
					if (entity[i].marked) {
						num_marked_entities += 1;
					}
				}
			}
		}

		public void ResetCounts()
		{
			num_segments = 0;
			num_vertices = 0;
			num_entities = 0;
			num_sides = 0;
			num_visible_segments = 0;
			num_marked_segments = 0;
			num_marked_sides = 0;
			num_marked_vertices = 0;
			num_marked_entities = 0;
		}

		public bool SelectedSegmentAlive()
		{
			if (selected_segment > -1) {
				if (segment[selected_segment].Alive) {
					return true;
				}
			}

			return false;
		}

		public bool SelectedSideMarked()
		{
			if (selected_segment > -1) {
				return (segment[selected_segment].side[selected_side].marked);
			} else {
				return false;
			}
		}

		public Decal GetSelectedDecal(int decal_idx)
		{
			if ((selected_segment > -1) && (selected_side > -1) && (decal_idx > -1)) {
				return segment[selected_segment].side[selected_side].decal[decal_idx];
			} else {
				return null;
			}
		}

		public List<Decal> GetSelectedOrMarkedDecals(int decal_idx)
		{
			List<Decal> decals = new List<Decal>();
			List<Side> sides = GetMarkedSides();
			if (sides.Count > 0) {
				foreach (Side s in sides) {
					decals.Add(s.decal[decal_idx]);
				}
			} else if (selected_segment > -1 && selected_side > -1) {
				decals.Add(segment[selected_segment].side[selected_side].decal[decal_idx]);
			}
			return decals;
		}

		//Returns list of Decal objects on marked sides
		public List<Decal> GetMarkedDecals(int decal_idx)
		{
			List<Decal> decals = new List<Decal>();
			List<Side> sides = GetMarkedSides();
			foreach (Side s in sides) {
				decals.Add(s.decal[decal_idx]);
			}
			return decals;
		}

		//Returns list of Decal objects that have been applied to marked sides
		public List<Decal> GetMarkedAppliedDecals(int decal_idx)
		{
			List<Decal> decals = new List<Decal>();
			List<Side> sides = GetMarkedSides();
			foreach (Side s in sides) {
				if (s.decal[decal_idx].Applied) {
					decals.Add(s.decal[decal_idx]);
				}
			}
			return decals;
		}

		public Entity GetSelectedEntity()
		{
			if (selected_entity > -1 && entity[selected_entity].alive) {
				return entity[selected_entity];
			} else {
				return null;
			}
		}

		//Returns the active texture set for this level
		public TextureSet GetTextureSet(bool show_error = true)
		{
			TextureSet texture_set = editor.TextureSets.Find(ts => (ts.Name == m_texture_set_name));

			if (texture_set == null) {
				texture_set = editor.TextureSets[0];
				if (show_error) {
					System.Windows.Forms.MessageBox.Show("Could not find texture set '" + m_texture_set_name + "'.  Using texture set '" + texture_set.Name + "' instead.");
				}
				m_texture_set_name = texture_set.Name;
			}

			return texture_set;
		}

		//Returns true if the point is inside the segment
		public bool PointInsideSegment(Segment seg, Vector3 pos)
		{
			if (!seg.Alive) {
				return false;
			}

			foreach (Side side in seg.side) {
				bool invertOrderForSide = ((seg.neighbor[side.num] > -1) && (seg.neighbor[side.num] < seg.num));
				QuadTriangulationOrder triangulationOrder = GetTriangulationOrder(this, seg.num, side.num);
				int[] side_verts = Utility.SideVertsFromSegVerts(seg.vert, side.num);
				int behind = 0;
				for (int tri = 0; tri < 2; tri++) {
					int[] verts = triangulationOrder.GetVertsForTriangle(tri, invertOrderForSide);
					if (Vector3.Dot(pos - vertex[side_verts[verts[0]]].position, GetTriangleNormal(vertex[side_verts[verts[0]]].position, vertex[side_verts[verts[1]]].position, vertex[side_verts[verts[2]]].position)) < 0f) { 
						if (!invertOrderForSide) {		//If a convex side, then behind either triangle means point is outside
							return false;
						}
						behind++;
					}
				}
				if (behind == 2) {		//For concave side, point must be behind both triangles to be out
					return false;
				}
			}

			return true;
		}

		public int FindSegmentForPoint(Vector3 pos)
		{
			try {
				foreach (Segment seg in EnumerateAliveSegments()) {
					if (PointInsideSegment(seg, pos)) {
						return seg.num;
					}
				}

				return -1;
			} catch {
				return -1;
			}
		}

	}
}
