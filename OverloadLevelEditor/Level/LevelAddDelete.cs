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
using Overload;

// LEVEL - Add and Delete
// Level functions that add and delete geometry/vertices
// (These are editor-only, not for game)
// (Some functions could possibly be moved into other Level sub-files)

namespace OverloadLevelEditor
{
	public partial class Level
	{
		public int CreateSegmentAtPosition(Vector3 pos, Vector3 rot)
		{
			int[] verts = new int[8];

			// Create new vertices
			for (int i = 0; i < Segment.NUM_VERTS; i++) {
				verts[i] = CreateVertex(pos + Utility.DefaultSVOffset(i) * DEF_SEG_SZH);

				// Abort if creating a vert failed
				if (verts[i] < 0) {
					editor.AddOutputText("Failed to create a segment because we couldn't create verts");
					return -1;
				}
			}

			int seg_idx = CreateSegment(verts, null);

			selected_segment = seg_idx;
			selected_side = (int)SideOrder.FRONT;
			selected_vertex = segment[seg_idx].side[0].vert[0];

			editor.AddOutputText("Created a default segment at: " + pos.ToString());

			return seg_idx;
		}

		public List<Tuple<int,int>> InsertMarkedSidesMulti(int extrude_length, Side s)
		{
			Vector3 side_normal = -s.FindNormal();
			Vector3 side_right, side_up, side_center;

			// Find the list of marked sides
			List<Side> side_list = new List<Side>();

			// Tag the segments (for refreshing decals)
			UnTagAllSegments();
			
			// Get the list of marked sides		
			for (int i = 0; i < MAX_SEGMENTS; i++) {
				if( !segment[i].Visible ) {
					continue;
				}
				for( int j = 0; j < Segment.NUM_SIDES; j++ ) {
					if( segment[i].neighbor[j] < 0 ) {
						if( segment[i].side[j].marked ) {
							side_list.Add( segment[i].side[j] );
							segment[i].m_tag = true;
						}
					}
				}
			}

			int idx_new;

			// Create a new segment for each marked side
			List<Tuple<int, int>> res = new List<Tuple<int, int>>();
			for (int i = 0; i < side_list.Count; i++) {
				side_right = side_list[i].FindBestEdgeDir();
				side_up = Vector3.Cross(side_normal, side_right).Normalized();
				side_center = side_list[i].FindCenter();

				idx_new = InsertSegmentBasic(side_list[i], side_center, side_normal, side_right, side_up, false, extrude_length);

				segment[idx_new].CopyTexturesAndDecalsFromSegmentAtBack(side_list[i].segment, side_list[i].num);
				side_list[i].DisableDecals();

				segment[idx_new].m_tag = true;

				// Mark the new side and unmark the old
				side_list[i].marked = false;
				segment[idx_new].side[side_list[i].num].marked = true;
				res.Add(Tuple.Create(idx_new, side_list[i].num));

				// Select the segment if it was the selected one
				if (side_list[i].num == selected_side && side_list[i].segment.num == selected_segment) {
					selected_segment = idx_new;
				}
			}

			return res;
		}

		public bool InsertMarkedSides()
		{
			if (num_marked_sides > 0 && selected_side > -1 && selected_segment > -1) {
				// Get the normal from the selected side
				Side s = segment[selected_segment].side[selected_side];
				if (s.segment.neighbor[selected_side] < 0 && s.marked) {
					UnTagAllVertices();

					InsertMarkedSidesMulti(editor.CurrExtrudeLength, s);

					JoinSidesWithTaggedVertices();

					return true;
				}
			}
			editor.AddOutputText("Need to mark at least one side, and the selected side must be marked too");

			return false;
		}

