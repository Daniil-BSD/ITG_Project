namespace ITG_Core.Brushes {
	public interface Brush<T> where T : struct {
		BrushTouple<T> this[in int index] { get; }
		BrushTouple<T>[] Touples { get; }
	}

	/// <summary>
	/// Defines the <see cref="BrushTouple{T}" />
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public struct BrushTouple<T> where T : struct {
		public readonly CoordinateBasic offset;

		public readonly T value;

		public BrushTouple(CoordinateBasic offset, T value)
		{
			this.offset = offset;
			this.value = value;
		}

		public override string ToString()
		{
			return base.ToString();
		}
	}
}
