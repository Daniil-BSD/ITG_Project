namespace ITG_Core {
	/// <summary>
	/// Defines the <see cref="MaxErrosionDeltaBuilder" />
	/// </summary>
	public class MaxErrosionDeltaBuilder : LayerBuilder<float, float> {
		public override Algorithm<float> Build(LandscapeBuilder.LandscapeIntermidiate intermidiate)
		{
			return new MaxErrosionDelta(Offset, intermidiate.Get<float>(SourceID));
		}
	}
}
