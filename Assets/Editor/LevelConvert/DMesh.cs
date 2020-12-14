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

using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using System;
using OpenTK;
using Newtonsoft.Json.Linq;
using OverloadLevelEditor.Clipping;

// DMESH
// DMesh geometry

namespace OverloadLevelEditor
{
	public enum DecalFlags
	{
		
	}

	public enum FaceFlags
	{
		NO_COLLIDE = 1,
		NO_RENDER = 2,
		COLOR1 = 4,
		COLOR2 = 8,
		COLOR3 = 16,
		COLOR4 = 32,
	}

	// This might get more options later
	public enum LightStyle
	{
		SPOT,
		POINT,
		POINT_NO_SHADOW,
		SPOT_NO_SHADOW,

		NUM,
	}

	// This is now used for security level, not flares
	public enum LightFlare
	{
		NONE,
		LEVEL1,
		LEVEL2,
		LEVEL3,
		LEVEL4,
		EXIT,

		NUM,
	}

	// DMESH - A decal mesh object (the base piece that gets tiled)
	// - Saved in local space, facing Y up, tiles every 4 in XZ
	public partial class DMesh
	{
		public const int NUM_LIGHTS = 4;
		public const int NUM_COLORS = 4;
		
		public string name; // Filename of this decal mesh
		public List<Vector3> vertex;
		public List<DTriangle> triangle;
		public int flags;
		public int smooth_angle_same = 0;
		public int smooth_angle_diff = 0;
		
		public List<string> tex_name;

		public List<DLight> light;
#if OVERLOAD_LEVEL_EDITOR
		public List<Color> color;
#else
		public List<Vector3> color;
#endif
		
		public bool IsFlagSet(DecalFlags df)
		{
			return ((flags & (int)df) == (int)df);
		}

		public DMesh(string n)
		{
			name = n;
			vertex = new List<Vector3>();
			triangle = new List<DTriangle>();
#if OVERLOAD_LEVEL_EDITOR
			m_tex_gl_id = new List<int>();
#endif
			tex_name = new List<string>();
			light = new List<DLight>();
#if OVERLOAD_LEVEL_EDITOR
			color = new List<Color>();
#else
			color = new List<Vector3>();
#endif

			for (int i = 0; i < NUM_LIGHTS; i++) {
				DLight dl = new DLight(Vector3.Zero);
				light.Add(dl);
			} 
			for (int i = 0; i < NUM_COLORS; i++) {
#if OVERLOAD_LEVEL_EDITOR
				color.Add(Color.White);
#else
				color.Add( Vector3.One );
#endif
			}
		}

		// For easy conversion from an OBJ
		public void AddVertex(float x, float y, float z)
		{
			vertex.Add(new Vector3(x, y, z));
		}

		// Assumes the face has 3 verts
		public void AddFace(int[] vrt_idx, Vector3[] nrml, Vector2[] uv, int tex_idx)
		{
			DTriangle tri = new DTriangle(vrt_idx, nrml, uv, tex_idx);
			triangle.Add(tri);
		}

		public void Copy(DMesh src, bool ignore_light = false)
		{
			name = src.name;
			flags = src.flags;
			for (int i = 0; i < src.vertex.Count; i++) {
				vertex.Add(src.vertex[i]);
			}
			for (int i = 0; i < src.triangle.Count; i++) {
				DTriangle dtri = new DTriangle(src.triangle[i], 0);
				triangle.Add(dtri);
			}
			CopyExtraProperties(src);

			for (int i = 0; i < src.tex_name.Count; i++) {
				tex_name.Add(src.tex_name[i]);
#if OVERLOAD_LEVEL_EDITOR
				m_tex_gl_id.Add(src.m_tex_gl_id[i]);
#endif
			}
			if (!ignore_light) {
				light.Clear();
				for (int i = 0; i < src.light.Count; i++) {
					DLight dl = new DLight(src.light[i]);
					light.Add(dl);
				}
				color.Clear();
				for (int i = 0; i < src.color.Count; i++) {
					color.Add(src.color[i]);
				}
			}
		}

