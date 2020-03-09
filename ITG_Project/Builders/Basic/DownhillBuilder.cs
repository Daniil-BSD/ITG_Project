namespace ITG_Core {
	/// <summary>
	/// Defines the <see cref="DownhillBuilder" />
	/// </summary>
	public class DownhillBuilder : LayerBuilder<Vec3, Vec3> {
		public override Algorithm<Vec3> Build(LandscapeBuilder.LandscapeIntermidiate intermidiate)
		{
			return new Downhill(Offset, intermidiate.Get<Vec3>(SourceID));
		}
	}
}
