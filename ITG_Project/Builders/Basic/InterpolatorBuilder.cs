namespace ITG_Core {
	/// <summary>
	/// Defines the <see cref="InterpolatorBuilder" />
	/// </summary>
	public class InterpolatorBuilder : InterpolatableAlgorithmBuilder<float, float> {
		public override Algorithm<float> Build(LandscapeBuilder.LandscapeIntermidiate intermidiate)
		{
			VerifyVallidity(intermidiate);
			return new Interpolator(Offset, intermidiate.Get<float>(SourceID), Scale);
		}
	}
}
