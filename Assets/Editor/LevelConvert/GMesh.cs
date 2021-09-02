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
using System.Linq;
using System.Collections.Generic;
using OpenTK;
using OverloadLevelEditor.Clipping;

namespace OverloadLevelEditor
{
	public static class MyExt
	{
		public static string AsString(this Vector3 v)
		{
			return string.Format("[x={0}, y={1}, z={2}]", v.X, v.Y, v.Z);
		}

		public static string AsString( this Vector2 v )
		{
			return string.Format( "[x={0}, y={1}]", v.X, v.Y );
		}
	}

	// Generated (decal) light (output form for level)
	public partial class GLight
	{
		public Vector3 m_position = Vector3.Zero;
		public Matrix4 m_orientation = Matrix4.Identity;
		public LightStyle m_style = LightStyle.POINT;
		public LightFlare m_flare = LightFlare.NONE;
		public int m_color_index = 0;
		public float m_intensity = 1.0f;
		public float m_range = 10.0f;
		public float m_spot_angle = 45.0f;

		public GLight()
		{
		}

		public GLight(GLight other)
		{
			if (other != null) {
				this.Copy(other);
			}
		}

		public void Copy(GLight other)
		{
			this.m_position = other.m_position;
			this.m_orientation = other.m_orientation;
			this.m_style = other.m_style;
			this.m_flare = other.m_flare;
			this.m_color_index = other.m_color_index;
			this.m_intensity = other.m_intensity;
			this.m_range = other.m_range;
			this.m_spot_angle = other.m_spot_angle;
		}
	}

	// Generated (decal) mesh (output form for level)
	public partial class GMesh
	{
		public Vector3 m_base_pos;
		public Vector3 m_base_normal;
		public Vector3 m_base_uvec;
		public Vector3 m_base_rvec;
		public Matrix4 m_base_rot;

		public GClipPlane[] m_clip_plane = new GClipPlane[(int)EdgeOrder.NUM];

		public List<Vector3> m_vertex;
		public List<DTriangle> m_triangle;
		public List<GLight> m_light;
		public List<Vector3> m_color;
		
		public List<string> m_tex_name;

		public GMesh()
		{
			m_vertex = new List<Vector3>();
			m_triangle = new List<DTriangle>();
			m_light = new List<GLight>();
			m_color = new List<Vector3>();
#if OVERLOAD_LEVEL_EDITOR
			m_tex_gl_id = new List<int>();
#endif
			m_tex_name = new List<string>();
		}

		public void Copy(GMesh src)
		{
			m_vertex = new List<Vector3>();
			m_triangle = new List<DTriangle>();
			m_light = new List<GLight>();
			m_color = new List<Vector3>();
#if OVERLOAD_LEVEL_EDITOR
			m_tex_gl_id = new List<int>();
#endif
			m_tex_name = new List<string>();

			for (int i = 0; i < (int)EdgeOrder.NUM; i++) {
				m_clip_plane[i].Copy(src.m_clip_plane[i]);
			}

			for (int i = 0; i < (int)src.m_vertex.Count; i++) {
				m_vertex.Add(src.m_vertex[i]);
			}

			for (int i = 0; i < (int)src.m_triangle.Count; i++) {
				m_triangle.Add(src.m_triangle[i]);
			}

			for (int i = 0; i < (int)src.m_light.Count; i++) {
				m_light.Add(new GLight(src.m_light[i]));
			}

			for (int i = 0; i < (int)src.m_color.Count; i++) {
				var src_c = src.m_color[i];
				m_color.Add(new Vector3(src_c.X, src_c.Y, src_c.Z));
			}

			for (int i = 0; i < (int)src.m_tex_name.Count; i++) {
#if OVERLOAD_LEVEL_EDITOR
				m_tex_gl_id.Add(src.m_tex_gl_id[i]);
#endif
				m_tex_name.Add(src.m_tex_name[i]);
			}
		}

		// For easy conversion from an OBJ
		public void AddVertex(float x, float y, float z)
		{
			m_vertex.Add(new Vector3(x, y, z));
		}

