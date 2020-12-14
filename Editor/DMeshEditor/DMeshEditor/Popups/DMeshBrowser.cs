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
using System.IO;

namespace OverloadLevelEditor
{
	public partial class DMeshBrowser : Form
	{
		public Editor editor;
		public bool loaded = false;

		private List<string> m_decal_list = new List<string>();

		public DMeshBrowser(Editor e)
		{
			editor = e;
			InitializeComponent();
			MinimumSize = new Size(Width, 200);
			MaximumSize = new Size(Width, 2048);
		}

		private void ListboxUpdate()
		{
			if (!loaded) {
				string filter_text = textBox_filter.Text;
				string old_selected = (string)listbox.SelectedItem;

				listbox.BeginUpdate();

				listbox.Items.Clear();

				if (string.IsNullOrEmpty(filter_text)) {
					foreach (string s in m_decal_list) {
						listbox.Items.Add(s);
					}
				} else {
					foreach (string s in m_decal_list) {
						if (System.Globalization.CultureInfo.CurrentCulture.CompareInfo.IndexOf(s, filter_text, System.Globalization.CompareOptions.IgnoreCase) >= 0) {
							listbox.Items.Add(s);
						}
					}
				}

				//Try to select the old selected
				listbox.SelectedItem = old_selected;
				if (listbox.SelectedItem == null) {    //Failed to find old item
					if (listbox.Items.Count == 0) {
						listbox.SelectedIndex = -1;
						listbox_SelectedIndexChanged(null, null);
					} else {
						listbox.SelectedIndex = 0;
					}
				}

				listbox.EndUpdate();
				loaded = true;
			}
		}

		public void LoadDecalNamesInDir(string dir, bool all_dir = false)
		{
			string[] files = Directory.GetFiles(dir, "*.dmesh", (all_dir ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly));

			m_decal_list.Clear();
			foreach (string file in files) {
				// Remove all the extra stuff to get the names
				string mesh_name = Utility.GetRelativeExtensionlessFilenameFromDirectory(dir, file);
				m_decal_list.Add(mesh_name);
			}
		}

		private void DMeshBrowser_Shown(object sender, EventArgs e)
		{
			LoadDecalNamesInDir(editor.m_filepath_decals);
			textBox_filter.Text = "";
			TryToSelectOpenDMesh();
			ListboxUpdate();
      }

		public void TryToSelectOpenDMesh()
		{
			if (editor.m_filepath_current_decal != null && editor.m_filepath_current_decal != "") {
				string short_name = Utility.GetPathlessFilename(editor.m_filepath_current_decal);
				if (short_name != null && short_name != "") {
					listbox.SelectedItem = short_name;
				}
			}
		}

		private void listbox_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (listbox.SelectedIndex > -1 && listbox.SelectedItem != null) {
				// Maybe load the selected decal
				if (m_auto_load) {
					MaybeSave();
					LoadDecal((string)listbox.SelectedItem); 
				}
			}
		}

		public void MaybeSave()
		{
			if (m_auto_save) {
				if (editor.m_dmesh.triangle.Count > 0) {
					editor.saveToolStripMenuItem_Click(this, null);
				}
			}
		}

		public void LoadDecal(string name)
		{
			string filename = editor.m_filepath_decals + "\\" + name + ".dmesh";
			if (File.Exists(filename)) {
				if (editor.LoadDecalMesh(editor.m_dmesh, filename)) {
					editor.RefreshGeometry();
				}
			} else {
				Utility.DebugPopup("Could not find file.  Loading aborted", "LOADING ERROR");
			}
		}


		private void textBox_filter_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			if (e.KeyChar == 27) {
				this.textBox_filter.Text = "";
				e.Handled = true;
			}
		}

		private void textBox_filter_TextChanged(object sender, EventArgs e)
		{
			ListboxUpdate();
		}

		private void button_clear_filter_Click(object sender, EventArgs e)
		{
			this.textBox_filter.Text = "";
		}

		public bool m_auto_save = false;
		public bool m_auto_load = true;

		private void label_auto_save_MouseDown(object sender, MouseEventArgs e)
		{
			m_auto_save = !m_auto_save;
			UpdateLabels();
		}

		private void label_auto_load_MouseDown(object sender, MouseEventArgs e)
		{
			m_auto_load = !m_auto_load;
			button_load_selected.Enabled = !m_auto_load;
			UpdateLabels();
		}

		public void UpdateLabels()
		{
			label_auto_save.Text = "Save: " + (m_auto_save ? "ON" : "OFF");
			label_auto_load.Text = "Load: " + (m_auto_load ? "ON" : "OFF");
		}

		private void button_load_selected_Click(object sender, EventArgs e)
		{
			if (listbox.SelectedItem != null) {
				MaybeSave();
				LoadDecal((string)listbox.SelectedItem);
			}
		}

		private void DMeshBrowser_FormClosing(object sender, FormClosingEventArgs e)
		{
			// Hide it instead of closing it
			e.Cancel = true;
			this.Hide();
		}
	}
}
