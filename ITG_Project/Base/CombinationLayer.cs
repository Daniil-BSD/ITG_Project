using System.Runtime.CompilerServices;

namespace ITG_Core.Base
{
	public abstract class CombinationLayer<T, S, M> : Layer<T, S> where T : struct where S : struct where M : struct
	{

		protected readonly Algorithm<M> modifier;

		public CombinationLayer(Coordinate offset, ITGThreadPool threadPool, Algorithm<S> source, Algorithm<M> modifier) : base(offset, threadPool, source)
		{
			this.modifier = modifier;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override Chunk<T> ChunkPopulation(in Coordinate coordinate)
		{
			ChunkPopulation(out Chunk<T> ret, source.GetChunck(coordinate), modifier.GetChunck(coordinate), coordinate);
			return ret;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected virtual void ChunkPopulation(out Chunk<T> main, in Chunk<S> mainRequest, in Chunk<M> modifierRequest, in Coordinate coordinate)
		{
			main = ChunkPopulation(coordinate);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override Sector<T> SectorPopulation(in RequstSector requstSector)
		{
			var sector = new Sector<T>(requstSector, false);
			Sector<S> mainRequest = source.GetSector(requstSector);
			Sector<M> modifierRequest = modifier.GetSector(requstSector);
			for (int i = 0; i < sector.width; i++)
			{
				for (int j = 0; j < sector.height; j++)
				{
					Coordinate coordinate = new Coordinate(i, j) + requstSector.coordinate;
					ChunkPopulation(out Chunk<T> chunk, source.GetChunck(coordinate), modifier.GetChunck(coordinate), coordinate);
					sector.Chunks[i, j] = chunk;
				}
			}
			return sector;
		}
	}
}