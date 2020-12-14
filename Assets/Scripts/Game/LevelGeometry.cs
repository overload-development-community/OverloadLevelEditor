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
using Overload;		//Or should this class be in the Overload namepace?

#if OVERLOAD_LEVEL_EDITOR
// Some stub classes so this file will compile
public class DoorBase : MonoBehaviour
{
}
#endif

[System.Serializable]
public struct PortalPolygonData
{
	/// <summary>
	/// The normal vector for the portal polygon
	/// Normal points into master segment
	/// </summary>
	public Vector3 Normal;

	/// <summary>
	/// The 'D' component of the plane equation for the portal polygon, where A, B, and C are the normal
	/// </summary>
	public float PlaneEqD;

	/// <summary>
	/// The vertices that form the portal polygon
	/// size = 3 or 4
	/// </summary>
	public int[] VertIndices;
}

[System.Serializable]
public class PortalData
{
	/// <summary>
	/// The master segment index for this portal
	/// Always the lower value between the two segment indices
	/// </summary>
	public int MasterSegmentIndex;

	/// <summary>
	/// The side on the master segment for this portal
	/// </summary>
	public int MasterSideIndex;

	/// <summary>
	/// The slave segment index for this portal
	/// Always the higher value between the two segment indices
	/// </summary>
	public int SlaveSegmentIndex;

	/// <summary>
	/// The side on the slave segment for this portal
	/// </summary>
	public int SlaveSideIndex;

	/// <summary>
	/// A portal will always be 4 verts, but not guaranteed to be planar, so it could be
	/// either 1 four-vertex polygon, or 2 three-vertex polygons to define it.
	/// Data stored in the Polygons array is ordered for the MasterSegmentIndex, and will
	/// need to be reversed/inverted for the SlaveSegmentIndex
	/// </summary>
	public PortalPolygonData[] Polygons;

	/// <summary>
	/// The door in this portal, or null if none
	/// </summary>
	public DoorBase DoorData;
}

public enum SegmentLightType
{
	/// <summary>
	/// This light came from a decal and was placed in the scene automatically by the level converter
	/// </summary>
	Decal,

	/// <summary>
	/// The user / level designer placed this light into the Unity scene under "!lights" container
	/// </summary>
	UserPlaced,
}

public enum SegmentReflectionProbeType
{
	/// <summary>
	/// This reflection probe was placed in the scene automatically by the level converter
	/// </summary>
	ConverterGenerated,

	/// <summary>
	/// The user / level designer placed this reflection probe into the Unity scene under "!probes" container
	/// </summary>
	UserPlaced,
}

[System.Serializable]
public class SegmentLightInfo
{
	/// <summary>
	/// The instance of the light
	/// </summary>
#if OVERLOAD_LEVEL_EDITOR
	public OverloadLevelExport.IGameObjectBroker LightObject;
#else
	public GameObject LightObject;
	public Vector3 Position;
	public bool IsActive = true;
	public float original_range;
	public float range_goal;
#endif

	/// <summary>
	/// What kind of light
	/// </summary>
	public SegmentLightType LightType;

#if !OVERLOAD_LEVEL_EDITOR
	/// <summary>
	/// The light component
	/// </summary>
	public Light LightComp;
#endif

	/// <summary>
	/// Which segment "owns" this light
	/// </summary>
	public int SegmentIndex;
}

[System.Serializable]
public class SegmentReflectionProbeInfo
{
	/// <summary>
	/// The instance of the reflection probe
	/// </summary>
#if OVERLOAD_LEVEL_EDITOR
	public OverloadLevelExport.IGameObjectBroker ProbeObject;
#else
	public GameObject ProbeObject;
#endif

	/// <summary>
	/// What kind of probe
	/// </summary>
	public SegmentReflectionProbeType ProbeType;

	/// <summary>
	/// Which segment "owns" this probe
	/// </summary>
	public int SegmentIndex;
}

//This must mirror the PathfindingType enum in Segment.cs
public enum PathfindingType {
	ALL,
	GUIDEBOT_ONLY,
	NONE,
	PORTAL_0,			// Robots can pathfind through this segment, but only if they enter via portal #0.  And they must exit the opposite side, whatever that is.
	PORTAL_1,			// etc...
	PORTAL_2,
	PORTAL_3,
	PORTAL_4,
	PORTAL_5,
};

public enum ExitSegmentType { Start, End, None, NUM };

[System.Serializable]
public class SegmentData
{
	/// <summary>
	/// Vertex information for the segment
	/// size = 8
	/// </summary>
	public int[] VertIndices;

