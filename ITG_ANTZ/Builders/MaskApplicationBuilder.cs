using ITG_ANTZ.Algorithms;
using ITG_Core;
using ITG_Core.Base;
using ITG_Core.Builders;

namespace ITG_ANTZ.Builders
{
	public class MaskApplicationBuilder : CombinationLayerBuilder<float, float, float>
	{
		public float PassValue { get; set; } = 1;
		public float FailValue { get; set; } = 0;
		public override Algorithm<float> Build(LandscapeBuilder.LandscapeIntermidiate intermidiate) => new MaskApplication(Offset, intermidiate.ThreadPool, intermidiate.Get<float>(SourceID), intermidiate.Get<float>(ModifierID), PassValue, FailValue);
	}
}