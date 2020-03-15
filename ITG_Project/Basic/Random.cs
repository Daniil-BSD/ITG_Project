namespace ITG_Core.Basic {
	using ITG_Core.Base;
	using System.Runtime.CompilerServices;

	/// <summary>
	/// Defines the <see cref="Random" />
	/// </summary>
	public class Random : Algorithm<uint> {
		private readonly int seed;

		public Random(Coordinate offset, ITGThreadPool threadPool, int seed = 0) : base(offset, threadPool)
		{
			this.seed = seed;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override Chunk<uint> ChunkPopulation(in Coordinate coordinate)
		{
			LehmerPlusSRNG random = new LehmerPlusSRNG(coordinate, seed);
			Chunk<uint> chunk = new Chunk<uint>();
			for ( int i = 0 ; i < Constants.CHUNK_SIZE ; i++ ) {
				for ( int j = 0 ; j < Constants.CHUNK_SIZE ; j++ ) {
					chunk[i, j] = random.Next();
				}
			}
			return chunk;
		}

		/// <summary>
		/// Defines the <see cref="LehmerPlusSRNG" />
		/// </summary>
		private class LehmerPlusSRNG {
			//Hard-coded values below are prime numbers with a very specific binary representations
			private static readonly ulong INPUT_SEED = 373447861;

			private static readonly ulong SEED = 268435399L * 268435459L;

			private static readonly ulong X_SEED = 373447927;

			private static readonly ulong Y_SEED = 373447969;

			private ulong state;

			public LehmerPlusSRNG(Coordinate coordinate, int seed = 0)
			{
				state = unchecked((uint) coordinate.x) * X_SEED + unchecked((uint) coordinate.y) * Y_SEED + unchecked((uint) seed) * INPUT_SEED + SEED;
				if ( state % 2 == 0 )
					state += 1;
				Next();
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public uint Next()
			{
				state = unchecked(state * SEED);
				return (uint) (state >> (8 + (int) (state >> 61)));
			}
		}
	}
}
