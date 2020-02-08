using System;
using System.Collections.Generic;
using System.Text;

namespace ITG_Core {
	public class PerlinNoiseBuilder : InterpolatableAlgorithmBuilder<float, Vec2> {

		public override Algorithm<float> Build(LandscapeBuilder.LandscapeItermidiate itermidiate)
		{
			base.Build(itermidiate);
			return new PerlinNoise(itermidiate.Get<Vec2>(SourceID), Scale);
		}
	}
}