		public void CopyExtraProperties(DMesh src)
		{
#if OVERLOAD_LEVEL_EDITOR
			for (int i = 0; i < src.polygon.Count; i++) {
				DPoly dpoly = new DPoly(src.polygon[i]);
				polygon.Add(dpoly);
			}
			for (int i = 0; i < src.vert_info.Count; i++) {
				DVert dvert = new DVert(src.vert_info[i]);
				vert_info.Add(dvert);
			}

			selected_poly = src.selected_poly;
			selected_vert = src.selected_vert;
			num_marked_polys = src.num_marked_polys;
			num_marked_verts = src.num_marked_verts;
#endif
		}

		// Centers everything like it should (on the origin, positive Y only)
		public void CenterMesh()
		{
			if (vertex.Count > 0) {
				UpdateMinMax();

				Vector3 center = Vector3.Zero;
				center.X = (min.X + max.X) * 0.5f;
				center.Y = min.Y;
				center.Z = (min.Z + max.Z) * 0.5f;

				for (int i = 0; i < vertex.Count; i++) {
					vertex[i] -= center;
				}
			}
		}

		// For aligning/centering the mesh
		Vector3 min;
		Vector3 max;
		
		public void UpdateMinMax()
		{
			if (vertex.Count > 0) {
				min = vertex[0];
				max = vertex[0];

				for (int i = 1; i < vertex.Count; i++) {
					min = Utility.V3Min(min, vertex[i]);
					max = Utility.V3Max(max, vertex[i]);
				}
			} else {
				min = Vector3.Zero;
				max = Vector3.Zero;
			}
		}

		public void NudgeMesh(Vector3 nudge_vec)
		{
			for (int i = 0; i < vertex.Count; i++) {
				vertex[i] += nudge_vec;
			}
		}

		// This can be used for 1 edge or 2 edge for a corner
		public void AlignMeshEdges(bool[] edge)
		{
			UpdateMinMax();

			Vector3 u_offset = Vector3.Zero;
			if (edge[(int)EdgeOrder.LEFT]) {
				u_offset = Vector3.UnitX * (-2f - min.X);
			} else if (edge[(int)EdgeOrder.RIGHT]) {
				u_offset = Vector3.UnitX * (2f - max.X);
			}
			for (int i = 0; i < vertex.Count; i++) {
				vertex[i] += u_offset;
			}

			Vector3 v_offset = Vector3.Zero;
			if (edge[(int)EdgeOrder.DOWN]) {
				v_offset = Vector3.UnitZ * (-2f - min.Z);

			} else if (edge[(int)EdgeOrder.UP]) {
				v_offset = Vector3.UnitZ * (2f - max.Z);
			}
			for (int i = 0; i < vertex.Count; i++) {
				vertex[i] += v_offset;
			}
		}

		public void MaybeFlip(DecalMirror dmirror)
		{
			switch (dmirror) {
				case DecalMirror.MIRROR_U:
					FlipMesh(-1f, 1f);
					break;
				case DecalMirror.MIRROR_V:
					FlipMesh(1f, -1f);
					break;
				default:
					break;
			}
		}

		public void RotateMesh90()
		{
			for (int i = 0; i < vertex.Count; i++) {
				vertex[i] = Vector3.Transform(vertex[i], Matrix4.CreateRotationY(Utility.RAD_90));
			}
		}

		public void RotateMesh180()
		{
			for (int i = 0; i < vertex.Count; i++) {
				vertex[i] = Vector3.Transform(vertex[i], Matrix4.CreateRotationY(Utility.RAD_180));
			}
		}

		public void FlipMesh(float u, float v)
		{
			Vector3 flip = new Vector3(u, 1f, v); 
			for (int i = 0; i < vertex.Count; i++) {
				vertex[i] *= flip;
			}

			for (int i = 0; i < triangle.Count; i++) {
				triangle[i].ReverseWindingOrder();
			}
		}

