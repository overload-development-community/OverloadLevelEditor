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
	public partial class EditorLevelCustomInfoPane : EditorDockContent
	{
		public EditorLevelCustomInfoPane(EditorShell shell)
			: base(shell)
		{
			InitializeComponent();
		}

		private void EditorLevelCustomInfoPane_Load(object sender, EventArgs e)
		{
			Populate();
		}

		public void Populate()
		{
			var level = ActiveLevel;
			if (level == null) {
				return;
			}

			string[] objective_names = typeof(Level.CustomLevelInfoObjective).GetEnumNames();
			comboBoxObjective.Items.Clear();
			comboBoxObjective.Items.AddRange(objective_names);

			this.textBoxExitMusicStartTime.Text = level.custom_level_info.m_exit_music_start_time.ToString();
			this.textBoxObjectiveCount.Text = level.custom_level_info.m_custom_count.ToString();
			this.checkBoxAlienLava.Checked = level.custom_level_info.m_alien_lava;
			this.checkBoxNoExplosionsOnExit.Checked = level.custom_level_info.m_exit_no_explosions;
			this.comboBoxObjective.SelectedIndex = (int)level.custom_level_info.m_objective;
		}

		public void SetDataString(string s)
		{
			var level = ActiveLevel;
			if (level == null)
				return;

			//level.challenge_data_string = s;
			//this.challengeModeTextBox.Text = s;
		}

		private void checkBoxAlienLava_CheckedChanged(object sender, EventArgs e)
		{
			var level = ActiveLevel;
			if (level == null)
				return;

			var new_val = checkBoxAlienLava.Checked;
			if (new_val != level.custom_level_info.m_alien_lava) {
				level.custom_level_info.m_alien_lava = new_val;
				level.dirty = true;
			}
		}

		private void checkBoxNoExplosionsOnExit_CheckedChanged(object sender, EventArgs e)
		{
			var level = ActiveLevel;
			if (level == null)
				return;

			var new_val = checkBoxNoExplosionsOnExit.Checked;
			if (new_val != level.custom_level_info.m_exit_no_explosions) {
				level.custom_level_info.m_exit_no_explosions = new_val;
				level.dirty = true;
			}
		}

		private void textBoxExitMusicStartTime_Leave(object sender, EventArgs e)
		{
			var level = ActiveLevel;
			if (level == null)
				return;

			float new_value;
			if (float.TryParse(textBoxExitMusicStartTime.Text, out new_value)) {
				new_value = Math.Max(0.0f, new_value);
				if (new_value != level.custom_level_info.m_exit_music_start_time) {
					level.custom_level_info.m_exit_music_start_time = new_value;
					level.dirty = true;
				}
			} else {
				textBoxExitMusicStartTime.Text = level.custom_level_info.m_exit_music_start_time.ToString();
			}
		}

		private void textBoxObjectiveCount_Leave(object sender, EventArgs e)
		{
			var level = ActiveLevel;
			if (level == null)
				return;

			int new_value;
			if (int.TryParse(textBoxObjectiveCount.Text, out new_value)) {
				new_value = Math.Max(0, new_value);
				if (new_value != level.custom_level_info.m_custom_count) {
					level.custom_level_info.m_custom_count = new_value;
					level.dirty = true;
				}
			} else {
				textBoxObjectiveCount.Text = level.custom_level_info.m_custom_count.ToString();
			}
		}

		private void comboBoxObjective_SelectedIndexChanged(object sender, EventArgs e)
		{
			var level = ActiveLevel;
			if (level == null)
				return;

			int new_value = (int)comboBoxObjective.SelectedIndex;
			if (new_value != -1) {
				if (new_value != (int)level.custom_level_info.m_objective) {
					level.custom_level_info.m_objective = (Level.CustomLevelInfoObjective)new_value;
					level.dirty = true;
				}
			} else {
				comboBoxObjective.SelectedIndex = (int)level.custom_level_info.m_objective;
			}
		}

		private void EditorLevelCustomInfoPane_VisibleChanged(object sender, EventArgs e)
		{
			if (this.Visible) {
				return;
			}

			// fake losing focus for the textboxes
			textBoxExitMusicStartTime_Leave(sender, e);
			textBoxObjectiveCount_Leave(sender, e);

		}
	}
}
