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

#if !OVERLOAD_LEVEL_EDITOR
using UnityEngine;
#else
using System.ComponentModel;
using System.Drawing.Design;
using System.Drawing;
#endif
using System;

namespace Overload
{
	/// <summary>
	/// We need to use an EntityGuid wrapper because Unity can't properly
	/// serialize Guids (it just ignores them). So, when it comes to Unity
	/// code we treat Guids as strings. But we want to try and share the code as much as possible
	/// </summary>
	[Serializable]
	public struct EntityGuid
	{
#if !OVERLOAD_LEVEL_EDITOR
		public string m_guid;
		public static EntityGuid Empty
		{
			get { return new EntityGuid() { m_guid = "00000000-0000-0000-0000-000000000000" }; }
		}

		public EntityGuid(string id)
		{
			m_guid = id;
		}

		public EntityGuid(Guid id)
		{
			m_guid = id.ToString();
		}

		public static implicit operator Guid(EntityGuid eguid)
		{
			return new Guid(eguid.m_guid);
		}

		public static bool operator !=(EntityGuid lhs, EntityGuid rhs)
		{
			return lhs.m_guid != rhs.m_guid;
		}

		public static bool operator ==(EntityGuid lhs, EntityGuid rhs)
		{
			return lhs.m_guid == rhs.m_guid;
		}

		public override int GetHashCode()
		{
			return m_guid.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(this, obj))
				return true;

			if (obj == null || obj.GetType() != typeof(EntityGuid))
				return false;

			EntityGuid other = (EntityGuid)obj;
			return m_guid == other.m_guid;
		}

		public override string ToString()
		{
			return m_guid;
		}
#else
		public Guid m_guid;
		public static EntityGuid Empty
		{
			get { return new EntityGuid() { m_guid = Guid.Empty }; }
		}

		public EntityGuid( string id )
		{
			m_guid = new Guid( id );
		}

		public EntityGuid( Guid id )
		{
			m_guid = id;
		}

		public static implicit operator Guid( EntityGuid eguid )
		{
			return eguid.m_guid;
		}

		public static bool operator ==( EntityGuid lhs, EntityGuid rhs )
		{
			return lhs.m_guid == rhs.m_guid;
		}

		public static bool operator !=( EntityGuid lhs, EntityGuid rhs )
		{
			return lhs.m_guid != rhs.m_guid;
		}

		public override int GetHashCode()
		{
			return m_guid.GetHashCode();
		}

		public override bool Equals( object obj )
		{
			if( ReferenceEquals( this, obj ) )
				return true;

			if( obj == null || obj.GetType() != typeof( EntityGuid ) )
				return false;

			EntityGuid other = (EntityGuid)obj;
			return m_guid == other.m_guid;
		}

		public override string ToString()
		{
			return m_guid.ToString();
		}
#endif
	}

	public enum EntityPropsType
	{
		Robot,
		Door,
		Item,
		Prop,
		Script,
		Trigger,
		Light,
		Special,
	}

	[Serializable]
	public abstract class EntityProps
	{
		public abstract EntityPropsType GetPropsType();
	}

	[Serializable]
	public class EntityPropsRobot : EntityProps
	{
		public override EntityPropsType GetPropsType()
		{
			return EntityPropsType.Robot;
		}

		public int AIOverride
		{
			get { return ai_override; }
			set { ai_override = value; }
		}

		public ItemPrefab ItemDrop1
		{
			get { return bonus_drop1; }
			set { bonus_drop1 = value; }
		}

		public ItemPrefab ItemDrop2
		{
			get { return bonus_drop2; }
			set { bonus_drop2 = value; }
		}

		public bool NoDefault
		{
			get { return replace_default_drop; }
			set { replace_default_drop = value; }
		}

		public bool Station
		{
			get { return station; }
			set { station = value; }
		}

		public bool Headlight
		{
			get { return headlight; }
			set { headlight = value; }
		}

		public bool Super
		{
			get { return super; }
			set { super = value; }
		}

		public bool Variant
		{
			get { return variant; }
			set { variant = value; }
		}

		public bool NGPlus
		{
			get { return ng_plus; }
			set { ng_plus = value; }
		}

		public bool Hidden
		{
			get { return hidden; }
			set { hidden = value; }
		}

		public bool Stasis
		{
			get { return stasis; }
			set { stasis = value; }
		}

		public int ai_override = 0;
		public bool station = false;
		public bool headlight = false;
		public bool super = false;
		public bool ng_plus = false;
		public bool hidden = false;
		public bool stasis = false;
		public bool variant = false;

		public ItemPrefab bonus_drop1 = ItemPrefab.none;
		public ItemPrefab bonus_drop2 = ItemPrefab.none;
		public bool replace_default_drop = false;
	}

