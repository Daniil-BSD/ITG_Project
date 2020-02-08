namespace ITG_Core {
	using System.Runtime.CompilerServices;

	/// <summary>
	/// Defines the <see cref="PerlinNoise" />
	/// </summary>
	public class PerlinNoise : InterpolatableAlgorithm<float, Vec2> {

		public PerlinNoise(Algorithm<Vec2> algorithm, int scale) : base(algorithm, scale) { }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Ease3(float v)
		{
			float temp = v * v;
			return (temp * 3 - temp * v * 2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Ease5(float v)
		{
			float temp = v * v * v;
			return (temp * v * v * 6 - temp * v * 15 + temp * 10);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override float Compute(Vec2 val00, Vec2 val01, Vec2 val10, Vec2 val11, float x, float y, float offset)
		{
			x += offset;
			y += offset;

			Vec2 v00 = new Vec2(x, y);
			Vec2 v01 = new Vec2(x, y - 1);
			Vec2 v10 = new Vec2(x - 1, y);
			Vec2 v11 = new Vec2(x - 1, y - 1);

			float p00 = Vec2.Dot(v00, val00);
			float p01 = Vec2.Dot(v01, val01);
			float p10 = Vec2.Dot(v10, val10);
			float p11 = Vec2.Dot(v11, val11);

			float easeX = Ease5(x);

			float top = p01 + easeX * (p11 - p01);
			float bottom = p00 + easeX * (p10 - p00);

			float ret = ((bottom + Ease5(y) * (top - bottom)) + 1) / 2;
			return Ease3(ret);
		}
	}
}
