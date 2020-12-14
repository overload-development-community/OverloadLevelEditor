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
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenTK.Graphics;
using OpenTK;
using OpenTK.Graphics.OpenGL;

// GLVIEW - Primary
// The editor contains four viewports of the level (top, right, front, and perspective)
// All four viewports use the same functions and share their OpenGL context
// The world is rotated when drawing, while the camera/grid are stationary
// Perspective view has the special case code, if any

namespace OverloadLevelEditor
{
	public partial class GLView : OpenTK.GLControl
	{
		public Editor editor;
		
		public ViewType m_view_type;
		bool m_gl_loaded = false;

		public Vector3 m_proj_offset = Vector3.Zero;
		public float m_proj_scale = 0.01f;

		public Vector2 m_cam_angles = Vector2.One * -0.5f;
		public float m_cam_distance = 5.0f;
		public Matrix4 m_cam_mat;
		public Matrix4 m_persp_mat;

		public Vector2 m_control_sz;
		
		public GLView(ViewType vt, Editor e)
		{
			editor = e;
			m_view_type = vt;
			InitializeComponent();
		}

		public void SetupViewport()
		{
			m_control_sz = new Vector2(gl_custom.Width, gl_custom.Height);
		}

		private void gl_custom_Load(object sender, EventArgs e)
		{
			m_gl_loaded = true;
			UpdateBGColor(editor.m_bg_color);
			UpdateClearColor();
			BuildGridGeometry(editor.m_grid_lines, editor.m_grid_spacing);
			BuildDefaults();
			CreateDefaultLight();
			SetupViewport();
		}

		private void gl_custom_Paint(object sender, PaintEventArgs e)
		{
			// Don't try to draw until the control has loaded
			if (!m_gl_loaded) {
				return;
			}

			gl_custom.MakeCurrent();

			GL.ClearColor(C_bg);
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
			GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
			GL.Enable(EnableCap.Blend);
			
			// Use all of the glControl painting area
			GL.Viewport(0, 0, (int)m_control_sz.X, (int)m_control_sz.Y);
			
			switch (m_view_type) {
				case ViewType.RIGHT:
				case ViewType.TOP:
				case ViewType.FRONT:
					// Rotation for the orthographic view is done in DrawObjects
					GL.MatrixMode(MatrixMode.Projection);
					GL.LoadIdentity();
					
					m_persp_mat = Matrix4.CreateOrthographic(m_control_sz.X * m_proj_scale, m_control_sz.Y * m_proj_scale, -999f, 999f);
					GL.LoadMatrix(ref m_persp_mat);

					m_cam_mat = Matrix4.CreateTranslation(-m_proj_offset);
					GL.MatrixMode(MatrixMode.Modelview);
					GL.LoadMatrix(ref m_cam_mat);
					break;
				case ViewType.PERSP:
					// Perspective mode
					GL.MatrixMode(MatrixMode.Projection);
					GL.LoadIdentity();
					m_persp_mat = Matrix4.CreatePerspectiveFieldOfView(Utility.RAD_90 * (float)editor.m_view_persp_fov / 90f, m_control_sz.X / m_control_sz.Y, 0.3f, 2000f);
					GL.LoadMatrix(ref m_persp_mat);
					
					Vector3 cam_pos = m_proj_offset + Vector3.Transform(Vector3.UnitZ * m_cam_distance, Matrix4.CreateRotationX(m_cam_angles.X) * Matrix4.CreateRotationY(m_cam_angles.Y));
					m_cam_mat = Matrix4.LookAt(cam_pos, m_proj_offset, Vector3.UnitY);

					GL.MatrixMode(MatrixMode.Modelview);
					GL.LoadMatrix(ref m_cam_mat);
					break;
			}
			
			// Make everything appear left-handed like Unity (instead of right-handed that OpenGL is by default)
			GL.Scale(1f, 1f, -1f);
					
			DrawGrid();
			DrawDMesh();

			// Mouse drag outline
			if (m_draw_mouse_drag) {
				DrawMouseDrag();
			}
			
			gl_custom.SwapBuffers();

		}

		public int m_resize_count = 0;

		private void gl_custom_Resize(object sender, EventArgs e)
		{
			SetupViewport();
			this.Invalidate();
		}

		private void gl_custom_MouseEnter(object sender, EventArgs e)
		{
			gl_custom.Focus();
		}

		public Vector2 m_mouse_pos;
		public Vector2 m_mouse_pos_down;
		public Vector2 m_mouse_pos_prev;
		public Stopwatch m_mouse_down_sw = new Stopwatch();
		public bool m_draw_mouse_drag = false;
		public bool m_cancel_move = false;

