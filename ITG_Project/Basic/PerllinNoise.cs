namespace ITG_Core.Basic {
	using System.Runtime.CompilerServices;
	using ITG_Core.Base;

	/// <summary>
	/// Defines the <see cref="PerlinNoise" />
	/// </summary>
	public class PerlinNoise : InterpolatableAlgorithm<float, Vec2> {

		public PerlinNoise(Coordinate offset, ITGThreadPool threadPool, Algorithm<Vec2> algorithm, int scale) : base(offset, threadPool, algorithm, scale)
		{
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Ease3(in float v)
		{
			float temp = v * v;
			return ( temp * 3 - temp * v * 2 );
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Ease5(in float v)
		{
			float p2 = v * v;
			float p3 = p2 * v;
			return ( p3 * p2 * 6 - p2 * p2 * 15 + p3 * 10 );
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override float Compute(in Vec2 val00, in Vec2 val01, in Vec2 val10, in Vec2 val11, in float x, in float y)
		{
			float X = x + initialOffset;
			float Y = y + initialOffset;
			float Xm1 = X - 1;
			float Ym1 = Y - 1;

			Vec2 v00 = new Vec2(X, Y);
			Vec2 v01 = new Vec2(X, Ym1);
			Vec2 v10 = new Vec2(Xm1, Y);
			Vec2 v11 = new Vec2(Xm1, Ym1);

			float p00 = Vec2.Dot(v00, val00);
			float p01 = Vec2.Dot(v01, val01);
			float p10 = Vec2.Dot(v10, val10);
			float p11 = Vec2.Dot(v11, val11);

			float easeX = Ease5(X);

			float top = p01 + easeX * ( p11 - p01 );
			float bottom = p00 + easeX * ( p10 - p00 );

			float ret = ( ( bottom + Ease5(Y) * ( top - bottom ) ) + 1 ) / 2;
			return ( Ease3(ret) - 0.5f ) * 2f;
		}
	}
}