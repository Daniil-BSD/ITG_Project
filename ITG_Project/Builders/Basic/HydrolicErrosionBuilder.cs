namespace ITG_Core {
	/// <summary>
	/// Defines the <see cref="HydrolicErrosionBuilder" />
	/// </summary>
	public class HydrolicErrosionBuilder : LayerBuilder<float, float> {

		public float BrushRadius { get; set; } = 3;

		public float DepositSpeed { get; set; } = 0.25f;

		public float ErodeSpeed { get; set; } = 0.25f;

		public float EvaporationSpeed { get; set; } = 0.25f;

		public float Gravity { get; set; } = 10f;

		public float Inertia { get; set; } = 0.0625f;

		public float InitialSpeed { get; set; } = 1;

		public float InitialVolume { get; set; } = 1;

		public int MaxIterations { get; set; } = 64;

		public float MaxSedimentForTermination { get; set; } = 0.0625f;

		public float MinSedimentCapacity { get; set; } = 0;

		public float MinVolume { get; set; } = 0.03125f;

		public float OutputFactor { get; set; } = 1;

		public float SedimentCapacityFactor { get; set; } = 1;

		public float StepLength { get; set; } = 1.5f;

		public int LayeringPower { get; set; } = 5;

		public int[] LayeringIndexes { get; set; } = new int[1] { 1 };

		//TODO
		public override Algorithm<float> Build(LandscapeBuilder.LandscapeIntermidiate intermidiate)
		{
			return new HydrolicErrosion(
				offset: Offset,
				threadPool: intermidiate.ThreadPool,
				source: intermidiate.Get<float>(SourceID),
				brushRadius: BrushRadius,
				depositSpeed: DepositSpeed,
				erodeSpeed: ErodeSpeed,
				evaporationSpeed: EvaporationSpeed,
				gravity: Gravity,
				inertia: Inertia,
				initialSpeed: InitialSpeed,
				initialVolume: InitialVolume,
				layeringIndexes: LayeringIndexes,
				layeringPower: LayeringPower,
				maxIterations: MaxIterations,
				maxSedimentForTermination: MaxSedimentForTermination,
				minSedimentCapacity: MinSedimentCapacity,
				minVolume: MinVolume,
				outputFactor: OutputFactor,
				sedimentCapacityFactor: SedimentCapacityFactor,
				stepLength: StepLength
			);
		}
	}
}
