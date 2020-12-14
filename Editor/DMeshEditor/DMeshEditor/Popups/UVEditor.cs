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
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using OpenTK;

namespace OverloadLevelEditor
{
	public partial class UVEditor : Form
	{
		public Bitmap tex = null;
		public Editor editor;

		public float[] zoom = { 0.05f, 0.03f, 0.02f, 0.014f, 0.01f, 0.007f, 0.005f, 0.0035f, 0.0025f, 0.002f, 0.0017f, 0.0014f, 0.0012f, 0.001f };
		public int zoom_level = 6;

		public float cam_x;
		public float cam_y;

		public int[] selected_uv = { -1, -1 }; // Poly/Vert
		public MouseButtons down_button = MouseButtons.None;
		public float previous_x = 0;
		public float previous_y = 0;
		public float start_x = 0;
		public float start_y = 0;
		public float last_mouse_x = 0;
		public float last_mouse_y = 0;

		public bool draw_drag_box = false;
		public bool tex_from_sel_poly = true;
		
		public UVEditor(Editor e)
		{
			editor = e;
			InitPens();
			InitializeComponent();
		}

		private void UVEditor_Load(object sender, EventArgs e)
		{
			
		}
		
		private void view_Paint(object sender, PaintEventArgs e)
		{
			DrawImage(e.Graphics, cur_texture, 0f, 0f, 1f, 1f);
			DrawImage(e.Graphics, cur_texture, -1f, 0f, 0f, 1f);
			DrawImage(e.Graphics, cur_texture, -1f, -1f, 0f, 0f);
			DrawImage(e.Graphics, cur_texture, 0f, -1f, 1f, 0f);

			DrawFilledBox(e.Graphics, Color.White, 128, 0f, 0f, 1f, 1f);

			DrawGrid(e.Graphics);

			DrawUVs(e.Graphics);

			if (draw_drag_box) {
				DrawBoxCorners(e.Graphics, p_drag_box, start_x, start_y, previous_x, previous_y);
			}
		}

		private void view_ClientSizeChanged(object sender, EventArgs e)
		{
			this.Refresh();
		}

		private void view_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			switch (e.KeyCode) {
				case Keys.Down:
				case Keys.Up:
				case Keys.Left:
				case Keys.Right:
					e.IsInputKey = true;
					break;
			}
		}

		private void view_MouseWheel(object sender, MouseEventArgs e)
		{
			// Zoom in and out
			if (e.Delta > 0) {
				IncreaseZoom();
			} else {
				DecreaseZoom();
			}

			last_mouse_x = e.X;
			last_mouse_y = e.Y;
			previous_x = TranslateXCoordinate(e.X);
			previous_y = TranslateYCoordinate(e.Y);
			//UpdateCoordinateLabel();
		}

		public Stopwatch m_mouse_down_sw = new Stopwatch();
		public bool m_cancel_move = false;

		private void view_MouseDown(object sender, MouseEventArgs e)
		{
			// Keep track of which button is down
			down_button = e.Button;
			start_x = previous_x = TranslateXCoordinate(e.X);
			start_y = previous_y = TranslateYCoordinate(e.Y);
			m_mouse_down_sw.Restart();
			view.Select();
			draw_drag_box = false;
					
			switch (e.Button) {
				case MouseButtons.Middle:
				case MouseButtons.Left:
					m_cancel_move = true;
					break;
				case MouseButtons.Right:
					m_cancel_move = false;
					break;
			}

			this.Refresh();
		}

		private void view_MouseMove(object sender, MouseEventArgs e)
		{
			Vector2 start_pos = new Vector2(start_x, start_y);
			Vector2 end_pos = new Vector2(previous_x, previous_y);
			
			switch (down_button) {
				case MouseButtons.Middle:
					// Move camera
					cam_x += (previous_x - TranslateXCoordinate(e.X));
					cam_y += (previous_y - TranslateYCoordinate(e.Y));
					this.Refresh();
					break;
				case MouseButtons.Left:
					draw_drag_box = true;
					this.Refresh();
					break;
				case MouseButtons.Right:
					if (!m_cancel_move) {
						Vector2 move_vec= (end_pos - start_pos);
						
						MoveUVs(move_vec);

						start_x = previous_x;
						start_y = previous_y;
					}
					this.Refresh();
					editor.RefreshGeometry();
					break;
			}

			// Convert coordinates and display them
			last_mouse_x = e.X;
			last_mouse_y = e.Y;
			previous_x = TranslateXCoordinate(e.X);
			previous_y = TranslateYCoordinate(e.Y);
			//UpdateCoordinateLabel();
		}

