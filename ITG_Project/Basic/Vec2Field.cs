namespace ITG_Core.Basic {
	using System.Runtime.CompilerServices;
	using ITG_Core.Base;

	/// <summary>
	/// Defines the <see cref="Vec2Field" />
	/// </summary>
	public class Vec2Field : Layer<Vec2, uint> {

		private readonly float magnitude;

		public Vec2Field(Coordinate offset, ITGThreadPool threadPool, Algorithm<uint> algorithm, float magnitude) : base(offset, threadPool, algorithm)
		{
			this.magnitude = magnitude;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override void ChunkPopulation(out Chunk<Vec2> main, in Chunk<uint> request, in Coordinate coordinate)
		{
			main = new Chunk<Vec2>();
			for ( int i = 0 ; i < Constants.CHUNK_SIZE ; i++ ) {
				for ( int j = 0 ; j < Constants.CHUNK_SIZE ; j++ ) {
					Vec2 vector = new Angle(request[i, j]).Vec2;
					vector.Magnitude = magnitude;
					main[i, j] = vector;
				}
			}
		}
	}
}