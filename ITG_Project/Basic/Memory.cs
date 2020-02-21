namespace ITG_Core {
	using System.Collections.Generic;
	using System.Runtime.CompilerServices;

	/// <summary>
	/// Defines the <see cref="Memory{T}" />
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class Memory<T> : Algorithm<T> where T : struct {
		private Algorithm<T> algorithm;

		private Dictionary<Coordinate, Chunk<T>> memory;

		public Memory(Algorithm<T> algorithm)
		{
			memory = new Dictionary<Coordinate, Chunk<T>>();
			this.algorithm = algorithm;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override Chunk<T> ChunkPopulation(in Coordinate coordinate)
		{
			if ( !memory.ContainsKey(coordinate) )
				memory.Add(coordinate, algorithm.ChunkPopulation(coordinate));
			return memory[coordinate];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		override public Sector<T> SectorPopulation(Sector<T> sector)
		{
			bool noMissingChunks = true;
			bool noPresentChunks = true;
			for ( int i = 0 ; i < sector.width ; i++ ) {
				for ( int j = 0 ; j < sector.height ; j++ ) {
					Coordinate coordinate = new Coordinate(i + sector.coordinate.x, j + sector.coordinate.y);
					if ( memory.ContainsKey(coordinate) ) {
						sector.Chunks[i, j] = memory[coordinate];
						noPresentChunks = false;
					} else {
						sector.Chunks[i, j] = null;
						noMissingChunks = false;
					}
				}
			}
			if ( noMissingChunks ) {
				return sector;
			} else if ( noPresentChunks ) {
				algorithm.SectorPopulation(sector);
				//saving all the chunks to memory
				for ( int i = 0 ; i < sector.width ; i++ ) {
					for ( int j = 0 ; j < sector.height ; j++ ) {
						Coordinate coordinate = new Coordinate(i + sector.coordinate.x, j + sector.coordinate.y);
						memory.Add(coordinate, sector.Chunks[i, j]);
					}
				}
			} else {
				//some are missing - solved individually
				for ( int i = 0 ; i < sector.width ; i++ ) {
					for ( int j = 0 ; j < sector.height ; j++ ) {
						if ( sector.Chunks[i, j] == null ) {
							Coordinate coordinate = new Coordinate(i + sector.coordinate.x, j + sector.coordinate.y);
							sector.Chunks[i, j] = ChunkPopulation(coordinate);
						}
					}
				}
			}
			return sector;
		}
	}
}
