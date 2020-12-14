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
using System.Drawing;
using System.Windows.Forms;
using OpenTK;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;


// TODO - MEETING
// More item/enemy types

// EDITOR - Primary
// Contains the direct editor UI/.NET commands, redirects them to the appropriate function in other files

// -----------------------------------------------------

// Entities
// Create different properties for each type
// - First need to create more detailed entities in the game, then editor can follow

// ISSUES (EXTERNAL)
// ProBuilder only exports 1 texture per OBJ

// Levels loaded in Unity don't have all their decals (see experiment)
// - Need to narrow down to a simple test case, might be decal-specific

// BUGS
// Segments draw with dark wireframe after textures are applied
// - Need a test case where it happens (don't think I fixed it)
// - Related to segments drawn after first decals

// FEATURES
// FPS-style camera mode (option in View panel)
// Filters for texture list

// Spline tunnels between two sides
// Should be able to adjust the handle lengths (doesn't have to be smooth adjustment)
// - Hermite interpolation seems to be the answer
// - Create linear interpolation first, then math can be added later to modify

// MAYBE
// Default setting for wall/ceiling/floor (after more textures)
// Rotate in small increments (for geometry)

namespace OverloadLevelEditor
{
	public enum ViewType
	{
		TOP,
		RIGHT,
		FRONT,
		PERSP,

		NUM,
	}

	public partial class Editor : EditorDockContent, OverloadLevelEditor.IEditor
    {
		public Level m_level;
		public Level m_lvl_buffer; // For copy/paste/etc
		public string m_filepath_levels = "";
		public string m_filepath_level_textures = "";
		public string m_filepath_decals = "";
		public string m_filepath_decal_textures = "";
		public string m_filepath_root = "C:\\projects\\overload\\Editor";

        public bool IsHeadlessProxyEditor { get { return false; } }
        public Level LoadedLevel { get { return m_level; } }
        public EditMode ActiveEditMode {
            get { return m_edit_mode; }
            set { m_edit_mode = value; }
        }
        public SideSelect ActiveSideSelect { get { return m_side_select; } }
        public PivotMode ActivePivotMode { get { return m_pivot_mode; } }
        public DragMode ActiveDragMode { get { return m_drag_mode; } }
		public InsertDecal ActiveInsertDecal { get { return m_insert_decal; } }
		public float CurrGridSnap { get { return m_grid_snap; } }
        public bool IsAutoCenter { get { return m_auto_center; } }
        public int CurrExtrudeLength { get { return m_extrude_length; } }
        public bool ShouldInsertAdvance { get { return m_insert_advance; } }
        public Matrix4 SourceSideRotation { get { return m_src_side_rotation; } }
        public Matrix4 DestSideRotation { get { return m_dest_side_rotation; } }

		public static bool EditorLoaded = false;


        public Editor(EditorShell shell)
			: base(shell)
		{
			InitializeComponent();

			SetupKeyboard();
		}

		private void Editor_Load(object sender, EventArgs e)
		{
			UpdateDirectories();

			LoadTextureSets();		//Do this before NewLevel()

			NewLevel();
			UndoInit();
			KeyPreview = true;
			m_lvl_buffer = new Level(this);

			gl_panel.Initialize(this);

			LoadPreferences();
			UpdateOptionLabels();
			EntityEditPane.UpdateEntityLabels();
			RefreshGrid();
			gl_panel.ResetViews();


			//Update initial states
			GeometryDecalsPane.UpdateDecalLabels();
			EditorLoaded = true;
		}

		//I don't know where to put this
		public List<TextureSet> TextureSets { get; set; }

		public void LoadTextureSets()
		{
			string path_to_file = Path.Combine(m_filepath_level_textures, "TextureSets.json");

			try {
				string serialized_data = File.ReadAllText(path_to_file);

				TextureSets = JsonConvert.DeserializeObject<List<TextureSet>>(serialized_data);
			}
			catch (Exception ex) {
				Utility.DebugLog("Failed to load texture set data: " + ex.Message);

				string error;
				if (ex is Newtonsoft.Json.JsonReaderException) {
					error = "Error parsing texture set file:";
				} else {
					error = "Error reading texture set file:";
				}

				MessageBox.Show(error + "\n\n" + ex.Message + "\n\nCreating default texture set.", "Texture Set Error");
			
				//Create default texure set
				TextureSet default_texture_set = new TextureSet();
				default_texture_set.Name = "Default";
				default_texture_set.Floor = "mat_foundryfloor_03b";
				default_texture_set.Ceiling = "mat_foundryceramic01";
				default_texture_set.Wall = "mat_foundryconcrete01b";
				default_texture_set.Cave = "rockwall_01a";

				TextureSets = new List<TextureSet>();
				TextureSets.Add(default_texture_set);
			}
		}

