namespace ITG_Core {
	using System;
	using System.Runtime.CompilerServices;

	/// <summary>
	/// Defines the <see cref="InterpolatableAlgorithm{T, S}" />
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <typeparam name="S1"></typeparam>
	/// <typeparam name="S2"></typeparam>
	public abstract class InterpolatableAlgorithm<T, S1, S2> : Merger<T, S1, S2> where T : struct where S1 : struct where S2 : struct {
		protected readonly int scale;

		public InterpolatableAlgorithm(Algorithm<S1> source, Algorithm<S2> source2, int scale) : base(source, source2)
		{
			this.scale = scale;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override Chunk<T> ChunkPopulation(in Coordinate coordinate)
		{
			return SectorPopulation(new Sector<T>(coordinate, 1, 1)).Chunks[0, 0];
		}

		/// ---------------------
		/// |	10			11	|
		/// |					|
		/// |			(xy)	|
		/// |					|
		/// |					|
		/// |	00			01	|
		/// ---------------------
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public abstract T Compute(in S1 val00a, in S1 val01a, in S1 val10a, in S1 val11a, in S2 val00b, in S2 val01b, in S2 val10b, in S2 val11b, in float x, in float y, in float offset);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override Sector<T> SectorPopulation(Sector<T> sector)
		{
			float step = 1f / scale;
			float initialOffset = step / 2;
			int corX = sector.coordinate.x;
			int corY = sector.coordinate.y;
			//used to find the corresponding index
			int offsetX = (int) Math.Floor((double) MathExt.Modulo(corX, scale) * Constants.CHUNK_SIZE / scale);
			int offsetY = (int) Math.Floor((double) MathExt.Modulo(corY, scale) * Constants.CHUNK_SIZE / scale);
			//used to find the corresponding intermidiate value index
			float pointOffsetX = MathExt.Modulo(corX * Constants.CHUNK_SIZE, scale) * step;
			float pointOffsetY = MathExt.Modulo(corY * Constants.CHUNK_SIZE, scale) * step;
			//sector values computing
			Coordinate coordinate = new Coordinate(MathExt.IntegerDevisionConsistent(corX, scale), MathExt.IntegerDevisionConsistent(corY, scale));
			int requestWidth = sector.width / scale + 2;
			int requestHeight = sector.height / scale + 2;

			Sector<S1> sourceSector1 = source.GetSector(new Sector<S1>(coordinate, requestWidth, requestHeight));
			Sector<S2> sourceSector2 = source2.GetSector(new Sector<S2>(coordinate, requestWidth, requestHeight));

			sector.FillUp();
			for ( int i = 0 ; i < sector.Width_units ; i++ ) {
				float x = step * i + pointOffsetX;
				int sectorIndexX = (int) Math.Floor(x) + offsetX;
				for ( int j = 0 ; j < sector.Height_units ; j++ ) {
					float y = step * j + pointOffsetY;
					int sectorIndexY = (int) Math.Floor(y) + offsetY;
					sector[i, j] = Compute(
						sourceSector1[sectorIndexX, sectorIndexY],
						sourceSector1[sectorIndexX, sectorIndexY + 1],
						sourceSector1[sectorIndexX + 1, sectorIndexY],
						sourceSector1[sectorIndexX + 1, sectorIndexY + 1],
						sourceSector2[sectorIndexX, sectorIndexY],
						sourceSector2[sectorIndexX, sectorIndexY + 1],
						sourceSector2[sectorIndexX + 1, sectorIndexY],
						sourceSector2[sectorIndexX + 1, sectorIndexY + 1],
						x % 1, y % 1,
						initialOffset
					);
				}
			}
			return sector;
		}
	}
}
