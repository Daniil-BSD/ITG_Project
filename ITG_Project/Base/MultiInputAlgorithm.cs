namespace ITG_Core {
	using System.Collections.Generic;

	/// <summary>
	/// Defines the <see cref="MultiInputAlgorithm{T, S}" />
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <typeparam name="S"></typeparam>
	public abstract class MultiInputAlgorithm<T, S> : Algorithm<T> where T : struct where S : struct {
		public readonly Algorithm<S>[] sources;

		public MultiInputAlgorithm(List<Algorithm<S>> sources) : base()
		{
			this.sources = sources.ToArray();
		}
	}
}
