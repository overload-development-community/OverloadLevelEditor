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

// GLVIEW - Draw
// All the drawing and setup for drawing is done here

namespace OverloadLevelEditor
{
	public partial class GLView : OpenTK.GLControl
	{
		public static Color C_bg = Color.FromArgb(255, 200, 200, 200);
		public static Color C_grid1 = Color.FromArgb(255, 215, 215, 215);
		public static Color C_grid2 = Color.FromArgb(255, 230, 230, 230);
		public static Color C_grid3 = Color.FromArgb(255, 245, 245, 245);

		public static Color C_base = Color.FromArgb(255, 255, 255, 255);
		public static Color C_base_wire = Color.FromArgb(255, 70, 70, 70);
		public static Color C_text_wire = Color.FromArgb(255, 255, 255, 255);
		public static Color C_vert = Color.FromArgb(255, 100, 80, 20);

		public static Color C_select = Color.FromArgb(255, 255, 255, 255);
		public static Color C_select_main = Color.FromArgb(255, 255, 100, 50);
		
		public static Color C_select_marked = Color.FromArgb(255, 150, 230, 255);
		public static Color C_select_marked_main = Color.FromArgb(255, 250, 70, 200);
		
		public static Color C_marked = Color.FromArgb(255, 50, 220, 255);

		public static Color C_light_soft = Color.FromArgb(255, 200, 200, 200);
		public static Color C_light = Color.FromArgb(255, 225, 225, 225);
		public static Color C_ambient_objects = Color.FromArgb(255, 25, 25, 25);
		public static Color C_ambient_solid = Color.FromArgb(255, 75, 75, 75);
		public static Color C_ambient_texture = Color.FromArgb(255, 255, 255, 255);

		public static Color C_drag_box = Color.FromArgb(192, 50, 220, 255);

		public static Color C_axis_x = Color.FromArgb(128, 255, 55, 55);
		public static Color C_axis_y = Color.FromArgb(128, 55, 255, 55);
		public static Color C_axis_z = Color.FromArgb(128, 55, 55, 255);

		float[] m_light_ambient_texture = { 1.5f, 1.5f, 1.5f, 1f };
		float[] m_light_ambient_solid = { 0.1f, 0.1f, 0.1f, 1f };

		public void UpdateBGColor(BGColor bg)
		{
			switch (bg) {
				default:
					C_bg = Color.FromArgb(255, 200, 200, 200);
					C_grid1 = Color.FromArgb(255, 215, 215, 215);
					C_grid2 = Color.FromArgb(255, 230, 230, 230);
					C_grid3 = Color.FromArgb(255, 245, 245, 245);
					break;
				case BGColor.DARK:
					C_bg = Color.FromArgb(255, 0, 0, 0);
					C_grid1 = Color.FromArgb(255, 15, 15, 15);
					C_grid2 = Color.FromArgb(255, 25, 25, 25);
					C_grid3 = Color.FromArgb(255, 35, 35, 35);
					break;
			}
		}

		public void UpdateClearColor()
		{
			GL.ClearColor(C_bg);
		}

		public void CreateDefaultLight()
		{
			float[] light_pos = { 5f, 25f, 100f, 0f };
			
			GL.Light(LightName.Light0, LightParameter.ConstantAttenuation, 0f);
			GL.Light(LightName.Light0, LightParameter.Position, light_pos);
			GL.Light(LightName.Light0, LightParameter.Diffuse, C_light);
			GL.Light(LightName.Light0, LightParameter.Ambient, C_ambient_objects);

			light_pos[0] = -5f;
			light_pos[1] = 100f;
			light_pos[2] = -25f;

			GL.Light(LightName.Light1, LightParameter.ConstantAttenuation, 0f);
			GL.Light(LightName.Light1, LightParameter.Position, light_pos);
			GL.Light(LightName.Light1, LightParameter.Diffuse, C_light_soft);
			GL.Light(LightName.Light1, LightParameter.Ambient, C_ambient_objects);
		}

