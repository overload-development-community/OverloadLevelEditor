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
using System.Diagnostics;

// LEVEL - Add and Delete
// Level functions that deal with entities
// (These are editor-only, not for game)
// (Some functions could possibly be moved into this sub-file from other Level sub-files)

namespace OverloadLevelEditor
{
	public partial class Level
	{
		public int CreateEntity(int segnum, Vector3 pos, Matrix4 rot, EntityType type, int subtype, bool select = true)
		{
			if (next_entity > -1) {
				int idx = next_entity;
				entity[idx].Create(segnum, pos, rot, type, subtype);
				GetNextEntityIndex();
				if (select) {
					selected_entity = idx;
				}
				return idx;
			} else {
				Utility.DebugPopup("Max level entities reached");
				return -1;
			}
		}

		public int CreateEntity(int segnum, Vector3 pos, Vector3 dir, EntityType type, int subtype, bool select = true)
		{
			int idx = CreateEntity(segnum, pos, Matrix4.Identity, type, subtype, select);
			if (idx > -1) {
				entity[idx].FaceDirection(dir);
			}
			return idx;
		}

		public void GetNextEntityIndex()
		{
			for (int i = 0; i < MAX_ENTITIES; i++) {
				next_entity = (next_entity + 1) % MAX_ENTITIES;
				if (!entity[next_entity].alive) {
					return;
				}
			}

			// Nothing found
			next_vertex = -1;
		}

		public void SetSelectedEntitySubType(int index)
		{
			Entity entity = GetSelectedEntity();
			if (entity != null) {
				entity.SubType = index;
			}
		}

		public void SetMarkedRobotsStation(bool station)
		{
			List<Entity> e_list = GetMarkedEntities(true);

			if (e_list.Count > 0) {
				for (int i = 0; i < e_list.Count; i++) {
					if (e_list[i].Type == EntityType.ENEMY) {
						((Overload.EntityPropsRobot)e_list[i].entity_props).station = station;
               }
				}
			}
		}

		public void SetMarkedRobotsNGP(bool ngp)
		{
			List<Entity> e_list = GetMarkedEntities(true);

			if (e_list.Count > 0) {
				for (int i = 0; i < e_list.Count; i++) {
					if (e_list[i].Type == EntityType.ENEMY) {
						((Overload.EntityPropsRobot)e_list[i].entity_props).ng_plus = ngp;
					}
				}
			}
		}

		//Resets rotation of selected entity
		public void EntityResetRotation()
		{
			Entity e = GetSelectedEntity();
			if (e != null) {
				e.m_rotation = Matrix4.Identity;
			}
		}

		//Resets rotation of marked entities
		public int EntityMarkedResetRotation()
		{
			List<Entity> e_list = GetMarkedEntities();

			int count = 0;
			if (e_list.Count > 0) {
				for (int i = 0; i < e_list.Count; i++) {
					e_list[i].m_rotation = Matrix4.Identity;
				}
			}
			return count;
		}

		public void EntityFaceSelectedSide()
		{
			Entity e = GetSelectedEntity();
			if (e != null && selected_segment > -1 && selected_side > -1) {
				e.FacePosition(GetSelectedSidePos());
			}
		}

		public void EntityCycleTeam()
		{
			Entity e = GetSelectedEntity();
			if (e != null) {
				// This cycles through ALL (0), A (1), and B (2)
				e.m_multiplayer_team_association_mask = (e.m_multiplayer_team_association_mask + 1) % 3;
			}
		}

		public int EntityMarkedFaceSelectedSide()
		{
			List<Entity> e_list = GetMarkedEntities();

			int count = 0;
			if ((e_list.Count > 0) && (selected_segment > -1) && (selected_side > -1)) {
				for (int i = 0; i < e_list.Count; i++) {
					e_list[i].FacePosition(GetSelectedSidePos());
					count++;
				}
			}
			return count;
		}

		public void EntityMoveToSide()
		{
			Entity e = GetSelectedEntity();
			if (e != null && selected_segment > -1 && selected_side > -1) {
				Side side = GetSelectedSide();

				//If moving a door, point the side to the door & set the split plane (if not already split), and clear door's old side
				if (e.Type == EntityType.DOOR) {

					//Clear door from old segment side
					foreach (Side s in this.segment[e.m_segnum].side) {
						if (s.Door == e.num) {
							s.Door = -1;
							break;
						}
					}
					Debug.Assert(side.Door == -1);
					side.Door = e.num;
					if (side.chunk_plane_order == -1) {
						side.chunk_plane_order = 0;
					}
				}

				e.SetPosition(side.FindCenterBetter(), selected_segment);
				e.FaceDirection(side.FindNormal());
			}
		}

