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

using System.Text;
using System.IO;
using UnityEngine;
using System;

namespace Overload
{
	public class LeaderboardEntry
	{
		public LeaderboardEntry(int rank, int score, string name)
		{
			m_rank = rank;
			m_score = score;
			m_name = name;
		}

		public LeaderboardEntry()
		{
		}

		public int m_score;
		public int m_rank;
		public int m_kills;
		public string m_name;
#if UNITY_XBOXONE
		public string m_xuid;
#endif
		public int m_game_time;
		public int m_favorite_weapon;
		public DateTime m_time_stamp;
		public bool m_data_is_valid;
	}

	// Yield class for using coroutines
	public class CloudDataYield : UnityEngine.CustomYieldInstruction
	{
		Func<bool> m_keep_waiting_func;

		public override bool keepWaiting {
			get {
				return m_keep_waiting_func();
			}
		}

		public CloudDataYield(Func<bool> keep_waiting_func)
		{
			m_keep_waiting_func = keep_waiting_func;
		}
	}

	//Various functions and values that vary by platform
	public static class Platform
	{
		private static string persistentDataPath {
			get {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
				return @"\\?\" + Application.persistentDataPath.Replace('/', '\\');     //Use UNC path to avoid errors on reserved filenames like "con" and "lpt1"
#else
				return Application.persistentDataPath;
#endif
			}
		}

		public enum MountPointType { Pilot, SavedGame, Temp }
		public enum MountPointMode { ReadOnly, ReadWrite, Create }

		//For PS4.  Does nothing on PC and Xbox
		public static bool OpenMountPoint(MountPointType type, MountPointMode mode, bool async = false)
		{
			return OpenMountPoint(type, 0, mode, async);
		}

		//For PS4.  Does nothing on PC and Xbox
		public static bool OpenMountPoint(MountPointType type, int index, MountPointMode mode, bool async = false)
		{
#if UNITY_PS4
			return PS4Manager.OpenMountPoint(type, index, mode, async);
#else
			return true;
#endif
		}

		//For PS4.  Does nothing on PC and Xbox
		public static void CloseMountPoint()
		{
#if UNITY_PS4
			PS4Manager.CloseMountPoint();
#endif
		}

		//Read a chunk of data from the per-user storage
		public static byte[] ReadBinaryUserData(string filename)
		{
#if UNITY_XBOXONE && !UNITY_EDITOR
			return XboxOneConnectedStorage.GetData(filename);
#elif UNITY_PS4 && !UNITY_EDITOR
			return PS4Manager.ReadData(filename);	
#else
			try {
				return File.ReadAllBytes(Path.Combine(persistentDataPath, filename));
			}
			catch {
				return null;
			}
#endif
		}

		//Read a chunk of data from the shared storage
		public static byte[] ReadBinarySharedData(string filename)
		{
			try {
				return File.ReadAllBytes(Path.Combine(persistentDataPath, filename));
			}
			catch {
				return null;
			}
		}

		public static string ReadTextUserData(string filename)
		{
#if UNITY_XBOXONE && !UNITY_EDITOR
			byte [] bytes = XboxOneConnectedStorage.GetData(filename);
			if (bytes == null) {
				return null;
			} else {
				return new string(Encoding.ASCII.GetChars(bytes));
			}
#elif UNITY_PS4 && !UNITY_EDITOR
			byte [] bytes = PS4Manager.ReadData(filename);
			if (bytes == null) {
				return null;
			} else {
				return new string(Encoding.ASCII.GetChars(bytes));
			}
#else
			try {
				return File.ReadAllText(Path.Combine(persistentDataPath, filename));
			}
			catch {
				return null;
			}
#endif
		}

		//Write a chunk of data to the per-user storage
		public static void WriteBinaryUserData(string filename, byte[] data)
		{
#if UNITY_XBOXONE && !UNITY_EDITOR
			XboxOneConnectedStorage.SaveData(filename, data);
#elif UNITY_PS4
			PS4Manager.WriteData(filename, data);
#else
			File.WriteAllBytes(Path.Combine(persistentDataPath, filename), data);
#endif
		}

		//Write a chunk of data to the shared storage
		public static void WriteBinarySharedData(string filename, byte[] data)
		{
			File.WriteAllBytes(Path.Combine(persistentDataPath, filename), data);
		}

		//Write a string to the per-user storage
		public static void WriteTextUserData(string filename, string text)
		{
#if UNITY_XBOXONE && !UNITY_EDITOR
			XboxOneConnectedStorage.SaveData(filename, Encoding.ASCII.GetBytes(text));
#elif UNITY_PS4
			PS4Manager.WriteData(filename, Encoding.ASCII.GetBytes(text));
#else
			File.WriteAllText(Path.Combine(persistentDataPath, filename), text);
#endif
		}

