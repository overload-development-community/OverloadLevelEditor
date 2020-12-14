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
using OpenTK;

namespace OverloadLevelEditor
{
	abstract public class DeformationModuleBaseNew
	{
		public DeformationModuleBaseNew()
		{
			m_noise_modules = new System.Collections.Generic.List<NoiseModuleBase>();
		}

		public void AddNoiseModule(NoiseModuleBase noise_module)
		{
			m_noise_modules.Add(noise_module);
		}

		public int GetNumNoiseModules()
		{
			return m_noise_modules.Count;
		}

		public NoiseModuleBase GetNoiseModule(int module_index)
		{
			if ((module_index >= 0) && (module_index < m_noise_modules.Count)) {
				return m_noise_modules[module_index];
			} else {
				return null;
			}
		}

		public abstract OpenTK.Vector3 GetDeformedPosition(OpenTK.Vector3 initial_position, OpenTK.Vector3 deformation_direction, float side_deformation_strength);

		public System.Collections.Generic.List<NoiseModuleBase> m_noise_modules;
	}

	// The standard deformation module just averages the different noise values produced from the different modules without any special weighting.
	public class StandardDeformationModule : DeformationModuleBaseNew
	{
		public static float s_default_deformation_strength = 2.0f;

		public StandardDeformationModule(OpenTK.Vector3 deformation_strength)
		{
			m_base_deformation_strength = deformation_strength;
		}

		public StandardDeformationModule()
		{
			m_base_deformation_strength = new Vector3(1f, 1f, 1f) * s_default_deformation_strength;
		}

		public override Vector3 GetDeformedPosition(Vector3 initial_position, Vector3 deformation_direction, float side_deformation_strength)
		{
			if (m_noise_modules.Count == 0) {
				return initial_position;
			} else {
				// Average the noise values for each individual noise module.
				float average_deformation_offset = 0.0f;

				foreach (var noise_module in m_noise_modules) {
					average_deformation_offset += noise_module.EvaluateNoise(initial_position, deformation_direction);
				}

				average_deformation_offset /= System.Convert.ToSingle(m_noise_modules.Count);
				Vector3 deformation_vec = deformation_direction * side_deformation_strength * average_deformation_offset;
				deformation_vec.X *= m_base_deformation_strength.X;
				deformation_vec.Y *= m_base_deformation_strength.Y;
				deformation_vec.Z *= m_base_deformation_strength.Z;

				return initial_position + deformation_vec;
			}
		}

		public OpenTK.Vector3 m_base_deformation_strength;
	}

	// The weighted average deformation module averages the different noise values produced from the different modules using the specified weights (normalized to sum to 1.0)
	public class WeightedAverageDeformationModule : DeformationModuleBaseNew
	{
		public static float s_default_deformation_strength = 2.0f;

		public WeightedAverageDeformationModule(OpenTK.Vector3 deformation_strength)
		{
			m_base_deformation_strength = deformation_strength;
			m_noise_module_weights = new System.Collections.Generic.Dictionary<NoiseModuleBase, float>();
		}

		public WeightedAverageDeformationModule()
		{
			m_base_deformation_strength = new Vector3(1f, 1f, 1f) * s_default_deformation_strength;
			m_noise_module_weights = new System.Collections.Generic.Dictionary<NoiseModuleBase, float>();
		}

		public void AddWeightedNoiseModule(NoiseModuleBase noise_module, float weight)
		{
			m_noise_modules.Add(noise_module);
			m_noise_module_weights[noise_module] = weight;
		}

		public float GetNoiseModuleWeight(int module_index)
		{
			var noise_module = GetNoiseModule(module_index);

			if (noise_module == null) {
				return 0.0f;
			} else if (m_noise_module_weights.ContainsKey(noise_module)) {
				return m_noise_module_weights[noise_module];
			} else {
				return 1.0f;
			}
		}

		public override Vector3 GetDeformedPosition(Vector3 initial_position, Vector3 deformation_direction, float side_deformation_strength)
		{
			if (m_noise_modules.Count == 0) {
				return initial_position;
			} else {
				// Average the noise values for each individual noise module.
				float average_deformation_offset = 0.0f;
				float weight_sum = 0.0f;

				foreach (var noise_module in m_noise_modules) {
					float module_weight;
					if (m_noise_module_weights.ContainsKey(noise_module)) {
						module_weight = m_noise_module_weights[noise_module];
					} else {
						module_weight = 1.0f;
					}

					average_deformation_offset += (noise_module.EvaluateNoise(initial_position, deformation_direction) * module_weight);
					weight_sum += module_weight;
				}

				average_deformation_offset /= weight_sum;
				Vector3 deformation_vec = deformation_direction * side_deformation_strength * average_deformation_offset;
				deformation_vec.X *= m_base_deformation_strength.X;
				deformation_vec.Y *= m_base_deformation_strength.Y;
				deformation_vec.Z *= m_base_deformation_strength.Z;

				return initial_position + deformation_vec;
			}
		}

		public OpenTK.Vector3 m_base_deformation_strength;
		public System.Collections.Generic.Dictionary<NoiseModuleBase, float> m_noise_module_weights;
	}

	// The additive deformation module just sums up the different noise values produced from the different modules.
	public class AdditiveDeformationModule : DeformationModuleBaseNew
	{
		public static float s_default_deformation_strength = 2.0f;

		public AdditiveDeformationModule(OpenTK.Vector3 deformation_strength)
		{
			m_base_deformation_strength = deformation_strength;
		}

