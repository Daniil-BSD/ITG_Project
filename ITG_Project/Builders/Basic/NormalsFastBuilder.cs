namespace ITG_Core.Basic.Builders {
	using ITG_Core.Base;
	using ITG_Core.Builders;

	/// <summary>
	/// Defines the <see cref="NormalsFastBuilder" />
	/// </summary>
	public class NormalsFastBuilder : LayerBuilder<Vec3, float> {

		public float GridStepOverHeightRange { get; set; }

		//TODO
		public override Algorithm<Vec3> Build(LandscapeBuilder.LandscapeIntermidiate intermidiate)
		{
			VerifyVallidity(intermidiate);
			return new NormalFast(Offset, intermidiate.ThreadPool, intermidiate.Get<float>(SourceID), gridStepOverHeightRange: GridStepOverHeightRange);
		}
	}
}