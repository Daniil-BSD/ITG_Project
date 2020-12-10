namespace ITG_Core.Basic.Builders {
	using ITG_Core.Base;
	using ITG_Core.Builders;

	/// <summary>
	/// Defines the <see cref="PerlinNoiseBuilder" />
	/// </summary>
	public class PerlinNoiseBuilder : InterpolatableAlgorithmBuilder<float, Vec2> {

		public override Algorithm<float> Build(LandscapeBuilder.LandscapeIntermidiate intermidiate)
		{
			VerifyVallidity(intermidiate);
			return new PerlinNoise(Offset, intermidiate.ThreadPool, intermidiate.Get<Vec2>(SourceID), Scale);
		}
	}
}