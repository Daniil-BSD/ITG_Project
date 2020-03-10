namespace ITG_Core {
	/// <summary>
	/// Defines the <see cref="Vec2FieldBuilder" />
	/// </summary>
	public class Vec2FieldBuilder : LayerBuilder<Vec2, uint> {
		public float Magnitude { get; set; } = Constants.SQRT_2_OVER_2_FLOAT;

		public override Algorithm<Vec2> Build(LandscapeBuilder.LandscapeIntermidiate intermidiate)
		{
			VerifyVallidity(intermidiate);
			return new Vec2Field(Offset, intermidiate.ThreadPool, intermidiate.Get<uint>(SourceID), Magnitude);
		}
	}
}