		// Assumes the face has 3 verts
		public void AddFace(int[] vrt_idx, Vector3[] nrml, Vector2[] uv, int tex_idx)
		{
			DTriangle tri = new DTriangle(vrt_idx, nrml, uv, tex_idx);
			m_triangle.Add(tri);
		}

		public GLight AddPointLight(Vector3 pos, LightFlare flare, int color_index, float intensity, float range, bool shadow = true)
		{
			GLight l = new GLight();
			l.m_style = (shadow ? LightStyle.POINT : LightStyle.POINT_NO_SHADOW);
			l.m_flare = flare;
			l.m_color_index = color_index;
			l.m_intensity = intensity;
			l.m_range = range;
			l.m_position = pos;
			this.m_light.Add(l);
			return l;
		}

		public GLight AddSpotLight(Vector3 pos, Matrix4 orient, float spot_angle, LightFlare flare, int color_index, float intensity, float range, bool shadow = true)
		{
			GLight l = new GLight();
			l.m_style = (shadow ? LightStyle.SPOT : LightStyle.SPOT_NO_SHADOW);
			l.m_flare = flare;
			l.m_color_index = color_index;
			l.m_intensity = intensity;
			l.m_range = range;
			l.m_spot_angle = spot_angle;
			l.m_position = pos;
			l.m_orientation = orient;
			this.m_light.Add(l);
			return l;
		}

		public void FindBaseAttributes(Decal d)
		{
			Side s = d.side;
			m_base_normal = s.FindNormal();

			// Center position
			switch (d.align) {
				case DecalAlign.TEXTURE:
					m_base_pos = s.FindUVCenterIn3D();
					break;
				case DecalAlign.CENTER:
					m_base_pos = s.FindCenter();
					break;
				case DecalAlign.EDGE_RIGHT:
				case DecalAlign.EDGE_DOWN:
				case DecalAlign.EDGE_LEFT:
				case DecalAlign.EDGE_UP:
					m_base_pos = s.FindEdgeCenter((int)d.align - (int)DecalAlign.EDGE_RIGHT);
					break;
			}

			m_base_rvec = Vector3.Zero;

			// Up vector
			switch (d.align) {
				case DecalAlign.TEXTURE:
					m_base_uvec = s.FindUVUpVector();
					break;
				case DecalAlign.CENTER:
					m_base_uvec = s.FindBestEdgeDir();
					break;
				case DecalAlign.EDGE_RIGHT:
				case DecalAlign.EDGE_DOWN:
				case DecalAlign.EDGE_LEFT:
				case DecalAlign.EDGE_UP:
					m_base_rvec = -s.FindEdgeDir((int)d.align - (int)DecalAlign.EDGE_RIGHT);
					m_base_uvec = Vector3.Cross(m_base_normal, m_base_rvec);
					break;
			}

			// Find the right vec for cases we haven't already
			if (m_base_rvec == Vector3.Zero) {
				m_base_rvec = Vector3.Cross(m_base_uvec, m_base_normal);
			}

			// Apply the rotation of the decal properties to the Up and Right Vector
			Matrix4 vec_rot = Matrix4.CreateFromAxisAngle(m_base_normal, d.RotationAngle());
			m_base_uvec = Vector3.TransformNormal(m_base_uvec, vec_rot);
			m_base_rvec = Vector3.TransformNormal(m_base_rvec, vec_rot);
			
			// Base rotation
			m_base_rot = Matrix4.Transpose(Matrix4.LookAt(Vector3.Zero, m_base_normal, m_base_uvec));
		}

#if OVERLOAD_LEVEL_EDITOR
		public void SetColors(List<System.Drawing.Color> src_colors)
		{
			this.m_color.Clear();
			for (int i = 0, num = src_colors.Count; i < num; ++i) {
				var src_color = src_colors[i];
				var r = (float)src_color.R / 255.0f;
				var g = (float)src_color.G / 255.0f;
				var b = (float)src_color.B / 255.0f;
				this.m_color.Add(new Vector3(r, g, b));
			}
		}
#else
		public void SetColors( List<Vector3> src_colors )
		{
			this.m_color.Clear();
			for( int i = 0, num = src_colors.Count; i < num; ++i ) {
				this.m_color.Add( src_colors[ i ] );
			}
		}
#endif

