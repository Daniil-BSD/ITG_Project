using ITG_ANTZ.Algorithms;
using ITG_Core;
using ITG_Core.Base;
using ITG_Core.Builders;

namespace ITG_ANTZ.Builders
{
	public class ConstantFloatBuilder : AlgorithmBuilder<float>
	{
		public float Value { get; set; }
		public override Algorithm<float> Build(LandscapeBuilder.LandscapeIntermidiate intermidiate) => new ConstantFloat(Offset, intermidiate.ThreadPool, Value);
	}
}