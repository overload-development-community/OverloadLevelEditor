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

public static class OpenTKHelpers
{
	public static UnityEngine.Vector4 ToUnity(this OpenTK.Vector4 v)
	{
		return new UnityEngine.Vector4(v.X, v.Y, v.Z, v.W);
	}

	public static UnityEngine.Vector3 ToUnity(this OpenTK.Vector3 v)
	{
		return new UnityEngine.Vector3(v.X, v.Y, v.Z);
	}

	public static UnityEngine.Vector2 ToUnity(this OpenTK.Vector2 v)
	{
		return new UnityEngine.Vector2(v.X, v.Y);
	}

	public static UnityEngine.Quaternion ToUnity(this OpenTK.Quaternion q)
	{
		return new UnityEngine.Quaternion(q.X, q.Y, q.Z, q.W);
	}

	public static UnityEngine.Matrix4x4 ToUnity(this OpenTK.Matrix4 m)
	{
		var res = new UnityEngine.Matrix4x4();
		for (int r = 0; r < 4; ++r) {
			for (int c = 0; c < 4; ++c) {
				res[r, c] = m[r, c];
			}
		}

		return res;
	}

	public static OpenTK.Vector4 ToOpenTK(this UnityEngine.Vector4 v)
	{
		return new OpenTK.Vector4(v.x, v.y, v.z, v.w);
	}

	public static OpenTK.Vector3 ToOpenTK(this UnityEngine.Vector3 v)
	{
		return new OpenTK.Vector3(v.x, v.y, v.z);
	}

	public static OpenTK.Vector2 ToOpenTK(this UnityEngine.Vector2 v)
	{
		return new OpenTK.Vector2(v.x, v.y);
	}

	public static OpenTK.Quaternion ToOpenTK(this UnityEngine.Quaternion q)
	{
		return new OpenTK.Quaternion(q.x, q.y, q.z, q.w);
	}

	public static OpenTK.Matrix4 ToOpenTK(this UnityEngine.Matrix4x4 m)
	{
		var res = new OpenTK.Matrix4();
		for (int r = 0; r < 4; ++r) {
			for (int c = 0; c < 4; ++c) {
				res[r, c] = m[r, c];
			}
		}
		return res;
	}
}

namespace OpenTK
{
	public struct Vector4
	{
		public float X;
		public float Y;
		public float Z;
		public float W;

		public Vector4(float x, float y, float z, float w)
		{
			this.X = x;
			this.Y = y;
			this.Z = z;
			this.W = w;
		}

		public float this[int key]
		{
			get
			{
				switch (key) {
					case 0: return this.X;
					case 1: return this.Y;
					case 2: return this.Z;
					case 3: return this.W;
					default: return 0.0f;
				}
			}
			set
			{
				switch (key) {
					case 0: this.X = value; break;
					case 1: this.Y = value; break;
					case 2: this.Z = value; break;
					case 3: this.W = value; break;
					default: break;
				}
			}
		}

		public Vector3 Xyz
		{
			get { return new Vector3(this.X, this.Y, this.Z); }
			set
			{
				this.X = value.X;
				this.Y = value.Y;
				this.Z = value.Z;
			}
		}

		public static Vector4 Transform(Vector4 vec, Matrix4 mat)
		{
			return new Vector4((((vec.X * mat.Row0.X) + (vec.Y * mat.Row1.X)) + (vec.Z * mat.Row2.X)) + (vec.W * mat.Row3.X), (((vec.X * mat.Row0.Y) + (vec.Y * mat.Row1.Y)) + (vec.Z * mat.Row2.Y)) + (vec.W * mat.Row3.Y), (((vec.X * mat.Row0.Z) + (vec.Y * mat.Row1.Z)) + (vec.Z * mat.Row2.Z)) + (vec.W * mat.Row3.Z), (((vec.X * mat.Row0.W) + (vec.Y * mat.Row1.W)) + (vec.Z * mat.Row2.W)) + (vec.W * mat.Row3.W));
		}

		public static Vector4 Lerp(Vector4 a, Vector4 b, float blend)
		{
			var res = new Vector4();
			res.X = (blend * (b.X - a.X)) + a.X;
			res.Y = (blend * (b.Y - a.Y)) + a.Y;
			res.Z = (blend * (b.Z - a.Z)) + a.Z;
			res.W = (blend * (b.W - a.W)) + a.W;
			return res;
		}

		public bool Equals(Vector4 other)
		{
			return ((((this.X == other.X) && (this.Y == other.Y)) && (this.Z == other.Z)) && (this.W == other.W));
		}

		public override bool Equals(object obj)
		{
			return ((obj is Vector4) && this.Equals((Vector4)obj));
		}

		public override int GetHashCode()
		{
			return (((this.X.GetHashCode() ^ this.Y.GetHashCode()) ^ this.Z.GetHashCode()) ^ this.W.GetHashCode());
		}

		public override string ToString()
		{
			return string.Format("[{0}, {1}, {2}, {3}]", this.X, this.Y, this.Z, this.W);
		}