		public void SaveTextureSets()
		{
			string serialized_data = JsonConvert.SerializeObject(TextureSets, Formatting.Indented);

			string path_to_file = System.IO.Path.Combine(m_filepath_level_textures, "TextureSets.json");

			if (!Overload.Perforce.EnsureFileAddedToSourceControl(path_to_file) || !Overload.Perforce.EnsureFileCheckedOutInSourceControl(path_to_file)) {
				FileInfo fileInfo = new System.IO.FileInfo(path_to_file);
				if (fileInfo.Exists) {
					fileInfo.IsReadOnly = false;
					MessageBox.Show("Warning: Cannot connect to Perforce when writing file\n\n   " + path_to_file + "\n\nMaking file writable; be sure to Reconcile Offline Work when you connect to Perforce.");
				}
			}

			File.WriteAllText(path_to_file, serialized_data);
		}

		public GLView GetView(ViewType view_type)
		{
			return gl_panel.GetView(view_type);
		}

		public void RefreshSelectedGMeshes()
		{
			const bool check_for_decal_issues = false;
			m_level.RefreshSideGMeshes(m_level.GetSelectedSide(), check_for_decal_issues);
			gl_panel.BuildLevelGMesh();
		}

		public void RefreshMarkedSideGMeshes()
		{
			const bool check_for_decal_issues = false;
			foreach (Side side in m_level.GetMarkedSides()) {
				m_level.RefreshSideGMeshes(side, check_for_decal_issues );
			}
			gl_panel.BuildLevelGMesh();
		}

		public void RefreshMarkedSegmentGMeshes()
		{
			const bool check_for_decal_issues = false;
			foreach (Segment seg in m_level.GetMarkedSegments()) {
				foreach (Side side in seg.side) {
					m_level.RefreshSideGMeshes(side, check_for_decal_issues );
				}
			}
			gl_panel.BuildLevelGMesh();
		}

		public void RefreshTaggedSegmentGMeshes()
		{
			const bool check_for_decal_issues = false;
			foreach (Segment seg in m_level.GetTaggedSegments()) {
				foreach (Side side in seg.side) {
					m_level.RefreshSideGMeshes(side, check_for_decal_issues);
				}
			}
			gl_panel.BuildLevelGMesh();
		}

		public void RefreshAllGMeshes()
		{
			const bool check_for_decal_issues = false;
			m_level.RefreshAllGMeshes( check_for_decal_issues );
			gl_panel.BuildLevelGMesh();
		}

		//Don't refresh any of the individual gmeshes; just compile them.  Used when gmeshes deleted.
		public void RefreshLevelGmesh()
		{
			gl_panel.BuildLevelGMesh();
		}

		public void EntityListUpdateEntity(Entity entity)
		{
			EntityListPane.UpdateEntity(entity);
		}

		public void EntityListRemoveEntity(Entity entity)
		{
			EntityListPane.RemoveEntity(entity);
		}

		public void EntityListSetSelectedEntity(int entity)
		{
			EntityListPane.SetSelected(entity);
		}

		Vector3 m_item_text_scale = new Vector3(0.04f, 0.04f, 0.04f);

