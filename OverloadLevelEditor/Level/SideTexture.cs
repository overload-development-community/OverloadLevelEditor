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
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OverloadLevelEditor;

// SIDE - Texture
// Texture-specific functions related to sides

namespace OverloadLevelEditor
{
	public partial class Side
	{
		public void PlanarMap(Axis axis, float scl, Vector3 offset)
		{
			Vector3 v;
			for (int i = 0; i < (int)NUM_VERTS; i++) {
				v = level.vertex[vert[i]].position;
				switch (axis) {
					case Axis.X:
						uv[i] = (v.Yz - offset.Yz) * scl;
						break;
					case Axis.Y:
						uv[i] = (v.Xz - offset.Xz) * scl;
						break;
					case Axis.Z:
						uv[i] = (v.Xy - offset.Xy) * scl;
						break;
				}
			}
		}

		public void BoxMap(float scl, Vector3 offset)
		{
			// Choose the planar direction based on the most significant part of the normal
			Vector3 norm = FindNormal();
			if (Math.Abs(norm.X) >= Math.Abs(norm.Y) && Math.Abs(norm.X) >= Math.Abs(norm.Z)) {
				PlanarMap(Axis.X, scl, offset);
			} else if (Math.Abs(norm.Y) >= Math.Abs(norm.X) && Math.Abs(norm.Y) >= Math.Abs(norm.Z)) {
				PlanarMap(Axis.Y, scl, offset);
			} else {
				PlanarMap(Axis.Z, scl, offset);
			}
		}

		public void UVSnapToFraction(int whole)
		{
			float snap = 1f / (float)(Math.Max(1, whole));

			for (int i = 0; i < NUM_VERTS; i++) {
				uv[i] = Utility.SnapValue(uv[i], snap);
			}
		}

		public void DefaultAlignment()
		{
			// Get matrix to rotate face to align with XY plane (first rotate to be vertical, then to face XY)
			// Planar Map with vert0 as 0,0
			Vector3[] v = new Vector3[NUM_VERTS];

			// Offset all the verts by vert2 to get a better frame of reference
			for (int i = 0; i < NUM_VERTS; i++) {
				v[i] = level.vertex[vert[i]].position - level.vertex[vert[Utility.TopLeftVertForSide(num)]].position;
			}
			Vector3 normal = FindNormal();

			v = GetXYAlignedPositions(v, normal);

			// Assign the UVs based on the XY positions
			for (int i = 0; i < NUM_VERTS; i++) {
				uv[i] = v[i].Xy * -Level.PLANAR_SCL;
			}
		}

		public Vector3 FindBestTextureUVec(Vector3 normal)
		{
			Vector3 temp_uvec;
			Vector3 temp_rvec;
			if (normal.Y > 0.999f) {
				return Vector3.UnitZ;
			} else if (normal.Y < -0.999f) {
				return -Vector3.UnitZ;
			} else if (normal.Y < -0.001f) {
				temp_uvec = -Vector3.UnitY;
				temp_rvec = Vector3.Cross(temp_uvec, normal);
				return Vector3.Cross(normal, temp_rvec);
			} else {
				temp_uvec = Vector3.UnitY;
				temp_rvec = Vector3.Cross(temp_uvec, normal);
				return Vector3.Cross(normal, temp_rvec);
			}
		}

		public Vector3[] GetXYAlignedPositions(Vector3[] v, Vector3 normal)
		{
			Matrix4 rot_mat;
			if (Math.Abs(normal.Y) < 0.999f) {
				Vector3 base_uvec = FindBestTextureUVec(normal);
				Vector3 base_rvec = Vector3.Cross(base_uvec, normal);
				Matrix4 base_rot = (Matrix4.LookAt(Vector3.Zero, normal, base_uvec));
				for (int i = 0; i < v.Length; i++) {
					v[i] = Vector3.Transform(v[i], base_rot);
					v[i].X *= -1f;
				}

				return v;
			} else {
				// Just need to rotate the verts to be vertical
				rot_mat = Matrix4.CreateFromAxisAngle(Vector3.UnitX, Utility.RAD_90 * normal.Y);

				// Rotate the verts
				for (int i = 0; i < v.Length; i++) {
					v[i] = Vector3.Transform(v[i], rot_mat);
				}
			}

			return v;
		}

		// Assumes the passed in side also uses default mapping
		public void UVAlignToSide(Side s)
		{
			// First set the default mapping
			DefaultAlignment();

			// Find matching pair of verts
			int v1 = -1;
			int v2 = -1;
			int other_v1 = -1;
			int other_v2 = -1;
			for (int i = 0; i < NUM_VERTS; i++) {
				for (int j = 0; j < NUM_VERTS; j++) {
					if (vert[i] == s.vert[j]) {
						if (v1 < 0) {
							v1 = i;
							other_v1 = j;
						} else {
							v2 = i;
							other_v2 = j;
						}
					}
				}
			}

			// Make sure the verts are valid
			if (v2 < 0) {
				Utility.DebugLog("Tried to align UVs on two sides that don't seem to have two matching verts");
				return;
			}

			// Double check the first one in the current side
			if (v1 == 0 && v2 == 3) {
				v1 = 3;
				v2 = 0;
				int tmp = other_v1;
				other_v1 = other_v2;
				other_v2 = tmp;
			}

			// Find the angle difference between my UV edge and the other side, and rotate the UVs to compensate
			Vector2 uv_edge = (uv[v2] - uv[v1]).Normalized();
			Vector2 uv_edge_other = (s.uv[other_v2] - s.uv[other_v1]).Normalized();
			Vector2 uv_first = uv[v1];
			Vector2 uv_first_other = s.uv[other_v1];
			float angle = (float)Math.Atan2(uv_edge.Y, uv_edge.X) - (float)Math.Atan2(uv_edge_other.Y, uv_edge_other.X); //(float)Math.Atan2(uv_edge.Y - uv_edge_other.Y, uv_edge.X - uv_edge_other.X);

			// Offset the UVs by the other side's UV
			for (int i = 0; i < NUM_VERTS; i++) {
				uv[i] = uv[i] - uv_first + uv_first_other;
			}

			// Updat first UV
			uv_first = uv[v1];

			for (int i = 0; i < NUM_VERTS; i++) {
				uv[i] = Utility.Vector2Rotate(uv[i] - uv_first, -angle) + uv_first;
			}
		}

		public Vector2 FindUVCenter()
		{
			Vector2 center = Vector2.Zero;
			for (int i = 0; i < NUM_VERTS; i++) {
				center += uv[i];
			}

			return center / 4f;
		}

		public void CenterU()
		{
			Vector2 center = FindUVCenter();
			for (int i = 0; i < NUM_VERTS; i++) {
				uv[i].X -= center.X;
			}
		}

		public void CenterV()
		{
			Vector2 center = FindUVCenter();
			for (int i = 0; i < NUM_VERTS; i++) {
				uv[i].Y -= center.Y;
			}
		}
	}
}