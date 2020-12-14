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

#if !OVERLOAD_LEVEL_EDITOR
using UnityEngine;
#endif

// OVERLOAD
// Add random definitions for game objects

namespace Overload
{
	// These must match what is listed in the camera_shake_data.txt file
	public enum CameraShakeType
	{
		NONE,
		EXPLODE_SMALL,
		EXPLODE_MEDIUM,
		START_BOOST,
		FIRE_DRILLER,
		FIRE_THUNDER,
		CHARGE_THUNDER,
		FIRE_IMPULSE,
		FIRE_FLAK,
		POWERUP_PICKUP,
		PLAYER_DAMAGE,
		FIRE_FALCON,
		FIRE_DEVASTATOR,
		EXPLODE_LARGE,
		REACTOR_SHAKE,
		FIRE_REFLEX,
		FIRE_CYCLONE,
		FIRE_LANCER,
		FIRE_CRUSHER,
		FIRE_MISSILE_POD,
		FIRE_HUNTER,
		FIRE_CREEPER,
		FIRE_NOVA,
		FIRE_TIMEBOMB,
		FIRE_VORTEX,
		
		NUM,
	}

	// Camera Shake Fall Off
	public enum FallOffType
	{
		SLOW, // Cos
		LINEAR, // Linear
		QUICK, // Sin - moderately fast
		QUICK2, // Squared - slightly faster
		QUICK3, // Cubed - fast
		SMOOTH, // Hermite
	}

	// Bounce behavior (projectiles)
	public enum BounceBehavior
	{
		none,
		BOUNCE_ALL,
		BOUNCE_MED,
		BOUNCE_LOW,
		BOUNCE_STOP,
		BOUNCE_AIM_LOW,
		BOUNCE_AIM_MED,
		BOUNCE_AIM_ALL,
	}

	public enum ProjSpawnPattern
	{
		RANDOM,
		RANDOM_AIM,
		SPLIT_X,
		SPLIT_Y,
		SPLIT_RANDOM,
	}
	
	public enum DoorLock
	{
		NONE,
		LEVEL1,
		LEVEL2,
		LEVEL3,
		LEVEL4,
		LOCKED,
		EXIT,	// Special message, only opens when reactor destroyed

		NUM,
	}

	public enum UnityObjectLayers
	{
		BUILTIN_0,			// Default - Fallback for things that aren't set yet (similar to Level overall)
		BUILTIN_1,			// TransparentFX - NOT USED
		BUILTIN_2,			// Ignore Raycast - NOT USED
		BUILTIN_3_AUDIO,	// Used for sound fx (but not ambient sounds or music or voice)
		BUILTIN_4,			// Water - NOT USED
		BUILTIN_5,			// UI
		BUILTIN_6,
		BUILTIN_7,
		DEBRIS,				// Floating debris (broken off robot pieces, etc)
		PLAYER_LEVEL,		// Level collider for player (ONE per ship)
		ITEMS,				// Items (vs. player_level)
		ENEMY_MESH,			// Enemy mesh
		PLAYER_PROJ,		// Player projectile collider (against enemy mesh colliders)
		ENEMY_PROJ,			// Enemy projectile collider (against player)
		LEVEL,				// Level geometry
		ENEMY_LEVEL,		// Sphere collider for enemies (ONE per enemy)
		PLAYER_MESH,		// Player mesh (currently a sphere, smaller than level collider)
		DOOR,             // Any solid non-level object, such as doors and props, that should not render with level (collides with robots, player, and projectiles) (RENAMING SHOULD HELP CONFUSION)
		DESTROYABLE,		// Destroyable object "mesh" (like enemy_mesh but for explosions), only ONE per destroyable (other parts can be marked enemy_mesh)
		PLAYER_TRIGGER,	// For triggers that only the player can activate (both weapons and ship, filtered by script)
		PROJ_LEVEL,			// Level collider for projectiles (usually smaller than the standard collider)
		ITEM_LEVEL,			// Level collider for items
		PLAYER_PROJ_COL,	// Collides with other projectiles on this layer, but otherwise should be same as Player_Proj
		KINETICFIELD,		// Collides with projectiles but not player/robots (RENAMING SHOULD HELP CONFUSION)
		AREA_EFFECT,		// Wind, and other things that only affect player/robots
		LENS_FLARES,		// Lens flares (uses ProFlares)
		LAVA,					// Explosive
		RP_IGNORE,			// Reflection probes ignore all objects in this layer
		OVERLAY_UI,
		BRIEFING,			// Robot briefing camera and object
		AUDIO_AMBIENCE,
		MONSTERBALL,		// Monsterball
	}

