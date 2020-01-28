namespace ITG_Core {
	/// <summary>
	/// Defines the <see cref="Interpolator" />
	/// </summary>
	public class Interpolator : InterpolatableAlgorithm<float, float> {
		public Interpolator(int scale) : base(new PerllinNoise(scale / 4), 4)
		{
		}

		public override float Compute(float val00, float val01, float val10, float val11, float x, float y, float offset)
		{
			float top = val01 + x * (val11 - val01);
			float bottom = val00 + x * (val10 - val00);
			float ret = (bottom + y * (top - bottom));
			return ret;
		}
	}
}
