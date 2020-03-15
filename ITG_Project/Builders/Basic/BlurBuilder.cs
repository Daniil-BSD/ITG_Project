namespace ITG_Core.Basic.Builders {
	using ITG_Core.Base;
	using ITG_Core.Bulders;

	/// <summary>
	/// Defines the <see cref="BlurBuilder" />
	/// </summary>
	public class BlurBuilder : NeighbourBasedAgorithmBuilder<float, float> {

		public float Force { get; set; } = 1;

		public override Algorithm<float> Build(LandscapeBuilder.LandscapeIntermidiate intermidiate)
		{
			VerifyVallidity(intermidiate);
			if ( Force == 0 )
				return new BlurBasic(Offset, intermidiate.ThreadPool, intermidiate.Get<float>(SourceID));
			return new BlurAdvanced(Offset, intermidiate.ThreadPool, intermidiate.Get<float>(SourceID), Force);
		}

		public override bool IsValid(LandscapeBuilder landscapeBuilder)
		{
			if ( !base.IsValid(landscapeBuilder) )
				return false;
			if ( Force > 1 )
				return false;
			if ( Force <= 0 )
				return false;

			return true;
		}
	}
}