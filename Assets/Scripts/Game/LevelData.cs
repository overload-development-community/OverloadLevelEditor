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
using System.Collections.Generic;
using Overload;
using System.Linq;

#if OVERLOAD_LEVEL_EDITOR
// Some stub classes so this file will compile
public class Reactor : MonoBehaviour
{
}
#endif

public class LevelData : MonoBehaviour
{
	[System.Serializable]
	public class SpawnPoint
	{
		public Vector3 position;
		public Quaternion orientation;
		public int multiplayer_team_association_mask = 0; // Each bit corresponds to a team. Default value of 0 means all teams (same as setting all the bits). Bit0 = team 0, Bit1 = team 1, etc.
		public int m_current_segment = -1;

		public SpawnPoint()
		{
			position = Vector3.zero;
			orientation = Quaternion.identity;
		}

		public SpawnPoint(Vector3 pos, Quaternion orient, int mp_team_association_mask)
		{
			position = pos;
			orientation = orient;
			multiplayer_team_association_mask = mp_team_association_mask;
		}
	}

	[System.Serializable]
	public class PortalDoorConnection
	{
		public int PortalIndex;
#if OVERLOAD_LEVEL_EDITOR
		public OverloadLevelExport.IComponentBroker ReferenceDoor;
#else
		public DoorBase ReferenceDoor;
#endif
	}

	//Set by level converter
	public GameObject m_game_manager_prefab;
	public LevelGeometry m_geometry;

	public SpawnPoint[] m_robot_spawn_points;
	public SpawnPoint[] m_item_spawn_points;
	public SpawnPoint[] m_player_spawn_points;
	public SpawnPoint[] m_mp_camera_points;
	public PortalDoorConnection[] m_portal_to_door_references;
	public string m_challenge_mode_text; // NOTE: This variable is not to be used. It is here because it used to be a part of user levels but ended up not working/usable so removed. However, since there now exists user levels, I can't remove this or it will break those levels.

	/// <summary>
	/// All of the level lights in the scene
	/// </summary>
	public SegmentLightInfo[] m_level_lights;

	/// <summary>
	/// All of the level reflection probes in the scene
	/// </summary>
	public SegmentReflectionProbeInfo[] m_level_reflection_probes;

	//Accessor for segment-to-segment visibility array
	public IndexAs2D m_segment_visibility = null;

	//Array of chunk GameObjects.  Each go contains the collision component and either the render mesh component or children with mesh render components.
	public GameObject[] ChunkObjects { get; private set; }

	//The reactor in the current level
	public Reactor Reactor { get; private set; }

	//Cryotube stuff
	public int NumCryotubes { get; private set; }

	public int m_alien_warp_segment;       // In level 12, segment containing alien warper that enables player to leave level

	public static int m_near_surface_segment;    // In level 1 (Ymir) a segment near enough to the surface so Guide-Bot can lead player to rescue.

	public int m_num_monitors;

	//Class to access one-dimensional array as 2D array
	public class IndexAs2D
	{
		private int[] m_source;
		private int m_width;			//Not necessarily really width.  Actually the length of dimension 0.

		public IndexAs2D(int[] source, int width)
		{
			m_source = source;
			m_width = width;
		}

		public int this[int i, int j] {
			get { return m_source[(i * m_width) + j]; }
		}
	}

	//Public properties to access the geometry as if it were here
	public SegmentData[] Segments { get { return m_geometry.Segments; } }
	public int NumSegments;
	public PortalData[] Portals { get { return m_geometry.Portals; } }
	public Vector3[] SegmentVerts { get { return m_geometry.SegmentVerts; } }
	public int[] SegmentRootIndicesIntoPerSegmentBSPData { get { return m_geometry.SegmentRootIndicesIntoPerSegmentBSPData; } }
	public BSPTreeNode[] PerSegmentBSPData { get { return m_geometry.PerSegmentBSPData; } }
	public AABBTreeNode[] SegmentAABBTree { get { return m_geometry.SegmentAABBTree; } }
	public ChunkData[] Chunks { get { return m_geometry.Chunks; } }
	public ChunkPortal[] ChunkPortals { get { return m_geometry.ChunkPortals; } }
	public Vector3[] PortalGeomVerts { get { return m_geometry.PortalGeomVerts; } }
	public PortalGeomTriangle[] PortalGeomTriangles { get { return m_geometry.PortalGeomTriangles; } }
	public PortalGeomData[] PortalGeomDatas { get { return m_geometry.PortalGeomDatas; } }
	public string ChallengeModeDataText { get { return m_geometry.ChallengeModeDataText; } }
	public int[] SegmentToSegmentVisibility { get { return m_geometry.SegmentToSegmentVisibility; } }
	public string GeometryHash { get { return m_geometry.GeometryHash; } }