		public void GenerateNonClippedGeometry(Decal d, DMesh dm)
		{
			// Limit max repeats based on the size of the face
			// NOTE: This may not the best way to limit repeats, but it works
			FindBaseAttributes( d );
			float max_length = d.side.FindLongestDiagonal();
			int max_repeats = 2 + (int)( max_length / 4f );

			int repeat_u = ( d.repeat_u > 0 ? d.repeat_u : max_repeats );
			int repeat_v = ( d.repeat_v > 0 ? d.repeat_v : max_repeats );

			int offset_u = d.offset_u;
			switch(d.align)
            {
				case DecalAlign.EDGE_RIGHT:
				case DecalAlign.EDGE_DOWN:
				case DecalAlign.EDGE_UP:
				case DecalAlign.EDGE_LEFT:
					// COMPATIBILITY: Due to a bug Offset U was effectively reversed in the original 1.0.1.0 release
					//                For backwards compatibility, retain this behaviour.
					offset_u = -offset_u;
					break;
				default:
					break;
            }

			// Get the implied offset based on the repeats
			int implied_offset_u = CalculateImpiedOffset( d.repeat_u, repeat_u );
			int implied_offset_v = CalculateImpiedOffset( d.repeat_v, repeat_v );

			// Adjust implied_offset_v for EDGE alignment (add 2)
			if( d.IsEdgeAlign() ) {
				implied_offset_v += 2;
			}

			// Temporary DMesh to hold the dm if it has to be mirrored
			DMesh temp_dm = new DMesh( dm.name );
			temp_dm.Copy( dm );
			temp_dm.MaybeFlip( d.mirror );

			// Create repeats of the geometry
			for( int i = 0; i < repeat_u; i++ ) {
				for( int j = 0; j < repeat_v; j++ ) {
					// Before adding, must do WHOLE clipping (can't do it after because we don't have the data then)

					// Create a CQuad that covers the outline of the expected area
					Vector3[] v = CreateGhostQuad( m_base_pos + m_base_rvec * ( implied_offset_u - offset_u + i * 4 ) + m_base_uvec * ( implied_offset_v + d.offset_v + j * 4 ), m_base_rot );
					CTriangle ct = new CTriangle( v );

					bool clip = false;
					// See if that clips against a whole plane, if not add it
					for( int k = 0; k < (int)EdgeOrder.NUM; k++ ) {
						if( m_clip_plane[k].whole ) {
							ClipPlane cp = ClipPlane.CreateFrom3Points( m_clip_plane[k].edge_pos1, m_clip_plane[k].edge_pos2, m_clip_plane[k].edge_pos2 + m_clip_plane[k].normal );
							if( ct.WouldBeClipped( cp ) ) {
								clip = true;
							}
						}
					}

					if( !clip ) {
						Vector3 offset = m_base_rvec * ( implied_offset_u - offset_u + i * 4 ) + m_base_uvec * ( implied_offset_v + d.offset_v + j * 4 );
						Vector3 gcenter = m_base_pos + offset;
						AddGeometry( temp_dm, gcenter );
						AddLights( temp_dm, gcenter );
					}
				}
			}

			// Add the textures
			for( int i = 0; i < dm.tex_name.Count; i++ ) {
				m_tex_name.Add( dm.tex_name[i] );
#if OVERLOAD_LEVEL_EDITOR
				m_tex_gl_id.Add( -1 );
#endif
			}
		}


        /// <summary>
        /// Return true if the mesh has degenerate triangles or other issues
        /// </summary>
        /// <returns></returns>
        public bool CheckAndCleanMeshIssues(bool removeSmallInvalidTriangles, out string issues)
		{
            return DMesh.CheckAndCleanMeshIssues( ref m_triangle, m_vertex, removeSmallInvalidTriangles, out issues );
		}

