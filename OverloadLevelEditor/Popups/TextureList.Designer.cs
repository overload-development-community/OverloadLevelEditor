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
			this.listview_Textures = new System.Windows.Forms.ListView();
			this.button_MarkSides = new System.Windows.Forms.Button();
			this.button_GetTextureFromSide = new System.Windows.Forms.Button();
			this.label_ViewMode = new System.Windows.Forms.Label();
			this.button_Test = new System.Windows.Forms.Button();
			this.button_apply = new System.Windows.Forms.Button();
			this.button_ClearFilter = new System.Windows.Forms.Button();
			this.textBox_Filter = new System.Windows.Forms.TextBox();
			this.comboBox_TextureCollection = new System.Windows.Forms.ComboBox();
			this.button_NewCollection = new System.Windows.Forms.Button();
			this.button_DeleteCollection = new System.Windows.Forms.Button();
			this.button_ClearCollection = new System.Windows.Forms.Button();
			this.button_cancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// listview_Textures
			// 
			this.listview_Textures.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.listview_Textures.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.listview_Textures.HideSelection = false;
			this.listview_Textures.Location = new System.Drawing.Point(9, 80);
			this.listview_Textures.Margin = new System.Windows.Forms.Padding(0);
			this.listview_Textures.MultiSelect = false;
			this.listview_Textures.Name = "listview_Textures";
			this.listview_Textures.Size = new System.Drawing.Size(438, 664);
			this.listview_Textures.TabIndex = 0;
			this.listview_Textures.UseCompatibleStateImageBehavior = false;
			this.listview_Textures.SelectedIndexChanged += new System.EventHandler(this.listview_Textures_SelectedIndexChanged);
			this.listview_Textures.DoubleClick += new System.EventHandler(this.listview_Textures_DoubleClick);
			this.listview_Textures.MouseClick += new System.Windows.Forms.MouseEventHandler(this.listview_Textures_MouseClick);
			this.listview_Textures.MouseEnter += new System.EventHandler(this.listview_Textures_MouseEnter);
			// 
			// button_MarkSides
			// 
			this.button_MarkSides.Location = new System.Drawing.Point(156, 10);
			this.button_MarkSides.Margin = new System.Windows.Forms.Padding(1);
			this.button_MarkSides.Name = "button_MarkSides";
			this.button_MarkSides.Size = new System.Drawing.Size(143, 21);
			this.button_MarkSides.TabIndex = 59;
			this.button_MarkSides.Text = "Mark Sides With Texture";
			this.button_MarkSides.UseVisualStyleBackColor = true;
			this.button_MarkSides.Click += new System.EventHandler(this.button_MarkSides_Click);
			// 
			// button_GetTextureFromSide
			// 
			this.button_GetTextureFromSide.Location = new System.Drawing.Point(156, 33);
			this.button_GetTextureFromSide.Margin = new System.Windows.Forms.Padding(1);
			this.button_GetTextureFromSide.Name = "button_GetTextureFromSide";
			this.button_GetTextureFromSide.Size = new System.Drawing.Size(143, 21);
			this.button_GetTextureFromSide.TabIndex = 60;
			this.button_GetTextureFromSide.Text = "Get Texture From Side";
			this.button_GetTextureFromSide.UseVisualStyleBackColor = true;
			this.button_GetTextureFromSide.Click += new System.EventHandler(this.button_GetTextureFromSide_Click);
			// 
			// label_ViewMode
			// 
			this.label_ViewMode.BackColor = System.Drawing.SystemColors.ControlDark;
			this.label_ViewMode.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label_ViewMode.Location = new System.Drawing.Point(9, 12);
			this.label_ViewMode.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label_ViewMode.Name = "label_ViewMode";
			this.label_ViewMode.Size = new System.Drawing.Size(144, 19);
			this.label_ViewMode.TabIndex = 61;
			this.label_ViewMode.Text = "View: LARGE";
			this.label_ViewMode.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.label_ViewMode.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_ViewMode_MouseDown);
			// 
			// button_Test
			// 
			this.button_Test.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.button_Test.Location = new System.Drawing.Point(156, 723);
			this.button_Test.Margin = new System.Windows.Forms.Padding(1);
			this.button_Test.Name = "button_Test";
			this.button_Test.Size = new System.Drawing.Size(143, 21);
			this.button_Test.TabIndex = 62;
			this.button_Test.Text = "TEST";
			this.button_Test.UseVisualStyleBackColor = true;
			this.button_Test.Visible = false;
			this.button_Test.Click += new System.EventHandler(this.button_Test_Click);
			// 
			// button_apply
			// 
			this.button_apply.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.button_apply.Location = new System.Drawing.Point(301, 10);
			this.button_apply.Margin = new System.Windows.Forms.Padding(1);
			this.button_apply.Name = "button_apply";
			this.button_apply.Size = new System.Drawing.Size(146, 44);
			this.button_apply.TabIndex = 63;
			this.button_apply.Text = "Apply Texture";
			this.button_apply.UseVisualStyleBackColor = true;
			this.button_apply.Click += new System.EventHandler(this.button_Apply_Click);
			// 
			// button_ClearFilter
			// 
			this.button_ClearFilter.Enabled = false;
			this.button_ClearFilter.Location = new System.Drawing.Point(125, 34);
			this.button_ClearFilter.Name = "button_ClearFilter";
			this.button_ClearFilter.Size = new System.Drawing.Size(29, 20);
			this.button_ClearFilter.TabIndex = 96;
			this.button_ClearFilter.Text = "X";
			this.button_ClearFilter.UseVisualStyleBackColor = true;
			this.button_ClearFilter.Click += new System.EventHandler(this.button_ClearFilter_Click);
			// 
			// textBox_Filter
			// 
			this.textBox_Filter.Location = new System.Drawing.Point(9, 34);
			this.textBox_Filter.Name = "textBox_Filter";
			this.textBox_Filter.Size = new System.Drawing.Size(114, 20);
			this.textBox_Filter.TabIndex = 95;
			this.textBox_Filter.TextChanged += new System.EventHandler(this.textBox_Filter_TextChanged);
			this.textBox_Filter.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_Filter_KeyPress);
			// 
			// comboBox_TextureCollection
			// 
			this.comboBox_TextureCollection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBox_TextureCollection.FormattingEnabled = true;
			this.comboBox_TextureCollection.Location = new System.Drawing.Point(9, 55);
			this.comboBox_TextureCollection.Name = "comboBox_TextureCollection";
			this.comboBox_TextureCollection.Size = new System.Drawing.Size(114, 21);
			this.comboBox_TextureCollection.TabIndex = 97;
			this.comboBox_TextureCollection.SelectedIndexChanged += new System.EventHandler(this.comboBox_TextureCollection_SelectedIndexChanged);
			// 
			// button_NewCollection
			// 
			this.button_NewCollection.Location = new System.Drawing.Point(156, 55);
			this.button_NewCollection.Name = "button_NewCollection";
			this.button_NewCollection.Size = new System.Drawing.Size(143, 21);
			this.button_NewCollection.TabIndex = 99;
			this.button_NewCollection.Text = "New Collection";
			this.button_NewCollection.UseVisualStyleBackColor = true;
			this.button_NewCollection.Click += new System.EventHandler(this.button_NewCollection_Click);
			// 
			// button_DeleteCollection
			// 
			this.button_DeleteCollection.Location = new System.Drawing.Point(301, 55);
			this.button_DeleteCollection.Name = "button_DeleteCollection";
			this.button_DeleteCollection.Size = new System.Drawing.Size(143, 21);
			this.button_DeleteCollection.TabIndex = 100;
			this.button_DeleteCollection.Text = "Delete Collection";
			this.button_DeleteCollection.UseVisualStyleBackColor = true;
			this.button_DeleteCollection.Click += new System.EventHandler(this.button_DeleteCollection_Click);
			// 
			// button_ClearCollection
			// 
			this.button_ClearCollection.Location = new System.Drawing.Point(125, 55);
			this.button_ClearCollection.Name = "button_ClearCollection";
			this.button_ClearCollection.Size = new System.Drawing.Size(29, 20);
			this.button_ClearCollection.TabIndex = 101;
			this.button_ClearCollection.Text = "X";
			this.button_ClearCollection.UseVisualStyleBackColor = true;
			this.button_ClearCollection.Click += new System.EventHandler(this.button_ClearCollection_Click);
			// 
			// button_cancel
			// 
			this.button_cancel.Location = new System.Drawing.Point(158, 10);
			this.button_cancel.Margin = new System.Windows.Forms.Padding(1);
			this.button_cancel.Name = "button_cancel";
			this.button_cancel.Size = new System.Drawing.Size(141, 44);
			this.button_cancel.TabIndex = 102;
			this.button_cancel.Text = "Cancel";
			this.button_cancel.UseVisualStyleBackColor = true;
			this.button_cancel.Click += new System.EventHandler(this.button_cancel_Click);
			// 
			// TextureList
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(456, 754);
			this.Controls.Add(this.button_cancel);
			this.Controls.Add(this.button_ClearCollection);
			this.Controls.Add(this.button_DeleteCollection);
			this.Controls.Add(this.button_NewCollection);
			this.Controls.Add(this.comboBox_TextureCollection);
			this.Controls.Add(this.button_ClearFilter);
			this.Controls.Add(this.textBox_Filter);
			this.Controls.Add(this.button_apply);
			this.Controls.Add(this.button_Test);
			this.Controls.Add(this.label_ViewMode);
			this.Controls.Add(this.button_GetTextureFromSide);
			this.Controls.Add(this.button_MarkSides);
			this.Controls.Add(this.listview_Textures);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "TextureList";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			this.Text = "Texture List";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TextureList_FormClosing);
			this.Load += new System.EventHandler(this.TextureList_Load);
			this.LocationChanged += new System.EventHandler(this.TextureList_LocationChanged);
			this.Layout += new System.Windows.Forms.LayoutEventHandler(this.TextureList_Layout);
			this.MouseEnter += new System.EventHandler(this.TextureList_MouseEnter);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ListView listview_Textures;
		private System.Windows.Forms.Button button_MarkSides;
		private System.Windows.Forms.Button button_GetTextureFromSide;
		private System.Windows.Forms.Label label_ViewMode;
		private System.Windows.Forms.Button button_Test;
		private System.Windows.Forms.Button button_apply;
		private System.Windows.Forms.Button button_ClearFilter;
		private System.Windows.Forms.TextBox textBox_Filter;
		private System.Windows.Forms.ComboBox comboBox_TextureCollection;
		private System.Windows.Forms.Button button_NewCollection;
		private System.Windows.Forms.Button button_DeleteCollection;
		private System.Windows.Forms.Button button_ClearCollection;
		private System.Windows.Forms.Button button_cancel;
	}
}