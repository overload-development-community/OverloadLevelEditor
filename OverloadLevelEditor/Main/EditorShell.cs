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
using System.Reflection;
using System.IO;
using WeifenLuo.WinFormsUI.Docking;
using WeakEvent;
using OpenTK;

namespace OverloadLevelEditor
{
	public partial class EditorShell : Form
	{
		public EditorOutputPane OutputPane { get; private set; }
		public EditorEntitiesEditPane EntityEditPane { get; private set; }
		public EditorTexturingPane TexturingPane { get; private set; }
		public EditorGeometryDecalsPane GeometryDecalsPane { get; private set; }
		public EditorGeometryPane GeometryPane { get; private set; }
		public EditorViewOptionsPane ViewOptionsPane { get; private set; }
		public EditorLevelGlobalPane LevelGlobalPane { get; private set; }
		public TunnelBuilder TunnelBuilderPane { get; private set; }
		public EntityList EntityListPane { get; private set; }
		public EditorLevelCustomInfoPane LevelCustomInfoPane { get; private set; }

		#region ActiveDocument / ActiveDocumentChanged
		public Editor ActiveDocument {
			get { return m_active_document; }

			private set {
				m_active_document = value;
				m_active_doc_changed_event_source.Raise(this, new ActiveDocumentEventArgs(m_active_document));
			}
		}
		private Editor m_active_document = null;

		private readonly WeakEventSource<ActiveDocumentEventArgs> m_active_doc_changed_event_source = new WeakEventSource<ActiveDocumentEventArgs>();
		public event EventHandler<ActiveDocumentEventArgs> ActiveDocumentChanged {
			add { m_active_doc_changed_event_source.Subscribe(value); }
			remove { m_active_doc_changed_event_source.Unsubscribe(value); }
		}
		#endregion

		public Level ActiveLevel {
			get { return ActiveDocument.m_level; }
		}

		public TextureManager tm_level;
		public TextureManager tm_decal;
		public DecalList decal_list;
		public UVEditor uv_editor;
		public TextureList texture_list;
		public TextureSetList texture_set_list;
		public string[] m_recent_files = { "", "", "", "" };

		#region CurrentLevelFilePath
		private string m_current_level_filepath = null;
		public string CurrentLevelFilePath {
			get {
				return m_current_level_filepath;
			}

			set {
				string filepath = value;
				if (string.IsNullOrWhiteSpace(filepath)) {
					filepath = null;
				}
				m_current_level_filepath = filepath;

				if (filepath == null) {
					this.Text = "Overload Level Editor - <New File>";
				} else {
					this.Text = "Overload Level Editor - " + filepath;
				}
			}
		}
		#endregion

		public EditorShell()
		{
			InitializeComponent();

			// Note: If adding new docking content, update GetContentFromPersistString, RestoreDefaultLayout, and DisconnectDockWindows
			OutputPane = new EditorOutputPane(this);
			EntityEditPane = new EditorEntitiesEditPane(this);
			TexturingPane = new EditorTexturingPane(this);
			GeometryDecalsPane = new EditorGeometryDecalsPane(this);
			GeometryPane = new EditorGeometryPane(this);
			ViewOptionsPane = new EditorViewOptionsPane(this);
			TunnelBuilderPane = new TunnelBuilder(this);
			EntityListPane = new EntityList(this);
			LevelGlobalPane = new EditorLevelGlobalPane(this);
			LevelCustomInfoPane = new EditorLevelCustomInfoPane(this);
			ActiveDocument = new Editor(this);

			// We need to have the Editor associated with dockPanel before we continue
			// We'll load the layout later on when we get the Load event
			ActiveDocument.Show(dockPanel, DockState.Document);

			Editor editor = ActiveDocument;
			tm_level = new TextureManager(editor);
			tm_decal = new TextureManager(editor);

			texture_list = new TextureList(editor);
			decal_list = new DecalList(editor);
			texture_set_list = new TextureSetList(editor);
			uv_editor = new UVEditor(editor);
		}

		private void CheckTextureLists()
		{
			string errors = "";
			if (tm_level.m_name.Count == 0)
			{
				MessageBox.Show("Errors in texture sets:\n\n" + "Cannot find default texture.", "Texture Set Errors");
			}
			else
			{
				string default_texture = tm_level.m_name[0];
				//Go through all texture sets and make sure textures exist
				foreach (TextureSet texture_set in ActiveDocument.TextureSets)
				{
					if (tm_level.FindTextureIDByName(texture_set.Wall) == -1)
					{
						errors += "Cannot find texture '" + texture_set.Wall + "' in texture set '" + texture_set.Name + "'.\n";
						texture_set.Wall = default_texture;
					}
					if (tm_level.FindTextureIDByName(texture_set.Floor) == -1)
					{
						errors += "Cannot find texture '" + texture_set.Floor + "' in texture set '" + texture_set.Name + "'.\n";
						texture_set.Floor = default_texture;
					}
					if (tm_level.FindTextureIDByName(texture_set.Ceiling) == -1)
					{
						errors += "Cannot find texture '" + texture_set.Ceiling + "' in texture set '" + texture_set.Name + "'.\n";
						texture_set.Ceiling = default_texture;
					}
					if (tm_level.FindTextureIDByName(texture_set.Cave) == -1)
					{
						errors += "Cannot find texture '" + texture_set.Cave + "' in texture set '" + texture_set.Name + "'.\n";
						texture_set.Cave = default_texture;
					}
				}
				if (errors != "")
				{
					MessageBox.Show("Errors in texture sets:\n\n" + errors + "\nMissing texutres replaced with '" + default_texture + "'.", "Texture Set Errors");
				}
			}
		}