		private void gl_custom_MouseWheel(object sender, MouseEventArgs e)
		{
			if (e.Delta > 0) {
				if (m_view_type == ViewType.PERSP && ModifierKeys == Keys.Control) {
					editor.AdjustPerspFOV(-10);
				} else {
					IncreaseZoom();
				}
			} else {
				if (m_view_type == ViewType.PERSP && ModifierKeys == Keys.Control) {
					editor.AdjustPerspFOV(10);
				} else {
					DecreaseZoom();
				}
			}
			gl_custom.Invalidate();
		}

		private void SetVertManipulationCursor()
		{
			Keys modifier_keys = ModifierKeys & ~Keys.Shift;

			if ((Control.MouseButtons & MouseButtons.Right) == MouseButtons.Right) {

				if (modifier_keys == Keys.None) {

					//Move
					Cursor.Current = Cursors.Hand;
					return;
				}
				if (modifier_keys == Keys.Alt) {

					//Rotate
					Cursor.Current = Cursors.PanEast;
					return;
				} else if (modifier_keys == Keys.Control) {

					//Scale
					Cursor.Current = Cursors.SizeAll;
					return;
				}
			}

			//Nothing
			Cursor.Current = Cursors.Default;
		}

		private void gl_custom_MouseDown(object sender, MouseEventArgs e)
		{
			m_mouse_pos.X = e.X;
			m_mouse_pos.Y = e.Y;
			m_mouse_pos_down = m_mouse_pos;
			m_mouse_pos_prev = m_mouse_pos;
			m_mouse_down_sw.Restart();
			m_cancel_move = false;
			switch (e.Button) {
				case MouseButtons.Left:
					editor.SaveStateForUndo("Mouse-Mark/select elements", false);	
					break;
				case MouseButtons.Right:
					editor.SaveStateForUndo("Mouse-Modify (move/rotate/scale) marked elements", false);
					SetVertManipulationCursor();
					break;
				case MouseButtons.Middle:
					m_cancel_move = true;
					break;
			} 
		}

		private void gl_custom_MouseMove(object sender, MouseEventArgs e)
		{
			m_mouse_pos.X = e.X;
			m_mouse_pos.Y = e.Y;
			Vector3 move_vec = Vector3.Zero;
			
			if (m_view_type == ViewType.PERSP) {
				switch (e.Button) {
					case (MouseButtons)((int)MouseButtons.Middle + (int)MouseButtons.Right):
						m_cam_angles.X += (m_mouse_pos.Y - m_mouse_pos_prev.Y) * -0.007f;
						m_cam_angles.Y += (m_mouse_pos.X - m_mouse_pos_prev.X) * -0.007f;
						m_cam_angles.X = Math.Min(Utility.PI * 0.49f, Math.Max(Utility.PI * -0.49f, m_cam_angles.X));
						gl_custom.Invalidate();
						break;
					case MouseButtons.Left:
						if (ModifierKeys == Keys.Alt) {
							m_cam_angles.X += (m_mouse_pos.Y - m_mouse_pos_prev.Y) * -0.007f;
							m_cam_angles.Y += (m_mouse_pos.X - m_mouse_pos_prev.X) * -0.007f;
							m_cam_angles.X = Math.Min(Utility.PI * 0.49f, Math.Max(Utility.PI * -0.49f, m_cam_angles.X));
							gl_custom.Invalidate();
						} else {
							m_draw_mouse_drag = true;
							gl_custom.Invalidate();
						}
						break;
					case MouseButtons.Middle:
						// Pan
						Vector3 base_offset;
						base_offset.X = (m_mouse_pos.X - m_mouse_pos_prev.X) * -m_cam_distance * 0.002f;
						base_offset.Y = (m_mouse_pos.Y - m_mouse_pos_prev.Y) * m_cam_distance * 0.002f;
						base_offset.Z = 0f;

						base_offset = Vector3.Transform(base_offset, m_cam_mat.ExtractRotation().Inverted());
						m_proj_offset += base_offset;

						m_cancel_move = true;
						gl_custom.Invalidate();
						break;
					case MouseButtons.Right:
						// Move marked
						if (!m_cancel_move) {
							move_vec.X = (m_mouse_pos.X - m_mouse_pos_down.X) * m_cam_distance * 0.002f;
							move_vec.Y = (m_mouse_pos.Y - m_mouse_pos_down.Y) * -m_cam_distance * 0.002f;

							if ((ModifierKeys & Keys.Shift) == Keys.Shift) {
								// SHIFT - Unsnapped movement
								FreeMove(move_vec, ModifierKeys);
							} else {
								// NO SHIFT - Snapped movement
								SnappedMove(move_vec, ModifierKeys);
							}
						}
						break;
				}
			} else {
				switch (e.Button) {
					case MouseButtons.Left:
						if (ModifierKeys == Keys.Alt) {
							m_proj_offset.X += (m_mouse_pos.X - m_mouse_pos_prev.X) * -m_proj_scale;
							m_proj_offset.Y += (m_mouse_pos.Y - m_mouse_pos_prev.Y) * m_proj_scale;
							gl_custom.Invalidate();
						} else {
							m_draw_mouse_drag = true;
							gl_custom.Invalidate();
						}
						break;
					case MouseButtons.Right:
						// Move marked
						if (!m_cancel_move) {
							move_vec.X = (m_mouse_pos.X - m_mouse_pos_down.X) * m_proj_scale;
							move_vec.Y = (m_mouse_pos.Y - m_mouse_pos_down.Y) * -m_proj_scale;

							if ((ModifierKeys & Keys.Shift) == Keys.Shift) {
								// SHIFT - Unsnapped movement
								FreeMove(move_vec, ModifierKeys);
							} else {
								// NO SHIFT - Snapped movement
								SnappedMove(move_vec, ModifierKeys);
							}
						}
						break;
					case MouseButtons.Middle:
						// Pan
						m_cancel_move = true;
						m_proj_offset.X += (m_mouse_pos.X - m_mouse_pos_prev.X) * -m_proj_scale;
						m_proj_offset.Y += (m_mouse_pos.Y - m_mouse_pos_prev.Y) * m_proj_scale;
						gl_custom.Invalidate();
						break;
				}
			}

			m_mouse_pos_prev = m_mouse_pos;
		}

