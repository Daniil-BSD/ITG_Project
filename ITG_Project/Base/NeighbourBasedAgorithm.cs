namespace ITG_Core.Base {
	using System.Runtime.CompilerServices;

	/// <summary>
	/// Defines the <see cref="InterpolatableAlgorithm{T, S}" />
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <typeparam name="S"></typeparam>
	public abstract class NeighbourBasedAgorithm<T, S> : Layer<T, S> where T : struct where S : struct {

		public NeighbourBasedAgorithm(Coordinate offset, ITGThreadPool threadPool, Algorithm<S> source) : base(offset, threadPool, source)
		{
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override Chunk<T> ChunkPopulation(in Coordinate coordinate)
		{
			return SectorPopulation(new RequstSector(coordinate, 1, 1)).Chunks[0, 0];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override Sector<T> SectorPopulation(in RequstSector requstSector)
		{
			RequstSector outgoingRequstSector = requstSector.GetExpandedCopy(1);
			Sector<S> sourceSector = source.GetSector(outgoingRequstSector);
			Sector<T> sector = new Sector<T>(requstSector);
			for ( int i = 0 ; i < sector.Width_units ; i++ ) {
				for ( int j = 0 ; j < sector.Height_units ; j++ ) {
					sector[i, j] = Compute(new Neighbourhood<S>(sourceSector, i, j));
				}
			}
			return sector;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public abstract T Compute(Neighbourhood<S> n);

		public class Neighbourhood<NT> where NT : struct {

			public readonly NT[,] data;

			public Neighbourhood(Sector<NT> s, in int i, in int j)
			{
				int x = i + Constants.CHUNK_SIZE;
				int y = j + Constants.CHUNK_SIZE;
				int xn = x - 1;
				int xo = x;
				int xp = x + 1;
				int yn = y - 1;
				int yo = y;
				int yp = y + 1;
				data = new NT[3, 3]{
					{ s[xn, yp] , s[xo, yp] , s[xp, yp]},
					{ s[xn, yo] , s[xo, yo] , s[xp, yo]},
					{ s[xn, yn] , s[xo, yn] , s[xp, yn]}
				};
			}

			public NT this[in int x, in int y] => data[x + 1, y + 1];
		}
	}
}