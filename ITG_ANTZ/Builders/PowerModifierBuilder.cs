using ITG_ANTZ.Algorithms;
using ITG_Core;
using ITG_Core.Base;
using ITG_Core.Builders;

namespace ITG_ANTZ.Builders
{
	public class PowerModifierBuilder : LayerBuilder<float, float>
	{
		public int Power { get; set; } = 2;

		public override Algorithm<float> Build(LandscapeBuilder.LandscapeIntermidiate intermidiate)
		{
			VerifyVallidity(intermidiate);
			return new PowerModifier(Offset, intermidiate.ThreadPool, intermidiate.Get<float>(SourceID), Power);
		}
	}
}