	public static bool CrushHierarchy = true;

	//This is called after a level is loaded.  Create the GameManager if this is the first level loaded.
	//This function should do nothing else.
	public void Awake()
	{
#if !OVERLOAD_LEVEL_EDITOR
		// Defer the Awake until after user level loading is complete
		if (UserLevelLoader.IsLoadingUserLevel) {
			UserLevelLoader.ExecuteAfterLoading(() => { Awake(); });
			return;
		}

		Debug.Log("LevelData Awake " + Time.realtimeSinceStartup);

		//If Game Manager doesn't exist (this is the first level to be loaded), create it
		if (Object.Equals(GameManager.m_gm, null)) {		//When the game is shutting down in the editor it apparently re-loads the editor level after closing down the GameManager causing this to be called when m_gm is fake-null.   So check for real null.
			GameManager.Required(m_game_manager_prefab != null, "Game manager prefab not specified");
			Instantiate(m_game_manager_prefab, Vector3.zero, Quaternion.identity);
		}

		//Check for geometry data
		GameManager.Required(m_geometry != null, "Geometry data not specified");

		//Save the global for the level initializer object
		GameManager.m_level_data = this;
		
		// Add local player immediately. Remote clients will only add players once scene is loaded both on server and locally.
		if (NetworkManager.IsServer()) {
			Client.AddPlayer();
		} else {
			GameManager.m_player_ship = PlayerShip.Instantiate();
			GameManager.m_local_player = GameManager.m_player_ship.c_player;
		}

		GameObject legacy_player_start = GameObject.FindGameObjectWithTag("LegacyPlayerStart");
		if (!GameplayManager.IsMultiplayer) {
			GameManager.m_player_ship.SetSpawnPosAndRotation(legacy_player_start);
		}
		if (legacy_player_start != null) {
			legacy_player_start.SetActive(false);  // start has a camera we want to disable
		}

		//Get the viewer component
		GameManager.m_viewer = Camera.main.GetComponent<Viewer>();
		GameManager.Required(GameManager.m_viewer != null, "Must have Viewer component on camera");

		//Get the mesh containers
		GameManager.m_mesh_container = GameObject.Find("_container_level_meshes");

		//
		// Do some processing on this object's data.  Don't do anything else here.
		//

		//Fix up the door references
		if (m_portal_to_door_references != null) {
			foreach (var portal_door_ref in m_portal_to_door_references) {
				int portal_index = portal_door_ref.PortalIndex;
				if (portal_index >= 0 && portal_index < m_geometry.Portals.Length) {
					m_geometry.Portals[portal_index].DoorData = portal_door_ref.ReferenceDoor;
				} else {
					Debug.LogError(string.Format("Portal Index {0} is out of range for door {1}", portal_index, portal_door_ref.ReferenceDoor.name));
				}
			}
		}

		//Create empty level light and reflection probe arrays if they're not present
		if (m_level_lights == null) {
			m_level_lights = new SegmentLightInfo[0];
		}

		if (m_level_reflection_probes == null || m_level_reflection_probes.Length == 0) {
			List<SegmentReflectionProbeInfo> srpi_list = new List<SegmentReflectionProbeInfo>();
			// Gather the reflection probes the hard way (slowly)
			GameObject userContainerObject = GameObject.Find("!rprobes");
			if (userContainerObject != null && userContainerObject.transform.parent == null) {
				// Gather all the reflection probes under "!probes"
				ReflectionProbe[] rps = userContainerObject.GetComponentsInChildren<ReflectionProbe>();
				int count = rps.Length;
            for (int i = 0; i < count; i++) {
					SegmentReflectionProbeInfo srpi = new SegmentReflectionProbeInfo();
					srpi.ProbeObject = rps[i].gameObject;
					srpi.SegmentIndex = FindSegmentContainingWorldPosition(rps[i].transform.position);
					if (srpi.SegmentIndex < 0) {
						srpi.SegmentIndex = ChunkManager.FindNearestSegment(rps[i].transform.position);
               }
					srpi.ProbeType = SegmentReflectionProbeType.UserPlaced;

					srpi_list.Add(srpi);
				}
			}

			m_level_reflection_probes = srpi_list.ToArray();

			if (m_level_reflection_probes == null) {
				m_level_reflection_probes = new SegmentReflectionProbeInfo[0];
			}
		}

		
		NumSegments = m_geometry.Segments.Length;

		// Associate lights to segments
		{
			Dictionary<int, List<SegmentLightInfo>> mapLightsToSegment = new Dictionary<int, List<SegmentLightInfo>>();

			foreach (var llight in m_level_lights) {
				if (llight.SegmentIndex < 0 || llight.SegmentIndex >= NumSegments) {
					Debug.LogErrorFormat("Level light at invalid segment index {0}", llight.SegmentIndex);
					continue;
				}

				List<SegmentLightInfo> list;
				if (!mapLightsToSegment.TryGetValue(llight.SegmentIndex, out list)) {
					list = new List<SegmentLightInfo>();
					mapLightsToSegment.Add(llight.SegmentIndex, list);
				}

				if (llight.LightObject != null && llight.LightComp != null) {
					list.Add(llight);
				}
			}

			foreach (var kvp in mapLightsToSegment) {
				m_geometry.Segments[kvp.Key].Lights = kvp.Value.ToArray();
			}

			// Provide an empty list for the other segments
			SegmentLightInfo[] dummyList = new SegmentLightInfo[0];
			for (int segIndex = 0; segIndex < NumSegments; ++segIndex) {
				if (mapLightsToSegment.ContainsKey(segIndex))
					continue;
				m_geometry.Segments[segIndex].Lights = dummyList;
			}
		}

		// Associate reflection probes to segments
		{
			Dictionary<int, List<SegmentReflectionProbeInfo>> mapProbesToSegment = new Dictionary<int, List<SegmentReflectionProbeInfo>>();

			foreach (var lprobe in m_level_reflection_probes) {
				if (lprobe.SegmentIndex < 0 || lprobe.SegmentIndex >= NumSegments) {
					Debug.LogErrorFormat("Level reflection probe at invalid segment index {0}", lprobe.SegmentIndex);
					continue;
				}

				List<SegmentReflectionProbeInfo> list;
				if (!mapProbesToSegment.TryGetValue(lprobe.SegmentIndex, out list)) {
					list = new List<SegmentReflectionProbeInfo>();
					mapProbesToSegment.Add(lprobe.SegmentIndex, list);
				}

				if (lprobe.ProbeObject != null) {
					list.Add(lprobe);
				}
			}

			foreach (var kvp in mapProbesToSegment) {
				m_geometry.Segments[kvp.Key].ReflectionProbes = kvp.Value.ToArray();
			}

			// Provide an empty list for the other segments
			SegmentReflectionProbeInfo[] dummyList = new SegmentReflectionProbeInfo[0];
			for (int segIndex = 0; segIndex < NumSegments; ++segIndex) {
				if (mapProbesToSegment.ContainsKey(segIndex))
					continue;
				m_geometry.Segments[segIndex].ReflectionProbes = dummyList;
			}
		}

		//Find connecting portals for chunk portals
		foreach (ChunkPortal chunk_portal in m_geometry.ChunkPortals) {
			if (chunk_portal.ConnectedPortal == -1) {
				PortalData seg_portal = m_geometry.Portals[m_geometry.Segments[chunk_portal.Segnum].Portals[chunk_portal.Sidenum]];
				int connected_segnum = (seg_portal.MasterSegmentIndex == chunk_portal.Segnum) ? seg_portal.SlaveSegmentIndex : seg_portal.MasterSegmentIndex;
				foreach (int connected_chunk_portal_index in m_geometry.Chunks[chunk_portal.ConnectedChunk].PortalIndices) {
					ChunkPortal connected_chunk_portal = m_geometry.ChunkPortals[connected_chunk_portal_index];
					if (connected_chunk_portal.Segnum == connected_segnum) {
						chunk_portal.ConnectedPortal = connected_chunk_portal.Num;
						connected_chunk_portal.ConnectedPortal = chunk_portal.Num;
					}
				}
				Assert.True(chunk_portal.ConnectedPortal != -1);
			}
		}

		//Make list of Chunks
		CreateChunkList();

		//Set up object to access segment visibility
		if (m_geometry.SegmentToSegmentVisibility == null) {
			var stopWatch = new System.Diagnostics.Stopwatch();
			stopWatch.Start();
			Segment2SegmentVis.ComputeSegmentVisibility(m_geometry, 55.0f);
			stopWatch.Stop();
			UnityEngine.Debug.LogFormat("ComputeSegmentVisibility took {0} ms", stopWatch.ElapsedMilliseconds);
		}
		if (m_geometry.SegmentToSegmentVisibility != null) {
			m_segment_visibility = new IndexAs2D(m_geometry.SegmentToSegmentVisibility, NumSegments);
		} else {
			m_segment_visibility = null;
		}

		//Make a list of ambient sound sources in the level.  Might make sense to keep that list here, but the gather function is a little audio-specific
		if (GameManager.m_audio != null) {		//Don't do this on game startup
			GameManager.m_audio.GatherAmbientSounds();
		}

		//Find the reactor, if one exists
		Reactor[] reactors = FindObjectsOfType<Reactor>();
		if (reactors.Length == 0) {
			//Debug.Log("There is no reactor in this level!");
			Reactor = null;
		} else {
			if (reactors.Length > 1) {
				Debug.Log("There are " + reactors.Length + " reactors in this level!  There should be no more than one!  Will only use the first one found");
			}
			Reactor = reactors[0];
		}

		// Find the alien warper if one exists.
		GameObject alien_warp = GameObject.FindGameObjectWithTag("AlienWarp");
		if (alien_warp != null) {
			m_alien_warp_segment = GameManager.m_level_data.FindSegmentContainingWorldPosition(alien_warp.transform.position);
			Debug.Log("Alien warper is in segment " + m_alien_warp_segment);
		} else {
			m_alien_warp_segment = -1;		// It's OK if there is no alien warper, most levels don't have one!
		}

		//Reset LevelCustomInfo statics in case the level doesn't have a LevelCustomInfo component
		LevelCustomInfo.Reset();

		//Find segments for spawn points
		SetSpawnPointSegments(m_robot_spawn_points);
		SetSpawnPointSegments(m_item_spawn_points);
		SetSpawnPointSegments(m_player_spawn_points);
		SetSpawnPointSegments(m_mp_camera_points);

		TriggerWindTunnel.UpdateSegmentsForWindTunnels();     // Deal with Wind Tunnels (WindTunnels)
		Forcefield.UpdateSegmentsForForcefields();		// Deal with Forcefields

		// Deal with teleporters.
		for (int i = 0; i < GameManager.m_level_data.Segments.Length; i++) {
			GameManager.m_level_data.Segments[i].WarpDestinationSegs = new int[6];
			for (int j=0; j<6; j++) {
				GameManager.m_level_data.Segments[i].WarpDestinationSegs[j] = -1;
			}
		}

		//Find & initialize cryotubes
		InitCryotubes();

		// Explode the hierarchy (for optimization reasons)
		if (CrushHierarchy) {
			GameManager.m_mesh_container.transform.DetachChildren();

			GameObject container = GameObject.Find("_container_level_lava_meshes");
			container.transform.DetachChildren();

			//container = GameObject.Find("_container_placed_entities");
			//int count = container.transform.childCount;
			//for (int i = count - 1; i >= 0; i--) {
			//	container.transform.GetChild(i).DetachChildren();
			//}
			LevelCustomInfo.TryToCrush();
		}

		Debug.Log("LevelData Awake DONE " + Time.realtimeSinceStartup);

#endif // !OVERLOAD_LEVEL_EDITOR
	}

#if !OVERLOAD_LEVEL_EDITOR
	private void InitCryotubes()
	{
		//Check for not playing a real level
		if (GameplayManager.Level == null) {
			return;
		}

		GameObject[] cryotubes = GameObject.FindGameObjectsWithTag("Cryotube");

		NumCryotubes = cryotubes.Length;
		bool[] found = new bool[NumCryotubes];

		if (GameplayManager.Level.CryotubeIDs != null && GameplayManager.Level.CryotubeIDs.Length != NumCryotubes) {
			Debug.LogFormat("Number of cryotube names ({0}) does not match number of cryotubes ({1}).", GameplayManager.Level.CryotubeIDs.Length, NumCryotubes);
		}

#if !EARLY_ACCESS
		//Override texture for cryotube pictures
		foreach (GameObject tube in cryotubes) {

			PropCryotube script = tube.GetComponent<PropCryotube>();
			int id = script.m_index;

			if (id >= NumCryotubes) {
				Debug.LogWarning("Invalid cryotube number " + id + " (num cryotubes = " + NumCryotubes + ")");
			}
			if (found[id]) {
				Debug.LogWarning("Found more than one cryotube with the same ID");
			}

			found[id] = true;

			//Set image in models
			Texture texture = (Texture)Resources.Load("Missions/" + GameplayManager.Level.FileName + "_rescue" + (id + 1).ToString("D2"));
			if (texture != null) {
				GameObject screen = tube.GetChildByNameRecursive("cryotubeA_screen_01a");
				GameManager.Required(screen != null, "Cannot find 'cryotubeA_screen_01a' in cryotube");
				MeshRenderer renderer = screen.GetComponent<MeshRenderer>();
				Material[] materials = renderer.materials;
				Assert.True(materials.Length == 1);
				materials[0].SetTexture("_MainTex", texture);
			}
		}
#endif

		for (int i = 0; i < NumCryotubes; i++) {
			if (!found[i]) {
				Debug.LogWarning("Did not find cryotube " + i);
			}
		}

		//Get the number of monitors
		m_num_monitors = Resources.FindObjectsOfTypeAll<PropMonitor>().Where(m => (m.gameObject.scene.name != null)).Count();
		Debug.Log("MONITORS: " + m_num_monitors + " in this level");
	}
#endif // !OVERLOAD_LEVEL_EDITOR