	public enum ItemType
	{
		NONE,
		POWERUP_SHIELD,
		POWERUP_ENERGY,
		POWERUP_AMMO,

		WEAPON_DRILLER,
		WEAPON_FLAK,
		WEAPON_CYCLONE,
		WEAPON_THUNDERBOLT,
		WEAPON_REFLEX,
		WEAPON_IMPULSE,
		WEAPON_SHOTGUN,

		MISSILE_FALCON,
		MISSILE_POD,
		MISSILE_HUNTER,
		MISSILE_CREEPER,
		MISSILE_SMART,
		MISSILE_DEVASTATOR,
		MISSILE_TIMEBOMB,
		MISSILE_VORTEX,

		TEMP_INVULN,
		TEMP_CLOAK,
		TEMP_RAPID,

		UPGRADE_L1,
		UPGRADE_L2,

		COLLECTIBLE,

		KEY_SECURITY,

		WEAPON_LANCER,
		
		LOG_ENTRY,
		POWERUP_ALIEN_ORB,

		NUM
	}

	// Can only choose 2A or 2B, they are not sequential
	public enum WeaponUnlock
	{
		LOCKED,
		LEVEL_0,
		LEVEL_1,
		LEVEL_2A,
		LEVEL_2B,

		NUM,
	}

	// Player stat upgrade categories
	public enum UpgradeType
	{
		ARMOR,
		ENERGY,
		AMMO,
		MOVE_SPEED,

		// Level 1 only
		SMASH_DAMAGE,
		SELF_REDUCTION,
		BOOST_SPEED,
		BOOST_HEATSINK,
		ACCESSORY_ENERGY,
		ITEM_DURATION,

		NUM,
	}

	// For players only
	public enum WeaponType
	{
		// Player (by default)
		IMPULSE,
		CYCLONE,
		REFLEX,
		CRUSHER,
		DRILLER,
		FLAK,
		THUNDERBOLT,
		LANCER,
		
		NUM,
	}

	// For players only
	public enum MissileType
	{
		// Player (by default)
		FALCON,
		MISSILE_POD,
		HUNTER,
		CREEPER,
		NOVA,
		DEVASTATOR,
		TIMEBOMB,
		VORTEX,

		NUM,
	}

	// Added by MK on 2018-04-02
	// For multiplayer super powerup spawning.
	public enum SuperType
	{
		// Missiles must be first, then others, then NUM
		FALCON,
		MISSILE_POD,
		HUNTER,
		CREEPER,
		NOVA,
		DEVASTATOR,
		TIMEBOMB,
		VORTEX,

		INVULNERABILITY,
		CLOAK,
		OVERDRIVE,

		NUM,
	}

	// Wheel weapon selection state (affects UI and gametime)
	public enum WheelSelectState
	{
		NONE,
		WEAPON,
		MISSILE,
		GUIDEBOT,	// Singleplayer-only
		QUICK_CHAT,	// Multiplayer-only

		NUM,
	}

	public enum CMUpgrade
	{
      STARTING_ARMOR,
		BOOST_SPEED,
		ACCESSORY_ENERGY,
		BLAST_DAMAGE,
		WEAPONS_UNLOCK_1,
		AMMO_BOOST,
		ITEM_DURATION,
		EXTRA_WEAPON,
		SMASH_DAMAGE,
		WEAPONS_UNLOCK_2,

		NUM,
	}

	public enum BonusStat
	{
		DAMAGE_PHYSICAL,  // +1% - 5
		DAMAGE_ENERGY,    // +1% - 5
		DAMAGE_EXPLOSIVE, // +1% - 5

		STARTING_ARMOR,   // +2 - 5
		STARTING_ENERGY,  // +5 - 5
		STARTING_AMMO,    // +20 - 5
		STARTING_MISSILE, // +1 pack - 3

		ENERGY_REGENERATION, // +0.1/kill - 3
		ARMOR_REGENERATION, // +0.1/kill - 3
		
