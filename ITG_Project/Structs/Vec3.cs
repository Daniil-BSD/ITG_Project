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

		public static Vec3 X
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new Vec3(1, 0, 0);
		}

		public static Vec3 Y
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new Vec3(0, 1, 0);
		}

		public static Vec3 Z
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new Vec3(0, 0, 1);
		}

		public Vec3 CrossX
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new Vec3(0, z, -y);
		}

		public Vec3 CrossY
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new Vec3(-z, 0, x);
		}

		public Vec3 CrossZ
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new Vec3(y, -x, 0);
		}

		public float Magnitude
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => (float)Math.Sqrt(MagnitudeSquared);
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set {
				float f = ( value == 0 ) ? 0 : Magnitude / value;
				this *= f;
			}
		}

		public float MagnitudeSquared
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => x * x + y * y + z * z;
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set => Magnitude = (float)Math.Sqrt(value);
		}

		public Vec3 NormalizedCopy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get {
				float f = Magnitude;
				if ( f == 1 )
					return new Vec3(this);
				f = ( f == 0 ) ? 0 : 1 / f;
				return new Vec3(x * f, y * f, z * f);
			}
		}

		public Vec2 XY
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new Vec2(x, y);
		}

		public Vec2 XZ
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new Vec2(x, z);
		}

		public Vec2 YZ
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new Vec2(y, z);
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

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vec3 Interpolate(in Vec3 V1, in Vec3 V2, in float f)
		{
			float dot = Vec3.Dot(V1, V2);
			double theta = Math.Acos(dot) * f;
			Vec3 RelativeVec = V2 - V1 * dot;
			RelativeVec.Normalize();
			return ( ( V1 * (float)Math.Cos(theta) ) + ( RelativeVec * (float)Math.Sin(theta) ) );
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vec3 NormalFromGradients(in float gradientX, in float gradientY)
		{
			return new Vec3(-gradientX, -gradientY, 1).Normalize();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vec3 NormalFromGradients(in Vec2 gradients)
		{
			return NormalFromGradients(gradients.x, gradients.y);
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

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vec3 operator +(in Vec3 V1, in Vec3 V2)
		{
			return new Vec3(V1.x + V2.x, V1.y + V2.y, V1.z + V2.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float Dot(in Vec3 V1)
		{
			return Dot(this, V1);
		}

		public override bool Equals(object obj)
		{
			if ( !( obj is Vec3 ) ) {
				return false;
			}
			Vec3 v = (Vec3)obj;
			return x == v.x && y == v.y && z == v.z;
		}

		public override int GetHashCode()
		{
			float copyX = x;
			float copyY = y;
			float copyZ = z;
			int xBin = ( *(int*)&copyX ) >> 12;
			int yBin = ( *(int*)&copyY ) >> 12;
			int zBin = ( *(int*)&copyZ ) >> 12;
			int mask = 0b0000_0000_0000_0000_0000_0011_1111_1111;
			return ( ( xBin << 22 ) >> 2 ) | ( yBin & mask ) << 10 | ( zBin & mask );
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vec3 Normalize()
		{
			float f = Magnitude;
			f = ( f == 0 ) ? 0 : 1 / f;
			x *= f;
			y *= f;
			z *= f;
			return this;
		}

		public override string ToString()
		{
			return "(" + x + ", " + y + ", " + z + ")";
		}
	}
}