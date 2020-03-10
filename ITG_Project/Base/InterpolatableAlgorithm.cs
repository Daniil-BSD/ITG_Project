namespace ITG_Core {
	using System;
	using System.Runtime.CompilerServices;

	/// <summary>
	/// Defines the <see cref="InterpolatableAlgorithm{T, S}" />
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <typeparam name="S"></typeparam>
	public abstract class InterpolatableAlgorithm<T, S> : Layer<T, S> where T : struct where S : struct {
		public readonly int scale;

		public InterpolatableAlgorithm(Coordinate offset, ITGThreadPool threadPool, Algorithm<S> source, int scale) : base(offset, threadPool, source)
		{
			this.scale = scale;
		}

		/// ---------------------
		/// |	10			11	|
		/// |					|
		/// |			(xy)	|
		/// |					|
		/// |					|
		/// |	00			01	|
		/// ---------------------
		public abstract T Compute(in S val00, in S val01, in S val10, in S val11, in float x, in float y, in float offset);


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override Chunk<T> ChunkPopulation(in Coordinate coordinate)
		{
			return SectorPopulation(new RequstSector(coordinate, 1, 1)).Chunks[0, 0];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override Sector<T> SectorPopulation(in RequstSector requstSector)
		{
			float step = 1f / scale;
			float initialOffset = step / 2;
			int corX = requstSector.coordinate.x;
			int corY = requstSector.coordinate.y;
			//used to find the corresponding index
			int offsetX = (int) Math.Floor((double) corX.Modulo(scale) * Constants.CHUNK_SIZE / scale);
			int offsetY = (int) Math.Floor((double) corY.Modulo(scale) * Constants.CHUNK_SIZE / scale);
			//used to find the corresponding intermidiate value index
			float pointOffsetX = MathExt.Modulo(corX * Constants.CHUNK_SIZE, scale) * step;
			float pointOffsetY = MathExt.Modulo(corY * Constants.CHUNK_SIZE, scale) * step;
			Sector<S> sourceSector = source.GetSector(new RequstSector(new Coordinate(corX.IntegerDevisionConsistent(scale), corY.IntegerDevisionConsistent(scale)), requstSector.width / scale + 2, requstSector.height / scale + 2));
			Sector<T> sector = new Sector<T>(requstSector);
			for ( int i = 0 ; i < sector.Width_units ; i++ ) {
				float x = step * i + pointOffsetX;
				int sectorIndexX = (int) Math.Floor(x) + offsetX;
				for ( int j = 0 ; j < sector.Height_units ; j++ ) {
					float y = step * j + pointOffsetY;
					int sectorIndexY = (int) Math.Floor(y) + offsetY;
					sector[i, j] = Compute(
						sourceSector[sectorIndexX, sectorIndexY],
						sourceSector[sectorIndexX, sectorIndexY + 1],
						sourceSector[sectorIndexX + 1, sectorIndexY],
						sourceSector[sectorIndexX + 1, sectorIndexY + 1],
						x % 1, y % 1,
						initialOffset
					);
				}
			}
			return sector;
		}
	}
}