		//Returns segnum, or -1 
		public int InsertSegmentSelectedSide(bool regular = false)
		{
			if (selected_side > -1 && selected_segment > -1) {
				Side s = segment[selected_segment].side[selected_side];
				if (s.segment.neighbor[selected_side] < 0) {
					Vector3 side_normal = -s.FindNormal();
					Vector3 side_right = s.FindBestEdgeDir(); // s.FindEdgeDir(0);
					Vector3 side_up = Vector3.Cross(side_normal, side_right).Normalized();
					Vector3 side_center = s.FindCenter();

					int idx_new = InsertSegmentBasic(s, side_center, side_normal, side_right, side_up, regular, editor.CurrExtrudeLength);

					segment[idx_new].CopyTexturesAndDecalsFromSegmentAtBack(segment[selected_segment], selected_side);
					s.DisableDecals();

					// Tag the two segments (for refreshing decals)
					UnTagAllSegments();
					segment[idx_new].m_tag = true;
					s.segment.m_tag = true;

					// Select the new cube (depending on the option)
					if (editor.ShouldInsertAdvance) {
						selected_segment = idx_new;
						//selected_side = (int)SideOrder.FRONT;
					}

					return idx_new;
				} else {
					// Already has a neighbor
				}
			} else {
				// Nothing selected
			}

			return -1;
		}

		public int InsertSegmentBetween(Side side0, Side side1)
		{
			// Insert the new segment (it becomes selected)
			int new_segnum = InsertSegmentSelectedSide(false);

			// Connect extruded side to side1
			JoinSides(segment[new_segnum].side[selected_side], side1);

			side0.DisableDecals();
			side1.DisableDecals();

			// Done
			return selected_segment;
		}

		public int InsertSegment4Verts(Side s, int[] other_verts)
		{
			int[] verts_new = new int[Segment.NUM_VERTS];
			// Create 4 new vertices, and use the 4 from the current side
			for (int i = 0; i < Side.NUM_VERTS; i++) {
				verts_new[i] = s.vert[i];
				verts_new[i + 4] = other_verts[i];
			}

			// Reorder verts so the new segment is oriented the same
			verts_new = ReorderVertsForSide(verts_new, (SideOrder)s.num);

			int[] neighbors_new = new int[Segment.NUM_SIDES];
			for (int i = 0; i < Segment.NUM_SIDES; i++) {
				neighbors_new[i] = -1;
			}
			neighbors_new[Utility.OppositeSide(s.num)] = s.segment.num;

			// Create the new segment
			int idx_new = CreateSegment(verts_new, neighbors_new);

			// Assign the old segments side neighbor
			s.segment.neighbor[s.num] = idx_new;

			return idx_new;
		}

		public int InsertSegmentBasic(Side s, Vector3 s_center, Vector3 s_normal, Vector3 s_right, Vector3 s_up, bool regular, int extrude_length)
		{
			int[] verts_new = new int[Segment.NUM_VERTS];
			// Create 4 new vertices, and use the 4 from the current side
			for (int i = 0; i < Side.NUM_VERTS; i++) {
				verts_new[i] = s.vert[i];
				if (regular) {
					// This means a standard-sized segment side
					verts_new[i + 4] = CreateVertex(s_center + s_normal * extrude_length + Utility.TwoVectorCorners(s_right, s_up, i) * Level.DEF_SEG_SZH);
				} else {
					verts_new[i + 4] = CreateVertex(vertex[s.vert[i]].position + s_normal * extrude_length);
				}
			}

			// Reorder verts so the new segment is oriented the same
			verts_new = ReorderVertsForSide(verts_new, (SideOrder)s.num);

			int[] neighbors_new = new int[Segment.NUM_SIDES];
			for (int i = 0; i < Segment.NUM_SIDES; i++) {
				neighbors_new[i] = -1;
			}
			neighbors_new[Utility.OppositeSide(s.num)] = s.segment.num;
			
			// Create the new segment
			int idx_new = CreateSegment(verts_new, neighbors_new);

			// Assign the old segments side neighbor
			s.segment.neighbor[s.num] = idx_new;

			return idx_new;
		}

		
		public int[] ReorderVertsForSide(int[] v, SideOrder so)
		{
			int[] nv = new int[Segment.NUM_VERTS];
			switch (so) {
				case SideOrder.FRONT:
					// No change
					for (int i = 0; i < Segment.NUM_VERTS; i++) {
						nv[i] = v[i];
					}
					break;
				case SideOrder.BACK:
					nv[0] = v[7];
					nv[1] = v[6];
					nv[2] = v[5];
					nv[3] = v[4];
					nv[4] = v[3];
					nv[5] = v[2];
					nv[6] = v[1];
					nv[7] = v[0];
					break;
				case SideOrder.LEFT:
					nv[0] = v[3];
					nv[1] = v[2];
					nv[2] = v[6];
					nv[3] = v[7];
					nv[4] = v[0];
					nv[5] = v[1];
					nv[6] = v[5];
					nv[7] = v[4];
					break;
				case SideOrder.RIGHT:
					nv[0] = v[4];
					nv[1] = v[5];
					nv[2] = v[1];
					nv[3] = v[0];
					nv[4] = v[7];
					nv[5] = v[6];
					nv[6] = v[2];
					nv[7] = v[3];
					break;
				case SideOrder.TOP:
					nv[0] = v[4]; 
					nv[1] = v[0]; 
					nv[2] = v[3];
					nv[3] = v[7];
					nv[4] = v[5];
					nv[5] = v[1];
					nv[6] = v[2];
					nv[7] = v[6];
					break;
				case SideOrder.BOTTOM:
					nv[0] = v[3];
					nv[1] = v[7];
					nv[2] = v[4];
					nv[3] = v[0];
					nv[4] = v[2];
					nv[5] = v[6];
					nv[6] = v[5];
					nv[7] = v[1];
					break;
			}

			return nv;
		}