		public void SnappedMove(Vector3 move_vec, Keys mod_keys)
		{
			float move_dist = move_vec.Length;
			if (mod_keys == Keys.None) {
				// Snapped movement
				if (move_dist >= editor.m_grid_snap) {
					Vector2 diff = m_mouse_pos - m_mouse_pos_down;
					if (Math.Abs(diff.X) > Math.Abs(diff.Y)) {
						m_mouse_pos_down.X = m_mouse_pos.X;
						move_vec.Y = 0f;
					} else {
						m_mouse_pos_down.Y = m_mouse_pos.Y;
						move_vec.X = 0f;
					}
					editor.m_dmesh.MoveMarked(this, move_vec, editor.m_grid_snap * (int)(Math.Abs(move_dist) / editor.m_grid_snap));
				}
			} else if (mod_keys == Keys.Alt) {
				// Snapped rotation - 15 degrees
				if (Math.Abs(move_vec.X) > 0.1f) {
					m_mouse_pos_down.X = m_mouse_pos.X;
					float rot = Utility.RAD_15;
					if (move_vec.X < 0f) {
						rot *= -1f;
					}
					editor.RotateMarkedElementsRaw(this, rot);
				}
			} else if (mod_keys == Keys.Control) {
				// Snapped scaling - 10%
				if (Math.Abs(move_vec.X) > 0.1f) {
					m_mouse_pos_down.X = m_mouse_pos.X;
					float scl = 1.1f;
					if (move_vec.X < 0f) {
						scl = 1f / 1.1f;
					}
					editor.ScaleMarkedElementsRaw(this, scl, scl);
				}
			}
		}

		public void FreeMove(Vector3 move_vec, Keys mod_keys)
		{
			m_mouse_pos_down = m_mouse_pos;
			if (mod_keys == (Keys.Shift | Keys.Alt)) {
				float rot = 0.2f * move_vec.X;
				editor.RotateMarkedElementsRaw(this, rot);
			} else if (mod_keys == (Keys.Shift | Keys.Control)) {
				// Raw scaling
				float sclx = 1f + 0.2f * move_vec.X;
				float scly = 1f + 0.2f * move_vec.Y;
				editor.ScaleMarkedElementsRaw(this, sclx, scly);
			} else {
				// Raw movement
				editor.m_dmesh.MoveMarkedRaw(this, move_vec);
			}
		}

		private void gl_custom_MouseUp(object sender, MouseEventArgs e)
		{
			m_mouse_pos.X = e.X;
			m_mouse_pos.Y = e.Y;// + 3f;
			m_draw_mouse_drag = false;

			switch (e.Button) {
				case MouseButtons.Left:
					if (ModifierKeys != Keys.Alt) {
						float dist = (m_mouse_pos - m_mouse_pos_down).Length;
						int drag_time = (int)m_mouse_down_sw.ElapsedMilliseconds;
						if ((dist < 5f && drag_time < 400) || dist < 1.5f || drag_time < 100) {
							editor.m_dmesh.MousePickSelect(this, m_mouse_pos);
						} else {
							editor.m_dmesh.MouseDragMark(this, m_mouse_pos_down, m_mouse_pos, ModifierKeys == Keys.Shift);
						}
					}
					break;
				case MouseButtons.Right:
					SetVertManipulationCursor();
					break;
			}

		}