		public static void DeleteUserData(string filename)
		{
#if UNITY_XBOXONE && !UNITY_EDITOR
			XboxOneConnectedStorage.DeleteData(filename);
#elif UNITY_PS4 && !UNITY_EDITOR
			throw new Exception("Cannot delete file on PS4");
#else
			string filepath = Path.Combine(persistentDataPath, filename);
			if (File.Exists(filepath)) {
				File.Delete(filepath);
			}
#endif
		}

		public static void DeleteMountPoint(MountPointType type, int index)
		{
#if UNITY_PS4
			switch (type) {
				case MountPointType.Pilot:
					PS4Manager.DeleteMountPoint("Pilot");
					break;

				case MountPointType.SavedGame:
					PS4Manager.DeleteMountPoint("SavedGame" + index);
					break;

				case MountPointType.Temp:
					PS4Manager.DeleteMountPoint("Temp");
					break;

				default:
					throw new Exception("Unknown Mount Point type");
			}
#else
			throw new Exception("Cannot delete mount point on this system");
#endif
		}

		public static string[] GetUserDataList(string extension)
		{
#if UNITY_XBOXONE && !UNITY_EDITOR
			return XboxOneConnectedStorage.GetListOfBlobNamesThatMatchExtension(extension).ToArray();
#elif UNITY_PS4 && !UNITY_EDITOR
			return PS4Manager.GetFileList(extension);
#else
			return Directory.GetFiles(persistentDataPath, "*" + extension);
#endif
		}

		public static bool m_system_language_changed = false;

		//Called at startup to initialize platform-specific stuff
		public static void Init()
		{
#if UNITY_XBOXONE
#if !UNITY_EDITOR
			XboxOneManager.Init();
#endif
#elif UNITY_PS4
			PS4Manager.Init();

			//Check if PS4's language changed; if so we override the pilot's setting
			SystemLanguage lang = Application.systemLanguage;
			m_system_language_changed = ((int)lang != PlayerPrefs.GetInt("SystemLanguage"));
			Debug.Log("System language changed: " + m_system_language_changed);
			PlayerPrefs.SetInt("SystemLanguage", (int)lang);

#elif !PLAYABLE_TEASER

			if (Steam.Initialize()) {
				CloudProvider = CloudProviders.Steam;

				//Check if Steam's language changed; if so we override the pilot's setting
				string lang = Steamworks.SteamApps.GetCurrentGameLanguage();
				m_system_language_changed = (lang != PlayerPrefs.GetString("SteamLanguage"));
				Debug.Log("System language changed: " + m_system_language_changed);
				PlayerPrefs.SetString("SteamLanguage", lang);

				//Set up callback for Steam overlay opened
				Steamworks.Callback<Steamworks.GameOverlayActivated_t>.Create(pCallback => {
					if (pCallback.m_bActive != 0) {
						if (GameManager.m_game_state == GameManager.GameState.GAMEPLAY) {
							GameManager.m_gm.OpenPauseMenu();
						}
					}
				});
			} else {    //Try Galaxy


#if !UNITY_STANDALONE_LINUX
				//Create Galaxy manager
				GogGalaxyManager galaxy_manager = GameManager.m_gm.gameObject.AddComponent<GogGalaxyManager>();

				if (galaxy_manager != null) {
					Debug.Log("SignedIn: " + Galaxy.Api.GalaxyInstance.User().SignedIn());
					Debug.Log("LoggedOn: " + Galaxy.Api.GalaxyInstance.User().IsLoggedOn());
				}

				//A callback on success will set CloudProvider to Galaxy
#endif
			}
#endif
		}

		//Called once per frame to do platform-specific stuff
		public static void Update()
		{
#if UNITY_XBOXONE && !UNITY_EDITOR
			XboxOneManager.Update();
#elif UNITY_PS4 && !UNITY_EDITOR
			PS4Manager.Update();
#endif

			UpdatePresence();
		}

		public static bool IsSaveDataLoaded {
			get {
#if UNITY_XBOXONE && !UNITY_EDITOR
				return XboxOneConnectedStorage.IsLoaded();
#else
				return true;   //Data always ready on PC & PS4
#endif
			}
		}

		//True if this is a PS4Pro or XB1X
		public static bool EnhancedMode {
			get {
#if UNITY_XBOXONE && !UNITY_EDITOR
				if (UnityEngine.XboxOne.Hardware.version == UnityEngine.XboxOne.HardwareVersion.XboxOneX_Devkit || UnityEngine.XboxOne.Hardware.version == UnityEngine.XboxOne.HardwareVersion.XboxOneX_Retail) {
					return true;
				} else {
					return false;
				}
#elif UNITY_PS4 && !UNITY_EDITOR
				return UnityEngine.PS4.Utility.neoMode;
#else
				return true;      //Not defined on PC
#endif
			}
		}

