namespace ITG_Core {
	/// <summary>
	/// Defines the <see cref="Layer{T, S}" />
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <typeparam name="S"></typeparam>
	public abstract class Layer<T, S> : Algorithm<T> where T : struct where S : struct {
		protected readonly Algorithm<S> source;

		public Layer(Coordinate offset, Algorithm<S> source) : base(offset)
		{
			this.source = source;
		}
	}
}
