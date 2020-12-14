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
using Perforce.P4;

namespace Overload
{
	public partial class Perforce : Form
	{
		public Perforce()
		{
			InitializeComponent();
		}

		private void Perforce_Load( object sender, EventArgs e )
		{
			buttonOk.Enabled = false;
		}

		private void UpdateOk()
		{
			string usernameText = textBoxUserName.Text;
			string clientnameText = textBoxClientName.Text;
			bool isGood = !string.IsNullOrWhiteSpace( usernameText ) && !string.IsNullOrWhiteSpace( clientnameText );
			buttonOk.Enabled = isGood;
		}

		private void textBoxUserName_TextChanged( object sender, EventArgs e )
		{
			UpdateOk();
		}

		private void textBoxClientName_TextChanged( object sender, EventArgs e )
		{
			UpdateOk();
		}

		private void buttonOk_Click( object sender, EventArgs e )
		{
			DialogResult = DialogResult.OK;
			Close();
		}

		private void buttonCancel_Click( object sender, EventArgs e )
		{
			DialogResult = DialogResult.Cancel;
			Close();
		}

#if !PUBLIC_RELEASE
		public static bool ShowConfiguration()
		{
			var form = new Perforce();
			if( form.ShowDialog() != DialogResult.OK ) {
				return false;
			}

			m_cached_username = form.textBoxUserName.Text;
			m_cached_clientname = form.textBoxClientName.Text;
			return true;
		}

		public static Connection GetP4Connection( string user, string ws_client )
		{
			// define the server, repository and connection
			Server server = new Server( new ServerAddress( "ssl:cinder.revivalprod.com:7337" ) );
			Repository rep = new Repository( server );
			Connection con = rep.Connection;

			// use the connection variables for this connection
			con.UserName = user;
			con.Client = new Client();
			con.Client.Name = ws_client;

			// connect to the server
			con.Connect( null );

			return con;
		}

		public static string m_cached_username = null;
		public static string m_cached_clientname = null;

		public static Connection GetP4Connection()
		{
			if( string.IsNullOrWhiteSpace( m_cached_username ) || string.IsNullOrWhiteSpace( m_cached_clientname ) ) {

				// Show the user the configuration dialog
				if( !ShowConfiguration() )
					return null;
			}

			return GetP4Connection( m_cached_username, m_cached_clientname );
		}
#endif // !PUBLIC_RELEASE

		public static bool EnsureFileAddedToSourceControl( params string[] filePaths )
		{
#if PUBLIC_RELEASE
			return true;
#else
			try {
				var conn = GetP4Connection();
				if( conn == null )
					return false;

				FileSpec[] filesToAdd = filePaths
					.Select( path => new FileSpec( null, null, new LocalPath( path ), null ) )
					.ToArray();

				var options = new AddFilesCmdOptions( AddFilesCmdFlags.NoP4Ignore, -1, null );

				IList<FileSpec> filesAdded = conn.Client.AddFiles( options, filesToAdd );

				// filesAdded will be null if the file already exists in Perforce

				return true;
			} catch( Exception ) {
				return false;
			}
#endif // PUBLIC_RELEASE
		}

		public static bool EnsureFileCheckedOutInSourceControl( params string[] filePaths )
		{
#if PUBLIC_RELEASE
			return true;
#else
			try {
				var conn = GetP4Connection();
				if( conn == null )
					return false;

				FileSpec[] filesToEdit = filePaths
					.Select( path => new FileSpec( null, null, new LocalPath( path ), null ) )
					.ToArray();

				var options = new EditCmdOptions( EditFilesCmdFlags.None, -1, null );
				IList<FileSpec> files = conn.Client.EditFiles( options, filesToEdit );

				return true;
			} catch( Exception ) {
				return false;
			}
#endif // PUBLIC_RELEASE
		}
	}	
}
