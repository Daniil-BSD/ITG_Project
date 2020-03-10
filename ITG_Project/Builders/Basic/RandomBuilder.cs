namespace ITG_Core {
	/// <summary>
	/// Defines the <see cref="RandomBuilder" />
	/// </summary>
	public class RandomBuilder : AlgorithmBuilder<uint> {
		public int Seed { get; set; }

		public override Algorithm<uint> Build(LandscapeBuilder.LandscapeIntermidiate intermidiate)
		{
			VerifyVallidity(intermidiate);
			return new Random(Offset, intermidiate.ThreadPool, Seed);
		}
	}
}