		private void EditorShell_Load(object sender, EventArgs e)
		{
			var editor = ActiveDocument;
			tm_level.LoadTexturesInDir(editor.m_filepath_level_textures, false);
			tm_decal.LoadTexturesInDir(editor.m_filepath_decal_textures, false, true);
			texture_list.InitImageLists();

			if (tm_level.m_name.Count == 0) {
				MessageBox.Show("Serious trouble: no textures found.");
			}

			//Make sure texture lists can find they textures they reference
			CheckTextureLists();

			texture_list.Hide();
			texture_set_list.Hide();
			decal_list.Show();				//Show to initialize dialog so there's not a huge delay when it's first brought up
			decal_list.Hide();
			uv_editor.Hide();

			//Show pop-ups that should be shown
			if (editor.m_texture_list_visible) {
				editor.ShowTextureList();
			}
			if (editor.m_texture_set_list_visible) {
				editor.ShowTextureSetList();
			}
			if (editor.m_decal_list_visible) {
				editor.ShowDecalList();
			}
			if (editor.m_uv_editor_visible) {
				editor.ShowUVEditor();
			}

			// Initialize Unity Editor-to-Editor connection before doing any further work so
			// we can start sending out updates as soon as possible, and as up to date as possible
			InstallUnityEditorToEditorSync();

			// Initialize the layout of the shell - the preferences should have been loaded by now
			string layout = ActiveDocument.m_editor_layout;
			if (string.IsNullOrWhiteSpace(layout)) {
				RestoreDefaultLayout();
			} else {
				if (!LoadLayout(layout)) {
					RestoreDefaultLayout();
				}
			}

			BuildAssetModels.Prepare();
		}

		private void Editor_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (!ActiveLevel.dirty) {
				return;
			}

			DialogResult dr = MessageBox.Show("Level has changed.  Save it before closing the editor?", "Save Level?", MessageBoxButtons.YesNoCancel);
			if (dr == DialogResult.Cancel) {
				// Nevermind!
				e.Cancel = true;
				return;
			}

			if (dr == DialogResult.Yes) {
				saveToolStripMenuItem_Click(this, EventArgs.Empty);
			}
		}

		private void EditorShell_FormClosed(object sender, FormClosedEventArgs e)
		{
			decal_list.SaveSelectedDecalMesh();

			texture_list.Owner = null;
			decal_list.Owner = null;

			ActiveDocument.SavePreferences();
			UninstallUnityEditorToEditorSync();
		}

		public void UpdateRecentFileMenu()
		{
			recent1ToolStripMenuItem.Enabled = recent1ToolStripMenuItem.Visible = (m_recent_files[0] != "");
			recent2ToolStripMenuItem.Enabled = recent2ToolStripMenuItem.Visible = (m_recent_files[1] != "");
			recent3ToolStripMenuItem.Enabled = recent3ToolStripMenuItem.Visible = (m_recent_files[2] != "");
			recent4ToolStripMenuItem.Enabled = recent4ToolStripMenuItem.Visible = (m_recent_files[3] != "");

			recent1ToolStripMenuItem.Text = m_recent_files[0];
			recent2ToolStripMenuItem.Text = m_recent_files[1];
			recent3ToolStripMenuItem.Text = m_recent_files[2];
			recent4ToolStripMenuItem.Text = m_recent_files[3];
		}

		public bool UndoEnabled {
			get {
				return undoToolStripMenuItem.Enabled;
			}
			set {
				undoToolStripMenuItem.Enabled = value;
			}
		}

		public bool RedoEnabled {
			get {
				return redoToolStripMenuItem.Enabled;
			}
			set {
				redoToolStripMenuItem.Enabled = value;
			}
		}

		#region Unity Editor-to-Editor Sync
#if !PUBLIC_RELEASE
		private Action m_shutdown_editor2editor = null;
#endif

