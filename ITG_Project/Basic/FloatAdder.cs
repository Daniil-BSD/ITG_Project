namespace ITG_Core.Basic {
	using System.Collections.Generic;
	using System.Runtime.CompilerServices;
	using ITG_Core.Base;

	/// <summary>
	/// Defines the <see cref="FloatAdder" />
	/// </summary>
	public class FloatAdder : MultiInputAlgorithm<float, float> {

		public readonly float correctionFactor;

		public readonly float deltaFactor;

		public readonly float retFactor;

		public override int StdSectorSize => 32;

		public FloatAdder(Coordinate offset, ITGThreadPool threadPool, List<Algorithm<float>> sources, float deltaFactor, float retFactor) : base(offset, threadPool, sources)
		{
			this.deltaFactor = deltaFactor;
			this.retFactor = retFactor;

			float theoreticalMax = 0f;
			float factor = 1.0f;
			for ( int sIndex = 0 ; sIndex < this.sources.Length ; sIndex++ ) {
				theoreticalMax += factor;
				factor *= deltaFactor;
			}
			correctionFactor = retFactor / theoreticalMax;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override Chunk<float> ChunkPopulation(in Coordinate coordinate)
		{
			Chunk<float> chunk = new Chunk<float>();

			float factor = correctionFactor;

			for ( int sIndex = 0 ; sIndex < sources.Length ; sIndex++ ) {
				Chunk<float> sourceChunk = sources[sIndex].GetChunck(coordinate);
				AddUpChunks(ref chunk, sourceChunk, factor);
				factor *= deltaFactor;
			}
			return chunk;
		}

		protected override Sector<float> SectorPopulation(in RequstSector requstSector)
		{
			Sector<float> sector = new Sector<float>(requstSector);
			float factor = correctionFactor;
			for ( int sIndex = 0 ; sIndex < sources.Length ; sIndex++ ) {
				Sector<float> sourceSector = sources[sIndex].GetSector(requstSector);
				Sector<float>.ForeachChunk(AddUpChunks, ref sector, sourceSector, factor);
				factor *= deltaFactor;
			}
			return sector;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void AddUpChunks(ref Chunk<float> main, in Chunk<float> addition, in float factor = 1f)
		{
			for ( int i = 0 ; i < Constants.CHUNK_SIZE ; i++ ) {
				for ( int j = 0 ; j < Constants.CHUNK_SIZE ; j++ ) {
					main[i, j] += addition[i, j] * factor;
				}
			}
		}
	}
}