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
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

// GLVIEW - Build
// Lots of the behind-the-scenes stuff for drawing
// Builds all the openGL lists for drawing (see GLViewDraw)

namespace OverloadLevelEditor
{
	public partial class GLView : OpenTK.GLControl
	{
		public int tex_id = -1;

		public const int GL_DECAL_OUTLINE = 49;
		public const int GL_DECAL = 50;
		public const int GL_LIGHT = 51;
		public const int GL_LIGHT_CONE = 52;

		public const int GL_GRID = 101;
		public const int GL_BASE = 102;
		public const int GL_VERTS = 103;
		public const int GL_MARKED = 104;
		public const int GL_SELECT = 105;

		public const int GL_BBOX = 110;
		public const int GL_VERT_NORMALS = 111;

		// Helpers
		public const int GL_CUT_EDGE1 = 130;
		public const int GL_GIMBAL = 131;
		
		public void BuildGridGeometry(int lines, float spacing)
		{
			GL.PushMatrix();
			GL.DeleteLists(GL_GRID, 1);
			GL.NewList(GL_GRID, ListMode.Compile);
			{
				GL.Begin(PrimitiveType.Lines);
				// Wide lines (every 1) up to 8 by 8
				for (int i = -8; i <= 8; i++) {
					GL.Color3(C_grid1);
					GL.Vertex2(i, -8);
					GL.Vertex2(i, 8);
					GL.Vertex2(-8, i);
					GL.Vertex2(8, i);
				}

				for (int i = -lines; i <= lines; i++) {
					if (i == 0) {
						GL.Color3(C_grid3);
					} else if (i % 4 == 0) {
						GL.Color3(C_grid2);
					} else {
						GL.Color3(C_grid1);
					}
					GL.Vertex2(i * spacing, -lines * spacing);
					GL.Vertex2(i * spacing, lines * spacing);
				}
				for (int i = -lines; i <= lines; i++) {
					if (i == 0) {
						GL.Color3(C_grid3);
					} else if (i % 4 == 0) {
						GL.Color3(C_grid2);
					} else {
						GL.Color3(C_grid1);
					}
					GL.Vertex2(-lines * spacing, i * spacing);
					GL.Vertex2(lines * spacing, i * spacing);
				}

				
				GL.End();
			}
			GL.EndList();
			GL.PopMatrix();
		}

		public void BuildDefaults()
		{
			// Helpers
			BuildCutEdge();
			BuildGimbal();
			BuildLight();
			BuildLightCone();
		}

		public const float G_SZ = 1f;
		public Vector3[] m_gimbal_vec = {
													new Vector3(0f, 0f, 0f),
													new Vector3(G_SZ, 0f, 0f),
													new Vector3(0f, G_SZ, 0f),
													new Vector3(0f, 0f, G_SZ),
													new Vector3(1f, 0f, G_SZ - 1f),
													new Vector3(-1f, 0f, G_SZ - 1f),
												};

		public void BuildGimbal()
		{
			GL.PushMatrix();
			GL.DeleteLists(GL_GIMBAL, 1);
			GL.NewList(GL_GIMBAL, ListMode.Compile);
			{
				GL.Begin(PrimitiveType.Lines);
				GL.Color4(C_axis_x);
				CreateLine(m_gimbal_vec[0], m_gimbal_vec[1]);
				GL.Color4(C_axis_y);
				CreateLine(m_gimbal_vec[0], m_gimbal_vec[2]);
				GL.Color4(C_axis_z);
				CreateLine(m_gimbal_vec[0], m_gimbal_vec[3]);
				CreateLine(m_gimbal_vec[3], m_gimbal_vec[4]);
				CreateLine(m_gimbal_vec[3], m_gimbal_vec[5]);

				GL.End();
			}

			GL.EndList();
			GL.PopMatrix();
		}

		public const float LT_SZ = 0.1f;
		public const float LT_MD = 0.025f;
		public const float LT_AR = 0.5f;

