namespace ITG_Core {
	using System.Runtime.CompilerServices;

	/// <summary>
	/// Defines the <see cref="Vec2Field" />
	/// </summary>
	public class Vec2Field : Layer<Vec2, uint> {
		private readonly float magnitude;

		public Vec2Field(Coordinate offset, Algorithm<uint> algorithm, float magnitude) : base(offset, algorithm)
		{
			this.magnitude = magnitude;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override Chunk<Vec2> ChunkPopulation(in Coordinate coordinate)
		{
			Chunk<Vec2> returnChunk = new Chunk<Vec2>();
			Chunk<uint> sourceChunk = source.GetChunck(coordinate);
			for ( int i = 0 ; i < Constants.CHUNK_SIZE ; i++ ) {
				for ( int j = 0 ; j < Constants.CHUNK_SIZE ; j++ ) {
					Vec2 vector = new Angle(sourceChunk[i, j]).Vec2;
					vector.Magnitude = magnitude;
					returnChunk[i, j] = vector;
				}
			}
			return returnChunk;
		}
	}
}