		// Do any work necessary to optimize and improve the GMesh
		public void OptimizeMesh()
		{
            const float kWeldDistanceMeters = 0.001f;
            const float kWeldDistanceSqMeters = kWeldDistanceMeters * kWeldDistanceMeters;

            // Will remap the vertices of the GMesh to reduce duplicates
			Dictionary<int, int> old_vertex_hash_to_new_vertex_index = new Dictionary<int, int>();
			Dictionary<int, int> old_vertex_index_to_new_vertex_index = new Dictionary<int, int>();
			List<Vector3> welded_verts = new List<Vector3>();
			foreach( int old_vert_index in Enumerable.Range( 0, m_vertex.Count ) ) {
				var v = m_vertex[old_vert_index];
				int hash = GetHash( v, kWeldDistanceMeters );
				int new_vert_index;

				bool is_new_vert = true;
				while( true ) {
					if( !old_vertex_hash_to_new_vertex_index.TryGetValue( hash, out new_vert_index ) ) {
						// New vertex
						break;
					}

					// Existing vertex - as a safety precaution, check to make sure verts that hash to the same
					// are really the same vertex.
					float dist_sq = ( v - welded_verts[new_vert_index] ).LengthSquared;
					if( dist_sq <= kWeldDistanceSqMeters ) {
						// Same vertex
						is_new_vert = false;
						break;
					}

					// Bump the hash value and try again
					++hash;
				}

				if( !is_new_vert ) {
					old_vertex_index_to_new_vertex_index.Add( old_vert_index, new_vert_index );
					continue;
				}

				// Add this vertex to the welded list
				new_vert_index = welded_verts.Count;
				welded_verts.Add( v );
				old_vertex_hash_to_new_vertex_index.Add( hash, new_vert_index );
				old_vertex_index_to_new_vertex_index.Add( old_vert_index, new_vert_index );
			}

			// Remap the triangles
			foreach( var tri in m_triangle ) {
				int[] new_vert_array = tri.vert
					.Select( old_vert_index => old_vertex_index_to_new_vertex_index[old_vert_index] )
					.ToArray();
				tri.vert = new_vert_array;
			}

			// Update the vertex list
			m_vertex = welded_verts;
		}

		public int GetHash( float val, float granularity )
		{
			int val_whole = (int)val;
			double val_remainder;
			if( val < 0.0f ) {
				val_remainder = Math.Ceiling( val ) - val;
			} else {
				val_remainder = val - Math.Floor( val );
			}

			// Don't lose the sign here or we'll have 0.1 and -0.1 map to the same thing
			int remainder_scaled = (int)( val_remainder * Math.Sign( val ) / granularity );

			uint hash_a = ( (uint)( (float)val_whole ).GetHashCode() ) + 0xCAFEC01A;
			uint hash_b = ( (uint)( (float)remainder_scaled ).GetHashCode() ) + 0xDEADBEEF;
			hash_a = (( hash_a >> 7 ) | ( hash_a << 25 )) * 754297;
			hash_b = (( hash_b >> 17 ) | ( hash_b << 15 )) * 60661;
			uint hash_c = ( hash_a >> 9 ) | ( hash_b << 23 );
			uint hash_d = ( hash_b >> 9 ) | ( hash_a << 23 );

			return (int)( ( hash_c ) ^ ( hash_d ) );
		}

		public int GetHash( Vector3 val, float granularity )
		{
			return ( GetHash( val.X, granularity ) * 109567 ) ^
				( GetHash( val.Y, granularity ) * 221849 ) ^
				( GetHash( val.Z, granularity ) * 758053 );
		}

