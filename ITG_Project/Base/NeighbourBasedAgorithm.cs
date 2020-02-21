namespace ITG_Core {
	using System.Runtime.CompilerServices;

	/// <summary>
	/// Defines the <see cref="InterpolatableAlgorithm{T, S}" />
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <typeparam name="S"></typeparam>
	public abstract class NeighbourBasedAgorithm<T, S> : Layer<T, S> where T : struct where S : struct {
		public NeighbourBasedAgorithm(Algorithm<S> source) : base(source)
		{
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override Chunk<T> ChunkPopulation(in Coordinate coordinate)
		{
			return SectorPopulation(new Sector<T>(coordinate, 1, 1)).Chunks[0, 0];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public abstract T Compute(Neighbourhood<S> n);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override Sector<T> SectorPopulation(Sector<T> sector)
		{
			Sector<S> sourceSector = new Sector<S>(new Coordinate(sector.coordinate.x - 1, sector.coordinate.y - 1), sector.width + 2, sector.height + 2);
			sourceSector = source.GetSector(sourceSector);
			sector.FillUp();
			for ( int i = Constants.CHUNK_SIZE ; i < sector.Width_units - Constants.CHUNK_SIZE ; i++ ) {
				for ( int j = Constants.CHUNK_SIZE ; j < sector.Height_units - Constants.CHUNK_SIZE ; j++ ) {
					sector[i, j] = Compute(new Neighbourhood<S>(sourceSector, i, j));
					;
				}
			}
			return sector;
		}


		public class Neighbourhood<NT> where NT : struct {
			public readonly NT[,] data;
			public NT this[in int x, in int y] {
				get {
					return data[x + 1, y + 1];
				}
			}
			public Neighbourhood(Sector<NT> s, in int x, in int y)
			{
				data = new NT[3, 3]{
					{ s[x - 1, y + 1]   , s[x, y + 1]   , s[x + 1, y + 1]},
					{ s[x - 1, y]       , s[x, y]       , s[x + 1, y]},
					{ s[x - 1, y - 1]   , s[x, y - 1]   , s[x + 1, y - 1]}
				};

			}

		}
	}
}
