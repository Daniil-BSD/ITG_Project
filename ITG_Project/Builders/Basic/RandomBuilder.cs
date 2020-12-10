namespace ITG_Core.Basic.Builders {
	using ITG_Core.Base;
	using ITG_Core.Builders;

	/// <summary>
	/// Defines the <see cref="RandomBuilder" />
	/// </summary>
	public class RandomBuilder : AlgorithmBuilder<uint> {

		public int Seed { get; set; } = 0;

		public override Algorithm<uint> Build(LandscapeBuilder.LandscapeIntermidiate intermidiate)
		{
			VerifyVallidity(intermidiate);
			return new Random(Offset, intermidiate.ThreadPool, Seed);
		}
	}
}