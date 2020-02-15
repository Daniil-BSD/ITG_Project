namespace ITG_Core {
	/// <summary>
	/// Defines the <see cref="NeighbourBasedMergerBuilder{T, S1, S2}" />
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <typeparam name="S1"></typeparam>
	/// <typeparam name="S2"></typeparam>
	public abstract class NeighbourBasedMergerBuilder<T, S1, S2> : MergerBuilder<T, S1, S2> where T : struct where S1 : struct where S2 : struct {
	}
}
