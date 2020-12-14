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
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Linq;

// TEXTURELIST
// Windows form that lists all the textures
// Can also set/find the texture to the current side

namespace OverloadLevelEditor
{
	public partial class TextureList : Form
	{
		// START - Allowing listview spacing settings
		[DllImport("user32.dll")]
		public static extern int SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
		[DllImport("user32.dll", CharSet = CharSet.Unicode)]
		private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, [MarshalAs(UnmanagedType.LPWStr)] string lp);

		public int MakeLong(short lowPart, short highPart)
		{
			return (int)(((ushort)lowPart) | (uint)(highPart << 16));
		}

		public void ListViewItem_SetSpacing(ListView listview, short leftPadding, short topPadding)
		{
			try {
				const int LVM_FIRST = 0x1000;
				const int LVM_SETICONSPACING = LVM_FIRST + 53;
				SendMessage(listview.Handle, LVM_SETICONSPACING, IntPtr.Zero, (IntPtr)MakeLong(leftPadding, topPadding));
			}
			catch {
				// Smother exception on non-Windows platforms
			}
		}

		// END - Allowing listview spacing settings

		public Editor editor;

		public TextureManager m_tex_manager;

		public TextureList(Editor e)
		{
			editor = e;
			m_tex_manager = editor.tm_level;

			InitializeComponent();

			MinimumSize = new Size(Width, 0);
			MaximumSize = new Size(Width, 2048);
		}

