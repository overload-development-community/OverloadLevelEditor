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
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace OverloadLevelEditor
{
	public partial class EditMarkerForm : Form
	{
		Regex m_indices_regex = new Regex(@"^(\d+)\s*(?:,\s*(\d+)\s*)*$", RegexOptions.None);
		Editor m_editor;

		public EditMarkerForm(Editor editor)
		{
			m_editor = editor;

			InitializeComponent();
		}

		private void EditSelectorForm_Load(object sender, EventArgs e)
		{
			textBoxIndices.Text = string.Empty;
			switch (m_editor.m_mm_edit_type) {
				case EditMode.ENTITY:
					radioTypeEntity.Checked = true;
					break;
				case EditMode.SEGMENT:
					radioTypeSegment.Checked = true;
					break;
				case EditMode.SIDE:
					radioTypeSide.Checked = true;
					break;
				case EditMode.VERTEX:
					radioTypeVertex.Checked = true;
					break;
			}
			switch (m_editor.m_mm_op_mode) {
				case OperationMode.ADD:
					radioOpSet.Checked = true;
					break;
				case OperationMode.REMOVE:
					radioOpClear.Checked = true;
					break;
				case OperationMode.TOGGLE:
					radioOpToggle.Checked = true;
					break;
			}
			buttonDoIt.Enabled = false;
      }

		private void textBoxIndices_TextChanged(object sender, EventArgs e)
		{
			string text = textBoxIndices.Text.Trim();
			buttonDoIt.Enabled = m_indices_regex.IsMatch(text);
		}

		EditMode GetEditType()
		{
			if (radioTypeEntity.Checked) {
				return EditMode.ENTITY;
			}
			if (radioTypeVertex.Checked) {
				return EditMode.VERTEX;
			}
			if (radioTypeSegment.Checked) {
				return EditMode.SEGMENT;
			}
			if (radioTypeSide.Checked) {
				return EditMode.SIDE;
			}
			return EditMode.NUM;
		}

		OperationMode GetOpMode()
		{
			if (radioOpToggle.Checked) {
				return OperationMode.TOGGLE;
			}

			if (radioOpSet.Checked) {
				return OperationMode.ADD;
			}

			if (radioOpClear.Checked) {
				return OperationMode.REMOVE;
			}

			return OperationMode.ADD;
		}

		private void buttonDoIt_Click(object sender, EventArgs e)
		{
			string text = textBoxIndices.Text.Trim();
			int[] indices = text.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
				.Select(str => int.Parse(str))
				.ToArray();

			m_editor.m_mm_edit_type = GetEditType();
			m_editor.m_mm_op_mode = GetOpMode();

			m_editor.m_level.DoMark(m_editor.m_mm_edit_type, m_editor.m_mm_op_mode, indices);
			m_editor.RefreshGeometry();

			DialogResult = DialogResult.OK;
			Close();
		}
	}
}