		public void DeleteMarked(bool silent = false)
		{
			switch (editor.ActiveEditMode) {
				case EditMode.SEGMENT:
					UnTagAllVertices();

					for (int i = 0; i < MAX_SEGMENTS; i++) {
						if (segment[i].Alive && segment[i].marked) {
							segment[i].Delete();

							//Delete any doors in this segment
							foreach (Side side in segment[i].side) {
								if (side.Door != -1) {
									entity[side.Door].Delete();
								}
							}

							//Look for entities in this segment
							foreach (Entity entity in EnumerateAliveEntities()) {
								if (entity.m_segnum == i) {
									entity.m_segnum = -1;
									editor.AddOutputText("Warning: Entity " + entity.num + " now has no segment.");
								}
							}
							if (selected_segment == i) {
								selected_segment = -1;
							}
						} else {
							// Tag vertices that should not be cleaned up
							for (int j = 0; j < Segment.NUM_VERTS; j++) {
								vertex[segment[i].vert[j]].m_tag = true;
							}
						}
					}

					// Destroy untagged verts too
					for (int i = 0; i < MAX_VERTICES; i++) {
						if (vertex[i].alive && !vertex[i].m_tag) {
							vertex[i].Delete();
							if (selected_vertex == i) {
								selected_vertex = -1;
							}
						}
					}
					break;
				case EditMode.ENTITY:
					for (int i = 0; i < MAX_ENTITIES; i++) {
						if (entity[i].alive && entity[i].marked) {
							entity[i].Delete();
						}
					}
					break;
			}
		}

		public int CopyMarkedSegments(Level src)
		{
			int count = 0;
			editor.SetEditModeSilent(EditMode.SEGMENT);

			// Tag all vertices in the source level
			src.TagMarkedElementVertices();

			// Copy all the tagged vertices (keep the same index for now)
			for (int i = 0; i < MAX_VERTICES; i++) {
				vertex[i].alive = false;

				if (src.vertex[i].m_tag) {
					vertex[i].Copy(src.vertex[i], false);
				}
			}

			// Copy all the marked segments (and their sides)
			for (int i = 0; i < MAX_SEGMENTS; i++) {
				segment[i].Alive = false;

				if (src.segment[i].marked) {
					count += 1;
					segment[i].Copy(src.segment[i], false);
				}
			}

			// Clean up any neighbors that aren't alive
			for (int i = 0; i < MAX_SEGMENTS; i++) {
				if (segment[i].Alive) {
					segment[i].RemoveDeadNeighbors();
				}
			}

			//Copy entities
			for (int i = 0; i < MAX_ENTITIES; i++) {
				entity[i].alive = false;

				if (src.entity[i].alive && src.entity[i].m_segnum > -1 && src.segment[src.entity[i].m_segnum].marked) {
					entity[i].Copy(src.entity[i], false, CopyGuidControl.CloneGuid);
				}
			}

			return count;
		}