		public void DrawMouseDrag()
		{
			GL.MatrixMode(MatrixMode.Projection);
			GL.LoadIdentity();
			GL.Ortho(0, m_control_sz.X, m_control_sz.Y, 0, -999, 999);

			GL.MatrixMode(MatrixMode.Modelview);
			GL.LoadIdentity();
			GL.LineWidth(1f);
			GL.Begin(PrimitiveType.Quads);
			GL.Color4(C_drag_box);
			GL.Vertex2(m_mouse_pos_down.X, m_mouse_pos_down.Y);
			GL.Vertex2(m_mouse_pos.X, m_mouse_pos_down.Y);
			GL.Vertex2(m_mouse_pos.X, m_mouse_pos.Y);
			GL.Vertex2(m_mouse_pos_down.X, m_mouse_pos.Y);
			GL.End();
		}

		public void DrawGrid()
		{
			if (editor.m_grid_display == GridDisplay.ALL || (m_view_type != ViewType.PERSP && editor.m_grid_display == GridDisplay.ORTHO)) {
				GL.PushMatrix();
				GL.LineWidth(1f);
			
				if (m_view_type == ViewType.PERSP) {
					// Grid uses Z buffer and is rotated to be XZ (instead of XY)
					GL.Rotate(-90, Vector3.UnitX);
					GL.Enable(EnableCap.DepthTest);
				}
				GL.Disable(EnableCap.Lighting);

				GL.CallList(GL_GRID);
				GL.PopMatrix();
			}
		}

		public void DrawDMesh()
		{
			GL.PushMatrix();
			GL.Enable(EnableCap.PolygonOffsetFill);
			GL.PolygonOffset(1f, 1f);

			GL.Enable(EnableCap.ColorMaterial);
			GL.ColorMaterial(MaterialFace.Front, ColorMaterialParameter.Diffuse);
			GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
			GL.CullFace(CullFaceMode.Front);
			GL.Enable(EnableCap.CullFace);
			GL.Enable(EnableCap.DepthTest);
			
			switch (m_view_type) {
				case ViewType.FRONT:
					break;
				case ViewType.TOP:
					GL.Rotate(-90, Vector3.UnitX);
					break;
				case ViewType.RIGHT:
					GL.Rotate(90, Vector3.UnitY);
					break;
				case ViewType.PERSP:
					break;
			}

			GL.Enable(EnableCap.Lighting);
			GL.Disable(EnableCap.Light1);
			switch (editor.m_lighting_type) {
				case LightingType.NONE:
					GL.Disable(EnableCap.Lighting);
					break;
				case LightingType.BRIGHT:
					GL.Enable(EnableCap.Lighting);
					GL.Enable(EnableCap.Light0);
					GL.Enable(EnableCap.Light1);
					break;
				case LightingType.DARK:
					GL.Enable(EnableCap.Lighting);
					GL.Enable(EnableCap.Light0);
					GL.Disable(EnableCap.Light1);
					break;
			}

			// Draw DMesh Geometry
			if (DrawTexture()) {
				SetAmbientColor(C_ambient_solid); 
				GL.Enable(EnableCap.Texture2D);
				GL.BindTexture(TextureTarget.Texture2D, editor.tm_decal.m_gl_id[0]);
			} else {
				GL.Disable(EnableCap.Texture2D);
				SetAmbientColor(C_ambient_objects); 
			}

			// Draw base geometry texture/flat
			if (DrawSolid()) {
				SetDiffuseObjectColor(C_base);
				GL.CallList(GL_DECAL);
			}

			GL.PolygonOffset(0f, 0f);
			GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
			GL.Disable(EnableCap.Texture2D);
			GL.Disable(EnableCap.CullFace);
			GL.Disable(EnableCap.Lighting);
			GL.Disable(EnableCap.ColorMaterial);
			GL.LineWidth(1f);

			// Vert normals
			if (editor.m_vert_display == VertDisplay.SHOW) {
				GL.CallList(GL_VERT_NORMALS);
			}

			// Lights
			for (int i = 0; i < DMesh.NUM_LIGHTS; i++) {
				if (editor.m_dmesh.light[i].enabled) {
					DLight dl = editor.m_dmesh.light[i];
					if (i == editor.m_selected_light) {
						GL.Color3(Color.GreenYellow);
					} else {
						GL.Color3(Color.LightYellow);
					}
					GL.PushMatrix();
					GL.Translate(dl.position);
					GL.CallList(GL_LIGHT);
					if (dl.style == LightStyle.SPOT) {
						var light_rotation = dl.rotation;
						GL.MultMatrix(ref light_rotation);
						GL.CallList(GL_LIGHT_CONE);
					}
					GL.PopMatrix();
				}
			}

			// Draw base geometry wireframe
			if (DrawWireframe()) {
				if (DrawTexture()) {
					GL.Color3(C_text_wire);
				} else {
					GL.Color3(C_base_wire);
				}
				GL.CallList(GL_DECAL_OUTLINE);
			}

			// Draw base verts
			if (editor.m_edit_mode == EditMode.VERT) {
				GL.PointSize(4f);
				GL.CallList(GL_VERTS);
			}

			// Draw selected
			GL.Disable(EnableCap.DepthTest);
			GL.LineWidth(3f);
			GL.PointSize(8f);
			GL.CallList(GL_SELECT);

			// Draw marked
			GL.Disable(EnableCap.DepthTest);
			GL.LineWidth(1f);
			GL.PointSize(4f);
			GL.CallList(GL_MARKED);

			
			if (m_view_type == ViewType.PERSP && editor.m_gimbal_display == GimbalDisplay.SHOW) {
				GL.Enable(EnableCap.Blend);
				GL.Disable(EnableCap.DepthTest);
				GL.CallList(GL_GIMBAL);
			}
			if (editor.m_cut_poly_state == 1) {
				GL.Color4(C_grid3);
				GL.Enable(EnableCap.Blend);
				GL.Disable(EnableCap.DepthTest);
				GL.CallList(GL_CUT_EDGE1);
			}

			GL.PopMatrix();
		}

