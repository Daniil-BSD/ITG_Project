﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ITG_Core {
	public class InterpolatorBuilder : InterpolatableAlgorithmBuilder<float, float> {

		public override Algorithm<float> Build(LandscapeBuilder.LandscapeItermidiate itermidiate)
		{
			base.Build(itermidiate);
			return new Interpolator(itermidiate.Get<float>(SourceID), Scale);
		}
	}
}
