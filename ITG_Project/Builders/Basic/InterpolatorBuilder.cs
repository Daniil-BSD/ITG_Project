
namespace ITG_Core.Basic.Builders {
	using ITG_Core.Base;
	using ITG_Core.Bulders;
	/// <summary>
	/// Defines the <see cref="InterpolatorBuilder" />
	/// </summary>
	public class InterpolatorBuilder : InterpolatableAlgorithmBuilder<float, float> {
		public override Algorithm<float> Build(LandscapeBuilder.LandscapeIntermidiate intermidiate)
		{
			VerifyVallidity(intermidiate);
			return new Interpolator(Offset, intermidiate.ThreadPool, intermidiate.Get<float>(SourceID), Scale);
		}
	}
}