		public static bool operator ==(Vector4 left, Vector4 right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(Vector4 left, Vector4 right)
		{
			return !left.Equals(right);
		}

		public static Vector4 operator *(Vector4 vec, Vector4 scale)
		{
			vec.X *= scale.X;
			vec.Y *= scale.Y;
			vec.Z *= scale.Z;
			vec.W *= scale.W;
			return vec;
		}

		public static Vector4 operator *(Vector4 vec, float scale)
		{
			vec.X *= scale;
			vec.Y *= scale;
			vec.Z *= scale;
			vec.W *= scale;
			return vec;
		}

		public static Vector4 operator *(float scale, Vector4 vec)
		{
			vec.X *= scale;
			vec.Y *= scale;
			vec.Z *= scale;
			vec.W *= scale;
			return vec;
		}

		public static Vector4 operator +(Vector4 left, Vector4 right)
		{
			left.X += right.X;
			left.Y += right.Y;
			left.Z += right.Z;
			left.W += right.W;
			return left;
		}

		public static Vector4 operator /(Vector4 vec, float scale)
		{
			float mult = 1f / scale;
			vec.X *= mult;
			vec.Y *= mult;
			vec.Z *= mult;
			vec.W *= mult;
			return vec;
		}

		public static Vector4 operator -(Vector4 left, Vector4 right)
		{
			left.X -= right.X;
			left.Y -= right.Y;
			left.Z -= right.Z;
			left.W -= right.W;
			return left;
		}

		public static Vector4 operator -(Vector4 vec)
		{
			vec.X = -vec.X;
			vec.Y = -vec.Y;
			vec.Z = -vec.Z;
			vec.W = -vec.W;
			return vec;
		}

		public static readonly Vector4 Zero = new Vector4(0.0f, 0.0f, 0.0f, 0.0f);
		public static readonly Vector4 UnitW = new Vector4(0.0f, 0.0f, 0.0f, 1.0f);
	}

	public struct Vector3
	{
		public float X;
		public float Y;
		public float Z;

		public Vector3(float x, float y, float z)
		{
			this.X = x;
			this.Y = y;
			this.Z = z;
		}

		public Vector3(Vector4 v)
		{
			this.X = v.X;
			this.Y = v.Y;
			this.Z = v.Z;
		}

		public float Length
		{
			get { return (float)Math.Sqrt(this.X * this.X + this.Y * this.Y + this.Z * this.Z); }
		}

		public float LengthSquared
		{
			get { return (((this.X * this.X) + (this.Y * this.Y)) + (this.Z * this.Z)); }
		}

		public static float Dot(Vector3 v1, Vector3 v2)
		{
			return v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z;
		}

		public static Vector3 Transform(Vector3 vec, Matrix4 mat)
		{
			Vector4 v4 = new Vector4(vec.X, vec.Y, vec.Z, 1.0f);
			Vector4 res = Vector4.Transform(v4, mat);
			return res.Xyz;
		}

		public static Vector3 Lerp(Vector3 a, Vector3 b, float blend)
		{
			var res = new Vector3();
			res.X = (blend * (b.X - a.X)) + a.X;
			res.Y = (blend * (b.Y - a.Y)) + a.Y;
			res.Z = (blend * (b.Z - a.Z)) + a.Z;
			return res;
		}

		public void Normalize()
		{
			float scale = 1.0f / this.Length;
			this.X *= scale;
			this.Y *= scale;
			this.Z *= scale;
		}

		public static Vector3 Normalize(Vector3 vec)
		{
			float scale = 1.0f / vec.Length;
			vec.X *= scale;
			vec.Y *= scale;
			vec.Z *= scale;
			return vec;
		}

		public Vector3 Normalized()
		{
			Vector3 v = this;
			v.Normalize();
			return v;
		}

		public static Vector3 Cross(Vector3 left, Vector3 right)
		{
			Vector3 result;
			Cross(ref left, ref right, out result);
			return result;
		}

		public static void Cross(ref Vector3 left, ref Vector3 right, out Vector3 result)
		{
			result = new Vector3((left.Y * right.Z) - (left.Z * right.Y), (left.Z * right.X) - (left.X * right.Z), (left.X * right.Y) - (left.Y * right.X));
		}

		public static Vector3 TransformNormal(Vector3 norm, Matrix4 mat)
		{
			mat.Invert();
			return TransformNormalInverse(norm, mat);
		}

		public static void TransformNormal(ref Vector3 norm, ref Matrix4 mat, out Vector3 result)
		{
			Matrix4 Inverse = Matrix4.Invert(mat);
			TransformNormalInverse(ref norm, ref Inverse, out result);
		}

		public static Vector3 TransformNormalInverse(Vector3 norm, Matrix4 invMat)
		{
			Vector3 n;
			n.X = Dot(norm, new Vector3(invMat.Row0));
			n.Y = Dot(norm, new Vector3(invMat.Row1));
			n.Z = Dot(norm, new Vector3(invMat.Row2));
			return n;
		}

		public static void TransformNormalInverse(ref Vector3 norm, ref Matrix4 invMat, out Vector3 result)
		{
			result.X = ((norm.X * invMat.Row0.X) + (norm.Y * invMat.Row0.Y)) + (norm.Z * invMat.Row0.Z);
			result.Y = ((norm.X * invMat.Row1.X) + (norm.Y * invMat.Row1.Y)) + (norm.Z * invMat.Row1.Z);
			result.Z = ((norm.X * invMat.Row2.X) + (norm.Y * invMat.Row2.Y)) + (norm.Z * invMat.Row2.Z);
		} 

		public bool Equals(Vector3 other)
		{
			return (((this.X == other.X) && (this.Y == other.Y)) && (this.Z == other.Z));
		}

		public override bool Equals(object obj)
		{
			return ((obj is Vector3) && this.Equals((Vector3)obj));
		}

		public override int GetHashCode()
		{
			return ((this.X.GetHashCode() ^ this.Y.GetHashCode()) ^ this.Z.GetHashCode());
		}

		public override string ToString()
		{
			return string.Format("[{0}, {1}, {2}]", this.X, this.Y, this.Z);
		}