		public void UpdateSegmentNumbering()
		{
			gl_panel.RemoveAllTextItems();
			
			switch (m_show_3d_text_type) {
				default:
					break;
				case ShowTextType.MARK_SEGS:
				case ShowTextType.ALL_SEGS:
					foreach (Segment seg in m_level.EnumerateVisibleSegments()) {
						if ((m_show_3d_text_type == ShowTextType.MARK_SEGS) && !seg.marked && m_level.selected_segment != seg.num) {
							continue;
						}
						GLTextItem item = new GLTextItem(seg.FindCenter(), seg.num.ToString(), Color.Red, false);
						item.Facing = true;
						item.Scale = new Vector3(0.06f, 0.06f, 0.06f);
						gl_panel.AddTextItem(item);
					}
					break;
				case ShowTextType.POWERUPS:
				case ShowTextType.ROBOTS:
				case ShowTextType.MARK_ENTS:
					foreach (Entity ent in m_level.EnumerateAliveEntities()) {
						if (ent.m_segnum > -1 && m_level.segment[ent.m_segnum].m_hidden) {
							continue;
						}
						bool selected = (ent.num == m_level.selected_entity);
                  if (m_show_3d_text_type == ShowTextType.POWERUPS) {
							if (ent.Type == EntityType.ITEM) {
								GLTextItem item = new GLTextItem(ent.position - Vector3.UnitY, (GetItemString(ent)), (selected ? Color.HotPink : Color.Orange), false);
								item.Facing = true;
								item.Scale = m_item_text_scale;
								gl_panel.AddTextItem(item);
							}
						} else if (m_show_3d_text_type == ShowTextType.ROBOTS) {
							if (ent.Type == EntityType.ENEMY) {
								GLTextItem item = new GLTextItem(ent.position - Vector3.UnitY, (GetRobotString(ent)), (selected ? Color.HotPink : Color.Red), false);
								item.Facing = true;
								item.Scale = m_item_text_scale;
								gl_panel.AddTextItem(item);
							}
						} else {
							if (ent.marked || selected) {
								GLTextItem item;
                        if (ent.Type == EntityType.ITEM) {
									item = new GLTextItem(ent.position - Vector3.UnitY, (GetItemString(ent)), (selected ? Color.HotPink : Color.Orange), false);
								} else if (ent.Type == EntityType.ENEMY) {
									item = new GLTextItem(ent.position - Vector3.UnitY, (GetRobotString(ent)), (selected ? Color.HotPink : Color.Red), false);
								} else if(ent.Type == EntityType.PROP) {
									item = new GLTextItem(ent.position - Vector3.UnitY, GetPropString(ent), (selected ? Color.HotPink : Color.Orange), false);
								} else if (ent.Type == EntityType.SCRIPT) {
									item = new GLTextItem(ent.position - Vector3.UnitY, ((ScriptSubType)ent.SubType).ToString(), (selected ? Color.HotPink : Color.Orange), false);
								} else if (ent.Type == EntityType.TRIGGER) {
									item = new GLTextItem(ent.position - Vector3.UnitY, ((TriggerSubType)ent.SubType).ToString(), (selected ? Color.HotPink : Color.Orange), false);
								} else if (ent.Type == EntityType.SPECIAL) {
									item = new GLTextItem(ent.position - Vector3.UnitY, ((SpecialSubType)ent.SubType).ToString(), (selected ? Color.HotPink : Color.Orange), false);
								} else if (ent.Type == EntityType.DOOR) {
									item = new GLTextItem(ent.position - Vector3.UnitY, ((DoorSubType)ent.SubType).ToString(), (selected ? Color.HotPink : Color.Orange), false);
								} else {
									continue;
								}
								item.Facing = true;
								item.Scale = m_item_text_scale;
								gl_panel.AddTextItem(item);
							}
						}
					}
					break;
			}
		}

		public string GetPropString(Entity ent)
		{
			Overload.EntityPropsProp epp = (Overload.EntityPropsProp)ent.entity_props;
			return (((PropSubType)ent.SubType).ToString() + "-" + epp.index.ToString() + (epp.invulnerable ? " [I]" : ""));
		}

		public string GetRobotString(Entity ent)
		{
			Overload.EntityPropsRobot epr = (Overload.EntityPropsRobot)ent.entity_props;
			return (epr.super ? "[$]" : (epr.variant ? "[V]" : "")) + ((Overload.EnemyType)ent.SubType).ToString() + (epr.station ? "::" : "") + (epr.ng_plus ? "+" : "");
      }

		public string GetItemString(Entity ent)
		{
			Overload.EntityPropsItem epi = (Overload.EntityPropsItem)ent.entity_props;
			return ((epi.super ? "[$]" : "") + ((ItemSubType)ent.SubType).ToString() + (epi.secret ? "+" : ""));
		}