	/// <summary>
	/// Center position for the segment
	/// </summary>
	public Vector3 Center;

	/// <summary>
	/// The minimum corner position of the surrounding AABB
	/// </summary>
	public Vector3 MinCornerPos;

	/// <summary>
	/// The maximum corner position of the surrounding AABB
	/// </summary>
	public Vector3 MaxCornerPos;

	/// <summary>
	/// The approximate plane equation information for each side
	/// Note: If a side is not coplanar than this will be as close as it can get
	/// </summary>
	public Vector4[] ApproxSidePlaneEquations;

	/// <summary>
	/// The side portal information, for each side, provides the index of the connecting portal
	/// -1 if the side is solid
	/// Note: The lower segment index is always the master-segment to a portal, while the
	/// higher segment index of the two connecting segments is the slave-segment to the portal.
	/// Portal data is defined ordered for the master side.
	/// </summary>
	public int[] Portals;

	// Note: The order of the verts of a Portal are controlled by the Master segment side
	// of the portal. If you are on the Slave segment side and want to convert the side
	// information as the portal, you'll have to reverse the order of verts for your side.
	public static readonly int[][] SideVertexOrder = new int[][] {
		new int[]{ 7, 6, 2, 3 }, // Left
		new int[]{ 0, 4, 7, 3 }, // Top
		new int[]{ 0, 1, 5, 4 }, // Right
		new int[]{ 2, 6, 5, 1 }, // Bottom
		new int[]{ 4, 5, 6, 7 }, // Front
		new int[]{ 3, 2, 1, 0 }, // Back
	};

	/// <summary>
	/// Which rendering chunk this segment is in
	/// </summary>
	public int ChunkNum = -1;

	public uint DecalFlags = 0;			// set bit in 0..5 to indicate side contains a 3d decal
	public uint DoorFlags = 0;          // set bit in 0..5 to indicate side contains a door

	public bool Dark = false;                               // True if segment is set to be "dark", marking it harder for robots to see the player
	public PathfindingType Pathfinding = PathfindingType.ALL;       // Whether all robots, no robots, or just the Guidebot can get through this segment
	public ExitSegmentType ExitSegment = ExitSegmentType.None;

	/// <summary>
	/// What level lights are in this segment
	/// </summary>
	public SegmentLightInfo[] Lights;

	/// <summary>
	/// What level reflection probes are in this segment
	/// </summary>
	public SegmentReflectionProbeInfo[] ReflectionProbes;

	/// <summary>
	/// The deformation height of each of the six sides
	/// </summary>
	public float[] DeformationHeights;

	/// <summary>
	/// -1 = no warper on this side,
	/// else segment # you warp to
	/// </summary>
	public int[] WarpDestinationSegs;
}

/// <summary>
/// Portal information for chunks
/// </summary>
[System.Serializable]
public class ChunkPortal
{
	//Which portal this is
	public int Num;

	//Which chunk this belongs to
	public int Chunknum;

	//Which segment contains the portal
	public int Segnum;

	//Which side is the portal
	public int Sidenum;

	//Which chunk this connects to
	public int ConnectedChunk;

	//Which portal this connects to
	public int ConnectedPortal = -1;

    //Which entry in the "full portal geometry" array contains the triangles for this portal.
    public int PortalGeomNum = -1;
}

/// <summary>
/// Portal triangles, used to render the portals in the automap.
/// </summary>
[System.Serializable]
public class PortalGeomTriangle
{
    public int FirstVertIndex;    // There will always be three indices per triangle, because triangle.
}

/// <summary>
/// Full deformed geometry information for portals, used to render them in the automap.
/// </summary>
[System.Serializable]
public class PortalGeomData
{
    // Index of the first vertex in the PortalGeomTriangles array
    public int StartIndex;

    // Number of triangles for this portal
    public int NumTriangles;
}

/// <summary>
/// Data needed by the portal renderer
/// </summary>
[System.Serializable]
public class ChunkData
{
	//The list of indices into the big list of portals
	public int[] PortalIndices;

	//The list of segments in this chunk
	public int[] Segnums;

	//Is this chunk (part of) an energy center
	public bool IsEnergyCenter;
}

/// <summary>
/// A node representation of the segment BSP tree
/// </summary>
[System.Serializable]
public struct BSPTreeNode
{
	/// <summary>
	/// The plane equation of the separator plane at this node level
	/// </summary>
	public Vector4 PlaneEq;

	/// <summary>
	/// Index into the BSPTreeNode[] to the list of nodes that lie on the back side of the splitter plane of this node (-1 if there is no data)
	/// </summary>
	public int BackNodeIndex;

