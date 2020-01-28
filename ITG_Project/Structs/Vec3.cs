﻿namespace ITG_Core {
	using System;

	/// <summary>
	/// Defines the <see cref="Vec3" />
	/// </summary>
	public unsafe struct Vec3 {
		public float x;

		public float y;

		public float z;

		public float Magnitude {
			get {
				return (float) Math.Sqrt(x * x + y * y + z * z);
			}
			set {
				float f = (value == 0) ? 0 : Magnitude / value;
				this *= f;
			}
		}

		public Vec3 NormalizedCopy {
			get {
				float f = Magnitude;
				f = (f == 0) ? 0 : 1 / f;
				return new Vec3(x * f, y * f, z * f);
			}
		}

		public unsafe Vec3(float f)
		{
			x = y = z = f;
		}

		public Vec3(float X, float Y, float Z)
		{
			x = X;
			y = Y;
			z = Z;
		}

		public Vec3(float* X, float* Y, float* Z)
		{
			x = *X;
			y = *Y;
			z = *Z;
		}

		public Vec3(ref Vec3 V)
		{
			x = V.x;
			y = V.y;
			z = V.z;
		}

		public static Vec3 Cross(Vec3 V1, Vec3 V2)
		{
			return *Vec3.Cross(&V1, &V2);
		}

		public static Vec3* Cross(Vec3* V1, Vec3* V2)
		{
			float x = V1->y * V2->z - V2->y * V1->z;
			float y = V2->x * V1->z - V1->x * V2->z;
			float z = V1->x * V2->y - V2->x * V1->y;
			Vec3 v = new Vec3(&x, &y, &z);
			return &v;
		}

		public static float Dot(Vec3 V1, Vec3 V2)
		{
			return Vec3.Dot(&V1, &V2);
		}

		public static float Dot(Vec3* V1, Vec3* V2)
		{
			return V1->x * V2->x + V1->y * V2->y + V1->z * V2->z;
		}

		public static Vec3* Minus(Vec3* V1, Vec3* V2)
		{
			Vec3 v = new Vec3(V1->x - V2->x, V1->y - V2->y, V1->z - V2->z);
			return &v;
		}

		public static Vec3* Neg(Vec3* V1)
		{
			Vec3 v = new Vec3(-V1->x, -V1->y, -V1->z);
			return &v;
		}

		public static Vec3* Plus(Vec3* V1, Vec3* V2)
		{
			Vec3 v = new Vec3(V1->x + V2->x, V1->y + V2->y, V1->z + V2->z);
			return &v;
		}

		public static Vec3* Scale(Vec3* V1, float f)
		{
			Vec3 v = new Vec3(V1->x * f, V1->y * f, V1->z * f);
			return &v;
		}

		public static Vec3* Scale(Vec3* V1, Vec3* V2)
		{
			Vec3 v = new Vec3(V1->x * V2->x, V1->y * V2->y, V1->z * V2->z);
			return &v;
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

		public Vec3 normalize()
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



		public static Vec3 operator -(Vec3 V1)
		{
			return *Vec3.Neg(&V1);
		}

		public static Vec3 operator -(Vec3 V1, Vec3 V2)
		{
			return *Vec3.Minus(&V1, &V2);
		}

		public static Vec3 operator +(Vec3 V1, Vec3 V2)
		{
			return *Vec3.Plus(&V1, &V2);
		}

		public static Vec3 operator *(Vec3 V1, Vec3 V2)
		{
			return *Vec3.Scale(&V1, &V2);
		}

		public static Vec3 operator *(Vec3 V1, float f)
		{
			return *Vec3.Scale(&V1, f);
		}

		public static Vec3 operator *(float f, Vec3 V1)
		{
			return V1 * f;
		}

		public static Vec3 operator /(Vec3 V1, float f)
		{
			return V1 * (1 / f);
		}
	}
}