		//Version from Steam
		public static string UserName {
			get {
#if UNITY_XBOXONE
				return XboxOneManager.GetActiveUserName();
#elif UNITY_PS4
				return UnityEngine.PS4.PS4Input.RefreshUsersDetails(0).userName;
#else
				if (CloudProvider == CloudProviders.Steam) {
					return Steam.UserName;
#if !UNITY_STANDALONE_LINUX
				} else if (CloudProvider == CloudProviders.Galaxy) {
					return GogGalaxyManager.m_user_name;
#endif
				} else {
					return "";
				}
#endif
			}
		}

#if UNITY_STANDALONE || UNITY_EDITOR
		private enum CloudProviders { None, Steam, Galaxy }
		private static CloudProviders CloudProvider = CloudProviders.None;
#endif

		//Returns true if cloud leaderboards are available
		public static bool LeaderboardsAvailable {
			get {
#if UNITY_XBOXONE
				return (XboxOneManager.GetActiveUser() != null && XboxOneManager.HasNetworkConnection());
#elif UNITY_PS4
				return (PS4Manager.NetworkStatus == NetworkStatusValues.Connected);
#else
				return (CloudProvider != CloudProviders.None);
#endif
			}
		}

		//Are stats available?
		public static bool StatsAvailable {
			get {
#if UNITY_XBOXONE
				return (XboxOneManager.GetActiveUser() != null && XboxOneManager.HasNetworkConnection());
#elif UNITY_PS4
				return true;		//On PS4, stats are actually stored locally so they're always available
#else
				if (CloudProvider == CloudProviders.Steam) {
					return Steam.HaveStats;
#if !UNITY_STANDALONE_LINUX
				} else if (CloudProvider == CloudProviders.Galaxy) {
					return GogGalaxyManager.HaveStats;
#endif
				} else {
					return false;
				}
#endif
			}
		}

		//Text message with the current system error status.  If everything is good, returns null.
		public static string OnlineErrorMessage {
			get {
#if UNITY_XBOXONE
				if (XboxOneManager.HasNetworkConnection()) {
					return null;
				} else {
					return Loc.LS("NO CONNECTION TO XBOX LIVE");
				}
#elif UNITY_PS4
				switch (PS4Manager.NetworkStatus) {
					case NetworkStatusValues.Connected: return null;

					case NetworkStatusValues.NotSignedUp:			return Loc.LS("NO SONY ENTERTAINMENT NETWORK ACCOUNT");
					case NetworkStatusValues.SignedOut:				return Loc.LS("SIGNED OUT OF SONY ENTERTAINMENT NETWORK ACCOUNT");
					case NetworkStatusValues.SystemUpdateNeeded:	return Loc.LS("MUST UPDATE SYSTEM SOFTWARE");
					case NetworkStatusValues.GameUpdateNeeded:		return Loc.LS("MUST UPDATE OVERLOAD");
					case NetworkStatusValues.AgeRestriction:		return Loc.LS("AGE RESTRICTION");
					case NetworkStatusValues.UserNotFound:			return Loc.LS("USER NOT FOUND");        //Don't know when this can happen
					case NetworkStatusValues.NoNetwork:					return Loc.LS("NETWORK CONNECTION PROBLEM");
					case NetworkStatusValues.Unknown:				return Loc.LS("UNKNOWN REASON");    //Haven't received reply to PSN check request

					default:
						Debug.LogError("Unexpected NetworkStatus: " + PS4Manager.NetworkStatus);
						return Loc.LS("UNKNOWN REASON");
				}
#else
				if (CloudProvider == CloudProviders.Steam) {
					if (!SteamManager.Initialized) {
						return Loc.LS("Steam is not available; Client is probably not running." /* On-screen error message */);
					} else if (!Steam.IsConnected) {
						return Loc.LS("Not connected to Steam servers; no network connection?" /* On-screen error message */);
					} else {
						return null;   //No error
					}
#if !UNITY_STANDALONE_LINUX
				} else if (CloudProvider == CloudProviders.Galaxy) {
					if (!GogGalaxyManager.IsInitialized()) {
						return Loc.LS("Galaxy is not available; Client is probably not running." /* On-screen error message */);
					} else if (!GogGalaxyManager.IsConnected) {
						return Loc.LS("Not connected to Galaxy servers; no network connection?" /* On-screen error message */);
					} else if (!GogGalaxyManager.m_have_leaderboards) {
						return Loc.LS("Cannot access Galaxy leaderboards; no network connection?" /* On-screen error message */);
					} else {
						return null;   //No error
					}
#endif
				} else {
					return Loc.LS("NO CLOUD DATA PROVIDER SPECIFIED");
				}
#endif
			}
		}

		//
		// Interfaces to CloudData functions
		//

		public static CloudDataYield UpdateChallengeLeaderboardScore(string level_name, bool submode, int difficulty_level, int score, int kills, int weapon, float time)
		{
#if UNITY_XBOXONE
			return XboxOneLeaderboards.UpdateLeaderboardScore(level_name, submode, difficulty_level, score, kills, weapon, time);
#elif UNITY_PS4
			string leaderboard_name = Platform.GetChallengeLeaderboardName(level_name, submode, difficulty_level);
			return PS4Leaderboards.UpdateChallengeScore(leaderboard_name, score, kills, weapon, time);
#else
			string leaderboard_name = Platform.GetChallengeLeaderboardName(level_name, submode, difficulty_level);
			if (CloudProvider == CloudProviders.Steam) {
				return Steam.UpdateLeaderboardScore(leaderboard_name, score, kills, weapon, time);
#if !UNITY_STANDALONE_LINUX
			} else if (CloudProvider == CloudProviders.Galaxy) {
				return GogGalaxyManager.UpdateLeaderboardScore(leaderboard_name, score, kills, weapon, time);
#endif
			} else { 
				return null;
			}
#endif
		}