		// Color for this is determined while drawing
		public void BuildLight()
		{
			GL.PushMatrix();
			GL.DeleteLists(GL_LIGHT, 1);
			GL.NewList(GL_LIGHT, ListMode.Compile);
			{
				GL.Begin(PrimitiveType.Lines);
				GL.Vertex3(LT_SZ, 0f, 0f);
				GL.Vertex3(-LT_SZ, 0f, 0f);

				GL.Vertex3(0f, LT_SZ, 0f);
				GL.Vertex3(0f, -LT_SZ, 0f);

				GL.Vertex3(0f, 0f, LT_SZ);
				GL.Vertex3(0f, 0f, -LT_SZ);

				GL.Vertex3(LT_MD, LT_MD, LT_MD);
				GL.Vertex3(-LT_MD, -LT_MD, -LT_MD);

				GL.Vertex3(LT_MD, -LT_MD, LT_MD);
				GL.Vertex3(-LT_MD, LT_MD, -LT_MD);

				GL.Vertex3(-LT_MD, LT_MD, LT_MD);
				GL.Vertex3(LT_MD, -LT_MD, -LT_MD);

				GL.Vertex3(LT_MD, LT_MD, -LT_MD);
				GL.Vertex3(-LT_MD, -LT_MD, LT_MD);
				GL.End();
			}
			GL.EndList();
			GL.PopMatrix();
		}

		public void BuildLightCone()
		{
			GL.PushMatrix();
			GL.DeleteLists(GL_LIGHT_CONE, 1);
			GL.NewList(GL_LIGHT_CONE, ListMode.Compile);
			{
				GL.Begin(PrimitiveType.Lines);
				GL.Vertex3(0f, 0f, 0f);
				GL.Vertex3(0f, 0f, LT_AR);

				GL.Vertex3(0f, 0f, LT_AR);
				GL.Vertex3(LT_SZ, 0f, LT_AR - LT_SZ);

				GL.Vertex3(0f, 0f, LT_AR);
				GL.Vertex3(-LT_SZ, 0f, LT_AR - LT_SZ);
				GL.End();
			}
			GL.EndList();
			GL.PopMatrix();
		}

		public void BuildDMeshAll()
		{
			tex_id = -1;
			BuildDecalMesh(editor.m_dmesh);
			BuildDecalMeshOutline(editor.m_dmesh);
			BuildDMeshGeometry();
			BuildMarkedGeometry();
			BuildSelectedGeometry();
			BuildCutEdge();
			BuildVertNormals(editor.m_dmesh);
		}

		public void BuildDMeshGeometry()
		{
			GL.PushMatrix();
			GL.DeleteLists(GL_BASE, 1);
			GL.NewList(GL_BASE, ListMode.Compile);
			tex_id = -1;

			{
				GL.Begin(PrimitiveType.Quads);


				GL.End();
			}

			GL.EndList();

			GL.DeleteLists(GL_VERTS, 1);
			GL.NewList(GL_VERTS, ListMode.Compile);

			{
				GL.Begin(PrimitiveType.Points);
				GL.Color3(C_vert);

				for (int i = 0; i < editor.m_dmesh.vertex.Count; i++) {
					CreateVertexPoint(editor.m_dmesh.vertex[i]);
				}
				GL.End();
			}

			GL.EndList();
			GL.PopMatrix();
		}

		public void BuildMarkedGeometry()
		{
			GL.PushMatrix();
			GL.DeleteLists(GL_MARKED, 1);
			GL.NewList(GL_MARKED, ListMode.Compile);

			{
				for (int i = 0; i < editor.m_dmesh.polygon.Count; i++) {
					DPoly dp = editor.m_dmesh.polygon[i];
					if (dp.marked) {
						GL.Begin(PrimitiveType.Polygon);
						GL.Color3(C_marked);
						CreatePolygon(dp, editor.m_dmesh);
						GL.End();
					}
				}

				// Verts
				GL.Begin(PrimitiveType.Points);
				GL.Color3(C_marked);
				for (int i = 0; i < editor.m_dmesh.vertex.Count; i++) {
					if (editor.m_dmesh.vert_info[i].marked) {
						CreateVertexPoint(editor.m_dmesh.vertex[i]);
					}
				}
				GL.End();
			}

			GL.EndList();
			GL.PopMatrix();
		}