		private void InstallUnityEditorToEditorSync()
		{
#if !PUBLIC_RELEASE
			var instance = EditorToEditorSyncRx.Instance;
			var unsub_packets = instance.ObserveIncomingRawPackets()
				.Subscribe(
				// This function is called when the Unity editor sends us a packet of data
				(byte[] packet_data) => {


				},
				// This function is called if there is an error while reading incoming packets
				(Exception /*ex*/ ) => {


				});

			OverloadLevelEditor.GLView perpsective_view = ActiveDocument.GetView(ViewType.PERSP);

			Vector3 last_cam_pivot_pt = Vector3.Zero;
			float last_cam_fov_y = 90.0f;
			float last_cam_dist = 0.1f;
			float last_cam_angle_x = 0.0f;
			float last_cam_angle_y = 0.0f;

			perpsective_view.InstallRenderUpdateCallback((glv) => {

				float fov_y = Utility.RAD_90 * (float)ActiveDocument.m_view_persp_fov / 90f;
				Vector2 cam_angles = glv.m_cam_angles;

				if (fov_y == last_cam_fov_y &&
					 glv.m_cam_distance == last_cam_dist &&
					 glv.m_proj_offset == last_cam_pivot_pt &&
					 cam_angles.X == last_cam_angle_x &&
					 cam_angles.Y == last_cam_angle_y) {
					// nothing changed
					return;
				}

				// update cache
				last_cam_fov_y = fov_y;
				last_cam_pivot_pt = glv.m_proj_offset;
				last_cam_dist = glv.m_cam_distance;
				last_cam_angle_x = cam_angles.X;
				last_cam_angle_y = cam_angles.Y;

				// send out an update
				using (var memory_stream = new MemoryStream())
				using (var writer = new BinaryWriter(memory_stream)) {

					// header
					writer.Write((byte)42);

					// version
					writer.Write((byte)0);

					// fov y
					writer.Write(fov_y);

					// cam distance
					writer.Write(last_cam_dist);

					// cam pivot pos
					writer.Write(last_cam_pivot_pt.X);
					writer.Write(last_cam_pivot_pt.Y);
					writer.Write(last_cam_pivot_pt.Z);

					// cam angles
					writer.Write(last_cam_angle_x);
					writer.Write(last_cam_angle_y);


					byte[] packet_data = memory_stream.ToArray();
					EditorToEditorSyncRx.Instance.SendRaw(packet_data);
				}
			});

			// Setup the teardown function
			m_shutdown_editor2editor = new Action(() => {

				// No longer want to receive packets
				unsub_packets.Dispose();

				// No longer want to send out updates from the renderer
				perpsective_view.InstallRenderUpdateCallback(null);

				// Cleanup Editor2Editor sync
				instance.Dispose();
			});
#endif // !PUBLIC_RELEASE
		}

		private void UninstallUnityEditorToEditorSync()
		{
#if !PUBLIC_RELEASE
			if (m_shutdown_editor2editor != null) {
				m_shutdown_editor2editor();
				m_shutdown_editor2editor = null;
			}
#endif // !PUBLIC_RELEASE
		}
		#endregion

		/// <summary>
		/// Check to see if the active level is dirty or not, if it is it displays
		/// a prompt to save it.
		/// </summary>
		/// <returns>Returns true if ok to continue, false to cancel operation</returns>
		private bool CheckLevelChangedAndSave()
		{
			if (!ActiveLevel.dirty) {
				return true;
			}

			DialogResult dr = MessageBox.Show("Level has changed.  Save it before continuing?", "Save Level?", MessageBoxButtons.YesNoCancel);
			if (dr == DialogResult.Cancel) {
				// Nevermind!
				return false;
			}

			if (dr == DialogResult.Yes) {
				saveToolStripMenuItem_Click(this, EventArgs.Empty);
			}

			return true;
		}

		private void LoadOrNewFileHelper(Func<bool> activate_level)
		{
			if (!CheckLevelChangedAndSave()) {
				// Nevermind!
				return;
			}

			// Do the work to bring in the new level
			if (!activate_level())
				return;

			ActiveDocument.RefreshAllGMeshes();

			ActiveDocument.RefreshGeometry();
		}

		private void LoadFileHelper(Func<string> get_level_filename)
		{
			LoadOrNewFileHelper(() => {
				string filename = get_level_filename();
				if (filename == null) {
					return false;
				}

				if (!File.Exists(filename)) {
					Utility.DebugPopup("Could not find file.  Loading aborted", "LOADING ERROR");
					return false;
				}

				if (!ActiveDocument.LoadLevel(filename)) {
					Utility.DebugPopup("Failed to load level.  Loading aborted", "LOADING ERROR");
					return false;
				}

				return true;
			});
		}

		private void newToolStripMenuItem_Click(object sender, EventArgs e)
		{
			LoadOrNewFileHelper(() => {
				ActiveDocument.NewLevel();
				return true;
			});
		}

		public void loadToolStripMenuItem_Click(object sender, EventArgs e)
		{
			LoadFileHelper(() => {
				using (var od = new OpenFileDialog()) {
					od.AddExtension = true;
					od.CheckFileExists = true;
					od.CheckPathExists = true;
					od.DefaultExt = ".overload";
					od.Filter = "Overload Levels (*.overload) | *.overload";
					od.Multiselect = false;
					od.Title = "Open Level";
					od.InitialDirectory = ActiveDocument.m_filepath_levels;

					var res = od.ShowDialog();
					if (res != System.Windows.Forms.DialogResult.OK)
						return null;

					return od.FileName;
				}
			});
		}

		private void LoadRecentFile(int idx)
		{
			LoadFileHelper(() => {
				string filename = m_recent_files[idx];
				if (string.IsNullOrWhiteSpace(filename)) {
					return null;
				}
				return filename;
			});
		}

		public void BackupCurrentLevel()
		{
			try {
				string backup_folder = Path.Combine(ActiveDocument.m_filepath_levels, "Backup");
				if (!Directory.Exists(backup_folder)) {
					Directory.CreateDirectory(backup_folder);
				}

				DateTime date = DateTime.Now;
				string filename = string.Format("LevelBackup_{0}_{1}_{2}.overload", date.Month, date.Day, (date.Hour * 3600 + date.Minute * 60 + date.Second));
				string path = Path.Combine(backup_folder, filename);
				if (!ActiveDocument.SaveLevel(path, true)) {
					OutputPane.AddText("!!! Failed to save backup");
				}
			}
			catch (Exception ex) {
				OutputPane.AddText(string.Format("!!! Failed to save backup - {0}", ex.Message));
			}
		}

