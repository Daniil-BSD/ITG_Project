namespace ITG_Core {
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Runtime.CompilerServices;




	public class Sector<T> where T : struct {

		public readonly Coordinate coordinate;
		private Chunk<T>[,] chunks;
		public readonly int width;
		public readonly int height;


		//public T this[int x, int y] => chunks[x / Constants.CHUNK_SIZE, y / Constants.CHUNK_SIZE][x % Constants.CHUNK_SIZE, y % Constants.CHUNK_SIZE];
		public T this[in int x, in int y] {
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get {
				//Console.WriteLine(x + " , " + y);
				return chunks[x / Constants.CHUNK_SIZE, y / Constants.CHUNK_SIZE][x % Constants.CHUNK_SIZE, y % Constants.CHUNK_SIZE];
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set {
				chunks[x / Constants.CHUNK_SIZE, y / Constants.CHUNK_SIZE][x % Constants.CHUNK_SIZE, y % Constants.CHUNK_SIZE] = value;
			}
		}
		public Chunk<T>[,] Chunks => chunks;
		public int Width_units => width * Constants.CHUNK_SIZE;
		public int Height_units => height * Constants.CHUNK_SIZE;
		public Sector(Coordinate coordinate, in int width, in int height)
		{
			this.chunks = new Chunk<T>[width, height];
			this.width = width;
			this.height = height;
			this.coordinate = coordinate;
		}
		public Sector(Coordinate coordinate, int width, int height)
		{
			this.chunks = new Chunk<T>[width, height];
			this.width = width;
			this.height = height;
			this.coordinate = coordinate;
		}

		public IEnumerator GetEnumerator()
		{
			throw new NotImplementedException();
		}

		public ValueEnumerator GetValueEnumerator()
		{
			FillUp();
			return new ValueEnumerator(this);
		}

		public ChunkEnumerator GetChunkEnumerator()
		{
			FillUp();
			return new ChunkEnumerator(this);
		}

		public void FillUp()
		{
			for ( int i = 0 ; i < width ; i++ )
				for ( int j = 0 ; j < height ; j++ )
					if ( chunks[i, j] == null )
						chunks[i, j] = new Chunk<T>();
		}

		public override string ToString()
		{
			string ret = "";
			for ( int i = 0 ; i < width ; i++ ) {
				for ( int j = 0 ; j < height ; j++ ) {
					ret += chunks[i, j].ToString() + "\t";
				}
				ret += "\n";
			}
			return ret + "";
		}

#pragma warning disable CS0693 // Type parameter has the same name as the type parameter from outer type
		public class ChunkEnumerator : IEnumeratorPlus<Chunk<T>> {
			private IEnumerator<Chunk<T>> chunkEnumerator;
			public Chunk<T> Current => chunkEnumerator.Current;
			object IEnumerator.Current => Current;


			//TODO:
			public int X => throw new NotImplementedException();

			public int Y => throw new NotImplementedException();

			public ChunkEnumerator(Sector<T> area)
			{
				chunkEnumerator = (IEnumerator<Chunk<T>>) area.chunks.GetEnumerator();
			}

			public bool MoveNext()
			{

				return chunkEnumerator.MoveNext();
			}

			public void Reset()
			{
				chunkEnumerator.Reset();
			}

			public void Dispose()
			{
				chunkEnumerator.Dispose();
			}

			public IEnumerator<Chunk<T>> GetEnumerator()
			{
				return this;
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return this;
			}

			public void SetCurernt<T1>(T1 t) where T1 : struct
			{
				throw new NotImplementedException();
			}
		}

		public class ValueEnumerator : IEnumeratorPlus<T> {
			private Sector<T> sector;
			private IEnumerator chunkEnumerator;
			private IEnumerator valueEnumerator;
			public T Current => (T) valueEnumerator.Current;
			object IEnumerator.Current => Current;
			private int chunkIndex;
			private int valueIndex;
			private int sectorWidth;
			public int X {
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get {
					return (chunkIndex % sectorWidth) * Constants.CHUNK_SIZE + valueIndex % Constants.CHUNK_SIZE;
				}
			}

			public int Y {
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get {
					return (chunkIndex / sectorWidth) * Constants.CHUNK_SIZE + valueIndex / Constants.CHUNK_SIZE;
				}
			}

			public ValueEnumerator(Sector<T> sector)
			{
				this.sector = sector;
				chunkEnumerator = (IEnumerator) sector.chunks.GetEnumerator();
				chunkEnumerator.MoveNext();
				valueEnumerator = ((Chunk<T>) chunkEnumerator.Current).value.GetEnumerator();
				valueEnumerator.MoveNext();
				sectorWidth = sector.width;
				this.sector = sector;
				chunkIndex = 0;
				valueIndex = 0;
			}

			public void Dispose()
			{
				//valueEnumerator.Dispose();
				//chunkEnumerator.Dispose();
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public bool MoveNext()
			{
				if ( !valueEnumerator.MoveNext() ) {
					if ( chunkEnumerator.MoveNext() ) {
						chunkIndex++;
						valueIndex = 0;
						valueEnumerator = ((Chunk<T>) chunkEnumerator.Current).value.GetEnumerator();
						valueEnumerator.MoveNext();
						return true;
					} else {
						return false;
					}
				}
				valueIndex++;
				return true;
			}

			public void Reset()
			{
				throw new NotImplementedException();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return this;
			}

			IEnumerator<T> IEnumerable<T>.GetEnumerator()
			{
				return this;
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public void SetCurernt(T t)
			{
				//Console.WriteLine(valueIndex % Constants.CHUNK_SIZE + " , " + valueIndex / Constants.CHUNK_SIZE);
				((Chunk<T>) chunkEnumerator.Current)[valueIndex / Constants.CHUNK_SIZE, valueIndex % Constants.CHUNK_SIZE] = t;
			}
		}
#pragma warning restore CS0693 // Type parameter has the same name as the type parameter from outer type
	}
}