		public void SetProjOffsetAllViews(Vector3 pos)
		{
			gl_panel.SetProjOffset(pos);
		}

		public void RefreshGeometry(bool refresh_editor = true)
		{
			m_level.UpdateCounts();
			gl_panel.BuildLevelExceptGMesh();
			UpdateSegmentNumbering();
			if (refresh_editor) {
				this.Refresh();
			}
			UpdateCountLabels();
			TexturingPane.UpdateTextureLabels();
			GeometryDecalsPane.UpdateDecalLabels();
			EntityEditPane.UpdateEntityLabels();
		}

		protected override void OnSizeChanged(EventArgs e)
		{
			// The docking system says we were resized, for some reason
			// this isn't causing the child controls to resize for their
			// Dock property - so, force it to update
			gl_panel.Size = this.Size;
		}

		private void label_editmode_MouseDown(object sender, MouseEventArgs e)
		{
			CycleEditMode(e.Button == MouseButtons.Right);
		}

		private void Editor_KeyDown(object sender, KeyEventArgs e)
		{
			// See EditorKeyboard.cs (which is triggered by GLView)
		}

		public void ShowTextureSetList()
		{
			texture_set_list.TopLevel = true;
			if (!texture_set_list.Visible) {
				texture_set_list.Show(this);
				texture_set_list.Location = m_texture_set_list_loc;
			}
		}

		public void ShowTextureList()
		{
			texture_list.TopLevel = true;
			if (!texture_list.Visible) {
				texture_list.Show(this);
				texture_list.Location = m_tex_list_loc;
			}
		}

		public void ShowDecalList()
		{
			decal_list.TopLevel = true;
			if (!decal_list.Visible) {
				decal_list.Show(this);
				decal_list.Location = m_decal_list_loc;
			}
		}

		public void ShowUVEditor()
		{
			uv_editor.TopLevel = true;
			if (!uv_editor.Visible) {
				uv_editor.Show(this);
				uv_editor.Location = m_uv_editor_loc;
			}
		}

		public void ShowTunnelBuilder()
		{
			TunnelBuilderVisibility = true;
		}

		public void MarkAllFloors()
		{
			SaveStateForUndo("Mark all floors");
			m_level.SelectAllSidesWithNumber((int)SideOrder.BOTTOM);
			RefreshGeometry();
		}

		public void MarkAllWalls()
		{
			SaveStateForUndo("Mark all walls");
			m_level.SelectAllSidesWithNumber((int)SideOrder.LEFT);
			m_level.SelectAllSidesWithNumber((int)SideOrder.RIGHT);
			m_level.SelectAllSidesWithNumber((int)SideOrder.FRONT);
			m_level.SelectAllSidesWithNumber((int)SideOrder.BACK);
			RefreshGeometry();
		}

		public void MarkAllCeilings()
		{
			SaveStateForUndo("Mark all ceilings");
			m_level.SelectAllSidesWithNumber((int)SideOrder.TOP);
			RefreshGeometry();
		}

		public void DoQuickDebugTexturing()
		{
			SaveStateForUndo("Quick debug texturing");
			m_level.QuickTextureMarkedSegments(true);
			RefreshGeometry();
		}

		public void ApplyNoiseToMarkedSides()
		{
			SaveStateForUndo("Noise to marked sides");
			m_level.ApplyNoiseToMarkedSides(0.1f);
			RefreshGeometry();
		}

		public void SmoothMarkedVerts()
		{
			SaveStateForUndo("Smooth marked sides");
			m_level.SmoothMarkedSides(0.05f);
			RefreshGeometry();
		}

		public void FlattenAllNonFlatSides()
		{
			SaveStateForUndo("Flatten non-flat sides");
			m_level.FlattenAllNonFlatSides();
			RefreshGeometry();
		}

		public void ReorientMarkedSegments()
		{
			SaveStateForUndo("Reorient marked segments");
			m_level.ReorientMarkedSegments();
			RefreshGeometry();
		}
	}

	public class GLEditorViewPanel : Panel
	{
		public Label[] label_viewport = new Label[(int)ViewType.NUM];
		public GLView[] gl_view = new GLView[(int)ViewType.NUM];
		public Editor m_editor;