    //Set the segment numbers for spawn points
    private void SetSpawnPointSegments(SpawnPoint[] spawn_points)
	{
		if (spawn_points == null) {
			return;
		}

		foreach (SpawnPoint sp in spawn_points) {
			sp.m_current_segment = FindSegmentContainingWorldPosition(sp.position);
			Assert.True(sp.m_current_segment != -1);
		}
	}

#if !OVERLOAD_LEVEL_EDITOR
	//Make a list of all the rendering object chunks by chunk number
	private void CreateChunkList()
	{
		//Get number of chunks
		int num_chunks = m_geometry.Chunks.Length;

		//Get the level object
		GameObject[] level_objects = GameObject.FindGameObjectsWithTag("Level");
		GameManager.Required(level_objects.Length == 1, "Found " + level_objects.Length + " objects with Level tag.");

		//Find the container containing the meshes
		GameObject mesh_container = level_objects[0].GetChildByName("_container_level_meshes");
		GameManager.Required(mesh_container != null, "Level mesh container not found in level");

		//Create array
		ChunkObjects = new GameObject[num_chunks];

		//Make sure level object has same number of chunks
		Assert.True(mesh_container.transform.childCount == num_chunks);

		//Get each chunk in container
		for (int i = 0; i < num_chunks; i++) {
			GameObject go = mesh_container.transform.GetChild(i).gameObject;

			// Which chunk are we processing? Ask because we might not get the chunks in the correct order
			int go_chunk_index;
			bool parse_result = int.TryParse(go.name.Substring(go.name.Length - 4), out go_chunk_index);
			Assert.True(parse_result);
			Assert.True(go_chunk_index >= 0 && go_chunk_index < num_chunks);
			Assert.True(ChunkObjects[go_chunk_index] == null);

			ChunkObjects[go_chunk_index] = go;
		}

		// Verify all chunks found
		for (int i = 0; i < num_chunks; ++i) {
			Assert.True(ChunkObjects[i] != null);
		}
	}

