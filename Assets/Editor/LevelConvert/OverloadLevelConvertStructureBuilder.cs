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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public partial class OverloadLevelConverter
{
	class StructureBuilder
	{
		/// <summary>
		/// Hold information about a connected segment (and through which side is it connected)
		/// </summary>
		struct Connection
		{
			public int SideOfConnection;
			public int ConnectedSegmentIndex;
		}

		/// <summary>
		/// Holds all the connections for a segment
		/// </summary>
		class SegmentConnectionInfo
		{
			public List<Connection> Connections = new List<Connection>();
		}

		//This class contains a list of portals.  The relal data will contain an array.
		public class TempChunkData
		{
			//The list of portal indices
			public List<int> PortalIndices = new List<int>();

			//The list of segments in this chunk
			public List<int> Segnums = new List<int>();

				//Set to true if the chunk contains an energy center material
				public bool HasEnergyCenterMaterial = false;
		}

		const float coplanar_normal_dot = 0.999999f;
		OverloadLevelEditor.Level m_levelData;
		Dictionary<int, int> m_editorSegmentIndexToPackedSegmentIndex;
		int[] m_packedSegmentIndexToEditorSegmentIndex;
		SegmentConnectionInfo[] m_segmentConnections; // Indexed from packed segment index
		int m_numPackedSegments;

		/// <summary>
		/// Helper function for the constructor to setup data given the level
		/// </summary>
		void PrepareForBuild(Dictionary<int, int> editorSegmentIndexToPackedSegmentIndex)
		{
			m_editorSegmentIndexToPackedSegmentIndex = editorSegmentIndexToPackedSegmentIndex;

			int highestPackedSegmentIndex = m_editorSegmentIndexToPackedSegmentIndex.Select(kvp => kvp.Value).Max();
			m_numPackedSegments = highestPackedSegmentIndex + 1;

			// Create the reverse mapping for packed -> editor
			m_packedSegmentIndexToEditorSegmentIndex = new int[m_numPackedSegments];

			for (int i = 0; i < m_numPackedSegments; ++i) {
				m_packedSegmentIndexToEditorSegmentIndex[i] = -1;
			}
			foreach (var kvp in m_editorSegmentIndexToPackedSegmentIndex) {
				m_packedSegmentIndexToEditorSegmentIndex[kvp.Value] = kvp.Key;
			}
			for( int i = 0; i < m_numPackedSegments; ++i ) {
				DebugAssert( m_packedSegmentIndexToEditorSegmentIndex[i] != -1 );
			}

			m_segmentConnections = new SegmentConnectionInfo[m_numPackedSegments];

			for (int i = 0; i < m_numPackedSegments; ++i) {
				m_segmentConnections[i] = new SegmentConnectionInfo();
			}
		}

		public StructureBuilder(OverloadLevelEditor.Level levelData, Dictionary<int, int> editorSegmentIndexToPackedSegmentIndex)
		{
			m_levelData = levelData;
			PrepareForBuild(editorSegmentIndexToPackedSegmentIndex);
		}


		// Returns volume of tetrahedron formed by points a, b, c, d.
		// V = ( (a-d) . ((b-d) X (c-d)) ) / 6
		public float TetrahedronVolume(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
		{
			float vol;
			Vector3 ad, bd, cd;

			ad = a - d;
			bd = b - d;
			cd = c - d;

			Vector3 bdcd;

			bdcd = Vector3.Cross(bd, cd);

			vol = Vector3.Dot(ad, bdcd) / 6.0f;

			return vol;

		}

		// Check for degenerate segment due to:
		//	 - Given a triangle and the four vertices on the opposite side, two verticies lie on opposite sides of the triangle's plane
		//	 - Two sides are very close together.
		// Return true if either of the above occur.
		//	public static readonly int[][] SideVertexOrder = new int[][] {
		//	new int[]{ 7, 6, 2, 3 }, // Left
		//	new int[]{ 0, 4, 7, 3 }, // Top
		//	new int[]{ 0, 1, 5, 4 }, // Right
		//	new int[]{ 2, 6, 5, 1 }, // Bottom
		//	new int[]{ 4, 5, 6, 7 }, // Front
		//	new int[]{ 3, 2, 1, 0 }, // Back
		//};
		public void SegmentDegenerateFacesNear(SegmentData segdata, LevelGeometry levelData, int segment_index)
		{
			Vector3 a, b, c;
			float[] distances = new float[4];
			float distance_sum = 0.0f;
			int other_side_index;

			for (int sidenum = 0; sidenum < 6; sidenum++) {
				int degen_count = 0;
				for (int trinum = 0; trinum < 2; trinum++) {
					//a = levelData.SegmentVerts[segdata.VertIndices[SegmentData.SideVertexOrder[i][0]]];
					a = levelData.SegmentVerts[segdata.VertIndices[SegmentData.SideVertexOrder[sidenum][0]]];
					if (trinum == 0) {
						b = levelData.SegmentVerts[segdata.VertIndices[SegmentData.SideVertexOrder[sidenum][1]]];
						c = levelData.SegmentVerts[segdata.VertIndices[SegmentData.SideVertexOrder[sidenum][2]]];
					} else {
						b = levelData.SegmentVerts[segdata.VertIndices[SegmentData.SideVertexOrder[sidenum][2]]];
						c = levelData.SegmentVerts[segdata.VertIndices[SegmentData.SideVertexOrder[sidenum][3]]];
					}

					Vector3 ab = b - a;
					Vector3 ac = c - a;
					Vector3 normal = Vector3.Cross(ab, ac).normalized;

					other_side_index = other_side_lookup[sidenum];

					for (int j = 0; j < 4; j++) {    // Check all vertices on opposite side
						distances[j] = Vector3.Dot(levelData.SegmentVerts[segdata.VertIndices[SegmentData.SideVertexOrder[other_side_index][j]]] - a, normal);
					}

					distance_sum = Math.Abs(distances[0]);

					// Have all distances.  If one is negative and any are positive, or vice versa, we have a degneracy.
					if (distances[0] < 0.0f) {
						for (int k = 1; k < 4; k++) {
							distance_sum += Math.Abs(distances[k]);

							if (distances[k] > 0.0f) {
								degen_count++;
							}
						}
					} else {
						for (int k = 1; k < 4; k++) {
							distance_sum += Math.Abs(distances[k]);

							if (distances[k] < 0.0f) {
								degen_count++;
							}
						}
					}
				}

				if (degen_count == 2) {    // Requiring both triangles to partition vertices on opposite side.  Requiring only one flagged too many not so bad segments.
					Debug.LogWarning("Segment " + segment_index + " is degenerate.  Side #" + other_side_lookup[sidenum] + " has vertices on both sides of side #" + sidenum + ".  Distances = " + distances[0] + " " + distances[1] + " " + distances[2] + " " + distances[3]);
				}

				if (distance_sum < 0.2f) {
					Debug.LogWarning("Segment " + segment_index + " is degenerate.  Sides " + other_side_lookup[sidenum] + " and " + sidenum + " are very, very close together");
				}
			}
		}

		int[] other_side_lookup = new int[] { 2, 3, 0, 1, 5, 4 };

		// Return the volume of a segment.
		// Creates 12 tetrahedra.  They are formed by the 12 side triangles and the segment center.
		public float SegmentVolume(SegmentData segdata, LevelGeometry levelData)
		{
			float vol = 0.0f;
			Vector3 center = segdata.Center;

			Vector3[] sideVerts = new Vector3[4];

			for (int i = 0; i < 6; i++) {

				sideVerts[0] = levelData.SegmentVerts[segdata.VertIndices[SegmentData.SideVertexOrder[i][0]]];
				sideVerts[1] = levelData.SegmentVerts[segdata.VertIndices[SegmentData.SideVertexOrder[i][1]]];
				sideVerts[2] = levelData.SegmentVerts[segdata.VertIndices[SegmentData.SideVertexOrder[i][2]]];
				sideVerts[3] = levelData.SegmentVerts[segdata.VertIndices[SegmentData.SideVertexOrder[i][3]]];
				QuadTriangulationOrder quadTriangulation = GetTriangulationOrder(sideVerts, false);

				int[] tri0 = quadTriangulation.GetVertsForTriangle(0, false);
				int[] tri1 = quadTriangulation.GetVertsForTriangle(1, false);

				Vector3 a, b, c;

				a = sideVerts[tri0[0]];
				b = sideVerts[tri0[1]];
				c = sideVerts[tri0[2]];
				vol += Mathf.Abs(TetrahedronVolume(a, b, c, center));

				a = sideVerts[tri1[0]];
				b = sideVerts[tri1[1]];
				c = sideVerts[tri1[2]];
				vol += Mathf.Abs(TetrahedronVolume(a, b, c, center));

			}

			return vol;
		}

		/// <summary>
		/// Define a connection from one segment to another
		/// </summary>
		/// <param name="fromSegmentIndex">Description for fromSegment</param>
		/// <param name="fromSideIndex"></param>
		/// <param name="toSegmentIndex"></param>
		public void AddNeighborSegment(int fromSegmentIndex, int fromSideIndex, int toSegmentIndex)
		{
			int packedFromIndex = m_editorSegmentIndexToPackedSegmentIndex[fromSegmentIndex];
			int packedToIndex = m_editorSegmentIndexToPackedSegmentIndex[toSegmentIndex];

			var sci = m_segmentConnections[packedFromIndex];
			sci.Connections.Add(new Connection { SideOfConnection = fromSideIndex, ConnectedSegmentIndex = packedToIndex });
		}

		public delegate IDisposable DoProgressUpdateDelegate( string update );

		[Flags]
		public enum BuildFlags
		{
			UNUSED = 1 << 0
		}

		/// <summary>
		/// Complete the creation of the pathfinding information
		/// </summary>
		public LevelGeometry Build( DoProgressUpdateDelegate DoProgressUpdate, BuildFlags buildFlags, Dictionary<PortalSideKey, List<Vector3[]>> portalTriangles )
		{
			var levelData = ScriptableObject.CreateInstance<LevelGeometry>();
			var portalList = new List<PortalData>();

			List<Vector3> portal_geom_vertices = new List<Vector3>();
			List<PortalGeomTriangle> portal_geom_triangles = new List<PortalGeomTriangle>();
			List<PortalGeomData> portal_geom_datas = new List<PortalGeomData>();

			List<ChunkPortal> chunk_portals = new List<ChunkPortal>();
			int num_chunks = 0;
			TempChunkData[] chunks = null;

			var energy_center_material_rx = new System.Text.RegularExpressions.Regex("energy_center", System.Text.RegularExpressions.RegexOptions.Compiled | System.Text.RegularExpressions.RegexOptions.IgnoreCase);

			using( DoProgressUpdate( "Building Portal/Segment structure" ) ) {

				//
				// map editor -> packed verts
				//
				// Create a map to map from editor vert indices to the packed indices
				var mapEditorVertToPackedVert = new Dictionary<int, int>();
				for( int editorVertIndex = 0; editorVertIndex < m_levelData.vertex.Length; ++editorVertIndex ) {
					if( !m_levelData.vertex[editorVertIndex].alive ) {
						continue;
					}

					int packedVertIndex = mapEditorVertToPackedVert.Count;
					mapEditorVertToPackedVert.Add( editorVertIndex, packedVertIndex );
				}

				//
				// build vertex array
				levelData.SegmentVerts = new Vector3[mapEditorVertToPackedVert.Count];
				foreach( var kvp in mapEditorVertToPackedVert ) {
					int editorIndex = kvp.Key;
					int packedIndex = kvp.Value;
					levelData.SegmentVerts[packedIndex] = m_levelData.vertex[editorIndex].position.ToUnity();
				}

				//
				// segment and portal structure
				levelData.Segments = new SegmentData[m_numPackedSegments];

				//Get the number of chunks
				for( int editorSegmentIndex = 0; editorSegmentIndex < OverloadLevelEditor.Level.MAX_SEGMENTS; ++editorSegmentIndex ) {
					if( m_levelData.segment[editorSegmentIndex].Alive ) {
						num_chunks = Math.Max( num_chunks, m_levelData.segment[editorSegmentIndex].m_chunk_num + 1 );
					}
				}

				//Array of chunk data
				chunks = new TempChunkData[num_chunks];
				for( int i = 0; i < num_chunks; i++ ) {
					chunks[i] = new TempChunkData();
				}

				//
				// Note: Go in editor segment index order, as portal construction requires it
				for( int editorSegmentIndex = 0; editorSegmentIndex < OverloadLevelEditor.Level.MAX_SEGMENTS; ++editorSegmentIndex ) {
					if( !m_levelData.segment[editorSegmentIndex].Alive ) {
						continue;
					}

					int packedSegmentIndex = m_editorSegmentIndexToPackedSegmentIndex[editorSegmentIndex];

					var editorSegmentData = m_levelData.segment[editorSegmentIndex];
					var packedSegmentData = new SegmentData();

					// Convert the editor data to the packed data

					// First the verts and center position
					int numVerts = editorSegmentData.vert.Length;
					packedSegmentData.VertIndices = new int[numVerts];
					packedSegmentData.Center = Vector3.zero;
					packedSegmentData.MinCornerPos = new Vector3( 99999999.9f, 99999999.9f, 99999999.9f );
					packedSegmentData.MaxCornerPos = -packedSegmentData.MinCornerPos;
					for( int vIdx = 0; vIdx < numVerts; ++vIdx ) {
						int editorVertIndex = editorSegmentData.vert[vIdx];
						int packedVertIndex = mapEditorVertToPackedVert[editorVertIndex];
						var vertPos = levelData.SegmentVerts[packedVertIndex];
						packedSegmentData.VertIndices[vIdx] = packedVertIndex;
						packedSegmentData.Center += vertPos;
						packedSegmentData.MinCornerPos = Vector3.Min( packedSegmentData.MinCornerPos, vertPos );
						packedSegmentData.MaxCornerPos = Vector3.Max( packedSegmentData.MaxCornerPos, vertPos );
					}
					packedSegmentData.Center *= 1.0f / (float)numVerts;

					//Set chunk num & editor segnumn
					packedSegmentData.ChunkNum = editorSegmentData.m_chunk_num;
					if( packedSegmentIndex != editorSegmentIndex ) {
						Debug.LogError( "Editor segment does not match game segment" );
					}
					chunks[editorSegmentData.m_chunk_num].Segnums.Add( editorSegmentIndex );     //Add segment to list of segs for its chunk

					//Some properties from the editor
					packedSegmentData.Pathfinding = (PathfindingType)editorSegmentData.m_pathfinding;
					packedSegmentData.Dark = editorSegmentData.m_dark;
					packedSegmentData.ExitSegment = (ExitSegmentType)editorSegmentData.m_exit_segment_type;

					//Deformation heights from the sides
					packedSegmentData.DeformationHeights = new float[editorSegmentData.side.Length];
					for (int sideIdx1 = 0; sideIdx1 < editorSegmentData.side.Length; ++sideIdx1) {
						packedSegmentData.DeformationHeights[sideIdx1] = editorSegmentData.side[sideIdx1].deformation_height;
					}

					// Next the side connection data and approximate side plane equations
					int numSides = editorSegmentData.neighbor.Length;
					packedSegmentData.Portals = new int[numSides];
					packedSegmentData.ApproxSidePlaneEquations = new Vector4[numSides];
					for( int sideIdx1 = 0; sideIdx1 < numSides; ++sideIdx1 ) {

						// Note: Sides that are portals are ultimately defined by their master segment.
						// This is important because sides are quads, but not necessary co-planar. The master
						// side will determine the triangulation of the side, and we need to have both sides
						// of the portal agree on the triangulation. So, we need to detect if we are working
						// on a side that is a portal here, and if so, and it is for a slave side, then we
						// need to munge the vertex order to match that of the master side.
						int neighborEditorSegIndex = editorSegmentData.neighbor[sideIdx1];
						bool isSlaveSideOfPortal = neighborEditorSegIndex != -1 && neighborEditorSegIndex < editorSegmentIndex;
						int[] quadSideVertexOrder = SegmentData.SideVertexOrder[sideIdx1];

						PortalGeomData portal_geom_data = null;

						//If there's a connection, add to the chunk's portal list
						if( ( neighborEditorSegIndex != -1 ) && ( m_levelData.segment[neighborEditorSegIndex].m_chunk_num != editorSegmentData.m_chunk_num ) ) {
							portal_geom_data = new PortalGeomData();
							ChunkPortal portal = new ChunkPortal() { Num = chunk_portals.Count, Chunknum = editorSegmentData.m_chunk_num, Segnum = packedSegmentIndex, Sidenum = sideIdx1, ConnectedChunk = m_levelData.segment[neighborEditorSegIndex].m_chunk_num, PortalGeomNum = portal_geom_datas.Count };
							chunk_portals.Add( portal );
							portal_geom_datas.Add( portal_geom_data );
							chunks[editorSegmentData.m_chunk_num].PortalIndices.Add( portal.Num );
						}

						int[] quadSideVertexIndices;
						int[] tri0VertexIndices;
						int[] tri1VertexIndices;
						bool isCoplanar;
						if( isSlaveSideOfPortal ) {
							//
							// Special case for a slave-side portal 
							//

							// Take the vertices of the master side as reference
							var editorMasterSegmentData = m_levelData.segment[neighborEditorSegIndex];
							int masterSideIndex1 = Enumerable.Range( 0, 6 )
								.Where( msi => editorMasterSegmentData.neighbor[msi] == editorSegmentIndex )
								.DefaultIfEmpty( -1 )
								.FirstOrDefault();
							if( masterSideIndex1 == -1 ) {
								throw new Exception( string.Format( "Corrupt File: Segment {0} (Side {1}) claims to be a neighbor to Segment {2}, but it does not agree.", editorSegmentIndex, sideIdx1, neighborEditorSegIndex ) );
							}
							int masterPackedSegment = m_editorSegmentIndexToPackedSegmentIndex[neighborEditorSegIndex];
							int[] masterSideVertexOrder = SegmentData.SideVertexOrder[masterSideIndex1];

							// Treat as if the master side data
							quadSideVertexOrder = SegmentData.SideVertexOrder[masterSideIndex1];
							var temp = new int[4];
							temp[0] = quadSideVertexOrder[0];
							temp[1] = quadSideVertexOrder[3];
							temp[2] = quadSideVertexOrder[2];
							temp[3] = quadSideVertexOrder[1];
							quadSideVertexOrder = temp;

							QuadTriangulationOrder masterSideOrdering = GetTriangulationOrder( m_levelData, masterPackedSegment, masterSideIndex1 );
							tri0VertexIndices = masterSideOrdering.GetVertsForTriangle( 0, true );
							tri1VertexIndices = masterSideOrdering.GetVertsForTriangle( 1, true );
							isCoplanar = masterSideOrdering.IsCoplanar;

							SegmentData masterPackedSegmentData = levelData.Segments[masterPackedSegment];
							tri0VertexIndices = tri0VertexIndices.Select( triVertIndex => masterPackedSegmentData.VertIndices[masterSideVertexOrder[triVertIndex]] ).ToArray();
							tri1VertexIndices = tri1VertexIndices.Select( triVertIndex => masterPackedSegmentData.VertIndices[masterSideVertexOrder[triVertIndex]] ).ToArray();
							quadSideVertexIndices = quadSideVertexOrder.Select( segmentVertex => masterPackedSegmentData.VertIndices[segmentVertex] ).ToArray();
						} else {
							//
							// General case for a side
							//
							QuadTriangulationOrder sideOrdering = GetTriangulationOrder( m_levelData, packedSegmentIndex, sideIdx1 );
							tri0VertexIndices = sideOrdering.GetVertsForTriangle( 0, false );
							tri1VertexIndices = sideOrdering.GetVertsForTriangle( 1, false );
							isCoplanar = sideOrdering.IsCoplanar;

							tri0VertexIndices = tri0VertexIndices.Select( triVertIndex => packedSegmentData.VertIndices[quadSideVertexOrder[triVertIndex]] ).ToArray();
							tri1VertexIndices = tri1VertexIndices.Select( triVertIndex => packedSegmentData.VertIndices[quadSideVertexOrder[triVertIndex]] ).ToArray();
							quadSideVertexIndices = quadSideVertexOrder.Select( segmentVertex => packedSegmentData.VertIndices[segmentVertex] ).ToArray();
						}

						var sideTri0VertexData = tri0VertexIndices.Select( vertexIndex => levelData.SegmentVerts[vertexIndex] ).ToArray();
						var sideTri1VertexData = tri1VertexIndices.Select( vertexIndex => levelData.SegmentVerts[vertexIndex] ).ToArray();

						if (portal_geom_data != null) {
							PortalSideKey psk = new PortalSideKey(editorSegmentIndex, sideIdx1);

							List<Vector3[]> side_portal_triangles = portalTriangles[psk];

							if (side_portal_triangles != null) {
								portal_geom_data.StartIndex = portal_geom_triangles.Count;
								portal_geom_data.NumTriangles = side_portal_triangles.Count;

								for (int i = 0; i < side_portal_triangles.Count; i++) {
									PortalGeomTriangle geomTriangle = new PortalGeomTriangle();

									geomTriangle.FirstVertIndex = portal_geom_vertices.Count;
									portal_geom_vertices.AddRange(side_portal_triangles[i]);

									portal_geom_triangles.Add(geomTriangle);
								}
							}
						}

						// calculate the normals of the two triangles
						Vector3 poly0_normal = GetTriangleNormal( sideTri0VertexData[0], sideTri0VertexData[1], sideTri0VertexData[2] );
						Vector3 poly1_normal = GetTriangleNormal( sideTri1VertexData[0], sideTri1VertexData[1], sideTri1VertexData[2] );

						// Average the normals to get the approximate normal for the side
						Vector3 approxNormal = ( ( poly0_normal + poly1_normal ) * 0.5f ).normalized;
						packedSegmentData.ApproxSidePlaneEquations[sideIdx1] = new Vector4( approxNormal.x, approxNormal.y, approxNormal.z, -Vector3.Dot( approxNormal, sideTri0VertexData[0] ) );

						// Check to see if there is an energy center material here and track that
						if (chunks[editorSegmentData.m_chunk_num].HasEnergyCenterMaterial == false && energy_center_material_rx.IsMatch(editorSegmentData.side[sideIdx1].tex_name)) {
							// Found an energy center material here
							chunks[editorSegmentData.m_chunk_num].HasEnergyCenterMaterial = true;
						}

						// MK -- THIS IS WHERE IT CHECKS TO SEE WHETHER A PORTAL SHOULD EXIST -- NOTE THAT IT IS NOT CHECKING FOR A 3D DECAL.
						if( neighborEditorSegIndex == -1 ) {
							// No neighbor to this side
							packedSegmentData.Portals[sideIdx1] = -1;
							continue;
						}

						// See if there is a 3d decal here.
						// Note that 3d decals are one-sided affecting only one segment/side.
						// Check the connected side when all segments are packed.  Simpler.
						bool hasDecal = editorSegmentData.side[sideIdx1].decal.Any( d => d != null && !string.IsNullOrEmpty( d.mesh_name ) && d.gmesh != null && !d.hidden );
						if( hasDecal ) {
							packedSegmentData.DecalFlags |= (uint)1 << sideIdx1;            // Yes!  Mark it
						}

						// See if there is a door here.  If so, put in DoorFlags.
						if( editorSegmentData.side[sideIdx1].Door != -1 ) {
							packedSegmentData.DoorFlags |= (uint)1 << sideIdx1;
						}

						// If our segment index is the greater, we are the slave side, and the portal information is already defined, we just need to connect to it
						int otherEditorSegmentIndex = editorSegmentData.neighbor[sideIdx1];
						int otherPackedSegmentIndex = m_editorSegmentIndexToPackedSegmentIndex[otherEditorSegmentIndex];
						Assert.True( otherEditorSegmentIndex == otherPackedSegmentIndex );   //These should be the same now
						if( otherEditorSegmentIndex < editorSegmentIndex ) {
							//
							// Slave Segment Side
							//
							// We have already processed the master segment of the portal, so just connect to the defined portal
							Assert.True( otherPackedSegmentIndex < packedSegmentIndex ); // just to make sure our invariant remains true
							var masterSegmentData = levelData.Segments[otherPackedSegmentIndex];
							int masterSideIndex2;
							int portalIndex = -1;
							for( masterSideIndex2 = 0; masterSideIndex2 < 6; ++masterSideIndex2 ) {
								portalIndex = masterSegmentData.Portals[masterSideIndex2];
								if( portalIndex == -1 )
									continue;
								if( portalList[portalIndex].SlaveSegmentIndex != packedSegmentIndex )
									continue;

								// This has to be a match because a segment isn't connecting to another segment
								// multiple times or we have a seriously distorted couple segments connected to each other
								packedSegmentData.Portals[sideIdx1] = portalIndex;
								break;
							}

							if( masterSideIndex2 == 6 ) {
								throw new Exception( "Matching segment for portal was not found" );
							}
						} else {
							//
							// Master Segment Side
							//
							// We are responsible for defining the portal
							Assert.True( packedSegmentIndex < otherPackedSegmentIndex ); // just to make sure our invariant remains true
							int portalIndex = portalList.Count;
							var portalData = new PortalData();
							portalList.Add( portalData );

							packedSegmentData.Portals[sideIdx1] = portalIndex;
							portalData.MasterSegmentIndex = packedSegmentIndex;
							portalData.MasterSideIndex = sideIdx1;
							portalData.SlaveSegmentIndex = otherPackedSegmentIndex;

							//Find side on slave segment
							int slave_sidenum;
							for( slave_sidenum = 0; slave_sidenum < 6; slave_sidenum++ ) {
								if( m_levelData.segment[otherPackedSegmentIndex].neighbor[slave_sidenum] == packedSegmentIndex ) {
									break;
								}
							}
							Assert.True( slave_sidenum != 6 );        //Bad if we couldn't find connection
							portalData.SlaveSideIndex = slave_sidenum;

							if( isCoplanar ) {
								// Coplanar!
								portalData.Polygons = new PortalPolygonData[1];
								portalData.Polygons[0].Normal = poly0_normal;
								portalData.Polygons[0].PlaneEqD = -Vector3.Dot( poly0_normal, sideTri0VertexData[0] );
								portalData.Polygons[0].VertIndices = new int[4] {
								quadSideVertexIndices[0], quadSideVertexIndices[1], quadSideVertexIndices[2], quadSideVertexIndices[3]
							};
							} else {
								// Non-planar
								portalData.Polygons = new PortalPolygonData[2];
								portalData.Polygons[0].Normal = poly0_normal;
								portalData.Polygons[0].PlaneEqD = -Vector3.Dot( poly0_normal, sideTri0VertexData[0] );
								portalData.Polygons[0].VertIndices = new int[3] {
								tri0VertexIndices[0], tri0VertexIndices[1], tri0VertexIndices[2]
							};

								portalData.Polygons[1].Normal = poly1_normal;
								portalData.Polygons[1].PlaneEqD = -Vector3.Dot( poly1_normal, sideTri1VertexData[0] );
								portalData.Polygons[1].VertIndices = new int[3] {
								tri1VertexIndices[0], tri1VertexIndices[1], tri1VertexIndices[2]
							};
							}
						} // end if/else master/slave selection
					} // end loop over sides

					levelData.Segments[packedSegmentIndex] = packedSegmentData;
					float v = SegmentVolume( packedSegmentData, levelData );
					if( v < 0.5f ) {
						Debug.Log( "Game segment #" + packedSegmentIndex + " (in editor: " + editorSegmentIndex + ") " + " has very small volume of " + v );
					}

					SegmentDegenerateFacesNear( packedSegmentData, levelData, packedSegmentIndex );
				}
			}

			using( DoProgressUpdate( "Propagating decals and doors" ) ) {
				PropagateDecalAndDoorFlags( levelData, portalList );
			}


			levelData.Portals = portalList.ToArray();

			levelData.PortalGeomVerts = portal_geom_vertices.ToArray();
			levelData.PortalGeomTriangles = portal_geom_triangles.ToArray();
			levelData.PortalGeomDatas = portal_geom_datas.ToArray();

			using ( DoProgressUpdate( "Building segment BSP tree" ) ) {
				//
				// segment AABB tree and per-segment BSP trees
				//
				// The segment AABB tree can be used to query spatial information of the segment bounds by an AABB tree
				// The per-segment BSP trees are mini-BSP trees per segment, from the segment geometry, that can be used as needed
				BuildBSPTree( levelData );
			}

			levelData.ChunkPortals = chunk_portals.ToArray();

			//Set chunk data
			levelData.Chunks = new ChunkData[num_chunks];
			for (int i = 0; i < num_chunks; i++) {
				levelData.Chunks[i] = new ChunkData();
				levelData.Chunks[i].PortalIndices = chunks[i].PortalIndices.ToArray();
				levelData.Chunks[i].Segnums = chunks[i].Segnums.ToArray();
				levelData.Chunks[i].IsEnergyCenter = chunks[i].HasEnergyCenterMaterial;
			}

			return levelData;
		}

		// Decal flags and door flags are associated with one segment:side.
		// Propagate to the connected side.
		// Note that the flags can be on either the master or slave segment.
		// Note that this code has some redundancy.
		// If segment 1 shares a portal with segment 2 and segment 1 contains the one-sided decal,
		// it will be propagated to segment 2 and then when segment 2 is processed, back to segment 1.
		// This is not a problem -- just a tiny waste of time.
		// Code cleanup: Make a separate function.  Share code for the master/slave distinction.
		// Apologies for weird variable names, wanted ones that would not confuse the debugger.
		void PropagateDecalAndDoorFlags(LevelGeometry levelData, List<PortalData> portalList)
		{
			for (int ii = 0; ii < levelData.Segments.Length; ii++) {
				for (int jj = 0; jj < 6; jj++) {
					bool decalflag = (levelData.Segments[ii].DecalFlags & ((uint)(1 << jj))) != 0;
					bool doorflag = (levelData.Segments[ii].DoorFlags & ((uint)(1 << jj))) != 0;
					if (decalflag || doorflag) {
						int p = levelData.Segments[ii].Portals[jj];
						var pp = portalList[p];
						int dest_seg_index = (pp.MasterSegmentIndex == ii) ? pp.SlaveSegmentIndex : pp.MasterSegmentIndex;

						// dest_seg_index needs to get decal and/or door bit flipped.
						// To find out which bit, need to see which side in dest_seg_index shares this portal.
						for (int kk = 0; kk < 6; kk++) {
							if (levelData.Segments[dest_seg_index].Portals[kk] == p) {
								if (decalflag) {
									levelData.Segments[dest_seg_index].DecalFlags |= ((uint)1 << kk);
								}

								if (doorflag) {
									levelData.Segments[dest_seg_index].DoorFlags |= ((uint)1 << kk);
								}

								break;
							}
						}
					}
				}
			}
		}

		#region BSP/AABB tree Generation
		public class BSPVertex : OverloadLevelEditor.Clipping.IVertex
		{
			public BSPVertex()
			{
			}

			public BSPVertex(Vector3 pos)
			{
				this.Position = new OpenTK.Vector3(pos.x, pos.y, pos.z);
			}

			public OpenTK.Vector3 Position
			{
				get;
				set;
			}

			public OverloadLevelEditor.Clipping.IVertex LerpTo(OverloadLevelEditor.Clipping.IVertex _other, float t)
			{
				BSPVertex other = (BSPVertex)_other;

				var newPos = OpenTK.Vector3.Lerp(this.Position, other.Position, t);
				return new BSPVertex { Position = newPos };
			}
		}

		struct BSPPolygon
		{
			public BSPVertex[] m_vert_data;
			public Vector4 m_plane_eq;
		}

		public class BSPNode
		{
			public Vector4 m_plane_eq;
			public BSPNode m_back_node = null;
			public BSPNode m_front_node = null;
		}

		public class AABBNode
		{
			public AABB m_aabb;
			public int m_segment_index = -1;
			public AABBNode m_min_child = null;
			public AABBNode m_max_child = null;
		}

		static BSPNode BuildBSPNodeRecursive(List<BSPPolygon> polys)
		{
			if (polys == null || polys.Count == 0) {
				return null;
			}

			BSPNode node = new BSPNode();

			// Choose a polygon to use as the split
			// TODO: Choose a better splitting polygon
			int num_total_polys = polys.Count;

			for (int split_poly_index = 0; split_poly_index < num_total_polys; ++split_poly_index) {
				node.m_plane_eq = polys[split_poly_index].m_plane_eq;

				var clip_plane = new OverloadLevelEditor.Clipping.ClipPlane();
				clip_plane.A = node.m_plane_eq.x;
				clip_plane.B = node.m_plane_eq.y;
				clip_plane.C = node.m_plane_eq.z;
				clip_plane.D = node.m_plane_eq.w;

				List<BSPPolygon> back_side_polys = new List<BSPPolygon>();
				List<BSPPolygon> front_side_polys = new List<BSPPolygon>();

				for (int poly_idx = 0; poly_idx < num_total_polys; ++poly_idx) {
					if (poly_idx == split_poly_index) {
						// Skip the splitter
						continue;
					}

					// split the triangle by the plane
					var split_info = OverloadLevelEditor.Clipping.Clipper.SplitPolygonByPlane(polys[poly_idx].m_vert_data, clip_plane);
					if (split_info.back != null) {
						var back_poly = new BSPPolygon();
						back_poly.m_plane_eq = polys[poly_idx].m_plane_eq;
						back_poly.m_vert_data = split_info.back.Cast<BSPVertex>().ToArray();
						back_side_polys.Add(back_poly);
					}

					if (split_info.front != null || split_info.coplaner != null) {
						var front_poly = new BSPPolygon();
						front_poly.m_plane_eq = polys[poly_idx].m_plane_eq;
						if (split_info.front != null) {
							front_poly.m_vert_data = split_info.front.Cast<BSPVertex>().ToArray();
						} else {
							front_poly.m_vert_data = split_info.coplaner.Cast<BSPVertex>().ToArray();
						}
						front_side_polys.Add(front_poly);
					}
				}

				if (back_side_polys.Count == 0 && front_side_polys.Count == 0) {
					// leaf node
					node.m_back_node = null;
					node.m_front_node = null;
					break;
				} else if ((back_side_polys.Count == 0 || front_side_polys.Count == 0) && split_poly_index < num_total_polys - 1) {
					// bad splitting plane, look for a better one
					continue;
				}

				// Recurse to the front side and back side lists and build up their nodes
				node.m_back_node = BuildBSPNodeRecursive(back_side_polys);
				node.m_front_node = BuildBSPNodeRecursive(front_side_polys);
				break;
			}

			return node;
		}

		static AABBNode BuildAABBTreeRecursive(List<AABBNode> base_nodes)
		{
			if (base_nodes.Count == 0) {
				return null;
			}

			if (base_nodes.Count == 1) {
				return base_nodes[0];
			}

			// get the bounding box for all nodes
			Vector3 total_min_xyz = base_nodes[0].m_aabb.MinXYZ;
			Vector3 total_max_xyz = base_nodes[0].m_aabb.MaxXYZ;
			for (int i = 1, count = base_nodes.Count; i < count; ++i) {
				total_min_xyz = Vector3.Min(total_min_xyz, base_nodes[i].m_aabb.MinXYZ);
				total_max_xyz = Vector3.Max(total_max_xyz, base_nodes[i].m_aabb.MaxXYZ);
			}

			// construct the root node
			var root_node = new AABBNode();
			root_node.m_aabb.MinXYZ = total_min_xyz;
			root_node.m_aabb.MaxXYZ = total_max_xyz;

			// which axis is the major axis?
			Vector3 axis_lengths = total_max_xyz - total_min_xyz;
			int major_axis = 0;
			if (axis_lengths[0] < axis_lengths[1]) {
				major_axis = 1;
			}
			if (axis_lengths[major_axis] < axis_lengths[2]) {
				major_axis = 2;
			}

			// sort the list of nodes by their center along the major axis
			base_nodes.Sort((AABBNode left, AABBNode right) => {
				float left_center = (left.m_aabb.MinXYZ[major_axis] + left.m_aabb.MaxXYZ[major_axis]) * 0.5f;
				float right_center = (right.m_aabb.MinXYZ[major_axis] + right.m_aabb.MaxXYZ[major_axis]) * 0.5f;

				int res = 0;
				if (left_center < right_center) {
					res = -1;
				} else if (left_center > right_center) {
					res = 1;
				}

				return res;
			});

			// split the nodes in half
			int num_left_side = base_nodes.Count / 2;
			var min_list = new List<AABBNode>();
			for (int i = 0; i < num_left_side; ++i) {
				min_list.Add(base_nodes[i]);
			}

			var max_list = new List<AABBNode>();
			for (int i = num_left_side, num_nodes = base_nodes.Count; i < num_nodes; ++i) {
				max_list.Add(base_nodes[i]);
			}

			root_node.m_min_child = BuildAABBTreeRecursive(min_list);
			root_node.m_max_child = BuildAABBTreeRecursive(max_list);

			return root_node;
		}

		public static void BuildBSPTree(LevelGeometry level_data)
		{
			// Generate a BSP tree for each segment, and then an AABB tree for all of the segments.
			// At runtime we can use the AABB tree to narrow down the probable segments, and then
			// use their individual BSP trees to determine containment.
			int num_segs = level_data.Segments.Length;
			BSPNode[] segment_bsp_roots = new BSPNode[num_segs];
			AABB[] segment_aabbs = new AABB[num_segs];
			for (int i = 0; i < num_segs; ++i) {
				var seg_data = level_data.Segments[i];

				// Build up the polygon list for this segment
				var segment_bsp_poly_list = new List<BSPPolygon>();

				Vector3 segment_min_pt = new Vector3(99999999.9f, 99999999.9f, 99999999.9f);
				Vector3 segment_max_pt = new Vector3(-99999999.9f, -99999999.9f, -99999999.9f);

				for (int side_index = 0; side_index < 6; ++side_index) {
					// Note: Sides that are portals are ultimately defined by their master segment.
					// This is important because sides are quads, but not necessary co-planar. The master
					// side will determine the triangulation of the side, and we need to have both sides
					// of the portal agree on the triangulation. So, we need to detect if we are working
					// on a side that is a portal here, and if so, and it is for a slave side, then we
					// need to munge the vertex order to match that of the master side.
					int portal_index = seg_data.Portals[side_index];
					bool is_slave_side_of_portal = portal_index != -1 && level_data.Portals[portal_index].SlaveSegmentIndex == i;
					int[] quad_side_vertex_order = SegmentData.SideVertexOrder[side_index];

					int[] quad_side_vertex_indices;
					int[] tri0_vertex_indices;
					int[] tri1_vertex_indices;
					bool is_coplanar;
					if (is_slave_side_of_portal) {
						//
						// Special case for a slave-side portal 
						//

						// Take the vertices of the master side
						int master_segment_index = level_data.Portals[portal_index].MasterSegmentIndex;
						var master_seg_data = level_data.Segments[master_segment_index];
						int master_side_index = Enumerable.Range(0, 6)
							.First(msi => master_seg_data.Portals[msi] == portal_index);

						// Treat as if the master side data
						quad_side_vertex_order = SegmentData.SideVertexOrder[master_side_index];

						// But swap indices [1] and [3] to reverse the winding order
						var temp = new int[4];
						temp[0] = quad_side_vertex_order[0];
						temp[1] = quad_side_vertex_order[3];
						temp[2] = quad_side_vertex_order[2];
						temp[3] = quad_side_vertex_order[1];
						quad_side_vertex_order = temp;

						quad_side_vertex_indices = quad_side_vertex_order.Select(segmentVertex => master_seg_data.VertIndices[segmentVertex]).ToArray();

						QuadTriangulationOrder master_side_ordering = GetTriangulationOrder(level_data, master_segment_index, master_side_index);
						tri0_vertex_indices = master_side_ordering.GetVertsForTriangle(0, true);
						tri1_vertex_indices = master_side_ordering.GetVertsForTriangle(1, true);
						is_coplanar = master_side_ordering.IsCoplanar;

						SegmentData master_packed_segment_data = level_data.Segments[master_segment_index];
						tri0_vertex_indices = tri0_vertex_indices.Select(tri_vert_index => master_packed_segment_data.VertIndices[quad_side_vertex_order[tri_vert_index]]).ToArray();
						tri1_vertex_indices = tri1_vertex_indices.Select(tri_vert_index => master_packed_segment_data.VertIndices[quad_side_vertex_order[tri_vert_index]]).ToArray();
						quad_side_vertex_indices = quad_side_vertex_order.Select(segment_vertex => master_packed_segment_data.VertIndices[segment_vertex]).ToArray();
					} else {
						//
						// General case for a side
						//

						QuadTriangulationOrder sideOrdering = GetTriangulationOrder(level_data, i, side_index);
						tri0_vertex_indices = sideOrdering.GetVertsForTriangle(0, false);
						tri1_vertex_indices = sideOrdering.GetVertsForTriangle(1, false);
						is_coplanar = sideOrdering.IsCoplanar;

						tri0_vertex_indices = tri0_vertex_indices.Select(tri_vert_index => seg_data.VertIndices[quad_side_vertex_order[tri_vert_index]]).ToArray();
						tri1_vertex_indices = tri1_vertex_indices.Select(tri_vert_index => seg_data.VertIndices[quad_side_vertex_order[tri_vert_index]]).ToArray();
						quad_side_vertex_indices = quad_side_vertex_order.Select(segment_vertex => seg_data.VertIndices[segment_vertex]).ToArray();
					}

					var side_tri0_vertex_data = tri0_vertex_indices.Select(vertex_index => level_data.SegmentVerts[vertex_index]).ToArray();
					var side_tri1_vertex_data = tri1_vertex_indices.Select(vertex_index => level_data.SegmentVerts[vertex_index]).ToArray();
					var quad_side_vertex_data = quad_side_vertex_indices.Select(vertex_index => level_data.SegmentVerts[vertex_index]).ToArray();

					// update min/max of the segment
					foreach (var vert in quad_side_vertex_data) {
						segment_min_pt = Vector3.Min(segment_min_pt, vert);
						segment_max_pt = Vector3.Max(segment_max_pt, vert);
					}

					// calculate the normals of the two triangles
					Vector3 poly0_normal = GetTriangleNormal(side_tri0_vertex_data[0], side_tri0_vertex_data[1], side_tri0_vertex_data[2]);
					Vector3 poly1_normal = GetTriangleNormal(side_tri1_vertex_data[0], side_tri1_vertex_data[1], side_tri1_vertex_data[2]);

					if (is_coplanar) {
						// coplanar!
						var bsp_poly = new BSPPolygon();
						bsp_poly.m_plane_eq = new Vector4(poly0_normal.x, poly0_normal.y, poly0_normal.z, -Vector3.Dot(poly0_normal, quad_side_vertex_data[0]));
						bsp_poly.m_vert_data = quad_side_vertex_indices
							.Select(vidx => new BSPVertex(level_data.SegmentVerts[vidx]))
							.ToArray();
						segment_bsp_poly_list.Add(bsp_poly);
					} else {
						// Not planar - split to two triangles
						float plane0_d = -Vector3.Dot(poly0_normal, side_tri0_vertex_data[0]);
						float plane1_d = -Vector3.Dot(poly1_normal, side_tri1_vertex_data[0]);

						var bsp_poly0 = new BSPPolygon();
						bsp_poly0.m_plane_eq = new Vector4(poly0_normal.x, poly0_normal.y, poly0_normal.z, plane0_d);
						bsp_poly0.m_vert_data = side_tri0_vertex_data
							.Select(v => new BSPVertex(v))
							.ToArray();

						var bsp_poly1 = new BSPPolygon();
						bsp_poly1.m_plane_eq = new Vector4(poly1_normal.x, poly1_normal.y, poly1_normal.z, plane1_d);
						bsp_poly1.m_vert_data = side_tri1_vertex_data
							.Select(v => new BSPVertex(v))
							.ToArray();

						segment_bsp_poly_list.Add(bsp_poly0);
						segment_bsp_poly_list.Add(bsp_poly1);
					}
				}

				// generate BSP for segment
				segment_bsp_roots[i] = BuildBSPNodeRecursive(segment_bsp_poly_list);

				// setup AABB for segment
				segment_aabbs[i].MinXYZ = segment_min_pt;
				segment_aabbs[i].MaxXYZ = segment_max_pt;
			}

			// This map allows us to go from a BSPNode to an index into a flattened array
			var bsp_node_to_tree_node_index = new Dictionary<BSPNode, int>();
			foreach (var root_node in segment_bsp_roots) {
				BuildBSPNodeMap(root_node, bsp_node_to_tree_node_index);
			}

			// Flatten all of the BSP nodes out into a linear array
			level_data.PerSegmentBSPData = new BSPTreeNode[bsp_node_to_tree_node_index.Count];
			foreach (var kvp in bsp_node_to_tree_node_index) {
				var bsp_node = kvp.Key;
				var flat_index = kvp.Value;

				level_data.PerSegmentBSPData[flat_index].PlaneEq = bsp_node.m_plane_eq;

				if (bsp_node.m_back_node == null) {
					level_data.PerSegmentBSPData[flat_index].BackNodeIndex = -1;
				} else {
					level_data.PerSegmentBSPData[flat_index].BackNodeIndex = bsp_node_to_tree_node_index[bsp_node.m_back_node];
				}

				if (bsp_node.m_front_node == null) {
					level_data.PerSegmentBSPData[flat_index].FrontNodeIndex = -1;
				} else {
					level_data.PerSegmentBSPData[flat_index].FrontNodeIndex = bsp_node_to_tree_node_index[bsp_node.m_front_node];
				}
			}

			// Get the root indices for each segment
			// ... while we are here, setup the base nodes for the AABB tree
			level_data.SegmentRootIndicesIntoPerSegmentBSPData = new int[num_segs];
			List<AABBNode> base_nodes = new List<AABBNode>();
			for (int i = 0; i < num_segs; ++i) {
				level_data.SegmentRootIndicesIntoPerSegmentBSPData[i] = bsp_node_to_tree_node_index[segment_bsp_roots[i]];

				var aabb_node = new AABBNode();
				aabb_node.m_aabb = segment_aabbs[i];
				aabb_node.m_segment_index = i;
				base_nodes.Add(aabb_node);
			}

			// Construct the AABB tree from the bottom up
			AABBNode root_aabb_node = BuildAABBTreeRecursive(base_nodes);

			// Get the node map to flatten the array
			var aabb_node_to_index = new Dictionary<AABBNode, int>();
			BuildAABBNodeMap(root_aabb_node, aabb_node_to_index);

			// Flatten all of the AABB tree nodes out into a linear array
			level_data.SegmentAABBTree = new AABBTreeNode[aabb_node_to_index.Count];
			foreach (var kvp in aabb_node_to_index) {
				var tree_node = kvp.Key;
				var flat_index = kvp.Value;

				level_data.SegmentAABBTree[flat_index].Bounds = tree_node.m_aabb;
				level_data.SegmentAABBTree[flat_index].SegmentIndex = tree_node.m_segment_index;

				if (tree_node.m_min_child == null) {
					level_data.SegmentAABBTree[flat_index].MinChildIndex = -1;
				} else {
					level_data.SegmentAABBTree[flat_index].MinChildIndex = aabb_node_to_index[tree_node.m_min_child];
				}

				if (tree_node.m_max_child == null) {
					level_data.SegmentAABBTree[flat_index].MaxChildIndex = -1;
				} else {
					level_data.SegmentAABBTree[flat_index].MaxChildIndex = aabb_node_to_index[tree_node.m_max_child];
				}
			}
		}

		static void BuildBSPNodeMap(BSPNode node, Dictionary<BSPNode, int> node_to_tree_node_index)
		{
			if (node == null)
				return;

			// Insert ourself into the map
			int index = node_to_tree_node_index.Count;
			node_to_tree_node_index.Add(node, index);

			// Recurse
			BuildBSPNodeMap(node.m_back_node, node_to_tree_node_index);
			BuildBSPNodeMap(node.m_front_node, node_to_tree_node_index);
		}

		static void BuildAABBNodeMap(AABBNode node, Dictionary<AABBNode, int> aabb_node_to_index)
		{
			if (node == null)
				return;

			// Insert ourself into the map
			int index = aabb_node_to_index.Count;
			aabb_node_to_index.Add(node, index);

			// Recurse
			BuildAABBNodeMap(node.m_min_child, aabb_node_to_index);
			BuildAABBNodeMap(node.m_max_child, aabb_node_to_index);
		}

		#endregion

		#region Path Distances
		class PathDistanceBuild
		{
			public PathDistanceBuild(int numSegs, bool canRobotsUseDoors)
			{
				m_numSegs = numSegs;
				m_SegmentBuffer = new bool[m_numSegs];
				m_PathNodes = new PathNode[6 * m_numSegs];
				m_assumeRobotCanUseDoors = canRobotsUseDoors;
			}

			public struct PathNode
			{
				public float distance;     // Distance from beginning of path
				public int segnum;         // segment
				public int parent;         // How I got here from start_seg
			}

			public PathNode[] m_PathNodes;
			public bool[] m_SegmentBuffer;
			public HashSet<int> m_portalIndexWithDoor = new HashSet<int>();
			public bool m_assumeRobotCanUseDoors;
			private int m_numSegs;

			public void MarkAllSegments()
			{
				for (int i = 0; i < m_numSegs; i++) {
					m_SegmentBuffer[i] = false;
				}

				m_PathNodes[0].segnum = -1;
			}
		}

		enum SideOrder
		{
			// Left (7623)
			LEFT,
			// Top (0473)
			TOP,
			// Right (0154)
			RIGHT,
			// Bottom (2651)
			BOTTOM,
			// Front (3210)
			FRONT,
			// Back (4567)
			BACK,
		}

		static int OppositeSide(int side_idx)
		{
			switch (side_idx) {
				case (int)SideOrder.LEFT: return (int)SideOrder.RIGHT;
				case (int)SideOrder.RIGHT: return (int)SideOrder.LEFT;
				case (int)SideOrder.TOP: return (int)SideOrder.BOTTOM;
				case (int)SideOrder.BOTTOM: return (int)SideOrder.TOP;
				case (int)SideOrder.FRONT: return (int)SideOrder.BACK;
				case (int)SideOrder.BACK: return (int)SideOrder.FRONT;
				default: return (int)SideOrder.FRONT;
			}
		}

		// In cur_segment.  Wondering if we can enter next_segment.  Portals[portal_num] connects them.
		// Return true if next_segment can be legally entered with regard to direction due to wind direction.
		// Keep in mind that some of the pathfinding functions create the path backwards.  In that case, you must call OppositeSide to determine if entry is legal!
		// Works for Guide-Bot, too, who should only pass downwind through wind tunnels.
		static bool LegalEntryPortal(LevelGeometry level_data, int cur_segment, int portal_num, bool backwards)
		{
			int next_segment;
			int next_segment_entry_side;

			PortalData portal = level_data.Portals[portal_num];

			if (portal.MasterSegmentIndex == cur_segment) {
				next_segment = portal.SlaveSegmentIndex;
				next_segment_entry_side = portal.SlaveSideIndex;
			} else {
				next_segment = portal.MasterSegmentIndex;
				next_segment_entry_side = portal.MasterSideIndex;
			}

			SegmentData nextsegp = level_data.Segments[next_segment];

			if (nextsegp.Pathfinding < PathfindingType.PORTAL_0 || nextsegp.Pathfinding > PathfindingType.PORTAL_5)
				return true;

			int portal_int = (int)(nextsegp.Pathfinding) - (int)(PathfindingType.PORTAL_0);

			if (backwards) {
				portal_int = OppositeSide(portal_int);
			}

			return (portal_int == next_segment_entry_side);
		}

		// Stuff path in an array
		// Note that since the path was created backwards, "end_seg" is our final goal, but end_index is the segment where the path begins.
		private static int StorePath1(PathDistanceBuild build, int[] dest, int end_index, int start_seg, int end_seg)
		{
			int pindex = 0;

			int cur_index = end_index;

			if (end_index == -1) {
#if !OVERLOAD_LEVEL_EDITOR
				Debug.Log(Time.frameCount + ": end_index = -1 in StorePath.  No path stored.");
#endif
				return -1;
			}

			while (cur_index != 0) {
				int segnum = build.m_PathNodes[cur_index].segnum;

				if (dest != null) {
					dest[pindex++] = segnum;
				}

				cur_index = build.m_PathNodes[cur_index].parent;

				if (pindex >= 1000) {
					return -1;
				}
			}

			if (dest != null) {
				dest[pindex] = end_seg;
			}

			pindex++;
			return pindex;
		}

		static bool FindShortestPath(LevelGeometry level_data, PathDistanceBuild build, int start_seg, int end_seg, int avoid_seg, float max_distance, int[] path, out int path_length, out float path_distance)
		{
			Vector3 parent_pos;
			int pathnode_index = 0;
			int cur_seg;
			int final_seg_index;
			int highest_segment_index = 1;   // Highest index of a segment in m_PathNodes[]. You can stop checking when you hit this.
			cur_seg = end_seg;
			path_distance = 0.0f;

			if (start_seg == -1 || end_seg == -1) {
				//Debug.log("Illegal start or end seg, values = " + start_seg + " " + end_seg);
				path_length = -1;
				return false;
			}

			if (level_data.Segments[cur_seg].Pathfinding == PathfindingType.NONE) {
				//Debug.Log("End segment is in a no pathfinding segment -- there is no legal path!");
				path_length = -1;
				return false;
			}

			parent_pos = level_data.Segments[cur_seg].Center;

			build.MarkAllSegments();
			if (avoid_seg != -1) {
				build.m_SegmentBuffer[avoid_seg] = true;
			}

			// Put start_seg in list.
			build.m_PathNodes[0].segnum = cur_seg;
			build.m_PathNodes[0].distance = 0f;
			build.m_PathNodes[0].parent = -1;
			build.m_SegmentBuffer[cur_seg] = true;    // mark as used

			pathnode_index = 1;
			int pi;

			for (pi = 0; pi < highest_segment_index + 1; pi++) {
				if (build.m_PathNodes[pi].segnum != -1) {
					parent_pos = level_data.Segments[build.m_PathNodes[pi].segnum].Center;

					// Add segments reachable through six portals to m_PathNodes.

					cur_seg = build.m_PathNodes[pi].segnum;

					int k = 0;

					for (int i = 0; i < 6; i++) {
						var thisSeg = level_data.Segments[cur_seg];
						int portalIdx = thisSeg.Portals[i];
						if (portalIdx != -1) {
							var portal = level_data.Portals[portalIdx];

							// If there is a 3d decal on this side, treat the portal as impassable
							if ((thisSeg.DecalFlags & (uint)(1 << i)) != 0)
								continue;

							// If there is a door on this side, see if robot can open this door
							if( build.m_portalIndexWithDoor.Contains(portalIdx) && !build.m_assumeRobotCanUseDoors ) {
								continue;
							}

							if ((thisSeg.Pathfinding == PathfindingType.NONE && pi != 0) || (thisSeg.Pathfinding == PathfindingType.GUIDEBOT_ONLY))    // This segment is flagged as not allowed to be part of a path.
								continue;

							if (!LegalEntryPortal(level_data, cur_seg, portalIdx, true)) {
								continue;
							}

							int destSegIdx = portal.MasterSegmentIndex == cur_seg ? portal.SlaveSegmentIndex : portal.MasterSegmentIndex;

							if (!build.m_SegmentBuffer[destSegIdx]) {             // true = already used
								float cur_distance = build.m_PathNodes[pi].distance + (level_data.Segments[destSegIdx].Center - parent_pos).magnitude;  // Accumulating path length in meters, not segments.
								if (cur_distance < max_distance) {
									build.m_PathNodes[pathnode_index + k].distance = cur_distance;
									build.m_PathNodes[pathnode_index + k].segnum = destSegIdx;     // Not explored yet.  Mark as on some path.
									build.m_PathNodes[pathnode_index + k].parent = pi; // want array index, not segment m_PathNodes[pi].segnum;
									build.m_SegmentBuffer[destSegIdx] = true;          // mark as used
									k++;
									highest_segment_index = pathnode_index + k;
								} else {    // Mark this segment as not visitable due to exceeding max_distance.
									build.m_PathNodes[pathnode_index + k].segnum = -1;
								}
							} else {
								build.m_PathNodes[pathnode_index + k].segnum = -1;
							}

							if (destSegIdx == start_seg) {
								final_seg_index = pathnode_index + k - 1;    // sub 1 because of increment ~6 lines above.
								path_distance = build.m_PathNodes[final_seg_index].distance;
								goto FSP_path_found;
							}
						} else {
							build.m_PathNodes[pathnode_index + k].segnum = -1;
						}
					}

					pathnode_index += k;
				}
			}

			final_seg_index = -1;

			FSP_path_found:
			;
			if (pi > highest_segment_index) {
				// Note that this condition isn't necessarily a problem.  Sometimes we try to create a path to find out if a segment is reachable,
				// but that doesn't mean there's a problem if there is no path.
				//Debug.log( "Line #844: Could not create path from " + start_seg + " to " + end_seg );
				path_length = -1;
				return false;
			}

			if (final_seg_index == -1) {
				final_seg_index = -1;
			}

			path_length = StorePath1(build, path, final_seg_index, start_seg, end_seg);
			if (path_length == -1) {
				return false;
			}

			return true;
		}

		static float ComputePathDistance(LevelGeometry level_data, PathDistanceBuild build, int seg1, int seg2, out int path_length, out int second_segment, out int second_last_segment)
		{
			var segments = level_data.Segments;
			int len = segments.Length;

			int[] out_path = new int[len];
			float path_distance;
			bool r = FindShortestPath(level_data, build, seg1, seg2, -1, 9999f, out_path, out path_length, out path_distance);

			if (path_length < 2) {
				second_segment = -1;
				second_last_segment = -1;
			} else {
				second_segment = out_path[1];
				second_last_segment = out_path[path_length - 2];
			}

			if (!r) {
				return 0f;     // No path between two segments.
			}
			return path_distance;
		}

		private static LevelData.SpawnPoint ConvertToSpawnPoint(LevelGeometry levelData, OverloadLevelEditor.Level.LocatorInfo loc)
		{
			var sp = new LevelData.SpawnPoint(loc.Pos.ToUnity(), loc.Rotation.ToUnity(), 0);
			sp.m_current_segment = LevelData.FindSegmentContainingWorldPosition(levelData, sp.position);
			return sp;
		}

		public static void ComputePathDistances(LevelGeometry levelData, OverloadLevelEditor.Level overloadLevelData, Dictionary<int, int> dooNumToPortalMap, bool canRobotsOpenDoors )
		{
			LevelData.SpawnPoint[] robotSpawnPoints = overloadLevelData.ExtractRobotSpawnPoints()
				.Select(loc => ConvertToSpawnPoint(levelData, loc))
				.ToArray();

			var spawnp = robotSpawnPoints;
			int numSegments = levelData.Segments.Length;
			int numSpawnPoints = spawnp.Length;

			// PathDistances[numSpawnPoints][numSegments]
			var pathDistances = new PathDistanceData[numSpawnPoints * numSegments];
			PathDistanceBuild build = new PathDistanceBuild(numSegments, canRobotsOpenDoors);

			if (dooNumToPortalMap != null) {
				foreach (var doorPortalKVP in dooNumToPortalMap) {
					int portalIndex = doorPortalKVP.Value;
					if (portalIndex >= 0 && portalIndex < levelData.Portals.Length) {
						build.m_portalIndexWithDoor.Add(portalIndex);
					} else {
						Debug.LogError(string.Format("Portal Index {0} is out of range for door", portalIndex));
					}
				}
			}

			for (int i = 0; i < numSpawnPoints; i++) {
				// Iterate over all spawn points
				int spawn_seg = spawnp[i].m_current_segment;
				if (spawn_seg < 0 || spawn_seg >= numSegments) {
					throw new Exception("Spawn point segment out of bounds.  Index: " + i + " Position: " + spawnp[i].position);
				}

				for (int j = 0; j < numSegments; j++) {
					int arrayIndex = i * numSegments + j; // [numSpawnPoints][numSegments]

					if (spawn_seg == j) {
						// This spawn point starts in the same segment we are ending in
						pathDistances[arrayIndex].Distance = 0.1f;
						continue;
					}

					int path_length, second_segment, second_last_segment;
					float dist = ComputePathDistance(levelData, build, spawn_seg, j, out path_length, out second_segment, out second_last_segment);
					if (dist == 0f) {
						dist = 999f;    // Ick, ugly hack.
					}

					pathDistances[arrayIndex].Distance = dist;
					pathDistances[arrayIndex].PathLength = path_length;
					pathDistances[arrayIndex].SecondSegment = second_segment;
				}
			}

			levelData.PathDistances = pathDistances;
		}
#endregion
    }
}
