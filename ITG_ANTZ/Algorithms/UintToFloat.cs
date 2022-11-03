using ITG_Core;
using ITG_Core.Base;

namespace ITG_ANTZ.Algorithms
{
	public class UintToFloat : Layer<float, uint>
	{
		public UintToFloat(Coordinate offset, ITGThreadPool threadPool, Algorithm<uint> source) : base(offset, threadPool, source)
		{
		}

		protected override void ChunkPopulation(out Chunk<float> main, in Chunk<uint> request, in Coordinate coordinate)
		{
			main = new Chunk<float>();
			for (int i = 0; i < Constants.CHUNK_SIZE; i++)
			{
				for (int j = 0; j < Constants.CHUNK_SIZE; j++)
				{
					main[i, j] = ((float)request[i, j]) / uint.MaxValue;
				}
			}
		}
	}
}