		public void CopyLevel(Level src, CopyGuidControl guid_control)
		{
			for (int i = 0; i < MAX_VERTICES; i++) {
				vertex[i].alive = false;

				if (src.vertex[i].alive) {
					vertex[i].Copy(src.vertex[i], true);
				}
			}

			for (int i = 0; i < MAX_SEGMENTS; i++) {
				segment[i].Alive = false;

				if (src.segment[i].Alive) {
					segment[i].Copy(src.segment[i], true);
				}
			}

			for (int i = 0; i < MAX_ENTITIES; i++) {
				entity[i].alive = false;

				if (src.entity[i].alive) {
					entity[i].Copy( src.entity[i], true, guid_control );
				}
			}

			CopyLevelProperties(src);
		}

		

		public int[] paste_vert = new int[MAX_VERTICES];
		public int[] paste_seg = new int[MAX_SEGMENTS];

		public void PasteSegments(Level src, bool aligned)
		{
			// Set to segment mode and unmark all of them
			editor.SetEditModeSilent(EditMode.SEGMENT);
			ToggleMarkAll(true);

			// Create all the new vertices with correct position, keeping track of the new indices
			for (int i = 0; i < MAX_VERTICES; i++) {
				if (src.vertex[i].alive) {
					if (aligned) {
						paste_vert[i] = CreateVertex(editor.AlignPasteVert(src.vertex[i].position));
					} else {
						paste_vert[i] = CreateVertex(src.vertex[i].position);
					}
				}
			}

			// Create all the new segments (copy), keeping track of the new indices
			for (int i = 0; i < MAX_SEGMENTS; i++) {
				if (src.segment[i].Alive) {
					paste_seg[i] = CreateSegment(src.segment[i].vert, src.segment[i].neighbor);
					segment[paste_seg[i]].Copy(src.segment[i], false); // All of these will be marked, for later usage and moving
				}
			}

			// Fix up the segments' neighbors to use the new indices
			// And fix up the segments' vertices to use the new indices
			for (int i = 0; i < MAX_SEGMENTS; i++) {
				if (segment[i].Alive && segment[i].marked) {
					for (int j = 0; j < Segment.NUM_SIDES; j++) {
						if (segment[i].neighbor[j] > -1) {
							segment[i].neighbor[j] = paste_seg[segment[i].neighbor[j]];
						}
						// And update the sides' vertices too
						for (int k = 0; k < Side.NUM_VERTS; k++) {
							segment[i].side[j].vert[k] = paste_vert[segment[i].side[j].vert[k]];
						}
					}

					for (int j = 0; j < Segment.NUM_VERTS; j++) {
						segment[i].vert[j] = paste_vert[segment[i].vert[j]];
					}
				}
			}
		}

		private void FixEntityLinks(Entity entity, Dictionary<Guid, Guid> guid_mapping)
		{
			if (entity.entity_props is IHasLinks) {
				EntityGuid[] links = ((IHasLinks)entity.entity_props).EntityLinks;

				for (int i = 0; i < links.Length; i++) {
					Guid new_guid;

					//If the target of the link got pasted too, then update the link
					if (guid_mapping.TryGetValue(links[i], out new_guid)) {
						links[i].m_guid = new_guid;
					}
				}
			}
		}

		//Important: This function depends on the paste_seg array to fix up entity segment numbers
		public void PasteEntities(Level src, bool aligned)
		{
			Dictionary<Guid, Guid> guid_mapping = new Dictionary<Guid, Guid>();

			//First, make a mapping of old GUID to new GUID
			foreach (Entity entity in src.EnumerateAliveEntities()) {
				guid_mapping.Add(entity.guid, Guid.NewGuid());
			}

			//Now copy objects to new level
			foreach (Entity src_entity in src.EnumerateAliveEntities()) {
				Entity dest_entity = entity[next_entity];
				GetNextEntityIndex();

				//Copy the entity and give it its new guid
				dest_entity.Copy(src_entity, false, guid_mapping[src_entity.guid]);

				//Set the new segment number of the entity
				dest_entity.m_segnum = paste_seg[src_entity.m_segnum];

				//If entity is a door, find the side and point it to new entity
				if (dest_entity.Type == EntityType.DOOR) {
					foreach (Side side in segment[dest_entity.m_segnum].side) {
						if (side.Door == src_entity.num) {
							side.Door = dest_entity.num;
							break;
						}
					}
				}

				//Move & rotate entity if required
				if (aligned) {
					dest_entity.SetPosition(editor.AlignPasteVert(dest_entity.position));
					dest_entity.Rotate(editor.SourceSideRotation.Inverted());
					dest_entity.Rotate(Matrix4.CreateRotationY(Utility.RAD_180));
					dest_entity.Rotate(editor.DestSideRotation);
				}

				//If entity has links, fix them up
				FixEntityLinks(dest_entity, guid_mapping);
			}
		}

