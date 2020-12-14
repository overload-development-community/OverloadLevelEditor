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
	partial class TextureList
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
			this.listview = new System.Windows.Forms.ListView();
			this.label_filter = new System.Windows.Forms.Label();
			this.button_mark_sides = new System.Windows.Forms.Button();
			this.button1 = new System.Windows.Forms.Button();
			this.label_view_mode = new System.Windows.Forms.Label();
			this.button_apply = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// listview
			// 
			this.listview.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.listview.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.listview.HideSelection = false;
			this.listview.Location = new System.Drawing.Point(9, 55);
			this.listview.Margin = new System.Windows.Forms.Padding(0);
			this.listview.Name = "listview";
			this.listview.Size = new System.Drawing.Size(438, 690);
			this.listview.Sorting = System.Windows.Forms.SortOrder.Ascending;
			this.listview.TabIndex = 0;
			this.listview.TileSize = new System.Drawing.Size(128, 128);
			this.listview.UseCompatibleStateImageBehavior = false;
			this.listview.SelectedIndexChanged += new System.EventHandler(this.listview_SelectedIndexChanged);
			this.listview.MouseEnter += new System.EventHandler(this.listview_MouseEnter);
			// 
			// label_filter
			// 
			this.label_filter.BackColor = System.Drawing.SystemColors.ControlDark;
			this.label_filter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label_filter.Location = new System.Drawing.Point(9, 11);
			this.label_filter.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label_filter.Name = "label_filter";
			this.label_filter.Size = new System.Drawing.Size(144, 19);
			this.label_filter.TabIndex = 35;
			this.label_filter.Text = "Filter: NONE";
			this.label_filter.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// button_mark_sides
			// 
			this.button_mark_sides.Location = new System.Drawing.Point(156, 10);
			this.button_mark_sides.Margin = new System.Windows.Forms.Padding(1);
			this.button_mark_sides.Name = "button_mark_sides";
			this.button_mark_sides.Size = new System.Drawing.Size(143, 21);
			this.button_mark_sides.TabIndex = 59;
			this.button_mark_sides.Text = "Mark Polys With Texture";
			this.button_mark_sides.UseVisualStyleBackColor = true;
			this.button_mark_sides.Click += new System.EventHandler(this.button_mark_sides_Click);
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(156, 33);
			this.button1.Margin = new System.Windows.Forms.Padding(1);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(143, 21);
			this.button1.TabIndex = 60;
			this.button1.Text = "Get Texture From Poly";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// label_view_mode
			// 
			this.label_view_mode.BackColor = System.Drawing.SystemColors.ControlDark;
			this.label_view_mode.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label_view_mode.Location = new System.Drawing.Point(9, 34);
			this.label_view_mode.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label_view_mode.Name = "label_view_mode";
			this.label_view_mode.Size = new System.Drawing.Size(144, 19);
			this.label_view_mode.TabIndex = 61;
			this.label_view_mode.Text = "View: LARGE";
			this.label_view_mode.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.label_view_mode.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_view_mode_MouseDown);
			// 
			// button_apply
			// 
			this.button_apply.Location = new System.Drawing.Point(301, 10);
			this.button_apply.Margin = new System.Windows.Forms.Padding(1);
			this.button_apply.Name = "button_apply";
			this.button_apply.Size = new System.Drawing.Size(146, 44);
			this.button_apply.TabIndex = 63;
			this.button_apply.Text = "Apply Texture";
			this.button_apply.UseVisualStyleBackColor = true;
			this.button_apply.Click += new System.EventHandler(this.button_apply_Click);
			// 
			// TextureList
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(456, 754);
			this.Controls.Add(this.button_apply);
			this.Controls.Add(this.label_view_mode);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.button_mark_sides);
			this.Controls.Add(this.label_filter);
			this.Controls.Add(this.listview);
			this.KeyPreview = true;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "TextureList";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			this.Text = "Texture List";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TextureList_FormClosing);
			this.Load += new System.EventHandler(this.TextureList_Load);
			this.LocationChanged += new System.EventHandler(this.TextureList_LocationChanged);
			this.MouseEnter += new System.EventHandler(this.TextureList_MouseEnter);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ListView listview;
		private System.Windows.Forms.Label label_filter;
		private System.Windows.Forms.Button button_mark_sides;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Label label_view_mode;
		private System.Windows.Forms.Button button_apply;

	}
}