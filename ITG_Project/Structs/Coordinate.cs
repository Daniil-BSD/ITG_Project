﻿namespace ITG_Core {
	using System;
	using System.Runtime.CompilerServices;

	/// <summary>
	/// type for representing the position on the 2D plane.
	/// </summary>
	public struct Coordinate {

		public static readonly Coordinate Origin = new Coordinate(0, 0);

		public readonly int x;

		public readonly int y;

		public uint UintX => unchecked((uint)x) + int.MaxValue;

		public uint UintY => unchecked((uint)y) + int.MaxValue;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Coordinate(in Coordinate coordinate)
		{
			x = coordinate.x;
			y = coordinate.y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Coordinate(in int X, in int Y)
		{
			x = X;
			y = Y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Coordinate(in long X, in long Y)
		{
			x = unchecked((int)X);
			y = unchecked((int)Y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Coordinate(uint X, uint Y)
		{
			x = unchecked((int)X);
			y = unchecked((int)Y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Coordinate operator -(in Coordinate c1)
		{
			if ( c1.x == 0 && c1.y == 0 )
				return c1;
			return new Coordinate(-c1.x, -c1.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Coordinate operator -(in Coordinate c1, in Coordinate c2)
		{
			if ( c2.x == 0 && c2.y == 0 )
				return c1;
			if ( c1.x == 0 && c1.y == 0 )
				return -c2;
			return new Coordinate(c1.x - c2.x, c1.y - c2.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Coordinate operator %(in Coordinate c1, in int i)
		{
			return new Coordinate(c1.x.Modulo(i), c1.y.Modulo(i));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Coordinate operator *(in Coordinate c1, in int i)
		{
			if ( i == 1 )
				return c1;
			return new Coordinate(c1.x * i, c1.y * i);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Coordinate operator /(in Coordinate c1, in int i)
		{
			return new Coordinate(c1.x.IntegerDevisionConsistent(i), c1.y.IntegerDevisionConsistent(i));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Coordinate operator +(in Coordinate c1, in Coordinate c2)
		{
			if ( c2.x == 0 && c2.y == 0 )
				return c1;
			if ( c1.x == 0 && c1.y == 0 )
				return c2;
			return new Coordinate(c1.x + c2.x, c1.y + c2.y);
		}

		public override bool Equals(object obj)
		{
			if ( !( obj is Coordinate ) ) {
				return false;
			}
			Coordinate coordinate = (Coordinate)obj;
			return x == coordinate.x && y == coordinate.y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override int GetHashCode()
		{
			return ( ( x << 16 ) | ( y & 0b0000_0000_0000_0000_1111_1111_1111_1111 ) );
		}

		public override string ToString()
		{
			return "(" + x + ", " + y + ")";
		}
	}

	/// <summary>
	/// Defines the <see cref="CoordinateBasic" />
	/// </summary>
	public struct CoordinateBasic {

		public int x;

		public int y;

		public CoordinateBasic(in int x, in int y)
		{
			this.x = x;
			this.y = y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator CoordinateBasic(in Vec2 original)
		{
			return new CoordinateBasic(original.x.ToIntegerConsistent(), original.y.ToIntegerConsistent());
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator Coordinate(in CoordinateBasic original)
		{
			return new Coordinate(original.x, original.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator CoordinateBasic(in Coordinate original)
		{
			return new CoordinateBasic(original.x, original.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator Vec2(in CoordinateBasic original)
		{
			return new Vec2(original.x, original.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static CoordinateBasic operator -(in CoordinateBasic c1, in CoordinateBasic c2)
		{
			return new CoordinateBasic(c1.x - c2.x, c1.y - c2.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static CoordinateBasic operator +(in CoordinateBasic c1, in CoordinateBasic c2)
		{
			return new CoordinateBasic(c1.x + c2.x, c1.y + c2.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override int GetHashCode()
		{
			return ( ( x << 16 ) | ( y & 0b0000_0000_0000_0000_1111_1111_1111_1111 ) );
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override string ToString()
		{
			return "(" + x + ", " + y + ")";
		}
	}
}