		//When entities are not getting pasted, go through pasted segments and remove door referneces
		//Important: This function depends on the paste_seg array to fix up entity segment numbers
		public void ClearDoorReferences(Level src)
		{
			foreach (Entity src_entity in src.EnumerateAliveEntities()) {
				if (src_entity.Type == EntityType.DOOR) {
					foreach (Side side in segment[paste_seg[src_entity.m_segnum]].side) {
						if (side.Door == src_entity.num) {
							side.Door = -1;
							side.chunk_plane_order = -1;
							break;
						}
					}
				}
			}
		}

		// NOTE: This assumes the marked segments are isolated
		public void FlipMarkedSegments(Axis axis, Vector3 center)
		{
			editor.SetEditModeSilent(EditMode.SEGMENT);

			// Flip all the vertices along the select axis
			TagMarkedElementVertices();
			for (int i = 0; i < MAX_VERTICES; i++) {
				if (vertex[i].m_tag) {
					FlipVertex(vertex[i], axis, center);
				}
			}

			// Invert the segments (so they have the correct face orientation and neighbors)
			for (int i = 0; i < MAX_SEGMENTS; i++) {
				if (segment[i].Alive && segment[i].marked) {
					segment[i].Invert();
				}
			}
		}

		public void RotateMarkedSegments(Axis axis, float rot, Vector3 center)
		{
			//Get the rotation matrix
			Matrix4 rot_mat = Matrix4.Identity;
			switch (axis) {
				case Axis.X:
					rot_mat = Matrix4.CreateRotationX(rot);
					break;
				case Axis.Y:
					rot_mat = Matrix4.CreateRotationY(rot);
					break;
				case Axis.Z:
					rot_mat = Matrix4.CreateRotationZ(-rot);
					break;
			}

			//Do the rotate
			RotateMarkedSegments(rot_mat, center);
		}

		public void RotateMarkedSegments(Matrix4 rot_mat, Vector3 center)
		{
			editor.SetEditModeSilent(EditMode.SEGMENT);

			// Rotate the vertices along the select axis
			TagMarkedElementVertices();
			foreach (Vertex vert in EnumerateTaggedVertices()) {
				RotateVertex(vert, rot_mat, center);
			}

			//Rotate all the entities in the marked segments
			foreach (Entity entity in EnumerateAliveEntities()) {
				if ((entity.m_segnum != -1) && segment[entity.m_segnum].marked) {
					entity.RotateAroundPosition(center, rot_mat);
				}
			}
		}

		public void RotateMarkedSegments(Vector3 axis, float rot, Vector3 center)
		{
			Matrix4 rot_mat = Matrix4.CreateFromAxisAngle(axis, rot);
			RotateMarkedSegments(rot_mat, center);
		}

		public void FlipVertex(Vertex vtx, Axis axis, Vector3 center)
		{
			switch (axis) {
				case Axis.X:
					vtx.position.X = (vtx.position.X - center.X) * -1f + center.X;
					break;
				case Axis.Y:
					vtx.position.Y = (vtx.position.Y - center.Y) * -1f + center.Y;
					break;
				case Axis.Z:
					vtx.position.Z = (vtx.position.Z - center.Z) * -1f + center.Z;
					break;
			}
		}

		public void RotateVertex(Vertex vtx, Matrix4 rot_mat, Vector3 center)
		{
			vtx.position -= center;
			vtx.position = Vector3.Transform(vtx.position, rot_mat);
			vtx.position += center;
		}

		public bool MarkedSegmentsAreIsolated()
		{
			editor.SetEditModeSilent(EditMode.SEGMENT);
			TagMarkedElementVertices();

			for (int i = 0; i < MAX_SEGMENTS; i++) {
				if (segment[i].Alive && !segment[i].marked) {
					for (int j = 0; j < Segment.NUM_VERTS; j++) {
						if (vertex[segment[i].vert[j]].m_tag) {
							return false;
						}
					}
				}
			}

			return true;
		}
	}
}