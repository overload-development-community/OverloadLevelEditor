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
	public partial class EditorLevelGlobalPane : EditorDockContent
	{
		public EditorLevelGlobalPane( EditorShell shell )
			: base( shell )
		{
			InitializeComponent();
		}

		public void UpdateLabels()
		{
			LevelGlobalData lgd = ActiveLevel.global_data;

			slider_cave_simplify.ValueText = Utility.ConvertFloatTo2Dec(lgd.simplify_strength);
			label_cave_grid.Text = "Grid: " + lgd.grid_size + "x" + lgd.grid_size;
			label_cave_presmooth.Text = "PreProcess Smooth: " + lgd.pre_smooth;
			label_cave_postsmooth.Text = "PostProcess Smooth: " + lgd.post_smooth;
			label_cave_preset1.Text = "Preset 1: " + lgd.deform_presets[0].ToString();
			label_cave_preset2.Text = "Preset 2: " + lgd.deform_presets[1].ToString();
			label_cave_preset3.Text = "Preset 3: " + lgd.deform_presets[2].ToString();
			label_cave_preset4.Text = "Preset 4: " + lgd.deform_presets[3].ToString();
		}

		private void slider_cave_simplify_Feedback(object sender, SliderLabelArgs e)
		{
			ActiveDocument.ChangeCaveSimplify(e.Increment);
		}

		private void label_cave_grid_MouseDown(object sender, MouseEventArgs e)
		{
			ActiveDocument.CycleCaveGrid(e.Button == MouseButtons.Right);
		}

		private void label_cave_presmooth_MouseDown(object sender, MouseEventArgs e)
		{
			ActiveDocument.CycleCavePreSmooth(e.Button == MouseButtons.Right);
		}

		private void label_cave_postsmooth_MouseDown(object sender, MouseEventArgs e)
		{
			ActiveDocument.CycleCavePostSmooth(e.Button == MouseButtons.Right);
		}

		private void label_cave_preset1_MouseDown(object sender, MouseEventArgs e)
		{
			ActiveDocument.CycleCavePreset(0, e.Button == MouseButtons.Right);
		}

		private void label_cave_preset2_MouseDown(object sender, MouseEventArgs e)
		{
			ActiveDocument.CycleCavePreset(1, e.Button == MouseButtons.Right);
		}

		private void label_cave_preset3_MouseDown(object sender, MouseEventArgs e)
		{
			ActiveDocument.CycleCavePreset(2, e.Button == MouseButtons.Right);
		}

		private void label_cave_preset4_MouseDown(object sender, MouseEventArgs e)
		{
			ActiveDocument.CycleCavePreset(3, e.Button == MouseButtons.Right);
		}
	}
}