		public void SetDiffuseObjectColor(Color c)
		{
			GL.ColorMaterial(MaterialFace.Front, ColorMaterialParameter.Diffuse);
			GL.Enable(EnableCap.ColorMaterial);
			GL.Color3(c);
		}

		public void SetAmbientColor(Color c)
		{
			GL.ColorMaterial(MaterialFace.Front, ColorMaterialParameter.Ambient);
			GL.Enable(EnableCap.ColorMaterial);
			GL.Color3(c);
		}

		public bool DrawWireframe()
		{
			if (m_view_type == ViewType.PERSP) {
				return (editor.m_view_mode_persp == ViewMode.WIREFRAME || editor.m_view_mode_persp == ViewMode.WIRE_SOLID || editor.m_view_mode_persp == ViewMode.WIRE_TEXTURE);
			} else {
				return (editor.m_view_mode_ortho == ViewMode.WIREFRAME || editor.m_view_mode_ortho == ViewMode.WIRE_SOLID || editor.m_view_mode_ortho == ViewMode.WIRE_TEXTURE);
			}
		}

		public bool DrawSolid()
		{
			if (m_view_type == ViewType.PERSP) {
				return (editor.m_view_mode_persp != ViewMode.WIREFRAME);
			} else {
				return (editor.m_view_mode_ortho != ViewMode.WIREFRAME);
			}
		}

		public bool DrawTexture()
		{
			if (m_view_type == ViewType.PERSP) {
				return (editor.m_view_mode_persp == ViewMode.TEXTURED || editor.m_view_mode_persp == ViewMode.WIRE_TEXTURE);
			} else {
				return (editor.m_view_mode_ortho == ViewMode.TEXTURED || editor.m_view_mode_ortho == ViewMode.WIRE_TEXTURE);
			}
		}
	}
}
