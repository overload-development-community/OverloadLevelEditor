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
using Newtonsoft.Json.Linq;

namespace OverloadLevelEditor
{
	public enum EntityType
	{
		ENEMY,
		PROP,
		ITEM,
		DOOR,
		SCRIPT,
		TRIGGER,
		LIGHT,
		SPECIAL,

		NUM,
	}

	// Possible Properties for each type:
	// Enemy: sub_type, item/enemy drop, AI
	// Prop: sub_type, item/enemy drop, behavior
	// Item: sub_type, ?
	// Door: sub_type, locked
	// Script: sub_type, link 1/2/etc, counter, timer
	// Trigger: sub_type, size, one-time
	// Light: sub_type, angle, range, R/G/B, intensity
	// Special: sub_type, ?

	public enum PropSubType
	{
		N0000_MINE,
		N0000_CONTAINER,
		POWER_CORE,
		REACTOR_OM,	// Outer moons reactor (8m tall)
		REACTOR_TN, // Titan reactor (12m tall)
		N0000_SWITCH_TIMED,
		N0000_SWITCH_TOGGLE,
		SWITCH_ONETIME,
		FORCE_FIELD,
		FORCE_FIELD_ALIEN,
		CRYOTUBE,
		CRYOTUBE_EMPTY,
		EMLIGHT_OM1,
		N0000_KINETIC_FIELD,
		TELEPORTER,
		EC_GRATE,
		EC_ENTRANCE,
		MONITOR_OM1,
		MONITOR_OM2,
		MONITOR_OM3,
		MONITOR_SECRET1,
		MONITOR_SECRET2,
		MONITOR_TN1,
		MONITOR_TN2,
		MONITOR_TN3,
		FAN_OM1,	// 4x4m
		FAN_OM2,	// 8x8m
		FAN_TN1,	// 4x4m
		FAN_TN2,	// 8x8m
		FAN_TN3, // 4x4m
		FAN_TN4, // 4x4m
		N0000_FAN_TN_CORNER,
		REACTORTURRET_TN,
		ALIEN_POWER,
		ALIEN_SWITCH,
		ALIEN_WARP,
		ALIEN_STASIS,
		ALIEN_CONTAINER,
		ALIEN_SOCKET,
		REACTOR_OM16,	// Alien-ized OM reactor
		MP_CAMERA,

		NUM,
	}

	public enum ItemSubType
	{
		SHIELDS,
		ENERGY,
		AMMO,

		FALCON,
		MISSILE_POD,
		HUNTER,
		CREEPER,
		NOVA,
		DEVASTATOR,
		TIMEBOMB,
		VORTEX,

		DRILLER,
		FLAK,
		THUNDERBOLT,
		IMPULSE,
		CYCLONE,
		REFLEX,
		CRUSHER,
		LANCER,

		INVULN,
		CLOAK,
		RAPID,

		UPGRADE_L1,
		UPGRADE_L2,

		N0000_COLLECTIBLE,
		CM_SPAWN,

		SECURITY_KEY,

		LOG_ENTRY,
		ALIEN_ORB,

		NUM,
	}

	public enum DoorSubType
	{
		OM1,
		OM2,
		OM_SECURITY1,
		OM_EXIT,
		FOUNDRY1,
		TITAN2,
		TITAN_WIDE1,
		CC1,
		EXIT,

		ALIEN1,
		ALIEN2,

		SECRET_OMCEILING03D,
		SECRET_OMFLOOR03A,
		SECRET_OMFLOOR06M,
		SECRET_OMWALL18A,
		SECRET_OMWALL19A,
		SECRET_OM_WALL_20A,
		SECRET_OM_WALL_42B,

		SECRET_INDMETALCEILING01C,
		SECRET_INDMETALCEILING01I,
		SECRET_INDMETALPANEL01L,
		SECRET_INDMETALWALL01G,
		SECRET_INDMETALWALL01H,
		SECRET_INDMETALWALL03C,
		SECRET_INDMETALWALL12D,
		SECRET_INDMETALWALL13D,
		SECRET_INDSTONEFLOOR01A,
		SECRET_INDSTONEFLOOR01B,

