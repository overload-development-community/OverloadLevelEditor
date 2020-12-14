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
using OpenTK;
using Newtonsoft.Json.Linq;

namespace OverloadLevelEditor
{
	public partial class Side
	{
		public const int NUM_VERTS = 4;
		public const int NUM_EDGES = 4;

		public int[] vert = new int[NUM_VERTS];

		public Vector2[] uv = new Vector2[NUM_VERTS];
		public Vector2[] uv2 = new Vector2[NUM_VERTS];
		public Vector2[] uv3 = new Vector2[NUM_VERTS];

		public string tex_name;
		public float deformation_height;
		public int deformation_preset = 0;

		// Lava flag (for setting alpha of edges)
		public bool is_lava = false;
		public Edge[] edge = new Edge[NUM_EDGES];

		public const int NUM_DECALS = 2;

		public Decal[] decal = new Decal[NUM_DECALS];

		public int Door;        // MK, 5/4/2016.  Sides know if they have a door attached.

		public Segment segment; // My parent segment
		public Level level { get { return segment.level; } }
		public int num;
		public bool marked;
		public int chunk_plane_order;       //if -1, this isn't a splitting plane.  Else it determines priority.

		Vector2[] uv_default = { new Vector2(0f, 0f),
										 new Vector2(0f, 1f),
										 new Vector2(1f, 1f),
										 new Vector2(1f, 0f)
									  };

		public Side(Segment seg, int n)
		{
			segment = seg;
			num = n;

			Init();
		}

		public string DEFAULT_TEX_NAME = "om_floor_03c";

		public void Init()
		{
			marked = false;
			chunk_plane_order = -1;
			tex_name = DEFAULT_TEX_NAME;
#if OVERLOAD_LEVEL_EDITOR
			m_tag = false;
			m_tex_gl_id = (Editor.EditorLoaded && level != null && level.editor.tm_level != null ? level.editor.tm_level.FindTextureIDByName(tex_name) : -1);
#endif
			deformation_preset = 0;
			deformation_height = 0f;
			Door = -1;

			for (int i = 0; i < NUM_VERTS; i++) {
				vert[i] = -1;
				uv[i] = uv_default[i];
				uv2[i] = new Vector2(0.0f, 0.0f);
				uv3[i] = new Vector2(0.0f, 0.0f);
			}

			for (int i = 0; i < NUM_DECALS; i++) {
				decal[i] = new Decal(this, i);
			}

			for (int i = 0; i < NUM_EDGES; i++) {
				edge[i] = new Edge();
			}
		}

		public void Copy(Side src, bool full)
		{
			marked = (full ? src.marked : false);
			chunk_plane_order = src.chunk_plane_order;

#if OVERLOAD_LEVEL_EDITOR
			m_tex_gl_id = src.m_tex_gl_id;
#endif
			tex_name = src.tex_name;
			deformation_preset = src.deformation_preset;
			deformation_height = src.deformation_height;
			Door = src.Door;

			for (int i = 0; i < NUM_VERTS; i++) {
				vert[i] = src.vert[i];
				uv[i] = src.uv[i];
				uv2[i] = src.uv2[i];
				uv3[i] = src.uv3[i];
			}

			for (int i = 0; i < NUM_DECALS; i++) {
				decal[i].Copy(src.decal[i]);
			}
		}