		public static bool operator ==(Vector3 left, Vector3 right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(Vector3 left, Vector3 right)
		{
			return !left.Equals(right);
		}

		public static Vector3 operator +(Vector3 left, Vector3 right)
		{
			left.X += right.X;
			left.Y += right.Y;
			left.Z += right.Z;
			return left;
		}

		public static Vector3 operator /(Vector3 vec, float scale)
		{
			float mult = 1f / scale;
			vec.X *= mult;
			vec.Y *= mult;
			vec.Z *= mult;
			return vec;
		}

		public static Vector3 operator *(Vector3 vec, Vector3 scale)
		{
			vec.X *= scale.X;
			vec.Y *= scale.Y;
			vec.Z *= scale.Z;
			return vec;
		}

		public static Vector3 operator *(Vector3 vec, float scale)
		{
			vec.X *= scale;
			vec.Y *= scale;
			vec.Z *= scale;
			return vec;
		}

		public static Vector3 operator *(float scale, Vector3 vec)
		{
			vec.X *= scale;
			vec.Y *= scale;
			vec.Z *= scale;
			return vec;
		}
		
		public static Vector3 operator -(Vector3 left, Vector3 right)
		{
			left.X -= right.X;
			left.Y -= right.Y;
			left.Z -= right.Z;
			return left;
		}

		public static Vector3 operator -(Vector3 vec)
		{
			vec.X = -vec.X;
			vec.Y = -vec.Y;
			vec.Z = -vec.Z;
			return vec;
		}

		public float this[int key]
		{
			get
			{
				switch (key) {
					case 0: return this.X;
					case 1: return this.Y;
					case 2: return this.Z;
					default: return 0.0f;
				}
			}
			set
			{
				switch (key) {
					case 0: this.X = value; break;
					case 1: this.Y = value; break;
					case 2: this.Z = value; break;
					default: break;
				}
			}
		}

		public static readonly Vector3 One = new Vector3(1.0f, 1.0f, 1.0f);
		public static readonly Vector3 Zero = new Vector3(0.0f, 0.0f, 0.0f);
		public static readonly Vector3 UnitX = new Vector3(1.0f, 0.0f, 0.0f);
		public static readonly Vector3 UnitY = new Vector3(0.0f, 1.0f, 0.0f);
		public static readonly Vector3 UnitZ = new Vector3(0.0f, 0.0f, 1.0f);
	}

	public struct Vector2
	{
		public float X;
		public float Y;

		public Vector2(float x, float y)
		{
			this.X = x;
			this.Y = y;
		}

		public float this[int key]
		{
			get
			{
				switch (key) {
					case 0: return this.X;
					case 1: return this.Y;
					default: return 0.0f;
				}
			}
			set
			{
				switch (key) {
					case 0: this.X = value; break;
					case 1: this.Y = value; break;
					default: break;
				}
			}
		}

		public static Vector2 Lerp(Vector2 a, Vector2 b, float blend)
		{
			var res = new Vector2();
			res.X = (blend * (b.X - a.X)) + a.X;
			res.Y = (blend * (b.Y - a.Y)) + a.Y;
			return res;
		}

		public bool Equals(Vector2 other)
		{
			return ((this.X == other.X) && (this.Y == other.Y));
		}

		public override bool Equals(object obj)
		{
			return ((obj is Vector2) && this.Equals((Vector2)obj));
		}

		public override string ToString()
		{
			return string.Format("[{0}, {1}]", this.X, this.Y);
		}

		public override int GetHashCode()
		{
			return (this.X.GetHashCode() ^ this.Y.GetHashCode());
		}

		public static bool operator ==(Vector2 left, Vector2 right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(Vector2 left, Vector2 right)
		{
			return !left.Equals(right);
		}

		public static Vector2 operator -( Vector2 left, Vector2 right )
		{
			left.X -= right.X;
			left.Y -= right.Y;
			return left;
		}

        public static Vector2 operator +( Vector2 left, Vector2 right )
        {
            left.X += right.X;
            left.Y += right.Y;
            return left;
        }

        public static Vector2 operator -( Vector2 vec )
		{
			vec.X = -vec.X;
			vec.Y = -vec.Y;
			return vec;
		}

		public float Length
		{
			get { return (float)Math.Sqrt( this.X * this.X + this.Y * this.Y ); }
		}

		public float LengthSquared
		{
			get { return ( ( ( this.X * this.X ) + ( this.Y * this.Y ) ) ); }
		}

		public static readonly Vector2 Zero = new Vector2(0.0f, 0.0f);
	}

	public struct Quaternion
	{
		private Vector3 xyz;
		private float w;

		public static readonly Quaternion Identity;

		static Quaternion()
		{
			Identity = new Quaternion(0f, 0f, 0f, 1f);
		}

		public Quaternion(Vector3 v, float w)
		{
			this.xyz = v;
			this.w = w;
		}

		public Quaternion(float x, float y, float z, float w)
			: this(new Vector3(x, y, z), w)
		{
		}

		public static Quaternion Conjugate(Quaternion q)
		{
			return new Quaternion(-q.Xyz, q.W);
		}

		public static void Conjugate(ref Quaternion q, out Quaternion result)
		{
			result = new Quaternion(-q.Xyz, q.W);
		}

		public static Quaternion FromAxisAngle(Vector3 axis, float angle)
		{
			if (axis.LengthSquared == 0f) {
				return Identity;
			}
			Quaternion result = Identity;
			angle *= 0.5f;
			axis.Normalize();
			result.Xyz = (Vector3)(axis * ((float)Math.Sin((double)angle)));
			result.W = (float)Math.Cos((double)angle);
			return Normalize(result);
		}

