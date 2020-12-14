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
using System.Runtime.InteropServices;

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

		public int MakeLong(short lowPart, short highPart)
		{
			return (int)(((ushort)lowPart) | (uint)(highPart << 16));
		}

		public void ListViewItem_SetSpacing(ListView listview, short leftPadding, short topPadding)
		{
			try {
				const int LVM_FIRST = 0x1000;
				const int LVM_SETICONSPACING = LVM_FIRST + 53;
				SendMessage( listview.Handle, LVM_SETICONSPACING, IntPtr.Zero, (IntPtr)MakeLong( leftPadding, topPadding ) );
			} catch {
				// Smother exception on non-Windows platforms
			}
		}

		// END - Allowing listview spacing settings

		public Editor editor;
		
		public TextureList(Editor e)
		{
			editor = e;
			InitializeComponent();
			MinimumSize = new Size(Width, 0);
			MaximumSize = new Size(2048, 2048);
		}

		private void TextureList_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (e.CloseReason != CloseReason.FormOwnerClosing) {
				this.Hide();
				e.Cancel = true;
			}
		}

		public ListViewItem[] m_list_items;
		public ImageList m_image_list_large;
		public ImageList m_image_list_small;
		public Bitmap m_bitmap;
		public Font m_list_font;
		public Font m_list_font_tiny;

		private void TextureList_Load(object sender, EventArgs e)
		{
			
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

			int num_bitmaps = editor.tm_decal.m_bitmap.Count;
			for (int i = 0; i < num_bitmaps; i++) {
				m_image_list_large.Images.Add(editor.tm_decal.m_bitmap[i]);
				m_image_list_small.Images.Add(editor.tm_decal.m_bitmap[i]);
			}
			Utility.DebugLog("Color depth: " + m_image_list_large.ColorDepth.ToString());
			listview.LargeImageList = m_image_list_large;
			listview.SmallImageList = m_image_list_small;

			m_list_items = new ListViewItem[num_bitmaps];			
			for (int i = 0; i < num_bitmaps; i++) {
				m_list_items[i] = new ListViewItem();
				m_list_items[i].Text = editor.tm_decal.m_name[i];
				m_list_items[i].ImageIndex = i;

				listview.Items.Add(m_list_items[i]);
			}

			m_list_font = listview.Font;
			listview.View = View.LargeIcon;
			m_list_font_tiny = new Font(FontFamily.GenericSansSerif, 4f);
			ListViewItem_SetSpacing(listview, 128 + 10, 128 + 20);

			/*ListViewItem lvi = new ListViewItem("test", 0);
			lvi.Checked = true;
			listview.Items.Add(lvi);

			ImageList il = new ImageList();
			il.Images.Add(Bitmap.FromFile(editor.m_filepath_decal_textures + "\\concrete1.png"));

			listview.LargeImageList = il;*/
		}

		private void label_view_mode_MouseDown(object sender, MouseEventArgs e)
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

		public void CycleViewMode()
		{
			m_view_mode = (ViewMode)(((int)m_view_mode + 1) % (int)ViewMode.NUM);
			switch (m_view_mode) {
				case ViewMode.LARGE:
					listview.LargeImageList = m_image_list_large;
					ListViewItem_SetSpacing(listview, 128 + 10, 128 + 20);
					listview.Font = m_list_font;
					listview.View = View.LargeIcon;
					break;
				case ViewMode.SMALL:
					ListViewItem_SetSpacing(listview, 64 + 4, 64 + 11);
					listview.LargeImageList = m_image_list_small;
					listview.Font = m_list_font_tiny;
					break;
			}

			label_view_mode.Text = "View: " + m_view_mode.ToString();

			this.Refresh();
		}

		private void button_test_Click(object sender, EventArgs e)
		{
			ListViewItem lvi = listview.FocusedItem;
			if (lvi != null) {
				Utility.DebugPopup("Current texture is: " + lvi.Text);
			} else {
				Utility.DebugPopup("No texture selected");
			}
		}

		private void TextureList_MouseEnter(object sender, EventArgs e)
		{
			listview.Focus();
		}

		private void listview_MouseEnter(object sender, EventArgs e)
		{
			listview.Focus();
		}

		private void button_apply_Click(object sender, EventArgs e)
		{
			if (listview.SelectedItems.Count > 0) {
				ListViewItem lvi = listview.SelectedItems[0];
				if (lvi != null) {
					editor.ApplyTexture(lvi.Text, editor.tm_decal.m_gl_id[lvi.ImageIndex]);
				}
			}
		}

		private void button_mark_sides_Click(object sender, EventArgs e)
		{
			if (listview.SelectedItems.Count > 0) {
				ListViewItem lvi = listview.SelectedItems[0];
				if (lvi != null) {
					editor.MarkPolysWithTexture(lvi.Text);
				}
			}
		}

		private void button1_Click(object sender, EventArgs e)
		{
			string s = editor.GetSelectedPolyTexture();
			s = Utility.GetPathlessFilename(s);
			for (int i = 0; i < listview.Items.Count; i++) {
				if (listview.Items[i].Text.ToLower() == s.ToLower()) {
					SelectItemNum(i);
				}
			}
		}

		public void SelectItemNum(int num)
		{
			if (num < listview.Items.Count) {
				listview.FocusedItem = listview.Items[num];
				listview.Items[num].Selected = true;
				listview.Select();
				listview.EnsureVisible(num);
			}
		}

		private void TextureList_LocationChanged(object sender, EventArgs e)
		{
			if (Visible) {
				editor.m_tex_list_loc = Location;
			}
		}

		private void listview_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (listview.SelectedItems.Count > 0) {
				ListViewItem lvi = listview.SelectedItems[0];
				if (lvi != null) {
					editor.uv_editor.SetTexture(m_image_list_large.Images[lvi.ImageIndex]);
				}
			}
		}
	}
}