		public void Serialize(JObject root)
		{
			root["marked"] = this.marked;
			root["chunk_plane_order"] = this.chunk_plane_order;
			root["tex_name"] = this.tex_name;
			root["deformation_preset"] = this.deformation_preset;
			root["deformation_height"] = this.deformation_height;

			bool has_uv2 = false;
			bool has_uv3 = false;
			for (int i = 0; i < NUM_VERTS && (has_uv2 == false || has_uv3 == false); ++i) {
				if (this.uv2[i].X != 0.0f || this.uv2[i].Y != 0.0f) {
					has_uv2 = true;
				}
				if (this.uv3[i].X != 0.0f || this.uv3[i].Y != 0.0f) {
					has_uv3 = true;
				}
			}

			var jVerts = new JArray();
			var jUVs = new JArray();
			for (int i = 0; i < NUM_VERTS; ++i) {
				jVerts.Add(this.vert[i]);

				var jUV = new JObject();
				jUV["u"] = this.uv[i].X;
				jUV["v"] = this.uv[i].Y;

				if (has_uv2) {
					jUV["u2"] = this.uv2[i].X;
					jUV["v2"] = this.uv2[i].Y;
				}

				if (has_uv3) {
					jUV["u3"] = this.uv3[i].X;
					jUV["v3"] = this.uv3[i].Y;
				}

				jUVs.Add(jUV);
			}
			root["verts"] = jVerts;
			root["uvs"] = jUVs;

			var jDecals = new JArray();
			for (int i = 0; i < NUM_DECALS; i++) {
				var jDecal = new JObject();
				this.decal[i].Serialize(jDecal);
				jDecals.Add(jDecal);
			}
			root["decals"] = jDecals;

			root["door"] = this.Door;
		}

		public void Deserialize(JObject root)
		{
#if OVERLOAD_LEVEL_EDITOR
			this.m_tag = false;
			this.m_tex_gl_id = -1;
#endif
			this.marked = root["marked"].GetBool(false);
			this.chunk_plane_order = root["chunk_plane_order"].GetInt(-1);
			if (root["marked_for_chunking"].GetBool(false)) {           //Handle old levels with bool.  Set order to zero.
				this.chunk_plane_order = 0;
			}
			this.tex_name = root["tex_name"].GetString(string.Empty);
			this.deformation_preset = root["deformation_preset"].GetInt(0);
			this.deformation_height = root["deformation_height"].GetFloat(0f);
			if (this.tex_name.ToLower().Contains("lava")) {
				is_lava = true;
				deformation_height = Math.Max(0.01f, deformation_height);
			}

			var jVerts = root["verts"].GetArray();
			var jUVs = root["uvs"].GetArray();
			for (int i = 0; i < NUM_VERTS; ++i) {
				this.vert[i] = jVerts[i].GetInt(0);
				this.uv[i].X = jUVs[i]["u"].GetFloat(0.0f);
				this.uv[i].Y = jUVs[i]["v"].GetFloat(0.0f);
				this.uv2[i].X = jUVs[i]["u2"].GetFloat(0.0f);
				this.uv2[i].Y = jUVs[i]["v2"].GetFloat(0.0f);
				this.uv3[i].X = jUVs[i]["u3"].GetFloat(0.0f);
				this.uv3[i].Y = jUVs[i]["v3"].GetFloat(0.0f);
			}

			var jDecals = root["decals"].GetArray();
			for (int i = 0; i < NUM_DECALS; i++) {
				this.decal[i].Deserialize(jDecals[i].GetObject());
			}

			this.Door = root["door"].GetInt(-1);
		}

		public void DeserializeComplete()
		{
			for (int i = 0; i < NUM_DECALS; i++) {
				this.decal[i].DeserializeComplete();
			}
		}

		public bool HasNeighbor { get { return (this.segment.neighbor[this.num] > -1); } }

		public Vector3 FindNormal()
		{
			return Utility.FindNormal(level.vertex[vert[0]].position, level.vertex[vert[1]].position, level.vertex[vert[2]].position);
		}

#if OVERLOAD_LEVEL_EDITOR
		public Matrix4 FindOrientation()
		{
			Vector3 forward = -Utility.FindNormal(level.vertex[vert[0]].position, level.vertex[vert[1]].position, level.vertex[vert[2]].position);
			Vector3 up = (level.vertex[vert[0]].position - level.vertex[vert[1]].position).Normalized();
			Vector3 right = Vector3.Cross(up, forward);

			return new Matrix4(right.X, right.Y, right.Z, 0.0f, up.X, up.Y, up.Z, 0.0f, forward.X, forward.Y, forward.Z, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f);
		}
#endif