		SECRET_ALIEN_CEILING_10D,
		SECRET_ALIEN_CEILING_13B,
		SECRET_ALIEN_FLOOR_01A,
		SECRET_ALIEN_FLOOR_02B,
		SECRET_ALIEN_FLOOR_09A,
		SECRET_ALIEN_WALLS_02B,
		SECRET_ALIEN_WALLS_09A,
		SECRET_ALIEN_WALLS_09B,
		SECRET_ALIEN_WALLS_09C,
		SECRET_ALIEN_WALLS_09E,

		FOUNDRYSECRET01A,
		FACTORY1,
		FACTORYSECRET1,
		INDSECRET1,
        
        USERDOOR1,
        USERDOOR2,
        USERDOOR3,
        USERDOOR4,
        USERDOOR5,
        USERDOOR6,
        USERDOOR7,
        USERDOOR8,
        USERDOOR9,

        NUM,
	}

	public enum ScriptSubType
	{
		DOOR_UNLOCK,
		DOOR_OPEN,
		ACTIVATE_MATCEN,
		ON_DESTROY,
		ON_COUNT,
		DISABLE_SHIELD,
		FORCEFIELD_DISABLE,
		REVEAL_ROBOT,
		DOOR_LOCK,
		ON_PICKUP,
		LOCKDOWN_MASTER,
		LOCKDOWN_DOOR,
		LOCKDOWN_LIGHT,
		LOCKDOWN_ROBOT,
		LOCKDOWN_BOSS,
		TUTORIAL_MESSAGE,
		ACTIVATE_OBJECT,
		DEACTIVATE_OBJECT,
		ANALYTICS_CHOKEPOINT,
		ROBOT_ATTACK,
		CHECKPOINT_SAVE,
		COMM_MESSAGE,
		TELEPORT_OUT,
		ON_ROBOT_KILLS,
		LEVEL1,
		LEVEL12,
		LEVEL16,
		ACTIVATE_ALIEN_WARP,
		ALIEN_DOOR_LINK,
		SECRET_LEVEL,
		FADE_MUSIC,
		OBJECTIVE_MESSAGE,
		HOLOGUIDE_POSITION,
		DISABLE_OCCLUSION,
		NUM,
	}

	public enum TriggerSubType
	{
		BOX,
		BOX_EXIT,
		BOX_STAY,
		BOX_ENERGY,
		BOX_WINDTUNNEL,
		BOX_WARPER,
		BOX_LAVA_NORMAL,
		BOX_LAVA_ALIEN,
		SPHERE,
		SPHERE_EXIT,
		SPHERE_STAY,
		REFLECTION_PROBE,
		
		NUM,
	}

	public enum LightSubType
	{
		POINT,
		SPOT,
		NO_SHADOW,
		SPOT_NO_SHADOW,

		NUM,
	}

	public enum SpecialSubType
	{
		PLAYER_START,
		ROBOT_SPAWN_POINT,
		MATCEN,

		NUM,
	}

	public partial class Entity
	{
		private Vector3 m_position;
		public Matrix4 m_rotation;
		private EntityType m_type;
		private int m_sub_type;
		public Overload.EntityProps entity_props;

		public int m_segnum;

		public int m_multiplayer_team_association_mask; // Each bit corresponds to a permitted team (leave 0 to mean "all teams" - default). Bit0 - team 0 can make use of, Bit1 - team 1 can make use of, etc.

		public bool alive;
		public bool marked;

		public bool tag; // For level functions

		public int num;

		private Guid internal_guid;

		public Level level;

		public Guid reference_guid; // This is an ephemeral value only to be used for special user level converters