		public void AddGeometry( DMesh dm, Vector3 offset )
		{
			// Currently, this creates the geometry in World space
			Matrix4 full_rotation = Matrix4.Mult( Matrix4.CreateRotationX( -Utility.RAD_90 ), m_base_rot );

			// Build up the transform to use for the normals
			// Note: Invert directions because the uvec was setup for texture V coordinates
			Matrix4 normal_full_rotation = new Matrix4();
			normal_full_rotation.M11 = -m_base_rvec.X;
			normal_full_rotation.M12 = -m_base_rvec.Y;
			normal_full_rotation.M13 = -m_base_rvec.Z;
			normal_full_rotation.M14 = 0.0f;
			normal_full_rotation.M21 = m_base_uvec.X;
			normal_full_rotation.M22 = m_base_uvec.Y;
			normal_full_rotation.M23 = m_base_uvec.Z;
			normal_full_rotation.M24 = 0.0f;
			normal_full_rotation.M31 = -m_base_normal.X;
			normal_full_rotation.M32 = -m_base_normal.Y;
			normal_full_rotation.M33 = -m_base_normal.Z;
			normal_full_rotation.M34 = 0.0f;
			normal_full_rotation.M41 = 0.0f;
			normal_full_rotation.M42 = 0.0f;
			normal_full_rotation.M43 = 0.0f;
			normal_full_rotation.M44 = 1.0f;
			normal_full_rotation = Matrix4.CreateRotationX( -Utility.RAD_90 ) * normal_full_rotation;

			int vert_idx_offset = m_vertex.Count;
			for( int i = 0; i < dm.vertex.Count; i++ ) {
				Vector3 rot_pos = Vector3.Transform( dm.vertex[i], full_rotation );
				m_vertex.Add( rot_pos + offset );
			}

			for( int i = 0; i < dm.triangle.Count; i++ ) {
				DTriangle d_tri = new DTriangle( dm.triangle[i], vert_idx_offset );

				for( int v = 0; v < 3; ++v ) {
					d_tri.normal[v] = OpenTK.Vector3.Transform( d_tri.normal[v], normal_full_rotation );
				}

				m_triangle.Add( d_tri );
			}
		}

		public void AddLights(DMesh dm, Vector3 offset)
		{
			var src_enabled_lights = dm.light.Where(l => l.enabled).ToList();

			if (src_enabled_lights.Count == 0) {
				return;
			}

			// Currently, this creates the geometry in World space
			Matrix4 decal_space_to_segment_space = Matrix4.CreateRotationX(-Utility.RAD_90);
			Matrix4 decal_space_to_world_space = Matrix4.Mult(decal_space_to_segment_space, this.m_base_rot);

			foreach (var src_light in src_enabled_lights) {
				Vector3 world_pos = Vector3.Transform(src_light.position, decal_space_to_world_space) + offset;
				if (src_light.style == LightStyle.POINT) {
					AddPointLight(world_pos, src_light.flare, src_light.color_index, src_light.intensity, src_light.range);
				} else if (src_light.style == LightStyle.POINT_NO_SHADOW) {
					AddPointLight(world_pos, src_light.flare, src_light.color_index, src_light.intensity, src_light.range, false);
				} else if (src_light.style == LightStyle.SPOT || src_light.style == LightStyle.SPOT_NO_SHADOW) {
					Matrix4 world_orient = src_light.rotation * decal_space_to_world_space;
					AddSpotLight(world_pos, world_orient, src_light.angle, src_light.flare, src_light.color_index, src_light.intensity, src_light.range, src_light.style == LightStyle.SPOT);
				}
			}
		}

		public Vector3[] CreateGhostQuad(Vector3 offset, Matrix4 rot)
		{
			Vector3[] v = new Vector3[4];
			v[0] = new Vector3(2f, 0f, 2f);
			v[1] = new Vector3(2f, 0f, -2f);
			v[2] = new Vector3(-2f, 0f, -2f);
			v[3] = new Vector3(-2f, 0f, 2f);

			for (int i = 0; i < v.Length; i++) {
				v[i] = Vector3.Transform(v[i], rot);
				v[i] += offset;
			}

			return v;
		}

		public int CalculateImpiedOffset(int set_repeat, int final_repeat)
		{
			if (set_repeat > 0) {
				// Even and Odd numbers lead to different alignments
				return (set_repeat - 1) * -2;
			} else {
				// This forces even number to still be aligned the same as odds, but could lead to weirdness
				return ((final_repeat - 1) / 2) * -4;
			}
		}