		public void FlipVertNormalsZ()
		{
			for (int i = 0; i < triangle.Count; i++) {
				triangle[i].normal[0].Z *= -1f;
				triangle[i].normal[1].Z *= -1f;
				triangle[i].normal[2].Z *= -1f;
			}
		}

		public void FlipVertNormalsY()
		{
			for (int i = 0; i < triangle.Count; i++) {
				triangle[i].normal[0].Y *= -1f;
				triangle[i].normal[1].Y *= -1f;
				triangle[i].normal[2].Y *= -1f;
			}
		}

		public void FlipVertNormalsX()
		{
			for (int i = 0; i < triangle.Count; i++) {
				triangle[i].normal[0].X *= -1f;
				triangle[i].normal[1].X *= -1f;
				triangle[i].normal[2].X *= -1f;
			}
		}

		public Vector3[] vert_scrn_pos;

		public int SelectFace(Vector2 mouse_pos, Matrix4 cam_mat, Matrix4 persp_mat, Vector2 control_sz)
		{
			// Transform all the verts to screen position
			vert_scrn_pos = new Vector3[vertex.Count];
			for (int i = 0; i < vertex.Count; i++) {
				vert_scrn_pos[i] = WorldToScreenPos(vertex[i], cam_mat, persp_mat, control_sz);
			}

			int closest_tri = -1;
			float closest_z = 99999f;
			Vector3 center_pos;

			Vector3 normal;
			// Go through all the triangles, first check normal, then test for being inside
			for (int i = 0; i < triangle.Count; i++) {
				normal = Utility.FindNormal(vert_scrn_pos[triangle[i].vert[0]], vert_scrn_pos[triangle[i].vert[1]], vert_scrn_pos[triangle[i].vert[2]]);
				if (normal.Z > 0f) {
					bool inside = Utility.PointInsideTri(mouse_pos, vert_scrn_pos[triangle[i].vert[0]], vert_scrn_pos[triangle[i].vert[1]], vert_scrn_pos[triangle[i].vert[2]]);
					if (inside) {
						center_pos = (vert_scrn_pos[triangle[i].vert[0]] + vert_scrn_pos[triangle[i].vert[1]] + vert_scrn_pos[triangle[i].vert[2]]) * 0.3333f;
						if (center_pos.Z < closest_z) {
							closest_tri = i;
							closest_z = center_pos.Z;
						}
					}
				}
			}

			// Update stuff based on the closest triangle
			return closest_tri;
		}

		public Vector3 WorldToScreenPos(Vector3 pos, Matrix4 cam_mat, Matrix4 persp_mat, Vector2 control_sz)
		{
			pos = Vector3.Transform(pos, Matrix4.CreateRotationX(-Utility.RAD_90));
			pos.Z *= -1f;

			return Utility.WorldToScreen(pos, cam_mat, persp_mat, control_sz.X, control_sz.Y, true);
		}

		public int GetFirstAdjacentFace(int idx)
		{
			for (int i = 0; i < triangle.Count; i++) {
				if (i != idx && AreTrianglesAdjacent(triangle[i], triangle[idx])) {
					return i;
				}
			}

			// Return the original face if none found
			return idx;
		}

		public const float MATCH_DIST_SQ = 0.0001f;
		public const float MATCH_NORMAL = 0.999f;

		public bool AreTrianglesAdjacent(DTriangle t1, DTriangle t2)
		{
			int match_verts = 0;
			for (int i = 0; i < 3; i++) {
				for (int j = 0; j < 3; j++) {
					if ((vertex[t1.vert[i]] - vertex[t2.vert[j]]).LengthSquared <= MATCH_DIST_SQ) {
						match_verts += 1;
					}
				}
			}

			if (match_verts >= 2) {
				Vector3 n1 = Utility.FindNormal(vertex[t1.vert[0]], vertex[t1.vert[1]], vertex[t1.vert[2]]);
				Vector3 n2 = Utility.FindNormal(vertex[t2.vert[0]], vertex[t2.vert[1]], vertex[t2.vert[2]]);

				if (Vector3.Dot(n1, n2) > MATCH_NORMAL) {
					return true;
				}
			}

			return false;
		}

