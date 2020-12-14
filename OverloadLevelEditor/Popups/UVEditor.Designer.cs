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

namespace OverloadLevelEditor
{
	partial class UVEditor : Form
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
			this.view = new System.Windows.Forms.PictureBox();
			this.slider_grid_snap = new OverloadLevelEditor.SliderLabel();
			this.button_snap_to_grid = new System.Windows.Forms.Button();
			this.button_snap_marked_pairs = new System.Windows.Forms.Button();
			this.button_snap_selected = new System.Windows.Forms.Button();
			this.button_GetTextureFromSide = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.view)).BeginInit();
			this.SuspendLayout();
			// 
			// view
			// 
			this.view.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.view.BackColor = System.Drawing.SystemColors.ButtonHighlight;
			this.view.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.view.Location = new System.Drawing.Point(0, 0);
			this.view.Margin = new System.Windows.Forms.Padding(0);
			this.view.Name = "view";
			this.view.Size = new System.Drawing.Size(817, 814);
			this.view.TabIndex = 0;
			this.view.TabStop = false;
			this.view.KeyDown += new System.Windows.Forms.KeyEventHandler(this.view_KeyDown);
			this.view.ClientSizeChanged += new System.EventHandler(this.view_ClientSizeChanged);
			this.view.Paint += new System.Windows.Forms.PaintEventHandler(this.view_Paint);
			this.view.MouseDown += new System.Windows.Forms.MouseEventHandler(this.view_MouseDown);
			this.view.MouseEnter += new System.EventHandler(this.view_MouseEnter);
			this.view.MouseMove += new System.Windows.Forms.MouseEventHandler(this.view_MouseMove);
			this.view.MouseUp += new System.Windows.Forms.MouseEventHandler(this.view_MouseUp);
			this.view.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.view_MouseWheel);
			this.view.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.view_PreviewKeyDown);
			// 
			// slider_grid_snap
			// 
			this.slider_grid_snap.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.slider_grid_snap.Location = new System.Drawing.Point(818, 22);
			this.slider_grid_snap.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.slider_grid_snap.Name = "slider_grid_snap";
			this.slider_grid_snap.RightMouseMultiplier = 2;
			this.slider_grid_snap.Size = new System.Drawing.Size(143, 19);
			this.slider_grid_snap.SlideText = "Grid Snap";
			this.slider_grid_snap.SlideTol = 30;
			this.slider_grid_snap.TabIndex = 107;
			this.slider_grid_snap.ToolTop = "Oh yeah!";
			this.slider_grid_snap.ValueText = "0.125";
			this.slider_grid_snap.Feedback += new System.EventHandler<SliderLabelArgs>(this.slider_grid_snap_Feedback);
			// 
			// button_snap_to_grid
			// 
			this.button_snap_to_grid.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_snap_to_grid.Location = new System.Drawing.Point(818, 0);
			this.button_snap_to_grid.Margin = new System.Windows.Forms.Padding(1);
			this.button_snap_to_grid.Name = "button_snap_to_grid";
			this.button_snap_to_grid.Size = new System.Drawing.Size(143, 21);
			this.button_snap_to_grid.TabIndex = 106;
			this.button_snap_to_grid.Text = "Snap To Grid";
			this.button_snap_to_grid.UseVisualStyleBackColor = true;
			this.button_snap_to_grid.Click += new System.EventHandler(this.button_snap_to_grid_Click);
			// 
			// button_snap_marked_pairs
			// 
			this.button_snap_marked_pairs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_snap_marked_pairs.Location = new System.Drawing.Point(818, 52);
			this.button_snap_marked_pairs.Margin = new System.Windows.Forms.Padding(1);
			this.button_snap_marked_pairs.Name = "button_snap_marked_pairs";
			this.button_snap_marked_pairs.Size = new System.Drawing.Size(143, 21);
			this.button_snap_marked_pairs.TabIndex = 108;
			this.button_snap_marked_pairs.Text = "Snap Marked Pairs";
			this.button_snap_marked_pairs.UseVisualStyleBackColor = true;
			this.button_snap_marked_pairs.Click += new System.EventHandler(this.button_snap_marked_pairs_Click);
			// 
			// button_snap_selected
			// 
			this.button_snap_selected.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_snap_selected.Location = new System.Drawing.Point(818, 75);
			this.button_snap_selected.Margin = new System.Windows.Forms.Padding(1);
			this.button_snap_selected.Name = "button_snap_selected";
			this.button_snap_selected.Size = new System.Drawing.Size(143, 21);
			this.button_snap_selected.TabIndex = 109;
			this.button_snap_selected.Text = "Snap To Selected";
			this.button_snap_selected.UseVisualStyleBackColor = true;
			this.button_snap_selected.Click += new System.EventHandler(this.button_snap_selected_Click);
			// 
			// button_GetTextureFromSide
			// 
			this.button_GetTextureFromSide.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_GetTextureFromSide.Location = new System.Drawing.Point(818, 98);
			this.button_GetTextureFromSide.Margin = new System.Windows.Forms.Padding(1);
			this.button_GetTextureFromSide.Name = "button_GetTextureFromSide";
			this.button_GetTextureFromSide.Size = new System.Drawing.Size(143, 21);
			this.button_GetTextureFromSide.TabIndex = 110;
			this.button_GetTextureFromSide.Text = "Get Texture From Side";
			this.button_GetTextureFromSide.UseVisualStyleBackColor = true;
			this.button_GetTextureFromSide.Click += new System.EventHandler(this.button_GetTextureFromSide_Click);
			// 
			// UVEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(963, 813);
			this.Controls.Add(this.button_GetTextureFromSide);
			this.Controls.Add(this.button_snap_selected);
			this.Controls.Add(this.button_snap_marked_pairs);
			this.Controls.Add(this.slider_grid_snap);
			this.Controls.Add(this.button_snap_to_grid);
			this.Controls.Add(this.view);
			this.Name = "UVEditor";
			this.Text = "UVEditor";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.UVEditor_FormClosing);
			this.Load += new System.EventHandler(this.UVEditor_Load);
			this.Shown += new System.EventHandler(this.UVEditor_Shown);
			this.LocationChanged += new System.EventHandler(this.UVEditor_LocationChanged);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.UVEditor_KeyDown);
			((System.ComponentModel.ISupportInitialize)(this.view)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.PictureBox view;
		private SliderLabel slider_grid_snap;
		private System.Windows.Forms.Button button_snap_to_grid;
		private System.Windows.Forms.Button button_snap_marked_pairs;
		private System.Windows.Forms.Button button_snap_selected;
		private Button button_GetTextureFromSide;
	}
}