		public static string GetChallengeLeaderboardName(string level_name, bool submode, int difficulty_level)
		{
#if UNITY_XBOXONE
			// Fix up level name to match Xbox One back end
			string first = level_name.ToUpper().Replace("TRINOMULAR","TRIGONOMALUS").Replace(GameplayManager.CMCombinedLeaderboardNames[0],"COMBINED");
			first = first[0] + first.Substring(1).ToLower();

			// Difficulty (Insane+ converted to InsanePlus)
			string second = MenuManager.GetDifficultyLevelName(difficulty_level, true).ToLower();
			second = (char.ToUpper(second[0]) + second.Substring(1)).Replace("+","Plus");

			// Mode
			string third = submode ? "Countdown" : "Infinite";

			return first + second + third + "Leaderboard";
#else
			string id = "challenge";
			string diff_name = MenuManager.GetDifficultyLevelName(difficulty_level, true).ToLower();
#if UNITY_PS4
			char separator = ':';
#else
			char separator = (CloudProvider == CloudProviders.Galaxy) ? '_' : ':';
			if (CloudProvider == CloudProviders.Galaxy) {
				diff_name = diff_name.Replace('+', 'P');
			} else if (CloudProvider == CloudProviders.Steam) {
				id = "challenge3";
			}
#endif
			return id + separator + ChallengeManager.GetChallengeSubModeName(submode, true) + separator + difficulty_level.ToString() + diff_name + separator + level_name;
#endif
		}

		//Returns true if data requested, or false if data is not available
		//range_start of -1 means get entries around the player
		public static CloudDataYield RequestChallengeLeaderboardData(string level_name, bool submode, int difficulty_level, int range_start, int num_entries, bool friends)
		{
			string leaderboard_name = GetChallengeLeaderboardName(level_name, submode, difficulty_level);
#if UNITY_XBOXONE
#if !UNITY_EDITOR
			return XboxOneLeaderboards.RequestLeaderboardData(leaderboard_name, range_start, num_entries, friends);
#else
			return null;
#endif
#elif UNITY_PS4
			return PS4Leaderboards.RequestChallengeData(leaderboard_name, range_start, num_entries, friends);
#else
			if (CloudProvider == CloudProviders.Steam) {
				return Steam.RequestLeaderboardData(leaderboard_name, range_start, num_entries, friends);
#if !UNITY_STANDALONE_LINUX
			} else if (CloudProvider == CloudProviders.Galaxy) {
				return GogGalaxyManager.RequestLeaderboardData(leaderboard_name, range_start, num_entries, friends);
#endif
			} else {
				return null;
			}
#endif
		}

		public static CloudDataYield RequestChallengeLeaderboardUserScore(string level_name, bool submode, int difficulty_level)
		{
			return RequestChallengeLeaderboardData(level_name, submode, difficulty_level, -1, 1, false);
		}

		public enum LeaderboardDataState { HaveData, NoProvider, NoConnection, NoLeaderboard, Waiting }

		//Downloads the leaderboard data if it has arrived, else null.
		//Returns the array of requested entries, and the total leaderboard length (not the requested data length)
		//Also returns the index in the list of the item for the current user, or -1 if the user not in the list
		public static LeaderboardEntry[] GetLeaderboardData(out int leaderboard_length, out int user_index, out LeaderboardDataState result)
		{
#if UNITY_XBOXONE
			return XboxOneLeaderboards.GetLeaderboardData(out leaderboard_length, out user_index, out result);
#elif UNITY_PS4
			return PS4Leaderboards.GetData(out leaderboard_length, out user_index, out result);
#else
			if (CloudProvider == CloudProviders.Steam) {
				return Steam.GetLeaderboardData(out leaderboard_length, out user_index, out result);
#if !UNITY_STANDALONE_LINUX
			} else if (CloudProvider == CloudProviders.Galaxy) {
				return GogGalaxyManager.GetLeaderboardData(out leaderboard_length, out user_index, out result);
#endif
			} else {
				leaderboard_length = 0;
				user_index = -1;
				result = LeaderboardDataState.NoProvider;
				return null;
			}
#endif
		}

		public static LeaderboardEntry GetLeaderboardSingleEntry()
		{
			int len;
			int user_index;
			LeaderboardDataState result;

			LeaderboardEntry[] data = GetLeaderboardData(out len, out user_index, out result);
			return (data != null && data.Length == 1) ? data[0] : new LeaderboardEntry(-1, 0, "");
		}

