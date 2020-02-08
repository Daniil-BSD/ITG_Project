namespace ITG_Core {
	/// <summary>
	/// Defines the <see cref="PerlinNoiseBuilder" />
	/// </summary>
	public class PerlinNoiseBuilder : InterpolatableAlgorithmBuilder<float, Vec2> {
		public override Algorithm<float> Build(LandscapeBuilder.LandscapeItermidiate itermidiate)
		{
			base.Build(itermidiate);
			return new PerlinNoise(itermidiate.Get<Vec2>(SourceID), Scale);
		}
	}
}