	// For the given position pos:
	//		segment = segment containing pos
	//		side = side with center nearest pos
	public static void FindNearestSegmentAndSide(Vector3 pos, out int segment, out int side)
	{
		segment = GameManager.m_level_data.FindSegmentContainingWorldPosition(pos);
		side = -1;

		if (segment >= 0) {
			side = GetNearestSideCenter(segment, pos);
		} else {
			Debug.LogError("Box warper not contained in any segment!  Position = " + pos);
		}
	}

	// Return index of side whose center is nearest to pos.
	public static int GetNearestSideCenter(int segment, Vector3 pos)
	{
		float nearest_dist = 9999f;
		int nearest_index = -1;

		for (int i = 0; i < 6; i++) {
			Vector3 side_center = GetSideCenter(segment, i);
			float dist = (side_center - pos).magnitude;
			if (dist < nearest_dist) {
				nearest_dist = dist;
				nearest_index = i;
			}
		}

		return nearest_index;
	}

	// Return center point of a side by taking the average of the four points.
	public static Vector3 GetSideCenter(int curseg, int side)
	{
		var sideVertexOrder = SegmentData.SideVertexOrder[side];
		Vector3 v = new Vector3(0, 0, 0);

		// Get average of four points on portal side.
		for (int k = 0; k < 4; k++) {
			Vector3 v2 = GameManager.m_level_data.SegmentVerts[GameManager.m_level_data.Segments[curseg].VertIndices[sideVertexOrder[k]]];
			v += v2;
		}

		v /= 4;

		return v;
	}
#endif // !OVERLOAD_LEVEL_EDITOR

