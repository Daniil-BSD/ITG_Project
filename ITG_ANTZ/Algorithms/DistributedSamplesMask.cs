using ITG_Core;
using ITG_Core.Base;

namespace ITG_ANTZ.Algorithms
{
	public class DistributedSamplesMask : Algorithm<float>
	{

		public readonly float coverage;
		public readonly int layering;
		public readonly float passValue;
		public readonly float failValue;
		private readonly LayeringEnumeratorBuilder layeringEnumeratorBuilder;
		private readonly Chunk<float> bufferChunk;
		public DistributedSamplesMask(Coordinate offset, ITGThreadPool threadPool, float coverage, int layering, float passValue, float failValue) : base(offset, threadPool)
		{
			this.coverage = coverage;
			this.layering = layering;
			this.passValue = passValue;
			this.failValue = failValue;
			layeringEnumeratorBuilder = new LayeringEnumeratorBuilder(layering, coverage);
			bufferChunk = GenerateChunk();
		}

		protected override Chunk<float> ChunkPopulation(in Coordinate coordinate)
		{
			return bufferChunk;
		}


		public Chunk<float> GenerateChunk()
		{
			var ret = new Chunk<float>();

			for (int i = 0; i < Constants.CHUNK_SIZE; i++)
			{
				for (int j = 0; j < Constants.CHUNK_SIZE; j++)
				{
					ret[i, j] = failValue;
				}
			}

			LayeringEnumerator enumerator = layeringEnumeratorBuilder.BuildEnumerator(Constants.CHUNK_SIZE, Constants.CHUNK_SIZE);

			while (enumerator.MoveNext())
			{
				ret[enumerator.Current.x, enumerator.Current.y] = passValue;
			}

			return ret;
		}
	}
}