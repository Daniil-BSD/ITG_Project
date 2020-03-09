namespace ITG_Core {
	using System.Runtime.CompilerServices;

	/// <summary>
	/// Defines the <see cref="NeighbourBasedMerger{T, S1, S2}" />
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <typeparam name="S1"></typeparam>
	/// <typeparam name="S2"></typeparam>
	public abstract class NeighbourBasedMerger<T, S1, S2> : NeighbourBasedAgorithm<T, S1> where T : struct where S1 : struct where S2 : struct {
		protected readonly Algorithm<S2> source2;

		public NeighbourBasedMerger(Coordinate offset, Algorithm<S1> source, Algorithm<S2> source2) : base(offset, source)
		{
			this.source2 = source2;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public abstract T Compute(Neighbourhood<S1> n1, Neighbourhood<S2> n2);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override Sector<T> SectorPopulation(in RequstSector requstSector)
		{
			RequstSector outgoingRequstSector = requstSector.GetExpandedCopy(1);
			var sourceSector1 = source.GetSector(outgoingRequstSector);
			var sourceSector2 = source2.GetSector(outgoingRequstSector);
			var sector = new Sector<T>(requstSector);
			for ( int i = Constants.CHUNK_SIZE ; i < sector.Width_units - Constants.CHUNK_SIZE ; i++ ) {
				for ( int j = Constants.CHUNK_SIZE ; j < sector.Height_units - Constants.CHUNK_SIZE ; j++ ) {
					sector[i, j] = Compute(new Neighbourhood<S1>(sourceSector1, i, j), new Neighbourhood<S2>(sourceSector2, i, j));
				}
			}
			return sector;
		}
	}
}
