namespace ITG_Core {
	using System.Collections.Generic;

	/// <summary>
	/// Defines the <see cref="FloatAdderBuilder" />
	/// </summary>
	public class FloatAdderBuilder : MultiInputAlgorithmBuilder<float, float> {
		public float DeltaFactor { get; set; } = 0.5f;

		public float RetFactor { get; set; } = 1.2f;

		public override Algorithm<float> Build(LandscapeBuilder.LandscapeItermidiate itermidiate)
		{
			VerifyVallidity(itermidiate);
			List<Algorithm<float>> algorithms = new List<Algorithm<float>>();
			foreach ( var sourceID in Sources )
				algorithms.Add(itermidiate.Get<float>(sourceID));
			return new FloatAdder(algorithms, DeltaFactor, RetFactor);
		}
	}
}