		public void Normalize()
		{
			float scale = 1f / this.Length;
			this.Xyz = (Vector3)(this.Xyz * scale);
			this.W *= scale;
		}

		public static Quaternion Normalize(Quaternion q)
		{
			Quaternion result;
			Normalize(ref q, out result);
			return result;
		}

		public static void Normalize(ref Quaternion q, out Quaternion result)
		{
			float scale = 1f / q.Length;
			result = new Quaternion((Vector3)(q.Xyz * scale), q.W * scale);
		}

		public Quaternion Normalized()
		{
			Quaternion q = this;
			q.Normalize();
			return q;
		}

		public static Quaternion Multiply(Quaternion left, Quaternion right)
		{
			Quaternion result;
			Multiply(ref left, ref right, out result);
			return result;
		}

		public static Quaternion Multiply(Quaternion quaternion, float scale)
		{
			return new Quaternion(quaternion.X * scale, quaternion.Y * scale, quaternion.Z * scale, quaternion.W * scale);
		}

		public static void Multiply(ref Quaternion quaternion, float scale, out Quaternion result)
		{
			result = new Quaternion(quaternion.X * scale, quaternion.Y * scale, quaternion.Z * scale, quaternion.W * scale);
		}

		public static void Multiply(ref Quaternion left, ref Quaternion right, out Quaternion result)
		{
			result = new Quaternion(((Vector3)((right.W * left.Xyz) + (left.W * right.Xyz))) + Vector3.Cross(left.Xyz, right.Xyz), (left.W * right.W) - Vector3.Dot(left.Xyz, right.Xyz));
		}

		public static Quaternion operator +(Quaternion left, Quaternion right)
		{
			left.Xyz += right.Xyz;
			left.W += right.W;
			return left;
		}

		public static bool operator ==(Quaternion left, Quaternion right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(Quaternion left, Quaternion right)
		{
			return !left.Equals(right);
		}

		public static Quaternion operator *(Quaternion left, Quaternion right)
		{
			Multiply(ref left, ref right, out left);
			return left;
		}

		public static Quaternion operator *(Quaternion quaternion, float scale)
		{
			Multiply(ref quaternion, scale, out quaternion);
			return quaternion;
		}

		public static Quaternion operator *(float scale, Quaternion quaternion)
		{
			return new Quaternion(quaternion.X * scale, quaternion.Y * scale, quaternion.Z * scale, quaternion.W * scale);
		}

		public static Quaternion operator -(Quaternion left, Quaternion right)
		{
			left.Xyz -= right.Xyz;
			left.W -= right.W;
			return left;
		}

		public Vector4 ToAxisAngle()
		{
			Quaternion q = this;
			if (Math.Abs(q.W) > 1f) {
				q.Normalize();
			}
			Vector4 result = new Vector4
			{
				W = 2f * ((float)Math.Acos((double)q.W))
			};
			float den = (float)Math.Sqrt(1.0 - (q.W * q.W));
			if (den > 0.0001f) {
				result.Xyz = (Vector3)(q.Xyz / den);
				return result;
			}
			result.Xyz = Vector3.UnitX;
			return result;
		}

		public void ToAxisAngle(out Vector3 axis, out float angle)
		{
			Vector4 result = this.ToAxisAngle();
			axis = result.Xyz;
			angle = result.W;
		}

		public float Length
		{
			get
			{
				return (float)Math.Sqrt((double)((this.W * this.W) + this.Xyz.LengthSquared));
			}
		}

		public float LengthSquared
		{
			get
			{
				return ((this.W * this.W) + this.Xyz.LengthSquared);
			}
		}

		public float W
		{
			get { return this.w; }
			set { this.w = value; }
		}

		public float X
		{
			get { return this.xyz.X; }
			set { this.xyz.X = value; }
		}

		public float Y
		{
			get { return this.xyz.Y; }
			set { this.xyz.Y = value; }
		}

		public float Z
		{
			get { return this.xyz.Z; }
			set { this.xyz.Z = value; }
		}

		public Vector3 Xyz
		{
			get { return this.xyz; }
			set { this.xyz = value; }
		}

		public override string ToString()
		{
			return string.Format("V: {0}, W: {1}", this.Xyz, this.W);
		}

		public override int GetHashCode()
		{
			return (this.Xyz.GetHashCode() ^ this.W.GetHashCode());
		}

		public bool Equals(Quaternion other)
		{
			return ((this.Xyz == other.Xyz) && (this.W == other.W));
		}

		public override bool Equals(object other)
		{
			return ((other is Quaternion) && (this == ((Quaternion)other)));
		}
	}

	public struct Matrix4
	{
		public Vector4 Row0;
		public Vector4 Row1;
		public Vector4 Row2;
		public Vector4 Row3;

		public Matrix4(Vector4 r0, Vector4 r1, Vector4 r2, Vector4 r3)
		{
			this.Row0 = r0;
			this.Row1 = r1;
			this.Row2 = r2;
			this.Row3 = r3;
		}

		public float this[int row, int col]
		{
			get
			{
				switch (row) {
					case 0: return Row0[col];
					case 1: return Row1[col];
					case 2: return Row2[col];
					case 3: return Row3[col];
					default: return 0.0f;
				}
			}
			set
			{
				switch (row) {
					case 0: Row0[col] = value; break;
					case 1: Row1[col] = value; break;
					case 2: Row2[col] = value; break;
					case 3: Row3[col] = value; break;
					default: break;
				}
			}
		}

