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
using System.Diagnostics;

namespace OverloadLevelEditor
{
	public partial class EntityList : EditorDockContent
	{
		public EntityList(EditorShell shell)
			: base( shell )
		{
			InitializeComponent();
		}

		private void EntityList_Load( object sender, EventArgs e )
		{
			Populate();
		}

		private void AddCount(TreeNode node)
		{
			if (node.Nodes.Count > 0) {
				node.Text = node.Name + " (" + node.Nodes.Count + ")";
			}
		}

		public void Populate()
		{
			var level = ActiveLevel;
			if( level == null )
				return;

			treeView_entity_list.BeginUpdate();
			treeView_entity_list.Nodes.Clear();
			foreach (Entity entity in level.EnumerateAliveEntities()) { 
				AddEntity(entity);
			}
			treeView_entity_list.EndUpdate();
		}

		public void AddEntity(Entity entity)
		{
			string type_name = Editor.CleanupName(entity.Type.ToString());
			string subtype_name = Editor.CleanupName(entity.SubTypeName());
			TreeNode[] nodes = treeView_entity_list.Nodes.Find(type_name, false);
			TreeNode type_node = treeView_entity_list.Nodes.Find(type_name, false).FirstOrDefault();
			if (type_node == null) {
				type_node = treeView_entity_list.Nodes.Add(type_name, type_name);
			}
			TreeNode subtype_node = type_node.Nodes.Find(subtype_name, false).FirstOrDefault();
			if (subtype_node == null) {
				subtype_node = type_node.Nodes.Add(subtype_name, subtype_name);
				AddCount(type_node);
			}
			TreeNode entity_node = subtype_node.Nodes.Add(entity.guid.ToString(), entity.num + ": " + entity.guid.ToPrettyString());
			AddCount(subtype_node);
		}

		public void RemoveEntity(Entity entity)
		{
			TreeNode node = treeView_entity_list.Nodes.Find(entity.guid.ToString(), true).FirstOrDefault();
			while (node != null) {
				TreeNode parent = node.Parent;
				TreeNodeCollection parent_nodes = (parent == null) ? treeView_entity_list.Nodes : parent.Nodes;
				parent_nodes.Remove(node);
				if ((parent != null) && (parent_nodes.Count != 0)) {
					AddCount(parent);
					parent = null;    //don't remove parent, because it has other children
				}
				node = parent;
			}
		}

		//Called when type or subtype changed
		public void UpdateEntity(Entity entity)
		{
			treeView_entity_list.BeginUpdate();
			RemoveEntity(entity);
			AddEntity(entity);
			treeView_entity_list.EndUpdate();
		}

		public void SetSelected(int entity_num)
		{
			if (entity_num != -1) {
				var level = ActiveLevel;
				TreeNode node = treeView_entity_list.Nodes.Find(level.entity[entity_num].guid.ToString(), true).FirstOrDefault();
				treeView_entity_list.SelectedNode = node;
			}
		}

		private void treeView_entity_list_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
		{
			//if a leaf node (and thus an entity) select the entity
			if (e.Node.Nodes.Count == 0) {
				var editor = ActiveDocument;
				var level = ActiveLevel;

				level.selected_entity = level.FindEntityWithGUID(e.Node.Name).num;
				editor.RefreshGeometry();
			}
		}

		private void treeView_entity_list_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
		{
			//if a leaf node (and thus an entity) move camera to view it
			if (e.Node.Nodes.Count == 0) {
				var editor = ActiveDocument;
				var level = ActiveLevel;

				editor.SetProjOffsetAllViews(level.FindEntityWithGUID(e.Node.Name).position);
				editor.RefreshGeometry();
			}
		}
	}
}
