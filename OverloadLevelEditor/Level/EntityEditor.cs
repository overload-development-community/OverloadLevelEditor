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
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using Newtonsoft.Json.Linq;
using Overload;

// ENTITY - Editor
// Editor-specific functions for Entities
// Still have a lot to do with Entities (particularly editing specific types)

namespace OverloadLevelEditor
{
	public enum CopyGuidControl
	{
		CloneGuid,
		GenerateNewGuid,
		//IgnoreGuid,				//Not used, and killing it made Copy() easier
	}

	public partial class Entity
	{
		public void Create(int segnum, Vector3 pos, Matrix4 rot, EntityType type, int subtype)
		{
			alive = true;
			marked = false;
			tag = true;
			internal_guid = Guid.NewGuid();

			m_type = type;
			m_sub_type = subtype;
			entity_props = null;
			SetEntityProps();

			m_segnum = segnum;
			m_position = pos;
			m_rotation = rot;
		}

		public void Delete()
		{
			//If this is a door, find the segment/side and remove the link to this
			if (Type == EntityType.DOOR) {      // Remove this door from side.

				bool found = false;
				foreach (Segment segment in level.EnumerateAliveSegments()) {
					foreach (Side side in segment.side) {
						if (side.Door == this.num) {
							side.Door = -1;
							found = true;
							break;
						}
					}
					if (found) {
						break;
					}
				}
			}

			level.editor.EntityListRemoveEntity(this);

			alive = false;
			marked = false;
			tag = false;
			internal_guid = Guid.Empty;

		}

		public void Copy(Entity src, bool full, CopyGuidControl guid_control)
		{
			Guid new_guid = Guid.Empty;
			if (src.alive) {
				switch (guid_control) {
					case CopyGuidControl.CloneGuid:
						new_guid = src.guid;
						break;
					case CopyGuidControl.GenerateNewGuid:
						new_guid = Guid.NewGuid();
						break;
				}
			}
			
			Copy(src, full, new_guid);
		}

		public void Copy(Entity src, bool full, Guid new_guid)
		{
			alive = src.alive;
			marked = (full ? src.marked : true);

			internal_guid = new_guid;
			m_segnum = src.m_segnum;
			m_position = src.position;
			m_rotation = src.m_rotation;
			m_multiplayer_team_association_mask = src.m_multiplayer_team_association_mask;

			m_type = src.Type;
			m_sub_type = src.SubType;
			SetEntityProps();
			Utility.CopyTo(src.entity_props, entity_props);
		}

		public void CopyProperties(Entity src, bool all_props)
		{
			System.Diagnostics.Debug.Assert(m_type == src.Type);
			m_sub_type = src.SubType;
			m_multiplayer_team_association_mask = src.m_multiplayer_team_association_mask;
			if (all_props) {
				Utility.CopyTo(src.entity_props, entity_props);
			}
		}

		//Returns true if the entity is in the segment it thinks it's in
		public bool InSegment()
		{
			return (m_segnum != -1) && level.PointInsideSegment(level.segment[m_segnum], position);
		}

		public void Move(Vector3 mv, PivotMode pivot_mode)
		{
			if (pivot_mode == PivotMode.LOCAL) { 
				mv = Vector3.Transform(mv, m_rotation);
			}
			SetPosition(m_position + mv);
		}

		public void Rotate(Matrix4 rot)
		{
			m_rotation = m_rotation * rot;
		}

		// Get the local forward/right/up and rotate around them
		public void RotateLocal(Axis axis, float amt)
		{
			Matrix4 rot = Matrix4.Identity;
			switch (axis) {
				case Axis.X:
					rot = Matrix4.CreateRotationX(amt);
					break;
				case Axis.Y:
					rot = Matrix4.CreateRotationY(amt);
					break;
				case Axis.Z:
					rot = Matrix4.CreateRotationZ(amt);
					break;
			}

			// Order matters!
			m_rotation = rot * m_rotation;
		}

		public void RotateAroundPosition(Vector3 pos, Matrix4 rot)
		{
			Vector3 diff = position - pos;
			diff = Vector3.Transform(diff, rot);
			SetPosition(pos + diff);
			Rotate(rot);
		}

		// Rotate around a center point (global axes)
		public void RotateAroundPosition(Vector3 pos, Axis axis, float amt)
		{
			Matrix4 rot = Matrix4.Identity;
			switch (axis) {
				case Axis.X:
					rot = Matrix4.CreateRotationX(amt);
					break;
				case Axis.Y:
					rot = Matrix4.CreateRotationY(amt);
					break;
				case Axis.Z:
					rot = Matrix4.CreateRotationZ(amt);
					break;
			}
			RotateAroundPosition(pos, rot);
		}

		public string[] SubTypeNames()
		{
			switch (Type) {
				case EntityType.ENEMY:
					return Enum.GetNames(typeof(Overload.EnemyType));
				case EntityType.PROP:
					return Enum.GetNames(typeof(PropSubType));
				case EntityType.ITEM:
					return Enum.GetNames(typeof(ItemSubType));
				case EntityType.DOOR:
					return Enum.GetNames(typeof(DoorSubType));
				case EntityType.SCRIPT:
					return Enum.GetNames(typeof(ScriptSubType));
				case EntityType.TRIGGER:
					return Enum.GetNames(typeof(TriggerSubType));
				case EntityType.LIGHT:
					return Enum.GetNames(typeof(LightSubType));
				case EntityType.SPECIAL:
					return Enum.GetNames(typeof(SpecialSubType));
				default:
					System.Diagnostics.Debug.Assert(false);
					return new string[0];
			}
		}
		public void FacePosition(Vector3 pos)
		{
			Vector3 dir = (pos - position);
			if (dir != Vector3.Zero) {
				dir.Normalize();
				FaceDirection(dir);
			}
		}

		public void FaceDirection(Vector3 dir)
		{
			if (Math.Abs(dir.Y) < 0.995f) {
				m_rotation = Matrix4.LookAt(Vector3.Zero, -dir, Vector3.UnitY);
			} else {
				m_rotation = Matrix4.LookAt(Vector3.Zero, -dir, Vector3.UnitZ * (-dir.Y));
			}
			m_rotation.Transpose();
		}

		public void AssignLink(int slot, Guid guid)
		{
			IHasLinks props_links = entity_props as IHasLinks;
			if (props_links != null) {
				props_links.EntityLinks[slot] = new EntityGuid(guid);
			}
		}

		public void ClearLink(int slot)
		{
			IHasLinks props_links = entity_props as IHasLinks;
			if (props_links != null) {
				props_links.EntityLinks[slot] = EntityGuid.Empty;
			}
		}

		public Guid GetLinkGUID(int slot)
		{
			IHasLinks props_links = entity_props as IHasLinks;
			return (props_links == null) ? Guid.Empty : props_links.EntityLinks[slot];
		}
	}
}