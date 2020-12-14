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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenTK.Graphics.OpenGL;
using OpenTK;

namespace OverloadLevelEditor
{
	public enum TunnelMode
	{
		TUNNEL,
		ARC,

		NUM,
	}

	public enum ScalingMode
	{
		LINEAR,
		SMOOTH,

		NUM,
	}

	public partial class TunnelBuilder : Form
	{
		public Editor editor;
		public TunnelMode m_tunnel_mode = TunnelMode.TUNNEL;

		public int m_segments = 10;
		public int m_seg_length = 4;
		public int m_arc_segments = 10;
		
		public int m_radius_x = 20;
		public int m_radius_z = 20;

		public int m_slope_x = 0;
		public int m_slope_y = 0;
		public int m_smooth_slope_x = 0;
		public int m_smooth_slope_y = 0;

		public int m_waves_x = 0;
		public int m_waves_y = 0;
		public int m_wave_size_x = 4;
		public int m_wave_size_y = 4;

		public int m_scale_start_x = 100;
		public int m_scale_start_y = 100;
		public int m_scale_end_x = 100;
		public int m_scale_end_y = 100;

		public int m_twist_mid = 0;
		public int m_twist_smooth = 0;
		public int m_twist_linear = 0;

		public float m_noise_x = 0f;
		public float m_noise_y = 0f;
		public float m_noise_z = 0f;
		public int m_noise_smoothing = 1;

		public int m_twists = 0;
		public int m_handle_selected = 20;
		public int m_handle_marked = 20;
		public float m_connect_seg_size = 1f;

		public ScalingMode m_scaling_mode = ScalingMode.LINEAR;

		public TunnelBuilder(Editor e)
		{
			editor = e; 
			InitializeComponent();

			UpdateVisibility();
		}

		public void UpdateVisibility()
		{
			bool on_tunnel = false;
			bool on_arc = false;

			switch (m_tunnel_mode) {
				case TunnelMode.TUNNEL:
					on_tunnel = true;
					break;
				case TunnelMode.ARC:
					on_arc = true;
					break;
			}

			slider_seg_length.Enabled = on_tunnel;
			slider_arc_segments.Enabled = on_arc;

			slider_radius_x.Enabled = on_arc;
			slider_radius_z.Enabled = on_arc;
		}

		public void UpdateLabels()
		{
			label_mode.Text = "Mode: " + m_tunnel_mode.ToString();
			label_scale_type.Text = "Scaling: " + m_scaling_mode.ToString();

			// General
			slider_segments.ValueText = m_segments.ToString();
			slider_seg_length.ValueText = m_seg_length.ToString();
			slider_arc_segments.ValueText = m_arc_segments.ToString();
			slider_radius_x.ValueText = m_radius_x.ToString();
			slider_radius_z.ValueText = m_radius_z.ToString();
			slider_slope_x.ValueText = m_slope_x.ToString();
			slider_slope_y.ValueText = m_slope_y.ToString();
			slider_smooth_slope_x.ValueText = m_smooth_slope_x.ToString();
			slider_smooth_slope_y.ValueText = m_smooth_slope_y.ToString();
			slider_waves_x.ValueText = m_waves_x.ToString();
			slider_waves_y.ValueText = m_waves_y.ToString();
			slider_wave_size_x.ValueText = m_wave_size_x.ToString();
			slider_wave_size_y.ValueText = m_wave_size_y.ToString();
			slider_scale_sx.ValueText = m_scale_start_x.ToString();
			slider_scale_sy.ValueText = m_scale_start_y.ToString();
			slider_scale_ex.ValueText = m_scale_end_x.ToString();
			slider_scale_ey.ValueText = m_scale_end_y.ToString();
			slider_twists.ValueText = m_twists.ToString();
			slider_twist_smooth.ValueText = m_twist_smooth.ToString();
			slider_twist_linear.ValueText = m_twist_linear.ToString();
			slider_noise_x.ValueText = Utility.ConvertFloatTo1Dec(m_noise_x);
			slider_noise_y.ValueText = Utility.ConvertFloatTo1Dec(m_noise_y);
			slider_noise_z.ValueText = Utility.ConvertFloatTo1Dec(m_noise_z);
			slider_noise_smoothing.ValueText = m_noise_smoothing.ToString();

			// Connection tunnel only
			slider_handle_selected.ValueText = m_handle_selected.ToString();
			slider_handle_marked.ValueText = m_handle_marked.ToString();
			slider_twist_mid.ValueText = m_twist_mid.ToString();
			slider_connect_seg_size.ValueText = Utility.ConvertFloatTo1Dec(m_connect_seg_size);
		}

		private void slider_segments_Feedback(object sender, SliderLabelArgs e)
		{
			m_segments = Utility.Clamp(m_segments + e.Increment, 1, 100);
			UpdateLabels();
		}

		private void slider_seg_length_Feedback(object sender, SliderLabelArgs e)
		{
			m_seg_length = Utility.Clamp(m_seg_length + e.Increment, 1, 20);
			UpdateLabels();
		}

		private void slider_arc_segments_Feedback(object sender, SliderLabelArgs e)
		{
			m_arc_segments = Utility.Clamp(m_arc_segments + e.Increment, 1, 100);
			UpdateLabels();
		}

		private void slider_radius_x_Feedback(object sender, SliderLabelArgs e)
		{
			m_radius_x = Utility.Clamp(m_radius_x + e.Increment, 8, 100);
			UpdateLabels();
		}

		private void slider_radius_z_Feedback(object sender, SliderLabelArgs e)
		{
			m_radius_z = Utility.Clamp(m_radius_z + e.Increment, 8, 100);
			UpdateLabels();
		}

