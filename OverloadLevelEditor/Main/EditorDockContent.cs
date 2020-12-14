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

using System.ComponentModel;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace OverloadLevelEditor
{
	public class EditorDockContent : DockContent
	{
		public EditorDockContent()
			: this( null )
		{
		}

		public EditorDockContent( EditorShell shell )
		{
			// Stash the Shell instance for later use
			Shell = shell;

			if( shell == null ) {
				return;
			}

			// Register to watch for the document changed event and just call to the virtual
			// function so it can easily be handled.
			Shell.ActiveDocumentChanged += ( object sender, ActiveDocumentEventArgs args ) => {
				OnActiveDocumentChanged( args.Document );
			};
		}

		/// <summary>
		/// Access to the Editor Shell that manages the entire application
		/// </summary>
		protected EditorShell Shell { get; private set; }

		/// <summary>
		/// Access the current active document on the shell
		/// </summary>
		[Browsable( false )]
		public Editor ActiveDocument
		{
			get {
				return Shell.ActiveDocument;
			}
		}

		/// <summary>
		/// Called should the active document change
		/// </summary>
		/// <param name="doc"></param>
		public virtual void OnActiveDocumentChanged( Editor doc )
		{
		}

		/// <summary>
		/// Access to the current active document's level data
		/// </summary>
		[Browsable( false )]
		public Level ActiveLevel
		{
			get {
				var editor = ActiveDocument;
				return ( editor == null ) ? null : editor.m_level;
			}
		}

		/// <summary>
		/// Access to the Output pane
		/// </summary>
		[Browsable( false )]
		public EditorOutputPane OutputPane
		{
			get {
				return Shell.OutputPane;
			}
		}

		/// <summary>
		/// Access to the Entity Edit pane (used to create and manage entities in the level)
		/// </summary>
		[Browsable( false )]
		public EditorEntitiesEditPane EntityEditPane
		{
			get {
				return Shell.EntityEditPane;
			}
		}

		/// <summary>
		/// Access to the Texturing pane
		/// </summary>
		[Browsable( false )]
		public EditorTexturingPane TexturingPane
		{
			get {
				return Shell.TexturingPane;
			}
		}

		/// <summary>
		/// Access to the Geometry Decals pane
		/// </summary>
		[Browsable( false )]
		public EditorGeometryDecalsPane GeometryDecalsPane
		{
			get {
				return Shell.GeometryDecalsPane;
			}
		}

		/// <summary>
		/// Access to the Geometry pane
		/// </summary>
		[Browsable( false )]
		public EditorGeometryPane GeometryPane
		{
			get {
				return Shell.GeometryPane;
			}
		}

		[Browsable( false )]
		public EditorViewOptionsPane ViewOptionsPane
		{
			get {
				return Shell.ViewOptionsPane;
			}
		}

		[Browsable( false )]
		public TunnelBuilder TunnelBuilderPane
		{
			get {
				return Shell.TunnelBuilderPane;
			}
		}

		[Browsable(false)]
		public EditorLevelGlobalPane LevelGlobalPane
		{
			get
			{
				return Shell.LevelGlobalPane;
			}
		}

		[Browsable( false )]
		public TextureManager tm_level
		{
			get {
				return Shell.tm_level;
			}
		}

		[Browsable( false )]
		public TextureManager tm_decal
		{
			get {
				return Shell.tm_decal;
			}
		}

		[Browsable( false )]
		public DecalList decal_list
		{
			get {
				return Shell.decal_list;
			}
		}

		[Browsable(false)]
		public UVEditor uv_editor
		{
			get
			{
				return Shell.uv_editor;
			}
		}

		[Browsable(false)]
		public TextureSetList texture_set_list {
			get {
				return Shell.texture_set_list;
			}
		}

		[Browsable( false )]
		public TextureList texture_list
		{
			get {
				return Shell.texture_list;
			}
		}

		[Browsable( false )]
		public int NumRecentFiles
		{
			get {
				return Shell.m_recent_files.Length;
			}
		}

		public void SetRecentFile( int index, string path )
		{
			if( index < 0 || index >= NumRecentFiles )
				return;
			Shell.m_recent_files[index] = path;
		}

		public string GetRecentFile( int index )
		{
			if( index < 0 || index >= NumRecentFiles )
				return string.Empty;
			return Shell.m_recent_files[index];
		}

		[Browsable( false )]
		public bool TunnelBuilderVisibility
		{
			get {
				if( Shell == null )
					return false;
				return Shell.TunnelBuilderVisibility;
			}
			set {
				if( Shell != null ) {
					Shell.TunnelBuilderVisibility = value;
				}
			}
		}

		[Browsable( false )]
		public EntityList EntityListPane
		{
			get {
				return Shell.EntityListPane;
			}
		}

		[Browsable(false)]
		public EditorLevelCustomInfoPane LevelCustomInfoPane
		{
			get
			{
				return Shell.LevelCustomInfoPane;
			}
		}

		[Browsable( false )]
		public bool EntityListVisibility
		{
			get {
				if( Shell == null )
					return false;
				return Shell.EntityListVisibility;
			}
			set {
				if( Shell != null ) {
					Shell.EntityListVisibility = value;
				}
			}
		}

		[Browsable(false)]
		public bool LevelCustomInfoVisibility
		{
			get
			{
				if (Shell == null)
					return false;
				return Shell.LevelCustomInfoVisibility;
			}
			set
			{
				if (Shell != null) {
					Shell.LevelCustomInfoVisibility = value;
				}
			}
		}
		
		private void InitializeComponent()
		{
			this.SuspendLayout();
			// 
			// EditorDockContent
			// 
			this.ClientSize = new System.Drawing.Size(284, 262);
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Name = "EditorDockContent";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			this.ResumeLayout(false);

		}
	}
}
