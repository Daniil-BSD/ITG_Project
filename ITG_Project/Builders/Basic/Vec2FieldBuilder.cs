namespace ITG_Core {
	/// <summary>
	/// Defines the <see cref="Vec2FieldBuilder" />
	/// </summary>
	public class Vec2FieldBuilder : LayerBuilder<Vec2, uint> {
		public float Magnitude { get; set; } = Constants.SQRT_2_OVER_2_FLOAT;

		public override Algorithm<Vec2> Build(LandscapeBuilder.LandscapeItermidiate itermidiate)
		{
			VerifyVallidity(itermidiate);
			return new Vec2Field(itermidiate.Get<uint>(SourceID), Magnitude);
		}
	}
}
