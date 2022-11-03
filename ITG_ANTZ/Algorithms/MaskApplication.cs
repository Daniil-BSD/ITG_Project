using ITG_Core;
using ITG_Core.Base;

namespace ITG_ANTZ.Algorithms
{
	public class MaskApplication : CombinationLayer<float, float, float>
	{

		public readonly float passValue;
		public readonly float failValue;
		public MaskApplication(Coordinate offset, ITGThreadPool threadPool, Algorithm<float> source, Algorithm<float> mask, float passValue, float failValue) : base(offset, threadPool, source, mask)
		{
			this.passValue = passValue;
			this.failValue = failValue;
		}

		protected override void ChunkPopulation(out Chunk<float> main, in Chunk<float> mainRequest, in Chunk<float> modifierRequest, in Coordinate coordinate)
		{
			main = new Chunk<float>();
			for (int i = 0; i < Constants.CHUNK_SIZE; i++)
			{
				for (int j = 0; j < Constants.CHUNK_SIZE; j++)
				{
					main[i, j] = (mainRequest[i, j] >= modifierRequest[i, j]) ? passValue : failValue;
				}
			}
		}
	}
}