		public Vector3 FindAverageNormal(bool invertWinding = false)
		{
			Vector3[] vertPos = new Vector3[4];
			for (int i = 0; i < 4; ++i) {
				vertPos[i] = level.vertex[vert[i]].position;
			}

			int[] tri0Index = new int[3];
			int[] tri1Index = new int[3];

			if (invertWinding) {
				tri0Index[0] = 0;
				tri0Index[1] = 1;
				tri0Index[2] = 3;
				tri1Index[0] = 1;
				tri1Index[1] = 2;
				tri1Index[2] = 3;
			} else {
				tri0Index[0] = 0;
				tri0Index[1] = 1;
				tri0Index[2] = 2;
				tri1Index[0] = 0;
				tri1Index[1] = 2;
				tri1Index[2] = 3;
			}

			Vector3 tri0Normal = Utility.FindNormal(vertPos[tri0Index[0]], vertPos[tri0Index[1]], vertPos[tri0Index[2]]);
			Vector3 tri1Normal = Utility.FindNormal(vertPos[tri1Index[0]], vertPos[tri1Index[1]], vertPos[tri1Index[2]]);
			Vector3 avgNormal = tri0Normal + tri1Normal;
			return avgNormal.Normalized();
		}

		public Side FindAdjacentSideSameSegment(int edge)
		{
			int v1 = vert[edge];
			int v2 = vert[(edge + 1) % NUM_VERTS];

			int ov1;
			int ov2;
			// Match two verts between this side and its parent segment's sides
			for (int i = 0; i < Segment.NUM_SIDES; i++) {
				if (i != num) {
					for (int j = 0; j < Side.NUM_VERTS; j++) {
						ov1 = segment.side[i].vert[j];
						ov2 = segment.side[i].vert[(j + 1) % NUM_VERTS];
						if ((v1 == ov1 && v2 == ov2) || (v2 == ov1 && v1 == ov2)) {
							return segment.side[i];
						}
					}
				}
			}

			return null;
		}

		public Side FindAdjacentSideOtherSegment(int edge)
		{
			int v1 = vert[edge];
			int v2 = vert[(edge + 1) % NUM_VERTS];

			for (int i = 0; i < Level.MAX_SEGMENTS; i++) {
				if (level.segment[i].Alive && i != segment.num) {
					for (int j = 0; j < Segment.NUM_SIDES; j++) {
						if (level.segment[i].neighbor[j] < 0 && level.segment[i].side[j].SharesTheseTwoVerts(v1, v2)) {
							return level.segment[i].side[j];
						}
					}
				}
			}

			// Fall thru case (finds the side for this segment)
			return FindAdjacentSideSameSegment(edge);
		}

		public Vector3 FindAdjacentSideEdgeDirAway(int edge)
		{
			// First get the adjacent side
			Side s = FindAdjacentSideSameSegment(edge);

			int v1 = vert[edge];
			int v2 = vert[(edge + 1) % NUM_VERTS];
			int ov1;
			int ov2;
			// Then find the edge in that segment that only shares the first edge vert (and not the second)
			for (int i = 0; i < Side.NUM_VERTS; i++) {
				ov1 = s.vert[i];
				ov2 = s.vert[(i + 1) % NUM_VERTS];
				if ((ov1 == v1 && ov2 != v2)) {
					return (level.vertex[ov2].position - level.vertex[ov1].position).Normalized();
				}

				if (ov2 == v1 && ov1 != v2) {
					return (level.vertex[ov1].position - level.vertex[ov2].position).Normalized();
				}
			}

			// Fall thru case (should never happen)
			Utility.DebugPopup("Could not find adjacent edge direction, segment #: " + segment.num, "ERROR");

			return FindNormal();
		}


