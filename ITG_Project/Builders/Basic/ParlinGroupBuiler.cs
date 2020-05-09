namespace ITG_Core.Basic.Builders {
	using System;
	using System.Collections.Generic;
	using ITG_Core.Base;
	using ITG_Core.Bulders;

	/// <summary>
	/// Defines the <see cref="ParlinGroupBuiler" />
	/// </summary>
	public class ParlinGroupBuiler : AlgorithmGroupBuilder<float> {

		public static readonly string IDENTIFIER_INTERPOLATOR = "interpol";

		public static readonly string IDENTIFIER_PERLIN = "perlin";

		public bool BottomUp { get; set; } = true;

		public float DeltaFactor { get; set; } = 0.5f;

		public float LowerTargetScale { get; set; } = 2;

		public int MaxInterpolationScale { get; set; } = 8;

		public int MaxLayers { get; set; } = 16;

		public float MaxPerlinScale { get; set; } = 128;

		public CoordinateBasic OffsetGlobal { get; set; } = new CoordinateBasic(0, 0);

		public float RetFactor { get; set; } = 1.375f;

		public float ScaleStep { get; set; } = 2;

		public float UpperTargetScale { get; set; } = 1024;

		public float MinNoiseFactor { get; set; } = 0.00f;

		public string Vec2FieldID { get; set; }

		private static int RoundTI(in float f)
		{
			return (int)Math.Round(f);
		}

		public override Dictionary<string, IAlgorithm> BuildGeneric(LandscapeBuilder.LandscapeIntermidiate intermidiate)
		{
			Dictionary<string, IAlgorithm> ret = new Dictionary<string, IAlgorithm>();
			List<Algorithm<float>> sources = new List<Algorithm<float>>();
			Algorithm<Vec2> vec2Source = intermidiate.Get<Vec2>(Vec2FieldID);
			ITGThreadPool threadPool = intermidiate.ThreadPool;
			if ( BottomUp ) {
				for ( float f = LowerTargetScale ; f <= UpperTargetScale && sources.Count < MaxLayers ; f *= ScaleStep ) {
					int scale = RoundTI(f);
					int interpolatorScacle = 1;
					while ( scale / interpolatorScacle > MaxPerlinScale && interpolatorScacle < MaxInterpolationScale ) {
						interpolatorScacle++;
					}
					int perlinScale = scale / interpolatorScacle;
					if ( interpolatorScacle == 1 ) {
						string ref_key = IDENTIFIER_PERLIN + sources.Count;
						PerlinNoise perlin = new PerlinNoise(OffsetGlobal, threadPool, vec2Source, perlinScale);
						ret.Add(ref_key, perlin);
						sources.Insert(0, perlin);
					} else {
						string ref_key_perlin = IDENTIFIER_PERLIN + sources.Count;
						string ref_key_interpol = IDENTIFIER_INTERPOLATOR + sources.Count;
						PerlinNoise perlin = new PerlinNoise(OffsetGlobal, threadPool, vec2Source, perlinScale);
						Interpolator interpol = new Interpolator(Coordinate.Origin, threadPool, perlin, interpolatorScacle);
						ret.Add(ref_key_perlin, perlin);
						ret.Add(ref_key_interpol, interpol);
						sources.Insert(0, interpol);
					}
				}
			} else {
				float factor = 1;
				for ( float f = UpperTargetScale ; f > LowerTargetScale && sources.Count < MaxLayers && factor > MinNoiseFactor ; f /= ScaleStep ) {
					int scale = RoundTI(f);
					int interpolatorScacle = 1;
					while ( scale / interpolatorScacle > MaxPerlinScale && interpolatorScacle < MaxInterpolationScale ) {
						interpolatorScacle++;
					}
					int perlinScale = scale / interpolatorScacle;
					if ( interpolatorScacle == 1 ) {
						string ref_key = IDENTIFIER_PERLIN + sources.Count;
						PerlinNoise perlin = new PerlinNoise(OffsetGlobal, threadPool, vec2Source, perlinScale);
						ret.Add(ref_key, perlin);
						sources.Add(perlin);
					} else {
						string ref_key_perlin = IDENTIFIER_PERLIN + sources.Count;
						string ref_key_interpol = IDENTIFIER_INTERPOLATOR + sources.Count;
						PerlinNoise perlin = new PerlinNoise(OffsetGlobal, threadPool, vec2Source, perlinScale);
						Interpolator interpol = new Interpolator(Coordinate.Origin, threadPool, perlin, interpolatorScacle);
						ret.Add(ref_key_perlin, perlin);
						ret.Add(ref_key_interpol, interpol);
						sources.Add(interpol);
					}
					factor *= DeltaFactor;
				}
			}

			ret.Add(LandscapeBuilder.MAIN_ALGORITHM_KEY, new FloatAdder(OffsetGlobal, intermidiate.ThreadPool, sources, DeltaFactor, RetFactor));
			return ret;
		}

		public override Type GetGenericType(string key)
		{
			return typeof(float);
		}

		public override bool IsValid(LandscapeBuilder landscapeBuilder)
		{
			if ( !base.IsValid(landscapeBuilder) )
				return false;
			if ( DeltaFactor <= 0 )
				return false;
			if ( MaxLayers < 2 )
				return false;
			if ( MaxPerlinScale < 2 )
				return false;
			if ( RetFactor == 0 )
				return false;
			if ( ScaleStep <= 0 )
				return false;
			if ( UpperTargetScale < 2 )
				return false;
			if ( LowerTargetScale < 2 )
				return false;
			if ( UpperTargetScale <= LowerTargetScale )
				return false;
			if ( MaxInterpolationScale < 1 )
				return false;
			if ( !landscapeBuilder.CheckValidityOf(Vec2FieldID) )
				return false;
			if ( !landscapeBuilder.TypeOf(Vec2FieldID).IsSubclassOf(typeof(AlgorithmBuilder<Vec2>)) )
				return false;
			return true;
		}

		public override List<string> ValidityMessages(LandscapeBuilder landscapeBuilder)
		{
			List<string> messages = base.ValidityMessages(landscapeBuilder);
			if ( DeltaFactor <= 0 )
				messages.Add("DeltaFactor has to be a positive, non-zero real number.");
			if ( MaxLayers < 2 )
				messages.Add("MaxLayers has to be greater than one.");
			if ( MaxPerlinScale < 2 )
				messages.Add("MaxPerlinScale has to be greater than one.");
			if ( RetFactor == 0 )
				messages.Add("RetFactor has to be a non-zero real number.");
			if ( ScaleStep <= 0 )
				messages.Add("ScaleStep has to be a positive, non-zero real number.");
			if ( UpperTargetScale < 2 )
				messages.Add("UpperTargetScale has to be greater than one.");
			if ( LowerTargetScale < 2 )
				messages.Add("LowerTargetScale has to be greater than one.");
			if ( UpperTargetScale <= LowerTargetScale )
				messages.Add("UpperTargetScale has to be greater than LowerTargetScale.");
			if ( MaxInterpolationScale < 1 )
				messages.Add("MaxInterpolationScale has to be a greater than zero.");
			if ( !landscapeBuilder.CheckValidityOf(Vec2FieldID) )
				messages.Add("Source Layer \"" + Vec2FieldID + "\" is missing or invalid.");
			else if ( !landscapeBuilder.TypeOf(Vec2FieldID).IsSubclassOf(typeof(AlgorithmBuilder<Vec2>)) )
				messages.Add("Source \"" + Vec2FieldID + "\" is of uncompattible type.");
			return messages;
		}
	}
}