#if OVERLOAD_LEVEL_EDITOR
	public class PercentTypeConverter : TypeConverter
	{
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return sourceType == typeof(string);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
		{
			if (value is string) {
				string s = (string)value;
				s = s.Replace("%", "");
				return float.Parse(s, System.Globalization.NumberStyles.AllowThousands, culture) / 100f;
			}

			return base.ConvertFrom(context, culture, value);
		}

		public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(string)) {
				float f = (float)value;
				return (f * 100).ToString() + "%";
			}

			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
#endif

	[Serializable]
	public class EntityPropsDoor : EntityProps
	{
		public override EntityPropsType GetPropsType()
		{
			return EntityPropsType.Door;
		}

		public DoorLock DoorLock
		{
			get { return m_door_lock; }
			set { m_door_lock = value; }
		}

		public bool RobotAccess
		{
			get { return m_robot_access; }
			set { m_robot_access = value; }
		}

		public bool NoChunk			// True -> this door is not part of any chunk for activation purposes
		{
			get { return m_no_chunk; }
			set { m_no_chunk = value; }
		}

		public DoorLock m_door_lock = DoorLock.NONE;
		public bool m_robot_access = false;
		public bool m_no_chunk = false;			// Not part of a chunk for activation purposes

		//Used in level conversion, but not in the editor
		public int m_portal;
	}

	[Serializable]
	public class EntityPropsItem : EntityProps
	{
		public override EntityPropsType GetPropsType()
		{
			return EntityPropsType.Item;
		}

		public bool Respawns
		{
			get { return respawns; }
			set { respawns = value; }
		}

		public bool Super
		{
			get { return super; }
			set { super = value; }
		}

		public bool Secret
		{
			get { return secret; }
			set { secret = value; }
		}

		public int Index 
		{
			get { return index; }
			set { index = value; }
		}

		public bool respawns;
		public bool super;
		public bool secret;
		public int index;
	}

	[Serializable]
	public class EntityPropsProp : EntityProps
	{
		public override EntityPropsType GetPropsType()
		{
			return EntityPropsType.Prop;
		}

		public bool Invulnerable
		{
			get { return invulnerable; }
			set { invulnerable = value; }
		}

		public int Index
		{
			get { return index; }
			set { index = value; }
		}

		public bool NoChunk
		{
			get { return m_no_chunk; }
			set { m_no_chunk = value; }
		}

		public float? Hp
        {
			get { return m_hp; }
			set { m_hp = value; }
        }

		public bool m_no_chunk = false;
		public bool invulnerable = false;
		public int index = 0;
		public float? m_hp = null;
	}

	[Serializable]
	public class EntityPropsScript : EntityProps, IHasLinks
	{
		public override EntityPropsType GetPropsType()
		{
			return EntityPropsType.Script;
		}

		public float Delay
		{
			get { return delay; }
			set { delay = value; }
		}

		public EntityGuid[] EntityLinks
		{
			get { return entity_link; }
			set { entity_link = value; }
		}

		public bool ShowMessage
		{
			get { return show_message; }
			set { show_message = value; }
		}

		public int SpecialIndex
		{
			get { return special_index; }
			set { special_index = value; }
		}

		public const int MAX_LINKS = 5;
		public EntityGuid[] entity_link = new EntityGuid[MAX_LINKS];

		public float delay = 0f;
		public bool show_message = false;
		public int special_index = 0;

		public EntityPropsScript()
		{
			for (int i = 0; i < MAX_LINKS; i++) {
				entity_link[i] = EntityGuid.Empty;
			}
		}
	}

	interface IHasLinks
	{
		EntityGuid[] EntityLinks { get; set; }
	}

	[Serializable]
	public class EntityPropsTrigger : EntityProps, IHasLinks
	{
		public override EntityPropsType GetPropsType()
		{
			return EntityPropsType.Trigger;
		}

		public const int MAX_LINKS = 5;
		public EntityGuid[] entity_link = new EntityGuid[MAX_LINKS];

		public bool OneTime
		{
			get { return one_time; }
			set { one_time = value; }
		}

		public float RptDelay
		{
			get { return repeat_delay; }
			set { repeat_delay = value; }
		}

		public bool Weapons
		{
			get { return player_weapons; }
			set { player_weapons = value; }
		}

		public EntityGuid[] EntityLinks
		{
			get { return entity_link; }
			set { entity_link = value; }
		}

#if !(UNITY_STANDALONE || UNITY_PS4 || UNITY_XBOXONE)
		public float SizeX
		{
			get { return size.X; }
			set { size.X = value; }
		}

		public float SizeY
		{
			get { return size.Y; }
			set { size.Y = value; }
		}

		public float SizeZ
		{
			get { return size.Z; }
			set { size.Z = value; }
		}
#endif

		public bool one_time = true;
		public float repeat_delay = 1f;
		public bool player_weapons = false;
#if !(UNITY_STANDALONE || UNITY_PS4 || UNITY_XBOXONE)
		public OpenTK.Vector3 size = new OpenTK.Vector3(4f, 4f, 1f);
#else
		public Vector3 size = Vector3.zero;
#endif

		public EntityPropsTrigger()
		{
			for (int i = 0; i < MAX_LINKS; i++) {
				entity_link[i] = EntityGuid.Empty;
			}
		}
	}

	[Serializable]
	public class EntityPropsLight : EntityProps
	{
		public override EntityPropsType GetPropsType()
		{
			return EntityPropsType.Light;
		}

		public float SpotAngle
		{
			get { return angle; }
			set { angle = value; }
		}

		public float Range
		{
			get { return range; }
			set { range = value; }
		}

		public float Intensity
		{
			get { return intensity; }
			set { intensity = value; }
		}

		public float Hue
		{
			get { return c_hue; }
			set { c_hue = value; UpdateColor(); }
		}

		public float Sat
		{
			get { return c_sat; }
			set { c_sat = value; UpdateColor(); }
		}

		public float Bright
		{
			get { return c_bri; }
			set { c_bri = value; UpdateColor(); }
		}

		public float angle = 45f;
		public float range = 15f;
		public float intensity = 3f;
		public bool shadows = true;
		public float c_hue = 0f;
		public float c_sat = 0f;
		public float c_bri = 1f;

		// For displaying the color (no editing)
#if !(UNITY_STANDALONE || UNITY_PS4 || UNITY_XBOXONE)
		public System.Drawing.Color color = System.Drawing.Color.White;

		public EditColor BaseColor
		{
			get { return new EditColor() {  Color = color }; }
			set { color = value.Color; UpdateHSB(); }
		}
#endif

		public void UpdateColor()
		{
#if OVERLOAD_LEVEL_EDITOR
			color = HSBColor.ConvertToSystemColor(c_hue, c_sat, c_bri);
#endif
		}

		public void UpdateHSB()
		{
#if OVERLOAD_LEVEL_EDITOR
			var hsb = new HSBColor(color);
			c_hue = hsb.h;
			c_sat = hsb.s;
			c_bri = hsb.b;
#endif
		}
	}

	public enum MatcenSpawnWait
	{
		VERY_SLOW = 10,
		SLOW = 7,
		MEDIUM = 5,
		FAST = 4,
		VERY_FAST = 3
	}

	public enum MatcenSpecialProperties
	{
		NONE = 0,
		VARIANT,
		SUPER,
	}

	[Serializable]
	public class EntityPropsSpecial : EntityProps
	{
		public override EntityPropsType GetPropsType()
		{
			return EntityPropsType.Special;
		}

		public MatcenSpecialProperties SpecialProps
		{
			get { return special_props; }
			set { special_props = value; }
		}

		public int MaxAlive
		{
			get { return m_max_alive; }
			set { m_max_alive = value; }
		}
		public MatcenSpawnWait SpawnWait
		{
			get { return m_spawn_wait; }
			set { m_spawn_wait = value; }
		}

		public EnemyType Spawn_1
		{

			get { return matcen_spawn_type_1; }
			set { matcen_spawn_type_1 = value; }
		}

		public float Probability_1
		{

			get { return matcen_spawn_probability_1; }
			set { matcen_spawn_probability_1 = value; }
		}

		
		public EnemyType Spawn_2
		{
			get { return matcen_spawn_type_2; }
			set { matcen_spawn_type_2 = value; }
		}

		public float Probability_2
		{

			get { return matcen_spawn_probability_2; }
			set { matcen_spawn_probability_2 = value; }
		}

		public bool Invulnerable
		{
			get { return ed_invulnerable; }
			set { ed_invulnerable = value; }
		}

		public float? Hp
        {
			get { return m_hp; }
			set { m_hp = value; }
        }

		public MatcenSpecialProperties special_props = MatcenSpecialProperties.NONE;
		public int m_max_alive = 3;
		public MatcenSpawnWait m_spawn_wait = MatcenSpawnWait.MEDIUM;
		public EnemyType matcen_spawn_type_1 = EnemyType.RECOILA;
		public float matcen_spawn_probability_1 = 1f;
		public EnemyType matcen_spawn_type_2 = EnemyType.RECOILA;
		public float matcen_spawn_probability_2 = 0f;
		public bool ed_invulnerable = false;
		public float? m_hp = null;
	}

