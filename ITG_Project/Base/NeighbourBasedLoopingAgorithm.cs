using System.Runtime.CompilerServices;

namespace ITG_Core.Base {

	public abstract class NeighbourBasedLoopingAgorithm<T> : NeighbourBasedAgorithm<T, T> where T : struct {

		public readonly int repeats;

		public NeighbourBasedLoopingAgorithm(Coordinate offset, ITGThreadPool threadPool, Algorithm<T> source, int repeats) : base(offset, threadPool, source)
		{
			this.repeats = repeats;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected sealed override Sector<T> SectorPopulation(in RequstSector requstSector)
		{
			RequstSector outgoingRequstSector = requstSector.GetExpandedCopy(1);
			Sector<T> sourceSector = source.GetSector(outgoingRequstSector);
			Sector<T> sector = new Sector<T>(requstSector);
			for ( int k = repeats - 1 ; k >= 0 ; k-- ) {
				for ( int i = -k ; i < sector.Width_units + k ; i++ ) {
					for ( int j = -k ; j < sector.Height_units + k ; j++ ) {
						if ( k == 0 )
							sector[i, j] = Compute(new Neighborhood<T>(sourceSector, i, j));
						else
							sourceSector[Constants.CHUNK_SIZE + i, Constants.CHUNK_SIZE + j] = Compute(new Neighborhood<T>(sourceSector, i, j));
					}
				}
			}
			return sector;
		}
	}
}