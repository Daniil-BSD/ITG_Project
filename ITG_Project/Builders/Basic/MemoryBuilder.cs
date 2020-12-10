namespace ITG_Core.Basic.Builders {
	using ITG_Core.Base;
	using ITG_Core.Builders;

	/// <summary>
	/// Defines the <see cref="MemoryBuilder{T}" />
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class MemoryBuilder<T> : LayerBuilder<T, T> where T : struct {

		public override Algorithm<T> Build(LandscapeBuilder.LandscapeIntermidiate intermidiate)
		{
			VerifyVallidity(intermidiate);
			return new Memory<T>(Offset, intermidiate.ThreadPool, intermidiate.Get<T>(SourceID));
		}
	}
}