		private void slider_slope_x_Feedback(object sender, SliderLabelArgs e)
		{
			m_slope_x = Utility.Clamp(m_slope_x + e.Increment, -20, 20);
			UpdateLabels();
		}

		private void slider_slope_y_Feedback(object sender, SliderLabelArgs e)
		{
			m_slope_y = Utility.Clamp(m_slope_y + e.Increment, -20, 20);
			UpdateLabels();
		}

		private void slider_smooth_slope_x_Feedback(object sender, SliderLabelArgs e)
		{
			m_smooth_slope_x = Utility.Clamp(m_smooth_slope_x + e.Increment, -20, 20);
			UpdateLabels();
		}

		private void slider_smooth_slope_y_Feedback(object sender, SliderLabelArgs e)
		{
			m_smooth_slope_y = Utility.Clamp(m_smooth_slope_y + e.Increment, -20, 20);
			UpdateLabels();
		}

		private void slider_waves_x_Feedback(object sender, SliderLabelArgs e)
		{
			m_waves_x = Utility.Clamp(m_waves_x + e.Increment, -4, 4);
			UpdateLabels();
		}

		private void slider_waves_y_Feedback(object sender, SliderLabelArgs e)
		{
			m_waves_y = Utility.Clamp(m_waves_y + e.Increment, -4, 4);
			UpdateLabels();
		}

		private void slider_wave_size_x_Feedback(object sender, SliderLabelArgs e)
		{
			m_wave_size_x = Utility.Clamp(m_wave_size_x + e.Increment, 1, 20);
			UpdateLabels();
		}

		private void slider_wave_size_y_Feedback(object sender, SliderLabelArgs e)
		{
			m_wave_size_y = Utility.Clamp(m_wave_size_y + e.Increment, 1, 20);
			UpdateLabels();
		}

		private void slider_scale_sx_Feedback(object sender, SliderLabelArgs e)
		{
			m_scale_start_x = Utility.Clamp(m_scale_start_x + e.Increment * 25, 25, 2000);
			UpdateLabels();
		}

		private void slider_scale_sy_Feedback(object sender, SliderLabelArgs e)
		{
			m_scale_start_y = Utility.Clamp(m_scale_start_y + e.Increment * 25, 25, 2000);
			UpdateLabels();
		}

		private void slider_scale_ex_Feedback(object sender, SliderLabelArgs e)
		{
			m_scale_end_x = Utility.Clamp(m_scale_end_x + e.Increment * 25, 25, 2000);
			UpdateLabels();
		}

		private void slider_scale_ey_Feedback(object sender, SliderLabelArgs e)
		{
			m_scale_end_y = Utility.Clamp(m_scale_end_y + e.Increment * 25, 25, 2000);
			UpdateLabels();
		}

		private void slider_twists_Feedback(object sender, SliderLabelArgs e)
		{
			m_twists = Utility.Clamp(m_twists + e.Increment, -4, 4);
			UpdateLabels();
		}

		private void slider_twist_linear_Feedback(object sender, SliderLabelArgs e)
		{
			m_twist_linear = Utility.Clamp(m_twist_linear + e.Increment * 5, -180, 180);
			UpdateLabels();
		}

		private void slider_twist_smooth_Feedback(object sender, SliderLabelArgs e)
		{
			m_twist_smooth = Utility.Clamp(m_twist_smooth + e.Increment * 5, -180, 180);
			UpdateLabels();
		}

		private void slider_twist_mid_Feedback(object sender, SliderLabelArgs e)
		{
			m_twist_mid = Utility.Clamp(m_twist_mid + e.Increment * 5, -90, 90);
			UpdateLabels();
		}

		private void slider_noise_x_Feedback(object sender, SliderLabelArgs e)
		{
			m_noise_x = Utility.SnapValue(Utility.Clamp(m_noise_x + e.Increment * 0.5f, 0f, 5f), 0.5f);
			UpdateLabels();
		}

		private void slider_noise_y_Feedback(object sender, SliderLabelArgs e)
		{
			m_noise_y = Utility.SnapValue(Utility.Clamp(m_noise_y + e.Increment * 0.5f, 0f, 5f), 0.5f);
			UpdateLabels();
		}

		private void slider_noise_z_Feedback(object sender, SliderLabelArgs e)
		{
			m_noise_z = Utility.SnapValue(Utility.Clamp(m_noise_z + e.Increment * 0.5f, 0f, 5f), 0.5f);
			UpdateLabels();
		}

		private void slider_noise_smoothing_Feedback(object sender, SliderLabelArgs e)
		{
			m_noise_smoothing = Utility.Clamp(m_noise_smoothing + e.Increment, 0, 20);
			UpdateLabels();
		}

		private void slider_handle_selected_Feedback(object sender, SliderLabelArgs e)
		{
			m_handle_selected = Utility.Clamp(m_handle_selected + e.Increment * 5, 0, 200);
			UpdateLabels();
		}

		private void slider_handle_marked_Feedback(object sender, SliderLabelArgs e)
		{
			m_handle_marked = Utility.Clamp(m_handle_marked + e.Increment * 5, 0, 200);
			UpdateLabels();
		}

		private void slider_connect_seg_size_Feedback(object sender, SliderLabelArgs e)
		{
			m_connect_seg_size = Utility.SnapValue(Utility.Clamp(m_connect_seg_size + e.Increment * 0.1f, 0.5f, 10f), 0.1f);
			UpdateLabels();
		}

