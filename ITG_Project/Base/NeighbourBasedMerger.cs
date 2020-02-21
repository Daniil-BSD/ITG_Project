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

		public NeighbourBasedMerger(Algorithm<S1> source, Algorithm<S2> source2) : base(source)
		{
			this.source2 = source2;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public abstract T Compute(Neighbourhood<S1> n1, Neighbourhood<S2> n2);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override Sector<T> SectorPopulation(Sector<T> sector)
		{
			Coordinate coordinate = new Coordinate(sector.coordinate.x - 1, sector.coordinate.y - 1);
			var requestWidth = sector.width + 2;
			int requestHeight = sector.height + 2;
			Sector<S1> sourceSector1 = new Sector<S1>(coordinate, requestWidth, requestHeight);
			Sector<S2> sourceSector2 = new Sector<S2>(coordinate, requestWidth, requestHeight);
			sourceSector1 = source.GetSector(sourceSector1);
			sourceSector2 = source2.GetSector(sourceSector2);
			sector.FillUp();
			for ( int i = Constants.CHUNK_SIZE ; i < sector.Width_units - Constants.CHUNK_SIZE ; i++ ) {
				for ( int j = Constants.CHUNK_SIZE ; j < sector.Height_units - Constants.CHUNK_SIZE ; j++ ) {
					sector[i, j] = Compute(new Neighbourhood<S1>(sourceSector1, i, j), new Neighbourhood<S2>(sourceSector2, i, j));
				}
			}
			return sector;
		}
	}
}
