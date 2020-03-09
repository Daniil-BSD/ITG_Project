namespace ITG_Core {
	/// <summary>
	/// Defines the <see cref="MemoryBuilder{T}" />
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class MemoryBuilder<T> : LayerBuilder<T, T> where T : struct {
		public override Algorithm<T> Build(LandscapeBuilder.LandscapeIntermidiate intermidiate)
		{
			VerifyVallidity(intermidiate);
			return new Memory<T>(Offset, intermidiate.Get<T>(SourceID));
		}
	}
}
