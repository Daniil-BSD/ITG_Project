namespace System {
	using System.Runtime.CompilerServices;

	/// <summary>
	/// Defines the <see cref="MathExt" />
	/// </summary>
	public static class MathExt {
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int IntegerDevisionConsistent(this in int dividedivident, in int deviser)
		{
			if ( deviser < 0 )
				if ( dividedivident > 0 )
					return -1 - ((dividedivident - 1) / -deviser);
			if ( dividedivident < 0 )
				return -1 - ((-dividedivident - 1) / deviser);
			return dividedivident / deviser;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long IntegerDevisionConsistent(this in long dividedivident, in long deviser)
		{
			if ( deviser < 0 )
				if ( dividedivident > 0 )
					return -1 - ((dividedivident - 1) / -deviser);
			if ( dividedivident < 0 )
				return -1 - ((-dividedivident - 1) / deviser);
			return dividedivident / deviser;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Max(in float a, in float b)
		{
			return (a < b) ? b : a;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Max(in float a, in float b, in float c)
		{
			return Max(a, Max(b, c));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int Max(in int a, in int b)
		{
			return (a < b) ? b : a;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int Max(in int a, in int b, in int c)
		{
			return Max(a, Max(b, c));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Min(in float a, in float b)
		{
			return (a > b) ? b : a;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Min(in float a, in float b, in float c)
		{
			return Min(a, Min(b, c));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int Min(in int a, in int b)
		{
			return (a > b) ? b : a;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int Min(in int a, in int b, in int c)
		{
			return Min(a, Min(b, c));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double Modulo(this in double number, in double modulo)
		{
			if ( number % modulo == 0 )
				return 0;
			return (number < 0) ? modulo - (-number % modulo) : number % modulo;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Modulo(this in float number, in float modulo)
		{
			if ( number % modulo == 0 )
				return 0;
			return (number < 0) ? modulo - (-number % modulo) : number % modulo;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int Modulo(this in int number, in int modulo)
		{
			if ( number % modulo == 0 )
				return 0;
			return (number < 0) ? modulo - (-number % modulo) : number % modulo;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long Modulo(this in long number, in long modulo)
		{
			if ( number % modulo == 0 )
				return 0;
			return (number < 0) ? modulo - (-number % modulo) : number % modulo;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int ToIntegerConsistent(this in double f)
		{
			return (f < 0) ? (int) f - 1 : (int) f;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int ToIntegerConsistent(this in float f)
		{
			return (f < 0 && f % 1 != 0) ? (int) f - 1 : (int) f;
		}
	}
}
