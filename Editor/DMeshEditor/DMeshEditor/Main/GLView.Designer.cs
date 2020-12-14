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

namespace OverloadLevelEditor
{
	partial class GLView
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.gl_custom = new OpenTK.GLControl();
			this.SuspendLayout();
			// 
			// gl_custom
			// 
			this.gl_custom.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gl_custom.BackColor = System.Drawing.Color.Black;
			this.gl_custom.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.gl_custom.Location = new System.Drawing.Point(0, 0);
			this.gl_custom.Name = "gl_custom";
			this.gl_custom.Size = new System.Drawing.Size(150, 150);
			this.gl_custom.TabIndex = 0;
			this.gl_custom.VSync = false;
			this.gl_custom.Load += new System.EventHandler(this.gl_custom_Load);
			this.gl_custom.Paint += new System.Windows.Forms.PaintEventHandler(this.gl_custom_Paint);
			this.gl_custom.KeyDown += new System.Windows.Forms.KeyEventHandler(this.gl_custom_KeyDown);
			this.gl_custom.KeyUp += new System.Windows.Forms.KeyEventHandler(this.gl_custom_KeyUp);
			this.gl_custom.MouseDown += new System.Windows.Forms.MouseEventHandler(this.gl_custom_MouseDown);
			this.gl_custom.MouseEnter += new System.EventHandler(this.gl_custom_MouseEnter);
			this.gl_custom.MouseMove += new System.Windows.Forms.MouseEventHandler(this.gl_custom_MouseMove);
			this.gl_custom.MouseUp += new System.Windows.Forms.MouseEventHandler(this.gl_custom_MouseUp);
			this.gl_custom.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.gl_custom_MouseWheel);
			this.gl_custom.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.gl_custom_PreviewKeyDown);
			this.gl_custom.Resize += new System.EventHandler(this.gl_custom_Resize);
			// 
			// GLView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.gl_custom);
			this.Name = "GLView";
			this.ResumeLayout(false);

		}

		#endregion

		private OpenTK.GLControl gl_custom;
	}
}