		/*public void GenerateTunnel()
		{
			editor.SaveStateForUndo("Generating tunnel in mode: " + m_tunnel_mode.ToString());

			Side start_side = editor.m_level.GetSelectedSide();
			float total_dist;
			if (m_tunnel_mode == TunnelMode.TUNNEL) {
				total_dist = (float)(m_seg_length * m_segments);
			} else {
				total_dist = (float)(m_seg_length * m_arc_segments);
			}

			editor.m_level.UnTagAllVertices();
			for (int i = 0; i < m_segments; i++) {
				editor.m_level.InsertMarkedSidesMulti(m_seg_length, start_side);
			}

			editor.m_level.JoinSidesWithTaggedVertices();

			// Apply the modifiers
			List<Vertex> vert_list = editor.m_level.GetTaggedVerts();

			Vector3 s_origin = start_side.FindCenter();
			Vector3 s_normal = -start_side.FindNormal();
			Vector3 s_upvec = FindBestUVec(s_normal);
			s_normal.Z *= -1f;
			Matrix4 s_rotate = Matrix4.LookAt(Vector3.Zero, s_normal, s_upvec); 
			Vector3 v_pos;
			Vector3 v_offset;

			// Set up the noise amount (and smooth it)
			Vector3[] noise_vec = new Vector3[vert_list.Count];
			Vector3[] noise_vec_smooth = new Vector3[vert_list.Count];
			for (int i = 0; i < vert_list.Count; i++) {
				noise_vec[i].X = (m_noise_x * Utility.RandomRange(-1f, 1f));
				noise_vec[i].Y = (m_noise_y * Utility.RandomRange(-1f, 1f));
				noise_vec[i].Z = (m_noise_z * Utility.RandomRange(-1f, 1f));
			}
			for (int j = 0; j < m_noise_smoothing; j++) {
				for (int i = 1; i < vert_list.Count - 1; i++) {
					noise_vec_smooth[i] = 0.2f * noise_vec[i] + 0.4f * noise_vec[i - 1] + 0.4f * noise_vec[i + 1];
				}
				for (int i = 0; i < vert_list.Count; i++) {
					noise_vec[i] = noise_vec_smooth[i];
				}
			}

			for (int i = 0; i < vert_list.Count; i++) {
				v_pos = vert_list[i].position - s_origin;
				v_pos = Vector3.Transform(v_pos, Matrix4.Transpose(s_rotate));

				float rel_dist = (v_pos.Z / total_dist);
				if (m_tunnel_mode == TunnelMode.ARC) {
					// Nullify Z for Arcs
					v_pos.Z = 0f;
				}
				// SCALE
				if (m_scaling_mode == ScalingMode.LINEAR) {
					v_pos.X = Utility.Lerp(m_scale_start_x * 0.01f, m_scale_end_x * 0.01f, rel_dist) * v_pos.X;
					v_pos.Y = Utility.Lerp(m_scale_start_y * 0.01f, m_scale_end_y * 0.01f, rel_dist) * v_pos.Y;
				} else {
					v_pos.X = Utility.Hermite(m_scale_start_x * 0.01f, m_scale_end_x * 0.01f, rel_dist) * v_pos.X;
					v_pos.Y = Utility.Hermite(m_scale_start_y * 0.01f, m_scale_end_y * 0.01f, rel_dist) * v_pos.Y;
				}

				// NOISE
				v_pos.X += Utility.Sinerp(0.0f, 1.0f, (1f - Math.Abs(1f - rel_dist * 2f))) * (noise_vec[i].X);
				v_pos.Y += Utility.Sinerp(0.0f, 1.0f, (1f - Math.Abs(1f - rel_dist * 2f))) * (noise_vec[i].Y);
				v_pos.Z += Utility.Sinerp(0.0f, 1.0f, (1f - Math.Abs(1f - rel_dist * 2f))) * (noise_vec[i].Z);

				// TWISTS
				if (m_twist_mid != 0) {
					v_pos = Vector3.Transform(v_pos, Matrix4.CreateRotationZ(Utility.Hermite(0.0f, 1.0f, (1f - Math.Abs(1f - rel_dist * 2f)))
						* (float)m_twist_mid / -180f * Utility.PI));
				}
				if (m_twist_linear != 0) {
					v_pos = Vector3.Transform(v_pos, Matrix4.CreateRotationZ(Utility.Lerp(0f, 1f, rel_dist)
						* (float)m_twist_linear / -180f * Utility.PI));
				}
				if (m_twist_smooth != 0) {
					v_pos = Vector3.Transform(v_pos, Matrix4.CreateRotationZ(Utility.Hermite(0f, 1f, rel_dist)
						* (float)m_twist_smooth / -180f * Utility.PI));
				}

				
				// SLOPE
				v_pos.X += (rel_dist) * m_slope_x;
				v_pos.Y += (rel_dist) * m_slope_y;

				// SMOOTH SLOPE
				v_pos.X += Utility.Hermite(0f, 1f, rel_dist) * m_smooth_slope_x;
				v_pos.Y += Utility.Hermite(0f, 1f, rel_dist) * m_smooth_slope_y;

				// WAVES
				if (m_waves_x != 0) {
					v_pos.X += 0.5f * Utility.Coserp(0.0f, 1.0f, rel_dist * m_waves_x * 2) * (m_waves_x < 0 ? -1 : 1) * m_wave_size_x;
				}
				if (m_waves_y != 0) {
					v_pos.Y += 0.5f * Utility.Coserp(0.0f, 1.0f, rel_dist * m_waves_y * 2) * (m_waves_y < 0 ? -1 : 1) * m_wave_size_y;
				}

				// ARC
				if (m_tunnel_mode == TunnelMode.ARC) {
					// ROTATE AROUND BASE POSITION
					v_pos = Vector3.Transform(v_pos, Matrix4.CreateRotationY(Utility.RAD_360 * rel_dist));

					// OFFSET
					v_offset = Vector3.Transform(Vector3.UnitX, Matrix4.CreateRotationY(-Utility.RAD_360 * rel_dist));
					v_offset.X = (1.0f - v_offset.X) * m_radius_x;
					v_offset.Z *= m_radius_z;

					v_pos += v_offset;
				}
				
				// Assign back to the vert
				v_pos = Vector3.Transform(v_pos, s_rotate);

				vert_list[i].position = v_pos + s_origin;
			}

			// Set texture alignment
			List<Side> side_list = editor.m_level.GetSidesWithTaggedVertices();
			for (int i = 0; i < side_list.Count; i++) {
				side_list[i].DefaultAlignment();
			}

			editor.RefreshGeometry();
		}*/

