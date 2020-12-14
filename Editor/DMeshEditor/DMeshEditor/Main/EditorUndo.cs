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
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

// EDITOR - Undo
// Everything to do with undo is stored here
// Uses a set number of level copies in memory to allow Undo
// Anything that can be undone should call SaveStateForUndo

namespace OverloadLevelEditor
{
   public partial class Editor : Form
   {
		public const int MAX_UNDOS = 32;
		public DMesh[] m_undo_dmesh = new DMesh[MAX_UNDOS];
		public string[] m_undo_name = new string[MAX_UNDOS];
		public int m_undo_next = 0;
		public int m_undo_current = 0;
		public int m_undo_count = 0;
		public int m_redo_count = 0;

		public void UndoInit()
		{
			for (int i = 0; i < MAX_UNDOS; i++) {
				m_undo_dmesh[i] = new DMesh("undo_dmesh" + i.ToString());
				m_undo_name[i] = "";
			}
		}

		// Save the current level state before doing an action
		// - Set dirty to false if the level should be saved on exit (generally true, but selection/marking don't need to be saved)
		public void SaveStateForUndo(string name, bool dirty = true)
		{
			Utility.DebugLog("UNDO save state: " + name);
			if (dirty) {
				// Don't make level dirty for selection
				m_dmesh.dirty = true;
			}
			
			m_undo_count += 1;
			if (m_undo_count >= MAX_UNDOS) {
				m_undo_count = MAX_UNDOS - 1;
			}
			m_redo_count = 0;
			
			m_undo_dmesh[m_undo_next].CopyDMesh(m_dmesh, true);
			m_undo_name[m_undo_next] = name;
			m_undo_current = m_undo_next;

			undoToolStripMenuItem.Enabled = true;
			redoToolStripMenuItem.Enabled = false;

			m_undo_next = (m_undo_next + 1) % MAX_UNDOS;
		}

		public void RestoreUndo()
		{
			if (m_undo_count > 0) {
				//Utility.DebugLog("UNDO: Restoring to: " + undo_current.ToString() + " - Backing up to: " + undo_next.ToString());
				m_undo_dmesh[m_undo_next].CopyDMesh(m_dmesh, true);
				m_dmesh.CopyDMesh(m_undo_dmesh[m_undo_current], true);

				AddOutputText("Undo command: " + m_undo_name[m_undo_current]);
				
				m_undo_next = m_undo_current;
				m_undo_current -= 1;
				if (m_undo_current < 0) {
					m_undo_current = MAX_UNDOS - 1;
				}

				m_undo_count -= 1;
				if (m_undo_count <= 0) {
					undoToolStripMenuItem.Enabled = false;
				}
				
				m_redo_count += 1;
				if (m_redo_count >= MAX_UNDOS) {
					m_redo_count = MAX_UNDOS - 1;
				}
				redoToolStripMenuItem.Enabled = true;

				RefreshGeometry();
			}
		}

		public void RestoreRedo()
		{
			if (m_redo_count > 0) {
				//Utility.DebugLog("REDO: Restoring to: " + ((undo_next + 1) % MAX_UNDOS).ToString() + " Current undo: " + undo_next);
				m_undo_current = m_undo_next;
				m_undo_next = (m_undo_next + 1) % MAX_UNDOS;
				m_dmesh.CopyDMesh(m_undo_dmesh[m_undo_next], true);

				m_redo_count -= 1;
				if (m_redo_count == 0) {
					redoToolStripMenuItem.Enabled = false;
				}
				m_undo_count += 1;
				undoToolStripMenuItem.Enabled = true;

				RefreshGeometry();
				
				AddOutputText("Redo command: " + m_undo_name[m_undo_current]);
			}
		}
   }
}
