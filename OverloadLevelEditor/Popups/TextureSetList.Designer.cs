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
	partial class TextureSetList
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TextureSetList));
			this.comboBoxTextureSets = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.buttonSelect = new System.Windows.Forms.Button();
			this.buttonDelete = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.labelWallName = new System.Windows.Forms.Label();
			this.labelFloorName = new System.Windows.Forms.Label();
			this.labelCeilingName = new System.Windows.Forms.Label();
			this.labelCaveName = new System.Windows.Forms.Label();
			this.pictureBoxWall = new System.Windows.Forms.PictureBox();
			this.pictureBoxFloor = new System.Windows.Forms.PictureBox();
			this.pictureBoxCeiling = new System.Windows.Forms.PictureBox();
			this.pictureBoxCave = new System.Windows.Forms.PictureBox();
			this.label6 = new System.Windows.Forms.Label();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonDone = new System.Windows.Forms.Button();
			this.buttonAddNew = new System.Windows.Forms.Button();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.buttonRenameTextureSet = new System.Windows.Forms.Button();
			this.buttonApply = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxWall)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxFloor)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxCeiling)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxCave)).BeginInit();
			this.SuspendLayout();
			// 
			// comboBoxTextureSets
			// 
			this.comboBoxTextureSets.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxTextureSets.FormattingEnabled = true;
			this.comboBoxTextureSets.Location = new System.Drawing.Point(15, 30);
			this.comboBoxTextureSets.Name = "comboBoxTextureSets";
			this.comboBoxTextureSets.Size = new System.Drawing.Size(192, 21);
			this.comboBoxTextureSets.TabIndex = 0;
			this.toolTip1.SetToolTip(this.comboBoxTextureSets, "Select a texture set to view or edit.");
			this.comboBoxTextureSets.SelectedIndexChanged += new System.EventHandler(this.comboBoxTextureSets_SelectedIndexChanged);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(13, 14);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(62, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "Texture Set";
			// 
			// buttonSelect
			// 
			this.buttonSelect.Location = new System.Drawing.Point(302, 27);
			this.buttonSelect.Name = "buttonSelect";
			this.buttonSelect.Size = new System.Drawing.Size(94, 27);
			this.buttonSelect.TabIndex = 2;
			this.buttonSelect.Text = "Select";
			this.toolTip1.SetToolTip(this.buttonSelect, "Set as selected texture set for the level.");
			this.buttonSelect.UseVisualStyleBackColor = true;
			this.buttonSelect.Click += new System.EventHandler(this.buttonSelect_Click);
			// 
			// buttonDelete
			// 
			this.buttonDelete.Image = ((System.Drawing.Image)(resources.GetObject("buttonDelete.Image")));
			this.buttonDelete.Location = new System.Drawing.Point(213, 30);
			this.buttonDelete.Name = "buttonDelete";
			this.buttonDelete.Size = new System.Drawing.Size(21, 21);
			this.buttonDelete.TabIndex = 3;
			this.toolTip1.SetToolTip(this.buttonDelete, "Delete this texture set.");
			this.buttonDelete.UseVisualStyleBackColor = true;
			this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label2.Location = new System.Drawing.Point(12, 95);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(39, 20);
			this.label2.TabIndex = 4;
			this.label2.Text = "Wall";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label3.Location = new System.Drawing.Point(12, 195);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(45, 20);
			this.label3.TabIndex = 5;
			this.label3.Text = "Floor";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label4.Location = new System.Drawing.Point(12, 295);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(56, 20);
			this.label4.TabIndex = 6;
			this.label4.Text = "Ceiling";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label5.Location = new System.Drawing.Point(17, 395);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(45, 20);
			this.label5.TabIndex = 7;
			this.label5.Text = "Cave";
			// 
			// labelWallName
			// 
			this.labelWallName.AutoSize = true;
			this.labelWallName.Location = new System.Drawing.Point(154, 124);
			this.labelWallName.Name = "labelWallName";
			this.labelWallName.Size = new System.Drawing.Size(35, 13);
			this.labelWallName.TabIndex = 8;
			this.labelWallName.Text = "label6";
			// 
			// labelFloorName
			// 
			this.labelFloorName.AutoSize = true;
			this.labelFloorName.Location = new System.Drawing.Point(154, 228);
			this.labelFloorName.Name = "labelFloorName";
			this.labelFloorName.Size = new System.Drawing.Size(35, 13);
			this.labelFloorName.TabIndex = 9;
			this.labelFloorName.Text = "label7";
			// 
			// labelCeilingName
			// 
			this.labelCeilingName.AutoSize = true;
			this.labelCeilingName.Location = new System.Drawing.Point(154, 328);
			this.labelCeilingName.Name = "labelCeilingName";
			this.labelCeilingName.Size = new System.Drawing.Size(35, 13);
			this.labelCeilingName.TabIndex = 10;
			this.labelCeilingName.Text = "label8";
			// 
			// labelCaveName
			// 
			this.labelCaveName.AutoSize = true;
			this.labelCaveName.Location = new System.Drawing.Point(156, 428);
			this.labelCaveName.Name = "labelCaveName";
			this.labelCaveName.Size = new System.Drawing.Size(35, 13);
			this.labelCaveName.TabIndex = 11;
			this.labelCaveName.Text = "label9";
			// 
			// pictureBoxWall
			// 
			this.pictureBoxWall.Location = new System.Drawing.Point(70, 95);
			this.pictureBoxWall.Name = "pictureBoxWall";
			this.pictureBoxWall.Size = new System.Drawing.Size(80, 80);
			this.pictureBoxWall.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.pictureBoxWall.TabIndex = 12;
			this.pictureBoxWall.TabStop = false;
			this.toolTip1.SetToolTip(this.pictureBoxWall, "Click to change texture.");
			this.pictureBoxWall.Click += new System.EventHandler(this.pictureBoxWall_Click);
			// 
			// pictureBoxFloor
			// 
			this.pictureBoxFloor.Location = new System.Drawing.Point(70, 195);
			this.pictureBoxFloor.Name = "pictureBoxFloor";
			this.pictureBoxFloor.Size = new System.Drawing.Size(80, 80);
			this.pictureBoxFloor.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.pictureBoxFloor.TabIndex = 13;
			this.pictureBoxFloor.TabStop = false;
			this.toolTip1.SetToolTip(this.pictureBoxFloor, "Click to change texture.");
			this.pictureBoxFloor.Click += new System.EventHandler(this.pictureBoxFloor_Click);
			// 
			// pictureBoxCeiling
			// 
			this.pictureBoxCeiling.Location = new System.Drawing.Point(70, 295);
			this.pictureBoxCeiling.Name = "pictureBoxCeiling";
			this.pictureBoxCeiling.Size = new System.Drawing.Size(80, 80);
			this.pictureBoxCeiling.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.pictureBoxCeiling.TabIndex = 14;
			this.pictureBoxCeiling.TabStop = false;
			this.toolTip1.SetToolTip(this.pictureBoxCeiling, "Click to change texture.");
			this.pictureBoxCeiling.Click += new System.EventHandler(this.pictureBoxCeiling_Click);
			// 
			// pictureBoxCave
			// 
			this.pictureBoxCave.Location = new System.Drawing.Point(70, 395);
			this.pictureBoxCave.Name = "pictureBoxCave";
			this.pictureBoxCave.Size = new System.Drawing.Size(80, 80);
			this.pictureBoxCave.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.pictureBoxCave.TabIndex = 15;
			this.pictureBoxCave.TabStop = false;
			this.toolTip1.SetToolTip(this.pictureBoxCave, "Click to change texture.");
			this.pictureBoxCave.Click += new System.EventHandler(this.pictureBoxCave_Click);
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label6.Location = new System.Drawing.Point(13, 70);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(134, 13);
			this.label6.TabIndex = 16;
			this.label6.Text = "Click on texture to change.";
			// 
			// buttonCancel
			// 
			this.buttonCancel.Location = new System.Drawing.Point(197, 495);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(94, 27);
			this.buttonCancel.TabIndex = 17;
			this.buttonCancel.Text = "Cancel";
			this.toolTip1.SetToolTip(this.buttonCancel, "Exit without saving changes to texture sets.");
			this.buttonCancel.UseVisualStyleBackColor = true;
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// buttonDone
			// 
			this.buttonDone.Location = new System.Drawing.Point(95, 495);
			this.buttonDone.Name = "buttonDone";
			this.buttonDone.Size = new System.Drawing.Size(94, 27);
			this.buttonDone.TabIndex = 18;
			this.buttonDone.Text = "Done";
			this.toolTip1.SetToolTip(this.buttonDone, "Save changes to texture sets.");
			this.buttonDone.UseVisualStyleBackColor = true;
			this.buttonDone.Click += new System.EventHandler(this.buttonDone_Click);
			// 
			// buttonAddNew
			// 
			this.buttonAddNew.Image = ((System.Drawing.Image)(resources.GetObject("buttonAddNew.Image")));
			this.buttonAddNew.Location = new System.Drawing.Point(267, 29);
			this.buttonAddNew.Name = "buttonAddNew";
			this.buttonAddNew.Size = new System.Drawing.Size(21, 21);
			this.buttonAddNew.TabIndex = 20;
			this.toolTip1.SetToolTip(this.buttonAddNew, "Add a new texture set.");
			this.buttonAddNew.UseVisualStyleBackColor = true;
			this.buttonAddNew.Click += new System.EventHandler(this.buttonAddNew_Click);
			// 
			// buttonRenameTextureSet
			// 
			this.buttonRenameTextureSet.Image = ((System.Drawing.Image)(resources.GetObject("buttonRenameTextureSet.Image")));
			this.buttonRenameTextureSet.Location = new System.Drawing.Point(240, 30);
			this.buttonRenameTextureSet.Name = "buttonRenameTextureSet";
			this.buttonRenameTextureSet.Size = new System.Drawing.Size(21, 21);
			this.buttonRenameTextureSet.TabIndex = 21;
			this.toolTip1.SetToolTip(this.buttonRenameTextureSet, "Rename this texture set.");
			this.buttonRenameTextureSet.UseVisualStyleBackColor = true;
			this.buttonRenameTextureSet.Click += new System.EventHandler(this.buttonRenameTextureSet_Click);
			// 
			// buttonApply
			// 
			this.buttonApply.Location = new System.Drawing.Point(297, 495);
			this.buttonApply.Name = "buttonApply";
			this.buttonApply.Size = new System.Drawing.Size(94, 27);
			this.buttonApply.TabIndex = 22;
			this.buttonApply.Text = "Apply";
			this.toolTip1.SetToolTip(this.buttonApply, "Save changes to texture sets.");
			this.buttonApply.UseVisualStyleBackColor = true;
			this.buttonApply.Click += new System.EventHandler(this.buttonApply_Click);
			// 
			// TextureSetList
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(403, 534);
			this.Controls.Add(this.buttonApply);
			this.Controls.Add(this.buttonRenameTextureSet);
			this.Controls.Add(this.buttonAddNew);
			this.Controls.Add(this.buttonDone);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.pictureBoxCave);
			this.Controls.Add(this.pictureBoxCeiling);
			this.Controls.Add(this.pictureBoxFloor);
			this.Controls.Add(this.pictureBoxWall);
			this.Controls.Add(this.labelCaveName);
			this.Controls.Add(this.labelCeilingName);
			this.Controls.Add(this.labelFloorName);
			this.Controls.Add(this.labelWallName);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.buttonDelete);
			this.Controls.Add(this.buttonSelect);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.comboBoxTextureSets);
			this.Name = "TextureSetList";
			this.Text = "Texture Sets";
			this.toolTip1.SetToolTip(this, "Make this the default texture set for the level");
			this.Activated += new System.EventHandler(this.TextureSetList_Activated);
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TextureSetList_FormClosing);
			this.Load += new System.EventHandler(this.TextureSetList_Load);
			this.LocationChanged += new System.EventHandler(this.TextureSetList_LocationChanged);
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxWall)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxFloor)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxCeiling)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxCave)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ComboBox comboBoxTextureSets;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button buttonSelect;
		private System.Windows.Forms.Button buttonDelete;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label labelWallName;
		private System.Windows.Forms.Label labelFloorName;
		private System.Windows.Forms.Label labelCeilingName;
		private System.Windows.Forms.Label labelCaveName;
		private System.Windows.Forms.PictureBox pictureBoxWall;
		private System.Windows.Forms.PictureBox pictureBoxFloor;
		private System.Windows.Forms.PictureBox pictureBoxCeiling;
		private System.Windows.Forms.PictureBox pictureBoxCave;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonDone;
		private System.Windows.Forms.Button buttonAddNew;
		private System.Windows.Forms.Button buttonRenameTextureSet;
		private System.Windows.Forms.Button buttonApply;
	}
}