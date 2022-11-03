using ITG_ANTZ.Algorithms;
using ITG_Core;
using ITG_Core.Base;
using ITG_Core.Builders;

namespace ITG_ANTZ.Builders
{
	public class UintToFloatBuilder : LayerBuilder<float, uint>
	{
		public override Algorithm<float> Build(LandscapeBuilder.LandscapeIntermidiate intermidiate) => new UintToFloat(Offset, intermidiate.ThreadPool, intermidiate.Get<uint>(SourceID));
	}
}