	//
	//  Functions that operate on the level geometry data
	//

	public static Vector3 ConvertWorldPositionToLevelPosition(LevelGeometry geometry, Vector3 world_pos)
	{
		// TODO: If we want to handle the level object translating and rotating, then convert here
		// Assume level is out origin with no rotation for now
		return world_pos;
	}

	public Vector3 ConvertWorldPositionToLevelPosition(Vector3 world_pos)
	{
		return ConvertWorldPositionToLevelPosition(m_geometry, world_pos);
	}

	public Vector3 ConvertLevelPositionToWorldPosition(Vector3 level_pos)
	{
		// TODO: If we want to handle the level object translating and rotating, then convert here
		// Assume level is out origin with no rotation for now
		return level_pos;
	}

	public Vector3 ConvertWorldVectorToLevelVector(Vector3 world_vec)
	{
		// TODO: If we want to handle the level object translating and rotating, then convert here
		// Assume level is out origin with no rotation for now
		return world_vec;
	}

	public static int FindSegmentContainingWorldPositionOLD(LevelGeometry geometry, Vector3 world_pos)
	{
		var level_pos = ConvertWorldPositionToLevelPosition(geometry, world_pos);
		Vector4 level_pos_homogeneous = new Vector4(level_pos.x, level_pos.y, level_pos.z, 1.0f);

		// Search the segments
		int num_segments = geometry.Segments.Length;
		for (int i = 0; i < num_segments; ++i) {
			var seg_data = geometry.Segments[i];
			if (level_pos.x < seg_data.MinCornerPos.x || level_pos.x > seg_data.MaxCornerPos.x ||
				 level_pos.y < seg_data.MinCornerPos.y || level_pos.y > seg_data.MaxCornerPos.y ||
				 level_pos.z < seg_data.MinCornerPos.z || level_pos.z > seg_data.MaxCornerPos.z) {
				// Not within the AABB
				continue;
			}

			// Within the AABB, but we still need to make sure it is within all of the sides
			bool is_inside = true;
			for (int side_idx = 0; side_idx < 6; ++side_idx) {
				float plane_eval = Vector4.Dot(seg_data.ApproxSidePlaneEquations[side_idx], level_pos_homogeneous);
				if (plane_eval < 0.0f) {
					// Not on the positive side of this side
					is_inside = false;
					break;
				}
			}

			if (!is_inside) {
				continue;
			}

			// We are in this segment
			return i;
		}

		// No segment found
		return -1;
	}

