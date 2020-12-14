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

using System.Collections.Generic;

namespace OverloadLevelEditor
{
	public static class ArrayExtensions
	{
		public static void Init<T>(this T[] array, T defaultVaue)
		{
			if (array == null)
				return;
			for (int i = 0; i < array.Length; i++) {
				array[i] = defaultVaue;
			}
		}
	}

	public static class Chunking
	{
		public static bool EnableAutomaticChunking = false;

		private enum SegmentType { Single, Connector, Complex }

		private static Level m_level;
		private static int m_num_chunks;
		private static int[] m_chunk_size = new int[Level.MAX_SEGMENTS];
		private static bool[] m_connected = new bool[Level.MAX_SEGMENTS];

		//Find the side of the segment the connects to a segment *not* equal to enter_seg
		private static int FindExitSide(Segment seg, int enter_seg)
		{
			for (int s = 0; s < 6; s++) {
				if ((seg.neighbor[s] != -1) && (seg.neighbor[s] != enter_seg))
					return s;
			}

			return -1;
		}

		private const int MAX_CONNECTOR_CHUNK_SIZE = 5;

		public static void FindConnectedSegments(Segment start_seg)
		{
			int chunknum = start_seg.m_chunk_num;

			m_connected.Init(false);
			List<Segment> connected = new List<Segment>(Level.MAX_SEGMENTS);

			connected.Add(start_seg);
			m_connected[start_seg.num] = true;
			int current = 0;

			//Build list of neighbors, and keep processing them until there aren't any more
			while (current < connected.Count) {
				Segment seg = connected[current];

				for (int s = 0; s < Segment.NUM_SIDES; s++) {
					int neighbor = seg.neighbor[s];
					if (neighbor != -1) {
						Segment neighbor_seg = m_level.segment[neighbor];
						if (neighbor_seg.m_chunk_num == chunknum) {
							if (!m_connected[neighbor]) {
								connected.Add(m_level.segment[neighbor]);
								m_connected[neighbor] = true;
							}
						}
					}
				}
				current++;
			}
		}

		//Determine if there are disconnected segments in this group, and if so rejoin them to the proper chunk 
		private static void RejoinDisconnectedChunks(int chunk_segnum, int other_chunknum)
		{
			Segment chunk_seg = m_level.segment[chunk_segnum];
			int chunknum = chunk_seg.m_chunk_num;

			//Find all segments attached to this segment
			FindConnectedSegments(chunk_seg);

			//Now move any non-connected segments back to the original chunk
			foreach (Segment seg in m_level.EnumerateAliveSegments()) {
				if ((seg.m_chunk_num == chunknum) && !m_connected[seg.num]) {
					seg.m_chunk_num = other_chunknum;
					m_chunk_size[chunknum]--;
					m_chunk_size[other_chunknum]++;
				}
			}
		}

		struct SplitPlane
		{
			public int m_segnum, m_sidenum;
			public int m_order;

			public SplitPlane(int segnum, int sidenum, int order)
			{
				m_segnum = segnum;
				m_sidenum = sidenum;
				m_order = order;
			}
		}