		public GLEditorViewPanel()
		{	
		}

		public void Initialize( Editor editor )
		{
			m_editor = editor;
			this.Resize += new System.EventHandler( this.gl_panel_Resize );

			// Init views and view labels
			Size sz = this.Size;
			sz.Height -= 5;
			sz.Width -= 5;

			// Create GL views and labels
			for( int i = 0; i < (int)ViewType.NUM; i++ ) {
				gl_view[i] = new GLView( (ViewType)i, m_editor );
				gl_view[i].Parent = this;
				switch( i ) {
				case 0:
					gl_view[i].Location = new Point( 0, 0 );
					break;
				case 1:
					gl_view[i].Location = new Point( sz.Width / 2, 0 );
					break;
				case 2:
					gl_view[i].Location = new Point( 0, sz.Height / 2 );
					break;
				case 3:
					gl_view[i].Location = new Point( sz.Width / 2, sz.Height / 2 );
					break;
				}
				gl_view[i].Size = new Size( sz.Width / 2, sz.Height / 2 );
				gl_view[i].Show();
			}

			for( int i = 0; i < (int)ViewType.NUM; i++ ) {
				label_viewport[i] = new Label();
				label_viewport[i].Parent = this;
				label_viewport[i].Location = new Point( gl_view[i].Location.X + 2, gl_view[i].Location.Y + 2 );
				label_viewport[i].AutoSize = true;
				label_viewport[i].Text = ( (ViewType)i ).ToString();
				label_viewport[i].BackColor = GLView.C_bg;
				label_viewport[i].ForeColor = Color.White;
				label_viewport[i].Show();
				this.Controls.Add( label_viewport[i] );
				label_viewport[i].BringToFront();
			}
		}

		public bool IsViewInitialized()
		{
			if( gl_view == null || gl_view.Length == 0 || gl_view[0] == null )
				return false;
			return true;
		}

