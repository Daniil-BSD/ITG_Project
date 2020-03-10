namespace ITG_Core {
	/// <summary>
	/// Defines the <see cref="Downhill" />
	/// </summary>
	public class Downhill : Layer<Vec3, Vec3> {
		public Downhill(Coordinate offset, ITGThreadPool threadPool, Algorithm<Vec3> source) : base(offset, threadPool, source)
		{
		}

		protected override Chunk<Vec3> ChunkPopulation(in Coordinate coordinate)
		{
			Chunk<Vec3> returnChunk = new Chunk<Vec3>();
			Chunk<Vec3> sourceChunk = source.GetChunck(coordinate);
			for ( int i = 0 ; i < Constants.CHUNK_SIZE ; i++ ) {
				for ( int j = 0 ; j < Constants.CHUNK_SIZE ; j++ ) {
					Vec3 normal = sourceChunk[i, j];
					returnChunk[i, j] = new Vec3(normal.x * normal.z, normal.y * normal.z, -(normal.x * normal.x) - (normal.y * normal.y)).Normalize();
				}
			}
			return returnChunk;
		}
	}
}
