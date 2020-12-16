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
		public const int GL_GRID = 101;
		public const int GL_BASE = 102;
		public const int GL_VERTS = 103;
		public const int GL_MARKED = 104;
		public const int GL_SELECT = 105;
      public const int GL_CHUNKS = 106;
		public const int GL_GMESH = 107;
		
		// Entity shapes
		public const int GL_ENTITY = 110;
		public const int GL_ENEMY = GL_ENTITY + (int)EntityType.ENEMY;
		public const int GL_PROP = GL_ENTITY + (int)EntityType.PROP;
		public const int GL_ITEM = GL_ENTITY + (int)EntityType.ITEM;
		public const int GL_LIGHT = GL_ENTITY + (int)EntityType.LIGHT;
		public const int GL_LIGHT_DIST = 119;

		// These 3 unused currently (uses boxes)
		public const int GL_DOOR = GL_ENTITY + (int)EntityType.DOOR;
		public const int GL_SCRIPT = GL_ENTITY + (int)EntityType.SCRIPT;
		public const int GL_TRIGGER = GL_ENTITY + (int)EntityType.TRIGGER;
		public const int GL_SPECIAL = GL_ENTITY + (int)EntityType.SPECIAL;
		
		// Entity bounding boxes
		public const int GL_ENTITY_BBOX = 120;
		public const int GL_ENEMY_BBOX = GL_ENTITY_BBOX + (int)EntityType.ENEMY;
		public const int GL_PROP_BBOX = GL_ENTITY_BBOX + (int)EntityType.PROP;
		public const int GL_ITEM_BBOX = GL_ENTITY_BBOX + (int)EntityType.ITEM;
		public const int GL_DOOR_BBOX = GL_ENTITY_BBOX + (int)EntityType.DOOR;
		public const int GL_SCRIPT_BBOX = GL_ENTITY_BBOX + (int)EntityType.SCRIPT;
		public const int GL_TRIGGER_BBOX = GL_ENTITY_BBOX + (int)EntityType.TRIGGER;
		public const int GL_LIGHT_BBOX = GL_ENTITY_BBOX + (int)EntityType.LIGHT;
		public const int GL_SPECIAL_BBOX = GL_ENTITY_BBOX + (int)EntityType.SPECIAL;

		public const int GL_GM_BASE = 200;
		
		// Helpers
		public const int GL_ARROW = 130;
		public const int GL_GIMBAL = 131;
		public const int GL_CLIP_PLANES = 132;

		public void BuildGridGeometry(int lines, int spacing)
		{
			lines = (200 / spacing);
			GL.PushMatrix();
			GL.DeleteLists(GL_GRID, 1);
			GL.NewList(GL_GRID, ListMode.Compile);
			{
				GL.Begin(PrimitiveType.Lines);
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
			BuildDefaultEnemy();
			BuildDefaultProp();
			BuildDefaultItem();
			BuildDefaultLight();
			BuildDefaultLightDist();
			BuildDefaultSpecial();
			for (int i = 0; i < (int)EntityType.NUM; i++) {
				BuildEntityBBox((EntityType)i);
			}

			// Helpers
			BuildArrow();
			BuildGimbal();
		}

		public const float G_SZ = 10f;
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

		public const float CLIP_DIST = 2f;

		public static Color[] C_decal_clip = {
															 Color.FromArgb(255, 255, 150, 50),
															 Color.FromArgb(255, 150, 255, 50),
															 Color.FromArgb(255, 50, 150, 255),
															 Color.FromArgb(255, 50, 255, 150),
														};

		public void BuildClipPlanes()
		{
			if (editor.m_decal_active == -1) {
				return;
			}

			GL.PushMatrix();
			GL.DeleteLists(GL_CLIP_PLANES, 1);
			GL.NewList(GL_CLIP_PLANES, ListMode.Compile);
			{
				GL.Begin(PrimitiveType.Lines);
				if (editor.m_level.selected_segment > -1 && editor.m_level.selected_side > -1) {
					Side s = editor.m_level.segment[editor.m_level.selected_segment].side[editor.m_level.selected_side];
					Decal d = s.decal[editor.m_decal_active];
					for (int i = 0; i < Decal.NUM_EDGES; i++) {
						if (d.clip_normal[i] != Vector3.Zero) {
							GL.Color3(C_decal_clip[i]);
							CreateLine(s.FindEdgeCenter(i), s.FindEdgeCenter(i) + d.clip_normal[i] * CLIP_DIST);
							CreateLine(s.FindEdgeCenterOffset(i, 0.1f), s.FindEdgeCenterOffset(i, 0.1f) + d.clip_normal[i] * CLIP_DIST);
							CreateLine(s.FindEdgeCenterOffset(i, 0.9f), s.FindEdgeCenterOffset(i, 0.9f) + d.clip_normal[i] * CLIP_DIST);
							CreateLine(s.FindEdgeCenterOffset(i, 0.1f) + d.clip_normal[i] * CLIP_DIST, s.FindEdgeCenterOffset(i, 0.9f) + d.clip_normal[i] * CLIP_DIST);
						}
					}
				}

				GL.End();
			}

			GL.EndList();
			GL.PopMatrix();
		}

		public void BuildLevelExceptGMesh()
		{
			tex_id = -1;
			BuildLevelGeometry();
         BuildLevelGeometryChunks();
			BuildMarkedGeometry();
			BuildSelectedGeometry();
			BuildClipPlanes();
		}

		public void DeleteGMesh()
		{
			GL.PushMatrix();
			GL.DeleteLists(GL_GMESH, 1);
			GL.NewList(GL_GMESH, ListMode.Compile);
			GL.EndList();
			GL.PopMatrix();
		}

		public void BuildLevelGMesh()
		{
			GL.PushMatrix();
			GL.DeleteLists(GL_GMESH, 1);
			GL.NewList(GL_GMESH, ListMode.Compile);
			tex_id = -1;
			{
				GL.Begin(PrimitiveType.Quads);

				// This is a dumb way of doing things, but until it's too slow, it should be ok
				// TODO: Add flag to sides that need to be rebuilt, and only rebuild those
				for (int i = 0; i < Level.MAX_SEGMENTS; i++) {
					if (editor.m_level.segment[i].Visible) {
						CreateSegmentGMesh(editor.m_level.segment[i]);
					}
				}
				GL.End();
			}

			GL.EndList();
			GL.PopMatrix();
		}

		public void BuildLevelGeometry()
		{
			GL.PushMatrix();
			GL.DeleteLists(GL_BASE, 1);
			GL.NewList(GL_BASE, ListMode.Compile);
			tex_id = -1;

			{
				GL.Begin(PrimitiveType.Quads);
				
				for (int i = 0; i < Level.MAX_SEGMENTS; i++) {
					if (editor.m_level.segment[i].Visible) {
						CreateSegmentSides(editor.m_level.segment[i]);
					}
				}
				GL.End();
			}

			GL.EndList();

			GL.DeleteLists(GL_VERTS, 1);
			GL.NewList(GL_VERTS, ListMode.Compile);

			{
				GL.Begin(PrimitiveType.Points);
				GL.Color3(C_vert);

				//Determine which vertices visble
				foreach (Vertex v in editor.m_level.vertex) {
					v.m_tag = false;
				}
				foreach (Segment seg in editor.m_level.EnumerateVisibleSegments()) {
					foreach (int vertnum in seg.vert) {
						System.Diagnostics.Debug.Assert(editor.m_level.vertex[vertnum].alive);
						editor.m_level.vertex[vertnum].m_tag = true;
					}
				}

				//Add alive vertices to render list
				for (int i = 0; i < Level.MAX_VERTICES; i++) {
					if (editor.m_level.vertex[i].m_tag) {
						CreateVertexPoint(editor.m_level.vertex[i].position);
					}
				}
				GL.End();
			}

			GL.EndList();
			GL.PopMatrix();
		}

        public void BuildLevelGeometryChunks()
        {
            GL.PushMatrix();
            GL.DeleteLists(GL_CHUNKS, 1);
            GL.NewList(GL_CHUNKS, ListMode.Compile);
            tex_id = -1;

            {
                GL.Begin(PrimitiveType.Quads);

                for (int i = 0; i < Level.MAX_SEGMENTS; i++) {
                    if (editor.m_level.segment[i].Visible) {
                        GL.Color4(editor.m_level.ChunkColor[editor.m_level.segment[i].m_chunk_num]);
                        CreateSegmentSides(editor.m_level.segment[i]);
                    }
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
				GL.Begin(PrimitiveType.Quads);
				GL.Color3(C_marked);
				for (int i = 0; i < Level.MAX_SEGMENTS; i++) {
					if (editor.m_level.segment[i].Visible) {
						CreateSegmentSidesMarked(editor.m_level.segment[i]);
					}
				}
				GL.End();

				// Verts
				GL.Begin(PrimitiveType.Points);
				GL.Color3(C_marked);
				for (int i = 0; i < Level.MAX_VERTICES; i++) {
					if (editor.m_level.vertex[i].alive && editor.m_level.vertex[i].marked) {
						CreateVertexPoint(editor.m_level.vertex[i].position);
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
				GL.Begin(PrimitiveType.Quads);
				CreateSelectedSegment(editor.m_level.selected_segment);

				if (editor.m_level.selected_segment > -1 && editor.m_level.selected_side > -1) {
					Side side = editor.m_level.segment[editor.m_level.selected_segment].side[editor.m_level.selected_side];
					if (editor.m_edit_mode == EditMode.SIDE) {
						GL.Color3(side.marked ? C_select_marked_main : C_select_main);
					} else {
						GL.Color3(side.marked ? C_select_marked : C_select);
					}
					CreateSideQuadsShrunk(side,0.8f);
				}
				GL.End();

				// Verts
				GL.Begin(PrimitiveType.Points);
				if (editor.m_level.selected_vertex > -1) {
					Vertex vertex = editor.m_level.vertex[editor.m_level.selected_vertex];
					if (editor.m_edit_mode == EditMode.VERTEX) {
						GL.Color3(vertex.marked ? C_select_marked_main : C_select_main);
					} else {
						GL.Color3(vertex.marked ? C_select_marked : C_select);
					}
					CreateVertexPoint(vertex.position);
				}
				GL.End();
			}

			GL.EndList();
			GL.PopMatrix();
		}

		public Vector3[] m_arrow_verts = { new Vector3(0.125f, 0f, 1f),
													  new Vector3(-0.125f, 0f, 1f),
													  new Vector3(0f, 0f, 1.125f),
													  new Vector3(0f, 0f, 0f),
													  new Vector3(0f, 0.375f, 0f),
													};

		public void BuildArrow()
		{
			GL.PushMatrix();
			GL.DeleteLists(GL_ARROW, 1);
			GL.NewList(GL_ARROW, ListMode.Compile);
			{
				GL.Begin(PrimitiveType.Lines);

				CreateLine(m_arrow_verts[0], m_arrow_verts[2]);
				CreateLine(m_arrow_verts[1], m_arrow_verts[2]);
				CreateLine(m_arrow_verts[2], m_arrow_verts[3]);
				CreateLine(m_arrow_verts[3], m_arrow_verts[4]);

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

		public Vector3[] enemy_verts = { new Vector3(0.5f, -0.25f, -0.75f),
													  new Vector3(-0.5f, -0.25f, -0.75f),
													  new Vector3(0f, 0.25f, -0.75f),
													  new Vector3(0f, 0.0f, -0.75f),
													  new Vector3(0f, -0.25f, 0.75f),
													};

		public void BuildDefaultEnemy()
		{
			GL.PushMatrix();
			GL.DeleteLists(GL_ENEMY, 1);
			GL.NewList(GL_ENEMY, ListMode.Compile);

			{
				GL.Begin(PrimitiveType.Triangles);
				//GL.Color3(C_enemy);
				CreateTriangle(enemy_verts[3], enemy_verts[0], enemy_verts[4]);
				CreateTriangle(enemy_verts[1], enemy_verts[3], enemy_verts[4]);
				CreateTriangle(enemy_verts[0], enemy_verts[2], enemy_verts[4]);
				CreateTriangle(enemy_verts[2], enemy_verts[1], enemy_verts[4]);
				CreateTriangle(enemy_verts[2], enemy_verts[3], enemy_verts[1]);
				CreateTriangle(enemy_verts[0], enemy_verts[3], enemy_verts[2]);

				GL.End();
			}

			GL.EndList();
			GL.PopMatrix();
		}

		public Vector3[] prop_verts = { new Vector3(0.5f, 0.0f, 0.0f),
													  new Vector3(0.0f, 0.0f, -0.5f),
													  new Vector3(-0.5f, 0.0f, 0.0f),
													  new Vector3(0.0f, 0.0f, 0.5f),
													  new Vector3(0.0f, 0.5f, 0.0f),
													  new Vector3(0.0f, -0.5f, 0.0f),
													};

		public void BuildDefaultProp()
		{
			GL.PushMatrix();
			GL.DeleteLists(GL_PROP, 1);
			GL.NewList(GL_PROP, ListMode.Compile);

			{
				GL.Begin(PrimitiveType.Triangles);
				//GL.Color3(C_prop);
				CreateTriangle(prop_verts[0], prop_verts[1], prop_verts[4]);
				CreateTriangle(prop_verts[1], prop_verts[2], prop_verts[4]);
				CreateTriangle(prop_verts[2], prop_verts[3], prop_verts[4]);
				CreateTriangle(prop_verts[3], prop_verts[0], prop_verts[4]);
				CreateTriangle(prop_verts[1], prop_verts[0], prop_verts[5]);
				CreateTriangle(prop_verts[2], prop_verts[1], prop_verts[5]);
				CreateTriangle(prop_verts[3], prop_verts[2], prop_verts[5]);
				CreateTriangle(prop_verts[0], prop_verts[3], prop_verts[5]);
				
				GL.End();
			}

			GL.EndList();
			GL.PopMatrix();
		}

		public void BuildDefaultLightDist()
		{
			GL.PushMatrix();
			GL.DeleteLists(GL_LIGHT_DIST, 1);
			GL.NewList(GL_LIGHT_DIST, ListMode.Compile);

			{
				float scl = 0.5f;
				GL.Begin(PrimitiveType.Triangles);
				//GL.Color3(C_prop);
				CreateTriangle(prop_verts[0] * scl, prop_verts[1] * scl, prop_verts[4] * scl);
				CreateTriangle(prop_verts[1] * scl, prop_verts[2] * scl, prop_verts[4] * scl);
				CreateTriangle(prop_verts[2] * scl, prop_verts[3] * scl, prop_verts[4] * scl);
				CreateTriangle(prop_verts[3] * scl, prop_verts[0] * scl, prop_verts[4] * scl);
				CreateTriangle(prop_verts[1] * scl, prop_verts[0] * scl, prop_verts[5] * scl);
				CreateTriangle(prop_verts[2] * scl, prop_verts[1] * scl, prop_verts[5] * scl);
				CreateTriangle(prop_verts[3] * scl, prop_verts[2] * scl, prop_verts[5] * scl);
				CreateTriangle(prop_verts[0] * scl, prop_verts[3] * scl, prop_verts[5] * scl);

				GL.End();
			}

			GL.EndList();
			GL.PopMatrix();
		}

		public Vector3[] m_item_verts = { new Vector3(0.5f, -0.25f, -0.5f),
													  new Vector3(-0.5f, -0.25f, -0.5f),
													  new Vector3(-0.5f, -0.25f, 0.5f),
													  new Vector3(0.5f, -0.25f, 0.5f),
													  new Vector3(0.0f, 0.25f, 0.0f),
													};

		public void BuildDefaultItem()
		{
			GL.PushMatrix();
			GL.DeleteLists(GL_ITEM, 1);
			GL.NewList(GL_ITEM, ListMode.Compile);

			{
				GL.Begin(PrimitiveType.Triangles);
				//GL.Color3(C_item);
				CreateTriangle(m_item_verts[1], m_item_verts[0], m_item_verts[2]);
				CreateTriangle(m_item_verts[0], m_item_verts[3], m_item_verts[2]);
				CreateTriangle(m_item_verts[0], m_item_verts[1], m_item_verts[4]);
				CreateTriangle(m_item_verts[1], m_item_verts[2], m_item_verts[4]);
				CreateTriangle(m_item_verts[2], m_item_verts[3], m_item_verts[4]);
				CreateTriangle(m_item_verts[3], m_item_verts[0], m_item_verts[4]);

				GL.End();
			}

			GL.EndList();
			GL.PopMatrix();
		}

		public Vector3[] m_light_verts = { new Vector3(0.5f, 0.0f, 0.25f),
													  new Vector3(0.0f, 0.0f, -0.5f),
													  new Vector3(-0.5f, 0.0f, 0.25f),
													  new Vector3(0.0f, 0.0f, 0.5f),
													  new Vector3(0.0f, 0.5f, 0.25f),
													  new Vector3(0.0f, -0.5f, 0.25f),
													};

		public void BuildDefaultLight()
		{
			GL.PushMatrix();
			GL.DeleteLists(GL_LIGHT, 1);
			GL.NewList(GL_LIGHT, ListMode.Compile);

			{
				GL.Begin(PrimitiveType.Triangles);
				//GL.Color3(C_enemy);
				CreateTriangle(m_light_verts[0], m_light_verts[1], m_light_verts[4]);
				CreateTriangle(m_light_verts[1], m_light_verts[2], m_light_verts[4]);
				CreateTriangle(m_light_verts[2], m_light_verts[3], m_light_verts[4]);
				CreateTriangle(m_light_verts[3], m_light_verts[0], m_light_verts[4]);
				CreateTriangle(m_light_verts[1], m_light_verts[0], m_light_verts[5]);
				CreateTriangle(m_light_verts[2], m_light_verts[1], m_light_verts[5]);
				CreateTriangle(m_light_verts[3], m_light_verts[2], m_light_verts[5]);
				CreateTriangle(m_light_verts[0], m_light_verts[3], m_light_verts[5]);

				GL.End();
			}

			GL.EndList();
			GL.PopMatrix();
		}

		public Vector3[] m_special_verts = { new Vector3(0.5f, -0.5f, -0.25f),
													  new Vector3(0.0f, 0.0f, -0.75f),
													  new Vector3(-0.5f, -0.5f, -0.25f),
													  new Vector3(0.0f, 0.0f, 0.75f),
													  new Vector3(0.0f, 0.5f, -0.5f),
													  new Vector3(0.0f, 0.0f, -0.5f),
													};

		public void BuildDefaultSpecial()
		{
			GL.PushMatrix();
			GL.DeleteLists(GL_SPECIAL, 1);
			GL.NewList(GL_SPECIAL, ListMode.Compile);

			{
				GL.Begin(PrimitiveType.Triangles);
				//GL.Color3(C_special);
				CreateTriangle(m_special_verts[0], m_special_verts[1], m_special_verts[4]);
				CreateTriangle(m_special_verts[1], m_special_verts[2], m_special_verts[4]);
				CreateTriangle(m_special_verts[2], m_special_verts[3], m_special_verts[4]);
				CreateTriangle(m_special_verts[3], m_special_verts[0], m_special_verts[4]);
				CreateTriangle(m_special_verts[1], m_special_verts[0], m_special_verts[5]);
				CreateTriangle(m_special_verts[2], m_special_verts[1], m_special_verts[5]);
				CreateTriangle(m_special_verts[3], m_special_verts[2], m_special_verts[5]);
				CreateTriangle(m_special_verts[0], m_special_verts[3], m_special_verts[5]);

				GL.End();
			}

			GL.EndList();
			GL.PopMatrix();
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

		public void BuildEntityBBox(EntityType et)
		{
			GL.PushMatrix();
			GL.DeleteLists(GL_ENEMY_BBOX + (int)et, 1);
			GL.NewList(GL_ENEMY_BBOX + (int)et, ListMode.Compile);

			{
				Vector3 scl = Utility.GetEntityTypeSize(et);
				GL.Begin(PrimitiveType.Quads);
				CreateBox(scl.X, scl.Y, scl.Z);

				GL.End();
			}

			GL.EndList();
			GL.PopMatrix();
		}

		public void CreateBox(float x, float y, float z)
		{
			Vector3[] quad_verts = new Vector3[4];
			for (int i = 0; i < 6; i++) {
				quad_verts = Utility.SideVertsFromSegVerts(m_box_verts, i);
				for (int j = 0; j < 4; j++) {
					quad_verts[j].X *= x;
					quad_verts[j].Y *= y;
					quad_verts[j].Z *= z;
				}
				CreateQuad(quad_verts[3], quad_verts[2], quad_verts[1], quad_verts[0]);
			}
		}

		public void CreateLineBox(Vector3 v)
		{
			GL.PushMatrix();
			{
				GL.Begin(PrimitiveType.Quads);
				CreateBox(v.X, v.Y, v.Z);
				GL.End();
			}
			GL.PopMatrix();
		}

		const float NUM_DISC_VERTS = 32;

		public void CreateLineSphere(float radius)
		{
			GL.PushMatrix();
			{
				CreateLineDisc(radius, Axis.X);
				CreateLineDisc(radius, Axis.Y);
				CreateLineDisc(radius, Axis.Z);
			}
			GL.PopMatrix();
		}

		public void CreateLineLightingDisc(float radius, float angle)
		{
			GL.PushMatrix();
			GL.Begin(PrimitiveType.Lines);
			float offset_side = (float)Math.Sin(Utility.RAD_1 * angle * 0.5f) * radius;
			float offset_forward = (float)Math.Cos(Utility.RAD_1 * angle * 0.5f) * radius;
			GL.Vertex3(new Vector3(0, 0, 0));
			GL.Vertex3(new Vector3(offset_side, 0, offset_forward));
			GL.Vertex3(new Vector3(0, 0, 0));
			GL.Vertex3(new Vector3(-offset_side, 0, offset_forward));
			GL.Vertex3(new Vector3(0, 0, 0));
			GL.Vertex3(new Vector3(0, offset_side, offset_forward));
			GL.Vertex3(new Vector3(0, 0, 0));
			GL.Vertex3(new Vector3(0, -offset_side, offset_forward));
			GL.End();

			for (int n = 0; n < 2; n++) {
				GL.Begin(PrimitiveType.LineLoop);
				offset_side = (float)Math.Sin(Utility.RAD_1 * angle * 0.5f) * radius;
				offset_forward = (float)Math.Cos(Utility.RAD_1 * angle * 0.5f) * radius;

				Vector3 v = Vector3.Zero;
				float a;
				for (int i = 0; i < NUM_DISC_VERTS; i++) {
					a = (float)i / (float)NUM_DISC_VERTS;
					a *= Utility.RAD_360;
					v.X = (float)Math.Sin(a) * offset_side;
					v.Y = (float)Math.Cos(a) * offset_side;
					v.Z = offset_forward;
					GL.Vertex3(v);
				}
				angle *= 0.5f;
				GL.End();
			}
			GL.PopMatrix();
		}

		public void CreateLinkDiamonds(Entity e)
		{
			for (int i = 0; i < 5; i++) {
				Guid guid = e.GetLinkGUID(i);
            if (guid != Guid.Empty) {
					Entity link_entity = editor.LoadedLevel.FindEntityWithGUID(guid, false);
					if (link_entity == null) {
						continue;
					}
					GL.PushMatrix();
					Matrix4 m4 = e.m_rotation.Inverted();
               GL.MultMatrix(ref m4);
               GL.Translate(link_entity.position - e.position);
					GL.CallList(GL_LIGHT_DIST);
					GL.PopMatrix();
				}
			}
		}

		public void CreateLineDisc(float radius, Axis axis)
		{
			GL.Begin(PrimitiveType.LineLoop);

			Vector3 v = Vector3.Zero;
			float a;
			switch (axis) {
				case Axis.X:
					for (int i = 0; i < NUM_DISC_VERTS; i++) {
						a = (float)i / (float)NUM_DISC_VERTS;
						a *= Utility.RAD_360;
						v.Y = (float)Math.Sin(a) * radius;
						v.Z = (float)Math.Cos(a) * radius;
						GL.Vertex3(v);
					}
					break;
				case Axis.Y:
					for (int i = 0; i < NUM_DISC_VERTS; i++) {
						a = (float)i / (float)NUM_DISC_VERTS;
						a *= Utility.RAD_360;
						v.X = (float)Math.Sin(a) * radius;
						v.Z = (float)Math.Cos(a) * radius;
						GL.Vertex3(v);
					}
					break;
				case Axis.Z:
					for (int i = 0; i < NUM_DISC_VERTS; i++) {
						a = (float)i / (float)NUM_DISC_VERTS;
						a *= Utility.RAD_360;
						v.X = (float)Math.Sin(a) * radius;
						v.Y = (float)Math.Cos(a) * radius;
						GL.Vertex3(v);
					}
					break;
			}
			GL.End();
		}

		public Color GetEntityTypeColor(EntityType et)
		{
			switch (et) {
				default:
				case EntityType.ENEMY: return C_enemy;
				case EntityType.PROP: return C_prop;
				case EntityType.ITEM: return C_item;
				case EntityType.DOOR: return C_door;
				case EntityType.SCRIPT: return C_script;
				case EntityType.TRIGGER: return C_trigger;
			}
		}

		public void BuildDefaultDoor()
		{

		}

		public void BuildDefaultScript()
		{
			
		}

		public void BuildDefaultTrigger()
		{
			
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

		public void CreateSelectedSegment(int seg_idx)
		{
			if (seg_idx > -1) {
				Segment seg = editor.m_level.segment[seg_idx];
				if (editor.m_edit_mode == EditMode.SEGMENT) {
					GL.Color3(seg.marked ? C_select_marked_main : C_select_main);
				} else {
					GL.Color3(seg.marked ? C_select_marked : C_select);
				}
				for (int i = 0; i < Segment.NUM_SIDES; i++) {
					CreateSideQuads(seg.side[i]);
				}
			}
		}

		public void CreateSegmentSidesMarked(Segment seg)
		{
			for (int i = 0; i < Segment.NUM_SIDES; i++) {
				if (seg.marked) {
					CreateSideQuads(seg.side[i]);
				}
				if (seg.side[i].marked) {
					CreateSideQuadsShrunk(seg.side[i],0.8f);
				}

				if ((seg.side[i].chunk_plane_order > -1) && (editor.m_cutter_display == CutterDisplay.SHOW)) {
					GL.Color3(C_marked_for_chunking);
					CreateSideQuadsShrunk(seg.side[i],0.6f);
					GL.Color3(C_marked);   //Restore color
				}
			}
		}

		public void CreateSegmentSides(Segment seg, bool all = false)
		{
			for (int i = 0; i < Segment.NUM_SIDES; i++) {
				if (seg.neighbor[i] < 0 || all) {
					CreateSideQuads(seg.side[i]);
				}
			}
		}

		public void CreateSegmentGMesh(Segment seg)
		{
			for (int i = 0; i < Segment.NUM_SIDES; i++) {
				// Hidden flag used to hide decals on invisible sides (not required)
				for (int j = 0; j < Side.NUM_DECALS; j++) {
					if (seg.side[i].decal[j].gmesh != null) {
						BuildGeneratedMesh(seg.side[i].decal[j].gmesh, seg.num * Segment.NUM_SIDES + i);
					}
				}
			}
		}

		public int tex_id = -1;

		public void CreateSideQuads(Side side)
		{
			if (tex_id != side.m_tex_gl_id) {
				tex_id = side.m_tex_gl_id;
				GL.End();

				GL.BindTexture(TextureTarget.Texture2D, side.m_tex_gl_id);
				GL.Begin(PrimitiveType.Quads);
			}

			Vector3[] v = new Vector3[4];
			for (int i = 0; i < 4; i++) {
				v[i] = editor.m_level.vertex[side.vert[i]].position;
			}

			// Normal is calculated from first 3 verts
			Vector3 normal = Utility.FindNormal(v[0], v[1], v[2]);

			for (int i = 0; i < 4; i++) {
				GL.TexCoord2(side.uv[i]);
				GL.Normal3(normal);
				GL.Vertex3(v[i]);
			}
		}

		public void CreateSideQuadsShrunk(Side side, float scale)
		{
			Vector3[] v = new Vector3[4];
			Vector3 center = Vector3.Zero;
			for (int i = 0; i < 4; i++) {
				v[i] = editor.m_level.vertex[side.vert[i]].position;
				center += v[i];
			}
			center *= 0.25f;

			for (int i = 0; i < 4; i++) {
				v[i] = v[i] * scale + center * (1.0f - scale);
			}

			// Normal is calculated from first 3 verts
			Vector3 normal = Utility.FindNormal(v[0], v[1], v[2]);

			for (int i = 0; i < 4; i++) {
				GL.Normal3(normal);
				GL.Vertex3(v[i]);
			}
		}

		public void CreateVertexPoint(Vector3 v)
		{
			GL.Vertex3(v);
		}

		public void BuildGeneratedMesh(GMesh gm, int idx)
		{
			GL.End();
			GL.Begin(PrimitiveType.Triangles);
			//GL.Color3(Color.White);

			for (int i = 0; i < gm.m_triangle.Count; i++) {
				// Don't draw no_render triangles
				if ((gm.m_triangle[i].flags & (int)FaceFlags.NO_RENDER) == (int)FaceFlags.NO_RENDER) {
					continue;
				}
				if (tex_id != gm.m_tex_gl_id[gm.m_triangle[i].tex_index]) {
					GL.End();
					tex_id = gm.m_tex_gl_id[gm.m_triangle[i].tex_index];
					GL.BindTexture(TextureTarget.Texture2D, tex_id);
					GL.Begin(PrimitiveType.Triangles);
				}

				CreateGMTriangle(gm.m_triangle[i], gm);
			}

			GL.End();
			GL.Begin(PrimitiveType.Quads);
		}

		public void CreateGMTriangle(DTriangle tri, GMesh mesh)
		{
			Vector3 normal = Utility.FindNormal(mesh.m_vertex[tri.vert[0]], mesh.m_vertex[tri.vert[1]], mesh.m_vertex[tri.vert[2]]);
			for (int i = 0; i < 3; i++) {
				GL.TexCoord2(tri.tex_uv[i]);
				//GL.Normal3(tri.normal[i]);
				GL.Normal3(normal);
				GL.Vertex3(mesh.m_vertex[tri.vert[i]]);
			}
		}
	}
}