		public void TextureList_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (!this.Modal) {
				if (e.CloseReason != CloseReason.FormOwnerClosing) {
					this.Hide();
					e.Cancel = true;
				}
			}
		}

		public ListViewItem[] m_list_items;
		public ImageList m_image_list_large;
		public ImageList m_image_list_small;
		public Bitmap m_bitmap;
		public Font m_list_font;
		public Font m_list_font_tiny;

		private ContextMenu m_texture_context_menu;

		private bool first_time = true;

		private void TextureList_Load(object sender, EventArgs e)
		{
			if (!first_time) {
				return;
			}
			first_time = false;

			//Set cue text
			if (textBox_Filter.IsHandleCreated) {
				SendMessage(textBox_Filter.Handle, 0x1501, (IntPtr)1, "Filter Text");
			}

			if (comboBox_TextureCollection.IsHandleCreated) {
				SendMessage(comboBox_TextureCollection.Handle, 0x1703, (IntPtr)0, "Texture Collection");
			}

			//Initialize collections combo box
			comboBox_TextureCollection.Items.Clear();
			foreach (Editor.TextureCollection collection in editor.TextureCollections) {
				comboBox_TextureCollection.Items.Add(collection.Name);
			}

			m_texture_context_menu = new ContextMenu();

			CollectionControlsUpdate();
		}

		// Must not be called until after textures are loaded by the level texture manager
		public void InitImageLists()
		{
			m_image_list_large = new ImageList();
			m_image_list_large.ColorDepth = ColorDepth.Depth32Bit;
			m_image_list_large.ImageSize = new Size(128, 128);

			m_image_list_small = new ImageList();
			m_image_list_small.ColorDepth = ColorDepth.Depth32Bit;
			m_image_list_small.ImageSize = new Size(64, 64);

			int num_bitmaps = m_tex_manager.m_bitmap.Count;
			for (int i = 0; i < num_bitmaps; i++) {
				m_image_list_large.Images.Add(m_tex_manager.m_bitmap[i]);
				m_image_list_small.Images.Add(m_tex_manager.m_bitmap[i]);
			}
			Utility.DebugLog("Color depth: " + m_image_list_large.ColorDepth.ToString());
			listview_Textures.LargeImageList = m_image_list_large;
			listview_Textures.SmallImageList = m_image_list_small;

			m_list_items = new ListViewItem[num_bitmaps];
			for (int i = 0; i < num_bitmaps; i++) {
				m_list_items[i] = new ListViewItem();
				m_list_items[i].Text = m_tex_manager.m_name[i];
				m_list_items[i].ImageIndex = i;

			}

			m_list_font = listview_Textures.Font;
			m_list_font_tiny = new Font(FontFamily.GenericSansSerif, 4f);
			SetSpacing();

			ListViewUpdate();
		}

		private void ListViewUpdate()
		{
			string filter_text = textBox_Filter.Text;

			listview_Textures.BeginUpdate();

			listview_Textures.Items.Clear();

			if (comboBox_TextureCollection.SelectedIndex > -1) {
				Editor.TextureCollection collection = GetSelectedTextureCollection();
				foreach (string texture in collection.EnumerateTexturesSorted()) { 
					ListViewItem item = m_list_items.FirstOrDefault(i => i.Text == texture);
					if (item != null) {
						listview_Textures.Items.Add(item);
					}
				}
			} else if (string.IsNullOrEmpty(filter_text)) {
				foreach (ListViewItem item in m_list_items) {
					listview_Textures.Items.Add(item);
				}
			} else {
				foreach (ListViewItem item in m_list_items) {
					if (System.Globalization.CultureInfo.CurrentCulture.CompareInfo.IndexOf(item.Text, filter_text, System.Globalization.CompareOptions.IgnoreCase) >= 0) {
						listview_Textures.Items.Add(item);
					}
				}
			}

			listview_Textures.EndUpdate();
		}

		private void label_ViewMode_MouseDown(object sender, MouseEventArgs e)
		{
			CycleViewMode();
		}

		public enum ViewMode
		{
			LARGE,
			SMALL,

			NUM,
		}

		public ViewMode m_view_mode = 0;

		private void SetSpacing()
		{
			switch (m_view_mode) {
				case ViewMode.LARGE:
					listview_Textures.LargeImageList = m_image_list_large;
					ListViewItem_SetSpacing(listview_Textures, 128 + 10, 128 + 20);
					listview_Textures.Font = m_list_font;
					listview_Textures.View = View.LargeIcon;
					break;
				case ViewMode.SMALL:
					ListViewItem_SetSpacing(listview_Textures, 64 + 4, 64 + 11);
					listview_Textures.LargeImageList = m_image_list_small;
					listview_Textures.Font = m_list_font_tiny;
					break;
			}
		}

		public void CycleViewMode()
		{
			m_view_mode = (ViewMode)(((int)m_view_mode + 1) % (int)ViewMode.NUM);

			SetSpacing();

			label_ViewMode.Text = "View: " + m_view_mode.ToString();

			this.Refresh();
		}

		private void button_Test_Click(object sender, EventArgs e)
		{
			ListViewItem lvi = listview_Textures.FocusedItem;
			if (lvi != null) {
				Utility.DebugPopup("Current texture is: " + lvi.Text);
			} else {
				Utility.DebugPopup("No texture selected");
			}
		}

		private void TextureList_MouseEnter(object sender, EventArgs e)
		{
			//Don't do this because it can take focus away from filter text box
			//listview.Focus();
		}

		private void listview_Textures_MouseEnter(object sender, EventArgs e)
		{
			//Don't do this because it can take focus away from filter text box
			//listview.Focus();
		}

		public int SelectedTextureIndex = -1;

		private void button_Apply_Click(object sender, EventArgs e)
		{
			if (listview_Textures.SelectedItems.Count > 0) {
				ListViewItem lvi = listview_Textures.SelectedItems[0];
				if (lvi != null) {
					if (this.Modal) {
						SelectedTextureIndex = (lvi == null) ? -1 : lvi.ImageIndex;
						this.Close();
					} else {
						editor.ApplyTexture(lvi.Text, m_tex_manager.m_gl_id[lvi.ImageIndex]);
					}
				}
			}
		}

		private void button_MarkSides_Click(object sender, EventArgs e)
		{
			if (listview_Textures.SelectedItems.Count > 0) {
				ListViewItem lvi = listview_Textures.SelectedItems[0];
				if (lvi != null) {
					editor.MarkSidesWithTexture(lvi.Text);
				}
			}
		}

		private void button_GetTextureFromSide_Click(object sender, EventArgs e)
		{
			string s = "";
			s = editor.GetSelectedSideTexture();

			for (int i = 0; i < listview_Textures.Items.Count; i++) {
				if (listview_Textures.Items[i].Text.ToLower() == s.ToLower()) {
					listview_Textures.Items[i].Selected = true;
					listview_Textures.FocusedItem = listview_Textures.Items[i];
					listview_Textures.Select();
					listview_Textures.EnsureVisible(i);
				}
			}
		}

		private void TextureList_LocationChanged(object sender, EventArgs e)
		{
			if (Visible) {
				editor.m_tex_list_loc = Location;
			}
		}

		private void textBox_Filter_TextChanged(object sender, EventArgs e)
		{
			button_ClearFilter.Enabled = (textBox_Filter.Text != "");
			ListViewUpdate();
		}

		private void textBox_Filter_KeyPress(object sender, KeyPressEventArgs e)
		{
			//Clear text if escape pressed
			if (e.KeyChar == 27) {
				this.textBox_Filter.Text = "";
				e.Handled = true;
			}
		}

		private void button_ClearFilter_Click(object sender, EventArgs e)
		{
			this.textBox_Filter.Text = "";
		}

		private void TextureList_Layout(object sender, LayoutEventArgs e)
		{
			if (this.Modal) {
				button_apply.Text = "Select Texture";
				button_MarkSides.Visible = button_GetTextureFromSide.Visible = false;
				button_cancel.Visible = true;
			} else {
				button_apply.Text = "Apply Texture";
				button_MarkSides.Visible = button_GetTextureFromSide.Visible = true;
				button_cancel.Visible = false;
			}

			SetSpacing();
		}

		private string m_collection_added_to = null;

		private string EscapeAmpersands(string s)
		{
			return s.Replace("&", "&&");
		}

		private void listview_Textures_MouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right) {
				if (listview_Textures.FocusedItem.Bounds.Contains(e.Location) == true) {

					string selected_texture = listview_Textures.SelectedItems[0].Text;

					//Clear context menu so we can add items to it
					m_texture_context_menu.MenuItems.Clear();

					//Add "Add to Previous" if there's a previous and it still exists and this texture's not in it
					if (!String.IsNullOrEmpty(m_collection_added_to)) {
						Editor.TextureCollection collection = GetTextureCollection(m_collection_added_to);
						if ((collection != null) && !collection.Contains(selected_texture)) {
							m_texture_context_menu.MenuItems.Add(new MenuItem("Add to '" + EscapeAmpersands(m_collection_added_to) + "'", menuItem_AddToPreviousCollection_Click));
						}
					}

					//Add "Add to Collection" if there are collections that this texture isn't already in
					MenuItem menuItem_Add = new MenuItem("Add to Collection");
					foreach (Editor.TextureCollection collection in editor.TextureCollections) {
						if (!collection.Contains(selected_texture)) {
							menuItem_Add.MenuItems.Add(EscapeAmpersands(collection.Name), menuItem_AddToCollection_Click);
						}
					}
					if (menuItem_Add.MenuItems.Count > 0) {
						m_texture_context_menu.MenuItems.Add(menuItem_Add);
					}

					//Add "Remove from Collection" if there's a selected collection and it contails the selected texture
					if (comboBox_TextureCollection.SelectedIndex > -1) {
						Editor.TextureCollection collection = GetSelectedTextureCollection();
						if (collection.Contains(selected_texture)) {
							m_texture_context_menu.MenuItems.Add(new MenuItem("Remove from Collection", menuItem_DeleteFromCollection_Click));
						}
					}

					//If there are no items, add one
					if (m_texture_context_menu.MenuItems.Count == 0) {
						MenuItem empty_item = new MenuItem("All texture collections already contain this texture");
						empty_item.Enabled = false;
						m_texture_context_menu.MenuItems.Add(empty_item);
					}

					//Show the menu
					m_texture_context_menu.Show((Control)sender, e.Location);
				}
			}
		}

		private void listview_Textures_DoubleClick(object sender, EventArgs e)
		{
			if (this.Modal) {
				ListViewItem lvi = listview_Textures.SelectedItems[0];
				SelectedTextureIndex = (lvi == null) ? -1 : lvi.ImageIndex;
				this.Close();
			}
		}

		private void listview_Textures_SelectedIndexChanged(object sender, EventArgs e)
		{
			// Set the texture for the UV editor
			if (listview_Textures.SelectedItems.Count > 0) {
				ListViewItem lvi = listview_Textures.SelectedItems[0];
				if (lvi != null) {
					editor.uv_editor.SetTexture(m_image_list_large.Images[lvi.ImageIndex]);
				}
			}
		}

		private void CollectionControlsUpdate()
		{
			//Enable combo box if there's at least one item in it
			comboBox_TextureCollection.Enabled = (comboBox_TextureCollection.Items.Count > 0);

			//Enable clear & delete if an item selected
			button_ClearCollection.Enabled = button_DeleteCollection.Enabled = (comboBox_TextureCollection.SelectedIndex != -1);
			textBox_Filter.Enabled = !button_ClearCollection.Enabled;
		}

		private void comboBox_TextureCollection_SelectedIndexChanged(object sender, EventArgs e)
		{
			CollectionControlsUpdate();
			ListViewUpdate();
		}

		private Editor.TextureCollection GetTextureCollection(string collection_name)
		{
			return editor.TextureCollections.Find(c => c.Name == collection_name);

		}
		private Editor.TextureCollection GetSelectedTextureCollection()
		{
			return GetTextureCollection((string)comboBox_TextureCollection.SelectedItem);
		}

		private void button_NewCollection_Click(object sender, EventArgs e)
		{
			string collection_name = InputBox.GetInput("New collection", "Enter name for new collection", "");

			if (string.IsNullOrWhiteSpace(collection_name)) {
				return;
			}

			collection_name = collection_name.Trim();

			editor.TextureCollections.Add(new Editor.TextureCollection(collection_name));
			comboBox_TextureCollection.Items.Add(collection_name);
			CollectionControlsUpdate();
		}

		private void button_DeleteCollection_Click(object sender, EventArgs e)
		{
			DialogResult result = MessageBox.Show("Are you sure you want to delete colleciton '" + comboBox_TextureCollection.SelectedItem + "'?", "Delete Texture Collection", MessageBoxButtons.YesNo);

			if (result == DialogResult.Yes) {

				Editor.TextureCollection collection = GetSelectedTextureCollection();
				editor.TextureCollections.Remove(collection);
				comboBox_TextureCollection.Items.Remove(comboBox_TextureCollection.SelectedItem);
				comboBox_TextureCollection.SelectedItem = -1;
				ListViewUpdate();
				CollectionControlsUpdate();
			}
		}

		private void menuItem_AddToPreviousCollection_Click(object sender, EventArgs e)
		{
			ListViewItem lvi = listview_Textures.SelectedItems[0];
			System.Diagnostics.Debug.Assert(lvi != null);
			Editor.TextureCollection collection = GetTextureCollection(m_collection_added_to);
			collection.AddTexture(lvi.Text);
		}

		private void menuItem_AddToCollection_Click(object sender, EventArgs e)
		{
			if (listview_Textures.SelectedItems.Count > 0) {
				ListViewItem lvi = listview_Textures.SelectedItems[0];
				System.Diagnostics.Debug.Assert(lvi != null);

				Editor.TextureCollection collection = editor.TextureCollections.Find(c => EscapeAmpersands(c.Name) == ((MenuItem)sender).Text);
				collection.AddTexture(lvi.Text);
				m_collection_added_to = collection.Name;
			}
		}

		private void menuItem_DeleteFromCollection_Click(object sender, EventArgs e)
		{
			Editor.TextureCollection collection = GetSelectedTextureCollection();
			collection.RemoveTexture(listview_Textures.SelectedItems[0].Text);
			ListViewUpdate();
		}

		private void button_ClearCollection_Click(object sender, EventArgs e)
		{
			comboBox_TextureCollection.SelectedIndex = -1;
			ListViewUpdate();
		}

		private void button_cancel_Click(object sender, EventArgs e)
		{
			SelectedTextureIndex = -1;
			Close();
		}
	}
}