		private void CheckLevelBeforeSaving()
		{
			int num_errors = ActiveDocument.CheckLevelValidity();
			if (num_errors > 0) {
				Utility.DebugPopup("Found " + num_errors + ((num_errors == 1) ? " error" : " errors") + " in the level.  See output window for details.\n\nThe level will be saved, but it's recommended that you fix the error(s) before using it in the game.", "CRITICAL ERROR");
			}
		}

		private void saveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			// Save a backup first
			BackupCurrentLevel();

			string current_path = CurrentLevelFilePath;
			if (String.IsNullOrWhiteSpace(current_path)) {
                if( !DoSaveAs() ) {
                    return;
                }
			} else {
				Overload.Perforce.EnsureFileCheckedOutInSourceControl(current_path);
				CheckLevelBeforeSaving();
				ActiveDocument.SaveLevel(current_path);
			}
		}

        private bool DoSaveAs()
        {
            using (var sd = new SaveFileDialog())
            {
                sd.AddExtension = true;
                sd.CheckPathExists = true;
                sd.DefaultExt = ".overload";
                sd.FileName = CurrentLevelFilePath;
                sd.Filter = "Overload Levels (*.overload) | *.overload";
                sd.OverwritePrompt = true;
                sd.Title = "Save an Overload Level";
                sd.InitialDirectory = ActiveDocument.m_filepath_levels;

                var res = sd.ShowDialog();
                if (res != System.Windows.Forms.DialogResult.OK)
                    return false;

                var filename = sd.FileName;
                Overload.Perforce.EnsureFileAddedToSourceControl(filename);
                Overload.Perforce.EnsureFileCheckedOutInSourceControl(filename);
                CheckLevelBeforeSaving();
                return ActiveDocument.SaveLevel(filename);
            }
        }

        private void saveasToolStripMenuItem_Click(object sender, EventArgs e)
		{
            DoSaveAs();
        }

		private void exportAsChallengeModeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (var sd = new SaveFileDialog()) {
				var initialFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), Path.Combine("Revival", "Overload"));
				if (!Directory.Exists(initialFolder)) {
					Directory.CreateDirectory(initialFolder);
				}

				const string ext = ".cm";
				string filename = Path.ChangeExtension(Path.GetFileName(CurrentLevelFilePath), ext);

				sd.AddExtension = true;
				sd.CheckPathExists = true;
				sd.DefaultExt = ext;
				sd.FileName = filename;
				sd.Filter = "Overload Challenge Mode Levels (*.cm) | *.cm";
				sd.OverwritePrompt = true;
				sd.Title = "Export an Overload Level for Challenge Mode";
				sd.InitialDirectory = initialFolder;

				var res = sd.ShowDialog();
				if (res != System.Windows.Forms.DialogResult.OK)
					return;

				var outputFilename = sd.FileName;

				// Save a backup first
				BackupCurrentLevel();

				string current_path = CurrentLevelFilePath;
				if (String.IsNullOrWhiteSpace(current_path)) {
					if (!DoSaveAs()) {
						return;
					}
					current_path = CurrentLevelFilePath;
				} else if (ActiveDocument.ActiveLevel.dirty) {
					Overload.Perforce.EnsureFileCheckedOutInSourceControl(current_path);
					CheckLevelBeforeSaving();
					if (!ActiveDocument.SaveLevel(current_path)) {
						return;
					}
				}

				ActiveDocument.ExportLevel(current_path, outputFilename, OverloadLevelExport.LevelType.ChallengeMode);
			}
		}

		private void exportAsSinglePlayerToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (var sd = new SaveFileDialog()) {
				var initialFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), Path.Combine("Revival", "Overload"));
				if (!Directory.Exists(initialFolder)) {
					Directory.CreateDirectory(initialFolder);
				}

				const string ext = ".sp";
				string filename = Path.ChangeExtension(Path.GetFileName(CurrentLevelFilePath), ext);

				sd.AddExtension = true;
				sd.CheckPathExists = true;
				sd.DefaultExt = ext;
				sd.FileName = filename;
				sd.Filter = "Overload Single Player Mode Levels (*.sp) | *.sp";
				sd.OverwritePrompt = true;
				sd.Title = "Export an Overload Level for a Single Player Mission";
				sd.InitialDirectory = initialFolder;

				var res = sd.ShowDialog();
				if (res != System.Windows.Forms.DialogResult.OK)
					return;

				var outputFilename = sd.FileName;

				// Save a backup first
				BackupCurrentLevel();

				string current_path = CurrentLevelFilePath;
				if (String.IsNullOrWhiteSpace(current_path)) {
					if (!DoSaveAs()) {
						return;
					}
					current_path = CurrentLevelFilePath;
				} else if (ActiveDocument.ActiveLevel.dirty) {
					Overload.Perforce.EnsureFileCheckedOutInSourceControl(current_path);
					CheckLevelBeforeSaving();
					if (!ActiveDocument.SaveLevel(current_path)) {
						return;
					}
				}

				ActiveDocument.ExportLevel(current_path, outputFilename, OverloadLevelExport.LevelType.SinglePlayer);
			}
		}

		private void exportAsMultiplayerToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (var sd = new SaveFileDialog()) {
				var initialFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), Path.Combine("Revival", "Overload"));
				if (!Directory.Exists(initialFolder)) {
					Directory.CreateDirectory(initialFolder);
				}

				const string ext = ".mp";
				string filename = Path.ChangeExtension(Path.GetFileName(CurrentLevelFilePath), ext);

				sd.AddExtension = true;
				sd.CheckPathExists = true;
				sd.DefaultExt = ext;
				sd.FileName = filename;
				sd.Filter = "Overload Multiplayer Mode Levels (*.mp) | *.mp";
				sd.OverwritePrompt = true;
				sd.Title = "Export an Overload Level for a Multiplayer Mission";
				sd.InitialDirectory = initialFolder;

				var res = sd.ShowDialog();
				if (res != System.Windows.Forms.DialogResult.OK)
					return;

				var outputFilename = sd.FileName;

				// Save a backup first
				BackupCurrentLevel();

				string current_path = CurrentLevelFilePath;
				if (String.IsNullOrWhiteSpace(current_path)) {
					if (!DoSaveAs()) {
						return;
					}
					current_path = CurrentLevelFilePath;
				} else if (ActiveDocument.ActiveLevel.dirty) {
					Overload.Perforce.EnsureFileCheckedOutInSourceControl(current_path);
					CheckLevelBeforeSaving();
					if (!ActiveDocument.SaveLevel(current_path)) {
						return;
					}
				}

				ActiveDocument.ExportLevel(current_path, outputFilename, OverloadLevelExport.LevelType.MultiPlayer);
			}
		}

		private void recent1ToolStripMenuItem_Click(object sender, EventArgs e)
		{
			LoadRecentFile(0);
		}

		private void recent2ToolStripMenuItem_Click(object sender, EventArgs e)
		{
			LoadRecentFile(1);
		}

		private void recent3ToolStripMenuItem_Click(object sender, EventArgs e)
		{
			LoadRecentFile(2);
		}

		private void recent4ToolStripMenuItem_Click(object sender, EventArgs e)
		{
			LoadRecentFile(3);
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void exitAndSaveLayoutToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ActiveDocument.StoreLayoutToPreferences(GetLayoutConfiguration());
			Close();
		}

		private void undoToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ActiveDocument.RestoreUndo();
		}

		private void redoToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ActiveDocument.RestoreRedo();
		}

		private void markAllFloorsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ActiveDocument.MarkAllFloors();
		}

		private void markAllWallsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ActiveDocument.MarkAllWalls();
		}

		private void markAllCeilingsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ActiveDocument.MarkAllCeilings();
		}

		private void doQuickDebugTexturingToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ActiveDocument.DoQuickDebugTexturing();
		}

		private void splitLevelIntoChunksToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Chunking.DetermineLevelChunking(ActiveDocument.m_level);
		}

		private void applyNoiseToMarkedSidesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ActiveDocument.ApplyNoiseToMarkedSides();
		}

		private void smoothMarkedVertsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ActiveDocument.SmoothMarkedVerts();
		}

		private void flattenAllNonFlatSidesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ActiveDocument.FlattenAllNonFlatSides();
		}

		private void testLevelValidity_ToolStripMenuItem(object sender, EventArgs e)
		{
			int num_errors = ActiveDocument.CheckLevelValidity(true);

			if (num_errors > 0) {
				Utility.DebugPopup("Found " + num_errors + ((num_errors == 1) ? " error" : " errors") + " in the level.\n\nSegments & entities with errors are marked.\n\nSee output window for details.", "CRITICAL ERROR");
			} else {
				Utility.DebugPopup("Found no errors in level.", "Message");
			}
		}

		public void viewToolStripMenuItem_Paint(object sender, PaintEventArgs e)
		{
			entitiesToolStripMenuItem.CheckState = EntityEditPaneVisibility ? CheckState.Checked : CheckState.Unchecked;
			geometryEditToolStripMenuItem.CheckState = GeometryPaneVisibility ? CheckState.Checked : CheckState.Unchecked;
			geometryDecalsToolStripMenuItem.CheckState = GeometryDecalsPaneVisibility ? CheckState.Checked : CheckState.Unchecked;
			texturingToolStripMenuItem.CheckState = TexturingPaneVisibility ? CheckState.Checked : CheckState.Unchecked;
			viewOptionsToolStripMenuItem.CheckState = ViewOptionsPaneVisibility ? CheckState.Checked : CheckState.Unchecked;
			outputToolStripMenuItem.CheckState = OutputPaneVisibility ? CheckState.Checked : CheckState.Unchecked;
			globalDataToolStripMenuItem.CheckState = LevelGlobalPaneVisibility ? CheckState.Checked : CheckState.Unchecked;

			entityListToolStripMenuItem.CheckState = EntityListVisibility ? CheckState.Checked : CheckState.Unchecked;
			decalListToolStripMenuItem.CheckState = decal_list.Visible ? CheckState.Checked : CheckState.Unchecked;
			textureListToolStripMenuItem.CheckState = texture_list.Visible ? CheckState.Checked : CheckState.Unchecked;
			textureSetListToolStripMenuItem.CheckState = texture_set_list.Visible ? CheckState.Checked : CheckState.Unchecked;
			tunnelBuilderToolStripMenuItem.CheckState = TunnelBuilderVisibility ? CheckState.Checked : CheckState.Unchecked;
			levelCustomInfoToolStripMenuItem.CheckState = LevelCustomInfoVisibility ? CheckState.Checked : CheckState.Unchecked;
			uVEditorToolStripMenuItem.CheckState = uv_editor.Visible ? CheckState.Checked : CheckState.Unchecked;
		}

		private void levelCustomInfoToolStripMenuItem_Click(object sender, EventArgs e)
		{
			LevelCustomInfoVisibility = !LevelCustomInfoVisibility;
		}
		
		private void entityListToolStripMenuItem_Click(object sender, EventArgs e)
		{
			EntityListVisibility = !EntityListVisibility;
		}

		private void decalListToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (!decal_list.Visible) {
				ActiveDocument.ShowDecalList();
			} else {
				decal_list.DecalList_FormClosing(this, new FormClosingEventArgs(CloseReason.UserClosing, false));
			}
		}

		private void textureListToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (!texture_list.Visible) {
				ActiveDocument.ShowTextureList();
			} else {
				texture_list.TextureList_FormClosing(this, new FormClosingEventArgs(CloseReason.UserClosing, false));
			}
		}

		private void tunnelBuilderToolStripMenuItem_Click(object sender, EventArgs e)
		{
			TunnelBuilderVisibility = !TunnelBuilderVisibility;
		}

		private void aboutOverloadLevelEditorToolStripMenuItem_Click(object sender, EventArgs e)
		{
			AboutBox dialog = new AboutBox();

			Assembly assembly = Assembly.GetEntryAssembly();

			dialog.label_ProgramName.Text = (assembly.GetCustomAttributes(typeof(AssemblyProductAttribute), false).FirstOrDefault() as AssemblyProductAttribute).Product;
			dialog.label_VersionNum.Text = Application.ProductVersion;
			dialog.label_Copyright.Text = (assembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false).FirstOrDefault() as AssemblyCopyrightAttribute).Copyright;
			dialog.ShowDialog();
		}

		bool GetVisibilityHelper(EditorDockContent content)
		{
			if (content == null)
				return false;

			var dock_state = content.DockState;
			return dock_state != DockState.Hidden && dock_state != DockState.Unknown;
		}

		void SetVisibilityHelper(EditorDockContent content, bool want_visible)
		{
			if (content == null)
				return;

			bool is_visible = (content.DockState != DockState.Hidden && content.DockState != DockState.Unknown);
			if (want_visible == is_visible) {
				return;
			}

			if (want_visible) {
				content.Show(dockPanel);
			} else {
				content.Hide();
			}
		}

		public bool EntityEditPaneVisibility {
			get { return GetVisibilityHelper(EntityEditPane); }
			set { SetVisibilityHelper(EntityEditPane, value); }
		}

		public bool GeometryPaneVisibility {
			get { return GetVisibilityHelper(GeometryPane); }
			set { SetVisibilityHelper(GeometryPane, value); }
		}

		public bool GeometryDecalsPaneVisibility {
			get { return GetVisibilityHelper(GeometryDecalsPane); }
			set { SetVisibilityHelper(GeometryDecalsPane, value); }
		}

		public bool TexturingPaneVisibility {
			get { return GetVisibilityHelper(TexturingPane); }
			set { SetVisibilityHelper(TexturingPane, value); }
		}

		public bool OutputPaneVisibility {
			get { return GetVisibilityHelper(OutputPane); }
			set { SetVisibilityHelper(OutputPane, value); }
		}

		public bool ViewOptionsPaneVisibility {
			get { return GetVisibilityHelper(ViewOptionsPane); }
			set { SetVisibilityHelper(ViewOptionsPane, value); }
		}

		public bool TunnelBuilderVisibility {
			get { return GetVisibilityHelper(TunnelBuilderPane); }
			set { SetVisibilityHelper(TunnelBuilderPane, value); }
		}

		public bool EntityListVisibility {
			get { return GetVisibilityHelper(EntityListPane); }
			set { SetVisibilityHelper(EntityListPane, value); }
		}

		public bool LevelCustomInfoVisibility {
			get { return GetVisibilityHelper(LevelCustomInfoPane); }
			set { SetVisibilityHelper(LevelCustomInfoPane, value); }
		}

		public bool LevelGlobalPaneVisibility {
			get { return GetVisibilityHelper(LevelGlobalPane); }
			set { SetVisibilityHelper(LevelGlobalPane, value); }
		}

		private void entitiesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			EntityEditPaneVisibility = !EntityEditPaneVisibility;
		}

		private void geometryEditToolStripMenuItem_Click(object sender, EventArgs e)
		{
			GeometryPaneVisibility = !GeometryPaneVisibility;
		}

		private void geometryDecalsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			GeometryDecalsPaneVisibility = !GeometryDecalsPaneVisibility;
		}

		private void texturingToolStripMenuItem_Click(object sender, EventArgs e)
		{
			TexturingPaneVisibility = !TexturingPaneVisibility;
		}

		private void viewOptionsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ViewOptionsPaneVisibility = !ViewOptionsPaneVisibility;
		}

		private void outputToolStripMenuItem_Click(object sender, EventArgs e)
		{
			OutputPaneVisibility = !OutputPaneVisibility;
		}

		private void globalDataToolStripMenuItem_Click(object sender, EventArgs e)
		{
			LevelGlobalPaneVisibility = !LevelGlobalPaneVisibility;
		}

		#region Layout management
		private void DisconnectDockWindows()
		{
			ActiveDocument.DockPanel = null;

			OutputPane.DockPanel = null;
			EntityEditPane.DockPanel = null;
			TexturingPane.DockPanel = null;
			GeometryDecalsPane.DockPanel = null;
			GeometryPane.DockPanel = null;
			ViewOptionsPane.DockPanel = null;
			TunnelBuilderPane.DockPanel = null;
			EntityListPane.DockPanel = null;
			LevelCustomInfoPane.DockPanel = null;
			LevelGlobalPane.DockPanel = null;
		}

		private IDockContent GetContentFromPersistString(string persistString)
		{
			// Note: Docking windows can override GetPersistString to return extra information
			if (persistString == typeof(EditorOutputPane).ToString()) {
				return OutputPane;
			} else if (persistString == typeof(EditorEntitiesEditPane).ToString()) {
				return EntityEditPane;
			} else if (persistString == typeof(EditorTexturingPane).ToString()) {
				return TexturingPane;
			} else if (persistString == typeof(EditorGeometryDecalsPane).ToString()) {
				return GeometryDecalsPane;
			} else if (persistString == typeof(EditorGeometryPane).ToString()) {
				return GeometryPane;
			} else if (persistString == typeof(EditorViewOptionsPane).ToString()) {
				return ViewOptionsPane;
			} else if (persistString == typeof(TunnelBuilder).ToString()) {
				return TunnelBuilderPane;
			} else if (persistString == typeof(EntityList).ToString()) {
				return EntityListPane;
			} else if (persistString == typeof(EditorLevelCustomInfoPane).ToString()) {
				return LevelCustomInfoPane;
			} else if (persistString == typeof(EditorLevelGlobalPane).ToString()) {
				return LevelGlobalPane;
			} else {
				return ActiveDocument;
			}
		}

		public void RestoreDefaultLayout()
		{
			dockPanel.SuspendLayout(true);
			try {
				DisconnectDockWindows();

				OutputPane.Show(dockPanel, DockState.DockBottom);
				EntityEditPane.Show(dockPanel, DockState.DockRightAutoHide);
				TexturingPane.Show(dockPanel, DockState.DockRightAutoHide);
				GeometryDecalsPane.Show(dockPanel, DockState.DockRightAutoHide);
				GeometryPane.Show(dockPanel, DockState.DockLeft);
				ViewOptionsPane.Show(dockPanel, DockState.DockLeftAutoHide);
				LevelGlobalPane.Show(dockPanel, DockState.DockLeftAutoHide);
				ActiveDocument.Show(dockPanel, DockState.Document);

				//Non-docking windows
				m_active_document.texture_list.Location = m_active_document.m_tex_list_loc = new Point(20, 20);
				m_active_document.texture_set_list.Location = m_active_document.m_texture_set_list_loc = new Point(40, 40);
				m_active_document.decal_list.Location = m_active_document.m_decal_list_loc = new Point(60, 60);
				m_active_document.uv_editor.Location = m_active_document.m_uv_editor_loc = new Point(80, 80);
			}
			finally {
				dockPanel.ResumeLayout(true, true);
			}

			// This is the actual layout that we want for the default
			LoadLayout(DefaultLayout.layout_string);
		}

		public bool LoadLayout(string layout)
		{
			return SetLayoutConfiguration(layout);
		}

		string GetLayoutConfiguration()
		{
			try {
				using (var memory_stream = new MemoryStream()) {
					dockPanel.SaveAsXml(memory_stream, Encoding.UTF8);
					return Convert.ToBase64String(memory_stream.ToArray());
				}
			}
			catch {
				return null;
			}
		}

		bool SetLayoutConfiguration(string encoded_layout)
		{
			try {
				using (var memory_stream = new MemoryStream(Convert.FromBase64String(encoded_layout))) {
					var deserializer = new DeserializeDockContent(GetContentFromPersistString);

					dockPanel.SuspendLayout(true);
					try {
						DisconnectDockWindows();
						dockPanel.LoadFromXml(memory_stream, deserializer);
					}
					finally {
						dockPanel.ResumeLayout(true, true);
					}
				}
				return true;
			}
			catch {
				return false;
			}
		}

		private void saveLayoutToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ActiveDocument.StoreLayoutToPreferences(GetLayoutConfiguration());
		}

		private void loadLayoutToolStripMenuItem_Click(object sender, EventArgs e)
		{
			string layout = ActiveDocument.m_editor_layout;
			if (string.IsNullOrWhiteSpace(layout)) {
				RestoreDefaultLayout();
			} else {
				LoadLayout(layout);
			}
		}

		private void restoreDefaultLayoutToolStripMenuItem_Click(object sender, EventArgs e)
		{
			RestoreDefaultLayout();
			ActiveDocument.StoreLayoutToPreferences(GetLayoutConfiguration());
		}
		#endregion

		private void manualMarkToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var form = new EditMarkerForm(ActiveDocument);
			form.Show();
		}

		private void reOrientMarkedSegmentsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ActiveDocument.ReorientMarkedSegments();
		}

		private void textureSetListToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (!texture_set_list.Visible) {
				ActiveDocument.ShowTextureSetList();
			} else {
				texture_set_list.TextureSetList_FormClosing(this, new FormClosingEventArgs(CloseReason.UserClosing, false));
			}
		}

		private void fixInvalidDoorReferencesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ActiveDocument.RemoveInvalidDoorReferences();
		}

		private void uVEditorToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (!uv_editor.Visible) {
				ActiveDocument.ShowUVEditor();
			} else {
				uv_editor.UVEditor_FormClosing(this, new FormClosingEventArgs(CloseReason.UserClosing, false));
			}
		}

		private void hideMarkedSegmentsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ActiveDocument.HideMarkedSegments();
		}

		private void hideUnmarkedSegmentsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ActiveDocument.HideUnmarkedSegments();
		}

		private void unhideHiddenSegmentsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ActiveDocument.UnhideHiddenSegments();
		}

		private void editToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
		{
			hideMarkedSegmentsToolStripMenuItem.Enabled = (ActiveDocument.m_level.num_marked_segments > 0);
			hideUnmarkedSegmentsToolStripMenuItem.Enabled = (ActiveDocument.m_level.num_visible_segments > ActiveDocument.m_level.num_marked_segments);
			unhideHiddenSegmentsToolStripMenuItem.Enabled = ActiveDocument.m_level.m_segments_are_hidden;
		}

		private void makeAllDoorsBeSplitPlanesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ActiveDocument.MakeAllDoorsBeSplitPlanes();
		}

		private void findImpassibleSegmentsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ActiveDocument.FindImpassibleSegments();
		}

		private void hiddenToolStripMenuItem_Click(object sender, EventArgs e)
		{

		}

		private void toolStripMenuItem8_Click(object sender, EventArgs e)
		{

		}

		private void clearClipPlanesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show("Are you sure you want to clear all split planes?", "Clear split planes?", MessageBoxButtons.YesNo) == DialogResult.Yes) {
				ActiveDocument.ClearSplitPlanes();
				ActiveDocument.RefreshGeometry();
			}
		}

		private void order5ToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ActiveDocument.MakeAllDoorsBeSplitPlanes(5);
		}

		private void order0ToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ActiveDocument.MakeAllDoorsBeSplitPlanes(0);
		}

		private void averageVertYValuesOfMarkedSidesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ActiveDocument.AverageMarkedSideVertsY();
		}

		private void markImpassibleSegmentsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ActiveDocument.MarkImpassibleSegments();
		}

		private void setAllRobotsToDropPowerupsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ActiveDocument.AllRobotsDropPowerups();
		}

		private void refreshEntityListToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ActiveDocument.EntityListPane.Populate();
		}

		private void copyEntityStatsToClipboardToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ActiveDocument.CopyEntityStatsClipboard();
		}

		private void copyTextureStatsToClipboardToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ActiveDocument.CopyTextureStatsToClipboard();
		}

		private void createSPMissionFileToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (CurrentLevelFilePath != null && CurrentLevelFilePath != "") {
				var init_path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), Path.Combine("Revival", "Overload"));
				if (!Directory.Exists(init_path)) {
					ActiveDocument.AddOutputText("Could not find the directory to create a mission file (export the level first)");
					return;
				}

				const string ext = ".mission";
				string orig_name = Path.GetFileName(CurrentLevelFilePath);
				string extless_name = Path.GetFileNameWithoutExtension(CurrentLevelFilePath);
				string filename = Path.ChangeExtension(orig_name, ext);

				ActiveDocument.ExportSimpleMission(init_path, filename, extless_name);
			} else {
				ActiveDocument.AddOutputText("Could not find the filename to create a mission file (export the level first)");
			}
		}

		private void challengeModeDataHelpToolStripMenuItem_Click(object sender, EventArgs e)
		{

		}

		private void shortcutKeysToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Popups.ShortcutKeys skform = new Popups.ShortcutKeys();
			skform.Show();
		}

		private void createCMDataFileToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (CurrentLevelFilePath != null && CurrentLevelFilePath != "") {
				var init_path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), Path.Combine("Revival", "Overload"));
				if (!Directory.Exists(init_path)) {
					ActiveDocument.AddOutputText("Could not find the directory to create a mission file (export the level first)");
					return;
				}

				const string ext = ".txt";
				string orig_name = Path.GetFileName(CurrentLevelFilePath);
				string extless_name = Path.GetFileNameWithoutExtension(CurrentLevelFilePath);
				string filename = Path.ChangeExtension(orig_name, ext).Replace(extless_name, "challenge_mode_" + extless_name);

				ActiveDocument.GenerateCMData(init_path, filename, extless_name);
			} else {
				ActiveDocument.AddOutputText("Could not find the filename to create a mission file (export the level first)");
			}
		}
	}

	public class ActiveDocumentEventArgs : EventArgs
	{
		public Editor Document { get; private set; }

		public ActiveDocumentEventArgs(Editor editor)
		{
			Document = editor;
		}
	}
}