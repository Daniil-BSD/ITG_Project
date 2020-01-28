namespace ITG_Core {
	using System;
	using System.Runtime.CompilerServices;

	/// <summary>
	/// Defines the <see cref="InterpolatableAlgorithm{T, S}" />
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <typeparam name="S"></typeparam>
	public abstract class InterpolatableAlgorithm<T, S> : Algorithm<T> where T : struct where S : struct {
		protected readonly int scale;

		protected readonly Algorithm<S> source;

		public InterpolatableAlgorithm(Algorithm<S> source, int scale)
		{
			this.source = source;
			this.scale = scale;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override Chunk<T> ChunkPopulation(Coordinate coordinate)
		{
			return SectorPopulation(new Sector<T>(coordinate, 1, 1)).Chunks[0, 0];
		}

		public abstract T Compute(S val00, S val01, S val10, S val11, float x, float y, float offset);

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
			Sector<S> sourceSector = source.GetSector(new Sector<S>(new Coordinate(MathExt.IntegerDevisionConsistent(corX, scale), MathExt.IntegerDevisionConsistent(corY, scale)), sector.width / scale + 2, sector.height / scale + 2));
			sector.FillUp();
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
