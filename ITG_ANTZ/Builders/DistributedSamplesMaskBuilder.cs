using ITG_Core;
using ITG_Core.Base;
using ITG_Core.Builders;

namespace ITG_ANTZ.Algorithms
{
	public class DistributedSamplesMaskBuilder : AlgorithmBuilder<float>
	{
		public float Coverage { get; set; }
		public int Layering { get; set; }
		public float PassValue { get; set; } = 1;
		public float FailValue { get; set; } = 0;
		public override Algorithm<float> Build(LandscapeBuilder.LandscapeIntermidiate intermidiate) => new DistributedSamplesMask(Offset, intermidiate.ThreadPool, Coverage, Layering, PassValue, FailValue);
	}
}