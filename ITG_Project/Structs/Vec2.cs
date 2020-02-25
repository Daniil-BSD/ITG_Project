namespace ITG_Core {
	using System;

	/// <summary>
	/// Defines the <see cref="Vec2" />
	/// </summary>
	public unsafe struct Vec2 {
		public float x;

		public float y;

		public float Magnitude {
			get {
				return (float) Math.Sqrt(x * x + y * y);
			}
			set {
				float f = (value == 0) ? 0 : Magnitude / value;
				this *= f;
			}
		}

		public Vec2 NormalizedCopy {
			get {
				float f = Magnitude;
				f = (f == 0) ? 0 : 1 / f;
				return new Vec2(x * f, y * f);
			}
		}

		public unsafe Vec2(in float f)
		{
			x = y = f;
		}

		public Vec2(in float X, in float Y)
		{
			x = X;
			y = Y;
		}

		public Vec2(in Vec2 V)
		{
			x = V.x;
			y = V.y;
		}

		public static float Dot(in Vec2 V1, in Vec2 V2)
		{
			return V1.x * V2.x + V1.y * V2.y;
		}

		public override bool Equals(object obj)
		{
			if ( !(obj is Vec2) ) {
				return false;
			}
			var v = (Vec2) obj;
			return x == v.x && y == v.y;
		}

		public override int GetHashCode()
		{
			float copyX = x;
			float copyY = y;
			int xBin = (*(int*) &copyX) >> 7;
			int yBin = (*(int*) &copyY) >> 7;
			return (xBin << 16) | (yBin & 0b0000_0000_0000_0000_1111_1111_1111_1111);
		}

		public Vec2 Normalize()
		{
			float f = Magnitude;
			f = (f == 0) ? 0 : 1 / f;
			x *= f;
			y *= f;
			return this;
		}

		public override string ToString()
		{
			return "(" + x + ", " + y + ")";
		}



		public static Vec2 operator -(in Vec2 V1)
		{
			return new Vec2(-V1.x, -V1.y);
		}

		public static Vec2 operator -(in Vec2 V1, in Vec2 V2)
		{
			return new Vec2(V1.x - V2.x, V1.y - V2.y);
		}

		public static Vec2 operator +(in Vec2 V1, in Vec2 V2)
		{
			return new Vec2(V1.x + V2.x, V1.y + V2.y);
		}

		public static Vec2 operator *(in Vec2 V1, in Vec2 V2)
		{
			return new Vec2(V1.x * V2.x, V1.y * V2.y);
		}

		public static Vec2 operator *(in Vec2 V1, in float f)
		{
			return new Vec2(V1.x * f, V1.y * f);
		}

		public static Vec2 operator *(in float f, in Vec2 V1)
		{
			return V1 * f;
		}

		public static Vec2 operator /(in Vec2 V1, in float f)
		{
			return new Vec2(V1.x / f, V1.y / f);
		}
	}
}
