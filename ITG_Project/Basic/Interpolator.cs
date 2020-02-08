namespace ITG_Core {
	/// <summary>
	/// Defines the <see cref="Interpolator" />
	/// </summary>
	public class Interpolator : InterpolatableAlgorithm<float, float> {
		public Interpolator(Algorithm<float> algorithm, int scale) : base(algorithm, scale) { }

		public override float Compute(float val00, float val01, float val10, float val11, float x, float y, float offset)
		{
			float top = val01 + x * (val11 - val01);
			float bottom = val00 + x * (val10 - val00);
			float ret = (bottom + y * (top - bottom));
			return ret;
		}
	}
}