		public static void SetAchievement(Achievements a)
		{
			Debug.Log("SetAchievement " + a);

			if (!GameplayManager.m_i_am_a_cheater && !GameManager.m_cheating_detected) {
#if UNITY_XBOXONE
#if !UNITY_EDITOR
				XboxOneAchievements.UnlockAchievement(a);
#endif
#elif UNITY_PS4 && !UNITY_EDITOR
				PS4Manager.UnlockTrophy((int)a);
#else
				if (CloudProvider == CloudProviders.Steam) {
					bool result = Steamworks.SteamUserStats.SetAchievement(a.ToString());
					Debug.Log("Result = " + result);
					StoreStats();
#if !UNITY_STANDALONE_LINUX
				} else if (CloudProvider == CloudProviders.Galaxy) {
					try {
						Galaxy.Api.GalaxyInstance.Stats().SetAchievement(a.ToString());
						StoreStats();
					}
					catch (Exception ex) {
						Debug.Log("Error setting Galaxy achievement: " + ex.Message);
					}
#endif
				}
#endif
			}
		}

		public static void SetStat(Stats stat, int value)
		{
			if (!GameplayManager.m_i_am_a_cheater && !GameManager.m_cheating_detected) {
#if UNITY_XBOXONE && !UNITY_EDITOR
				XboxOneStats.SetStat(stat, value);
#elif UNITY_PS4 && !UNITY_EDITOR
				PlayerPrefs.SetInt("stat_" + stat.ToString(), value);
#else
				if (CloudProvider == CloudProviders.Steam) {
					Steamworks.SteamUserStats.SetStat(stat.ToString(), value);
#if !UNITY_STANDALONE_LINUX
				} else if (CloudProvider == CloudProviders.Galaxy) {
					try {
						Galaxy.Api.GalaxyInstance.Stats().SetStatInt(stat.ToString(), value);
					}
					catch (Exception ex) {
						Debug.Log("Error setting Galaxy stat: " + ex.Message);
					}
#endif
				}
#endif
			}
		}

		public static void SetStat(Stats stat, float value)
		{
#if UNITY_XBOXONE && !UNITY_EDITOR
			XboxOneStats.SetStat(stat, value);
#elif UNITY_PS4 && !UNITY_EDITOR
			PlayerPrefs.SetFloat("stat_" + stat.ToString(), value);
#else
			if (CloudProvider == CloudProviders.Steam) {
				Steamworks.SteamUserStats.SetStat(stat.ToString(), value);
#if !UNITY_STANDALONE_LINUX
			} else if (CloudProvider == CloudProviders.Galaxy) {
				try {
					Galaxy.Api.GalaxyInstance.Stats().SetStatFloat(stat.ToString(), value);
				}
				catch (Exception ex) {
					Debug.LogWarning("Error settings Galaxy stat: " + ex.Message);
				}
#endif
			}
#endif
		}

		//Increments a stat used to track an achivement.  For PS4, awards the achievment if the trigger value is reached.
		public static void IncrementAchievementStat(Stats stat, int trigger, Achievements a)
		{
			if (NetworkManager.IsHeadless()) {
				return;
			}

			//Debug.Log("IncrementAchievementStat " + stat + " " + trigger + " " + a);
			int value = GetStatInt(stat) + 1;
			//Debug.Log("new value: " + value);
			SetStat(stat, value);
#if UNITY_XBOXONE
			return;		//XB1 handles triggered achievements
#elif UNITY_STANDALONE || UNITY_EDITOR
			if (CloudProvider != CloudProviders.Galaxy) {
				return;		//Steam handles triggered achievements (do nothing if no cloud provider)
			}
#endif
			//Should get here if PS4 or Galaxy
			if (value == trigger) {
				SetAchievement(a);
			}
		}

		//Increments a stat used for analytics
		public static void IncrementStat(Stats stat)
		{
			if (NetworkManager.IsHeadless()) {
				return;
			}

			int value = GetStatInt(stat) + 1;
			SetStat(stat, value);
		}

		public static float GetStatFloat(Stats stat)
		{
			if (!StatsAvailable) {
				return 0;
			}
#if UNITY_XBOXONE && !UNITY_EDITOR
			return XboxOneStats.GetStatFloat(stat);
#elif UNITY_PS4 && !UNITY_EDITOR
			return PlayerPrefs.GetFloat("stat_" + stat.ToString());
#else
			if (CloudProvider == CloudProviders.Steam) {
				float value;
				Steamworks.SteamUserStats.GetStat(stat.ToString(), out value);
				return value;
#if !UNITY_STANDALONE_LINUX
			} else if (CloudProvider == CloudProviders.Galaxy) {
				try {
					return Galaxy.Api.GalaxyInstance.Stats().GetStatFloat(stat.ToString());
				}
				catch (Exception ex) {
					Debug.Log("Error getting Galaxy stat: " + ex.Message);
					return 0;
				}
#endif
			} else {
				return 0;
			}
#endif
		}

