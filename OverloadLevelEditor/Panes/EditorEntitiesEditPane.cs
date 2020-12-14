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
using OpenTK;
using System.Windows.Forms;
using Overload;

namespace OverloadLevelEditor
{
	public partial class EditorEntitiesEditPane : EditorDockContent
	{
		List<EntityLinkLabel> entity_link_labels = new List<EntityLinkLabel>();
		bool m_ignore_combobox_entity_sub_type_changed = false;

		public EditorEntitiesEditPane(EditorShell shell)
			: base(shell)
		{
			InitializeComponent();
		}

		private void EditorEntitiesEditPane_Load(object sender, EventArgs e)
		{
			foreach (string s in Enum.GetNames(typeof(EntityType))) {
				if (s != "NUM") {
					comboBox_entity_type.Items.Add(Editor.CleanupName(s));
				}
			}

			comboBox_entity_type.SelectedIndex = 0;
		}

		public void UpdateOptionLabels()
		{
			var level = ActiveLevel;

			var entity_pivot = ActiveDocument.m_pivot_mode;
			label_entity_pivot.Text = "Pivot: " + entity_pivot.ToString().Replace('_', ' ');
		}

		void ClearEntityLinkLabels()
		{
			foreach (EntityLinkLabel ell in entity_link_labels) {
				ell.Dispose();
			}
			entity_link_labels.Clear();
		}

		void AddLinkLabel(string name, Guid link_guid)
		{
			var editor = ActiveDocument;

			EntityLinkLabel ol = new EntityLinkLabel();
			ol.LabelText = name;
			ol.ValueText = editor.EntityGUIDString(link_guid);
			ol.Index = entity_link_labels.Count;
			ol.Location = new Point(label_entity_pivot.Left - 1, label_entity_pivot.Bottom + 2 + ol.Index * 19);
			ol.ToolTop = "LMB: Assign 1st marked entity (and unmark it)\nRMB: Mark linked entity\nMMB: Select linked entity\nSHIFT+LMB: Clear link";
			this.Controls.Add(ol);
			ol.Feedback += new System.EventHandler<EntityLinkLabelArgs>(editor.ELLFeedback);
			entity_link_labels.Add(ol);
		}

		public void AddEntityLinkLabels(Entity e)
		{
			switch (e.Type) {
				case EntityType.SCRIPT: {
						var eps = (EntityPropsScript)e.entity_props;
						AddLinkLabel("Link 0", eps.entity_link[0]);
						AddLinkLabel("Link 1", eps.entity_link[1]);
						AddLinkLabel("Link 2", eps.entity_link[2]);
						AddLinkLabel("Link 3", eps.entity_link[3]);
						AddLinkLabel("Link 4", eps.entity_link[4]);
					}
					break;
				case EntityType.TRIGGER: {
						var ept = (EntityPropsTrigger)e.entity_props;
						AddLinkLabel("Link 0", ept.entity_link[0]);
						AddLinkLabel("Link 1", ept.entity_link[1]);
						AddLinkLabel("Link 2", ept.entity_link[2]);
						AddLinkLabel("Link 3", ept.entity_link[3]);
						AddLinkLabel("Link 4", ept.entity_link[4]);
					}
					break;
				default:
					break;
			}
		}