		public void Serialize(JObject root)
		{
			JArray j_verts = new JArray();
			for (int i = 0; i < vertex.Count; i++) {
				JObject j_vert = new JObject();
				j_vert["x"] = vertex[i].X;
				j_vert["y"] = vertex[i].Y;
				j_vert["z"] = vertex[i].Z;
				j_verts.Add(j_vert);
			}
			root["verts"] = j_verts;

#if OVERLOAD_LEVEL_EDITOR
			SerializePolys(root);
#endif

			JObject j_tris = new JObject();
			root["triangles"] = j_tris;
			for (int i = 0; i < triangle.Count; i++) {
				JObject j_tri = new JObject();
				triangle[i].Serialize(j_tri);
				j_tris[i.ToString()] = j_tri;
			}

			JArray j_tex_names = new JArray();
			for (int i = 0; i < tex_name.Count; i++) {
				j_tex_names.Add(tex_name[i]);
			}
			root["tex_names"] = j_tex_names;

			JObject j_lights = new JObject();
			root["lights"] = j_lights;
			for (int i = 0; i < light.Count; i++) {
				JObject j_light = new JObject();
				light[i].Serialize(j_light);
				j_lights[i.ToString()] = j_light;
			}

			JArray j_colors = new JArray();
			for (int i = 0; i < color.Count; i++) {
				JObject j_color = new JObject();
#if OVERLOAD_LEVEL_EDITOR
				j_color["r"] = color[i].R;
				j_color["g"] = color[i].G;
				j_color["b"] = color[i].B;
#else
				j_color[ "r" ] = color[ i ].X;
				j_color[ "g" ] = color[ i ].Y;
				j_color[ "b" ] = color[ i ].Z;
#endif
				j_colors.Add(j_color);
			}
			root["colors"] = j_colors;

			root["smooth_diff"] = smooth_angle_diff;
			root["smooth_same"] = smooth_angle_same;
		}

		public void Deserialize(JObject root)
		{
			vertex.Clear();
			triangle.Clear();
#if OVERLOAD_LEVEL_EDITOR
			m_tex_gl_id.Clear();
#endif
			tex_name.Clear();
			
			Vector3 vec = Vector3.Zero;
			JArray j_verts = root["verts"].GetArray();
			foreach (JObject j_vert in j_verts) {
				vec.X = j_vert["x"].GetFloat(0.0f);
				vec.Y = j_vert["y"].GetFloat(0.0f);
				vec.Z = j_vert["z"].GetFloat(0.0f);
				vertex.Add(vec);
			}

#if OVERLOAD_LEVEL_EDITOR
			DeserializePolys(root);
#endif

			JObject j_tris = root["triangles"].GetObject();
			foreach (var kvp in j_tris) {
				JObject j_tri = kvp.Value.GetObject();
				DTriangle d_tri = new DTriangle(j_tri);
				triangle.Add(d_tri);
			}

			JArray j_tex_names = root["tex_names"].GetArray();
			for (int i = 0; i < j_tex_names.Count; i++) {
#if OVERLOAD_LEVEL_EDITOR
				m_tex_gl_id.Add(-1);
#endif
				tex_name.Add(j_tex_names[i].GetString(""));
			}

			JObject j_lights = root["lights"].GetObject();
			if (j_lights.Count > 0) {
				light.Clear();
			}
			int count = 0;
			foreach (var kvp in j_lights) {
				JObject j_light = kvp.Value.GetObject();
				DLight d_light = new DLight(j_light);
				if (count < 4) {
					light.Add(d_light);
					count += 1;
				}
			}

			int cr, cg, cb;
			count = 0;
			JArray j_colors = root["colors"].GetArray();
			if (j_colors.Count > 0) {
				color.Clear();
			}
			foreach (JObject j_color in j_colors) {
#if OVERLOAD_LEVEL_EDITOR
				cr = j_color["r"].GetInt(255);
				cg = j_color["g"].GetInt(255);
				cb = j_color["b"].GetInt(255);
				if (count < 4) {
					color.Add(Color.FromArgb(cr, cg, cb));
				}
				count += 1;
#else
				cr = j_color[ "r" ].GetInt( 255 );
				cg = j_color[ "g" ].GetInt( 255 );
				cb = j_color[ "b" ].GetInt( 255 );
				color.Add( new Vector3( (float)cr / 255.0f, (float)cg / 255.0f, (float)cb / 255.0f ) );
#endif
			}

#if OVERLOAD_LEVEL_EDITOR
			for (int i = color.Count; i < NUM_COLORS; i++) {
				color.Add(Color.White);
			}
#endif

			smooth_angle_same = root["smooth_same"].GetInt(smooth_angle_same);
			smooth_angle_diff = root["smooth_diff"].GetInt(smooth_angle_diff);
		}

