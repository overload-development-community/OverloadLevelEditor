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
using System.Collections;
using OpenTK;

namespace OverloadLevelEditor
{
	abstract public class NoiseModifierBase
	{
		abstract public float ModifyNoise(float noise_value, OpenTK.Vector2 source_range);
		abstract public OpenTK.Vector2 ModifyRange(OpenTK.Vector2 source_range);
	}

	public class ScaleAndBiasNoiseModifier : NoiseModifierBase
	{
		public static float s_default_scale = 1.0f;
		public static float s_default_bias = 0.0f;

		public ScaleAndBiasNoiseModifier()
		{
			m_scale = s_default_scale;
			m_bias = s_default_bias;
		}

		public ScaleAndBiasNoiseModifier(float scale, float bias)
		{
			m_scale = scale;
			m_bias = bias;
		}

		public override float ModifyNoise(float noise_value, OpenTK.Vector2 source_range)
		{
			return (noise_value * m_scale) + m_bias;
		}

		public override OpenTK.Vector2 ModifyRange(OpenTK.Vector2 source_range)
		{
			// OpenTK.Vector2 doesn't really have any math operations defined, which is meh.
			float scale_biased_min = (source_range.X * m_scale) + m_bias;
			float scale_biased_max = (source_range.Y * m_scale) + m_bias;

			// If the scale is negative, this could flip the min and max.
			OpenTK.Vector2 modified_range;
			modified_range.X = System.Math.Min(scale_biased_min, scale_biased_max);
			modified_range.Y = System.Math.Max(scale_biased_min, scale_biased_max);

			return modified_range;
		}

		// Scale is applied BEFORE bias - out = (in * scale) + bias, NOT out = (in + bias) * scale!
		public float m_scale;
		public float m_bias;
	}

	public class AbsoluteValueNoiseModifier : NoiseModifierBase
	{
		public override float ModifyNoise(float noise_value, OpenTK.Vector2 source_range)
		{
			return System.Math.Abs(noise_value);
		}

		public override OpenTK.Vector2 ModifyRange(OpenTK.Vector2 source_range)
		{
			// Three cases to handle:
			//	 1.) min and max both positive - no change.
			//  2.) min and max both negative - make both positive and flip max and min.
			//  3.) min negative, max positive - new min = 0, new max = larger of (old max) or (-old min)
			//  
			OpenTK.Vector2 modified_range;

			if (source_range.X >= 0.0f) {
				modified_range = source_range;
			} else if (source_range.Y < 0.0f) {
				modified_range.X = -1.0f * source_range.Y;
				modified_range.Y = -1.0f * source_range.X;
			} else {
				modified_range.X = 0.0f;
				modified_range.Y = System.Math.Max(source_range.Y, -1.0f * source_range.X);
			}

			return modified_range;
		}
	}

	public class ExponentNoiseModifier : NoiseModifierBase
	{
		public static float s_default_exponent = 1.0f;

		public ExponentNoiseModifier()
		{
			m_exponent = s_default_exponent;
		}

		public ExponentNoiseModifier(float exponent)
		{
			m_exponent = exponent;
		}

		public override float ModifyNoise(float noise_value, OpenTK.Vector2 source_range)
		{
			return System.Convert.ToSingle(System.Math.Pow(noise_value, m_exponent));
		}

		public override OpenTK.Vector2 ModifyRange(OpenTK.Vector2 source_range)
		{
			float scale_biased_min = System.Convert.ToSingle(System.Math.Pow(source_range.X, m_exponent));
			float scale_biased_max = System.Convert.ToSingle(System.Math.Pow(source_range.Y, m_exponent));

			// If the exponent is negative, this could flip the min and max.
			OpenTK.Vector2 modified_range;
			modified_range.X = System.Math.Min(scale_biased_min, scale_biased_max);
			modified_range.Y = System.Math.Max(scale_biased_min, scale_biased_max);

			return modified_range;
		}

		public float m_exponent;
	}

	public class RangeRemapNoiseModifier : NoiseModifierBase
	{
		public static float s_default_range_min = 0.0f;
		public static float s_default_range_max = 1.0f;

		public RangeRemapNoiseModifier()
		{
			m_remap_range_min = s_default_range_min;
			m_remap_range_max = s_default_range_max;
		}

		public RangeRemapNoiseModifier(float remap_range_min, float remap_range_max)
		{
			m_remap_range_min = remap_range_min;
			m_remap_range_max = remap_range_max;
		}

