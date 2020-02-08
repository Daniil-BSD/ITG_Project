namespace ITG_Core {
	using System.Collections.Generic;

	/// <summary>
	/// Defines the <see cref="IEnumeratorPlus{T}" />
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IEnumeratorPlus<T> : IEnumerable<T>, IEnumerator<T> {
		int X { get; }

		int Y { get; }
	}
}