		public void BuildSelectedGeometry()
		{
			GL.PushMatrix();
			GL.DeleteLists(GL_SELECT, 1);
			GL.NewList(GL_SELECT, ListMode.Compile);

			{
				GL.Begin(PrimitiveType.Polygon);
				int idx = editor.m_dmesh.selected_poly;
				if (idx > -1 && idx < editor.m_dmesh.polygon.Count) {
					DPoly dp = editor.m_dmesh.polygon[idx];
					if (editor.m_edit_mode == EditMode.POLY) {
						GL.Color3(dp.marked ? C_select_marked_main : C_select_main);
					} else {
						GL.Color3(dp.marked ? C_select_marked : C_select);
					}
					CreatePolygon(dp, editor.m_dmesh);
				}
				GL.End();

				// Verts
				GL.Begin(PrimitiveType.Points);
				idx = editor.m_dmesh.selected_vert;
				if (idx > -1 && idx < editor.m_dmesh.vertex.Count) {
					Vector3 v = editor.m_dmesh.vertex[idx];
					DVert dv = editor.m_dmesh.vert_info[idx];
					if (editor.m_edit_mode == EditMode.VERT) {
						GL.Color3(dv.marked ? C_select_marked_main : C_select_main);
					} else {
						GL.Color3(dv.marked ? C_select_marked : C_select);
					}
					CreateVertexPoint(v);
				}
				GL.End();
			}

			GL.EndList();
			GL.PopMatrix();
		}

		public void BuildDecalMesh(DMesh dm)
		{
			GL.PushMatrix();
			GL.DeleteLists(GL_DECAL, 1);
			GL.NewList(GL_DECAL, ListMode.Compile);
			int tex_id = -1;

			{
				GL.Color3(Color.White);

				for (int i = 0; i < dm.polygon.Count; i++) {
					if (dm.polygon[i].flags == (int)FaceFlags.NO_COLLIDE) {
						if (editor.m_vis_type == VisibilityType.NO_RENDER || editor.m_vis_type == VisibilityType.NORMAL_ONLY) {
							continue;
						}
						GL.Color3(Color.LightYellow);
					} else if (dm.polygon[i].flags == (int)FaceFlags.NO_RENDER) {
						if (editor.m_vis_type == VisibilityType.NO_COLLIDE || editor.m_vis_type == VisibilityType.NORMAL_ONLY) {
							continue;
						}
						GL.Color3(Color.LightPink);
					} else if (dm.polygon[i].flags == (int)FaceFlags.NO_COLLIDE + (int)FaceFlags.NO_RENDER) {
						// This is a pointless face
						GL.Color3(Color.DarkRed);
					} else {
						GL.Color3(dm.GetPolyColor(dm.polygon[i]));
					}

					if (dm.m_tex_gl_id == null || dm.polygon[i].tex_index >= dm.m_tex_gl_id.Count) {
						// Set nothing, "invalid" texture
						GL.BindTexture(TextureTarget.Texture2D, -1);
					} else if (dm.polygon[i].tex_index < 0 || dm.m_tex_gl_id[dm.polygon[i].tex_index] < 0) {
						GL.BindTexture(TextureTarget.Texture2D, -1);
					} else if (tex_id != dm.m_tex_gl_id[dm.polygon[i].tex_index]) {
						tex_id = dm.m_tex_gl_id[dm.polygon[i].tex_index];
						GL.BindTexture(TextureTarget.Texture2D, tex_id);
					}

					GL.Begin(PrimitiveType.Polygon);
					CreatePolygon(dm.polygon[i], dm);
					GL.End();
				}
			}

			GL.EndList();
			GL.PopMatrix();
		}