#if OVERLOAD_LEVEL_EDITOR
	class ColorEditor : UITypeEditor
	{
		public static int[] CustomColors;

		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.Modal;
		}

		public override object EditValue(ITypeDescriptorContext context,
			IServiceProvider provider, object value)
		{
			var dlg = new System.Windows.Forms.ColorDialog();
			dlg.FullOpen = true;
			dlg.Color = ((EditColor)value).Color;
			dlg.CustomColors = CustomColors;
			var res = dlg.ShowDialog();
			CustomColors = dlg.CustomColors;
			if (res == System.Windows.Forms.DialogResult.OK)
				return new EditColor() { Color = dlg.Color };
			return value;
		}

		public override void PaintValue(System.Drawing.Design.PaintValueEventArgs e)
		{
			e.Graphics.FillRectangle(new SolidBrush(((EditColor)e.Value).Color), e.Bounds);
			return;
		}

		public override bool GetPaintValueSupported(System.ComponentModel.ITypeDescriptorContext context)
		{
			return true;
		}
	}

	[Editor(typeof(ColorEditor), typeof(UITypeEditor))]
	public struct EditColor
	{
		public System.Drawing.Color Color;

		public override string ToString()
		{
			var rgb = Color.ToArgb();
			int r = (rgb >> 16) & 255, g = (rgb >> 8) & 255, b = rgb & 255;
			return r.ToString("x2") + g.ToString("x2") + b.ToString("x2");
		}
	}
#endif

}