		public static int GetStatInt(Stats stat)
		{
			if (!StatsAvailable) {
				return 0;
			}
			
#if UNITY_XBOXONE && !UNITY_EDITOR
			return XboxOneStats.GetStatInt(stat);;
#elif UNITY_PS4 && !UNITY_EDITOR
			return PlayerPrefs.GetInt("stat_" + stat.ToString());
#else
			if (CloudProvider == CloudProviders.Steam) {
				int value = 0;
				Steamworks.SteamUserStats.GetStat(stat.ToString(), out value);
				return value;
#if !UNITY_STANDALONE_LINUX
			} else if (CloudProvider == CloudProviders.Galaxy) {
				try { 
					return Galaxy.Api.GalaxyInstance.Stats().GetStatInt(stat.ToString());
				}
				catch (Exception ex) {
					Debug.Log("Error getting Galaxy stat: " + ex.Message);
					return 0;
				}
#endif
			} else {
				return 0;
			}
#endif
		}

		public static double GetGlobalStatDouble(Stats stat)
		{
			if (!StatsAvailable) {
				return 0;
			}

#if UNITY_XBOXONE && !UNITY_EDITOR
			return 0;
#elif UNITY_PS4 && !UNITY_EDITOR
			return 0;	//No global stats on PS4
#else
			if (CloudProvider == CloudProviders.Steam) {
				double value = 0;
				Steamworks.SteamUserStats.GetGlobalStat(stat.ToString(), out value);
				return value;
			} else {
				return 0;
			}
#endif
		}

		public static long GetGlobalStatLong(Stats stat)
		{
			if (!StatsAvailable) {
				return 0;
			}

#if UNITY_XBOXONE && !UNITY_EDITOR
			return 0;
#elif UNITY_PS4 && !UNITY_EDITOR
			return 0;	//No global stats on PS4
#else
			if (CloudProvider == CloudProviders.Steam) {
				long value = 0;
				Steamworks.SteamUserStats.GetGlobalStat(stat.ToString(), out value);
				return value;
			} else {
				return 0;
			}
#endif
		}

		public static void StoreStats()
		{
			if (!StatsAvailable) {
				return;
			}

			Debug.Log("StoreStats");
			
#if UNITY_XBOXONE
			// stats are stored automatically when they are modified
#elif UNITY_PS4
			PlayerPrefs.Save();
#else
			if (CloudProvider == CloudProviders.Steam) {
				Steamworks.SteamUserStats.StoreStats();
#if !UNITY_STANDALONE_LINUX
			} else if (CloudProvider == CloudProviders.Galaxy) {
				Galaxy.Api.GalaxyInstance.Stats().StoreStatsAndAchievements();
#endif
			}
#endif
		}

		public static void ResetAllStats(bool achievements_too = false)
		{
#if UNITY_XBOXONE
#elif UNITY_PS4
#else
			if (CloudProvider == CloudProviders.Steam) {
				Steamworks.SteamUserStats.ResetAllStats(achievements_too);
#if !UNITY_STANDALONE_LINUX
			} else if (CloudProvider == CloudProviders.Galaxy) {
				if (!achievements_too) {
					Debug.LogError("On Galaxy, it's not possible to delete the stats but not the achievements.");
				} else {
					Galaxy.Api.GalaxyInstance.Stats().ResetStatsAndAchievements();
				}
#endif
			}
#endif
		}

		public static Loc.Lang SystemLanguage {
			get {
#if UNITY_STANDALONE || UNITY_EDITOR
				if (CloudProvider == CloudProviders.Steam) {
					switch (Steamworks.SteamApps.GetCurrentGameLanguage()) {
						case "german": return Loc.Lang.GERMAN;
						case "french": return Loc.Lang.FRENCH;
						case "spanish": return Loc.Lang.SPANISH;
						case "russian": return Loc.Lang.RUSSIAN;
						default: return Loc.Lang.ENGLISH;
					}
				}
#endif
				switch (Application.systemLanguage) {
					case UnityEngine.SystemLanguage.French: return Loc.Lang.FRENCH;
					case UnityEngine.SystemLanguage.German: return Loc.Lang.GERMAN;
					case UnityEngine.SystemLanguage.Spanish: return Loc.Lang.SPANISH;
					case UnityEngine.SystemLanguage.Russian: return Loc.Lang.RUSSIAN;
					default: return Loc.Lang.ENGLISH;
				}
			}
		}

#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
		public static void EnableGalaxy(bool success)
		{
			if (success) {
				CloudProvider = CloudProviders.Galaxy;
				GameplayManager.CMCombinedLeaderboardNames[0] = "--COMBINED--";
				GameplayManager.CMCombinedLeaderboardNames[1] = "--COMBINED DLC--";
				Debug.Log("SignedIn: " + Galaxy.Api.GalaxyInstance.User().SignedIn());
				Debug.Log("LoggedOn: " + Galaxy.Api.GalaxyInstance.User().IsLoggedOn());
			} else {
				GameObject.Destroy(GameManager.m_gm.gameObject.GetComponent<GogGalaxyManager>());
			}
		}
#endif

