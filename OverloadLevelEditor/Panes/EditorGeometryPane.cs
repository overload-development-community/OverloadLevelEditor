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
using OpenTK;

namespace OverloadLevelEditor
{
	public partial class EditorGeometryPane : EditorDockContent
	{
		public EditorGeometryPane( EditorShell shell )
			: base( shell )
		{
			InitializeComponent();
		}

		public void UpdateOptionsLabels()
		{
			var editor = ActiveDocument;
			var coplanar_tol = editor.m_coplanar_tol;
			var extrude_length = editor.m_extrude_length;
			var drag_mode = editor.m_drag_mode;
			var side_select = editor.m_side_select;

			slider_coplanar_angle.ValueText = coplanar_tol.ToString();
			int angle = editor.m_rotate_angle;  //Put in variable to avoid warning: "Accessing a member on 'Editor.m_rotate_angle' may cause a runtime exception because it is a field of a marshal-by-reference class"
			slider_rotate_angle.ValueText = angle.ToString();
			slider_extrude_length.ValueText = extrude_length.ToString();
			label_insert_advance.Text = "Insert Advance: " + ( editor.m_insert_advance ? "ON" : "OFF" );
			label_drag_select.Text = "Drag Mode: " + drag_mode.ToString();
			label_side_select.Text = "Side Select: " + side_select.ToString();
		}

		private void button_rotate_at_selected__CCW_Click( object sender, EventArgs e )
		{
			ActiveDocument.RotateAtSelectedSide(true);
		}

		private void button_connect_with_segment_Click( object sender, EventArgs e )
		{
			ActiveDocument.ConnectSelectedSideToMarkedWithSegment();
		}

		private void button_mark_walls_straight_Click( object sender, EventArgs e )
		{
			ActiveDocument.MarkWallSidesStraight();
		}

		private void slider_extrude_length_Feedback( object sender, SliderLabelArgs e )
		{
			ActiveDocument.ChangeExtrudeLength( e.Increment );
		}

		private void button_merge_verts_Click( object sender, EventArgs e )
		{
			var editor = ActiveDocument;
			var level = ActiveLevel;

			editor.SaveStateForUndo( "Merge overlapping verts" );
			level.MergeAllOverlappingVerts();
			editor.RefreshGeometry();
		}

		private void slider_coplanar_angle_Feedback( object sender, SliderLabelArgs e )
		{
			ActiveDocument.ChangeCoplanarAngle( e.Increment * 5 );
		}

		private void button_mark_walls_Click( object sender, EventArgs e )
		{
			ActiveDocument.MarkWallSides();
		}
		private void button_mark_coplanar_Click( object sender, EventArgs e )
		{
			ActiveDocument.MarkCoplanarSides();
		}

		private void button_join_all_Click( object sender, EventArgs e )
		{
			ActiveDocument.JoinMarkedSides();
		}

		private void button_join_side_Click( object sender, EventArgs e )
		{
			ActiveDocument.JoinSelectedSideToMarked();
		}
		private void button_isolate_marked_Click( object sender, EventArgs e )
		{
			ActiveDocument.IsolateMarkedSegments();
		}

		private void button_paste_mirrorx_Click( object sender, EventArgs e )
		{
			ActiveDocument.PasteMirror( Axis.X );
		}

		private void button_paste_mirrory_Click( object sender, EventArgs e )
		{
			ActiveDocument.PasteMirror( Axis.Y );
		}

		private void button_paste_mirrorz_Click( object sender, EventArgs e )
		{
			ActiveDocument.PasteMirror( Axis.Z );
		}

		private void button_paste_at_side_Click( object sender, EventArgs e )
		{
			ActiveDocument.PasteBufferSegments(true, false);
		}

		private void button_paste_at_side_with_entities_Click(object sender, EventArgs e)
		{
			ActiveDocument.PasteBufferSegments(true, true);
		}

		private void button_flipx_Click( object sender, EventArgs e )
		{
			ActiveDocument.MaybeFlipMarkedSegments( Axis.X );
		}

		private void button_flipy_Click( object sender, EventArgs e )
		{
			ActiveDocument.MaybeFlipMarkedSegments( Axis.Y );
		}

		private void button_flipz_Click( object sender, EventArgs e )
		{
			ActiveDocument.MaybeFlipMarkedSegments( Axis.Z );
		}

		private void label_side_select_MouseDown( object sender, MouseEventArgs e )
		{
			ActiveDocument.CycleSideSelect();
		}

		private void label_insert_advance_MouseDown( object sender, MouseEventArgs e )
		{
			ActiveDocument.CycleInsertAdvance();
		}
		private void label_drag_select_MouseDown( object sender, MouseEventArgs e )
		{
			ActiveDocument.CycleDragMode();
		}
		private void button_default_segment_Click( object sender, EventArgs e )
		{
			var editor = ActiveDocument;
			var level = ActiveLevel;

			editor.SaveStateForUndo( "Create default segment" );
			level.CreateSegmentAtPosition( Vector3.Zero, Vector3.Zero );
			editor.RefreshGeometry();
		}

		private void button_copy_marked_Click( object sender, EventArgs e )
		{
			ActiveDocument.CopyMarkedSegments();
		}

		private void button_paste_default_Click( object sender, EventArgs e )
		{
			ActiveDocument.PasteBufferSegments(false, false);
		}

		private void button_paste_default_with_entities_Click(object sender, EventArgs e)
		{
			ActiveDocument.PasteBufferSegments(false, true);
		}

		private void button_tunnel_builder_Click(object sender, EventArgs e)
		{
			ActiveDocument.ShowTunnelBuilder();
		}

		private void button_2way_Click(object sender, EventArgs e)
		{
			ActiveDocument.Split2Way();
		}

		private void button_5way_Click(object sender, EventArgs e)
		{
			ActiveDocument.Split5Way();
		}

		private void button_7way_Click(object sender, EventArgs e)
		{
			ActiveDocument.Split7Way();
		}

		private void slider_rotate_angle_Feedback(object sender, SliderLabelArgs e)
		{
			ActiveDocument.ChangeRotateAngle(e.Increment * 5);
		}

		private void button_rotate_at_selected_CW_Click(object sender, EventArgs e)
		{
			ActiveDocument.RotateAtSelectedSide(false);
		}
	}
}