		public void BuildDecalMeshOutline(DMesh dm)
		{
			GL.PushMatrix();
			GL.DeleteLists(GL_DECAL_OUTLINE, 1);
			GL.NewList(GL_DECAL_OUTLINE, ListMode.Compile);
			
			{
				for (int i = 0; i < dm.polygon.Count; i++) {
					if (dm.polygon[i].flags == (int)FaceFlags.NO_COLLIDE) {
						if (editor.m_vis_type == VisibilityType.NO_RENDER || editor.m_vis_type == VisibilityType.NORMAL_ONLY) {
							continue;
						}
					} else if (dm.polygon[i].flags == (int)FaceFlags.NO_RENDER) {
						if (editor.m_vis_type == VisibilityType.NO_COLLIDE || editor.m_vis_type == VisibilityType.NORMAL_ONLY) {
							continue;
						}
					}

					GL.Begin(PrimitiveType.Polygon);
					CreatePolygon(dm.polygon[i], dm);
					GL.End();
				}
			}

			GL.EndList();
			GL.PopMatrix();
		}

		public void BuildVertNormals(DMesh dm)
		{
			GL.PushMatrix();
			GL.DeleteLists(GL_VERT_NORMALS, 1);
			GL.NewList(GL_VERT_NORMALS, ListMode.Compile);
			
			{
				GL.Color3(Color.Green);

				for (int i = 0; i < dm.polygon.Count; i++) {
					if (dm.polygon[i].flags == (int)FaceFlags.NO_COLLIDE) {
						if (editor.m_vis_type == VisibilityType.NO_RENDER || editor.m_vis_type == VisibilityType.NORMAL_ONLY) {
							continue;
						}
					} else if (dm.polygon[i].flags == (int)FaceFlags.NO_RENDER) {
						if (editor.m_vis_type == VisibilityType.NO_COLLIDE || editor.m_vis_type == VisibilityType.NORMAL_ONLY) {
							continue;
						}
					} else if (dm.polygon[i].flags == (int)FaceFlags.NO_COLLIDE + (int)FaceFlags.NO_RENDER) {
						// This is a pointless face
					} else {
					}

					GL.Begin(PrimitiveType.Lines);
					CreateVertNormals(dm.polygon[i], dm);
					GL.End();
				}


			}

			GL.EndList();
			GL.PopMatrix();
		}

		public void CreateVertNormals(DPoly poly, DMesh mesh)
		{
			Vector3 v;
			for (int i = 0; i < poly.num_verts; i++) {
				v = mesh.vertex[poly.vert[i]];
				CreateLine(v, v + poly.normal[i] * 0.1f);
			}
		}

		public void CreateTriangle(DTriangle tri, DMesh mesh)
		{
			for (int i = 0; i < 3; i++) {
				GL.TexCoord2(tri.tex_uv[i]);
				GL.Normal3(tri.normal[i]);
				GL.Vertex3(mesh.vertex[tri.vert[i]]);
			}
		}

		public void CreatePolygon(DPoly poly, DMesh mesh)
		{
			for (int i = 0; i < poly.num_verts; i++) {
				GL.TexCoord2(poly.tex_uv[i]);
				GL.Normal3(poly.normal[i]);
				GL.Vertex3(mesh.vertex[poly.vert[i]]);
			}
		}

		public void CreateLinesFromTriangle(DTriangle tri, DMesh mesh)
		{
			GL.Vertex3(mesh.vertex[tri.vert[0]]);
			GL.Vertex3(mesh.vertex[tri.vert[1]]);
			GL.Vertex3(mesh.vertex[tri.vert[1]]);
			GL.Vertex3(mesh.vertex[tri.vert[2]]);
			GL.Vertex3(mesh.vertex[tri.vert[2]]);
			GL.Vertex3(mesh.vertex[tri.vert[0]]);
		}

	

