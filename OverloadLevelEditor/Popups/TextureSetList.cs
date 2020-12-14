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
	public partial class TextureSetList : Form
	{
		private Editor m_editor;
		private TextureSet m_selected_set;
		private bool m_modified;

		private void SetModified(bool modified)
		{
			m_modified = modified;
			UpdateButtons();
		}

		public TextureSetList(Editor e)
		{
			m_editor = e;

			InitializeComponent();
		}

		private string selected_text = " (selected)";

		private string GetSelectedItemText()
		{
			string text = (comboBoxTextureSets.SelectedItem == null) ? "" : comboBoxTextureSets.SelectedItem.ToString();

			if (text.EndsWith(selected_text)) {
				text = text.Substring(0, text.Length - selected_text.Length);
			}

			return text;
		}

		private string DisplayName(string name)
		{
			if (name == m_editor.m_level.m_texture_set_name) {
				return name + selected_text;
			}

			return name;
		}

		private void UpdateComboBox()
		{
			string old_selected = GetSelectedItemText();
			int selected_index = 0;

			comboBoxTextureSets.Items.Clear();
			if (m_editor.m_level != null) {
				int index = 0;
				foreach (TextureSet texture_set in m_editor.TextureSets) {
					string name = texture_set.Name;

					if (name == old_selected) {
						selected_index = index;
					}

					comboBoxTextureSets.Items.Add(DisplayName(name));

					index++;
				}
			}

			comboBoxTextureSets.SelectedIndex = selected_index;
		}

		private void UpdateButtons()
		{
			buttonDone.Enabled = true;
			buttonCancel.Enabled = buttonApply.Enabled = m_modified;
			buttonDelete.Enabled = (m_editor.TextureSets.Count > 1);
		}

		public void TextureSetList_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (m_modified) {
				DialogResult result = MessageBox.Show("Save changes to texture sets?", "Save Changes", MessageBoxButtons.YesNoCancel);
				if (result == DialogResult.Yes) {
					SaveChanges();
				} else if (result == DialogResult.No) {
					DiscardChanges();
				} else if (result == DialogResult.Cancel) {
					e.Cancel = true;
					return;
				}
			}

			if (e.CloseReason != CloseReason.FormOwnerClosing) {
				this.Hide();
				e.Cancel = true;
			}
		}

		private void TextureSetList_LocationChanged(object sender, EventArgs e)
		{
			if (Visible) {
				m_editor.m_texture_set_list_loc = Location;
			}
		}

		private void UpdateTextures()
		{
			labelWallName.Text = m_selected_set.Wall;
			labelFloorName.Text = m_selected_set.Floor;
			labelCeilingName.Text = m_selected_set.Ceiling;
			labelCaveName.Text = m_selected_set.Cave;

			pictureBoxWall.Image = m_editor.tm_level.m_bitmap[m_editor.tm_level.FindTextureIndexByName(m_selected_set.Wall)];
			pictureBoxFloor.Image = m_editor.tm_level.m_bitmap[m_editor.tm_level.FindTextureIndexByName(m_selected_set.Floor)];
			pictureBoxCeiling.Image = m_editor.tm_level.m_bitmap[m_editor.tm_level.FindTextureIndexByName(m_selected_set.Ceiling)];
			pictureBoxCave.Image = m_editor.tm_level.m_bitmap[m_editor.tm_level.FindTextureIndexByName(m_selected_set.Cave)];
		}

		private void comboBoxTextureSets_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (comboBoxTextureSets.SelectedIndex != -1) {
				m_selected_set = m_editor.TextureSets[comboBoxTextureSets.SelectedIndex];
				UpdateTextures();
			}
		}

		private int GetTextureIndex()
		{
			bool restore_texture_list = false;

			//Close texture list if it's open
			if (m_editor.ActiveDocument.texture_list.Visible) {
				m_editor.ActiveDocument.texture_list.TextureList_FormClosing(this, new FormClosingEventArgs(CloseReason.UserClosing, false));
				restore_texture_list = true;
			}

			//Open it as modal
			m_editor.ActiveDocument.texture_list.SelectedTextureIndex = -1;
			m_editor.ActiveDocument.texture_list.StartPosition = FormStartPosition.Manual;
			m_editor.ActiveDocument.texture_list.Location = m_editor.ActiveDocument.m_tex_list_loc;
			m_editor.ActiveDocument.texture_list.ShowDialog();

			if (restore_texture_list) {
				m_editor.ActiveDocument.ShowTextureList();
			}

			return m_editor.ActiveDocument.texture_list.SelectedTextureIndex;
		}

		private void UpdateTexture(ref string texture_name)
		{
			int index = GetTextureIndex();

			if (index == -1) {
				return;
			}

			if (texture_name != m_editor.tm_level.m_name[index]) {
				texture_name = m_editor.tm_level.m_name[index];
				SetModified(true);
				UpdateTextures();
			}

		}

		private void pictureBoxWall_Click(object sender, EventArgs e)
		{
			UpdateTexture(ref m_selected_set.Wall);
		}

		private void pictureBoxFloor_Click(object sender, EventArgs e)
		{
			UpdateTexture(ref m_selected_set.Floor);
		}

		private void pictureBoxCeiling_Click(object sender, EventArgs e)
		{
			UpdateTexture(ref m_selected_set.Ceiling);
		}

		private void pictureBoxCave_Click(object sender, EventArgs e)
		{
			UpdateTexture(ref m_selected_set.Cave);
		}

		private void buttonDelete_Click(object sender, EventArgs e)
		{
			if (m_editor.TextureSets.Count == 1) {
				MessageBox.Show("You cannot delete the only texture set", "Delete Texture Set");
				return;
			}
			DialogResult result = MessageBox.Show("Are you sure you want to delete texture set '" + GetSelectedItemText() + "'?", "Delete Texture Set", MessageBoxButtons.YesNo);

			if (result == DialogResult.Yes) {
				m_editor.TextureSets.Remove(m_selected_set);
				SetModified(true);
				UpdateComboBox();
			}
		}

		private void buttonAddNew_Click(object sender, EventArgs e)
		{
			string new_name = InputBox.GetInput("New texture set", "Enter a name for the new texture set", "");

			if (string.IsNullOrWhiteSpace(new_name)) {
				return;
			}

			new_name = new_name.Trim();

			if (m_editor.TextureSets.Find(ts => (ts.Name == new_name)) != null) {
				MessageBox.Show("Invalid namne: There is already a texture set with that name.", "New Texture Set");
				return;
			}

			TextureSet new_texture_set = new TextureSet();
			new_texture_set.Name = new_name;
			new_texture_set.Wall = new_texture_set.Floor = new_texture_set.Ceiling = new_texture_set.Cave = m_editor.tm_level.m_name[0];

			m_editor.TextureSets.Add(new_texture_set);
			SetModified(true);
			UpdateComboBox();
			comboBoxTextureSets.SelectedItem = new_name;
		}

		private void buttonApply_Click(object sender, EventArgs e)
		{
			SaveChanges();
		}

		private void buttonDone_Click(object sender, EventArgs e)
		{
			SaveChanges();
			Close();
		}

		private void SaveChanges()
		{
			m_editor.SaveTextureSets();
			SetModified(false);
		}
		private void DiscardChanges()
		{
			m_editor.LoadTextureSets();               //Reload old values
			UpdateComboBox();
			SetModified(false);
		}

		private void buttonCancel_Click(object sender, EventArgs e)
		{
			if (m_modified) {
				DialogResult result = MessageBox.Show("Discard changes to texture sets?", "Discard Changes", MessageBoxButtons.OKCancel);
				if (result == DialogResult.Cancel) {
					return;
				}
				DiscardChanges();
			}

			Close();
		}

		private void buttonSelect_Click(object sender, EventArgs e)
		{
			m_editor.ActiveDocument.m_level.m_texture_set_name = m_selected_set.Name;
			UpdateComboBox();
		}

		private void TextureSetList_Load(object sender, EventArgs e)
		{
			UpdateComboBox();
			if (comboBoxTextureSets.Items.Count > 0) {
				comboBoxTextureSets.SelectedIndex = 0;
			}
			UpdateButtons();
		}

		private void buttonRenameTextureSet_Click(object sender, EventArgs e)
		{
			string new_name = InputBox.GetInput("New name", "Enter a new name for this texture set", GetSelectedItemText());

			if (string.IsNullOrWhiteSpace(new_name)) {
				return;
			}

			new_name = new_name.Trim();

			if (m_selected_set.Name == m_editor.m_level.m_texture_set_name) {
				m_editor.m_level.m_texture_set_name = new_name;
			}

			m_selected_set.Name = new_name;
			m_modified = true;
			comboBoxTextureSets.Items[comboBoxTextureSets.SelectedIndex] = DisplayName(new_name);
		}

		private void TextureSetList_Activated(object sender, EventArgs e)
		{
			//Update combo box for possible new selected texture set
			UpdateComboBox();
		}
	}
}
