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

// These are here so the editor doesn't require Unity to be installed to build or run.
namespace UnityEngine
{
	public struct Color
	{
		public float r;
		public float g;
		public float b;
		public float a;

		public static Color black = new Color( 0.0f, 0.0f, 0.0f );
		public static Color green = new Color( 0.0f, 1.0f, 0.0f );

		public Color( float _r, float _g, float _b, float _a = 1.0f)
		{
			r = _r;
			g = _g;
			b = _b;
			a = _a;
		}
	}

	public struct Color32
	{
		public byte r;
		public byte g;
		public byte b;
		public byte a;

		public Color32(byte _r, byte _g, byte _b, byte _a)
		{
			r = _r;
			g = _g;
			b = _b;
			a = _a;
		}
	}

    public struct Vector3
	{
		public float x;
		public float y;
		public float z;

		public static Vector3 zero = new Vector3( 0.0f, 0.0f, 0.0f );
		public static Vector3 one = new Vector3( 1.0f, 1.0f, 1.0f );
		public static Vector3 right = new Vector3( 1.0f, 0.0f, 0.0f );
		public static Vector3 up = new Vector3( 0.0f, 1.0f, 0.0f );
		public static Vector3 forward = new Vector3( 0.0f, 0.0f, 1.0f );

		public Vector3( float _x, float _y, float _z )
		{
			x = _x;
			y = _y;
			z = _z;
		}

		public static implicit operator Vector4( Vector3 d )
		{
			return new Vector4( d );
		}

		public float this[int key]
		{
			get {
				switch( key ) {
				case 0: return x;
				case 1: return y;
				case 2: return z;
				default:
					return 0.0f;
				}
			}
			set {
				switch( key ) {
				case 0: x = value; break;
				case 1: y = value; break;
				case 2: z = value; break;
				default:
					break;
				}
			}
		}

		public float sqrMagnitude
		{
			get { return SqrMagnitude( this ); }
		}

		public float magnitude
		{
			get { return (float)Math.Sqrt( (double)this.sqrMagnitude ); }
		}

		public Vector3 normalized
		{
			get {
				Vector3 v = this;
				return Normalize( v );
			}
		}

		public static Vector3 Cross( Vector3 lhs, Vector3 rhs )
		{
			return new Vector3( ( lhs.y * rhs.z ) - ( lhs.z * rhs.y ), ( lhs.z * rhs.x ) - ( lhs.x * rhs.z ), ( lhs.x * rhs.y ) - ( lhs.y * rhs.x ) );
		}

		public static float Distance(Vector3 a, Vector3 b)
		{
			Vector3 delta = a - b;
			return (float)Math.Sqrt( Dot( delta, delta ) );
		}

		public static float Dot( Vector3 a, Vector3 b )
		{
			return ( a.x * b.x ) + ( a.y * b.y ) + ( a.z * b.z );
		}

		public static Vector3 Lerp( Vector3 a, Vector3 b, float t )
		{
			float x = a.x * ( 1.0f - t ) + b.x * t;
			float y = a.y * ( 1.0f - t ) + b.y * t;
			float z = a.z * ( 1.0f - t ) + b.z * t;
			return new Vector3( x, y, z );
		}

		public static float SqrMagnitude( Vector3 v )
		{
			return Dot( v, v );
		}

		public static float Magnitude( Vector3 v )
		{
			return (float)Math.Sqrt( SqrMagnitude( v ) );
		}

		public static Vector3 Min( Vector3 lhs, Vector3 rhs )
		{
			var res = new Vector3();
			res.x = Math.Min( lhs.x, rhs.x );
			res.y = Math.Min( lhs.y, rhs.y );
			res.z = Math.Min( lhs.z, rhs.z );
			return res;
		}

		public static Vector3 Max( Vector3 lhs, Vector3 rhs )
		{
			var res = new Vector3();
			res.x = Math.Max( lhs.x, rhs.x );
			res.y = Math.Max( lhs.y, rhs.y );
			res.z = Math.Max( lhs.z, rhs.z );
			return res;
		}