        enum BadTriangleReason
        {
            ZeroArea,
            BadUV,
        }

        class BadTriangle
        {
            public int index;
            public BadTriangleReason reason;
        }

        static double CalculateTriangleArea( Vector3 edge0, Vector3 edge1, Vector3 edge2 )
        {
            // Calculate the area of the triangle
            float lenA = edge0.Length;
            float lenB = edge1.Length;
            float lenC = edge2.Length;
            float perimeter = ( lenA + lenB + lenC ) * 0.5f;
            double triangleArea = Math.Sqrt( perimeter * ( perimeter - lenA ) * ( perimeter - lenB ) * ( perimeter - lenC ) );
            return triangleArea;
        }

        public static bool CheckAndCleanMeshIssues( ref List<DTriangle> triangle, List<Vector3> vertex, bool removeSmallInvalidTriangles, out string issues )
        {
            const float kLengthEpsilon = 0.0001f;
            const float kDotEpsilon = 0.99999f;
            const double kUVEpsilon = 0.000001;
            const double kMaxAreaForAutoRemoval = 0.0003;

            Vector3[] verts = new Vector3[3];

            bool result = false;

            System.Text.StringBuilder builder = new System.Text.StringBuilder();

            List<BadTriangle> badTriangleIndices = new List<BadTriangle>();
            foreach( int triIdx in Enumerable.Range( 0, triangle.Count ) ) {
                var tri = triangle[triIdx];
                if( tri.vert.Length != 3 ) {
                    builder.AppendFormat( "-- Polygon is not 3 verts\n" );
                    result = true;
                    badTriangleIndices.Add( new BadTriangle() { index = triIdx, reason = BadTriangleReason.ZeroArea } );
                    continue;
                }

                for( int i = 0; i < 3; ++i ) {
                    verts[i] = vertex[tri.vert[i]];
                }

                Vector3 edge0 = verts[0] - verts[1];
                Vector3 edge1 = verts[2] - verts[1];
                Vector3 edge2 = verts[0] - verts[2];

                bool isZeroAreaTriangle = false;
                bool isBadUVTriangle = false;

                if( edge0.Length <= kLengthEpsilon ) {
                    builder.AppendFormat( "-- triangle in polygon {3} edge0 is collapsed {0} {1} {2}\n", verts[0].ToString(), verts[1].ToString(), verts[2].ToString(), tri.poly_num.ToString() );
                    isZeroAreaTriangle = true;
                }
                if( edge1.Length <= kLengthEpsilon ) {
                    builder.AppendFormat( "-- triangle in polygon {3} edge1 is collapsed {0} {1} {2}\n", verts[0].ToString(), verts[1].ToString(), verts[2].ToString(), tri.poly_num.ToString() );
                    isZeroAreaTriangle = true;
                }
                if( edge2.Length <= kLengthEpsilon ) {
                    builder.AppendFormat( "-- triangle in polygon {3} edge2 is collapsed {0} {1} {2}\n", verts[0].ToString(), verts[1].ToString(), verts[2].ToString(), tri.poly_num.ToString() );
                    isZeroAreaTriangle = true;
                }

                if( !isZeroAreaTriangle ) {
                    float edge_dot = Math.Abs( Vector3.Dot( edge0.Normalized(), edge1.Normalized() ) );
                    if( edge_dot >= kDotEpsilon ) {
                        builder.AppendFormat( "-- triangle has no area {0} {1} {2}\n", verts[0].ToString(), verts[1].ToString(), verts[2].ToString() );
                        isZeroAreaTriangle = true;
                    }
                }

                // Check UVs to make sure there is a gradient
                Vector2 w1 = tri.tex_uv[0];
                Vector2 w2 = tri.tex_uv[1];
                Vector2 w3 = tri.tex_uv[2];
                double s1 = w2.X - w1.X;
                double s2 = w3.X - w1.X;
                double t1 = w2.Y - w1.Y;
                double t2 = w3.Y - w1.Y;

                double den = ( s1 * t2 - s2 * t1 );
                if( Math.Abs( den ) <= kUVEpsilon ) {
                    double area = CalculateTriangleArea( edge0, edge1, edge2 );
                    if( area <= kMaxAreaForAutoRemoval ) {
                        builder.AppendFormat( "-- triangle has invalid UVs {0} {1} {2} -- area {3} (REMOVING)\n", w1.ToString(), w2.ToString(), w3.ToString(), area );
                        isZeroAreaTriangle = true;
                    } else {
                        builder.AppendFormat( "-- triangle has invalid UVs {0} {1} {2} -- area {3} (FIXING)\n", w1.ToString(), w2.ToString(), w3.ToString(), area );
                        isBadUVTriangle = true;
                    }
                }

                if( isZeroAreaTriangle ) {
                    result = true;

                    // Calculate the area of the triangle
                    double triangleArea = CalculateTriangleArea( edge0, edge1, edge2 );
                    if( triangleArea < kMaxAreaForAutoRemoval ) {
                        badTriangleIndices.Add( new BadTriangle() { index = triIdx, reason = BadTriangleReason.ZeroArea } );
                    }
                } else if( isBadUVTriangle ) {
                    result = true;
                    badTriangleIndices.Add( new BadTriangle() { index = triIdx, reason = BadTriangleReason.BadUV } );
                }
            }

            if( removeSmallInvalidTriangles && badTriangleIndices.Count > 0 ) {
                List<DTriangle> newTriangles = new List<DTriangle>();
                int numRemoved = 0;

                int numBadTriangles = badTriangleIndices.Count;
                int badIndex = 0;
                foreach( int srcTriIndex in Enumerable.Range( 0, triangle.Count ) ) {

                    if( badIndex < numBadTriangles && badTriangleIndices[badIndex].index == srcTriIndex ) {
                        bool skipThis = false;
                        switch( badTriangleIndices[badIndex++].reason ) {
                        case BadTriangleReason.ZeroArea: {
                                skipThis = true;
                            }
                            break;
                        case BadTriangleReason.BadUV: {
                                DTriangle tri = triangle[srcTriIndex];
                                tri.tex_uv[1] = tri.tex_uv[1] + new Vector2( 0.1f, 0.1f );
                                tri.tex_uv[2] = tri.tex_uv[2] - new Vector2( 0.1f, 0.1f );

                            }
                            break;
                        }
                        if( skipThis ) {
                            ++numRemoved;
                            continue;
                        }
                    }

                    newTriangles.Add( triangle[srcTriIndex] );
                }

                triangle = newTriangles;
                builder.AppendFormat( "Removed {0} triangles\n", numRemoved );
            }

            issues = builder.ToString();

            return result;
        }

