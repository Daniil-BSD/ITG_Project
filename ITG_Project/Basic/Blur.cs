namespace ITG_Core.Basic {
	using ITG_Core.Base;

	public class BlurAdvanced : BlurBasic {

		private readonly float forceOverNine;

		private readonly float oneMinusForce;

		public BlurAdvanced(Coordinate offset, ITGThreadPool threadPool, Algorithm<float> source, float force) : base(offset, threadPool, source)
		{
			oneMinusForce = 1 - force;
			forceOverNine = force / 9;
		}

		public override float Compute(Neighbourhood<float> n)
		{
			float sum =
				n.data[0, 2] + n.data[1, 2] + n.data[2, 2] +
				n.data[0, 1] + n.data[1, 1] + n.data[2, 1] +
				n.data[0, 0] + n.data[1, 0] + n.data[2, 0];
			return sum * forceOverNine + n.data[1, 1] * oneMinusForce;
		}
	}

	/// <summary>
	/// Defines the <see cref="Blur" />
	/// </summary>
	public class BlurBasic : NeighbourBasedAgorithm<float, float> {

		public BlurBasic(Coordinate offset, ITGThreadPool threadPool, Algorithm<float> source) : base(offset, threadPool, source)
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