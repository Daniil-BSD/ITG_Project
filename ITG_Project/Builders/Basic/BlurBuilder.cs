namespace ITG_Core {
	/// <summary>
	/// Defines the <see cref="BlurBuilder" />
	/// </summary>
	public class BlurBuilder : NeighbourBasedAgorithmBuilder<float, float> {
		public override Algorithm<float> Build(LandscapeBuilder.LandscapeIntermidiate intermidiate)
		{
			VerifyVallidity(intermidiate);
			return new Blur(Offset, intermidiate.ThreadPool, intermidiate.Get<float>(SourceID));
		}
	}
}