		public void UpdateEntityLabels()
		{
			Editor editor = ActiveDocument;
			Level level = ActiveLevel;

			Entity e = level.GetSelectedEntity();
			if (e != null) {
				label_entity_type_value.Text = e.Type.ToString();
				label_entity_guid_value.Text = e.num + ": " + e.guid.ToPrettyString();

				// Update the complex labels, if necessary
				if (e.guid != editor.m_entity_prev_guid) {
					ClearEntityLinkLabels();
					prop_grid_entity.SelectedObject = e.GetEntityProps();
					AddEntityLinkLabels(e);

					comboBox_entity_subtype.Items.Clear();
					foreach (string s in e.SubTypeNames()) {
						if (s != "NUM") {
							comboBox_entity_subtype.Items.Add(Editor.CleanupName(s));
						}
					}
					m_ignore_combobox_entity_sub_type_changed = true;
					comboBox_entity_subtype.SelectedItem = Editor.CleanupName(e.SubTypeName());
					m_ignore_combobox_entity_sub_type_changed = false;

					comboBox_entity_subtype.Enabled = true;

					editor.m_entity_prev_guid = e.guid;
				}
			} else {
				label_entity_type_value.Text = "";
				label_entity_guid_value.Text = "";

				// Update the complex labels, if necessary
				if (editor.m_entity_prev_guid != Guid.Empty) {
					prop_grid_entity.SelectedObject = null;
					ClearEntityLinkLabels();

					m_ignore_combobox_entity_sub_type_changed = true;
					comboBox_entity_subtype.SelectedItem = null;
					m_ignore_combobox_entity_sub_type_changed = false;
					comboBox_entity_subtype.Enabled = false;

               editor.m_entity_prev_guid = Guid.Empty;
				}
			}

			//Enable/disable place entity buttons based on selected segment/side, with special considerations for doors
			button_place_entity_segment.Enabled = ((level.selected_segment > -1) && (comboBox_entity_type.SelectedIndex != (int)EntityType.DOOR));
			button_place_entity_side.Enabled = ((level.selected_segment > -1) && (level.selected_side > -1) && (level.GetSelectedSide().Door == -1));

			//Enable/disable buttons that depend on selected entity
			label_entity_type.Enabled = label_entity_type_value.Enabled = label_entity_guid.Enabled = label_entity_guid_value.Enabled =
				comboBox_entity_subtype.Enabled =
				button_entity_reset_rotation.Enabled = button_entity_face_selected_side.Enabled =
				button_entity_align_side.Enabled = button_entity_move.Enabled =
				button_entity_mark_type.Enabled = button_entity_mark_subtype.Enabled = (e != null);

			//Enable move to segment if there's a selected segment, a selected entity, and the entity isn't a door
			button_entity_move_segment.Enabled = ((level.selected_segment > -1) && (e != null) && (e.Type != EntityType.DOOR));

			//Enable move to side if there's a selected segment, a selected side, a selected entity and, if the entity is a door, there's not already a door in the side (unless the door is this door, in which case we allow move to side for alignment)
			button_entity_move_side.Enabled = ((level.selected_segment > -1) && (level.selected_side > -1) && (e != null) && (((level.GetSelectedSide().Door == -1) || (e.Type != EntityType.DOOR)) || (level.GetSelectedSide().Door == level.GetSelectedEntity().num)));

			//Enable/diable buttons that depend on marked
			List<Entity> marked_entities = level.GetMarkedEntities();
			button_entity_marked_reset_rotation.Enabled = button_entity_marked_face_selected_side.Enabled = button_entity_duplicate.Enabled = (marked_entities.Count > 0);

			//Enable/disable buttons that depend on selected AND marked
			bool enabled = false;
			if (e != null) {		//We have a selected entity; see if there are any marked of the same type
				foreach (Entity entity in level.GetMarkedEntities()) {
					if ((entity != e) && (entity.Type == e.Type)) {
						enabled = true;
						break;
					}
				}
			}
			button_entity_copy_to_marked.Enabled = button_entity_copy_subtype.Enabled = enabled;

			if (e != null) {
				string team = "ALL";
				if (e.m_multiplayer_team_association_mask == 1) {
					team = "A";
				} else if (e.m_multiplayer_team_association_mask == 2) {
					team = "B";
				}
				label_team.Text = "MP Team: " + team;
			} else {
				label_team.Text = "MP Team: -";
			}
		}

		private void prop_grid_entity_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
		{
			prop_grid_entity.Refresh();
		}

		private void button_entity_move_Click(object sender, EventArgs e)
		{
			var editor = ActiveDocument;
			var level = ActiveLevel;

			string coords_text = InputBox.GetInput("Entity Coords", "Enter new coodtinates for this enity", "");
			if (coords_text == null) {
				return;
			}

			string[] pos_coords = coords_text.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
			if (pos_coords.Length != 3)
				return;

			Vector3 pos = Vector3.Zero;
			if (float.TryParse(pos_coords[0], out pos.X) && float.TryParse(pos_coords[1], out pos.Y) && float.TryParse(pos_coords[2], out pos.Z)) {
				level.EntityMoveToPos(pos);
				editor.RefreshGeometry();
			}
		}

		private void comboBox_entity_type_SelectedIndexChanged(object sender, EventArgs e)
		{
			//Disallow/allow placing in center of segment based on whether placing a door or not.
			UpdateEntityLabels();
		}

