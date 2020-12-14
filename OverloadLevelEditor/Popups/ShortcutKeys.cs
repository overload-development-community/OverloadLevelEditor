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

namespace OverloadLevelEditor.Popups
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
			AddShortcut("F1", "Show Decal List");
			AddShortcut("F2", "Show Texture List");
			AddShortcut("F3", "Show Tunnel Builder");
			AddShortcut("CTRL + O", "Open level");
			AddShortcut("CTRL + S", "Save current level");
			AddShortcut("Tab/~", "Cycle the element edit mode (SHIFT reverses)");
			AddShortcut("", "");

			AddShortcut("INSERT/I", "Insert a new segment at the selected side");
			AddShortcut("CTRL + INSERT/I", "Insert a new segment at the marked sides (direction from selected side)");
			AddShortcut("SHIFT + INSERT/I", "Insert a new segment at the selected side (standard size)");
			AddShortcut("DELETE", "Delete marked segments/entities (can't delete sides/verts)");
			AddShortcut("SPACE", "Toggle marking on the selected element");
			AddShortcut("SHIFT + SPACE", "Toggle all marked elements");
			AddShortcut("CTRL + SPACE", "Clear all marked elements");
			AddShortcut("F", "Frame selected elements in the view (CTRL active view only)");
			AddShortcut("SHIFT + F", "Frame whole level in the view (CTRL active view only)");
			AddShortcut("G", "Frame selected elements in the view, no zooming (CTRL active view only)");
			AddShortcut("", "");

			AddShortcut("SHIFT + G", "Cycle the grid display");
			AddShortcut("CTRL + G", "Snap the marked elements to the grid");
			AddShortcut("Q", "Mark connected sides with the same texture");
			AddShortcut("SHIFT + Q", "Mark connected coplanar sides (to the selected side)");
			AddShortcut("CTRL + Q", "Mark connected vertical sides (with roughly the same vertical position)");
			AddShortcut("SHIFT + J", "Join selected side to closest/marked side");
			AddShortcut("K", "Set marked enemies as Station = true");
			AddShortcut("L", "Set marked enemies as NG+ = true");

			AddShortcut("[/]", "Change the grid snap setting");
			AddShortcut("SHIFT + [/]", "Change the visible grid setting");
			AddShortcut("CTRL + C", "Copy the marked segments (selected side is starting point)");
			AddShortcut("CTRL + V", "Paste the copied segments (selected side is attach point)");
			AddShortcut("F5 / RETURN", "Refresh all meshes and update chunking");
			AddShortcut("CTRL + SHIFT + F1", "Flatten selected sides to average Y value (good for lava)");
			AddShortcut("", "");

			AddShortcut("X / SHIFT + X", "Cycle the selected side (on the selected segment)");
			AddShortcut("C / SHIFT + C", "Cycle the selected segment");
			AddShortcut("N / SHIFT + N", "Select the next adjacent segment (using selected side)");
			AddShortcut("SHIFT + V", "Cycle the selected vert on the selected side");
			AddShortcut("SHIFT + T", "Align marked side UVs to the selected side's");
			AddShortcut("CTRL + T", "Reset the marked side UVs to the default");
			AddShortcut("UP/DOWN/LEFT/RIGHT", "Move the marked/selected polygon UVs");
			AddShortcut("CTRL + LEFT/RIGHT", "Rotate the marked/selected polygon UVs");
			AddShortcut("", "");

			AddShortcut("NUMPAD 2/4/6/8/+/-", "Move marked elements (ALT for less)");
			AddShortcut("CTRL + NUMPAD KEYS", "Rotate marked elements (ALT for less)");

			AddShortcut("", "");

			AddShortcut("MOUSE WHEEL", "Zoom view in/out");
			AddShortcut("CTRL + MOUSE WHEEL", "Adjust perspective view FOV");
			AddShortcut("MMB DRAG", "Pan view");
			AddShortcut("LMB CLICK", "Select element");
			AddShortcut("LMB DRAG", "Mark element");
			AddShortcut("ALT + LMB DRAG", "Rotate perspective view (or MMB + RMB DRAG)");
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
