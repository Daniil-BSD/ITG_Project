namespace ITG_Core {
	/// <summary>
	/// Defines the <see cref="MemoryBuilder{T}" />
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class MemoryBuilder<T> : LayerBuilder<T, T> where T : struct {
		public override Algorithm<T> Build(LandscapeBuilder.LandscapeItermidiate itermidiate)
		{
			base.Build(itermidiate);
			return new Memory<T>(itermidiate.Get<T>(SourceID));
		}
	}
}