		private void comboBox_entity_subtype_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!m_ignore_combobox_entity_sub_type_changed) {
				ActiveDocument.SetSelectedEntitySubType(((ComboBox)sender).SelectedIndex);
			}
		}

		private void button_entity_align_side_Click(object sender, EventArgs e)
		{
			ActiveDocument.EntityAlignToSide();
		}

		private void button_entity_copy_subtype_Click(object sender, EventArgs e)
		{
			Editor editor = ActiveDocument;
			Level level = editor.m_level;

			Entity entity = level.GetSelectedEntity();

			if (entity != null) {
				editor.SaveStateForUndo("Copy entity subtype");
				int num = level.CopyEntityPropertiesToMarked(entity, false);
				editor.RefreshGeometry();
				ActiveDocument.AddOutputText("Copied subtype to " + num + " entities.");
			}
		}

		private void button_entity_mark_subtype_Click(object sender, EventArgs e)
		{
			Editor editor = ActiveDocument;
			Level level = editor.m_level;

			if (level.selected_entity > -1 && level.entity[level.selected_entity].alive) {
				editor.SaveStateForUndo("Mark entities of same subtype");
				level.MarkEntitiesOfSubtype(level.entity[level.selected_entity].Type, level.entity[level.selected_entity].SubType);
				editor.RefreshGeometry();
			}
		}

		private void button_entity_mark_type_Click(object sender, EventArgs e)
		{
			Editor editor = ActiveDocument;
			Level level = editor.m_level;

			Entity entity = level.GetSelectedEntity();

			if (entity != null) {
				editor.SaveStateForUndo("Mark entities of same type");
				int num = level.MarkEntitiesOfType(entity.Type);
				ActiveDocument.AddOutputText("Marked " + num + " " + entity.Type.ToString() + " entities.");
				editor.RefreshGeometry();
			}
		}

		private void button_entity_copy_to_marked_Click(object sender, EventArgs e)
		{
			Editor editor = ActiveDocument;
			Level level = editor.m_level;

			if (level.selected_entity > -1 && level.entity[level.selected_entity].alive) {
				editor.SaveStateForUndo("Copy entity properties");
				int num = level.CopyEntityPropertiesToMarked(level.entity[level.selected_entity], true);
				ActiveDocument.AddOutputText("Copied properties to " + num + " entities.");
				editor.RefreshGeometry();
			}
		}

		private void button_entity_move_segment_Click(object sender, EventArgs e)
		{
			ActiveDocument.EntityMoveToSegment();
		}

		private void button_entity_move_side_Click(object sender, EventArgs e)
		{
			ActiveDocument.EntityMoveToSide();
		}

		private void button_entity_duplicate_Click(object sender, EventArgs e)
		{
			int num = ActiveDocument.DuplicateMarkedEntities();
			ActiveDocument.AddOutputText("Duplicated " + num + " entities.");
		}

		private void label_entity_pivot_MouseDown(object sender, MouseEventArgs e)
		{
			ActiveDocument.CycleEntityPivot();
		}

		private void button_entity_marked_face_selected_side_Click(object sender, EventArgs e)
		{
			int num = ActiveDocument.EntityMarkedFaceSelectedSide();
			ActiveDocument.AddOutputText(num + "marked entities turned to face selected side.");
		}

		private void button_entity_marked_reset_rotation_Click(object sender, EventArgs e)
		{
			int num = ActiveDocument.EntityMarkedResetRotation();
			ActiveDocument.AddOutputText("Rotation reset for " + num + " marked entities.");
		}

		private void button_place_entity_side_Click(object sender, EventArgs e)
		{
			ActiveDocument.CreateEntityOnSide();
		}

		private void button_place_entity_segment_Click(object sender, EventArgs e)
		{
			ActiveDocument.CreateEntityInSegment();
		}

		private void button_entity_reset_rotation_Click(object sender, EventArgs e)
		{
			ActiveDocument.EntityResetRotation();
		}

		private void button_entity_face_selected_side_Click(object sender, EventArgs e)
		{
			ActiveDocument.EntityFaceSelectedSide();
		}

		private void label_team_Click(object sender, EventArgs e)
		{
			ActiveDocument.EntityCycleTeam();
		}
	}
}