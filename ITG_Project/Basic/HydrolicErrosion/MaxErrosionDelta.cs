namespace ITG_Core {
	/// <summary>
	/// Defines the <see cref="MaxErrosionDelta" />
	/// </summary>
	public class MaxErrosionDelta : NeighbourBasedAgorithm<float, float> {
		public MaxErrosionDelta(Coordinate offset, ITGThreadPool threadPool, Algorithm<float> source) : base(offset, threadPool, source)
		{
		}

		public override float Compute(Neighbourhood<float> n)
		{
			float min = float.MaxValue;
			for ( int i = -1 ; i < 2 ; i++ ) {
				for ( int j = -1 ; j < 2 ; j++ ) {
					if ( min > n[i, j] ) {
						min = n[i, j];
					}
				}
			}
			return n[0, 0] - min;
		}
	}
}
