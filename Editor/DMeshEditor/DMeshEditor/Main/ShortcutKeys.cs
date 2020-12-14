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
	public partial class ShortcutKeys : Form
	{
		public ShortcutKeys()
		{
			InitializeComponent();
		}

		public string shortcut_string = "";
		public string description_string = "";

		private void ShortcutKeys_Load(object sender, EventArgs e)
		{
			AddShortcut("SHIFT + X", "Cut the selected polygon along the nearest edge (two steps)");
			AddShortcut("CTRL + SHIFT + X", "Cut the selected polygon but don't split the poly");
			AddShortcut("SHIFT + A", "Create new polygon from the marked verts");
			AddShortcut("A", "Add new vert at mouse position");
			AddShortcut("R", "Cycle scale mode");
			AddShortcut("Tab/~", "Cycle poly/vert mode");
			AddShortcut("", "");

			AddShortcut("CTRL + INSERT/I", "Extrude marked polygons");
			AddShortcut("SHIFT + INSERT/I", "Extrude marked verts");
			AddShortcut("INSERT/I", "Extrude the selected polygon");
			AddShortcut("DELETE", "Delete the marked polygons/verts");
			AddShortcut("SPACE", "Toggle marking on the selected vert/polygon");
			AddShortcut("SHIFT + SPACE", "Toggle all marked polygons/verts");
			AddShortcut("CTRL + SPACE", "Clear all marked polygons + verts");
			AddShortcut("CTRL + F", "Reverse the marked polygon's normal");
			AddShortcut("F", "Frame all elements in the view");
			AddShortcut("", "");

			AddShortcut("SHIFT + G", "Cycle the grid display");
			AddShortcut("CTRL + G", "Snap the marked elements to the grid");
			AddShortcut("CTRL + Q", "Mark coplanar polygons (to the selected polygon)");
			AddShortcut("[/]", "Change the grid snap setting");
			AddShortcut("SHIFT + [/]", "Change the visible grid setting");
			AddShortcut("CTRL + C", "Copy the marked polgons");
			AddShortcut("CTRL + V", "Paste the copied polygons");
			AddShortcut("", "");

			AddShortcut("C", "Cycle the selected polygon");
			AddShortcut("V", "Cycle the selected vert");
			AddShortcut("SHIFT + V", "Cycle the selected vert on the selected polygon");
			AddShortcut("SHIFT + T", "Align marked polygon UVs to the selected polygon's");
			AddShortcut("CTRL + T", "Reset the marked polygon UVs to the default");
			AddShortcut("UP/DOWN/LEFT/RIGHT", "Move the marked/selected polygon UVs");
			AddShortcut("SHIFT + LEFT/RIGHT", "Rotate the marked/selected polygon UVs");
			AddShortcut("", "");

			AddShortcut("NUMPAD 2/4/6/8/+/-", "Move marked elements");
			AddShortcut("NUMPAD 1/3", "Scale marked elements");
			AddShortcut("NUMPAD 7/9", "Rotate marked elements");

			AddShortcut("", "");

			AddShortcut("MOUSE WHEEL", "Zoom view in/out");
			AddShortcut("CTRL + MOUSE WHEEL", "Adjust perspective view FOV");
			AddShortcut("MMB DRAG", "Pan view");
			AddShortcut("LMB CLICK", "Select polygon/vert");
			AddShortcut("LMB DRAG", "Mark polygon/vert");
			AddShortcut("ALT + LMB DRAG", "Rotate perspective view");
			AddShortcut("RMB DRAG", "Move marked elements");
			AddShortcut("ALT + RMB DRAG", "Rotate marked elements");
			AddShortcut("CTRL + RMB DRAG", "Scale marked elements");

			label_shortcuts.Text = shortcut_string;
			label_descriptions.Text = description_string;
			
			// Update the size of the form
			this.Size = new Size(label_descriptions.Right + 15, label_descriptions.Bottom + 45);
		}

		public void AddShortcut(string sc, string desc)
		{
			shortcut_string += (sc + "\n");
			description_string += (desc + "\n");
		}
	}
}