		public override float ModifyNoise(float noise_value, OpenTK.Vector2 source_range)
		{
			return MathUtils.Remap(noise_value, source_range.X, source_range.Y, m_remap_range_min, m_remap_range_max);
		}

		public override OpenTK.Vector2 ModifyRange(OpenTK.Vector2 source_range)
		{
			float remapped_min = MathUtils.Remap(source_range.X, source_range.X, source_range.Y, m_remap_range_min, m_remap_range_max);
			float remapped_max = MathUtils.Remap(source_range.Y, source_range.X, source_range.Y, m_remap_range_min, m_remap_range_max);
			
			// A remap is just a glorified scale and bias - if you pass in a remap_range_max that is actually SMALLER than remap_range_min,
			//  it will flip the output around.  Just in case someone tries to do that, and you end up with max < min, catch that and flip them.
			OpenTK.Vector2 modified_range;
			modified_range.X = System.Math.Min(remapped_min, remapped_max);
			modified_range.Y = System.Math.Max(remapped_min, remapped_max);

			return modified_range;
		}

		public float m_remap_range_min;
		public float m_remap_range_max;
	}

	public class RangeClampNoiseModifier : NoiseModifierBase
	{
		public float s_default_range_min = 0.0f;
		public float s_default_range_max = 1.0f;

		public RangeClampNoiseModifier()
		{
			m_clamp_range_min = s_default_range_min;
			m_clamp_range_max = s_default_range_max;
		}

		public RangeClampNoiseModifier(float clamp_range_min, float clamp_range_max)
		{
			m_clamp_range_min = clamp_range_min;
			m_clamp_range_max = clamp_range_max;
		}

		public override float ModifyNoise(float noise_value, OpenTK.Vector2 source_range)
		{
			float clamped_noise_value;

			if (noise_value < m_clamp_range_min) {
				clamped_noise_value = m_clamp_range_min;
			} else if (noise_value > m_clamp_range_max) {
				clamped_noise_value = m_clamp_range_max;
			} else {
				clamped_noise_value = noise_value;
			}

			return clamped_noise_value;
		}

		public override OpenTK.Vector2 ModifyRange(OpenTK.Vector2 source_range)
		{
			OpenTK.Vector2 modified_range;
			modified_range.X = System.Math.Max(source_range.X, m_clamp_range_min);
			modified_range.Y = System.Math.Min(source_range.Y, m_clamp_range_max);

			// If the source range and the clamp range do not overlap, then the modified range
			//  will appear inverted (max < min).  In this case, everything will clamp to one
			//  end of the range.
			if (modified_range.Y < modified_range.X) {
				if (modified_range.X == source_range.X) {
					modified_range.Y = modified_range.X;
				} else {
					modified_range.X = modified_range.Y;
				}
			}

			return modified_range;
		}

		public float m_clamp_range_min;
		public float m_clamp_range_max;
	}

	abstract public class NoiseModuleBase
	{
		public NoiseModuleBase()
		{
			m_axial_frequencies.X = m_axial_frequencies.Y = m_axial_frequencies.Z = 1.0f;
			m_modifiers = new System.Collections.Generic.List<NoiseModifierBase>();
			m_anti_vertical_bias = false;
			m_positive_vertical_bias = false;
			m_negative_vertical_bias = false;
		}

		public NoiseModuleBase(OpenTK.Vector3 axial_frequencies, bool anti_y = false, bool pos_y = false, bool neg_y = false)
		{
			m_axial_frequencies = axial_frequencies;
			m_modifiers = new System.Collections.Generic.List<NoiseModifierBase>();
			m_anti_vertical_bias = anti_y;
			m_positive_vertical_bias = pos_y;
			m_negative_vertical_bias = neg_y;
		}

		abstract public float EvaluateNoise(OpenTK.Vector3 coordinates, OpenTK.Vector3 deform_direction);
		abstract public OpenTK.Vector2 GetNoiseRange();

		public void AppendNoiseModifier(NoiseModifierBase modifier)
		{
			m_modifiers.Add(modifier);
		}

		public void ClearNoiseModifiers()
		{
			m_modifiers.Clear();
		}