		public Vector4 Column0
		{
			get { return new Vector4(Row0.X, Row1.X, Row2.X, Row3.X); }
			set
			{
				Row0.X = value.X;
				Row1.X = value.Y;
				Row2.X = value.Z;
				Row3.X = value.W;
			}
		}

		public Vector4 Column1
		{
			get { return new Vector4(Row0.Y, Row1.Y, Row2.Y, Row3.Y); }
			set
			{
				Row0.Y = value.X;
				Row1.Y = value.Y;
				Row2.Y = value.Z;
				Row3.Y = value.W;
			}
		}

		public Vector4 Column2
		{
			get { return new Vector4(Row0.Z, Row1.Z, Row2.Z, Row3.Z); }
			set
			{
				Row0.Z = value.X;
				Row1.Z = value.Y;
				Row2.Z = value.Z;
				Row3.Z = value.W;
			}
		}

		public Vector4 Column3
		{
			get { return new Vector4(Row0.W, Row1.W, Row2.W, Row3.W); }
			set
			{
				Row0.W = value.X;
				Row1.W = value.Y;
				Row2.W = value.Z;
				Row3.W = value.W;
			}
		}

		public float M11
		{
			get { return this.Row0.X; }
			set { this.Row0.X = value; }
		}

		public float M12
		{
			get { return this.Row0.Y; }
			set { this.Row0.Y = value; }
		}

		public float M13
		{
			get { return this.Row0.Z; }
			set { this.Row0.Z = value; }
		}

		public float M14
		{
			get { return this.Row0.W; }
			set { this.Row0.W = value; }
		}

		public float M21
		{
			get { return this.Row1.X; }
			set { this.Row1.X = value; }
		}

		public float M22
		{
			get { return this.Row1.Y; }
			set { this.Row1.Y = value; }
		}

		public float M23
		{
			get { return this.Row1.Z; }
			set { this.Row1.Z = value; }
		}

		public float M24
		{
			get { return this.Row1.W; }
			set { this.Row1.W = value; }
		}

		public float M31
		{
			get { return this.Row2.X; }
			set { this.Row2.X = value; }
		}

		public float M32
		{
			get { return this.Row2.Y; }
			set { this.Row2.Y = value; }
		}

		public float M33
		{
			get { return this.Row2.Z; }
			set { this.Row2.Z = value; }
		}

		public float M34
		{
			get { return this.Row2.W; }
			set { this.Row2.W = value; }
		}

		public float M41
		{
			get { return this.Row3.X; }
			set { this.Row3.X = value; }
		}

		public float M42
		{
			get { return this.Row3.Y; }
			set { this.Row3.Y = value; }
		}

		public float M43
		{
			get { return this.Row3.Z; }
			set { this.Row3.Z = value; }
		}

		public float M44
		{
			get { return this.Row3.W; }
			set { this.Row3.W = value; }
		}


 

		public static Matrix4 CreateRotationX(float angle)
		{
			Matrix4 result;
			CreateRotationX(angle, out result);
			return result;
		}

		public static void CreateRotationX(float angle, out Matrix4 result)
		{
			float cos = (float)Math.Cos((double)angle);
			float sin = (float)Math.Sin((double)angle);
			result = Identity;
			result.Row1.Y = cos;
			result.Row1.Z = sin;
			result.Row2.Y = -sin;
			result.Row2.Z = cos;
		}

		public static Matrix4 CreateRotationY(float angle)
		{
			Matrix4 result;
			CreateRotationY(angle, out result);
			return result;
		}

		public static void CreateRotationY(float angle, out Matrix4 result)
		{
			float cos = (float)Math.Cos((double)angle);
			float sin = (float)Math.Sin((double)angle);
			result = Identity;
			result.Row0.X = cos;
			result.Row0.Z = -sin;
			result.Row2.X = sin;
			result.Row2.Z = cos;
		}

		public static Matrix4 CreateRotationZ(float angle)
		{
			Matrix4 result;
			CreateRotationZ(angle, out result);
			return result;
		}

		public static void CreateRotationZ(float angle, out Matrix4 result)
		{
			float cos = (float)Math.Cos((double)angle);
			float sin = (float)Math.Sin((double)angle);
			result = Identity;
			result.Row0.X = cos;
			result.Row0.Y = sin;
			result.Row1.X = -sin;
			result.Row1.Y = cos;
		}

		public static Matrix4 CreateFromAxisAngle(Vector3 axis, float angle)
		{
			Matrix4 result;
			CreateFromAxisAngle(axis, angle, out result);
			return result;
		}

		public static void CreateFromAxisAngle(Vector3 axis, float angle, out Matrix4 result)
		{
			axis.Normalize();
			float axisX = axis.X;
			float axisY = axis.Y;
			float axisZ = axis.Z;
			float cos = (float)Math.Cos((double)-angle);
			float sin = (float)Math.Sin((double)-angle);
			float t = 1f - cos;
			float tXX = (t * axisX) * axisX;
			float tXY = (t * axisX) * axisY;
			float tXZ = (t * axisX) * axisZ;
			float tYY = (t * axisY) * axisY;
			float tYZ = (t * axisY) * axisZ;
			float tZZ = (t * axisZ) * axisZ;
			float sinX = sin * axisX;
			float sinY = sin * axisY;
			float sinZ = sin * axisZ;
			result.Row0.X = tXX + cos;
			result.Row0.Y = tXY - sinZ;
			result.Row0.Z = tXZ + sinY;
			result.Row0.W = 0f;
			result.Row1.X = tXY + sinZ;
			result.Row1.Y = tYY + cos;
			result.Row1.Z = tYZ - sinX;
			result.Row1.W = 0f;
			result.Row2.X = tXZ - sinY;
			result.Row2.Y = tYZ + sinX;
			result.Row2.Z = tZZ + cos;
			result.Row2.W = 0f;
			result.Row3 = Vector4.UnitW;
		}