		SHIP_SPEED,       // +1% - 5
		BOOST_SPEED,      // +1% - 5
		BOOST_DURATION,   // +5% - 3
		BOOST_RECHARGE,   // +5% - 3

		PROTECTION_SELF,     // -15% - 5
		PROTECTION_PHYSICAL, // -1% - 5
		PROTECTION_ENERGY,   // -1% - 5
		PROTECTION_EXPLOSIVE,// -1% - 5

		ACCESSORY_ENERGY,    // -33% - 3

		NUM,
	}

	public enum ProjTeam
	{
		PLAYER,	// Shot by the player
		ENEMY,	// Shot by a robot
		MP,		// Shot by *another* player

		NUM,
	}

	public enum EnemyType
	{
		GRUNTA,
		GRUNTB,
		RECOILA,
		RECOILB,
		CLAWBOTA,
		CLAWBOTB,
		HULKA,
		HULKB,
		DETONATORA,
		DETONATORB,
		BLADESA,
		DRONEA,
		DRONEB,
		CANNONA,
		CANNONB,
		VIPERA,
		VIPERB,
		TELEPORTERA,
		TELEPORTERB,

		BOSS1,
		BOSS2,
		BOSS3,

		// Second stage of boss robots
		BOSS1B,
		BOSS2B,
		BOSS3B,

		GUIDEBOT,
		
		// More prototypes (unused currently)
		//PREACTORA, // This should no longer be used - keeping to avoid breakage
		//PBLADESB,
		//PSPIDERA,
		//PSPIDERB,
		//PMINI_SPIDERA,
		
		NUM,
	}

    // AI modes
    public enum AIModeType
    {
		 ASLEEP,
		 SEARCHING,			// Became aware of player, but does not know where player is and player might not be visible, perhaps due to FOV.
		 APPROACH,
		 CHARGE,
		 BACKOFF,
		 CIRCLE,
		 PATHFIND,
		 STATION,
		 ZIGZAG,				// Chase random rays.
		 GETBEHIND,			// Get behind target.
		 AVOIDFRONT,
		 DRIFT,
		 STAYBACK,
		 STILL,
		 ZOOM,
		 //ZOOMPAST,
		 //ZOOMAWAY,
		 RUNAWAY,
		 DYING,
		 GRIND,
		 AMBUSH,
		 SNIPER,
		 DETONATOR,
		 CLAW,
		 COWARD,
		 BOSS1,
		 BOSS2,
		 BOSS3,
		 STATUE,				// For robots that are not supposed to move.  Luke used STILL, but that's a legal temporary mode for robots.
		 NULL,				// Not in any mode.  Needed for AI_next_mode.
    }

	// Added for AMBUSH mode
	public enum AISubmodeType
	{
		NULL,					// Unused, but defaults to 0 or NULL.
		ASLEEP,				// for DETONATOR
		LURKING,				// for AMBUSH + SNIPER
		INITIAL_ATTACK,	// for AMBUSH
		ATTACK,				// for AMBUSH + CHARGE + SNIPER + DETONATOR
		WAIT,					// for CHARGE + DETONATOR
		PATHFIND,			// for SNIPER
		BACKUP,				// for SNIPER
		BACKUP2,				// to handle animation states in DoModeClaw()s
		WANDER,				// for DETONATOR
		COAST,				// Coast to a stop for a short period of time.
		PATHFIND_TO_PLAYER,	// Only used in challenge mode for the initial pathfind to the player.

		// BOSS submodes
		SPRAYFIRE_START,		// Boss1 sprays fire left to right (and maybe reverse) in player's reference frame, forcing vertical dodge.  This submode gets to alignment
		SPRAYFIRE_SPRAY,		// Now spray!
		SPRAYFIRE_BACKOFF,	// Sprayed!  Now backoff
		DEVASTATOR_START,
		DEVASTATOR_SCREAM,
		DEVASTATOR_FIRE,
		DEVASTATOR_EXIT,
		BOSS_ZIGZAG,			// Zigzag without firing, short-lived
		BOSS_ZIGZAG_FIRE1,	// Zigzag, continuous primary fire
		BOSS_CHARGE_DEVASTATOR1,	// Zigzag, burst of mini devastators