		public float GetDirectionalStrength(Vector3 deform_direction)
		{
			float strength = 1f;
			if (m_anti_vertical_bias) {
				strength *= (1f - (deform_direction.Y * deform_direction.Y));
			} else if (m_positive_vertical_bias && m_negative_vertical_bias) {
				strength *= Math.Abs(deform_direction.Y);
			} else if (m_positive_vertical_bias) {
				if (deform_direction.Y < 0f) {
					strength *= 0f;
				} else {
					strength *= (deform_direction.Y);
				}
			} else if (m_negative_vertical_bias) {
				if (deform_direction.Y < 0f) {
					strength *= (-deform_direction.Y);
				} else {
					strength *= 0f;
				}
			}

			return strength;
		}

		public bool m_anti_vertical_bias;
		public bool m_positive_vertical_bias;
		public bool m_negative_vertical_bias;
		public OpenTK.Vector3 m_axial_frequencies;
		public System.Collections.Generic.List<NoiseModifierBase> m_modifiers;
	}

	abstract public class NoiseModuleFractalBase : NoiseModuleBase
	{
		public static uint s_default_num_octaves = 1;
		public static float s_default_fractal_increment = 1.0f;
		public static float s_default_lacunarity = 2.0f;

		public NoiseModuleFractalBase() : base()
		{
			m_num_octaves = s_default_num_octaves;
			m_fractal_increment = s_default_fractal_increment;
			m_lacunarity = s_default_lacunarity;
			m_basis_noise_module = null;
		}

		public NoiseModuleFractalBase(OpenTK.Vector3 axial_frequencies, bool anti_y = false, bool pos_y = false, bool neg_y = false) : base(axial_frequencies, anti_y, pos_y, neg_y)
		{
			m_num_octaves = s_default_num_octaves;
			m_fractal_increment = s_default_fractal_increment;
			m_lacunarity = s_default_lacunarity;
			m_basis_noise_module = null;
		}

		public NoiseModuleFractalBase(OpenTK.Vector3 axial_frequencies, uint num_octaves, NoiseModuleBase basis_noise_module, float fractal_increment, float lacunarity, bool anti_y = false, bool pos_y = false, bool neg_y = false) : base(axial_frequencies, anti_y, pos_y, neg_y)
		{
			m_num_octaves = num_octaves;
			m_fractal_increment = fractal_increment;
			m_lacunarity = lacunarity;
			m_basis_noise_module = basis_noise_module;
		}

		// The names "persistence" and "lacunarity" are not terribly self-explanatory, but they appear to be the accepted terms.
		//
		public uint m_num_octaves;                      // Number of successive octaves of noise to sum.
		public float m_fractal_increment;               // Exponent which relates the amplitude falloff/persistence with the lacunarity.  Persistence = 1 / lacunarity^fractal_increment.
		public float m_lacunarity;                      // Frequency multiplier for each successive octave.
		public NoiseModuleBase m_basis_noise_module;    // Another noise module that we should evaluate at each octave.
	}

	public class BasicSimplexNoiseModule : NoiseModuleBase
	{
		public BasicSimplexNoiseModule() : base()
		{
			m_open_simplex = new OpenSimplexNoise();
		}

		public BasicSimplexNoiseModule(OpenTK.Vector3 axial_frequencies, bool anti_y = false, bool pos_y = false, bool neg_y = false) : base(axial_frequencies, anti_y, pos_y, neg_y)
		{
			m_open_simplex = new OpenSimplexNoise();
		}

		public BasicSimplexNoiseModule(long seed) : base()
		{
			m_open_simplex = new OpenSimplexNoise(seed);
		}

		public BasicSimplexNoiseModule(OpenTK.Vector3 axial_frequencies, long seed, bool anti_y = false, bool pos_y = false, bool neg_y = false) : base(axial_frequencies, anti_y, pos_y, neg_y)
		{
			m_open_simplex = new OpenSimplexNoise(seed);
		}

		public override float EvaluateNoise(OpenTK.Vector3 coordinates, OpenTK.Vector3 deform_direction)
		{
			float base_noise_value = System.Convert.ToSingle(m_open_simplex.eval(coordinates.X * m_axial_frequencies.X, coordinates.Y * m_axial_frequencies.Y, coordinates.Z * m_axial_frequencies.Z));

			base_noise_value *= GetDirectionalStrength(deform_direction);

			// We can't just get the noise range at the end here, because some of the modifiers in the middle of the list
			//  may depend on knowing the proper noise range AT THAT POINT in the processing, not the original or final ranges.
			//  So, we have to build up the range incrementally as we go.
			float output_noise_value = base_noise_value;
			OpenTK.Vector2 output_noise_range = new OpenTK.Vector2(-1.0f, 1.0f);

			foreach (var modifier in m_modifiers) {
				output_noise_value = modifier.ModifyNoise(output_noise_value, output_noise_range);
				output_noise_range = modifier.ModifyRange(output_noise_range);
			}

			return output_noise_value;
		}

