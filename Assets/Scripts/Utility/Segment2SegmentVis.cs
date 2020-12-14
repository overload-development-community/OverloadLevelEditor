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

public static class Segment2SegmentVis
{
	// If seg1 is visible from seg2, return true.
	// Cast a bunch of rays between the two.  If all intersect the world, return false.
	// Assumes segments are convex.
	// For now: Assumes solid doors are open.  Complex solution is to assume doors are closed, then at runtime can cast vector based on actual door state.
	static bool TestSegmentToSegmentVisibility(LevelGeometry levelData, int seg1, int seg2)
	{
		if (seg1 == seg2) {  // You can always see yourself
			return true;
		}

		var verts = levelData.SegmentVerts;
		var seg1p = levelData.Segments[seg1];
		var seg2p = levelData.Segments[seg2];

		// If segments share an open portal, then assume visibility
		for (int i = 0; i < 6; i++) {
			var p = seg1p.Portals[i];
			if (p != -1) {
				if (levelData.Portals[p].SlaveSegmentIndex == seg2)
					return true;
				else if (levelData.Portals[p].MasterSegmentIndex == seg2)
					return true;
			}
		}

		for (int i = 0; i < 6; i++) {
			var p = seg2p.Portals[i];
			if (p != -1) {
				if (levelData.Portals[p].SlaveSegmentIndex == seg1)
					return true;
				else if (levelData.Portals[p].MasterSegmentIndex == seg1)
					return true;
			}
		}

		Vector3 center1 = seg1p.Center;
		Vector3 center2 = seg2p.Center;

		int layer_mask = (1 << ((int)Overload.UnityObjectLayers.LEVEL)) | (1 << ((int)Overload.UnityObjectLayers.BUILTIN_0) | (1 << ((int)Overload.UnityObjectLayers.LAVA)));

		Vector3 p1, p2;
		float seg1_numerator = 7f;
		float seg1_denominator = 8f;
		float seg2_numerator = 7f;
		float seg2_denominator = 8f;

		bool seg1_deformed = m_segment_is_deformed[seg1];
		bool seg2_deformed = m_segment_is_deformed[seg2];

		if (seg1_deformed) {
			seg1_numerator = 3f;
			seg1_denominator = 4f;
		}

		if (seg2_deformed) {
			seg2_numerator = 3f;
			seg2_denominator = 4f;
		}

		for (int i = 0; i < 9; i++) {       // Why 9?  Because: 8 vertices plus center.
			if (i == 8) {
				p1 = center1;
			} else {
				p1 = (verts[seg1p.VertIndices[i]] * seg1_numerator + center1) / seg1_denominator;        // Point near corner, towards center

				if (seg1_deformed && !m_center_visibility[seg1, i]) {
					continue;
				}

				// Verify that p1 is inside the segment.  Deformation can cause it to be outside.  It's possible the center is outside, too!  Do both directions because Linecast not reliable otherwise.
				//if (Physics.Linecast(p1, center1, layer_mask) || Physics.Linecast(center1, p1, layer_mask)) {
				//	// p1 is outside the world, do not use it.
				//	continue;
				//}

			}

			for (int j = 0; j < 9; j++) {
				if (j == 8) {
					p2 = center2;
				} else {
					p2 = (verts[seg2p.VertIndices[j]] * seg2_numerator + center2) / seg2_denominator;         // Point near corner, towards center

					if (seg2_deformed && !m_center_visibility[seg2, j]) {
						continue;
					}

					//if (Physics.Linecast(center2, p2, layer_mask) || Physics.Linecast(p2, center2, layer_mask)) {
					//	continue;
					//}
				}

				// Do both directions because these points can be outside world and return wrong result
				if (!(Physics.Linecast(p1, p2, layer_mask) || Physics.Linecast(p2, p1, layer_mask))) {
					// If just one point can see the other, then there is visibility
					return true;
				}
			}
		}

		return false;
	}

	static bool[,] m_center_visibility;   // cache.  True means a near-vertex point has visibility to its segment center.  If it doesn't, then it is probably outside the world due to deformation.

	static bool[] m_segment_is_deformed;  // Set true for segments which contain at least one deformed side.