		public Entity(Level lvl, int n)
		{
			level = lvl;

			m_type = EntityType.ENEMY;
			m_multiplayer_team_association_mask = 0;
			m_sub_type = 0;
			alive = false;
			marked = false;
			tag = false;
			num = n;
			m_position = Vector3.Zero;
			m_rotation = Matrix4.Identity;
			entity_props = null;
			internal_guid = Guid.Empty;
			reference_guid = Guid.Empty;
		}

		public void SetPosition(Vector3 pos, int segnum)
		{
			m_position = pos;
			m_segnum = segnum;
		}

#if OVERLOAD_LEVEL_EDITOR
		public void SetPosition(Vector3 pos)
		{
			m_position = pos;

			//See if has gone out of segment and if so look for new segment
			if (!InSegment()) {
				int old_segnum = m_segnum;
				m_segnum = level.FindSegmentForPoint(position);
				if (m_segnum == -1) {
					level.editor.AddOutputText("Entity " + num + " not in segment");
				}
				else {
					level.editor.AddOutputText("Entity " + num + " moved from segment " + old_segnum + " to " + m_segnum);
				}
			}
		}
#endif

		public Vector3 position { get { return m_position; } }

		public Matrix4 rotation { get { return m_rotation; } }

		//Entity is visible it it's alive and the segment it belongs to is visible
		public bool Visible { get { return (alive && ((this.m_segnum == -1) || this.level.segment[this.m_segnum].Visible)); } }

		public EntityType Type {
			get { return m_type; }
		}

		public int SubType {
			get { return m_sub_type; }

			set {
				m_sub_type = value;
#if OVERLOAD_LEVEL_EDITOR
				if (level == level.editor.LoadedLevel) {  //don't update if undo level
					level.editor.EntityListUpdateEntity(this);
				}
#endif
			}
		}

		public Guid guid {
			get { return internal_guid; }
			// no 'set' -- trying to prevent accidental changing of the guid
		}

		Overload.EntityProps CreateNewEntityProps<T>() where T : Overload.EntityProps, new()
		{
			return new T();
		}

		//Sets the entity's enity_props based on the type of the entity
		public void SetEntityProps()
		{
			switch (Type) {
				default:
				case EntityType.ENEMY:
					entity_props = CreateNewEntityProps<Overload.EntityPropsRobot>();
					break;
				case EntityType.DOOR:
					entity_props = CreateNewEntityProps<Overload.EntityPropsDoor>();
					break;
				case EntityType.ITEM:
					entity_props = CreateNewEntityProps<Overload.EntityPropsItem>();
					break;
				case EntityType.LIGHT:
					entity_props = CreateNewEntityProps<Overload.EntityPropsLight>();
					break;
				case EntityType.PROP:
					entity_props = CreateNewEntityProps<Overload.EntityPropsProp>();
					break;
				case EntityType.SCRIPT:
					entity_props = CreateNewEntityProps<Overload.EntityPropsScript>();
					break;
				case EntityType.SPECIAL:
					entity_props = CreateNewEntityProps<Overload.EntityPropsSpecial>();
					break;
				case EntityType.TRIGGER:
					entity_props = CreateNewEntityProps<Overload.EntityPropsTrigger>();
					break;
			}
		}

		public int NumSubTypes()
		{
			switch (Type) {
				case EntityType.ENEMY:
					return (int)Overload.EnemyType.NUM;
				case EntityType.PROP:
					return (int)PropSubType.NUM;
				case EntityType.ITEM:
					return (int)ItemSubType.NUM;
				case EntityType.DOOR:
					return (int)DoorSubType.NUM;
				case EntityType.SCRIPT:
					return (int)ScriptSubType.NUM;
				case EntityType.TRIGGER:
					return (int)TriggerSubType.NUM;
				case EntityType.LIGHT:
					return (int)LightSubType.NUM;
				case EntityType.SPECIAL:
					return (int)SpecialSubType.NUM;
				default:
					return 0;
			}
		}