		public override OpenTK.Vector2 GetNoiseRange()
		{
			OpenTK.Vector2 output_noise_range = new OpenTK.Vector2(-1.0f, 1.0f);

			foreach (var modifier in m_modifiers) {
				output_noise_range = modifier.ModifyRange(output_noise_range);
			}

			return output_noise_range;
		}

		private OpenSimplexNoise m_open_simplex;
	}

	public class StandardFractalNoiseModule : NoiseModuleFractalBase
	{

		public StandardFractalNoiseModule() : base()
		{

		}

		public StandardFractalNoiseModule(OpenTK.Vector3 axial_frequencies, bool anti_y = false, bool pos_y = false, bool neg_y = false) : base(axial_frequencies, anti_y, pos_y, neg_y)
		{

		}

		public StandardFractalNoiseModule(OpenTK.Vector3 axial_frequencies, uint num_octaves, NoiseModuleBase basis_noise_module, float fractal_increment, float lacunarity, bool anti_y = false, bool pos_y = false, bool neg_y = false)
			: base(axial_frequencies, num_octaves, basis_noise_module, fractal_increment, lacunarity, anti_y, pos_y, neg_y)
		{

		}

		public override float EvaluateNoise(OpenTK.Vector3 coordinates, OpenTK.Vector3 deform_direction)
		{
			if (m_basis_noise_module == null) {
				throw new System.InvalidOperationException("Cannot evaluate fractal noise when no basis noise module has been provided.");
			}

			float persistence = System.Convert.ToSingle(System.Math.Pow(m_lacunarity, -1.0f * m_fractal_increment));

			float current_frequency_scale = 1.0f;
			float current_amplitude_scale = 1.0f;
			float noise_sum = 0.0f;

			float amplitude_min = 0.0f;
			float amplitude_max = 0.0f;   // After all octaves, values must be in the [amplitude_min..amplitude_max] range.

			OpenTK.Vector2 basis_noise_range = m_basis_noise_module.GetNoiseRange();

			for (uint octave = 0; octave < m_num_octaves; ++octave) {
				OpenTK.Vector3 scaled_coordinates = coordinates * m_axial_frequencies;
				scaled_coordinates.X *= current_frequency_scale;
				scaled_coordinates.Y *= current_frequency_scale;
				scaled_coordinates.Z *= current_frequency_scale;

				float base_noise_value = m_basis_noise_module.EvaluateNoise(scaled_coordinates, deform_direction) * current_amplitude_scale;

				noise_sum += base_noise_value;
				amplitude_min += (basis_noise_range.X * current_amplitude_scale);
				amplitude_max += (basis_noise_range.Y * current_amplitude_scale);

				current_amplitude_scale *= persistence;
				current_frequency_scale *= m_lacunarity;
			}

			float output_noise_value = noise_sum;
			OpenTK.Vector2 output_noise_range = new OpenTK.Vector2(amplitude_min, amplitude_max);

			foreach (var modifier in m_modifiers) {
				output_noise_value = modifier.ModifyNoise(output_noise_value, output_noise_range);
				output_noise_range = modifier.ModifyRange(output_noise_range);
			}

			return output_noise_value;
		}

		public override OpenTK.Vector2 GetNoiseRange()
		{
			if (m_basis_noise_module == null) {
				throw new System.InvalidOperationException("Cannot evaluate fractal noise when no basis noise module has been provided.");
			}

			float persistence = System.Convert.ToSingle(System.Math.Pow(m_lacunarity, -1.0f * m_fractal_increment));

			float amplitude_min = 0.0f;
			float amplitude_max = 0.0f;
			float current_amplitude_scale = 1.0f;

			OpenTK.Vector2 basis_noise_range = m_basis_noise_module.GetNoiseRange();

			for (uint octave = 0; octave < m_num_octaves; ++octave) {
				amplitude_min += (basis_noise_range.X * current_amplitude_scale);
				amplitude_max += (basis_noise_range.Y * current_amplitude_scale);

				current_amplitude_scale *= persistence;
			}

			OpenTK.Vector2 output_noise_range = new OpenTK.Vector2(amplitude_min, amplitude_max);

			foreach (var modifier in m_modifiers) {
				output_noise_range = modifier.ModifyRange(output_noise_range);
			}

			return output_noise_range;
		}
	}