		public static void DetermineLevelChunking(Level level)
		{
			m_level = level;
			m_num_chunks = 0;

			int num_segments = 0;

			SegmentType[] SegmentTypes = null;
			if (EnableAutomaticChunking) {
				SegmentTypes = new SegmentType[Level.MAX_SEGMENTS];
			}

			//Clear out chunk count array
			m_chunk_size.Init(0);

			if (!EnableAutomaticChunking) {

				//No automatic chunking, so put everything in one chunk
				foreach (int segmentIndex in level.EnumerateAliveSegmentIndices()) {
					level.segment[segmentIndex].m_chunk_num = 0;
					num_segments++;
				}
				m_chunk_size[0] = num_segments;
				m_num_chunks = 1;

			} else {

				//Loop through segment, assigning each to a chunk
				foreach (int segmentIndex in level.EnumerateAliveSegmentIndices()) {

					var segmentData = level.segment[segmentIndex];
					num_segments++;

					//Count the number of portals
					int num_portals = 0;
					for (int sideIdx = 0; sideIdx < 6; ++sideIdx) {
						if (segmentData.neighbor[sideIdx] != -1) {
							num_portals++;
						}
					}

					//Figure out what kind of segment; default to complex
					SegmentTypes[segmentIndex] = SegmentType.Complex;
					if (num_portals == 1) {
						SegmentTypes[segmentIndex] = SegmentType.Single;
					} else if (num_portals == 2) {

						//If two portals, see if they're opposite
						if (((segmentData.neighbor[0] != -1) && (segmentData.neighbor[2] != -1)) ||
									((segmentData.neighbor[1] != -1) && (segmentData.neighbor[3] != -1)) ||
									((segmentData.neighbor[4] != -1) && (segmentData.neighbor[5] != -1))) {

							//Ok, we've got two opposite sides. 
							SegmentTypes[segmentIndex] = SegmentType.Connector;
						}
					}

					//Now decide what to do with this segment
					int chunk_num = -1;
					for (int sideIdx = 0; sideIdx < 6; ++sideIdx) {
						if (segmentData.neighbor[sideIdx] != -1) {
							int other = segmentData.neighbor[sideIdx];

							//Only look at lower-numbered segments, which will already have type set
							if (other < segmentIndex) {

								//Join with the other if it's the same as us, or if we're a signel or it's a single
								if ((SegmentTypes[other] == SegmentTypes[segmentIndex]) || (SegmentTypes[segmentIndex] == SegmentType.Single) || (SegmentTypes[other] == SegmentType.Single)) {
									int other_chunk_num = level.segment[other].m_chunk_num;

									//We should join with the other segment.  See if we're already in a chunk
									if (chunk_num == -1) {

										//We're not in a chunk, so join with the other
										chunk_num = other_chunk_num;

									} else {
										//We're already in a chunk, so move all segments from other chunk to our chunk
										for (int t = 0; t < segmentIndex; t++) {
											if (level.segment[t].Alive) {
												if (level.segment[t].m_chunk_num == other_chunk_num) {
													level.segment[t].m_chunk_num = chunk_num;
													m_chunk_size[other_chunk_num]--;
													m_chunk_size[chunk_num]++;
												}
											}
										}
									}
								}
							}
						}
					}

					//If we didn't find a chunk to join, start a new one
					if (chunk_num == -1) {
						chunk_num = m_num_chunks++;
					}

					segmentData.m_chunk_num = chunk_num;
					m_chunk_size[chunk_num]++;
				}

				//Now, go through and split up any overly big connector chunks
				//Sorry that this code is so ugly and hard to follow
				for (int chunknum = 0; chunknum < m_num_chunks; chunknum++) {

					//If more than five, split
					if (m_chunk_size[chunknum] > MAX_CONNECTOR_CHUNK_SIZE) {

						//Determine if this is a connector chunk, and if so find terminal segment
						Segment seg = null;
						int segnum, sidenum = -1, other_segnum = -1;
						for (segnum = 0; segnum < Level.MAX_SEGMENTS; segnum++) {
							seg = level.segment[segnum];
							if (seg.Alive && (seg.m_chunk_num == chunknum)) {

								//If this is a complex chunk, bail
								if (SegmentTypes[segnum] == SegmentType.Complex)
									break;

								//It's a connector; see if this one is the terminus
								if (SegmentTypes[segnum] == SegmentType.Connector) {

									for (sidenum = 0; sidenum < 6; sidenum++) {
										other_segnum = seg.neighbor[sidenum];
										if (other_segnum != -1) {
											if (SegmentTypes[other_segnum] != SegmentType.Connector) {      //Connected to something else, so this is terminus
												break;
											}
										}
									}
								}
								if (sidenum < 6)
									break;
							}
						}
						if ((segnum < Level.MAX_SEGMENTS) && (SegmentTypes[segnum] == SegmentType.Connector)) {   //We've found a terminal connector

#if OVERLOAD_LEVEL_EDITOR
						level.editor.AddOutputText("Spitting chunk " + chunknum + "( " + m_chunk_size[chunknum] + " segments)");
#endif
							int exit_side = FindExitSide(seg, other_segnum);

							//Compute size of new chunk(s)
							int num_sub_chunks = (m_chunk_size[chunknum] + MAX_CONNECTOR_CHUNK_SIZE - 1) / MAX_CONNECTOR_CHUNK_SIZE;
							int new_chunk_size = (m_chunk_size[chunknum] + num_sub_chunks / 2) / num_sub_chunks;

							//The first batch of segments stay in the old chunk
							for (int t = new_chunk_size; t > 0; t--) {
								exit_side = FindExitSide(seg, other_segnum);
								if (exit_side != -1) {
									other_segnum = segnum;
									segnum = seg.neighbor[exit_side];
									seg = level.segment[segnum];
								}
							}

							//Walk through chain until halfway, moving segments to new chunk
							int new_chunk_num = m_num_chunks++;
							int moved_count = 0;

							while ((exit_side != -1) && ((SegmentTypes[segnum] == SegmentType.Connector) || (SegmentTypes[segnum] == SegmentType.Single))) {
								seg.m_chunk_num = new_chunk_num;
								m_chunk_size[chunknum]--;
								m_chunk_size[new_chunk_num]++;
								moved_count++;
								exit_side = FindExitSide(seg, other_segnum);
								if (exit_side != -1) {
									other_segnum = segnum;
									segnum = seg.neighbor[exit_side];
									seg = level.segment[segnum];
								}
								if (moved_count == new_chunk_size) {
									new_chunk_num = m_num_chunks++;
									moved_count = 0;
								}
							}
						}
					}
				}
			} // if EnableAutomaticChunking

			//Get all the manually-added separation planes
			List<SplitPlane> planes = new List<SplitPlane>();
			foreach (int segnum in level.EnumerateAliveSegmentIndices()) {
				Segment seg = level.segment[segnum];

				for (int sidenum = 0; sidenum < 6; sidenum++) {
					Side side = seg.side[sidenum];
					if (side.chunk_plane_order != -1) {
						planes.Add(new SplitPlane(segnum, sidenum, side.chunk_plane_order));
					}
				}
			}

			//If we have any planes, sort them and do the splits
			if (planes.Count > 0) {
				//Sort planes
				planes.Sort((p1, p2) => p1.m_order.CompareTo(p2.m_order));

				//Process the planes in order
				foreach (SplitPlane plane in planes) {
					Segment seg = level.segment[plane.m_segnum];
					Side side = seg.side[plane.m_sidenum];

					int connected_segnum = seg.neighbor[plane.m_sidenum];

					//Ok, found one.  Split the chunk this segment is in.
					int old_chunk_num = seg.m_chunk_num;
					int new_chunk_num = m_num_chunks++;
					OpenTK.Vector3 sep_normal = side.FindNormal();
					OpenTK.Vector3 sep_vert = level.vertex[side.vert[0]].position;

					foreach (int checksegnum in level.EnumerateAliveSegmentIndices()) {
						Segment checkseg = level.segment[checksegnum];
						if (checkseg.m_chunk_num != old_chunk_num) {
							continue;
						}

						if (OpenTK.Vector3.Dot(checkseg.FindCenter() - sep_vert, sep_normal) < 0.0f) {
							checkseg.m_chunk_num = new_chunk_num;
							m_chunk_size[old_chunk_num]--;
							m_chunk_size[new_chunk_num]++;
						}
					}

					//Check to see if either of resulting chunks contain non-connected geometry, and if so add them back to the proper chunk
					if (connected_segnum != -1) {
						RejoinDisconnectedChunks(plane.m_segnum, new_chunk_num);
						RejoinDisconnectedChunks(connected_segnum, old_chunk_num);
					}
				}
			}

			// Compact the chunks array
			int[] map_old_chunk_to_compact_chunk = new int[m_num_chunks];
			int compact_chunk_count = 0;
			for (int chunknum = 0; chunknum < m_num_chunks; ++chunknum) {
				if (m_chunk_size[chunknum] == 0) {
					map_old_chunk_to_compact_chunk[chunknum] = -1;
				} else {
					map_old_chunk_to_compact_chunk[chunknum] = compact_chunk_count++;
				}
			}

			foreach (int segnum in level.EnumerateAliveSegmentIndices()) {
				Segment seg = level.segment[segnum];
				System.Diagnostics.Debug.Assert(seg.m_chunk_num >= 0 && seg.m_chunk_num < m_num_chunks);

				int compact_chunk_idx = map_old_chunk_to_compact_chunk[seg.m_chunk_num];
				System.Diagnostics.Debug.Assert(compact_chunk_idx != -1);

				seg.m_chunk_num = compact_chunk_idx;
			}

			level.m_num_chunks = compact_chunk_count;

#if OVERLOAD_LEVEL_EDITOR
			level.editor.AddOutputText("Num segments = " + num_segments + ", num chunks = " + level.m_num_chunks);

			//Generate colors for chunks
			for (int c = 0; c < m_num_chunks; c++) {
				level.ChunkColor[c] = System.Drawing.Color.FromArgb(255, Utility.RandomRange(0, 127), Utility.RandomRange(0, 127), Utility.RandomRange(0, 127));
			}
#endif  //OVERLOAD_LEVEL_EDITOR

		}
	}
}