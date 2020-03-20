using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ITG_Core.Base;

namespace ITG_Core.Basic {

	public class MemoryStrictSectoring<T> : Layer<T, T> where T : struct {

		private ConcurrentDictionary<Coordinate, SectorPlaceholder> memory;

		public readonly int sectorSize;

		public override sealed int StdSectorSize => int.MaxValue;

		public MemoryStrictSectoring(Coordinate offset, ITGThreadPool threadPool, Algorithm<T> algorithm, int sectorSize) : base(offset, threadPool, algorithm)
		{
			this.sectorSize = sectorSize;
			memory = new ConcurrentDictionary<Coordinate, SectorPlaceholder>();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private Sector<T> GetSectorByCoordinate(in Coordinate coordinate)
		{
			if ( memory.ContainsKey(coordinate) )
				return memory[coordinate];
			RequstSector requestSector = GetSectorRequestByCoordinate(coordinate);
			Sector<T> sector = source.GetSector(requestSector);
			memory.TryAdd(coordinate, sector);
			return sector;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private SectorPlaceholder GetSectorPlaceholderByCoordinate(in Coordinate coordinate)
		{
			if ( memory.ContainsKey(coordinate) )
				return memory[coordinate];
			RequstSector requestSector = GetSectorRequestByCoordinate(coordinate);
			SectorPlaceholder placeholder = new SectorPlaceholder(new SectorJob<T>(requestSector, SectorPopulationDelegateImpl));
			memory.TryAdd(coordinate, placeholder);
			return placeholder;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private RequstSector GetSectorRequestByCoordinate(in Coordinate coordinate)
		{
			return new RequstSector(coordinate * sectorSize, sectorSize, sectorSize);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private Sector<T> SectorPopulationDelegateImpl(in RequstSector requstSector)
		{
			return source.GetSector(requstSector);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override Chunk<T> ChunkPopulation(in Coordinate coordinate)
		{
			Coordinate key = coordinate / sectorSize;
			return GetSectorByCoordinate(key).GetChunk(coordinate % sectorSize);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override Sector<T> SectorPopulation(in RequstSector requstSector)
		{
			Coordinate requestOffset = requstSector.coordinate % sectorSize;
			Coordinate requestOrigin = requstSector.coordinate / sectorSize;
			int widthInSectors = ( ( ( requstSector.width - 1 + requestOffset.x ) / sectorSize ) + 1 );
			int heightInSectors = ( ( ( requstSector.height - 1 + requestOffset.y ) / sectorSize ) + 1 );

			SectorPlaceholder[] sectorPlaceholders = new SectorPlaceholder[widthInSectors * heightInSectors];
			for ( int x = 0 ; x < widthInSectors ; x++ ) {
				for ( int y = 0 ; y < heightInSectors ; y++ ) {
					Coordinate coordinate = new Coordinate(x, y) + requestOrigin;
					sectorPlaceholders[x * heightInSectors + y] = GetSectorPlaceholderByCoordinate(coordinate);
				}
			}
			SectorJob<T>[] incompleteJobs = SectorPlaceholder.GetIncompleteJobs(sectorPlaceholders);
			if ( incompleteJobs.Length > 0 ) {
				threadPool.Execute(incompleteJobs);
			}

			Sector<T> sector = new Sector<T>(requstSector);
			for ( int x = 0 ; x < requstSector.width ; x++ ) {
				for ( int y = 0 ; y < requstSector.height ; y++ ) {
					Coordinate chunkCoordinate = new Coordinate(x, y) + requstSector.coordinate;
					sector.Chunks[x, y] = GetSectorByCoordinate(chunkCoordinate / sectorSize).GetChunk(chunkCoordinate % sectorSize);
				}
			}
			return sector;
		}

		public override void Drop()
		{
			memory.Clear();
		}

		private class SectorPlaceholder {

			private Sector<T> sector;

			private SectorJob<T> sectorJob;

			public bool IsComplete
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => sector != null;
			}

			public Sector<T> Sector
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get {
					if ( IsComplete )
						return sector;
					sector = sectorJob.ExecuteFromMainThread();
					sectorJob = null;
					return sector;
				}
			}

			public SectorPlaceholder(SectorJob<T> sectorJob)
			{
				sector = null;
				this.sectorJob = sectorJob;
			}

			public SectorPlaceholder(Sector<T> sector)
			{
				this.sector = sector;
				sectorJob = null;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static SectorJob<T>[] GetIncompleteJobs(SectorPlaceholder[] placeholders)
			{
				List<SectorJob<T>> jobs = new List<SectorJob<T>>(placeholders.Length);
				for ( int i = 0 ; i < placeholders.Length ; i++ ) {
					SectorJob<T> job = placeholders[i].sectorJob;
					if ( job != null )
						jobs.Add(job);
				}
				return jobs.ToArray();
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static implicit operator Sector<T>(SectorPlaceholder placeholder)
			{
				return placeholder.Sector;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static implicit operator SectorPlaceholder(Sector<T> sector)
			{
				return new SectorPlaceholder(sector);
			}
		}
	}
}