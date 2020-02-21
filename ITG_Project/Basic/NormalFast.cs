﻿namespace ITG_Core.Basic {
	using System.Runtime.CompilerServices;

	/// <summary>
	/// Defines the <see cref="NormalFast" />
	/// </summary>
	public unsafe class NormalFast : NeighbourBasedAgorithm<Vec3, float> {
		private readonly float gridStepOverHeightRange;

		public NormalFast(Algorithm<float> algorithm, float gridStepOverHeightRange) : base(algorithm)
		{
			this.gridStepOverHeightRange = gridStepOverHeightRange;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override Vec3 Compute(Neighbourhood<float> n)
		{
			return new Vec3(n[-1, 0] - n[1, 0], n[0, -1] - n[0, 1], 2 * gridStepOverHeightRange).Normalize();
		}
	}
}