		public enum IdType { Steam, Galaxy, PSN, XBLive, Local}

		//Returns unique online user ID for this player
		public static void GetUserID(out IdType id_type, out string id)
		{
#if UNITY_XBOXONE
			// Xbox One does not want XUID stored in a database, so we are going to treat Xbox One like PC without Steam/GOG. Old method is
			// commented out below for comparison.
			/*
			id_type = IdType.XBLive;
			id = XboxOneManager.GetActiveUserXuid();
			*/
			id_type = IdType.Local;
			id = Player.Mp_user_id;
			if (string.IsNullOrEmpty(id)) {
				id = Guid.NewGuid().ToString();
				Player.Mp_user_id = id;
				PilotManager.SavePreferences();	// save prefs, which stores Player.Mp_user_id in the profile
			}
			return;
#elif UNITY_PS4
			id_type = IdType.PSN;
			id = PS4Manager.UserId.ToString();
			return;
#else
			if (CloudProvider == CloudProviders.Steam) {
				id_type = IdType.Steam;
				id = Steamworks.SteamUser.GetSteamID().ToString();
				return;
#if !UNITY_STANDALONE_LINUX
			} else if (CloudProvider == CloudProviders.Galaxy) {
				id_type = IdType.Galaxy;
				id = Galaxy.Api.GalaxyInstance.User().GetGalaxyID().ToString();
				return;
#endif
			} else {
				id_type = IdType.Local;
				id = PlayerPrefs.GetString("UserID");
				if (id == "" ) {
					id = Guid.NewGuid().ToString();
					PlayerPrefs.SetString("UserID", id);
				}
			}
#endif
		}

		private static float m_UpdatePresenceTimer = 0f;
		private static float m_UpdatePresenceDelaySeconds = 30.0f;

		public enum PresenceState { Starting, Menus, Training, Mission, Challenge, WaitingForMatch, Multiplayer, NUM }

		private static PresenceState m_prev_state = PresenceState.NUM;
		private static bool m_prev_paused = false;
		private static LevelInfo m_prev_level = null;

		private static void UpdatePresence()
		{
			m_UpdatePresenceTimer -= Time.unscaledDeltaTime;
			if (m_UpdatePresenceTimer > 0.0f) {
				return;
			}
			m_UpdatePresenceTimer = m_UpdatePresenceDelaySeconds;

			PresenceState state = PresenceState.NUM;
			bool paused = false;
			LevelInfo level = null;

			if (GameManager.m_game_state == GameManager.GameState.NONE) {
				state = PresenceState.Starting;
			} else if ((GameManager.m_game_state == GameManager.GameState.MENU) && !MenuManager.m_game_paused) {
				state = (MenuManager.m_menu_state == MenuState.MP_PRE_MATCH_MENU) ? PresenceState.WaitingForMatch : PresenceState.Menus;
			} else {

#if !UNITY_XBOXONE
				//Playing something
				paused = MenuManager.m_game_paused;
				level = GameplayManager.Level;
#endif

				if (GameplayManager.Level.Mission.Type == MissionType.TRAINING) {
					state = PresenceState.Training;
				} else if (NetworkManager.IsMultiplayerSceneLoading() || NetworkManager.IsMultiplayerSceneLoaded()) {
					state = PresenceState.Multiplayer;
					paused = false;
				} else if (GameplayManager.IsChallengeMode) {
					state = PresenceState.Challenge;
				} else {
					state = PresenceState.Mission;
				}
			}

			Assert.True(state != PresenceState.NUM);

			if ((state == m_prev_state) && (paused == m_prev_paused) && (level == m_prev_level)) {
				return;
			}
			Debug.Log("Presence: " + state + " " + paused + " " + ((level == null) ? "" : level.DisplayName));

			m_prev_state = state;
			m_prev_paused = paused;
			m_prev_level = level;

#if UNITY_XBOXONE
#if !UNITY_EDITOR
			XboxOneRichPresence.SetPresence(state, paused);
#endif
#elif UNITY_PS4
			PS4Manager.SetPresence(state, paused, level);
#else
			if (CloudProvider == CloudProviders.Steam) {
				Steam.SetPresence(state, paused, level);
			}
#if !UNITY_STANDALONE_LINUX
			else if (CloudProvider == CloudProviders.Galaxy) {
				GogGalaxyManager.SetPresence(state, paused, level);
			}
#endif
#endif
		}

#if UNITY_STANDALONE || UNITY_PS4
		private static string[,] m_status_strings = new string[7, (int)Loc.Lang.NUM] {		//The order here must match PresenceState enum and Loc.Lang enum
			{ "Starting Up", "Wird gestartet", "Puesta en marcha", "Lancement", "Запуск" },
			{ "In Menus", "Im Menü", "En los Menús", "Dans les Menus", "В Меню" },
			{ "{0} {1}", "{0} {1}", "{1} {0}", "{0} {1}", "{0} {1}" },
			{ "{0} {1} in {2}", "{0} {1} in {2}", "{1} en {2} {0}", "{0} {1} dans {2}", "{0} {1} в {2}" },
			{ "{0} Challenge Mode in {1}", "{0} Herausforderungsmodus in {1}", "Modo Desafío en {1} {0}", "{0} Mode Défi dans {1}", "{0} Режим Испытания в {1}" },
			{ "Waiting for Match", "Warte auf Spiel", "Esperando la partida", "En attente d’une partie", "Ожидание Матча" },
			{ "{0} Multiplayer in {1}", "{0} Mehrspieler in {1}", "Modo Multijugador en {1} {0}", "{0} Multijoueur dans {1}", "{0} Мультиплеер в {1}" },
		};