		public static Matrix4 CreateTranslation(Vector3 vector)
		{
			Matrix4 result;
			CreateTranslation(vector.X, vector.Y, vector.Z, out result);
			return result;
		}

		public static void CreateTranslation(ref Vector3 vector, out Matrix4 result)
		{
			result = Identity;
			result.Row3.X = vector.X;
			result.Row3.Y = vector.Y;
			result.Row3.Z = vector.Z;
		}

		public static Matrix4 CreateTranslation(float x, float y, float z)
		{
			Matrix4 result;
			CreateTranslation(x, y, z, out result);
			return result;
		}

		public static void CreateTranslation(float x, float y, float z, out Matrix4 result)
		{
			result = Identity;
			result.Row3.X = x;
			result.Row3.Y = y;
			result.Row3.Z = z;
		}

		public static Matrix4 CreateFromQuaternion(Quaternion q)
		{
			Matrix4 result;
			CreateFromQuaternion(ref q, out result);
			return result;
		}

		public static void CreateFromQuaternion(ref Quaternion q, out Matrix4 result)
		{
			Vector3 axis;
			float angle;
			q.ToAxisAngle(out axis, out angle);
			CreateFromAxisAngle(axis, angle, out result);
		}

		public Vector4 ExtractProjection()
		{
			return this.Column3;
		}

		// row_normalise
		// Whether the method should row-normalise (i.e. remove scale from) the Matrix. Pass false if you know it's already normalised.
		public Quaternion ExtractRotation(bool row_normalise = true)
		{
			Vector3 row0 = this.Row0.Xyz;
			Vector3 row1 = this.Row1.Xyz;
			Vector3 row2 = this.Row2.Xyz;
			if (row_normalise) {
				row0 = row0.Normalized();
				row1 = row1.Normalized();
				row2 = row2.Normalized();
			}
			Quaternion q = new Quaternion();
			double trace = 0.25 * (((row0[0] + row1[1]) + row2[2]) + 1.0);
			if (trace > 0.0) {
				double sq = Math.Sqrt(trace);
				q.W = (float)sq;
				sq = 1.0 / (4.0 * sq);
				q.X = (float)((row1[2] - row2[1]) * sq);
				q.Y = (float)((row2[0] - row0[2]) * sq);
				q.Z = (float)((row0[1] - row1[0]) * sq);
			}
			else if ((row0[0] > row1[1]) && (row0[0] > row2[2])) {
				double sq = 2.0 * Math.Sqrt(((1.0 + row0[0]) - row1[1]) - row2[2]);
				q.X = (float)(0.25 * sq);
				sq = 1.0 / sq;
				q.W = (float)((row2[1] - row1[2]) * sq);
				q.Y = (float)((row1[0] + row0[1]) * sq);
				q.Z = (float)((row2[0] + row0[2]) * sq);
			}
			else if (row1[1] > row2[2]) {
				double sq = 2.0 * Math.Sqrt(((1.0 + row1[1]) - row0[0]) - row2[2]);
				q.Y = (float)(0.25 * sq);
				sq = 1.0 / sq;
				q.W = (float)((row2[0] - row0[2]) * sq);
				q.X = (float)((row1[0] + row0[1]) * sq);
				q.Z = (float)((row2[1] + row1[2]) * sq);
			}
			else {
				double sq = 2.0 * Math.Sqrt(((1.0 + row2[2]) - row0[0]) - row1[1]);
				q.Z = (float)(0.25 * sq);
				sq = 1.0 / sq;
				q.W = (float)((row1[0] - row0[1]) * sq);
				q.X = (float)((row2[0] + row0[2]) * sq);
				q.Y = (float)((row2[1] + row1[2]) * sq);
			}
			q.Normalize();
			return q;
		}

		public Vector3 ExtractScale()
		{
			return new Vector3(this.Row0.Xyz.Length, this.Row1.Xyz.Length, this.Row2.Xyz.Length);
		}

		public Vector3 ExtractTranslation()
		{
			return this.Row3.Xyz;
		}

		public void Invert()
		{
			this = Invert(this);
		}

		public void Transpose()
		{
			this = Transpose(this);
		}

		public static Matrix4 Invert(Matrix4 mat)
		{
			Matrix4 result;
			Invert(ref mat, out result);
			return result;
		}

		public static Matrix4 Transpose(Matrix4 mat)
		{
			return new Matrix4(mat.Column0, mat.Column1, mat.Column2, mat.Column3);
		}