		/*public void GenerateConnection()
		{
			editor.SaveStateForUndo("Generating tunnel connection");

			if (editor.m_level.num_marked_sides == 1) {
				// Placeholder for connection-style tunnel generation
				// Generally works like GenerateTunnel, except it gets influenced by two sides (marked and selected)
				// - Cannot extrude multiple sides
				// Find the closest pair of verts, then use sorting order (reverse directions) to find other pairs
				Side start_side = editor.m_level.GetMarkedSides()[0];
				Side end_side = editor.m_level.GetSelectedSide();
				
				if (start_side.segment.num != end_side.segment.num) {
					// Find the default verts (closest, then aligned)
					int[] start_verts = new int[4];
					int[] end_verts = new int[4];
					FindClosestVerts(start_side, end_side, start_verts, end_verts);

					GenerateConnectedTunnel(start_side, end_side, start_verts, end_verts);

					editor.RefreshGeometry();
				} else {
					editor.AddOutputText("Marked and selected side must be different in different segments");
				}
			} else {
				editor.AddOutputText("Need to have 1 marked side");
			}
		}*/
		
		Matrix4 GetWorldToSideMatrix(Vector3 side_center_pt_world_space, Vector3 side_forward_vector_world_space, Vector3 side_up_vector_side_space, Vector3 side_right_vector_side_space)
		{
			var rot_matrix = new Matrix4();
			rot_matrix.Column0 = new Vector4(side_right_vector_side_space, 0.0f);
			rot_matrix.Column1 = new Vector4(side_up_vector_side_space, 0.0f);
			rot_matrix.Column2 = new Vector4(side_forward_vector_world_space, 0.0f);
			rot_matrix.Column3 = Vector4.UnitW;

			Matrix4 translate = Matrix4.CreateTranslation(-side_center_pt_world_space);

			return translate * rot_matrix;
		}

		Matrix4 GetLocalToWorldMatrix(Vector3 world_pos, Vector3 forward_vector, Vector3 up_vector)
		{
			Vector3 right_vector = Vector3.Cross(up_vector, forward_vector).Normalized();
			up_vector = Vector3.Cross(forward_vector, right_vector).Normalized();
			
			var rot_matrix = new Matrix4();
			rot_matrix.Column0 = new Vector4(right_vector, 0.0f);
			rot_matrix.Column1 = new Vector4(up_vector, 0.0f);
			rot_matrix.Column2 = new Vector4(forward_vector, 0.0f);
			rot_matrix.Column3 = Vector4.UnitW;
			rot_matrix.Transpose();

			Matrix4 translate = Matrix4.CreateTranslation(world_pos);

			return rot_matrix * translate;
		}

		Quaternion CreateOrientFromAxes(Vector3 r, Vector3 u, Vector3 f)
		{
			var rot_matrix = new Matrix3(r, u, f);
			rot_matrix.Transpose();
			return Quaternion.FromMatrix(rot_matrix);
		}

		/*Quaternion CreateOrientBySideVertex(Side s, Vector3 side_center, Vector3 world_forward_dir, int side_vert_index)
		{
			Vector3 u_vec = (s.level.vertex[s.vert[side_vert_index]].position - side_center).Normalized();
			Vector3 r_vec = Vector3.Cross(u_vec, world_forward_dir).Normalized();
			u_vec = Vector3.Cross(world_forward_dir, r_vec).Normalized();
			return CreateOrientFromAxes(r_vec, u_vec, world_forward_dir);
		}*/

		Quaternion CreateQuaternionFromAxisCosAngle(Vector3 axis, float cos_angle)
		{
			return Quaternion.FromAxisAngle(axis, (float)Math.Acos(cos_angle));
		}

		// Returns a [0,1] value that represents the 1-cos of the
		// angle between the quaternions. A value of 0 means the orientations
		// are the same, while a value of 1 means a 180 degree rotation
		float GetOrientDist(Quaternion a, Quaternion b)
		{
			float dot = (a.X * b.X) + (a.Y * b.Y) + (a.Z * b.Z) + (a.W * b.W);
			if (dot < 0.0f) {
				// don't take the long way on the rotation
				dot = (a.X * -b.X) + (a.Y * -b.Y) + (a.Z * -b.Z) + (a.W * -b.W);
			}

			return 1.0f - dot;
		}

		Quaternion GetTurnQuaternion(Vector3 from, Vector3 to, Vector3 flip_axis)
		{
			var normal_dot = Vector3.Dot( from, to );
			if( normal_dot >= 0.99f ) {
				return Quaternion.Identity;
			}

			if (normal_dot < -0.99f) {
				// 180 degree flip
				Vector3 rotation_axis = flip_axis;
				if( Math.Abs( Vector3.Dot( rotation_axis, from ) ) >= 0.99f ) {
					if( Math.Abs( Vector3.Dot( rotation_axis, Vector3.UnitY ) ) >= 0.99f ) {
						rotation_axis = Vector3.UnitX;
					} else {
						rotation_axis = Vector3.UnitY;
					}
				}

				return CreateQuaternionFromAxisCosAngle( rotation_axis, -1.0f );
			}

			return CreateQuaternionFromAxisCosAngle( Vector3.Cross( from, to ).Normalized(), normal_dot );
		}

