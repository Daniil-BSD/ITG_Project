namespace ITG_Core {
	/// <summary>
	/// Defines the <see cref="RandomBuilder" />
	/// </summary>
	public class RandomBuilder : AlgorithmBuilder<uint> {
		public int Seed { get; set; }

		public override Algorithm<uint> Build(LandscapeBuilder.LandscapeItermidiate itermidiate)
		{
			base.Build(itermidiate);
			return new Random(Seed);
		}
	}
}
