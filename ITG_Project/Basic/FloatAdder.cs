namespace ITG_Core {
	using System.Collections.Generic;
	using System.Runtime.CompilerServices;

	/// <summary>
	/// Defines the <see cref="FloatAdder" />
	/// </summary>
	public class FloatAdder : MultiInputAlgorithm<float, float> {
		public readonly float deltaFactor;

		public readonly float retFactor;

		public FloatAdder(List<Algorithm<float>> sources, float deltaFactor, float retFactor) : base(sources)
		{
			this.deltaFactor = deltaFactor;
			this.retFactor = retFactor;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override Chunk<float> ChunkPopulation(in Coordinate coordinate)
		{
			Chunk<float> chunk = new Chunk<float>();
			float factor = 1.0f;
			float theoreticalMax = 0f;
			for ( int sIndex = 0 ; sIndex < sources.Length ; sIndex++ ) {
				Chunk<float> sourceChunk = sources[sIndex].GetChunck(coordinate);
				for ( int i = 0 ; i < Constants.CHUNK_SIZE ; i++ ) {
					for ( int j = 0 ; j < Constants.CHUNK_SIZE ; j++ ) {
						chunk[i, j] += sourceChunk[i, j] * factor;
					}
				}
				theoreticalMax += factor;
				factor *= deltaFactor;
			}
			float correctionFactor = retFactor / theoreticalMax;
			for ( int i = 0 ; i < Constants.CHUNK_SIZE ; i++ ) {
				for ( int j = 0 ; j < Constants.CHUNK_SIZE ; j++ ) {
					chunk[i, j] *= correctionFactor;
				}
			}
			return chunk;
		}
	}
}
