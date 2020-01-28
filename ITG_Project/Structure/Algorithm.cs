namespace ITG_Core {
	using System.Runtime.CompilerServices;

	/// <summary>
	/// Defines the <see cref="AlgorithmBuilder{T}" />
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class AlgorithmBuilder<T> where T : struct {
		abstract public Algorithm<T> Build();
	}

	/// <summary>
	/// Defines the <see cref="Algorithm{T}" />
	/// </summary>
	/// <typeparam name="T"></typeparam>
	abstract public class Algorithm<T> where T : struct {
		private readonly int stdSectorSide = Constants.CHUNK_SIZE / 2;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Chunk<T> GetChunck(Coordinate coordinate)
		{
			return ChunkPopulation(coordinate);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Sector<T> GetSector(Sector<T> sector)
		{
			int subsectorsX = sector.width / stdSectorSide;
			int subsectorsY = sector.height / stdSectorSide;
			if ( subsectorsX <= 1 && subsectorsY <= 1 )
				return SectorPopulation(sector);
			Sector<T>[,] subsectors = new Sector<T>[subsectorsX, subsectorsY];
			//Console.WriteLine(subsectorsX + " , " + subsectorsY);
			for ( int i = 0 ; i < subsectorsX ; i++ ) {
				for ( int j = 0 ; j < subsectorsY ; j++ ) {
					int width = stdSectorSide;
					int height = stdSectorSide;
					if ( i == subsectorsX - 1 )
						width = sector.width - stdSectorSide * i;
					if ( j == subsectorsY - 1 )
						height = sector.height - stdSectorSide * j;
					//Console.WriteLine(width + " , " + height);
					subsectors[i, j] = new Sector<T>(new Coordinate(sector.coordinate.x + (i * stdSectorSide), sector.coordinate.y + (j * stdSectorSide)), width, height);

					subsectors[i, j] = SectorPopulation(subsectors[i, j]);
				}
			}

			for ( int si = 0 ; si < subsectorsX ; si++ ) {
				for ( int sj = 0 ; sj < subsectorsY ; sj++ ) {
					for ( int i = 0 ; i < subsectors[si, sj].width ; i++ ) {
						for ( int j = 0 ; j < subsectors[si, sj].height ; j++ ) {

							//Console.Write((si * stdSectorSide + i) + " , " + (sj * stdSectorSide + j) + " \t");
							sector.Chunks[si * stdSectorSide + i, sj * stdSectorSide + j] = subsectors[si, sj].Chunks[i, j];
						}
					}
				}
			}
			return sector;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		abstract public Chunk<T> ChunkPopulation(Coordinate coordinate);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		virtual public Sector<T> SectorPopulation(Sector<T> sector)
		{
			for ( int i = 0 ; i < sector.width ; i++ ) {
				for ( int j = 0 ; j < sector.height ; j++ ) {
					sector.Chunks[i, j] = ChunkPopulation(new Coordinate(i + sector.coordinate.x, j + sector.coordinate.y));
				}
			}
			return sector;
		}
	}
}
