namespace ITG_Core.Basic.Builders {
	using ITG_Core.Bulders;
	using ITG_Core.Base;
	/// <summary>
	/// Defines the <see cref="DownhillBuilder" />
	/// </summary>
	public class DownhillBuilder : LayerBuilder<Vec3, Vec3> {
		public override Algorithm<Vec3> Build(LandscapeBuilder.LandscapeIntermidiate intermidiate)
		{
			return new Downhill(Offset, intermidiate.ThreadPool, intermidiate.Get<Vec3>(SourceID));
		}
	}
}
