using System.Runtime.CompilerServices;

namespace ITG_Core.Base {
	/// <summary>
	/// Defines the <see cref="Layer{T, S}" />
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <typeparam name="S"></typeparam>
	public abstract class Layer<T, S> : Algorithm<T> where T : struct where S : struct {
		protected readonly Algorithm<S> source;

		public Layer(Coordinate offset, ITGThreadPool threadPool, Algorithm<S> source) : base(offset, threadPool)
		{
			this.source = source;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override Chunk<T> ChunkPopulation(in Coordinate coordinate)
		{
			ChunkPopulation(out Chunk<T> ret, source.GetChunck(coordinate), coordinate);
			return ret;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected virtual void ChunkPopulation(out Chunk<T> main, in Chunk<S> request, in Coordinate coordinate)
		{
			main = ChunkPopulation(coordinate);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override Sector<T> SectorPopulation(in RequstSector requstSector)
		{
			Sector<T> sector = new Sector<T>(requstSector, false);
			Sector<S> request = source.GetSector(requstSector);
			Sector<T>.ForeachChunk(ChunkPopulation, ref sector, request);
			return sector;
		}
	}
}