	public class RidgedMultiFractalNoiseModule : NoiseModuleFractalBase
	{
		public static float s_default_noise_offset = 0.7f;

		public RidgedMultiFractalNoiseModule() : base()
		{
			m_noise_offset = s_default_noise_offset;
		}

		public RidgedMultiFractalNoiseModule(OpenTK.Vector3 axial_frequencies, bool anti_y = false, bool pos_y = false, bool neg_y = false) : base(axial_frequencies, anti_y, pos_y, neg_y)
		{
			m_noise_offset = s_default_noise_offset;
		}

		public RidgedMultiFractalNoiseModule(OpenTK.Vector3 axial_frequencies, uint num_octaves, NoiseModuleBase basis_noise_module, float fractal_increment, float lacunarity, float noise_offset, bool anti_y = false, bool pos_y = false, bool neg_y = false)
			: base(axial_frequencies, num_octaves, basis_noise_module, fractal_increment, lacunarity, anti_y, pos_y, neg_y)
      {
			m_noise_offset = noise_offset;
		}

		public override float EvaluateNoise(OpenTK.Vector3 coordinates, OpenTK.Vector3 deform_direction)
		{
			if (m_basis_noise_module == null) {
				throw new System.InvalidOperationException("Cannot evaluate fractal noise when no basis noise module has been provided.");
			}

			float persistence = System.Convert.ToSingle(System.Math.Pow(m_lacunarity, -1.0f * m_fractal_increment));

			float current_frequency_scale = 1.0f;
			float current_weight = 1.0f;
			float current_amplitude_scale = 1.0f;
			float noise_sum = 0.0f;

			float amplitude_min = 0.0f;
			float amplitude_max = 0.0f;   // After all octaves, values must be in the [amplitude_min..amplitude_max] range.

			OpenTK.Vector2 basis_noise_range = m_basis_noise_module.GetNoiseRange();

			for (uint octave = 0; octave < m_num_octaves; ++octave) {
				OpenTK.Vector3 scaled_coordinates = coordinates * m_axial_frequencies;
				scaled_coordinates.X *= current_frequency_scale;
				scaled_coordinates.Y *= current_frequency_scale;
				scaled_coordinates.Z *= current_frequency_scale;

				float base_noise_value = (m_basis_noise_module.EvaluateNoise(scaled_coordinates, deform_direction) + m_noise_offset) * current_amplitude_scale;
				float weighted_noise_value = base_noise_value * current_weight;

				noise_sum += weighted_noise_value;

				amplitude_min += ((basis_noise_range.X + m_noise_offset) * current_amplitude_scale * current_weight);
				amplitude_max += ((basis_noise_range.Y + m_noise_offset) * current_amplitude_scale * current_weight);

				current_amplitude_scale *= persistence;
				current_frequency_scale *= m_lacunarity;
				current_weight = System.Math.Min(weighted_noise_value, 1.0f);
			}

			float output_noise_value = noise_sum;
			OpenTK.Vector2 output_noise_range = new OpenTK.Vector2(amplitude_min, amplitude_max);

			foreach (var modifier in m_modifiers) {
				output_noise_value = modifier.ModifyNoise(output_noise_value, output_noise_range);
				output_noise_range = modifier.ModifyRange(output_noise_range);
			}

			return output_noise_value;
		}

		public override OpenTK.Vector2 GetNoiseRange()
		{
			if (m_basis_noise_module == null) {
				throw new System.InvalidOperationException("Cannot evaluate fractal noise when no basis noise module has been provided.");
			}

			float persistence = System.Convert.ToSingle(System.Math.Pow(m_lacunarity, -1.0f * m_fractal_increment));

			float amplitude_min = 0.0f;
			float amplitude_max = 0.0f;
			float current_amplitude_scale = 1.0f;

			OpenTK.Vector2 basis_noise_range = m_basis_noise_module.GetNoiseRange();

			for (uint octave = 0; octave < m_num_octaves; ++octave) {
				amplitude_min += (basis_noise_range.X * current_amplitude_scale);
				amplitude_max += (basis_noise_range.Y * current_amplitude_scale);

				current_amplitude_scale *= persistence;
			}

			OpenTK.Vector2 output_noise_range = new OpenTK.Vector2(amplitude_min, amplitude_max);

			foreach (var modifier in m_modifiers) {
				output_noise_range = modifier.ModifyRange(output_noise_range);
			}

			return output_noise_range;
		}

		public float m_noise_offset;
	}