        /// <summary>
        /// Return true if the mesh has degenerate triangles or other issues
        /// </summary>
        /// <returns></returns>
        public bool CheckAndCleanMeshIssues( bool removeSmallInvalidTriangles, out string issues )
        {
            return CheckAndCleanMeshIssues( ref triangle, vertex, removeSmallInvalidTriangles, out issues );
        }
    }

	public class DLight
	{
		public bool enabled;
		public Vector3 position;
		public LightStyle style;
		public LightFlare flare;
		public int color_index;
		public float intensity;
		public float range;
		
		// These are only relevant for spot lights (aka not point lights)
		// - Also note that lights have no roll value
		public float angle;
		private float _rot_yaw = 0.0f;
		private float _rot_pitch = 0.0f;
		private Matrix4 _rotation = Matrix4.Identity;		

		public float rot_yaw
		{
			get { return _rot_yaw; }
			set
			{
				_rot_yaw = value;
				_rotation = CalculateRotationMatrix(this);
			}
		}

		public float rot_pitch
		{
			get { return _rot_pitch; }
			set
			{
				_rot_pitch = value;
				_rotation = CalculateRotationMatrix(this);
			}
		}

		public Matrix4 rotation
		{
			get { return _rotation; }
			// Note: No 'set' as it is calculated from the other properties
		}

