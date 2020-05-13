namespace ITG_Core.Base {
	using System;
	using System.Runtime.CompilerServices;

	public interface IAlgorithm {

		Type GetGenericType();
	}

	abstract public class Algorithm<T> : IAlgorithm where T : struct {

		public readonly Coordinate offset;

		public readonly ITGThreadPool threadPool;

		// overridable constant for children
		public virtual int StdSectorSize
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => Constants.DEFAULT_SECTOR_SIZE;
		}

		public delegate Sector<T> SectorPopulationDelegate(in RequstSector requstSector);

		public Algorithm(Coordinate offset, ITGThreadPool threadPool)
		{
			this.offset = offset;
			this.threadPool = threadPool;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected virtual Chunk<T> ChunkPopulation(in Coordinate coordinate)
		{
			return SectorPopulation(new RequstSector(coordinate, 1, 1)).Chunks[0, 0];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected virtual Sector<T> SectorPopulation(in RequstSector requstSector)
		{
			Sector<T> sector = new Sector<T>(requstSector, false);
			for ( int i = 0 ; i < sector.width ; i++ ) {
				for ( int j = 0 ; j < sector.height ; j++ ) {
					sector.Chunks[i, j] = ChunkPopulation(new Coordinate(i, j) + sector.Coordinate);
				}
			}
			return sector;
		}

		public virtual void Drop()
		{
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Chunk<T> GetChunck(in Coordinate coordinate)
		{
			return ChunkPopulation(coordinate + offset);
		}

		public Type GetGenericType()
		{
			return typeof(T);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Sector<T> GetSector(in RequstSector requstSector)
		{
			int stdSectorSize = StdSectorSize;
			int subsectorsX = requstSector.width / stdSectorSize;
			int subsectorsY = requstSector.height / stdSectorSize;
			if ( subsectorsX <= 1 && subsectorsY <= 1 )
				return SectorPopulation(requstSector.GetOffsetCopy(offset)).OffsetBack(offset);

			RequstSector[] subsectorRequests = new RequstSector[subsectorsX * subsectorsY];
			for ( int i = 0 ; i < subsectorsX ; i++ ) {
				for ( int j = 0 ; j < subsectorsY ; j++ ) {
					int width = stdSectorSize;
					int height = stdSectorSize;
					if ( i == subsectorsX - 1 )
						width = requstSector.width - stdSectorSize * i;
					if ( j == subsectorsY - 1 )
						height = requstSector.height - stdSectorSize * j;
					subsectorRequests[i * subsectorsY + j] = new RequstSector(new Coordinate(i * stdSectorSize, j * stdSectorSize) + requstSector.coordinate + offset, width, height);
				}
			}
			Sector<T>[] subsectors = threadPool.Execute(subsectorRequests, SectorPopulation);
			Sector<T> sector = new Sector<T>(requstSector);

			for ( int si = 0 ; si < subsectorsX ; si++ ) {
				for ( int sj = 0 ; sj < subsectorsY ; sj++ ) {
					int subsectorIndex = si * subsectorsY + sj;
					for ( int i = 0 ; i < subsectors[subsectorIndex].width ; i++ ) {
						for ( int j = 0 ; j < subsectors[subsectorIndex].height ; j++ ) {
							sector.Chunks[si * stdSectorSize + i, sj * stdSectorSize + j] = subsectors[subsectorIndex].Chunks[i, j];
						}
					}
				}
			}
			return sector;
		}
	}
}