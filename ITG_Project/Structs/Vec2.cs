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

		public unsafe Vec2(float f)
		{
			x = y = f;
		}

		public Vec2(float X, float Y)
		{
			x = X;
			y = Y;
		}

		public Vec2(float* X, float* Y)
		{
			x = *X;
			y = *Y;
		}

		public Vec2(ref Vec2 V)
		{
			x = V.x;
			y = V.y;
		}

		public static float Dot(Vec2 V1, Vec2 V2)
		{
			return Vec2.Dot(&V1, &V2);
		}

		public static float Dot(Vec2* V1, Vec2* V2)
		{
			return V1->x * V2->x + V1->y * V2->y;
		}

		public static Vec2* Minus(Vec2* V1, Vec2* V2)
		{
			Vec2 v = new Vec2(V1->x - V2->x, V1->y - V2->y);
			return &v;
		}

		public static Vec2* Neg(Vec2* V1)
		{
			Vec2 v = new Vec2(-V1->x, -V1->y);
			return &v;
		}

		public static Vec2* Plus(Vec2* V1, Vec2* V2)
		{
			Vec2 v = new Vec2(V1->x + V2->x, V1->y + V2->y);
			return &v;
		}

		public static Vec2* Scale(Vec2* V1, float f)
		{
			Vec2 v = new Vec2(V1->x * f, V1->y * f);
			return &v;
		}

		public static Vec2* Scale(Vec2* V1, Vec2* V2)
		{
			Vec2 v = new Vec2(V1->x * V2->x, V1->y * V2->y);
			return &v;
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

		public Vec2 normalize()
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




		public static Vec2 operator -(Vec2 V1)
		{
			return *Vec2.Neg(&V1);
		}

		public static Vec2 operator -(Vec2 V1, Vec2 V2)
		{
			return *Vec2.Minus(&V1, &V2);
		}

		public static Vec2 operator +(Vec2 V1, Vec2 V2)
		{
			return *Vec2.Plus(&V1, &V2);
		}

		public static Vec2 operator *(Vec2 V1, Vec2 V2)
		{
			return *Vec2.Scale(&V1, &V2);
		}

		public static Vec2 operator *(Vec2 V1, float f)
		{
			return *Vec2.Scale(&V1, f);
		}

		public static Vec2 operator *(float f, Vec2 V1)
		{
			return V1 * f;
		}

		public static Vec2 operator /(Vec2 V1, float f)
		{
			return V1 * (1 / f);
		}
	}
}