		// Get the rotation matrix for the spot light, in decal modeling space
		public static Matrix4 CalculateRotationMatrix(DLight l)
		{
			// Get the orientation for the user factors
			var user_set_orient = Quaternion.FromAxisAngle(Vector3.UnitX, l.rot_pitch) * Quaternion.FromAxisAngle(Vector3.UnitY, -l.rot_yaw);

			// Get the orientation that will make the user factors start from a spot light that points out from the decal
			var decal_to_spot_space = Quaternion.FromAxisAngle(Vector3.UnitX, -Utility.RAD_90);

			// Combine the two so that the default spot light orientation is independent of the user factor orientation
			// finally arriving at a Decal-To-Spotlight rotation.
			var new_orientation_std = (user_set_orient * Quaternion.Conjugate(decal_to_spot_space)).Normalized();
			var new_orientation_dts = Quaternion.Conjugate(new_orientation_std);
			return Matrix4.CreateFromQuaternion(new_orientation_dts);
		}
		
		public DLight(Vector3 pos)
		{
			enabled = false;
			position = Vector3.Zero;
			style = LightStyle.POINT;
			flare = LightFlare.NONE;
			_rot_yaw = 0f;
			_rot_pitch = 0f;
			color_index = 0;
			intensity = 1f;
			range = 10f;
			angle = 45f;
			_rotation = CalculateRotationMatrix(this);
		}

		public DLight(DLight src)
		{
			Copy(src);
		}

		public void Copy(DLight src)
		{
			enabled = src.enabled;
			position = src.position;
			style = src.style;
			flare = src.flare;
			_rot_yaw = src._rot_yaw;
			_rot_pitch = src._rot_pitch;
			color_index = src.color_index;
			intensity = src.intensity;
			range = src.range;
			angle = src.angle;
			_rotation = src._rotation;
		}

		public DLight(JObject root)
		{
			enabled = root["enabled"].GetBool(false);
			style = root["style"].GetEnum<LightStyle>(LightStyle.SPOT);
			flare = root["flare"].GetEnum<LightFlare>(LightFlare.NONE);

			position = root["position"].GetVector3(Vector3.Zero);
			_rot_yaw = root["rot_yaw"].GetFloat(0f);
			_rot_pitch = root["rot_pitch"].GetFloat(0f);

			color_index = root["color_index"].GetInt(0);

			intensity = root["intensity"].GetFloat(1f);
			range = root["range"].GetFloat(1f);
			angle = root["angle"].GetFloat(45f);

			// Note: The above wrote directly to the backing values (_rot_yaw, _rot_pitch and _angle), so
			// we are responsible for updating the backing for _rotation. This was done so we didn't recalculate
			// rotation for each of those properties
			_rotation = CalculateRotationMatrix(this);
		}

		public void Serialize(JObject root)
		{
			root["enabled"] = enabled;
			root["style"] = style.ToString();
			root["flare"] = flare.ToString();

			root["position"] = position.Serialize();
			root["rot_yaw"] = rot_yaw;
			root["rot_pitch"] = rot_pitch;
			
			root["color_index"] = color_index;

			root["intensity"] = intensity;
			root["range"] = range;
			root["angle"] = angle;
		}

		public void CycleStyle()
		{
			style = (LightStyle)(((int)style + 1) % (int)LightStyle.NUM);
		}