		void FindHermiteCurveMovementPlaneInfo(Utility.HermiteCurveEvaluator curve, out Vector3 plane_center_pt, out Vector3 plane_normal)
		{
			const int num_sample_points = 11;
			const float delta = 1.0f / (float)( num_sample_points - 1 );
			const float sample_scale = 1.0f / (float)num_sample_points;

			plane_center_pt = Vector3.Zero;
			for( int i = 0; i < num_sample_points; ++i ) {
				float t = (float)i * delta;
				Vector3 sample_pt = curve.Eval( t );
				plane_center_pt += ( sample_pt * sample_scale );
			}

			Vector3 center_to_start = ( curve.StartPt - plane_center_pt ).Normalized();
			Vector3 center_to_end = ( curve.EndPt - plane_center_pt ).Normalized();
			plane_normal = Vector3.Cross(center_to_end, center_to_start).Normalized();
		}

		Vector3 RotateDirAroundZ(Vector3 from, Vector3 to, float t, bool clockwise)
		{
			// These are the distances along the UnitZ axis that the vectors are
			float from_z_dist = from.Z;
			float to_z_dist = to.Z;
			float curr_z_dist = Utility.Lerp(from_z_dist, to_z_dist, t);

			// Project the from/to vectors onto the Z=0 plane
			Vector3 from_z0_plane = new Vector3(from.X, from.Y, 0.0f);
			Vector3 to_z0_plane = new Vector3(to.X, to.Y, 0.0f);
			from_z0_plane.Normalize();
			to_z0_plane.Normalize();

			// Get the rotation angle on the XY plane for the vectors
			float from_rot = (float)Math.Atan2(from_z0_plane.Y, from_z0_plane.X);
			float to_rot = (float)Math.Atan2(to_z0_plane.Y, to_z0_plane.X);
			float two_pi =  2.0f * (float)Math.PI;
			while( from_rot < 0.0f ) {
				from_rot += two_pi;
			}
			while( to_rot < 0.0f ) {
				to_rot += two_pi;
			}

			// keep consistent rotation direction
			if (clockwise) {
				if (to_rot < from_rot) {
					to_rot += two_pi;
				}
			}
			else {
				if (from_rot < to_rot) {
					from_rot += two_pi;
				}
			}

			float this_rot = Utility.Lerp(from_rot, to_rot, t);

			// Get a unit vector at 0 rotation, but the appropriate Z distance
			Vector3 base_unit_vector = new Vector3(1.0f - (curr_z_dist * curr_z_dist), 0.0f, curr_z_dist);
			var rot_matrix = Matrix4.CreateRotationZ(this_rot);
			Vector3 lerped = Vector3.TransformVector(base_unit_vector, rot_matrix).Normalized();
			return lerped;
		}

