namespace ITG_Core.Basic {
	using System.Runtime.CompilerServices;
	using ITG_Core.Base;

	/// <summary>
	/// Defines the <see cref="Interpolator" />
	/// </summary>
	public class Interpolator : InterpolatableAlgorithm<float, float> {

		public Interpolator(Coordinate offset, ITGThreadPool threadPool, Algorithm<float> algorithm, int scale) : base(offset, threadPool, algorithm, scale)
		{
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float ComputeStatic(in float val00, in float val01, in float val10, in float val11, in float x, in float y)
		{
			float top = val01 + x * ( val11 - val01 );
			float bottom = val00 + x * ( val10 - val00 );
			float ret = ( bottom + y * ( top - bottom ) );
			return ret;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float ComputeStatic(in float val00, in float val01, in float val10, in float val11, in Vec2 position)
		{
			return ComputeStatic(val00, val01, val10, val11, position.x, position.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override float Compute(in float val00, in float val01, in float val10, in float val11, in float x, in float y, in float offset)
		{
			return ComputeStatic(val00, val01, val10, val11, x, y);
		}
	}
}