		public static float ZOOM_SPD = 0.93f;
		public static float ZOOM_SPD_PERSP = 0.94f;

		public void IncreaseZoom()
		{
			if (m_view_type == ViewType.PERSP) {
				m_cam_distance *= ZOOM_SPD_PERSP;
				m_cam_distance = Math.Max(0.1f, m_cam_distance);
			} else {
				m_proj_scale *= ZOOM_SPD;
				m_proj_scale = Math.Max(0.001f, m_proj_scale);
			}
		}

		public void DecreaseZoom()
		{
			if (m_view_type == ViewType.PERSP) {
				m_cam_distance /= ZOOM_SPD_PERSP;
				m_cam_distance = Math.Min(100f, m_cam_distance);
			} else {
				m_proj_scale /= ZOOM_SPD;
				m_proj_scale = Math.Min(0.5f, m_proj_scale);
			}
		}

		private void gl_custom_KeyDown(object sender, KeyEventArgs e)
		{
			editor.KeyboardDown(e, this);
			SetVertManipulationCursor();
		}

		private void gl_custom_KeyUp(object sender, KeyEventArgs e)
		{
			SetVertManipulationCursor();
		}

		private void gl_custom_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			switch (e.KeyCode) {
				case Keys.Down:
				case Keys.Up:
				case Keys.Left:
				case Keys.Right:
				case Keys.Tab:
					e.IsInputKey = true;
					break;
			}
		}

		public void FitFrame(List<Vector3> pos_list)
		{
			if (pos_list.Count < 2) return;

			Vector3 center = Vector3.Zero;
			Vector3 raw_min = Vector3.Zero;
			Vector3 raw_max = Vector3.Zero;
			Vector3 proj_min = Vector3.Zero;
			Vector3 proj_max = Vector3.Zero;

			// Transform the list into screen coordinates, and calculate the actual center too
			for (int i = 0; i < pos_list.Count; i++) {
				if (i == 0) {
					raw_min = pos_list[i];
					raw_max = pos_list[i];
				} else {
					raw_min.X = Math.Min(raw_min.X, pos_list[i].X);
					raw_min.Y = Math.Min(raw_min.Y, pos_list[i].Y);
					raw_min.Z = Math.Min(raw_min.Z, pos_list[i].Z);
					raw_max.X = Math.Max(raw_max.X, pos_list[i].X);
					raw_max.Y = Math.Max(raw_max.Y, pos_list[i].Y);
					raw_max.Z = Math.Max(raw_max.Z, pos_list[i].Z);
				}
				pos_list[i] = Utility.WorldToScreenPos(pos_list[i], this);
				if (i == 0) {
					proj_min = pos_list[i];
					proj_max = pos_list[i];
				} else {
					proj_min.X = Math.Min(proj_min.X, pos_list[i].X);
					proj_min.Y = Math.Min(proj_min.Y, pos_list[i].Y);
					proj_min.Z = Math.Min(proj_min.Z, pos_list[i].Z);
					proj_max.X = Math.Max(proj_max.X, pos_list[i].X);
					proj_max.Y = Math.Max(proj_max.Y, pos_list[i].Y);
					proj_max.Z = Math.Max(proj_max.Z, pos_list[i].Z);
				}
			}

			center = (raw_min + raw_max) * 0.5f;
			
			// Adjust the offset to center on the list
			switch (m_view_type) {
				case ViewType.PERSP:
					center.Z *= -1f;
					m_proj_offset = center;
					break;
				case ViewType.FRONT:
					m_proj_offset.X = center.X;
					m_proj_offset.Y = center.Y;
					break;
				case ViewType.RIGHT:
					m_proj_offset.X = center.Z;
					m_proj_offset.Y = center.Y;
					break;
				case ViewType.TOP:
					m_proj_offset.X = center.X;
					m_proj_offset.Y = center.Z;
					break;
			}

			// Adjust the scale/cam_distance (use fov too) to fit everything in ok
			if (m_view_type == ViewType.PERSP) {
				// This is isn't awesome/accurate, but it's good enough
				float extent = (raw_max - raw_min).Length;
				m_cam_distance = extent * 1.0f;
			} else {
				float x_dist = proj_max.X - proj_min.X;
				float y_dist = proj_max.Y - proj_min.Y;
				float proj_new = Math.Max(x_dist / m_control_sz.X, y_dist / m_control_sz.Y) * m_proj_scale;
				m_proj_scale = proj_new * 1.5f;
			}

			gl_custom.Invalidate();
		}
	}
}
