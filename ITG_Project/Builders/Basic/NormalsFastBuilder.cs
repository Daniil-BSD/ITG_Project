namespace ITG_Core {
	/// <summary>
	/// Defines the <see cref="NormalsFastBuilder" />
	/// </summary>
	public class NormalsFastBuilder : LayerBuilder<Vec3, float> {
		public float GridStepOverHeightRange { get; set; }

		//TODO
		public override Algorithm<Vec3> Build(LandscapeBuilder.LandscapeIntermidiate intermidiate)
		{
			return new NormalFast(Offset, intermidiate.Get<float>(SourceID), gridStepOverHeightRange: GridStepOverHeightRange);
		}
	}
}