    public class HeightMapNoiseModule : NoiseModuleBase
    {
        public enum ProjectionAxis
        {
            POSITIVE_X,
            NEGATIVE_X,
            POSITIVE_Y,
            NEGATIVE_Y,
            POSITIVE_Z,
            NEGATIVE_Z,

            NUM
        }

        public class HeightMapData
        {
            public HeightMapData(string height_map_path)
            {
#if !OVERLOAD_LEVEL_EDITOR
                UnityEngine.Texture2D unity_heightmap_tex = UnityEditor.AssetDatabase.LoadAssetAtPath(height_map_path, typeof(UnityEngine.Texture2D)) as UnityEngine.Texture2D;
                if (unity_heightmap_tex != null) {
                    try {
                        UnityEngine.Color32[] raw_heightmap_data = unity_heightmap_tex.GetPixels32();

                        m_width = Convert.ToUInt32(unity_heightmap_tex.width);
                        m_height = Convert.ToUInt32(unity_heightmap_tex.height);
                        m_height_data = new float[m_width, m_height];


                        // Unity textures are set up so that the rows are laid out bottom to top in memory.  We want to rearrange this
                        //  so that m_height_data[0,0] is the TOP-LEFT corner of the map, and the coordinates are in the order [x, y] ([column, row])
                        for (uint row_index = 0; row_index < m_height; ++row_index) {
                            uint unity_row_index = (m_height - 1) - row_index;

                            for (uint col_index = 0; col_index < m_width; ++col_index) {
                                uint pixel_index = (unity_row_index * m_width) + col_index;
                                var pixel_color32 = raw_heightmap_data[pixel_index];

                                // Assuming that the heightmap textures are grayscale, so it doesn't matter which color channel we use.
                                //  Bias the height value so that a pixel value of 128 is 0 deformation, 255 is full forward deformation,
                                //  and both 0 and 1 equal full backward deformation.  (The clamping at 0/1 is because the halfway point
                                //  of 0-255 is 127.5, which falls between two integers.  Moving the center point over to exactly 128.0
                                //  makes the positive and negative ranges unequal in size.
                                int biased_height_value = System.Math.Max(Convert.ToInt32(pixel_color32.r) - 128, -127);
                                m_height_data[col_index, row_index] = Convert.ToSingle(biased_height_value) / 127.0f;
                            }
                        }
                    }
                    catch (System.Exception e) {
                        UnityEngine.Debug.LogError(e.Message);

                        // Just create an empty heightmap.
                        m_width = m_height = 1;
                        m_height_data = new float[1, 1];
                        m_height_data[0, 0] = 0.0f;
                    }

                    // There doesn't seem to be a way to unload the asset from the AssetDatabase once we're done with it.  Googling shows a bunch 
                    //  of other people who are confused by this, but no good answer except to use AssetBundles or the Resources interface, which
                    //  would require us to put all heightmaps in the Assets/Resources folder.  These would then get bundled into standalone builds,
                    //  which is entirely unnecessary.  So...I guess we just keep all heightmaps loaded until you close out the Unity Editor session.
                    //  I doubt this will be a memory or performance issue, but it's still annoying.
                } else {
                    // Just create an empty heightmap.
                    m_width = m_height = 1;
                    m_height_data = new float[1, 1];
                    m_height_data[0, 0] = 0.0f;
                }
#elif UNITY_STANDALONE
                // In standalone builds we can only load certain resources which have been packaged into the build.
                //  However, we shouldn't ever need to use this code in a standalone build (it's editor-only level
                //  conversion functionality), so just make sure it compiles - it doesn't need to work.
                m_width = m_height = 1;
                m_height_data = new float[1, 1];
                m_height_data[0, 0] = 0.0f;
#else
				try {
                    System.Drawing.Bitmap heightmap_tex = new System.Drawing.Bitmap(height_map_path);

                    m_width = System.Convert.ToUInt32(heightmap_tex.Width);
                    m_height = System.Convert.ToUInt32(heightmap_tex.Height);
                    m_height_data = new float[m_width, m_height];

                    for (int row_index = 0; row_index < heightmap_tex.Height; ++row_index) {
                        for (int col_index = 0; col_index < heightmap_tex.Width; ++col_index) {
                           var pixel_color32 = heightmap_tex.GetPixel(col_index, row_index);

                            // Assuming that the heightmap textures are grayscale, so it doesn't matter which color channel we use.
                            //  Bias the height value so that a pixel value of 128 is 0 deformation, 255 is full forward deformation,
                            //  and both 0 and 1 equal full backward deformation.  (The clamping at 0/1 is because the halfway point
                            //  of 0-255 is 127.5, which falls between two integers.  Moving the center point over to exactly 128.0
                            //  makes the positive and negative ranges unequal in size.
                            int biased_height_value = System.Math.Max(Convert.ToInt32(pixel_color32.R) - 128, -127);
                            m_height_data[col_index, row_index] = Convert.ToSingle(biased_height_value) / 127.0f;
                        }
                    }
                } catch (System.IO.FileNotFoundException exc) {
                    System.Windows.Forms.MessageBox.Show("Failed to find height map: " + exc.FileName);
                    m_width = m_height = 1;
                    m_height_data = new float[1, 1];
                    m_height_data[0, 0] = 0.0f;
                }
#endif
            }