	// Returns:
	// true - contained
	// false - not contained
	public bool TraverseSegmentBSPFromNode(Vector4 pos, int root_node)
	{
		return TraverseSegmentBSPFromNode(m_geometry, pos, root_node);
	}

	public static bool TraverseSegmentBSPFromNode(LevelGeometry geometry, Vector4 pos, int root_node)
	{
		const float coplanar_normal_epsilon = 0.0001f;

		// Start traversing the BSP 
		int curr_node_index = root_node;
		while (curr_node_index != -1) {

			var plane_dist = Vector4.Dot(pos, geometry.PerSegmentBSPData[curr_node_index].PlaneEq);

			if (plane_dist >= -coplanar_normal_epsilon && plane_dist <= coplanar_normal_epsilon) {
				//
				// Consider to be on the splitter plane
				//
				int front_node_index = geometry.PerSegmentBSPData[curr_node_index].FrontNodeIndex;
				bool front_is_leaf = front_node_index == -1;
				if (front_is_leaf && plane_dist >= 0.0f) {
					// The front is a leaf and we technically were in front of it
					return true;
				}

				// Travel both both paths to find out how we should report this
				if (front_is_leaf == false) {
					bool front_result = TraverseSegmentBSPFromNode(geometry, pos, front_node_index);
					if (front_result) {
						// Consider it inside
						return true;
					}
				}

				// Either the front is a leaf and we are just slightly in back of it, or, the front
				// is not a leaf, and we were in back of it. So, we'll just consider us on the back
				// side of the plane.
				int back_node_index = geometry.PerSegmentBSPData[curr_node_index].BackNodeIndex;
				bool back_is_leaf = back_node_index == -1;
				if (back_is_leaf) {
					// at the leaf with no where to go
					return false;
				}

				// take the back path
				curr_node_index = back_node_index;
			} else if (plane_dist > 0.0f) {
				//
				// Consider to be on the front side of the splitter plane
				//
				int front_node_index = geometry.PerSegmentBSPData[curr_node_index].FrontNodeIndex;
				if (front_node_index == -1) {
					// at the leaf, we were in front of all we need to be
					return true;
				}

				// take the front path
				curr_node_index = front_node_index;
			} else {
				//
				// Consider to be on the back side of the splitter plane
				//
				int back_node_index = geometry.PerSegmentBSPData[curr_node_index].BackNodeIndex;
				if (back_node_index == -1) {
					// at the leaf with no where to go
					return false;
				}

				// take the back path
				curr_node_index = back_node_index;
			}
		}

		// Not sure how we got here...
		return false;
	}

