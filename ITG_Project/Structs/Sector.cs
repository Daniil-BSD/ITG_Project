namespace ITG_Core {
	using System;
	using System.Runtime.CompilerServices;

	/// <summary>
	/// Defines the <see cref="RequstSector" />
	/// </summary>
	public struct RequstSector {
		public readonly Coordinate coordinate;

		public readonly int height;

		public readonly int width;

		public int Height_units => height * Constants.CHUNK_SIZE;

		public int Width_units => width * Constants.CHUNK_SIZE;

		public RequstSector(in Coordinate coordinate, in int width, in int height)
		{
			this.width = width;
			this.height = height;
			this.coordinate = new Coordinate(coordinate);
		}

		public RequstSector(RequstSector original, in int expansion)
		{
			width = original.width + expansion + expansion;
			height = original.height + expansion + expansion;
			coordinate = original.coordinate - new Coordinate(expansion, expansion);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public RequstSector GetExpandedCopy(in int expansion)
		{
			return new RequstSector(this, expansion);
		}

		public RequstSector GetOffsetCopy(Coordinate offset)
		{
			return new RequstSector(coordinate + offset, width, height);
		}
	}

	public class Sector<T> where T : struct {

		private Coordinate coordinate;
		private Chunk<T>[,] chunks;
		public readonly int width;
		public readonly int height;


		//public T this[int x, int y] => chunks[x / Constants.CHUNK_SIZE, y / Constants.CHUNK_SIZE][x % Constants.CHUNK_SIZE, y % Constants.CHUNK_SIZE];
		public T this[in int x, in int y] {
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get {
				return chunks[x / Constants.CHUNK_SIZE, y / Constants.CHUNK_SIZE][x % Constants.CHUNK_SIZE, y % Constants.CHUNK_SIZE];
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set {
				chunks[x / Constants.CHUNK_SIZE, y / Constants.CHUNK_SIZE][x % Constants.CHUNK_SIZE, y % Constants.CHUNK_SIZE] = value;
			}
		}

		public T this[in CoordinateBasic coordinate] {
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get {
				return chunks[coordinate.x / Constants.CHUNK_SIZE, coordinate.y / Constants.CHUNK_SIZE][coordinate.x % Constants.CHUNK_SIZE, coordinate.y % Constants.CHUNK_SIZE];
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set {
				chunks[coordinate.x / Constants.CHUNK_SIZE, coordinate.y / Constants.CHUNK_SIZE][coordinate.x % Constants.CHUNK_SIZE, coordinate.y % Constants.CHUNK_SIZE] = value;
			}
		}


		public Chunk<T>[,] Chunks => chunks;
		public Coordinate Coordinate => coordinate;
		public int Width_units => width * Constants.CHUNK_SIZE;
		public int Height_units => height * Constants.CHUNK_SIZE;
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Sector(in Coordinate coordinate, in int width, in int height, in bool fillup = true)
		{
			this.chunks = new Chunk<T>[width, height];
			this.width = width;
			this.height = height;
			this.coordinate = new Coordinate(coordinate);
			if ( fillup )
				FillUp();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Sector(in RequstSector requstSector, in bool fillup = true)
		{
			this.chunks = new Chunk<T>[requstSector.width, requstSector.height];
			this.width = requstSector.width;
			this.height = requstSector.height;
			this.coordinate = requstSector.coordinate;
			if ( fillup )
				FillUp();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void FillUp()
		{
			for ( int i = 0 ; i < width ; i++ )
				for ( int j = 0 ; j < height ; j++ )
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
		public delegate void ForeachChunkDelegate<T1, T2, P>(ref Chunk<T1> main, in Chunk<T2> secondary, in P param) where T1 : struct where T2 : struct;
		public static void ForeachChunk<T1, T2, P>(ForeachChunkDelegate<T1, T2, P> body, ref Sector<T1> sector1, in Sector<T2> sector2, in P param) where T1 : struct where T2 : struct
		{
			if ( !sector1.Equals(sector2) )
				throw new ArgumentException("Mismatched sectors.");
			for ( int i = 0 ; i < sector1.width ; i++ )
				for ( int j = 0 ; j < sector1.height ; j++ )
					body(ref sector1.chunks[i, j], sector2.chunks[i, j], param);
		}

		public delegate void ChunkPopulationDelegate<S>(out Chunk<T> main, in Chunk<S> request, in Coordinate coordinate) where S : struct;
		public static void ForeachChunk<S>(ChunkPopulationDelegate<S> ChunkPopulation, ref Sector<T> main, in Sector<S> request) where S : struct
		{
			if ( !main.Equals(request) )
				throw new ArgumentException("Mismatched sectors.");
			for ( int i = 0 ; i < main.width ; i++ )
				for ( int j = 0 ; j < main.height ; j++ )
					ChunkPopulation(out main.chunks[i, j], request.chunks[i, j], main.coordinate + new Coordinate(i, j));
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public T ValueAtDefault(in float x, in float y)
		{

			int coorX = (x + 0.5f).ToIntegerConsistent();
			int coorY = (y + 0.5f).ToIntegerConsistent();

			return this[coorX, coorY];
		}

		public Sector<T> GetCopy()
		{
			Sector<T> copy = new Sector<T>(coordinate, width, height, fillup: false);
			for ( int i = 0 ; i < width ; i++ )
				for ( int j = 0 ; j < height ; j++ )
					copy.chunks[i, j] = chunks[i, j];
			return copy;
		}
		public Sector<T> GetDeepCopy()
		{
			Sector<T> copy = new Sector<T>(coordinate, width, height, fillup: false);
			for ( int i = 0 ; i < width ; i++ ) {
				for ( int j = 0 ; j < height ; j++ ) {
					copy.chunks[i, j] = chunks[i, j].GetCopy();
				}
			}
			return copy;
		}

		public Sector<T> GetCopy(int reductionRadius)
		{
			int reductionRadius2 = reductionRadius + reductionRadius;
			Sector<T> copy = new Sector<T>(coordinate + new Coordinate(reductionRadius, reductionRadius), width - reductionRadius2, height - reductionRadius2, fillup: false);
			for ( int i = reductionRadius ; i < width - reductionRadius ; i++ )
				for ( int j = reductionRadius ; j < height - reductionRadius ; j++ )
					copy.chunks[i - reductionRadius, j - reductionRadius] = chunks[i, j];
			return copy;
		}

		public Sector<T> OffsetBack(Coordinate offset)
		{
			coordinate = coordinate - offset;
			return this;
		}

		// override object.Equals
		public override bool Equals(object obj)
		{
			if ( obj == null || GetType() != obj.GetType() ) {
				return false;
			}
			var sec = (Sector<T>) obj;
			return sec.Coordinate.Equals(coordinate) && sec.width == width && sec.height == height;
		}
		public bool Equals<S>(Sector<S> sec) where S : struct
		{
			return sec.Coordinate.Equals(coordinate) && sec.width == width && sec.height == height;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator RequstSector(in Sector<T> sector)
		{
			return new RequstSector(sector.coordinate, sector.width, sector.height);
		}
	}

	/// <summary>
	/// Defines the <see cref="SectorExtentions" />
	/// </summary>
	public static unsafe partial class SectorExtentions {
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float ValueAt(this Sector<float> sector, in float x, in float y)
		{
			int coorX = x.ToIntegerConsistent();
			int coorY = y.ToIntegerConsistent();
			int coorXpo = coorX + 1;
			int coorYpo = coorX + 1;

			float val00 = sector[coorX, coorY];
			float val10 = sector[coorXpo, coorY];
			float val01 = sector[coorX, coorYpo];
			float val11 = sector[coorXpo, coorYpo];

			float X = x.Modulo(1);
			float Y = x.Modulo(1);

			float top = val01 + X * (val11 - val01);
			float bottom = val00 + X * (val10 - val00);
			float ret = (bottom + Y * (top - bottom));
			return ret;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float ValueAt(this Sector<float> sector, in Vec2 vec)
		{
			return sector.ValueAt(vec.x, vec.y);
		}
	}
}
