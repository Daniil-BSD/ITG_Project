namespace ITG_Core {
	/// <summary>
	/// Defines the <see cref="InterpolatorBuilder" />
	/// </summary>
	public class InterpolatorBuilder : InterpolatableAlgorithmBuilder<float, float> {
		public override Algorithm<float> Build(LandscapeBuilder.LandscapeItermidiate itermidiate)
		{
			VerifyVallidity(itermidiate);
			return new Interpolator(itermidiate.Get<float>(SourceID), Scale);
		}
	}
}