		/// <summary>
		/// Given the verts of a triangle, and a barycentric coordinate, calculate the point of the triangle represented by the coordinate
		/// </summary>
		/// <param name="tri0"></param>
		/// <param name="tri1"></param>
		/// <param name="tri2"></param>
		/// <param name="baryCoords"></param>
		/// <returns>Point in the triangle represented by baryCoords</returns>
		public static Vector2 Barycentric(Vector2 tri0, Vector2 tri1, Vector2 tri2, Vector2 baryCoords)
		{
			return new Vector2((tri0.X + (baryCoords.X * (tri1.X - tri0.X))) + (baryCoords.Y * (tri2.X - tri0.X)),
									(tri0.Y + (baryCoords.X * (tri1.Y - tri0.Y))) + (baryCoords.Y * (tri2.Y - tri0.Y)));
		}

		public static Vector3 Barycentric(Vector3 tri0, Vector3 tri1, Vector3 tri2, Vector2 baryCoords)
		{
			return new Vector3((tri0.X + (baryCoords.X * (tri1.X - tri0.X))) + (baryCoords.Y * (tri2.X - tri0.X)),
									(tri0.Y + (baryCoords.X * (tri1.Y - tri0.Y))) + (baryCoords.Y * (tri2.Y - tri0.Y)),
									(tri0.Z + (baryCoords.X * (tri1.Z - tri0.Z))) + (baryCoords.Y * (tri2.Z - tri0.Z)));
		}

		/// <summary>
		/// Given the verts of a triangle and a point on the plane of the triangle, calculate the barycentric coordinates
		/// of that point
		/// </summary>
		/// <param name="tri0"></param>
		/// <param name="tri1"></param>
		/// <param name="tri2"></param>
		/// <param name="pt"></param>
		/// <returns>Barycentric coordinates of pt</returns>
		public static Vector2 Calculate2DBarycentric(Vector2 tri0, Vector2 tri1, Vector2 tri2, Vector2 pt)
		{
			float double_area = -tri1.Y * tri2.X + tri0.Y * (-tri1.X + tri2.X) + tri0.X * (tri1.Y - tri2.Y) + tri1.X * tri2.Y;

			float s = 1.0f / double_area * (tri0.Y * tri2.X - tri0.X * tri2.Y + (tri2.Y - tri0.Y) * pt.X + (tri0.X - tri2.X) * pt.Y);
			float t = 1.0f / double_area * (tri0.X * tri1.Y - tri0.Y * tri1.X + (tri0.Y - tri1.Y) * pt.X + (tri1.X - tri0.X) * pt.Y);
			return new Vector2(s, t);
		}

		/// <summary>
		/// Given a UV coordinate, this function will return the point in world space on the plane of
		/// this side for that UV coordinate.
		/// </summary>
		/// <param name="uv">The UV coordinates of the query</param>
		/// <param name="isOnSide">Set to true if the UV coordinate is within the triangles of this side, otherwise it is outside of the side</param>
		/// <returns></returns>
		public Vector3 CalculateWorldPointOfUV(Vector2 uv, out bool isOnSide)
		{
			var triangleVertices = new int[2][]
											{
												new int[3] {0, 1, 2},
												new int[3] {0, 2, 3},
											};

			// Loop over the triangles of this side, process each individually
			for (int triIdx = 0; triIdx < 2; ++triIdx) {
				var verts = triangleVertices[triIdx];

				// Define a triangle in UV space
				Vector2 uvTri0 = this.uv[verts[0]];
				Vector2 uvTri1 = this.uv[verts[1]];
				Vector2 uvTri2 = this.uv[verts[2]];

				// Get the Barycentric coordinates of our query UV in this UV triangle
				Vector2 bary = Calculate2DBarycentric(uvTri0, uvTri1, uvTri2, uv);
				isOnSide = (bary.X >= 0.0f && bary.Y >= 0.0f && (bary.X + bary.Y) <= 1.0f);

				if (triIdx == 0) {
					// Skip over the first triangle if the barycentric coordinates are off-triangle
					// Perhaps the UV is on the second triangle, barring that we'll fallback to calculating
					// it off-triangle
					if (!isOnSide) {
						// Skip - not on this triangle
						continue;
					}
				}

				// Using the same Barycentric coordinate, calculate the point using the world space triangle
				var levelVertex = this.level.vertex;
				var sideVertToLevelVert = this.vert;
				Vector3 posTri0 = levelVertex[sideVertToLevelVert[verts[0]]].position;
				Vector3 posTri1 = levelVertex[sideVertToLevelVert[verts[1]]].position;
				Vector3 posTri2 = levelVertex[sideVertToLevelVert[verts[2]]].position;
				return Barycentric(posTri0, posTri1, posTri2, bary);
			}

			// Satisfy the warning - though we won't get here
			isOnSide = false;
			return Vector3.Zero;
		}

