namespace ITG_Core {
	/// <summary>
	/// type for representing the position on the 2D plane.
	/// </summary>
	public struct Coordinate {
		public readonly int x;

		public readonly int y;

		public uint UintX => unchecked((uint) x) + int.MaxValue;

		public uint UintY => unchecked((uint) y) + int.MaxValue;

		public Coordinate(in int X, in int Y)
		{
			x = X;
			y = Y;
		}

		public Coordinate(long X, long Y)
		{
			x = unchecked((int) X);
			y = unchecked((int) Y);
		}

		public Coordinate(uint X, uint Y)
		{
			x = unchecked((int) X);
			y = unchecked((int) Y);
		}

		public override bool Equals(object obj)
		{
			if ( !(obj is Coordinate) ) {
				return false;
			}
			var coordinate = (Coordinate) obj;
			return x == coordinate.x && y == coordinate.y;
		}

		public override int GetHashCode()
		{
			return ((x << 16) | (y & 0b0000_0000_0000_0000_1111_1111_1111_1111));
		}

		public override string ToString()
		{
			return "(" + x + ", " + y + ")";
		}
	}
}
