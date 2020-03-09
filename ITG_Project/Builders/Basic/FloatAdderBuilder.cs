namespace ITG_Core {
	using System.Collections.Generic;

	/// <summary>
	/// Defines the <see cref="FloatAdderBuilder" />
	/// </summary>
	public class FloatAdderBuilder : MultiInputAlgorithmBuilder<float, float> {
		public float DeltaFactor { get; set; } = 1f;

		public float RetFactor { get; set; } = 1f;

		public override Algorithm<float> Build(LandscapeBuilder.LandscapeIntermidiate intermidiate)
		{
			VerifyVallidity(intermidiate);
			List<Algorithm<float>> algorithms = new List<Algorithm<float>>();
			foreach ( var sourceID in Sources )
				algorithms.Add(intermidiate.Get<float>(sourceID));
			return new FloatAdder(Offset, algorithms, DeltaFactor, RetFactor);
		}
	}
}