		public void ClipGeometry()
		{
			GMesh temp_gm = null;

			// Save the light data before it gets lost during clipping
			var light_storage = new List<GLight>();
			for (int i = 0, num_lights = this.m_light.Count; i < num_lights; ++i) {
				var light_copy = new GLight(this.m_light[i]);
				light_storage.Add(light_copy);
			}
			this.m_light.Clear();

			for (int i = 0; i < (int)EdgeOrder.NUM; i++) {
				if (m_clip_plane[i].visible) {
					temp_gm = ClipGeometryToPlane(m_clip_plane[i]);
               Copy(temp_gm);
				}
			}

			// Restore the light data
			// TODO(Jeff): What if the light should be clipped away?
			for (int i = 0, num_lights = light_storage.Count; i < num_lights; ++i) {
				this.m_light.Add(light_storage[i]);
			}
		}

		public GMesh ClipGeometryToPlane(GClipPlane gcp)
		{
			GMesh clip_gm = new GMesh();

			// Add the textures first
			for (int i = 0; i < m_tex_name.Count; i++) {
#if OVERLOAD_LEVEL_EDITOR
				clip_gm.AddTexture(m_tex_gl_id[i], m_tex_name[i]);
#else
				clip_gm.m_tex_name.Add(m_tex_name[i]);
#endif
			}

			// Copy the clip planes
			for (int i = 0; i < (int)EdgeOrder.NUM; i++) {
				clip_gm.m_clip_plane[i] = new GClipPlane(m_clip_plane[i]);
			}

			ClipPlane cp = ClipPlane.CreateFrom3Points(gcp.edge_pos1, gcp.edge_pos2, gcp.edge_pos2 + gcp.normal);
			for (int i = 0; i < m_triangle.Count; i++) {
				CTriangle ct = new CTriangle(m_triangle[i], m_vertex[m_triangle[i].vert[0]], m_vertex[m_triangle[i].vert[1]], m_vertex[m_triangle[i].vert[2]]);
				ct.ClipFacePlane(cp);

				if (ct.clipped_verts.Length < 3) {
					continue;
				}

				int num_triangles = ct.clipped_verts.Length - 2;
				for (int clipped_tri_idx = 0; clipped_tri_idx < num_triangles; ++clipped_tri_idx) {
					CTriangle extracted_ct = ct.ExtractClippedTriangle(clipped_tri_idx);

					if (extracted_ct.HasDegenerateTriangles(false)) {
						continue;
					}

					clip_gm.AddFaceFromCTriangle(extracted_ct);
				}
			}
			
			return clip_gm;
		}

		public void AddFaceFromCTriangle(CTriangle ct)
		{
			int start_vrt = m_vertex.Count;
			for (int i = 0; i < 3; i++) {
				m_vertex.Add(ct.original_verts[i].Position);
			}

			bool use_clipped_data = false;
			DTriangle tri = new DTriangle(ct, start_vrt, 0, use_clipped_data);
			m_triangle.Add(tri);
		}
	}

	public class GClipPlane
	{
		public bool visible = false;
		public bool whole = false;
		public Vector3 edge_pos1;
		public Vector3 edge_pos2;
		public Vector3 normal;

		public GClipPlane(bool vis, Vector3 ep1, Vector3 ep2, Vector3 nrml, bool whl)
		{
			visible = vis;
			edge_pos1 = ep1;
			edge_pos2 = ep2;
			normal = nrml;
			whole = whl;
		}

		public GClipPlane(Decal d, int idx)
		{
			visible = d.IsClipActive(idx);
			edge_pos1 = d.side.FindEdgeCenterOffset(idx, 0.25f);
			edge_pos2 = d.side.FindEdgeCenterOffset(idx, 0.75f);
			normal = d.clip_normal[idx];
			whole = (d.clip[idx] == DecalClip.WHOLE);
		}

		public GClipPlane(GClipPlane src)
		{
			Copy(src);
		}

		public void Copy(GClipPlane src)
		{
			visible = src.visible;
			edge_pos1 = src.edge_pos1;
			edge_pos2 = src.edge_pos2;
			normal = src.normal;
			whole = src.whole;
		}
	}
}