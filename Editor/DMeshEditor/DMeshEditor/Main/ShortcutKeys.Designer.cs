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
	partial class ShortcutKeys
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.label_shortcuts = new System.Windows.Forms.Label();
			this.label_descriptions = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// label_shortcuts
			// 
			this.label_shortcuts.AutoSize = true;
			this.label_shortcuts.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label_shortcuts.Location = new System.Drawing.Point(5, 5);
			this.label_shortcuts.Name = "label_shortcuts";
			this.label_shortcuts.Size = new System.Drawing.Size(112, 14);
			this.label_shortcuts.TabIndex = 0;
			this.label_shortcuts.Text = "label_shortcuts";
			// 
			// label_descriptions
			// 
			this.label_descriptions.AutoSize = true;
			this.label_descriptions.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label_descriptions.Location = new System.Drawing.Point(134, 5);
			this.label_descriptions.Name = "label_descriptions";
			this.label_descriptions.Size = new System.Drawing.Size(133, 14);
			this.label_descriptions.TabIndex = 1;
			this.label_descriptions.Text = "label_descriptions";
			// 
			// ShortcutKeys
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(643, 587);
			this.Controls.Add(this.label_descriptions);
			this.Controls.Add(this.label_shortcuts);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ShortcutKeys";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.Text = "ShortcutKeys";
			this.TopMost = true;
			this.Load += new System.EventHandler(this.ShortcutKeys_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label_shortcuts;
		private System.Windows.Forms.Label label_descriptions;
	}
}