		public void Serialize(JObject root)
		{
			root["guid"] = this.internal_guid.ToString();
			root["ref_guid"] = this.reference_guid.ToString();
			root["position"] = this.position.Serialize();
			root["rotation"] = this.m_rotation.Serialize();
			root["segnum"] = this.m_segnum.ToString();
			root["type"] = this.Type.ToString();
			root["sub_type"] = SubTypeName(); // note: Use the string name of the enum so future enum changes are handled and sub_types don't mysteriously change
			root["mp_team"] = this.m_multiplayer_team_association_mask.ToString();

			var e_props = new JObject();
			var serializer = new EntityPropsSerializer();
			serializer.Serialize(entity_props, e_props);

			root["properties"] = e_props;
		}

		public void Deserialize(JObject root)
		{
			this.alive = true;
			this.marked = false;

			// Older versions of serialized Entity does not
			// have Guids defined. Detect those cases and
			// generate the Guid at deserialization
			var guid_type_obj = root["guid"];
			if (guid_type_obj != null && guid_type_obj.Type == JTokenType.String) {
				this.internal_guid = guid_type_obj.GetGuid(Guid.Empty);
			} else {
				// Generate a Guid
				this.internal_guid = Guid.NewGuid();
			}

			this.reference_guid = root["ref_guid"].GetGuid(Guid.Empty);
			this.m_position = root["position"].GetVector3();
			this.m_rotation = root["rotation"].GetMatrix4();
			this.m_segnum = root["segnum"].GetInt(-1);
			this.m_multiplayer_team_association_mask = root["mp_team"].GetInt(0);
			this.m_type = root["type"].GetEnum<EntityType>(EntityType.ENEMY);
			SetEntityProps();

			var sub_type_obj = root["sub_type"];
			if (sub_type_obj.Type == JTokenType.String) {
				this.SubType = GetSubTypeByName(sub_type_obj.GetString());
			} else {
				this.SubType = sub_type_obj.GetInt(0);
			}

			var serializer = new EntityPropsSerializer();
			serializer.Deserialize(entity_props, root["properties"].GetObject());
		}

		public Object GetEntityProps()
		{
			switch (Type) {
				default:
					return null;
				case EntityType.ENEMY:
					return (Overload.EntityPropsRobot)entity_props;
				case EntityType.DOOR:
					return (Overload.EntityPropsDoor)entity_props;
				case EntityType.ITEM:
					return (Overload.EntityPropsItem)entity_props;
				case EntityType.LIGHT:
					return (Overload.EntityPropsLight)entity_props;
				case EntityType.PROP:
					return (Overload.EntityPropsProp)entity_props;
				case EntityType.SCRIPT:
					return (Overload.EntityPropsScript)entity_props;
				case EntityType.SPECIAL:
					return (Overload.EntityPropsSpecial)entity_props;
				case EntityType.TRIGGER:
					return (Overload.EntityPropsTrigger)entity_props;
			}
		}

		public void DeserializeComplete()
		{
		}

		public System.Type GetTypeEnumType()
		{
			switch (this.Type) {
				case EntityType.ENEMY:
					return typeof(Overload.EnemyType);
				case EntityType.PROP:
					return typeof(PropSubType);
				case EntityType.ITEM:
					return typeof(ItemSubType);
				case EntityType.DOOR:
					return typeof(DoorSubType);
				case EntityType.SCRIPT:
					return typeof(ScriptSubType);
				case EntityType.TRIGGER:
					return typeof(TriggerSubType);
				case EntityType.LIGHT:
					return typeof(LightSubType);
				case EntityType.SPECIAL:
					return typeof(SpecialSubType);
				default:
					return null;
			}
		}

		public string SubTypeName()
		{
			var enum_type = GetTypeEnumType();
			if (enum_type == null) {
				return "NONE";
			}

			return Enum.GetName(enum_type, this.SubType);
		}

		public int GetSubTypeByName(string subtype_name)
		{
			var enum_type = GetTypeEnumType();
			if (enum_type == null) {
				System.Diagnostics.Debugger.Break();
				return 0;
			}

			try {
				return (int)Enum.Parse(enum_type, subtype_name, true);
			}
			catch {
				return 0;
			}
		}
	}
}
