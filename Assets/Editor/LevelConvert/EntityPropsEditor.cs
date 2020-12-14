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

using System.Linq;
using Newtonsoft.Json.Linq;
using Overload;

namespace OverloadLevelEditor
{
	public class EntityPropsSerializer
	{
		public void Serialize( EntityProps entity_props, JObject serialization_obj )
		{
			// Dispatch to the correct function
			var final_type = entity_props.GetType();
			var method = GetType()
				.GetMethods()
				.Where( mi => mi.Name == "Serialize" )
				.Select( mi => new { MethodInfo = mi, Parameters = mi.GetParameters() } )
				.Where( info => info.Parameters.Length == 2 && info.Parameters[ 0 ].ParameterType == final_type && info.Parameters[ 1 ].ParameterType == typeof( JObject ) )
				.Select( info => info.MethodInfo )
				.Single();
			method.Invoke( this, new object[] { entity_props, serialization_obj } );
		}

		public void Deserialize( EntityProps entity_props, JObject serialization_obj )
		{
			// Dispatch to the correct function
			var final_type = entity_props.GetType();
			var method = GetType()
				.GetMethods()
				.Where( mi => mi.Name == "Deserialize" )
				.Select( mi => new { MethodInfo = mi, Parameters = mi.GetParameters() } )
				.Where( info => info.Parameters.Length == 2 && info.Parameters[ 0 ].ParameterType == final_type && info.Parameters[ 1 ].ParameterType == typeof( JObject ) )
				.Select( info => info.MethodInfo )
				.Single();
			method.Invoke( this, new object[] { entity_props, serialization_obj } );
		}

		// ROBOT
		public void Serialize( EntityPropsRobot entity_props, JObject root )
		{
			root[ "ai_override" ] = entity_props.ai_override.ToString();
			root["station"] = entity_props.station.ToString();
			root["headlight"] = entity_props.headlight.ToString();
			root["ng_plus"] = entity_props.ng_plus.ToString();
			root["super"] = entity_props.super.ToString();
			root["variant"] = entity_props.variant.ToString();
			root["hidden"] = entity_props.hidden.ToString();
			root["stasis"] = entity_props.stasis.ToString();

			root["bonus_drop1"] = entity_props.bonus_drop1.ToString();
			root["bonus_drop2"] = entity_props.bonus_drop2.ToString();
			root["replace_default_drop"] = entity_props.replace_default_drop.ToString();
		}

		public void Deserialize( EntityPropsRobot entity_props, JObject root )
		{
			entity_props.ai_override = root["ai_override"].GetInt();
			entity_props.station = root["station"].GetBool();
			entity_props.headlight = root["headlight"].GetBool();	
			entity_props.ng_plus = root["ng_plus"].GetBool();
			entity_props.super = root["super"].GetBool();
			entity_props.variant = root["variant"].GetBool();
			entity_props.hidden = root["hidden"].GetBool();
			entity_props.stasis = root["stasis"].GetBool();

			entity_props.bonus_drop1 = root["bonus_drop1"].GetEnum<ItemPrefab>(ItemPrefab.none);
			entity_props.bonus_drop2 = root["bonus_drop2"].GetEnum<ItemPrefab>(ItemPrefab.none);
			entity_props.replace_default_drop = root["replace_default_drop"].GetBool();
		}

		// DOOR
		public void Serialize( EntityPropsDoor entity_props, JObject root )
		{
			root["door_lock"] = ((int)entity_props.m_door_lock).ToString();
			root["robot_access"] = entity_props.m_robot_access.ToString();
			root["no_chunk"] = entity_props.m_no_chunk.ToString();
		}

		public void Deserialize( EntityPropsDoor entity_props, JObject root )
		{
			entity_props.m_door_lock = root["door_lock"].GetEnum<DoorLock>(DoorLock.NONE);
			entity_props.m_robot_access = root["robot_access"].GetBool();
			entity_props.m_no_chunk = root["no_chunk"].GetBool();
		}

		// ITEM
		public void Serialize( EntityPropsItem entity_props, JObject root )
		{
			root["respawns"] = entity_props.respawns.ToString();
			root["super"] = entity_props.super.ToString();
			root["secret"] = entity_props.secret.ToString();
			root["index"] = entity_props.index.ToString();
		}

		public void Deserialize( EntityPropsItem entity_props, JObject root )
		{
			entity_props.respawns = root["respawns"].GetBool();
			entity_props.super = root["super"].GetBool();
			entity_props.secret = root["secret"].GetBool();
			entity_props.index = root["index"].GetInt();
		}

		// PROP
		public void Serialize( EntityPropsProp entity_props, JObject root )
		{
			root["invulnerable"] = entity_props.invulnerable.ToString();
			root["m_no_chunk"] = entity_props.m_no_chunk.ToString();
			root["index"] = entity_props.index.ToString();
		}

		public void Deserialize( EntityPropsProp entity_props, JObject root )
		{
			entity_props.invulnerable = root["invulnerable"].GetBool();
			entity_props.m_no_chunk = root["m_no_chunk"].GetBool();
			entity_props.index = root["index"].GetInt();
		}

		// SCRIPT
		public void Serialize( EntityPropsScript entity_props, JObject root )
		{
			for( int i = 0; i < EntityPropsScript.MAX_LINKS; i++ ) {
				root[ "entity_link" + i.ToString() ] = entity_props.entity_link[ i ].ToString();
			}
			root[ "delay" ] = entity_props.delay.ToString();
			root["show_message"] = entity_props.show_message.ToString();
			root["special_index"] = entity_props.special_index.ToString();
		}

