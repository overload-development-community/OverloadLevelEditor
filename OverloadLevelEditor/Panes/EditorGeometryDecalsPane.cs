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
	public partial class EditorGeometryDecalsPane : EditorDockContent
	{
		public EditorGeometryDecalsPane( EditorShell shell )
			: base( shell )
		{
			InitializeComponent();
		}

		private void EditorGeometryDecalsPane_Load( object sender, EventArgs e )
		{
			// Set colors to match the clip planes
			label_decal_clip_right.BackColor = GLView.C_decal_clip[(int)EdgeOrder.RIGHT];
			label_decal_clip_down.BackColor = GLView.C_decal_clip[(int)EdgeOrder.DOWN];
			label_decal_clip_left.BackColor = GLView.C_decal_clip[(int)EdgeOrder.LEFT];
			label_decal_clip_up.BackColor = GLView.C_decal_clip[(int)EdgeOrder.UP];
		}

		public void UpdateOptionsLabels()
		{
			var cp_display = ActiveDocument.m_cp_display;
			label_draw_clips.Text = "Clip Planes: " + cp_display.ToString();
			var insert_decal = ActiveDocument.m_insert_decal;
			label_decal_insert.Text = "Insert Decal: " + insert_decal.ToString();
		}

		void UpdateActiveDecalLabels()
		{
			var d = ActiveLevel.GetSelectedDecal( ActiveDocument.m_decal_active );
			bool applied = d != null && d.Applied;
			label_decal_alignment.Text = "Align: " + ( applied ? d.align.ToString() : "" );
			slider_decal_rotation.ValueText = applied ? ( d.rotation * 45 ).ToString() : "";
			label_decal_mirror.Text = "Mirror: " + ( applied ? d.mirror.ToString() : "" );
			slider_decal_repeat_u.ValueText = applied ? ( d.repeat_u > 0 ? d.repeat_u.ToString() : "MAX" ) : "";
			slider_decal_repeat_v.ValueText = applied ? ( d.repeat_v > 0 ? d.repeat_v.ToString() : "MAX" ) : "";
			slider_decal_offset_u.ValueText = applied ? ( d.offset_u.ToString() + "/4" ) : "";
			slider_decal_offset_v.ValueText = applied ? ( d.offset_v.ToString() + "/4" ) : "";
			label_decal_clip_up.Text = applied ? d.clip[(int)EdgeOrder.UP].ToString() : "";
			label_decal_clip_down.Text = applied ? d.clip[(int)EdgeOrder.DOWN].ToString() : "";
			label_decal_clip_left.Text = applied ? d.clip[(int)EdgeOrder.LEFT].ToString() : "";
			label_decal_clip_right.Text = applied ? d.clip[(int)EdgeOrder.RIGHT].ToString() : "";

			// Enable/disable controls based on active decal
			label_decal_alignment.Enabled = label_decal_mirror.Enabled =
				slider_decal_rotation.Enabled = slider_decal_repeat_u.Enabled = slider_decal_repeat_v.Enabled = slider_decal_offset_u.Enabled = slider_decal_offset_v.Enabled =
				button_preset_adj_lr.Enabled = button_preset_adj_ud.Enabled = button_preset_avg_lr.Enabled = button_preset_avg_ud.Enabled = button_preset_none.Enabled = button_preset_whole.Enabled = applied;
		}

		public void UpdateDecalLabels()
		{
			var editor = ActiveDocument;
			var level = ActiveLevel;

			// Store decal controls in arrays so we can access them in a loop
			System.Diagnostics.Debug.Assert( Side.NUM_DECALS == 2 );
			Button[] decal_marked_hide_buttons = new Button[] { button_decal_hide_marked1, button_decal_hide_marked2 };
			Button[] decal_copy_buttons = new Button[] { button_decal_copy_1, button_decal_copy_2 };
			Button[] decal_hide_buttons = new Button[] { button_decal_hide1, button_decal_hide2 };
			Label[] decal_labels = new Label[] { label_decal1, label_decal2 };

			// Clear active decal if not active
			{
				var curr_selected_decal = level.GetSelectedDecal( editor.m_decal_active );
				if( curr_selected_decal == null || !curr_selected_decal.Applied ) {
					editor.m_decal_active = -1;
				}
			}

			//Needed below
			bool any_marked = ( ( level.num_marked_sides > 0 ) && !( ( level.num_marked_sides == 1 ) && ( level.GetSelectedSide() != null ) && level.GetSelectedSide().marked ) );

			//Update for each decal on a side
			for( int i = 0; i < Side.NUM_DECALS; i++ ) {

				//Determine if this decal is active
				Decal d = level.GetSelectedDecal( i );
				bool applied = d != null && d.Applied;

				//If there's no active decal but this one is applied, make it active
				if( ( editor.m_decal_active == -1 ) && applied ) {
					editor.m_decal_active = i;
				}

				//Update labels, hide buttons, copy-to-marked buttons
				bool hidden = ( applied && d.hidden );
				decal_labels[i].Text = ( hidden ? "HIDDEN " : "" ) + ( i + 1 ) + ": " + ( applied ? d.mesh_name : "-" );
				decal_labels[i].BackColor = ( editor.m_decal_active == i ) ? Color.Gold : SystemColors.ControlDark;
				decal_labels[i].Enabled = applied;
				decal_hide_buttons[i].Text = ( hidden ? "Unhide" : "Hide" ) + ( i + 1 );
				decal_hide_buttons[i].Enabled = applied;
				decal_copy_buttons[i].Enabled = ( applied && any_marked );

				//Update hide/unhide marked decal buttons
				bool any_hidden = false;
				List<Decal> marked_applied_decals = level.GetMarkedAppliedDecals( i );
				foreach( Decal decal in marked_applied_decals ) {
					if( decal.hidden ) {
						any_hidden = true;
						break;
					}
				}
				decal_marked_hide_buttons[i].Enabled = ( marked_applied_decals.Count > 0 );
				decal_marked_hide_buttons[i].Text = ( any_hidden ? "Unhide" : "Hide" ) + " Marked Decals " + ( i + 1 );
			}

			// Update text of controls that are based on the active decal
			UpdateActiveDecalLabels();
		}

		private void button_decal_hide_marked1_Click( object sender, EventArgs e )
		{
			ActiveDocument.ToggleMarkedDecalHidden( 0 );
		}

		private void button_decal_hide_marked2_Click( object sender, EventArgs e )
		{
			ActiveDocument.ToggleMarkedDecalHidden( 1 );
		}
		private void button_decal_hide1_Click( object sender, EventArgs e )
		{
			ActiveDocument.ToggleDecalHidden( 0 );
		}

		private void button_decal_hide2_Click( object sender, EventArgs e )
		{
			ActiveDocument.ToggleDecalHidden( 1 );
		}
		private void button_preset_avg_lr_Click( object sender, EventArgs e )
		{
			ActiveDocument.SetDecalPresetClip( DecalClip.SEGMENT, DecalClip.NONE, DecalClip.SEGMENT, DecalClip.NONE );
		}

		private void button_preset_avg_ud_Click( object sender, EventArgs e )
		{
			ActiveDocument.SetDecalPresetClip( DecalClip.NONE, DecalClip.SEGMENT, DecalClip.NONE, DecalClip.SEGMENT);
		}

		private void button_preset_adj_lr_Click( object sender, EventArgs e )
		{
			ActiveDocument.SetDecalPresetClip( DecalClip.ADJACENT, DecalClip.NONE, DecalClip.ADJACENT, DecalClip.NONE );
		}

		private void button_preset_adj_ud_Click( object sender, EventArgs e )
		{
			ActiveDocument.SetDecalPresetClip( DecalClip.NONE, DecalClip.ADJACENT, DecalClip.NONE, DecalClip.ADJACENT );
		}

		private void button_preset_none_Click( object sender, EventArgs e )
		{
			ActiveDocument.SetDecalPresetClip( DecalClip.NONE, DecalClip.NONE, DecalClip.NONE, DecalClip.NONE );
		}

		private void button_preset_whole_Click( object sender, EventArgs e )
		{
			ActiveDocument.ResetDecalSettings();
      }

		private void label_draw_clips_MouseDown( object sender, MouseEventArgs e )
		{
			ActiveDocument.CycleDecalClipDisplay();
		}
		private void slider_decal_repeat_u_Feedback( object sender, SliderLabelArgs e )
		{
			ActiveDocument.ChangeDecalRepeat( e.Increment, 0);
		}

		private void slider_decal_repeat_v_Feedback( object sender, SliderLabelArgs e )
		{
			ActiveDocument.ChangeDecalRepeat( 0, e.Increment);
		}

		private void slider_decal_offset_u_Feedback( object sender, SliderLabelArgs e )
		{
			ActiveDocument.ChangeDecalOffset( e.Increment, 0);
		}

		private void slider_decal_offset_v_Feedback( object sender, SliderLabelArgs e )
		{
			ActiveDocument.ChangeDecalOffset( 0, e.Increment);
		}

		private void button_decal_copy_1_Click( object sender, EventArgs e )
		{
			ActiveDocument.CopyDecalPropertiesToMarked( 0 );
		}

		private void button_decal_copy_2_Click( object sender, EventArgs e )
		{
			ActiveDocument.CopyDecalPropertiesToMarked( 1 );
		}

		private void slider_decal_rotation_Feedback( object sender, SliderLabelArgs e )
		{
			ActiveDocument.ChangeDecalRotation( e.Increment);
		}
		private void label_decal1_MouseDown( object sender, MouseEventArgs e )
		{
			ActiveDocument.SetActiveDecal( 0 );
		}

		private void label_decal2_MouseDown( object sender, MouseEventArgs e )
		{
			ActiveDocument.SetActiveDecal( 1 );
		}

		private void label_decal_alignment_MouseDown( object sender, MouseEventArgs e )
		{
			ActiveDocument.CycleDecalAlignment( e.Button == MouseButtons.Right );
		}

		private void label_decal_mirror_MouseDown( object sender, MouseEventArgs e )
		{
			ActiveDocument.CycleDecalMirror();
		}

		private void label_decal_clip_up_MouseDown( object sender, MouseEventArgs e )
		{
			ActiveDocument.CycleDecalClip( (int)EdgeOrder.UP, e.Button == MouseButtons.Right );
		}

		private void label_decal_clip_right_MouseDown( object sender, MouseEventArgs e )
		{
			ActiveDocument.CycleDecalClip( (int)EdgeOrder.RIGHT, e.Button == MouseButtons.Right );
		}

		private void label_decal_clip_down_MouseDown( object sender, MouseEventArgs e )
		{
			ActiveDocument.CycleDecalClip( (int)EdgeOrder.DOWN, e.Button == MouseButtons.Right );
		}

		private void label_decal_clip_left_MouseDown( object sender, MouseEventArgs e )
		{
			ActiveDocument.CycleDecalClip( (int)EdgeOrder.LEFT, e.Button == MouseButtons.Right );
		}

		private void button_decal_list_Click( object sender, EventArgs e )
		{
			ActiveDocument.ShowDecalList();
		}

		private void label_decal_insert_MouseDown(object sender, MouseEventArgs e)
		{
			ActiveDocument.CycleInsertDecal();
		}
	}
}