		public void EntityAlignToSide()
		{
			Entity e = GetSelectedEntity();
			if (e != null && selected_segment > -1 && selected_side > -1) {
				e.m_rotation = GetSelectedSideOrientation();
			}
		}

		public void EntityMoveToPos(Vector3 pos)
		{
			Entity e = GetSelectedEntity();
			if (e != null) {
				int segnum = FindSegmentForPoint(pos);
				if (segnum == -1) {
					editor.AddOutputText("Entity " + e.num + " not in a segment");
				}
				e.SetPosition(pos, segnum);
			}
		}

		public void EntityMoveToSegment()
		{
			Entity e = GetSelectedEntity();
			if (e != null && selected_segment > -1 && selected_side > -1) {
				Debug.Assert(e.Type != EntityType.DOOR);
				e.SetPosition(GetSelectedSegmentPos(), selected_segment);
				e.FacePosition(GetSelectedSidePos());
			}
		}

		public List<Entity> GetMarkedEntities(bool or_selected = false)
		{
			List<Entity> e_list = new List<Entity>();

			for (int i = 0; i < MAX_ENTITIES; i++) {
				if (entity[i].alive && entity[i].marked) {
					e_list.Add(entity[i]);
				}
			}

			if (e_list.Count == 0 && or_selected) {
				if (selected_entity > -1 && selected_entity < entity.Length) {
					e_list.Add(entity[selected_entity]);
				}
			}

			return e_list;
		}

		public int MarkAllEntities(bool mark)
		{
			int count = 0;
			for (int i = 0; i < MAX_ENTITIES; i++) {
				entity[i].marked = mark;
				count++;
			}
			return count;
		}

		public int DuplicateMarkedEntities()
		{
			int idx, count = 0;
			Dictionary<Guid, Guid> guid_mapping = new Dictionary<Guid, Guid>();

			//First, make a mapping of old GUID to new GUID
			foreach (Entity entity in GetMarkedEntities()) {
				guid_mapping.Add(entity.guid, Guid.NewGuid());
			}

			//Duplicate
			foreach (Entity entity in GetMarkedEntities()) {
				idx = next_entity;
				GetNextEntityIndex();
				if (idx > -1) {
					this.entity[idx].Copy(entity, false, guid_mapping[entity.guid]);
					FixEntityLinks(this.entity[idx], guid_mapping);
					count++;
				}
				entity.marked = false;
			}

			return count;
		}

		public Entity FindEntityWithGUID(string id, bool show_warning)
		{
			return FindEntityWithGUID(Guid.Parse(id), show_warning);
		}

		public Entity FindEntityWithGUID(Guid id, bool show_warning)
		{
			for (int i = 0; i < MAX_ENTITIES; i++) {
				if (entity[i].alive && entity[i].guid == id) {
					return entity[i];
				}
			}

#if OVERLOAD_LEVEL_EDITOR
			if (show_warning) {
				Utility.DebugPopup(string.Format("Could not find the entity with GUID: {0}", id.ToPrettyString()));
			}
#endif
			return null;
		}

		public int CopyEntityPropertiesToMarked(Entity src, bool all_props)
		{
			int count = 0;
			foreach (Entity entity in GetMarkedEntities()) {
				if ((entity != src) && (entity.Type == src.Type)) {
					entity.CopyProperties(src, all_props);
					count++;
				}
			}
			return count;
		}

		public int MarkEntitiesOfType(EntityType et)
		{
			int count = 0;
			for (int i = 0; i < MAX_ENTITIES; i++) {
				if (entity[i].alive && entity[i].Type == et) {
					entity[i].marked = true;
					count++;
				}
			}
			return count;
		}

		public int MarkEntitiesOfSubtype(EntityType et, int st)
		{
			int count = 0;
			for (int i = 0; i < MAX_ENTITIES; i++) {
				if (entity[i].alive && entity[i].Type == et && entity[i].SubType == st) {
					entity[i].marked = true;
					count++;
				}
			}
			return count;
		}
	}
}