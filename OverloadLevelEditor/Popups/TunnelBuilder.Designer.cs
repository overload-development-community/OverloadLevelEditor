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
	partial class TunnelBuilder
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
			this.label_arc_direction = new System.Windows.Forms.Label();
			this.slider_noise_smoothing = new OverloadLevelEditor.SliderLabel();
			this.button_create = new System.Windows.Forms.Button();
			this.slider_noise_z = new OverloadLevelEditor.SliderLabel();
			this.slider_noise_y = new OverloadLevelEditor.SliderLabel();
			this.slider_noise_x = new OverloadLevelEditor.SliderLabel();
			this.slider_twist_smooth = new OverloadLevelEditor.SliderLabel();
			this.slider_twist_linear = new OverloadLevelEditor.SliderLabel();
			this.slider_twist_mid = new OverloadLevelEditor.SliderLabel();
			this.label_scale_type = new System.Windows.Forms.Label();
			this.slider_scale_ey = new OverloadLevelEditor.SliderLabel();
			this.slider_scale_ex = new OverloadLevelEditor.SliderLabel();
			this.slider_wave_size_y = new OverloadLevelEditor.SliderLabel();
			this.slider_wave_size_x = new OverloadLevelEditor.SliderLabel();
			this.slider_radius_z = new OverloadLevelEditor.SliderLabel();
			this.slider_radius_x = new OverloadLevelEditor.SliderLabel();
			this.slider_smooth_slope_y = new OverloadLevelEditor.SliderLabel();
			this.slider_smooth_slope_x = new OverloadLevelEditor.SliderLabel();
			this.slider_waves_y = new OverloadLevelEditor.SliderLabel();
			this.slider_waves_x = new OverloadLevelEditor.SliderLabel();
			this.slider_slope_y = new OverloadLevelEditor.SliderLabel();
			this.slider_slope_x = new OverloadLevelEditor.SliderLabel();
			this.slider_arc_segments = new OverloadLevelEditor.SliderLabel();
			this.slider_seg_length = new OverloadLevelEditor.SliderLabel();
			this.slider_segments = new OverloadLevelEditor.SliderLabel();
			this.label_mode = new System.Windows.Forms.Label();
			this.button_connection = new System.Windows.Forms.Button();
			this.slider_handle_selected = new OverloadLevelEditor.SliderLabel();
			this.slider_twists = new OverloadLevelEditor.SliderLabel();
			this.slider_handle_marked = new OverloadLevelEditor.SliderLabel();
			this.slider_connect_seg_size = new OverloadLevelEditor.SliderLabel();
			this.button_reset = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label_arc_direction
			// 
			this.label_arc_direction.BackColor = System.Drawing.SystemColors.ControlDark;
			this.label_arc_direction.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label_arc_direction.Location = new System.Drawing.Point(2, 138);
			this.label_arc_direction.Margin = new System.Windows.Forms.Padding(1);
			this.label_arc_direction.Name = "label_arc_direction";
			this.label_arc_direction.Size = new System.Drawing.Size(143, 19);
			this.label_arc_direction.TabIndex = 122;
			this.label_arc_direction.Text = "Arc Direction: RIGHT";
			this.label_arc_direction.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.label_arc_direction.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_arc_direction_MouseDown);
			// 
			// slider_noise_smoothing
			// 
			this.slider_noise_smoothing.Location = new System.Drawing.Point(2, 546);
			this.slider_noise_smoothing.Margin = new System.Windows.Forms.Padding(1);
			this.slider_noise_smoothing.Name = "slider_noise_smoothing";
			this.slider_noise_smoothing.RightMouseMultiplier = 5;
			this.slider_noise_smoothing.Size = new System.Drawing.Size(144, 19);
			this.slider_noise_smoothing.SlideText = "Smoothing Passes";
			this.slider_noise_smoothing.SlideTol = 10;
			this.slider_noise_smoothing.TabIndex = 121;
			this.slider_noise_smoothing.ToolTop = "How many times to smooth the generated randomness";
			this.slider_noise_smoothing.ValueText = "1";
			this.slider_noise_smoothing.Feedback += new System.EventHandler<SliderLabelArgs>(this.slider_noise_smoothing_Feedback);
			// 
			// button_create
			// 
			this.button_create.Location = new System.Drawing.Point(1, 567);
			this.button_create.Margin = new System.Windows.Forms.Padding(1);
			this.button_create.Name = "button_create";
			this.button_create.Size = new System.Drawing.Size(145, 31);
			this.button_create.TabIndex = 114;
			this.button_create.Text = "Create Tunnel";
			this.button_create.UseVisualStyleBackColor = true;
			this.button_create.Click += new System.EventHandler(this.button_create_Click);
			// 
			// slider_noise_z
			// 
			this.slider_noise_z.Location = new System.Drawing.Point(2, 525);
			this.slider_noise_z.Margin = new System.Windows.Forms.Padding(1);
			this.slider_noise_z.Name = "slider_noise_z";
			this.slider_noise_z.RightMouseMultiplier = 2;
			this.slider_noise_z.Size = new System.Drawing.Size(144, 19);
			this.slider_noise_z.SlideText = "Noise Z";
			this.slider_noise_z.SlideTol = 10;
			this.slider_noise_z.TabIndex = 120;
			this.slider_noise_z.ToolTop = "Randomness in the Z axis";
			this.slider_noise_z.ValueText = "0.0";
			this.slider_noise_z.Feedback += new System.EventHandler<SliderLabelArgs>(this.slider_noise_z_Feedback);
			// 
			// slider_noise_y
			// 
			this.slider_noise_y.Location = new System.Drawing.Point(2, 504);
			this.slider_noise_y.Margin = new System.Windows.Forms.Padding(1);
			this.slider_noise_y.Name = "slider_noise_y";
			this.slider_noise_y.RightMouseMultiplier = 2;
			this.slider_noise_y.Size = new System.Drawing.Size(144, 19);
			this.slider_noise_y.SlideText = "Noise Y";
			this.slider_noise_y.SlideTol = 10;
			this.slider_noise_y.TabIndex = 119;
			this.slider_noise_y.ToolTop = "Randomness in the Y axis";
			this.slider_noise_y.ValueText = "0.0";
			this.slider_noise_y.Feedback += new System.EventHandler<SliderLabelArgs>(this.slider_noise_y_Feedback);
			// 
			// slider_noise_x
			// 
			this.slider_noise_x.Location = new System.Drawing.Point(2, 483);
			this.slider_noise_x.Margin = new System.Windows.Forms.Padding(1);
			this.slider_noise_x.Name = "slider_noise_x";
			this.slider_noise_x.RightMouseMultiplier = 2;
			this.slider_noise_x.Size = new System.Drawing.Size(144, 19);
			this.slider_noise_x.SlideText = "Noise X";
			this.slider_noise_x.SlideTol = 10;
			this.slider_noise_x.TabIndex = 118;
			this.slider_noise_x.ToolTop = "Randomness in the X axis";
			this.slider_noise_x.ValueText = "0.0";
			this.slider_noise_x.Feedback += new System.EventHandler<SliderLabelArgs>(this.slider_noise_x_Feedback);
			// 
			// slider_twist_smooth
			// 
			this.slider_twist_smooth.Location = new System.Drawing.Point(2, 435);
			this.slider_twist_smooth.Margin = new System.Windows.Forms.Padding(1);
			this.slider_twist_smooth.Name = "slider_twist_smooth";
			this.slider_twist_smooth.RightMouseMultiplier = 9;
			this.slider_twist_smooth.Size = new System.Drawing.Size(144, 19);
			this.slider_twist_smooth.SlideText = "Smooth End Twist";
			this.slider_twist_smooth.SlideTol = 10;
			this.slider_twist_smooth.TabIndex = 117;
			this.slider_twist_smooth.ToolTop = "Smoothed twisting amount";
			this.slider_twist_smooth.ValueText = "0";
			this.slider_twist_smooth.Feedback += new System.EventHandler<SliderLabelArgs>(this.slider_twist_smooth_Feedback);
			// 
			// slider_twist_linear
			// 
			this.slider_twist_linear.Location = new System.Drawing.Point(2, 456);
			this.slider_twist_linear.Margin = new System.Windows.Forms.Padding(1);
			this.slider_twist_linear.Name = "slider_twist_linear";
			this.slider_twist_linear.RightMouseMultiplier = 9;
			this.slider_twist_linear.Size = new System.Drawing.Size(144, 19);
			this.slider_twist_linear.SlideText = "Linear End Twist";
			this.slider_twist_linear.SlideTol = 10;
			this.slider_twist_linear.TabIndex = 116;
			this.slider_twist_linear.ToolTop = "Linear twisting amount";
			this.slider_twist_linear.ValueText = "0";
			this.slider_twist_linear.Feedback += new System.EventHandler<SliderLabelArgs>(this.slider_twist_linear_Feedback);
			// 
			// slider_twist_mid
			// 
			this.slider_twist_mid.Location = new System.Drawing.Point(2, 414);
			this.slider_twist_mid.Margin = new System.Windows.Forms.Padding(1);
			this.slider_twist_mid.Name = "slider_twist_mid";
			this.slider_twist_mid.RightMouseMultiplier = 9;
			this.slider_twist_mid.Size = new System.Drawing.Size(144, 19);
			this.slider_twist_mid.SlideText = "Mid-Twist Amount";
			this.slider_twist_mid.SlideTol = 10;
			this.slider_twist_mid.TabIndex = 115;
			this.slider_twist_mid.ToolTop = "Amount of twist in the middle (returns to 0 at end)";
			this.slider_twist_mid.ValueText = "0";
			this.slider_twist_mid.Feedback += new System.EventHandler<SliderLabelArgs>(this.slider_twist_mid_Feedback);
			// 
			// label_scale_type
			// 
			this.label_scale_type.BackColor = System.Drawing.SystemColors.ControlDark;
			this.label_scale_type.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label_scale_type.Location = new System.Drawing.Point(2, 387);
			this.label_scale_type.Margin = new System.Windows.Forms.Padding(1);
			this.label_scale_type.Name = "label_scale_type";
			this.label_scale_type.Size = new System.Drawing.Size(143, 19);
			this.label_scale_type.TabIndex = 114;
			this.label_scale_type.Text = "Scaling: LINEAR";
			this.label_scale_type.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.label_scale_type.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_scale_type_MouseDown);
			// 
			// slider_scale_ey
			// 
			this.slider_scale_ey.Location = new System.Drawing.Point(2, 366);
			this.slider_scale_ey.Margin = new System.Windows.Forms.Padding(1);
			this.slider_scale_ey.Name = "slider_scale_ey";
			this.slider_scale_ey.RightMouseMultiplier = 4;
			this.slider_scale_ey.Size = new System.Drawing.Size(144, 19);
			this.slider_scale_ey.SlideText = "Scale End Y";
			this.slider_scale_ey.SlideTol = 10;
			this.slider_scale_ey.TabIndex = 113;
			this.slider_scale_ey.ToolTop = "Size of the segments at the end of the tunnel";
			this.slider_scale_ey.ValueText = "100";
			this.slider_scale_ey.Feedback += new System.EventHandler<SliderLabelArgs>(this.slider_scale_ey_Feedback);
			// 
			// slider_scale_ex
			// 
			this.slider_scale_ex.Location = new System.Drawing.Point(2, 345);
			this.slider_scale_ex.Margin = new System.Windows.Forms.Padding(1);
			this.slider_scale_ex.Name = "slider_scale_ex";
			this.slider_scale_ex.RightMouseMultiplier = 4;
			this.slider_scale_ex.Size = new System.Drawing.Size(144, 19);
			this.slider_scale_ex.SlideText = "Scale End X";
			this.slider_scale_ex.SlideTol = 10;
			this.slider_scale_ex.TabIndex = 112;
			this.slider_scale_ex.ToolTop = "Size of the segments at the end of the tunnel";
			this.slider_scale_ex.ValueText = "100";
			this.slider_scale_ex.Feedback += new System.EventHandler<SliderLabelArgs>(this.slider_scale_ex_Feedback);
			// 
			// slider_wave_size_y
			// 
			this.slider_wave_size_y.Location = new System.Drawing.Point(2, 318);
			this.slider_wave_size_y.Margin = new System.Windows.Forms.Padding(1);
			this.slider_wave_size_y.Name = "slider_wave_size_y";
			this.slider_wave_size_y.RightMouseMultiplier = 4;
			this.slider_wave_size_y.Size = new System.Drawing.Size(144, 19);
			this.slider_wave_size_y.SlideText = "Wave Size Y";
			this.slider_wave_size_y.SlideTol = 10;
			this.slider_wave_size_y.TabIndex = 109;
			this.slider_wave_size_y.ToolTop = "Size of the Y waves";
			this.slider_wave_size_y.ValueText = "4";
			this.slider_wave_size_y.Feedback += new System.EventHandler<SliderLabelArgs>(this.slider_wave_size_y_Feedback);
			// 
			// slider_wave_size_x
			// 
			this.slider_wave_size_x.Location = new System.Drawing.Point(2, 297);
			this.slider_wave_size_x.Margin = new System.Windows.Forms.Padding(1);
			this.slider_wave_size_x.Name = "slider_wave_size_x";
			this.slider_wave_size_x.RightMouseMultiplier = 4;
			this.slider_wave_size_x.Size = new System.Drawing.Size(144, 19);
			this.slider_wave_size_x.SlideText = "Wave Size X";
			this.slider_wave_size_x.SlideTol = 10;
			this.slider_wave_size_x.TabIndex = 108;
			this.slider_wave_size_x.ToolTop = "Size of the X waves";
			this.slider_wave_size_x.ValueText = "4";
			this.slider_wave_size_x.Feedback += new System.EventHandler<SliderLabelArgs>(this.slider_wave_size_x_Feedback);
			// 
			// slider_radius_z
			// 
			this.slider_radius_z.Location = new System.Drawing.Point(2, 117);
			this.slider_radius_z.Margin = new System.Windows.Forms.Padding(1);
			this.slider_radius_z.Name = "slider_radius_z";
			this.slider_radius_z.RightMouseMultiplier = 5;
			this.slider_radius_z.Size = new System.Drawing.Size(144, 19);
			this.slider_radius_z.SlideText = "Radius Z";
			this.slider_radius_z.SlideTol = 10;
			this.slider_radius_z.TabIndex = 107;
			this.slider_radius_z.ToolTop = "Radius of the arc in the Z direction";
			this.slider_radius_z.ValueText = "20";
			this.slider_radius_z.Feedback += new System.EventHandler<SliderLabelArgs>(this.slider_radius_z_Feedback);
			// 
			// slider_radius_x
			// 
			this.slider_radius_x.Location = new System.Drawing.Point(2, 96);
			this.slider_radius_x.Margin = new System.Windows.Forms.Padding(1);
			this.slider_radius_x.Name = "slider_radius_x";
			this.slider_radius_x.RightMouseMultiplier = 5;
			this.slider_radius_x.Size = new System.Drawing.Size(144, 19);
			this.slider_radius_x.SlideText = "Radius XY";
			this.slider_radius_x.SlideTol = 10;
			this.slider_radius_x.TabIndex = 106;
			this.slider_radius_x.ToolTop = "Radius of the arc in the X/Y direction";
			this.slider_radius_x.ValueText = "20";
			this.slider_radius_x.Feedback += new System.EventHandler<SliderLabelArgs>(this.slider_radius_x_Feedback);
			// 
			// slider_smooth_slope_y
			// 
			this.slider_smooth_slope_y.Location = new System.Drawing.Point(2, 228);
			this.slider_smooth_slope_y.Margin = new System.Windows.Forms.Padding(1);
			this.slider_smooth_slope_y.Name = "slider_smooth_slope_y";
			this.slider_smooth_slope_y.RightMouseMultiplier = 4;
			this.slider_smooth_slope_y.Size = new System.Drawing.Size(144, 19);
			this.slider_smooth_slope_y.SlideText = "Smooth Slope Y";
			this.slider_smooth_slope_y.SlideTol = 10;
			this.slider_smooth_slope_y.TabIndex = 105;
			this.slider_smooth_slope_y.ToolTop = "Slope that fades in/out along tunnel (or 1 full arc)";
			this.slider_smooth_slope_y.ValueText = "0";
			this.slider_smooth_slope_y.Feedback += new System.EventHandler<SliderLabelArgs>(this.slider_smooth_slope_y_Feedback);
			// 
			// slider_smooth_slope_x
			// 
			this.slider_smooth_slope_x.Location = new System.Drawing.Point(2, 207);
			this.slider_smooth_slope_x.Margin = new System.Windows.Forms.Padding(1);
			this.slider_smooth_slope_x.Name = "slider_smooth_slope_x";
			this.slider_smooth_slope_x.RightMouseMultiplier = 4;
			this.slider_smooth_slope_x.Size = new System.Drawing.Size(144, 19);
			this.slider_smooth_slope_x.SlideText = "Smooth Slope X";
			this.slider_smooth_slope_x.SlideTol = 10;
			this.slider_smooth_slope_x.TabIndex = 104;
			this.slider_smooth_slope_x.ToolTop = "Slope that fades in/out along tunnel (or 1 full arc)";
			this.slider_smooth_slope_x.ValueText = "0";
			this.slider_smooth_slope_x.Feedback += new System.EventHandler<SliderLabelArgs>(this.slider_smooth_slope_x_Feedback);
			// 
			// slider_waves_y
			// 
			this.slider_waves_y.Location = new System.Drawing.Point(2, 276);
			this.slider_waves_y.Margin = new System.Windows.Forms.Padding(1);
			this.slider_waves_y.Name = "slider_waves_y";
			this.slider_waves_y.RightMouseMultiplier = 4;
			this.slider_waves_y.Size = new System.Drawing.Size(144, 19);
			this.slider_waves_y.SlideText = "Waves Y";
			this.slider_waves_y.SlideTol = 10;
			this.slider_waves_y.TabIndex = 103;
			this.slider_waves_y.ToolTop = "Number of 1/2 waves in the Y direction";
			this.slider_waves_y.ValueText = "0";
			this.slider_waves_y.Feedback += new System.EventHandler<SliderLabelArgs>(this.slider_waves_y_Feedback);
			// 
			// slider_waves_x
			// 
			this.slider_waves_x.Location = new System.Drawing.Point(2, 255);
			this.slider_waves_x.Margin = new System.Windows.Forms.Padding(1);
			this.slider_waves_x.Name = "slider_waves_x";
			this.slider_waves_x.RightMouseMultiplier = 4;
			this.slider_waves_x.Size = new System.Drawing.Size(144, 19);
			this.slider_waves_x.SlideText = "Waves X";
			this.slider_waves_x.SlideTol = 10;
			this.slider_waves_x.TabIndex = 102;
			this.slider_waves_x.ToolTop = "Number of 1/2 waves in the X direction";
			this.slider_waves_x.ValueText = "0";
			this.slider_waves_x.Feedback += new System.EventHandler<SliderLabelArgs>(this.slider_waves_x_Feedback);
			// 
			// slider_slope_y
			// 
			this.slider_slope_y.Location = new System.Drawing.Point(2, 186);
			this.slider_slope_y.Margin = new System.Windows.Forms.Padding(1);
			this.slider_slope_y.Name = "slider_slope_y";
			this.slider_slope_y.RightMouseMultiplier = 4;
			this.slider_slope_y.Size = new System.Drawing.Size(144, 19);
			this.slider_slope_y.SlideText = "Linear Slope Y";
			this.slider_slope_y.SlideTol = 10;
			this.slider_slope_y.TabIndex = 101;
			this.slider_slope_y.ToolTop = "Slope along tunnel or full radius of arc";
			this.slider_slope_y.ValueText = "0";
			this.slider_slope_y.Feedback += new System.EventHandler<SliderLabelArgs>(this.slider_slope_y_Feedback);
			// 
			// slider_slope_x
			// 
			this.slider_slope_x.Location = new System.Drawing.Point(2, 165);
			this.slider_slope_x.Margin = new System.Windows.Forms.Padding(1);
			this.slider_slope_x.Name = "slider_slope_x";
			this.slider_slope_x.RightMouseMultiplier = 4;
			this.slider_slope_x.Size = new System.Drawing.Size(144, 19);
			this.slider_slope_x.SlideText = "Linear Slope X";
			this.slider_slope_x.SlideTol = 10;
			this.slider_slope_x.TabIndex = 100;
			this.slider_slope_x.ToolTop = "Slope along tunnel or full radius of arc";
			this.slider_slope_x.ValueText = "0";
			this.slider_slope_x.Feedback += new System.EventHandler<SliderLabelArgs>(this.slider_slope_x_Feedback);
			// 
			// slider_arc_segments
			// 
			this.slider_arc_segments.Location = new System.Drawing.Point(2, 75);
			this.slider_arc_segments.Margin = new System.Windows.Forms.Padding(1);
			this.slider_arc_segments.Name = "slider_arc_segments";
			this.slider_arc_segments.RightMouseMultiplier = 5;
			this.slider_arc_segments.Size = new System.Drawing.Size(144, 19);
			this.slider_arc_segments.SlideText = "Per Full Arc";
			this.slider_arc_segments.SlideTol = 10;
			this.slider_arc_segments.TabIndex = 99;
			this.slider_arc_segments.ToolTop = "Segments in a full arc (can be more or less than Segments)";
			this.slider_arc_segments.ValueText = "20";
			this.slider_arc_segments.Feedback += new System.EventHandler<SliderLabelArgs>(this.slider_arc_segments_Feedback);
			// 
			// slider_seg_length
			// 
			this.slider_seg_length.Location = new System.Drawing.Point(2, 54);
			this.slider_seg_length.Margin = new System.Windows.Forms.Padding(1);
			this.slider_seg_length.Name = "slider_seg_length";
			this.slider_seg_length.RightMouseMultiplier = 2;
			this.slider_seg_length.Size = new System.Drawing.Size(144, 19);
			this.slider_seg_length.SlideText = "Segment Length";
			this.slider_seg_length.SlideTol = 10;
			this.slider_seg_length.TabIndex = 99;
			this.slider_seg_length.ToolTop = "Extrusion length per segment";
			this.slider_seg_length.ValueText = "4";
			this.slider_seg_length.Feedback += new System.EventHandler<SliderLabelArgs>(this.slider_seg_length_Feedback);
			// 
			// slider_segments
			// 
			this.slider_segments.Location = new System.Drawing.Point(2, 33);
			this.slider_segments.Margin = new System.Windows.Forms.Padding(1);
			this.slider_segments.Name = "slider_segments";
			this.slider_segments.RightMouseMultiplier = 5;
			this.slider_segments.Size = new System.Drawing.Size(144, 19);
			this.slider_segments.SlideText = "Segments";
			this.slider_segments.SlideTol = 10;
			this.slider_segments.TabIndex = 98;
			this.slider_segments.ToolTop = "Number of segments to create";
			this.slider_segments.ValueText = "10";
			this.slider_segments.Feedback += new System.EventHandler<SliderLabelArgs>(this.slider_segments_Feedback);
			// 
			// label_mode
			// 
			this.label_mode.BackColor = System.Drawing.SystemColors.ControlDark;
			this.label_mode.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label_mode.Location = new System.Drawing.Point(2, 2);
			this.label_mode.Margin = new System.Windows.Forms.Padding(1);
			this.label_mode.Name = "label_mode";
			this.label_mode.Size = new System.Drawing.Size(143, 29);
			this.label_mode.TabIndex = 107;
			this.label_mode.Text = "Mode: TUNNEL";
			this.label_mode.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.label_mode.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_builder_mode_MouseDown);
			// 
			// button_connection
			// 
			this.button_connection.Location = new System.Drawing.Point(1, 708);
			this.button_connection.Margin = new System.Windows.Forms.Padding(1);
			this.button_connection.Name = "button_connection";
			this.button_connection.Size = new System.Drawing.Size(145, 31);
			this.button_connection.TabIndex = 115;
			this.button_connection.Text = "Create Connection";
			this.button_connection.UseVisualStyleBackColor = true;
			this.button_connection.Click += new System.EventHandler(this.button_connection_Click);
			// 
			// slider_handle_selected
			// 
			this.slider_handle_selected.Location = new System.Drawing.Point(2, 645);
			this.slider_handle_selected.Margin = new System.Windows.Forms.Padding(1);
			this.slider_handle_selected.Name = "slider_handle_selected";
			this.slider_handle_selected.RightMouseMultiplier = 4;
			this.slider_handle_selected.Size = new System.Drawing.Size(144, 19);
			this.slider_handle_selected.SlideText = "Sel. Handle Length";
			this.slider_handle_selected.SlideTol = 10;
			this.slider_handle_selected.TabIndex = 117;
			this.slider_handle_selected.ToolTop = "Strength of curve at the selected side";
			this.slider_handle_selected.ValueText = "10";
			this.slider_handle_selected.Feedback += new System.EventHandler<SliderLabelArgs>(this.slider_handle_selected_Feedback);
			// 
			// slider_twists
			// 
			this.slider_twists.Location = new System.Drawing.Point(1, 687);
			this.slider_twists.Margin = new System.Windows.Forms.Padding(1);
			this.slider_twists.Name = "slider_twists";
			this.slider_twists.RightMouseMultiplier = 4;
			this.slider_twists.Size = new System.Drawing.Size(144, 19);
			this.slider_twists.SlideText = "Quarter Twists";
			this.slider_twists.SlideTol = 10;
			this.slider_twists.TabIndex = 116;
			this.slider_twists.ToolTop = "90 twists in the tunnel (if 0 is not correct, try -4)";
			this.slider_twists.ValueText = "0";
			this.slider_twists.Feedback += new System.EventHandler<SliderLabelArgs>(this.slider_twists_Feedback);
			// 
			// slider_handle_marked
			// 
			this.slider_handle_marked.Location = new System.Drawing.Point(2, 666);
			this.slider_handle_marked.Margin = new System.Windows.Forms.Padding(1);
			this.slider_handle_marked.Name = "slider_handle_marked";
			this.slider_handle_marked.RightMouseMultiplier = 4;
			this.slider_handle_marked.Size = new System.Drawing.Size(144, 19);
			this.slider_handle_marked.SlideText = "Mrk. Handle Length";
			this.slider_handle_marked.SlideTol = 10;
			this.slider_handle_marked.TabIndex = 118;
			this.slider_handle_marked.ToolTop = "Strenth of the curve at the marked side";
			this.slider_handle_marked.ValueText = "10";
			this.slider_handle_marked.Feedback += new System.EventHandler<SliderLabelArgs>(this.slider_handle_marked_Feedback);
			// 
			// slider_connect_seg_size
			// 
			this.slider_connect_seg_size.Location = new System.Drawing.Point(2, 624);
			this.slider_connect_seg_size.Margin = new System.Windows.Forms.Padding(1);
			this.slider_connect_seg_size.Name = "slider_connect_seg_size";
			this.slider_connect_seg_size.RightMouseMultiplier = 4;
			this.slider_connect_seg_size.Size = new System.Drawing.Size(144, 19);
			this.slider_connect_seg_size.SlideText = "Segment Size";
			this.slider_connect_seg_size.SlideTol = 10;
			this.slider_connect_seg_size.TabIndex = 122;
			this.slider_connect_seg_size.ToolTop = "(Rough) Extrusion length per segment of a connection";
			this.slider_connect_seg_size.ValueText = "5.0";
			this.slider_connect_seg_size.Feedback += new System.EventHandler<SliderLabelArgs>(this.slider_connect_seg_size_Feedback);
			// 
			// button_reset
			// 
			this.button_reset.Location = new System.Drawing.Point(1, 600);
			this.button_reset.Margin = new System.Windows.Forms.Padding(1);
			this.button_reset.Name = "button_reset";
			this.button_reset.Size = new System.Drawing.Size(145, 22);
			this.button_reset.TabIndex = 123;
			this.button_reset.Text = "Reset Settings";
			this.button_reset.UseVisualStyleBackColor = true;
			this.button_reset.Click += new System.EventHandler(this.button_reset_Click);
			// 
			// TunnelBuilder
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoScroll = true;
			this.ClientSize = new System.Drawing.Size(147, 742);
			this.Controls.Add(this.button_reset);
			this.Controls.Add(this.label_arc_direction);
			this.Controls.Add(this.slider_connect_seg_size);
			this.Controls.Add(this.slider_noise_smoothing);
			this.Controls.Add(this.button_connection);
			this.Controls.Add(this.button_create);
			this.Controls.Add(this.slider_handle_marked);
			this.Controls.Add(this.slider_noise_z);
			this.Controls.Add(this.label_mode);
			this.Controls.Add(this.slider_noise_y);
			this.Controls.Add(this.slider_twists);
			this.Controls.Add(this.slider_noise_x);
			this.Controls.Add(this.slider_twist_smooth);
			this.Controls.Add(this.slider_handle_selected);
			this.Controls.Add(this.slider_twist_linear);
			this.Controls.Add(this.slider_twist_mid);
			this.Controls.Add(this.slider_segments);
			this.Controls.Add(this.label_scale_type);
			this.Controls.Add(this.slider_seg_length);
			this.Controls.Add(this.slider_scale_ey);
			this.Controls.Add(this.slider_arc_segments);
			this.Controls.Add(this.slider_scale_ex);
			this.Controls.Add(this.slider_slope_x);
			this.Controls.Add(this.slider_wave_size_y);
			this.Controls.Add(this.slider_slope_y);
			this.Controls.Add(this.slider_wave_size_x);
			this.Controls.Add(this.slider_waves_x);
			this.Controls.Add(this.slider_radius_z);
			this.Controls.Add(this.slider_waves_y);
			this.Controls.Add(this.slider_radius_x);
			this.Controls.Add(this.slider_smooth_slope_x);
			this.Controls.Add(this.slider_smooth_slope_y);
			this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)(((((WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.DockLeft) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockRight) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockTop) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockBottom)));
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.HideOnClose = true;
			this.Margin = new System.Windows.Forms.Padding(4);
			this.MaximumSize = new System.Drawing.Size(400, 930);
			this.MinimumSize = new System.Drawing.Size(163, 34);
			this.Name = "TunnelBuilder";
			this.Text = "TUNNEL BUILDER";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TunnelBuilder_FormClosing);
			this.Load += new System.EventHandler(this.TunnelBuilder_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private OverloadLevelEditor.SliderLabel slider_seg_length;
		private OverloadLevelEditor.SliderLabel slider_segments;
		private OverloadLevelEditor.SliderLabel slider_slope_x;
		private OverloadLevelEditor.SliderLabel slider_smooth_slope_y;
		private OverloadLevelEditor.SliderLabel slider_smooth_slope_x;
		private OverloadLevelEditor.SliderLabel slider_waves_y;
		private OverloadLevelEditor.SliderLabel slider_waves_x;
		private OverloadLevelEditor.SliderLabel slider_slope_y;
		private OverloadLevelEditor.SliderLabel slider_arc_segments;
		private OverloadLevelEditor.SliderLabel slider_radius_z;
		private OverloadLevelEditor.SliderLabel slider_radius_x;
		private OverloadLevelEditor.SliderLabel slider_scale_ey;
		private OverloadLevelEditor.SliderLabel slider_scale_ex;
		private OverloadLevelEditor.SliderLabel slider_wave_size_y;
		private OverloadLevelEditor.SliderLabel slider_wave_size_x;
		private System.Windows.Forms.Label label_mode;
		private System.Windows.Forms.Button button_create;
		private System.Windows.Forms.Label label_scale_type;
		private System.Windows.Forms.Button button_connection;
		private OverloadLevelEditor.SliderLabel slider_twists;
		private OverloadLevelEditor.SliderLabel slider_handle_selected;
		private OverloadLevelEditor.SliderLabel slider_twist_smooth;
		private OverloadLevelEditor.SliderLabel slider_twist_linear;
		private OverloadLevelEditor.SliderLabel slider_twist_mid;
		private OverloadLevelEditor.SliderLabel slider_noise_z;
		private OverloadLevelEditor.SliderLabel slider_noise_y;
		private OverloadLevelEditor.SliderLabel slider_noise_x;
		private OverloadLevelEditor.SliderLabel slider_noise_smoothing;
		private OverloadLevelEditor.SliderLabel slider_handle_marked;
		private OverloadLevelEditor.SliderLabel slider_connect_seg_size;
		private System.Windows.Forms.Label label_arc_direction;
		private System.Windows.Forms.Button button_reset;
	}
}