        public static Vector3 Normalize( Vector3 v )
		{
			float len = Magnitude( v );
			float recip = 1.0f / len;
			return new Vector3() { x = v.x * recip, y = v.y * recip, z = v.z * recip };
		}

		public void Normalize()
		{
			float len = this.magnitude;
			float recip = 1.0f / len;
			x *= recip;
			y *= recip;
			z *= recip;
		}

		public static void OrthoNormalize( ref Vector3 normal, ref Vector3 tangent )
		{
			normal.Normalize();
			tangent.Normalize();
			Vector3 o = Vector3.Cross( normal, tangent );
			o.Normalize();
			tangent = Vector3.Cross( o, normal );
			tangent.Normalize();
		}

		public void Set(float _x, float _y, float _z)
		{
			x = _x;
			y = _y;
			z = _z;
		}

		public static Vector3 operator -( Vector3 a, Vector3 b )
		{
			return new Vector3( a.x - b.x, a.y - b.y, a.z - b.z );
		}

		public static Vector3 operator -( Vector3 a )
		{
			return new Vector3( -a.x, -a.y, -a.z );
		}

		public static Vector3 operator +( Vector3 a, Vector3 b )
		{
			return new Vector3( a.x + b.x, a.y + b.y, a.z + b.z );
		}

		public static Vector3 operator *( float a, Vector3 b )
		{
			return new Vector3( a * b.x, a * b.y, a * b.z );
		}

		public static Vector3 operator *( Vector3 b, float a )
		{
			return new Vector3( a * b.x, a * b.y, a * b.z );
		}

		public static Vector3 operator /( Vector3 a, float b )
		{
			return new Vector3( a.x / b, a.y / b, a.z / b );
		}

		public static Vector3 operator /(float a, Vector3 b)
		{
			return new Vector3(a / b.x, a / b.y, a / b.z);
		}
	}

	public struct Vector2
	{
		public float x;
		public float y;

		public Vector2( float _x, float _y )
		{
			x = _x;
			y = _y;
		}

		public float this[int key]
		{
			get {
				switch( key ) {
				case 0:
					return x;
				case 1:
					return y;
				default:
					return 0.0f;
				}
			}
			set {
				switch( key ) {
				case 0:
					x = value;
					break;
				case 1:
					y = value;
					break;
				default:
					break;
				}
			}
		}

		public static Vector2 Lerp( Vector2 a, Vector2 b, float t )
		{
			float x = a.x * ( 1.0f - t ) + b.x * t;
			float y = a.y * ( 1.0f - t ) + b.y * t;
			return new Vector2( x, y );
		}
	}

	public struct Vector4
	{
		public float x;
		public float y;
		public float z;
		public float w;

		public Vector4( Vector3 v )
		{
			x = v.x;
			y = v.y;
			z = v.z;
			w = 0.0f;
		}

		public Vector4( float _x, float _y, float _z, float _w )
		{
			x = _x;
			y = _y;
			z = _z;
			w = _w;
		}

		public float this[int key]
		{
			get {
				switch( key ) {
				case 0:
					return x;
				case 1:
					return y;
				case 2:
					return z;
				case 3:
					return w;
				default:
					return 0.0f;
				}
			}
			set {
				switch( key ) {
				case 0:
					x = value;
					break;
				case 1:
					y = value;
					break;
				case 2:
					z = value;
					break;
				case 3:
					w = value;
					break;
				default:
					break;
				}
			}
		}

		public override bool Equals(object other)
		{
			if (!(other is Vector4)) {
				return false;
			}
			Vector4 vector = (Vector4)other;
			return (((this.x.Equals(vector.x) && this.y.Equals(vector.y)) && this.z.Equals(vector.z)) && this.w.Equals(vector.w));
		}

		public override int GetHashCode()
		{
			return (((this.x.GetHashCode() ^ (this.y.GetHashCode() << 2)) ^ (this.z.GetHashCode() >> 2)) ^ (this.w.GetHashCode() >> 1));
		}

