using System;
using System.Collections.Generic;
using System.Text;

namespace ITG_Core {
	public class FloatShifterBuilder : LayerBuilder<float, float> {
		public float Factor { get; set; } = 1;
		public override Algorithm<float> Build(LandscapeBuilder.LandscapeIntermidiate intermidiate)
		{
			return new FloatShifter(Offset, intermidiate.Get<float>(SourceID), Factor);
		}
	}
}
