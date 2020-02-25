namespace ITG_Core {
	using System;
	using System.Runtime.CompilerServices;

	/// <summary>
	/// Defines the <see cref="HydrolicErrosion" />
	/// </summary>
	public class HydrolicErrosion : Layer<float, float> {
		public readonly int layers;

		public HydrolicErrosion(Algorithm<float> source) : base(source)
		{
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override Chunk<float> ChunkPopulation(in Coordinate coordinate)
		{
			return SectorPopulation(new Sector<float>(coordinate, 1, 1)).Chunks[0, 0];
		}

		public override Sector<float> SectorPopulation(Sector<float> sector)
		{
			throw new NotImplementedException();
		}
	}
}
