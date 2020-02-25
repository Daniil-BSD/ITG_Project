namespace ITG_Core {
	using System;
	using System.Runtime.CompilerServices;

	/// <summary>
	/// Defines the <see cref="Vec3" />
	/// </summary>
	public unsafe struct Vec3 {
		public float x;

		public float y;

		public float z;

		public Vec3 CrossX {
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get {
				return new Vec3(0, z, -y);
			}
		}

		public Vec3 CrossY {
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get {
				return new Vec3(-z, 0, x);
			}
		}

		public Vec3 CrossZ {
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get {
				return new Vec3(y, -x, 0);
			}
		}

		public float Magnitude {
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get {
				return (float) Math.Sqrt(x * x + y * y + z * z);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set {
				float f = (value == 0) ? 0 : Magnitude / value;
				this *= f;
			}
		}

		public Vec3 NormalizedCopy {
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get {
				float f = Magnitude;
				if ( f == 1 )
					return new Vec3(this);
				f = (f == 0) ? 0 : 1 / f;
				return new Vec3(x * f, y * f, z * f);
			}
		}

		public unsafe Vec3(in float f)
		{
			x = y = z = f;
		}

		public Vec3(in float X, in float Y, in float Z)
		{
			x = X;
			y = Y;
			z = Z;
		}

		public Vec3(in Vec3 V)
		{
			x = V.x;
			y = V.y;
			z = V.z;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vec3 Cross(in Vec3 V1, in Vec3 V2)
		{
			float x = V1.y * V2.z - V2.y * V1.z;
			float y = V2.x * V1.z - V1.x * V2.z;
			float z = V1.x * V2.y - V2.x * V1.y;
			return new Vec3(x, y, z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Dot(in Vec3 V1, in Vec3 V2)
		{
			return V1.x * V2.x + V1.y * V2.y + V1.z * V2.z;
		}

		public override bool Equals(object obj)
		{
			if ( !(obj is Vec3) ) {
				return false;
			}
			var v = (Vec3) obj;
			return x == v.x && y == v.y && z == v.z;
		}

		public override int GetHashCode()
		{
			float copyX = x;
			float copyY = y;
			float copyZ = z;
			int xBin = (*(int*) &copyX) >> 12;
			int yBin = (*(int*) &copyY) >> 12;
			int zBin = (*(int*) &copyZ) >> 12;
			return ((xBin << 22) >> 2) | (yBin & 0b0000_0000_0000_0000_0000_0011_1111_1111) << 10 | (zBin & 0b0000_0000_0000_0000_0000_0011_1111_1111);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vec3 Normalize()
		{
			float f = Magnitude;
			f = (f == 0) ? 0 : 1 / f;
			x *= f;
			y *= f;
			z *= f;
			return this;
		}

		public override string ToString()
		{
			return "(" + x + ", " + y + ", " + z + ")";
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vec3 operator -(in Vec3 V1)
		{
			return new Vec3(-V1.x, -V1.y, -V1.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vec3 operator -(in Vec3 V1, in Vec3 V2)
		{
			return new Vec3(V1.x - V2.x, V1.y - V2.y, V1.z - V2.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vec3 operator +(in Vec3 V1, in Vec3 V2)
		{
			return new Vec3(V1.x + V2.x, V1.y + V2.y, V1.z + V2.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vec3 operator *(in Vec3 V1, in Vec3 V2)
		{
			return new Vec3(V1.x * V2.x, V1.y * V2.y, V1.z * V2.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vec3 operator *(in Vec3 V1, in float f)
		{
			return new Vec3(V1.x * f, V1.y * f, V1.z * f);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vec3 operator *(in float f, in Vec3 V1)
		{
			return new Vec3(V1.x * f, V1.y * f, V1.z * f);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vec3 operator /(in Vec3 V1, in float f)
		{
			return new Vec3(V1.x / f, V1.y / f, V1.z / f);
		}
	}
}