	public static void ComputeSegmentVisibility(LevelGeometry levelData, float secondaryVisibilityDistance)
	{
		int num_segs = levelData.Segments.Length;
		int[] segmentToSegmentVisibility = new int[num_segs * num_segs];
		m_segment_is_deformed = new bool[num_segs];

		m_center_visibility = new bool[num_segs, 8];          // visibility from segment center to 8 near-vertex points.

		// Initialize m_center_visibility which tells whether a vertex can see the center of its segment.
		var verts = levelData.SegmentVerts;
		int layer_mask = (1 << ((int)Overload.UnityObjectLayers.LEVEL)) | (1 << ((int)Overload.UnityObjectLayers.BUILTIN_0) | (1 << ((int) Overload.UnityObjectLayers.LAVA)));

		// Determine visibility from center to near-vertex point for all segments
		for (int i = 0; i < num_segs; i++) {
			var segp = levelData.Segments[i];
			Vector3 p_center = segp.Center;

			// Deformed segments have their points pulled in closer to the center.
			float numerator = 7f;
			float denominator = 8f;
			bool stuff_center_visibility_array = true;

			for (int j = 0; j < 6; j++) {
				if (segp.DeformationHeights[j] != 0f) {
					stuff_center_visibility_array = false;
					m_segment_is_deformed[i] = true;

					numerator = 3f;
					denominator = 4f;

					for (int k = 0; k < 8; k++) {
						Vector3 p1 = (verts[segp.VertIndices[k]] * numerator + p_center) / denominator;        // Point near corner, towards center

						bool r = Physics.Linecast(p1, p_center, layer_mask) || Physics.Linecast(p_center, p1, layer_mask);
						m_center_visibility[i, k] = !r;
					}
					break;
				}
			}

			if (stuff_center_visibility_array) {
				for (int j = 0; j < 8; j++) {			// Was 6, changed to 8 by MK on 2018-03-26
					m_center_visibility[i, j] = true;
				}
			}
		}

		// Initialize direct visibility
		for (int i = 0; i < num_segs; i++) {

			// Every segment can see itself
			segmentToSegmentVisibility[(i * num_segs) + i] = 1;

			for (int j = 0; j < i; j++) {
				int res = TestSegmentToSegmentVisibility(levelData, i, j) ? 1 : 0;
				segmentToSegmentVisibility[(i * num_segs) + j] = res;
				segmentToSegmentVisibility[(j * num_segs) + i] = res;
			}
		}

		float squareSecondaryVisibilityDistance = secondaryVisibilityDistance * secondaryVisibilityDistance;

		// Mark segments indirectly visible segments. A segment is indirectly visible if it is within
		// secondaryVisibilityDistance meters from the center of a directly visible segment AND is
		// directly visible to the checked segment.
		//
		// If:
		// * Segment[i] is directly visible to Segment[j]
		// * Segment[k] is directly visible to Segment[j]
		// * Segment[k] is NOT directly visible to Segment[i]
		// * dist( center(Segment[k]), center(Segment[j]) ) < secondaryVisibilityDistance
		// Then:
		// * Segment[k] is indirectly visible to Segment[i]
		//
		// Note: The inverse (Segment[i] is indirectly visible to Segment[k]) is not necessarily true as
		// dist( center(Segment[i]), center(Segment[j]) ) isn't guaranteed to be < secondaryVisibilityDistance
		for (int i = 0; i < num_segs; i++) {
			for (int j = 0; j < num_segs; j++) {
				Vector3 centerSegmentJ = levelData.Segments[j].Center;
				if (segmentToSegmentVisibility[(i * num_segs) + j] != 1) {
					// j is not directly visible from i -- skip
					continue;
				}

				// Mark all segments (Segment[k]) within secondaryVisibilityDistance from the center of Segment[j] as
				// indirectly visible to Segment[i]. Segment[k] *must* be directly visible to Segment[j].
				for (int k = 0; k < num_segs; k++) {
					if (segmentToSegmentVisibility[(j * num_segs) + k] != 1) {
						// k is not directly visible from j -- skip
						continue;
					}
					if (segmentToSegmentVisibility[(i * num_segs) + k] != 0) {
						// k is already visible to i
						continue;
					}

					if ((centerSegmentJ - levelData.Segments[k].Center).sqrMagnitude < squareSecondaryVisibilityDistance) {
						// Mark Segment[k] as indirectly visible to Segment[i]
						segmentToSegmentVisibility[(i * num_segs) + k] = 2;
					}
				}
			}
		}

		m_center_visibility = null;
		m_segment_is_deformed = null;
		levelData.SegmentToSegmentVisibility = segmentToSegmentVisibility;
	}
}
