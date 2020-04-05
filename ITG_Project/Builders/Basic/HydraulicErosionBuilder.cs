namespace ITG_Core.Basic.Builders {
	using ITG_Core.Base;
	using ITG_Core.Bulders;

	/// <summary>
	/// Defines the <see cref="HydrolicErrosionBuilder" />
	/// </summary>
	public class HydraulicErosionBuilder : LayerBuilder<float, float> {

		public float BrushRadius { get; set; } = 1.25f;

		public float CoverageFactor { get; set; } = 1f;

		public float DepositSpeed { get; set; } = 0.0675f;

		public float ErodeSpeed { get; set; } = 0.125f;

		public float EvaporationSpeed { get; set; } = 0.085f;

		public float Friction { get; set; } = 0.25f;

		public float Gravity { get; set; } = 0.5f;

		public float InitialSpeed { get; set; } = 0;

		public float InitialVolume { get; set; } = 4;

		public int LayeringPower { get; set; } = 5;

		public int MaxIterations { get; set; } = 80;

		public int MaxSectorSize { get; set; } = 64;

		public float MinModification { get; set; } = 0.005f;

		public float MaxModification { get; set; } = 0.025f;

		public float MinSedimentCapacity { get; set; } = 0;

		public float OutputFactor { get; set; } = 0.375f;

		public float SedimentCapacityFactor { get; set; } = 20;

		public float StepLength { get; set; } = 1f;
		public float SedimentFactor { get; set; } = 1f;

		//TODO
		public override Algorithm<float> Build(LandscapeBuilder.LandscapeIntermidiate intermidiate)
		{
			VerifyVallidity(intermidiate);
			return new HydrolicErosion(
				offset: Offset,
				threadPool: intermidiate.ThreadPool,
				source: intermidiate.Get<float>(SourceID),
				brushRadius: BrushRadius,
				depositSpeed: DepositSpeed,
				erodeSpeed: ErodeSpeed,
				evaporationSpeed: EvaporationSpeed,
				gravity: Gravity,
				initialSpeed: InitialSpeed,
				initialVolume: InitialVolume,
				layeringPower: LayeringPower,
				maxIterations: MaxIterations,
				minSedimentCapacity: MinSedimentCapacity,
				outputFactor: OutputFactor,
				sedimentCapacityFactor: SedimentCapacityFactor,
				stepLength: StepLength,
				friction: Friction,
				minModification: MinModification,
				maxSectorSize: MaxSectorSize,
				coverageFactor: CoverageFactor,
				maxModification: MaxModification,
				sedimentFactor: SedimentFactor
			);
		}

		public override bool IsValid(LandscapeBuilder landscapeBuilder)
		{
			if ( !base.IsValid(landscapeBuilder) )
				return false;
			if ( BrushRadius < 1 )
				return false;
			if ( DepositSpeed > 1 || DepositSpeed <= 0 )
				return false;
			if ( ErodeSpeed > 1 || ErodeSpeed <= 0 )
				return false;
			if ( EvaporationSpeed > 1 || EvaporationSpeed < 0 )
				return false;
			if ( Friction > 1 || Friction < 0 )
				return false;
			if ( InitialVolume <= 0 )
				return false;
			if ( CoverageFactor <= 0 )
				return false;
			if ( LayeringPower < 0 )
				return false;
			if ( MaxIterations <= 0 )
				return false;
			if ( MinModification < 0 )
				return false;
			if ( MinSedimentCapacity < 0 )
				return false;
			if ( OutputFactor <= 0 )
				return false;
			if ( SedimentCapacityFactor <= 0 )
				return false;
			if ( StepLength <= 0 )
				return false;
			if ( MaxSectorSize <= 0 )
				return false;
			return true;
		}
	}
}