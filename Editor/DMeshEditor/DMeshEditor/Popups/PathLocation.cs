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
	public partial class PathLocation : Form
	{
		Editor editor;

		public PathLocation(Editor e)
		{
			editor = e;
			InitializeComponent();
		}

		private void button_browse1_Click(object sender, EventArgs e)
		{
			FolderBrowserDialog fbd = new FolderBrowserDialog();
			fbd.SelectedPath = editor.m_filepath_root;
			DialogResult dr = fbd.ShowDialog();

			if (dr == DialogResult.OK || dr == DialogResult.Yes) {
				editor.AddOutputText("Selected directory: " + fbd.SelectedPath);
				editor.m_filepath_root = fbd.SelectedPath;
				UpdateLabels();
			}
		}

		private void PathLocation_Load(object sender, EventArgs e)
		{
			UpdateLabels();
		}

		public void UpdateLabels()
		{
			label_dir1.Text = editor.m_filepath_root;
			label_dir2.Text = editor.m_filepath_decal_textures;
			label_dir3.Text = editor.m_filepath_level_textures;
			label_dir4.Text = editor.m_filepath_decals;
		}

		private void button_reset_defaults_Click(object sender, EventArgs e)
		{
			editor.UpdateDirectories();
			UpdateLabels();
		}

		private void button_browse2_Click(object sender, EventArgs e)
		{
			FolderBrowserDialog fbd = new FolderBrowserDialog();
			fbd.SelectedPath = editor.m_filepath_decal_textures;
			DialogResult dr = fbd.ShowDialog();

			if (dr == DialogResult.OK || dr == DialogResult.Yes) {
				editor.AddOutputText("Selected directory: " + fbd.SelectedPath);
				editor.m_filepath_decal_textures = fbd.SelectedPath;
				UpdateLabels();
			}
		}

		private void button_browse3_Click(object sender, EventArgs e)
		{
			FolderBrowserDialog fbd = new FolderBrowserDialog();
			fbd.SelectedPath = editor.m_filepath_level_textures;
			DialogResult dr = fbd.ShowDialog();

			if (dr == DialogResult.OK || dr == DialogResult.Yes) {
				editor.AddOutputText("Selected directory: " + fbd.SelectedPath);
				editor.m_filepath_level_textures = fbd.SelectedPath;
				UpdateLabels();
			}
		}

		private void button_browse4_Click(object sender, EventArgs e)
		{
			FolderBrowserDialog fbd = new FolderBrowserDialog();
			fbd.SelectedPath = editor.m_filepath_decals;
			DialogResult dr = fbd.ShowDialog();

			if (dr == DialogResult.OK || dr == DialogResult.Yes) {
				editor.AddOutputText("Selected directory: " + fbd.SelectedPath);
				editor.m_filepath_decals = fbd.SelectedPath;
				UpdateLabels();
			}
		}
	}
}