		// Find closest increment of 0.5, 0.5 near the center of the side
		public Vector3 FindUVCenterIn3D()
		{
			// Create a 2D bounding box of the UV coordinates for this side to get the valid range of
			// UVs. From this bounding box we'll know which increments of (0.5 + i, 0.5 + j) where [i,j] are whole numbers
			// on the number line from -inf to +inf, cropped to the respective min/max of the UV bounding box.
			Vector2 min = this.uv[0];
			Vector2 max = this.uv[0];
			for (int i = 1; i < NUM_VERTS; i++) {
				min = Utility.V2Min(min, this.uv[i]);
				max = Utility.V2Max(max, this.uv[i]);
			}

			// Snap the values to the closest whole value
			int start_x = (int)Utility.SnapValue(min.X, 1.0f);
			int end_x = (int)Utility.SnapValue(max.X, 1.0f);
			int start_y = (int)Utility.SnapValue(min.Y, 1.0f);
			int end_y = (int)Utility.SnapValue(max.Y, 1.0f);

			// Default is 0.5, 0.5, others must be closer to use instead
			bool unused_bool;
			Vector3 closest_pos = CalculateWorldPointOfUV(new Vector2(0.5f, 0.5f), out unused_bool);
			Vector3 center = FindCenter();
			float closest_dist = (closest_pos - center).Length;

			// Find the actual closest increment
			Vector3 pos;
			float dist;
			for (int i = start_x; i < end_x; i++) {
				for (int j = start_y; j < end_y; j++) {
					pos = CalculateWorldPointOfUV(new Vector2(0.5f + (float)i, 0.5f + (float)j), out unused_bool);
					dist = (pos - center).Length;
					if (dist < closest_dist) {
						closest_pos = pos;
						closest_dist = dist;
					}
				}
			}

			return closest_pos;
		}

		public Vector3 FindUVUpVector()
		{
			// Using the V direction of the textures, translate to world space to get a direction vector
			bool dummy;
			Vector3 pt0 = CalculateWorldPointOfUV(new Vector2(0.0f, 0.0f), out dummy);
			Vector3 pt1 = CalculateWorldPointOfUV(new Vector2(0.0f, 1.0f), out dummy);
			Vector3 upDirection = pt0 - pt1;
			return upDirection.Normalized();
		}

		public Vector3 FindCenter()
		{
			Vector3 center = Vector3.Zero;
			for (int i = 0; i < NUM_VERTS; i++) {
				center += level.vertex[vert[i]].position;
			}

			return (center / 4f);
		}

#if OVERLOAD_LEVEL_EDITOR
		//Instead of averaging the four points, finds the center point on the triangle edge that bisects the side.
		//This will generate a valid point for non-planar sides
		public Vector3 FindCenterBetter()
		{
			Segment seg = this.segment;
			Level level = seg.level;

			bool invertOrderForSide = ((seg.neighbor[this.num] > -1) && (seg.neighbor[this.num] < seg.num));
			Level.QuadTriangulationOrder triangulationOrder = Level.GetTriangulationOrder(level, seg.num, this.num);
			int[] side_verts = Utility.SideVertsFromSegVerts(this.segment.vert, this.num);

			int corner = triangulationOrder.GetBisectingEdgeCorner(invertOrderForSide);

			return (level.vertex[side_verts[corner]].position + level.vertex[side_verts[corner + 2]].position) / 2f;
		}
#endif

		public Vector3 FindEdgeDir(EdgeOrder edge)
		{
			return FindEdgeDir((int)edge);
		}