            public uint m_width;
            public uint m_height;
            public float[,] m_height_data;
        }

        // No default/parameterless constructor, since you need to specify a filename and projection axis.
        //  We assume for the moment that all heightmaps must be located at Assets/Textures/DeformationHeightMaps.
        //  If that restriction becomes a problem, we can change this behavior to either search the entire Assets tree,
        //  or require the higher-level code to provide a full relative path instead of just a filename.
        //
        //  The projection axis takes the place of the anti_y/pos_y/neg_y params in the base noise module class, so those base
        //  params are not used for this module.
        public HeightMapNoiseModule(string height_map_filename, ProjectionAxis axis) : base()
        {
            m_heightmap_data = new HeightMapData("Assets\\Textures\\DeformationHeightMaps\\" + height_map_filename);
            m_projection_axis = axis;
            m_projection_offset = OpenTK.Vector3.Zero;
        }

        public HeightMapNoiseModule(string height_map_filename, OpenTK.Vector3 axial_frequencies, OpenTK.Vector3 projection_offset, ProjectionAxis axis) : base(axial_frequencies)
        {
            m_heightmap_data = new HeightMapData("Assets\\Textures\\DeformationHeightMaps\\" + height_map_filename);
            m_projection_axis = axis;
            m_projection_offset = projection_offset;
        }

