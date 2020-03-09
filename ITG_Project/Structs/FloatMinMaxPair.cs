namespace ITG_Core {
	/// <summary>
	/// Defines the <see cref="FloatMinMaxPair" />
	/// </summary>
	public struct FloatMinMaxPair {
		public float max;

		public float min;

		public FloatMinMaxPair(in float min, in float max)
		{
			this.min = min;
			this.max = max;
		}
	}
}