		public Vector3 FindEdgeDir(int edge)
		{
			return (level.vertex[vert[edge]].position - level.vertex[vert[(edge + 1) % Side.NUM_EDGES]].position).Normalized();
		}

		public Vector3 FindEdgeCenterOffset(int edge, float pct)
		{
			Vector3 center;
			switch (edge) {
				default:
				case (int)EdgeOrder.RIGHT:
					center = level.vertex[vert[0]].position * pct + level.vertex[vert[1]].position * (1f - pct);
					break;
				case (int)EdgeOrder.DOWN:
					center = level.vertex[vert[1]].position * pct + level.vertex[vert[2]].position * (1f - pct);
					break;
				case (int)EdgeOrder.LEFT:
					center = level.vertex[vert[2]].position * pct + level.vertex[vert[3]].position * (1f - pct);
					break;
				case (int)EdgeOrder.UP:
					center = level.vertex[vert[3]].position * pct + level.vertex[vert[0]].position * (1f - pct);
					break;
			}
			return (center);
		}

		public Vector3 FindEdgeCenter(int edge)
		{
			Vector3 center;
			switch (edge) {
				default:
				case (int)EdgeOrder.RIGHT:
					center = level.vertex[vert[0]].position + level.vertex[vert[1]].position;
					break;
				case (int)EdgeOrder.DOWN:
					center = level.vertex[vert[1]].position + level.vertex[vert[2]].position;
					break;
				case (int)EdgeOrder.LEFT:
					center = level.vertex[vert[2]].position + level.vertex[vert[3]].position;
					break;
				case (int)EdgeOrder.UP:
					center = level.vertex[vert[3]].position + level.vertex[vert[0]].position;
					break;
			}
			return (center / 2f);
		}

		public Vector3 FindBestEdgeDir()
		{
			Vector3 e1 = FindEdgeDir(0);
			Vector3 e2 = FindEdgeDir(1);

			if (Utility.AlmostCardinal(e1)) {
				return e1;
			}
			else if (Utility.AlmostCardinal(e2)) {
				return Vector3.Cross(e2, FindNormal());
			}
			else {
				return e1;
			}
		}

		public float FindLongestDiagonal()
		{
			float max = (level.vertex[vert[0]].position - level.vertex[vert[2]].position).Length;
			max = Math.Max(max, (level.vertex[vert[1]].position - level.vertex[vert[3]].position).Length);
			return max;
		}


		public bool HasTwoSharedVerts(Side s)
		{
			int shared = 0;
			for (int i = 0; i < Side.NUM_VERTS; i++) {
				for (int j = 0; j < Side.NUM_VERTS; j++) {
					if (vert[i] == s.vert[j]) {
						shared += 1;
						if (shared >= 2) {
							return true;
						}
					}
				}
			}

			return false;
		}

		public bool SharesTheseTwoVerts(int v1, int v2)
		{
			int shared = 0;
			for (int i = 0; i < Side.NUM_VERTS; i++) {
				if (vert[i] == v1 || vert[i] == v2) {
					shared += 1;
					if (shared >= 2) {
						return true;
					}
				}
			}

			return false;
		}

		public bool HasAnySharedVerts(Side s)
		{
			for (int i = 0; i < Side.NUM_VERTS; i++) {
				for (int j = 0; j < Side.NUM_VERTS; j++) {
					if (vert[i] == s.vert[j]) {
						return true;
					}
				}
			}

			return false;
		}

		public bool SomeEdgeRequiresTesselation()
		{
			for (int i = 0; i < NUM_EDGES; i++) {
				if (edge[i].adjacent_to_tesselate) {
					return true;
				}
			}
			return false;
		}

		public bool[] EdgesRequiringTesselation()
		{
			bool[] ert = new bool[NUM_EDGES];
			for (int i = 0; i < NUM_EDGES; i++) {
				ert[i] = edge[i].adjacent_to_tesselate;
			}

			return ert;
		}
	}

	// Edge data is not saved out, it is generated
	public class Edge
	{
		public bool adjacent_to_lava = false;
		public bool adjacent_to_tesselate = false;
	}
}