        public override float EvaluateNoise(OpenTK.Vector3 coordinates, OpenTK.Vector3 deform_direction)
        {
            OpenTK.Vector3 offset_coordinates = coordinates + m_projection_offset;
            OpenTK.Vector2 noise_uv;
            float directional_strength;

            // UV projection is set up so that we always map 64 texels per meter when the axial frequencies are 1:1.
            //  That way, non-square textures are not squeezed back to square by the projection, and different resolution
            //  heightmaps keep the same relative sample density.
            OpenTK.Vector2 heightmap_dimensions_in_meters;
            heightmap_dimensions_in_meters.X = Convert.ToSingle(m_heightmap_data.m_width) / 64.0f;
            heightmap_dimensions_in_meters.Y = Convert.ToSingle(m_heightmap_data.m_height) / 64.0f;

            switch (m_projection_axis) {
                case ProjectionAxis.POSITIVE_X:
                    noise_uv.X = offset_coordinates.Z * m_axial_frequencies.Z;
                    noise_uv.Y = -offset_coordinates.Y * m_axial_frequencies.Y;
                    directional_strength = System.Math.Max(deform_direction.X, 0.0f);
                    break;
                case ProjectionAxis.NEGATIVE_X:
                    noise_uv.X = -offset_coordinates.Z * m_axial_frequencies.Z;
                    noise_uv.Y = -offset_coordinates.Y * m_axial_frequencies.Y;
                    directional_strength = System.Math.Max(-deform_direction.X, 0.0f);
                    break;
                case ProjectionAxis.POSITIVE_Y:
                    noise_uv.X = offset_coordinates.X * m_axial_frequencies.X;
                    noise_uv.Y = -offset_coordinates.Z * m_axial_frequencies.Z;
                    directional_strength = System.Math.Max(deform_direction.Y, 0.0f);
                    break;
                case ProjectionAxis.NEGATIVE_Y:
                    noise_uv.X = offset_coordinates.X * m_axial_frequencies.X;
                    noise_uv.Y = offset_coordinates.Z * m_axial_frequencies.Z;
                    directional_strength = System.Math.Max(-deform_direction.Y, 0.0f);
                    break;
                case ProjectionAxis.POSITIVE_Z:
                    noise_uv.X = -offset_coordinates.X * m_axial_frequencies.Z;
                    noise_uv.Y = -offset_coordinates.Y * m_axial_frequencies.Y;
                    directional_strength = System.Math.Max(deform_direction.Z, 0.0f);
                    break;
                case ProjectionAxis.NEGATIVE_Z:
                    noise_uv.X = offset_coordinates.X * m_axial_frequencies.Z;
                    noise_uv.Y = -offset_coordinates.Y * m_axial_frequencies.Y;
                    directional_strength = System.Math.Max(-deform_direction.Z, 0.0f);
                    break;
                default:
                    noise_uv.X = noise_uv.Y = 0.0f;
                    directional_strength = 0.0f;
                    break;
            }

            noise_uv.X /= heightmap_dimensions_in_meters.X;
            noise_uv.Y /= heightmap_dimensions_in_meters.Y;

            // Bilinear filtering.  Texel center for texel (A, B) is located at UV (A + 0.5, B + 0.5).
            float noise_u_px = (noise_uv.X * Convert.ToSingle(m_heightmap_data.m_width)) - 0.5f;
            float noise_v_px = (noise_uv.Y * Convert.ToSingle(m_heightmap_data.m_height)) - 0.5f;

            int px_u_left = Convert.ToInt32(System.Math.Floor(noise_u_px));
            int px_v_top = Convert.ToInt32(System.Math.Floor(noise_v_px));

            int px_u_right = px_u_left + 1;
            int px_v_bottom = px_v_top + 1;

            float interp_u = noise_u_px - Convert.ToSingle(px_u_left);
            float interp_v = noise_v_px - Convert.ToSingle(px_v_top);

            // Pixel positions right now are unbounded (except for the limits of a 32-bit integer).
            //  We need to convert these positions to handle texture wrapping.
            px_u_left = px_u_left % (int)m_heightmap_data.m_width;
            px_u_right = px_u_right % (int)m_heightmap_data.m_width;
            px_v_top = px_v_top % (int)m_heightmap_data.m_height;
            px_v_bottom = px_v_bottom % (int)m_heightmap_data.m_height;

            // (A % B) = C returns a negative result when A is negative.  Adding B to the negative result
            //  will always give us the positive value we want in these cases, since (C) and (C+B) are both the same in mod B.
            if (px_u_left < 0) {
                px_u_left += (int)m_heightmap_data.m_width;
            }
            if (px_u_right < 0) {
                px_u_right += (int)m_heightmap_data.m_width;
            }
            if (px_v_top < 0) {
                px_v_top += (int)m_heightmap_data.m_height;
            }
            if (px_v_bottom < 0) {
                px_v_bottom += (int)m_heightmap_data.m_height;
            }

            // Gather and interpolate the four values.
            float noise_tl = m_heightmap_data.m_height_data[px_u_left, px_v_top];
            float noise_tr = m_heightmap_data.m_height_data[px_u_right, px_v_top];
            float noise_bl = m_heightmap_data.m_height_data[px_u_left, px_v_bottom];
            float noise_br = m_heightmap_data.m_height_data[px_u_right, px_v_bottom];

            float noise_top = (interp_u * noise_tr) + ((1.0f - interp_u) * noise_tl);
            float noise_bottom = (interp_u * noise_br) + ((1.0f - interp_u) * noise_bl);

            float base_noise_value = (interp_v * noise_bottom) + ((1.0f - interp_v) * noise_top);

            base_noise_value *= directional_strength;

            // We can't just get the noise range at the end here, because some of the modifiers in the middle of the list
            //  may depend on knowing the proper noise range AT THAT POINT in the processing, not the original or final ranges.
            //  So, we have to build up the range incrementally as we go.
            float output_noise_value = base_noise_value;
            OpenTK.Vector2 output_noise_range = new OpenTK.Vector2(-1.0f, 1.0f);

            foreach (var modifier in m_modifiers) {
                output_noise_value = modifier.ModifyNoise(output_noise_value, output_noise_range);
                output_noise_range = modifier.ModifyRange(output_noise_range);
            }

            return output_noise_value;
        }

        public override OpenTK.Vector2 GetNoiseRange()
        {
            OpenTK.Vector2 output_noise_range = new OpenTK.Vector2(-1.0f, 1.0f);

            foreach (var modifier in m_modifiers) {
                output_noise_range = modifier.ModifyRange(output_noise_range);
            }

            return output_noise_range;
        }

        HeightMapData m_heightmap_data;
        ProjectionAxis m_projection_axis;
        OpenTK.Vector3 m_projection_offset;
    }
}