		public void Deserialize( EntityPropsScript entity_props, JObject root )
		{
			for( int i = 0; i < EntityPropsScript.MAX_LINKS; i++ ) {
            entity_props.entity_link[ i ] = root[ "entity_link" + i.ToString() ].GetEntityGuid();
#if OVERLOAD_LEVEL_EDITOR
				Utility.DebugLog("Script Deserialize " + root["entity_link" + i.ToString()] + " converted to " + entity_props.entity_link[i]);
#endif
         }
			entity_props.delay = root[ "delay" ].GetFloat();
			entity_props.show_message = root["show_message"].GetBool();
			entity_props.special_index = root["special_index"].GetInt();
		}

		// TRIGGER
		public void Serialize( EntityPropsTrigger entity_props, JObject root )
		{
			for (int i = 0; i < EntityPropsTrigger.MAX_LINKS; i++) {
				root["entity_link" + i.ToString()] = entity_props.entity_link[i].ToString();
			}
			root["onetime"] = entity_props.one_time.ToString();
			root[ "repeat_delay" ] = entity_props.repeat_delay.ToString();
			root["player_weapons"] = entity_props.player_weapons.ToString();
#if !OVERLOAD_LEVEL_EDITOR
			root[ "size" ] = entity_props.size.ToOpenTK().Serialize();
#else
			root[ "size" ] = entity_props.size.Serialize();
#endif
		}

		public void Deserialize( EntityPropsTrigger entity_props, JObject root )
		{
			for (int i = 0; i < EntityPropsTrigger.MAX_LINKS; i++) {
				entity_props.entity_link[i] = root["entity_link" + i.ToString()].GetEntityGuid();
#if OVERLOAD_LEVEL_EDITOR
				Utility.DebugLog("Trigger Deserialize " + root["entity_link" + i.ToString()] + " converted to " + entity_props.entity_link[i]);
#endif
			}
			entity_props.one_time = root["onetime"].GetBool();
			entity_props.repeat_delay = root[ "repeat_delay" ].GetFloat();
			entity_props.player_weapons = root["player_weapons"].GetBool();
#if !OVERLOAD_LEVEL_EDITOR
			entity_props.size = root[ "size" ].GetVector3().ToUnity();
#else
			entity_props.size = root[ "size" ].GetVector3();
#endif
		}

		// LIGHT
		public void Serialize( EntityPropsLight entity_props, JObject root )
		{
			root["angle"] = entity_props.angle.ToString();
			root[ "range" ] = entity_props.range.ToString();
			root[ "intensity" ] = entity_props.intensity.ToString();
			root[ "shadows" ] = entity_props.shadows.ToString();
			root[ "c_hue" ] = entity_props.c_hue.ToString();
			root[ "c_sat" ] = entity_props.c_sat.ToString();
			root[ "c_bri" ] = entity_props.c_bri.ToString();
		}

		public void Deserialize( EntityPropsLight entity_props, JObject root )
		{
			entity_props.angle = root["angle"].GetFloat();
			entity_props.range = root[ "range" ].GetFloat();
			entity_props.intensity = root[ "intensity" ].GetFloat();
			entity_props.shadows = root[ "shadows" ].GetBool();
			entity_props.c_hue = root[ "c_hue" ].GetFloat();
			entity_props.c_sat = root[ "c_sat" ].GetFloat();
			entity_props.c_bri = root[ "c_bri" ].GetFloat();

#if OVERLOAD_LEVEL_EDITOR
			entity_props.UpdateColor();
#endif
		}

		// SPECIAL
		public void Serialize( EntityPropsSpecial entity_props, JObject root )
		{
			root["special_props"] = entity_props.special_props.ToString();
			root["matcen_spawn_type_1"] = entity_props.matcen_spawn_type_1.ToString();
			root["matcen_spawn_probability_1"] = entity_props.matcen_spawn_probability_1.ToString();
			root["matcen_spawn_type_2"] = entity_props.matcen_spawn_type_2.ToString();
			root["matcen_spawn_probability_2"] = entity_props.matcen_spawn_probability_2.ToString();
			root["m_max_alive"] = entity_props.m_max_alive.ToString();
			root["m_spawn_wait"] = entity_props.m_spawn_wait.ToString();
			root["ed_invulnerable"] = entity_props.ed_invulnerable.ToString();
		}

		public void Deserialize(EntityPropsSpecial entity_props, JObject root )
		{
			entity_props.special_props = root["special_props"].GetEnum<MatcenSpecialProperties>(MatcenSpecialProperties.NONE); ;
			entity_props.matcen_spawn_type_1 = root["matcen_spawn_type_1"].GetEnum<EnemyType>(EnemyType.RECOILA);
			entity_props.matcen_spawn_probability_1 = root["matcen_spawn_probability_1"].GetFloat();
			entity_props.matcen_spawn_type_2 = root["matcen_spawn_type_2"].GetEnum<EnemyType>(EnemyType.RECOILA);
			entity_props.matcen_spawn_probability_2 = root["matcen_spawn_probability_2"].GetFloat();
			entity_props.m_max_alive = root["m_max_alive"].GetInt();
			entity_props.m_spawn_wait = root["m_spawn_wait"].GetEnum<MatcenSpawnWait>(MatcenSpawnWait.MEDIUM);
			entity_props.ed_invulnerable = root["ed_invulnerable"].GetBool();
		}
	}

}
