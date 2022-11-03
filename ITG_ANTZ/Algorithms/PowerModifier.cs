using ITG_Core;
using ITG_Core.Base;

namespace ITG_ANTZ.Algorithms
{
	public class PowerModifier : Layer<float, float>
	{
		public readonly int Power;
		public PowerModifier(Coordinate offset, ITGThreadPool threadPool, Algorithm<float> source, int power) : base(offset, threadPool, source)
		{
			Power = power;
		}
		protected override void ChunkPopulation(out Chunk<float> main, in Chunk<float> request, in Coordinate coordinate)
		{
			main = new Chunk<float>();
			for (int i = 0; i < Constants.CHUNK_SIZE; i++)
			{
				for (int j = 0; j < Constants.CHUNK_SIZE; j++)
				{
					main[i, j] = request[i, j];
				}
			}
			for (int p = 1; p < Power; p++)
			{
				for (int i = 0; i < Constants.CHUNK_SIZE; i++)
				{
					for (int j = 0; j < Constants.CHUNK_SIZE; j++)
					{
						main[i, j] *= request[i, j];
					}
				}
			}
		}

	}
}