		/*public void GenerateConnectedTunnel(Side s1, Side s2, int[] v1, int[] v2)
		{
			var max_segment_length = this.m_connect_seg_size;
			var twist_factor = this.m_twists;

			var start_center_pt = s1.FindCenter();
			var start_normal = -s1.FindNormal();
			var start_velocity_scale = max_segment_length * (float)this.m_handle_marked;
			var start_velocity = start_normal * start_velocity_scale;
			var end_center_pt = s2.FindCenter();
			var end_normal = s2.FindNormal();
			var end_velocity_scale = max_segment_length * (float)this.m_handle_selected;
			var end_velocity = end_normal * end_velocity_scale;
			var center_curve_evaluator = new Utility.HermiteCurveEvaluator(start_center_pt, start_velocity, end_center_pt, end_velocity);

			// Evaluate the curve to find a center that the curve is roughly moving around and a normal to the plane's movement
			// The goal is to always try to keep the up vector orientation with the plane normal, in an attempt to stabilize the tunnel
			Vector3 curve_plane_center;
			Vector3 curve_plane_normal;
			FindHermiteCurveMovementPlaneInfo(center_curve_evaluator, out curve_plane_center, out curve_plane_normal);


			Vector3 start_side_world_up_vector = curve_plane_normal;
			Vector3 start_side_world_right_vector = Vector3.Cross(start_side_world_up_vector, start_normal).Normalized();
			start_side_world_up_vector = Vector3.Cross(start_normal, start_side_world_right_vector).Normalized();

			int end_side_first_vertex;
			{
				// Find which vertex on the end side best corresponds to the first vertex on the start side
				Quaternion start_world_orient = CreateOrientBySideVertex(s1, start_center_pt, start_normal, 0);
				var to_end_orient = GetTurnQuaternion(start_normal, end_normal, curve_plane_normal);
				Quaternion end_world_orient = to_end_orient * start_world_orient;

				float best_corner_dist = 10.0f;
				end_side_first_vertex = 0;
				for (int i = 0; i < 4; ++i) {
					var end_vert_world_orient = CreateOrientBySideVertex(s2, end_center_pt, end_normal, i);
					var orient_dist = GetOrientDist(end_world_orient, end_vert_world_orient);
					if (orient_dist < best_corner_dist) {
						best_corner_dist = orient_dist;
						end_side_first_vertex = i;
					}
				}

				// Offset the chosen vertex by the user-supplied twist factor
				end_side_first_vertex = (end_side_first_vertex + twist_factor);
				while (end_side_first_vertex < 0) {
					end_side_first_vertex += 4;
				}
				end_side_first_vertex &= 3;
			}

			// Note: use a consistent 'up' vector to calculate these basis vectors for the end side so that
			// by the time the curve has reached the end, the orientation is as expected since the up vector
			// and forward vectors will be the same.
			Vector3 end_side_world_up_vector = curve_plane_normal;
			Vector3 end_side_world_right_vector = Vector3.Cross(end_side_world_up_vector, end_normal).Normalized();
			end_side_world_up_vector = Vector3.Cross(end_normal, end_side_world_right_vector).Normalized();

			// Create transforms that go from world space into the start and end side spaces
			Matrix4 world_to_start_side_transform = GetWorldToSideMatrix(start_center_pt, start_normal, start_side_world_up_vector, start_side_world_right_vector);
			Matrix4 world_to_end_side_transform = GetWorldToSideMatrix(end_center_pt, end_normal, end_side_world_up_vector, end_side_world_right_vector);

			// Calculate an orientation per vertex (in side space), along with a distance from
			// side center to vert. We need to do this because the corners have to rotate about
			// the center point to go from start to end
			var start_center_to_corner_dir = new Vector3[4];
			var end_center_to_corner_dir = new Vector3[4];
			var start_center_to_corner_len = new float[4];
			var end_center_to_corner_len = new float[4];
			for (int i = 0; i < 4; ++i) {
				Vector3 start_corner_offset = Vector3.TransformPosition(s1.level.vertex[s1.vert[i]].position, world_to_start_side_transform);
				start_center_to_corner_len[i] = start_corner_offset.Length;
				Vector3 start_corner_dir = start_corner_offset.Normalized();

				// Account for reverse winding - starting from the first vertex
				int winding_index;
				if ((i & 1) != 0) {
					winding_index = 4 - i;
				}
				else {
					winding_index = i;
				}
				int end_corner_index = (winding_index + end_side_first_vertex) & 3;

				Vector3 end_corner_offset = Vector3.TransformPosition(s2.level.vertex[s2.vert[end_corner_index]].position, world_to_end_side_transform);
				end_center_to_corner_len[i] = end_corner_offset.Length;
				Vector3 end_corner_dir = end_corner_offset.Normalized();

				start_center_to_corner_dir[i] = start_corner_dir;
				end_center_to_corner_dir[i] = end_corner_dir;
			}

			// Build up the list of points, along the center curve, to where
			// the segment boundaries are. We want to try to move max_segment_length
			// along the curve, but, we don't want to curve too sharply or the segment
			// will bend back in on itself. So, start from the max distance we can go
			// and then back it off so we don't have too much of a turn in one segment.
			var new_segment_end_pts = new List<float>();
			float last_from_t_center_search = 0.0f;
			Vector3 last_segment_forward_vector = start_normal;
			Vector3 last_segment_center_pos = start_center_pt;
			const float max_turn_angle_degrees = 45.0f;
			float cos_max_turn = (float)Math.Cos(max_turn_angle_degrees * Math.PI / 180.0f);
			float cos_lo_turn = (float)Math.Cos((max_turn_angle_degrees - 1.0f) * Math.PI / 180.0f);
			float cos_hi_turn = (float)Math.Cos((max_turn_angle_degrees + 1.0f) * Math.PI / 180.0f);
			var side_vertex_cache = new List<Vector3>();

			Func<float, float, int, Func<float, Vector3, int>, Tuple<float,Vector3>> binary_search_test_curve = null;
			binary_search_test_curve = (float min_t, float max_t, int iterations_remaining, Func<float, Vector3, int> eval) =>
			{
				float test_t = min_t;
				Vector3 test_pos = Vector3.Zero;
				while (--iterations_remaining >= 0) {
					test_t = (min_t + max_t) * 0.5f;
					test_pos = center_curve_evaluator.Eval(test_t);
					int res = eval(test_t, test_pos);
					if (res < 0) {
						max_t = test_t;
					}
					else if (res > 0) {
						min_t = test_t;
					}
					else {
						break;
					}
				}
				return Tuple.Create(test_t, test_pos);
			};

			Func<float, Vector3, Tuple<uint, Vector3, Vector3, Vector3, Vector3>> calculate_side_corners = null;
			calculate_side_corners = (float t, Vector3 t_pos) =>
												 {
													 // Calculate the orientation for this segment
													 Vector3 segment_side_world_forward_vector = (t_pos - last_segment_center_pos).Normalized();
													 Vector3 segment_side_world_up_vector = curve_plane_normal;
													 Vector3 segment_side_world_right_vector = Vector3.Cross(segment_side_world_up_vector, segment_side_world_forward_vector).Normalized();
													 segment_side_world_up_vector = Vector3.Cross(segment_side_world_forward_vector, segment_side_world_right_vector).Normalized();

													 // Generate the orientation for this new side
													 Matrix4 new_side_to_world = GetLocalToWorldMatrix(t_pos, segment_side_world_forward_vector, segment_side_world_up_vector);

													 uint bad_mask = 0x00;
													 Vector3 res_a = Vector3.Zero;
													 Vector3 res_b = Vector3.Zero;
													 Vector3 res_c = Vector3.Zero;
													 Vector3 res_d = Vector3.Zero;
													 bool twist_direction = this.m_twists >= 0;
													 for (int i = 0; i < 4; ++i) {
														 var start_corner_dir = start_center_to_corner_dir[i];
														 var end_corner_dir = end_center_to_corner_dir[i];
														 Vector3 dir_to_corner_in_side_space = RotateDirAroundZ(start_corner_dir, end_corner_dir, t, twist_direction);

														 Vector3 offset_to_corner_side_space = dir_to_corner_in_side_space * Utility.Lerp(start_center_to_corner_len[i], end_center_to_corner_len[i], t);

														 Vector3 corner_in_world_space = Vector3.TransformPosition(offset_to_corner_side_space, new_side_to_world);

														 // Make sure this corner isn't going behind the last side
														 Vector3 last_center_to_corner = (corner_in_world_space - last_segment_center_pos).Normalized();
														 float dot_with_last_forward = Vector3.Dot(last_center_to_corner, last_segment_forward_vector);
														 if (dot_with_last_forward <= 0.0f) {
															 bad_mask |= (uint)( 1 << i );
														 }

														 switch (i) {
															 case 0:
																 res_a = corner_in_world_space;
																 break;
															 case 1:
																 res_b = corner_in_world_space;
																 break;
															 case 2:
																 res_c = corner_in_world_space;
																 break;
															 case 3:
																 res_d = corner_in_world_space;
																 break;
														 }
													 }

													 return Tuple.Create(bad_mask, res_a, res_b, res_c, res_d);
												 };


			while (last_from_t_center_search < 1.0f) {
				// First move out by the desired distance
				float real_dist;
				Vector3 t_pos;
				float next_t = center_curve_evaluator.CalculateNextTByDistance(last_from_t_center_search, max_segment_length, out real_dist, out t_pos);

				// Now make sure the turn isn't too sharp
				Vector3 new_forward_vector = (t_pos - last_segment_center_pos).Normalized();
				if (Vector3.Dot(new_forward_vector, last_segment_forward_vector) <= cos_max_turn) {
					// Too much of an angle, back it off
					var res = binary_search_test_curve( last_from_t_center_search, next_t, 30, ( t, t_val ) =>
						                                                                           {
							                                                                           var nf = ( t_val - last_segment_center_pos ).Normalized();
							                                                                           float dot = Vector3.Dot( nf, last_segment_forward_vector );
							                                                                           if( dot > cos_lo_turn ) {
								                                                                           // Not strong enough of a turn
								                                                                           return 1;
							                                                                           } else if( dot < cos_hi_turn ) {
								                                                                           // Too strong of a turn
								                                                                           return -1;
							                                                                           } else {
								                                                                           // Good enough...
								                                                                           return 0;
							                                                                           }
						                                                                           } );
					next_t = res.Item1;
					t_pos = res.Item2;
					new_forward_vector = ( t_pos - last_segment_center_pos ).Normalized();
				}

				// Don't create a tiny segment at the end, just an extra large one
				float dist_from_end_pt = (end_center_pt - t_pos).Length;
				if (dist_from_end_pt < (max_segment_length * 0.5f)) {
					// Less than a half segment to go - just finish out the tunnel
					break;
				}

				// Check the corners to make sure they don't go behind the last face
				var corner_check_res = calculate_side_corners(next_t, t_pos);
				if (corner_check_res.Item1 != 0 && new_segment_end_pts.Count > 0) {
					// This side has verts going behind the side we just came from
					// We'll fix this by just snapping those verts
					uint bad_corner_mask = corner_check_res.Item1;
					int last_segment_base_index = (new_segment_end_pts.Count - 1) * 4;
					for (int i = 0; i < 4; ++i) {
						Vector3 corner_pos;
						switch (i) {
							default:
							case 0: corner_pos = corner_check_res.Item2; break;
							case 1: corner_pos = corner_check_res.Item3; break;
							case 2: corner_pos = corner_check_res.Item4; break;
							case 3: corner_pos = corner_check_res.Item5; break;
						}

						if ((bad_corner_mask & (uint)(1 << i)) != 0) {
							// this corner is bad, take from the last face
							corner_pos = side_vertex_cache[last_segment_base_index + i] + (last_segment_forward_vector * 0.1f);
						}

						side_vertex_cache.Add(corner_pos);
					}
				}
				else {
					// Add the new verts directly - they look good
					side_vertex_cache.Add(corner_check_res.Item2);
					side_vertex_cache.Add(corner_check_res.Item3);
					side_vertex_cache.Add(corner_check_res.Item4);
					side_vertex_cache.Add(corner_check_res.Item5);
				}

				// This is a good side
				new_segment_end_pts.Add(next_t);

				// Prepare for the next side
				last_from_t_center_search = next_t;
				last_segment_forward_vector = new_forward_vector;
				last_segment_center_pos = t_pos;
			}

			if (new_segment_end_pts.Count > 0) {
				if (new_segment_end_pts[new_segment_end_pts.Count - 1] < 0.98f) {
					// Add the end of the curve in
					new_segment_end_pts.Add(1.0f);

					var corner_check_res_final = calculate_side_corners(1.0f, center_curve_evaluator.EndPt);
					side_vertex_cache.Add(corner_check_res_final.Item2);
					side_vertex_cache.Add(corner_check_res_final.Item3);
					side_vertex_cache.Add(corner_check_res_final.Item4);
					side_vertex_cache.Add(corner_check_res_final.Item5);
				}
				else {
					// Snap the last point to the end
					new_segment_end_pts[new_segment_end_pts.Count - 1] = 1.0f;
				}
			}

			// Create the new segments that we will eventually move to the curve
			int num_segments_to_create = new_segment_end_pts.Count;
			editor.m_level.UnTagAllVertices();
			List<Tuple<int, int>> last_inserted_segment = null;
			for (int i = 0; i < num_segments_to_create; i++) {
				last_inserted_segment = editor.m_level.InsertMarkedSidesMulti((int)(m_connect_seg_size + 0.5f), s1);
			}
			editor.m_level.JoinSidesWithTaggedVertices();

			// Copy the verts over to the new segments
			List<Vertex> vert_list = editor.m_level.GetTaggedVerts();
			System.Diagnostics.Debug.Assert(vert_list.Count == side_vertex_cache.Count);
			for (int i = 0, end_i = side_vertex_cache.Count; i < end_i; ++i) {
				vert_list[i].position = side_vertex_cache[i];
			}

			// Join the final segment of the curve to the end side
			if (last_inserted_segment != null && last_inserted_segment.Count == 1) {
				int seg_idx = last_inserted_segment[0].Item1;
				int side_idx = last_inserted_segment[0].Item2;
				editor.m_level.ClearAllMarked();
				s2.marked = true;
				editor.m_level.segment[seg_idx].side[side_idx].marked = true;
				editor.m_level.JoinMarkedSides();
			}
		}*/

#if false
		public void GenerateConnectedTunnelOLD(Side s1, Side s2, int[] v1, int[] v2)
		{
			float total_dist;
			if (tunnel_mode == TunnelMode.TUNNEL) {
				total_dist = (float)(seg_length * segments);
			} else {
				total_dist = (float)(seg_length * arc_segments);
			}

			editor.level.UnTagAllVertices();
			for (int i = 0; i < segments; i++) {
				editor.level.InsertMarkedSidesMulti(seg_length, s1);
			}

			editor.level.JoinSidesWithTaggedVertices();

			int vert_num = 0;

			// Apply the modifiers
			List<Vertex> vert_list = editor.level.GetTaggedVerts();

			Vector3 s_origin = s1.FindCenter();
			Vector3 s_normal = -s1.FindNormal();
			Vector3 e_normal_raw = -s2.FindNormal();
			Vector3 s_normal_raw = s_normal;
			Vector3 curve_normal = (s_normal_raw + e_normal_raw).Normalized();
			Vector3 s_upvec = FindBestUVec(s_normal);
			s_normal.Z *= -1f;
			Matrix4 s_rotate = Matrix4.LookAt(Vector3.Zero, s_normal, s_upvec); 
			Vector3 v_pos;
			
			// Calculated per vert
			Vector3 start_pos;
			Vector3 end_pos = Vector3.Zero;
			for (int i = 0; i < vert_list.Count; i++) {
				v_pos = vert_list[i].position - s_origin;
				v_pos = Vector3.Transform(v_pos, Matrix4.Transpose(s_rotate));

				// Find out the relative distance along the tunnel
				float rel_dist = (v_pos.Z / total_dist);

				start_pos = vert_list[i].position - v_pos.Z * s_normal_raw;

				// Hack to find which vert this lines up with originally (so you can find an end position)
				for (int j = 0; j < 4; j++) {
					if ((start_pos - editor.level.vertex[s1.vert[v1[j]]].position).Length < 0.1f) {
						end_pos = editor.level.vertex[s2.vert[v2[j]]].position;
					}
				}
				
				// Set my position to be to difference between the start and end positions
				vert_list[i].position = start_pos * (1.0f - rel_dist) + (end_pos) * rel_dist;
			}
		}
#endif

