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
	partial class DMeshBrowser
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
			this.button_clear_filter = new System.Windows.Forms.Button();
			this.textBox_filter = new System.Windows.Forms.TextBox();
			this.listbox = new System.Windows.Forms.ListBox();
			this.label_auto_save = new System.Windows.Forms.Label();
			this.label_auto_load = new System.Windows.Forms.Label();
			this.button_load_selected = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// button_clear_filter
			// 
			this.button_clear_filter.Location = new System.Drawing.Point(169, 3);
			this.button_clear_filter.Margin = new System.Windows.Forms.Padding(0);
			this.button_clear_filter.Name = "button_clear_filter";
			this.button_clear_filter.Size = new System.Drawing.Size(26, 20);
			this.button_clear_filter.TabIndex = 97;
			this.button_clear_filter.Text = "X";
			this.button_clear_filter.UseVisualStyleBackColor = true;
			this.button_clear_filter.Click += new System.EventHandler(this.button_clear_filter_Click);
			// 
			// textBox_filter
			// 
			this.textBox_filter.Location = new System.Drawing.Point(2, 3);
			this.textBox_filter.Margin = new System.Windows.Forms.Padding(0);
			this.textBox_filter.Name = "textBox_filter";
			this.textBox_filter.Size = new System.Drawing.Size(167, 20);
			this.textBox_filter.TabIndex = 96;
			this.textBox_filter.TextChanged += new System.EventHandler(this.textBox_filter_TextChanged);
			this.textBox_filter.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_filter_KeyPress);
			// 
			// listbox
			// 
			this.listbox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.listbox.FormattingEnabled = true;
			this.listbox.Location = new System.Drawing.Point(2, 23);
			this.listbox.Margin = new System.Windows.Forms.Padding(0);
			this.listbox.Name = "listbox";
			this.listbox.Size = new System.Drawing.Size(196, 797);
			this.listbox.TabIndex = 95;
			this.listbox.SelectedIndexChanged += new System.EventHandler(this.listbox_SelectedIndexChanged);
			// 
			// label_auto_save
			// 
			this.label_auto_save.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label_auto_save.BackColor = System.Drawing.SystemColors.ControlDark;
			this.label_auto_save.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label_auto_save.Location = new System.Drawing.Point(2, 843);
			this.label_auto_save.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label_auto_save.Name = "label_auto_save";
			this.label_auto_save.Size = new System.Drawing.Size(97, 19);
			this.label_auto_save.TabIndex = 98;
			this.label_auto_save.Text = "Save: OFF";
			this.label_auto_save.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.label_auto_save.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_auto_save_MouseDown);
			// 
			// label_auto_load
			// 
			this.label_auto_load.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label_auto_load.BackColor = System.Drawing.SystemColors.ControlDark;
			this.label_auto_load.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label_auto_load.Location = new System.Drawing.Point(101, 843);
			this.label_auto_load.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label_auto_load.Name = "label_auto_load";
			this.label_auto_load.Size = new System.Drawing.Size(97, 19);
			this.label_auto_load.TabIndex = 99;
			this.label_auto_load.Text = "Load: ON";
			this.label_auto_load.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.label_auto_load.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_auto_load_MouseDown);
			// 
			// button_load_selected
			// 
			this.button_load_selected.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.button_load_selected.Enabled = false;
			this.button_load_selected.Location = new System.Drawing.Point(2, 822);
			this.button_load_selected.Margin = new System.Windows.Forms.Padding(0);
			this.button_load_selected.Name = "button_load_selected";
			this.button_load_selected.Size = new System.Drawing.Size(196, 20);
			this.button_load_selected.TabIndex = 100;
			this.button_load_selected.Text = "Load Selected";
			this.button_load_selected.UseVisualStyleBackColor = true;
			this.button_load_selected.Click += new System.EventHandler(this.button_load_selected_Click);
			// 
			// DMeshBrowser
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(201, 863);
			this.Controls.Add(this.button_load_selected);
			this.Controls.Add(this.label_auto_load);
			this.Controls.Add(this.label_auto_save);
			this.Controls.Add(this.button_clear_filter);
			this.Controls.Add(this.textBox_filter);
			this.Controls.Add(this.listbox);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.Name = "DMeshBrowser";
			this.Text = "DMeshBrowser";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DMeshBrowser_FormClosing);
			this.Shown += new System.EventHandler(this.DMeshBrowser_Shown);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button button_clear_filter;
		private System.Windows.Forms.TextBox textBox_filter;
		private System.Windows.Forms.ListBox listbox;
		private System.Windows.Forms.Label label_auto_save;
		private System.Windows.Forms.Label label_auto_load;
		private System.Windows.Forms.Button button_load_selected;
	}
}