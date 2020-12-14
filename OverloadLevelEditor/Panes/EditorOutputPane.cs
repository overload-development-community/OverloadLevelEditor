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
	public partial class EditorOutputPane : EditorDockContent
	{
		public EditorOutputPane( EditorShell shell )
			: base( shell )
		{
			InitializeComponent();
		}

		public override void OnActiveDocumentChanged( Editor doc )
		{
			Clear();

			// TODO: If we save the output text on the document, then we should
			// restore it here.
		}

		public void AddText( string format, params object[] args )
		{
			string result_string = string.Format( format, args );
			if( !result_string.EndsWith( Environment.NewLine ) ) {
				textBox.AppendText( result_string + Environment.NewLine );
			} else {
				textBox.AppendText( result_string );
			}

			textBox.Select( textBox.Text.Length - 1, 0 );
			textBox.ScrollToCaret();
		}

		public void Clear()
		{
			textBox.Clear();
		}

		private void clearToolStripMenuItem_Click( object sender, EventArgs e )
		{
			Clear();
		}
	}
}
