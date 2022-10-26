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

using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OverloadLevelExport;

public static class LevelConvertExt
{
	public static string ToSnakeCase(this string input_str)
	{
		// Translate "CamelCase" to "camel_case"
		var sb = new StringBuilder();

		bool had_lowercase = false;
		for (int i = 0; i < input_str.Length; ++i) {
			char input_ch = input_str[i];

			if (Char.IsUpper(input_ch)) {
				if (had_lowercase) {
					sb.Append('_');
				}
				sb.Append(Char.ToLowerInvariant(input_ch));
			} else {
				if (input_ch >= 'a' && input_ch <= 'z') {
					had_lowercase = true;
				}
				sb.Append(input_ch);
			}
		}

		return sb.ToString();
	}
}

public partial class OverloadLevelConverter
{
	public partial class LevelConvertState
	{
		void PlaceEntities(ISceneBroker scene, OverloadLevelEditor.Level overloadLevelData, bool isNewLevel, IGameObjectBroker sceneLevelContainerObject, Dictionary<int, int> doorNumToPortalMap)
		{
			List<LevelData.SpawnPoint> item_spawn_points = new List<LevelData.SpawnPoint>();
			List<LevelData.SpawnPoint> player_spawn_points = new List<LevelData.SpawnPoint>();
			List<LevelData.SpawnPoint> mp_camera_points = new List<LevelData.SpawnPoint>();

			bool found_player_start = false;

			// Create a container object to reduce clutter
			IGameObjectBroker entity_container_object = null;
			{
				if (!isNewLevel) {
					// Look for the existing container object
					for (int child_idx = 0, num_children = sceneLevelContainerObject.Transform.ChildCount; child_idx < num_children; ++child_idx) {
						var test_object = sceneLevelContainerObject.Transform.GetChild(child_idx).ownerGameObject;
						if (test_object.Name == s_specialEntityContainerObjectName) {
							entity_container_object = test_object;
							break;
						}
					}

					if (entity_container_object != null) {
						// Clean out all the old entities
						while (entity_container_object.Transform.ChildCount > 0) {
							scene.DestroyGameObject(entity_container_object.Transform.GetChild(0).ownerGameObject);
						}
					}
				}

				if (entity_container_object == null) {
					entity_container_object = scene.CreateRootGameObject(s_specialEntityContainerObjectName);
					entity_container_object.Transform.Parent = sceneLevelContainerObject.Transform;
				}
			}

			// Get the valid entity container types
			string[] entity_category_names = Enumerable.Range(0, (int)OverloadLevelEditor.EntityType.NUM)
				.Select(idx => Enum.GetName(typeof(OverloadLevelEditor.EntityType), (OverloadLevelEditor.EntityType)idx))
				.ToArray();

			// Create the container categories
			IGameObjectBroker[] entity_category_objects = new IGameObjectBroker[entity_category_names.Length];
			for (int i = 0, count = entity_category_names.Length; i < count; ++i) {
				var entity_category = entity_category_names[i];
				entity_category_objects[i] = scene.CreateRootGameObject(entity_category);
				entity_category_objects[i].Transform.Parent = entity_container_object.Transform;
			}

			var cached_prefab_map = new Dictionary<OverloadLevelEditor.EntityType, Dictionary<int, IGameObjectBroker>>();
			bool added_player_ship = false;

#if OVERLOAD_LEVEL_EDITOR
			var map_guid_to_gameobject = new Dictionary<System.Guid, IGameObjectBroker>();
#else
			var map_guid_to_gameobject = new Dictionary<string, IGameObjectBroker>();
#endif
			var deferred_properties_list = new List<KeyValuePair<IGameObjectBroker, OverloadLevelEditor.Entity>>();

			// Called when a door/portal link is found
			List<LevelData.PortalDoorConnection> portal_door_references = new List<LevelData.PortalDoorConnection>();
			Action<int, IComponentBroker> link_portal_to_door = (int portal_index, IComponentBroker door) => {
				portal_door_references.Add(new LevelData.PortalDoorConnection() {
					PortalIndex = portal_index,
#if OVERLOAD_LEVEL_EDITOR
					ReferenceDoor = door,
#else
					ReferenceDoor = door.InternalObject as DoorBase,
#endif
				});
			};

			// Called to resolve a DropType to a prefab
			/*Func<Overload.DropTypes, GameObject> resolve_droptype_prefab = (Overload.DropTypes drop_type) => {
				string prefab_name = ItemPrefabs[drop_type];

				// Construct a prefab name: "entity_item_<enum_name>"
				string enum_as_string = drop_type.ToString();
				string enum_as_snake_case_string = enum_as_string.ToSnakeCase();
				string enum_as_lower_string = enum_as_string.ToLowerInvariant();
				string[] search_prefab_names = new string[] {
					prefab_name,
					"entity_item_" + enum_as_lower_string,
					"entity_item_" + enum_as_snake_case_string,
				};

				foreach (var search_prefab_name in search_prefab_names) {
					var prefab_match_guids = AssetDatabase.FindAssets(search_prefab_name + " t:GameObject");
					if (prefab_match_guids != null && prefab_match_guids.Length != 0) {
						// TODO(Jeff): If there are multiple options - should we pick the one that matches the name the closest?
						var entity_prefab_path = AssetDatabase.GUIDToAssetPath(prefab_match_guids[0]);
						return AssetDatabase.LoadAssetAtPath(entity_prefab_path, typeof(GameObject)) as GameObject;
					}
				}

				return null;
			};*/

			foreach (int entity_idx in overloadLevelData.EnumerateAliveEntityIndices()) {
				var entity_src = overloadLevelData.entity[entity_idx];
				var entity_orient = OpenTKExtensions.OpenTKQuaternion.ExtractRotation(entity_src.rotation).ToUnity();

				//Look for robot spawn points
				if ((entity_src.Type == OverloadLevelEditor.EntityType.SPECIAL) && (entity_src.SubType == (int)OverloadLevelEditor.SpecialSubType.ROBOT_SPAWN_POINT)) {
					// Robot spawn points -- handled separately
					continue;
				} else if ((entity_src.Type == OverloadLevelEditor.EntityType.SPECIAL) && (entity_src.SubType == (int)OverloadLevelEditor.SpecialSubType.PLAYER_START)) {
					if (!found_player_start) {
						// First player spawn point (still added to list)
						player_spawn_points.Add(new LevelData.SpawnPoint(entity_src.position.ToUnity(), entity_orient, entity_src.m_multiplayer_team_association_mask));
						found_player_start = true;
					} else {
						// (Alternative) Player spawn points
						player_spawn_points.Add(new LevelData.SpawnPoint(entity_src.position.ToUnity(), entity_orient, entity_src.m_multiplayer_team_association_mask));
						continue;
					}
				} else if ((entity_src.Type == OverloadLevelEditor.EntityType.ITEM) && (entity_src.SubType == (int)OverloadLevelEditor.ItemSubType.CM_SPAWN)) {
					// Item spawn points (uses "super" flag for team bit = 1, otherwise team bit = 0)
					item_spawn_points.Add(new LevelData.SpawnPoint(entity_src.position.ToUnity(), entity_orient, ((Overload.EntityPropsItem)entity_src.entity_props).super ? 1 : 0));
					continue;
				} else if ((entity_src.Type == OverloadLevelEditor.EntityType.PROP) && (entity_src.SubType == (int)OverloadLevelEditor.PropSubType.MP_CAMERA)) {
					// Item spawn points
					mp_camera_points.Add(new LevelData.SpawnPoint(entity_src.position.ToUnity(), entity_orient, 0));
					continue;
				}
				if(entity_src.Type == OverloadLevelEditor.EntityType.TRIGGER && entity_src.SubType == (int)OverloadLevelEditor.TriggerSubType.REFLECTION_PROBE)
				{
					// reflection probe -- handled separately
					continue;
				}

				// Resolve the prefab for the entity
				Dictionary<int, IGameObjectBroker> cache_subtype_map;
				if (!cached_prefab_map.TryGetValue(entity_src.Type, out cache_subtype_map)) {
					cache_subtype_map = new Dictionary<int, IGameObjectBroker>();
					cached_prefab_map.Add(entity_src.Type, cache_subtype_map);
				}

				// SPECIAL CASE: If the subtype is set to 'NUM' (max value) then assume that it is being set internally
				// for level conversion purposes. In that case, the name of the prefab will be specially named and will
				// have been defined before the conversion process.
				bool cacheIt = true;
				string entity_prefab_name;
				if (entity_src.NumSubTypes() == entity_src.SubType) {
					// This is the special path
					entity_prefab_name = string.Format("$INTERNAL$:{0}", entity_src.reference_guid.ToString());
					cacheIt = false;
				} else {
					// This is the normal path
					entity_prefab_name = string.Format("entity_{0}_{1}", entity_src.Type.ToString(), entity_src.SubTypeName());
				}

				IGameObjectBroker cached_game_object_prefab;
				if (!cache_subtype_map.TryGetValue(entity_src.SubType, out cached_game_object_prefab)) {
					cached_game_object_prefab = scene.FindAndLoadPrefabAsset(entity_prefab_name);
					if (cached_game_object_prefab == null) {
						Debug.LogError(string.Format("Unable to resolve prefab named '{0}'", entity_prefab_name));
					}

					if (cacheIt) {
						cache_subtype_map.Add(entity_src.SubType, cached_game_object_prefab);
					}
				}

				if (cached_game_object_prefab == null) {
					// Failed to find the Prefab
					continue;
				}

				var entity_position = entity_src.position.ToUnity();
				var entity_instance = scene.InstantiatePrefab(cached_game_object_prefab);
				if (entity_instance == null) {
					// Failed to instantiate the Prefab
					continue;
				}
				entity_instance.Transform.Position = entity_position;
				entity_instance.Transform.Rotation = entity_orient;

				if ((int)entity_src.Type < entity_category_objects.Length && entity_category_objects[(int)entity_src.Type] != null) {
					// put under the correct category
					entity_instance.Transform.Parent = entity_category_objects[(int)entity_src.Type].Transform;
				} else {
					// fallback to the container
					entity_instance.Transform.Parent = entity_container_object.Transform;
				}

				string desired_entity_name;
				if (entity_src.Type == OverloadLevelEditor.EntityType.SPECIAL && entity_src.SubType == (int)OverloadLevelEditor.SpecialSubType.PLAYER_START && !added_player_ship) {
					// Name the player ship to the name scripts are looking for
					desired_entity_name = "PlayerShip";
					added_player_ship = true;
				} else {
#if OVERLOAD_LEVEL_EDITOR
					desired_entity_name = entity_prefab_name.ToLowerInvariant();
#else
					desired_entity_name = cached_game_object_prefab.Name;
#endif
				}
				entity_instance.Name = desired_entity_name;

				// Remember this object by its Guid for when we apply properties
				var entity_guid = new Overload.EntityGuid(entity_src.guid);
				map_guid_to_gameobject.Add(entity_guid.m_guid, entity_instance);

				// Add this for later
				deferred_properties_list.Add(new KeyValuePair<IGameObjectBroker, OverloadLevelEditor.Entity>(entity_instance, entity_src));
			}

			Action<IEnumerable<Overload.EntityGuid>, IComponentBroker, string> resolve_entity_links = (IEnumerable<Overload.EntityGuid> src_links, IComponentBroker comp, string propertyName) => {
				List<IGameObjectBroker> dst_links = new List<IGameObjectBroker>();
				foreach (var src_link in src_links) {
					if (src_link == Overload.EntityGuid.Empty) {
						// Skip over the unknowns
						continue;
					}

					// Resolve
					IGameObjectBroker go;
					if (!map_guid_to_gameobject.TryGetValue(src_link.m_guid, out go)) {
						Debug.LogError(string.Format("Unable to resolve GUID: {0}", src_link.m_guid));
					} else {
						dst_links.Add(go);
					}
				}
#if OVERLOAD_LEVEL_EDITOR
				comp.SetProperty(propertyName, dst_links);
#else
				List<GameObject> dst_links_as_go = dst_links.Select(gob => gob.InternalObject).ToList();
				comp.SetProperty(propertyName, dst_links_as_go);
#endif
			};

			// Apply all of the properties now that we know all of the entities and their GUIDs
			foreach (var kvo in deferred_properties_list) {
				IGameObjectBroker entity_instance = kvo.Key;
				OverloadLevelEditor.Entity entity_src = kvo.Value;

				//string category_name = entity_category_names[(int)entity_src.Type];
				Overload.EntityProps entity_props = entity_src.entity_props;
				Overload.EntityPropsType entity_props_type = entity_props.GetPropsType();

				// Apply the properties now
				switch (entity_props_type) {

					case Overload.EntityPropsType.Door: {
							var e_door = entity_instance.GetComponentInChildren("DoorBase");
							var p_door = (Overload.EntityPropsDoor)entity_props;

							int portal_index;
							if (!doorNumToPortalMap.TryGetValue(entity_src.num, out portal_index)) {
								Debug.LogError(string.Format("Unable to resolve portal from door (Entity {0})", entity_src.num));
								portal_index = -1;
							}

							if (p_door.TriggerDepth.HasValue)
                            {
								entity_instance.GetComponentInChildren("DoorAnimating").SetProperty<float>("m_player_trigger_depth", p_door.TriggerDepth.Value);
							}

							e_door.SetProperty("LockType", p_door.m_door_lock);
							e_door.SetProperty("Portal", portal_index);
							e_door.SetProperty("NoChunk", p_door.m_no_chunk);
							link_portal_to_door(portal_index, e_door);
						} break;

					case Overload.EntityPropsType.Robot: {
							var e_robot = entity_instance.GetComponentInChildren("Robot");
							var p_robot = (Overload.EntityPropsRobot)entity_props;
							e_robot.SetProperty("AI_robot_station", p_robot.station);
							e_robot.SetProperty("m_headlight_on", p_robot.headlight);
							e_robot.SetProperty("m_init_super", p_robot.super);
							e_robot.SetProperty("m_init_variant", p_robot.variant);
							e_robot.SetProperty("m_init_ng_plus", p_robot.ng_plus);
							e_robot.SetProperty("m_init_hidden", p_robot.hidden);
							e_robot.SetProperty("m_init_stasis", p_robot.stasis);

							e_robot.SetProperty("m_bonus_drop1", p_robot.bonus_drop1);
							e_robot.SetProperty("m_bonus_drop2", p_robot.bonus_drop2);
							e_robot.SetProperty("m_replace_default_drop", p_robot.replace_default_drop);
						} break;

					case Overload.EntityPropsType.Item: {
							var e_item = entity_instance.GetComponentInChildren("Item");
							var p_item = (Overload.EntityPropsItem)entity_props;
							e_item.SetProperty("m_respawning", p_item.respawns);
							e_item.SetProperty("m_super", p_item.super);
							e_item.SetProperty("m_secret", p_item.secret);
							e_item.SetProperty("m_index", p_item.index);
						} break;

					case Overload.EntityPropsType.Light: {
							var e_light = entity_instance.GetComponentInChildren("Light");
							var p_light = (Overload.EntityPropsLight)entity_props;
							e_light.SetProperty("spotAngle", p_light.angle);
							e_light.SetProperty("range", p_light.range);
							e_light.SetProperty("intensity", p_light.intensity);
							e_light.SetProperty("color", HSBColor.ConvertToColor(p_light.c_hue, p_light.c_sat, p_light.c_bri));
						} break;

					case Overload.EntityPropsType.Prop: {
							var e_prop = entity_instance.GetComponentInChildren("PropBase");
							var p_prop = (Overload.EntityPropsProp)entity_props;

							e_prop.SetProperty("m_invulnerable", p_prop.invulnerable);
							e_prop.SetProperty("m_index", p_prop.index);
							e_prop.SetProperty("NoChunk", p_prop.m_no_chunk);
						} break;

					case Overload.EntityPropsType.Script: {
							var e_script = entity_instance.GetComponentInChildren("ScriptBase");
							var p_script = (Overload.EntityPropsScript)entity_props;
							e_script.SetProperty("delay", p_script.delay);
							e_script.SetProperty("show_message", p_script.show_message);
							e_script.SetProperty("special_index", p_script.special_index);
							resolve_entity_links(p_script.entity_link, e_script, "c_go_link");
						} break;

					case Overload.EntityPropsType.Special: {
							var e_special = entity_instance.GetComponentInChildren("RobotMatcen");
							var p_special = (Overload.EntityPropsSpecial)entity_props;
							float spawn_prob1 = (float)p_special.matcen_spawn_probability_1;
							float spawn_prob2 = (float)p_special.matcen_spawn_probability_2;
							if (spawn_prob1 + spawn_prob2 == 0f) {   // Don't allow them both to be zero, likely only happen to matcens that were placed before probability support.
								spawn_prob1 = 1f;
								spawn_prob2 = 1f;
							}

							e_special.SetProperty("m_special_props", p_special.special_props);
							e_special.SetProperty("m_spawnable_robot_1", (int)p_special.matcen_spawn_type_1);
							e_special.SetProperty("m_spawnable_probability_1", spawn_prob1);
							e_special.SetProperty("m_spawnable_robot_2", (int)p_special.matcen_spawn_type_2);
							e_special.SetProperty("m_spawnable_probability_2", spawn_prob2);
							e_special.SetProperty("m_matcen_max_robots_alive", p_special.m_max_alive);
							e_special.SetProperty("m_matcen_spawn_wait", (float)p_special.m_spawn_wait);
							e_special.SetProperty("m_invulnerable", (bool)p_special.ed_invulnerable);
						} break;

					case Overload.EntityPropsType.Trigger: {
							// NOTE: On Export a Trigger is only going to have one of
							// these components, but we don't want to do if/else here
							// because that would mess up LevelEditor export. When doing
							// LevelEditor export, an IGameObjectBroker will have all components
							// queried, so it will set all of these properties. At runtime, when
							// loading the exported level, it will see the component missing and
							// will just skip over setting those properties. If you have an
							// if/else that would mess up this process.
							var p_trigger = (Overload.EntityPropsTrigger)entity_props;
							var e_trigger = entity_instance.GetComponentInChildren("TriggerBase");
							e_trigger.SetProperty("m_one_time", p_trigger.one_time);
							e_trigger.SetProperty("m_repeat_delay", p_trigger.repeat_delay);
							e_trigger.SetProperty("m_player_weapons", p_trigger.player_weapons);
							e_trigger.SetProperty("m_size", p_trigger.size);
							resolve_entity_links(p_trigger.entity_link, e_trigger, "c_go_link");

							// Energy center special case
							var e_ec = entity_instance.GetComponentInChildren("TriggerEnergy");
							e_ec.SetProperty("m_size", p_trigger.size);
							
							// Ambient particle special case
							if(entity_src.SubType == (int)OverloadLevelEditor.TriggerSubType.BOX_LAVA_NORMAL 
								|| entity_src.SubType == (int)OverloadLevelEditor.TriggerSubType.BOX_LAVA_ALIEN)
							{
								// Ambient particle entities should have this component but don't
								var e_ap = entity_instance.AddComponent("TriggerAmbientParticles");
								e_ap.SetProperty("m_repeat_delay", p_trigger.repeat_delay);
								e_ap.SetProperty("m_size", p_trigger.size);
							}
							
							// Wind tunnel special case
							var e_wt = entity_instance.GetComponentInChildren("TriggerWindTunnel");
							e_wt.SetProperty("m_one_time", p_trigger.one_time);
							e_wt.SetProperty("m_repeat_delay", p_trigger.repeat_delay);
							e_wt.SetProperty("m_player_weapons", p_trigger.player_weapons);
							e_wt.SetProperty("m_size", p_trigger.size);
							resolve_entity_links(p_trigger.entity_link, e_wt, "c_go_link");
						} break;
				}
			}

			// Certain data can't be stored in the level meta data - in particular anything that has
			// references to GameObjects and such that are in the scene (an asset can't reference objects
			// in a Unity scene)
			IComponentBroker level_object_initializer = sceneLevelContainerObject.GetComponent("LevelData");
			level_object_initializer.SetProperty("m_robot_spawn_points", overloadLevelData.ExtractRobotSpawnPoints().Select(loc => new LevelData.SpawnPoint(loc.Pos.ToUnity(), loc.Rotation.ToUnity(), 0)).ToArray());
			level_object_initializer.SetProperty("m_player_spawn_points", player_spawn_points.ToArray());
			level_object_initializer.SetProperty("m_item_spawn_points", item_spawn_points.ToArray());
			level_object_initializer.SetProperty("m_mp_camera_points", mp_camera_points.ToArray());
			level_object_initializer.SetProperty("m_portal_to_door_references", portal_door_references.ToArray());
		}
	}
}
