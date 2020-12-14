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
	public partial class EditorTexturingPane : EditorDockContent
	{
		public EditorTexturingPane( EditorShell shell )
			: base( shell )
		{
			InitializeComponent();
		}

		public void UpdateTextureLabels()
		{
			Segment seg = ActiveLevel.GetSelectedSegment();
			sliderLabelPathfinding.Enabled = sliderLabelDark.Enabled =  sliderLabelExitSegment.Enabled = (seg != null);
			if (seg != null) {
				sliderLabelPathfinding.ValueText = seg.m_pathfinding.ToString();
				sliderLabelExitSegment.ValueText = seg.m_exit_segment_type.ToString();
				sliderLabelDark.ValueText = seg.m_dark ? "YES" : "NO";

				Side s = ActiveLevel.GetSelectedSide();
				sliderLabelSplitPlaneOrder.Enabled = (s != null);
				if (s != null) {
					label_texture_name.Text = s.tex_name;

					slider_deformation_height.ValueText = Utility.ConvertFloatTo2Dec(s.deformation_height);

					sliderLabelSplitPlaneOrder.ValueText = (s.chunk_plane_order == -1) ? "OFF" : s.chunk_plane_order.ToString();

					// Cave preset
					button_cave1.BackColor = (s.deformation_preset == 0 ? Color.Yellow : SystemColors.Control);
					button_cave2.BackColor = (s.deformation_preset == 1 ? Color.Yellow : SystemColors.Control);
					button_cave3.BackColor = (s.deformation_preset == 2 ? Color.Yellow : SystemColors.Control);
					button_cave4.BackColor = (s.deformation_preset == 3 ? Color.Yellow : SystemColors.Control);
				}
			}
		}

		private void button_copy_def_height_Click( object sender, EventArgs e )
		{
			ActiveDocument.CopySideDeformationHeightToMarked();
		}

		private void slider_deformation_height_Feedback( object sender, SliderLabelArgs e )
		{
			ActiveDocument.ChangeSideDeformationHeight( e.Increment );
		}

		private void button_texture_center_u_Click( object sender, EventArgs e )
		{
			ActiveLevel.UVCenterU();
			ActiveDocument.RefreshGeometry();
		}

		private void button_texture_center_v_Click( object sender, EventArgs e )
		{
			ActiveLevel.UVCenterV();
			ActiveDocument.RefreshGeometry();
		}

		private void button_texture_show_list_Click( object sender, EventArgs e )
		{
			ActiveDocument.ShowTextureList();
		}

		private void button_texture_default_map_Click( object sender, EventArgs e )
		{
			ActiveLevel.UVDefaultMapMarkedSides();
			ActiveDocument.RefreshGeometry();
		}

		private void button_texture_snap4_Click( object sender, EventArgs e )
		{
			ActiveLevel.UVSnapToFraction( 4 );
			ActiveDocument.RefreshGeometry();
		}

		private void button_texture_snap8_Click( object sender, EventArgs e )
		{
			ActiveLevel.UVSnapToFraction( 8 );
			ActiveDocument.RefreshGeometry();
		}

		private void button_texture_planar_x_Click( object sender, EventArgs e )
		{
			ActiveLevel.UVPlanarMapMarkedSides( Axis.X );
			ActiveDocument.RefreshGeometry();
		}

		private void button_texture_planar_y_Click( object sender, EventArgs e )
		{
			ActiveLevel.UVPlanarMapMarkedSides( Axis.Y );
			ActiveDocument.RefreshGeometry();
		}

		private void button_texture_planar_z_Click( object sender, EventArgs e )
		{
			ActiveLevel.UVPlanarMapMarkedSides( Axis.Z );
			ActiveDocument.RefreshGeometry();
		}

		private void button_align_marked_Click( object sender, EventArgs e )
		{
			ActiveLevel.UVAlignToSide();
			ActiveDocument.RefreshGeometry();
		}
		private void button_texture_box_map_Click( object sender, EventArgs e )
		{
			ActiveLevel.UVBoxMapMarkedSides();
			ActiveDocument.RefreshGeometry();
		}

		private void button_cave1_Click(object sender, EventArgs e)
		{
			ActiveDocument.SetSideCavePreset(0);
		}

		private void button_cave2_Click(object sender, EventArgs e)
		{
			ActiveDocument.SetSideCavePreset(1);
		}

		private void button_cave3_Click(object sender, EventArgs e)
		{
			ActiveDocument.SetSideCavePreset(2);
		}

		private void button_cave4_Click(object sender, EventArgs e)
		{
			ActiveDocument.SetSideCavePreset(3);
		}

		private void button_mark_caves_Click(object sender, EventArgs e)
		{
			ActiveDocument.MarkSidesWithCavePreset(ActiveDocument.GetSelectedSideCavePreset());
		}

		private void button_mark_height_Click(object sender, EventArgs e)
		{
			ActiveDocument.MarkSidesWithCavePreset(ActiveDocument.GetSelectedSideCavePreset(), true);
		}

		private void sliderLabelSplitPlaneOrder_Feedback(object sender, SliderLabelArgs e)
		{
			ActiveDocument.ChangeSplitPlaneOrder(e.Increment);
		}

        private void sliderLabelPathfinding_Feedback(object sender, SliderLabelArgs e)
        {
            ActiveDocument.CyclePathfinding(e.Increment);
        }

		private void sliderLabelExitSegment_Feedback(object sender, SliderLabelArgs e)
		{
			ActiveDocument.CycleExitSegment(e.Increment);
		}

		private void sliderLabelDark_Feedback(object sender, SliderLabelArgs e)
        {
            ActiveDocument.ToggleDark();
        }
    }
}