		BOSS_CIRCLE,
		BOSS_CIRCLE2,			// Used by Boss2, in circle2 submode fires creepers
		BOSS_STILL,
		BOSS_CHARGE,
		BOSS_HIDE,				// Boss2 sneaks away.
		BOSS_PREPARE_TO_CLOAK,	// Boss2 will cloak, but we don't want it to happen as soon as it triggers

		BOSS3_LANCER_CHARGEUP,         // Super attack featuring...Lancer!
		BOSS3_LANCER_FIRE,         // Super attack featuring...Lancer!
		BOSS3_VORTEX,	// ...Thunderbolt
		BOSS3_MINIONS,			// ...MINIONS!  Actually spawned detonators.
		BOSS3_REGULAR,			// A non-super attack
		ZOOM_PAST,
		ZOOM_AWAY,
	}

	public enum AIGuidebotSubmodeType
	{
		NONE,
		START,
		NAPPING,
		GO_TO_KEY,
		GO_TO_SECURITY_DOOR,
		GO_TO_PLAYER,
		GO_AWAY,
		GO_TO_REACTOR_OR_BOSS,
		GO_TO_ROBOT,
		GO_TO_ARMOR,
		GO_TO_ENERGY_OR_AMMO,
		GO_TO_POWERUP,
		GO_TO_CRYOTUBE,
		GO_TO_EXIT,
		GO_TO_ENERGY_CENTER,
		WANDER_NEAR_PLAYER,
		GO_TO_SURFACE,
		GO_TO_ALIEN_SHIP,
	}

	// Firing pattern distribution of where shots originate
	public enum FiringDistribution
	{
		SEQUENTIAL,
		SINGLE_BURST,
		RANDOM,
		PAIRED_SEQ,
		PAIRED_BURST,
		CUSTOM,
	}

	//// TODO: Expand this to the full list of songs
	//public enum MusicClip
	//{
	//	MENU01,
	//	EXIT01,
	//	EXIT02,
	//	GAMEPLAY01,
	//	GAMEPLAY02,
	//	GAMEPLAY03,
	//	GAMEPLAY04,
	//	GAMEPLAY05,
	//	GAMEPLAY06,
	//	GAMEPLAY07,
	//	GAMEPLAY08,
	//	BOSS01,
		
	//	NUM_MUSIC_CLIPS
	//}

	// For messages that only show once per ID
	public enum MessageID
	{
		NONE,
		LEVEL1_DOOR,
		LEVEL2_DOOR,
		LEVEL3_DOOR,
		LEVEL4_DOOR,
		EXIT_DOOR,
		ITEM_INVULN,
		ITEM_RAPID,
		ITEM_CLOAK,
		ITEM_ENERGY,
		ITEM_AMMO,
		ITEM_ARMOR,
		// These should match the missile order exactly
		ITEM_MISSILE_FALCON, 
		ITEM_MISSILE_MISSILE_POD,
		ITEM_MISSILE_HUNTER,
		ITEM_MISSILE_CREEPER,
		ITEM_MISSILE_NOVA,
		ITEM_MISSILE_DEVASTATOR,
		ITEM_MISSILE_TIMEBOMB,
		ITEM_MISSILE_VORTEX,
	}

	// 
	public enum DamageType
	{
		GENERIC,
		EXPLOSIVE,
		ENERGY,
		PLAYER_CHARGE,

		NUM,
	}

#if !OVERLOAD_LEVEL_EDITOR
	public struct DamageInfo
	{
		public GameObject owner;
		public float damage;
		public float stun_multiplier;
		public float push_force;
		public float push_torque;
		public Vector3 pos;
		public Vector3 push_dir;
		public DamageType type;
		public ProjPrefab weapon;     // the weapon that did the damage
		public bool force_death;      // ensure this robot or whatever DIES!
		public Robot robot_owner;
	}

	//For arrays of localizable strings
	public abstract class StringListType
	{
		protected string[] m_values = null;

		public StringListType()
		{
			Loc.LanguageChanged += OnLanguageChanged;
			Refresh();
		}

		//Called when the language changes
		private void OnLanguageChanged(object sender, System.EventArgs args)
		{
			Refresh();
		}

		//Called to populate the array when the language changes
		protected abstract void Refresh();

		//Return a value from the array
		public string this[int type] { get { return m_values[type]; } }
		public string this[System.Enum e] { get { return m_values[System.Convert.ToInt32(e)]; } }
	}
#endif
}
