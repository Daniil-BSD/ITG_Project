namespace ITG_Core {
	/// <summary>
	/// Defines the <see cref="Blur" />
	/// </summary>
	public class Blur : NeighbourBasedAgorithm<float, float> {
		public Blur(Coordinate offset, ITGThreadPool threadPool, Algorithm<float> source) : base(offset, threadPool, source)
		{
		}

		public override float Compute(Neighbourhood<float> n)
		{
			float sum =
				n.data[0, 2] + n.data[1, 2] + n.data[2, 2] +
				n.data[0, 1] + n.data[1, 1] + n.data[2, 1] +
				n.data[0, 0] + n.data[1, 0] + n.data[2, 0];
			return sum / 9;
		}
	}
}
