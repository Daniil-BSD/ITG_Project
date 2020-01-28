namespace System {
	using System.Runtime.CompilerServices;

	/// <summary>
	/// Defines the <see cref="MathExt" />
	/// </summary>
	public static class MathExt {
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int IntegerDevisionConsistent(int dividedivident, int deviser)
		{
			if ( deviser < 0 ) {
				deviser = -deviser;
				dividedivident = -dividedivident;
			}
			if ( dividedivident < 0 )
				return -1 - ((-dividedivident - 1) / deviser);
			return dividedivident / deviser;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long IntegerDevisionConsistent(long dividedivident, long deviser)
		{
			if ( deviser < 0 ) {
				deviser = -deviser;
				dividedivident = -dividedivident;
			}
			if ( dividedivident < 0 )
				return -1 - ((-dividedivident - 1) / deviser);
			return dividedivident / deviser;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double Modulo(double number, double modulo)
		{
			if ( number % modulo == 0 )
				return 0;
			return (number < 0) ? modulo - (-number % modulo) : number % modulo;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Modulo(float number, float modulo)
		{
			if ( number % modulo == 0 )
				return 0;
			return (number < 0) ? modulo - (-number % modulo) : number % modulo;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int Modulo(int number, int modulo)
		{
			if ( number % modulo == 0 )
				return 0;
			return (number < 0) ? modulo - (-number % modulo) : number % modulo;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long Modulo(long number, long modulo)
		{
			if ( number % modulo == 0 )
				return 0;
			return (number < 0) ? modulo - (-number % modulo) : number % modulo;
		}
	}
}