	public static bool IsContainedWithinAABB(Vector3 pos, AABB aabb)
	{
		for (int i = 0; i < 3; ++i) {
			if (pos[i] < aabb.MinXYZ[i] || pos[i] > aabb.MaxXYZ[i]) {
				return false;
			}
		}
		return true;
	}

	private static int FindSegmentContainingWorldPositionWorker(LevelGeometry geometry, Vector3 pos, int curr_aabb_node)
	{
		if (!IsContainedWithinAABB(pos, geometry.SegmentAABBTree[curr_aabb_node].Bounds)) {
			// Not within the bounds - bail out
			return -1;
		}

		int min_child_index = geometry.SegmentAABBTree[curr_aabb_node].MinChildIndex;
		int max_child_index = geometry.SegmentAABBTree[curr_aabb_node].MaxChildIndex;

		if (min_child_index == -1 && max_child_index == -1) {
			// This is a leaf node, we found a possible segment, now dive into the BSP to see if it agrees
			int segment_index = geometry.SegmentAABBTree[curr_aabb_node].SegmentIndex;
			int bsp_root_index = geometry.SegmentRootIndicesIntoPerSegmentBSPData[segment_index];

			Vector4 pos_homogeneous = new Vector4(pos.x, pos.y, pos.z, 1.0f);
			bool is_within = TraverseSegmentBSPFromNode(geometry, pos_homogeneous, bsp_root_index);
			if (is_within) {
				return segment_index;
			}

			// not actually within the segment polygons
			return -1;
		}

		if (min_child_index != -1) {
			int res = FindSegmentContainingWorldPositionWorker(geometry, pos, min_child_index);
			if (res != -1) {
				// found a segment
				return res;
			}
		}

		if (max_child_index != -1) {
			int res = FindSegmentContainingWorldPositionWorker(geometry, pos, max_child_index);
			if (res != -1) {
				// found a segment
				return res;
			}
		}

		// nope
		return -1;
	}

	/// <summary>
	/// Query the level, given a position in world space, return the segment containing the position
	/// </summary>
	/// <param name="level_data">Level data</param>
	/// <param name="world_pos">The position in the world</param>
	/// <param name="hint_segment">If provided this segment will be checked first as an optimization</param>
	/// <returns>The segment index, -1 if the point is not within any segment</returns>
	/// If can't find segment containing world_pos, will check some nearby points.  If it finds one that is inside a segment, it will return value in new_pos.
	/// 
	public int FindSegmentContainingWorldPosition(Vector3 world_pos, int hint_segment = -1, bool recurse = true)
	{
		return FindSegmentContainingWorldPosition(m_geometry, world_pos, hint_segment, recurse);
	}

