namespace ITG_Core {
	/// <summary>
	/// Defines the <see cref="Merger{T, S1, S2}" />
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <typeparam name="S1"></typeparam>
	/// <typeparam name="S2"></typeparam>
	public abstract class Merger<T, S1, S2> : Layer<T, S1> where T : struct where S1 : struct where S2 : struct {
		protected readonly Algorithm<S2> source2;

		public Merger(Coordinate offset, ITGThreadPool threadPool, Algorithm<S1> source, Algorithm<S2> source2) : base(offset, threadPool, source)
		{
			this.source2 = source2;
		}
	}
}