		public static void Invert(ref Matrix4 mat, out Matrix4 result)
		{
			int[] colIdx = new int[4];
			int[] rowIdx = new int[4];
			int[] pivotIdx = new int[] { -1, -1, -1, -1 };
			float[,] inverse = new float[,] { { mat.Row0.X, mat.Row0.Y, mat.Row0.Z, mat.Row0.W }, { mat.Row1.X, mat.Row1.Y, mat.Row1.Z, mat.Row1.W }, { mat.Row2.X, mat.Row2.Y, mat.Row2.Z, mat.Row2.W }, { mat.Row3.X, mat.Row3.Y, mat.Row3.Z, mat.Row3.W } };
			int icol = 0;
			int irow = 0;
			for (int i = 0; i < 4; i++) {
				float maxPivot = 0f;
				for (int j = 0; j < 4; j++) {
					if (pivotIdx[j] != 0) {
						for (int k = 0; k < 4; k++) {
							if (pivotIdx[k] == -1) {
								float absVal = Math.Abs(inverse[j, k]);
								if (absVal > maxPivot) {
									maxPivot = absVal;
									irow = j;
									icol = k;
								}
							}
							else if (pivotIdx[k] > 0) {
								result = mat;
								return;
							}
						}
					}
				}
				pivotIdx[icol]++;
				if (irow != icol) {
					for (int k = 0; k < 4; k++) {
						float f = inverse[irow, k];
						inverse[irow, k] = inverse[icol, k];
						inverse[icol, k] = f;
					}
				}
				rowIdx[i] = irow;
				colIdx[i] = icol;
				float pivot = inverse[icol, icol];
				if (pivot == 0f) {
					throw new InvalidOperationException("Matrix is singular and cannot be inverted.");
				}
				float oneOverPivot = 1f / pivot;
				inverse[icol, icol] = 1f;
				for (int k = 0; k < 4; k++) {
					inverse[icol, k] *= oneOverPivot;
				}
				for (int j = 0; j < 4; j++) {
					if (icol != j) {
						float f = inverse[j, icol];
						inverse[j, icol] = 0f;
						for (int k = 0; k < 4; k++) {
							inverse[j, k] -= inverse[icol, k] * f;
						}
					}
				}
			}
			for (int j = 3; j >= 0; j--) {
				int ir = rowIdx[j];
				int ic = colIdx[j];
				for (int k = 0; k < 4; k++) {
					float f = inverse[k, ir];
					inverse[k, ir] = inverse[k, ic];
					inverse[k, ic] = f;
				}
			}
			result.Row0.X = inverse[0, 0];
			result.Row0.Y = inverse[0, 1];
			result.Row0.Z = inverse[0, 2];
			result.Row0.W = inverse[0, 3];
			result.Row1.X = inverse[1, 0];
			result.Row1.Y = inverse[1, 1];
			result.Row1.Z = inverse[1, 2];
			result.Row1.W = inverse[1, 3];
			result.Row2.X = inverse[2, 0];
			result.Row2.Y = inverse[2, 1];
			result.Row2.Z = inverse[2, 2];
			result.Row2.W = inverse[2, 3];
			result.Row3.X = inverse[3, 0];
			result.Row3.Y = inverse[3, 1];
			result.Row3.Z = inverse[3, 2];
			result.Row3.W = inverse[3, 3];
		}

		public static void Transpose(ref Matrix4 mat, out Matrix4 result)
		{
			result.Row0 = mat.Column0;
			result.Row1 = mat.Column1;
			result.Row2 = mat.Column2;
			result.Row3 = mat.Column3;
		}

		public static Matrix4 LookAt(Vector3 eye, Vector3 target, Vector3 up)
		{
			Matrix4 result;
			Vector3 z = Vector3.Normalize(eye - target);
			Vector3 x = Vector3.Normalize(Vector3.Cross(up, z));
			Vector3 y = Vector3.Normalize(Vector3.Cross(z, x));
			result.Row0.X = x.X;
			result.Row0.Y = y.X;
			result.Row0.Z = z.X;
			result.Row0.W = 0f;
			result.Row1.X = x.Y;
			result.Row1.Y = y.Y;
			result.Row1.Z = z.Y;
			result.Row1.W = 0f;
			result.Row2.X = x.Z;
			result.Row2.Y = y.Z;
			result.Row2.Z = z.Z;
			result.Row2.W = 0f;
			result.Row3.X = -(((x.X * eye.X) + (x.Y * eye.Y)) + (x.Z * eye.Z));
			result.Row3.Y = -(((y.X * eye.X) + (y.Y * eye.Y)) + (y.Z * eye.Z));
			result.Row3.Z = -(((z.X * eye.X) + (z.Y * eye.Y)) + (z.Z * eye.Z));
			result.Row3.W = 1f;
			return result;
		}

		public static Matrix4 LookAt(float eyeX, float eyeY, float eyeZ, float targetX, float targetY, float targetZ, float upX, float upY, float upZ)
		{
			return LookAt(new Vector3(eyeX, eyeY, eyeZ), new Vector3(targetX, targetY, targetZ), new Vector3(upX, upY, upZ));
		}

		public static Matrix4 Mult(Matrix4 left, Matrix4 right)
		{
			Matrix4 result;
			Mult(ref left, ref right, out result);
			return result;
		}

		public static Matrix4 Mult(Matrix4 left, float right)
		{
			Matrix4 result;
			Mult(ref left, right, out result);
			return result;
		}

