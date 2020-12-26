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

using OpenTK;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace OverloadLevelEditor
{
	public partial class DMesh
	{
		public void CreateQuad(Vector2 sz)
		{
			// Create 4 verts
			int v0 = vert_info.Count;
			Create4Verts(sz, 0f);

			// Create a quad from those 4 verts
			CreateQuadFromVerts(v0, v0 + 1, v0 + 2, v0 + 3);
		}

		public void CreateQuadFromVerts(int v1, int v2, int v3, int v4, bool marked = true, bool tagged = false)
		{
			List<int> v_idx = new List<int>();
			v_idx.Add(v1);
			v_idx.Add(v2);
			v_idx.Add(v3);
			v_idx.Add(v4);
			DPoly poly = new DPoly(v_idx, -1, this);
			poly.tag = tagged;
			if (marked) {
				poly.marked = true;
				num_marked_polys += 1;
			}
			polygon.Add(poly);
		}

		public void CreateTriFromVerts(int v1, int v2, int v3, bool marked = true)
		{
			List<int> v_idx = new List<int>();
			v_idx.Add(v1);
			v_idx.Add(v2);
			v_idx.Add(v3);
			DPoly poly = new DPoly(v_idx, -1, this);
			if (marked) {
				poly.marked = true;
				num_marked_polys += 1;
			}
			polygon.Add(poly);
		}

		public void CreatePolyFromVerts(int v_start, int v_end)
		{
			List<int> v_idx = new List<int>();
			for (int i = v_start; i < v_end; i++) {
				v_idx.Add(i);
			}
			DPoly poly = new DPoly(v_idx, -1, this);
			poly.marked = true;
			polygon.Add(poly);
			num_marked_polys += 1;
		}

		public void CreateBox(Vector3 sz)
		{
			// Create 8 verts
			int v0 = vert_info.Count;
			Create4Verts(sz.Xz, 0f);
			Create4Verts(sz.Xz, sz.Y);

			// Create 6 quads from those 8 verts
			CreateQuadFromVerts(v0 + 3, v0 + 2, v0 + 1, v0 + 0); // Bottom
			CreateQuadFromVerts(v0 + 4, v0 + 5, v0 + 6, v0 + 7); // Top
			CreateQuadFromVerts(v0 + 7, v0 + 6, v0 + 2, v0 + 3); // 
			CreateQuadFromVerts(v0 + 0, v0 + 1, v0 + 5, v0 + 4); // 
			CreateQuadFromVerts(v0 + 0, v0 + 4, v0 + 7, v0 + 3); // 
			CreateQuadFromVerts(v0 + 2, v0 + 6, v0 + 5, v0 + 1); // 
		}

		public void Create4Verts(Vector2 sz, float y)
		{
			AddVertexEditor(sz.X, y, sz.Y);
			AddVertexEditor(sz.X, y, -sz.Y);
			AddVertexEditor(-sz.X, y, -sz.Y);
			AddVertexEditor(-sz.X, y, sz.Y);			
		}

		public void AddVertexEditor(Vector3 v, bool marked = true)
		{
			vertex.Add(v);
			vert_info.Add(new DVert(marked));
			if (marked) {
				num_marked_verts += 1;
			}
		}

		public void AddVertexEditor(float x, float y, float z)
		{
			AddVertexEditor(new Vector3(x, y, z));
		}

		public void CreateCylinder(Axis axis, float radius, float height, int segments)
		{
			// Create two rings of verts
			int v0 = vert_info.Count;
			Vector3 pos = Vector3.Zero;
			for (int i = 0; i < segments; i++) {
				pos.Xz = Utility.Vector2Rotate(Vector2.UnitX * radius, Utility.RAD_360 * (float)i / (float)segments);
				AddVertexEditor(pos.X, pos.Y, pos.Z);
			}

			pos.Y = height;
			for (int i = 0; i < segments; i++) {
				pos.Xz = Utility.Vector2Rotate(Vector2.UnitX * radius, Utility.RAD_360 * (float)i / (float)segments);
				AddVertexEditor(pos.X, pos.Y, pos.Z);
			}
			
			// If Axis is not Y, rotate the verts along the X/Z
			switch (axis) {
				case Axis.X:
					for (int i = v0; i < v0 + segments * 2; i++) {
						vertex[i] = Vector3.Transform(vertex[i], Matrix4.CreateRotationZ(Utility.RAD_90));
					}
					break;
				case Axis.Z:
					for (int i = v0; i < v0 + segments * 2; i++) {
						vertex[i] = Vector3.Transform(vertex[i], Matrix4.CreateRotationX(Utility.RAD_90));
					}
					break;
			}

			// Create polygons for top/bottom rings and flip the one
			CreatePolyFromVerts(v0, v0 + segments);
			CreatePolyFromVerts(v0 + segments, v0 + segments * 2);
			polygon[polygon.Count - 1].ReverseWindingOrder();
			polygon[polygon.Count - 1].RecalculateNormal(this);

			// Create quad for first/last verts
			// Create quads for other verts
			CreateQuadFromVerts(v0 + segments * 2 - 1, v0 + segments, v0, v0 + segments - 1);
			for (int i = v0; i < v0 + segments - 1; i++) {
				CreateQuadFromVerts(i + segments, i + segments + 1, i + 1, i);
			}
		}

		public void AddPolyFromMarkedVerts()
		{
			UpdateMarkedCounts();
			if (num_marked_verts > 2 && num_marked_verts <= DPoly.MAX_VERTS) {
				List<int> m_verts = GetMarkedVerts();
				Vector3 normal = NormalOfVertListPolys(m_verts);
				DPoly dp = new DPoly(m_verts, -1, this, true);
				dp.marked = true;
				num_marked_polys += 1;

				int idx = polygon.Count;
				polygon.Add(dp);
				polygon[idx].RecalculateNormal(this);
				if (Vector3.Dot(polygon[idx].face_normal, normal) < 0f) {
					polygon[idx].ReverseWindingOrder();
				}
			}
		}

		public void AddPolyFromVertsList(List<int> vert_list, bool marked = false)
		{
			Vector3 normal = NormalOfVertListPolys(vert_list);
			DPoly dp = new DPoly(vert_list, -1, this, true);
			if (marked) {
				dp.marked = true;
				num_marked_polys += 1;
			} else {
				dp.marked = false;
			}

			int idx = polygon.Count;
			polygon.Add(dp);
			polygon[idx].RecalculateNormal(this);
			if (Vector3.Dot(polygon[idx].face_normal, normal) < 0f) {
				polygon[idx].ReverseWindingOrder();
			}
		}

		public Vector3 NormalOfVertListPolys(List<int> vert_list)
		{
			Vector3 normal = Vector3.Zero;
			int count = 0;
			foreach (DPoly dp in polygon) {
				if (dp.HasVertsInListCount(vert_list) > 0) {
					normal += dp.face_normal;
					count += 1;
				}
			}

			if (count > 0) {
				normal.Normalize();
			}
			return normal;
		}

		public void ExtrudeSelectedPoly(float dist)
		{
			if (selected_poly > -1 && selected_poly < polygon.Count) {
				ClearAllMarkedPoly();
				DPoly dp = polygon[selected_poly];
				dp.marked = true;
				// EXTRUDE!
				Vector3 extrude_vec = DPoly.CalculatePolyNormal(dp, this) * dist;

				// Add extra verts and assign polygon verts
				int first_vert = vertex.Count;
				for (int i = 0; i < dp.num_verts; i++) {
					AddVertexEditor(vertex[dp.vert[i]] + extrude_vec);
				}

				// Create new polygons around the rim
				List<int> v_list = new List<int>();
				for (int i = 0; i < dp.num_verts; i++) {
					v_list.Clear();
					v_list.Add(dp.vert[i]);
					v_list.Add(dp.vert[(i + 1) % dp.num_verts]);
					v_list.Add(first_vert + (i + 1) % dp.num_verts);
					v_list.Add(first_vert + i);
					DPoly new_poly = new DPoly(v_list, dp.tex_index, this);
					polygon.Add(new_poly);
				}

				// Assign original polygon verts
				for (int i = 0; i < dp.num_verts; i++) {
					dp.vert[i] = first_vert + i;
				}
			} else {
				editor.AddOutputText("No polygon selected for extrude");
			}
		}

		public void ExtrudeMarkedPolys(float dist)
		{
			if (selected_poly > -1 && selected_poly < polygon.Count) {
				DPoly selected_dp = polygon[selected_poly];
				if (selected_dp.marked) {
					// EXTRUDE!
					Vector3 extrude_vec = DPoly.CalculatePolyNormal(selected_dp, this) * dist;

					// Extrude all the marked polygons
					TagAllPolys(false);
					foreach (DPoly dp in GetMarkedPolys(false)) {
						// Add extra verts and assign polygon verts
						int first_vert = vertex.Count;
						for (int i = 0; i < dp.num_verts; i++) {
							AddVertexEditor(vertex[dp.vert[i]] + extrude_vec);
						}

						// Create new polygons around the rim
						List<int> v_list = new List<int>();
						for (int i = 0; i < dp.num_verts; i++) {
							v_list.Clear();
							v_list.Add(dp.vert[i]);
							v_list.Add(dp.vert[(i + 1) % dp.num_verts]);
							v_list.Add(first_vert + (i + 1) % dp.num_verts);
							v_list.Add(first_vert + i);
							DPoly new_poly = new DPoly(v_list, dp.tex_index, this);
							new_poly.tag = true;
							polygon.Add(new_poly);
						}

						// Assign original polygon verts
						for (int i = 0; i < dp.num_verts; i++) {
							dp.vert[i] = first_vert + i;
						}
					}

					// Merge overlapping verts
					MergeOverlappingVerts();

					// Remove all tagged polygons that share all verts
					List<DPoly> polys_to_remove = new List<DPoly>();
					foreach (DPoly dp1 in polygon) {
						if (dp1.tag) {
							dp1.tag = false; 
							foreach (DPoly dp2 in polygon) {
								if (dp2.tag) {
									if (dp1.SharesAllVerts(dp2)) {
										polys_to_remove.Add(dp1);
										polys_to_remove.Add(dp2);
										dp2.tag = false;
									}
								}
							}
						}
					}

					foreach (DPoly dp in polys_to_remove) {
						polygon.Remove(dp);
					}
				} else {
					editor.AddOutputText("Selected polygon must be marked for multi-extrude");
				}
			} else {
				editor.AddOutputText("Must select polygon for multi-extrude");
			}
		}

		public void InsetExtrudeMarkedPolys(float inset_amt, float dist)
		{
			if (selected_poly > -1 && selected_poly < polygon.Count) {
				DPoly selected_dp = polygon[selected_poly];
				if (selected_dp.marked) {
					// EXTRUDE!
					Vector3 extrude_vec = DPoly.CalculatePolyNormal(selected_dp, this) * dist;

					Vector3 center_vec = AverageMarkedPolyVertPosition();

					// Extrude all the marked polygons
					TagAllPolys(false);
					foreach (DPoly dp in GetMarkedPolys(false)) {
						// Add extra verts and assign polygon verts
						int first_vert = vertex.Count;
						for (int i = 0; i < dp.num_verts; i++) {
							AddVertexEditor(vertex[dp.vert[i]] * (1f - inset_amt) + center_vec * inset_amt + extrude_vec);
						}

						// Create new polygons around the rim
						List<int> v_list = new List<int>();
						for (int i = 0; i < dp.num_verts; i++) {
							v_list.Clear();
							v_list.Add(dp.vert[i]);
							v_list.Add(dp.vert[(i + 1) % dp.num_verts]);
							v_list.Add(first_vert + (i + 1) % dp.num_verts);
							v_list.Add(first_vert + i);
							DPoly new_poly = new DPoly(v_list, dp.tex_index, this);
							new_poly.tag = true;
							polygon.Add(new_poly);
						}

						// Assign original polygon verts
						for (int i = 0; i < dp.num_verts; i++) {
							dp.vert[i] = first_vert + i;
						}
					}

					// Merge overlapping verts
					MergeOverlappingVerts();

					// Remove all tagged polygons that share all verts
					List<DPoly> polys_to_remove = new List<DPoly>();
					foreach (DPoly dp1 in polygon) {
						if (dp1.tag) {
							dp1.tag = false;
							foreach (DPoly dp2 in polygon) {
								if (dp2.tag) {
									if (dp1.SharesAllVerts(dp2)) {
										polys_to_remove.Add(dp1);
										polys_to_remove.Add(dp2);
										dp2.tag = false;
									}
								}
							}
						}
					}

					foreach (DPoly dp in polys_to_remove) {
						polygon.Remove(dp);
					}
				} else {
					editor.AddOutputText("Selected polygon must be marked for inset/bevel");
				}
			} else {
				editor.AddOutputText("Must select polygon for inset/bevel");
			}
		}

		public void SplitEdge()
		{
			List<int> verts = GetMarkedVerts();
			if (verts.Count > 1) {
				for (int i = 0; i < verts.Count; i++) {
					for (int j = i + 1; j < verts.Count - 0; j++) {
						// Reset the added vert (only allow 1 per pair of verts)
						DPoly.AddedVert = -1;

						// Go through each polygon and see if both verts are in it
						foreach (DPoly dp in polygon) {
							if (dp.HasBothVerts(verts[i], verts[j])) {
								// Add a vert between them if consecutive
								dp.MaybeAddVertBetween(verts[i], verts[j], this);
								editor.AddOutputText("Add vert on poly at verts" + verts[i] + "-" + verts[j]);
							}
						}
					}
				}
			} else {
				editor.AddOutputText("Must mark more than 1 vert for splitting an edge");
			}
		}

		public struct BevelVerts
		{
			public bool used;
			public List<int> bevel_verts;
			public List<Vector3> bevel_push;
		}

		// 180 (-1) 1 (sqrt 1) 1.000
		// 120 (-0.5) 1.151 (sqrt 1.325) .868
		// 90  (0) 1.414 (sqrt 2) .707
		// 60	 (0.5) 1.981 (sqrt 4) .505
		// 0	 (1) 00		.000
		// 1 / (cos ((1 + dot) * 0.25 pi));
		// 1 / sqrt ((1 - dot) / 2)
		
		public void BevelEdgeVerts()
		{
			List<int> verts = GetMarkedVerts();
			if (verts.Count >= 1) {
				ClearAllMarkedPoly();

				// Keep track of all the edge pairs (including edges with one marked vert and one not) for later stuff
				List<int> edge_v1 = new List<int>();
				List<int> edge_v2 = new List<int>();
				BevelVerts[] bevel_output = new BevelVerts[vertex.Count];
				Vector3[] vert_normal = new Vector3[vertex.Count];

				int count = vertex.Count;
				for (int i = 0; i < count; i++) {
					bevel_output[i].used = false;
					if (vert_info[i].marked) {
						for (int j = 0; j < count; j++) {
							if (vert_info[j].marked) {
								if (i < j) {
									// Both marked, with i < j
									if (SharesAnEdge(i, j)) {
										edge_v1.Add(i);
										edge_v2.Add(j);
									}
								}
							} else {
								// One marked, one not (has to share two edges or we just ignore it)
								if (SharesTwoEdges(i, j)) {
									edge_v1.Add(i);
									edge_v2.Add(j);
								}
							}
						}
					}
				}

				// For each vert:
				count = verts.Count;
				for (int i = 0; i < count; i++) {
					bevel_output[verts[i]].used = true;
					bevel_output[verts[i]].bevel_verts = new List<int>();
					bevel_output[verts[i]].bevel_push = new List<Vector3>();
					foreach (DPoly dp in polygon) {
						// - Add an extra vert for each polygon it touches
						if (dp.HasAVert(verts[i])) {
							// Create a new vert
							Vector3 pos = vertex[verts[i]];

							int idx = vertex.Count;
							AddVertexEditor(vertex[verts[i]], false);
							bevel_output[verts[i]].bevel_verts.Add(idx);

							// Move it towards the center of the polygon
							Vector3 diff = (dp.ClosestTwoVertsDirection(pos, verts[i], this));
							float dot = (dp.ClosestTwoVertsDot(pos, verts[i], this));
							float multiplier = (float)System.Math.Sqrt((1f - dot) * 0.5f);
							//float multiplier = (float)System.Math.Cos((1f + dot) * 0.25f * (float)System.Math.PI);
							if (multiplier > 0.1f) {
								multiplier = editor.m_bevel_width / multiplier;
							} else {
								multiplier = editor.m_bevel_width * 10f;
							}
							diff = diff * multiplier;
							bevel_output[verts[i]].bevel_push.Add(diff);
							dp.ReplaceVert(verts[i], idx);
						}
					}
					
				}

				// Adjust the new vert positions by our previous calculations (delayed so pushing other verts doesn't affect the calculations)
				for (int i = 0; i < bevel_output.Length; i++) {
					if (bevel_output[i].used) {
						for (int j = 0; j < bevel_output[i].bevel_verts.Count; j++) {
							vertex[bevel_output[i].bevel_verts[j]] += bevel_output[i].bevel_push[j];
						}

						// - Add a new polygon connecting all the new verts (if 3 or more verts)
						if (bevel_output[i].bevel_verts.Count > 2) {
							AddPolyFromVertsList(bevel_output[i].bevel_verts, true);
						}
					}			
				}

				// Then for each edge:
				// - Add a polygon
				for (int i = 0; i < edge_v1.Count; i++) {
					if (bevel_output[edge_v1[i]].used && bevel_output[edge_v1[i]].bevel_verts.Count > 1) {
						Vector3 center = (vertex[edge_v1[i]] + vertex[edge_v2[i]]) * 0.5f;
						List<int> edge_poly_verts = new List<int>();

						// Find the closest two verts of each group of verts
						List<int> closest_two;
						closest_two = GetClosest2VertsFromList(center, bevel_output[edge_v1[i]].bevel_verts);
						edge_poly_verts.Add(closest_two[0]);
						edge_poly_verts.Add(closest_two[1]);

						if (bevel_output[edge_v2[i]].used) {
							closest_two.Clear();
							closest_two = GetClosest2VertsFromList(center, bevel_output[edge_v2[i]].bevel_verts);
							edge_poly_verts.Add(closest_two[0]);
							edge_poly_verts.Add(closest_two[1]);
						} else {
							// Add just the original vert
							edge_poly_verts.Add(edge_v2[i]);
						}

						AddPolyFromVertsList(edge_poly_verts, true);
					}
				}

				DeleteMarked(EditMode.VERT);
			}
		}

		public List<int> GetClosest2VertsFromList(Vector3 pos, List<int> bevel_verts)
		{
			List<int> closest_two = new List<int>();

			int close1_idx = -1;
			int close2_idx = -1;
			float close1_dist = 99999f;
			float close2_dist = 99999f;
			float dist;

			for (int i = 0; i < bevel_verts.Count; i++) {
				dist = (vertex[bevel_verts[i]] - pos).Length;
				if (dist < close1_dist) {
					close2_dist = close1_dist;
					close2_idx = close1_idx;
					close1_dist = dist;
					close1_idx = i;
				} else if (dist < close2_dist) {
					close2_dist = dist;
					close2_idx = i;
				}
			}

			closest_two.Add(bevel_verts[close1_idx]);
			closest_two.Add(bevel_verts[close2_idx]);

			return closest_two;
		}

		public bool SharesAnEdge(int v1, int v2)
		{
			foreach (DPoly dp in polygon) {
				if (dp.HasBothVertsAdjacent(v1, v2)) {
					return true;
				}
			}

			return false;
		}

		public bool SharesTwoEdges(int v1, int v2)
		{
			int count = 0;
			foreach (DPoly dp in polygon) {
				if (dp.HasBothVertsAdjacent(v1, v2)) {
					count += 1;
				}
			}

			return (count == 2);
		}

		// The old beveling algorithm (buggy, not great)
		public void BevelEdge()
		{
			List<int> verts = GetMarkedVerts();
			if (verts.Count > 1) {
				ClearAllMarkedPoly();

				// 1. Find the average normal of all polygons containing the marked verts (that's our bevel normal)
				// 1a. Also find the skew direction (cross product of normal and avg difference)
				Vector3 avg_normal = Vector3.Zero;
				Vector3 avg_diff = Vector3.Zero;
				int count = 0;
				foreach (DPoly dp in polygon) {
					for (int i = 0; i < verts.Count; i++) {
						if (dp.HasAVert(verts[i])) {
							avg_normal += dp.face_normal;
							count += 1;
						}
					}
				}
				for (int i = 1; i < verts.Count; i++) {
					avg_diff += (vertex[verts[i]] - vertex[verts[i - 1]]).Normalized();
				}

				if (count > 0) {
					avg_normal /= count;
					avg_diff /= (verts.Count - 1);
				} else {
					editor.AddOutputText("Verts didn't touch enough polygons");
					return;
				}

				Vector3 skew_dir = Vector3.Cross(avg_normal, avg_diff).Normalized();

				// 2. For each vert, add a new vert at the same location, and push them away from each other a small amount perpendicular to normal (use bevel amt)
				List<int> new_verts = new List<int>();
				for (int i = 0; i < verts.Count; i++) {
					new_verts.Add(vertex.Count);
					AddVertexEditor(vertex[verts[i]]);

					// Separate the verts
					vertex[new_verts[i]] += skew_dir * (0.5f * editor.m_inset_length);
					vertex[verts[i]] += skew_dir * (-0.5f * editor.m_inset_length);
				}

				TagAllPolys(false);
				count = polygon.Count;

				// Mark all polygons with only 1 vert in the original list
				for (int p = 0; p < count; p++) {
					DPoly dp = polygon[p];
					if (dp.HasVertsInListCount(verts) == 1) {
						dp.tag = true;
					}
				}

				// 3. For each polygon with two verts, choose the closest of each pair and connect to those (half will have new verts, half won't)
				// 4. For each polygon with 1 vert, do the same thing
				// 5. If switching, create the new polygons and marked the verts as used (each can be used twice
				for (int p = 0; p < count; p++) {
					DPoly dp = polygon[p];
					int v1 = -1;
					int v2 = -1;
					for (int i = 0; i < verts.Count; i++) {
						for (int j = 0; j < dp.num_verts; j++) {
							if (dp.vert[j] == verts[i]) {
								// Choose the closest of the old vert and the new one
								float dot1 = dp.ClosestTwoVertsDot(vertex[verts[i]], j, this);
								float dot2 = dp.ClosestTwoVertsDot(vertex[new_verts[i]], j, this);
								if (dot1 > dot2) {
									dp.vert[j] = new_verts[i];
									if (v1 > -1) {
										v2 = i;
									} else {
										v1 = i;
									}
								}
							}
						}

						if (v1 > -1) {
							if (v2 > -1) {
								if (!PolyWithVertsAlreadyExists(verts[v1], verts[v2], new_verts[v2], new_verts[v1])) {
									// Create a quad
									int idx = polygon.Count;
									CreateQuadFromVerts(verts[v1], verts[v2], new_verts[v2], new_verts[v1], true);
									polygon[idx].RecalculateNormal(this);
									if (Vector3.Dot(polygon[idx].face_normal, dp.face_normal) < 0f) {
										polygon[idx].ReverseWindingOrder();
									}
								}
							} else if (dp.tag) {
								// Create a triangle (maybe)
								// - Try to find another polygon with just the 1 vert
								// - If it exists, use the other vert that they also share (as long as it's not in either vert list)
								for (int q = 0; q < count; q++) {
									DPoly dp2 = polygon[q];
									if (dp2.tag && p != q && dp2.SharesOneVertPlusOne(dp, verts[v1])) {
										int idx = dp2.FindSharedVertNotThisOne(dp, verts[v1]);
										if (idx > -1 && !verts.Contains(idx) && !new_verts.Contains(idx)) {
											CreateTriFromVerts(verts[v1], idx, new_verts[v1]);
											idx = polygon.Count - 1;
											polygon[idx].RecalculateNormal(this);
											if (Vector3.Dot(polygon[idx].face_normal, dp.face_normal) < 0f) {
												polygon[idx].ReverseWindingOrder();
											}
											break;
										}
									}
								}

							}
						}
					}
				}
			} else {
				editor.AddOutputText("Must mark more than 1 vert for beveling an edge");
			}
		}

		public bool PolyWithVertsAlreadyExists(int v1, int v2, int v3, int v4)
		{
			List<int> vert_list = new List<int>();
			vert_list.Add(v1);
			vert_list.Add(v2);
			vert_list.Add(v3);
			vert_list.Add(v4);
			foreach (DPoly dp in polygon) {
				if (dp.HasVertsInListCount(vert_list) == 4) {
					return true;
				}
			}

			return false;
		}

		public void CombineMarkedVerts()
		{
			List<int> verts = GetMarkedVerts();
			if (verts.Count > 1) {
				// Move all the marked vertices into the average position
				Vector3 pos = Vector3.Zero;
				for (int i = 0; i < verts.Count; i++) {
					pos += vertex[verts[i]];
				}
				pos /= verts.Count;

				for (int i = 0; i < verts.Count; i++) {
					vertex[verts[i]] = pos;
				}

				// Assign all the polygons combining any of these verts to the first one
				foreach (DPoly dp in polygon) {
					for (int i = 1; i < verts.Count; i++) {
						for (int j = 0; j < dp.num_verts; j++) {
							if (dp.vert[j] == verts[i]) {
								dp.vert[j] = verts[0];
							}
						}
					}
				}

				// Delete all but the first vert
				vert_info[verts[0]].marked = false;
				DeleteMarked(EditMode.VERT);

				// Remove any polygons that have 2 or less verts, and remove extra polyverts
				RemoveExtraPolyVerts();
			} else {
				editor.AddOutputText("Must marked 2 or more verts to combine them");
			}
		}

		public void SplitPolygonWithMarkedVerts()
		{
			List<int> verts = GetMarkedVerts();
			if (verts.Count == 2) {
				int split_count = 0;
				for (int i = 0; i < polygon.Count; i++) {
					if (polygon[i].HasBothVertsNonAdjacent(verts[0], verts[1])) {
						SplitPolygonByVerts(polygon[i], verts[0], verts[1]);
						split_count += 1;
					}
				}

				editor.AddOutputText("Split " + split_count + " polygon(s) with the marked verts");
			} else {
				editor.AddOutputText("Must have 2 marked verts non-adjacent");
			}
		}

		public void SplitPolygonByVerts(DPoly dp, int verts0, int verts1)
		{
			// Get the two sets of verts (those after v0, those after v1)
			List<int> verts_after0 = dp.GetVertsStartUpTo(verts0, verts1);
			List<int> verts_after1 = dp.GetVertsStartUpTo(verts1, verts0);

			// Then make a new polygon copy
			int new_poly_idx = polygon.Count;
			CopyPolygonSameVerts(dp);

			// Remove unneeded verts from both
			dp.RemoveVertList(verts_after0);
			polygon[new_poly_idx].RemoveVertList(verts_after1);
		}

		public void SplitPolygonByPlane(DPoly dp, Clipping.ClipPlane plane)
		{
			var vertices = dp.vert.Select((vert, index) =>
			{
				var cVertex = new Clipping.CVertex();
				cVertex.Position = vertex[vert];
				cVertex.UV = dp.tex_uv[index];
				cVertex.Normal = dp.normal[index];
				return cVertex;
			});

			if (!Clipping.Clipper.WouldBeClipped(vertices, plane))
			{
				return;
			}
			var split_info = Clipping.Clipper.SplitPolygonByPlane(vertices, plane);

			List<int> back_verts = new List<int>();
			foreach (var vert in split_info.back.Cast<Clipping.CVertex>())
			{
				int vert_idx = MaybeAddVertAtPosition(vert.Position);
				back_verts.Add(vert_idx);
				// New points (i.e. those on the split plane) should be in both the front and back group.
				// So we only look for them in the back group.
				if (!dp.vert.Contains(vert_idx))
				{
					dp.AddVert(vert_idx, vert.Normal, vert.UV);
				}
			}
			dp.ReSortVerts(this);

			List<int> front_verts = new List<int>();
			foreach (var vert in split_info.front)
			{
				int vert_idx = FindVertIndexAtPosition(vert.Position);
				if (vert_idx >= 0)
				{
					front_verts.Add(vert_idx);
				}
				else
				{
					Utility.DebugPopup("Unexpected vertices found in split polygon.");
				}
			}

			// Make a copy of the resulting polygon
			int new_poly_idx = polygon.Count;
			CopyPolygonSameVerts(dp);
			var new_poly = polygon[new_poly_idx];

			// Then remove unwanted verts from each side
			for (int i = dp.num_verts - 1; i >= 0; i--)
			{
				if (!back_verts.Contains(dp.vert[i]))
				{
					dp.RemoveVert(i);
				}
			}
			for (int i = new_poly.num_verts - 1; i >= 0; i--)
			{
				if (!front_verts.Contains(new_poly.vert[i]))
				{
					new_poly.RemoveVert(i);
				}
			}
		}

		public void SubdividMarkedPolys()
		{
			List<DPoly> poly_list = GetMarkedPolys();
			if (poly_list.Count > 0) {
				TagAllPolys(false);
				int count = poly_list.Count;
				foreach (DPoly dp in poly_list) {
					Vector3 center = dp.FindCenter(this);
					int center_vert = vertex.Count;
					// Add extra verts to every marked polygon (at the center)
					AddVertexEditor(center, false);

					List<int> edge_verts = new List<int>();
					// Add a vert to every edge (check if there's a vert there first, and if so don't add it)
					for (int i = 0; i < dp.num_verts; i++) {
						Vector3 pos = (vertex[dp.vert[i]] + vertex[dp.vert[(i + 1) % dp.num_verts]]) * 0.5f;
						int vert_idx = MaybeAddVertAtPosition(pos);
						edge_verts.Add(vert_idx);
					}

					// - Add N quads where N = number of verts for polygon, tag them
					for (int i = 0; i < dp.num_verts; i++) {
						CreateQuadFromVerts(dp.vert[i], edge_verts[i], center_vert, edge_verts[(i + dp.num_verts - 1) % dp.num_verts], false, true);
					}
				}

				// - Delete the marked polygons
				DeleteMarked(EditMode.POLY);

				// - Mark the tagged polygons
				MarkTaggedPolys();

				// MAYBE/LATER: Reconnect any un-marked polygons to the new verts (if they share an edge)
			} 
		}

		// Returns the index of the vert
		public int MaybeAddVertAtPosition(Vector3 pos)
		{
			for (int i = 0; i < vertex.Count; i++) {
				if (Utility.AlmostOverlapping(pos, vertex[i])) {
					return i;
				}
			}

			// No matches
			AddVertexEditor(pos, false);
			return vertex.Count - 1;
		}

		public int FindVertIndexAtPosition(Vector3 pos)
		{
			for (int i = 0; i < vertex.Count; i++)
			{
				if (Utility.AlmostOverlapping(pos, vertex[i]))
				{
					return i;
				}
			}
			return -1;
		}

		public void GetVertBounds(List<int> verts, out Vector3 min, out Vector3 max)
		{
			min = vertex[verts[0]];
			max = vertex[verts[0]];
			foreach (int idx in verts) {
				min = Utility.V3Min(min, vertex[idx]);
				max = Utility.V3Max(max, vertex[idx]);
			}
		}

		public void ExtrudeMarkedVerts(float dist)
		{
			List<int> verts = GetMarkedVerts();
			if (verts.Count > 1) {
				ClearAllMarkedPoly();

				// Figure out which direction to go (lesser of X/Y/Z)
				Vector3 min, max;
				GetVertBounds(verts, out min, out max);
				Vector3 diff = max - min;
				Vector3 extrude_vec = (max.Y < 0f ? -Vector3.UnitY :  Vector3.UnitY);
				if (diff.X < diff.Y && diff.X < diff.Z) {
					extrude_vec = (max.X < 0f ? -Vector3.UnitX : Vector3.UnitX);
				} else if (diff.Z < diff.Y && diff.Z < diff.X) {
					extrude_vec = (max.X < 0f ? -Vector3.UnitZ : Vector3.UnitZ);
				}
				extrude_vec *= dist;

				int first_vert = vertex.Count;
				// Create the new verts in the extrude direction
				foreach (int idx in verts) {
					AddVertexEditor(vertex[idx] + extrude_vec);
				}

				// Create a new polygon for each vert except the last
				List<int> v_list = new List<int>();
				for (int i = 0; i < verts.Count - 1; i++) {
					v_list.Clear();
					v_list.Add(verts[i]);
					v_list.Add(verts[(i + 1)]);
					v_list.Add(first_vert + (i + 1));
					v_list.Add(first_vert + i);
					DPoly new_poly = new DPoly(v_list, -1, this);
					new_poly.marked = true;
					polygon.Add(new_poly);
				}

				// Unmarked the old verts and mark the new ones
				for (int i = 0; i < verts.Count; i++) {
					vert_info[verts[i]].marked = false;
					vert_info[first_vert + i].marked = true;
				}
			} else {
				editor.AddOutputText("Must mark at least two verts to extrude");
			}
		}

		public void CombineTwoPolys()
		{
			UpdateMarkedCounts();
			if (num_marked_polys == 2) {
				List<DPoly> polys = GetMarkedPolys();

				if (DPoly.HasTwoOrMoreSharedVerts(polys[0], polys[1])) {
					if (Vector3.Dot(DPoly.CalculatePolyNormal(polys[0], this), DPoly.CalculatePolyNormal(polys[1], this)) > 0.8f) {
						// Align the UVs
						polys[1].UVAlignToPoly(polys[0], this);

						// Add verts from c_polys[1] to c_polys[0]
						for (int i = 0; i < polys[1].num_verts; i++) {
							if (!polys[0].vert.Contains(polys[1].vert[i])) {
								polys[0].AddVert(polys[1].vert[i], polys[1].normal[i], polys[1].tex_uv[i]);
							}
						}

						// Sort the verts in c_polys[0]
						polys[0].ReSortVerts(this);

						// Delete c_polys[1]
						polygon.Remove(polys[1]);
					} else {
						editor.AddOutputText("Polys aren't planar enough, cannot combine");
					}
				} else {
					editor.AddOutputText("Polys don't share two verts, cannot combine");
				}
			} else {
				editor.AddOutputText("Must mark exactly two polys to combine");
			}
		}

		public void PolysTriangulateFan(bool mark_them = false)
		{
			List<DPoly> polys = GetMarkedPolys();
			
			for (int i = 0; i < polys.Count; i++) {
				// Add the center as a vert
				int center_vert = vertex.Count;
				Vector3 center_pos = polys[i].FindCenter(this);
				Vector2 center_uv = polys[i].FindUVCenter();
				AddVertexEditor(center_pos, false);

				// Create a new triangle for each vert
				List<int> vrts = new List<int>();
				for (int j = 0; j < polys[i].vert.Count; j++) {
					vrts.Clear();
					vrts.Add(center_vert);
					vrts.Add(polys[i].vert[j]);
					vrts.Add(polys[i].vert[(j + 1) % polys[i].vert.Count]);
					DPoly dp = new DPoly(vrts, polys[i].tex_index, this, false);
					dp.tex_uv[0] = center_uv;
					dp.tex_uv[1] = polys[i].tex_uv[j];
					dp.tex_uv[2] = polys[i].tex_uv[(j + 1) % polys[i].vert.Count];
					dp.marked = mark_them;
					polygon.Add(dp);
				}
			}

			// Remove the old polys
			for (int i = 0; i < polys.Count; i++) {
				polygon.Remove(polys[i]);
			}
		}

		public void PolysTriangulateVert(bool mark_them = false)
		{
			List<DPoly> polys = GetMarkedPolys();

			for (int i = 0; i < polys.Count; i++) {
				// Create a new triangle for each vert - 2
				List<int> vrts = new List<int>();
				for (int j = 1; j < polys[i].vert.Count - 1; j++) {
					vrts.Clear();
					vrts.Add(polys[i].vert[0]);
					vrts.Add(polys[i].vert[j]);
					vrts.Add(polys[i].vert[j + 1]);
					DPoly dp = new DPoly(vrts, polys[i].tex_index, this, false);
					dp.tex_uv[0] = polys[i].tex_uv[0];
					dp.tex_uv[1] = polys[i].tex_uv[j];
					dp.tex_uv[2] = polys[i].tex_uv[j + 1];
					dp.marked = mark_them;
					polygon.Add(dp);
				}
			}

			// Remove the old polys
			for (int i = 0; i < polys.Count; i++) {
				polygon.Remove(polys[i]);
			}
		}

		public void PolysTriangulateNonPlanar()
		{
			// This is kind of a cheat - it just unmarks planar polygons, and triangulates the rest
			List<DPoly> poly_list = GetMarkedPolys(false);

			for (int i = 0; i < poly_list.Count; i++) {
				if (poly_list[i].IsPlanar(this)) {
					poly_list[i].marked = false;
				}
			}

			poly_list = GetMarkedPolys(false);
			if (poly_list.Count > 0) {
				PolysTriangulateVert(true);
			}
		}

		public void SwitchTriEdge()
		{
			List<DPoly> polys = GetMarkedPolys();
			if (polys.Count == 2 && polys[0].num_verts == 3 && polys[1].num_verts == 3) {
				if (DPoly.HasTwoOrMoreSharedVerts(polys[0], polys[1])) {
					List<int> vrts = new List<int>();

					// Save off an unshared vert index
					int unshared_vert = polys[0].vert[0];
					for (int i = 0; i < 3; i++) {
						if (polys[1].vert.Contains(polys[0].vert[i])) {
							unshared_vert = polys[0].vert[i];
						}
					}

					// Combine the two polygons
					CombineTwoPolys();

					// Shift the vert order by 1 (maybe) to ensure that when you triangulate the polygon, it is split the other way)
					polys = GetMarkedPolys();
					polys[0].MaybeShiftVerts(unshared_vert);

					// Triangulate and mark the polygons
					PolysTriangulateVert(true);
				} else {
					editor.AddOutputText("Polys don't share two verts, cannot switch edge");
				}
			} else {
				editor.AddOutputText("Must mark two triangles");
			}
		}
	}
}