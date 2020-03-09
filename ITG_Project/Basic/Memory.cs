namespace ITG_Core {
	using System;
	using System.Collections.Concurrent;
	using System.Runtime.CompilerServices;

	/// <summary>
	/// Defines the <see cref="Memory{T}" />
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class Memory<T> : Algorithm<T> where T : struct {
		private Algorithm<T> algorithm;

		private ConcurrentDictionary<Coordinate, Chunk<T>> memory;

		public Memory(Coordinate offset, Algorithm<T> algorithm) : base(offset)
		{
			memory = new ConcurrentDictionary<Coordinate, Chunk<T>>();
			this.algorithm = algorithm;
		}

		public void Drop()
		{
			memory.Clear();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void PushData(in Chunk<T> chunk, in Coordinate coordinate)
		{
			TrySavingSector(coordinate, chunk);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override Chunk<T> ChunkPopulation(in Coordinate coordinate)
		{
			TrySavingSector(coordinate, algorithm.GetChunck(coordinate));
			return memory[coordinate];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override Sector<T> SectorPopulation(in RequstSector requstSector)
		{
			bool noMissingChunks = true;
			bool noPresentChunks = true;
			Sector<T> sector = new Sector<T>(requstSector);
			for ( int i = 0 ; i < sector.width ; i++ ) {
				for ( int j = 0 ; j < sector.height ; j++ ) {
					Coordinate coordinate = new Coordinate(i, j) + sector.Coordinate;
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
				sector = algorithm.GetSector(sector);
				for ( int i = 0 ; i < sector.width ; i++ ) {
					for ( int j = 0 ; j < sector.height ; j++ ) {
						Coordinate coordinate = new Coordinate(i, j) + sector.Coordinate;
						TrySavingSector(coordinate, sector.Chunks[i, j]);
					}
				}
			} else {
				//some are missing - solved individually
				for ( int i = 0 ; i < sector.width ; i++ ) {
					for ( int j = 0 ; j < sector.height ; j++ ) {
						if ( sector.Chunks[i, j] == null ) {
							Coordinate coordinate = new Coordinate(i, j) + sector.Coordinate;
							sector.Chunks[i, j] = ChunkPopulation(coordinate);
						}
					}
				}
			}
			return sector;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected void TrySavingSector(in Coordinate key, in Chunk<T> value)
		{
			if ( !memory.TryAdd(key, value) )
				if ( !memory[key].Equals(value) )
					throw new PushConflictException<T>(memory[key], value);
		}
	}

	/// <summary>
	/// Defines the <see cref="PushConflictException{T}" />
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class PushConflictException<T> : Exception where T : struct {
		public readonly Chunk<T> inMemory;

		public readonly Chunk<T> pushed;

		public override string Message => "\noriginal:\t" + inMemory + "\nnew:\t\t" + pushed;

		public PushConflictException(Chunk<T> inMemory, Chunk<T> pushed)
		{
			this.inMemory = inMemory;
			this.pushed = pushed;
		}
	}
}
