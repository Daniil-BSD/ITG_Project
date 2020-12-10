using ITG_Core.Base;
using ITG_Core.Basic;

namespace ITG_Core.Builders {

	public class CoverterUintToFloatBuilder : LayerBuilder<float, uint> {

		public override Algorithm<float> Build(LandscapeBuilder.LandscapeIntermidiate intermidiate)
		{
			VerifyVallidity(intermidiate);
			return new CoverterUintToFloat(Offset, intermidiate.ThreadPool, intermidiate.Get<uint>(SourceID));
		}
	}
}