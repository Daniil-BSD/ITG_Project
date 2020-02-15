namespace ITG_Core {
	/// <summary>
	/// Defines the <see cref="Interpolator" />
	/// </summary>
	public class Interpolator : InterpolatableAlgorithm<float, float> {
		public Interpolator(Algorithm<float> algorithm, int scale) : base(algorithm, scale)
		{
		}

		public override float Compute(in float val00, in float val01, in float val10, in float val11, in float x, in float y, in float offset)
		{
			float top = val01 + x * (val11 - val01);
			float bottom = val00 + x * (val10 - val00);
			float ret = (bottom + y * (top - bottom));
			return ret;
		}
	}
}