		public Vector3[] m_cut_edge_verts = { new Vector3(0.1f, 0.1f, 0.1f),
														new Vector3(-0.1f, -0.1f, -0.1f),
														new Vector3(-0.1f, 0.1f, 0.1f),
														new Vector3(0.1f, -0.1f, -0.1f),
														new Vector3(0.1f, -0.1f, 0.1f),
														new Vector3(-0.1f, 0.1f, -0.1f),
														};

		public void BuildCutEdge()
		{
			GL.PushMatrix();
			GL.DeleteLists(GL_CUT_EDGE1, 1);
			GL.NewList(GL_CUT_EDGE1, ListMode.Compile);
			{
				GL.Begin(PrimitiveType.Lines);

				CreateLine(Editor.POLYCUT_POS + m_cut_edge_verts[0], Editor.POLYCUT_POS + m_cut_edge_verts[1]);
				CreateLine(Editor.POLYCUT_POS + m_cut_edge_verts[2], Editor.POLYCUT_POS + m_cut_edge_verts[3]);
				CreateLine(Editor.POLYCUT_POS + m_cut_edge_verts[4], Editor.POLYCUT_POS + m_cut_edge_verts[5]);
				
				GL.End();
			}

			GL.EndList();
			GL.PopMatrix();
		}

		public void CreateLine(Vector3 v1, Vector3 v2)
		{
			GL.Vertex3(v1);
			GL.Vertex3(v2);
		}

		public static Vector3[] m_box_verts = { new Vector3(1f, 1f, -1f),
													  new Vector3(1f, -1f, -1f),
													  new Vector3(-1f, -1f, -1f),
													  new Vector3(-1f, 1f, -1f),
													  new Vector3(1f, 1f, 1f),
													  new Vector3(1f, -1f, 1f),
													  new Vector3(-1f, -1f, 1f),
													  new Vector3(-1f, 1f, 1f),
													};

		public void BuildBBox(Vector3 scl)
		{
			GL.PushMatrix();
			GL.DeleteLists(GL_BBOX, 1);
			GL.NewList(GL_BBOX, ListMode.Compile);

			{
				GL.Begin(PrimitiveType.Quads);
				CreateBox(scl.X, scl.Y, scl.Z);
				GL.End();
			}

			GL.EndList();
			GL.PopMatrix();
		}

		public void CreateBox(float x, float y, float z)
		{
			/*Vector3[] quad_verts = new Vector3[4];
			for (int i = 0; i < 6; i++) {
				quad_verts = Utility.SideVertsFromSegVerts(m_box_verts, i);
				for (int j = 0; j < 4; j++) {
					quad_verts[j].X *= x;
					quad_verts[j].Y *= y;
					quad_verts[j].Z *= z;
				}
				CreateQuad(quad_verts[3], quad_verts[2], quad_verts[1], quad_verts[0]);
			}*/
		}

		public void CreateQuad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
		{
			Vector3 normal = Utility.FindNormal(v1, v2, v3);

			GL.Normal3(normal);
			GL.Vertex3(v1);
			GL.Normal3(normal);
			GL.Vertex3(v2);
			GL.Normal3(normal);
			GL.Vertex3(v3);
			GL.Normal3(normal);
			GL.Vertex3(v4);
		}

		public void CreateTriangle(Vector3 v1, Vector3 v2, Vector3 v3)
		{
			Vector3 normal = Utility.FindNormal(v1, v2, v3);

			GL.Normal3(normal);
			GL.Vertex3(v1);
			GL.Normal3(normal);
			GL.Vertex3(v2);
			GL.Normal3(normal);
			GL.Vertex3(v3);
		}

		public void CreateVertexPoint(Vector3 v)
		{
			GL.Vertex3(v);
		}
	}
}
