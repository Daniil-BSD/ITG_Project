using System;
using System.Collections.Generic;
using System.Text;

namespace ITG_Core {
	public class FloatShifter : Layer<float, float> {

		private readonly float factor;

		public FloatShifter(Coordinate offset, ITGThreadPool threadPool, Algorithm<float> source, float factor) : base(offset, threadPool, source)
		{
			this.factor = factor;
		}

		protected override Chunk<float> ChunkPopulation(in Coordinate coordinate)
		{
			var original = source.GetChunck(coordinate);
			var ret = new Chunk<float>();
			for ( int i = 0 ; i < Constants.CHUNK_SIZE ; i++ ) {
				for ( int j = 0 ; j < Constants.CHUNK_SIZE ; j++ ) {
					ret[i, j] = original[i, j] * factor;
				}
			}
			return ret;
		}
	}
}