		/*public void FindClosestVerts(Side s1, Side s2, int[] v1, int[] v2)
		{
			float dist_sq;
			float closest_dist_sq = 99999f;
			int close_v1 = -1;
			int close_v2 = -1;
			// Find the closest pair
			for (int i = 0; i < 4; i++) {
				for (int j = 0; j < 4; j++) {
					dist_sq = (editor.m_level.vertex[s1.vert[i]].position - editor.m_level.vertex[s2.vert[j]].position).LengthSquared;

					if (dist_sq < closest_dist_sq) {
						close_v1 = i;
						close_v2 = j;
						closest_dist_sq = dist_sq;
					}
				}
			}

			// Now get the set of verts
			for (int i = 0; i < 4; i++) {
				v1[i] = (close_v1 + i) % 4;
				v2[i] = (close_v2 - i - m_twists + 8) % 4;
			}
		}*/

		public Vector3 FindBestUVec(Vector3 normal)
		{
			Vector3 temp_uvec;
			Vector3 temp_rvec;
			if (normal.Y > 0.999f) {
				return Vector3.UnitZ;
			} else if (normal.Y < -0.999f) {
				return -Vector3.UnitZ;
			} else {
				temp_uvec = Vector3.UnitY;
				temp_rvec = Vector3.Cross(temp_uvec, normal);
				return Vector3.Cross(normal, temp_rvec);
			}
		}

		private void button_create_Click(object sender, EventArgs e)
		{
			//GenerateTunnel();
		}

		private void label_scale_type_MouseDown(object sender, MouseEventArgs e)
		{
			m_scaling_mode = (ScalingMode)(((int)m_scaling_mode + 1) % (int)ScalingMode.NUM);
			UpdateLabels();
		}

		private void label_builder_mode_MouseDown(object sender, MouseEventArgs e)
		{
			m_tunnel_mode = (TunnelMode)(((int)m_tunnel_mode + 1) % (int)TunnelMode.NUM);
			UpdateVisibility();
			UpdateLabels();
		}

		private void TunnelBuilder_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (e.CloseReason != CloseReason.FormOwnerClosing) {
				this.Hide();
				e.Cancel = true;
			}
		}

		private void button_connection_Click(object sender, EventArgs e)
		{
			//GenerateConnection();
		}

		private void TunnelBuilder_LocationChanged(object sender, EventArgs e)
		{
			if (Visible) {
				editor.m_tunnel_builder_loc = Location;
			}
		}
	}
}