		public static float Dot( Vector4 lhs, Vector4 rhs )
		{
			return lhs.x * rhs.x + lhs.y * rhs.y + lhs.z * rhs.z + lhs.w * rhs.w;
		}

		public static Vector4 operator +(Vector4 a, Vector4 b)
		{
			return new Vector4(a.x + b.x, a.y + b.y, a.z + b.z, a.w + b.w);
		}

		public static Vector4 operator -(Vector4 a, Vector4 b)
		{
			return new Vector4(a.x - b.x, a.y - b.y, a.z - b.z, a.w - b.w);
		}

		public static float SqrMagnitude(Vector4 a)
		{
			return Dot(a, a);
		}

		public static bool operator ==(Vector4 lhs, Vector4 rhs)
		{
			return (SqrMagnitude(lhs - rhs) < 9.999999E-11f);
		}

		public static bool operator !=(Vector4 lhs, Vector4 rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct Quaternion
	{
		public float x;
		public float y;
		public float z;
		public float w;

		public Quaternion( float x, float y, float z, float w )
		{
			this.x = x;
			this.y = y;
			this.z = z;
			this.w = w;
		}

		public static Quaternion identity = new Quaternion(0.0f, 0.0f, 0.0f, 1.0f);
		
	}

	public struct Matrix4x4
	{
		public float m00;
		public float m10;
		public float m20;
		public float m30;
		public float m01;
		public float m11;
		public float m21;
		public float m31;
		public float m02;
		public float m12;
		public float m22;
		public float m32;
		public float m03;
		public float m13;
		public float m23;
		public float m33;

		public Matrix4x4(float _m00, float _m01, float _m02, float _m03, float _m10, float _m11, float _m12, float _m13, float _m20, float _m21, float _m22, float _m23, float _m30, float _m31, float _m32, float _m33)
		{
			m00 = _m00;
			m01 = _m01;
			m02 = _m02;
			m03 = _m03;
			m10 = _m10;
			m11 = _m11;
			m12 = _m12;
			m13 = _m13;
			m20 = _m20;
			m21 = _m21;
			m22 = _m22;
			m23 = _m23;
			m30 = _m30;
			m31 = _m31;
			m32 = _m32;
			m33 = _m33;
		}

		public static Matrix4x4 identity = new Matrix4x4(
			1.0f, 0.0f, 0.0f, 0.0f,
			0.0f, 1.0f, 0.0f, 0.0f,
			0.0f, 0.0f, 1.0f, 0.0f,
			0.0f, 0.0f, 0.0f, 1.0f);

		public float this[int index]
		{
			get
			{
				switch (index) {
					case 0:
						return this.m00;

					case 1:
						return this.m10;

					case 2:
						return this.m20;

					case 3:
						return this.m30;

					case 4:
						return this.m01;

					case 5:
						return this.m11;

					case 6:
						return this.m21;

					case 7:
						return this.m31;

					case 8:
						return this.m02;

					case 9:
						return this.m12;

					case 10:
						return this.m22;

					case 11:
						return this.m32;

					case 12:
						return this.m03;

					case 13:
						return this.m13;

					case 14:
						return this.m23;

					case 15:
						return this.m33;
				}
				throw new IndexOutOfRangeException("Invalid matrix index!");
			}
			set
			{
				switch (index) {
					case 0:
						this.m00 = value;
						break;

					case 1:
						this.m10 = value;
						break;

					case 2:
						this.m20 = value;
						break;

					case 3:
						this.m30 = value;
						break;

					case 4:
						this.m01 = value;
						break;

					case 5:
						this.m11 = value;
						break;

					case 6:
						this.m21 = value;
						break;

					case 7:
						this.m31 = value;
						break;

					case 8:
						this.m02 = value;
						break;

					case 9:
						this.m12 = value;
						break;

					case 10:
						this.m22 = value;
						break;

					case 11:
						this.m32 = value;
						break;

					case 12:
						this.m03 = value;
						break;

					case 13:
						this.m13 = value;
						break;

					case 14:
						this.m23 = value;
						break;

					case 15:
						this.m33 = value;
						break;

					default:
						throw new IndexOutOfRangeException("Invalid matrix index!");
				}
			}
		}

		public float this[int row, int column]
		{
			get
			{
				return this[row + (column * 4)];
			}
			set
			{
				this[row + (column * 4)] = value;
			}
		}

		public Vector4 GetRow(int i)
		{
			return new Vector4(this[i, 0], this[i, 1], this[i, 2], this[i, 3]);
		}

		public void SetRow(int i, Vector4 v)
		{
			this[i, 0] = v.x;
			this[i, 1] = v.y;
			this[i, 2] = v.z;
			this[i, 3] = v.w;
		}

		public static Matrix4x4 operator *(Matrix4x4 lhs, Matrix4x4 rhs)
		{
			return new Matrix4x4 { m00 = (((lhs.m00 * rhs.m00) + (lhs.m01 * rhs.m10)) + (lhs.m02 * rhs.m20)) + (lhs.m03 * rhs.m30), m01 = (((lhs.m00 * rhs.m01) + (lhs.m01 * rhs.m11)) + (lhs.m02 * rhs.m21)) + (lhs.m03 * rhs.m31), m02 = (((lhs.m00 * rhs.m02) + (lhs.m01 * rhs.m12)) + (lhs.m02 * rhs.m22)) + (lhs.m03 * rhs.m32), m03 = (((lhs.m00 * rhs.m03) + (lhs.m01 * rhs.m13)) + (lhs.m02 * rhs.m23)) + (lhs.m03 * rhs.m33), m10 = (((lhs.m10 * rhs.m00) + (lhs.m11 * rhs.m10)) + (lhs.m12 * rhs.m20)) + (lhs.m13 * rhs.m30), m11 = (((lhs.m10 * rhs.m01) + (lhs.m11 * rhs.m11)) + (lhs.m12 * rhs.m21)) + (lhs.m13 * rhs.m31), m12 = (((lhs.m10 * rhs.m02) + (lhs.m11 * rhs.m12)) + (lhs.m12 * rhs.m22)) + (lhs.m13 * rhs.m32), m13 = (((lhs.m10 * rhs.m03) + (lhs.m11 * rhs.m13)) + (lhs.m12 * rhs.m23)) + (lhs.m13 * rhs.m33), m20 = (((lhs.m20 * rhs.m00) + (lhs.m21 * rhs.m10)) + (lhs.m22 * rhs.m20)) + (lhs.m23 * rhs.m30), m21 = (((lhs.m20 * rhs.m01) + (lhs.m21 * rhs.m11)) + (lhs.m22 * rhs.m21)) + (lhs.m23 * rhs.m31), m22 = (((lhs.m20 * rhs.m02) + (lhs.m21 * rhs.m12)) + (lhs.m22 * rhs.m22)) + (lhs.m23 * rhs.m32), m23 = (((lhs.m20 * rhs.m03) + (lhs.m21 * rhs.m13)) + (lhs.m22 * rhs.m23)) + (lhs.m23 * rhs.m33), m30 = (((lhs.m30 * rhs.m00) + (lhs.m31 * rhs.m10)) + (lhs.m32 * rhs.m20)) + (lhs.m33 * rhs.m30), m31 = (((lhs.m30 * rhs.m01) + (lhs.m31 * rhs.m11)) + (lhs.m32 * rhs.m21)) + (lhs.m33 * rhs.m31), m32 = (((lhs.m30 * rhs.m02) + (lhs.m31 * rhs.m12)) + (lhs.m32 * rhs.m22)) + (lhs.m33 * rhs.m32), m33 = (((lhs.m30 * rhs.m03) + (lhs.m31 * rhs.m13)) + (lhs.m32 * rhs.m23)) + (lhs.m33 * rhs.m33) };
		}

		public Vector3 MultiplyPoint3x4(Vector3 v)
		{
			Vector3 vector;
			vector.x = (((this.m00 * v.x) + (this.m01 * v.y)) + (this.m02 * v.z)) + this.m03;
			vector.y = (((this.m10 * v.x) + (this.m11 * v.y)) + (this.m12 * v.z)) + this.m13;
			vector.z = (((this.m20 * v.x) + (this.m21 * v.y)) + (this.m22 * v.z)) + this.m23;
			return vector;
		}

		public static Matrix4x4 Inverse(Matrix4x4 m)
		{
			Matrix4x4 matrixx;
			InternalInvert(ref m, out matrixx);
			return matrixx;
		}

		internal static bool Invert(Matrix4x4 inMatrix, out Matrix4x4 dest)
		{
			return InternalInvert(ref inMatrix, out dest);
		}

		public Matrix4x4 inverse
		{
			get
			{
				return Inverse(this);
			}
		}

		static bool InternalInvert(ref Matrix4x4 matrix, out Matrix4x4 result)
		{
			//                                       -1
			// If you have matrix M, inverse Matrix M   can compute
			//
			//     -1       1      
			//    M   = --------- A
			//            det(M)
			//
			// A is adjugate (adjoint) of M, where,
			//
			//      T
			// A = C
			//
			// C is Cofactor matrix of M, where,
			//           i + j
			// C   = (-1)      * det(M  )
			//  ij                    ij
			//
			//     [ a b c d ]
			// M = [ e f g h ]
			//     [ i j k l ]
			//     [ m n o p ]
			//
			// First Row
			//           2 | f g h |
			// C   = (-1)  | j k l | = + ( f ( kp - lo ) - g ( jp - ln ) + h ( jo - kn ) )
			//  11         | n o p |
			//
			//           3 | e g h |
			// C   = (-1)  | i k l | = - ( e ( kp - lo ) - g ( ip - lm ) + h ( io - km ) )
			//  12         | m o p |
			//
			//           4 | e f h |
			// C   = (-1)  | i j l | = + ( e ( jp - ln ) - f ( ip - lm ) + h ( in - jm ) )
			//  13         | m n p |
			//
			//           5 | e f g |
			// C   = (-1)  | i j k | = - ( e ( jo - kn ) - f ( io - km ) + g ( in - jm ) )
			//  14         | m n o |
			//
			// Second Row
			//           3 | b c d |
			// C   = (-1)  | j k l | = - ( b ( kp - lo ) - c ( jp - ln ) + d ( jo - kn ) )
			//  21         | n o p |
			//
			//           4 | a c d |
			// C   = (-1)  | i k l | = + ( a ( kp - lo ) - c ( ip - lm ) + d ( io - km ) )
			//  22         | m o p |
			//
			//           5 | a b d |
			// C   = (-1)  | i j l | = - ( a ( jp - ln ) - b ( ip - lm ) + d ( in - jm ) )
			//  23         | m n p |
			//
			//           6 | a b c |
			// C   = (-1)  | i j k | = + ( a ( jo - kn ) - b ( io - km ) + c ( in - jm ) )
			//  24         | m n o |
			//
			// Third Row
			//           4 | b c d |
			// C   = (-1)  | f g h | = + ( b ( gp - ho ) - c ( fp - hn ) + d ( fo - gn ) )
			//  31         | n o p |
			//
			//           5 | a c d |
			// C   = (-1)  | e g h | = - ( a ( gp - ho ) - c ( ep - hm ) + d ( eo - gm ) )
			//  32         | m o p |
			//
			//           6 | a b d |
			// C   = (-1)  | e f h | = + ( a ( fp - hn ) - b ( ep - hm ) + d ( en - fm ) )
			//  33         | m n p |
			//
			//           7 | a b c |
			// C   = (-1)  | e f g | = - ( a ( fo - gn ) - b ( eo - gm ) + c ( en - fm ) )
			//  34         | m n o |
			//
			// Fourth Row
			//           5 | b c d |
			// C   = (-1)  | f g h | = - ( b ( gl - hk ) - c ( fl - hj ) + d ( fk - gj ) )
			//  41         | j k l |
			//
			//           6 | a c d |
			// C   = (-1)  | e g h | = + ( a ( gl - hk ) - c ( el - hi ) + d ( ek - gi ) )
			//  42         | i k l |
			//
			//           7 | a b d |
			// C   = (-1)  | e f h | = - ( a ( fl - hj ) - b ( el - hi ) + d ( ej - fi ) )
			//  43         | i j l |
			//
			//           8 | a b c |
			// C   = (-1)  | e f g | = + ( a ( fk - gj ) - b ( ek - gi ) + c ( ej - fi ) )
			//  44         | i j k |
			//
			// Cost of operation
			// 53 adds, 104 muls, and 1 div.
			float a = matrix.m00, b = matrix.m01, c = matrix.m02, d = matrix.m03;
			float e = matrix.m10, f = matrix.m11, g = matrix.m12, h = matrix.m13;
			float i = matrix.m20, j = matrix.m21, k = matrix.m22, l = matrix.m23;
			float m = matrix.m30, n = matrix.m31, o = matrix.m32, p = matrix.m33;

			float kp_lo = k * p - l * o;
			float jp_ln = j * p - l * n;
			float jo_kn = j * o - k * n;
			float ip_lm = i * p - l * m;
			float io_km = i * o - k * m;
			float in_jm = i * n - j * m;

			float a11 = +(f * kp_lo - g * jp_ln + h * jo_kn);
			float a12 = -(e * kp_lo - g * ip_lm + h * io_km);
			float a13 = +(e * jp_ln - f * ip_lm + h * in_jm);
			float a14 = -(e * jo_kn - f * io_km + g * in_jm);

			float det = a * a11 + b * a12 + c * a13 + d * a14;

			if (Math.Abs(det) < float.Epsilon) {
				result = new Matrix4x4(float.NaN, float.NaN, float.NaN, float.NaN,
											  float.NaN, float.NaN, float.NaN, float.NaN,
											  float.NaN, float.NaN, float.NaN, float.NaN,
											  float.NaN, float.NaN, float.NaN, float.NaN);
				return false;
			}

			float invDet = 1.0f / det;

			result.m00 = a11 * invDet;
			result.m10 = a12 * invDet;
			result.m20 = a13 * invDet;
			result.m30 = a14 * invDet;

			result.m01 = -(b * kp_lo - c * jp_ln + d * jo_kn) * invDet;
			result.m11 = +(a * kp_lo - c * ip_lm + d * io_km) * invDet;
			result.m21 = -(a * jp_ln - b * ip_lm + d * in_jm) * invDet;
			result.m31 = +(a * jo_kn - b * io_km + c * in_jm) * invDet;

			float gp_ho = g * p - h * o;
			float fp_hn = f * p - h * n;
			float fo_gn = f * o - g * n;
			float ep_hm = e * p - h * m;
			float eo_gm = e * o - g * m;
			float en_fm = e * n - f * m;

			result.m02 = +(b * gp_ho - c * fp_hn + d * fo_gn) * invDet;
			result.m12 = -(a * gp_ho - c * ep_hm + d * eo_gm) * invDet;
			result.m22 = +(a * fp_hn - b * ep_hm + d * en_fm) * invDet;
			result.m32 = -(a * fo_gn - b * eo_gm + c * en_fm) * invDet;

			float gl_hk = g * l - h * k;
			float fl_hj = f * l - h * j;
			float fk_gj = f * k - g * j;
			float el_hi = e * l - h * i;
			float ek_gi = e * k - g * i;
			float ej_fi = e * j - f * i;

			result.m03 = -(b * gl_hk - c * fl_hj + d * fk_gj) * invDet;
			result.m13 = +(a * gl_hk - c * el_hi + d * ek_gi) * invDet;
			result.m23 = -(a * fl_hj - b * el_hi + d * ej_fi) * invDet;
			result.m33 = +(a * fk_gj - b * ek_gi + c * ej_fi) * invDet;

			return true;
		}

		public static Matrix4x4 Scale(Vector3 vector)
		{
			Matrix4x4 m;
			m.m00 = vector.x; m.m01 = 0.0f; m.m02 = 0.0f; m.m03 = 0.0f;
			m.m10 = 0.0f; m.m11 = vector.y; m.m12 = 0.0f; m.m13 = 0.0f;
			m.m20 = 0.0f; m.m21 = 0.0f; m.m22 = vector.z; m.m23 = 0.0f;
			m.m30 = 0.0f; m.m31 = 0.0f; m.m32 = 0.0f; m.m33 = 1.0f;
			return m;
		}

		public static Matrix4x4 Translate(Vector3 vector)
		{
			Matrix4x4 m;
			m.m00 = 1.0f; m.m01 = 0.0f; m.m02 = 0.0f; m.m03 = vector.x;
			m.m10 = 0.0f; m.m11 = 1.0f; m.m12 = 0.0f; m.m13 = vector.y;
			m.m20 = 0.0f; m.m21 = 0.0f; m.m22 = 1.0f; m.m23 = vector.z;
			m.m30 = 0.0f; m.m31 = 0.0f; m.m32 = 0.0f; m.m33 = 1.0f;
			return m;
		}

		public static Matrix4x4 Rotate(Quaternion q)
		{
			float x = q.x * 2.0f;
			float y = q.y * 2.0f;
			float z = q.z * 2.0f;
			float xx = q.x * x;
			float yy = q.y * y;
			float zz = q.z * z;
			float xy = q.x * y;
			float xz = q.x * z;
			float yz = q.y * z;
			float wx = q.w * x;
			float wy = q.w * y;
			float wz = q.w * z;

			Matrix4x4 m;
			m.m00 = 1.0f - (yy + zz); m.m10 = xy + wz; m.m20 = xz - wy; m.m30 = 0.0F;
			m.m01 = xy - wz; m.m11 = 1.0f - (xx + zz); m.m21 = yz + wx; m.m31 = 0.0F;
			m.m02 = xz + wy; m.m12 = yz - wx; m.m22 = 1.0f - (xx + yy); m.m32 = 0.0F;
			m.m03 = 0.0F; m.m13 = 0.0F; m.m23 = 0.0F; m.m33 = 1.0F;
			return m;
		}

		public static Matrix4x4 TRS(Vector3 pos, Quaternion rot, Vector3 scale)
		{
			var tx = Translate(pos);
			var rx = Rotate(rot);
			var sx = Scale(scale);
			var final = tx * rx * sx;
			return final;
		}
	}

	public struct BoneWeight
	{
		private float m_Weight0;
		private float m_Weight1;
		private float m_Weight2;
		private float m_Weight3;
		private int m_BoneIndex0;
		private int m_BoneIndex1;
		private int m_BoneIndex2;
		private int m_BoneIndex3;

		public float weight0
		{
			get
			{
				return this.m_Weight0;
			}
			set
			{
				this.m_Weight0 = value;
			}
		}

		public float weight1
		{
			get
			{
				return this.m_Weight1;
			}
			set
			{
				this.m_Weight1 = value;
			}
		}

		public float weight2
		{
			get
			{
				return this.m_Weight2;
			}
			set
			{
				this.m_Weight2 = value;
			}
		}

		public float weight3
		{
			get
			{
				return this.m_Weight3;
			}
			set
			{
				this.m_Weight3 = value;
			}
		}

		public int boneIndex0
		{
			get
			{
				return this.m_BoneIndex0;
			}
			set
			{
				this.m_BoneIndex0 = value;
			}
		}
		public int boneIndex1
		{
			get
			{
				return this.m_BoneIndex1;
			}
			set
			{
				this.m_BoneIndex1 = value;
			}
		}

		public int boneIndex2
		{
			get
			{
				return this.m_BoneIndex2;
			}
			set
			{
				this.m_BoneIndex2 = value;
			}
		}
		public int boneIndex3
		{
			get
			{
				return this.m_BoneIndex3;
			}
			set
			{
				this.m_BoneIndex3 = value;
			}
		}

		public override int GetHashCode()
		{
			return (((((((this.boneIndex0.GetHashCode() ^ (this.boneIndex1.GetHashCode() << 2)) ^ (this.boneIndex2.GetHashCode() >> 2)) ^ (this.boneIndex3.GetHashCode() >> 1)) ^ (this.weight0.GetHashCode() << 5)) ^ (this.weight1.GetHashCode() << 4)) ^ (this.weight2.GetHashCode() >> 4)) ^ (this.weight3.GetHashCode() >> 3));
		}

		public override bool Equals(object other)
		{
			Vector4 vector;
			if (!(other is BoneWeight)) {
				return false;
			}

			BoneWeight weight = (BoneWeight)other;
			if ((this.boneIndex0.Equals(weight.boneIndex0) && this.boneIndex1.Equals(weight.boneIndex1)) && (this.boneIndex2.Equals(weight.boneIndex2) && this.boneIndex3.Equals(weight.boneIndex3))) {
				vector = new Vector4(this.weight0, this.weight1, this.weight2, this.weight3);
				return vector.Equals(new Vector4(weight.weight0, weight.weight1, weight.weight2, weight.weight3));
			} else {
				return false;
			}
		}

		public static bool operator ==(BoneWeight lhs, BoneWeight rhs)
		{
			return ((((lhs.boneIndex0 == rhs.boneIndex0) && (lhs.boneIndex1 == rhs.boneIndex1)) && ((lhs.boneIndex2 == rhs.boneIndex2) && (lhs.boneIndex3 == rhs.boneIndex3))) && (new Vector4(lhs.weight0, lhs.weight1, lhs.weight2, lhs.weight3) == new Vector4(rhs.weight0, rhs.weight1, rhs.weight2, rhs.weight3)));
		}

		public static bool operator !=(BoneWeight lhs, BoneWeight rhs)
		{
			return !(lhs == rhs);
		}
	}


	public static class Random
	{
		static System.Random rand = new System.Random();

		public static float Range( float min, float max )
		{
			float t = (float)rand.NextDouble();
			return Mathf.Lerp( min, max, t );
		}
	}

	public static class Mathf
	{
		public static float Deg2Rad = (float)( Math.PI / 180.0 );
		public static float Rad2Deg = (float)( 180.0 / Math.PI );
		public static readonly float PI = (float)Math.PI;

		public static float Max( float x, float y )
		{
			return Math.Max( x, y );
		}

		public static float Min( float x, float y )
		{
			return Math.Min( x, y );
		}

		public static float Sin( float x )
		{
			return (float)Math.Sin( (double)x );
		}

		public static float Cos( float x )
		{
			return (float)Math.Cos( (double)x );
		}

		public static float Acos( float x )
		{
			return (float)Math.Acos( (double)x );
		}

		public static float Abs( float x )
		{
			return (float)Math.Abs( x );
		}

		public static float Pow( float x, float y )
		{
			return (float)Math.Pow( (float)x, (float)y );
		}

		public static float Ceil( float x)
		{
			return (float)Math.Ceiling( (double)x );
		}

		public static float Clamp( float x, float min, float max )
		{
			return Math.Max( Math.Min( x, max ), min );
		}

		public static float Clamp01( float x )
		{
			return Math.Max( Math.Min( x, 1.0f ), 0.0f );
		}

		public static float Floor( float x)
		{
			return (float)Math.Floor( (double)x );
		}

		public static float Lerp( float a, float b, float t )
		{
			t = Clamp01( t );
			return a + ( b - a ) * t;
		}

		public static float LerpAngle( float a, float b, float t )
		{
			float a_rad = a * Deg2Rad;
			float b_rad = b * Deg2Rad;
			float lerp_rad = Lerp( a_rad, b_rad, t );

			float two_pi = (float)( Math.PI * 2.0 );
			while( lerp_rad < 0.0f ) {
				lerp_rad += two_pi;
			}
			while( lerp_rad >= two_pi ) {
				lerp_rad -= two_pi;
			}

			float res_deg = lerp_rad * Rad2Deg;
			return res_deg;
		}

		public static float Round(float v)
		{
			return (float)Math.Round( (double)v );
		}

		public static int Sign(float x)
		{
			return Math.Sign( (double)x );
		}
	}
}