		public static void Mult(ref Matrix4 left, ref Matrix4 right, out Matrix4 result)
		{
			float lM11 = left.Row0.X;
			float lM12 = left.Row0.Y;
			float lM13 = left.Row0.Z;
			float lM14 = left.Row0.W;
			float lM21 = left.Row1.X;
			float lM22 = left.Row1.Y;
			float lM23 = left.Row1.Z;
			float lM24 = left.Row1.W;
			float lM31 = left.Row2.X;
			float lM32 = left.Row2.Y;
			float lM33 = left.Row2.Z;
			float lM34 = left.Row2.W;
			float lM41 = left.Row3.X;
			float lM42 = left.Row3.Y;
			float lM43 = left.Row3.Z;
			float lM44 = left.Row3.W;
			float rM11 = right.Row0.X;
			float rM12 = right.Row0.Y;
			float rM13 = right.Row0.Z;
			float rM14 = right.Row0.W;
			float rM21 = right.Row1.X;
			float rM22 = right.Row1.Y;
			float rM23 = right.Row1.Z;
			float rM24 = right.Row1.W;
			float rM31 = right.Row2.X;
			float rM32 = right.Row2.Y;
			float rM33 = right.Row2.Z;
			float rM34 = right.Row2.W;
			float rM41 = right.Row3.X;
			float rM42 = right.Row3.Y;
			float rM43 = right.Row3.Z;
			float rM44 = right.Row3.W;
			result.Row0.X = (((lM11 * rM11) + (lM12 * rM21)) + (lM13 * rM31)) + (lM14 * rM41);
			result.Row0.Y = (((lM11 * rM12) + (lM12 * rM22)) + (lM13 * rM32)) + (lM14 * rM42);
			result.Row0.Z = (((lM11 * rM13) + (lM12 * rM23)) + (lM13 * rM33)) + (lM14 * rM43);
			result.Row0.W = (((lM11 * rM14) + (lM12 * rM24)) + (lM13 * rM34)) + (lM14 * rM44);
			result.Row1.X = (((lM21 * rM11) + (lM22 * rM21)) + (lM23 * rM31)) + (lM24 * rM41);
			result.Row1.Y = (((lM21 * rM12) + (lM22 * rM22)) + (lM23 * rM32)) + (lM24 * rM42);
			result.Row1.Z = (((lM21 * rM13) + (lM22 * rM23)) + (lM23 * rM33)) + (lM24 * rM43);
			result.Row1.W = (((lM21 * rM14) + (lM22 * rM24)) + (lM23 * rM34)) + (lM24 * rM44);
			result.Row2.X = (((lM31 * rM11) + (lM32 * rM21)) + (lM33 * rM31)) + (lM34 * rM41);
			result.Row2.Y = (((lM31 * rM12) + (lM32 * rM22)) + (lM33 * rM32)) + (lM34 * rM42);
			result.Row2.Z = (((lM31 * rM13) + (lM32 * rM23)) + (lM33 * rM33)) + (lM34 * rM43);
			result.Row2.W = (((lM31 * rM14) + (lM32 * rM24)) + (lM33 * rM34)) + (lM34 * rM44);
			result.Row3.X = (((lM41 * rM11) + (lM42 * rM21)) + (lM43 * rM31)) + (lM44 * rM41);
			result.Row3.Y = (((lM41 * rM12) + (lM42 * rM22)) + (lM43 * rM32)) + (lM44 * rM42);
			result.Row3.Z = (((lM41 * rM13) + (lM42 * rM23)) + (lM43 * rM33)) + (lM44 * rM43);
			result.Row3.W = (((lM41 * rM14) + (lM42 * rM24)) + (lM43 * rM34)) + (lM44 * rM44);
		}

		public static void Mult(ref Matrix4 left, float right, out Matrix4 result)
		{
			result.Row0 = (Vector4)(left.Row0 * right);
			result.Row1 = (Vector4)(left.Row1 * right);
			result.Row2 = (Vector4)(left.Row2 * right);
			result.Row3 = (Vector4)(left.Row3 * right);
		}

		public static Matrix4 Add(Matrix4 left, Matrix4 right)
		{
			Matrix4 result;
			Add(ref left, ref right, out result);
			return result;
		}

		public static void Add(ref Matrix4 left, ref Matrix4 right, out Matrix4 result)
		{
			result.Row0 = left.Row0 + right.Row0;
			result.Row1 = left.Row1 + right.Row1;
			result.Row2 = left.Row2 + right.Row2;
			result.Row3 = left.Row3 + right.Row3;
		}

		public static Matrix4 Subtract(Matrix4 left, Matrix4 right)
		{
			Matrix4 result;
			Subtract(ref left, ref right, out result);
			return result;
		}

		public static void Subtract(ref Matrix4 left, ref Matrix4 right, out Matrix4 result)
		{
			result.Row0 = left.Row0 - right.Row0;
			result.Row1 = left.Row1 - right.Row1;
			result.Row2 = left.Row2 - right.Row2;
			result.Row3 = left.Row3 - right.Row3;
		}

		public bool Equals(Matrix4 other)
		{
			return ((((this.Row0 == other.Row0) && (this.Row1 == other.Row1)) && (this.Row2 == other.Row2)) && (this.Row3 == other.Row3));
		}

		public override bool Equals(object obj)
		{
			return ((obj is Matrix4) && this.Equals((Matrix4)obj));
		}

		public override int GetHashCode()
		{
			return (((this.Row0.GetHashCode() ^ this.Row1.GetHashCode()) ^ this.Row2.GetHashCode()) ^ this.Row3.GetHashCode());
		}

		public static bool operator ==(Matrix4 left, Matrix4 right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(Matrix4 left, Matrix4 right)
		{
			return !left.Equals(right);
		}

		public static Matrix4 operator *(Matrix4 left, Matrix4 right)
		{
			return Mult(left, right);
		}

		public static Matrix4 operator *(Matrix4 left, float right)
		{
			return Mult(left, right);
		}

		public static Matrix4 operator -(Matrix4 left, Matrix4 right)
		{
			return Subtract(left, right);
		}

		public static Matrix4 operator +(Matrix4 left, Matrix4 right)
		{
			return Add(left, right);
		}

		public static readonly Matrix4 Identity = new Matrix4(
			new Vector4(1.0f, 0.0f, 0.0f, 0.0f),
			new Vector4(0.0f, 1.0f, 0.0f, 0.0f),
			new Vector4(0.0f, 0.0f, 1.0f, 0.0f),
			new Vector4(0.0f, 0.0f, 0.0f, 1.0f));
	}
}