	/// <summary>
	/// Index into the BSPTreeNode[] to the list of nodes that lie on the front side of the splitter plane of this node (-1 if there is no data)
	/// </summary>
	public int FrontNodeIndex;
}

/// <summary>
/// An AABB representation
/// </summary>
[System.Serializable]
public struct AABB
{
	/// <summary>
	/// The minimum corner
	/// </summary>
	public Vector3 MinXYZ;

	/// <summary>
	/// The maximum corner
	/// </summary>
	public Vector3 MaxXYZ;
}

[System.Serializable]
public struct AABBTreeNode
{
	/// <summary>
	/// The bounds of this node
	/// </summary>
	public AABB Bounds;

	/// <summary>
	/// The index into the array to move to the minimum side of the split (-1 if none)
	/// </summary>
	public int MinChildIndex;

	/// <summary>
	/// The index into the array to move to the maximum side of the split (-1 if none)
	/// </summary>
	public int MaxChildIndex;

	/// <summary>
	/// If this is a leaf node (MinChildIndex and MaxChildIndex are -1), this is the segment index represented at this bounds
	/// </summary>
	public int SegmentIndex;
}

[System.Serializable]
public struct PathDistanceData
{
    public float Distance;
    public int PathLength;
    public int SecondSegment;
    public int SecondLastSegment;
}

[System.Serializable]
public class LevelGeometry : ScriptableObject {
	/// <summary>
	/// The name of the source (.overload) file without the extension
	/// </summary>
	[HideInInspector]
	public string FileName;

	/// <summary>
	/// Segment array
	/// size = NumSegments
	/// </summary>
	[HideInInspector]
    public SegmentData[] Segments;

    /// <summary>
    /// Portal connection information
    /// </summary>
    [HideInInspector]
	public PortalData[] Portals;

	/// <summary>
	/// Vertex position information, shared by Segments[] and Portals[]
	/// </summary>
	[HideInInspector]
	public Vector3[] SegmentVerts;

	/// <summary>
	/// Access the index into PerSegmentBSPData, by segment, to start a BSP query
	/// </summary>
	[HideInInspector]
	public int[] SegmentRootIndicesIntoPerSegmentBSPData;

	/// <summary>
	/// Holds BSP tree information per-segment
	/// </summary>
	[HideInInspector]
	public BSPTreeNode[] PerSegmentBSPData;

	/// <summary>
	/// Holds the AABB tree for all of the segments (root index == 0)
	/// </summary>
	[HideInInspector]
	public AABBTreeNode[] SegmentAABBTree;

	/// <summary>
	/// Stores the chunk connectivity information needed for culling
	/// </summary>
	[HideInInspector]
	public ChunkData[] Chunks;

	/// <summary>
	/// Stores the chunk's portals
	/// </summary>
	[HideInInspector]
	public ChunkPortal[] ChunkPortals;

    /// <summary>
    /// Vertex position information for portal geometry - used by automap.
    /// </summary>
    [HideInInspector]
    public Vector3[] PortalGeomVerts;

    /// <summary>
    /// Triangle information for portal geometry - used by automap.
    /// </summary>
    [HideInInspector]
    public PortalGeomTriangle[] PortalGeomTriangles;

    /// <summary>
    /// Triangle information for portal geometry - used by automap.
    /// </summary>
    [HideInInspector]
    public PortalGeomData[] PortalGeomDatas;

    /// <summary>
    /// Text that contains challenge mode data.  Will eventually make this real structured data
    /// </summary>
    [HideInInspector]
	public string ChallengeModeDataText;

	/// <summary>
	/// 2D array that will determine if two segments (the indices into the array) can see each other or not
	/// 0 -- not visible
	/// 1 -- directly visible
	/// 2 -- indirectly visible by distance
	/// </summary>
	[HideInInspector]
	public int[] SegmentToSegmentVisibility;

    /// <summary>
    /// 2D array [numRobotSpawnPoints][numSegments]
	 /// numRobotSpawnPoints -- see m_level_data.m_robot_spawn_points
    /// </summary>
	 [HideInInspector]
    public PathDistanceData[] PathDistances;

    /// <summary>
    /// Hash of the original level segment and decal geometry. This is used in optimizing level re-conversion.
    /// </summary>
    public string GeometryHash;

	/// <summary>
	/// Hash of the original level's robot spawn points. This is used in optimizing level re-conversion.
	/// </summary>
	public string RobotSpawnPointsHash;
}