		private static string[] m_text_playing = new string[(int)Loc.Lang.NUM] { "Playing", "Spielt", "en juego", "En jeu", "Игра" };
		private static string[] m_text_paused = new string[(int)Loc.Lang.NUM] { "[Paused]", "[Pausiert]", "[en pausa]", "[En pause]", "[Пауза]" };

		public static string GetPresenceString(Platform.PresenceState state, bool paused, LevelInfo level, int lang)
		{
			Assert.True(state != Platform.PresenceState.NUM);

			string paused_or_playing = paused ? m_text_paused[lang] : m_text_playing[lang];
			string level_name = "";
			string mission_name = "";
			if (level != null) {
				level_name = level.m_display_name[lang];
				mission_name = level.Mission.m_display_name[lang];
			}

			return string.Format(m_status_strings[(int)state, lang], paused_or_playing, level_name, mission_name);
		}
#endif

		// This function does a ROT13 on a string and returns it
		// Use this to do a little obscuring of some strings that we
		// just want to make it not so obvious to a hacker.
		public static string Xform(string str)
		{
			char[] array = str.ToCharArray();
			for (int i = 0; i < array.Length; i++) {
				int number = (int)array[i];

				if (number >= 'a' && number <= 'z') {
					if (number > 'm') {
						number -= 13;
					} else {
						number += 13;
					}
				} else if (number >= 'A' && number <= 'Z') {
					if (number > 'M') {
						number -= 13;
					} else {
						number += 13;
					}
				}
				array[i] = (char)number;
			}
			return new string(array);
		}

		private static int m_developer_mode = -1;

		/// <summary>
		/// Force disable developer mode
		/// </summary>
		public static void DisableDeveloperMode()
		{
			m_developer_mode = 0;
		}

		public static void EnableDeveloperMode()
		{
			m_developer_mode = 1;
		}

		/// <summary>
		/// This function will return true if we believe the game is running
		/// on a developer's machine (as opposed to someone outside)
		/// </summary>
		/// <returns></returns>
		public static bool IsRunningInDeveloperMode()
		{
#if UNITY_EDITOR
			// Editor is, by definition, developer mode
			if (m_developer_mode == -1) {
				m_developer_mode = 1;
			}
			return m_developer_mode == 1;
#elif UNITY_STANDALONE
			// Standalone Windows, we may be running in developer mode
			if( m_developer_mode != -1 ){
				// Already determined
				return m_developer_mode == 1;
			}

			string checkFilename = Xform("eriviny.vav"); // Check for the existence of revival.ini somewhere up the parent folder chain
			string currentDirectory = Environment.CurrentDirectory;
			while (currentDirectory != null) {
				if (System.IO.File.Exists(System.IO.Path.Combine(currentDirectory, checkFilename))) {
					m_developer_mode = 1;
					break;
				}

				// Pop-up a folder
				var parentDir = System.IO.Directory.GetParent(currentDirectory);
				if (parentDir == null) {
					currentDirectory = null;
				} else {
					currentDirectory = parentDir.FullName;
				}
			}
			if (m_developer_mode == -1) {
				string val = Environment.GetEnvironmentVariable(Xform("ERIVINY_PBASVTHENGVBA_SVYR")); // REVIVAL_CONFIGURATION_FILE
				if (val == "1") {
					m_developer_mode = 1;
				}
			}
			if (m_developer_mode == -1) {
				// Doesn't look like a developer
				m_developer_mode = 0;
			}

			// What did we find?
			return m_developer_mode == 1;
#elif UNITY_XBOXONE || UNITY_PS4
			// for consoles, developer mode is only started by holding down a button combination when loading bar starts
			// see MaybeEnableDeveloperMode() in GameManager.cs for button check
			return m_developer_mode == 1;			        
#else
			// No other platform is supported in developer mode
			return false;
#endif
		}

		

		public static string PlatformName {
#if UNITY_XBOXONE
			get { return "XBOX LIVE";  }
#elif UNITY_PS4
			get { return "PSN";  }
#else
			get {
				if (CloudProvider == CloudProviders.Steam) {
					return "STEAM";
#if !UNITY_STANDALONE_LINUX
				} else if (CloudProvider == CloudProviders.Galaxy) {
					return "GALAXY";
#endif
				} else {
					return "NONE";
				}
			}
#endif
		}
	}
}