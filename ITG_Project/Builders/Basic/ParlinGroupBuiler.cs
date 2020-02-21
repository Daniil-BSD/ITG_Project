namespace ITG_Core {
	using System;
	using System.Collections.Generic;

	/// <summary>
	/// Defines the <see cref="ParlinGroupBuiler" />
	/// </summary>
	public class ParlinGroupBuiler : AlgorithmGroupBuilder<float> {
		public static readonly string IDENTIFIER_INTERPOLATOR = "interpol";

		public static readonly string IDENTIFIER_PERLIN = "perlin";

		public bool BottomUp { get; set; } = true;

		public float DeltaFactor { get; set; } = 0.5f;

		public float InitialScale { get; set; } = 1;

		public int MaxLayers { get; set; } = 16;

		public float MaxPerlinScale { get; set; } = 128;

		public float RetFactor { get; set; } = 1.375f;

		public float ScaleStep { get; set; } = 2;

		public float TargetScale { get; set; } = 1024;

		public string Vec2FieldID { get; set; }

		public override Dictionary<string, Algorithm> BuildGeneric(LandscapeBuilder.LandscapeItermidiate itermidiate)
		{
			Dictionary<string, Algorithm> ret = new Dictionary<string, Algorithm>();
			List<Algorithm<float>> sources = new List<Algorithm<float>>();
			Algorithm<Vec2> vec2Source = itermidiate.Get<Vec2>(Vec2FieldID);
			if ( BottomUp ) {
				for ( float f = InitialScale ; f <= TargetScale && sources.Count < MaxLayers ; f *= ScaleStep ) {
					int scale = RoundTI(f);
					int interpolatorScacle = 1;
					while ( scale / interpolatorScacle > MaxPerlinScale ) {
						interpolatorScacle *= 2;
					}
					int perlinScale = scale / interpolatorScacle;
					if ( interpolatorScacle == 1 ) {
						string ref_key = IDENTIFIER_PERLIN + sources.Count;
						var perlin = new PerlinNoise(vec2Source, perlinScale);
						ret.Add(ref_key, perlin);
						sources.Insert(0, perlin);
					} else {
						string ref_key_perlin = IDENTIFIER_PERLIN + sources.Count;
						string ref_key_interpol = IDENTIFIER_INTERPOLATOR + sources.Count;
						var perlin = new PerlinNoise(vec2Source, perlinScale);
						var interpol = new Interpolator(perlin, interpolatorScacle);
						ret.Add(ref_key_perlin, perlin);
						ret.Add(ref_key_interpol, interpol);
						sources.Insert(0, interpol);
					}
				}
				ret.Add(LandscapeBuilder.MAIN_ALGORITHM_KEY, new FloatAdder(sources, DeltaFactor, RetFactor));
				return ret;
			} else {
				for ( float f = TargetScale ; f > InitialScale && sources.Count < MaxLayers ; f /= ScaleStep ) {
					int scale = RoundTI(f);
					int interpolatorScacle = 1;
					while ( scale / interpolatorScacle > MaxPerlinScale ) {
						interpolatorScacle *= 2;
					}
					int perlinScale = scale / interpolatorScacle;
					if ( interpolatorScacle == 1 ) {
						string ref_key = IDENTIFIER_PERLIN + sources.Count;
						var perlin = new PerlinNoise(vec2Source, perlinScale);
						ret.Add(ref_key, perlin);
						sources.Add(perlin);
					} else {
						string ref_key_perlin = IDENTIFIER_PERLIN + sources.Count;
						string ref_key_interpol = IDENTIFIER_INTERPOLATOR + sources.Count;
						var perlin = new PerlinNoise(vec2Source, perlinScale);
						var interpol = new Interpolator(perlin, interpolatorScacle);
						ret.Add(ref_key_perlin, perlin);
						ret.Add(ref_key_interpol, interpol);
						sources.Add(interpol);
					}
				}
				ret.Add(LandscapeBuilder.MAIN_ALGORITHM_KEY, new FloatAdder(sources, DeltaFactor, RetFactor));
				return ret;
			}
		}

		public override Type GetGenericType(string key)
		{
			return typeof(float);
		}

		public override bool IsValid(LandscapeBuilder landscapeBuilder)
		{
			return base.IsValid(landscapeBuilder);
		}

		public override string ValidityMessage(LandscapeBuilder landscapeBuilder)
		{
			return base.ValidityMessage(landscapeBuilder);
		}

		public override List<string> ValidityMessages(LandscapeBuilder landscapeBuilder)
		{
			return base.ValidityMessages(landscapeBuilder);
		}

		//TODO: validation
		private int RoundTI(in float f)
		{
			return (int) Math.Round(f);
		}
	}
}