		public void ResetViews()
		{
			if( !IsViewInitialized() )
				return;

			Size sz = this.Size;
			sz.Height -= 5;
			sz.Width -= 5;

			switch( m_editor.m_view_layout ) {
			case ViewLayout.FOUR_WAY:
				for( int i = 0; i < (int)ViewType.NUM; i++ ) {
					var view = gl_view[i];
					var label_view = label_viewport[i];

					switch( i ) {
					case 0:
						view.Location = new Point( 0, 0 );
						break;
					case 1:
						view.Location = new Point( sz.Width / 2, 0 );
						break;
					case 2:
						view.Location = new Point( 0, sz.Height / 2 );
						break;
					case 3:
						view.Location = new Point( sz.Width / 2, sz.Height / 2 );
						break;
					}
					view.Size = new Size( sz.Width / 2, sz.Height / 2 );
					label_view.Location = new Point( view.Location.X + 2, view.Location.Y + 2 );
				}
				break;
			case ViewLayout.RIGHT:
				for( int i = 0; i < (int)ViewType.NUM; i++ ) {
					var view = gl_view[i];
					var label_view = label_viewport[i];

					switch( i ) {
					case 0:
						view.Location = new Point( 0, 0 );
						view.Size = new Size( sz.Width / 3, sz.Height / 3 );
						break;
					case 1:
						view.Location = new Point( 0, sz.Height / 3 );
						view.Size = new Size( sz.Width / 3, sz.Height / 3 );
						break;
					case 2:
						view.Location = new Point( 0, sz.Height * 2 / 3 );
						view.Size = new Size( sz.Width / 3, sz.Height / 3 );
						break;
					case 3:
						view.Location = new Point( sz.Width / 3, 0 );
						view.Size = new Size( sz.Width * 2 / 3, sz.Height );
						break;
					}

					label_view.Location = new Point( view.Location.X + 2, view.Location.Y + 2 );
				}
				break;
			case ViewLayout.BOTTOM:
					for (int i = 0; i < (int)ViewType.NUM; i++) {
						var view = gl_view[i];
						var label_view = label_viewport[i];

						switch (i) {
							case 0:
								view.Location = new Point(0, 0);
								view.Size = new Size(sz.Width / 3, sz.Height / 3);
								break;
							case 1:
								view.Location = new Point(sz.Width / 3, 0);
								view.Size = new Size(sz.Width / 3, sz.Height / 3);
								break;
							case 2:
								view.Location = new Point(sz.Width * 2 / 3, 0);
								view.Size = new Size(sz.Width / 3, sz.Height / 3);
								break;
							case 3:
								view.Location = new Point(0, sz.Height / 3);
								view.Size = new Size(sz.Width, sz.Height * 2 / 3);
								break;
						}

						label_view.Location = new Point(view.Location.X + 2, view.Location.Y + 2);
					}
					break;
			case ViewLayout.TOP:
				for( int i = 0; i < (int)ViewType.NUM; i++ ) {
					var view = gl_view[i];
					var label_view = label_viewport[i];

					switch( i ) {
					case 0:
						view.Location = new Point( 0, 0 );
						view.Size = new Size( sz.Width, sz.Height * 2 / 3 );
						break;
					case 1:
						view.Location = new Point( 0, sz.Height * 2 / 3 );
						view.Size = new Size( sz.Width / 3, sz.Height / 3 );
						break;
					case 2:
						view.Location = new Point( sz.Width / 3, sz.Height * 2 / 3 );
						view.Size = new Size( sz.Width / 3, sz.Height / 3 );
						break;
					case 3:
						view.Location = new Point( sz.Width * 2 / 3, sz.Height * 2 / 3 );
						view.Size = new Size( sz.Width / 3, sz.Height / 3 );
						break;
					}

					label_view.Location = new Point( view.Location.X + 2, view.Location.Y + 2 );
				}
				break;
			case ViewLayout.DOUBLE:
				for( int i = 0; i < (int)ViewType.NUM; i++ ) {
					var view = gl_view[i];
					var label_view = label_viewport[i];

					switch( i ) {
					case 0:
						view.Location = new Point( 0, 0 );
						view.Size = new Size( sz.Width / 2, sz.Height * 2 / 3 );
						break;
					case 1:
						view.Location = new Point( 0, sz.Height * 2 / 3 );
						view.Size = new Size( sz.Width / 4, sz.Height / 3 );
						break;
					case 2:
						view.Location = new Point( sz.Width / 4, sz.Height * 2 / 3 );
						view.Size = new Size( sz.Width / 4, sz.Height / 3 );
						break;
					case 3:
						view.Location = new Point( sz.Width / 2, 0 );
						view.Size = new Size( sz.Width / 2, sz.Height );
						break;
					}

					label_view.Location = new Point( view.Location.X + 2, view.Location.Y + 2 );
				}
				break;
			}
		}

		private void gl_panel_Resize( object sender, EventArgs e )
		{
			ResetViews();
		}

		public void OnEditorWindowResized()
		{
			if( !IsViewInitialized() )
				return;

			foreach( var v in gl_view ) {
				v.SetupViewport();
				v.Invalidate();
			}
		}

		public GLView GetView( ViewType type )
		{
			return gl_view[(int)type];
		}

		public void FitFrame( List<Vector3> pos_list, bool dont_change_zoom = false )
		{
			foreach( var v in gl_view ) {
				v.FitFrame( pos_list, dont_change_zoom );
			}
		}

		public void BuildLevelGMesh()
		{
			var view = gl_view[0];
			view.BuildLevelGMesh();
		}

		public void BuildLevelExceptGMesh()
		{
			var view = gl_view[0];
			view.BuildLevelExceptGMesh();
		}

		public void DeleteGMesh()
		{
			var view = gl_view[0];
			if( view != null ) {
				view.DeleteGMesh();
			}
		}

		public void RefreshGrid()
		{
			var view = gl_view[0];
			view.UpdateBGColor( m_editor.m_bg_color );
			view.UpdateClearColor();
			view.BuildGridGeometry( m_editor.m_grid_lines, m_editor.m_grid_spacing );
		}
		public void RemoveAllTextItems()
		{
			foreach( GLView v in gl_view ) {
				v.RemoveAllTextItems();
			}
		}

		public void AddTextItem( GLTextItem item )
		{
			foreach( GLView v in gl_view ) {
				v.AddTextItem( item );
			}
		}

		public void SetProjOffset( Vector3 pos )
		{
			foreach( GLView view in gl_view ) {
				view.SetProjOffset( pos );
			}
		}
	}
}
