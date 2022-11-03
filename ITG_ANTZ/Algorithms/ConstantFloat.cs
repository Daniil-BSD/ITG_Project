using ITG_Core;
using ITG_Core.Base;

namespace ITG_ANTZ.Algorithms
{
	public class ConstantFloat : Algorithm<float>
	{
		public readonly float value;

		public ConstantFloat(Coordinate offset, ITGThreadPool threadPool, float value) : base(offset, threadPool)
		{
			this.value = value;
		}

		protected override Chunk<float> ChunkPopulation(in Coordinate coordinate)
		{
			var ret = new Chunk<float>();
			for (int i = 0; i < Constants.CHUNK_SIZE; i++)
			{
				for (int j = 0; j < Constants.CHUNK_SIZE; j++)
				{
					ret[i, j] = value;
				}
			}
			return ret;
		}
	}
}