	public static int FindSegmentContainingWorldPosition(LevelGeometry geometry, Vector3 world_pos, int hint_segment = -1, bool recurse = true)
	{
		if (geometry.PerSegmentBSPData == null || geometry.PerSegmentBSPData.Length == 0 || geometry.SegmentAABBTree == null || geometry.SegmentAABBTree.Length == 0) {
			// Old level data - needs to be reconverted
			return FindSegmentContainingWorldPositionOLD(geometry, world_pos);
		}

		var level_pos = ConvertWorldPositionToLevelPosition(geometry, world_pos);

		int num_segments = geometry.Segments.Length;
		if (hint_segment >= 0 && hint_segment < num_segments) {
			// A hint has been provided, so first check to see if the position is within the segment AABB of the hint.
			AABB segment_aabb = new AABB();
			segment_aabb.MinXYZ = geometry.Segments[hint_segment].MinCornerPos;
			segment_aabb.MaxXYZ = geometry.Segments[hint_segment].MaxCornerPos;
			if (IsContainedWithinAABB(level_pos, segment_aabb)) {
				// It is plausible that this point is within the hint segment, check the BSP tree
				int bsp_root_index = geometry.SegmentRootIndicesIntoPerSegmentBSPData[hint_segment];

				Vector4 pos_homogeneous = new Vector4(level_pos.x, level_pos.y, level_pos.z, 1.0f);
				bool is_within = TraverseSegmentBSPFromNode(geometry, pos_homogeneous, bsp_root_index);
				if (is_within) {
					// yes!
					return hint_segment;
				}
			}
		}

		// traverse the segment AABB tree to find possible segments we might be in
		int segment_index = FindSegmentContainingWorldPositionWorker(geometry, level_pos, 0);

		if (segment_index == -1) {
			if (recurse) {
				segment_index = FSCWP_CheckNearbyPoints(geometry, world_pos);
				if (segment_index != -1) {
					// intentionally blank
				}
			} else {
				if (!FSCWP_message_shown) {      // only show once per occurrence
#if UNITY_EDITOR
					//Debug.LogError(Time.frameCount + ": Could not find segment containing point " + level_pos + " hint segment = " + hint_segment);
#endif
					FSCWP_message_shown = true;
				}
			}
		}

		return segment_index;
	}

	private static bool FSCWP_message_shown = false;

	// Debug/rescue function.
	// If segment containing world_pos cannot be found, check nearby points.
	private static int FSCWP_CheckNearbyPoints(LevelGeometry geometry, Vector3 world_pos)
	{
		Vector3 pos;
		FSCWP_message_shown = false;

		for (int i = 0; i < 5; i++) {
			for (int j = 0; j < 2; j++) {
				for (int k = 0; k < 3; k++) {
					int i2 = i * i;
					pos = world_pos;
					switch (k) {
						case 0:
							if (j == 0)
								pos.x += 0.1f * i2;
							else
								pos.x -= 0.1f * i2;
							break;
						case 1:
							if (j == 0)
								pos.y += 0.1f * i2;
							else
								pos.y -= 0.1f * i2;
							break;
						case 2:
							if (j == 0)
								pos.z += 0.1f * i2;
							else
								pos.z -= 0.1f * i2;
							break;
						default:
							Assert.True(false);
							break;
					}

					int segnum = FindSegmentContainingWorldPosition(geometry, pos, -1, false);    // last parm = false means don't further recurse!
					if (segnum != -1) {
						//print(Time.frameCount + ": Position " + world_pos + " not in any segment, but nearby position " + pos + " is in segment " + segnum);
						return segnum;
					}
				}
			}
		}

		return -1;
	}

	// Return true if the target point is visible from the view point
	// MK 2017-08-26: Added option to ignore doors for pathfinding through doors.  Robots could not see their next point, but that's OK in the case of a door.
	public bool SegmentVisibleFromPoint(Vector3 target_pos, Vector3 view_pos, bool ignore_doors = false)
	{
		RaycastHit hit;      //Don't care about this

		// Want to check only Level, Player, Door_Solid
		int layer_mask = (1 << (int)(UnityObjectLayers.LEVEL) | (1 << (int)UnityObjectLayers.LAVA));

		if (!ignore_doors) {
         layer_mask |= (1 << (int)(UnityObjectLayers.DOOR));
		}

		Vector3 vec;
		vec.x = target_pos.x - view_pos.x;
		vec.y = target_pos.y - view_pos.y;
		vec.z = target_pos.z - view_pos.z;

		float dist = (float)System.Math.Sqrt(vec.x * vec.x + vec.y * vec.y + vec.z * vec.z);
		vec.x /= dist;
		vec.y /= dist;
		vec.z /= dist;

		bool raycast_result = Physics.Raycast(view_pos, vec, out hit, dist, layer_mask);      // 999f is max distance

		return !raycast_result;
	}


}
