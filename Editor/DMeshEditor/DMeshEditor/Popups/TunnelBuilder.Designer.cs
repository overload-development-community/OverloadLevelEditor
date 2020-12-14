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
			this.panel_tunnel = new System.Windows.Forms.Panel();
			this.slider_noise_smoothing = new SliderLabel();
			this.slider_noise_z = new SliderLabel();
			this.slider_noise_y = new SliderLabel();
			this.slider_noise_x = new SliderLabel();
			this.slider_twist_smooth = new SliderLabel();
			this.slider_twist_linear = new SliderLabel();
			this.slider_twist_mid = new SliderLabel();
			this.label_scale_type = new System.Windows.Forms.Label();
			this.slider_scale_ey = new SliderLabel();
			this.slider_scale_ex = new SliderLabel();
			this.slider_scale_sy = new SliderLabel();
			this.slider_scale_sx = new SliderLabel();
			this.slider_wave_size_y = new SliderLabel();
			this.slider_wave_size_x = new SliderLabel();
			this.slider_radius_z = new SliderLabel();
			this.slider_radius_x = new SliderLabel();
			this.slider_smooth_slope_y = new SliderLabel();
			this.slider_smooth_slope_x = new SliderLabel();
			this.slider_waves_y = new SliderLabel();
			this.slider_waves_x = new SliderLabel();
			this.slider_slope_y = new SliderLabel();
			this.slider_slope_x = new SliderLabel();
			this.label_properties = new System.Windows.Forms.Label();
			this.slider_arc_segments = new SliderLabel();
			this.slider_seg_length = new SliderLabel();
			this.slider_segments = new SliderLabel();
			this.label_mode = new System.Windows.Forms.Label();
			this.button_create = new System.Windows.Forms.Button();
			this.button_connection = new System.Windows.Forms.Button();
			this.slider_handle_selected = new SliderLabel();
			this.slider_twists = new SliderLabel();
			this.slider_handle_marked = new SliderLabel();
			this.slider_connect_seg_size = new SliderLabel();
			this.panel1 = new System.Windows.Forms.Panel();
			this.panel_tunnel.SuspendLayout();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel_tunnel
			// 
			this.panel_tunnel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel_tunnel.Controls.Add(this.slider_noise_smoothing);
			this.panel_tunnel.Controls.Add(this.button_create);
			this.panel_tunnel.Controls.Add(this.slider_noise_z);
			this.panel_tunnel.Controls.Add(this.slider_noise_y);
			this.panel_tunnel.Controls.Add(this.slider_noise_x);
			this.panel_tunnel.Controls.Add(this.slider_twist_smooth);
			this.panel_tunnel.Controls.Add(this.slider_twist_linear);
			this.panel_tunnel.Controls.Add(this.slider_twist_mid);
			this.panel_tunnel.Controls.Add(this.label_scale_type);
			this.panel_tunnel.Controls.Add(this.slider_scale_ey);
			this.panel_tunnel.Controls.Add(this.slider_scale_ex);
			this.panel_tunnel.Controls.Add(this.slider_scale_sy);
			this.panel_tunnel.Controls.Add(this.slider_scale_sx);
			this.panel_tunnel.Controls.Add(this.slider_wave_size_y);
			this.panel_tunnel.Controls.Add(this.slider_wave_size_x);
			this.panel_tunnel.Controls.Add(this.slider_radius_z);
			this.panel_tunnel.Controls.Add(this.slider_radius_x);
			this.panel_tunnel.Controls.Add(this.slider_smooth_slope_y);
			this.panel_tunnel.Controls.Add(this.slider_smooth_slope_x);
			this.panel_tunnel.Controls.Add(this.slider_waves_y);
			this.panel_tunnel.Controls.Add(this.slider_waves_x);
			this.panel_tunnel.Controls.Add(this.slider_slope_y);
			this.panel_tunnel.Controls.Add(this.slider_slope_x);
			this.panel_tunnel.Controls.Add(this.label_properties);
			this.panel_tunnel.Controls.Add(this.slider_arc_segments);
			this.panel_tunnel.Controls.Add(this.slider_seg_length);
			this.panel_tunnel.Controls.Add(this.slider_segments);
			this.panel_tunnel.Location = new System.Drawing.Point(0, 37);
			this.panel_tunnel.Margin = new System.Windows.Forms.Padding(1);
			this.panel_tunnel.Name = "panel_tunnel";
			this.panel_tunnel.Size = new System.Drawing.Size(149, 556);
			this.panel_tunnel.TabIndex = 92;
			// 
			// slider_noise_smoothing
			// 
			this.slider_noise_smoothing.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.slider_noise_smoothing.Location = new System.Drawing.Point(2, 502);
			this.slider_noise_smoothing.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.slider_noise_smoothing.Name = "slider_noise_smoothing";
			this.slider_noise_smoothing.RightMouseMultiplier = 5;
			this.slider_noise_smoothing.Size = new System.Drawing.Size(143, 19);
			this.slider_noise_smoothing.SlideText = "Smoothing Passes";
			this.slider_noise_smoothing.SlideTol = 10;
			this.slider_noise_smoothing.TabIndex = 121;
			this.slider_noise_smoothing.ToolTop = "How many times to smooth the generate randomness";
			this.slider_noise_smoothing.ValueText = "1";
			this.slider_noise_smoothing.Feedback += new System.EventHandler<SliderLabelArgs>(this.slider_noise_smoothing_Feedback);
			// 
			// slider_noise_z
			// 
			this.slider_noise_z.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.slider_noise_z.Location = new System.Drawing.Point(2, 483);
			this.slider_noise_z.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.slider_noise_z.Name = "slider_noise_z";
			this.slider_noise_z.RightMouseMultiplier = 2;
			this.slider_noise_z.Size = new System.Drawing.Size(143, 19);
			this.slider_noise_z.SlideText = "Noise Z";
			this.slider_noise_z.SlideTol = 10;
			this.slider_noise_z.TabIndex = 120;
			this.slider_noise_z.ToolTop = "Randomness in the Z axis";
			this.slider_noise_z.ValueText = "0.0";
			this.slider_noise_z.Feedback += new System.EventHandler<SliderLabelArgs>(this.slider_noise_z_Feedback);
			// 
			// slider_noise_y
			// 
			this.slider_noise_y.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.slider_noise_y.Location = new System.Drawing.Point(2, 464);
			this.slider_noise_y.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.slider_noise_y.Name = "slider_noise_y";
			this.slider_noise_y.RightMouseMultiplier = 2;
			this.slider_noise_y.Size = new System.Drawing.Size(143, 19);
			this.slider_noise_y.SlideText = "Noise Y";
			this.slider_noise_y.SlideTol = 10;
			this.slider_noise_y.TabIndex = 119;
			this.slider_noise_y.ToolTop = "Randomness in the Y axis";
			this.slider_noise_y.ValueText = "0.0";
			this.slider_noise_y.Feedback += new System.EventHandler<SliderLabelArgs>(this.slider_noise_y_Feedback);
			// 
			// slider_noise_x
			// 
			this.slider_noise_x.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.slider_noise_x.Location = new System.Drawing.Point(2, 445);
			this.slider_noise_x.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.slider_noise_x.Name = "slider_noise_x";
			this.slider_noise_x.RightMouseMultiplier = 2;
			this.slider_noise_x.Size = new System.Drawing.Size(143, 19);
			this.slider_noise_x.SlideText = "Noise X";
			this.slider_noise_x.SlideTol = 10;
			this.slider_noise_x.TabIndex = 118;
			this.slider_noise_x.ToolTop = "Randomness in the X axis";
			this.slider_noise_x.ValueText = "0.0";
			this.slider_noise_x.Feedback += new System.EventHandler<SliderLabelArgs>(this.slider_noise_x_Feedback);
			// 
			// slider_twist_smooth
			// 
			this.slider_twist_smooth.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.slider_twist_smooth.Location = new System.Drawing.Point(2, 403);
			this.slider_twist_smooth.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.slider_twist_smooth.Name = "slider_twist_smooth";
			this.slider_twist_smooth.RightMouseMultiplier = 9;
			this.slider_twist_smooth.Size = new System.Drawing.Size(143, 19);
			this.slider_twist_smooth.SlideText = "Smooth End Twist";
			this.slider_twist_smooth.SlideTol = 10;
			this.slider_twist_smooth.TabIndex = 117;
			this.slider_twist_smooth.ToolTop = "Smoothed twisting amount";
			this.slider_twist_smooth.ValueText = "0";
			this.slider_twist_smooth.Feedback += new System.EventHandler<SliderLabelArgs>(this.slider_twist_smooth_Feedback);
			// 
			// slider_twist_linear
			// 
			this.slider_twist_linear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.slider_twist_linear.Location = new System.Drawing.Point(2, 422);
			this.slider_twist_linear.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.slider_twist_linear.Name = "slider_twist_linear";
			this.slider_twist_linear.RightMouseMultiplier = 9;
			this.slider_twist_linear.Size = new System.Drawing.Size(143, 19);
			this.slider_twist_linear.SlideText = "Linear End Twist";
			this.slider_twist_linear.SlideTol = 10;
			this.slider_twist_linear.TabIndex = 116;
			this.slider_twist_linear.ToolTop = "Linear twisting amount";
			this.slider_twist_linear.ValueText = "0";
			this.slider_twist_linear.Feedback += new System.EventHandler<SliderLabelArgs>(this.slider_twist_linear_Feedback);
			// 
			// slider_twist_mid
			// 
			this.slider_twist_mid.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.slider_twist_mid.Location = new System.Drawing.Point(2, 384);
			this.slider_twist_mid.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.slider_twist_mid.Name = "slider_twist_mid";
			this.slider_twist_mid.RightMouseMultiplier = 9;
			this.slider_twist_mid.Size = new System.Drawing.Size(143, 19);
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
			this.label_scale_type.Location = new System.Drawing.Point(2, 361);
			this.label_scale_type.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label_scale_type.Name = "label_scale_type";
			this.label_scale_type.Size = new System.Drawing.Size(143, 19);
			this.label_scale_type.TabIndex = 114;
			this.label_scale_type.Text = "Scaling: LINEAR";
			this.label_scale_type.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.label_scale_type.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_scale_type_MouseDown);
			// 
			// slider_scale_ey
			// 
			this.slider_scale_ey.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.slider_scale_ey.Location = new System.Drawing.Point(2, 342);
			this.slider_scale_ey.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.slider_scale_ey.Name = "slider_scale_ey";
			this.slider_scale_ey.RightMouseMultiplier = 4;
			this.slider_scale_ey.Size = new System.Drawing.Size(143, 19);
			this.slider_scale_ey.SlideText = "Scale End Y";
			this.slider_scale_ey.SlideTol = 10;
			this.slider_scale_ey.TabIndex = 113;
			this.slider_scale_ey.ToolTop = "Size of the segments at the end of the tunnel";
			this.slider_scale_ey.ValueText = "100";
			this.slider_scale_ey.Feedback += new System.EventHandler<SliderLabelArgs>(this.slider_scale_ey_Feedback);
			// 
			// slider_scale_ex
			// 
			this.slider_scale_ex.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.slider_scale_ex.Location = new System.Drawing.Point(2, 323);
			this.slider_scale_ex.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.slider_scale_ex.Name = "slider_scale_ex";
			this.slider_scale_ex.RightMouseMultiplier = 4;
			this.slider_scale_ex.Size = new System.Drawing.Size(143, 19);
			this.slider_scale_ex.SlideText = "Scale End X";
			this.slider_scale_ex.SlideTol = 10;
			this.slider_scale_ex.TabIndex = 112;
			this.slider_scale_ex.ToolTop = "Size of the segments at the end of the tunnel";
			this.slider_scale_ex.ValueText = "100";
			this.slider_scale_ex.Feedback += new System.EventHandler<SliderLabelArgs>(this.slider_scale_ex_Feedback);
			// 
			// slider_scale_sy
			// 
			this.slider_scale_sy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.slider_scale_sy.Enabled = false;
			this.slider_scale_sy.Location = new System.Drawing.Point(2, 304);
			this.slider_scale_sy.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.slider_scale_sy.Name = "slider_scale_sy";
			this.slider_scale_sy.RightMouseMultiplier = 4;
			this.slider_scale_sy.Size = new System.Drawing.Size(143, 19);
			this.slider_scale_sy.SlideText = "Scale Start Y";
			this.slider_scale_sy.SlideTol = 10;
			this.slider_scale_sy.TabIndex = 111;
			this.slider_scale_sy.ToolTop = "Segment size at the start (cannot change currently)";
			this.slider_scale_sy.ValueText = "100";
			this.slider_scale_sy.Feedback += new System.EventHandler<SliderLabelArgs>(this.slider_scale_sy_Feedback);
			// 
			// slider_scale_sx
			// 
			this.slider_scale_sx.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.slider_scale_sx.Enabled = false;
			this.slider_scale_sx.Location = new System.Drawing.Point(2, 285);
			this.slider_scale_sx.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.slider_scale_sx.Name = "slider_scale_sx";
			this.slider_scale_sx.RightMouseMultiplier = 4;
			this.slider_scale_sx.Size = new System.Drawing.Size(143, 19);
			this.slider_scale_sx.SlideText = "Scale Start X";
			this.slider_scale_sx.SlideTol = 10;
			this.slider_scale_sx.TabIndex = 110;
			this.slider_scale_sx.ToolTop = "Segment size at the start (cannot change currently)";
			this.slider_scale_sx.ValueText = "100";
			this.slider_scale_sx.Feedback += new System.EventHandler<SliderLabelArgs>(this.slider_scale_sx_Feedback);
			// 
			// slider_wave_size_y
			// 
			this.slider_wave_size_y.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.slider_wave_size_y.Location = new System.Drawing.Point(2, 262);
			this.slider_wave_size_y.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.slider_wave_size_y.Name = "slider_wave_size_y";
			this.slider_wave_size_y.RightMouseMultiplier = 4;
			this.slider_wave_size_y.Size = new System.Drawing.Size(143, 19);
			this.slider_wave_size_y.SlideText = "Wave Size Y";
			this.slider_wave_size_y.SlideTol = 10;
			this.slider_wave_size_y.TabIndex = 109;
			this.slider_wave_size_y.ToolTop = "Size of the Y waves";
			this.slider_wave_size_y.ValueText = "4";
			this.slider_wave_size_y.Feedback += new System.EventHandler<SliderLabelArgs>(this.slider_wave_size_y_Feedback);
			// 
			// slider_wave_size_x
			// 
			this.slider_wave_size_x.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.slider_wave_size_x.Location = new System.Drawing.Point(2, 243);
			this.slider_wave_size_x.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.slider_wave_size_x.Name = "slider_wave_size_x";
			this.slider_wave_size_x.RightMouseMultiplier = 4;
			this.slider_wave_size_x.Size = new System.Drawing.Size(143, 19);
			this.slider_wave_size_x.SlideText = "Wave Size X";
			this.slider_wave_size_x.SlideTol = 10;
			this.slider_wave_size_x.TabIndex = 108;
			this.slider_wave_size_x.ToolTop = "Size of the Y waves";
			this.slider_wave_size_x.ValueText = "4";
			this.slider_wave_size_x.Feedback += new System.EventHandler<SliderLabelArgs>(this.slider_wave_size_x_Feedback);
			// 
			// slider_radius_z
			// 
			this.slider_radius_z.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.slider_radius_z.Location = new System.Drawing.Point(2, 98);
			this.slider_radius_z.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.slider_radius_z.Name = "slider_radius_z";
			this.slider_radius_z.RightMouseMultiplier = 5;
			this.slider_radius_z.Size = new System.Drawing.Size(143, 19);
			this.slider_radius_z.SlideText = "Radius Z";
			this.slider_radius_z.SlideTol = 10;
			this.slider_radius_z.TabIndex = 107;
			this.slider_radius_z.ToolTop = "";
			this.slider_radius_z.ValueText = "20";
			this.slider_radius_z.Feedback += new System.EventHandler<SliderLabelArgs>(this.slider_radius_z_Feedback);
			// 
			// slider_radius_x
			// 
			this.slider_radius_x.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.slider_radius_x.Location = new System.Drawing.Point(2, 79);
			this.slider_radius_x.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.slider_radius_x.Name = "slider_radius_x";
			this.slider_radius_x.RightMouseMultiplier = 5;
			this.slider_radius_x.Size = new System.Drawing.Size(143, 19);
			this.slider_radius_x.SlideText = "Radius X";
			this.slider_radius_x.SlideTol = 10;
			this.slider_radius_x.TabIndex = 106;
			this.slider_radius_x.ToolTop = "";
			this.slider_radius_x.ValueText = "20";
			this.slider_radius_x.Feedback += new System.EventHandler<SliderLabelArgs>(this.slider_radius_x_Feedback);
			// 
			// slider_smooth_slope_y
			// 
			this.slider_smooth_slope_y.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.slider_smooth_slope_y.Location = new System.Drawing.Point(2, 182);
			this.slider_smooth_slope_y.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.slider_smooth_slope_y.Name = "slider_smooth_slope_y";
			this.slider_smooth_slope_y.RightMouseMultiplier = 4;
			this.slider_smooth_slope_y.Size = new System.Drawing.Size(143, 19);
			this.slider_smooth_slope_y.SlideText = "Smooth Slope Y";
			this.slider_smooth_slope_y.SlideTol = 10;
			this.slider_smooth_slope_y.TabIndex = 105;
			this.slider_smooth_slope_y.ToolTop = "";
			this.slider_smooth_slope_y.ValueText = "0";
			this.slider_smooth_slope_y.Feedback += new System.EventHandler<SliderLabelArgs>(this.slider_smooth_slope_y_Feedback);
			// 
			// slider_smooth_slope_x
			// 
			this.slider_smooth_slope_x.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.slider_smooth_slope_x.Location = new System.Drawing.Point(2, 163);
			this.slider_smooth_slope_x.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.slider_smooth_slope_x.Name = "slider_smooth_slope_x";
			this.slider_smooth_slope_x.RightMouseMultiplier = 4;
			this.slider_smooth_slope_x.Size = new System.Drawing.Size(143, 19);
			this.slider_smooth_slope_x.SlideText = "Smooth Slope X";
			this.slider_smooth_slope_x.SlideTol = 10;
			this.slider_smooth_slope_x.TabIndex = 104;
			this.slider_smooth_slope_x.ToolTop = "";
			this.slider_smooth_slope_x.ValueText = "0";
			this.slider_smooth_slope_x.Feedback += new System.EventHandler<SliderLabelArgs>(this.slider_smooth_slope_x_Feedback);
			// 
			// slider_waves_y
			// 
			this.slider_waves_y.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.slider_waves_y.Location = new System.Drawing.Point(2, 224);
			this.slider_waves_y.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.slider_waves_y.Name = "slider_waves_y";
			this.slider_waves_y.RightMouseMultiplier = 4;
			this.slider_waves_y.Size = new System.Drawing.Size(143, 19);
			this.slider_waves_y.SlideText = "Waves Y";
			this.slider_waves_y.SlideTol = 10;
			this.slider_waves_y.TabIndex = 103;
			this.slider_waves_y.ToolTop = "Number of 1/2 waves in the Y direction";
			this.slider_waves_y.ValueText = "0";
			this.slider_waves_y.Feedback += new System.EventHandler<SliderLabelArgs>(this.slider_waves_y_Feedback);
			// 
			// slider_waves_x
			// 
			this.slider_waves_x.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.slider_waves_x.Location = new System.Drawing.Point(2, 205);
			this.slider_waves_x.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.slider_waves_x.Name = "slider_waves_x";
			this.slider_waves_x.RightMouseMultiplier = 4;
			this.slider_waves_x.Size = new System.Drawing.Size(143, 19);
			this.slider_waves_x.SlideText = "Waves X";
			this.slider_waves_x.SlideTol = 10;
			this.slider_waves_x.TabIndex = 102;
			this.slider_waves_x.ToolTop = "Number of 1/2 waves in the X direction";
			this.slider_waves_x.ValueText = "0";
			this.slider_waves_x.Feedback += new System.EventHandler<SliderLabelArgs>(this.slider_waves_x_Feedback);
			// 
			// slider_slope_y
			// 
			this.slider_slope_y.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.slider_slope_y.Location = new System.Drawing.Point(2, 140);
			this.slider_slope_y.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.slider_slope_y.Name = "slider_slope_y";
			this.slider_slope_y.RightMouseMultiplier = 4;
			this.slider_slope_y.Size = new System.Drawing.Size(143, 19);
			this.slider_slope_y.SlideText = "Linear Slope Y";
			this.slider_slope_y.SlideTol = 10;
			this.slider_slope_y.TabIndex = 101;
			this.slider_slope_y.ToolTop = "";
			this.slider_slope_y.ValueText = "0";
			this.slider_slope_y.Feedback += new System.EventHandler<SliderLabelArgs>(this.slider_slope_y_Feedback);
			// 
			// slider_slope_x
			// 
			this.slider_slope_x.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.slider_slope_x.Location = new System.Drawing.Point(2, 121);
			this.slider_slope_x.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.slider_slope_x.Name = "slider_slope_x";
			this.slider_slope_x.RightMouseMultiplier = 4;
			this.slider_slope_x.Size = new System.Drawing.Size(143, 19);
			this.slider_slope_x.SlideText = "Linear Slope X";
			this.slider_slope_x.SlideTol = 10;
			this.slider_slope_x.TabIndex = 100;
			this.slider_slope_x.ToolTop = "";
			this.slider_slope_x.ValueText = "0";
			this.slider_slope_x.Feedback += new System.EventHandler<SliderLabelArgs>(this.slider_slope_x_Feedback);
			// 
			// label_properties
			// 
			this.label_properties.BackColor = System.Drawing.SystemColors.Control;
			this.label_properties.Location = new System.Drawing.Point(2, -1);
			this.label_properties.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label_properties.Name = "label_properties";
			this.label_properties.Size = new System.Drawing.Size(143, 19);
			this.label_properties.TabIndex = 45;
			this.label_properties.Text = "PROPERTIES";
			this.label_properties.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// slider_arc_segments
			// 
			this.slider_arc_segments.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.slider_arc_segments.Location = new System.Drawing.Point(2, 56);
			this.slider_arc_segments.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.slider_arc_segments.Name = "slider_arc_segments";
			this.slider_arc_segments.RightMouseMultiplier = 5;
			this.slider_arc_segments.Size = new System.Drawing.Size(143, 19);
			this.slider_arc_segments.SlideText = "Per Full Arc";
			this.slider_arc_segments.SlideTol = 10;
			this.slider_arc_segments.TabIndex = 99;
			this.slider_arc_segments.ToolTop = "Segments in a full arc (can be more or less than Segments)";
			this.slider_arc_segments.ValueText = "10";
			this.slider_arc_segments.Feedback += new System.EventHandler<SliderLabelArgs>(this.slider_arc_segments_Feedback);
			// 
			// slider_seg_length
			// 
			this.slider_seg_length.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.slider_seg_length.Location = new System.Drawing.Point(2, 37);
			this.slider_seg_length.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.slider_seg_length.Name = "slider_seg_length";
			this.slider_seg_length.RightMouseMultiplier = 2;
			this.slider_seg_length.Size = new System.Drawing.Size(143, 19);
			this.slider_seg_length.SlideText = "Segment Length";
			this.slider_seg_length.SlideTol = 10;
			this.slider_seg_length.TabIndex = 99;
			this.slider_seg_length.ToolTop = "Extrusion length per segment";
			this.slider_seg_length.ValueText = "4";
			this.slider_seg_length.Feedback += new System.EventHandler<SliderLabelArgs>(this.slider_seg_length_Feedback);
			// 
			// slider_segments
			// 
			this.slider_segments.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.slider_segments.Location = new System.Drawing.Point(2, 18);
			this.slider_segments.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.slider_segments.Name = "slider_segments";
			this.slider_segments.RightMouseMultiplier = 5;
			this.slider_segments.Size = new System.Drawing.Size(143, 19);
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
			this.label_mode.Location = new System.Drawing.Point(0, 0);
			this.label_mode.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label_mode.Name = "label_mode";
			this.label_mode.Size = new System.Drawing.Size(149, 37);
			this.label_mode.TabIndex = 107;
			this.label_mode.Text = "Mode: TUNNEL";
			this.label_mode.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.label_mode.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_builder_mode_MouseDown);
			// 
			// button_create
			// 
			this.button_create.Location = new System.Drawing.Point(3, 532);
			this.button_create.Margin = new System.Windows.Forms.Padding(1);
			this.button_create.Name = "button_create";
			this.button_create.Size = new System.Drawing.Size(143, 21);
			this.button_create.TabIndex = 114;
			this.button_create.Text = "Create Tunnel";
			this.button_create.UseVisualStyleBackColor = true;
			this.button_create.Click += new System.EventHandler(this.button_create_Click);
			// 
			// button_connection
			// 
			this.button_connection.Location = new System.Drawing.Point(2, 94);
			this.button_connection.Margin = new System.Windows.Forms.Padding(1);
			this.button_connection.Name = "button_connection";
			this.button_connection.Size = new System.Drawing.Size(143, 21);
			this.button_connection.TabIndex = 115;
			this.button_connection.Text = "Create Connection";
			this.button_connection.UseVisualStyleBackColor = true;
			this.button_connection.Click += new System.EventHandler(this.button_connection_Click);
			// 
			// slider_handle_selected
			// 
			this.slider_handle_selected.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.slider_handle_selected.Location = new System.Drawing.Point(2, 27);
			this.slider_handle_selected.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.slider_handle_selected.Name = "slider_handle_selected";
			this.slider_handle_selected.RightMouseMultiplier = 4;
			this.slider_handle_selected.Size = new System.Drawing.Size(143, 19);
			this.slider_handle_selected.SlideText = "Sel. Handle Length";
			this.slider_handle_selected.SlideTol = 10;
			this.slider_handle_selected.TabIndex = 117;
			this.slider_handle_selected.ToolTop = "";
			this.slider_handle_selected.ValueText = "20";
			this.slider_handle_selected.Feedback += new System.EventHandler<SliderLabelArgs>(this.slider_handle_selected_Feedback);
			// 
			// slider_twists
			// 
			this.slider_twists.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.slider_twists.Location = new System.Drawing.Point(2, 65);
			this.slider_twists.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.slider_twists.Name = "slider_twists";
			this.slider_twists.RightMouseMultiplier = 4;
			this.slider_twists.Size = new System.Drawing.Size(143, 19);
			this.slider_twists.SlideText = "Quarter Twists";
			this.slider_twists.SlideTol = 10;
			this.slider_twists.TabIndex = 116;
			this.slider_twists.ToolTop = "";
			this.slider_twists.ValueText = "0";
			this.slider_twists.Feedback += new System.EventHandler<SliderLabelArgs>(this.slider_twists_Feedback);
			// 
			// slider_handle_marked
			// 
			this.slider_handle_marked.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.slider_handle_marked.Location = new System.Drawing.Point(2, 46);
			this.slider_handle_marked.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.slider_handle_marked.Name = "slider_handle_marked";
			this.slider_handle_marked.RightMouseMultiplier = 4;
			this.slider_handle_marked.Size = new System.Drawing.Size(143, 19);
			this.slider_handle_marked.SlideText = "Mrk. Handle Length";
			this.slider_handle_marked.SlideTol = 10;
			this.slider_handle_marked.TabIndex = 118;
			this.slider_handle_marked.ToolTop = "";
			this.slider_handle_marked.ValueText = "20";
			this.slider_handle_marked.Feedback += new System.EventHandler<SliderLabelArgs>(this.slider_handle_marked_Feedback);
			// 
			// slider_connect_seg_size
			// 
			this.slider_connect_seg_size.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.slider_connect_seg_size.Location = new System.Drawing.Point(2, 8);
			this.slider_connect_seg_size.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.slider_connect_seg_size.Name = "slider_connect_seg_size";
			this.slider_connect_seg_size.RightMouseMultiplier = 5;
			this.slider_connect_seg_size.Size = new System.Drawing.Size(143, 19);
			this.slider_connect_seg_size.SlideText = "Segment Size";
			this.slider_connect_seg_size.SlideTol = 10;
			this.slider_connect_seg_size.TabIndex = 122;
			this.slider_connect_seg_size.ToolTop = "Extrusion length per segment";
			this.slider_connect_seg_size.ValueText = "1.0";
			this.slider_connect_seg_size.Feedback += new System.EventHandler<SliderLabelArgs>(this.slider_connect_seg_size_Feedback);
			// 
			// panel1
			// 
			this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel1.Controls.Add(this.slider_connect_seg_size);
			this.panel1.Controls.Add(this.button_connection);
			this.panel1.Controls.Add(this.slider_handle_marked);
			this.panel1.Controls.Add(this.slider_twists);
			this.panel1.Controls.Add(this.slider_handle_selected);
			this.panel1.Location = new System.Drawing.Point(0, 595);
			this.panel1.Margin = new System.Windows.Forms.Padding(1);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(149, 118);
			this.panel1.TabIndex = 123;
			// 
			// TunnelBuilder
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(149, 713);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.label_mode);
			this.Controls.Add(this.panel_tunnel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "TunnelBuilder";
			this.Text = "TunnelBuilder";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TunnelBuilder_FormClosing);
			this.LocationChanged += new System.EventHandler(this.TunnelBuilder_LocationChanged);
			this.panel_tunnel.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private SliderLabel slider_seg_length;
		private SliderLabel slider_segments;
		private SliderLabel slider_slope_x;
		private System.Windows.Forms.Panel panel_tunnel;
		private System.Windows.Forms.Label label_properties;
		private SliderLabel slider_smooth_slope_y;
		private SliderLabel slider_smooth_slope_x;
		private SliderLabel slider_waves_y;
		private SliderLabel slider_waves_x;
		private SliderLabel slider_slope_y;
		private SliderLabel slider_arc_segments;
		private SliderLabel slider_radius_z;
		private SliderLabel slider_radius_x;
		private SliderLabel slider_scale_ey;
		private SliderLabel slider_scale_ex;
		private SliderLabel slider_scale_sy;
		private SliderLabel slider_scale_sx;
		private SliderLabel slider_wave_size_y;
		private SliderLabel slider_wave_size_x;
		private System.Windows.Forms.Label label_mode;
		private System.Windows.Forms.Button button_create;
		private System.Windows.Forms.Label label_scale_type;
		private System.Windows.Forms.Button button_connection;
		private SliderLabel slider_twists;
		private SliderLabel slider_handle_selected;
		private SliderLabel slider_twist_smooth;
		private SliderLabel slider_twist_linear;
		private SliderLabel slider_twist_mid;
		private SliderLabel slider_noise_z;
		private SliderLabel slider_noise_y;
		private SliderLabel slider_noise_x;
		private SliderLabel slider_noise_smoothing;
		private SliderLabel slider_handle_marked;
		private SliderLabel slider_connect_seg_size;
		private System.Windows.Forms.Panel panel1;
	}
}