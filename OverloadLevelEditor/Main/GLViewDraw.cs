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
	public class GLTextItem
	{
		public GLTextItem(Vector3 pos, string text, Color color, bool screen_space = true)
		{
			this.Text = text;
			this.TextColor = color;
			this.Alpha = 1.0f;
			this.PointSize = 24.0f;
			this.IsScreenSpace = screen_space;
			this.Scale = new Vector3( 1.0f, 1.0f, 1.0f );
			this.WorldPosition = pos;
			this.WorldRotation = Matrix4.Identity;
			this.DepthTestEnable = false;
			this.Facing = false;
			this.Align = screen_space ? Alignment.TopLeft : Alignment.Center;
		}

		public string Text
		{
			get;
			set;
		}

		public Color TextColor
		{
			get;
			set;
		}

		public float Alpha
		{
			get;
			set;
		}

		public bool DepthTestEnable
		{
			get;
			set;
		}

		public float PointSize
		{
			get;
			set;
		}

		public bool Facing
		{
			get;
			set;
		}

		public float[] AlignFactor = { 0.0f, 0.5f, 1.0f };

		public enum Alignment
		{
			//Values:  0,1,2 : X left, center, right; 0,4,8: Y bottom, center, top
			Center = 4+1,
			BottomLeft = 0+0,
			CenterLeft = 4+0,
			TopLeft = 8+0,
			TopCenter = 8+1,
			TopRight = 8+2,
			CenterRight = 4+2,
			BottomRight = 0+2,
			BottomCenter = 0+1
		}

		public Alignment Align
		{
			get;
			set;
		}

		public bool IsScreenSpace
		{
			get;
			set;
		}

		public Vector3 Scale
		{
			get;
			set;
		}

		public Vector3 WorldPosition
		{
			get;
			set;
		}

		public Matrix4 WorldRotation
		{
			get;
			set;
		}
	}

	public partial class GLView : OpenTK.GLControl
	{
		public static Color C_bg = Color.FromArgb(255, 200, 200, 200);
		public static Color C_grid1 = Color.FromArgb(255, 215, 215, 215);
		public static Color C_grid2 = Color.FromArgb(255, 230, 230, 230);
		public static Color C_grid3 = Color.FromArgb(255, 245, 245, 245);

		public static Color C_base = Color.FromArgb(255, 255, 255, 255);
		public static Color C_base_wire = Color.FromArgb(255, 120, 120, 120);
		public static Color C_vert = Color.FromArgb(255, 100, 80, 20);

		public static Color C_select = Color.FromArgb(255, 255, 255, 255);
		public static Color C_select_main = Color.FromArgb(255, 255, 100, 50);
		
		public static Color C_select_marked = Color.FromArgb(255, 150, 230, 255);
		public static Color C_select_marked_main = Color.FromArgb(255, 250, 70, 200);
		
		public static Color C_marked = Color.FromArgb(255, 50, 220, 255);
		public static Color C_marked_for_chunking = Color.FromArgb(255, 255, 50, 50);

		public static Color C_light = Color.FromArgb(255, 255, 250, 240);
		public static Color C_ambient_objects = Color.FromArgb(255, 25, 25, 25);
		public static Color C_ambient_solid = Color.FromArgb(255, 75, 75, 75);
		public static Color C_ambient_texture = Color.FromArgb(255, 255, 255, 255);

		public static Color C_drag_box = Color.FromArgb(192, 50, 220, 255);

		public static Color C_axis_x = Color.FromArgb(128, 255, 55, 55);
		public static Color C_axis_y = Color.FromArgb(128, 55, 255, 55);
		public static Color C_axis_z = Color.FromArgb(128, 55, 55, 255);

		public static Color C_enemy = Color.FromArgb(255, 200, 120, 100);
		public static Color C_prop = Color.FromArgb(255, 150, 180, 120);
		public static Color C_item = Color.FromArgb(255, 120, 200, 120);
		public static Color C_door = Color.FromArgb(255, 200, 200, 60);
		public static Color C_script = Color.FromArgb(255, 120, 120, 180);
		public static Color C_trigger = Color.FromArgb(255, 60, 200, 200);
		public static Color C_special = Color.FromArgb(255, 20, 220, 120);

		float[] m_light_ambient_texture = { 1.5f, 1.5f, 1.5f, 1f };
		float[] m_light_ambient_solid = { 0.1f, 0.1f, 0.1f, 1f };
		HashSet<GLTextItem> m_text_items = new HashSet<GLTextItem>();

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

		public void AddTextItem( GLTextItem item )
		{
			m_text_items.Add( item );
		}

		public void RemoveTextItem( GLTextItem item )
		{
			m_text_items.Remove( item );
		}

		public void RemoveAllTextItems()
		{
			m_text_items.Clear();
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
			GL.Light(LightName.Light1, LightParameter.Diffuse, C_light);
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
			if (Math.Sign(m_mouse_pos.X - m_mouse_pos_down.X) == Math.Sign(m_mouse_pos.Y - m_mouse_pos_down.Y)) {
				GL.Vertex2(m_mouse_pos_down.X, m_mouse_pos_down.Y);
				GL.Vertex2(m_mouse_pos.X, m_mouse_pos_down.Y);
				GL.Vertex2(m_mouse_pos.X, m_mouse_pos.Y);
				GL.Vertex2(m_mouse_pos_down.X, m_mouse_pos.Y);
			}
			else {
				GL.Vertex2(m_mouse_pos_down.X, m_mouse_pos_down.Y);
				GL.Vertex2(m_mouse_pos_down.X, m_mouse_pos.Y);
				GL.Vertex2(m_mouse_pos.X, m_mouse_pos.Y);
				GL.Vertex2(m_mouse_pos.X, m_mouse_pos_down.Y);
			}
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
				} else {
					GL.Disable(EnableCap.DepthTest);
				}
				GL.Disable(EnableCap.Lighting);
				GL.Disable( EnableCap.Texture2D );

				GL.CallList(GL_GRID);
				GL.PopMatrix();
			}
		}

		private void DrawTextItemsInternal( bool draw_item_individually, IEnumerable<GLTextItem> items )
		{
			foreach( var text_item in items ) {

				float r = (float)text_item.TextColor.R / 255.0f;
				float g = (float)text_item.TextColor.G / 255.0f;
				float b = (float)text_item.TextColor.B / 255.0f;
				GL.Color4( r, g, b, text_item.Alpha );

				float x = 0.0f;
				float y = 0.0f;

				if( draw_item_individually ) {
					var rot = text_item.WorldRotation;
					if (text_item.Facing) {
						switch( m_view_type ) {
						case ViewType.RIGHT: {
								Vector3 back_vector = -Vector3.UnitX;
								Vector3 up_vector = Vector3.UnitY;
								Vector3 right_vector = Vector3.UnitZ;
								rot = new Matrix4( new Vector4( right_vector ), new Vector4( up_vector ), new Vector4( back_vector ), Vector4.UnitW );
							}
							break;
						case ViewType.TOP: {
								Vector3 back_vector = -Vector3.UnitY;
								Vector3 up_vector = Vector3.UnitZ;
								Vector3 right_vector = Vector3.UnitX;
								rot = new Matrix4( new Vector4( right_vector ), new Vector4( up_vector ), new Vector4( back_vector ), Vector4.UnitW );
							}
							break;
						case ViewType.FRONT: {
								Vector3 back_vector = Vector3.UnitZ;
								Vector3 up_vector = Vector3.UnitY;
								Vector3 right_vector = Vector3.UnitX;
								rot = new Matrix4( new Vector4( right_vector ), new Vector4( up_vector ), new Vector4( back_vector ), Vector4.UnitW );
							}
							break;
						case ViewType.PERSP: {
								// There is a -1 scale applied to invert the Z direction
								// This is confusing, but, we need to treat things like +Z is back (instead of forward as we display)
								Vector3 cam_nozscale = new Vector3( m_cam_pos.X, m_cam_pos.Y, -m_cam_pos.Z );

								// Calculate the back vector for the text item - which is to look at the camera
								Vector3 back_vector = ( text_item.WorldPosition - cam_nozscale ).Normalized();

								// Use the camera's right vector to keep the text oriented in left-to-right on the screen for readability
								// We want the camera's right vector to be the right vector for the text item, as this will ensure that
								// the text is readable to the camera (as opposed to readable from the text item's point of view) in a left-to-right fashion.
								// Remember to flip the Z of the camera because of the Z inverse scale.
								Vector3 right_vector = new Vector3( m_cam_mat.M11, m_cam_mat.M21, -m_cam_mat.M31 );

								// When the camera's left vector aligns with the text item's back vector we don't exactly
								// know how to align. This can only happen if the text item is to the left/right of the camera.
								// In this case we'll align the up of the text item with the up of the camera
								Vector3 up_vector;
								if( Math.Abs( Vector3.Dot( back_vector, right_vector ) ) > 0.99f ) {
									// Use the camera up for alignment
									// Remember to flip the Z of the camera because of the Z inverse scale.
									up_vector = new Vector3( m_cam_mat.M12, m_cam_mat.M22, -m_cam_mat.M32 );
									// Keep it orthonormal
									right_vector = Vector3.Cross( up_vector, back_vector ).Normalized();
									up_vector = Vector3.Cross( back_vector, right_vector );
								} else {
									// Calculate the up vector from the right and back
									up_vector = Vector3.Cross( back_vector, right_vector ).Normalized();
									// Keep it orthonormal
									right_vector = Vector3.Cross( up_vector, back_vector );
								}
								rot = new Matrix4( new Vector4( right_vector ), new Vector4( up_vector ), new Vector4( back_vector ), Vector4.UnitW );
							}
							break;
						}
					}

					GL.PushMatrix();
					GL.Translate( text_item.WorldPosition );
					GL.Scale(text_item.Scale);
					GL.MultMatrix( ref rot );
					if( text_item.DepthTestEnable ) {
						GL.Enable( EnableCap.DepthTest );
					}
					m_font_stash.BeginDraw();
				} else {
					x = text_item.WorldPosition.X;
					y = text_item.WorldPosition.Y;
				}

				//Adjust x & y for alignment
				float min_x, max_x, min_y, max_y;
				m_font_stash.DimText(m_font_stash_font_id, text_item.PointSize, text_item.Text, out min_x, out min_y, out max_x, out max_y);
				x -= (max_x - min_x) * text_item.AlignFactor[(int)text_item.Align & 3];
				y -= (max_y - min_y) * text_item.AlignFactor[(int)text_item.Align / 4];

				float next_x;
				m_font_stash.DrawText( m_font_stash_font_id, text_item.PointSize, x, y, text_item.Text, out next_x );

				if( draw_item_individually ) {
					m_font_stash.EndDraw();
					if( text_item.DepthTestEnable ) {
						GL.Disable( EnableCap.DepthTest );
					}
					GL.PopMatrix();
				}
			}
		}

		public void Draw3DTextItems()
		{
			GL.PushMatrix();

			GL.Disable( EnableCap.CullFace );
			GL.Disable( EnableCap.DepthTest );
			GL.Disable( EnableCap.Lighting );
			GL.Enable( EnableCap.Texture2D );

			switch( m_view_type ) {
				case ViewType.FRONT:
					break;
				case ViewType.TOP:
					GL.Rotate( -90, Vector3.UnitX );
					break;
				case ViewType.RIGHT:
					GL.Rotate( 90, Vector3.UnitY );
					break;
				case ViewType.PERSP:
					break;
			}

			DrawTextItemsInternal( true, m_text_items.Where( x => x.IsScreenSpace == false ) );

			GL.Disable( EnableCap.Texture2D );
			GL.Enable( EnableCap.DepthTest );
			GL.Enable( EnableCap.CullFace );

			GL.PopMatrix();
		}

		public void DrawScreenSpaceTextItems()
		{
			GL.Disable( EnableCap.DepthTest );
			GL.Disable( EnableCap.Lighting );
			GL.Enable( EnableCap.Texture2D );

			m_font_stash.BeginDraw();
			DrawTextItemsInternal( false, m_text_items.Where( x => x.IsScreenSpace == true ) );
			m_font_stash.EndDraw();

			GL.Disable( EnableCap.Texture2D );
			GL.Enable( EnableCap.DepthTest );
		}

		public void DrawLevel()
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
			// Draw Entities solid
			Entity e;
			for (int i = 0; i < Level.MAX_ENTITIES; i++) {
				if (editor.m_level.entity[i].Visible) {
					e = editor.m_level.entity[i];

					if (BuildAssetModels.GetEntityList(e.Type, e.SubType, (e.entity_props as Overload.EntityPropsProp)?.Index) != -1)
						continue;

					GL.PushMatrix();
					GL.Translate(e.position);
					GL.MultMatrix(ref e.m_rotation);

					GL.Color4(GetEntityTypeColor(e.Type));
					switch (e.Type) {
						case EntityType.ENEMY:
							GL.CallList(GL_ENEMY);
							break;
						case EntityType.PROP:
							GL.CallList(GL_PROP);
							break;
						case EntityType.ITEM:
							GL.CallList(GL_ITEM);
							break;
						case EntityType.DOOR:
							GL.CallList(GL_DOOR_BBOX);
							break;
						case EntityType.SCRIPT:
							GL.CallList(GL_SCRIPT_BBOX);
							break;
						case EntityType.TRIGGER:
							GL.CallList(GL_TRIGGER_BBOX);
							break;
						case EntityType.LIGHT:
							GL.CallList(GL_LIGHT);
							break;
						case EntityType.SPECIAL:
							if (e.SubType == (int)SpecialSubType.PLAYER_START) {
								Vector3 scl = Vector3.One;
								scl.X = 1.6f;
								scl.Y = 0.4f;
								scl.Z = 1.2f;
								GL.Scale(scl);
							}
							GL.CallList(GL_SPECIAL);
							GL.Scale(Vector3.One);
							break;
					}
					GL.PopMatrix();
				}
			}

			switch (editor.m_lighting_type) {
				case LightingType.OFF:
					GL.Disable(EnableCap.Lighting);
					break;
				case LightingType.BRIGHT:
					if (DrawChunks()) {
						GL.Disable(EnableCap.Lighting);
					} else {
						GL.Enable(EnableCap.Lighting);
						GL.Enable(EnableCap.Light0);
						GL.Enable(EnableCap.Light1);
						SetAmbientColor(C_ambient_solid);
					}
					break;
				case LightingType.DARK:
					if (DrawChunks()) {
						GL.Disable(EnableCap.Lighting);
					} else {
						GL.Enable(EnableCap.Lighting);
						GL.Enable(EnableCap.Light0);
						GL.Disable(EnableCap.Light1);
						SetAmbientColor(C_ambient_solid);
					}
					break;
			}

			// Draw Level Geometry
			if (DrawTexture()) {
				SetAmbientColor(C_ambient_texture);
				GL.Enable(EnableCap.Texture2D);
				if (editor.tm_level.m_gl_id.Count > 0)
				{
					GL.BindTexture(TextureTarget.Texture2D, editor.tm_level.m_gl_id[0]);
				}
			} else {
				GL.Disable(EnableCap.Texture2D);
				SetAmbientColor(C_ambient_solid);
			}

			// Draw base geometry texture/flat
			if (DrawSolid()) {
				SetDiffuseObjectColor(C_base);
				GL.CallList(GL_BASE);
				GL.CallList(GL_GMESH);

				for (int i = 0; i < Level.MAX_ENTITIES; i++) {
					if (editor.m_level.entity[i].Visible) {
						e = editor.m_level.entity[i];

						var list = BuildAssetModels.GetEntityList(e.Type, e.SubType, (e.entity_props as Overload.EntityPropsProp)?.Index);
						if (list == -1)
							continue;

						GL.PushMatrix();
						GL.Translate(e.position);
						GL.MultMatrix(ref e.m_rotation);
						GL.CallList(list);
						GL.PopMatrix();
					}
				}
			}

			//Draw chunk data
			if (DrawChunks()) {
				GL.CallList(GL_CHUNKS);
			}

			GL.PolygonOffset(0f, 0f);
			GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
			GL.Disable(EnableCap.Texture2D);
			GL.Disable(EnableCap.CullFace);
			GL.Disable(EnableCap.Lighting);
			GL.Disable(EnableCap.ColorMaterial);
			GL.Disable(EnableCap.PolygonOffsetFill);
			GL.LineWidth(1f);

			// Draw base geometry wireframe
			if (DrawWireframe()) {
				GL.Color4(C_base_wire);
				GL.CallList(GL_BASE);
				GL.CallList(GL_GMESH);
			}

			// Draw base verts
			if (editor.m_edit_mode == EditMode.VERTEX) {
				GL.PointSize(4f);
				GL.CallList(GL_VERTS);
			}

			GL.Disable(EnableCap.DepthTest);

			GL.LineWidth(3f);
			GL.PointSize(8f);
			GL.CallList(GL_SELECT);

			// Draw object bounding boxes (marked or not)
			for (int i = 0; i < Level.MAX_ENTITIES; i++) {
				if (editor.m_level.entity[i].Visible) {
					e = editor.m_level.entity[i];
					if (i == editor.m_level.selected_entity) {
						GL.Disable(EnableCap.DepthTest);
						GL.LineWidth(3f);
						if (editor.m_edit_mode == EditMode.ENTITY) {
							GL.Color3(e.marked ? C_select_marked_main : C_select_main);
						} else {
							GL.Color3(e.marked ? C_select_marked : C_select);
						}
					} else {
						GL.Enable(EnableCap.DepthTest);
						GL.LineWidth(1f);
						GL.Color3(e.marked ? C_marked : C_base_wire);
					}

					GL.PushMatrix();
					GL.Translate(e.position);
					GL.MultMatrix(ref e.m_rotation);
					GL.CallList(GL_ENTITY_BBOX + (int)e.Type);
					GL.CallList(GL_ARROW);
					// Selelected entity stuff (extra drawing)
					if (i == editor.m_level.selected_entity) {
						switch (e.Type) {
							case EntityType.ENEMY: break;
							case EntityType.PROP: break;
							case EntityType.ITEM: break;
							case EntityType.DOOR: break;
							case EntityType.LIGHT:
								if (e.SubType == (int)LightSubType.POINT || e.SubType == (int)LightSubType.NO_SHADOW) {
									// Point light
									CreateLineSphere(((Overload.EntityPropsLight)e.entity_props).range);
								} else {
									// Spot light
									CreateLineLightingDisc(((Overload.EntityPropsLight)e.entity_props).range, ((Overload.EntityPropsLight)e.entity_props).angle);
                        }
								break;
							case EntityType.SCRIPT:
								// Draw the script links if selected
								CreateLinkDiamonds(e);
								break;
							case EntityType.TRIGGER:
								// Draw the trigger links if selected
								CreateLinkDiamonds(e);

								if (e.SubType < (int)TriggerSubType.SPHERE) {
									CreateLineBox(((Overload.EntityPropsTrigger)e.entity_props).size * 0.5f);
								} else {
									CreateLineSphere(((Overload.EntityPropsTrigger)e.entity_props).size.X);
								}
								break;
							case EntityType.SPECIAL: break;
						}
					}
					GL.PopMatrix();
				}
			}

			GL.Disable(EnableCap.DepthTest);
			GL.LineWidth(1f);
			GL.PointSize(4f);
			
			// Draw marked stuff
			GL.CallList(GL_MARKED);

			if (m_view_type == ViewType.PERSP && editor.m_gimbal_display == GimbalDisplay.SHOW) {
				GL.Enable(EnableCap.Blend);
				GL.Disable(EnableCap.DepthTest);
				GL.CallList(GL_GIMBAL);
			}
			if (m_view_type == ViewType.PERSP && editor.m_cp_display == ClipPlaneDisplay.SHOW) {
				GL.Disable(EnableCap.DepthTest);
				GL.CallList(GL_CLIP_PLANES);
			}

			GL.PolygonMode( MaterialFace.FrontAndBack, PolygonMode.Fill );
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
                return (editor.m_view_mode_persp == ViewMode.WIREFRAME || editor.m_view_mode_persp == ViewMode.WIRE_SOLID || editor.m_view_mode_persp == ViewMode.WIRE_TEXTURE || editor.m_view_mode_persp == ViewMode.CHUNKS);
			} else {
				return (editor.m_view_mode_ortho == ViewMode.WIREFRAME || editor.m_view_mode_ortho == ViewMode.WIRE_SOLID || editor.m_view_mode_ortho == ViewMode.WIRE_TEXTURE || editor.m_view_mode_ortho == ViewMode.CHUNKS);
			}
		}

		public bool DrawSolid()
		{
			if (m_view_type == ViewType.PERSP) {
                return ((editor.m_view_mode_persp != ViewMode.WIREFRAME) && (editor.m_view_mode_persp != ViewMode.CHUNKS));
			} else {
				return ((editor.m_view_mode_ortho != ViewMode.WIREFRAME) && (editor.m_view_mode_ortho != ViewMode.CHUNKS));
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

        public bool DrawChunks()
        {
            if (m_view_type == ViewType.PERSP) {
                return (editor.m_view_mode_persp == ViewMode.CHUNKS);
            }
            else {
                return (editor.m_view_mode_ortho == ViewMode.CHUNKS);
            }
        }
    }
}