		public AdditiveDeformationModule()
		{
			m_base_deformation_strength = new Vector3(1f, 1f, 1f) * s_default_deformation_strength;
		}

		public override Vector3 GetDeformedPosition(Vector3 initial_position, Vector3 deformation_direction, float side_deformation_strength)
		{
			if (m_noise_modules.Count == 0) {
				return initial_position;
			} else {
				// Average the noise values for each individual noise module.
				float total_deformation_offset = 0.0f;
				foreach (var noise_module in m_noise_modules) {
					total_deformation_offset += noise_module.EvaluateNoise(initial_position, deformation_direction);
				}

				Vector3 deformation_vec = deformation_direction * side_deformation_strength * total_deformation_offset;
				deformation_vec.X *= m_base_deformation_strength.X;
				deformation_vec.Y *= m_base_deformation_strength.Y;
				deformation_vec.Z *= m_base_deformation_strength.Z;

				return initial_position + deformation_vec;
			}
		}

		public OpenTK.Vector3 m_base_deformation_strength;
	}

	// The weighted additive deformation module just sums up the different noise values produced from the different modules using the specified weights additively.
	public class WeightedAdditiveDeformationModule : DeformationModuleBaseNew
	{
		public static float s_default_deformation_strength = 2.0f;

		public WeightedAdditiveDeformationModule(OpenTK.Vector3 deformation_strength)
		{
			m_base_deformation_strength = deformation_strength;
			m_noise_module_weights = new System.Collections.Generic.Dictionary<NoiseModuleBase, float>();
		}

		public WeightedAdditiveDeformationModule()
		{
			m_base_deformation_strength = new Vector3(1f, 1f, 1f) * s_default_deformation_strength;
			m_noise_module_weights = new System.Collections.Generic.Dictionary<NoiseModuleBase, float>();
		}

		public void AddWeightedNoiseModule(NoiseModuleBase noise_module, float weight)
		{
			m_noise_modules.Add(noise_module);
			m_noise_module_weights[noise_module] = weight;
		}

		public float GetNoiseModuleWeight(int module_index)
		{
			var noise_module = GetNoiseModule(module_index);

			if (noise_module == null) {
				return 0.0f;
			} else if (m_noise_module_weights.ContainsKey(noise_module)) {
				return m_noise_module_weights[noise_module];
			} else {
				return 1.0f;
			}
		}

		public override Vector3 GetDeformedPosition(Vector3 initial_position, Vector3 deformation_direction, float side_deformation_strength)
		{
			if (m_noise_modules.Count == 0) {
				return initial_position;
			} else {
				// Average the noise values for each individual noise module.
				float total_deformation_offset = 0.0f;
				foreach (var noise_module in m_noise_modules) {
					float module_weight;
					if (m_noise_module_weights.ContainsKey(noise_module)) {
						module_weight = m_noise_module_weights[noise_module];
					} else {
						module_weight = 1.0f;
					}

					total_deformation_offset += (noise_module.EvaluateNoise(initial_position, deformation_direction) * module_weight);
				}

				Vector3 deformation_vec = deformation_direction * side_deformation_strength * total_deformation_offset;
				deformation_vec.X *= m_base_deformation_strength.X;
				deformation_vec.Y *= m_base_deformation_strength.Y;
				deformation_vec.Z *= m_base_deformation_strength.Z;

				return initial_position + deformation_vec;
			}
		}

		public bool m_anti_vertical_bias;
		public bool m_positive_vertical_bias;
		public bool m_negative_vertical_bias;
		public OpenTK.Vector3 m_base_deformation_strength;
		public System.Collections.Generic.Dictionary<NoiseModuleBase, float> m_noise_module_weights;
	}

	// This custom example deformation modifier takes the first three noise modules added, and uses each of those noise values to offset in the X, Y, Z axes of the deformation direction.
	//  So, if the normalized deformation vector is (+/-1, 0, 0), it offsets entirely by the value of the first noise module, (0, +/-1, 0) would use entirely the noise of the second module,
	//  (0, 0, +/-1) would use the third noise modules, and arbitrary vectors would use some other combination of the three noise values.  If there are less than three noise modules present
	//  in the deformation module, then the remaining axes are not offset.  Any noise modules beyond the first three are ignored.
	public class CustomExampleDeformationModule : DeformationModuleBaseNew
	{
		public CustomExampleDeformationModule()
		{

		}

		public override Vector3 GetDeformedPosition(Vector3 initial_position, Vector3 deformation_direction, float side_deformation_strength)
		{
			float x_noise, y_noise, z_noise;

			if (m_noise_modules.Count > 0) {
				x_noise = m_noise_modules[0].EvaluateNoise(initial_position, deformation_direction);
			} else {
				x_noise = 0.0f;
			}

			if (m_noise_modules.Count > 1) {
				y_noise = m_noise_modules[1].EvaluateNoise(initial_position, deformation_direction);
			} else {
				y_noise = 0.0f;
			}

			if (m_noise_modules.Count > 2) {
				z_noise = m_noise_modules[2].EvaluateNoise(initial_position, deformation_direction);
			} else {
				z_noise = 0.0f;
			}

			Vector3 deformation_vec = deformation_direction * side_deformation_strength;
			deformation_vec.X *= x_noise;
			deformation_vec.Y *= y_noise;
			deformation_vec.Z *= z_noise;

			return initial_position + deformation_vec;
		}
	}
}
