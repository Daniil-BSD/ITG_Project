namespace ITG_Core.Basic.Builders {
	using System.Collections.Generic;
	using ITG_Core.Base;
	using ITG_Core.Builders;

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

		public float MaxModification { get; set; } = 0.025f;

		public int MaxSectorSize { get; set; } = 64;

		public float MinModification { get; set; } = 0.005f;

		public float MinSedimentCapacity { get; set; } = 0;

		public float OutputFactor { get; set; } = 0.375f;

		public float SedimentCapacityFactor { get; set; } = 20;

		public float SedimentFactor { get; set; } = 1f;

		public float StepLength { get; set; } = 1f;

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

		public override List<string> ValidityMessages(LandscapeBuilder landscapeBuilder)
		{
			List<string> messages = base.ValidityMessages(landscapeBuilder);
			if ( BrushRadius < 1 )
				messages.Add("BrushRadius must be at least 1.");
			if ( DepositSpeed > 1 || DepositSpeed <= 0 )
				messages.Add("DepositSpeed must be between 1(no restriction) and 0 (no deposit). 0 is invalid.");
			if ( ErodeSpeed > 1 || ErodeSpeed <= 0 )
				messages.Add("ErodeSpeed must be between 1(no restriction) and 0 (no deposit). 0 is invalid.");
			if ( EvaporationSpeed > 1 || EvaporationSpeed < 0 )
				messages.Add("ErodeSpeed must be between 1 and 0.");
			if ( Friction > 1 || Friction < 0 )
				messages.Add("Friction must be between 1 and 0.");
			if ( InitialVolume <= 0 )
				messages.Add("InitialVolume must be positive.");
			if ( CoverageFactor <= 0 )
				messages.Add("CoverageFactor must be at least 1.");
			if ( LayeringPower < 0 )
				messages.Add("LayeringPower must be at least 1.");
			if ( MaxIterations <= 0 )
				messages.Add("MaxIterations must be at least 1.");
			if ( MinModification < 0 )
				messages.Add("MinModification must be positive.");
			if ( MinSedimentCapacity < 0 )
				messages.Add("MinSedimentCapacity must be positive.");
			if ( OutputFactor <= 0 )
				messages.Add("OutputFactor must be positive.");
			if ( SedimentCapacityFactor <= 0 )
				messages.Add("SedimentCapacityFactor must be positive.");
			if ( StepLength <= 0 )
				messages.Add("StepLength must be positive.");
			if ( MaxSectorSize <= 0 )
				messages.Add("MaxSectorSize must be at least 1.");
			return messages;
		}
	}
}