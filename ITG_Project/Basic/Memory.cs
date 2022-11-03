namespace ITG_Core.Basic
{
	using System;
	using System.Collections.Concurrent;
	using System.Runtime.CompilerServices;
	using ITG_Core.Base;

	public class Memory<T> : Layer<T, T> where T : struct
	{

		private ConcurrentDictionary<Coordinate, Chunk<T>> memory;

		public const bool ASSUME_NO_ADD_CONFLICT = true;

		public Memory(Coordinate offset, ITGThreadPool threadPool, Algorithm<T> source) : base(offset, threadPool, source)
		{
			memory = new ConcurrentDictionary<Coordinate, Chunk<T>>();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void RemoveBottomRowsFromRequest(ref RequstSector requstSector)
		{
			while (requstSector.height > 0)
			{
				CoordinateBasic cell = requstSector.coordinate;
				for (int i = 0; i < requstSector.width; i++)
				{
					if (!memory.ContainsKey(cell))
					{
						return;
					}

					cell.x++;
				}
				requstSector = new RequstSector(requstSector.coordinate + new Coordinate(0, 1), height: requstSector.height - 1, width: requstSector.width);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void RemoveLeftColumnsFromRequest(ref RequstSector requstSector)
		{
			while (requstSector.width > 0)
			{
				CoordinateBasic cell = requstSector.coordinate;
				for (int i = 0; i < requstSector.height; i++)
				{
					if (!memory.ContainsKey(cell))
					{
						return;
					}

					cell.y++;
				}
				requstSector = new RequstSector(requstSector.coordinate + new Coordinate(1, 0), height: requstSector.height, width: requstSector.width - 1);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void RemoveRightColumnsFromRequest(ref RequstSector requstSector)
		{
			while (requstSector.width > 0)
			{
				CoordinateBasic cell = requstSector.coordinate + new Coordinate(requstSector.width - 1, 0);
				for (int i = 0; i < requstSector.height; i++)
				{
					if (!memory.ContainsKey(cell))
					{
						return;
					}

					cell.y++;
				}
				requstSector = new RequstSector(requstSector.coordinate, height: requstSector.height, width: requstSector.width - 1);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void RemoveTopRowsFromRequest(ref RequstSector requstSector)
		{
			while (requstSector.height > 0)
			{
				CoordinateBasic cell = requstSector.coordinate + new Coordinate(0, requstSector.height - 1);
				for (int i = 0; i < requstSector.width; i++)
				{
					if (!memory.ContainsKey(cell))
					{
						return;
					}

					cell.x++;
				}
				requstSector = new RequstSector(requstSector.coordinate, height: requstSector.height - 1, width: requstSector.width);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override Chunk<T> ChunkPopulation(in Coordinate coordinate)
		{
			if (!memory.ContainsKey(coordinate))
			{
				TrySavingSector(coordinate, source.GetChunck(coordinate));
			}
			return memory[coordinate];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override Sector<T> SectorPopulation(in RequstSector requstSector)
		{
			int missingChunks = 0;
			int presentChunks = 0;
			var sector = new Sector<T>(requstSector);
			for (int i = 0; i < sector.width; i++)
			{
				for (int j = 0; j < sector.height; j++)
				{
					Coordinate coordinate = new Coordinate(i, j) + sector.Coordinate;
					if (memory.ContainsKey(coordinate))
					{
						sector.Chunks[i, j] = memory[coordinate];
						presentChunks++;
					}
					else
					{
						sector.Chunks[i, j] = null;
						missingChunks++;
					}
				}
			}
			if (missingChunks == 0)
			{
				return sector;
			}
			else if (presentChunks == 0)
			{
				sector = source.GetSector(sector);
				for (int i = 0; i < sector.width; i++)
				{
					for (int j = 0; j < sector.height; j++)
					{
						Coordinate coordinate = new Coordinate(i, j) + sector.Coordinate;
						TrySavingSector(coordinate, sector.Chunks[i, j]);
					}
				}
			}
			else
			{
				//some are missing - solved by requesting the whole thing individually
				RequstSector outgoingRequstSector = requstSector;

				if (missingChunks <= presentChunks)
				{
					RemoveTopRowsFromRequest(ref outgoingRequstSector);
					RemoveRightColumnsFromRequest(ref outgoingRequstSector);
					RemoveBottomRowsFromRequest(ref outgoingRequstSector);
					RemoveLeftColumnsFromRequest(ref outgoingRequstSector);
				}

				//Console.WriteLine(outgoingRequstSector.width * outgoingRequstSector.height + "\t\t" + requstSector.width * requstSector.height);
				Sector<T> requestedSector = source.GetSector(outgoingRequstSector);
				for (int i = 0; i < requestedSector.width; i++)
				{
					for (int j = 0; j < requestedSector.height; j++)
					{
						Coordinate coordinate = new Coordinate(i, j) + requestedSector.Coordinate;
						TrySavingSector(coordinate, requestedSector.Chunks[i, j]);
					}
				}
				return SectorPopulation(requstSector);
			}
			return sector;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected void TrySavingSector(in Coordinate key, in Chunk<T> value)
		{
			if (!memory.TryAdd(key, value))
			{
				if (!ASSUME_NO_ADD_CONFLICT && !memory[key].Equals(value))
				{
					throw new PushConflictException<T>(memory[key], value);
				}
			}
		}

		public override void Drop()
		{
			memory.Clear();
		}
	}

	/// <summary>
	/// Defines the <see cref="PushConflictException{T}" />
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class PushConflictException<T> : Exception where T : struct
	{

		public readonly object inMemory;

		public readonly object pushed;

		public override string Message => "\noriginal:\t" + inMemory + "\nnew:\t\t" + pushed;

		public PushConflictException(object inMemory, object pushed)
		{
			this.inMemory = inMemory;
			this.pushed = pushed;
		}
	}
}