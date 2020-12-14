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
using System.Diagnostics;
using OpenTK;

// LEVEL - Editor
// General level functions that the editor uses
// (Some functions could possibly be moved into other Level sub-files)

namespace OverloadLevelEditor
{
	public partial class Level
	{
		public class QuadTriangulationOrder
		{
			// For non-coplanar
			public QuadTriangulationOrder(int[] vertIndices)
			{
				Debug.Assert(vertIndices.Length == 6);
				m_vertIndices = vertIndices;
			}

			// For coplanar
			public QuadTriangulationOrder()
			{
				m_vertIndices = null;
			}

			/// <summary>
			/// Get the vertex order for the quad's triangle
			/// </summary>
			/// <param name="triIndex">Which triangle of the quad (0 or 1)</param>
			/// <param name="inverted">Whether or not to invert (wind backwards) the verts, only pass true if you are working on a slave portal</param>
			/// <returns>an int[3]</returns>
			public int[] GetVertsForTriangle(int triIndex, bool inverted)
			{
				Debug.Assert(triIndex >= 0 && triIndex <= 1);

				if (m_vertIndices == null) {
					// Coplanar polygon, order doesn't matter too much
					int[] res = new int[] { 0, triIndex + 1, triIndex + 2 };
					if (inverted) {
						if (triIndex == 0) {
							res[2] = 3;
						} else {
							res[0] = 1;
						}
					}
					return res;
				} else {

					int baseIndex = triIndex * 3;
					int[] res = new int[] { m_vertIndices[baseIndex + 0], m_vertIndices[baseIndex + 1], m_vertIndices[baseIndex + 2] };

					if (inverted) {
						if (triIndex == 0) {
							if (res[2] == 2) {
								res[2] = 3;
							} else {
								res[2] = 2;
							}
						} else {
							if (res[0] == 0) {
								res[0] = 1;
							} else {
								res[0] = 0;
							}
						}
					}

					return res;
				}
			}

			//Returns the vertex number of one of the verts of the bisecting edge.  The other corner is the returned value + 2.
			public int GetBisectingEdgeCorner(bool inverted)
			{
				if (m_vertIndices == null) {
					// Coplanar polygon, order doesn't matter too much
					return 0;
				} else {

					//Third vertex is the one on the edge; return that number minus 2 (vert 0 or 1) unless inverted, in which case we toggle 0 <-> 1
					int corner = m_vertIndices[2] - 2;
					return inverted ? -(corner - 1) : corner;
				}
			}

			public bool IsCoplanar {
				get {
					return m_vertIndices == null;
				}
			}

			private int[] m_vertIndices;
		}

		public static Vector3 GetTriangleNormal(Vector3 v0, Vector3 v1, Vector3 v2)
		{
			Vector3 e0 = (v1 - v0).Normalized();
			Vector3 e1 = (v2 - v0).Normalized();
			return Vector3.Cross(e0, e1).Normalized();
		}

		public static QuadTriangulationOrder GetTriangulationOrder(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3)
		{
			// There are two ways to triangulate a non-coplanar quad:
			//  1) [ (0,1,2) and (0,2,3) ] or 
			//  2) [ (0,1,3) and (1,2,3) ]
			//
			// We want to pick the triangulation that results in the two triangles pointer towards each other (instead of away)

			// Assume triangulation #1...
			Vector3 normal0 = GetTriangleNormal(v0, v1, v2);
			float planeD0 = -Vector3.Dot(normal0, v0);
			float v3DistToPlane = Vector3.Dot(normal0, v3) + planeD0;

			// If v3 is on the same plane as v0, v1, v2 ... then it is a coplanar quad
			if (Math.Abs(v3DistToPlane) <= 0.00001f) {
				// Coplanar
				return new QuadTriangulationOrder();
			}

			// This is the correct triangulation if v3 is above the plane of [v0,v1,v2]
			if (v3DistToPlane > 0.0f) {
				return new QuadTriangulationOrder(new int[] { 0, 1, 2, 0, 2, 3 });
			}

			// We need to use triangulation #2
			Vector3 normal1 = GetTriangleNormal(v0, v1, v3);
			float planeD1 = -Vector3.Dot(normal1, v0);
			float v2DistToPlane = Vector3.Dot(normal1, v2) + planeD1;

			// Added this because it's happening too often with entities
			v2DistToPlane = Math.Max(v2DistToPlane, 0f);
			//Debug.Assert(v2DistToPlane >= 0.0f);

			return new QuadTriangulationOrder(new int[] { 0, 1, 3, 1, 2, 3 });
		}

		public static QuadTriangulationOrder GetTriangulationOrder(Vector3[] verts)
		{
			return GetTriangulationOrder(verts[0], verts[1], verts[2], verts[3]);
		}

		public static QuadTriangulationOrder GetTriangulationOrder(Level level, int segmentIndex, int sideIndex)
		{
			Segment segment = level.segment[segmentIndex];

			Side side = segment.side[sideIndex];
			if (side.HasNeighbor) {
				// Portals are always triangulated the same way
				// Master side: (0,1,2) (0,2,3)
				//  Slave side: (0,1,3) (1,2,3)
				// However, we store as the master side and only invert in TriangulationOrder
				return new QuadTriangulationOrder();
			}

			int[] vertIndices = side.vert;
			Vector3 v0 = level.vertex[vertIndices[0]].position;
			Vector3 v1 = level.vertex[vertIndices[1]].position;
			Vector3 v2 = level.vertex[vertIndices[2]].position;
			Vector3 v3 = level.vertex[vertIndices[3]].position;
			return GetTriangulationOrder(v0, v1, v2, v3);
		}
	}
}