		public void CycleFlare()
		{
			flare = (LightFlare)(((int)flare + 1) % (int)LightFlare.NUM);
		}

		public void CycleColorIndex()
		{
			color_index = (color_index + 1) % DMesh.NUM_COLORS;
		}
	}

	public partial class DTriangle
	{
		public int[] vert = new int[3];
		public Vector3[] normal = new Vector3[3];
		public Vector2[] tex_uv = new Vector2[3];
		public int tex_index;
		public int flags;
		public int poly_num;		//Which polygon this came from

		public DTriangle()
		{
		}

		public DTriangle(int[] vrt_idx, Vector3[] nrml, Vector2[] uv, int tex_idx)
		{
			for (int i = 0; i < 3; i++) {
				vert[i] = vrt_idx[i];
				normal[i] = nrml[i];
				tex_uv[i] = uv[i];
			}

			tex_index = tex_idx;
			flags = 0;
		}

		public DTriangle(DTriangle src, int vert_idx_offset)
		{
			for (int i = 0; i < 3; i++) {
				vert[i] = src.vert[i] + vert_idx_offset;
				normal[i] = src.normal[i];
				tex_uv[i] = src.tex_uv[i];
			}

			tex_index = src.tex_index;
			flags = src.flags;
		}

		public DTriangle(CTriangle ct, int vert_idx_offset, int triangle_index, bool use_clipped_data)
		{
			CVertex[] vdata = use_clipped_data ? ct.clipped_verts : ct.original_verts;

         int[] vert_index = new int[] { 0, triangle_index + 1, triangle_index + 2 };
			for (int i = 0; i < 3; ++i) {
				this.vert[i] = vert_idx_offset + vert_index[i];
				this.normal[i] = vdata[vert_index[i]].Normal;
				this.tex_uv[i] = vdata[vert_index[i]].UV;
			}

			tex_index = ct.tex_index;
			flags = ct.flags;
		}

		public bool IsFlagSet(FaceFlags ff)
		{
			return ((flags & (int)ff) == (int)ff);
		}

		public void ReverseWindingOrder()
		{
			DTriangle flip_tri = new DTriangle(this, 0);
			for (int i = 0; i < 3; i++) {
				vert[i] = flip_tri.vert[2 - i];
				normal[i] = flip_tri.normal[2 - i];
				tex_uv[i] = flip_tri.tex_uv[2 - i];
			}
		}

		public DTriangle(JObject root)
		{
			tex_index = root["tex_index"].GetInt(0);
			flags = root["flags"].GetInt(0);

			JArray j_verts = root["verts"].GetArray();
			JArray j_normals = root["normals"].GetArray();
			JArray j_uvs = root["uvs"].GetArray();
			for (int i = 0; i < 3; i++) {
				vert[i] = j_verts[i].GetInt(0);
				normal[i].X = j_normals[i]["x"].GetFloat(0.0f);
				normal[i].Y = j_normals[i]["y"].GetFloat(0.0f);
				normal[i].Z = j_normals[i]["z"].GetFloat(0.0f);
				tex_uv[i].X = j_uvs[i]["u"].GetFloat(0.0f);
				tex_uv[i].Y = j_uvs[i]["v"].GetFloat(0.0f);
			}
		}

		public void Serialize(JObject root)
		{
			root["tex_index"] = tex_index;
			root["flags"] = flags;

			JArray j_verts = new JArray();
			JArray j_normals = new JArray();
			JArray j_uvs = new JArray();
			for (int i = 0; i < 3; i++) {
				j_verts.Add(vert[i]);

				JObject j_normal = new JObject();
				j_normal["x"] = normal[i].X;
				j_normal["y"] = normal[i].Y;
				j_normal["z"] = normal[i].Z;
				j_normals.Add(j_normal);

				JObject j_uv = new JObject();
				j_uv["u"] = tex_uv[i].X;
				j_uv["v"] = tex_uv[i].Y;
				j_uvs.Add(j_uv);
			}
			root["verts"] = j_verts;
			root["normals"] = j_normals;
			root["uvs"] = j_uvs;
		}
	}
}