		private void view_MouseUp(object sender, MouseEventArgs e)
		{
			down_button = MouseButtons.None;
			draw_drag_box = false;
			previous_x = TranslateXCoordinate(e.X);
			previous_y = TranslateYCoordinate(e.Y);
			Vector2 start_pos = new Vector2(start_x, start_y);
			Vector2 end_pos = new Vector2(previous_x, previous_y);
			
			switch (e.Button) {
				case MouseButtons.Left:
					// Select vert
					float dist = (start_pos - end_pos).Length;
					int drag_time = (int)m_mouse_down_sw.ElapsedMilliseconds;
					if ((dist < 0.05f && drag_time < 400) || dist < 0.01f || drag_time < 100) {
						SelectVert(end_pos);
					} else {
						editor.SaveStateForUndo("Mark UVs with mouse");
			
						DragMark(start_pos, end_pos, ModifierKeys == Keys.Shift);
					}
					break;
			}

			this.Refresh();
		}

		private void UVEditor_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (e.CloseReason != CloseReason.FormOwnerClosing) {
				this.Hide();
				e.Cancel = true;
			}
		}

		private void UVEditor_KeyDown(object sender, KeyEventArgs e)
		{
			switch (e.KeyCode) {
				case Keys.Space:
					editor.SaveStateForUndo("Mark UVs");
			
					if (e.Shift) {
						ToggleMarkAll();
					} else {
						ToggleMarkSelected();
					}
					this.Refresh();
					break;
				case Keys.NumPad8:
					MoveMarked(-Vector2.UnitY, e.Alt);
					break;
				case Keys.NumPad2:
					MoveMarked(Vector2.UnitY, e.Alt);
					break;
				case Keys.NumPad4:
					MoveMarked(-Vector2.UnitX, e.Alt);
					break;
				case Keys.NumPad6:
					MoveMarked(Vector2.UnitX, e.Alt);
					break;
				case Keys.Z:
					if (e.Control) {
						editor.RestoreUndo();
						this.Refresh();
					}
					break;
			}
		}

		public void view_KeyDown(object sender, KeyEventArgs e)
		{
			UVEditor_KeyDown(this, e);
		}

		private void slider_grid_snap_Feedback(object sender, SliderLabelArgs e)
		{
			if (e.Increment > 0) {
				GRID_SNAP = Math.Min(1f, GRID_SNAP * 2f);
			} else if (e.Increment < 0) {
				GRID_SNAP = Math.Max(1f / 64f, GRID_SNAP / 2f);
			} else {
				return;
			}
			GRID_SNAP = Utility.SnapValue(GRID_SNAP, 1f / 64f);
			UpdateOptionLabels();
		}

		public void UpdateOptionLabels()
		{
			slider_grid_snap.ValueText = Utility.ConvertFloatTo1Thru3Dec(GRID_SNAP);
		}

		private void view_MouseEnter(object sender, EventArgs e)
		{
			this.Focus();
		}

		private void button_snap_to_grid_Click(object sender, EventArgs e)
		{
			editor.SaveStateForUndo("UV snap to grid");
			SnapMarkedVerts();
			this.Refresh();
		}

		private void button_snap_marked_pairs_Click(object sender, EventArgs e)
		{
			editor.SaveStateForUndo("UV snap pairs");
			SnapMarkedPairs();
			this.Refresh();
		}

		private void button_snap_selected_Click(object sender, EventArgs e)
		{
			editor.SaveStateForUndo("UV snap to selected");
			SnapToSelected();
			this.Refresh();
		}

		private void label_tex_from_sel_poly_MouseClick(object sender, MouseEventArgs e)
		{
			tex_from_sel_poly = !tex_from_sel_poly;
			label_tex_from_sel_poly.Text = "Tex From Sel Poly: " + (tex_from_sel_poly ? "ON" : "OFF");
		}

		private void button_uv_copy_Click(object sender, EventArgs e)
		{
			CopyUVs();
		}

		private void button_uv_paste_Click(object sender, EventArgs e)
		{
			PasteUVs();
		}
	}
}
