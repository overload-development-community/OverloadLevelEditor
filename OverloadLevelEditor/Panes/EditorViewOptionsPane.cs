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

namespace OverloadLevelEditor
{
	public partial class EditorViewOptionsPane : EditorDockContent
	{
		public EditorViewOptionsPane( EditorShell shell )
			: base( shell )
		{
			InitializeComponent();
		}

		public void UpdateOptionLabels()
		{
			var editor = ActiveDocument;
			var grid_spacing = editor.m_grid_spacing;
			var grid_snap = editor.m_grid_snap;
			var grid_display = editor.m_grid_display;
			var view_layout = editor.m_view_layout;
			var view_mode_persp = editor.m_view_mode_persp;
			var view_mode_ortho = editor.m_view_mode_ortho;
			var bg_color = editor.m_bg_color;
			var gimbal_display = editor.m_gimbal_display;
			var lighting_type = editor.m_lighting_type;
			var show_3d_text = editor.m_show_3d_text_type;
			var cutter_display = editor.m_cutter_display;

			slider_grid_spacing.ValueText = grid_spacing.ToString();
			slider_grid_snap.ValueText = grid_snap.ToString() + ( grid_snap >= 1f ? ".0" : "" );
			label_grid_display.Text = "Display: " + grid_display.ToString();
			label_view_layout.Text = "Layout: " + view_layout.ToString();
			label_view_persp.Text = "Persp: " + view_mode_persp.ToString();
			label_view_ortho.Text = "Ortho: " + view_mode_ortho.ToString();
			label_view_dark.Text = "Background: " + bg_color.ToString();
			label_gimbal.Text = "Gimbal: " + gimbal_display.ToString();
			label_lighting.Text = "Lighting: " + lighting_type.ToString();
			label_show_segment_numbers.Text = "Show Text: " + show_3d_text.ToString();
			label_auto_center.Text = "Auto-center: " + ( editor.m_auto_center ? "ON" : "OFF" );
			label_show_cutters.Text = "Split Planes: " + cutter_display.ToString();
		}

		private void label_auto_center_Click( object sender, EventArgs e )
		{
			ActiveDocument.ToggleAutoCenter();
		}

		private void label_show_segment_numbers_MouseDown( object sender, MouseEventArgs e )
		{
			ActiveDocument.CycleShowSegmentNumbers(e.Button == MouseButtons.Right);
		}

		private void label_lighting_MouseDown( object sender, MouseEventArgs e )
		{
			ActiveDocument.CycleLightingType();
		}

		private void label_gimbal_MouseDown( object sender, MouseEventArgs e )
		{
			ActiveDocument.CycleGimbalDisplay();
		}

		private void label_view_dark_MouseDown( object sender, MouseEventArgs e )
		{
			ActiveDocument.CycleBGColor();
		}

		private void label_view_ortho_MouseDown( object sender, MouseEventArgs e )
		{
			ActiveDocument.CycleViewModeOrtho( e.Button == MouseButtons.Right );
		}

		private void label_view_persp_MouseDown( object sender, MouseEventArgs e )
		{
			ActiveDocument.CycleViewModePersp( e.Button == MouseButtons.Right );
		}

		private void label_view_layout_MouseDown( object sender, MouseEventArgs e )
		{
			ActiveDocument.CycleViewLayout( e.Button == MouseButtons.Right );
		}
		
		private void slider_grid_spacing_Feedback( object sender, SliderLabelArgs e )
		{
			ActiveDocument.ChangeGridSpacing( e.Increment );
		}

		private void slider_grid_snap_Feedback( object sender, SliderLabelArgs e )
		{
			ActiveDocument.ChangeGridSnap( e.Increment );
		}

		private void button_snap_marked_Click( object sender, EventArgs e )
		{
			ActiveDocument.SnapMarkedElementsToGrid();
		}

		private void label_grid_display_MouseDown( object sender, MouseEventArgs e )
		{
			ActiveDocument.CycleGridDisplay();
		}

		private void label_show_cutters_MouseClick(object sender, MouseEventArgs e)
		{
			ActiveDocument.CycleCutterDisplay();
		}
	}
}
